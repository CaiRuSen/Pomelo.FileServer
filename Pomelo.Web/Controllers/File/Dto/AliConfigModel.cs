﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.File.Dto
{
    public class AliConfigModel
    {
        public string OSSaccessKeyId { get; set; }

        public string OSSaccessKeySecret { get; set; }

        public string OSSendpoint { get; set; }
        public string OSSbucketName { get; set; }

        public string OSSCDNUrl { get; set; }
    }
}
