using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ElasticMapReduce
    {
        /// <summary>
        /// Only contains job flows created within the last two months are returned.
        /// </summary>
        public ListComparisonResults<Amazon.ElasticMapReduce.Model.JobFlowDetail> JobFlows { get; set; }

    }

}
