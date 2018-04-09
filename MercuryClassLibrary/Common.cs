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
        public  string guidRu;
        public string guidGa;
        public string guidGo;
        public string guidEx;

        internal  string UserName;
        internal  string Password;
        internal  string Login;
        internal  string ApiKey;

        public void Init()
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(@"c:\temp\pass.txt"))
            {
                UserName = reader.ReadLine();
                Password = reader.ReadLine();
                Login = reader.ReadLine();
                ApiKey = reader.ReadLine();

                guidRu = reader.ReadLine();
                guidGa = reader.ReadLine();
                guidGo = reader.ReadLine();
                guidEx = reader.ReadLine();
            }
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
