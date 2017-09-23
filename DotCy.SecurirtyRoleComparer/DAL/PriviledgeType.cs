using DotCyToolboxPlugins.BPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Metadata;

namespace DotCyToolboxPlugins.DAL {

    public class PriviledgeType {

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string ObjectTypeCode { get; set; }

        public int AccessRightID { get; set; }
        public Nullable<AccessRightEnum> AccessRight { get; set; }

        public EntityMetadata EntityMD { get; set; }
        public string EntityDisplayName { get; set; }

        public bool CanBeBasic { get; set; }
        public bool CanBeGlobal { get; set; }
        public bool CanBeLocal { get; set; }
        public bool CanBeDeep { get; set; }
        public bool CanBeEntityReference { get; set; }
        public bool CanBeParentEntityReference { get; set; }

        public string GrouppingKey {
            get {
                if (string.IsNullOrEmpty(ObjectTypeCode) || ObjectTypeCode == "none") {
                    return this.Name;
                } else {
                    return ObjectTypeCode;
                }
            }
        }


    } // Class: PriviledgeType

} // namespace DotCyToolboxPlugins.DAL
