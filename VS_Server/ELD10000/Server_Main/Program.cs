using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server_Http;
using System.Threading;
using Server_Public;

namespace Server_Main
{
    class Program
    {
        private static MyHttpServer httpServer;
        static void Main(string[] args)
        {
            Console.WriteLine("Start Run Server........");
            StartServer();


        }

        /// <summary>
        /// 启动相关服务器
        /// </summary>
        public static void StartServer()
        {
            if (httpServer == null)
            {
                //添加测试token
                Token_Manager.addTestToken();
                //实例化http服务并设置端口号
                httpServer = new MyHttpServer(8080);
                //加载dll文件
                httpServer.importDll();
                //开启服务监听
                Thread thread = new Thread(new ThreadStart(httpServer.listen));
                thread.Start();
            }
        }

    }
}
