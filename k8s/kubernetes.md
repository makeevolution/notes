course: https://app.pluralsight.com/library/courses/kubernetes-installation-configuration-fundamentals/table-of-contents
---------------------------------
### Debugging issues
- How to get into a `crashloopbackoff` container: in the container yaml, set this: `command: ["sh", "-c", "while true; do echo hello; sleep 86400; done"]` and thus you can investigate the container's contents (e.g. whoami, ls -la, pwd, etc.) easily!!!
- How to copy file or folder from pod container to host: `kubectl cp -c yourbackendcontainernameinthepod <your-namespace>/<your-backend-pod-name-get-it-from-kubectl-get-pods>:mydesireddatabase.sqlite mydesireddatabase.sqlite`; you may get warning but ignore it, file/folder is copied
- If you wanna talk, from outside your cluster (e.g. VDI terminal when you were at AMLS) to a pod whose port is exposed to a service with name aldo-elasticsearch in your namespace: `kubectl port-forward --namespace aldo-elasticsearch svc/aldo-elasticsearch 9200:9200`
------------------------------------------------------------
### Throwaway pod
If you need a pod that runs forever, copy paste below and kubectl apply it:
```
apiVersion: v1
kind: Secret
metadata:
  name: mysecret
data:
  .dockerconfigjson: someJWTtokentopullyourimagefromyourprivatereg
type: kubernetes.io/dockerconfigjson

---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: test
spec:
  selector:
    matchLabels:
      app: mypod
  replicas: 1
  template:
    metadata:
      labels:
        app: mypod
    spec:
      imagePullSecrets:
      - name: mysecret  # only relevant really if you pull from private regs
      containers:
        - name: kubectlexecintome
          image: busybox  # or some other image you wanna get into in k8s
          imagePullPolicy: Always
          command:                                                                                                                                 
            - "/bin/sh"                                                                                                                               
            - "-c"                                                                                                                                     
            - "tail -f /dev/null"
```
------------------------------------------------------------
# commands
Get all objects currently registered: kubectl get all

Context related
- kubectl config use-context
-	kubectl config get-contexts
-	kubectl config current-context
-	kubectl config set-context
-	kubectl config delete-context
-	kubectl config view
-	kubectl config use-context <context-name> --namespace=<namespace>
 
Namespace related
-	kubectl get namespaces
-	kubectl create namespace
-	kubectl delete namespace
-	kubectl describe namespace
-	kubectl apply -f <filename.yaml> (to create a namespace from a YAML file)
-	kubectl edit namespace <namespace-name> (to edit a namespace)
-	kubectl config set-context --current --namespace=<namespace-name> (to set the namespace for the current context)
-	kubectl get pods --namespace=<namespace-name> (to list all the pods in a specific namespace)
 
deployment related
-	kubectl get deployments (to list all deployments)
-	kubectl create deployment (to create a new deployment)
-	kubectl delete deployment (to delete a deployment)
-	kubectl describe deployment (to get detailed information about a deployment)
-	kubectl apply -f <filename.yaml> (to create or update a deployment from a YAML file)
-	kubectl rollout status deployment/<deployment-name> (to check the rollout status of a deployment)
-	kubectl rollout history deployment/<deployment-name> (to view the rollout history of a deployment)
-	kubectl rollout undo deployment/<deployment-name> (to undo the most recent rollout of a deployment)
-	kubectl scale deployment/<deployment-name> --replicas=<number-of-replicas> (to scale a deployment)
 
pods related
-	kubectl get pods (to list all pods)
-	kubectl describe pod (to get detailed information about a pod)
-	kubectl logs (to view the logs of a pod)
-	kubectl exec (to execute a command in a running container of a pod)
-	kubectl delete pod (to delete a pod)
-	kubectl apply -f <filename.yaml> (to create or update a pod from a YAML file)
-	kubectl port-forward (to forward a port from a pod to the local machine)
-	kubectl label pod (to add or remove labels from a pod)
 
