using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotCyToolboxPlugins.BPL {

    public class GroupSelectionItem {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public bool IsDefaultItem { get; set; }

        public bool IsSpecialPrivileges { get; set; }

        public bool IsAllPrivileges { get; set; }
        public bool OnlyCustom { get; set; }
        public bool AuditEnabled { get; set; }
        public bool AuditDisabled { get; set; }
        public bool DuplicateDetectionEnabled { get; set; }
        public bool OnlyGlobal { get; set; }
        public bool OnlyUserTeamOwned { get; set; }
        public bool OnlyBusinessOwned { get; set; }
        public bool IsActivity { get; set; }
        public bool OnlyOrganizationOwned { get; set; }
        public bool IsActivityParty { get; set; }
        public bool IsBPFEntity { get; set; }
        public bool IsBusinessProcessEnabled { get; set; }
        public bool IsChildEntity { get; set; }
        public bool IsConnectionsEnabled { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsDocumentManagementEnabled { get; set; }
        public bool IsEnabledForCharts { get; set; }
        public bool IsEnabledForExternalChannels { get; set; }
        public bool IsEnabledForTrace { get; set; }
        public bool IsImportable { get; set; }
        public bool IsInteractionCentricEnabled { get; set; }
        public bool IsKnowledgeManagementEnabled { get; set; }
        public bool IsMailMergeEnabled { get; set; }
        public bool IsManaged { get; set; }
        public bool IsNotManaged { get; set; }
        public bool IsOfflineInMobileClient { get; set; }
        public bool IsOneNoteIntegrationEnabled { get; set; }
        public bool IsOptimisticConcurrencyEnabled { get; set; }
        public bool IsQuickCreateEnabled { get; set; }
        public bool IsReadOnlyInMobileClient { get; set; }
        public bool IsSLAEnabled { get; set; }
        public bool IsValidForAdvancedFind { get; set; }
        public bool IsValidForQueue { get; set; }
        public bool IsVisibleInMobile { get; set; }
        public bool IsVisibleInMobileClient { get; set; }
        public bool ChangeTrackingEnabled { get; set; }
        public bool CanTriggerWorkflow { get; set; }


        public override string ToString() {
            return this.Name;
        }

    } // Class: GroupSelectionItem

} // namespace: DotCyToolboxPlugins.BPL
