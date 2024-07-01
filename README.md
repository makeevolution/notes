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
        # Check if the folder is empty
        if [ -z "$(ls -A "$folder")" ]; then
          # Remove the folder if it is empty
          rm -rf "$folder"
          echo "Removed empty folder: $folder"
        fi
      fi
    done
```
