using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vrh.CheckSuccess;
using ReelCheck.Core.Configuration;

namespace Vrh.CheckSuccess.Test
{
    [TestClass]
    public class CheckSuccessTest: Vrh.UnitTest.Base.VrhUnitTestBaseClass
    {
        [TestMethod]
        public void DoTest()
        {
            ReelcheckConfiguration config = new ReelcheckConfiguration("Config.xml");
            CheckSuccessType cs = config.CheckSuccess;

            Dictionary<string, string> dataElements = new Dictionary<string, string>();
            dataElements.Add("FVSCODE", "1234567890");
            dataElements.Add("MTSCODE", "0987654321");

            bool result = CheckSuccess.DoCheck(cs, dataElements, CheckSuccess.CheckOperations.AND);
            Assert.AreEqual(true, result);
        }
    }
}
