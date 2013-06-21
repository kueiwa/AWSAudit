using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class SimpleNotificationService
    {
        public ListComparisonResults<Amazon.SimpleNotificationService.Model.Subscription> Subscriptions { get; set; }
        public ListComparisonResults<SNSSubscriptionAttribute> SubscriptionAttributes { get; set; }
        public ListComparisonResults<Amazon.SimpleNotificationService.Model.Topic> Topics { get; set; }
        public ListComparisonResults<SNSTopicAttribute> TopicAttributes { get; set; }
    }
    public class SNSTopicAttribute
    {
        public string TopicArn { get; set; }
        public List<Amazon.SimpleNotificationService.Model.TopicAttribute> Attributes { get; set; }
        public SNSTopicAttribute(string arn, List<Amazon.SimpleNotificationService.Model.TopicAttribute> att)
        {
            TopicArn = arn;
            Attributes = att;
        }
    }
    public class SNSSubscriptionAttribute
    {
        public string SubscriptionArn { get; set; }
        public List<Amazon.SimpleNotificationService.Model.SubscriptionAttribute> Attributes { get; set; }
        public SNSSubscriptionAttribute(string arn, List<Amazon.SimpleNotificationService.Model.SubscriptionAttribute> att)
        {
            SubscriptionArn = arn;
            Attributes = att;
        }
    }
}
