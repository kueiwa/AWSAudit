using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class DirectConnect
    {
        /// <summary>
        /// A connection represents the physical network connection between the Direct Connect location and the customer.
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.Connection> Connections { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.DescribeConnectionDetailResult> ConnectionDetails { get; set; }

        /// <summary>
        /// Detailed information about an offering, including basic offering information plus order steps. 
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.DescribeOfferingDetailResult> OfferingDetails { get; set; }
        /// <summary>
        /// A list of offerings.
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.Offering> Offerings { get; set; }
        /// <summary>
        /// A list of virtual gateways. 
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.VirtualGateway> VirtualGateways { get; set; }
        /// <summary>
        /// A list of virtual interfaces. 
        /// </summary>
        public ListComparisonResults<Amazon.DirectConnect.Model.VirtualInterface> VirtualInterfaces { get; set; }


    }

}
