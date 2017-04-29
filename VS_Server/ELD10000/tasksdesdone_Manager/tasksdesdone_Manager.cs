using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IInterface;
using Server_DAL;
using Server_Public;
using System.Xml;
using System.Data;

namespace tasksdesdone_Manager
{
    public class tasksdesdone_Manager:IInterface.IInterface
    {
        string tableStr = "tasksdesdone";
        string inQuireStr = "";
        public string Process(string xmlStr)
        {
            string operation_Result = "";
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
                }
                catch
                {
                    Console.WriteLine("xml格式错误");
                    return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E2006\"></result>";
                }

                switch (operation)
                {
                    //插入操作
                    case "insert":
                        inQuireStr = "";
                        operation_Result = Insert(infoNodes);

                        break;

                    case "delete":
                        inQuireStr = "";
                        operation_Result = Delete(infoNodes);
                        break;

                    case "update":
                        inQuireStr = "";
                        operation_Result = Update(infoNodes);
                        break;

                    case "select":
                        inQuireStr = "";
                        operation_Result = Inquire(infoNodes);
                        break;
                    default:
                        operation_Result = "E2004";//操作类型错误！
                        break;

                }
            }
            catch
            {
                return "E1003";//数据格式错误！
            }

            if (operation_Result == "")//表明操作正确
            {
                operation_Result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"\">\r\n" + inQuireStr + "\r\n</result>\r\n";
            }
            else
            {
                operation_Result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"" + operation_Result + "\">\r\n" + "\r\n</result>\r\n";
            }


            Console.WriteLine("user_Manager:");
            Console.WriteLine(operation_Result);
            return operation_Result;

        }

        public string url()
        {
            return "/tasksdesdoneManager";
        }
        /// <summary>
        /// 查询语句
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private string Inquire(XmlNodeList nodeList)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            Boolean AllAttrNull = true;
            try
            {
                dataDict.Clear();//移除所有数据
                XmlNode infoNode = nodeList[0];
                XmlAttributeCollection xmlatt = infoNode.Attributes;
                //string d = xmlatt[1].LocalName;
                for (int i = 0; i < xmlatt.Count; i++)
                {
                    dataDict.Add(xmlatt[i].LocalName, xmlatt[i].Value.ToString());
                    if (xmlatt[i].Value.ToString() != "")
                        AllAttrNull = false;
                }
                //查询字符命令
                string sqlStr = "";
                if (!AllAttrNull)
                {
                    sqlStr = "select * from " + tableStr + " where " + GetSqlStr.getWhereSql(dataDict);
                }
                else
                {
                    sqlStr = "select * from " + tableStr + " ";
                }

                DataSet ds = DBHelper.GetDataSet(sqlStr);
                if (ds != null)
                {
                    inQuireStr = DataSet2Xml.dataSet2Xml(ds);
                }
            }
            catch
            {
                return "E1003";//数据格式错误！
            }
            return "";
        }




        /// <summary>
        /// 根据ID修改数据
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private string Update(XmlNodeList nodeList)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            try
            {
                for (int m = 0; m < nodeList.Count; m++)
                {
                    dataDict.Clear();//移除所有数据
                    XmlNode infoNode = nodeList[m];
                    XmlAttributeCollection xmlatt = infoNode.Attributes;
                    string d = xmlatt[1].LocalName;
                    for (int i = 0; i < xmlatt.Count; i++)
                    {
                        dataDict.Add(xmlatt[i].LocalName, xmlatt[i].Value.ToString());
                    }
                    string sqlStr = "UPDATE " + tableStr + " set " + GetSqlStr.getUpdateSql4NoKey(dataDict);


                    try
                    {
                        if (DBHelper.ExecuteNonQuery(sqlStr) <= 0)
                        {
                            return "";
                        }
                        else
                        {
                            sqlStr = "select * from " + tableStr + " where " + GetSqlStr.getWhereSql(dataDict);
                            inQuireStr = DataSet2Xml.dataSet2Xml(DBHelper.GetDataSet(sqlStr));
                        }
                    }
                    catch
                    {
                        return "E1003";//数据格式错误！
                    }
                }

            }
            catch
            {
                return "E1003";//数据格式错误！
            }

            return "";
        }


        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private string Delete(XmlNodeList nodeList)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            string sqlStr = "";
            try
            {
                for (int m = 0; m < nodeList.Count; m++)
                {
                    XmlNode infoNode = nodeList[m];
                    XmlAttributeCollection xmlatt = infoNode.Attributes;
                    dataDict.Clear();//移除所有数据
                    for (int i = 0; i < xmlatt.Count; i++)
                    {
                        dataDict.Add(xmlatt[i].LocalName, xmlatt[i].Value.ToString());
                    }

                    string sqlStrReq = GetSqlStr.getDeleteSql(dataDict);
                    sqlStr = "DELETE FROM " + tableStr + " where " + sqlStrReq;

                    try
                    {
                        if (DBHelper.ExecuteNonQuery(sqlStr) <= 0)
                        {
                            return "";
                        }
                        else
                        {
                            sqlStr = "select * from " + tableStr + " where " + sqlStrReq;
                            inQuireStr = DataSet2Xml.dataSet2Xml(DBHelper.GetDataSet(sqlStr));
                        }
                    }
                    catch
                    {
                        return "E1003";//数据格式错误！
                    }
                }

            }
            catch
            {
                return "E1003";//数据格式错误！
            }
            return "";
        }

        /// <summary>
        /// 将xml数据插入数据库
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private string Insert(XmlNodeList nodeList)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            string inquireStr = "";
            try
            {
                for (int m = 0; m < nodeList.Count; m++)
                {
                    dataDict.Clear();//移除所有数据
                    XmlNode infoNode = nodeList[m];
                    XmlAttributeCollection xmlatt = infoNode.Attributes;
                    for (int i = 0; i < xmlatt.Count; i++)
                    {
                        dataDict.Add(xmlatt[i].LocalName, xmlatt[i].Value.ToString());

                    }

                    //数据检查

                    //string res = "";
                    //if ((res = Server_Public.Info_Check.info_Check(tableStr, dataDict)) != "")
                    //{
                    //    return res + "";
                    //}
                    inquireStr = GetSqlStr.getDeleteSql(dataDict);
                    string sqlStr = "insert into " + tableStr + GetSqlStr.getInsertSql(dataDict);

                    try
                    {
                        if (DBHelper.ExecuteNonQuery(sqlStr) <= 0)
                        {
                            return "E1003";//数据格式错误！
                        }
                        else
                        {

                            sqlStr = "select * from " + tableStr + " where " + inquireStr;
                            inQuireStr = DataSet2Xml.dataSet2Xml(DBHelper.GetDataSet(sqlStr));
                        }
                    }
                    catch
                    {
                        return "E1003";//数据格式错误！
                    }
                }

            }
            catch
            {
                return "E1003";//数据格式错误！
            }
            return "";
        }
    }
}
 

