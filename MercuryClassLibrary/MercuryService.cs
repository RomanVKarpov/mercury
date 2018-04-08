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

        public void ModifyEnterpriseOperation()
        {
            var req = new ModifyEnterpriseRequest
            {
                initiator = new User
                {
                    login = Login
                },

                modificationOperation = new ENTModificationOperation
                {
                    type = RegisterModificationType.CREATE,
                    reason = "создание"
                },
            };

            

            var resultingList = new EnterpriseList();

            req.modificationOperation.resultingList = resultingList;


            var ent = resultingList.enterprise;

            AppRequest(req);
        }

        public void AppRequest(ModifyEnterpriseRequest data1)
        {
            var mod = data1.ToString();

            var req = new ApplicationManagementService.submitApplicationRequest
            {
                apiKey = ApiKey,
            };
            req.application = new Application();

            req.application.data.Any = SerializeToXmlElement(data1);


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
