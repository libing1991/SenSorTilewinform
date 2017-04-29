using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Server_DAL;
using Server_Public;
using System.Xml;

namespace Login_Manager
{
    public class login_Manager:IInterface.IInterface
    {

        string tableStr = "users";
        
        public string Process(string xmlStr)
        {
            string operation_Result = "";
            bool loginState = false;
            string userId = "";
            string PWD = "";
            string operation = "";//数据操作 增删改查
            XmlNodeList infoNodes = null;//数据信息节点
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                try
                {
                    xmlDoc.LoadXml(xmlStr);
                    XmlNode xmlTable = xmlDoc.DocumentElement.SelectNodes("/request/database")[0];
                    operation = xmlTable.Attributes["operation"].Value;
                    //string tableStr = "users";
                    infoNodes = xmlDoc.DocumentElement.SelectNodes("/request/database/info");
                    userId = infoNodes[0].Attributes["userID"].Value;
                    if (infoNodes[0].Attributes["PWD"]!=null)
                    PWD = infoNodes[0].Attributes["PWD"].Value;
                }
                catch
                {
                    return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"xml\"></result>";
                }


                switch (operation)
                {
                    //插入操作
                    case "login":
                        if (Token_Manager.CheckIdLoged(userId))
                        {
                            operation_Result = "E2003";//用户已登录
                        }
                        else
                        {
                            loginState = Login(userId, PWD);
                            operation_Result = "E2001";//用户名密码错误
                        }
                        break;

                    case "logout":
                        Token_Manager.UserLogout(userId);

                        break;

                    
                    default:
                        operation_Result = "E2004";//操作类型错误
                        break;

                }
            }
            catch
            {
                return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E1003\"></result>";//数据格式错误
            }

            if (loginState)//表明操作正确
            {

               // operation_Result = "<result err=\"\">\r\n" + inQuireStr + "\r\n</result>\r\n";
                operation_Result = "Success:"+userId;
            }
            else
            {
                operation_Result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"" + operation_Result + "\">\r\n" + "\r\n</result>\r\n";
            }


            Console.WriteLine("user_Manager:");
            //Console.WriteLine(operation_Result);
            return operation_Result;

        }

        public string url()
        {
            return "/loginManager";
        }

        private  Boolean Login(string userID,string PWD)
        {
            
            try
            {
                string sqlStr = "SELECT * from " + tableStr + " where ID='" + userID + "' AND PWD = '" + PWD + "'";
                DataSet ds = DBHelper.GetDataSet(sqlStr);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        
        }
    }
}