The hierarchy in Kubernetes is as follows:
-	A cluster is a collection of nodes that run containerized applications.
-	Each cluster can contain one or more namespaces, which provide a way to group and isolate resources within a cluster.
-	Each namespace can contain multiple Kubernetes objects, such as pods, deployments, services, config maps, and secrets.
-	A pod is the smallest deployable unit in Kubernetes and represents a single instance of a running process in a cluster. A pod can contain one or more containers that share the same network and storage resources. Each container in a pod runs in its own isolated environment, similar to a lightweight virtual machine.
- A pod is usually contained within another Kubernetes object e.g. deployments, services, etc. To permanently delete a pod belonging to another Kubernetes object, you need to delete that object instead
 
- An ingress controller is a service that will create its own separate namespace from the other pods. 
You use it to load balance pods running in other namespaces (e.g. pods in default namespace for example).
 
 -------------
 
 What does single-node cluster mean? It means that the cluster contains only one node (i.e. one VM), aka the master node, and no worker nodes.
 Usually we have worker nodes (i.e. other VMs) that do the work that the master node orchestrates. The worker nodes will create pod(s) inside themselves to do the work.
 With a single node configuration, this means the master node will create the pods inside itself that do the work that it orchestrates. The master node itself also creates a pod that does the orchestration. This all means that nodes do not "do" the work; rather they spin up pods (i.e. containers) that would do the work(s) assigned to them.
 
 --------------

 How to set up your own cluster from scratch? https://1drv.ms/f/s!ApGS4APr_VQLpu8-A4kTgICxMlzEng?e=Bou0Dj

 --------------

 Book used for notes below: Kubernetes in Action

 ---------------

 ### Why we need deployment/replicationcontroller/statefulset/etc to manage pods?

 When you create unmanaged pods, a cluster node is selected to run the pod and then its containers are run on that node. Kubernetes then monitors those containers and automatically restarts them if they fail. But if the whole node fails, the pods on the node are lost and will not be replaced with new ones, unless those pods are managed by a Deployment, ReplicationControllers or similar.

 Kubernetes keeps your containers running by restarting them if they crash or if their liveness probes fail. This job is performed by the Kubelet on the node hosting the pod—the Kubernetes Control Plane components running on the master(s) have no part in this process.

 But if the node itself crashes, it’s the Control Plane that must create replacements for all the pods that went down with the node. It doesn’t do that for pods that you create directly. Those pods aren’t managed by anything except by the Kubelet, but because the Kubelet runs on the node itself, it can’t do anything if the node fails.

 ---------------

  ### Selector and labels

  A YAML like this:
  ```
  apiVersion: v1
kind: ReplicationController        ❶
metadata:
  name: kubia                      ❷
spec:
  replicas: 3                      ❸
  selector:                        ❹
    app: kubia                     ❹
  template:                        ❺
    metadata:                      ❺
      labels:                      ❺
        app: kubia                 ❺
    spec:                          ❺
      containers:                  ❺
      - name: kubia                ❺
        image: luksa/kubia         ❺
        ports:                     ❺
        - containerPort: 8080
  ```

  Means that the ReplicationController set will track pods with label `app=kubia`, and additionally create pods with `app=kubia` such that there are 3 pods of such label in the cluster. If you manually rename the label of one of the pods, it will not be tracked by this object anymore.

  Changing a ReplicationController’s pod template `labels` only affects pods created afterward and has no effect on existing pods.

 ---------------
 ### Liveliness

 Kubernetes can probe a container using one of the three mechanisms:

 - An HTTP GET probe performs an HTTP GET request on the container’s IP address, a port and path you specify. If the probe receives a response, and the response code doesn’t represent an error (in other words, if the HTTP response code is 2xx or 3xx), the probe is considered successful. If the server returns an error response code or if it doesn’t respond at all, the probe is considered a failure and the container will be restarted as a result.

 - A TCP Socket probe tries to open a TCP connection to the specified port of the container. If the connection is established successfully, the probe is successful. Otherwise, the container is restarted.

 - An Exec probe executes an arbitrary command inside the container and checks the command’s exit status code. If the status code is 0, the probe is successful. All other codes are considered failures.

 Restart reason can be described: `kubectl describe po kubia-liveness`

  The exit code e.g. 137 is a sum of two numbers: 128+x, where x is the signal number sent to the process that caused it to terminate. In the example, x equals 9, which is the number of the SIGKILL signal, meaning the process was killed forcibly.

When a container is killed, a completely new container is created—it’s not the same container being restarted again.

Always remember to set an initial delay to account for your app’s startup time

