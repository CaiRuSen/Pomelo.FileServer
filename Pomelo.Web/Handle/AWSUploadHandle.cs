using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
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
    public sealed class AWSUploadHandle : IUploadFile
    {
        private static readonly AWSUploadHandle singleton = new AWSUploadHandle(); //在第一次引用类的任何成员时创建实例，公共语言运行库负责处理变量初始化 


        private static AWSConfigModel aWSConfigModel= GetAWSConfig();
        private AWSUploadHandle() { }//构造方法设置为private，防止外界利用 new 创建该类实例

        public static AWSUploadHandle GetAWSUploadHandler()
        { //提供全局唯一访问点 
            return singleton;
        }

        public async Task<string> Upload(IFormFile file, string filePath) =>await AWSUpload(file.OpenReadStream(), filePath);

         
        public async Task<string> Upload(Stream file, string filePath) => await AWSUpload(file, filePath);


        public async Task<string> Upload(string file, string filePath) => await AWSUpload(file, filePath);


         



        private async Task<string> AWSUpload(Stream file, string filePath)
        {
            try
            { 

                IAmazonS3 client = new AmazonS3Client(aWSConfigModel.OSSaccessKeyId, aWSConfigModel.OSSaccessKeySecret, RegionEndpoint.APSoutheast1);

                PutObjectRequest request = new PutObjectRequest()
                {
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = aWSConfigModel.OSSbucketName,
                    Key = filePath,
                    InputStream = file
                };
                var response = await client.PutObjectAsync(request);


                if (response.HttpStatusCode.ToString() == "OK")
                { 
                    return $"{aWSConfigModel.OSSCDNUrl}/{filePath}";
                }
                else
                {
                    return "error";
                }


            }
            catch (AmazonS3Exception s3Exception)
            {
                return $"{s3Exception.Message} | {s3Exception.InnerException}";
            }
        }
         

        private async Task<string> AWSUpload(string file, string filePath)
        {
            try
            {

                IAmazonS3 client = new AmazonS3Client(aWSConfigModel.OSSaccessKeyId, aWSConfigModel.OSSaccessKeySecret, RegionEndpoint.APSoutheast1);

                PutObjectRequest request = new PutObjectRequest()
                {
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = aWSConfigModel.OSSbucketName,
                    Key = filePath,
                    FilePath = file
                };
                var response = await client.PutObjectAsync(request);


                if (response.HttpStatusCode.ToString() == "OK")
                { 
                    return $"{aWSConfigModel.OSSCDNUrl}/{filePath}";
                }
                else
                {
                    return "error";
                } 
            }
            catch (AmazonS3Exception s3Exception)
            {
                return $"{s3Exception.Message} | {s3Exception.InnerException}";
            }
        }
         

         
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        private static AWSConfigModel GetAWSConfig()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var aliConfig = config.GetSection("AWSConfig").Get<AWSConfigModel>();

            return aliConfig;
        }
    }
}
