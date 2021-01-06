using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Web.Controllers
{  
    /// <summary>
    /// Home控制器，用于页面跳转
    /// </summary>
   
    public class HomeController : ControllerBase
    {
        

        public IActionResult Admin()
        {
            //默认根目录为 wwwroot
            return File("admin/index.html", "text/html");
        }


      
    }
}