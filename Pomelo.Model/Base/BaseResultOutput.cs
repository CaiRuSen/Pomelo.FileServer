using Pomelo.Enum.Base; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Model.Base
{
    public class BaseResultOutput
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public  ResultCode Code { get; set; } = ResultCode.Succeed;

        /// <summary>
        /// 特殊状态值
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; } = "";

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = "成功";

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = DateTime.Now.ToString();
    }
     

}
