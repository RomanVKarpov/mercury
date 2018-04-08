using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryClassLibrary.ApplicationManagementService;

namespace MercuryClassLibrary
{
    public class MercuryMainService
    {
        private ApplicationManagementServicePortTypeClient service = null;

        private static string Login = string.Empty;
        private static string ApiKey = string.Empty;

        public MercuryMainService()
        {

            service = new ApplicationManagementServicePortTypeClient();
            var cred = new System.ServiceModel.Description.ClientCredentials();

            var cmn = new Common();
            cmn.Init();

            Login = cmn.Login;
            ApiKey = cmn.ApiKey;

            service.ClientCredentials.UserName.UserName = cmn.UserName;
            service.ClientCredentials.UserName.Password = cmn.Password;
        }

        public static string GetLogin() => Login;
        public static string GetApiKey() => ApiKey;

        public void ModifyEnterpriseOperation()
        {
            var req = new ModifyEnterpriseRequest
            {
                initiator = new ApplicationManagementService.User
                {
                    login = Login
                }
            };

            var modificationOperation = new ApplicationManagementService.ENTModificationOperation
            {
                type = ApplicationManagementService.RegisterModificationType.FIND_OR_CREATE,
                reason = "создание"
            };

            var resultingList = new ApplicationManagementService.EnterpriseList();
            var ent = resultingList.enterprise;
        }

        public void AppRequest(ModifyEnterpriseRequest data)
        {
            var req = new ApplicationManagementService.submitApplicationRequest
            {
                apiKey = ApiKey,
            };
            req.application = new Application
            {
                data = new ApplicationDataWrapper()
            };


        }
    }
}
