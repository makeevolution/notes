course: https://app.pluralsight.com/library/courses/kubernetes-installation-configuration-fundamentals/table-of-contents

 Book used for notes: Kubernetes in Action
 
 Course: https://udemy.com/course/dive-into-cloud-native-containers-kubernetes-and-the-kcna/learn/lecture/42070196#overview

 How to set up your own cluster from scratch? https://1drv.ms/f/s!ApGS4APr_VQLpu8-A4kTgICxMlzEng?e=Bou0Dj
 
 ---------------
 

# Job and CronJob objects

 ------------------

 ### Volumes

 Here’s a list of several of the available volume types:

- emptyDir—A simple empty directory used for storing transient data, and sharing data between containers in the pod. These are stored in `/var/lib/kubelet/pods/<pod_uid>/volumes/kubernetes.io-empty-dir/<volume_name>`

example: 
```
apiVersion: v1
kind: Pod
metadata:
  name: fortune
spec:
  containers:
  - image: luksa/fortune                   ❶
    name: html-generator                   ❶
    volumeMounts:                          ❷
    - name: html                           ❷
      mountPath: /var/htdocs               ❷
  - image: nginx:alpine                    ❸
    name: web-server                       ❸
    volumeMounts:                          ❹
    - name: html                           ❹
      mountPath: /usr/share/nginx/html     ❹
      readOnly: true                       ❹
    ports:
    - containerPort: 80
      protocol: TCP
  volumes:                                 ❺
  - name: html                             ❺
    emptyDir: {}
```
will create `/var/lib/kubelet/pods/<pod_uid>/volumes/kubernetes.io-empty-dir/html` folder in the node, and the contents of this folder will be available in `/var/htdocs` of `html-generator` and `/usr/share/nginx/html` of `web-server`. Note that if the containers contain data in their respective folders, those will be wiped out (Remember API Libs issue with AT that we solved!)

- hostPath—Used for mounting directories from the worker node’s filesystem into the pod.

- gitRepo—A volume initialized by checking out the contents of a Git repository.

- nfs—An NFS share mounted into the pod.

- gcePersistentDisk (Google Compute Engine Persistent Disk), awsElasticBlockStore (Amazon Web Services Elastic Block Store Volume), azureDisk (Microsoft Azure Disk Volume)—Used for mounting cloud provider-specific storage.

- cinder, cephfs, iscsi, flocker, glusterfs, quobyte, rbd, flexVolume, vsphere-Volume, photonPersistentDisk, scaleIO—Used for mounting other types of network storage.

- configMap, secret, downwardAPI—Special types of volumes used to expose certain Kubernetes resources and cluster information to the pod.

- persistentVolumeClaim—A way to use a pre- or dynamically provisioned persistent storage.

 To enable apps to request storage in a Kubernetes cluster without having to deal with infrastructure specifics, two new resources were introduced. They are Persistent-Volumes and PersistentVolumeClaims. The names may be a bit misleading, because even regular Kubernetes volumes can be used to store persistent data. The process is as shown:
 ![Alt text](k8s/pics/image-1.png)

 So the admin creates a persistent disk first (point 1. in above picture):
 ```
 $ gcloud container clusters list
NAME   ZONE            MASTER_VERSION  MASTER_IP       ...
kubia  europe-west1-b  1.2.5           104.155.84.137  ...

$ gcloud compute disks create --size=1GiB --zone=europe-west1-b mongodb
WARNING: You have selected a disk size of under [200GB]. This may result in
     poor I/O performance. For more information, see:
     https://developers.google.com/compute/docs/disks#pdperformance.
Created [https://www.googleapis.com/compute/v1/projects/rapid-pivot-
     136513/zones/europe-west1-b/disks/mongodb].
NAME     ZONE            SIZE_GB  TYPE         STATUS
mongodb  europe-west1-b  1        pd-standard  READY
 ```
 And then we make a `PersistentVolume` that refers to this disk:
 ```
 apiVersion: v1
kind: PersistentVolume
metadata:
  name: mongodb-pv
spec:
  capacity:                                 ❶
    storage: 1Gi                            ❶
  accessModes:                              ❷
  - ReadWriteOnce                           ❷
  - ReadOnlyMany                            ❷
  persistentVolumeReclaimPolicy: Retain     ❸
  gcePersistentDisk:                        ❹
    pdName: mongodb                         ❹
    fsType: ext4 
 ```

 And we can claim this volume using `PersistentVolumeClaim`

 ```
 apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: mongodb-pvc              ❶
spec:
  resources:
    requests:                    ❷
      storage: 1Gi               ❷
  accessModes:                   ❸
  - ReadWriteOnce                ❸
  storageClassName: ""           ❹
 ```

 In the above we are not specifically saying we are looking for `PersistentVolume` with name `mongodb-pv`; we are just asking for a random persistent volume that can cater our request. 
 and use it in our Pod e.g.
 ```
 apiVersion: v1
kind: Pod
metadata:
  name: mongodb
spec:
  containers:
  - image: mongo
    name: mongodb
    volumeMounts:
    - name: mongodb-data
      mountPath: /data/db
    ports:
    - containerPort: 27017
      protocol: TCP
  volumes:
  - name: mongodb-data
    persistentVolumeClaim:          ❶
      claimName: mongodb-pvc        ❶
 ```
 PersistentVolumes, like cluster Nodes, don’t belong to any namespace, unlike pods and PersistentVolumeClaims.
 
 ![Alt text](k8s/pics/image.png)

 Instead of creating `PersistentVolume` objects, alternatively, we can deploy a PersistentVolume provisioner and define one or more StorageClass objects to let users choose what type of PersistentVolume they want. The users can refer to the StorageClass in their PersistentVolumeClaims and the provisioner will take that into account when provisioning the persistent storage.

 Assuming a provisioner is available (not gonna go through how to set it up here), we can create a StorageClass object:

 ```
 apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: fast
provisioner: kubernetes.io/gce-pd #usually provisioned by azure or AWS
parameters:
  type: pd-ssd                           ❷
  zone: europe-west1-b                   ❷
 ```

