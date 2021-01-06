using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pomelo.Web.Controllers.File.Dto; 
using Pomelo.Model.Base;
using Microsoft.Extensions.Configuration; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using Pomelo.Enum.Base;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using Pomelo.Common.Helper;  
using Pomelo.Web.Handle;
using Pomelo.Web.Filter;
using System.Collections.Generic;

namespace Pomelo.Web.Controllers
{
    [EnableCors("cors")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : BaseController
    {
        private IHostingEnvironment _env;

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="logger"></param> 
        public FileController(ILogger<BaseController> logger, IHostingEnvironment env) : base(logger)
        {
            _env = env;
        }


        /// <summary>
        /// 图片base64上传
        /// </summary>
        /// <param name="input"></param>
        /// <param name="AppId"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        [PomeloAuthorize]
        [HttpPost]
        public async Task<BaseResultOutput> ImageBase64Upload(Base64UploadInput input, string AppId, string AppSecret)
        {
            BaseResultOutput baseResultOutput = new BaseResultOutput();

            try
            {
                string base64string = HttpUtility.UrlDecode(input.Base64);
                base64string = base64string.Replace(" ", "+").Split(',')[1];

                byte[] bt = Convert.FromBase64String(base64string);
                MemoryStream stream = new MemoryStream(bt);


                string filePath = $"Content/Image/{DateTime.Now.ToString("yyyyMMdd")}/";
                string fileName = $"{Guid.NewGuid()}.jpg";

                string uploadUrl = await UploadFactory.CreateUpload().Upload(stream, filePath + fileName);


                baseResultOutput.Data = uploadUrl;


                return baseResultOutput;
            }
            catch (Exception ex)
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = ex.Message;
                return baseResultOutput;
            }
        }


        /// <summary>
        /// 图片文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="AppId"></param>
        /// <param name="AppSecret"></param>
        /// <param name="Width">图片宽度</param>
        /// <param name="Height">图片高度</param>
        /// <param name="WaterText">文字水印</param>
        /// <returns></returns>
        [PomeloAuthorize]
        [HttpPost]
        public async Task<BaseResultOutput> ImageFileUpload(IFormFile file, string AppId, string AppSecret, int Width, int Height, string WaterText)
        {
            BaseResultOutput baseResultOutput = new BaseResultOutput();

            try
            {
                if (file == null)
                {
                    baseResultOutput.Code = ResultCode.Succeed;
                    baseResultOutput.Data = "文件接收异常";

                    return baseResultOutput;
                }

                //图片文件校验 
                switch (file.ContentType)
                {
                    case "image/jpeg":
                        break;
                    case "image/png":
                        break;
                    default:
                        baseResultOutput.Code = ResultCode.Succeed;
                        baseResultOutput.Data = "文件格式异常";
                        return baseResultOutput;
                }


                string filePath = $"Content/Image/{DateTime.Now.ToString("yyyyMMdd")}/";
                string fileName = $"{Guid.NewGuid()}.jpg";

                string tempPath = _env.WebRootPath + $"\\temp\\{fileName}";


                Bitmap bp = new Bitmap(file.OpenReadStream());
                ImageHelper.ZoomAuto(bp, tempPath, Width, Height, WaterText, "", 100);

                string uploadUrl = await UploadFactory.CreateUpload().Upload(tempPath, filePath + fileName);


                System.IO.File.Delete(tempPath);

                baseResultOutput.Code = ResultCode.Succeed;
                baseResultOutput.Data = uploadUrl;

                return baseResultOutput;
            }
            catch (Exception ex)
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = ex.Message;
                return baseResultOutput;
            }
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="AppId"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        [PomeloAuthorize]
        [HttpPost]
        public async Task<BaseResultOutput> FileUpload(IFormFile file, string AppId, string AppSecret)
        {
            BaseResultOutput baseResultOutput = new BaseResultOutput();

            try
            {
                if (file == null)
                {
                    baseResultOutput.Code = ResultCode.Succeed;
                    baseResultOutput.Data = "文件接收异常";

                    return baseResultOutput;
                }

                if (!CheckFileExt(file))
                {
                    baseResultOutput.Code = ResultCode.Succeed;
                    baseResultOutput.Data = "该文件格式不允许上传";

                    return baseResultOutput;
                }

                string filePath = $"Content/File/{DateTime.Now.ToString("yyyyMMdd")}/";
                string fileName = $"{Guid.NewGuid()}.{file.FileName.Split('.').Last()}";

                string uploadUrl = await UploadFactory.CreateUpload().Upload(file, filePath + fileName);


                baseResultOutput.Code = ResultCode.Succeed;
                baseResultOutput.Data = uploadUrl;

                return baseResultOutput;
            }
            catch (Exception ex)
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = ex.Message;
                return baseResultOutput;
            }
        }




        private bool CheckFileExt(IFormFile file)
        {

            var contentTypeList = FileContentType();

            foreach (var item in contentTypeList)
            {
                if (file.ContentType.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }


        private static List<string> FileContentType()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var ext = config.GetSection("ContentType").Value.Split('|').ToList();
            return ext;
        }

    }
}