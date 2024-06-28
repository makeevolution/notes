# notes

## Notes on shortcuts/lessons learned for different topics
https://killer.sh/ckad
id: produce_to_rabbitmq
namespace: company.team

tasks:
  - id: establish_an_exchange
    type: "io.kestra.plugin.amqp.DeclareExchange"
    url: amqp://guest:guest@rabbitmq-0.rabbitmq.rabbit.svc.cluster.local:5672/
    name: kestra-test-exchange
    durability: true  # survive a server restart
    exchangeType: FANOUT  # i.e. all messages sent to this exchange will be transmitted to all queues
  
  - id: bind_the_exchange_to_a_test_queue_we_made
    type: io.kestra.plugin.amqp.QueueBind
    url: amqp://guest:guest@rabbitmq-0.rabbitmq.rabbit.svc.cluster.local:5672/
    exchange: kestra-test-exchange
    queue: myQueue
    
  - id: publish-to-the-above-exchange
    type: io.kestra.plugin.amqp.Publish
    url: amqp://guest:guest@rabbitmq-0.rabbitmq.rabbit.svc.cluster.local:5672/
    exchange: kestra-test-exchange
    from:
      -  data: testtodayy
         headers: 
          testHeader: KestraTest
