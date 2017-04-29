using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using System.Data;
using Server_DAL;

namespace userInfo_Inquire
{
    public class userInfo_Inquire : IInterface.IInterface
    {
        
        public string Process(string xmlStr)
        {
            string retStr = "err";
            string user = "";
            string json = xmlStr;
            JsonData jd = JsonMapper.ToObject(json);
            string M = (String)jd["M"];
            user = (String)jd["user"];
            if (M.Equals("1"))
            { 
                //查询当天时间

                if (!user.Equals(""))
                    retStr = Inquiry_NowDat(user);
                else
                    retStr = "err";

            }
            else if (M.Equals("2"))
            {
                //查询最后一次
                if (!user.Equals(""))
                    retStr = Inquiry_lastDat(user);
                else
                    retStr = "err";

            }
            else if (M.Equals("3"))
            {
                //查询时间段
                string stTime = (String)jd["stTime"];
                string endTime = (String)jd["endTime"];
                if (!user.Equals(""))
                    retStr = Inquiry_TimeDat(user, stTime, endTime);
                else
                    retStr = "err";

            }


            return retStr;
        }
        private string Inquiry_NowDat(String user)
        {
            string str="";
            string sqlStr = "select * from " + user + " where " + "to_days(time) = to_days(now())";
            DataSet ds = DBHelper.GetDataSet(sqlStr);
            if (ds != null)
            {
                str = Server_Public.DatasetToJson.dataToJson(ds);
            }
            
            return str;
        
        }

        private string Inquiry_lastDat(String user)
        {
            string str = "";
            string sqlStr = "select * from "+user+" order by time desc limit 1";
            DataSet ds = DBHelper.GetDataSet(sqlStr);
            if (ds != null)
            {
                str = Server_Public.DatasetToJson.dataToJson(ds);
            }

            return str;

        }
        private string Inquiry_TimeDat(String user,String stTime,String endTime)
        {
            string str = "";
            string sqlStr = "select * from  "+user+" where time between '"+stTime+"' and '"+endTime+"'";
            DataSet ds = DBHelper.GetDataSet(sqlStr);
            if (ds != null)
            {
                str = Server_Public.DatasetToJson.dataToJson(ds);
            }

            return str;

        }


        public string url()
        {
            return "/userInfo_Inquire";
        }
    }
}
