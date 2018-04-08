using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MercuryClassLibrary.EnterpriseService;

namespace MercuryClassLibrary
{
    public static class Common
    {
        private static EnterpriseServicePortTypeClient service = null;

        public static void InitService(string userName, string password)
        {
            service = new EnterpriseServicePortTypeClient();
            var cred = new System.ServiceModel.Description.ClientCredentials();

            service.ClientCredentials.UserName.UserName = userName;
            service.ClientCredentials.UserName.Password = password;
        }

        public static EnterpriseServicePortTypeClient GetService()
        {
            if (service == null)
                return null;
            else
                return service;
        }

        internal static string ServiceModelExceptionToString(FaultException<FaultInfo> exceptionInfo)
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
