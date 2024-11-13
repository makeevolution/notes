3# notes

## Notes on shortcuts/lessons learned for different topics

# Filename: RunPreCommitPeriodically.ps1

# Interval in seconds (e.g., 3600 for 1 hour)
$interval = 3600  # Modify this as needed

# Initial reference commit to compare against (e.g., the last commit)
$fromRef = "HEAD~1"

Write-Output "Starting periodic pre-commit checks..."

# Infinite loop to periodically check for changes
while ($true) {
    try {
        Write-Output "Checking for changes since $fromRef..."

        # Run pre-commit only on files changed between $fromRef and the latest commit
        & pre-commit run --from-ref $fromRef --to-ref HEAD

        # Update $fromRef to the latest HEAD after each check to focus on new changes
        $fromRef = "HEAD"

        Write-Output "Pre-commit check completed. Sleeping for $interval seconds..."

        # Sleep for the specified interval
        Start-Sleep -Seconds $interval
    }
    catch {
        Write-Error "An error occurred: $_"
        break
    }
}
