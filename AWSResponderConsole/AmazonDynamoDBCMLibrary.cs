using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using Amazon.DynamoDB.DataModel;
using Amazon.SecurityToken;
using Amazon.Runtime;

namespace AWSResponderConsole
{

    class AmazonDynamoDBCMLibrary
    {
        AmazonDynamoDB client;
        public AmazonDynamoDBCMLibrary()
         {
             AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient();
             RefreshingSessionAWSCredentials sessionCredentials = new RefreshingSessionAWSCredentials(stsClient);
             client = new AmazonDynamoDBClient(sessionCredentials);
         }
        public AmazonDynamoDBCMLibrary(Amazon.Runtime.SessionAWSCredentials creds)
        {
            client = new AmazonDynamoDBClient(creds);
        }
        public void AddAccountBaselineCILog(AccountBaselineCILog CILog)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            context.Save(CILog);
        }
        public void AddAccountBaselineCI(AccountBaselineCI CI)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            context.Save(CI);
        }
        public void ModifyAccountBaselineCI (AccountBaselineCI CI)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            AccountBaselineCI oCI = context.Load < AccountBaselineCI>(CI.Hash);
            if(oCI==null)
                new Exception("Non-existent CI");
            context.Save(CI);
        }
        public IEnumerable<AccountBaselineCI> GetAllCIs()
        {
            DynamoDBContext context = new DynamoDBContext(client);
            IEnumerable<AccountBaselineCI> allcis = context.Scan<AccountBaselineCI>();
            return allcis;
        }
        public IEnumerable<AccountBaselineCI> SearchCIs(string hash, long range)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            IEnumerable<AccountBaselineCI> allcis = context.Query <AccountBaselineCI>(hash, Amazon.DynamoDB.DocumentModel.QueryOperator.GreaterThanOrEqual, range);
            return allcis;
        }
        /// <summary>
        /// If you do not want to include the ReleaseYear in the search criterion, you will use the following method to query based on the Title. Notice that this returns a List object that contains a Dictionary object with the Key Value Pair of Items.
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public List<Dictionary<string, AttributeValue>> SearchCIsByAccountAndClass(string AccountID, string ConfigurationItemClass)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            QueryRequest reqQuery = new QueryRequest();
            reqQuery.TableName = "AcountBaselineCIs";
            reqQuery.HashKeyValue = new AttributeValue() { S = AccountID + ConfigurationItemClass };

            QueryResponse resQuery = client.Query(reqQuery);
            List<Dictionary<string, AttributeValue>> dic = resQuery.QueryResult.Items;
            return dic;
        }
        public void DeleteCI(AccountBaselineCI ci)
        {
            DynamoDBContext context = new DynamoDBContext(client);
            AccountBaselineCI oCI = context.Load < AccountBaselineCI>(ci.Hash, ci.RangeKey);
            if (oCI == null)
                new Exception("Non-existent CI");
            context.Delete(ci);
        }

        public void Init(Amazon.Runtime.SessionAWSCredentials creds)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(creds);
            Amazon.DynamoDB.Model.ListTablesResponse resp = client.ListTables(new ListTablesRequest());

                List<string> currentTables = client.ListTables(new ListTablesRequest()).ListTablesResult.TableNames;

            if (!currentTables.Contains("AcountBaselineCIs"))
            {
                CreateTableRequest reqCreateTable = new CreateTableRequest();
                CreateTableResponse resCreateTable=new CreateTableResponse();

                reqCreateTable.TableName = "AcountBaselineCIs";

                reqCreateTable.ProvisionedThroughput = new ProvisionedThroughput();
                reqCreateTable.ProvisionedThroughput.ReadCapacityUnits=10;
                reqCreateTable.ProvisionedThroughput.WriteCapacityUnits=10;

                reqCreateTable.KeySchema = new KeySchema();

                reqCreateTable.KeySchema.HashKeyElement = new KeySchemaElement();
                reqCreateTable.KeySchema.HashKeyElement.AttributeName = "Hash";
                reqCreateTable.KeySchema.HashKeyElement.AttributeType = "S";

                reqCreateTable.KeySchema.RangeKeyElement = new KeySchemaElement();
                reqCreateTable.KeySchema.RangeKeyElement.AttributeName = "RangeKey";
                reqCreateTable.KeySchema.RangeKeyElement.AttributeType = "N";

                resCreateTable = client.CreateTable(reqCreateTable);
                string tablestatus = resCreateTable.CreateTableResult.TableDescription.TableStatus;
                while (tablestatus != "ACTIVE")
                {
                    tablestatus = client.DescribeTable(new DescribeTableRequest()
                                                        .WithTableName(reqCreateTable.TableName))
                                    .DescribeTableResult.Table.TableStatus;
                    System.Threading.Thread.Sleep(5000);

                }
            }
            if (!currentTables.Contains("AcountBaselineCILog"))
            {
                CreateTableRequest reqCreateTable = new CreateTableRequest();
                CreateTableResponse resCreateTable = new CreateTableResponse();

                reqCreateTable.TableName = "AcountBaselineCILog";

                reqCreateTable.ProvisionedThroughput = new ProvisionedThroughput();
                reqCreateTable.ProvisionedThroughput.ReadCapacityUnits = 10;
                reqCreateTable.ProvisionedThroughput.WriteCapacityUnits = 10;

                reqCreateTable.KeySchema = new KeySchema();

                reqCreateTable.KeySchema.HashKeyElement = new KeySchemaElement();
                reqCreateTable.KeySchema.HashKeyElement.AttributeName = "Hash";
                reqCreateTable.KeySchema.HashKeyElement.AttributeType = "S";

                reqCreateTable.KeySchema.RangeKeyElement = new KeySchemaElement();
                reqCreateTable.KeySchema.RangeKeyElement.AttributeName = "RangeKey";
                reqCreateTable.KeySchema.RangeKeyElement.AttributeType = "N";

                resCreateTable = client.CreateTable(reqCreateTable);
                string tablestatus =resCreateTable.CreateTableResult.TableDescription.TableStatus;
                while (tablestatus != "ACTIVE")
                {
                    tablestatus = client.DescribeTable(new DescribeTableRequest()
                                                        .WithTableName(reqCreateTable.TableName))
                                    .DescribeTableResult.Table.TableStatus;
                    System.Threading.Thread.Sleep(5000);
                }
            }

        }

    }
}
