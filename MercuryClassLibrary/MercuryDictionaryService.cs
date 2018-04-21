using MercuryClassLibrary.EnterpriseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MercuryClassLibrary
{
    public class DictionaryService
    {
        private static EnterpriseServicePortTypeClient service = null;

        public DictionaryService()
        {
            if (!Common.Initialized)
            {
                throw new InvalidOperationException("Сервис не инициализирован. Используйте метод InitService()");
            }

            string endpoint = Common.EndpointAddressDictionary ?? throw new ArgumentNullException(nameof(Common.EndpointAddressDictionary));

            var addr = new EndpointAddress(Common.EndpointAddressDictionary);

            var binding = new BasicHttpBinding
            {
                Security = new BasicHttpSecurity
                {
                    Mode = BasicHttpSecurityMode.Transport,
                    Transport = new HttpTransportSecurity()
                    {
                        ClientCredentialType = HttpClientCredentialType.Basic
                    }
                }
            };

            service = new EnterpriseServicePortTypeClient(binding, addr);

            service.ClientCredentials.UserName.UserName = Common.UserName;
            service.ClientCredentials.UserName.Password = Common.UserPassword;
        }

        public List<string> EnterpriseList(string guid)
        {
            List<string> result = new List<string>();

            var response = GetBusinessEntityByGuid(guid);

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

        public ResultInfo getRussianEnterpriseList(string enterpriseName)
        {
            if (string.IsNullOrEmpty(enterpriseName))
            {
                return new ResultInfo(false, "Не указано наименование предприятия");
            }

            var req = new getRussianEnterpriseListRequest
            {
                listOptions = new ListOptions
                {
                    count = "100"
                },
                enterprise = new Enterprise
                {
                    name = enterpriseName
                }
            };

            var res = new ResultInfo();

            try
            {
                var list = service.GetRussianEnterpriseList(req);
            }
            catch (System.ServiceModel.FaultException<FaultInfo> e)
            {
                res.Success = false;
                string err = Common.ServiceModelExceptionToString(e);

                res.Message = e.Detail.message + Environment.NewLine + err;
            }

            return res;
        }

        private getBusinessEntityByGuidResponse GetBusinessEntityByGuid(string guid)
        {
            var req = new getBusinessEntityByGuidRequest
            {
                guid = guid
            };

            var res = new ResultInfo();

            getBusinessEntityByGuidResponse response = null;

            try
            {
                response = service.GetBusinessEntityByGuid(req);
            }
            catch (System.ServiceModel.FaultException<FaultInfo> e)
            {
                LastError.SetError(e);
            }

            return response;
        }
    }
}
