using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class Route53
    {
        /// <summary>
        /// A list of HostedZones
        /// </summary>
        public ListComparisonResults<Amazon.Route53.Model.HostedZone> HostedZones { get; set; }
        /// <summary>
        /// A list of ResourceRecordSets
        /// </summary>
        public ListComparisonResults<Amazon.Route53.Model.ResourceRecordSet> ResourceRecordSets { get; set; }

    }

}
