using DotCyToolboxPlugins.BPL;
using DotCyToolboxPlugins.DAL;
using DotCyToolboxPlugins.Utils;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility.Interfaces;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;


namespace DotCyToolboxPlugins.Controls {

    public partial class UC_CompareRoles : PluginControlBase, IXrmToolBoxPluginControl, IGitHubPlugin, IPayPalPlugin, IHelpPlugin, IStatusBarMessenger {

        /// <summary>
        /// Executed when the security roles loading process completes
        /// </summary>
        public event EventHandler SecurityRolesCompleted;

        /// <summary>
        /// Executed when the security role comparison completes or is aborted
        /// </summary>
        public event EventHandler SecurityComparissonCompleted;

        /// <summary>
        /// The list of security roles loaded
        /// </summary>
        internal List<SecurityRoleType> SecurityRoles { get; private set; }

        /// <summary>
        /// The list of security privileges loaded
        /// </summary>
        internal List<PriviledgeType> Privileges { get; private set; }

        /// <summary>
        /// The list of roles that have been selected for comparison
        /// </summary>
        private List<Guid> ComparedRoleIDs = new List<Guid>();

        /// <summary>
        /// The privileges assigned to each of the selected for comparison roles
        /// </summary>
        private Dictionary<Guid, List<RolePriviledgeType>> PrivilegesPerRole;

        /// <summary>
        /// The metadata per entity type code
        /// </summary>
        private Dictionary<string, EntityMetadata> CachedEntityMetadata;


