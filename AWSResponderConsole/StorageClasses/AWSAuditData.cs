using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class AWSAuditData
    {
        public AWSAuditData() { Accounts = new Dictionary<string, AWSAccountData>(); }
        public DateTime Date { get; set; }
        public DateTime BaselineDate { get; set; }
        public string RoleArn { get; set; }
        public Dictionary<string, AWSAccountData> Accounts { get; set; }
    }

}
