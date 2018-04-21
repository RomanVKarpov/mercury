using MercuryClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMercury
{
    class Program
    {
        private static string guidRu;
        private static string guidGa;
        private static string guidGo;
        private static string guidEx;

        private static string UserName;
        private static string Password;
        private static string Login;
        private static string ApiKey;
        //private static string serviceName;

        static void Main(string[] args)
        {
            Init();

            Common.SetCredential(Login, ApiKey, UserName, Password);
            Common.EndpointAddressAms = "https://api2.vetrf.ru:8002/platform/services/2.0/ApplicationManagementService";

            var res = new MercuryMainService();

            bool request = false;

            if (request)
                res.ModifyEnterpriseOperation(guidEx, "20180421_1", "Парус", "Реализация пищевых продуктов",  "создание площадки");
            else
            {
                var ApplicationId = "42ae0fc2-efcc-4158-a260-9cdcd3dcf607";
                var issuerId = guidEx;

                res.AppResponse(ApplicationId, issuerId);
            }

            var err = LastError.GetError();

            if (!err.Success)
            {
                Console.WriteLine(err.Message);
                Console.ReadKey();
            }
            else
                Console.WriteLine("Success");

        }

        /*
       // This is just to illustrate how it can be implemented on an imperative declarared binding, channel and client.

       string url = "SOME WCF URL";
       BasicHttpBinding wsBinding = new BasicHttpBinding();                
       EndpointAddress endpointAddress = new EndpointAddress(url);

       ChannelFactory<ISomeService> channelFactory = new ChannelFactory<ISomeService>(wsBinding, endpointAddress);
       channelFactory.Endpoint.Behaviors.Add(new InspectorBehavior());
       ISomeService client = channelFactory.CreateChannel();
   */

        //private static string GetLogin()
        //{
        //    string login = string.Empty;

        //    using (StreamReader reader = new StreamReader(@"c:\temp\login.txt"))
        //    {
        //        login = reader.ReadLine();
        //    }

        //    return login;
        //}

        private static void Init()
        {
            using (StreamReader reader = new StreamReader(@"c:\temp\pass.txt"))
            {
                UserName = reader.ReadLine();
                Password = reader.ReadLine();
                Login = reader.ReadLine();
                ApiKey = reader.ReadLine();

                //serviceName = reader.ReadLine();
                reader.ReadLine();
                guidRu = reader.ReadLine();
                guidGa = reader.ReadLine();
                guidGo = reader.ReadLine();
                guidEx = reader.ReadLine();
            }
        }
    }
}
