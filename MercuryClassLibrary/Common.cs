using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
//using MercuryClassLibrary.EnterpriseService;

namespace MercuryClassLibrary
{
    public  class Common
    {
        private static bool initialized = false;

        //public string guidRu;
        //public string guidGa;
        //public string guidGo;
        //public string guidEx;

        private static string userName;
        private static string userPassword;
        private static string login;
        private static string apiKey;

        public static bool Initialized { get => initialized; }

        public static string UserName { get => userName; }
        public static string UserPassword { get => userPassword; }
        public static string Login { get => login;  }
        public static string ApiKey { get => apiKey;  }
        public static string ServiceId { get; set; }

        public static string EndpointAddressAms { get; set; }
        public static string EndpointAddressDictionary { get; set; }

        //public void Init()
        //{
        //    using (System.IO.StreamReader reader = new System.IO.StreamReader(@"c:\temp\pass.txt"))
        //    {
        //        UserName = reader.ReadLine();
        //        Password = reader.ReadLine();
        //        Login = reader.ReadLine();
        //        ApiKey = reader.ReadLine();

        //        guidRu = reader.ReadLine();
        //        guidGa = reader.ReadLine();
        //        guidGo = reader.ReadLine();
        //        guidEx = reader.ReadLine();
        //    }


        //}
        static Common()
        {
            ServiceId = "mercury-g2b.service:2.0";
        }

        public static void SetCredential(string ServiceLogin, string ApiKey, string Name, string Password)
        {
            login = ServiceLogin;
            apiKey = ApiKey;

            userName = Name;
            userPassword = Password;

            initialized = true;
        }

        //public static void SetCredential(string Name, string Password)
        //{
        //    userName = Name;
        //    userPassword = Password;
        //}

        internal static string ServiceModelExceptionToString(FaultException<EnterpriseService.FaultInfo> exceptionInfo)
        {
            var err = string.Empty;
            var Detail = exceptionInfo.Detail;

            if (Detail != null)
            {
                err += Detail.message + Environment.NewLine;

                if (Detail.error != null)
                {
                    foreach (var item in Detail.error)
                    {
                        err += item.Value + Environment.NewLine;
                    }
                }
            }

            return err;
        }

        internal static string ServiceModelExceptionToString(FaultException<ApplicationManagementService.FaultInfo> exceptionInfo)
        {
            var err = string.Empty;
            var Detail = exceptionInfo.Detail;

            if (Detail != null)
            {
                err += Detail.message + Environment.NewLine;

                if (Detail.error != null)
                {
                    foreach (var item in Detail.error)
                    {
                        err += item.Value + Environment.NewLine;
                    }
                }
            }

            return err;
        }
    }
}
