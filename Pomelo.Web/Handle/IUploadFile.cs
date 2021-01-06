using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Handle
{
    public interface IUploadFile
    {
        public Task<string> Upload(IFormFile file, string filePath);
         
        public Task<string> Upload(Stream file, string filePath);

        public Task<string> Upload(string file, string filePath);
    }
}
