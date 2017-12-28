using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhuang.Commons.Mail;
using Zhuang.Commons.Utils;

namespace Zhuang.Commons.Test.Mail
{
    [TestClass]
    public class MyMailClientTest
    {
        [TestMethod]
        public void TestSend()
        {
            MyMailClient mm = new MyMailClient("smtp.139.com", null, "13798106142@139.com", "448075543@qq.com", "13798106142@139.com", "26816455");

            mm.Send("Hello", "<h1>Hello</h1>");
        }
    }
}
