﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometAsyncClient
{
    public class RequestInfo
    {
        //action,包括订阅、取消订阅、单发发送消息、广播消息(subscribe,unsubscribe,send、broadcase)
        public string Action { get; set; }

        public string FormUserID { get; set; }

        public TextMessage Data { get; set; }
    }
}