and our `PersistentVolumeClaim` then becomes:
```
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: mongodb-pvc
spec:
  resources:
    requests:
      storage: 100Mi
  accessModes:
    - ReadWriteOnce
  storageClassName: fast             ❶
```

In a diagram:
![Alt text](image-2.png)
Thus we see that the difference is that the PersistentVolume is created dynamically by the provisioner, rather than the Admin having to create each PersistentVolume and having to deal with specifying which PersistentDisk to use.

- A pvc needs to be ReadWriteMany if possible, otherwise pods claiming it cannot respawn on a different node if the node it is running on initially is being serviced!
------------------

# Secrets
We use this to store sensitive info that our app need.

This info will be exposed as an env var in our container, where our app will access it from.

So for example, create a secret (for example) `kubectl create secret generic colour-secret --from-literal=COLOUR=red --from-literal=KEY=value`

and access the secret in your deployment like
```
apiVersion: v1
kind: Pod
metadata:
  creationTimestamp: null
  labels:
    run: ubuntu
  name: ubuntu
spec:
  containers:
  - command:
    - bash
    - -c
    - env; sleep infinity
    image: ubuntu
    name: ubuntu
    resources: {}
    envFrom:
    - secretRef:
        name: colour-secret
  dnsPolicy: ClusterFirst
  restartPolicy: Never
```

NOTE: The secret is by default base64 encoded. So in your app/command you need to manual decode it. For example, in bash, you can do `echo ${colour-secret} | base64 -d`

Secrets are saved in etcd (if you use that as backing storage in your cluster)

## Secrets and security management

We can show the yaml of the secret created above as shown:
```
root@control-plane:~# kubectl get secret colour-secret -o yaml

apiVersion: v1
data:
  COLOUR: cmVk
  KEY: dmFsdWU=
kind: Secret
metadata:
  creationTimestamp: "2024-08-31T17:51:48Z"
  name: colour-secret
  namespace: default
  resourceVersion: "555"
  uid: 4f4ba275-ac2e-481c-9e20-e1864e581dfa
type: Opaque
```

`cmVk` is base64 `red` and `dmFsdWU=` is base64 for `VALUE`.

Thus if you create secrets as YAML, store the value as the encoded version.

In VFM, we store secrets (e.g. token, passwords) as a yaml file in our Helm charts and it is commited and thus visible in git history, or even in some `config.yaml` or `enums.py`. This is wrong! Although the secret is encoded, the person who gains access to our git can see the yaml, and thus can decode by trying against all existing encoding algorithms in this universe! 

To make it more secure, we should have:
- Create the secret through CLI
- Set the secret name in `values.yaml` of our Helm chart
- When we want to update the secret, do it again through CLI, and update the `values.yaml`
- How do we then track history of secret updates if not through git? Use `Auditing` feature of k8s https://blog.kubesimplify.com/the-ultimate-guide-to-audit-logging-in-kubernetes-from-setup-to-analysis
- We can store this auditing yaml in our helm chart (since we want to probably audit secrets in a specific namespace) for backup/if we want to change what we want to audit.
- Or even the cluster manager (if we use cloud) already implement at a cluster level this as part of their service (so all secrets in all namespaces are audited), and so we don't have to do anything!!

Therefore, in this way our encoded secrets are located only in one location (k8s) and not also in git, reducing risk of bocor.

# RBAC (https://www.youtube.com/watch?v=jvhKOAyD8S8)

### RBAC for humans
- K8S uses certificates to authorize users access to the cluster.
- The flow is:
    - When you provision a k8s cluster, a `ca.crt` and `ca.key` file is created under (for Kind clusters) `/etc/kubernetes/pki` in the node(s)
    - We use this file to sign user certificates. We can create a certificate for a user named Bob by:
      - Running `openssl genrsa -out bob.key 2048`. This will generate a private key file called `bob.key`
    - Then, we need the create a Certificate Signing Request (CSR) by running `openssl req -new -key bob.key -out bob.csr -subj "/CN=Bob Smith/O=Shopping"`; where CN is Common Name and O is Organization `https://knowledge.digicert.com/general-information/what-is-a-distinguished-name`; CN can be your user name and O is the namespace this person is allowed to access (e.g. like the scope in OAuth)
    - Then we can generate a certificate by `signing` this `request`: `openssl x509 -req -in bob.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out bob.crt -days 1`
    - (So flow is `create private key ->  create certificate signing request from key -> sign the certificate signing request using ca.key and ca.crt -> you get a certificate`)
    - Then, we use this certificate to create the .kubeconfig file for access to the cluster. This kubeconfig is what we give Bob.
