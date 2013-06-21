using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AWSResponderConsole
{
    public class AuditReport
    {
        #region Reporting
        public void SummaryReport(AWSAuditData results, bool outputsummary, string outputfile)
        {
            StringBuilder sb = new StringBuilder("");
            #region Calculate column widths
            int colwidth = 0;
            foreach (string accountkey in results.Accounts.Keys)
            {
                // Get type.
                System.Type type = results.Accounts[accountkey].IAM.GetType();
                // Loop over properties.
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    // Get name.
                    string itemName = type.Name + "." + propertyInfo.Name;
                    int keylength = itemName.Length;
                    colwidth = colwidth > keylength ? colwidth : keylength;
                }
                foreach (string regionkey in results.Accounts[accountkey].Regions.Keys)
                {
                    System.Type region_type = results.Accounts[accountkey].Regions[regionkey].GetType();
                    foreach (PropertyInfo propertyInfo in region_type.GetProperties())
                    {
                        object t = propertyInfo.GetValue(results.Accounts[accountkey].Regions[regionkey], null);
                        if (t != null)
                        {
                            System.Type classtype = t.GetType();
                            foreach (PropertyInfo propertyInfo2 in classtype.GetProperties())
                            {
                                string itemName = classtype.Name + "." + propertyInfo2.Name;
                                int keylength = itemName.Length;
                                colwidth = colwidth > keylength ? colwidth : keylength;
                            }
                        }
                    }
                }
            }
            #endregion
            foreach (string accountkey in results.Accounts.Keys)
            {
                string Header = "Config Item".PadRight(colwidth - 2, '_') +
                                  "Current".PadLeft(8, '_') + "Addition".PadLeft(9, '_') +
                                  "Deletion".PadLeft(9, '_');

                sb.AppendLine("".PadLeft(Header.Length, '*'));
                sb.AppendLine("Audit of account number:" + accountkey);
                sb.AppendLine("".PadLeft(Header.Length, '*'));
                System.Type type = results.Accounts[accountkey].IAM.GetType();
                // Loop over properties.
                sb.AppendLine("IAM:");
                sb.AppendLine("".PadLeft(Header.Length, '*'));
                sb.AppendLine(Header);
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.MemberType == MemberTypes.Property && propertyInfo.PropertyType.FullName.Contains("ListComparisonResults"))
                    {
                        object a = propertyInfo.GetValue(results.Accounts[accountkey].IAM, null);
                        if (a != null)
                        {
                            System.Type ComparisonType = a.GetType();
                            PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                            System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                            PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                            System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                            PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                            System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                            // Get name.
                            string itemName = type.Name + "." + propertyInfo.Name;
                            sb.AppendLine(itemName.PadRight(colwidth, ' ') +
                                          current.Count.ToString().PadLeft(5, ' ') +
                                          additions.Count.ToString().PadLeft(8, ' ') +
                                          deletions.Count.ToString().PadLeft(8, ' '));
                        }
                    }
                }
                foreach (string regionkey in results.Accounts[accountkey].Regions.Keys)
                {
                    sb.AppendLine("".PadLeft(Header.Length, '*'));
                    sb.AppendLine(regionkey.PadRight(Header.Length / 2, ' ') + regionkey.PadLeft(Header.Length / 2, ' '));
                    sb.AppendLine("".PadLeft(Header.Length, '-'));
                    sb.AppendLine(Header);

                    System.Type region_type = results.Accounts[accountkey].Regions[regionkey].GetType();
                    foreach (PropertyInfo propertyInfo in region_type.GetProperties())
                    {
                        object t = propertyInfo.GetValue(results.Accounts[accountkey].Regions[regionkey], null);
                        if (t != null)
                        {
                            System.Type classtype = t.GetType();
                            foreach (PropertyInfo propertyInfo2 in classtype.GetProperties())
                            {
                                object a = propertyInfo2.GetValue(t, null);
                                if (a != null)
                                {
                                    System.Type ComparisonType = a.GetType();
                                    PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                                    System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                                    PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                                    System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                                    PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                                    System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                                    // Get name.
                                    string itemName = classtype.Name + "." + propertyInfo2.Name;
                                    sb.AppendLine(itemName.PadRight(colwidth, ' ') +
                                                  current.Count.ToString().PadLeft(5, ' ') +
                                                  additions.Count.ToString().PadLeft(8, ' ') +
                                                  deletions.Count.ToString().PadLeft(8, ' '));
                                }
                            }
                        }
                    }
                }
                sb.AppendLine("".PadLeft(Header.Length, '*'));
                sb.AppendLine("End of Report" + accountkey);
                sb.AppendLine("".PadLeft(Header.Length, '*'));
                Console.WriteLine(sb);

                if (outputfile != null)
                {
                    File.WriteAllText(outputfile, sb.ToString());
                }

            }

        }
        public void CvsReport(AWSAuditData results, bool outputsummary, string outputfile)
        {
            StringBuilder sb = new StringBuilder("");
            foreach (string accountkey in results.Accounts.Keys)
            {
                string Header = "\"AccountNumber\",\"Region\",\"Config Item\",\"Current\",\"Addition\",\"Deletion\",\"BaselineDate\",\"Date\"";
                sb.AppendLine(Header);
                System.Type type = results.Accounts[accountkey].IAM.GetType();
                // Loop over properties.
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.MemberType == MemberTypes.Property && propertyInfo.PropertyType.FullName.Contains("ListComparisonResults"))
                    {
                        object a = propertyInfo.GetValue(results.Accounts[accountkey].IAM, null);
                        if (a != null)
                        {
                            System.Type ComparisonType = a.GetType();
                            PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                            System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                            PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                            System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                            PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                            System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                            // Get name.
                            string itemName = type.Name + "." + propertyInfo.Name;
                            sb.AppendLine(String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                                        accountkey, "none", itemName, 
                                                        current.Count.ToString(),
                                                        additions.Count.ToString(),
                                                        deletions.Count.ToString(),
                                                        results.BaselineDate.ToUniversalTime().ToString("s") + "Z",
                                                        results.Date.ToUniversalTime().ToString("s")+"Z"));
                        }
                    }
                }
                foreach (string regionkey in results.Accounts[accountkey].Regions.Keys)
                {

                    System.Type region_type = results.Accounts[accountkey].Regions[regionkey].GetType();
                    foreach (PropertyInfo propertyInfo in region_type.GetProperties())
                    {
                        object t = propertyInfo.GetValue(results.Accounts[accountkey].Regions[regionkey], null);
                        if (t != null)
                        {
                            System.Type classtype = t.GetType();
                            foreach (PropertyInfo propertyInfo2 in classtype.GetProperties())
                            {
                                object a = propertyInfo2.GetValue(t, null);
                                if (a != null)
                                {
                                    System.Type ComparisonType = a.GetType();
                                    PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                                    System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                                    PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                                    System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                                    PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                                    System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                                    // Get name.
                                    string itemName = classtype.Name + "." + propertyInfo2.Name;
                                    sb.AppendLine(String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                                                accountkey, regionkey, itemName,
                                                                current.Count.ToString(),
                                                                additions.Count.ToString(),
                                                                deletions.Count.ToString(),
                                                                results.BaselineDate.ToUniversalTime().ToString("s") + "Z",
                                                                results.Date.ToUniversalTime().ToString("s") + "Z"));
                                }
                            }
                        }
                    }
                }
                Console.WriteLine(sb);

                if (outputfile != null)
                {
                    try
                    {
                        File.WriteAllText(outputfile, sb.ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                    finally { }
                }

            }

        }
        public void CPEList(AWSAuditData results, bool outputsummary, string outputfile)
        {
            StringBuilder sb = new StringBuilder("");
            StringBuilder titles = new StringBuilder("");

            foreach (string accountkey in results.Accounts.Keys)
            {

                System.Type type = results.Accounts[accountkey].IAM.GetType();
                // Loop over properties.
                sb.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
                sb.AppendLine("<cpe-list xmlns:config=\"http://scap.nist.gov/schema/configuration/0.1\" "+
                              "xmlns:meta=\"http://scap.nist.gov/schema/cpe-dictionary-metadata/0.2\" "+
                              "xmlns:ns6=\"http://scap.nist.gov/schema/scap-core/0.1\" "+
                              "xmlns:scap-core=\"http://scap.nist.gov/schema/scap-core/0.3\" "+
                              "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "+
                              "xmlns=\"http://cpe.mitre.org/dictionary/2.0\" "+
                              "xsi:schemaLocation=\"http://scap.nist.gov/schema/configuration/0.1 http://nvd.nist.gov/schema/configuration_0.1.xsd http://scap.nist.gov/schema/scap-core/0.3 http://nvd.nist.gov/schema/scap-core_0.3.xsd http://cpe.mitre.org/dictionary/2.0 http://cpe.mitre.org/files/cpe-dictionary_2.2.xsd http://scap.nist.gov/schema/scap-core/0.1 http://nvd.nist.gov/schema/scap-core_0.1.xsd http://scap.nist.gov/schema/cpe-dictionary-metadata/0.2 http://nvd.nist.gov/schema/cpe-dictionary-metadata_0.2.xsd\">");
                sb.AppendLine("<generator>");
                sb.AppendLine("<product_name>National Vulnerability Database (NVD)</product_name>");
                sb.AppendLine("<product_version>2.18.0-SNAPSHOT (PRODUCTION)</product_version>");
                sb.AppendLine("<schema_version>2.2</schema_version>");
                sb.AppendLine("<timestamp>2013-05-24T03:50:00.129Z</timestamp>");
                sb.AppendLine("</generator>");


                int draftId = 700000;
                ++draftId;
                sb.AppendLine(String.Format("\t<cpe-item name=\"cpe:/a:amazon:{0}:1.0.0.0\">", type.Name));
                sb.AppendLine(String.Format("\t\t<title xml:lang=\"en-US\">Amazon {0} 1.0.0.0</title>", type.Name));
                titles.AppendLine(String.Format("Amazon {0} 1.0.0.0", type.Name));
                sb.AppendLine("\t\t<meta:item-metadata modification-date=" +
                                                DateTime.UtcNow.ToString("s") + "Z" +
                                                " status=\"DRAFT\" " +
                                                " nvd-id=" + draftId.ToString() + "/>");
                sb.AppendLine("\t</cpe-item>");
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {


                    /*
                    if (propertyInfo.MemberType == MemberTypes.Property && propertyInfo.PropertyType.FullName.Contains("ListComparisonResults"))
                    {
                        object a = propertyInfo.GetValue(results.Accounts[accountkey].IAM, null);
                        if (a != null)
                        {
                            System.Type ComparisonType = a.GetType();
                            PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                            System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                            PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                            System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                            PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                            System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                            // Get name.
                            string itemName = type.Name + "." + propertyInfo.Name;
                            ++draftId;
                            string draftIdstr = draftId.ToString();
                            sb.AppendLine(String.Format("\t<cpe-item name=\"cpe:/a:amazon:{0}:1.0.0.0\">",type.Name));
                            sb.AppendLine(String.Format("\t\t<title xml:lang=\"en-US\">Amazon {0} 1.0.0.0</title>",type.Name));
                            sb.AppendLine("\t\t<meta:item-metadata modification-date="+
                                                         DateTime.UtcNow.ToString("s") + "Z" +
                                                         " status=\"DRAFT\" "+
                                                         " nvd-id=" + draftIdstr + "/>");
                            sb.AppendLine("\t</cpe-item>");
                        }
                    }
                     */
                }
                foreach (string regionkey in results.Accounts[accountkey].Regions.Keys)
                {

                    System.Type region_type = results.Accounts[accountkey].Regions[regionkey].GetType();
                    foreach (PropertyInfo propertyInfo in region_type.GetProperties())
                    {
                        object t = propertyInfo.GetValue(results.Accounts[accountkey].Regions[regionkey], null);
                        if (t != null)
                        {
                            System.Type classtype = t.GetType();

                            ++draftId;
                            sb.AppendLine(String.Format("\t<cpe-item name=\"cpe:/a:amazon:{0}:1.0.0.0\">", classtype.Name.ToLower().Replace(" ","")));
                            sb.AppendLine(String.Format("\t\t<title xml:lang=\"en-US\">Amazon {0} 1.0.0.0</title>", classtype.Name));
                            titles.AppendLine(String.Format("Amazon {0} 1.0.0.0", classtype.Name));

                            sb.AppendLine("\t\t<meta:item-metadata modification-date=" +
                                                            DateTime.UtcNow.ToString("s") + "Z" +
                                                            " status=\"DRAFT\" " +
                                                            " nvd-id=" + draftId.ToString() + "/>");
                            sb.AppendLine("\t</cpe-item>");

                            /*
                            foreach (PropertyInfo propertyInfo2 in classtype.GetProperties())
                            {
                                object a = propertyInfo2.GetValue(t, null);
                                if (a != null)
                                {
                                    System.Type ComparisonType = a.GetType();
                                    PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                                    System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                                    PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                                    System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                                    PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                                    System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                                    // Get name.
                                    string itemName = classtype.Name + "." + propertyInfo2.Name;
                                    sb.AppendLine(String.Format("\t\t<AWSObject name=\"Amazon.{0}.{1}.{2}\">",
                                                                regionkey, classtype.Name, propertyInfo2.Name));
                                    sb.AppendLine("\t\t\t<ObjectRule valueProperty=\"current\" operator=\"<\" baseline=\"15\" product=\"fail\" >");
                                    sb.AppendLine("\t\t</AWSObject>");
                                }
                            }
                             */
                        }
                    }
                }
                sb.AppendLine("</cpe-list>"); 
                string rules = sb.ToString();
                string titleList = titles.ToString();
                Console.WriteLine(sb);
            }

        }

        public void RulesList(AWSAuditData results, bool outputsummary, string outputfile)
        {
            StringBuilder sb = new StringBuilder("");

            foreach (string accountkey in results.Accounts.Keys)
            {

                System.Type type = results.Accounts[accountkey].IAM.GetType();
                // Loop over properties.
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sb.AppendLine("<configuration>");
                sb.AppendLine("\t<configSections>");
                sb.AppendLine("\t\t<sectionGroup name=\"AwsRules\">");
                sb.AppendLine("\t\t\t<section name=\"AWSObject\" type=\"CustomConfig.Module, CustomConfig\" allowLocation=\"true\" allowDefinition=\"Everywhere\" />");
                sb.AppendLine("\t\t</sectionGroup>");
                sb.AppendLine("\t</configSections>");

                sb.AppendLine("\t<AwsRules>");

                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.MemberType == MemberTypes.Property && propertyInfo.PropertyType.FullName.Contains("ListComparisonResults"))
                    {
                        object a = propertyInfo.GetValue(results.Accounts[accountkey].IAM, null);
                        if (a != null)
                        {
                            System.Type ComparisonType = a.GetType();
                            PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                            System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                            PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                            System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                            PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                            System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                            // Get name.
                            string itemName = type.Name + "." + propertyInfo.Name;


                            sb.AppendLine(String.Format("\t\t<AWSObject name=\"Amazon.{0}.{1}.{2}\">",
                                                        "none", type.Name, propertyInfo.Name));
                            sb.AppendLine("\t\t\t<ObjectRule valueProperty=\"current\" operator=\"<\" baseline=\"15\" product=\"fail\" >");
                            sb.AppendLine("\t\t</AWSObject>");
                            
                            //sb.AppendLine(String.Format("[Amazon.{0}.{1}.{2}]","none", type.Name, propertyInfo.Name));
                        }
                    }
                }
                foreach (string regionkey in results.Accounts[accountkey].Regions.Keys)
                {

                    System.Type region_type = results.Accounts[accountkey].Regions[regionkey].GetType();
                    foreach (PropertyInfo propertyInfo in region_type.GetProperties())
                    {
                        object t = propertyInfo.GetValue(results.Accounts[accountkey].Regions[regionkey], null);
                        if (t != null)
                        {
                            System.Type classtype = t.GetType();
                            foreach (PropertyInfo propertyInfo2 in classtype.GetProperties())
                            {
                                object a = propertyInfo2.GetValue(t, null);
                                if (a != null)
                                {
                                    System.Type ComparisonType = a.GetType();
                                    PropertyInfo piCurrent = ComparisonType.GetProperty("Current");
                                    System.Collections.IList current = (System.Collections.IList)piCurrent.GetValue(a, null);
                                    PropertyInfo piAdditions = ComparisonType.GetProperty("Additions");
                                    System.Collections.IList additions = (System.Collections.IList)piAdditions.GetValue(a, null);
                                    PropertyInfo piDeletions = ComparisonType.GetProperty("Deletions");
                                    System.Collections.IList deletions = (System.Collections.IList)piDeletions.GetValue(a, null);
                                    // Get name.
                                    string itemName = classtype.Name + "." + propertyInfo2.Name;
                                    sb.AppendLine(String.Format("\t\t<AWSObject name=\"Amazon.{0}.{1}.{2}\">",
                                                                regionkey, classtype.Name, propertyInfo2.Name));
                                    sb.AppendLine("\t\t\t<ObjectRule valueProperty=\"current\" operator=\"<\" baseline=\"15\" product=\"fail\" >");
                                    sb.AppendLine("\t\t</AWSObject>");
                                    //sb.AppendLine("");
                                    //sb.AppendLine(String.Format("[Amazon.{0}.{1}.{2}]",regionkey, classtype.Name, propertyInfo2.Name));

                                }
                            }
                        }
                    }
                }
                sb.AppendLine("\t</AwsRules>");
                sb.AppendLine("</configuration>"); 
                string rules = sb.ToString();
                Console.WriteLine(sb);

                if (outputfile != null)
                {
                    File.WriteAllText(outputfile, sb.ToString());
                }

            }

        }
        #endregion

    }
}
