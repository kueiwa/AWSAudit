using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class Glacier
    {
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.Glacier.Model.GlacierJobDescription> Jobs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.Glacier.Model.UploadListElement> MultiPartUploads { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.Glacier.Model.DescribeVaultOutput> Vaults { get; set; }

    }

}
