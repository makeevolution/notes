
- `https://killercoda.com/playgrounds/scenario/ckad`

## General
- Do not write YAML, but use cmd line and output to yaml to check e.g.
  ```
  kubectl get pods <PODNAME> -o yaml
  ```
- Edit yaml: `kubectl edit resource/resourcename` e.g. `kubectl edit deployment/name`
- `k explain po.spec > po-spec
k explain po --recursive=true > po-full`
- Run a throwaway, rm will ensure it is deleted once complete, and restart makes sure it doesnt restart `k run busybox --image=busybox --rm -it --restart=Never -- wget -qO- https://www.google.com`
- `alias kn='kubectl config set-context --current --namespace'` the order matters!
- If you use `-it`, you don't need to do `/bin/sh -c` in your command!
- If you make a command in yaml though, need to still include sh:
  `command: ['sh', '-c', "echo 'test 123' > somefile.txt"]

### ConfigMap
- Stores data. The `data` field is always a key-value pair.
```
apiVersion: v1
kind: ConfigMap
metadata:
  name: complex-config
data:
  # This will be a multi-line file, e.g., a configuration file or certificate
  config-file.yaml: |
    setting1: value1
    setting2: value2
    setting3: value3
    # Multiline data can include complex configurations
    database:
      host: db.example.com
      port: 5432
  # This is a simple key-value pair that could be used as an environment variable
  ENVIRONMENT: "production"
```
- This `data` can be used in two ways in the pod: as a file mounted into the pod or as a env var. Here we show both! :
```
apiVersion: v1
kind: Pod
metadata:
  name: pod-using-complex-config
spec:
  containers:
  - name: my-container
    image: nginx
    # Using the simple key as an environment variable
    env:
    - name: ENVIRONMENT
      valueFrom:
        configMapKeyRef:
          name: complex-config
          key: ENVIRONMENT
    # Mounting the multi-line YAML file as a volume
    volumeMounts:
    - name: config-volume
      mountPath: /etc/config
  volumes:
  - name: config-volume
    configMap:
      name: complex-config
```
In this pod, there will be two files then created, `ENVIRONMENT` and `config.yaml`, in the path `/etc/config`, with the contents appropriately. Also, the env var `ENVIRONMENT` with value `production` will be available.

So for env var, you can choose which key to inject, but for volume mount, all data in the configmap will be written!

### Volumes and VolumeMounts!
- All the bullshit that always makes you feel overwhelmed and lose time in the exam (e.g. `emptyDir`, `persistentVolumeClaim`) is ALWAYS in the `volumes` section; in `volumeMounts` you only be a customer and ask hey I want this volume with this name, I wanna mount it in this location within me!
- All the posssibilities of the bullshit (with separators to really dumb down this thing)
```
apiVersion: v1
kind: Pod
metadata:
  name: my-app
spec:
########################################################################
  containers:
  - name: app-container
    image: nginx
    #!!!!!!!!!!!!!
    volumeMounts:
    - name: config-volume          # Mounting ConfigMap as files
      mountPath: /etc/config       # Files will be accessible in /etc/config with filenames of keys and content of each file being the value of each key
    - name: persistent-storage      # Mounting PersistentVolume for persistent storage
      mountPath: /data              # Files will be accessible in /data
    - name: secret-volume           # Mounting Secret as files
      mountPath: /etc/secret        # will create files with filenames of keys and content of each file being the value of each key
    - name: temp-storage            # Mounting EmptyDir for temporary storage
      mountPath: /tmp               # Files will be accessible in /tmp by all containers in the pod claiming temp-storage
    - name: host-volume             # Mounting HostPath for access to host filesystem
      mountPath: /host              # Files will be accessible in /host
    - name: nfs-volume              # Mounting NFS for network storage
      mountPath: /mnt/nfs           # Files will be accessible in /mnt/nfs
    - name: downward-api-volume     # Mounting Downward API for pod metadata
      mountPath: /etc/podinfo       # Pod metadata will be accessible in /etc/podinfo
      #!!!!!!!!!!!!!!!
