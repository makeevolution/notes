course: https://app.pluralsight.com/library/courses/kubernetes-installation-configuration-fundamentals/table-of-contents

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

# Services, NGINX, and kubeproxy, how it works

- kube-proxy is a key component of any Kubernetes deployment.  Its role is to load-balance traffic that is destined for services (via cluster IPs and node ports) to the correct backend pods.

- Kube-proxy can run in one of three modes, each implemented with different data plane technologies: userspace, iptables, or IPVS.

- The service object load balances requests to the pods in its selector based on the load balancing setting of the chosen dataplane. The IPVS has much more load balancing options. Iptables only has random way of selection (it selects a random pod to serve the request)

- The service object sits behind an Ingress object, which in your case is of type NGINX. NGINX has and implements its own load balancing skills.
- 
- By default, the NGINX bypasses the services object and targets directly the pods underneath. Thus the pods benefit directly from NGINX's more advanced load balancing capabilities (rather than the Service's iptables random load balancing strategy). However, the NGINX does not consult the iptables for the available pods. Thus if a pod is terminating even after its ip address has been removed from the iptable, that pod can still be requested by NGINX and thus users can still see `connection refused`. This would not happen with Services; it always checks with the iptable first.

- To make NGINX not bypass Service, set this annotation https://github.com/kubernetes/ingress-nginx/issues/257#issuecomment-335835670. But this means your pods are not load balanced by NGINX anymore and you have to set IPVS rules on your service object for more advanced load balancing rules.

- More discussions and info here https://www.reddit.com/r/kubernetes/comments/161xrdb/am_i_load_balancing_correctly/ https://kubernetes.io/docs/reference/networking/virtual-ips/ 

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