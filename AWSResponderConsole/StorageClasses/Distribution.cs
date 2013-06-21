using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class Distribution
    {
        public Amazon.CloudFront.Model.Distribution DistributionInfo { get; set; }
        public Amazon.CloudFront.Model.DistributionConfig DistributionConfig { get; set; }
        public string DistributionId { get; set; }
        public Amazon.CloudFront.Model.DistributionSummary DistributionSummary { get; set; }

        public Distribution(Amazon.CloudFront.Model.Distribution distribution,
                                     Amazon.CloudFront.Model.DistributionConfig config,
                                     Amazon.CloudFront.Model.DistributionSummary summary,
                                     string id)
        {
            DistributionConfig = config;
            DistributionInfo = distribution;
            DistributionId = id;
            DistributionSummary = summary;
        }
    }

}
