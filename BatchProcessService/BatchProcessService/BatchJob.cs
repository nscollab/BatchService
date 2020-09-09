using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace BatchProcessService
{
    public static class BatchJob
    {
        public class UserEntity : TableEntity
        {
            public UserEntity() { }
            public UserEntity(string department, string UserName)
            {
                PartitionKey = department;
                RowKey = UserName;
            }
            public string ContactNumber { get; set; }
        }

        [FunctionName("BatchJob")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            List<string> contactList;
            TableBatchOperation batchOperationObj;
            UserEntity userEntityObj;
            int UserIncrementCounter = 0;

            try
            {
                //Getting Mock List Data
                contactList = GetMockContactnumbers();

                //Authenticating Table and creating Table instance
                CloudTable cloudTable = AuthTable();

                //Creating Batches of 100 items in order to insert them into AzureStorage
                var batches = contactList.Batch(100);

                log.Info($"Batch Job Started : {DateTime.Now}");

                #region BATCH JOB

                //Iterating through each batch
                foreach (var batch in batches)
                {
                    batchOperationObj = new TableBatchOperation();

                    //Iterating through each batch entities
                    foreach (var item in batch)
                    {
                        userEntityObj = new UserEntity("HR Department", "rowKey");
                        {
                            UserIncrementCounter++;
                            userEntityObj.PartitionKey = "HR Department";
                            userEntityObj.RowKey = "User" + UserIncrementCounter.ToString();

                            userEntityObj.ContactNumber = item;

                        }
                        batchOperationObj.InsertOrReplace(userEntityObj);
                    }

                    cloudTable.ExecuteBatch(batchOperationObj);
                }

                #endregion BATCH JOB

                log.Info($"Batch Job Completed : {DateTime.Now}");

            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }

            if (nextbatch.Count > 0)
            {
                yield return nextbatch;
            }
        }

        private static CloudTable AuthTable()
        {
            string accountName = "<<YOUR ACCOUNT STORAGE NAME>";
            string accountKey = "<<SORAGE ACCOUNT KEY>>";
            string TargetTableName = "TestTable";

            try
            {
                StorageCredentials storageCreds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount cloudStoageAccount = new CloudStorageAccount(storageCreds, useHttps: true);

                CloudTableClient cloudTableClient = cloudStoageAccount.CreateCloudTableClient();

                CloudTable cloudTable = cloudTableClient.GetTableReference(TargetTableName);

                return cloudTable;
            }
            catch
            {
                return null;
            }
        }

        public static List<string> GetMockContactnumbers()
        {
            Random rand = new Random();
            List<string> contactList = new List<string>();

            for (int i = 0; i < 250; i++)
            {
                //rand = new Random();
                //contactList = new List<string>();

                int randomNumber = rand.Next(8000000, 9999999);

                contactList.Add(randomNumber.ToString());
            }

            return contactList;
        }
    }
}
