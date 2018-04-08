using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercuryClassLibrary
{
    public class DictionaryService
    {
        public DictionaryService(string userName, string password)
        {
            Common.InitService(userName, password);
        }



        public List<string> Cs_EnterpriseExistByHs(string guid)
        {
            List<string> result = new List<string>();

            var response = GetBusinessEntityByGUID(guid);

            int cnt = 0;

            if (response != null)
            {
                if (response.businessEntity.activityLocation == null)
                {
                    LastError.SetError("Список площадок пуст");
                }
                else
                {
                    cnt = response.businessEntity.activityLocation.Count();

                    foreach (var item in response.businessEntity.activityLocation)
                    {
                        result.Add(item.enterprise.name);
                    }
                }
            }

            return result;
        }

        public ResultInfo getRussianEnterpriseListRequest(string enterpriseName)
        {
            if (string.IsNullOrEmpty(enterpriseName))
            {
                return new ResultInfo(false, "Не указано наименование предприятия");
            }

            var serv = Common.GetService();

            var req = new EnterpriseService.getRussianEnterpriseListRequest
            {
                listOptions = new EnterpriseService.ListOptions
                {
                    count = "100"
                },
                enterprise = new EnterpriseService.Enterprise
                {
                    //guid = "3560d237-8c42-abce-9bb3-d4d48b4d2130"
                    name = enterpriseName
                }
            };

            var res = new ResultInfo();

            try
            {
                var list = serv.GetRussianEnterpriseList(req);
            }
            catch (System.ServiceModel.FaultException<EnterpriseService.FaultInfo> e)
            {
                res.Success = false;
                string err = Common.ServiceModelExceptionToString(e);

                res.Message = e.Detail.message + Environment.NewLine + err;
            }

            return res;
        }

        private EnterpriseService.getBusinessEntityByGuidResponse GetBusinessEntityByGUID(string guid)
        {
            var serv = Common.GetService();

            var req = new EnterpriseService.getBusinessEntityByGuidRequest
            {
                guid = guid
            };

            var res = new ResultInfo();

            EnterpriseService.getBusinessEntityByGuidResponse response = null;

            try
            {
                response = serv.GetBusinessEntityByGuid(req);
            }
            catch (System.ServiceModel.FaultException<EnterpriseService.FaultInfo> e)
            {
                LastError.SetError(e);
            }

            return response;
        }
    }
}
