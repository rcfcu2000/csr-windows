using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels
{
    [Serializable]
    public class MResult<T>
    {

        public MResult()
        {

        }

        [JsonProperty(PropertyName = "status_code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data;
    }

    public static class MResultHelper
    {
        public static Action<MResult<object>> ConvertToMResultObject<T>(Action<MResult<T>> result) where T : class
        {
            Action<MResult<object>> action = (o) =>
            {
                if (o.Data == null)
                {
                    return;
                }
                JToken jToken = JToken.Parse(o.Data.ToString());
                T myObject = jToken.ToObject<T>();
                result?.Invoke(new MResult<T>()
                {
                    Code = o.Code,
                    Message = o.Message,
                    Data = myObject,
                });
            };
            return action;
        }
    }
}
