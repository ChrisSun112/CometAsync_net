using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace CometAsyncService
{
    public class CometRequestPool
    {
        private CometAsyncResult cometResult;
        private static IDictionary<string, CometAsyncResult> CometRequestList = new Dictionary<string, CometAsyncResult>();


    }
}