using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ElasticLoadBalancing
    {
        /// <summary>
        /// A list of LoadBalancerDescriptions
        /// </summary>
        public ListComparisonResults<ElasticLoadBalancerPolicies> ElasticLoadBalancerAndPolicies { get; set; }
    }
    public class ElasticLoadBalancerPolicies
    {
        public List<Amazon.ElasticLoadBalancing.Model.PolicyDescription> PolicyDescriptions { get; set; }
        public Amazon.ElasticLoadBalancing.Model.LoadBalancerDescription LoadBalancerDescription { get; set; }
        public ElasticLoadBalancerPolicies() { }
        public ElasticLoadBalancerPolicies(Amazon.ElasticLoadBalancing.Model.LoadBalancerDescription LoadBalancer,
            List<Amazon.ElasticLoadBalancing.Model.PolicyDescription> pols)
        {
            LoadBalancerDescription = LoadBalancer;
            PolicyDescriptions = pols;
        }
    }
}
