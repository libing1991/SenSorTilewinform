using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_Public
{
   public class Token_Manager
    {
       private static Dictionary<string, string> userToken = new Dictionary<string, string>();
       private static Dictionary<string, DateTime> userTime = new Dictionary<string, DateTime>();
       private static int tokenDelayTime = 1000;

       public static void addTestToken()
       {
           userToken.Add("0", "1");
           userTime.Add("0", DateTime.Now);
       }



       /// <summary>
       /// 检查该ID是否已经登录
       /// </summary>
       /// <param name="userId"></param>
       /// <returns>T:已登录 F：未登录</returns>
      public static bool CheckIdLoged(string userId)
      {
          updateAllToken();
          try
          {
              string d = userToken[userId];
              return true;
          }
          catch
          {
              return false;
          }
      }
       /// <summary>
       /// 用户退出
       /// </summary>
       /// <param name="userId"></param>
      public static void UserLogout(string userId)
      {
          try
          {
              userToken.Remove(userId);
              userTime.Remove(userId);
          }
          catch
          { 
          
          }

      }
       
       
       /// <summary>
       /// 获取Token
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
       public string GetGuid(string userId,string Ip)
       {
           string Token = GuidTo16String()+Ip;

           try
           {
           if (userToken[userId] != "")
           {
               userToken[userId] = Token;
               userTime[userId] = DateTime.Now;
           }
           }
           catch
           {
               userToken.Add(userId,Token);
               userTime.Add(userId, DateTime.Now);
           }

           return Token;
       }
       /// <summary>
       /// 更新某一个Token操作
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
       public bool updataTime(string userId)
       {
           try
           {
               userTime[userId] = DateTime.Now;
           }
           catch
           {
               return false;
           }
           return true;
       }
       /// <summary>
       /// 去除失效Token
       /// </summary>
       public static void updateAllToken()
       {
           DateTime dt = DateTime.Now;
           TimeSpan ts;
           List<string> oldKey=new List<string>();

           foreach (KeyValuePair<string, DateTime> kvp in userTime)
           {
               ts = dt-kvp.Value ;
               if (ts.TotalMinutes >= tokenDelayTime)
               {
                   oldKey.Add(kvp.Key);
               }
           }

           foreach (string key in oldKey)
           { 
           userToken.Remove(key);
           userTime.Remove(key);
           }
       }
       /// <summary>
       /// 更新某一个Token
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
       public bool updateToken(string userId)
       {
           DateTime dt = DateTime.Now;
           TimeSpan ts;

           try
           {
               ts = dt-userTime[userId];
               if ((int)ts.TotalMinutes >= tokenDelayTime)
               {
                   userToken.Remove(userId);
                   userTime.Remove(userId);
                   return false;
               }
               return true;
           }
           catch
           {
               return false;
           }
           
       }
       /// <summary>
       /// 获取当前token是否有效
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="Token"></param>
       /// <returns></returns>
       public bool getAllow(string userId, string Token)
       {
           if (updateToken(userId))
           {
               string parmToken = userToken[userId];
               if (parmToken == Token)
               {
                   updataTime(userId);//更新时间
                   return true;
               }
           }
           
            return false;

       }

        /// <summary>
        /// 由连字符分隔的32位数字
        /// </summary>
        /// <returns></returns>
        public static  string GetGuid()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            return guid.ToString();
        }
        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        private string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        private long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        } 
    }
}
