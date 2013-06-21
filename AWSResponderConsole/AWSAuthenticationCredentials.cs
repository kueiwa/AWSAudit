using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;

namespace AWSResponderConsole
{
    public class AWSAuthenticationCredentials
    {
        #region Authentication
        internal string GetConsoleInput(string prompt, string defaultvalue, bool maskinput)
        {
            StringBuilder input = new StringBuilder(defaultvalue);
            Console.Write(prompt);
            if (maskinput)
                Console.Write("".PadRight(input.Length, '*'));
            else
                Console.Write(input);
            for (ConsoleKeyInfo keyinfo = Console.ReadKey(true); keyinfo.Key != ConsoleKey.Enter; keyinfo = Console.ReadKey(true))
            {
                //handle backspace
                if (keyinfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        int left = Console.CursorLeft;
                        int top = Console.CursorTop;
                        Console.SetCursorPosition(left - 1, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(left - 1, top);
                        input.Length -= 1;
                    }
                }
                else
                {
                    Console.Write(maskinput ? '*' : keyinfo.KeyChar);
                    input.Append(keyinfo.KeyChar);
                }
            }
            Console.WriteLine();
            return input.ToString();
        }
        public Amazon.SecurityToken.Model.Credentials GetConsoleLogin(string accessKeyID, string secretAccessKeyID, string MFASerialID, int TokenLifetime, bool suppressrequestaccountinformation)
        {
            Amazon.SecurityToken.Model.Credentials login = null;
            while (login == null)
            {
                string MFASerial = "";
                if (!suppressrequestaccountinformation)
                {
                    accessKeyID = GetConsoleInput("Enter AWSAccessKey :", accessKeyID, false);
                    secretAccessKeyID = GetConsoleInput("Enter AWSSecretAccessKey :", secretAccessKeyID, true);
                    Console.WriteLine("MFA authentication required=============================================");
                    Console.WriteLine("MFA Serial Number For virtual devices this is usually");
                    Console.WriteLine("arn:aws:iam::123456789012:mfa/user or the serial number on the back of a hardware device");
                    Console.WriteLine("");
                    string defaultvalue = MFASerialID;
                    MFASerial = GetConsoleInput("Enter MFA the Serial Number :", defaultvalue, false);
                }
                string MFAToken = GetConsoleInput("Enter the MFA Token Code:", "", true);
                string error;
                login = GetCredentials(accessKeyID, secretAccessKeyID, TokenLifetime,
                                               MFASerial, MFAToken, out error);

                if (login == null)
                {
                    Console.WriteLine("");
                    Console.WriteLine(String.Format("Invalid login {0}", error));
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to retry (ctrl+c) to break");
                }
                else
                {
                    Console.Write("Authentication passed");
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write(".");
                        System.Threading.Thread.Sleep(250);
                    }
                    Console.WriteLine();
                }
            }
            return login;
        }

        private Amazon.SecurityToken.Model.Credentials GetCredentials(
                         string accessKeyId, string secretAccessKeyId, int durationseconds,
                         string MFASerialNumber, string MFAToken, out string err)
        {
            Amazon.SecurityToken.Model.Credentials result = null;
            try
            {
                err = null;
                Amazon.SecurityToken.AmazonSecurityTokenServiceClient stsClient =
                    new Amazon.SecurityToken.AmazonSecurityTokenServiceClient(accessKeyId,
                                                         secretAccessKeyId);

                Amazon.SecurityToken.Model.GetSessionTokenRequest getSessionTokenRequest = null;
                //use MFA
                if (MFASerialNumber.Length > 4 && MFASerialNumber.Length == 6)
                {
                    getSessionTokenRequest = new Amazon.SecurityToken.Model.GetSessionTokenRequest()
                                                    .WithDurationSeconds(durationseconds)
                                                    .WithSerialNumber(MFASerialNumber)
                                                    .WithTokenCode(MFAToken);
                }
                else
                {
                    getSessionTokenRequest = new Amazon.SecurityToken.Model.GetSessionTokenRequest()
                                                    .WithDurationSeconds(durationseconds);
                }
                getSessionTokenRequest.DurationSeconds = durationseconds; // seconds

                Amazon.SecurityToken.Model.GetSessionTokenResponse sessionTokenResponse =
                              stsClient.GetSessionToken(getSessionTokenRequest);
                Amazon.SecurityToken.Model.GetSessionTokenResult sessionTokenResult = sessionTokenResponse.GetSessionTokenResult;
                Amazon.SecurityToken.Model.Credentials credentials = sessionTokenResult.Credentials;
                result = credentials;
            }
            catch (Amazon.SecurityToken.AmazonSecurityTokenServiceException ex)
            {
                err = ex.Message;
            }


            return result;
        }

        public Amazon.Runtime.SessionAWSCredentials AssumeRole(string roleArn, int durationseconds, Amazon.SecurityToken.Model.Credentials credentials)
        {
            Amazon.SecurityToken.AmazonSecurityTokenServiceClient stsClientForRole =
                new Amazon.SecurityToken.AmazonSecurityTokenServiceClient(credentials.AccessKeyId,
                                                     credentials.SecretAccessKey, credentials.SessionToken);
            Amazon.Runtime.SessionAWSCredentials sessionCredentials = null;
            if (roleArn == null)
            {
                sessionCredentials = new
                Amazon.Runtime.SessionAWSCredentials(credentials.AccessKeyId,
                                                         credentials.SecretAccessKey,
                                                         credentials.SessionToken);
            }
            else
            {
                Amazon.SecurityToken.Model.AssumeRoleResponse assumeRoleResp =
                        stsClientForRole.AssumeRole(new Amazon.SecurityToken.Model.AssumeRoleRequest()
                                        .WithRoleArn(roleArn)
                                        .WithDurationSeconds(durationseconds)
                                        .WithRoleSessionName("Demo"));
                Amazon.SecurityToken.Model.Credentials creds = assumeRoleResp.AssumeRoleResult.Credentials;
                sessionCredentials = new Amazon.Runtime.SessionAWSCredentials(creds.AccessKeyId,
                                                         creds.SecretAccessKey,
                                                         creds.SessionToken);
            }
            return sessionCredentials;
        }
        #endregion

    }
}