- What does a kubeconfig file contain?
    - It contains 4 sections: `clusters`, `users`, `contexts`, and `current-context`
      - `clusters` section contain info on the cluster URL API server and the certificate-authority-data of the cluster. The certificate-authority-data is a CA certificate that `kubectl` can later use when connecting to the cluster URL aforementioned. When the kubectl connects to the API server, it expects the server to return a certificate. `kubectl` will compare this returned cert with the value in `certificate-authority-data` to ensure the URL is legit and prevent man in the middle attack.
        - Create the section in the kubeconfig using: `kubectl config set-cluster dev-cluster --server=https://127.0.0.1:52807 --certificate-authority=ca.crt --embed-certs=true `
        - This will create a section like:
          ```
          apiVersion: v1
          clusters:
          - cluster:
              certificate-authority-data: ASDFWETAHTHAWHTWHHRTAWHERHAW_I_AM_SOME_LONG_STRING
              server: https://my-cluster-api:someport
            name: dev-cluster
          ```
      - The `users` section contain a name field, as well as the user's certificate data and key we generated for the user. The user in the cluster is not in the name; it is the data inside this certificate
       - Generate using `kubectl config set-credentials bob --client-certificate=bob.crt --client-key=bob.key --embed-certs=true`. This will create a section in the kubeconfig like:
         ```
         - users:
             name: bob
             user:
               client-certificate-data: SOMELONGSTRING
               client-key-data: SOMELONGSTRING
         ```
       - In AsML, we used token instead, which creates section
         ```
         user:
           token: some JWT token
         ```
      - The `context` BINDS a user to a cluster
        - Create section in the kubeconfig using `kubectl config set-context dev-cluster --cluster=cluster1 --namespace=shopping --user=bob`
        - In this above example, it will bind the user `bob` to the cluster `dev-cluster`. The `namespace` argument makes it such that if bob does kubectl commands, he will by default do it against the namespace `shopping`, unless he specifies `-n` switch.
        - Result:
          ```
          contexts:
            - context:
                cluster: dev-cluster
                namespace: shopping
                user: bob
          ```
      - The `current-context` is the context that the user are going to connect to when they run `kubectl`. To set this, you (or the user) can run:
        - `kubectl config use-context cluster1`
      - You can indeed create more user in `users` and context in `contexts` for defining multiple users and contexts, and bind them appropriately.
      - So, a full .kubeconfig file for bob will look like this
      ```
      apiVersion: v1
      kind: Config
      clusters:
      - cluster:
           certificate-authority-data: ASDFWETAHTHAWHTWHHRTAWHERHAW_I_AM_SOME_LONG_STRING
           server: https://my-cluster-api:someport       
        name: dev-cluster
      - users:
        - name: bob
          user:
            client-certificate: SOMELONGSTRING
            client-key-data: SOMELONGSTRING
      - contexts:
        - context:
            cluster: dev-cluster
            namespace: shopping
            user: bob
      - current-context: dev-cluster
      ```
  - Now, let's say some other person installed this YAML in our cluster without us knowing:
  ```
  apiVersion: rbac.authorization.k8s.io/v1
  kind: Role
  metadata:
   namespace: shopping
   name: somenameforthisrole
  rules:
  - apiGroups: [""]
    resources: ["pods", "pods/exec"]
    verbs: ["get", "watch", "list", "create", "delete"]
  - apiGroups: ["apps"]
    resources: ["deployments"]
    verbs: ["get", "watch", "list", "delete", "create"]
  ```
  - If now bob runs the `kubectl get pods`, he will get `Error from server (Forbidden): pods is forbidden: User `Bob Smith` cannot list resource "pods" in API group "" in the namespace "shopping"` But we gave him the kubeconfig above????
  - This is because in the above we gave **ACCESS** to Bob, but we still haven't given him **PERMISSION** to access resources.
  - The guy above installed something called a `Role`. To give **PERMISSION**, we need to give bob this `Role`.
  - The critical sections of the `Role` YAML explained:
    - What the hell is `apiGroups` and `resources`??? This confuses me a lot when I was at asml!!!
      - So k8s, out of the box, assigns k8s objects to apiGroups in different versions of its API. For example for API v1, for different k8s objects, you will always see these section at the top:
        For deployment:
        ```
        ---
        apiVersion: apps/v1
        kind: Deployment
        ```
        For statefulset:
        ```
        ---
        apiVersion: apps/v1
        kind: StatefulSet
        ```
        For Pods:
        ```
        apiVersion: v1
        kind: Pod
        ```
        You see how Deployment and statefulset is in apiGroup apps, and pod is in apiGroup "" (nothing before the version). This "pairing" is what you supply to the apiGroups and resources in the rules section of your role. For example, if you want to set rule for deployment and statefulset, you will need to set as (as also shown in example above, but in the above it only shows for deployments):
        ```
        - apiGroups: ["apps"]
          resources: ["deployments", "statefulsets]
          verbs: ["get", "watch", "list", "delete", "create"]
        ```
        So this means in V1 of the API, k8s puts `pods` in group `""` and `deployments` and `statefulsets` in `apps` group. When I say group here it refers to some internal group within its systems (idontcare what it really is, just that its like that). So it's like a **boilerplate** or **ceremony** that you need to do to use this feature.
     - The verbs is simply what `kubectl` commands you can do to the resources within the namespace.
  - lets bind the `Role` above to a `RoleBinding` k8s object, which will actually give the permission. Example:
    ```
    apiVersion: rbac.authorization.k8s.io/v1
    kind: RoleBinding
    metadata:
      name: manage-pods
      namespace: shopping
    subjects:
    - kind: User  // NOTE THE KIND HEREEEEEE!!!!!!!!! NOTE THIS WILL BE DIFFERENT FOR SERVICE ACCOUNTS (LATER BELOW)
      name: "Bob Smith"  // THIS IS THE NAME EMBEDDED IN YOUR CA CERTIFICATE I.E. THE CN ENTRY WE SET IN THE CERTIFICATE ABOVE!!
      apiGroup: rbac.authorization.k8s.io  // Like I explained above, this is the boilerplate/ceremony; for RoleBinding and Roles, they are inside apiGroup rbac.authorization.k8s.io so need to specify
    roleRef:
      kind: Role
      name: somenameforthisrole
      apiGroup: rbac.authorization.k8s.io
    ```
    Note that in the metadata you specify namespace. If you don't specify namespace, the role and rolebinding will be applied to default namespace!
