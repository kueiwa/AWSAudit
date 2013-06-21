using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AWSResponderConsole
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ms180959(v=vs.80).aspx
    /// </summary>
    class EnvelopedCmsSingleRecipient
    {

        //  Encrypt the message with the public key of
        //  the recipient. This is done by enveloping the message by
        //  using an EnvelopedCms object.
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms180959(v=vs.80).aspx
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="recipientCert"></param>
        /// <returns></returns>
        static public byte[] EncryptMsg(
            Byte[] msg,
            X509Certificate2 recipientCert)
        {
            //  Place the message in a ContentInfo object.
            //  This is required to build an EnvelopedCms object.
            ContentInfo contentInfo = new ContentInfo(msg);

            //  Instantiate an EnvelopedCms object with the ContentInfo
            //  above.
            //  Has default SubjectIdentifierType IssuerAndSerialNumber.
            //  Has default ContentEncryptionAlgorithm property value
            //  RSA_DES_EDE3_CBC.
            EnvelopedCms envelopedCms = new EnvelopedCms(contentInfo);

            //  Formulate a CmsRecipient object that
            //  represents information about the recipient 
            //  to encrypt the message for.
            CmsRecipient recip1 = new CmsRecipient(
                SubjectIdentifierType.IssuerAndSerialNumber,
                recipientCert);

            String.Format(
                "Encrypting data for a single recipient of " +
                "subject name {0} ... ",
                recip1.Certificate.SubjectName.Name);
            //  Encrypt the message for the recipient.
            envelopedCms.Encrypt(recip1);
            String.Format("Done.");

            //  The encoded EnvelopedCms message contains the message
            //  ciphertext and the information about each recipient 
            //  that the message was enveloped for.
            return envelopedCms.Encode();
        }

        //  Decrypt the encoded EnvelopedCms message.
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms180959(v=vs.80).aspx
        /// </summary>
        /// <param name="encodedEnvelopedCms"></param>
        /// <returns></returns>
        static public Byte[] DecryptMsg(byte[] encodedEnvelopedCms)
        {
            //  Prepare object in which to decode and decrypt.
            EnvelopedCms envelopedCms = new EnvelopedCms();

            //  Decode the message.
            envelopedCms.Decode(encodedEnvelopedCms);

            //  Display the number of recipients the message is
            //  enveloped for; it should be 1 for this example.
            DisplayEnvelopedCms(envelopedCms, false);

            //  Decrypt the message for the single recipient.
            String.Format("Decrypting Data ... ");
            envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);
            String.Format("\nDone.");

            //  The decrypted message occupies the ContentInfo property
            //  after the Decrypt method is invoked.
            return envelopedCms.ContentInfo.Content;
        }
        //  Display the ContentInfo property of an EnvelopedCms object.
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms180959(v=vs.80).aspx
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="envelopedCms"></param>
        static private void DisplayEnvelopedCmsContent(String desc,
            EnvelopedCms envelopedCms)
        {
            string result =
                String.Format(desc + " (length {0}):  ",
                    envelopedCms.ContentInfo.Content.Length);
            foreach (byte b in envelopedCms.ContentInfo.Content)
            {
                result += String.Format(b.ToString() + " ");
            }
            result += "\n";
        }
        //  Display some properties of an EnvelopedCms object.
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms180959(v=vs.80).aspx
        /// </summary>
        /// <param name="e"></param>
        /// <param name="displayContent"></param>
        static private void DisplayEnvelopedCms(EnvelopedCms e,
            Boolean displayContent)
        {
            string result = String.Format("\nEnveloped CMS/PKCS #7 Message " +
                "Information:");
            result += String.Format(
                "\tThe number of recipients for the Enveloped CMS/PKCS " +
                "#7 is: {0}", e.RecipientInfos.Count);
            for (int i = 0; i < e.RecipientInfos.Count; i++)
            {
                result += String.Format(
                    "\tRecipient #{0} has type {1}.",
                    i + 1,
                    e.RecipientInfos[i].RecipientIdentifier.Type);
            }
            if (displayContent)
            {
                DisplayEnvelopedCmsContent("Enveloped CMS/PKCS " +
                    "#7 Content", e);
            }
            Console.WriteLine();
        }

    }
}
