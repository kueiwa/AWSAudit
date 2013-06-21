using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ElasticTranscoder
    {
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.ElasticTranscoder.Model.Job> Jobs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.ElasticTranscoder.Model.Preset> Presets { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.ElasticTranscoder.Model.Pipeline> Pipelines { get; set; }

    }

}
