# Debugging issues
### From experience
- set default namespace for easy kubectl `kubectl config set-context --current --namespace=mynamespace`
- Easily get pod name from output of `kubectl get pods` given row name (for namespace set in your kubeconfig)
  ```
  #!/bin/bash

  # Check if a row number was provided
  if [ -z "$1" ]; then
    echo "Usage: $0 <row_number>"
    return
  fi

  # Set the row number from the first argument
  ROW_NUMBER=$1

  # Get the pod name for the given row number
  POD_NAME=$(kubectl get pods --no-headers | sed -n "${ROW_NUMBER}p" | awk '{print $1}')

  # Check if the pod name was found
  if [ -z "$POD_NAME" ]; then
    echo "No pod found at row $ROW_NUMBER"
    return
  fi

  # Output the pod name
  echo "Pod at row $ROW_NUMBER: $POD_NAME"
  echo "POD_NAME env var contains this pod name"
  # Now you can use this variable for further kubectl commands if needed

  # ./get-pod-by-row.sh 5
  ```

- How to get into a `crashloopbackoff` container: in the container yaml, set this: `command: ["sh", "-c", "while true; do echo hello; sleep 86400; done"]` to override the image's entrypoint, and thus you can investigate the container's contents (e.g. whoami, ls -la, pwd, etc.) easily!!!, and also run the supposed command one by one.
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