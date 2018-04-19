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
        private static string serviceName;

        static void Main(string[] args)
        {
            //var cmn = new MercuryClassLibrary.Common();
            //cmn.Init();

            Init();

            var res = new MercuryClassLibrary.MercuryMainService();

            bool request = false;

            if (request)
                res.ModifyEnterpriseOperation(guidGa, "Меркурий-6");
            else
            {
                var ApplicationId = "c9f830b5-f99a-4f1e-9b96-5a1436e0ac9c";
                var issuerId = guidGa;

                res.AppResponse(ApplicationId, issuerId);
            }

            var err = MercuryClassLibrary.LastError.GetError();

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

                serviceName = reader.ReadLine();
                guidRu = reader.ReadLine();
                guidGa = reader.ReadLine();
                guidGo = reader.ReadLine();
                guidEx = reader.ReadLine();
            }
        }
    }
}
