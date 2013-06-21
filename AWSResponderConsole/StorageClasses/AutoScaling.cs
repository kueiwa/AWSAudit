using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class AutoScaling
    {

        /// <summary>
        /// A list of specific policy adjustment types. 
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.AdjustmentType> AdjustmentTypes { get; set; }
        /// <summary>
        /// A list of Auto Scaling groups. 
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.AutoScalingGroup> AutoScalingGroups { get; set; }
        /// <summary>
        /// A list of Auto Scaling groups. 
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.AutoScalingInstanceDetails> AutoScalingInstances { get; set; }
        /// <summary>
        /// A list of AutoScalingNotification Types
        /// </summary>
        public ListComparisonResults<string> AutoScalingNotificationTypes { get; set; }
        /// <summary>
        /// A list of launch configurations. 
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.LaunchConfiguration> LaunchConfigurations { get; set; }
        ///
        ///The list of notification configurations. 
        ///
        public ListComparisonResults<Amazon.AutoScaling.Model.NotificationConfiguration> NotificationConfigurations { get; set; }
        /// <summary>
        /// List of scaling policies
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.ScalingPolicy> ScalingPolicies { get; set; }
        /// <summary>
        /// A list of ProcessType names.
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.ProcessType> Processes { get; set; }
        /// <summary>
        /// A list of Tags
        /// </summary>
        public ListComparisonResults<Amazon.AutoScaling.Model.TagDescription> Tags { get; set; }
        /// <summary>
        /// A list of Termination Policy Types 
        /// </summary>
        public ListComparisonResults<string> TerminationPolicyTypes { get; set; }



    }
}
