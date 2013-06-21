using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class RelationalDatabaseSystem
    {
        public ListComparisonResults<Amazon.RDS.Model.DBEngineVersion> DBEngineVersions;
        public ListComparisonResults<Amazon.RDS.Model.DBInstance> DBInstances;
        public ListComparisonResults<DBGroupParameters> DBGroupParameters;
        public ListComparisonResults<Amazon.RDS.Model.DBSecurityGroup> DBSecurityGroups;
        public ListComparisonResults<Amazon.RDS.Model.DBSnapshot> DBSnapshots;
        public ListComparisonResults<Amazon.RDS.Model.DBSubnetGroup> DBSubnetGroups;
        public ListComparisonResults<DefaultDBGroupParameters> DefaultDBGroupParameters;
        public ListComparisonResults<Amazon.RDS.Model.EventCategoriesMap> EventCategoriesMaps;
        public ListComparisonResults<Amazon.RDS.Model.Event> Events;
        public ListComparisonResults<Amazon.RDS.Model.EventSubscription> EventSubscriptions;
        public ListComparisonResults<Amazon.RDS.Model.OptionGroup> OptionGroups;
        public ListComparisonResults<Amazon.RDS.Model.OptionGroupOption> OptionGroupOptions;
        public ListComparisonResults<Amazon.RDS.Model.OrderableDBInstanceOption> OrderableDBInstanceOptions;
        public ListComparisonResults<Amazon.RDS.Model.ReservedDBInstance> ReservedDBInstances;
        public ListComparisonResults<Amazon.RDS.Model.ReservedDBInstancesOffering> ReservedDBInstancesOfferings;
    }
    public class DBGroupParameters
    {
        public Amazon.RDS.Model.DBParameterGroup group { get; set; }
        public List<Amazon.RDS.Model.Parameter> Parameters { get; set; }
        public DBGroupParameters(Amazon.RDS.Model.DBParameterGroup g, List<Amazon.RDS.Model.Parameter> p)
        {
            group = g;
            Parameters = p;
        }
    }
    public class DefaultDBGroupParameters
    {
        public string DBParameterGroupFamily { get; set; }
        public List<Amazon.RDS.Model.Parameter> Parameters { get; set; }
        public DefaultDBGroupParameters(String name, List<Amazon.RDS.Model.Parameter> p)
        {
            DBParameterGroupFamily = name;
            Parameters = p;
        }
    }

}
