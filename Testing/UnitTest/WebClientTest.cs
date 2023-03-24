using Api.Share.Tools;
using App.ConnectApi.Sms;
using App.WebClient.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class WebClientTest
    {
        [TestMethod]
        public void SendSms()
        {
            SmsApi _sms = new SmsApi();
            byte[] data_auth = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format($"{Const.sms_user_name}{Const.sms_pass}082134568344")));
            string auth = BitConverter.ToString(data_auth);
            auth = auth.Replace(@"-", "").ToLower();
            string message = Const.GenerateSmsContent("1234567");
            var x = _sms.Send(Const.sms_user_name, "082134568344", message, auth, Guid.NewGuid().ToString());
            Assert.AreEqual(x, true);
        }
    }
}
