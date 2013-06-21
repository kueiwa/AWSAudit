using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.DynamoDB.DataModel;

namespace AWSResponderConsole
{
    [DynamoDBTable("AcountBaselineCIs")]
    public class AccountBaselineCI
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
        public string AuditCategory { get; set; }
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
        [DynamoDBProperty]
        public string CCBApproved { get; set; }

        public override string ToString()
        {
            return string.Format(@"AccountID:{0} – Region {1} - Date:{2} ConfigurationItemName:{3} ConfigurationItemClass:{4} ConfigurationItemJSON:{5} User{6}",
                AccountID, Region, Date, ConfigurationItemName, ConfigurationItemClass, ConfigurationItemJSON, User);
        }
    }
}
