using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.HelperClasses
{
    public class Application
    {
        private static Application _instance;
        private static readonly object _lockObj = new object();
        private readonly string _applicationName;
        private readonly string _applicationVersion;
        private readonly string _environment;
        private readonly string _smitsAdminUsername;
        private readonly string _debugMode = "";

        // avoid instantiation of this class
        private Application()
        {
            _applicationName = ConfigurationManager.AppSettings["APP_NAME"] ?? "TWS";
            _environment = ConfigurationManager.AppSettings["ENVIRONMENT"] ?? "DEV";
            _smitsAdminUsername = ConfigurationManager.AppSettings["SMITS_ADMIN_USERNAME"] ?? string.Empty;
            _applicationVersion = typeof(MvcApplication).Assembly.GetName().Version.ToString();

#if DEBUG
            _debugMode = "DEBUG";
#endif
        }

        public static Application Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        _instance = new Application();
                    }
                }
                return _instance;
            }
        }

        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return _applicationVersion;
            }
        }

        public string Environment
        {
            get
            {
                return _environment;
            }
        }

        public string SmitsAdminUsername
        {
            get
            {
                return _smitsAdminUsername;
            }
        }

        public string DebugMode
        {
            get
            {
                return _debugMode;
            }
        }
    }
}