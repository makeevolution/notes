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

```
import re
from django.db.models import Q

def simplify_key_string(key_string):
    # Define regex patterns with word boundaries
    patterns = [
        r"\b(t(?:e(?:s(?:t)?)?)?)\b",
        r"\b(c(?:a(?:s(?:e)?)?)?)\b",
        r"\b(s(?:u(?:i(?:t(?:e)?)?)?)?)\b"
    ]
    
    # Initialize query and modified key_string
    query_search_on_execution_type = Q()
    modified_key_string = key_string
    
    # Iterate through patterns and modify key_string
    for pattern in patterns:
        regex_pattern = re.compile(pattern, re.IGNORECASE)
        match = regex_pattern.search(modified_key_string.lower())
        if match:
            query_search_on_execution_type |= (
                Q(test_suite_execution__isnull=True) if pattern == patterns[1]
                else Q(test_suite_execution__isnull=False) if pattern == patterns[2]
                else Q()
            )
            modified_key_string = re.sub(pattern, "", modified_key_string, flags=re.IGNORECASE)
    
    return query_search_on_execution_type, modified_key_string.strip()

# Example usage:
key_string = "This is a test case with suite."
queryset = YourModel.objects.filter(*simplify_key_string(key_string))
```
```
import pytest
from django.db.models import Q
from your_module import simplify_key_string  # Replace with actual module name

@pytest.mark.parametrize("input_string, expected_query, expected_modified_string", [
    ("This is a test case with suite.", 
     Q(test_suite_execution__isnull=True) | Q(test_suite_execution__isnull=False), 
     "This is a  with ."),
    ("Example with Test and Case.", 
     Q(test_suite_execution__isnull=True), 
     "Example with  and ."),
    ("Test Suite example.", 
     Q(test_suite_execution__isnull=False), 
     " example."),
    ("No matches here.", 
     Q(), 
     "No matches here."),
    ("Test Case and Suite.", 
     Q(test_suite_execution__isnull=True) | Q(test_suite_execution__isnull=False), 
     "  and ."),
    ("Just t for testing.", 
     Q(), 
     "Just t for testing.")
])
def test_simplify_key_string(input_string, expected_query, expected_modified_string):
    query_search_on_execution_type, modified_key_string = simplify_key_string(input_string)
    assert str(query_search_on_execution_type) == str(expected_query)  # Compare string representations
    assert modified_key_string == expected_modified_string
```