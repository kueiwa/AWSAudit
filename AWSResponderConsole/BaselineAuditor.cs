using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

using System.Data.SqlServerCe;

namespace AWSResponderConsole
{
    public class BaselineAuditor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaselineAuditor()
        {
            log4net.ThreadContext.Properties["SessionID"] = Environment.UserDomainName + "\\" + Environment.UserName;
            log4net.Config.XmlConfigurator.Configure();
        }
        public static string ToJSON(object obj)
        {
            return ToJSON(obj, false);
        }
        public static string ToJSON(object obj, bool pretty)
        {
            Newtonsoft.Json.JsonSerializerSettings st = new Newtonsoft.Json.JsonSerializerSettings();
            st.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            st.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
            st.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            if (pretty)
                st.Formatting = Newtonsoft.Json.Formatting.Indented;
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, st);
        }

        public ListComparisonResults<T> CheckCIBaseline<T>(AuditParams audit_params, List<T> newvalue, string AuditCategory)
        {

            ListComparisonResults<T> userChanges = new ListComparisonResults<T>();
            //Get the old user list
            if (newvalue != null)
            {
                AccountBaselineCI oldbaseline = GetNewestFromAccountBaselineCI(audit_params.AccountNumber,
                                            audit_params.AWSRegion != null ? audit_params.AWSRegion.DisplayName : "none",
                                            newvalue.GetType().GetProperty("Item").PropertyType.ToString(), audit_params.AWSCredentials,
                                            audit_params.CMBaselineDate, audit_params.CMLibrary, audit_params.RequireCCBApproval);
                List<T> oldvalue = new List<T>();
                //there is no old value, add this one
                long oldRangeKey = -1;
                if (oldbaseline != null)
                {
                    //TODO: if T = Amazon.DirectConnect.Model.DescribeOfferingDetailResult there's an error
                    oldvalue = oldbaseline.ConfigurationItemJSON.ToObject<List<T>>();
                    oldRangeKey = oldbaseline.RangeKey;
                }
                //compare to current
                userChanges = ListCompare<T>.CompareLists<T>(oldvalue, newvalue);
                //If different save log and CI
                if (userChanges.Additions.Count > 0 || userChanges.Deletions.Count > 0)
                {

                    long newBaselineRangeKey = AddObjectToAccountBaselineCI(audit_params.AccountNumber, newvalue, audit_params.AuditCategory,
                                                                            audit_params.AWSRegion != null ? audit_params.AWSRegion.DisplayName : "none",
                                                                            audit_params.AWSCredentials, audit_params.CMLibrary);
                    long range = AddChangesToAccountBaselineCILog(audit_params.AccountNumber, newBaselineRangeKey,
                                                                  oldRangeKey,
                                                                  audit_params.AWSRegion != null ? audit_params.AWSRegion.DisplayName : "none",
                                                                  "Additions", userChanges.Additions, audit_params.AWSCredentials, audit_params.CMLibrary);
                    range = AddChangesToAccountBaselineCILog(audit_params.AccountNumber, newBaselineRangeKey,
                                                             oldRangeKey,
                                                             audit_params.AWSRegion != null ? audit_params.AWSRegion.DisplayName : "none",
                                                             "Deletions", userChanges.Deletions, audit_params.AWSCredentials, audit_params.CMLibrary);
                    string changesJSON = ToJSON(userChanges);
                }
            }
            else
            {
                log.Error(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), "New value to check was null"));
            }
            return userChanges;
        }
        private AccountBaselineCI GetNewestFromAccountBaselineCI(string AccountID, string regionName,
                                                                        string ConfigurationItemClass, DateTime BaselineDate, CMLocalLibrary lib, bool RequireCCBApproval)
        {
            return GetNewestFromAccountBaselineCI(AccountID, regionName, ConfigurationItemClass, null, BaselineDate, lib, RequireCCBApproval);
        }

        private AccountBaselineCI GetNewestFromAccountBaselineCI(string AccountID, string regionName, string ConfigurationItemClass, Amazon.Runtime.SessionAWSCredentials creds, DateTime BaselineDate, CMLocalLibrary databaseLib, bool RequireCCBApproval)
        {
            return databaseLib.GetNewestFromAccountBaselineCI(AccountID, ConfigurationItemClass, regionName, BaselineDate, RequireCCBApproval);
        }

        private long AddObjectToAccountBaselineCI(string accountnum, object o, string AuditCategory, string region, Amazon.Runtime.SessionAWSCredentials creds,
                                                             CMLocalLibrary lib)
        {
            long result = -1;
            if (o != null)
            {
                try
                {
                    AccountBaselineCI CI = new AccountBaselineCI();
                    CI.AccountID = accountnum;
                    Type type = o.GetType().GetProperty("Item").PropertyType;
                    CI.ConfigurationItemClass = type.ToString();
                    CI.ConfigurationItemJSON = ToJSON(o);
                    CI.AuditCategory = AuditCategory;
                    CI.ConfigurationItemName = CI.ConfigurationItemClass;
                    CI.Date = DateTime.UtcNow;
                    CI.RangeKey = DateTime.UtcNow.Ticks;
                    CI.User = Environment.UserName;
                    CI.Region = region;
                    CI.Hash = CI.AccountID + ":" + CI.Region + ":" + CI.ConfigurationItemClass;

                    lib.AddAccountBaselineCI(CI);
                    result = CI.RangeKey;
                }
                catch (Amazon.DynamoDB.AmazonDynamoDBException dDBEx)
                {
                    string s = dDBEx.Message;
                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
            }
            return result;
        }
        private long AddChangesToAccountBaselineCILog(string accountnum,
                                                             long NewAccountBaselineCIRangeKey,
                                                             long OldAccountBaselineCIRangeKey, string region,
                                                             string changeType, object o,
                                                             Amazon.Runtime.SessionAWSCredentials creds,
                                                             CMLocalLibrary lib)
        {
            long result;
            AccountBaselineCILog CIlog = new AccountBaselineCILog();
            CIlog.AccountID = accountnum;
            Type type = o.GetType().GetProperty("Item").PropertyType;
            CIlog.ConfigurationItemClass = type.ToString();
            CIlog.ConfigurationItemJSON = ToJSON(o);
            CIlog.ConfigurationItemName = CIlog.ConfigurationItemClass;
            CIlog.Date = DateTime.UtcNow;
            CIlog.RangeKey = DateTime.UtcNow.Ticks;
            CIlog.ChangeType = changeType;
            CIlog.NewAccountBaselineCIRangeKey = NewAccountBaselineCIRangeKey;
            CIlog.OldAccountBaselineCIRangeKey = OldAccountBaselineCIRangeKey;
            CIlog.User = Environment.UserName;
            CIlog.Region = region;
            CIlog.Hash = CIlog.AccountID + ":" + region + ":" + CIlog.ConfigurationItemClass;
            lib.AddAccountBaselineCILog(CIlog);
            result = CIlog.RangeKey;
            return result;
        }

    }
}
