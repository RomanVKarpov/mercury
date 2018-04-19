using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MercuryClassLibrary.ApplicationManagementService;

using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.IO;
using System.Runtime.Serialization;

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
        private ApplicationManagementServicePortTypeClient service = null;

        private static string Login = string.Empty;
        private static string ApiKey = string.Empty;

        public MercuryMainService()
        {
            var contr = new ContractDescription("EnterpriseService.EnterpriseServicePortType");

            var sec = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.Transport,
                Transport = new HttpTransportSecurity()
                {
                    ClientCredentialType = HttpClientCredentialType.Basic
                }
            };

            var binding = new BasicHttpBinding();

            binding.Security = sec;

            var addr = new EndpointAddress("https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService");
            //var endpoint = new ServiceEndpoint(contr, binding, addr);


            service = new ApplicationManagementServicePortTypeClient(binding, addr);

            //          < endpoint address = "https://api2.vetrf.ru:8002/platform/services/2.0/EnterpriseService"
            //  binding = "basicHttpBinding" bindingConfiguration = "EnterpriseServiceBinding"
            //  contract = "EnterpriseService.EnterpriseServicePortType" name = "EnterpriseServiceBindingQSPort" />

            //< endpoint address = "https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService"
            //  binding = "basicHttpBinding" bindingConfiguration = "ApplicationManagementServiceBinding"
            //  contract = "ApplicationManagementService.ApplicationManagementServicePortType"
            //  name = "ApplicationManagementServiceBindingQSPort" />

            service.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());

            //var binding = new BasicHttpBinding();
            //var endpoint = new EndpointAddress("http://example.com/myservice");
            //var channelFactory = new ChannelFactory<IMyService>(binding, endpoint);
            //var client = channelFactory.CreateChannel();
            //client.SomeServiceMethod(request);

            //var transportElement = new HttpsTransportBindingElement();
            //(transportElement).AuthenticationScheme = System.Net.AuthenticationSchemes.Basic;

            //var messageElement = new TextMessageEncodingBindingElement
            //{
            //    MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.None)
            //};
            //var binding = new CustomBinding(messageElement, transportElement);
            //service.Endpoint.Binding = binding;

            //var cred = new ClientCredentials();

            var cmn = new Common();
            cmn.Init();

            Login = cmn.Login;
            ApiKey = cmn.ApiKey;

            service.ClientCredentials.UserName.UserName = cmn.UserName;
            service.ClientCredentials.UserName.Password = cmn.Password;
        }

        public static string GetLogin() => Login;
        public static string GetApiKey() => ApiKey;

        public void ModifyEnterpriseOperation(string ownerGuid, string name)
        {
            var Ent = new Enterprise()
            {
                name = name,
                type = "1",
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
                owner = new BusinessEntity
                {
                    guid = ownerGuid
                }
            };

            var activityList = new EnterpriseActivityList();
            EnterpriseActivity[] activity = new EnterpriseActivity[1];
            activity[0] = new EnterpriseActivity
            {
                name = "Реализация пищевых продуктов"
            };

            activityList.activity = activity;

            Ent.activityList = activityList;

            Enterprise[] entList = new Enterprise[1];
            entList[0] = Ent;

            var modifyEnt = new ModifyEnterpriseRequest
            {
                localTransactionId = "20180101_3",
                initiator = new User
                {
                    login = Login
                },

                modificationOperation = new ENTModificationOperation
                {
                    type = RegisterModificationType.CREATE,
                    reason = "создание",
                    resultingList = new EnterpriseList
                    {
                        enterprise = entList
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

            var dat1 = new ApplicationDataWrapper();
            dat1.Any = data;

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