- Finally, Bob can run `kubectl get pods` successfully; it will list all pods in the `shopping` namespace (note this is because we set the `namespace` to `shopping` in the .kubeconfig context binding, see above)

### ServiceAccount (RBAC for apps)
- Usually, `kubectl` stuff is done by humans through CLI. But what if your pod/your k8s objects needs to `kubectl` stuff against the k8s API (usually it will do curl to the kubernetes API of the cluster instead of bare `kubectl`, example command: `curl --cacert ${CACERT} --header "Authorization: Bearer $TOKEN" -s ${APISERVER}/api/v1/namespaces/shopping/pods/`)? How do we manage permissions for it?
- A service account is an identity a K8S object can use to do the `kubectl` stuff against the cluster.
- How does it work? We first create a k8s object called ServiceAccount (we meaning Bob) and a role for it:
  ```
  apiVersion: v1
  kind: ServiceAccount
  metadata:
    name: shopping-api
  ```
  
  ```
  apiVersion: rbac.authorization.k8s.io/v1
  kind: Role
  metadata:
    namespace: shopping
    name: shoppingroleforpod
  rules:  // Make the rules more restrictive than Bob's just for fun
  - apiGroups: [""]
    resources: ["pods"]
    verbs: ["get", "watch", "list"]
  ```
  We can then assign the ServiceAccount to a k8s object that we are going to deploy e.g. for a pod:
  ```
  apiVersion: v1
  kind: Pod
  metadata:
    name: shopping-api
  spec:
    containers:
    - image: nginx
      name: shopping-api
    serviceAccountName: shopping-api  // THIS IS THE NAME OF THE ROLE WE DEFINED ABOVE
  ```
  The pod still won't have any access to do `kubectl` stuff from inside of it, since we haven't bind it yet to a RoleBinding.
  Nugget: to confirm that it doesn't have access, follow the relevant section in `https://github.com/marcel-dempers/docker-development-youtube-series/blob/master/kubernetes/rbac/README.md#kubernetes-service-accounts`. Notice: Bob can do `kubectl exec` to the pod we created above since we have `pods/exec` in `resources` section of Role object for Bob!
  - Then we need then to assign a rolebinding to the serviceaccount's role:
    ```
    apiVersion: rbac.authorization.k8s.io/v1
    kind: RoleBinding
    metadata:
      name: shopping-api
      namespace: shopping
    subjects:
    - kind: ServiceAccount  // NOTE THE KIND HERE!!!!!!!!
      name: shopping-api
    roleRef:
      kind: Role
      name: shoppingroleforpod
      apiGroup: rbac.authorization.k8s.io
    ```
    Also notice! ServiceAccount is in apiGroup "", and this (somehow) means we dont specify it in the `subjects` section above.
  - The pod finally then has also access to do `kubectl` stuff, with permissions defined in the 
------------------

# Services and Ingresses, more info https://cloud.google.com/kubernetes-engine/docs/concepts/service
- There are 5 types of services objects:
  - ClusterIP (default)
    - Headless (ClusterIP with IP: None)
  - NodePort
  - LoadBalancer
  - ExternalName

- Difference in client behavior:
  - ClusterIP (default): Internal clients send requests to a stable internal IP address.
  - NodePort: Clients send requests to the IP address of a node on one or more nodePort values that are specified by the Service.
  - LoadBalancer: Clients send requests to the IP address of a network load balancer.

### ClusterIP
Exposes the service on an internal IP within the cluster. This IP is only accessible within the cluster, mainly used for internal communication between services.

#### Headless service
Headless Service is a special type of ClusterIP:
```
  ClusterIP:
      Headless Service: The clusterIP field is set to None. This means the service does not get a single, stable IP address.
  Service Discovery:
      Headless Service: Each pod behind the service gets its own DNS A record. When you query the DNS for the service, you get a list of IP addresses corresponding to the individual pods. This allows clients to connect directly to each pod.
  Use Case:
      Headless Service: Suitable for stateful applications where each pod needs to be addressed individually, such as databases (e.g., Cassandra, MongoDB) or applications requiring custom load balancing logic. 
      For example, a RabbitMQ cluster, see here for more info: https://github.com/makeevolution/messaging/blob/9e0f8425c5b46d5caec3088daa86679d9d3d67c1/rabbitmq/kubernetes/rabbit-statefulset.yaml#L1
  DNS Records:
      Headless Service: DNS queries return multiple A records, one for each pod. For example, my-app-0.my-headless-service.default.svc.cluster.local, my-app-1.my-headless-service.default.svc.cluster.local, etc.
  Load Balancing:
      Headless Service: Clients are responsible for implementing their own load balancing or connection logic.
```
```
apiVersion: v1
kind: Service
metadata:
  name: my-headless-service
spec:
  clusterIP: None  # This makes it none
  selector:
    app: my-app
  ports:
  - port: 80
    targetPort: 8080
```
### NodePort vs. LoadBalancer vs. Ingress (https://medium.com/google-cloud/kubernetes-nodeport-vs-loadbalancer-vs-ingress-when-should-i-use-what-922f010849e0, demo https://www.youtube.com/watch?v=V0uKqYXJRF4)
ClusterIP only allows communication inside the cluster. To allow communication from outside the cluster, there are 3 main ways:

