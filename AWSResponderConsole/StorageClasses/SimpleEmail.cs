using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class SimpleEmail
    {
        public ListComparisonResults<string> Identities { get; set; }
        public ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityDkimAttributes>> IdentityDkimAttributes { get; set; }
        public ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityNotificationAttributes>> IdentityNotificationAttributes { get; set; }
        public ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityVerificationAttributes>> IdentityVerificationAttributes { get; set; }
        public ListComparisonResults<Amazon.SimpleEmail.Model.GetSendQuotaResult> SendQuota { get; set; }
        public ListComparisonResults<Amazon.SimpleEmail.Model.SendDataPoint> SendStatistics { get; set; }
    }
}
