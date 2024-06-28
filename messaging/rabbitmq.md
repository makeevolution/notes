# General

https://github.com/makeevolution/messaging/tree/rabbit-mq/rabbitmq

- Notes about rabbitmq:
  - `Broker`: refers to a RabbitMQ server, responsible for receiving messages from producers (applications that send messages) and delivering them to consumers (applications that receive messages).
  - `Cluster`: A collection of brokers. In this context, the term `broker` is interchangeable with the term `node`
  - Components of a RabbitMQ Broker:

    Exchange: A message routing agent inside the broker that receives messages from producers. Receives messages from producers and routes them to queues based on routing rules.
        - 4 types: Fanout, Direct, Topic, Headers
        - See https://www.rabbitmq.com/tutorials/amqp-concepts#exchanges for more info

    Routing key: The property of the message, set by the publisher, that is used by exchanges for message routing. Not used by fanout exchanges, see https://www.rabbitmq.com/tutorials/amqp-concepts#exchanges

    Queue: Stores messages until they are consumed by consumers.

    Binding: Registers or bounds a queue to an exchange/tells the exchange "hey this queue exists and you need to be tracking it". See Exchange above for more information

    Connection and Channels: Provides connections for producers and consumers to interact with RabbitMQ. Channels within connections allow for multiplexing of communication.

- A client/publisher needs:

    - RabbitMQ host
        - This is the hostname of one of the rabbitmq nodes. If in k8s, the nodes will become a pod part of a statefulset; use a headless service and communicate to your node using its nodename; more info here https://github.com/makeevolution/messaging/blob/rabbit-mq/rabbitmq/kubernetes/rabbit-statefulset.yaml#L73 (this is for 3.9 though, setting through env var may be deprecated)
        - If deployed as a proper Bitnami chart, then you can set it here https://github.com/makeevolution/messaging/blob/rabbit-mq/rabbitmq/bitnami/rabbitmq-14.4.4/rabbitmq/values.yaml#L375. In any case, you can see the name in the UI.

    - RabbitMQ port to publish/consume
        - usually 5762
    - RabbitMQ username and password

- You will always need the `rabbitmq_peer_discovery_k8s` plugin for nodes in k8s to discover each other

- High availability:
    - If you deploy a cluster, you then have multiple nodes. You want messages in queues in a node to stay available if the node is down and still consumable. To have this HA setup, use quorum queue feature of RabbitMQ:  `https://www.rabbitmq.com/docs/quorum-queues#what-is-quorum` and `https://www.rabbitmq.com/docs/quorum-queues#declaring`
    - Well this means that the producer and the consumer needs to know all hosts that have the "mirrored" queue, so that if the host is down, the producer/consumer can then try contact the other host and get the message from the queue
