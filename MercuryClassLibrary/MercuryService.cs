using Cs_Mercury.ApplicationManagementService;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Cs_Mercury
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
            //service.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 10);

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

        public submitApplicationResponse ModifyEnterpriseOperation(string ownerGuid, string transactionId, string enterpriseName, string ActivityName, string reason)
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

            return AppRequest(ownerGuid, dataObject);
        }

        public submitApplicationResponse GetVetDocumentListOperation(string IssuerId, string EnterpriseGuid, 
                    string LocalTransactionId, int vetDocumentType, int VetDocumentStatus)
        {
            var vetList = new GetVetDocumentListRequest
            {
                localTransactionId = LocalTransactionId,
                initiator = new User
                {
                    login = Common.Login
                },
                vetDocumentType = (VetDocumentType)vetDocumentType,
                vetDocumentStatus = (VetDocumentStatus)VetDocumentStatus,
                enterpriseGuid = EnterpriseGuid
            };

            var wrapper = new getVetDocumentListRequestRequest(vetList);
            var dataObject = SerializeToXmlElement(wrapper, vetList);

            return AppRequest(IssuerId, dataObject);
        }

        public submitApplicationResponse AppRequest(string issuerId, XmlElement data)
        {
            //var mod = data.ToString();

            submitApplicationResponse response = null;

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
                response = service.submitApplicationRequest(req);
            }
            catch (FaultException<FaultInfo> ex)
            {
                LastError.SetError(ex);
                Common.ServiceModelExceptionToString(ex);
            }

            return response;
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

        public T Merc_getResponse<T>(string issuerId, string applicationId) where T : class, new()
        {
            T deserialized = null;

            receiveApplicationResultResponse response = AppResponse(issuerId, applicationId);

            if (response.application.status == ApplicationStatus.IN_PROCESS)
            {
                LastError.SetError(response.application.status);
            }
            else
            {
                //int cnt = 0;
                //int max = 10;
                //bool continueRequest = true;

                //var perf1 = DateTime.Now;

                //while ((cnt < max) && (continueRequest))
                //{
                //    cnt++;

                //    response = AppResponse(issuerId, applicationId);
                //    continueRequest = (response?.application?.result == null);
                //    if (continueRequest)
                //    {
                //        Thread.Sleep(500);
                //    }
                //}

                //var perfEnd = DateTime.Now - perf1;

                deserialized = Deserialize<T>(response?.application?.result?.Any);
            }

            return deserialized;
        }

        public receiveApplicationResultResponse AppResponse(string issuerId, string applicationId)
        {
            var req = new receiveApplicationResultRequest
            {
                apiKey = ApiKey,
                issuerId = issuerId,
                applicationId = applicationId
            };

            var response = service.receiveApplicationResult(req);

            return response;
        }

        //public static T Convert<T>(XmlElement xmlres) where T : class, new()
        //{
        //    return Deserialize<T>(xmlres);
        //}

        private T Deserialize<T>(XmlNode data) where T : class, new()
        {
            if (data == null)
                return null;

            var attr = new XmlRootAttribute
            {
                ElementName = data.LocalName, // "getVetDocumentListResponse",
                Namespace = data.NamespaceURI // "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2"
            };

            var ser = new XmlSerializer(typeof(T), attr);
            using (var xmlNodeReader = new XmlNodeReader(data))
            {
                return (T)ser.Deserialize(xmlNodeReader);
            }
        }

        //public submitApplicationResponse GetSubmitResponse()
        //{
        //    submitApplicationResponse response = new submitApplicationResponse();

        //    response.application
        //}
    }
}
