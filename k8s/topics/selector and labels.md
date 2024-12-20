A `label` is used to identify and organize resources.

A `selector` is used to select resources that has a given label.

Example: If we have a `pod` template

```
apiVersion: v1
kind: Pod
metadata:
  creationTimestamp: null
  labels:
    run: nginx
  name: nginx
spec:
  containers:
  - image: nginx
    name: nginx
    ports:
    - containerPort: 80
    resources: {}
  dnsPolicy: ClusterFirst
  restartPolicy: Always
status: {}`
```
It will have a label `run: nginx`. For a service to select this pod, make a service that has this value in selector section of it:

```
root@control-plane:~# kubectl expose pod/nginx --dry-run=client -o yaml
apiVersion: v1
kind: Service
metadata:
  creationTimestamp: null
  labels:
    run: nginx
  name: nginx
spec:
  ports:
  - port: 80
    protocol: TCP
    targetPort: 80
  selector:
    run: nginx
status:
  loadBalancer: {}
```

Note that in creating the above we use `kubectl expose` command, which makes a service object having a label with value equal to the object it is exposing; but the important part for this is the `selector: run: nginx` part

Another example: A YAML like this:
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