#### NodePort
`NodePort`: Exposes the service on each node's IP at a static port (the NodePort). A NodePort service maps a port on all nodes in the cluster to the service, making it accessible from outside the cluster using `<AnyOfTheNodesIP>:<NodePort>`. This means on each node you need to open the firewall rules to allow traffic to that particular port you expose.
![alt text](image-4.png)
```
apiVersion: v1
kind: Service
metadata:
  name: my-np-service
spec:
  type: NodePort
  selector:
    app: metrics
    department: engineering
  ports:
  - protocol: TCP
    port: 80
    targetPort: 30000
```
How does the service (in the picture above) then decide which pod to route the request to?

The process of deciding which pod receives the request is handled by Kubernetes’ internal load balancing mechanism. Kubernetes uses Round Robin (or similar algorithms) to distribute incoming traffic across all the available pods. The kube-proxy component running on each node is responsible for handling the load balancing. It watches for changes in services and endpoints and configures iptables (or IPVS) rules accordingly.

There are many downsides to using NodePort:
  - You can only have one service per port
  - You can only use ports 30000–32767
  - If your Node/VM IP address change, you need to deal with that

#### LoadBalancer
`LoadBalancer` object: Instead of contacting the Node IPs directly, we can setup something called a load balancer manually, which has a single IP address. We then configure our `service` object to be of type `LoadBalancer`. This load balancer is external to the Kubernetes cluster and is responsible for efficiently distributing incoming traffic across the nodes that are running the service’s pods. 

![alt text](image-5.png)

```
apiVersion: v1
kind: Service
metadata:
  name: my-lb-service
spec:
  type: LoadBalancer
  selector:
    app: products
    department: sales
  ports:
  - protocol: TCP
    port: 60000
    targetPort: 50001
```

If you run in the cloud, this external load balancer is provisioned automatically when you create this service object with type LoadBalancer, see `https://cloud.google.com/kubernetes-engine/docs/how-to/exposing-apps#creating_a_service_of_type_loadbalancer`. If you run on premise, you can set up and configure your own load balancer such as `metallb`, see `https://metallb.universe.tf/` and `https://www.youtube.com/watch?v=Yl8JKffmhuE&t=364s`

Note: Do not confuse this with k8s' load balancing! So there are two load balancers here: one is the external load balancer you (or your cloud provider) has set up, that will balance the request load across the nodes. Once the request hits a node, there is another load balancer (k8s' load balancer) that will balance the load across the pods!

### Ingress
Now, both `NodePort` and `LoadBalancer` above only exposes one service. If we have more services:
- For `NodePort` we need to open more ports in each node for each new service
- For `LoadBalancer` we need to have a new load balancer for each new service e.g.

![alt text](image-6.png)

Both approaches can get very expensive and time consuming. Is there a way we can just use one entrypoint and make multiple services accessible through it?

Yes, and that is called `ingress`.

![alt text](image-7.png)

So now we only have one load balancer and it can balance load across multiple services, cheaper.

The ingress controller is its own k8s workload, a namespace within the k8s cluster. Example of setting up https://spacelift.io/blog/kubernetes-ingress#setting-up-ingress-with-nginx--step-by-step

![alt text](image-8.png)
![alt text](image-10.png)

Thus the external load balancer will connect to the controller's `LoadBalancer` service (so as usual). Clients contact the external-IP in the above to communicate.

Note: the key difference with `LoadBalancer` (and `NodePort` or even `ClusterIP`) here is that we then don't use k8s' internal load balancing to balance load across the pods, but rather we use the Ingress controller's, which is more advanced (see the section NGINX and kube proxy below)

### In the context of GKE
In Google Kubernetes Engine (GKE):
- When you create a service of type `LoadBalancer`, they will auto provision you an external load balancer of type `external passthrough load balancer` `https://cloud.google.com/load-balancing/docs/network/networklb-backend-service`, so you can access from internet
- When you create an Ingress, they will auto provision an external load balancer of type `external application load balancer` `https://cloud.google.com/load-balancing/docs/https`, so you can access from internet

### Static IP
If the load balancer is restarted, the IP address may change! Thus, we need to configure a static IP for the load balancer! In GKE, you can do this like this https://cloud.google.com/kubernetes-engine/docs/tutorials/configuring-domain-name-static-ip

To associate your IP address with a domain name, see website_administration.md under linux folder for more information.

### NGINX, and kubeproxy, how it works

- kube-proxy is a key component of any Kubernetes deployment.  Its role is to load-balance traffic that is destined for services (via cluster IPs and NodePort) to the correct backend pods.

- Kube-proxy can run in one of three modes, each implemented with different data plane technologies: userspace, iptables, or IPVS.

- The service object load balances requests to the pods in its selector based on the load balancing setting of the chosen dataplane. The IPVS has much more load balancing options. Iptables only has random way of selection (it selects a random pod to serve the request)

- NOTE: If you use an ingress object in front of your service, which in your case is of type NGINX, NGINX has and implements its own load balancing skills (see `ingress` section above). By default, the NGINX bypasses the services object and targets directly the pods underneath (see `ingress` section above). Thus the pods benefit directly from NGINX's more advanced load balancing capabilities (rather than the Service's iptables random load balancing strategy). However, the NGINX does not consult the iptables for the available pods. Thus if a pod is terminating even after its ip address has been removed from the iptable, that pod can still be requested by NGINX and thus users can still see `connection refused`. This would not happen with Services; it always checks with the iptable first.

