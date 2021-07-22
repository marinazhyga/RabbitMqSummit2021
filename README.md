# RabbitMqSummit2021
This solution contains two approaches how to route data between edges. 
## First approach represents a producer that publishes to a fanout exchange, dedicated to a certain edge.
![](images/fanout_federation.png)
Steps how to run the demo solution:
1. Expand -> Fanout folder of the solution.
2. Run RabbitMqSummit2021.CloudFanout with a proper appsettings.json pointing to the cloud RabbitMq cluster.
3. Run RabbitMqSummit2021.EdgeFanout with a proper appsettings.json pointing to the edge RabbitMq instance.
## Second approach represents a producer that publishes to a direct exchange, where a routing key is a unique edge identifier.
![](images/direct_shovel.png)
Steps how to run the demo solution:
1. Expand -> Direct folder of the solution.
2. Run RabbitMqSummit2021.CloudDirect with a proper appsettings.json pointing to the cloud RabbitMq cluster.
3. Run RabbitMqSummit2021.EdgeDirect with a proper appsettings.json pointing to the edge RabbitMq instance.

