using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotCyToolboxPlugins.BPL {

    public enum PriviledgeDepthEnum {

        None = 0,
        User = 1,
        BusinessUnit = 2,
        ParentChild = 4,
        Organization = 8,

    } // enum: PriviledgeDepthEnum

} // namespace DotCyToolboxPlugins.BPL
