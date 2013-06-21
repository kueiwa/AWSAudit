using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class AuditParams
    {
        public Amazon.Runtime.SessionAWSCredentials AWSCredentials { get; set; }
        public string AccountNumber { get; set; }
        public Amazon.RegionEndpoint AWSRegion { get; set; }
        public DateTime CMBaselineDate { get; set; }
        public CMLocalLibrary CMLibrary { get; set; }
        public bool RequireCCBApproval { get; set; }
        public string AuditCategory { get; set; }
        public AuditParams(Amazon.Runtime.SessionAWSCredentials cred, string accountnumber, Amazon.RegionEndpoint region, DateTime BaselineDate,
                           CMLocalLibrary lib, bool requireCCBApproval)
        {
            AWSCredentials = cred;
            AccountNumber = accountnumber;
            AWSRegion = region;
            CMBaselineDate = BaselineDate;
            CMLibrary = lib;
            RequireCCBApproval = requireCCBApproval;
            AuditCategory = "none";
        }
    }

}
