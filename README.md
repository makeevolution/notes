# notes

## Notes on shortcuts/lessons learned for different topics
```
apiVersion: v1
kind: ConfigMap
metadata:
  name: cleanup-script
data:
  cleanup.sh: |
    #!/bin/sh
    echo "Running cleanup script..."
    
    directory="/mnt/pvc"
    
    for folder in "$directory"/*; do
      if [ -d "$folder" ]; then
        if [ ! -f "$folder/pod_in_use_marker.txt" ]; then
          rm -rf "$folder"
          echo "Removed folder: $folder"
        fi
      fi
    done
```
