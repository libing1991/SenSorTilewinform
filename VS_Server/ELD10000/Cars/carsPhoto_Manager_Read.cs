using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Server_DAL;
using Server_Public;
using IInterface;
using System.Xml;

namespace CarsPhoto_Manager
{
    class carsPhoto_Manager_Read : IInterface.IInterface
    {
        public string Process(string xmlStr)
        {
            string operation = "";//数据操作 增删改查
            string jpgFileName = "";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                try
                {
                    xmlDoc.LoadXml(xmlStr);
                    XmlNode xmlTable = xmlDoc.DocumentElement.SelectNodes("/request/database")[0];
                    operation = xmlTable.Attributes["operation"].Value;
                    //string tableStr = "users";
                    xmlTable = xmlDoc.DocumentElement.SelectNodes("/request/database/info")[0];
                    jpgFileName = xmlTable.Attributes["name"].Value;

                }
                catch
                {
                    return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E2006\">\r\n</result>";//xml
                }

                try
                {
                    jpgFileName = @"carsPhoto\" + jpgFileName;

                    string result = "<info name=\"" + Bmap_Manager.Bitmap2String(jpgFileName) + "\" />";

                    result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"\">\r\n " + result + " \r\n</result>";
                    return result;

                }
                catch
                {
                    return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E2008\">\r\n</result>";//读取图片出错
                }


            }
            catch
            {
                return "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E2008\">\r\n</result>";//读取图片出错
            }

        }

        public string url()
        {
            return "/carsPhotoManager/read";
        }
    }
}
