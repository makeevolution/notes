## Relationship between Deployment, ReplicaSet and Pods

A deployment like this
```
apiVersion: apps/v1
kind: Deployment
metadata:
  creationTimestamp: null
  labels:
    app: nginx
  name: nginx
spec:
  replicas: 12
  selector:
    matchLabels:
      app: nginx
  strategy: {}
  template:
    metadata:
      creationTimestamp: null
      labels:
        app: nginx
    spec:
      containers:
      - image: nginx
        name: nginx
        resources: {}
status: {}
```

Will create automatically these:

```
root@control-plane:~# kubectl get deployment
NAME    READY   UP-TO-DATE   AVAILABLE   AGE
nginx   1/1     1            1           7s
root@control-plane:~# kubectl get replicaset
NAME               DESIRED   CURRENT   READY   AGE
nginx-7854ff8877   1         1         1       8s
kubectl get pods -o wide
NAME                     READY   STATUS    RESTARTS   AGE   IP          NODE            NOMINATED NODE   READINESS GATES
nginx-7854ff8877-4vd75   1/1     Running   0          72s   10.42.0.3   control-plane   <none>           <none>
kubectl get pods -o wide
NAME                     READY   STATUS    RESTARTS   AGE   IP          NODE            NOMINATED NODE   READINESS GATES
root@control-plane:~# kubectl rollout history deployment/nginx
deployment.apps/nginx 
REVISION  CHANGE-CAUSE
1         <none>
```

So, notice how the ReplicaSet name is constructed of the 
- Deployment name and a unique ID

and the Pod name is constructed of the
- Replica set name and a unique ID.

When you do scale the deployment, no new ReplicaSet is created (the existing is updated):

```
root@control-plane:~# kubectl scale deployment/nginx --replicas=10; watch kubectl get pods -o wide
deployment.apps/nginx scaled
root@control-plane:~# kubectl get replicaset
NAME               DESIRED   CURRENT   READY   AGE
nginx-7854ff8877   10        10        10      6m46s
```

But if you change the image tag, a new ReplicaSet is created (e.g. change `nginx` to `nginx:latest` in deployment.yaml above)

```
root@control-plane:~# kubectl get replicaset
NAME               DESIRED   CURRENT   READY   AGE
nginx-7854ff8877   0         0         0       8m22s
nginx-b9b667b8b    12        12        12      16s
```

If we change image tag to an image that doesn't exist (`nginx:bbb` for example), we will get a bad replicaset
```
root@control-plane:~# kubectl get replicaset
NAME               DESIRED   CURRENT   READY   AGE
nginx-7854ff8877   0         0         0       11m
nginx-b9b667b8b    9         9         9       3m17s
nginx-6cc4df8d47   6         6         0       8s
```
Notice the previous ReplicaSet is still `current=9`, and the `desired` is maintained also at 9, since the new one never come up!

So we will rollback the deployment. Rolling back a deployment removes the current replica set and creates a new replicaset:
```
root@control-plane:~# kubectl rollout undo deployment/nginx && kubectl rollout status deployment/nginx
deployment.apps/nginx rolled back
root@control-plane:~# kubectl get replicaset
NAME               DESIRED   CURRENT   READY   AGE
nginx-6cc4df8d47   0         0         0       2m3s
nginx-b9b667b8b    0         0         0       5m12s
nginx-7854ff8877   9         9         9      13m
```

And note, that the deployment history will change from
```
deployment.apps/nginx 
REVISION  CHANGE-CAUSE
1         <none>
2         <none>
3         <none>
```

to

```
REVISION  CHANGE-CAUSE
1         <none>
3         <none>
4         <none>
```

i.e. the revision 2 is removed!

Thus, rolling back to a previous version removes that previous version from history!

So, if we rollback to a specific revision e.g. 1, then we will get history

```
REVISION  CHANGE-CAUSE
3         <none>
4         <none>
5         <none>
```