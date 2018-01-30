using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhuang.Commons.Utils;


namespace Zhuang.Commons.Test.Utils
{
    [TestClass]
    public class CmdUtilTest
    {
        [TestMethod]
        public void Exec()
        {
            Console.WriteLine( CmdUtil.Exec("dir"));
        }
    }
}
