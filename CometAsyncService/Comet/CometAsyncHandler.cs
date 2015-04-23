using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;

namespace CometAsyncService
{
    public class CometAsyncHandler:IHttpAsyncHandler
    {

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            CometAsyncResult result = new CometAsyncResult(context, cb, extraData);

            result.Execute();

            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            CometAsyncResult cometAsyncResult = result as CometAsyncResult;

            if (cometAsyncResult != null && cometAsyncResult.ResponseObject != null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(cometAsyncResult.ResponseObject.GetType());
                cometAsyncResult.HttpContext.Response.Charset = "utf-8";
                serializer.WriteObject(cometAsyncResult.HttpContext.Response.OutputStream, cometAsyncResult.ResponseObject);
            }

            cometAsyncResult.HttpContext.Response.End();
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("ASP.NET Should never use this property");
        }

        #endregion
    }
}