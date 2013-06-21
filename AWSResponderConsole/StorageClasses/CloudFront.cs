using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class CloudFront
    {

        /// <summary>
        /// A complex type that contains one InvalidationSummary element for each invalidation batch that was created by the current AWS account.
        /// </summary>
        public ListComparisonResults<Amazon.CloudFront.Model.Invalidation> InvalidationList { get; set; }
        /// <summary>
        /// The streaming distribution's configuration information. 
        /// </summary>
        public ListComparisonResults<StreamingDistribution> StreamingDistributions { get; set; }
        /// <summary>
        /// The distribution's configuration information. 
        /// </summary>
        public ListComparisonResults<Distribution> Distributions { get; set; }
        /// <summary>
        /// The distribution's configuration information. 
        /// </summary>
        public ListComparisonResults<OriginAccessIdentity> OriginAccessIdentities { get; set; }
    }
    public class OriginAccessIdentity
    {
        public Amazon.CloudFront.Model.CloudFrontOriginAccessIdentity OriginAccessIdentityInfo { get; set; }
        public string OriginAccessIdentityId { get; set; }
        public Amazon.CloudFront.Model.CloudFrontOriginAccessIdentitySummary OriginAccessIdentitySummary { get; set; }

        public OriginAccessIdentity(Amazon.CloudFront.Model.CloudFrontOriginAccessIdentity ident,
                                     Amazon.CloudFront.Model.CloudFrontOriginAccessIdentitySummary summary,
                                     string id)
        {
            OriginAccessIdentityInfo = ident;
            OriginAccessIdentityId = id;
            OriginAccessIdentitySummary = summary;
        }
    }
    public class StreamingDistribution
    {
        public Amazon.CloudFront.Model.StreamingDistributionConfig StreamingDistributionConfig { get; set; }
        public string StreamingDistributionId { get; set; }
        public Amazon.CloudFront.Model.StreamingDistributionSummary StreamingDistributionSummary { get; set; }
        public Amazon.CloudFront.Model.StreamingDistribution StreamingDistributionInfo { get; set; }

        public StreamingDistribution(Amazon.CloudFront.Model.StreamingDistribution distribution,
                                     Amazon.CloudFront.Model.StreamingDistributionConfig config,
                                     Amazon.CloudFront.Model.StreamingDistributionSummary summary,
                                     string id)
        {

            StreamingDistributionConfig = config;
            StreamingDistributionInfo = distribution;
            StreamingDistributionId = id;
            StreamingDistributionSummary = summary;
        }
    }

}
