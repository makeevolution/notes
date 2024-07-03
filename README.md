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
1. Asynchronous Communication

    RabbitMQ: Supports asynchronous message passing, allowing the sender to continue processing without waiting for the receiver to acknowledge the message.
    HTTP: Typically synchronous, meaning the client waits for the server to process the request and respond, which can lead to higher latency and reduced performance in high-load scenarios.

2. Decoupling

    RabbitMQ: Facilitates loose coupling between services. Producers and consumers do not need to be aware of each other’s state or availability. This allows for easier scaling and maintenance.
    HTTP: Services are often tightly coupled, as clients need to know the exact endpoint and protocol to communicate with the server.

3. Reliability and Durability

    RabbitMQ: Provides message durability and acknowledgment features, ensuring that messages are not lost even if the system crashes or restarts. Messages can be stored in queues until they are successfully processed.
    HTTP: Lacks built-in mechanisms for message durability. Retries and fault tolerance need to be handled manually.

4. Load Balancing and Scalability

    RabbitMQ: Handles load balancing by distributing messages across multiple consumers. This allows for horizontal scaling by adding more consumers to process messages.
    HTTP: Load balancing needs to be managed at the application level or through external load balancers. Scaling requires additional infrastructure.

5. Complex Routing and Message Patterns

    RabbitMQ: Supports complex routing logic with exchange types (direct, topic, fanout, headers) that allow messages to be routed to different queues based on various criteria.
    HTTP: Routing is generally simpler and relies on URL patterns and query parameters, making complex routing and message distribution harder to implement.

6. Built-in Retry Mechanism

    RabbitMQ: Automatically retries message delivery if a consumer fails to process a message. Messages can be re-queued or sent to a dead-letter exchange if not processed within a certain time.
    HTTP: Requires custom implementation for retry logic and handling of failed requests.

7. Guaranteed Delivery

    RabbitMQ: Ensures guaranteed delivery of messages with acknowledgments, persistence, and clustering features.
    HTTP: No built-in guaranteed delivery. Retries and confirmations need to be manually managed.

8. Backpressure Handling

    RabbitMQ: Manages backpressure by controlling the flow of messages based on consumer availability and capacity, preventing overload.
    HTTP: Typically lacks built-in backpressure mechanisms, leading to potential overload of servers if too many requests are made simultaneously.

9. Transactional Messaging

    RabbitMQ: Supports transactions, allowing multiple messages to be published or acknowledged in a single atomic operation.
    HTTP: Does not natively support transactional operations across multiple requests without additional mechanisms such as distributed transactions or two-phase commit.

10. Monitoring and Management

    RabbitMQ: Provides comprehensive monitoring and management tools through the RabbitMQ Management Plugin, which offers a web-based UI for viewing queue lengths, message rates, and other metrics.
```
