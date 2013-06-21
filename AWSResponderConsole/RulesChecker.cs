using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using log4net;
using log4net.Config;

namespace AWSResponderConsole
{
    public class RulesChecker
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RulesChecker()
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ThreadContext.Properties["SessionID"] = Environment.UserDomainName + "\\" + Environment.UserName;

        }
        private void ReadRules()
        {

        }

    }
}
