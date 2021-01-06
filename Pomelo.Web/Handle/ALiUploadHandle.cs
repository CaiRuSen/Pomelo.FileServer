using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pomelo.Web.Controllers.File.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Handle
{
    public sealed class ALiUploadHandle : IUploadFile
    {
        private static AliConfigModel aliConfigModel = GetAliConfig();
        private static ALiUploadHandle aLiUploadHandler = new ALiUploadHandle();
        private ALiUploadHandle() { }

        public static ALiUploadHandle GetALiUploadHandler()
        {
            return aLiUploadHandler;
        }

        public async Task<string> Upload(IFormFile file, string filePath) => AliUpload(file.OpenReadStream(), filePath);


        public async Task<string> Upload(Stream file, string filePath) => AliUpload(file, filePath);


        public async Task<string> Upload(string file, string filePath) => AliUpload(file, filePath);




        /// <summary>
        /// 阿里OSS上传
        /// </summary>
        /// <param name="filePath">"Content/Images/x.jpg</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string AliUpload(Stream stream, string filePath)
        {
            try
            {  
                // 由用户指定的OSS访问地址、阿里云颁发的AccessKeyId/AccessKeySecret构造一个新的OssClient实例。
                var ossClient = new OssClient(aliConfigModel.OSSendpoint, aliConfigModel.OSSaccessKeyId, aliConfigModel.OSSaccessKeySecret);

                // 上传文件。 
                var result = ossClient.PutObject(aliConfigModel.OSSbucketName, filePath, stream);

                stream.Dispose();

                return $"https://{aliConfigModel.OSSbucketName}.{aliConfigModel.OSSendpoint}/{filePath}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }



        /// <summary>
        /// 阿里OSS上传
        /// </summary>
        /// <param name="filePath">"Content/Images/x.jpg</param>
        /// <param name="file"></param>
        /// <returns></returns>
        private string AliUpload(string file, string filePath)
        {
            try
            {

                // 由用户指定的OSS访问地址、阿里云颁发的AccessKeyId/AccessKeySecret构造一个新的OssClient实例。
                var ossClient = new OssClient(aliConfigModel.OSSendpoint, aliConfigModel.OSSaccessKeyId, aliConfigModel.OSSaccessKeySecret);

                // 上传文件。 
                var result = ossClient.PutObject(aliConfigModel.OSSbucketName, filePath, file);

                return $"https://{aliConfigModel.OSSbucketName}.{aliConfigModel.OSSendpoint}/{filePath}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }



        /// <summary>
        /// 获取阿里配置文件信息
        /// </summary>
        /// <returns></returns>
        private static AliConfigModel GetAliConfig()
        {

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var aliConfig = config.GetSection("ALiConfig").Get<AliConfigModel>();

            return aliConfig;
        }

    }
}
