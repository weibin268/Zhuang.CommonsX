using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhuang.Commons.Utils;


namespace Zhuang.Commons.Test.Utils
{
    [TestClass]
    public class RandomUtilTest
    {
        [TestMethod]
        public void TestChoice()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(RandomUtil.Choice(new string[]{"zwb","abc","zhuang","hello","world"}));
            }
        }
    }
}
