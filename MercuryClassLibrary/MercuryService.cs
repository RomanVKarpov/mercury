using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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
            (new Enterprise[1])[0] = Ent;

            var req = new ModifyEnterpriseRequest
            {
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
                        enterprise = (new Enterprise[1])
                    }
                }
            };

            AppRequest(req);
        }

        public void AppRequest(ModifyEnterpriseRequest data1)
        {
            var mod = data1.ToString();

            var req = new submitApplicationRequest
            {
                apiKey = ApiKey,
            };
            req.application = new Application();

            req.application.data = new ApplicationDataWrapper();
            req.application.data.Any = SerializeToXmlElement(data1);

            var response = service.submitApplicationRequest(req);


            //XmlSerializer ser = new XmlSerializer(AppData.GetType());

            //string serialized = "";
            //StringBuilder sb = new StringBuilder();

            ////Serialize to memory stream
            //System.IO.TextWriter w = new System.IO.StringWriter(sb);
            //ser.Serialize(w, AppData);
            //w.Close();

            ////Read to string
            //serialized = sb.ToString();

            //var serializer = new XmlSerializer(typeof(List<Class1>), new XmlRootAttribute("root"));
            //var ms = new MemoryStream();
            //serializer.Serialize(ms, list);
            //ms.Position = 0;
            //var result = new StreamReader(ms).ReadToEnd();
        }

        public static XmlElement SerializeToXmlElement(object o)
        {
            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(o.GetType()).Serialize(writer, o);
            }

            return doc.DocumentElement;
        }
    }
}
