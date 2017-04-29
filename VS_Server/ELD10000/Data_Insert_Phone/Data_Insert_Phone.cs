using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using LitJson;
using Server_Public;
using Server_DAL;

namespace Data_Insert_Phone
{
    public class Data_Insert_Phone : IInterface.IInterface
    {
        string tableStr = "users";
        public class JsonParser
        {
            public string token;
            public string user;
            public float lon;
            public float Jlat;
            public int tap;
            public float temp;
            public float tel;
            public string RH;
            
        }
        public class Data1
        {

            public string token { get; set; }
            public string user { get; set; }
            public Double lon { get; set; }
            public Double Jlat { get; set; }
            public int tap { get; set; }
            public Double temp { get; set; }
            public string tel { get; set; }
            public Double RH { get; set; }
            public Double Vol { get;set;}
        }

        //{"token":"3434","user":"er","lon":"23.4567","Jlat":"34.5678","tap":"34","temp":"45.7","tel":"3434","RH":"34.56"}
        public string Process(string xmlStr)
        {
            Console.WriteLine("start insertdat1");
            string json = xmlStr;
            JsonData jd = JsonMapper.ToObject(json);

            Dictionary<string, string> dataDict = new Dictionary<string, string>();
           // dataDict.Add("token", (String)jd["token"]);

            string bdmap = Server_Public.BaiduMap.baiduMapChange((String)jd["lon"], (String)jd["Jlat"]);
            bdmap = System.Text.RegularExpressions.Regex.Replace(bdmap, "[[]", "");//去掉[]
            bdmap = System.Text.RegularExpressions.Regex.Replace(bdmap, "[]]", "");
            JsonData bmpjd = JsonMapper.ToObject(bdmap);


            //string s = bmpjd["result"].ToString();
           // JsonData xy = JsonMapper.ToObject(s);
            //JsonData x = (String)s[0];
            //string d = bmpjd["result"]["x"].ToString();
            //dataDict.Add();

            dataDict.Add("user", (String)jd["user"]);

            dataDict.Add("lon", bmpjd["result"]["x"].ToString());
            dataDict.Add("Jlat", bmpjd["result"]["y"].ToString());
            try
            {
                dataDict.Add("temp", (String)jd["temp"]);
            }
            catch
            { 
            }
            try
            {
                dataDict.Add("RH", (String)jd["RH"]);
            }
            catch
            {
            }
            try
            {
                dataDict.Add("tap", (String)jd["tap"]);
            }
            catch
            {
            }
            try
            {
                dataDict.Add("vol", (String)jd["vol"]);
            }
            catch
            {
            }
            
            //dataDict.Add("tel", (String)jd["tel"]);
            
            



            //Data1 dat = new Data1();
            //dat.token = (String)jd["token"];
            //dat.user=(String)jd["user"];
            
            //dat.tap=(int)jd["tap"];

            //dat.lon = (Double)jd["lon"];
            //dat.Jlat = (Double)jd["Jlat"];
            //dat.temp = (Double)jd["temp"];
            //dat.tel=(String)jd["tel"];
            //dat.RH= (Double)jd["RH"];
            Console.WriteLine("start insertdat1");
            Insert(dataDict);
            Console.WriteLine("start insertdat");
            return "dataInsertManager_OK";
        }

        public string url()
        {
            return "/dataInsertManager";
        }

        /// <summary>
        /// 将xml数据插入数据库
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private string Insert(Dictionary<string, string> dataDict)
        {
            try
            {
                Console.WriteLine("start insertdat4");
                string sqlStr = "insert into " + dataDict["user"] + GetSqlStr.getInsertSql(dataDict);
                Console.WriteLine("start insertdat5");
                try
                {
                    Console.WriteLine("start insertdat6");
                    if (DBHelper.ExecuteNonQuery(sqlStr) <= 0)
                    {
                        Console.WriteLine("start insertdat7");
                        return "E1003";
                    }
                    else
                    {
                        sqlStr = "select * from " + tableStr + " where " + GetSqlStr.getInsertSql(dataDict);
                        //inQuireStr = DataSet2Xml.dataSet2Xml(DBHelper.GetDataSet(sqlStr));
                    }
                }
                catch
                {
                    return "E1003";
                }
            }
            catch
            {
                return "E1003";
            }
            return "";
        }
    }
}
