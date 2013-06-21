using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
namespace AWSResponderConsole
{
    //Application settings wrapper class 
    sealed class AppSettings : ApplicationSettingsBase
    {


        public AppSettings()
        {

        }

        public X509Certificate2 ApplicationEncryptionCertificate
        {
            get 
            {
                X509Certificate2 _configcert = null;
                try
                {
                    //Try to use local certificate
                    _configcert = GetCertificate(this.ApplicationEncryptionCertificateThumbprint);
                }
                catch { }
                return _configcert;
            }
        }
        public string SelectCertificate()
        {
            string result = "";
            //1) Select all the installed certificates with an encryption potential in the users store
            StoreLocation[] locations = new StoreLocation[] { StoreLocation.CurrentUser };
            X509Certificate2 selectedCert = null;
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certCollection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);
            //2) List them out and request an number input to select the certificate
            bool repeat = true;
            Console.Clear();
            Console.WriteLine("A certificate for ecrypting applicaiton settngs was \nnot found please select a certificate");
            Console.WriteLine("");
            Console.WriteLine("Note: If you do not have a certificate, you can create a temporary with makecert.exe");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            while (repeat)
            {
                X509Certificate2Enumerator coll = foundCollection.GetEnumerator();
                bool eol = false;
                int linecounts = 0;
                while (!eol)
                {
                    Console.WriteLine(" Id - SimpleName");
                    
                    int line = 0;
                    for (line = 0; line < 9; line++)
                    {
                        eol = !coll.MoveNext();
                        if (eol)
                            break;
                        X509Certificate2 j = coll.Current;
                        Console.WriteLine("{0} - {1} - {2}", (line + 1).ToString().PadLeft(3, ' '),
                        coll.Current.GetNameInfo(X509NameType.SimpleName, false),
                        coll.Current.Issuer.Contains(',')?
                        coll.Current.Issuer.Substring(0, coll.Current.Issuer.IndexOf(',')):
                        coll.Current.Issuer);
                    }
                    Console.Write("Select a certificate (enter 1 - {0}) or \npress c to cancel and store settings clear text (not recommended)", line);
                    if(!eol)
                        Console.Write(" or press m for more");
                    ConsoleKeyInfo ki = new ConsoleKeyInfo();
                    int num = -1;
                    while (!(((num>=1) && (num<=line)) ||
                                Char.ToLower(ki.KeyChar).Equals('m')))
                    {
                        ki = Console.ReadKey(true);
                        if (!Int32.TryParse(ki.KeyChar.ToString(), out num))
                            num = -1;
                        else
                            num -= 1;
                            if ((num >= 1) && (num <= line))
                            {
                                Console.WriteLine(ki.KeyChar);
                                selectedCert = foundCollection[linecounts + num];
                                return selectedCert.Thumbprint;
                            }
                        if (Char.ToLower(ki.KeyChar).Equals('m'))
                        {
                            Console.WriteLine(ki.KeyChar);
                            break;
                        }
                        if (Char.ToLower(ki.KeyChar).Equals('c'))
                        {
                            Console.WriteLine(ki.KeyChar);
                            return "storecleartext";
                        }
                    }
                    linecounts += line;
                }
                Console.Write("We have reached the end, repeat (y/n):");
                ConsoleKeyInfo k = new ConsoleKeyInfo();
                while (!(Char.ToLower(k.KeyChar).Equals('y') || 
                         Char.ToLower(k.KeyChar).Equals('n') ))
                        k = Console.ReadKey();
            }
            //3) Offer a printout of a command line to create a certificate
            if (selectedCert == null)
            {

            }
            else //4) return the tuumb print of the cert.
                result = selectedCert.Thumbprint;

            return result;
        }
        public static X509Certificate2 GetCertificate(string Thumbprint)
        {
            X509Certificate2 result = null;
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection foundCollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 cert in foundCollection)
            {
                if (cert.Thumbprint.Equals(Thumbprint))
                {
                    result = cert;
                    break;
                }
            }
            return result;
        }

        [UserScopedSettingAttribute()]
        public AWSCredentials CloudCredentials
        {
            get { return (AWSCredentials)this["CloudCredentials"]; }
            set { this["CloudCredentials"] = value; }
        }

        [UserScopedSettingAttribute()]
        public String ApplicationEncryptionCertificateThumbprint
        {
            get { return Uri.UnescapeDataString((String)this["ApplicationEncryptionCertificateThumbprint"]+""); }
            set { this["ApplicationEncryptionCertificateThumbprint"] = Uri.EscapeDataString(value); }
        }


    }
}
