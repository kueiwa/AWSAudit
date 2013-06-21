using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class SQSService
    {
        public ListComparisonResults<string> QueueUrl { get; set; }
        public ListComparisonResults<QueueAttributes> QueueAttributes { get; set; }
    }
    public class QueueAttributes
    {
        public List<Amazon.SQS.Model.Attribute> Attributes { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int DelaySeconds { get; set; }
        public DateTime LastModifiedTimestamp { get; set; }
        public int MaximumMessageSize { get; set; }
        public int MessageRetentionPeriod { get; set; }
        public string Policy { get; set; }
        public string QueueARN { get; set; }
        public int VisibilityTimeout { get; set; }
    }
}
