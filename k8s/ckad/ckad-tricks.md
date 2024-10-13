
- `https://killercoda.com/playgrounds/scenario/ckad`

- Do not write YAML, but use cmd line and output to yaml to check e.g.
  ```
  kubectl get pods <PODNAME> -o yaml
  ```

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