###################################################################################
  volumes:
  - name: config-volume
    configMap:
      name: my-config               # Reference to a ConfigMap named 'my-config'
  - name: persistent-storage
    persistentVolumeClaim:
      claimName: my-pvc              # Reference to a PersistentVolumeClaim named 'my-pvc'
  - name: secret-volume
    secret:
      secretName: my-secret          # Reference to a Secret named 'my-secret'
  - name: temp-storage
    emptyDir: {}                    # Creates an EmptyDir volume
  - name: host-volume
    hostPath:
      path: /host/path               # Reference to a path on the host node
  - name: nfs-volume
    nfs:
      server: nfs-server.example.com  # NFS server hostname
      path: /path/to/share            # Path on the NFS server
  - name: downward-api-volume
    downwardAPI:
      items:
      - path: "pod_name"
        fieldRef:
          fieldPath: metadata.name     # Accesses pod name through Downward API
```
### Deployment
- Deployment history: `kubectl rollout deployment/deplname`
- Deployment strategies:
  - RollingUpdate	Gradually replaces old Pods with new ones, ensuring minimal downtime. This is the default   strategy.	Zero downtime, rollback possible	Takes longer for large updates
  - Recreate	Deletes all old Pods first, then creates new Pods.	Simple, no overlap between versions	Causes downtime during updates
  - Blue-Green	Deploys the new version alongside the old version and switches traffic once validation is complete.	Zero downtime, easy rollback	Resource-intensive, custom setup needed
    - To do this, edit the yaml using `kubectl edit` and put in the updates (e.g. new image version), and afterwards, make the `podSelector` label point to a new label, then apply it.
    - The service object will still point to the old pods with old label, so existing stuff is fine
    - We have the new deployment. Then we can test it out.
    - Once it's found to be fine, `kubcetl edit` the service to target the pods with the new labels.
    - To use Helm to do Blue-Green deployment, use `flagger` https://docs.flagger.app/tutorials/kubernetes-blue-green
  - Canary	Gradually releases the new version to a subset of users before full rollout.	Fine-grained control, risk mitigation	Requires custom implementation

## NetworkPolicies
- `Network Policies`: If you configure it for ingress, only those configured for that ingress will be allowed! Other connections won't work!
Example:

```
apiVersion: v1
items:
- apiVersion: networking.k8s.io/v1
  kind: NetworkPolicy
  metadata:
    name: internal-policy
  spec:
    egress:
    - to:
      - podSelector:
          matchLabels:
            name: mysql
      ports:
      - port: 3306
        protocol: TCP
    - to:
      - podSelector:
          matchLabels:
            name: payroll
      ports:
      - port: 8080
        protocol: TCP
    podSelector:
      matchLabels:
        name: internal
    policyTypes:
    - Egress
```
Means: For pods with label `name: internal`, only allow traffic from it, to pods with label `name: mysql` and `name: payroll`. Additionally, only allow traffic to port 3306 for the `name: mysql`, and to port 8080 for `name: payroll`.

- The above will be applied to `default` namespace. Meaning, pods with these labels that are not in the `default` namespace will not be affected.

- To apply a policy (or any other k8s rules) to a particular namespace, set it explicitly in `metadata` e.g.
```
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: np
  namespace: space1
spec:
  podSelector: {}
  policyTypes:
  - Egress
  egress:
  - ports:
    - port: 53
      protocol: TCP
    - port: 53
      protocol: UDP
  - to:
     - namespaceSelector:
        matchLabels:
         kubernetes.io/metadata.name: space2
```
will apply to all pods in namespace `space1`, the rule that outgoing traffic can only go to pods in namespace `space2`. 
NOTICE: different to the previous example, the ports section is NOT inside the `to` block! This means, the policy then still, allows outgoing traffic through ports 53 of those pods in `space1`, to anywhere in the internet (e.g. `curl nslookup google.com` is still allowed from these pods (i.e. traffic through port 53 to a destination outside of pods in `space2` is allowed!))
