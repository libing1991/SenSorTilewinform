using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_Public
{
    public class GetSqlStr
    {
        /// <summary>
        /// 获取更新字符串 ""="" AND ""="" where primaryKey=""
        /// </summary>
        /// <param name="dataDict"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static string getUpdateSql(Dictionary<string, string> dataDict, string primaryKey)
        {
            int operCount = 0;
            string sqlStr = "";
            foreach (KeyValuePair<string, string> kvp in dataDict)
            {
                operCount++;
                if (operCount == dataDict.Count)
                    sqlStr = sqlStr + kvp.Key + "='" + kvp.Value + "'";
                else
                    sqlStr = sqlStr + kvp.Key + "='" + kvp.Value + "',";
            }
            sqlStr += " where " + primaryKey + "='" + dataDict[primaryKey]+"'";

            return sqlStr;
        }

        /// <summary>
        /// 获取插入字符串 "（） values（）"
        /// </summary>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        public static string getInsertSql(Dictionary<string, string> dataDict)
        {
            int operCount = 0;
            string sqlStrValue = "";
            string sqlStrName = "";
            string sqlStr = "";
            foreach (KeyValuePair<string, string> kvp in dataDict)
            {

                if (kvp.Value == "")
                    continue;
                operCount++;
                if (operCount == 1)
                {
                    sqlStrValue = sqlStrValue + "'" + kvp.Value + "'";
                    sqlStrName = sqlStrName + " " + kvp.Key + " ";
                }
                else
                {
                    sqlStrValue = sqlStrValue + " , " +"'" + kvp.Value + "'";
                    sqlStrName = sqlStrName + " , " + " " + kvp.Key + " ";
                }


            }
            sqlStr = " (" + sqlStrName + ")" + "values (" + sqlStrValue + ")";
            return sqlStr;
        }


        /// <summary>
        /// 获取删除数据条件部分
        /// </summary>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        public static string getDeleteSql(Dictionary<string, string> dataDict)
        {
            int operCount = 0;
            string sqlStr = "";
            foreach (KeyValuePair<string, string> kvp in dataDict)
            {
                
                if (kvp.Value == "")
                    continue ;
                operCount++;
                if (operCount == 1)
                    sqlStr = sqlStr + kvp.Key + "='" + kvp.Value + "'";
                else
                    sqlStr = sqlStr +" AND "+ kvp.Key + "='" + kvp.Value + "'";
            }

            return sqlStr;
        }
        /// <summary>
        /// 获取条件部分 ""="" AND ""=""
        /// </summary>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        public static string getWhereSql(Dictionary<string, string> dataDict)
        {
            int operCount = 0;
            string sqlStr = "";
            foreach (KeyValuePair<string, string> kvp in dataDict)
            {

                if (kvp.Value == "")
                    continue;
                operCount++;
                if (operCount == 1)
                    sqlStr = sqlStr + kvp.Key + "='" + kvp.Value + "'";
                else
                    sqlStr = sqlStr + " AND " + kvp.Key + "='" + kvp.Value + "'";
            }

            return sqlStr;
        }
        /// <summary>
        /// 对没有主键的数据表建立set后面部分语句
        /// </summary>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        public static string getUpdateSql4NoKey(Dictionary<string, string> dataDict)
        {
            int operCount = 0;
            string sqlStr = "";
            string ParKey = "";
            foreach (KeyValuePair<string, string> kvp in dataDict)
            {
                if (kvp.Value == "")
                    continue;
                operCount++;
                if (operCount == 1)
                {
                    sqlStr = sqlStr + kvp.Key + "='" + kvp.Value + "'";
                    ParKey = kvp.Key;
                }
                else
                    sqlStr = sqlStr + " , " + kvp.Key + "='" + kvp.Value + "'";
            }
            sqlStr += " where " + ParKey + " = '" + dataDict[ParKey] + "'";
            return sqlStr;
        }

    }
}