For a better liveness check, you’d configure the probe to perform requests on a specific URL path (/health, for example) and have the app perform an internal status check of all the vital components running inside the app to ensure none of them has died or is unresponsive.

Be sure to check only the internals of the app and nothing influenced by an external factor. For example, a frontend web server’s liveness probe shouldn’t return a failure when the server can’t connect to the backend database. If the underlying cause is in the database itself, restarting the web server container will not fix the problem. Because the liveness probe will fail again, you’ll end up with the container restarting repeatedly until the database becomes accessible again.

------------------
 
 ### Cronjob objects

 Job resources will be created from the CronJob resource at approximately the scheduled time. The Job then creates the pods.

 It may happen that the Job or pod is created and run relatively late. You may have a hard requirement for the job to not be started too far over the scheduled time. In that case, you can specify a deadline by specifying the startingDeadlineSeconds field in the CronJob specification as shown in the following listing.

 In normal circumstances, a CronJob always creates only a single Job for each execution configured in the schedule, but it may happen that two Jobs are created at the same time, or none at all. To combat the first problem, your jobs should be idempotent (running them multiple times instead of once shouldn’t lead to unwanted results). For the second problem, make sure that the next job run performs any work that should have been done by the previous (missed) run.

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
 ![Alt text](image-1.png)

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
 
 ![Alt text](image.png)

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
# RBAC (https://www.youtube.com/watch?v=jvhKOAyD8S8)

### RBAC for humans
- K8S uses certificates to authorize users access to the cluster.
- The flow is:
    - When you provision a k8s cluster, a `ca.crt` and `ca.key` file is created under (for Kind clusters) `/etc/kubernetes/pki` in the node(s)
    - We use this file to sign user certificates. We can create a certificate for a user named Bob by:
    - Running `openssl genrsa -out bob.key 2048`. This will generate a private key file called `bob.key`
    - Then, we need the create a Certificate Signing Request (CSR) by running `openssl req -new -key bob.key -out bob.csr -subj "/CN=Bob Smith/O=Shopping"`; where CN is Common Name and O is Organization `https://knowledge.digicert.com/general-information/what-is-a-distinguished-name`; CN can be your user name and O is the namespace this person is allowed to access (e.g. like the scope in OAuth)
    - Then we can generate a certificate by `signing` this `request`: `openssl x509 -req -in bob.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out bob.crt -days 1`
    - (So flow is create private key ->  create certificate signing request from key -> sign the request using `ca.key` and `ca.crt` -> you get a certificate)
    - Then, we use this certificate to create the .kubeconfig file for access to the cluster. This kubeconfig is what we give Bob.
- What does a kubeconfig file contain?
    - It contains 3 sections: `clusters`, `users`, `contexts`, and `current-context`
      - `clusters` section contain info on the cluster URL API server and the certificate-authority-data of the cluster. The certificate-authority-data is a CA certificate that `kubectl` can later use when connecting to the cluster URL aforementioned. When the kubectl connects to the API server, it expects the server to return a certificate. `kubectl` will compare this returned cert with the value in `certificate-authority-data` to ensure the URL is legit and prevent man in the middle attack.
        - Create the section in the kubeconfig using: `kubectl config set-cluster dev-cluster --server=https://127.0.0.1:52807 --certificate-authority=ca.crt --embed-certs=true `
        - This will create a section like:
          ```
          apiVersion: v1
          clusters:
          - cluster:
              certificate-authority-data: ASDFWETAHTHAWHTWHHRTAWHERHAW_I_AM_SOME_LONG_STRING
              server: https://my-cluster-api:someport
            name: cluster1
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
         - Create section in the kubeconfig using `kubectl config set-context dev --cluster=cluster1 --namespace=shopping --user=bob`
         - In this above example, it will bind the user `bob` to the cluster `cluster1`, and gives access to resources in namespace `shopping`
         - Result:
           ```
           contexts:
             - context:
               cluster: cluster1
               namespace: shopping
               user: bob
           ```
         - So bob has access only to that `shopping` namespace (I think! but not sure about this point, need to test further)
       - The `current-context` is the context that the user are going to connect to when they run `kubectl`. To set this, you (or the user) can run:
         - `kubectl config use-context cluster1`
       - You can indeed create more user in `users` and context in `contexts` for defining multiple users and contexts, and bind them appropriately.
 - If now bob runs the `kubectl get pods`, he will get `Error from server (Forbidden): pods is forbidden: User `Bob Smith` cannot list resource "pods" in API group "" in the namespace "shopping"` But we gave him access above????
 - This is because in the above we gave **ACCESSS** to Bob, but we still haven't given him **PERMISSION** to access resources.
 - To give **PERMISSION**, we need to give bob a `Role`. A Role example for the namespace `sh:
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
  - The critical sections explained:
    - What the hell is `apiGroups` and `resources`??? This confuses me a lot when I was at asml!!!
      - So k8s groups assigns objects to apiGroups in different versions of its API. For example for API v1, for different k8s objects, you will always see these section at the top:
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
 - Still Bob wont have **permission** to e.g. list pods within the namespace! This is because we need to bind the `Role` above to a `RoleBinding` k8s object, which will actually give the permission. Example:
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
- Finally, Bob can run `kubectl get pods -n shopping` successfully; it will list all pods in the `shopping` namespace.

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

