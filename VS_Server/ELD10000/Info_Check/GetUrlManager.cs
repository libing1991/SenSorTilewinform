using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_Public
{
    public class GetUrlManager
    {
        public static string getUrlManager(string url)
        {

            string manager = url.Substring(1);
            manager = "manager=\"" + manager + "\"";
            return manager;
        }
    }
}
