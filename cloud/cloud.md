# Buzzwords
- Cloud native application delivery:
  - Rapid, scalable and resilient deployment of applications in cloud environments
  - Uses containersiation, microservices to achieve this
- GitOps:
  - A method of infrastructure management where we use `git` as the single source of truth to manage k8s clusters and application delivery. Read below for what this is useful for.

# GitOps
- We have used `helm` to deliver application (through the use of `helm install` in our experience till now.
- However, `helm` is mainly primarily only used for packaging; `helm install` is meant as supplementary, just for POC simple deployments.
  - `Helm`'s main purpose is it's templating feature, not the install feature.
- Most importantly, `helm` install does not have ability to track any changes/deviations of the deployed application with the git repository the helm chart is committed in.
  - Thus, if you deployed an app in namespace `namespacedeployedbyhelm` using Helm, and then someone accidentally does `kubectl delete namespace -n namespacedeployedbyhelm`, the deployment will not try to re-sync with the Helm code in the git repository, and the app goes down indefinitely!
  - i.e. although the deployed app is robust against application-level errors (through k8s features such as liveness probes etc.), it is not robust against accidental changes in the k8s manifests itself/platform-level changes
- `GitOps` is then the way to achieve this synchronization, with `Argo CD` being one such tool to achieve this
  - First we install `Argo CD` on our cluster, after install we get this
  ![alt text](argocd.png)
  - Then we can get into its UI and deploy a k8s manifest(s), this can also be a Helm chart (example: https://github.com/argoproj/argocd-example-apps/tree/master/helm-guestbook)
  ![alt te2xt](argocd_guestbook.png)
  We also get a nice UI on our deployment and what is going on
  visually; although we usually would use `k9s` for this
  ![alt te2xt](argocd_guestbookui.png)
  - If we do `kubectl delete namespace guestbook` from CLI, `Argo CD` will detect that there is deviation from what is deployed with what is there in git, and will restore to what git has
    ![alt te2xt](argocd_guestbookoutofsync.png)
- This would've been useful for `vfab dashboard` app back then
  - When we want to update our password in `secrets.yaml`, we had to
    - Checkout master of the chart of the app deployment in the worker node
    - Make a new branch
    - Change the password in `secrets.yaml`
    - Do `helm upgrade .....`
    - Push feature branch to remote
    - Make PR and merge to master
  - The above is time consuming and error prone; we may forgot to push feature branch and make PR. Manual things like this can make the Helm chart deployed and what is in master in git go out of sync easily!!! It was ok because it was only I who did anything with k8s; but if there are 10 people working on it, it will go awry.
  - Had we used GitOps instead (e.g. with the Argo CD), we only need to make the PR and merge to master, and the upgrade is done automatically by `Argo CD`! Less error prone!

# Pricing
- A VM in the cloud's pricing can be in 3 categories:
  - On-demand
  - Reserved
  - Spot
- On-demand:
  - Most flexible but most expensive
- Reserved:
  - Less flexible (need to commit at least for a given time period), but is less expensive
- Spot:
  - Can be deleted by the cloud provider at any time, if the VM is sitting idle for too long, but is the cheapest

# KCNA specific things
- logs can be output as text or json
- kubectl logs get both stdout and stderr
- prometheus expose liveness probe on `metrics` endpoint
- `APM server` uses a push model metric collection i.e. the apps send the metrics to the APM server (through `opentelemetry-sdk apm sdk` for Django as example, see powerpoint)
- `Prometheus` uses a pull model metric colelction i.e. the app needs to have a special endpoint open for prometheus to collect data from
  - Prometheus provides client libraries for most programming languages that you can use to instrument in-house apps for Prometheus integration.
  - If you don’t have access to the source code of 3rd-party apps, you won’t be able to directly instrument them via client libraries. In these situations, you can run a Prometheus exporter in a sidecar container that will reformat data for Prometheus.
  - If the app is a cron job (so it will die by the time prometheus tries to collect from it), it can push metrics to a tool called `PushGateway` that Prometheus offers, and then Prometheus will scrape it periodically
  - Prometheus also offer tool called `AlertManager` to alert people on things not going well/when certain metrics values are reached
- `namespaced vs virtualized vs sandboxed`;
  - namespaced resources isolated at kernel level ie process ids, network, file systems all are isolated, but they share the host kernel (less secure) e.g. Linux Containers LXC or Docker
     - if container host has kernel vulnerability, the host is vulnerable (kernel escape)
     - use tools like `seccomp, SELinux` to minimise damage
  - virtualized is VMWare, VirtualBox, Hyper V etc
    - expensive resource wise
  - Sandboxed is like running browser plugins or third party libraries ie run only one process pid in a secured environment
- Cluster network policies only manage traffic between pods, but they cant do zecurity like encrypt traffic etc
- Zipkin Jaeger is for tracing
- Traefik is competitor of nginx
- kubectl upgrades support one v up and one v down
- k8s has 3 release every year (every 4 months)
- Linkerd and Istio: same thing
- KNative is serverless like OpenFaas 

1. 5000 nodes, 500 pods; max no of nodes/pods supported:

Kubernetes officially supports clusters of up to 5000 nodes and 150,000 total pods (with a max of 300 pods per node). This is the officially tested limit as per Kubernetes' scalability documentation.

For example, with 5000 nodes, if you have 500 pods per node, you'd be overshooting the pod limit. Kubernetes is optimized for 150,000 pods in total cluster-wide.


2. Network vs Host Network:

Network (Pod Network): This is the default Kubernetes networking model, where each pod gets its own IP address and communicates with other pods or services via the network overlay (CNI plugins like Calico, Flannel).

Host Network: If a pod is running in host network mode, it shares the network namespace with the host (node). That means the pod will use the host’s IP address directly. This is useful when you want the pod to have direct access to the node's network interface.


3. Falco vs LXF:

Falco is a security runtime tool that monitors system calls and flags abnormal behaviors in a Kubernetes environment. It detects and responds to anomalies based on predefined rules.

LXF doesn’t seem to exist as a well-known security runtime tool. If you meant "AppArmor" or "SELinux," those are security frameworks, but Falco is the more relevant security runtime monitoring tool for Kubernetes.


4. Auto-created Namespaces (Kubernetes):

By default, Kubernetes creates the following namespaces:

default: The default namespace for resources not explicitly placed in a specific namespace.

kube-system: This namespace holds system-level pods like the Kubernetes API server, controller manager, etcd, and other control plane components.

kube-public: A namespace that is readable by all users, used for resources that should be publicly visible.

kube-node-lease: Used for node heartbeat information, helping the Kubernetes control plane keep track of node health.


5. External etcd: Recommended number of nodes and planes:

Recommended number of etcd nodes: 3, 5, or 7 nodes (always an odd number to maintain quorum in a distributed etcd cluster).

External etcd cluster: Yes, etcd can be external. Kubernetes uses etcd as its key-value store, and while you can run etcd on control plane nodes (embedded), running it externally is a best practice for better fault tolerance and separation of concerns.


6. kube-proxy – Where is it?

kube-proxy runs on every node in the cluster (both worker and control plane nodes). It maintains the network rules for Kubernetes services, enabling service discovery and load balancing within the cluster.


7. kubelet – Where is it?

kubelet is the agent that runs on all nodes, including control plane nodes. Its role is to communicate with the Kubernetes API server to ensure containers are running as expected on each node.


8. PROMQL (Prometheus Query Language):

PROMQL is the query language used by Prometheus to query time-series data from its data store. It is designed to filter and aggregate metric data, such as CPU usage, memory consumption, or request latencies. Unlike Splunk or Elasticsearch, Prometheus is designed for metrics monitoring, not logging.

PROMQL might feel different because it's tailored for numerical data over time, which is a bit different from the free-text search in Splunk or Elasticsearch.

9. Is runc OCI compliant? How about gVisor and Kata?

runc: Yes, runc is OCI compliant (Open Container Initiative). It’s a lightweight container runtime that spawns and runs containers based on OCI specs. runc is the default runtime used by Docker and Kubernetes.

gVisor: No, gVisor is not fully OCI compliant. It's a user-space container runtime that focuses on enhanced security by sandboxing containers.

Kata Containers: Yes, Kata is OCI compliant. It uses lightweight virtual machines to isolate containers for increased security, offering a balance between VMs and containers.


10. Service Mesh: Data Plane and Control Plane OR Service Proxy and Control Plane?

In a service mesh, the two main components are:

Data Plane: This consists of the proxies (like Envoy) running alongside application containers, intercepting and managing communication between services (this is what you might call the service proxy).

Control Plane: This manages and configures the data plane proxies, providing features like traffic routing, security policies, and monitoring (e.g., Istio’s control plane manages Envoy proxies).



The terms data plane and control plane are the most accurate, but you can think of them as service proxy (data plane) and management/control layer (control plane).

