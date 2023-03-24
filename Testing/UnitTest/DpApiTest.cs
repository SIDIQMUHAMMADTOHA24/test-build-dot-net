using Api.Share.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    [TestClass]
    public class DpApiTest
    {
        //private readonly DpApi _dpapi = new DpApi();
        //[TestMethod]
        //public void Enkrip()
        //{
        //    var result = _dpapi.Encrypt("3cU9VlKufaGYGvS6g45chxmLGGDjkkLlnCgY11Q7iTs=");
        //    Assert.AreNotEqual(result, null);
        //}

        //[TestMethod]
        //public void Dekrip()
        //{
        //    var result = _dpapi.Decrypt("AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA4HzcoRyB+kakG7gknGi6QAQAAAACAAAAAAAQZgAAAAEAACAAAAA9YEC2s3N3BvU/FrW5vEWiwrbGE6OpBh03hwxZjDbn2wAAAAAOgAAAAAIAACAAAACPJwoab32MEjgC0DPIlrbLsPhF1TNQefstdLLRrQgSM2AAAAC0qnCOneEfyTcW7CpK9RXLTHMTgjoF3/DWOtHzlfYvHE5W3JHhMsNLN+N5XZLYgm3cuDy9SN3Z0jJpU4gvhKj8gANWMd9PyPtZdCGvfwTZCQhnwRKemeUCVN+LZfnVNu1AAAAAJYl+uyl88OIK1lzUcxWCzpcVoZ1+a8JqFmv+Dx053o7RIc5IBiCyj/b+eiCYOpM+AtsGnwxXXUprAaG2meyZsw==");
        //    Assert.AreNotEqual(result, null);
        //}

        [TestMethod]
        public void Enkrip()
        {
            Kripto kripto = new Kripto();
            var result = kripto.Hashing("enkripacoba@gmail.com");
            var result2 = kripto.Hashing("enkripacoba@gmail.com");
            Assert.AreEqual(result, result2);
        }
    }
}
