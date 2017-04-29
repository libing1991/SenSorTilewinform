using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Design;
using System.Drawing;

namespace Server_Public
{
    public class Bmap_Manager
    {
        /// <summary>
        /// 存储图片到指定位置
        /// </summary>
        /// <param name="bs">BufferedStream数据流</param>
        /// <param name="Path">路径</param>
        /// <returns></returns>
        public static string Bit_Save(BufferedStream bs, string Path)
        {
            try
            {
                string dirName = "";
                int d = 0;
                while (d <= 500)
                {
                    d++;
                    dirName = System.IO.Directory.GetCurrentDirectory() + @"\mapManager\" + Path + @"\" + Token_Manager.GetGuid() + ".jpg";
                    if (File.Exists(dirName))
                        continue;
                    else
                        break;

                }
                if (d >= 500)
                    return "err";

                Bitmap bp = null;
                bp = new Bitmap(bs);
                bp.Save(dirName);

                return dirName;
            }
            catch
            {
                return "err";
            }

        }

        /// <summary>
        /// 存储图片到指定位置
        /// </summary>
        /// <param name="bs">BufferedStream数据流</param>
        /// <param name="Path">路径</param>
        /// <returns></returns>
        public static string Bit_Save(string bmpAsString, string Path)
        {
            string[] photoinfo;
            try
            {
                string fileName = "";
                string jpgPath = "";
                string dirName = "";
                //生成图片名字
                int d = 0;
                while (d <= 500)
                {
                    d++;
                    fileName = Token_Manager.GetGuid() + ".png";
                    dirName = System.IO.Directory.GetCurrentDirectory() + @"\bmpManager\" + Path + @"\" + fileName;
                    if (File.Exists(dirName))
                        continue;
                    else
                        break;
                }
                //全部重名
                if (d >= 500)
                    return "err";
                
                //分离数据   客户端上传数据格式为 ID（作为名字）+"\r\n"+图片信息
                photoinfo = bmpAsString.Split(new string[1] { "\r\n" }, System.StringSplitOptions.None);
                //jpgName = photoinfo[0] + ".png";
                byte[] imageBytes = Convert.FromBase64String(photoinfo[1]);

               // Stream st = new MemoryStream(imageBytes);
               // StreamReader sr = new StreamReader(st);
               //string name= sr.ReadLine();
               //string str= sr.ReadLine();
                jpgPath = System.IO.Directory.GetCurrentDirectory() + @"\bmpManager\" + Path + @"\" + photoinfo[0];

                //如果文件存在--删除文件
                if (File.Exists(jpgPath))
                {
                    File.Delete(jpgPath);
                }


                using (FileStream fstrm = new FileStream(dirName, FileMode.CreateNew, FileAccess.Write))
               {
                   using (BinaryWriter writer = new BinaryWriter(fstrm))
                   {
                       writer.Write(imageBytes);
                   }
               }



               //using (FileStream fstrm = new FileStream(dirName, FileMode.CreateNew, FileAccess.Write))
               //{
               //    using (BinaryWriter writer = new BinaryWriter(fstrm))
               //    {
               //        writer.Write(imageBytes);
               //    }
               //}
                //Bitmap bp = null;
                //bp = new Bitmap(bs);
                //bp.Save(dirName);

                return fileName;
            }
            catch
            {
                return "err";
            }

        }
        /// <summary>
        /// 将图片转为字符串
        /// </summary>
        /// <param name="FilePath">图片路径</param>
        /// <returns></returns>
        public static string Bitmap2String(string FilePath)
        {
            string FileFullPath = System.IO.Directory.GetCurrentDirectory() + @"\bmpManager\" + FilePath;
            using (MemoryStream ms = new MemoryStream()) {
                using (Image imageIn = Image.FromFile(FileFullPath)) 
                { 
                    using (Bitmap bmp = new Bitmap(imageIn)) 
                    { 
                        bmp.Save(ms, imageIn.RawFormat); 
                    } 
                } 
                byte[] array= ms.ToArray();
               return  Convert.ToBase64String(array);
            }
        }


    }
}
