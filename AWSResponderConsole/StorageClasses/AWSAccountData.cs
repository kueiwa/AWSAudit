using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AWSResponderConsole
{
    public class AWSAccountData
    {
        public IdentityAccountManagement IAM { get; set; }
        public Dictionary<string, AWSRegionData> Regions { get; set; }
        public AWSAccountData() { Regions = new Dictionary<string, AWSRegionData>(); }
    }
}
