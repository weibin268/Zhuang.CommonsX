using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhuang.Commons.Utils;


namespace Zhuang.Commons.Test.Utils
{
    [TestClass]
    public class EncryptionUtilTest
    {
        [TestMethod]
        public void TestEncryptByMD5()
        {
            Console.WriteLine(EncryptionUtil.EncryptByMD5("zwb"));
        }
    }
}
