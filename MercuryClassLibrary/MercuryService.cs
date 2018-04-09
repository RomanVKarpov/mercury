﻿using System;
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
            // Do something with the SOAP request
            string req = request.ToString();
            return null;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
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
            //ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
            //client.Endpoint.Behaviors.Add(new InspectorBehavior());
            //client.GetData(123);

            service = new ApplicationManagementServicePortTypeClient();

            service.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());

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
            //(new Enterprise[1])[0] = Ent;

            Enterprise[] entList = new Enterprise[1];
            entList[0] = Ent;


            var modifyEnt = new ModifyEnterpriseRequest
            {
                localTransactionId = "20180101_2",
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

            var modifyEnt_serialized = SerializeToXmlElement(modifyEnt);

            AppRequest(ownerGuid, modifyEnt_serialized);
        }

        public void AppRequest(string issuerId, XmlElement data1)
        {
            var mod = data1.ToString();

            var req = new submitApplicationRequest
            {
                apiKey = ApiKey
            };
            req.application = new Application
            {
                serviceId = "mercury-g2b.service:2.0",
                issuerId = issuerId,
                data = new ApplicationDataWrapper()
            };
            req.application.data.Any = data1;

            req.application.issueDate = DateTime.Now;
            req.application.issueDateSpecified = true;

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

        public static XmlElement SerializeToXmlElement(object o)
        {
            XmlDocument doc = new XmlDocument();

            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("merc", "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2");
            //ns.Add("apl", "http://api.vetrf.ru/schema/cdm/application");
            //ns.Add("vd", "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2");
            //ns.Add("dt", "http://api.vetrf.ru/schema/cdm/dictionary/v2");
            //ns.Add("bs", "http://api.vetrf.ru/schema/cdm/base");
            //ns.Add("apldef", "http://api.vetrf.ru/schema/cdm/application/ws-definitions");
            //ns.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                var xml = new XmlSerializer(o.GetType(), new XmlRootAttribute("modifyEnterpriseRequest"));
                xml.Serialize(writer, o);
            }

            return doc.DocumentElement;
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
