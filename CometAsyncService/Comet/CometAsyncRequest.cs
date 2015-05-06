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
        private CometAsyncResult responseCometResult;

        public CometAsyncRequest(CometAsyncResult result)
        {
            this.responseCometResult = result;
        }

        public void Execute()
        {
            requestInfo = GetRequestData();
            if (string.IsNullOrEmpty(requestInfo.FormUserID))
            {
                responseCometResult.ResponseObject = "系统提示:缺少FromUserID";
                responseCometResult.SetCompleted();
                return;
            }
            else if (string.IsNullOrEmpty(requestInfo.Action)) 
            {
                responseCometResult.ResponseObject = "系统提示:缺少Action";
                responseCometResult.SetCompleted();
                return;
            }
            switch (requestInfo.Action)
            {
                case "subscribe": //订阅
                    //to do
                    break;
                case "unsubscribe": //取消订阅
                    break;
                case "send":  //单发消息
                    SendMessage();
                    break;
                case "broadcase": //广播消息
                    break;
                case "reconnect"://重连接，维持http长连接
                    break;
                default:
                    throw new Exception("指定action不存在");
                   
            }
        }


        public void SendMessage()
        {

            TextMessage textMessage = requestInfo.Data as TextMessage;

            if (CometRequestPool.IsExistKey(requestInfo.FormUserID))
            {
                CometRequestPool.Modify(requestInfo.FormUserID, responseCometResult);
            }
            else
            {
                responseCometResult.ResponseObject = "系统提示:您没有订阅";
                responseCometResult.SetCompleted();
                return;

            }

            CometAsyncResult result = null;
            if (CometRequestPool.IsExistKey(textMessage.ToUserID))
            {
                result = CometRequestPool.Get(textMessage.ToUserID);
                result.ResponseObject = textMessage.Message;
                result.SetCompleted();
            }
            else
            {
                responseCometResult.ResponseObject = "系统提示:指定的UserID不存在";
                responseCometResult.SetCompleted();
            }
           
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void Subscribe()
        {

            if (!CometRequestPool.IsExistKey(requestInfo.FormUserID))
            {
                CometRequestPool.Add(requestInfo.FormUserID, responseCometResult);
            }
            else
            {
                CometRequestPool.Modify(requestInfo.FormUserID, responseCometResult);
            }

            responseCometResult.ResponseObject = "系统提示：订阅成功！";

            responseCometResult.SetCompleted();
            
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void Unsubscribe()
        {
            if (CometRequestPool.IsExistKey(requestInfo.FormUserID))
            {
                CometRequestPool.Remove(requestInfo.FormUserID);           
            }
            responseCometResult.ResponseObject = "系统提示：订阅成功！";
            responseCometResult.SetCompleted();
        }


        public void Reconnect()
        {


            if (CometRequestPool.IsExistKey(requestInfo.FormUserID))
            {
                CometRequestPool.Modify(requestInfo.FormUserID, responseCometResult);
            }
            else
            {
                responseCometResult.ResponseObject = "系统提示：用户" + requestInfo.FormUserID + "没有订阅";
                responseCometResult.SetCompleted();
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
                info = serializer.ReadObject(responseCometResult.HttpContext.Request.InputStream) as RequestInfo;
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