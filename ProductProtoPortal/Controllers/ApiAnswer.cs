using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProductProtoPortal.Controllers
{
    public class ApiAnswer
    {
        private static JsonSerializerSettings jss;

        static ApiAnswer()
        {
            jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jss.ContractResolver = new DefaultContractResolver() {NamingStrategy = new CamelCaseNamingStrategy()};
        }

        public string message { get; set; }
        public bool isSuccess { get; set; }
        public object result { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, jss);
        }

        public ApiAnswer(object result, string message = "", bool isOk = true)
        {
            this.result = result;
            this.message = message;
            this.isSuccess = isOk;
        }
    }
}