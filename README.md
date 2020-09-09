# BatchService

This project/service helps to bulk insert your data/Entities into Azure Table Storage.
To achieve this I'm creating a mock data of users consisting contact numbers. Just for demo purpose I will be using 250 entities, you may replace this by your business data.

Please note :
  * Azure table storage only accepts 100 entities for batch insert operations, so in order to insert more than 100 entities into table we have to create batches of entities
in multiple of 100. I've also added the same logic into this service.
  * Partition key is the most important part in this process, to insert or (InsertOrReplace) function in azure table in bulk operation your partition key must be same, for 
demo purpose I've used static partition key. You may replace this as per your need.

Hope this solution helps... please fill free to update the code on git hub.