        #region Constructor (public)
        public UC_CompareRoles() {
            InitializeComponent();

            // add the permission groups selections
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "* All Permissions", IsAllPrivileges = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Core Records", IsDefaultItem = true, Values =
                EntityGroups.Core_Records.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Sales", Values =
                EntityGroups.Sales.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Marketing", Values =
                EntityGroups.Marketing.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Service", Values =
                EntityGroups.Service.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Business Management", Values =
                EntityGroups.BusinessManagement.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Service Management", Values =
                EntityGroups.ServiceManagement.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() {
                Name = "Customization", Values =
                EntityGroups.Customization.Replace("\r", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            });

            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Custom Entities", OnlyCustom = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Special Permissions", IsSpecialPrivileges = true });

            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Audit Enabled Entities", AuditEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Audit Disabled Entities", AuditDisabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Duplication Detection Enabled Entities", DuplicateDetectionEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Global Privileges", OnlyGlobal = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Organization Owned Entities", OnlyOrganizationOwned = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "User or Team Owned Entities", OnlyUserTeamOwned = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Business Owned Entities", OnlyBusinessOwned = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Activity Entities", IsActivity = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Activity Party Entities", IsActivityParty = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "BPF Entities", IsBPFEntity = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Business Process Enabled Entities", IsBusinessProcessEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Child Entities", IsChildEntity = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Connections Enabled Entities", IsConnectionsEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Customizable Entities", IsCustomizable = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Document Management Enabled Entities", IsDocumentManagementEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Enabled For Charts Entities", IsEnabledForCharts = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Enabled For External Channels Entities", IsEnabledForExternalChannels = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Enabled For Trace Entities", IsEnabledForTrace = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Marked as Importable Entities", IsImportable = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Interaction Centric Enabled Entities", IsInteractionCentricEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Knowledge Management Enabled Entities", IsKnowledgeManagementEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Mail Merge Enabled Entities", IsMailMergeEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Managed Entities", IsManaged = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Unmanaged Entities", IsNotManaged = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Offline In Mobile Client Entities", IsOfflineInMobileClient = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "OneNote Integration Enabled Entities", IsOneNoteIntegrationEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Optimistic Concurrency Enabled Entities", IsOptimisticConcurrencyEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Quick Create Enabled Entities", IsQuickCreateEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "ReadOnly In Mobile Client Entities", IsReadOnlyInMobileClient = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "SLA Enabled Entities", IsSLAEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Valid For Advanced Find Entities", IsValidForAdvancedFind = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Valid For Queue Entities", IsValidForQueue = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Visible In Mobile Entities", IsVisibleInMobile = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Visible In Mobile Client Entities", IsVisibleInMobileClient = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Change Tracking Enabled", ChangeTrackingEnabled = true });
            cbPermissionGroup.Items.Add(new GroupSelectionItem() { Name = "Can Trigger Workflow", CanTriggerWorkflow = true });

            // set the core records as the default selection
            cbPermissionGroup.SelectedItem = cbPermissionGroup.Items.OfType<GroupSelectionItem>().FirstOrDefault(x => x.IsDefaultItem);
        }
        #endregion

        #region Method: LoadListOfRoles (internal - void)
        /// <summary>
        /// Loads the list of security roles
        /// </summary>
        internal void LoadListOfRoles() {
            try {
                // clear all comparison controls
                wbSecurityRolePriv.DocumentText = "";

                // clear all roles from the checkbox list control
                cblRoles.Items.Clear();

                // clear and disable the roles checkbox list
                ComparedRoleIDs.Clear();
                if (PrivilegesPerRole != null)
                    PrivilegesPerRole.Clear();

                // disable the controls
                cblRoles.Enabled = false;
                pnCompareRoles.Enabled = false;

                // start the loading process
                WorkAsync(new WorkAsyncInfo {
                    Message = "Retrieving Security Roles from CRM",
                    Work = (w, e) => {
                        // retrieve the roles
                        e.Result = RetrieveRolesWork(w, e);
                    },
                    ProgressChanged = e => {
                        // If progress has to be notified to user, use the following method:
                        SetWorkingMessage("Loading Roles..");

                        if (SendMessageToStatusBar != null && e.ProgressPercentage >= 100)
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(100, $"Loaded {(SecurityRoles?.Count).GetValueOrDefault(0)} Roles"));
                        else
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(e.ProgressPercentage, "Loading Roles.."));
                    },
                    PostWorkCallBack = e => {
                        // update the UI
                        RetrieveRolesCompleted(e);
                    },
                    AsyncArgument = null,
                    IsCancelable = true,
                    MessageWidth = 340,
                    MessageHeight = 150
                });
            } catch (System.Exception ex) {
                // re-enable the checkbox list
                this.BeginInvoke((MethodInvoker)delegate {
                    MessageBox.Show("An Error occurred and could not load the Security roles.\n" + ex.Message);
                    cblRoles.Enabled = true;
                    pnCompareRoles.Enabled = true;

                    // execute the loaded event for the control
                    SecurityRolesCompletedMethod();
                });
            }
        }
        #endregion

        #region Method: RetrieveRolesWork
        /// <summary>
        /// Loads in a background thread the list of security roles from the server
        /// </summary>
        /// <param name="workerThread"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private List<SecurityRoleType> RetrieveRolesWork(BackgroundWorker workerThread, DoWorkEventArgs e) {
            // get the security roles (as entities) from CRM
            var oSecurityRoleEntities = FetchXMLRetriever.FetchAllAsList(FetchXMLQueries.GetSecurityRoles, this.Service);

            // transform the entities into a list of security roles
            SecurityRoles = oSecurityRoleEntities.Select(oRole => new SecurityRoleType() {
                ID = oRole.Id,
                Name = oRole.GetAttributeValue<string>("name"),
                IsCustomizable = oRole.GetBoolManagedPropertyValue("iscustomizable").GetValueOrDefault(false),
                RoleidUnique = oRole.GetAttributeValue<Guid>("roleidunique"),
                IsManaged = oRole.GetAttributeValue<Nullable<bool>>("ismanaged").GetValueOrDefault(false),
                SolutionID = oRole.GetAttributeValue<Guid>("solutionid"),

                CreatedOn = oRole.GetAttributeValue<DateTime>("createdon"),
                ModifiedOn = oRole.GetAttributeValue<DateTime>("modifiedon"),

                BusinessUnitID = oRole.GetEntityReferenceEntityID("businessunitid"),
                BusinessUnitName = oRole.GetEntityReferenceEntityName("businessunitid"),

                ComponentStateID = oRole.GetOptionSetAttribueValueOrNull("componentstate"),
                ComponentStateName = oRole.GetOptionSetLabel("componentstate"),

                ParentRoleID = oRole.GetEntityReferenceEntityID("parentroleid"),
                ParentRoleName = oRole.GetEntityReferenceEntityName("parentroleid"),

                ParentRootRoleID = oRole.GetEntityReferenceEntityID("parentrootroleid"),
                ParentRootRoleName = oRole.GetEntityReferenceEntityName("parentrootroleid")
            }).ToList();

            workerThread.ReportProgress(101, "Retrieving Roles...");
            return SecurityRoles;
        }
        #endregion

        #region Method: RetrieveRolesCompleted
        /// <summary>
        /// executed when the list of security roles is loaded and updates the UI
        /// </summary>
        /// <param name="e"></param>
        private void RetrieveRolesCompleted(RunWorkerCompletedEventArgs e) {
            try {
                // if there is an error the throw the error
                if (e.Error != null)
                    throw e.Error;

                // update the UI with the list of roles
                UpdateListOfRolesUI();
            } catch (System.Exception ex) {
                MessageBox.Show("An Error occurred and could not load the Security roles.\n" + ex.Message);
            } finally {
                // re-enable the checkbox list
                this.BeginInvoke((MethodInvoker)delegate {
                    // execute the loaded event for the control
                    SecurityRolesCompletedMethod();
                });
            }
        }
        #endregion

        #region Method: UpdateListOfRolesUI
        /// <summary>
        /// updates the UI with the list of security roles after the loading, by adding them to the security roles checkbox list control
        /// </summary>
        private void UpdateListOfRolesUI() {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(UpdateListOfRolesUI));

            try {
                // load the roles in the checkbox list control
                if (SecurityRoles?.Count > 0) {
                    cblRoles.Items.Clear();
                    cblRoles.Items.AddRange(SecurityRoles.ToArray());
                }
            } catch (System.Exception ex) {
                MessageBox.Show("An Error occurred and could not load the Security roles.\n" + ex.Message);
            } finally {
                // re-enable the checkbox list and the comparison controls
                cblRoles.Enabled = true;
                pnCompareRoles.Enabled = true;
            }
        }
        #endregion

        #region Method: CompareSelectedRoles (internal)
        /// <summary>
        /// compares the selected roles
        /// </summary>
        internal void CompareSelectedRoles() {
            try {
                cblRoles.Enabled = false;
                pnCompareRoles.Enabled = false;

                // remove all existing comparisons
                wbSecurityRolePriv.DocumentText = "";

                // check if any security role was selected
                if (cblRoles.CheckedItems.Count < 2) {
                    MessageBox.Show("At least two (2) Security Roles need to be selected for a Comparison.");

                    // re-enable the controls
                    cblRoles.Enabled = true;
                    pnCompareRoles.Enabled = true;

                    // invoke the parent complete event
                    SecurityComparissonCompletedMethod();
                    return;
                }

                // set the ids of the roles to be compared
                ComparedRoleIDs.Clear();
                ComparedRoleIDs.AddRange(cblRoles.CheckedItems.OfType<SecurityRoleType>().Select(x => x.ID));

                // start the loading process for the role security privileges
                // start the loading process
                WorkAsync(new WorkAsyncInfo {
                    Message = "Retrieving Selected Role Privileges for Comparison",
                    Work = (w, e) => {
                        // retrieve the privileges
                        e.Result = LoadSecurityRolesPriviledgesWork(w, e);
                    },
                    ProgressChanged = e => {
                        // If progress has to be notified to user, use the following method:
                        SetWorkingMessage("Loading Role Privileges..");

                        if (SendMessageToStatusBar != null && e.ProgressPercentage >= 100)
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(100, $"Loaded Privileges for {(ComparedRoleIDs?.Count).GetValueOrDefault(0)} Roles"));
                        else
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(e.ProgressPercentage, "Loading Role Privileges.."));
                    },
                    PostWorkCallBack = e => {
                        // update the UI
                        LoadSecurityRolesPriviledgesComplete(e);
                    },
                    AsyncArgument = null,
                    IsCancelable = true,
                    MessageWidth = 340,
                    MessageHeight = 150
                });

            } catch (System.Exception ex) {
                MessageBox.Show("An Error occurred and could not load the Security roles.\n" + ex.Message);
                cblRoles.Enabled = true;
                pnCompareRoles.Enabled = true;
            }
        }
        #endregion

        #region Method: LoadSecurityRolesPriviledgesWork
        private Dictionary<Guid, List<RolePriviledgeType>> LoadSecurityRolesPriviledgesWork(BackgroundWorker workerThread, DoWorkEventArgs e) {
            // report progress and return the list
            workerThread.ReportProgress(20, "Retrieving Entity Metadata...");
            // load the entity metadata if not loaded
            if ((this.CachedEntityMetadata?.Count).GetValueOrDefault() < 1) {
                var oEntitiesMD = this.Service.GetListOfAllEntitiesMD();
                CachedEntityMetadata = oEntitiesMD.GroupBy(x => x.LogicalName).ToDictionary(x => x.Key, x => x.First());

                //System.IO.File.WriteAllText(@"d:\entities.csv", string.Join("\n", oEntitiesMD.Select(x => $"\"{x.LogicalName}\",\"{x.GetDisplayLabel()}\"").ToArray()));
            }

            // report progress and return the list
            workerThread.ReportProgress(40, "Retrieving Privileges List...");
            // Load the list of privileges if not loaded
            if ((Privileges?.Count).GetValueOrDefault() < 1) {
                // retrieve all privilege entities from CRM
                var oPriviledgeEntities = FetchXMLRetriever.FetchAllAsList(FetchXMLQueries.GetAllSecurityPriviledges, this.Service);

                Privileges = oPriviledgeEntities.Select(oPriv => new PriviledgeType() {
                    ID = oPriv.Id,
                    Name = oPriv.GetAttributeValue<string>("name"),
                    ObjectTypeCode = oPriv.GetAttributeAliasValue<string>("TypeCodes.objecttypecode") ?? "none",
                    AccessRightID = oPriv.GetAttributeValue<int>("accessright"),
                    AccessRight = oPriv.GetAttributeValue<int>("accessright").ToEnumSafe<AccessRightEnum>(),
                    CanBeBasic = oPriv.GetAttributeValue<Nullable<bool>>("canbebasic").GetValueOrDefault(false),
                    CanBeDeep = oPriv.GetAttributeValue<Nullable<bool>>("canbedeep").GetValueOrDefault(false),
                    CanBeLocal = oPriv.GetAttributeValue<Nullable<bool>>("canbelocal").GetValueOrDefault(false),
                    CanBeGlobal = oPriv.GetAttributeValue<Nullable<bool>>("canbeglobal").GetValueOrDefault(false),
                    CanBeEntityReference = oPriv.GetAttributeValue<Nullable<bool>>("canbeentityreference").GetValueOrDefault(false),
                    CanBeParentEntityReference = oPriv.GetAttributeValue<Nullable<bool>>("canbeparententityreference").GetValueOrDefault(false)
                }).OrderBy(x => x.ObjectTypeCode).ToList();

                Privileges.Where(oPriv => oPriv.ObjectTypeCode != "none").ForEach(oPriv => {
                    // get the entity name if one exists
                    string entityName = "";
                    EntityMetadata oEntityMD = null;
                    if (CachedEntityMetadata.TryGetValue(oPriv.ObjectTypeCode, out oEntityMD)) {
                        var oEntityNameLabel = oEntityMD.DisplayName.LocalizedLabels.FirstOrDefault();
                        if (oEntityNameLabel != null)
                            entityName = oEntityNameLabel.Label;
                    } else {
                        oEntityMD = null;
                    }

                    oPriv.EntityMD = oEntityMD;
                    oPriv.EntityDisplayName = entityName;
                });
            }

            // load the security role privileges in order to be able to compare
            var oRoleValues = string.Join("", ComparedRoleIDs.Select(id => $"<value>{id}</value>"));
            var oRolePrivEntities = FetchXMLRetriever.FetchAllAsList(string.Format(FetchXMLQueries.GetPriviledgesForRoles, oRoleValues), this.Service);

            // group the loaded privileges per role
            workerThread.ReportProgress(60, "Retrieving Role Privileges...");
            PrivilegesPerRole = oRolePrivEntities.GroupBy(x => x.GetAttributeValue<Guid>("roleid"))
                .ToDictionary(x => x.Key, x => x.Select(oPriv => new RolePriviledgeType() {
                    ID = oPriv.Id,
                    PrivilegeID = oPriv.GetAttributeValue<Guid>("privilegeid"),
                    Level = oPriv.GetAttributeValue<Nullable<int>>("privilegedepthmask").GetValueOrDefault(-1).ToEnumSafe<PriviledgeDepthEnum>()
                }).ToList());

            // report progress and return the list
            workerThread.ReportProgress(101, "Retrieving Role Privileges...");
            return PrivilegesPerRole;
        }
        #endregion

        #region Method: LoadSecurityRolesPriviledgesComplete
        private void LoadSecurityRolesPriviledgesComplete(RunWorkerCompletedEventArgs e) {
            try {
                // if there is an error the throw the error
                if (e.Error != null)
                    throw e.Error;

                // update the UI with the comparison data
                BuildCompareRolesUI();
            } catch (System.Exception ex) {
                MessageBox.Show("An Error occurred and could not complete the Security role comparison. Please try again.\n" + ex.Message);
            } finally {
                // re-enable the controls accordingly
                this.BeginInvoke((MethodInvoker)delegate {
                    SecurityComparissonCompletedMethod();
                });
            }
        }
        #endregion

        #region Method: BuildCompareRolesUI
        private void BuildCompareRolesUI() {
            // make sure the necessary data is loaded before attempting to build the table
            if ((ComparedRoleIDs.Count == 0) ||
                ((PrivilegesPerRole?.Count).GetValueOrDefault(0) < 1) ||
                ((SecurityRoles?.Count).GetValueOrDefault(0) < 1) ||
                ((Privileges?.Count).GetValueOrDefault(0) < 1)) {
                return;
            }

            if (this.InvokeRequired)
                this.BeginInvoke(new Action(BuildCompareRolesUI));

            try {
                // create the initial HTML page code with the body
                var oSB = new StringBuilder();
                oSB.AppendLine("<html lang='en'>");
                oSB.AppendLine("<head>");

                var oPrivLevels = Enum.GetValues(typeof(PriviledgeDepthEnum));

                // load the images for the permission depth
                var oImageData = new Dictionary<PriviledgeDepthEnum, string>();
                oImageData[PriviledgeDepthEnum.None] = Properties.Resources.none.GetBase64(ImageFormat.Gif);
                oImageData[PriviledgeDepthEnum.ParentChild] = Properties.Resources.ParentChildBU.GetBase64(ImageFormat.Gif);
                oImageData[PriviledgeDepthEnum.BusinessUnit] = Properties.Resources.BU.GetBase64(ImageFormat.Gif);
                oImageData[PriviledgeDepthEnum.User] = Properties.Resources.User.GetBase64(ImageFormat.Gif);
                oImageData[PriviledgeDepthEnum.Organization] = Properties.Resources.Organization.GetBase64(ImageFormat.Gif);

                // add the css elements for the images
                oSB.AppendLine("<style type='text/css'>");
                foreach (PriviledgeDepthEnum olevel in oPrivLevels) {
                    oSB.AppendLine($"img.{olevel} {{");
                    oSB.Append("background:url(data:image/gif;base64,");
                    oSB.Append(oImageData[olevel]);
                    oSB.AppendLine(");");
                    oSB.AppendLine("no-repeat left center;");
                    oSB.AppendLine("width: 16px;");
                    oSB.AppendLine("height: 16px;");
                    oSB.AppendLine("}");
                }

                // add other styles
                oSB.AppendLine("td.wbc {{");
                oSB.AppendLine("border-bottom:1px solid #d3d3d3;");
                oSB.AppendLine("text-align:center;");
                oSB.AppendLine("}");
                oSB.AppendLine("td.wobc {{");
                oSB.AppendLine("text-align:center;");
                oSB.AppendLine("}");

                oSB.AppendLine("td.wbl {{");
                oSB.AppendLine("border-bottom:1px solid #d3d3d3;");
                oSB.AppendLine("text-align:left;");
                oSB.AppendLine("}");
                oSB.AppendLine("td.wobl {{");
                oSB.AppendLine("text-align:left;");
                oSB.AppendLine("}");

                oSB.AppendLine("tr.hr {{");
                oSB.AppendLine("background-color:#929292;");
                oSB.AppendLine("height=1px;");
                oSB.AppendLine("}");

                oSB.AppendLine("</style>");
                oSB.AppendLine("</head>");

                oSB.AppendLine("<body style='padding:0x;margin:0px;font-size:10px'>");

                int numOfRowsPerPannel = ComparedRoleIDs.Count;

                // create the main table with the entity privileges
                CreateEntityPermissionsTable(oSB, numOfRowsPerPannel);

                // create the special privileges table
                CreateSpecialPermissionsTable(oSB, numOfRowsPerPannel);

                // set the document in the browser control
                oSB.AppendLine("</body></html>");
                wbSecurityRolePriv.DocumentText = oSB.ToString();

                //System.IO.File.WriteAllText(@"d:\test.html", oSB.ToString());
            } catch (System.Exception ex) {
                MessageBox.Show("An Error occurred and could not complete the Security role comparison. Please try again.\n" + ex.Message);
            } finally {
                // re-enable the checkbox list
                cblRoles.Enabled = true;
                pnCompareRoles.Enabled = true;
            }
        }
        #endregion

        #region Method: CreateEntityPermissionsTable
        private void CreateEntityPermissionsTable(StringBuilder oSB, int numOfRowsPerPannel) {
            // get the access and privilege levels list
            var oAccessLevels = Enum.GetValues(typeof(AccessRightEnum));

            List<PriviledgeType> oModPrivileges = FilterPrivileges();

            // start by grouping the privileges by type code
            var oPrivByTypeCode = oModPrivileges.GroupBy(x => x.GrouppingKey).ToDictionary(x => x.Key, x => x.ToList());

            // loop and create the table
            int cnt = 0;
            oSB.AppendLine("<br/><table cellpadding='0' cellspacing='2' style='width=98%;font-size:11px;font-family:arial;padding:2px'>");

            // add group header
            oSB.AppendLine("<tr style='background-color:#253789;color:white;font-size:13px;text-align:center;padding:2px;margin:2px'>");
            oSB.AppendLine($"<th colspan={oAccessLevels.Length + 2}>ENTITY BASED PERMISSIONS</th>");
            oSB.AppendLine("</tr>");

            // add the header
            oSB.AppendLine("<tr style='background-color:#318aca;color:white;text-align:left;padding:2px;margin:2px'>");
            oSB.AppendLine("<th>Entity/Object Type</th>");
            oSB.AppendLine("<th>Role</th>");
            foreach (AccessRightEnum oLevel in oAccessLevels) {
                oSB.AppendLine($"<th style='text-align:center'>{oLevel}</th>");
            }
            oSB.AppendLine("</tr>");

            // loop and add the entity privileges
            foreach (var oTypeCodeKeyPair in oPrivByTypeCode) {
                var oTypeCode = oTypeCodeKeyPair.Key;
                var oPrivileges = oTypeCodeKeyPair.Value;
                var oPrivilegeIDs = oPrivileges.Select(p => p.ID).ToList();
                var oFirstPrivilege = oPrivileges.FirstOrDefault();
                var oHasAccessLevel = oPrivileges.Any(p => p.AccessRight.HasValue);

                if (oHasAccessLevel) {
                    // check to make sure we have the right type
                    oSB.AppendLine($"<tr class=hr><td colspan='{oAccessLevels.Length + 2}'></tr>");
                    cnt++;

                    // create the container panel
                    // set a different color for even rows to differentiate
                    string bgColor = "white";
                    if (cnt % 2 > 0) {
                        bgColor = "#ffffe3";
                    }
                    oSB.AppendLine($"<tr bgcolor='{bgColor}'>");

                    // create the entity friendly name Label
                    oSB.Append($"<td rowspan='{numOfRowsPerPannel}'>");
                    if (!string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName)) {
                        oSB.Append($"<b>{oFirstPrivilege.EntityDisplayName}</b>");
                    }

                    // add the schema name if one exists
                    if (string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName) || !oFirstPrivilege.EntityDisplayName.Equals(oTypeCode, StringComparison.InvariantCultureIgnoreCase)) {
                        if (string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName)) {
                            oSB.Append($"<b>{oTypeCode}</b>");
                        } else {
                            oSB.Append("<br/>");
                            oSB.Append(oTypeCode);
                        }
                    }
                    oSB.Append("</td>");

                    // create the security role row panels
                    bool trClosed = false;

                    var lastRoleID = ComparedRoleIDs.Last();
                    foreach (var oRoleID in ComparedRoleIDs) {
                        var oRole = SecurityRoles.FirstOrDefault(r => r.ID == oRoleID);
                        var oRolePriv = PrivilegesPerRole[oRoleID];
                        var oTypeCodePrivileges = oRolePriv.Where(x => oPrivilegeIDs.Contains(x.PrivilegeID)).ToList();

                        string borderStyleStr = "wb";
                        if (oRoleID.Equals(lastRoleID))
                            borderStyleStr = "wob";

                        // open the row (if this is not the second column from the first row with the typecode/entity name)
                        if (trClosed)
                            oSB.AppendLine($"<tr bgcolor='{bgColor}' >");
                        else
                            trClosed = true;

                        // show the role name
                        oSB.AppendLine($"<td class={borderStyleStr}l>{oRole.Name}</td>");

                        foreach (AccessRightEnum oLevel in oAccessLevels) {
                            var oPrivLevel = oPrivileges.FirstOrDefault(p => p.AccessRight.HasValue && p.AccessRight.Value == oLevel);
                            if (oPrivLevel != null) {
                                var oRoleLevel = oRolePriv.FirstOrDefault(k => k.PrivilegeID == oPrivLevel.ID);
                                if (oRoleLevel != null) {
                                    if (oRoleLevel.Level.HasValue) {
                                        oSB.AppendLine($"<td class={borderStyleStr}c><img class={oRoleLevel.Level.Value} /></td>");
                                    } else {
                                        oSB.AppendLine($"<td class={borderStyleStr}c><img class={PriviledgeDepthEnum.None} /> </td>");
                                    }
                                } else {
                                    oSB.AppendLine($"<td class={borderStyleStr}c><img class={PriviledgeDepthEnum.None} /></td>");
                                }
                            } else {
                                oSB.AppendLine($"<td class={borderStyleStr}c></td>");
                            }
                        }

                        // close the row
                        oSB.AppendLine("</tr>");
                    }
                }
            }

            // add the no data row if no rows were loaded
            if (cnt == 0) {
                oSB.AppendLine("<tr style='text-align:center;padding:2px;margin:2px'>");
                oSB.AppendLine($"<td colspan={oAccessLevels.Length + 2}><i>No Privileges Found for this category</i></th>");
                oSB.AppendLine("</tr>");
            }

            oSB.AppendLine("</table><br/>");
        }
        #endregion

        #region Method: CreateSpecialPermissionsTable
        private void CreateSpecialPermissionsTable(StringBuilder oSB, int numOfRowsPerPannel) {
            // get the access and privilege levels list
            var oAccessLevels = Enum.GetValues(typeof(AccessRightEnum));

            List<PriviledgeType> oModPrivileges = FilterPrivileges();

            // start by grouping the privileges by type code
            var oPrivByTypeCode = oModPrivileges.GroupBy(x => x.GrouppingKey).ToDictionary(x => x.Key, x => x.ToList());

            // loop and create the table
            int cnt = 0;
            oSB.AppendLine("<br/><table cellpadding='0' cellspacing='2' style='width=98%;font-size:11px;font-family:arial;padding:2px'>");

            // add group header
            oSB.AppendLine("<tr style='background-color:#d9542c;color:white;font-size:13px;text-align:center;padding:2px;margin:2px'>");
            oSB.AppendLine($"<th colspan=3>SPECIAL PERMISSIONS</th>");
            oSB.AppendLine("</tr>");

            // add the header
            oSB.AppendLine("<tr style='background-color:#e17f61;color:white;text-align:left;padding:2px;margin:2px'>");
            oSB.AppendLine("<th>Entity/Object Type</th>");
            oSB.AppendLine("<th>Role</th>");
            oSB.AppendLine("<th>Right</th>");
            oSB.AppendLine("</tr>");

            // loop and add the privileges
            foreach (var oTypeCodeKeyPair in oPrivByTypeCode) {
                var oTypeCode = oTypeCodeKeyPair.Key;
                var oSpecialPrivileges = oTypeCodeKeyPair.Value.Where(x => x.ObjectTypeCode.Equals("none") || !x.AccessRight.HasValue).ToList();
                var oSpecialPrivilegeIDs = oSpecialPrivileges.Select(p => p.ID).ToList();
                var oFirstPrivilege = oSpecialPrivileges.FirstOrDefault();
                var oHasSpecialAccessLevel = (oSpecialPrivileges.Count > 0);

                if (oHasSpecialAccessLevel) {
                    // create a new section for each special privilege (if multiple exists for the entry)
                    foreach (var oPriv in oSpecialPrivileges) {
                        // check to make sure we have the right type
                        oSB.AppendLine($"<tr class=hr><td colspan='3'></tr>");
                        cnt++;

                        // create the container panel
                        // set a different color for even rows to differentiate
                        string bgColor = "white";
                        if (cnt % 2 > 0) {
                            bgColor = "#ffffe3";
                        }
                        oSB.AppendLine($"<tr bgcolor='{bgColor}'>");

                        // create the entity friendly name Label
                        oSB.Append($"<td rowspan='{numOfRowsPerPannel}'>");
                        if (!string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName)) {
                            oSB.Append($"<b>{oFirstPrivilege.EntityDisplayName}</b>");
                        }
                        if (!string.IsNullOrEmpty(oPriv.Name)) {
                            string privName = oPriv.Name;
                            if (privName.StartsWith("prv"))
                                privName = privName.Remove(0, 3);

                            // convert the privilege name to a sentence by tokenizing
                            if (!string.IsNullOrEmpty(privName))
                                privName = privName.ToSentenceCase();

                            oSB.Append($"<br/><i>{privName}</i>");
                        }

                        // add the schema name if one exists
                        if ((string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName) ||
                            !oFirstPrivilege.EntityDisplayName.Equals(oTypeCode, StringComparison.InvariantCultureIgnoreCase)) &&
                            !oPriv.Name.Equals(oTypeCode, StringComparison.InvariantCultureIgnoreCase)) {
                            if (string.IsNullOrEmpty(oFirstPrivilege.EntityDisplayName)) {
                                oSB.Append($"<b>{oTypeCode}</b>");
                            } else {
                                oSB.Append("<br/>");
                                oSB.Append(oTypeCode);
                            }
                        }
                        oSB.Append("</td>");

                        // create the security role row panels
                        bool trClosed = false;

                        var lastRoleID = ComparedRoleIDs.Last();
                        foreach (var oRoleID in ComparedRoleIDs) {
                            var oRole = SecurityRoles.FirstOrDefault(r => r.ID == oRoleID);
                            var oRolePriv = PrivilegesPerRole[oRoleID];
                            var oTypeCodePrivileges = oRolePriv.Where(x => oSpecialPrivilegeIDs.Contains(x.PrivilegeID)).ToList();

                            string borderStyleStr = "wb";
                            if (oRoleID.Equals(lastRoleID))
                                borderStyleStr = "wob";

                            // open the row (if this is not the second column from the first row with the typecode/entity name)
                            if (trClosed)
                                oSB.AppendLine($"<tr bgcolor='{bgColor}' >");
                            else
                                trClosed = true;

                            // show the role name
                            oSB.AppendLine($"<td class={borderStyleStr}l>{oRole.Name}</td>");

                            var oRoleLevel = oRolePriv.FirstOrDefault(k => k.PrivilegeID == oPriv.ID);
                            if (oRoleLevel != null) {
                                if (oRoleLevel.Level.HasValue) {
                                    oSB.AppendLine($"<td class={borderStyleStr}c><img class={oRoleLevel.Level.Value} /></td>");
                                } else {
                                    oSB.AppendLine($"<td class={borderStyleStr}c><img class={PriviledgeDepthEnum.None} /> </td>");
                                }
                            } else {
                                oSB.AppendLine($"<td class={borderStyleStr}c><img class={PriviledgeDepthEnum.None} /></td>");
                            }

                            // close the row
                            oSB.AppendLine("</tr>");
                        }
                    }
                }
            }

            // add the no data row if no rows were loaded
            if (cnt == 0) {
                oSB.AppendLine("<tr style='text-align:center;padding:2px;margin:2px'>");
                oSB.AppendLine("<td colspan=3><i>No Privileges Found for this category</i></th>");
                oSB.AppendLine("</tr>");
            }

            oSB.AppendLine("</table><br/>");
        }
        #endregion

        #region Method: FilterPrivileges
        private List<PriviledgeType> FilterPrivileges() {
            List<PriviledgeType> oModPrivileges = this.Privileges;

            // get the access and privilege levels list
            var oAccessLevels = Enum.GetValues(typeof(AccessRightEnum));

            // check if we need to filter
            var oSelectedGroup = (GroupSelectionItem)(cbPermissionGroup.SelectedItem);
            if (!oSelectedGroup.IsAllPrivileges) {
                // filter by custom entity
                if (oSelectedGroup.OnlyCustom) {
                    var oCustomEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsCustomEntity.GetValueOrDefault())
                        .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oCustomEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only audit enabled entities
                List<string> oFilteredEntityNames;
                if (oSelectedGroup.AuditEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsAuditEnabled != null && x.IsAuditEnabled.Value)
                        .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }
                // only Audit disabled entities
                if (oSelectedGroup.AuditDisabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsAuditEnabled != null && !x.IsAuditEnabled.Value)
                        .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }
                // only entities with duplication detection enabled
                if (oSelectedGroup.DuplicateDetectionEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsDuplicateDetectionEnabled != null && x.IsDuplicateDetectionEnabled.Value)
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only global privileges
                if (oSelectedGroup.OnlyGlobal) {
                    return oModPrivileges.Where(x => !string.IsNullOrEmpty(x.ObjectTypeCode) && x.ObjectTypeCode != "none").ToList();
                }

                // only entities that can be owned by teams or users
                if (oSelectedGroup.OnlyUserTeamOwned) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.OwnershipType != null &&
                                                     (x.OwnershipType.Value == OwnershipTypes.UserOwned || x.OwnershipType.Value == OwnershipTypes.TeamOwned))
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that can be owned by the organization
                if (oSelectedGroup.OnlyOrganizationOwned) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.OwnershipType != null &&
                                                  x.OwnershipType.Value == OwnershipTypes.OrganizationOwned)
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsActivity
                if (oSelectedGroup.IsActivity) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsActivity.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsActivityParty
                if (oSelectedGroup.IsActivityParty) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsActivityParty.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsBPFEntity
                if (oSelectedGroup.IsBPFEntity) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsBPFEntity.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsBusinessProcessEnabled
                if (oSelectedGroup.IsBusinessProcessEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsBusinessProcessEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsChildEntity
                if (oSelectedGroup.IsChildEntity) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsChildEntity.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsConnectionsEnabled
                if (oSelectedGroup.IsConnectionsEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsConnectionsEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsCustomizable
                if (oSelectedGroup.IsCustomizable) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsCustomizable.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsDocumentManagementEnabled
                if (oSelectedGroup.IsDocumentManagementEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsDocumentManagementEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsEnabledForCharts
                if (oSelectedGroup.IsEnabledForCharts) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsEnabledForCharts.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsEnabledForExternalChannels
                if (oSelectedGroup.IsEnabledForExternalChannels) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsEnabledForExternalChannels.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsEnabledForTrace
                if (oSelectedGroup.IsEnabledForTrace) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsEnabledForTrace.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsImportable
                if (oSelectedGroup.IsImportable) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsImportable.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsInteractionCentricEnabled
                if (oSelectedGroup.IsInteractionCentricEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsInteractionCentricEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsKnowledgeManagementEnabled
                if (oSelectedGroup.IsKnowledgeManagementEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsKnowledgeManagementEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsMailMergeEnabled
                if (oSelectedGroup.IsMailMergeEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsMailMergeEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsManaged
                if (oSelectedGroup.IsManaged) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsManaged.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as !IsManaged
                if (oSelectedGroup.IsNotManaged) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => !x.IsManaged.GetValueOrDefault(true))
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsOfflineInMobileClient
                if (oSelectedGroup.IsOfflineInMobileClient) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsOfflineInMobileClient.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsOneNoteIntegrationEnabled
                if (oSelectedGroup.IsOneNoteIntegrationEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsOneNoteIntegrationEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsOptimisticConcurrencyEnabled
                if (oSelectedGroup.IsOptimisticConcurrencyEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsOptimisticConcurrencyEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsQuickCreateEnabled
                if (oSelectedGroup.IsQuickCreateEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsQuickCreateEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsReadOnlyInMobileClient
                if (oSelectedGroup.IsReadOnlyInMobileClient) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsReadOnlyInMobileClient.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsSLAEnabled
                if (oSelectedGroup.IsSLAEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsSLAEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsValidForAdvancedFind
                if (oSelectedGroup.IsValidForAdvancedFind) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsValidForAdvancedFind.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsValidForQueue
                if (oSelectedGroup.IsValidForQueue) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsValidForQueue.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsVisibleInMobile
                if (oSelectedGroup.IsVisibleInMobile) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsVisibleInMobile.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as IsVisibleInMobileClient
                if (oSelectedGroup.IsVisibleInMobileClient) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.IsVisibleInMobileClient.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as ChangeTrackingEnabled
                if (oSelectedGroup.ChangeTrackingEnabled) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.ChangeTrackingEnabled.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are marked as CanTriggerWorkflow
                if (oSelectedGroup.CanTriggerWorkflow) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.CanTriggerWorkflow.GetValueOrDefault())
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only entities that are owned by the Business (OnlyBusinessOwned)
                if (oSelectedGroup.OnlyBusinessOwned) {
                    oFilteredEntityNames = CachedEntityMetadata.Values.ToList().Where(x => x.OwnershipType != null &&
                                                  x.OwnershipType.Value == OwnershipTypes.BusinessOwned)
                       .Select(x => x.LogicalName).ToList();
                    return oModPrivileges.Where(x => oFilteredEntityNames.Contains(x.ObjectTypeCode)).ToList();
                }

                // only special privileges (IsSpecialPrivileges)
                if (oSelectedGroup.IsSpecialPrivileges) {
                    return oModPrivileges.Where(x => x.ObjectTypeCode.Equals("none") || !x.AccessRight.HasValue).ToList();
                }

                // filter by entity name
                return oModPrivileges.Where(x => oSelectedGroup.Values.Contains(x.Name) || oSelectedGroup.Values.Contains(x.ObjectTypeCode)).ToList();
            }

            // return the filtered privileges
            return oModPrivileges;
        }
        #endregion

        #region Method: cbPermissionGroup_SelectedIndexChanged
        /// <summary>
        /// refreshes the role comparison table when the dropdown changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPermissionGroup_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                wbSecurityRolePriv.DocumentText = "";
                BuildCompareRolesUI();
            } catch (System.Exception ex) {
                MessageBox.Show("Errors occurred while attempting to refresh the permissions table.\n" + ex.Message);
            }
        }
        #endregion

        #region Method: UpdateConnection (public- override)
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail connectionDetail, string actionName = "", object parameter = null) {
            base.UpdateConnection(newService, connectionDetail, actionName, parameter);
        }
        #endregion

        #region Method: tbutLoadRoles_Click
        /// <summary>
        /// executed when the load roles button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbutLoadRoles_Click(object sender, EventArgs e) {
            tbutLoadRoles.Enabled = false;
            tbutCompareSelected.Enabled = false;
            ExecuteMethod(this.LoadListOfRoles);
        }
        #endregion

        #region Method: SecurityRolesCompletedMethod
        /// <summary>
        /// executed when the user control completes the security role loading (with success or with failure)
        /// </summary>
        private void SecurityRolesCompletedMethod() {
            tbutLoadRoles.Enabled = true;
            tbutCompareSelected.Enabled = this.SecurityRoles?.Count > 0;

            // execute any custom event
            SecurityRolesCompleted?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Method: tbutCompareSelected_Click
        /// <summary>
        /// executed when the compare roles button is pressed, in order to compare the selected roles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbutCompareSelected_Click(object sender, EventArgs e) {
            tbutLoadRoles.Enabled = false;
            tbutCompareSelected.Enabled = false;
            ExecuteMethod(this.CompareSelectedRoles);
        }
        #endregion

        #region Method: SecurityComparissonCompletedMethod
        /// <summary>
        /// executed when the user control completes the security role comparison process
        /// </summary>
        private void SecurityComparissonCompletedMethod() {
            tbutLoadRoles.Enabled = true;
            tbutCompareSelected.Enabled = this.SecurityRoles?.Count > 0;

            // execute any custom events
            SecurityComparissonCompleted?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        private void tbutClose_Click(object sender, EventArgs e) {
            base.CloseTool(); // PluginBaseControl method that notifies the XrmToolBox that the user wants to close the plugin
            // Override the ClosingPlugin method to allow for any plugin specific closing logic to be performed (saving configs, canceling close, etc...)
        }

        public override void ClosingPlugin(PluginCloseInfo info) {
            base.ClosingPlugin(info);

            info.Silent = true;
        }

        #region IGitHubPlugin Interface implementation
        string IGitHubPlugin.RepositoryName => "DotCyToolboxPlugins";
        string IGitHubPlugin.UserName => "panayiotisp";
        #endregion

        #region IPayPalPlugin Interface implementation
        string IPayPalPlugin.DonationDescription => "XRMToolBox Donation";
        string IPayPalPlugin.EmailAccount => "panayiotis@dotcy.com.cy";
        #endregion

        #region IHelpPlugin Interface implementation
        string IHelpPlugin.HelpUrl => "https://github.com/panayiotisp/DotCyToolboxPlugins";
        #endregion

        #region IHelpPlugin Interface implementation
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;
        #endregion

    } // Class: UC_CompareRoles

} // namespace: DotCyToolboxPlugins.Controls