# Services

### NGINX, and kubeproxy, how it works

- kube-proxy is a key component of any Kubernetes deployment.  Its role is to load-balance traffic that is destined for services (via cluster IPs and node ports) to the correct backend pods.

- Kube-proxy can run in one of three modes, each implemented with different data plane technologies: userspace, iptables, or IPVS.

- The service object load balances requests to the pods in its selector based on the load balancing setting of the chosen dataplane. The IPVS has much more load balancing options. Iptables only has random way of selection (it selects a random pod to serve the request)

- The service object sits behind an Ingress object, which in your case is of type NGINX. NGINX has and implements its own load balancing skills.
- 
- By default, the NGINX bypasses the services object and targets directly the pods underneath. Thus the pods benefit directly from NGINX's more advanced load balancing capabilities (rather than the Service's iptables random load balancing strategy). However, the NGINX does not consult the iptables for the available pods. Thus if a pod is terminating even after its ip address has been removed from the iptable, that pod can still be requested by NGINX and thus users can still see `connection refused`. This would not happen with Services; it always checks with the iptable first.

- To make NGINX not bypass Service, set this annotation https://github.com/kubernetes/ingress-nginx/issues/257#issuecomment-335835670. But this means your pods are not load balanced by NGINX anymore and you have to set IPVS rules on your service object for more advanced load balancing rules.

- More discussions and info here https://www.reddit.com/r/kubernetes/comments/161xrdb/am_i_load_balancing_correctly/ https://kubernetes.io/docs/reference/networking/virtual-ips/ 

### Difference between a headless service and a load balanced service
Headless services and load-balanced services in Kubernetes serve different purposes and have distinct characteristics. Here's a detailed comparison to help understand their differences:

Headless Service and example: 

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


Load-Balanced Service

    ClusterIP:
        Load-Balanced Service: The clusterIP is assigned by Kubernetes. This provides a single stable IP address for the service.
    Service Discovery:
        Load-Balanced Service: A single DNS A record is created for the service name. Clients use this single DNS name to connect to the service, which resolves to the service’s ClusterIP.
    Use Case:
        Load-Balanced Service: Suitable for stateless applications where clients can connect to any instance of the application, such as web servers, APIs, or microservices.
    DNS Records:
        Load-Balanced Service: DNS queries return a single A record corresponding to the service’s ClusterIP. For example, my-service.default.svc.cluster.local resolves to the ClusterIP.
    Load Balancing:
        Load-Balanced Service: Kubernetes handles the load balancing. The traffic is distributed among the pods behind the service using round-robin or other strategies.

```
apiVersion: v1
kind: Service
metadata:
  name: my-loadbalanced-service # my-loadbalanced-service.whatevernamespace.svc.cluster.local
spec:
  selector:
    app: my-app
  ports:
  - port: 80
    targetPort: 8080
  type: ClusterIP  # Default is ClusterIP, other types are NodePort, LoadBalancer, etc.
```
---------------------------
# Get logs of applications using Loki and display in Grafana

Followed the following with some modifications https://www.youtube.com/watch?v=Mn2YpMJaEBY&t=1359s

The following is the architecture. Each node has a log collector (`promtail`) that pipes its output to a log aggregator (`loki`) deployed in a node, which will then process the logs and pipe the results to `grafana` for visualization ![Alt text](grafanaloki.JPG)

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
