using MercuryClassLibrary.ApplicationManagementService;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Serialization;

namespace MercuryClassLibrary
{
    public class InspectorBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // No implementation necessary  
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MyMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // No implementation necessary  
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // No implementation necessary  
        }
    }

    public class MyMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            //Get rid of mustUnderstand Action node
            int headerIndexOfAction = request.Headers.FindHeader("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");
            if (headerIndexOfAction > -1)
                request.Headers.RemoveAt(headerIndexOfAction);

            // Do something with the SOAP request
            string req = request.ToString();
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Do something with the SOAP reply
            string replySoap = reply.ToString();
        }
    }

    public class MercuryMainService
    {
        enum EnterpriseType
        {
            Предприятие = 1, Рынок, СББЖ, Судно
        };

        private ApplicationManagementServicePortTypeClient service = null;

        private static string Login = string.Empty;
        private static string ApiKey = string.Empty;

        public MercuryMainService()
        {
            if (!Common.Initialized)
            {
                throw new InvalidOperationException("Сервис не инициализирован. Используйте метод InitService()");
            }

            //var contr = new ContractDescription("EnterpriseService.EnterpriseServicePortType");

            //var sec = new BasicHttpSecurity
            //{
            //    Mode = BasicHttpSecurityMode.Transport,
            //    Transport = new HttpTransportSecurity()
            //    {
            //        ClientCredentialType = HttpClientCredentialType.Basic
            //    }
            //};

            //var addr = new EndpointAddress("https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService");

            var addr = new EndpointAddress(Common.EndpointAddressAms);

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

            service = new ApplicationManagementServicePortTypeClient(binding, addr);

            service.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());

            //var binding = new BasicHttpBinding();
            //var endpoint = new EndpointAddress("http://example.com/myservice");
            //var channelFactory = new ChannelFactory<IMyService>(binding, endpoint);
            //var client = channelFactory.CreateChannel();
            //client.SomeServiceMethod(request);

            //var cred = new ClientCredentials();

            Login = Common.Login;
            ApiKey = Common.ApiKey;

            service.ClientCredentials.UserName.UserName = Common.UserName;
            service.ClientCredentials.UserName.Password = Common.UserPassword;
        }

        public static string GetLogin() => Login;
        public static string GetApiKey() => ApiKey;

        public void ModifyEnterpriseOperation(string ownerGuid, string transactionId, string enterpriseName, string ActivityName, string reason)
        {
            var Ent = new Enterprise()
            {
                name = enterpriseName,
                type = ((int)EnterpriseType.Предприятие).ToString(),
                address = new Address
                {
                    country = new Country
                    {
                        guid = "74a3cbb1-56fa-94f3-ab3f-e8db4940d96b" // Россия
                    },
                    region = new Region
                    {
                        guid = "27eb7c10-a234-44da-a59c-8b1f864966de" // Челябинская обл.
                    },
                    locality = new Locality
                    {
                        guid = "a376e68d-724a-4472-be7c-891bdb09ae32" // Челябинск
                    }

                },
                activityList = new EnterpriseActivityList
                {
                    activity = new EnterpriseActivity[1]
                    {
                        new EnterpriseActivity
                        {
                            name = ActivityName
                        }
                    }
                },
                owner = new BusinessEntity
                {
                    guid = ownerGuid
                }
            };

            //Ent.activityList = new EnterpriseActivityList
            //{
            //    activity = new EnterpriseActivity[1]
            //    {
            //        new EnterpriseActivity
            //        {
            //            name = ActivityName
            //        }
            //    }
            //};

            //var activityList = new EnterpriseActivityList();
            //EnterpriseActivity[] activity = new EnterpriseActivity[1];
            //activity[0] = new EnterpriseActivity
            //{
            //    name = "Реализация пищевых продуктов"
            //};

            //activityList.activity = activity;

            //Ent.activityList = activityList;

            //Enterprise[] entList = new Enterprise[1] { Ent };
            //entList[0] = Ent;

            var modifyEnt = new ModifyEnterpriseRequest
            {
                localTransactionId = transactionId,
                initiator = new User
                {
                    login = Login
                },

                modificationOperation = new ENTModificationOperation
                {
                    type = RegisterModificationType.CREATE,
                    reason = reason,
                    resultingList = new EnterpriseList
                    {
                        enterprise = new Enterprise[1] { Ent }
                    }
                }
            };


            var wrapper = new modifyEnterpriseRequestRequest(modifyEnt);

            var dataObject = SerializeToXmlElement(wrapper, modifyEnt);

            AppRequest(ownerGuid, dataObject);
        }

        public void AppRequest(string issuerId, XmlElement data)
        {
            var mod = data.ToString();

            //var dat1 = new ApplicationDataWrapper
            //{
            //    Any = data
            //};

            var req = new submitApplicationRequest
            {
                apiKey = ApiKey,
                application = new Application
                {
                    serviceId = "mercury-g2b.service:2.0",
                    issuerId = issuerId,
                    issueDate = DateTime.Now,
                    issueDateSpecified = true,
                    data = new ApplicationDataWrapper
                    {
                        Any = data
                    }
                }
            };

            try
            {
                var response = service.submitApplicationRequest(req);
            }
            catch (System.ServiceModel.FaultException<FaultInfo> ex)
            {
                LastError.SetError(ex);
                Common.ServiceModelExceptionToString(ex);
            }


        }

        public static XmlElement SerializeToXmlElement(object wrapper, object element)
        {
            XmlTypeAttribute xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                                   element.GetType(),
                                   typeof(XmlTypeAttribute)
                                 );

            var elementNamespace = xmlAttribute.Namespace;

            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("merc", "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2");
            //ns.Add("apl", "http://api.vetrf.ru/schema/cdm/application");
            //ns.Add("vd", "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2");
            //ns.Add("dt", "http://api.vetrf.ru/schema/cdm/dictionary/v2");
            //ns.Add("bs", "http://api.vetrf.ru/schema/cdm/base");
            //ns.Add("apldef", "http://api.vetrf.ru/schema/cdm/application/ws-definitions");
            //ns.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

            var doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                var xml = new XmlSerializer(wrapper.GetType(), elementNamespace);
                xml.Serialize(writer, wrapper);
            }

            //var out1 = doc.DocumentElement.OuterXml;

            var elem = (XmlElement)doc.DocumentElement.FirstChild;

            return elem;
        }

        public void AppResponse(string applicationId, string issuerId)
        {
            var req = new receiveApplicationResultRequest
            {
                apiKey = ApiKey,
                issuerId = issuerId,
                applicationId = applicationId
            };

            var response = service.receiveApplicationResult(req);
        }
    }
}
