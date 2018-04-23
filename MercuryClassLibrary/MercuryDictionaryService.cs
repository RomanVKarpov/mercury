using Cs_Mercury.EnterpriseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Cs_Mercury
{
    [Guid("5A3FDFE7-01F0-4B79-A183-C171FAF38C0F")]
    internal interface IDictServices { }

    [Guid("56777FDA-4E7B-4CE4-8727-B1A38D06CB7C")]
    public interface IMyEvents2 { }

    [Guid("9F2494D4-B888-4D9C-8454-9905DA07F4C4"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IMyEvents2))]
    public partial class DictionaryService: IDictServices
    {
        private static EnterpriseServicePortTypeClient service = null;

        public DictionaryService()
        {

        }

        public bool Init()
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

            return true;
        }

        public string GetTest(string guid)
        {
            return EnterpriseList(guid)[0];
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

        private getBusinessEntityByGuidResponse GetBusinessEntityByGuid(string Guid)
        {
            var req = new getBusinessEntityByGuidRequest
            {
                guid = Guid
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
