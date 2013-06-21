using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ElastiCache
    {
        /// <summary>
        /// A list of CacheClusters
        /// </summary>
        public ListComparisonResults<Amazon.ElastiCache.Model.CacheCluster> CacheClusters { get; set; }
        /// <summary>
        /// A collection of CacheParameterGroups
        /// </summary>
        public ListComparisonResults<Amazon.ElastiCache.Model.CacheParameterGroup> CacheParameterGroups { get; set; }
        /// <summary>
        /// A collection of CacheParameterGroupsParameters
        /// </summary>
        public ListComparisonResults<ElastiCacheParameterGroupParameters> CacheParameterGroupParametersList { get; set; }
        /// <summary>
        /// A collection of CacheSecurityGroup descriptions
        /// </summary>
        public ListComparisonResults<Amazon.ElastiCache.Model.CacheSecurityGroup> CacheSecurityGroups { get; set; }
        /// <summary>
        /// a list of CacheSubnetGroup descriptions
        /// </summary>
        public ListComparisonResults<Amazon.ElastiCache.Model.CacheSubnetGroup> CacheSubnetGroups { get; set; }
        /// <summary>
        /// information about reserved Cache Nodes for this account
        /// </summary>
        public ListComparisonResults<Amazon.ElastiCache.Model.ReservedCacheNode> ReservedCacheNodes { get; set; }


    }
    public class ElastiCacheParameterGroupParameters
    {
        public List<Amazon.ElastiCache.Model.Parameter> Parameters { get; set; }
        public string CacheParameterGroupName { get; set; }
        public ElastiCacheParameterGroupParameters() { }
        public ElastiCacheParameterGroupParameters(string ParameterGroupName, List<Amazon.ElastiCache.Model.Parameter> parameter)
        {
            CacheParameterGroupName = ParameterGroupName;
            Parameters = parameter;
        }
    }

}
