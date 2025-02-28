
3# notes

## Notes on shortcuts/lessons learned for different topics
    ORCHES_0001 = "Test case execution workflow completed!"
    START_CONSUME_THREAD_1 = {'code': 'COMMON_0000', 'message': 'Starting consumer thread for queue: {queue_name}'}
    GOT_MESSAGE_1 = {'code': 'COMMON_0001', 'message': 'Got message: {message}'}
    START_EXECUTION_0 = {'code': 'COMMON_0002', 'message': 'Start execution'}
    STOP_CONSUMER_THREAD_1 = {'code': 'COMMON_0003', 'message': 'Stopping consumer thread for queue: {queue_name}'}
    RETRIES_EXECEEDED_0 = {'code': 'COMMON_0004', 'message': 'Retries exceeded'}
    ATTEMPT_SEND_MESSAGE_2 = {'code': 'COMMON_0005', 'message': 'Attempting to send message to exchange {exchange_name} with routing key {routing_key}'}
    HEADERS_1 = {'code': 'COMMON_0006', 'message': 'Headers: {headers}'}
    MESSAGE_1 = {'code': 'COMMON_0007', 'message': 'Message: {message}'}
    MESSAGE_SENT_SUCCESS_0 = {'code': 'COMMON_0008', 'message': 'Message sent successfully'}
    EXCEPTION_SEND_MSG_1 = {'code': 'COMMON_0009', 'message': 'Exception occurred trying to send message: {message}!'}
    PROCESSING_MESSAGE_1 = {'code': 'COMMON_0010', 'message': 'Processing message with delivery tag: {delivery_tag}'}
    PROCESSED_MESSAGE_1 = {'code': 'COMMON_0011', 'message': 'Processed message and acknowledging delivery tag {delivery_tag}'}
    UNEXPECTED_ERROR_1 = {'code': 'COMMON_0012', 'message': 'Unexpected error occurred and sending nack for delivery tag: {delivery_tag}'}
    CONNECTION_CLOSED_0 = {'code': 'COMMON_0013', 'message': 'Connection is closing or already closed'}
    CLOSING_CONNECTION_0 = {'code': 'COMMON_0014', 'message': 'Closing connection'}
    CONNECTING_TO_BROKER_1 = {'code': 'COMMON_0015', 'message': 'Connecting to {msg_broker}'}
    CONNECTION_OPEN_SUCCESS_0 = {'code': 'COMMON_0016', 'message': 'Connection is successful, opening channel'}
    CONNECTION_OPEN_FAILED_0 = {'code': 'COMMON_0017', 'message': 'Connection open failed'}
    CONNECTION_CLOSED_RECONNECT_1 = {'code': 'COMMON_0018', 'message': 'Connection closed, reconnect necessary due to: {reason}'}
    CREATING_NEW_CHANNEL_0 = {'code': 'COMMON_0019', 'message': 'Creating a new channel'}
    CHANNEL_OPENED_0 = {'code': 'COMMON_0020', 'message': 'Channel opened'}
    ADDING_CHANNEL_CLOSED_CALLBACK_0 = {'code': 'COMMON_0021', 'message': 'Adding channel close callback'}
    CHANNEL_CLOSED_2 = {'code': 'COMMON_0022', 'message': 'Channel {channel} was closed: {reason}'}
    DECLARING_DLX_1 = {'code': 'COMMON_0023', 'message': 'Declaring dead letter exchange: {dlx}'}
    DECLARING_EXCHANGE_1 = {'code': 'COMMON_0024', 'message': 'Declaring exchange: {dlx}'}
    EXCHANGE_DECLARED_1 = {'code': 'COMMON_0025', 'message': 'Exchange declared: {exchange}'}
    DLX_DECLARED = {'code': 'COMMON_0026', 'message': 'Dead letter exchange declared: {dlx}'}
    DECLARING_DL_QUEUE_1 = {'code': 'COMMON_0027', 'message': 'Declaring dl queue: {dlqueue}'}
    DECLARING_QUEUE_1 = {'code': 'COMMON_0028', 'message': 'Declaring queue: {queue}'}
    BINDING_DLX_TO_QUEUE_3 = {'code': 'COMMON_0029', 'message': 'Binding dlx: {dl_exchange_name} to dl queue: {dl_queue_name} with routing key: {routing_key}'}
    BINDING_EXCHANGE_TO_QUEUE_3 = {'code': 'COMMON_0030', 'message': 'Binding exchange: {exchange_name} to queue: {queue} with routing key: {key}'}
    QUEUE_BOUND_1 = {'code': 'COMMON_0031', 'message': 'Queue bound: {queue}'}
    DL_QUEUE_BOUND_1 = {'code': 'COMMON_0032', 'message': 'Dead letter queue bound: {dlq}'}
    QOS_SET_TO_2 = {'code': 'COMMON_0033', 'message': 'QOS of {queue_name} set to: {prefetch_count}'}
    ISSUING_CONSUMER_RPC_0 = {'code': 'COMMON_0034', 'message': 'Issuing consumer related RPC commands'}
    ADDING_CONSUMER_CANCEL_CALLBACK_0 = {'code': 'COMMON_0035', 'message': 'Adding consumer cancellation callback to channel'}
    CONSUMER_CANCELLED_REMOTELY_1 = {'code': 'COMMON_0036', 'message': 'Consumer was cancelled remotely, shutting down: {method_frame}'}
    ACKNOWLEDGING_MSG_1 = {'code': 'COMMON_0037', 'message': 'Acknowledging message {msg}'}
    SENDING_BASIC_CANCEL_RPC_0 = {'code': 'COMMON_0038', 'message': 'Sending a Basic.Cancel RPC command to RabbitMQ'}
    RABBITMQ_ACKNOWLEDGED_CANCEL_1 = {'code': 'COMMON_0039', 'message': 'RabbitMQ acknowledged the cancellation of the consumer: {consumer_tag}'}
    CLOSING_CHANNEL_0 = {'code': 'COMMON_0040', 'message': 'Closing the channel'}
    STOPPING_0 = {'code': 'COMMON_0041', 'message': 'Stopping'}
    STOPPED_0 = {'code': 'COMMON_0042', 'message': 'Stopped'}
    RECONNECTING_AFTER_SECONDS_1 = {'code': 'COMMON_0043', 'message': 'Reconnecting after {delay} seconds'}
    ERROR_LOADING_CONSUMER_2 = {'code': 'COMMON_0044', 'message': 'Error loading consumer {consumer_class}: {error}'}



348        self, 

349        response: requests.Response, 

350        *args: typing.Any,  # pylint: disable=unused-argument 

351        **kwargs: typing.Any,  # pylint: disable=unused-argument 

352    ) -> requests.Response: 

353        """ 

354        Helper method to append auth headers on requests automatically made after a response i.e. a redirection 

355 

356        Args: 

357            response: The response from Jenkins 

358            args: variable arguments 

359            kwargs: variable keyword arguments 

360 

361        Returns: 

362            The response object with redirection request headers appended with the authorization headers 

363        """ 

364        auth_encoded = base64.b64encode(f"{self.username}:{self.password}".encode()).decode()  # noqa: WPS221 

365        response.request.headers["Authorization"] = f"Basic {auth_encoded}" 

366        return response 
        verify=False,
        stream=True,
        auth=test_class_instance.auth,
    )
