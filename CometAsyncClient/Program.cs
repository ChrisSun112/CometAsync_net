using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CometAsyncClient
{
    class Program
    {
        public static string url = "http://localhost:5189/";
        public static string userID = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("是否订阅？订阅请按1 ");

            string chiose = Console.ReadLine().Trim();
            if (chiose == "1")
            {
                Subscribe();
            }

            Console.WriteLine("开始聊天....");

            while (true)
            {

                Chat();
            }


        }

        static void Subscribe()
        {
            Console.WriteLine("请输入UserID");

            userID = Console.ReadLine().Trim();

            CometRequest state = new CometRequest();

            RequestInfo info = new RequestInfo { Action = "subscribe", FormUserID = userID };

            try
            {
                state.BeginWaitRequest<Object>(url, info);
            }
            catch (Exception e)
            {
                Console.WriteLine("订阅失败。");
            }

            //Console.WriteLine("订阅成功，可直接开始聊天！");


        }

        static void Chat()
        {

            string str = Console.ReadLine();
            str = str.Substring(1, str.Length - 1);

            string[] temp = str.Split('|');

            string toUserID = temp[0];
            string message = temp[1];

            CometRequest state = new CometRequest();

            RequestInfo info = new RequestInfo() { Action = "send", FormUserID = userID };
            info.Data = new TextMessage() { ToUserID = toUserID, Message = message };


            Thread t = new Thread(
                () => { state.BeginWaitRequest<Object>(url, info); }
            );
            t.Start();


            //state.SendGET(data);
        }

    }
}
