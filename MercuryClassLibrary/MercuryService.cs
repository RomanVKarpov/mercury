using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercuryClassLibrary
{
    public class MercuryMainService
    {
        public void ModifyEnterpriseOperation(string login)
        {
            var req = new ApplicationManagementService.ModifyEnterpriseRequest();
            req.initiator = new ApplicationManagementService.User
            {
                login = login
            };

            var modificationOperation = new ApplicationManagementService.ENTModificationOperation
            {
                type = ApplicationManagementService.RegisterModificationType.FIND_OR_CREATE,
                reason = "создание"
            };

            var resultingList = new ApplicationManagementService.EnterpriseList();
            var ent = resultingList.enterprise;

            //var oper = new ApplicationManagementService.

        }
    }
}
