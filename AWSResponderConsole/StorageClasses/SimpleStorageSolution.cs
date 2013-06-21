using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace AWSResponderConsole
{
    public class SimpleStorageSolution
    {
        /// <summary>
        /// Lists the S3 Buckets in the account
        /// </summary>
        public ListComparisonResults<S3Bucket> Buckets { get; set; }
        /// <summary>
        /// This is a list of objects in the bucket that match your search criteria. 
        /// </summary>
        public ListComparisonResults<S3Object> S3Objects { get; set; }
        /// <summary>
        /// This is a list of object versions in the bucket that match your search criteria. 
        /// </summary>
        public ListComparisonResults<S3ObjectVersion> Versions { get; set; }

        public ListComparisonResults<WebsiteConfiguration> WebsiteConfigurations { get; set; }

    }

}
