using System;
using System.Diagnostics;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests
{
    [TestFixture]
    public class TechCardTests
    {
        [Test]
        public void GetCard()
        {
            TechCardManager tcm = new TechCardManager();
            var tc = tcm.GetFromCrp("V3715/3PL");
            tc = tcm.LoadAdditionalLocal(tc);
            string s = JsonConvert.SerializeObject(tc);
            Console.WriteLine(s);
            Debug.WriteLine(s);
            int g = 0;
        }
    }
}