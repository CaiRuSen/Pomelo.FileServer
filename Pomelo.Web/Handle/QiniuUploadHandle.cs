using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pomelo.Web.Controllers.File.Dto;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Handle
{
    public sealed class QiniuUploadHandle : IUploadFile
    {
        private static QiNiuConfigModel qnConfigModel=GetQiniuConfig();
        private static readonly QiniuUploadHandle qiniuUploadHandler = new QiniuUploadHandle();
        private QiniuUploadHandle() { }


        public static QiniuUploadHandle GetQiniuUploadHandler()
        {
            return qiniuUploadHandler;
        }

        public async Task<string> Upload(IFormFile file, string filePath) => QiNiuUpload(file.OpenReadStream(), filePath);
     

        public async Task<string> Upload(Stream file, string filePath) => QiNiuUpload(file, filePath);
       

        public async Task<string> Upload(string file, string filePath) => QiNiuUpload(file, filePath);
         


        /// <summary>
        /// 七牛OSS上传
        /// </summary>
        /// <param name="filePath">"Content/Images/x.jpg</param>
        /// <param name="file"></param>
        /// <returns></returns>
        private string QiNiuUpload(string file, string filePath)
        {
            try
            {
                Mac mac = new Mac(qnConfigModel.OSSaccessKeyId, qnConfigModel.OSSaccessKeySecret);// AK SK使用
                PutPolicy putPolicy = new PutPolicy();
                putPolicy.Scope = qnConfigModel.OSSbucketName;
                string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());//token生成

                Config config = new Config()
                {
                    Zone = Zone.ZONE_CN_South,
                    UseHttps = true
                };

                FormUploader upload = new FormUploader(config);

                var result = upload.UploadFile(file, filePath, token, null);
                if (result.Code == 200)
                {
                   
                    return $"https://{qnConfigModel.OSSCDNUrl}/{filePath}";
                }
                else
                {
                    return result.RefText;
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }




        /// <summary>
        /// 七牛OSS上传
        /// </summary>
        /// <param name="filePath">"Content/Images/x.jpg</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string QiNiuUpload(Stream stream, string filePath)
        {
            try
            { 
                Mac mac = new Mac(qnConfigModel.OSSaccessKeyId, qnConfigModel.OSSaccessKeySecret);// AK SK使用
                PutPolicy putPolicy = new PutPolicy();
                putPolicy.Scope = qnConfigModel.OSSbucketName;
                string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());//token生成

                Config config = new Config()
                {
                    Zone = Zone.ZONE_CN_South,
                    UseHttps = true
                };

                FormUploader upload = new FormUploader(config);
                 
                var result = upload.UploadStream(stream, filePath, token, null);
                if (result.Code == 200)
                {
                    stream.Dispose();
                    return $"https://{qnConfigModel.OSSCDNUrl}/{filePath}";
                }
                else
                {
                    return result.RefText;
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }




        /// <summary>
        /// 获取七牛云配置文件信息
        /// </summary>
        /// <returns></returns>
        private static QiNiuConfigModel GetQiniuConfig()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var qnConfig = config.GetSection("QiNiuConfig").Get<QiNiuConfigModel>();

            return qnConfig;
        }

    }
}
