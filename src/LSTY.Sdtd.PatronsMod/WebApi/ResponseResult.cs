using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace LSTY.Sdtd.PatronsMod.WebApi
{
    /// <summary>
    /// Custom status code in controller and module
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// Succeeded
        /// </summary>
        Succeeded = 200,

        /// <summary>
        /// Redirect
        /// </summary>
        Redirect = 302,

        /// <summary>
        /// Failed
        /// </summary>
        Failed = 500,

        /// <summary>
        /// Refresh page
        /// </summary>
        Refresh = 1000
    }

    /// <summary>
    /// Custom response result
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// Custom status code in controller and module
        /// </summary>
        [JsonProperty("code")]
        public StatusCode Code;

        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public string Title;

        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty("message")]
        public string Message;

        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("data")]
        public object Data;

        public ResponseResult()
        {
            Code = StatusCode.Failed;
        }

        public Response ToResponse(HttpStatusCode httpStatusCode)
        {
            return new Response()
            {
                StatusCode = httpStatusCode,
                ContentType = "application/json;charset=utf-8",
                Contents = (stream) =>
                {
                    string json = JsonConvert.SerializeObject(this);

                    byte[] data = Encoding.UTF8.GetBytes(json);

                    stream.Write(data, 0, data.Length);
                }
            };
        }
    }
}