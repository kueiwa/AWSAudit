using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole.Utilities
{
    public class OvalReader
    {
        public List<string> RetrieveSettings(string path, object creds)
        {
            List<string> result =  new List<string>();
            List<string> pathObjects = path.Split('.').ToList();
            switch (pathObjects[0])
            {
                case "Amazon":
                               Amazon.Runtime.SessionAWSCredentials cred = (Amazon.Runtime.SessionAWSCredentials)creds;
                               AmazonObjectReader AoR = new AmazonObjectReader(cred);
                               pathObjects.RemoveAt(0);
                               result = AoR.GetObjects(pathObjects);
                               break;
                //TODO:  Implement CSP reader
                //case "Microsoft": MicrosoftObjectReader MoR = new MicrosoftObjectReader ();
                //               pathObjects.RemoveAt(0);
                //               result = MoR.GetObjects(pathObjects);
                //               break;

            }
		    return result;
        }
    }
}
