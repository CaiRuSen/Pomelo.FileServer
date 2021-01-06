using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.Common.Helper;
using Pomelo.Web.Middleware;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Handle
{
    public sealed class LocalUploadHandle : IUploadFile
    {
        private static IWebHostEnvironment _env = PomeloProvider.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        private static IHttpContextAccessor _httpcontext = PomeloProvider.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        private static readonly LocalUploadHandle loaclHandler = new LocalUploadHandle();
        private LocalUploadHandle() { }


        public static LocalUploadHandle GetLoaclHandler()
        {
            return loaclHandler;
        }



        public async Task<string> Upload(IFormFile file, string filePath) => await LocalUpload(file, filePath);


        public async Task<string> Upload(Stream file, string filePath) => await LocalUpload(file, filePath);


        public async Task<string> Upload(string file, string filePath) => await LocalUpload(file, filePath);





        /// <summary>
        /// 文件本地上传
        /// </summary>
        /// <param name="filePath"></param> 
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> LocalUpload(IFormFile file, string filePath)
        {
            try
            {
                string path = _env.WebRootPath + "\\" + filePath;
                CreateDirectory(filePath);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return FixUrl(filePath);
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


        /// <summary>
        /// 文件本地上传
        /// </summary>
        /// <param name="filePath"></param> 
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> LocalUpload(Stream file, string filePath)
        {
            try
            {
                string path = _env.WebRootPath + "\\" + filePath;
                CreateDirectory(filePath);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                return FixUrl(filePath); ;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


        /// <summary>
        /// 文件本地上传
        /// </summary>
        /// <param name="filePath"></param> 
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> LocalUpload(string file, string filePath)
        {
            try
            {
                CreateDirectory(filePath);

                if (File.Exists(file))
                {
                    File.Copy(file, _env.WebRootPath + "\\" + filePath.Replace("/", "\\"));
                }

                return FixUrl(filePath);
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }



        /// <summary>
        /// base64图片本地上传,含图片压缩
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="bt"></param>
        /// <returns></returns>
        private bool Base64ImageLocalUpload(string filePath, string fileName, byte[] bt)
        {
            try
            {
                string path = _env.WebRootPath + "\\" + filePath;
                if (!Directory.Exists(path)) //判断上传文件夹是否存在，若不存在，则创建
                {
                    Directory.CreateDirectory(path); //创建文件夹
                }

                Bitmap bp = new Bitmap(Bt2Image(bt));
                ImageHelper.ZoomAuto(bp, path + "\\" + fileName, 1080, 1080, "", "", 100);

                //System.IO.File.WriteAllBytes(path + "\\" + fileName, bt);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }








        /// <summary>
        /// 根据字节流返回Image类型
        /// </summary>
        /// <param name="streamByte"></param>
        /// <returns></returns>
        private Image Bt2Image(byte[] streamByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
            Image img = Image.FromStream(ms);
            return img;
        }


        private string FixUrl(string filePath)
        {
            string http = "http://";
            if (_httpcontext.HttpContext.Request.IsHttps)
            {
                http = "https://";
            }

            return $"{http}{_httpcontext.HttpContext.Request.Host.Host}:{_httpcontext.HttpContext.Request.Host.Port}/{filePath}";
        }


        private void CreateDirectory(string filePath)
        {
            string path = _env.WebRootPath + "\\" + filePath.Replace(filePath.Split('/').Last(), "");
            if (!Directory.Exists(path)) //判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(path); //创建文件夹
            }
        }
    }
}
