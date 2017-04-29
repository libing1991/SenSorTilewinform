using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server_Http;
using System.IO;
using System.Reflection;
using IInterface;
using System.Xml;
using Server_Public;

namespace Server_Main
{
    public class MyHttpServer : HttpServer
    {
        //dll查询字典
        Dictionary<string, IInterface.IInterface> dList = new Dictionary<string, IInterface.IInterface>();
        Token_Manager tokenManager = new Token_Manager();
        //打印请求信息
        private void WriteMsg(HttpRequestInfo info)
        {
            Console.WriteLine("Request URL:" + info.http_url);
            Console.WriteLine("Post Msg:" + info.http_data);
        }
      /*  /// <summary>
        /// 加载DLL文件
        /// </summary>
        public void importDll()
        {
            //获取根目录
            string dirName = System.IO.Directory.GetCurrentDirectory();
            dirName += @"\DllDev";
            string[] files= Directory.GetFiles(dirName, "*.dll");
            foreach (string filePath in files)
            {
                var ass = System.Reflection.Assembly.LoadFrom(filePath);
               
                foreach (var t in ass.GetTypes())
                {
                    if (t.GetInterface("IInterface") != null)
                    {
                        try
                        {
                            var plugin = (IInterface.IInterface)Activator.CreateInstance(t);
                            dList.Add(plugin.url(), plugin);
                            Console.WriteLine(plugin.url() + "====>" + filePath);
                        }
                        catch 
                        {
                            Console.WriteLine("import err:" + filePath);
                        }
                    }
                }
            }  
        }
       */


        /// <summary>
        /// 加载DLL文件
        /// </summary>
        public void importDll()
        {
            //dll名字
            string dllName="";
            //dll路径
            string dllPath = "";
            //获取根目录
            string dirName = System.IO.Directory.GetCurrentDirectory();
            dirName += @"\DllDev";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dirName + @"\dllFile.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNodeList dllList = xmlRoot.SelectNodes("/dllstore/dll");
            foreach (XmlNode dllNode in dllList)
            {
                dllName = dllNode.InnerText;
                dllPath = dirName + @"\" + dllName;
                //加载dll文件
                var ass = System.Reflection.Assembly.LoadFrom(dllPath);

                foreach (var t in ass.GetTypes())
                {
                    if (t.GetInterface("IInterface") != null)
                    {
                        try
                        {
                            var plugin = (IInterface.IInterface)Activator.CreateInstance(t);
                            dList.Add(plugin.url(), plugin);
                            Console.WriteLine(plugin.url() + "====>" + dllPath);
                        }
                        catch
                        {
                            Console.WriteLine("import err:" + dllPath);
                        }
                    }
                }

            }

        }
        public MyHttpServer(int port)
            : base(port)
        {
        }
        public override void handleGETRequest(HttpProcessor p, HttpRequestInfo info)
        {
            //打印请求信息
           // WriteMsg(info);
           p.writeSuccess();
            string responseStr = "";
            try
            {
                IInterface.IInterface Interface = dList[info.http_url];
                if (Interface != null)
                {
                    responseStr = Interface.Process(info.http_data);
                }
            }
            catch {
                p.outputStream.WriteLine("url Err!");
            }
            if (responseStr != "")
                p.outputStream.WriteLine(responseStr);

            //p.outputStream.WriteLine("def");

        }

        public override void handlePOSTRequest(HttpProcessor p, HttpRequestInfo info)
        {
            //打印请求信息
           // WriteMsg(info);
            //p.outputStream.WriteLine("def");
            Console.WriteLine(info.http_data);
            p.writeSuccess();
            string responseStr = "";
            if (info.http_url == "/loginManager" | info.http_url == "/logoutManager")
            {

                try
                {
                    IInterface.IInterface Interface = dList[info.http_url];
                    if (Interface != null)
                    {
                        responseStr = Interface.Process(info.http_data);
                    }
                }
                catch
                {
                    p.outputStream.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"E2005\"></result>");
                    Console.WriteLine("<result err=\"E2005\"></result>");
                }
                if (responseStr.IndexOf("Success") != -1)
                {
                    string u_ID = responseStr.Substring(responseStr.IndexOf(":") + 1);
                    string token = tokenManager.GetGuid(u_ID, info.http_IP);
                    responseStr = "<info token=\"" + token + "\"/>";
                    responseStr = "<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"\">\r\n" + responseStr + "\r\n</result>\r\n";
                }
                Console.WriteLine(responseStr);
                p.outputStream.WriteLine(responseStr);

            }
            else if (info.http_url == "/usersPhotoManager/write" || info.http_url == "/carsPhotoManager/write" || info.http_url == "/driversPhotoManager/write")
            {
                try
                {
                    IInterface.IInterface Interface = dList[info.http_url];
                    if (Interface != null)
                    {
                        responseStr = Interface.Process(info.http_data);
                    }
                }
                catch
                {
                    Console.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"E2005\"></result>");//url
                    p.outputStream.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"E2005\"></result>");//url错误
                }
                if (responseStr != "")
                    p.outputStream.WriteLine(responseStr);
            
            }    
            else
            {
                string checkResult = "";
                //if ((checkResult=TokenCheck(info)) == "success")
                //{
                try
                {
                    IInterface.IInterface Interface = dList[info.http_url];
                    if (Interface != null)
                    {
                        responseStr = Interface.Process(info.http_data);
                    }
                }
                catch
                {
                    Console.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"E2005\"></result>");//url
                    p.outputStream.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"E2005\"></result>");
                }
                if (responseStr != "")
                    p.outputStream.WriteLine(responseStr);
                //}
                //else
                //{
                //    p.outputStream.WriteLine("<result " + GetUrlManager.getUrlManager(info.http_url) + " err=\"" +  checkResult + "\"></result>");//重新登录 过期
                //}
            }
        }
        private string TokenCheck(HttpRequestInfo info)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(info.http_data);
                XmlNode xmlTable = xmlDoc.DocumentElement.SelectNodes("/request/user_info")[0];
                string userID = xmlTable.Attributes["userId"].Value;
                string token = xmlTable.Attributes["token"].Value;

                if (tokenManager.getAllow(userID, token))
                {
                    return "success";
                }
                else
                    return "E2002";
            }
            catch
            {
                return "E1003";
            }
            


        
        }

    }
}