- To make NGINX not bypass Service, set this annotation https://github.com/kubernetes/ingress-nginx/issues/257#issuecomment-335835670. But this means your pods are not load balanced by NGINX anymore and you have to set IPVS rules on your service object for more advanced load balancing rules.

- More discussions and info here https://www.reddit.com/r/kubernetes/comments/161xrdb/am_i_load_balancing_correctly/ https://kubernetes.io/docs/reference/networking/virtual-ips/ 

---------------------------

# Network Policies
- By default, if we have a service object, the pods are accessible from anywhere inside the cluster
- Sometimes we don't want this. We can create a `NetworkPolicy` object to restrict this. 
  - Example: This YAML
  ```
  apiVersion: networking.k8s.io/v1
  kind: NetworkPolicy
  metadata:
    name: allow-nginx-access
    namespace: default
  spec:
    podSelector:
      matchLabels:
        run: nginx
    policyTypes:
      - Ingress
    ingress:
      - from:
        - podSelector:
            matchLabels:
              run: curl
    ```
    will only allow traffic to the pod `run: nginx` from pods with label `run: curl`

----------------------------

# Security
- There are two important aspects of security for k8s:
  - `SecurityContext`
  - `Admission Controller`
- `SecurityContext`
  - I think you struggled with this a lot already with Kubewarden back in ASLM :) but for refresher
  ```
  apiVersion: v1
  kind: Pod
  metadata:
    creationTimestamp: null
    labels:
      run: ubuntu
    name: ubuntu
  spec:
    securityContext:
      runAsUser: 1000
      runAsGroup: 1000
    containers:
    - args:
      - sleep
      - infinity
      image: spurin/rootshell:latest
      name: ubuntu
      resources: {}
    dnsPolicy: ClusterFirst
    restartPolicy: Always
  status: {}
  ```
  If you run this using `kubectl exec -it ubuntu --bash`, you will see you are non root (run the command `id`)
  - However, in the above, if we run the command `/rootshell`, we can still execute commands that require root access!
  - To further restrict this, use `AllowPrivilegeEscalation`
  ```
  apiVersion: v1
  kind: Pod
  metadata:
    creationTimestamp: null
    labels:
      run: ubuntu
    name: ubuntu
  spec:
    securityContext:
      runAsUser: 1000
      runAsGroup: 1000
    containers:
    - args:
      - sleep
      - infinity
      image: spurin/rootshell:latest
      name: ubuntu
      resources: {}
      securityContext:
        allowPrivilegeEscalation: false
    dnsPolicy: ClusterFirst
    restartPolicy: Always
  status: {}
  ```
- This is good for a pod to pod basis, but what if you want to restrict this for cluster level?
  - Before k8s 1.25, we use a `PodSecurityPolicy` object.
  - But since 1.25, we use `AdmissionControllers`
- `AdmissionControllers`
  - Act as gatekeepers controlling what pods can and cannot do
  - Intercepts and processes requests to the Kubernetes API prior to persistence of the object, but after the request is authenticated and authorized.
  - When you make a request to create a pod, before your `yaml` is saved in `etcd` it goes through middelwares as shown below (lifecycle of a k8s api request):
  ![](lifecycleapik8s.png)
  - An `Admission Controller` can either be `static` i.e. comes out of the box by k8s, or `dynamic`, i.e. you implement it yourself through `admission webhooks`
  - As you can see above, there are two types of `Admission Controllers`: `Mutating Admission` and `Validating Admission`. 
- `Mutating Admission` controller:
  - It changes your `yaml` and adds stuff to it
  - An example of a `static` `mutating admission` controller is the `AlwaysPullImages`; if you don't specify this in your pod spec, it gets added by this controller!
  - An example of a `dynamic` `mutating admission` controller is a controller that automatically adds a label e.g. `cluster: mycluster` to a pod, or injecting a sidecar container into incoming pod e.g. for service mesh
- `Validating Admission` controller:
  - It validates your `yaml` (now already turned into JSON as shown in diagram above) and rejects it if it has some illegal stuff in it
  - An example of a `static` `dynamic admission` controller is the `PodSecurity` controller, which checks if the JSON has the correct security contexts. This comes out of the box (thus `static`) but has to be turned on manually.
  - You need to edit the `kube-apiserver.yaml` and then the server needs to restart; since the API server is a static pod, it restarts automatically
- Example plugins tailored for security in the wild include `kyverno`, `open policy agent`, `kubewarden`, etc.
  - They enforce securities on different levels, differences below
   (add pic of differences here)
# Get logs of applications using Loki and display in Grafana

Followed the following with some modifications https://www.youtube.com/watch?v=Mn2YpMJaEBY&t=1359s

The following is the architecture. Each node has a log collector (`promtail`) that pipes its output to a log aggregator (`loki`) deployed in a node, which will then process the logs and pipe the results to `grafana` for visualization ![Alt text](grafanaloki.jpg)

- Clone the repo `git clone https://github.com/grafana/helm-charts.git`. The commit that was found to work was `316f4a28a`
- Create a new namespace specific for monitoring `kubectl create ns monitoring`
- The charts to use are `grafana`, `loki-distributed`, and `promtail`
- Go to `values.yaml` of `promtail`, and change `config -> client -> url` section to send logs to the following loki url:
```
config:
  clients:
      - url: http://loki-loki-distributed-gateway/loki/api/v1/push
```
- Do `helm upgrade --install promtail charts/promtail/ -n monitoring` to install promtail
- Then install loki `helm upgrade --install loki charts/loki-distributed/ -n monitoring`
- Now for grafana, go to its `values.yaml` and edit datasources section to the following:
```
datasources:
  datasources.yaml:
    apiVersion: 1
    datasources:
    - name: Loki
      type: loki
      url: http://loki-loki-distributed-query-frontend.monitoring:3100
```
- Then `helm upgrade --install grafana charts/grafana -n monitoring`
- To see the dashboard on your own laptop, you can do `kubectl port-forward service/grafana -n monitoring 3000:80`. If you run this on a VPS, port-forward `3000` to `3000` of your local machine. Then visit `localhost:3000`
- Also deploy a sample app that logs some things:
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: kodecloud
spec:
  replicas: 1
  selector:
    matchLabels:
      app: kodecloud
  template:
    metadata:
      labels:
        app: kodecloud
    spec:
      containers:
      - name: your-container-name
        image: kodekloud/loki-demo
