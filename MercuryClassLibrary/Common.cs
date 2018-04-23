using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
//using MercuryClassLibrary.EnterpriseService;

namespace Cs_Mercury
{
    [Guid("6F1011E3-3F1D-4A2A-BE9E-A5F149CB3179")]
    internal interface ICommon { }

    //[Guid("8D769D71-0059-4F81-86F1-85D62C078154")]
    //public interface IMyEvents2 { }

    [Guid("A917CDB8-5773-4101-86DD-A77E3B8ED6F7"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IMyEvents2))]
    public class Common : ICommon
    {
        private const string ServiceNameAms = "ApplicationManagementService";
        private const string ServiceNameEnterpriseService = "EnterpriseService";
        private static bool initialized = false;

        //public string guidRu;
        //public string guidGa;
        //public string guidGo;
        //public string guidEx;

        private static string userName;

        public void SetEndpoint(string EndpointName, string Address)
        {
            switch (EndpointName)
            {
                case ServiceNameAms:
                    EndpointAddressAms = Address;
                    break;
                case ServiceNameEnterpriseService:
                    EndpointAddressDictionary = Address;
                    break;
                default:
                    break;
            }
        }

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
        public Common()
        {
            ServiceId = "mercury-g2b.service:2.0";
        }

        public void SetCredential(string ServiceLogin, string ApiKey, string Name, string Password)
        {
            login = ServiceLogin;
            apiKey = ApiKey;

            userName = Name;
            userPassword = Password;

            initialized = true;
        }

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
