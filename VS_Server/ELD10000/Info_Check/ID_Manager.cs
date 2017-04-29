using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server_Public
{
    public class ID_Manager
    {
        public static uint get_TxtId(string txtName)
        {
            uint ID = 0;
            uint ID2 = 0;
            try
            {
                string dirName = System.IO.Directory.GetCurrentDirectory();
                dirName = dirName + @"\IDManager\" + txtName;
                using (StreamReader sr = new StreamReader(dirName, Encoding.Default))
                {
                    String line;
                    if ((line = sr.ReadLine()) != null)
                        ID = uint.Parse(line.ToString().Trim());
                    else
                        Console.WriteLine("Txt文件错误：" + txtName);
                }
                ID2 = ID + 1;
                using (FileStream fileStream = File.OpenWrite(dirName))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine(ID2.ToString());
                    }

                }
            }
            catch
            {
                Console.WriteLine("txt文件读取错误:" + txtName);
            }

            return ID;
        }

    }
}
