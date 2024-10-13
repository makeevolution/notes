- Do not write YAML, but use cmd line and output to yaml e.g.
  ```
  kubectl create networkpolicy allow-nginx-ingress \
  --namespace=default \
  --pod-selector=app=nginx \
  --ingress \
  --port=80 --protocol=TCP \
  --from=podSelector=role=frontend \
  --dry-run=client -o yaml > network-policy.yaml

  ```
