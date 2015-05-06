using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace CometAsyncService
{
    /// <summary>
    /// CometAsyncResult Pool
    /// </summary>
    public class CometRequestPool
    {
        private CometAsyncResult cometResult;
        private static IDictionary<string, CometAsyncResult> cometRequestList = new Dictionary<string, CometAsyncResult>();

        /// <summary>
        /// pool中是否存在key的CometAsyncResult
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExistKey(string key)
        {
            return cometRequestList.Keys.Contains(key);
        }

        /// <summary>
        /// 增加CometAsyncResult到pool
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        public static void Add(string key,CometAsyncResult result)
        {
                    
            cometRequestList.Add(key,result);
            
        }

        public static void Remove(string key)
        {
            
            cometRequestList.Remove(key);
            
        }

        public static CometAsyncResult Get(string key)
        {
            return cometRequestList[key];
        }

        public static void Modify(string key, CometAsyncResult result)
        {
            cometRequestList[key] = result;
        }
    }
}