```
`kubectl apply -f deployment.yaml`

-------------------------------------

# Helm

- Deleting a statefulset through `helm uninstall` does not remove the PVC associated with it! `https://github.com/helm/helm/issues/5156` It's a bug?

‐------------------------------------

# K8S operators and Custom Resources, what is this thing? What is RabbitMqCluster type in the recommended way to deploy rabbitmq in k8s?

The kind `RabbitmqCluster` is not the operator itself. Instead, it is a custom resource defined by the operator. Here's a clearer breakdown of the terminology and components:

### Components:

1. **Custom Resource Definition (CRD)**:
   - This defines a new type of resource in Kubernetes. For instance, `RabbitmqCluster` is a custom resource defined by the RabbitMQ operator.
   - The CRD is used to extend the Kubernetes API with new custom resources that the operator manages.

2. **Custom Resource (CR)**:
   - This is an instance of a CRD. When you create a `RabbitmqCluster` resource in Kubernetes, you're creating a CR based on the `RabbitmqCluster` CRD.
   - Example:
     ```yaml
     apiVersion: rabbitmq.com/v1beta1
     kind: RabbitmqCluster
     metadata:
       name: my-rabbitmq-cluster
     spec:
       replicas: 3
     ```

3. **Operator**:
   - The operator is a controller that watches for changes to custom resources and ensures that the actual state of the system matches the desired state specified in the custom resources.
   - The operator is responsible for the logic of managing the application, such as deploying and scaling the `RabbitmqCluster` instances.

### How They Work Together:

1. **CRD (Defining the Resource)**:
   - The operator defines one or more CRDs to specify the schema for custom resources it will manage. For RabbitMQ, a CRD might be `RabbitmqCluster`.

2. **CR (Using the Resource)**:
   - You, as a user, create instances of these custom resources (CRs) using the defined CRDs. This involves writing YAML files specifying your desired configuration, such as a `RabbitmqCluster` with a specific number of replicas.

3. **Operator (Managing the Resource)**:
   - The operator watches for changes to these custom resources. When it detects a new `RabbitmqCluster` resource or a change to an existing one, it takes the necessary actions to ensure the actual state matches the desired state (e.g., creating RabbitMQ pods, setting up services, etc.).

### Example:

1. **Define the CRD** (this is done by the operator developer):
   ```yaml
   apiVersion: apiextensions.k8s.io/v1
   kind: CustomResourceDefinition
   metadata:
     name: rabbitmqclusters.rabbitmq.com
   spec:
     group: rabbitmq.com
     versions:
       - name: v1beta1
         served: true
         storage: true
         schema:
           openAPIV3Schema:
             type: object
             properties:
               spec:
                 type: object
                 properties:
                   replicas:
                     type: integer
     scope: Namespaced
     names:
       plural: rabbitmqclusters
       singular: rabbitmqcluster
       kind: RabbitmqCluster
       shortNames:
         - rmq
   ```

2. **Create a CR** (this is done by the user):
   ```yaml
   apiVersion: rabbitmq.com/v1beta1
   kind: RabbitmqCluster
   metadata:
     name: my-rabbitmq-cluster
   spec:
     replicas: 3
   ```

3. **Operator Logic** (implemented by the operator developer):
   - The operator watches for `RabbitmqCluster` resources.
   - When a `RabbitmqCluster` is created or modified, the operator takes action to ensure the RabbitMQ cluster is deployed and configured according to the specification in the `RabbitmqCluster` resource.

#### How do you create an operator? 

To create a RabbitMQ operator using Python and `kopf`, we'll follow a similar process to the previous example. The goal is to create a custom resource (`RabbitmqCluster`) and an operator to manage RabbitMQ instances.

### Step-by-Step Example

#### 1. **Install Required Packages**

First, install `kopf`, `kubernetes`, and other dependencies:

```bash
pip install kopf kubernetes pyyaml
```

#### 2. **Define the Custom Resource Definition (CRD)**

Create a YAML file for the `RabbitmqCluster` CRD:

```yaml
# rabbitmq-crd.yaml
apiVersion: apiextensions.k8s.io/v1
kind: CustomResourceDefinition
metadata:
  name: rabbitmqclusters.rabbitmq.com
spec:
  group: rabbitmq.com
  versions:
    - name: v1
      served: true
      storage: true
      schema:
        openAPIV3Schema:
          type: object
          properties:
            spec:
              type: object
              properties:
                size:
                  type: integer
  scope: Namespaced
  names:
    plural: rabbitmqclusters
    singular: rabbitmqcluster
    kind: RabbitmqCluster
    shortNames:
      - rmq
```

Apply the CRD to your cluster:

```bash
kubectl apply -f rabbitmq-crd.yaml
```

#### 3. **Create the Operator Logic**

Write the operator logic using `kopf` in a Python script (e.g., `rabbitmq_operator.py`):

