using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotCyToolboxPlugins.BPL {

    public enum AccessRightEnum {

        Read = 1,
        Write = 2,
        Append = 4,
        AppendTo = 16,
        Create = 32,
        Delete = 65536,
        Share = 262144,
        Assign = 524288

    } // enum: AccessRightEnum

} // namespace DotCyToolboxPlugins.BPL
