Context related
•	kubectl config use-context
•	kubectl config get-contexts
•	kubectl config current-context
•	kubectl config set-context
•	kubectl config delete-context
•	kubectl config view
•	kubectl config use-context <context-name> --namespace=<namespace>
Namespace related
•	kubectl get namespaces
•	kubectl create namespace
•	kubectl delete namespace
•	kubectl describe namespace
•	kubectl apply -f <filename.yaml> (to create a namespace from a YAML file)
•	kubectl edit namespace <namespace-name> (to edit a namespace)
•	kubectl config set-context --current --namespace=<namespace-name> (to set the namespace for the current context)
•	kubectl get pods --namespace=<namespace-name> (to list all the pods in a specific namespace)
deployment related
•	kubectl get deployments (to list all deployments)
•	kubectl create deployment (to create a new deployment)
•	kubectl delete deployment (to delete a deployment)
•	kubectl describe deployment (to get detailed information about a deployment)
•	kubectl apply -f <filename.yaml> (to create or update a deployment from a YAML file)
•	kubectl rollout status deployment/<deployment-name> (to check the rollout status of a deployment)
•	kubectl rollout history deployment/<deployment-name> (to view the rollout history of a deployment)
•	kubectl rollout undo deployment/<deployment-name> (to undo the most recent rollout of a deployment)
•	kubectl scale deployment/<deployment-name> --replicas=<number-of-replicas> (to scale a deployment)
pods related
•	kubectl get pods (to list all pods)
•	kubectl describe pod (to get detailed information about a pod)
•	kubectl logs (to view the logs of a pod)
•	kubectl exec (to execute a command in a running container of a pod)
•	kubectl delete pod (to delete a pod)
•	kubectl apply -f <filename.yaml> (to create or update a pod from a YAML file)
•	kubectl port-forward (to forward a port from a pod to the local machine)
•	kubectl label pod (to add or remove labels from a pod)
The hierarchy in Kubernetes is as follows:
•	A cluster is a collection of nodes that run containerized applications.
•	Each cluster can contain one or more namespaces, which provide a way to group and isolate resources within a cluster.
•	Each namespace can contain multiple Kubernetes objects, such as pods, deployments, services, config maps, and secrets.
•	A pod is the smallest deployable unit in Kubernetes and represents a single instance of a running process in a cluster. A pod can contain one or more containers that share the same network and storage resources.
•	Each container in a pod runs in its own isolated environment, similar to a lightweight virtual machine.
An ingress controller is a service that will create its own separate namespace from the other pods. 
 
You use it to load balance pods running in other namespaces (e.g. pods in default in the above pic).
