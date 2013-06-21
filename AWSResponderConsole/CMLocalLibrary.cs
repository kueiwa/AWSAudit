using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;


namespace AWSResponderConsole
{
     public class CMLocalLibrary: IDisposable
    {
        SqlCeConnection conn = null;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _disposed;
        public CMLocalLibrary() 
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log4net.ThreadContext.Properties["SessionID"] = Environment.UserDomainName + "\\" + Environment.UserName;
                string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(CMLocalLibrary)).CodeBase);
                AppSettings appset = new AppSettings();
                string awsdbfile = "AWSInventory.sdf";
                if (appset.CloudCredentials == null)
                {
                    log.Error(String.Format("Database connection cannot be opened, data file is not defined check command line for valid DB "));
                }
                else
                {
                    awsdbfile = appset.CloudCredentials.AWSConfigurationItemDatabase;
                }
                if (!System.IO.File.Exists(awsdbfile))
                    if (System.IO.File.Exists(path + "\\" + awsdbfile))
                        awsdbfile = path + "\\" + awsdbfile;
                conn = new SqlCeConnection(String.Format("Data Source = {0}; Persist Security Info=False", awsdbfile));
                conn.Open();
                log.Info(String.Format("Database connection opened for {0} ", awsdbfile));
                _disposed = false;
            }
            catch (Exception ex)
            {
                log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
            }
        }
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (conn!=null)
                        if (conn.State != System.Data.ConnectionState.Closed)
                            conn.Close();
                    Console.WriteLine("Object disposed.");
                }

                // Indicate that the instance has been disposed.
                conn = null;
                _disposed = true;
            }
        }
        public void AddAccountBaselineCILog(AccountBaselineCILog CILog)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                 //INSERT INTO table_name (column1,column2,column3,...)
                 //VALUES (value1,value2,value3,...);
                try
                {
                    cmd.CommandText = "INSERT INTO  AccountBaselineCILog(Hash, RangeKey, AccountID, Region, ChangeType, " +
                                      "ConfigurationItemClass, ConfigurationItemJSON, ConfigurationItemName," +
                                      "DateEntered, NewAccountBaselineCIRangeKey, OldAccountBaselineCIRangeKey, UserName, CCBApproved)" +
                                      String.Format(" VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', 0)",
                                      CILog.Hash, CILog.RangeKey, CILog.AccountID, CILog.Region, CILog.ChangeType,
                                      CILog.ConfigurationItemClass, CILog.ConfigurationItemJSON.Replace("'", "\""),
                                      CILog.ConfigurationItemName, CILog.Date, CILog.NewAccountBaselineCIRangeKey,
                                      CILog.OldAccountBaselineCIRangeKey, CILog.User);
                    int result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
            }
        }
        public void AddAccountBaselineCI(AccountBaselineCI CI)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                //INSERT INTO table_name (column1,column2,column3,...)
                //VALUES (value1,value2,value3,...);
                try
                {
                    cmd.CommandText = "INSERT INTO  AccountBaselineCIs(Hash, RangeKey, AccountID, Region, " +
                                      "ConfigurationItemClass, ConfigurationItemJSON, ConfigurationItemName," +
                                      "DateEntered, UserName, CCBApproved)" +
                                      String.Format(" VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', 0)",
                                      CI.Hash, CI.RangeKey, CI.AccountID, CI.Region,
                                      CI.ConfigurationItemClass, CI.ConfigurationItemJSON.Replace("'","\""),
                                      CI.ConfigurationItemName, CI.Date, CI.User);
                    int result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
            }
        }
        public void ModifyAccountBaselineCI(AccountBaselineCI CI)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                //UPDATE table_name SET column1=value1,column2=value2,...
                //WHERE some_column=some_value;
                cmd.CommandText = String.Format("UPDATE AccountBaselineCIs SET RangeKey={0}, AccountID={1}, Region={2}, " +
                                  "ConfigurationItemClass={3}, ConfigurationItemJSON={4},"+
                                  "ConfigurationItemName={5}," +
                                  "Date={6}, User={7}, CCB_Approved={8}",
                                  CI.RangeKey, CI.AccountID, CI.Region,
                                  CI.ConfigurationItemClass, CI.ConfigurationItemJSON.Replace("'", "\""),
                                  CI.ConfigurationItemName, CI.Date, CI.User, CI.CCBApproved);
                int result = cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<AccountBaselineCI> GetAllCIs()
        {
            List<AccountBaselineCI> result = new List<AccountBaselineCI>();
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = String.Format("Select * from AccountBaselineCIs");
                SqlCeDataReader myReader = null;
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    AccountBaselineCI CI = new AccountBaselineCI();
                    CI.Hash = (myReader["Hash"].ToString());
                    CI.RangeKey = (long.Parse(myReader["RangeKey"].ToString()));
                    CI.AccountID = (myReader["AccountID"].ToString());
                    CI.Region = (myReader["Region"].ToString());
                    CI.ConfigurationItemClass = (myReader["ConfigurationItemClass"].ToString());
                    CI.ConfigurationItemJSON= (myReader["ConfigurationItemJSON"].ToString());
                    CI.ConfigurationItemName= (myReader["ConfigurationItemName"].ToString());
                    CI.Date= (DateTime.Parse(myReader["Date"].ToString()));
                    CI.User= (myReader["User"].ToString());
                    CI.CCBApproved= (myReader["CCBApproved"].ToString());
                    result.Add(CI);
                }
            }
            return result;
        }
        public IEnumerable<AccountBaselineCI> SearchCIs(string hash, long range)
        {
            List<AccountBaselineCI> result = new List<AccountBaselineCI>();
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = String.Format("Select * from AccountBaselineCIs where Hash={0} and RangeKey>={1}", hash, range.ToString());
                SqlCeDataReader myReader = null;
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    AccountBaselineCI CI = new AccountBaselineCI();
                    CI.Hash = (myReader["Hash"].ToString());
                    CI.RangeKey = (long.Parse(myReader["RangeKey"].ToString()));
                    CI.AccountID = (myReader["AccountID"].ToString());
                    CI.Region = (myReader["Region"].ToString());
                    CI.ConfigurationItemClass = (myReader["ConfigurationItemClass"].ToString());
                    CI.ConfigurationItemJSON = (myReader["ConfigurationItemJSON"].ToString());
                    CI.ConfigurationItemName = (myReader["ConfigurationItemName"].ToString());
                    CI.Date = (DateTime.Parse(myReader["Date"].ToString()));
                    CI.User = (myReader["User"].ToString());
                    CI.CCBApproved = (myReader["CCBApproved"].ToString());
                    result.Add(CI);
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public List<AccountBaselineCI> SearchCIsByAccountAndClass(string AccountID, string ConfigurationItemClass,
                                                                  string regionName)
        {
            List<AccountBaselineCI> result = new List<AccountBaselineCI>();
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = String.Format("Select * from AccountBaselineCIs where AccountID={0} and ConfigurationItemClass={1} and Region={2}", AccountID, ConfigurationItemClass, regionName);
                SqlCeDataReader myReader = null;
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    AccountBaselineCI CI = new AccountBaselineCI();
                    CI.Hash = (myReader["Hash"].ToString());
                    CI.RangeKey = (long.Parse(myReader["RangeKey"].ToString()));
                    CI.AccountID = (myReader["AccountID"].ToString());
                    CI.Region = (myReader["Region"].ToString());
                    CI.ConfigurationItemClass = (myReader["ConfigurationItemClass"].ToString());
                    CI.ConfigurationItemJSON = (myReader["ConfigurationItemJSON"].ToString());
                    CI.ConfigurationItemName = (myReader["ConfigurationItemName"].ToString());
                    CI.Date = (DateTime.Parse(myReader["DateEntered"].ToString()));
                    CI.User = (myReader["UserName"].ToString());
                    CI.CCBApproved = (myReader["CCBApproved"].ToString());
                    result.Add(CI);
                }
            }
            return result;
        }

        public AccountBaselineCI GetNewestFromAccountBaselineCI(string AccountID, string ConfigurationItemClass,
                                                                  string regionName, DateTime BaselineDate, bool RequireCCBApproval)
        {
            AccountBaselineCI result = null;
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                string BaselineDateString = BaselineDate.ToShortDateString() + " " + BaselineDate.ToShortTimeString();
                cmd.CommandText = String.Format("Select * from AccountBaselineCIs where AccountID={0} "+
                                                "and ConfigurationItemClass='{1}' and Region='{2}' and " +
                                                "DateEntered < '{3}'"+
                                                (RequireCCBApproval ? " and CCBApproved=0" : "") +
                                                " order by RangeKey DESC", AccountID,
                                                ConfigurationItemClass, regionName, BaselineDateString);
                SqlCeDataReader myReader = null;
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    AccountBaselineCI CI = new AccountBaselineCI();
                    CI.Hash = (myReader["Hash"].ToString());
                    CI.RangeKey = (long.Parse(myReader["RangeKey"].ToString()));
                    CI.AccountID = (myReader["AccountID"].ToString());
                    CI.Region = (myReader["Region"].ToString());
                    CI.ConfigurationItemClass = (myReader["ConfigurationItemClass"].ToString());
                    CI.ConfigurationItemJSON = (myReader["ConfigurationItemJSON"].ToString());
                    CI.ConfigurationItemName = (myReader["ConfigurationItemName"].ToString());
                    CI.Date = (DateTime.Parse(myReader["DateEntered"].ToString()));
                    CI.User = (myReader["UserName"].ToString());
                    CI.CCBApproved = (myReader["CCBApproved"].ToString());
                    result = CI;
                }
            }
            return result;
        }
        public void DeleteCI(AccountBaselineCI CI)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = String.Format("DELETE FROM  AccountBaselineCIs where hash={0} and RangeKey={1}",
                                  CI.Hash, CI.RangeKey);
                int result = cmd.ExecuteNonQuery();
                log.Info(String.Format("Database CI {0} with RangeKey {1} in AccountBaselineCIs of connection {2} was deleted", CI.Hash, CI.RangeKey, conn.ConnectionString));
            }
        }

        public void Init()
        {
            string CITableName = "AccountBaselineCIs";
            string CILogTableName = "AccountBaselineCILog";
            using (SqlCeCommand command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT COUNT(*) FROM Information_Schema.Tables WHERE TABLE_NAME = '" + CITableName + "'";
                Int32 count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    string AccountBaselineCIsCreate = "CREATE TABLE " + CITableName + " ("
                                                + "Hash nvarchar (350), "
                                                + "RangeKey bigint, "
                                                + "AccountID nvarchar(100), "
                                                + "Region nvarchar(100), "
                                                + "ConfigurationItemClass nvarchar(1000), "
                                                + "ConfigurationItemJSON ntext, "
                                                + "ConfigurationItemName nvarchar(1000), "
                                                + "CCBApproved bit, "
                                                + "UserName nvarchar(100), "
                                                + "DateEntered datetime)";
                    command.CommandText = AccountBaselineCIsCreate;
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT COUNT(*) FROM Information_Schema.Tables WHERE TABLE_NAME = '" + CITableName + "'";
                    count = Convert.ToInt32(command.ExecuteScalar());
                    if(count>0)
                        log.Info(String.Format("Database altered, added table {0} for connection {1}", CITableName, conn.ConnectionString));
                }
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT COUNT(*) FROM Information_Schema.Tables WHERE TABLE_NAME = '" + CILogTableName + "'";
                count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    string AccountBaselineCILogCreate = "CREATE TABLE " + CILogTableName + " ("
                                                + "Hash nvarchar (350), "
                                                + "RangeKey bigint, "
                                                + "AccountID nvarchar(100), "
                                                + "Region nvarchar(100), "
                                                + "ChangeType nvarchar(100), "
                                                + "ConfigurationItemClass nvarchar(1000), "
                                                + "ConfigurationItemJSON ntext, "
                                                + "ConfigurationItemName nvarchar(1000), "
                                                + "NewAccountBaselineCIRangeKey bigint, "
                                                + "OldAccountBaselineCIRangeKey bigint,"
                                                + "CCBApproved bit, "
                                                + "UserName nvarchar(100), "
                                                + "DateEntered datetime)";
                    command.CommandText = AccountBaselineCILogCreate;
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT COUNT(*) FROM Information_Schema.Tables WHERE TABLE_NAME = '" + CILogTableName + "'";
                    count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0)
                        log.Info(String.Format("Database altered, added table {0} for connection {1}", CILogTableName, conn.ConnectionString));

                }
            }
 
        }

    }
}
