using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace AWSResponderConsole
{
    [Serializable]
    [XmlRoot("AWSCredentials")]
    public class AWSCredentials
    {
        public AWSCredentials()
        {
            try
            {
                AppSettings ap = new AppSettings();
                _configcert = ap.ApplicationEncryptionCertificate;
            }
            catch { }
            finally
            {
                EncryptSettings = (_configcert != null);
            }
            this.SetAWSAccessKey("");
            this.SetAWSSecretKey("");
            this.SetMFASerial("");
            this.SetAWSAuditRole("");
            this.SetAWSAccountsToAudit("");
            this.SetAWSRegionsToAudit("");
            this.SetAWSRegionsToExclude("");
            this.SetAuditReportFile("");
            this.SetTokenLifetime(1800);
            this.SetBaselineDate("Now()");
            this.SetAWSConfigurationItemDatabase("");
            this.SetRequireCCBApproval(true);
        }

        bool EncryptSettings = false;
        private X509Certificate2 _configcert;

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsaccesskey { get; set; }

        public string AWSAccessKey
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                            awsaccesskey), _configcert) :
                            Uri.UnescapeDataString(awsaccesskey);
            }
        }
        public void SetAWSAccessKey(string value)
        {
            awsaccesskey = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awssecretkey { get; set; }
        public string AWSSecretKey
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awssecretkey), _configcert) :
                            Uri.UnescapeDataString(awssecretkey);
            }
        }
        public void SetAWSSecretKey(string value)
        {
            awssecretkey = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string mfaserial { get; set; }
        public string MFASerial
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        mfaserial), _configcert) :
                            Uri.UnescapeDataString(mfaserial);
            }
        }
        public void SetMFASerial(string value)
        {
            mfaserial = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsauditrole { get; set; }
        public string AWSAuditRole
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awsauditrole), _configcert) :
                            Uri.UnescapeDataString(awsauditrole);
            }
        }
        public void SetAWSAuditRole(string value)
        {
            awsauditrole = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsaccountstoaudit { get; set; }
        public string AWSAccountsToAudit
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awsaccountstoaudit), _configcert) :
                            Uri.UnescapeDataString(awsaccountstoaudit);
            }
        }
        public void SetAWSAccountsToAudit(string value)
        {
            awsaccountstoaudit = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsregionstoaudit { get; set; }
        public string AWSRegionsToAudit
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awsregionstoaudit), _configcert) :
                            Uri.UnescapeDataString(awsregionstoaudit);
            }
        }
        public void SetAWSRegionsToAudit(string value)
        {
            awsregionstoaudit = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsregionstoexclude { get; set; }
        public string AWSRegionsToExclude
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awsregionstoexclude), _configcert) :
                            Uri.UnescapeDataString(awsregionstoexclude);
            }
        }
        public void SetAWSRegionsToExclude(string value)
        {
            awsregionstoexclude = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value);
        }

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string auditreportfile { get; set; }
        public string AuditReportFile
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        auditreportfile), _configcert) :
                            Uri.UnescapeDataString(auditreportfile);
            }
        }
        public void SetAuditReportFile(string value)
        {
            auditreportfile = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value.ToString(), _configcert)) :
                    Uri.EscapeDataString(value.ToString());
        }
        
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string tokenlifetime { get; set; }
        public string TokenLifetime 
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        tokenlifetime), _configcert) :
                            Uri.UnescapeDataString(tokenlifetime);
            }
        }
        public void SetTokenLifetime(Int32 value)
        {
            tokenlifetime = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value.ToString(), _configcert)) :
                    Uri.EscapeDataString(value.ToString());
        }


        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string requireccbapproval { get; set; }
        public string RequireCCBApproval
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        requireccbapproval), _configcert) :
                            Uri.UnescapeDataString(requireccbapproval);
            }
        }
        public void SetRequireCCBApproval(bool value)
        {
            requireccbapproval = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value.ToString(), _configcert)) :
                    Uri.EscapeDataString(value.ToString());
        }


        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string baselinedate { get; set; }
        public string BaselineDate
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        baselinedate), _configcert) :
                            Uri.UnescapeDataString(baselinedate);
            }
        }
        public void SetBaselineDate(string value)
        {
            baselinedate = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value.ToString());
        }

        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string awsconfigurationitemdatabase { get; set; }
        public string AWSConfigurationItemDatabase
        {
            get
            {
                return EncryptSettings ?
                            CryptoUtilities.GetX509DecryptedText(
                                    Uri.UnescapeDataString(
                                        awsconfigurationitemdatabase), _configcert) :
                            Uri.UnescapeDataString(awsconfigurationitemdatabase);
            }
        }
        public void SetAWSConfigurationItemDatabase(string value)
        {
            awsconfigurationitemdatabase = EncryptSettings ?
                    Uri.EscapeDataString(CryptoUtilities.GetX509EncryptedText(value, _configcert)) :
                    Uri.EscapeDataString(value.ToString());
        }

    }
}
