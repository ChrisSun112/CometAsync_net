using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace CometAsyncService
{
    public class CometAsyncResult : IAsyncResult
    {
        private HttpContext context;
        private AsyncCallback callback;
        private object asyncState;
        private bool isCompleted = false;
        private object responseObject;

        public CometAsyncResult(HttpContext context, AsyncCallback callback, object asyncState)
        {
            this.callback = callback;
            this.context = context;
            this.asyncState = asyncState;
        }

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return this.asyncState; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { throw new InvalidOperationException("ASP.NET Should never use this property"); }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return this.isCompleted; }
        }

        #endregion

        public HttpContext HttpContext
        {
            get { return this.context; }
        }

        public object ResponseObject
        {
            get { return this.responseObject; }
            set { this.responseObject = value; }
        }


        internal void SetCompleted()
        {
            this.isCompleted = true;

            if (callback != null)
                callback(this);
        }

       
    }


}