using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.DynamoDB.DataModel;

namespace AWSResponderConsole
{
    [DynamoDBTable("AcountBaselineCILog")]
    public class AccountBaselineCILog
    {
        [DynamoDBHashKey]
        public string Hash { get; set; }
        [DynamoDBRangeKey]
        public long RangeKey { get; set; }
        [DynamoDBProperty]
        public DateTime Date { get; set; }
        [DynamoDBProperty]
        public string AccountID { get; set; }
        [DynamoDBProperty]
        public long NewAccountBaselineCIRangeKey { get; set; }
        [DynamoDBProperty]
        public long OldAccountBaselineCIRangeKey { get; set; }
        [DynamoDBProperty]
        public string ChangeType { get; set; }
        [DynamoDBProperty]
        public string ConfigurationItemName { get; set; }
        [DynamoDBProperty]
        public string ConfigurationItemClass { get; set; }
        [DynamoDBProperty]
        public string ConfigurationItemJSON { get; set; }
        [DynamoDBProperty]
        public string User { get; set; }
        [DynamoDBProperty]
        public string Region { get; set; }

        public override string ToString()
        {
            return string.Format(@"AccountID:{0} – Region{1} - Date:{2} ChangeType:{3} ConfigurationItemName:{4} ConfigurationItemClass:{5} ConfigurationItemJSON:{6} User{7}",
                AccountID, Region, Date, ChangeType, ConfigurationItemName, ConfigurationItemClass, ConfigurationItemJSON, User);
        }
    }
}
