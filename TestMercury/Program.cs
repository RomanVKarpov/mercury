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
        static void Main(string[] args)
        {
            var merc = Init();

            string guidGalimova = "2084a15e-e93d-4b36-a604-412681a5da16";

            var res = new MercuryClassLibrary.MercuryMainService();

            string login = GetLogin();
            res.ModifyEnterpriseOperation(login);

            //var res = merc.Cs_EnterpriseExistByHs(guidGalimova);

            //if (MercuryClassLibrary.LastError.Success())
            //{
            //    foreach (var item in res)
            //    {
            //        Console.WriteLine(item);
            //    }
            //    Console.WriteLine(res.Count());
            //}
            //else
            //{
            //    Console.WriteLine(MercuryClassLibrary.LastError.resultInfo.Message);
            //}


            Console.ReadKey();
        }

        private static string GetLogin()
        {
            string login = string.Empty;

            using (StreamReader reader = new StreamReader(@"c:\temp\login.txt"))
            {
                login = reader.ReadLine();
            }

            return login;
        }

        private static MercuryClassLibrary.DictionaryService Init()
        {
            string UserName = "";
            string Password = "";

            using (StreamReader reader = new StreamReader(@"c:\temp\pass.txt"))
            {
                UserName = reader.ReadLine();
                Password = reader.ReadLine();
            }

            return new MercuryClassLibrary.DictionaryService(UserName, Password);
        }
    }
}
