using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Web.Controllers.File.Dto
{
    public class AuthModel
    {
        public bool IsCheck { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