```python
# rabbitmq_operator.py
import kopf
import kubernetes.client
from kubernetes.client.rest import ApiException

def create_rabbitmq_statefulset(namespace, name, size):
    # Define the StatefulSet template
    statefulset_template = {
        "apiVersion": "apps/v1",
        "kind": "StatefulSet",
        "metadata": {
            "name": name
        },
        "spec": {
            "serviceName": name,
            "replicas": size,
            "selector": {
                "matchLabels": {
                    "app": name
                }
            },
            "template": {
                "metadata": {
                    "labels": {
                        "app": name
                    }
                },
                "spec": {
                    "containers": [{
                        "name": "rabbitmq",
                        "image": "rabbitmq:3-management",
                        "ports": [
                            {"containerPort": 5672, "name": "amqp"},
                            {"containerPort": 15672, "name": "management"}
                        ]
                    }]
                }
            },
            "volumeClaimTemplates": [{
                "metadata": {
                    "name": "rabbitmq-data"
                },
                "spec": {
                    "accessModes": ["ReadWriteOnce"],
                    "resources": {
                        "requests": {
                            "storage": "1Gi"
                        }
                    }
                }
            }]
        }
    }

    # Create the StatefulSet
    api_instance = kubernetes.client.AppsV1Api()
    try:
        api_instance.create_namespaced_stateful_set(namespace, statefulset_template)
    except ApiException as e:
        if e.status != 409:  # Ignore error if the StatefulSet already exists
            raise

@kopf.on.create('rabbitmq.com', 'v1', 'rabbitmqclusters')
def create_fn(spec, name, namespace, **kwargs):
    size = spec.get('size', 1)
    create_rabbitmq_statefulset(namespace, name, size)

@kopf.on.update('rabbitmq.com', 'v1', 'rabbitmqclusters')
def update_fn(spec, name, namespace, **kwargs):
    size = spec.get('size', 1)
    api_instance = kubernetes.client.AppsV1Api()
    statefulset = api_instance.read_namespaced_stateful_set(name, namespace)
    if statefulset.spec.replicas != size:
        statefulset.spec.replicas = size
        api_instance.replace_namespaced_stateful_set(name, namespace, statefulset)

@kopf.on.delete('rabbitmq.com', 'v1', 'rabbitmqclusters')
def delete_fn(name, namespace, **kwargs):
    api_instance = kubernetes.client.AppsV1Api()
    try:
        api_instance.delete_namespaced_stateful_set(name, namespace)
    except ApiException as e:
        if e.status != 404:  # Ignore error if the StatefulSet does not exist
            raise
```

#### 4. ** deploy the Operator in the cluster**
make a dockerfile that will run the operator
```
# Dockerfile
FROM python:3.9-slim

RUN pip install kopf kubernetes pyyaml

COPY rabbitmq_operator.py /rabbitmq_operator.py

ENTRYPOINT ["kopf", "run", "/rabbitmq_operator.py"]
```

deploy to the cluster; it will watch any RabbitmqCluster k8s object created in the cluster and act accordingly
```
# rabbitmq-operator-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rabbitmq-operator
  namespace: default
spec:
  replicas: 1
  selector:
    matchLabels:
      app: rabbitmq-operator
  template:
    metadata:
      labels:
        app: rabbitmq-operator
    spec:
      serviceAccountName: rabbitmq-operator
      containers:
      - name: rabbitmq-operator
        image: <your-username>/rabbitmq-operator:latest
        imagePullPolicy: Always
```

As seen above we have a serviceAccountName so the operator can contact the k8s API to monitor the cluster and watch for any deployment of this new CRD. This means we also need to ask the cluster so the operator can monitor!

```
# rabbitmq-operator-rbac.yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: rabbitmq-operator
  namespace: default
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: rabbitmq-operator
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["create", "update", "patch", "delete", "get", "list", "watch"]
- apiGroups: ["apps"]
  resources: ["statefulsets"]
  verbs: ["create", "update", "patch", "delete", "get", "list", "watch"]
- apiGroups: ["rabbitmq.com"]
  resources: ["rabbitmqclusters"]
  verbs: ["create", "update", "patch", "delete", "get", "list", "watch"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: rabbitmq-operator
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: rabbitmq-operator
subjects:
- kind: ServiceAccount
  name: rabbitmq-operator
  namespace: default
```

Apply
```
kubectl apply -f rabbitmq-operator-rbac.yaml
kubectl apply -f rabbitmq-operator-deployment.yaml
```
#### 5. **Create a Custom Resource**

Define a `RabbitmqCluster` custom resource in a YAML file and apply it to your cluster:

```yaml
# rabbitmq-cr.yaml
apiVersion: rabbitmq.com/v1
kind: RabbitmqCluster
metadata:
  name: example-rabbitmq
  namespace: default
spec:
  size: 3
```

Apply the custom resource:

```bash
kubectl apply -f rabbitmq-cr.yaml
```

### Summary

In this example, you created a simple Kubernetes operator for managing RabbitMQ instances using Python and `kopf`. The operator includes:

- **CRD**: Defines the schema for the custom resource `RabbitmqCluster`.
- **Operator Logic**: Contains the logic to reconcile the desired state of `RabbitmqCluster` instances by creating, updating, and deleting the necessary StatefulSets.
- **Custom Resource**: An instance of `RabbitmqCluster` specifying the desired number of replicas.

This example can be extended with more complex logic to handle various operational tasks specific to RabbitMQ.

In summary, the kind `RabbitmqCluster` is a custom resource defined by the CRD, and the operator is the software that manages the lifecycle of these custom resources. The operator ensures that the actual state of the system matches the desired state specified in the custom resources.
