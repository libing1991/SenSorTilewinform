using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Server_Public
{
    public class BaiduMap
    {

        public static string baiduMapChange(string lon,string lat )
        {

            try
            {


                string strURL = "http://api.map.baidu.com/geoconv/v1/?coords="+lon+","+lat+"&from=3&to=5&ak=l7I9cu3okiezol4q8C3XmfUaTeih3x9a";
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(strURL);
                httpRequest.Timeout = 2000;
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();
                return content;

            
            }
            catch
            {
                return "";
            }

        }

    }
}
