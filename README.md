3# notes

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

    # Function to check if a directory is empty or contains only empty subdirectories
    check_and_delete_empty_dir() {
      local dir=$1

      # If the directory is empty, delete it
      if [ -z "$(ls -A "$dir")" ]; then
        rm -rf "$dir"
        echo "Removed empty folder: $dir"
        return
      fi

      # Recursively check subdirectories
      local is_empty=true
      for subdir in "$dir"/*; do
        if [ -d "$subdir" ]; then
          check_and_delete_empty_dir "$subdir"
          # If a subdirectory wasn't deleted, this directory isn't empty
          if [ -d "$subdir" ]; then
            is_empty=false
          fi
        else
          # If a file is found, the directory isn't empty
          is_empty=false
        fi
      done

      # If all subdirectories were empty and deleted, delete this directory too
      if $is_empty; then
        rm -rf "$dir"
        echo "Removed empty folder: $dir"
      fi
    }

    # Iterate through each folder in the directory and check if they are empty
    for folder in "$directory"/*; do
      if [ -d "$folder" ]; then
        check_and_delete_empty_dir "$folder"
      fi
    done
```
