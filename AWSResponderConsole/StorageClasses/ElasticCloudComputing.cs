using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.EC2;
using Amazon.EC2.Model;
namespace AWSResponderConsole
{
    public class ElasticCloudComputing
    {
        /// <summary>
        /// A static public IP address designed for dynamic cloud computing. Any elastic IP addresses that you associate with your account remains associated with your account until you explicitly release them.
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Address> Address { get; set; }
        /// <summary>
        /// Gets and sets the AvailabilityZone property
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.AvailabilityZone> AvailabilityZones { get; set; }
        /// <summary>
        /// Gets and sets the BundleTask property. Bundle Task
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.BundleTask> BundleTasks { get; set; }
        /// <summary>
        /// Gets and sets the ConversionTasks property.
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.ConversionTaskType> ConversionTasks { get; set; }
        /// <summary>
        /// List of customer gateways 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.CustomerGateway> CustomerGateway { get; set; }
        /// <summary>
        /// List of DHCP options 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.DhcpOptions> DhcpOptions { get; set; }
        /// <summary>
        /// Gets and sets the details of the created ExportVM tasks.
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.ExportTaskType> ExportTasks { get; set; }
        /// <summary>
        /// Gets and sets the Image property. List of AMIs 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Image> Image { get; set; }
        /// <summary>
        /// Gets and sets the ImageAttruibutes property. List of AMIs 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.ImageAttribute> ImageAttributes { get; set; }
        /// <summary>
        ///  List of instance attributes. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.InstanceAttribute> InstanceAttributes { get; set; }
        /// <summary>
        /// Information about the instance status. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.InstanceStatus> InstanceStatus { get; set; }
        /// <summary>
        /// Gets and sets the InternetGateways property. A list of Internet Gateways. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.InternetGateway> InternetGateways { get; set; }
        /// <summary>
        /// Gets and sets the KeyPair property. List of key pairs 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.KeyPair> KeyPair { get; set; }
        /// <summary>
        /// Gives you information about the network ACLs in your VPC. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.NetworkAcl> NetworkAcls { get; set; }
        /// <summary>
        /// Gets and sets the NetworkInterface property 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.NetworkInterface> NetworkInterface { get; set; }
        /// <summary>
        /// Gets and sets the NetworkInterface property 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.NetworkInterfaceAttribute> NetworkInterfaceAttribute { get; set; }
        /// <summary>
        /// Gets and sets the NetworkInterfacePrivateIpAddress property 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.NetworkInterfacePrivateIpAddress> NetworkInterfacePrivateIpAddress { get; set; }
        /// <summary>
        ///  Contains information about the specified PlacementGroups.
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.PlacementGroupInfo> PlacementGroupInfo { get; set; }
        /// <summary>
        /// List of regions 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Region> Region { get; set; }
        /// <summary>
        /// Reservations
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Reservation> Reservations { get; set; }
        public ListComparisonResults<ReservedInstancesOffering> ReservedInstancesOffering { get; set; }
        /// <summary>
        /// Reservations
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.RunningInstance> RunningInstances { get; set; }
        /// <summary>
        /// Gets and sets the ReservedInstances property. List of reserved instances 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.ReservedInstances> ReservedInstances { get; set; }
        /// <summary>
        /// Gets and sets the RouteTables property. A list of route tables. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.RouteTable> RouteTables { get; set; }
        /// <summary>
        /// Gets and sets the SecurityGroup property. List of security groups 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.SecurityGroup> SecurityGroup { get; set; }
        /// <summary>
        /// Gets and sets the SnapshotAttribute property. list of snapshot attributes 
        /// </summary>
        public ListComparisonResults<SnapshotAttribute> SnapshotAttributes { get; set; }
        /// <summary>
        /// Gets and sets the Snapshot property. List of snapshots 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Snapshot> Snapshot { get; set; }
        /// <summary>
        /// Gets and sets the SpotDatafeedSubscription property. The description of the Spot Datafeed subsciption. 
        /// </summary>
        public ListComparisonResults<SpotDatafeedSubscription> SpotDatafeedSubscription { get; set; }
        /// <summary>
        ///  A list of the SpotInstanceRequest objects returned by the service. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.SpotInstanceRequest> SpotInstanceRequest { get; set; }
        /// <summary>
        /// Gets and sets the SpotPriceHistory property. List of data points specifying the Spot Price history. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.SpotPriceHistory> SpotPriceHistory { get; set; }
        /// <summary>
        /// List of subnets 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Subnet> Subnet { get; set; }
        /// <summary>
        /// List of volumes 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Volume> Volume { get; set; }
        /// <summary>
        /// Status of requested volumes. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.DescribeVolumeAttributeResult> VolumeAttributes { get; set; }
        /// <summary>
        /// Status of requested volumes. 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.VolumeStatus> VolumeStatus { get; set; }
        /// <summary>
        /// Gets and sets the Vpc property. List of VPCs 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.Vpc> Vpc { get; set; }
        /// <summary>
        /// Gets and sets the VpcAttribute property. List of VpcAttributes 
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.DescribeVpcAttributeResult> VpcAttributes { get; set; }
        /// <summary>
        /// Gets and sets the VpnConnection property. List of vpn connections
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.VpnConnection> VpnConnections { get; set; }
        /// <summary>
        /// Gets and sets the VpnGateway property. List of Vpn Gateways
        /// </summary>
        public ListComparisonResults<Amazon.EC2.Model.VpnGateway> VpnGateways { get; set; }
    }

}
