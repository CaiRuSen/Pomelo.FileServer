using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Enum.Base
{
    public enum ResultMessage
    {

        [Description("成功")]
        Succeed = 0,


        [Description("数据不存在")]
        DataNull = 1,

        [Description("数量超出限制")]
        CountOutOfLimit = 2
    }
}
