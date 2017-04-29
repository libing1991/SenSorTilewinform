using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_Public
{
   public class Id_Time
    {
       /// <summary>
       /// 获取17位长度时间字符串
       /// </summary>
       /// <returns></returns>
       public static string getIdTimeNow()
       {
           DateTime time = DateTime.Now;
           string strId = "";
           strId = time.ToString("yyyyMMddHHmmssfff");
           return strId;
       }
    }
}
