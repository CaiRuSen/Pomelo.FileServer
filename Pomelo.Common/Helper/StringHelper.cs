using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Snowflake.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pomelo.Common.Helper
{
    public class StringHelper
    {

        private static IdWorker snowflake = new IdWorker(1, 1);
        private static object o = new object();

        /// <summary>
        /// object转换成Json字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(object data)
        {
            string Result = JsonConvert.SerializeObject(data);
            return Result;
        }


        /// <summary>
        /// Json转object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Json2Object<T>(string json)
        {
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }


        /// <summary>
        /// 读取json文件
        /// </summary>
        /// <param name="filepath">文件绝对路径</param>
        /// <returns></returns>
        public static T JsonFile2Object<T>(string filepath)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312")))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return Json2Object<T>(json);
        }



        /// <summary>
        /// list转datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable List2Table<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }


        /// <summary>
        /// List随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RandomList<T>(List<T> list)
        {
            Random random = new Random();
            List<T> newList = new List<T>();
            foreach (var item in list)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }



        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }




        /// <summary>
        /// 字符串裁切加 ...
        /// </summary>
        /// <param name="text"></param>
        /// <param name="subLength"></param>
        /// <returns></returns>
        public static string SubString(string text, int subLength)
        {
            try
            {
                int rawLength = text.Length;
                if (rawLength > subLength)
                {
                    return text.Substring(0, subLength) + "...";
                }
                else
                {
                    return text;
                }
            }
            catch (Exception)
            {
                return text;
            }
        }


        /// <summary>
        /// 雪花算法
        /// </summary>
        /// <returns></returns>
        public static string SnowflakeNo()
        {
            lock (o)
            {
                long id = snowflake.NextId();
                return id.ToString();
            }
        }


        public static string GetConfigValue(string SessionName)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string value = config.GetSection(SessionName).Value;

            return value;
        }
    }
}
