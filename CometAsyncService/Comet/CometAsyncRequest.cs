using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;

namespace CometAsyncService.Comet
{
    /// <summary>
    /// 客户端请求
    /// </summary>
    public class CometAsyncRequest
    {

        private RequestInfo requestInfo;
        private CometAsyncResult result;

        public CometAsyncRequest(CometAsyncResult result)
        {
            this.result = result;
        }

        public void Execute()
        {
            requestInfo = GetRequestData();
            switch (requestInfo.Action)
            {
                case "subscribe": //订阅
                    //to do
                    break;
                case "unsubscribe": //取消订阅
                    break;
                case "send":  //单发消息
                    break;
                case "broadcase": //广播消息
                    break;
                case "reconnect"://重连接，维持http长连接
                    break;
                default:
                    throw new Exception("制定action不存在");
                   
            }
        }


        public void SendMessage(CometAsyncResult result)
        {
            if (!result.HttpContext.Request.Params.AllKeys.Contains("toUserID") || !result.HttpContext.Request.Params.AllKeys.Contains("message")) //缺失id或message，返回请求者
            {
                cometResult = result;
                cometResult.ResponseObject = "系统提示:缺少toUserID或message";

            }
            else
            {
                string id = result.HttpContext.Request["toUserID"].Trim();
                string message = result.HttpContext.Request["message"];
                string userID = result.HttpContext.Request["userID"].Trim();

                if (CometRequestList.Keys.Contains(userID))
                {
                    CometRequestList[userID] = result;
                }

                if (CometRequestList.Keys.Contains(id))
                {
                    cometResult = CometRequestList[id];
                    cometResult.ResponseObject = message;

                }
                else if (id == "all") //广播消息
                {
                    foreach (var d in CometRequestList)
                    {
                        d.Value.ResponseObject = message;
                        d.Value.SetCompleted();
                    }

                    return;
                }
                else
                {
                    cometResult = result;
                    cometResult.ResponseObject = "系统提示:指定的toUserID：" + id + "没有订阅！";
                }
            }

            cometResult.SetCompleted(); ;
        }


        public CometAsyncResult Subscribe(CometAsyncResult result)
        {
            if (!result.HttpContext.Request.Params.AllKeys.Contains("userID"))
            {
                result.ResponseObject = "系统提示:缺少userID!";
            }
            else
            {
                string userID = result.HttpContext.Request["userID"].Trim();
                if (!CometRequestList.Keys.Contains(userID))
                {
                    CometRequestList.Add(userID, result);
                }
                else
                {
                    CometRequestList[userID] = result;
                }
                result.ResponseObject = "系统提示：订阅成功！";
            }

            cometResult = result;

            return cometResult;
        }

        public CometAsyncResult Unsubscribe(CometAsyncResult result)
        {
            if (!result.HttpContext.Request.Params.AllKeys.Contains("id"))
            {
                result.ResponseObject = "系统提示:缺少ID!";
            }
            else
            {
                string id = result.HttpContext.Request["id"].Trim();
                if (CometRequestList.Keys.Contains("id"))
                {
                    CometRequestList.Remove(id);
                }

                result.ResponseObject = "系统提示：取消订阅成功！";
            }

            return cometResult;
        }


        internal void Flash(CometAsyncResult result)
        {
            if (!result.HttpContext.Request.Params.AllKeys.Contains("userID"))
            {
                result.ResponseObject = "系统提示:缺少userID!";
            }
            else
            {
                string userID = result.HttpContext.Request["userID"].Trim();
                if (!CometRequestList.Keys.Contains(userID))
                {
                    CometRequestList.Add(userID, result);
                }
                else
                {
                    CometRequestList[userID] = result;
                }

            }


        }
        /// <summary>
        /// 读取请求的数据
        /// </summary>
        /// <returns></returns>
        private RequestInfo GetRequestData()
        {

            RequestInfo info = null;
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(RequestInfo.GetType());
                info = serializer.ReadObject(result.HttpContext.Request.InputStream) as RequestInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("解析请求数据异常，"+ex.Message);
            }

            return info;
        }

        public RequestInfo RequestInfo
        {
            get { return this.requestInfo; }
        }
    }
}