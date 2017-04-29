using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Server_Public
{
    public class DataSet2Xml
    {
        public static string dataSet2Xml(DataSet ds)
        { 
            DataTable dt = ds.Tables[0];
            string result="";
           int colsNum=dt.Columns.Count;
            foreach (DataRow dr in dt.Rows)
            {
                string result0 = "";
                for(int i=0;i<colsNum;i++)
                {
                    result0 = result0 + dt.Columns[i].ColumnName + "=\"" + dr.ItemArray[i].ToString() + "\" ";
                }
                result =result+ "<info " + result0 + "/>\r\n";
            }
            return result;
        }
    }
}
