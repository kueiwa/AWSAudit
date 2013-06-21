using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class Redshift
    {
        public ListComparisonResults<Amazon.Redshift.Model.Cluster> Clusters { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ClusterParameterGroup> ClusterParameterGroups { get; set; }
        public ListComparisonResults<ClusterGroupParameters> ClusterParameters { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ClusterSecurityGroup> ClusterSecurityGroups { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.Snapshot> Snapshots { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ClusterSubnetGroup> ClusterSubnetGroups { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ClusterVersion> ClusterVersions { get; set; }
        public ListComparisonResults<ClusterGroupParameters> DefaultClusterParameterGroups { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.Event> Events { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.OrderableClusterOption> OrderableClusterOptions { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ReservedNodeOffering> ReservedNodeOfferings { get; set; }
        public ListComparisonResults<Amazon.Redshift.Model.ReservedNode> ReservedNodes { get; set; }
        public ListComparisonResults<ClusterResizeData> ClusterResizedData { get; set; }

    }
    public class ClusterGroupParameters
    {
        public List<Amazon.Redshift.Model.Parameter> Parameters { get; set; }
        public string ParameterGroupName { get; set; }
        public ClusterGroupParameters(string parameterGroupName, List<Amazon.Redshift.Model.Parameter> param)
        {
            ParameterGroupName = parameterGroupName;
            Parameters = param;
        }
    }
    public class ClusterResizeData
    {
        public string ClusterIdentifier { get; set; }
        public Amazon.Redshift.Model.DescribeResizeResult DescribeResize { get; set; }
        public ClusterResizeData(string clusterID, Amazon.Redshift.Model.DescribeResizeResult resizeData)
        {
            ClusterIdentifier = clusterID;
            DescribeResize = resizeData;
        }
    }
}
