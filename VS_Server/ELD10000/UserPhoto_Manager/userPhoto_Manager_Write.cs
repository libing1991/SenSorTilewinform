using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Server_DAL;
using Server_Public;
using IInterface;

namespace UserPhoto_Manager
{
    public class userPhoto_Manager_Write:IInterface.IInterface
    {

        public string Process(string xmlStr)
        {
            string operation_Result = "";

            operation_Result = Bmap_Manager.Bit_Save(xmlStr, "userPhoto");
            if (operation_Result == "err")
            {
                operation_Result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"E2007\"></result>";//存储错误
            }
            else
            {
                string inQuireStr = "<info name=\"" + operation_Result + "\" />";
                operation_Result = "<result " + GetUrlManager.getUrlManager(this.url()) + " err=\"\">\r\n" + inQuireStr + "\r\n</result>\r\n"; ;
            }

            Console.WriteLine("user_Manager:");
            Console.WriteLine();
            return operation_Result;

        }

        public string url()
        {
            return "/usersPhotoManager/write";
        }
    }
}
