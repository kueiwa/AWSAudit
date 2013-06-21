using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class OpsWorks
    {
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.OpsWorks.Model.App> Apps { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Command> Commands { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Deployment> Deployments { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.ElasticIp> ElasticIps { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Instance> Instances { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Layer> Layers { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.LoadBasedAutoScalingConfiguration> LoadBasedAutoScalingConfigurations { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Permission> Permissions { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.RaidArray> RaidArrays { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.ServiceError> ServiceErrors { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Stack> Stacks { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.TimeBasedAutoScalingConfiguration> TimeBasedAutoScalingConfigurations { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.UserProfile> UserProfiles { get; set; }
        public ListComparisonResults<Amazon.OpsWorks.Model.Volume> Volumes { get; set; }
    }

}
