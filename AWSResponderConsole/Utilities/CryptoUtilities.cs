using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace AWSResponderConsole
{
    class CryptoUtilities
    {
        public static string GetConsoleInput(string prompt, string defaultvalue, bool maskinput)
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
        public static string GetX509EncryptedText(string PlainStringToEncrypt, X509Certificate2 x509_2)
        {
            try
            {
                string result = "";

                string PlainString = PlainStringToEncrypt;//.Trim();
                byte[] plainbytes = ASCIIEncoding.ASCII.GetBytes(PlainString);
                if (plainbytes.Length < 116)
                    result = GetX509EncryptedText(plainbytes, (RSACryptoServiceProvider)x509_2.PublicKey.Key);
                else //needed to handle byte[] >117
                    result = CreateBase64CMSMessageEnvelope(plainbytes, x509_2);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string GetX509EncryptedText(byte[] PlainBytesToEncrypt, RSACryptoServiceProvider rsa)
        {
            if (PlainBytesToEncrypt.Length > 116)
                throw new Exception("The string is too long for asymetric encryption");
            byte[] cipher = rsa.Encrypt(PlainBytesToEncrypt, false);
            //if successful 
            return Convert.ToBase64String(cipher);
        }
        private static string CreateBase64CMSMessageEnvelope(string PlainStringToEncrypt, X509Certificate2 recipientCert)
        {
            string PlainString = PlainStringToEncrypt;
            byte[] plainbytes = ASCIIEncoding.ASCII.GetBytes(PlainString);
            return CreateBase64CMSMessageEnvelope(plainbytes, recipientCert);
        }
        private static string CreateBase64CMSMessageEnvelope(byte[] PlainBytesToEncrypt, X509Certificate2 recipientCert)
        {
            byte[] encodedEnvelopedCms = EnvelopedCmsSingleRecipient.EncryptMsg(PlainBytesToEncrypt, recipientCert);
            return Convert.ToBase64String(encodedEnvelopedCms);
        }
        public static string GetX509DecryptedText(string EncryptedStringToDecrypt, X509Certificate2 x509_2)
        {
            try
            {
                string result = "";
                if (x509_2.HasPrivateKey)
                {
                    byte[] cipherbytes = Convert.FromBase64String(EncryptedStringToDecrypt);
                    try
                    {
                        result = GetX509DecryptedText(cipherbytes, (RSACryptoServiceProvider)x509_2.PrivateKey);
                    }
                    catch (Exception ex)
                    {
                        result = OpenBase64CMSMessageEnvelope(cipherbytes, x509_2);
                    }
                    return result;
                }
                else
                {
                    throw new Exception("Certificate used for has no private key.");
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        private static string GetX509DecryptedText(byte[] cipherbytes, RSACryptoServiceProvider rsa)
        {
            byte[] plainbytes = rsa.Decrypt(cipherbytes, false);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(plainbytes);
        }
        private static string GetX509DecryptedText(string EncryptedStringToDecrypt, RSACryptoServiceProvider rsa)
        {
            byte[] cipherbytes = Convert.FromBase64String(EncryptedStringToDecrypt);
            byte[] plainbytes = rsa.Decrypt(cipherbytes, false);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(plainbytes);
        }
        private static string OpenBase64CMSMessageEnvelope(byte[] EncryptedBytesToDecrypt, X509Certificate2 recipientCert)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();
            Byte[] decryptedMsg = EnvelopedCmsSingleRecipient.DecryptMsg(EncryptedBytesToDecrypt);
            return unicode.GetString(decryptedMsg);
        }
    }
}
