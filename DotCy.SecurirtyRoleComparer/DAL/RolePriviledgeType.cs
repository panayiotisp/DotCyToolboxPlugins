using DotCyToolboxPlugins.BPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotCyToolboxPlugins.DAL {

    public class RolePriviledgeType {

        public Guid ID { get; set; }
        public Guid PrivilegeID { get; set; }
        public Nullable<PriviledgeDepthEnum> Level { get; set; }

    } // Class: RolePriviledgeType

} // namespace DotCyToolboxPlugins.DAL
