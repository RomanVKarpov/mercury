using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private string guidGa;

        [TestInitialize]
        public void TestInit()
        {
            var cmn = new MercuryClassLibrary.Common();
            cmn.Init();

            guidGa = cmn.guidGa;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var service = new MercuryClassLibrary.DictionaryService();
            var res = service.M_EnterpriseList(guidGa);

            Assert.IsNotNull(res);
        }
    }
}
