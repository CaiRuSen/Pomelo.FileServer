using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Handle
{
    public class UploadFactory
    {
        private static readonly string config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetSection("UploadConfig").Value;

        private static IUploadFile uploads = null;

        public static IUploadFile CreateUpload()
        {
            switch (config)
            {
                case "ALiConfig":
                    uploads =   ALiUploadHandle.GetALiUploadHandler();
                    break;
                case "QiNiuConfig":
                    uploads =   QiniuUploadHandle.GetQiniuUploadHandler();
                    break;
                case "AWSConfig":
                    uploads = AWSUploadHandle.GetAWSUploadHandler();
                    break;
                default:
                    uploads = LocalUploadHandle.GetLoaclHandler();
                    break;
            }

            return uploads;

        }
    }
}
