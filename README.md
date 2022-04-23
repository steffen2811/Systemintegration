# Energy price using Kafka request-reply pattern.

<b>Steps to run:</b>

Run "Docker-compose up -d". This will start zookeeper, kafka and create topics.

Open Kafka_producer_energy_price.sln and run application.

<b>Info:</b>

Application will wait for request and return the price to the topic specified in the header. 

Correlation ID is also included to match the message ID from the request.
