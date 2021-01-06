using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Enum.Base
{
    public enum ResultCode:int
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Succeed = 0,

        /// <summary>
        /// 重错误
        /// </summary>
        [Description("重错误")]
        Error = 1,

        /// <summary>
        /// 轻错误
        /// </summary>
        [Description("轻错误")]
        ErrorRelax = 2,

        /// <summary>
        /// SessionKey错误
        /// </summary>
        [Description("SessionKey错误")]
        SessionKeyError = 3
    }
}
