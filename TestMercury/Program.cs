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

            bool request = true;

            if (request)
                res.ModifyEnterpriseOperation(guidGa, "Сириус");
            else
            {
                var ApplicationId = "4abd3e3d-4892-483f-a23b-a15c2bcfc189";
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
