using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ElastiBeanstalk
    {
        /// <summary>
        /// A list of applications
        /// </summary>
        public ListComparisonResults<Amazon.ElasticBeanstalk.Model.ApplicationDescription> ApplicationDescriptions { get; set; }
        /// <summary>
        /// A list of application versions
        /// </summary>
        public ListComparisonResults<Amazon.ElasticBeanstalk.Model.ApplicationVersionDescription> ApplicationVersionDescriptions { get; set; }
        /// <summary>
        /// A list of Environment Configurations
        /// </summary>
        public ListComparisonResults<Amazon.ElasticBeanstalk.Model.DescribeConfigurationOptionsResult> EnvironmentConfigurations { get; set; }
        /// <summary>
        /// A list of EnvironmentDescriptions
        /// </summary>
        public ListComparisonResults<Amazon.ElasticBeanstalk.Model.EnvironmentDescription> EnvironmentDescriptions { get; set; }
        /// <summary>
        /// A list of Environment Resources
        /// </summary>
        public ListComparisonResults<Amazon.ElasticBeanstalk.Model.EnvironmentResourceDescription> EnvironmentResources { get; set; }
    }

}
