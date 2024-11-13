3# notes

## Notes on shortcuts/lessons learned for different topics

#!/bin/bash

# Set the interval in seconds (e.g., 3600 seconds for 1 hour)
INTERVAL=3600  # Modify this as needed for the frequency you want

# Reference commit to compare against for changes
FROM_REF="HEAD~1"  # Set to the commit you want to compare with, such as the last commit

# Run an infinite loop until interrupted
while true; do
    echo "Checking for changes since $FROM_REF..."

    # Run pre-commit only on files changed between FROM_REF and the latest commit
    pre-commit run --from-ref "$FROM_REF" --to-ref HEAD

    # Update FROM_REF to the latest HEAD after each check to only catch new changes next time
    FROM_REF="HEAD"

    echo "Pre-commit completed. Sleeping for $INTERVAL seconds..."
    sleep $INTERVAL
done

  private paginate(page: number, itemsPerPage: number): string[] {
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return this.logs.slice(startIndex, endIndex);
  }
}
