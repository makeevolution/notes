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

```
            word_test_pattern = r"\b(t(?:e(?:s(?:t)?)?)?)\b"
            word_case_pattern = r"\b(c(?:a(?:s(?:e)?)?)?)\b"
            word_suite_pattern = r"\b(s(?:u(?:i(?:t(?:e)?)?)?)?)\b"
            regex_word_test_pattern = re.compile(word_test_pattern, re.IGNORECASE)
            regex_word_case_pattern = re.compile(word_case_pattern, re.IGNORECASE)
            regex_word_suite_pattern = re.compile(word_suite_pattern, re.IGNORECASE)
            word_test_in_keystring = regex_word_test_pattern.search(key_string.lower())
            word_case_in_keystring = regex_word_case_pattern.search(key_string.lower())
            word_suite_in_keystring = regex_word_suite_pattern.search(key_string.lower())
            if word_test_in_keystring:
                if word_case_in_keystring:
                    query_search_on_execution_type = Q() | Q(test_suite_execution__isnull=True)
                    key_string = re.sub(word_case_pattern, "", key_string, flags=re.IGNORECASE)
                elif word_suite_in_keystring:
                    query_search_on_execution_type = Q() | Q(test_suite_execution__isnull=False)
                    key_string = re.sub(word_suite_pattern, "", key_string, flags=re.IGNORECASE)
                else:
                    return queryset
                key_string = re.sub(word_test_pattern, "", key_string, flags=re.IGNORECASE)
            elif word_case_in_keystring:
                query_search_on_execution_type = Q() | Q(test_suite_execution__isnull=True)
                key_string = re.sub(word_case_pattern, "", key_string, flags=re.IGNORECASE)
                if word_suite_in_keystring:
                    query_search_on_execution_type = Q() | Q(test_suite_execution__isnull=False)
                    key_string = re.sub(word_suite_pattern, "", key_string, flags=re.IGNORECASE)
            elif word_suite_in_keystring:
                query_search_on_execution_type = Q() | Q(test_suite_execution__isnull=False)
                key_string = re.sub(word_suite_pattern, "", key_string, flags=re.IGNORECASE)
```
