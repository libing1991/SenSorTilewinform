using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Server_DAL;

namespace Server_Public 
{
    public class Info_Check 
    {
        private static string[] active = {"userID","time"};
        private static string[] user = { "ID", "name", "PWD", "IDNO", "sex", "departmentID", "phone", "address", "roleName", "createTime", "photo" };
        private static string[] authority = { "roleName", "sysAuthority", "sysUser", "sysDepartment", "sysCar", "sysTask", "sysAlarm" };
        private static string[] lost = { "userID", "operatorID", "time", "status", "remarks" };
        private static string[] taskdone = { "ID", "createTime", "endTime", "carID", "userID", "driverID", "GPS", "remarks" };
        private static string[] tasksdesdone = { "taskID", "departmentID", "userID", "oilMass", "openTime", "closeTime" };
        private static string[] taskdes = { "taskID", "departmentID", "userID", "oilMass", "openTime", "closeTime", "exStatus" };
        private static string[] departments = { "ID", "name", "phone", "address", "GPS", "remarks" };
        private static string[] alarms = { "taskID", "time", "address", "departmentID", "userID", "status", "description" };

        public static string info_Check(string database, Dictionary<string, string> dataDict)
        {
            string result = "";
            switch (database)
            { 
                case "users" :
                    result = user_Check(user,dataDict);
                    break;
                case "departments":
                    result = department_Check(departments, dataDict);
                    break;
                case "authority":

                    result = authority_Check(authority, dataDict);

                    break;
                case "active":
                    result = active_Check(active, dataDict);

                    break;
                case "lost":
                    result = active_Check(active, dataDict);

                    break;
            }
            return result;
        }
        /// <summary>
        /// 权限信息检查
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataDict"></param>
        /// <returns></returns> 
        private static string active_Check(string[] database, Dictionary<string, string> dataDict)
        {
            string result = "";

            //if ((result = num_Check(database, dataDict)) != "")
            //    return result;
            return result;
        
        }

        /// <summary>
        /// 权限信息检查
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataDict"></param>
        /// <returns></returns> 
        private static string authority_Check(string[] database, Dictionary<string, string> dataDict)
        {
            string result = "";

            //if ((result = num_Check(database, dataDict)) != "")
            //    return result;

            string sqlStr = "";
            //检测ID是否存在
            sqlStr = "SELECT * from authority where roleName='" + dataDict["roleName"] + "'";
            if (tbCheck(sqlStr))
                return "roleName";

            return result;

        }
        /// <summary>
        /// 公司信息检查
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        private static string department_Check(string[] database, Dictionary<string, string> dataDict) 
        {
            string result = "";

            //if ((result = num_Check(database, dataDict)) != "")
            //    return result;

            string sqlStr = "";
            //检测ID是否存在
            sqlStr = "SELECT * from departments where ID='" + dataDict["ID"] + "'";
            if (tbCheck(sqlStr))
                return "ID";

            return result;

        }
        /// <summary>
        /// users表检查
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        private static string user_Check(string[] database,Dictionary<string, string> dataDict)
        {
            string result = "";

            //if ((result = num_Check(database, dataDict)) != "")
            //    return result;
            
            //检查departmentID是否正确
            string sqlStr = "SELECT * from departments where ID='" + dataDict["departmentID"] + "'";
            if (!tbCheck(sqlStr))
                return "departmentID";
            //检查 roleName
            sqlStr = "SELECT * from authority where roleName='" + dataDict["roleName"] + "'";
            if (!tbCheck(sqlStr))
                return "roleName";
            //检测ID是否存在
            sqlStr = "SELECT * from users where ID='" + dataDict["ID"] + "'";
            if (tbCheck(sqlStr))
                return "ID";

            return result;

        }
        /// <summary>
        /// 检测数据个数和顺序
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataDict"></param>
        /// <returns></returns>
        private static string num_Check(string[] database, Dictionary<string, string> dataDict)
        {
            string result = "";
            int countNum = 0;
            //检查对应列个数和顺序
            if (database.Length == dataDict.Count)
                foreach (KeyValuePair<string, string> kvp in dataDict)
                {
                    if (database[countNum] != kvp.Key)
                        return database[countNum];

                    countNum++;
                }
            return result;
        }
        /// <summary>
        /// 检测某个字符串是否存在
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns> false 不存在  true 存在</returns>
        private static bool tbCheck(string sqlStr)
        {
            DataSet ds = DBHelper.GetDataSet(sqlStr);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
