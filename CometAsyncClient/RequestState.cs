using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CometAsyncClient
{
    public class CometRequest{

        private class RequestState
        {
            public const int BUFFER_SIZE = 1024;
            public byte[] BufferRead;
            public HttpWebRequest Request;
            public HttpWebResponse Response;
            public Stream StreamResponse;
            public Type ResponseType;
            public MemoryStream RequestData;

            public RequestState()
            {
                this.BufferRead = new byte[BUFFER_SIZE];
                this.Request = null;
                this.Response = null;
                this.StreamResponse = null;
                this.ResponseType = null;
                this.RequestData = new MemoryStream();
            }

        }
        private RequestState requestState = null;

      

        public void BeginWaitRequest<T>(string url, RequestInfo info)
        {
            if (this.requestState == null)
            {
                //  ok, create the request object that is going to perform this
                //  request
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                //  and create the post data
                request.Method = "POST";
                request.ContentType = "application/json;";
                request.Timeout = 600000;
                //request.KeepAlive = true;
                
                //写入参数
                Stream requestStream = null;
                if (info != null)
                {
                    requestStream = request.GetRequestStream();

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(info.GetType());

                    serializer.WriteObject(requestStream, info);

                }
                

                this.requestState = new RequestState();

                //  get the request
                this.requestState.Request = request;
                this.requestState.ResponseType = typeof(T);
                //  and start the async request
                request.BeginGetResponse(new AsyncCallback(BeginGetResponse_Completed), this.requestState);

            }
        }


        private void BeginGetResponse_Completed(IAsyncResult result)
        {
            RequestState requestState = (RequestState)result.AsyncState;
            HttpWebRequest request = requestState.Request;

            //  and end it
            

            try
            {
                requestState.Response = (HttpWebResponse)request.EndGetResponse(result);
            }
            catch (System.Net.WebException ex)
            {
                requestState.Response = (HttpWebResponse)ex.Response;

                switch (requestState.Response.StatusCode)
                {
                    case HttpStatusCode.NotFound: // 404
                        break;

                    case HttpStatusCode.InternalServerError: // 500
                        break;

                    default:
                        throw;
                }
            }

            //  and now get the stream object
            requestState.StreamResponse = requestState.Response.GetResponseStream();

            //  and begin reading the content
            requestState.StreamResponse.BeginRead(requestState.BufferRead, 0, RequestState.BUFFER_SIZE, new AsyncCallback(BeginRead_Completed), requestState);
        }

        private void BeginRead_Completed(IAsyncResult result)
        {
            RequestState requestState = (RequestState)result.AsyncState;
            Stream responseStream = requestState.StreamResponse;

            int read = responseStream.EndRead(result);

            if (read > 0)
            {
                //  stick it in the other buffer
                requestState.RequestData.Write(requestState.BufferRead, 0, read);
                //  and begin reading the content
                requestState.StreamResponse.BeginRead(requestState.BufferRead, 0, RequestState.BUFFER_SIZE, new AsyncCallback(BeginRead_Completed), requestState);
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(requestState.ResponseType);

                requestState.RequestData.Seek(0, SeekOrigin.Begin);
                object response = null;

                try
                {
                    response = serializer.ReadObject(requestState.RequestData);
                }
                catch (Exception ex)
                {
                }

                //
                //  completed here
                responseStream.Close();
                //  close the response
                requestState.Response.Close();
                requestState.RequestData.Close();
                this.requestState = null;

                //  call the end wait request
                this.EndWaitRequest(response);

                BeginWaitRequest<Object>(Program.url, new RequestInfo() { Action="reconnect",FormUserID=Program.userID});

            }
        }


        protected virtual void EndWaitRequest(object response)
        {
            string content = response as string;
            Console.WriteLine(content);
        }
    }
}
