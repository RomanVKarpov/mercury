using MercuryClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private string UserName;
        private string Password;
        private string Login;
        private string ApiKey;
        private string serviceName;

        private string guidRu;
        private string guidGa;
        private string guidGo;
        private string guidEx;

        [TestInitialize]
        public void TestInit()
        {
            using (StreamReader reader = new StreamReader(@"c:\temp\pass.txt"))
            {
                UserName = reader.ReadLine();
                Password = reader.ReadLine();
                Login = reader.ReadLine();
                ApiKey = reader.ReadLine();

                serviceName = reader.ReadLine();
                guidRu = reader.ReadLine();
                guidGa = reader.ReadLine();
                guidGo = reader.ReadLine();
                guidEx = reader.ReadLine();
            }

            Common.SetCredential(Login, ApiKey, UserName, Password);
            Common.EndpointAddressAms = "https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService";
            Common.EndpointAddressDictionary = "https://api2.vetrf.ru:8002/platform/services/2.0/EnterpriseService";

            //Common.InitService("https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService",
            //    "mercury-g2b.service:2.0",
            //    Login,
            //    ApiKey);
            //Common.SetCredential(UserName, Password);
        }

        [TestMethod]
        public void GetEnterpriseList()
        {
            var service = new DictionaryService();
            var res = service.EnterpriseList(guidGa);

            Assert.IsNotNull(res);
        }
    }
}
