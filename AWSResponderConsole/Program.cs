using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlServerCe;
using log4net;
using log4net.Config;
using Amazon;



using System.Web.Script.Serialization;

namespace AWSResponderConsole
{
    static class jsondeserializer
    {
        public static T ToObject<T>(this string obj, int recursionDepth = 100)
        {
            Newtonsoft.Json.JsonSerializerSettings st = new Newtonsoft.Json.JsonSerializerSettings();
            st.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj, st);
        }
    }
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static void printHelp()
        {
            Console.WriteLine("AWSResponderConsole Help");
            Console.WriteLine("This program is configured from the command line");
            Console.WriteLine("");
            Console.WriteLine("AK=value     AWSAccessKey for the account");
            Console.WriteLine("AKS=value    AWSSecretKey for the account");
            Console.WriteLine("MFA=value    MFASerial for the account");
            Console.WriteLine("BL=1Oct12    BaselineDate for the comparison, default is Now()");
            Console.WriteLine("TL=value     TokenLifetime of the session for the audit");
            Console.WriteLine("AR=value     AWSAuditRole to use for the audit, uses the assume role process");
            Console.WriteLine("ER=value     AWSRegionsToExclude from the audit");
            Console.WriteLine("RA=value     AWSRegionsToAudit region:region:region");
            Console.WriteLine("AA=value     AWSAccountsToAudit 00000000000:0000000000");
            Console.WriteLine("RF=value     AuditReportFile ");
            Console.WriteLine("CCB=value    Check for CCB Approval");
            Console.WriteLine("DB=value     AWSConfigurationItemDatabase");
            Console.WriteLine("SI=true     Suppress inputs for account information");
        }
        public static void Main(string[] args)
        {
            #region Setup Logging Classes
                log4net.Config.XmlConfigurator.Configure();
                log4net.ThreadContext.Properties["SessionID"] = Environment.UserDomainName + "\\" + Environment.UserName;
            #endregion
            #region ReadCommandLine
                if (args.Contains("\\?") || args.Contains("\\h") )
            {
                printHelp();
                return;
            }
            AppSettings appset = new AppSettings();
            bool suppressrequestaccountinformation = false;
            if (appset.ApplicationEncryptionCertificateThumbprint==null ||
                (appset.ApplicationEncryptionCertificate==null &&
                !appset.ApplicationEncryptionCertificateThumbprint.ToLower().Equals("StoreclearText")))
                appset.ApplicationEncryptionCertificateThumbprint = appset.SelectCertificate();
            if(appset.CloudCredentials==null)
                appset.CloudCredentials = new AWSCredentials();
            foreach (string setting in args)
            {
                string sw = setting.Split('=')[0];
                string value = "";
                if(setting.Split('=').Count()>0)
                    value = setting.Split('=')[1];
                switch (sw.ToUpper())
                {
                    case "AK":
                        {
                            appset.CloudCredentials.SetAWSAccessKey(value);
                        }
                        break;
                    case "AKS":
                        {
                            appset.CloudCredentials.SetAWSSecretKey(value);
                        }
                        break;
                    case "MFA":
                        {
                            appset.CloudCredentials.SetMFASerial(value);
                        }
                        break;
                    case "BL":
                        {
                            appset.CloudCredentials.SetBaselineDate(value);
                        }
                        break;
                    case "TL":
                        {
                            appset.CloudCredentials.SetTokenLifetime(Convert.ToInt32(value));
                        }
                        break;
                    case "AR":
                        {
                            appset.CloudCredentials.SetAWSAuditRole(value);
                        }
                        break;
                    case "ER":
                        {
                            appset.CloudCredentials.SetAWSRegionsToExclude(value);
                        }
                        break;
                    case "RA":
                        {
                            appset.CloudCredentials.SetAWSRegionsToAudit(value);
                        }
                        break;
                    case "AA":
                        {
                            appset.CloudCredentials.SetAWSAccountsToAudit(value);
                        }
                        break;
                    case "RF":
                        {
                            appset.CloudCredentials.SetAuditReportFile(value);
                        }
                        break;
                    case "CCB":
                        {
                            appset.CloudCredentials.SetRequireCCBApproval(value.ToLower().Equals("false"));
                        }
                        break;
                    case "DB":
                        {
                            appset.CloudCredentials.SetAWSConfigurationItemDatabase(value);
                        }
                        break;
                    case "SI":
                        {
                            suppressrequestaccountinformation = value.ToLower().Equals("true");
                        }
                        break;

                }
            }
            appset.Save();
            appset.Reload();
            #endregion

            AWSAuditData Audit = new AWSAuditData();
            #region Read ConfigFile or User inputs
            if( appset.CloudCredentials.AWSAccessKey== null || appset.CloudCredentials.AWSAccessKey.Length<1)
                appset.CloudCredentials.SetAWSAccessKey(CryptoUtilities.GetConsoleInput("Enter the AWSAccessKey:", "", false));
            string accessKeyIDs = appset.CloudCredentials.AWSAccessKey;
            if (appset.CloudCredentials.AWSSecretKey == null || appset.CloudCredentials.AWSSecretKey.Length < 1)
                appset.CloudCredentials.SetAWSSecretKey(CryptoUtilities.GetConsoleInput("Enter the AWSSecretKey:", "", true));
            string secretAccessKeyID = appset.CloudCredentials.AWSSecretKey;
            if (appset.CloudCredentials.MFASerial == null || appset.CloudCredentials.MFASerial.Length < 1)
                appset.CloudCredentials.SetMFASerial(CryptoUtilities.GetConsoleInput("Enter the MFASerial:", "", false));
            string MFASerialID = appset.CloudCredentials.MFASerial;
            if (appset.CloudCredentials.BaselineDate == null || appset.CloudCredentials.BaselineDate.Length < 1)
                appset.CloudCredentials.SetBaselineDate(CryptoUtilities.GetConsoleInput("Enter the BaselineDate:", "Now()", false));
            string BaselineDateString = appset.CloudCredentials.BaselineDate;
            if (appset.CloudCredentials.TokenLifetime == null || appset.CloudCredentials.TokenLifetime.Length < 1)
                appset.CloudCredentials.SetTokenLifetime(2000);
            int TokenLifetime = int.Parse(appset.CloudCredentials.TokenLifetime);
            if (appset.CloudCredentials.AWSAuditRole == null || appset.CloudCredentials.AWSAuditRole.Length < 1)
                appset.CloudCredentials.SetAWSAuditRole(CryptoUtilities.GetConsoleInput("Enter the AWSAuditRole:", "", false));
            string AuditRoleString = appset.CloudCredentials.AWSAuditRole;
            if (appset.CloudCredentials.AWSRegionsToExclude == null || appset.CloudCredentials.AWSRegionsToExclude.Length < 1)
                appset.CloudCredentials.SetAWSRegionsToExclude(CryptoUtilities.GetConsoleInput("Enter the AWSRegionsToExclude:", "", false));
            string AWSRegionsToExclude = appset.CloudCredentials.AWSRegionsToExclude;
            if (appset.CloudCredentials.AWSRegionsToAudit == null || appset.CloudCredentials.AWSRegionsToAudit.Length < 1)
                appset.CloudCredentials.SetAWSRegionsToAudit(CryptoUtilities.GetConsoleInput("Enter the AWSRegionsToAudit:", "all", false));
            string[] AWSRegionsToAudit = appset.CloudCredentials.AWSRegionsToAudit.Split(":".ToCharArray());
            if (appset.CloudCredentials.AWSAccountsToAudit == null || appset.CloudCredentials.AWSAccountsToAudit.Length < 1)
                appset.CloudCredentials.SetAWSAccountsToAudit(CryptoUtilities.GetConsoleInput("Enter the AWSAccountsToAudit:", "", false));
            string[] AWSAccountsToAudit = appset.CloudCredentials.AWSAccountsToAudit.Split(":".ToCharArray());
            if (appset.CloudCredentials.AuditReportFile == null || appset.CloudCredentials.AuditReportFile.Length < 1)
                appset.CloudCredentials.SetAuditReportFile("Output File {0}");
            string AuditReportFile = appset.CloudCredentials.AuditReportFile; 
            if (appset.CloudCredentials.RequireCCBApproval == null || appset.CloudCredentials.RequireCCBApproval.Length < 1)
                appset.CloudCredentials.SetRequireCCBApproval(true);
            bool RequireCCBApproval = appset.CloudCredentials.RequireCCBApproval.ToLower().Equals("true");
            #endregion

            #region Convert Settings
            Audit.Date = DateTime.Now;
            Audit.BaselineDate = DateTime.UtcNow;
            if (!BaselineDateString.ToLower().Contains("now"))
                Audit.BaselineDate = DateTime.Parse(BaselineDateString);
            Audit.RoleArn = AuditRoleString;
            List<Amazon.RegionEndpoint> regionlist = new List<Amazon.RegionEndpoint>();
            foreach (string region in AWSRegionsToAudit)
            {
                foreach (Amazon.RegionEndpoint endpoint in RegionEndpoint.EnumerableAllRegions)
                {
                    string s = endpoint.SystemName;
                    if ((region.ToLower().Equals("all") && !AWSRegionsToExclude.Contains(endpoint.SystemName))
                        || (endpoint.SystemName.Equals(region) && !AWSRegionsToExclude.Contains(endpoint.SystemName))
                        )
                        regionlist.Add(endpoint);
                }
            }

            #endregion

            #region Setup Utility Classes
            CMLocalLibrary databaseLib = new CMLocalLibrary();
            AWSAuthenticationCredentials AwsAuthenticator = new AWSAuthenticationCredentials();
            AWSAuditActions AuditActions = new AWSAuditActions();
            #endregion
            
            //Login
            Amazon.SecurityToken.Model.Credentials login =
                AwsAuthenticator.GetConsoleLogin(accessKeyIDs, secretAccessKeyID, MFASerialID, TokenLifetime, suppressrequestaccountinformation);
            //save new settings if different than config file
            accessKeyIDs = login.AccessKeyId;
            secretAccessKeyID = login.SecretAccessKey;
            string messagestring = "";
            foreach (string accountnumber in AWSAccountsToAudit)
            {
                //Assume role in the account
                Amazon.Runtime.SessionAWSCredentials cred =
                    AwsAuthenticator.AssumeRole(String.Format(Audit.RoleArn, accountnumber), TokenLifetime, login);
                Audit.Accounts.Add(accountnumber, new AWSAccountData());
                
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                System.Diagnostics.Stopwatch tot = new System.Diagnostics.Stopwatch();

                messagestring = String.Format("Function".PadRight(30) + "Elapsed Seconds".PadRight(17) + "Total".PadLeft(5));
                Console.WriteLine(messagestring);
                sw.Start();
                tot.Start();
                CMLocalLibrary lib = new CMLocalLibrary();
                lib.Init();
                AuditParams audit_params = new AuditParams(cred, accountnumber, null, Audit.BaselineDate, new CMLocalLibrary(), RequireCCBApproval);

                messagestring = String.Format("{0}{1}{2}", "CMLocalLibrary.Init()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
                Console.WriteLine(messagestring);
                log.Info(messagestring);
                sw.Restart();
                messagestring = String.Format("Beginning Audit of account number {0}", accountnumber);
                log.Info(messagestring);
                Console.WriteLine(messagestring);
                messagestring = String.Format("======================================================================");
                Console.WriteLine(messagestring);

                Audit.Accounts[accountnumber].IAM = AuditActions.ReadIAM(audit_params);
                messagestring = String.Format("{0}{1}{2}", "ReadIAM()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
                log.Info(messagestring);
                Console.WriteLine(messagestring);
                sw.Restart();
                foreach (Amazon.RegionEndpoint region in regionlist)
                {
                    audit_params.AWSRegion = region;
                    messagestring = String.Format("Audit of {0} region ", region.DisplayName);
                    log.Info(messagestring);
                    Console.WriteLine(messagestring);
                    messagestring = String.Format("======================================================================");
                    Console.WriteLine(messagestring);
                    Audit.Accounts[accountnumber].Regions.Add(region.DisplayName, AuditRegion(tot, audit_params));
                }
                AuditReport report = new AuditReport();
                report.CPEList(Audit, true, String.Format(AuditReportFile, DateTime.UtcNow.ToString("yyy.MM.dd")));

                report.SummaryReport(Audit, true, String.Format(AuditReportFile, DateTime.UtcNow.ToString("yyy.MM.dd")));
                report.CvsReport(Audit, true, String.Format(AuditReportFile + ".cvs", DateTime.UtcNow.ToString("yyy.MM.dd")));
                report.RulesList(Audit, true, String.Format(AuditReportFile + ".rule", DateTime.UtcNow.ToString("yyy.MM.dd")));

                
                messagestring = "";
                Console.WriteLine(messagestring);
                messagestring = "";
                Console.WriteLine(messagestring);
                Console.Write("press any key...");
                Console.Read();
            }
        }

        private static AWSRegionData AuditRegion(System.Diagnostics.Stopwatch tot, AuditParams audit_params)
        {
            AWSAuditActions AuditActions = new AWSAuditActions();
            AWSRegionData regionData = new AWSRegionData();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            string messagestring = "";
            regionData.AS =
                AuditActions.ReadAutoScaling(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadAutoScaling()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.CF =
                AuditActions.ReadCloudFront(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadCloudFront()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.CS =
                AuditActions.ReadCloudSearch(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadCloudSearch()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.CW =
                AuditActions.ReadCloudWatch(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadCloudWatch()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.DP =
                AuditActions.ReadDataPipeline(audit_params);

            messagestring = String.Format("{0}{1}{2}", "ReadDataPipeline()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.DyDB =
                AuditActions.ReadDynamoDB(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadDynamoDB()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), (tot.Elapsed.TotalSeconds).ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.DC =
                AuditActions.ReadDC(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadDC()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.EC2 =
                AuditActions.ReadECC(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadECC()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.EC =
                AuditActions.ReadElastiCache(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadElastiCache()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.EBS =
                AuditActions.ReadElastiBeanstalk(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadElastiBeanstalk()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();
            regionData.ELB =
                AuditActions.ReadElasticLoadBalancing(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadElasticLoadBalancing()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.EMR =
                AuditActions.ReadElasticMapReduce(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadElasticMapReduce()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.ET =
                AuditActions.ReadElasticTranscoder(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadElasticTranscoder()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.G =
                AuditActions.ReadGlacier(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadGlacier()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.IE =
                AuditActions.ReadImportExport(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadImportExport()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.OW =
                AuditActions.ReadOpsWorks(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadOpsWorks()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.R53 =
                AuditActions.ReadRoute53(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadRoute53()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.RDS =
                AuditActions.ReadRDS(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadRDS()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.RS =
                AuditActions.ReadRedshift(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadRedshift()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.SE =
                AuditActions.ReadSimpleEmail(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadSimpleEmail()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.SNS =
                AuditActions.ReadSimpleNotificationService(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadSimpleNotificationService()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.SW =
                AuditActions.ReadSimpleWorkflow(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadSimpleWorkflow()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            sw.Restart();

            regionData.SQS =
                AuditActions.ReadSQSService(audit_params);
            messagestring = String.Format("{0}{1}{2}", "ReadSQSService()".PadRight(30), sw.Elapsed.TotalSeconds.ToString().PadRight(17), tot.Elapsed.TotalSeconds.ToString().PadLeft(5));
            log.Info(messagestring);
            Console.WriteLine(messagestring);
            return regionData;
        }
 
        //https://s3.amazonaws.com/cloudformation-samples-us-east-1/AWSCloudFormer.template
        /*   //TODO:  This the Audit reads the classes, can it create a Template to allow a copy command or just create write functions?
{
                      "AWSTemplateFormatVersion": "2010-09-09",
                      "Resources": {
                        "eip10723138252": {
                          "Type": "AWS::EC2::EIP",
                          "Properties": {
                            "InstanceId": {
                              "Ref": "instancei902610ee"
                            }
                          }
                        },
                        "instancei902610ee": {
                          "Type": "AWS::EC2::Instance",
                          "Properties": {
                            "AvailabilityZone": "us-east-1c",
                            "DisableApiTermination": "FALSE",
                            "ImageId": "ami-f540c29c",
                            "InstanceType": "t1.micro",
                            "KeyName": "VPC_Keypair",
                            "Monitoring": "false",
                            "SubnetId": "subnet-da9339b0",
                            "PrivateIpAddress": "10.0.0.181"
                          }
                        },
                        "instanceidc95d1af": {
                          "Type": "AWS::EC2::Instance",
                          "Properties": {
                            "AvailabilityZone": "us-east-1c",
                            "DisableApiTermination": "FALSE",
                            "ImageId": "ami-3fec7956",
                            "InstanceType": "t1.micro",
                            "KernelId": "aki-88aa75e1",
                            "KeyName": "LinuxKeyPair",
                            "Monitoring": "false",
                            "SubnetId": "subnet-da9339b0",
                            "PrivateIpAddress": "10.0.0.45"
                          }
                        },
                        "instancei92b0efe1": {
                          "Type": "AWS::EC2::Instance",
                          "Properties": {
                            "AvailabilityZone": "us-east-1d",
                            "DisableApiTermination": "FALSE",
                            "ImageId": "ami-7539b41c",
                            "InstanceType": "m1.medium",
                            "KernelId": "aki-825ea7eb",
                            "KeyName": "UbuntuServer",
                            "Monitoring": "false",
                            "SecurityGroups": [
                              {
                                "Ref": "sgubuntuServSG"
                              }
                            ]
                          }
                        },
                        "instancei526ac637": {
                          "Type": "AWS::EC2::Instance",
                          "Properties": {
                            "AvailabilityZone": "us-east-1c",
                            "DisableApiTermination": "FALSE",
                            "ImageId": "ami-54cf5c3d",
                            "InstanceType": "t1.micro",
                            "KernelId": "aki-88aa75e1",
                            "KeyName": "LinuxKeyPair",
                            "Monitoring": "false",
                            "SubnetId": "subnet-da9339b0",
                            "PrivateIpAddress": "10.0.0.90"
                          }
                        },
                        "instancei23409541": {
                          "Type": "AWS::EC2::Instance",
                          "Properties": {
                            "AvailabilityZone": "us-east-1d",
                            "DisableApiTermination": "FALSE",
                            "ImageId": "ami-9f72f1f6",
                            "InstanceType": "t1.micro",
                            "KernelId": "aki-805ea7e9",
                            "Monitoring": "false",
                            "SecurityGroups": [
                              {
                                "Ref": "sgAWSCloudFormerInstanceSecurityGroupGGFN61N0OV46"
                              }
                            ]
                          }
                        },
                        "s3AuditingBucket": {
                          "Type": "AWS::S3::Bucket"
                        },
                        "s3ProgrammingBucket": {
                          "Type": "AWS::S3::Bucket"
                        },
                        "s3cftemplates15iftwe2wk3m8useast1": {
                          "Type": "AWS::S3::Bucket"
                        },
                        "sgAWSCloudFormerInstanceSecurityGroupGGFN61N0OV46": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "Enable Access to Rails application via port 80",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "80",
                                "ToPort": "80",
                                "CidrIp": "0.0.0.0/0"
                              }
                            ]
                          }
                        },
                        "sgubuntuServSG": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "ubuntuServSG",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "22",
                                "ToPort": "22",
                                "CidrIp": "0.0.0.0/0"
                              }
                            ]
                          }
                        },
                        "sgdefault": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "default VPC security group",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "-1",
                                "FromPort": "",
                                "ToPort": ""
                              }
                            ]
                          }
                        },
                        "sgDMZ2": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "DMZ_2",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "80",
                                "ToPort": "80",
                                "CidrIp": "0.0.0.0/0"
                              },
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "1433",
                                "ToPort": "1433",
                                "CidrIp": "0.0.0.0/0"
                              },
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "3389",
                                "ToPort": "3389",
                                "CidrIp": "0.0.0.0/0"
                              }
                            ]
                          }
                        },
                        "sgWebServerSG": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "Web Server SG",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "22",
                                "ToPort": "22",
                                "CidrIp": "76.73.215.0/24"
                              },
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "80",
                                "ToPort": "80",
                                "CidrIp": "0.0.0.0/0"
                              },
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "443",
                                "ToPort": "443",
                                "CidrIp": "0.0.0.0/0"
                              },
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "3389",
                                "ToPort": "3389",
                                "CidrIp": "0.0.0.0/0"
                              }
                            ]
                          }
                        },
                        "sgquickstart1": {
                          "Type": "AWS::EC2::SecurityGroup",
                          "Properties": {
                            "GroupDescription": "quick-start-1",
                            "SecurityGroupIngress": [
                              {
                                "IpProtocol": "tcp",
                                "FromPort": "22",
                                "ToPort": "22",
                                "CidrIp": "0.0.0.0/0"
                              }
                            ]
                          }
                        },
                        "dbsgdefault": {
                          "Type": "AWS::RDS::DBSecurityGroup",
                          "Properties": {
                            "GroupDescription": "default"
                          }
                        },
                        "ingress1": {
                          "Type": "AWS::EC2::SecurityGroupIngress",
                          "Properties": {
                            "GroupName": {
                              "Ref": "sgdefault"
                            },
                            "IpProtocol": "tcp",
                            "FromPort": "0",
                            "ToPort": "65535",
                            "SourceSecurityGroupName": {
                              "Ref": "sgdefault"
                            },
                            "SourceSecurityGroupOwnerId": "123456789012"
                          }
                        },
                        "ingress2": {
                          "Type": "AWS::EC2::SecurityGroupIngress",
                          "Properties": {
                            "GroupName": {
                              "Ref": "sgdefault"
                            },
                            "IpProtocol": "udp",
                            "FromPort": "0",
                            "ToPort": "65535",
                            "SourceSecurityGroupName": {
                              "Ref": "sgdefault"
                            },
                            "SourceSecurityGroupOwnerId": "123456789012"
                          }
                        },
                        "ingress3": {
                          "Type": "AWS::EC2::SecurityGroupIngress",
                          "Properties": {
                            "GroupName": {
                              "Ref": "sgdefault"
                            },
                            "IpProtocol": "icmp",
                            "FromPort": "-1",
                            "ToPort": "-1",
                            "SourceSecurityGroupName": {
                              "Ref": "sgdefault"
                            },
                            "SourceSecurityGroupOwnerId": "123456789012"
                          }
                        }
                      },
                      "Description": "This is my sample template"
                    }

         */

    }
}