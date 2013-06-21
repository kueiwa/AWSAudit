using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class IdentityAccountManagement
    {
        /// <summary>
        /// The AccessKey data type contains information about an AWS access key.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.AccessKey> AccessKey { get; set; }
        /// <summary>
        /// The AccessKey data type contains information about an AWS access key, without its secret key.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.AccessKeyMetadata> AccessKeyMetadata { get; set; }
        /// <summary>
        /// Contains the result of a successful invocation of the GetGroup action. Contains information about a group to include the Group class and list of users.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.GetGroupResult> GroupMetadata { get; set; }
        /// <summary>
        /// Contains the result of a successful invocation of the GetGroupPolicy action.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.GetGroupPolicyResult> GroupPolicyMetadata { get; set; }
        /// <summary>
        /// Contains the result of a successful invocation of the GetUserPolicy action.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.GetUserPolicyResult> UserPolicyMetadata { get; set; }
        /// <summary>
        /// Contains the result of a successful invocation of the GetRolePolicyResponse action.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.GetRolePolicyResult> GetRolePolicyResult { get; set; }
        /// <summary>
        /// The list of groups
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.Group> Groups { get; set; }
        /// <summary>
        /// The InstanceProfile data type contains information about an instance profile.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.InstanceProfile> InstanceProfiles { get; set; }
        /// <summary>
        /// The LoginProfile data type contains the user name and password create date for a user.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.LoginProfile> LoginProfiles { get; set; }
        /// <summary>
        /// The MFADevice data type contains information about an MFA device.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.MFADevice> MFADevices { get; set; }
        /// <summary>
        /// Password Policy 
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.PasswordPolicy> PasswordPolicy { get; set; }
        /// <summary>
        /// The Role data type contains information about a role.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.Role> Roles { get; set; }
        /// <summary>
        /// The ServerCertificate data type contains information about a server certificate.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.ServerCertificate> ServerCertificates { get; set; }
        /// <summary>
        /// ServerCertificateMetadata contains information about a server certificate without its certificate body, certificate chain, and private key.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.ServerCertificateMetadata> ServerCertificateMetadata { get; set; }
        /// <summary>
        /// The SigningCertificate data type contains information about an X.509 signing certificate.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.SigningCertificate> SigningCertificates { get; set; }
        /// <summary>
        /// A set of key value pairs containing account-level information.
        /// </summary>
        public ListComparisonResults<System.Collections.Generic.Dictionary<string, int>> SummaryMap { get; set; }
        /// <summary>
        /// The User data type contains information about a user.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.User> Users { get; set; }
        /// <summary>
        /// The VirtualMFADevice data type contains information about a virtual MFA device.
        /// </summary>
        public ListComparisonResults<Amazon.IdentityManagement.Model.VirtualMFADevice> VirtualMFADevices { get; set; }

    }

}
