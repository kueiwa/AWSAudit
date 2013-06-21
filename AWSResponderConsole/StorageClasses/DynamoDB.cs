using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class DynamoDB
    {
        /// <summary>
        /// A list of table names
        /// </summary>
        public ListComparisonResults<string> TableNames { get; set; }
        /// <summary>
        /// A collection of table information
        /// </summary>
        public ListComparisonResults<Amazon.DynamoDB.Model.TableDescription> TableDescriptions { get; set; }
    }

}
