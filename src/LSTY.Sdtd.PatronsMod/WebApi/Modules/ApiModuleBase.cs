using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class ApiModuleBase : NancyModule
    {
        public ApiModuleBase() : base("/api")
        {
        }

        protected void HttpGet(string path, string operationId, Func<dynamic, Response> action)
        {
            Get<Response>(path, action, null, operationId);
        }

        protected void HttpPost(string path, string operationId, Func<dynamic, Response> action)
        {
            Post<Response>(path, action, null, operationId);
        }

        protected TModel Bind<TModel>()
        {
            try
            {
                string json = null;

                using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                TModel obj = JsonConvert.DeserializeObject<TModel>(json);

                return obj;
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Body bind error");
                return default(TModel);
            }
        }

        #region SucceededResult
        protected virtual Response SucceededResult(object data = null, string message = null, string title = "OK")
        {
            ResponseResult responseResult = new ResponseResult()
            {
                Code = StatusCode.Succeeded,
                Data = data,
                Message = message,
                Title = title,
            };

            return responseResult.ToResponse(HttpStatusCode.OK);
        }
        #endregion

        #region FailedResult
        protected virtual Response FailedResult(object data = null, string message = null, string title = "Error")
        {
            ResponseResult responseResult = new ResponseResult()
            {
                Code = StatusCode.Failed,
                Data = data,
                Message = message,
                Title = title,
            };

            return responseResult.ToResponse(HttpStatusCode.OK);
        }
        #endregion

        #region RefreshResult
        protected virtual Response RefreshResult(string message = null, string title = "Error")
        {
            ResponseResult responseResult = new ResponseResult()
            {
                Code = StatusCode.Refresh,
                Data = null,
                Message = message,
                Title = title,
            };

            return responseResult.ToResponse(HttpStatusCode.OK);
        }
        #endregion
    }
}
