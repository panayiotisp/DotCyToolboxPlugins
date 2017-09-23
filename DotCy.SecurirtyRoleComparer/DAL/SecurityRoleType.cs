using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotCyToolboxPlugins.DAL {

    public class SecurityRoleType {

        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsCustomizable { get; set; }
        public Guid RoleidUnique { get; set; }
        public bool IsManaged { get; internal set; }
        public Guid SolutionID { get; internal set; }

        public DateTime ModifiedOn { get; internal set; }
        public DateTime CreatedOn { get; internal set; }

        public string BusinessUnitName { get; set; }
        public Nullable<Guid> BusinessUnitID { get; set; }

        public string ComponentStateName { get; set; }
        public Nullable<int> ComponentStateID { get; set; }

        public string ParentRoleName { get; set; }
        public Nullable<Guid> ParentRoleID { get; set; }

        public string ParentRootRoleName { get; set; }
        public Nullable<Guid> ParentRootRoleID { get; set; }


        public override string ToString() {
            return (this.BusinessUnitName ?? "") + "\\" + (this.Name ?? "");
        }

    } // Class: SecurityRoleType

} // namespace DotCyToolboxPlugins.DAL
