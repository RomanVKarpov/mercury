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

        static void Main(string[] args)
        {
            //var cmn = new MercuryClassLibrary.Common();
            //cmn.Init();

            Init();

            var res = new MercuryClassLibrary.MercuryMainService();

            //res.AppRequest();

            //string login = GetLogin();
            res.ModifyEnterpriseOperation();

            //var merc = new MercuryClassLibrary.DictionaryService();

            //var res1 = merc.M_EnterpriseList(guidGa);

            //if (MercuryClassLibrary.LastError.Success())
            //{
            //    foreach (var item in res1)
            //    {
            //        Console.WriteLine(item);
            //    }
            //    Console.WriteLine(res1.Count());
            //}
            //else
            //{
            //    Console.WriteLine(MercuryClassLibrary.LastError.resultInfo.Message);
            //}


            Console.ReadKey();
        }

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

                guidRu = reader.ReadLine();
                guidGa = reader.ReadLine();
                guidGo = reader.ReadLine();
                guidEx = reader.ReadLine();
            }
        }
    }
}
