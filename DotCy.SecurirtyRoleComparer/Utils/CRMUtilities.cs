using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;


namespace DotCyToolboxPlugins.Utils {

    public static class CRMUtilities {


        #region Method: GetAttributevalue (public - static - generic - overloaded)
        public static dynamic GetAttributevalue(Entity oEntity, string attributeName) {
            object objData;
            if (oEntity.Attributes.TryGetValue(attributeName, out objData) && objData != null) {
                return objData;
            } else {
                return null;
            }
        }
        /// <summary>
        /// get the value of an attribute from entity1 if not empty otherwise try to get it from entity2
        /// </summary>
        /// <param name="oEntity1"></param>
        /// <param name="oEntity2"></param>
        /// <param name="attributeName"></param>I 
        /// <returns></returns>
        public static dynamic GetAttributevalue(Entity oEntity1, Entity oEntity2, string attributeName) {
            if (oEntity1 != null && oEntity1.Attributes.ContainsKey(attributeName)) {
                return GetAttributevalue(oEntity1, attributeName);
            } else {
                if (oEntity2 != null && oEntity2.Attributes.ContainsKey(attributeName)) {
                    return GetAttributevalue(oEntity2, attributeName);
                } else {
                    return null;
                }
            }
        }
        #endregion

        #region Method: GetAttributevalueFromAliasedValue (public - static - generic - overloaded)
        public static dynamic GetAttributevalueFromAliasedValue(Entity oEntity, string attributeName) {
            object objData;
            if (oEntity.Attributes.TryGetValue(attributeName, out objData) && objData != null) {
                if (objData is AliasedValue) {
                    if (objData != null) {
                        return ((AliasedValue)objData).Value;
                    } else {
                        return null;
                    }
                }
                return objData;
            } else {
                return null;
            }
        }
        #endregion

        #region Method: GetAttributeAliasValue (public - static - generic - overloaded)
        public static T GetAttributeAliasValue<T>(this Entity oEntity, string attributeName) {
            object objData;
            if (oEntity.Attributes.TryGetValue(attributeName, out objData) && objData != null) {
                if (objData is AliasedValue) {
                    if (objData != null) {
                        return (T)(((AliasedValue)objData).Value);
                    } else {
                        return default(T);
                    }
                }
                return (T)objData;
            } else {
                return default(T);
            }
        }
        #endregion

        #region Method: GetEntityReferenceNameFromAliasedValue (public - static - extension method)
        public static string GetEntityReferenceNameFromAliasedValue(this Entity oEntity, string attributeName) {
            object objData = GetAttributevalueFromAliasedValue(oEntity, attributeName);
            if (objData != null && objData is EntityReference) {
                var oERef = (objData as EntityReference);
                if (oERef != null)
                    return oERef.Name;
            }
            return null;
        }
        #endregion

        #region Method: GetMoneyNullableValueFromAliasedValue (public - static - extension method)
        public static Nullable<decimal> GetMoneyNullableValueFromAliasedValue(this Entity oEntity, string attributeName) {
            object objData = GetAttributevalueFromAliasedValue(oEntity, attributeName);
            if (objData != null && objData is Money) {
                var oMoney = objData as Money;
                return oMoney != null ? oMoney.Value : (Nullable<decimal>)null;
            }
            return null;
        }
        #endregion

        #region Method: GetEntityReferenceIDFromAliasedValue (public - static - extension method)
        public static Nullable<Guid> GetEntityReferenceIDFromAliasedValue(this Entity oEntity, string attributeName) {
            object objData = GetAttributevalueFromAliasedValue(oEntity, attributeName);
            if (objData != null) {
                if (objData is EntityReference) {
                    var oERef = (objData as EntityReference);
                    if (oERef != null)
                        return oERef.Id;
                } else {
                    if (objData is Guid) {
                        return (Guid)objData;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Method: UpdateIfCRMValueIsDifferent (public - static)
        public static void UpdateIfCRMValueIsDifferent<T>(Microsoft.Xrm.Sdk.Entity oSourceCIFEntity,
                                                       Microsoft.Xrm.Sdk.Entity oTargetCIFEntity,
                                                       string attributeName, object objSourceVal) {
            bool isEmpty = (objSourceVal == null) || (objSourceVal == DBNull.Value);
            bool isDifferent = true;


            // object is a date time convert it to the universal date time standard
            System.Type oGenType = null;
            if (!isEmpty) {
                if (objSourceVal is DateTime) {
                    objSourceVal = ((DateTime)objSourceVal).ToUniversalTime();
                } else {
                    if (objSourceVal is Nullable<DateTime>) {
                        var oNullableDate = (Nullable<DateTime>)objSourceVal;
                        if (oNullableDate.HasValue) {
                            objSourceVal = new Nullable<DateTime>(oNullableDate.Value.ToUniversalTime());
                        } else {
                            objSourceVal = null;
                            isEmpty = true;
                        }
                    } else {
                        // if it is a string then assume the empty string as a null value
                        if (objSourceVal is string && (string)objSourceVal == "") {
                            isEmpty = true;
                            objSourceVal = null;
                        } else {
                            oGenType = typeof(T);
                            if (oGenType == typeof(OptionSetValue)) {
                                objSourceVal = new OptionSetValue((int)objSourceVal);
                            } else {
                                if (oGenType == typeof(Money)) {
                                    objSourceVal = new Money((decimal)objSourceVal);
                                }
                            }
                        }
                    }
                }
            }

            // get the CRM current value - if one is available to compare - if none assume different
            if (oSourceCIFEntity != null) {
                T existingValue = default(T);

                // get the existing value
                if (oSourceCIFEntity.Attributes.Contains(attributeName)) {
                    existingValue = (T)oSourceCIFEntity[attributeName];
                }

                // check if the values are different
                if (((existingValue == null) && !isEmpty) || ((existingValue != null) && isEmpty)) {
                    isDifferent = true;
                } else {
                    if ((existingValue == null) && isEmpty)
                        isDifferent = false;
                    else {
                        if (oGenType == typeof(OptionSetValue))
                            isDifferent = !((OptionSetValue)objSourceVal).EqualValues((T)existingValue);
                        else
                            if (oGenType == typeof(Money))
                            isDifferent = !((Money)objSourceVal).EqualValues((T)existingValue);
                        else
                            isDifferent = !existingValue.Equals((T)objSourceVal);
                    }
                }
            }

            // get the new string value if the objects are different
            if (isDifferent) {
                if (isEmpty) {
                    oTargetCIFEntity[attributeName] = null;
                } else {
                    oTargetCIFEntity[attributeName] = (T)objSourceVal;
                }
            }
        }
        #endregion

        #region Method: UpdateIfCRMValueIsDifferent_EntityReference (public - static)
        public static void UpdateIfCRMValueIsDifferent_EntityReference(Microsoft.Xrm.Sdk.Entity oSourceCIFEntity,
                                                       Microsoft.Xrm.Sdk.Entity oTargetCIFEntity,
                                                       string attributeName, object objSourceVal,
                                                       string logicalName, bool allowNull = true) {
            bool isEmpty = (objSourceVal == null) || (objSourceVal == DBNull.Value);
            bool isDifferent = true;

            // get the CRM current value - if one is available to compare - if none assume different
            if (oSourceCIFEntity != null) {
                EntityReference existingValue = null;

                // get the existing value
                if (oSourceCIFEntity.Attributes.Contains(attributeName))
                    existingValue = (EntityReference)oSourceCIFEntity[attributeName];

                // check if the values are different
                if (((existingValue == null) && !isEmpty) || ((existingValue != null) && isEmpty)) {
                    isDifferent = true;
                } else {
                    if ((existingValue == null) && isEmpty)
                        isDifferent = false;
                    else
                        if (!existingValue.Id.Equals((Guid)objSourceVal))
                        isDifferent = true;
                    else
                        isDifferent = false;
                }
            }

            // get the new string value if the objects are different
            if (isDifferent) {
                if (isEmpty) {
                    if (allowNull) {
                        if (oTargetCIFEntity.Contains(attributeName)) {
                            oTargetCIFEntity[attributeName] = null;
                        } else {
                            oTargetCIFEntity.Attributes.Add(attributeName, null);
                        }
                    }
                } else {
                    if (oTargetCIFEntity.Contains(attributeName)) {
                        oTargetCIFEntity[attributeName] = new EntityReference(logicalName, (Guid)objSourceVal);
                    } else {
                        oTargetCIFEntity.Attributes.Add(attributeName, new EntityReference(logicalName, (Guid)objSourceVal));
                    }
                }
            }
        }
        #endregion

        #region Method: EqualValues (extension methods for CRM types)
        public static bool EqualValues(this OptionSetValue optionSet, object compareOptionSet) {
            return ((OptionSetValue)compareOptionSet).Value.Equals(optionSet.Value);
        }
        public static bool EqualValues(this Money moneyObj, object compareMoneyObj) {
            return ((Money)compareMoneyObj).Value.Equals(moneyObj.Value);
        }
        #endregion

        #region Method: GetAttributeMataData (public - static)
        /// <summary>
        /// returns the metadata of an attribute
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="oEntity"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static AttributeMetadata GetAttributeMataData(IOrganizationService svc, Entity oEntity, string attributeName) {
            return GetAttributeMataData(svc, oEntity.LogicalName, attributeName);
        }
        public static AttributeMetadata GetAttributeMataData(IOrganizationService svc, string entityName, string attributeName) {
            var oRequest = new RetrieveAttributeRequest() {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            return ((RetrieveAttributeResponse)svc.Execute(oRequest)).AttributeMetadata;
        }
        #endregion

        #region Method: GetEntityMataData (public - static - overloaded)
        /// <summary>
        /// Returns the metadata for an entity
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="oEntity"></param>
        /// <returns></returns>
        public static EntityMetadata GetEntityMataData(IOrganizationService svc, Entity oEntity, bool retrieveAsIfPublished = true) {
            return GetEntityMataData(svc, oEntity.LogicalName, retrieveAsIfPublished);
        }
        public static EntityMetadata GetEntityMataData(IOrganizationService svc, string entityName, bool retrieveAsIfPublished = true) {
            var oRequest = new RetrieveEntityRequest() {
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity,
                LogicalName = entityName,
                RetrieveAsIfPublished = retrieveAsIfPublished
            };

            return ((RetrieveEntityResponse)svc.Execute(oRequest)).EntityMetadata;
        }
        public static EntityMetadata GetEntityMataData(IOrganizationService svc, string entityName, Microsoft.Xrm.Sdk.Metadata.EntityFilters oFilter, bool retrieveAsIfPublished = true) {
            var oRequest = new RetrieveEntityRequest() {
                EntityFilters = oFilter,
                LogicalName = entityName,
                RetrieveAsIfPublished = retrieveAsIfPublished
            };

            return ((RetrieveEntityResponse)svc.Execute(oRequest)).EntityMetadata;
        }
        #endregion

        #region Method: GetCRMEntityRefObject (public - extension method to Nullable<Guid>)
        public static EntityReference GetCRMEntityRefObject(this Nullable<Guid> valueIn, string targetEntName) {
            if (valueIn == null || !valueIn.HasValue) {
                return null;
            } else {
                return new EntityReference(targetEntName, valueIn.Value);
            }
        }
        #endregion

        #region Method: GetEntityReferenceEntityName (public - static - extension method)
        public static string GetEntityReferenceEntityName(this Entity oEntity, string attributeName) {
            return (oEntity?.GetAttributeValue<EntityReference>(attributeName))?.Name;
        }
        #endregion

        #region Method: GetEntityReferenceEntityID (public - static - extension method)
        public static Nullable<Guid> GetEntityReferenceEntityID(this Entity oEntity, string attributeName) {
            if (oEntity != null && oEntity.Attributes.ContainsKey(attributeName)) {
                var oERef = oEntity.GetAttributeValue<EntityReference>(attributeName);
                if (oERef != null)
                    return oERef.Id;
            }
            return null;
        }
        #endregion

        #region Method: GetOptionSetLabel (public - static - extension method)
        public static string GetOptionSetLabel(this Entity oEntity, string attributeName) {
            if (oEntity != null && oEntity.Attributes.ContainsKey(attributeName)) {
                var oFormattedVal = oEntity.FormattedValues.FirstOrDefault(x => x.Key.Equals(attributeName));
                return oFormattedVal.Value;
            }
            return null;
        }
        #endregion

        #region Method: GetOptionSetAttribueValueOrNull (public - staticGetOptionSetAttribueValueOrNull
        public static Nullable<int> GetOptionSetAttribueValueOrNull(this Entity oEntity, string attributeName) {
            return (oEntity.GetAttributeValue<OptionSetValue>(attributeName))?.Value;
        }
        #endregion

        #region Method: GetLookUpRefValueOrNull (public - static GetLookUpRefValueOrNull
        public static Nullable<Guid> GetLookUpRefValueOrNull(this Entity oEntity, string attributeName) {
            return (oEntity.GetAttributeValue<EntityReference>(attributeName))?.Id;
        }
        #endregion

        #region Method: GetListOfAllEntitiesMD (public - static)
        public static List<EntityMetadata> GetListOfAllEntitiesMD(this IOrganizationService service) {
            try {
                var oList = new List<EntityMetadata>();

                // load the list of entities
                var oReq = new OrganizationRequest() {
                    RequestName = "RetrieveAllEntities"
                };
                oReq.Parameters.Add(new KeyValuePair<string, object>("EntityFilters", EntityFilters.Entity));
                oReq.Parameters.Add(new KeyValuePair<string, object>("RetrieveAsIfPublished", true));

                var oResponse = (OrganizationResponse)service.Execute(oReq);

                // get the metadata
                if (oResponse.Results.Count > 0 && oResponse.Results.Contains("EntityMetadata")) {
                    var mdEntities = (Microsoft.Xrm.Sdk.Metadata.EntityMetadata[])oResponse.Results["EntityMetadata"];
                    oList.AddRange(mdEntities);
                }

                // return the list
                return oList;
            } catch (System.Exception ex) {
                throw;
            }
        }
        #endregion

        #region Method: GetCRMAttachmentData (static - public - overloaded)
        public static byte[] GetCRMAttachmentData(Guid annotationid, IOrganizationService CRMSrv) {
            string fileName;
            string mimetype;
            long versionNumber;
            return GetCRMAttachmentData(annotationid, CRMSrv, out fileName, out versionNumber, out mimetype);
        }
        public static byte[] GetCRMAttachmentData(Guid annotationid, IOrganizationService CRMSrv, out string fileName, out long versionNumber, out string mimetype) {
            // retrieve the annotation record from CRM

            fileName = string.Empty;
            mimetype = string.Empty;
            versionNumber = 0;
            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(new ConditionExpression("annotationid", ConditionOperator.Equal, annotationid));
            var notesEntity = CRMSrv.Retrieve("annotation", annotationid, new ColumnSet(new string[] { "documentbody", "filename", "mimetype", "versionnumber" }));

            if (notesEntity != null) {
                try {
                    fileName = notesEntity.GetAttributeValue<string>("filename") ?? "";
                    mimetype = notesEntity.GetAttributeValue<string>("mimetype") ?? "";
                    versionNumber = notesEntity.GetAttributeValue<long>("versionnumber");

                    var attachementData = Convert.FromBase64String(notesEntity["documentbody"].ToString());

                    // return the image data
                    return attachementData;
                } catch (Exception ex) {
                    throw;
                }
            }
            return null;
        }

        #endregion

        #region Method: GetCRMAttachmentMD (public - static)
        public static bool GetCRMAttachmentMD(Guid annotationid, IOrganizationService CRMSrv, out string fileName, out long versionNumber, out string mimetype) {
            // retrieve the annotation record from CRM

            fileName = string.Empty;
            mimetype = string.Empty;
            versionNumber = 0;
            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(new ConditionExpression("annotationid", ConditionOperator.Equal, annotationid));
            var notesEntity = CRMSrv.Retrieve("annotation", annotationid, new ColumnSet(new string[] { "documentbody", "filename", "mimetype", "versionnumber" }));

            if (notesEntity != null) {
                try {
                    fileName = notesEntity.GetAttributeValue<string>("filename") ?? "";
                    mimetype = notesEntity.GetAttributeValue<string>("mimetype") ?? "";
                    versionNumber = notesEntity.GetAttributeValue<long>("versionnumber");

                    return true;
                } catch (Exception ex) {
                    throw;
                }
            }
            return false;
        }
        #endregion

        #region Method: GetValuesAsStringDictionary (public - static - extension method)
        /// <summary>
        /// returns all of the values of an entity in a dictionary format
        /// </summary>
        /// <param name="oEntity"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetValuesAsStringDictionary(this Entity oEntity) {
            return oEntity.Attributes.ToDictionary(x => x.Key, y => y.Value.ToString());
        }
        #endregion

        #region Method: GetOrganizationID (public - static - extension method)
        public static Guid GetOrganizationID(this IOrganizationService svc) {
            return ((WhoAmIResponse)svc.Execute(new WhoAmIRequest())).OrganizationId;
        }
        #endregion

        #region Method: GetOrganizationBaseCurrency (public - static - extension method)
        /// <summary>
        /// returns the base currency id for the current organization
        /// </summary>
        /// <param name="svc"></param>
        /// <returns></returns>
        public static Guid GetOrganizationBaseCurrency(this IOrganizationService svc) {
            return GetOrganizationBaseCurrency(svc, svc.GetOrganizationID());
        }
        public static Guid GetOrganizationBaseCurrency(this IOrganizationService svc, Guid orgId) {
            var currentOrganization = svc.Retrieve("organization", orgId, new ColumnSet("basecurrencyid"));
            return currentOrganization.GetAttributeValue<EntityReference>("basecurrencyid").Id;
        }
        #endregion

        #region Method: GetOrganizationBaseCurrencyName (public - static - extension method)
        /// <summary>
        /// returns the base currency id for the current organization
        /// </summary>
        /// <param name="svc"></param>
        /// <returns></returns>
        public static string GetOrganizationBaseCurrencyName(this IOrganizationService svc) {
            return GetOrganizationBaseCurrencyName(svc, svc.GetOrganizationID());
        }
        public static string GetOrganizationBaseCurrencyName(this IOrganizationService svc, Guid orgId) {
            var currentOrganization = svc.Retrieve("organization", orgId, new ColumnSet("basecurrencyid"));
            return currentOrganization.GetAttributeValue<EntityReference>("basecurrencyid").Name;
        }
        #endregion

        #region Method: GetOrganizationBaseCurrencyISOCode (public - static - extension method)
        /// <summary>
        /// returns the base currency iso code for the current organization
        /// </summary>
        /// <param name="svc"></param>
        /// <returns></returns>
        public static string GetOrganizationBaseCurrencyISOCode(this IOrganizationService svc) {
            return GetOrganizationBaseCurrencyISOCode(svc, svc.GetOrganizationID());
        }
        public static string GetOrganizationBaseCurrencyISOCode(this IOrganizationService svc, Guid orgId) {
            var currentOrganization = svc.Retrieve("organization", orgId, new ColumnSet("basecurrencyid"));
            var baseCurrencyRef = currentOrganization.GetAttributeValue<EntityReference>("basecurrencyid");

            if (baseCurrencyRef == null)
                return "";

            var baseCurrency = svc.Retrieve(baseCurrencyRef.LogicalName, baseCurrencyRef.Id, new ColumnSet("isocurrencycode"));
            return baseCurrency.GetAttributeValue<string>("isocurrencycode");
        }
        #endregion

        #region Method: GetMoneyValue (public - static - extension method)
        /// <summary>
        /// returns a decimal from a money field
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityAttributeName"></param>
        /// <returns></returns>
        public static decimal GetMoneyValue(this Entity entity, string entityAttributeName) {
            if (!entity.Attributes.Contains(entityAttributeName))
                return 0;
            var oMoney = entity.GetAttributeValue<Money>(entityAttributeName);
            return oMoney != null ? oMoney.Value : 0;
        }
        #endregion

        #region Method: GetMoneyNullableValue (public - static - extension method)
        /// <summary>
        /// returns a decimal from a money field
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityAttributeName"></param>
        /// <returns></returns>
        public static Nullable<decimal> GetMoneyNullableValue(this Entity entity, string entityAttributeName) {
            if (!entity.Attributes.Contains(entityAttributeName))
                return null;
            var oMoney = entity.GetAttributeValue<Money>(entityAttributeName);
            return oMoney != null ? oMoney.Value : (Nullable<decimal>)null;
        }
        #endregion

        #region Method: GetAttributeValue (public - static - extension method - overloaded)
        public static T GetAttributeValue<T>(this IOrganizationService oCRMSrv, EntityReference entityRef, string attributeName) {
            return oCRMSrv.GetAttributeValue<T>(entityRef.Id, entityRef.LogicalName, attributeName);
        }
        public static T GetAttributeValue<T>(this IOrganizationService oCRMSrv, Nullable<Guid> id, string entityName, string attributeName) {
            Entity oEntity = null;
            if (id.HasValue) {
                oEntity = oCRMSrv.Retrieve(entityName, id.Value, new ColumnSet(new string[] { attributeName }));
            } else {
                var qe = new QueryExpression() {
                    EntityName = entityName,
                    ColumnSet = new ColumnSet(new string[] { attributeName })
                };
                oEntity = oCRMSrv.RetrieveMultiple(qe).Entities.FirstOrDefault();
            }
            if (oEntity != null)
                return oEntity.GetAttributeValue<T>(attributeName);
            else
                return default(T);
        }
        #endregion

        #region Method: CRMEntityExists (public - static - extension method
        /// <summary>
        /// returns true if the record exists
        /// </summary>
        /// <param name="oCRMSrv"></param>
        /// <param name="id"></param>
        /// <param name="entityName"></param>
        /// <param name="primaryIDAttributeName"></param>
        /// <returns></returns>
        public static bool CRMEntityExists(this IOrganizationService oCRMSrv, Guid id, string entityName, string primaryIDAttributeName) {
            var qe = new QueryExpression(entityName) {
                ColumnSet = new ColumnSet(new string[] { primaryIDAttributeName }),
                NoLock = true,
                TopCount = 1
            };
            qe.Criteria.AddCondition(primaryIDAttributeName, ConditionOperator.Equal, id);
            var oRetEntities = oCRMSrv.RetrieveMultiple(qe);

            // return true if records exists
            return oRetEntities != null && (oRetEntities.Entities?.Count > 0);
        }
        #endregion

        #region Method: GetAttributeValues (public - static - extension method)
        public static List<Entity> GetAttributeValues(this IOrganizationService oCRMSrv, Nullable<Guid> id, string entityName, string[] attributeNames) {
            Entity oEntity = null;
            if (id.HasValue) {
                oEntity = oCRMSrv.Retrieve(entityName, id.Value, new ColumnSet(attributeNames));
                return new List<Entity>(new Entity[] { oEntity });
            } else {
                var qe = new QueryExpression() {
                    EntityName = entityName,
                    ColumnSet = new ColumnSet(attributeNames)
                };
                return oCRMSrv.RetrieveMultiple(qe).Entities.ToList();
            }
        }
        #endregion

        #region Method: SetAttributeValueIfDifferent (public - static - extension method)
        /// <summary>
        /// adds/sets the value contained in <param name="newValue"></param> to the CRM Entity 
        /// object <param name="oNewValueEntity"></param> if the old value contained in <param name="oCurrentValueEntity"></param> is different.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oNewValueEntity"></param>
        /// <param name="attributeName"></param>
        /// <param name="oCurrentValueEntity"></param>
        /// <param name="newValue"></param>
        /// <param name="treatNullAsEmpty"></param>
        public static void SetAttributeValueIfDifferent<T>(this Entity oNewValueEntity, string attributeName, Entity oCurrentValueEntity, T newValue, bool treatNullAsEmpty = false) {
            bool valuesAreDifferent = (oCurrentValueEntity == null);

            // if the oCurrentValueEntity is null then assume that the values are different
            if (!valuesAreDifferent) {
                // get the old value
                T oldValue = oCurrentValueEntity.GetAttributeValue<T>(attributeName);

                // check if the old and new value are the same first check if one of the values is null
                var oType = typeof(T);
                if (!treatNullAsEmpty || (treatNullAsEmpty && (oType != typeof(Money))))
                    valuesAreDifferent = ((oldValue == null) != (newValue == null));

                // next check based on the data types (handle specialized types like entityreference)
                if (!valuesAreDifferent) {
                    if (oType == typeof(EntityReference)) {
                        var oEntRef1 = (EntityReference)((object)newValue);
                        var oEntRef2 = (EntityReference)((object)oldValue);

                        if (oEntRef1 != null && oEntRef2 != null) {
                            valuesAreDifferent = (oEntRef1.Id != oEntRef2.Id);
                        }
                    } else {
                        if (typeof(T) == typeof(OptionSetValue)) {
                            var oOptVal1 = (OptionSetValue)((object)newValue);
                            var oOptVal2 = (OptionSetValue)((object)oldValue);

                            if (oOptVal1 != null && oOptVal2 != null) {
                                valuesAreDifferent = (oOptVal1.Value != oOptVal2.Value);
                            }
                        } else {
                            if (typeof(T) == typeof(Money)) {
                                var oMoneyVal1 = (Money)((object)newValue);
                                var oMoneyVal2 = (Money)((object)oldValue);

                                if (treatNullAsEmpty) {
                                    if ((oMoneyVal1 == null && oMoneyVal2 != null && oMoneyVal2.Value == 0) ||
                                        (oMoneyVal2 == null && oMoneyVal1 != null && oMoneyVal1.Value == 0)) {
                                        valuesAreDifferent = false;
                                    } else {
                                        if (oMoneyVal1 != null && oMoneyVal2 != null) {
                                            valuesAreDifferent = (oMoneyVal1.Value != oMoneyVal2.Value);
                                        } else {
                                            valuesAreDifferent = ((oMoneyVal1 == null) != (oMoneyVal2 == null));
                                        }
                                    }
                                } else {
                                    if (oMoneyVal1 != null && oMoneyVal2 != null) {
                                        valuesAreDifferent = (oMoneyVal1.Value != oMoneyVal2.Value);
                                    }
                                }
                            } else {
                                if (typeof(T) == typeof(Nullable<DateTime>)) {
                                    var oNullableDT1 = (Nullable<DateTime>)((object)newValue);
                                    var oNullableDT2 = (Nullable<DateTime>)((object)oldValue);

                                    if (oNullableDT1.HasValue && oNullableDT2.HasValue) {
                                        valuesAreDifferent = (oNullableDT1.Value != oNullableDT2.Value);
                                    }
                                } else {
                                    if (oldValue != null && newValue != null)
                                        valuesAreDifferent = (!oldValue.Equals(newValue));
                                }
                            }
                        }
                    }
                }
            }

            // if the values are different then add the new value to the destination entity
            if (valuesAreDifferent) {
                oNewValueEntity.Attributes[attributeName] = newValue;
            }
        }
        #endregion

        #region Method: GetOrganizationService (public - static - extension method)
        /// <summary>
        /// returns a CRM service object instance
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IOrganizationService GetOrganizationService(this IServiceProvider serviceProvider, IPluginExecutionContext context) {
            // Obtain the organization service reference which you will need for web service calls.
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            if (context == null) {
                return serviceFactory.CreateOrganizationService(null);
            } else {
                return serviceFactory.CreateOrganizationService(context.UserId);
            }
        }
        #endregion

        #region Method: GetEntityRefName (public - static - extension method)
        /// <summary>
        /// returns the name of the entity reference object from the source entity <param name="entity" /> and if not found then retrieve the value directly from CRM
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="oCRMSrv"></param>
        /// <param name="context"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="entityAttributeName"></param>
        /// <param name="refEntityName"></param>
        /// <param name="refEntityAttributeName"></param>
        /// <returns></returns>
        public static string GetEntityRefName(this Entity entity, ref IOrganizationService oCRMSrv, IPluginExecutionContext context, IServiceProvider serviceProvider,
                                               string entityAttributeName, string refEntityName, string refEntityAttributeName) {
            string oNameStr = null;
            // get the entityreference object from the source entity
            var entityRef = entity.GetAttributeValue<EntityReference>(entityAttributeName);

            // format the name
            oNameStr = "";
            if (entityRef != null && !string.IsNullOrEmpty(entityRef.Name)) {
                oNameStr = entityRef.Name;
            } else {
                // load the name directly from CRM from the referenced entity
                if (entityRef != null) {
                    if (oCRMSrv == null)
                        oCRMSrv = serviceProvider.GetOrganizationService(context);
                    oNameStr = oCRMSrv.GetAttributeValue<string>(entityRef.Id, refEntityName, refEntityAttributeName);
                }
            }
            return oNameStr;
        }
        #endregion

        #region Method: GetEntityAttributeValue (public - static - extension method)
        /// <summary>
        /// returns the id of the entity reference object from the source entity <param name="entity" /> and if not found then retrieve the value directly from CRM
        /// </summary>
        public static T GetEntityAttributeValue<T>(this Entity entity, ref IOrganizationService oCRMSrv, IPluginExecutionContext context, IServiceProvider serviceProvider,
                                                     string entityAttributeName) {
            // get the value from the source entity
            if (entity.Contains(entityAttributeName)) {
                return entity.GetAttributeValue<T>(entityAttributeName);
            }
            // load the id directly from CRM from the referenced entity
            if (oCRMSrv == null)
                oCRMSrv = serviceProvider.GetOrganizationService(context);
            return oCRMSrv.GetAttributeValue<T>(entity.Id, entity.LogicalName, entityAttributeName);
        }
        #endregion

        #region Method: GetTopContext (public - static - extension method)
        /// <summary>
        /// Return the context of the first parent in the context hierarchy
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IPluginExecutionContext GetTopContext(this IPluginExecutionContext context) {
            IPluginExecutionContext topContext = context;
            while (topContext.ParentContext != null) {
                topContext = topContext.ParentContext;
            }
            return topContext;
        }
        #endregion

        #region Method: GetOptionSetAttribueValueOrNullFromAliasedValue (public - static - extension method)
        public static Nullable<int> GetOptionSetAttribueValueOrNullFromAliasedValue(this Entity oEntity, string attributeName) {
            object objData = GetAttributevalueFromAliasedValue(oEntity, attributeName);
            if (objData != null) {
                if (objData is OptionSetValue) {
                    var oOptionSetVal = (objData as OptionSetValue);
                    if (oOptionSetVal != null)
                        return oOptionSetVal.Value;
                } else {
                    if (objData is int) {
                        return (int)objData;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Method: RetrieveSingleValueFromCRM (public - static - extension method)
        /// <summary>
        /// Retrieve a single attribute from CRM
        /// </summary>
        /// <param name="oEntityRef"></param>
        /// <param name="oCRMSrv"></param>
        /// <param name="context"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static object RetrieveSingleValueFromCRM(this EntityReference oEntityRef, ref IOrganizationService oCRMSrv, IPluginExecutionContext context, IServiceProvider serviceProvider, string attributeName) {
            if (oEntityRef == null || string.IsNullOrWhiteSpace(attributeName))
                return null;

            if (oCRMSrv == null)
                oCRMSrv = serviceProvider.GetOrganizationService(context);

            var oResponse = oCRMSrv.Retrieve(oEntityRef.LogicalName, oEntityRef.Id, new ColumnSet(attributeName));
            if (oResponse != null && oResponse.Attributes.Count > 0 && oResponse.Attributes.Contains(attributeName)) {
                return oResponse.Attributes[attributeName];
            }
            return null;
        }
        #endregion

        #region Method: GetRecordIDFromAttributeValue (public - static - extension method)
        /// <summary>
        /// returns the id of the record from <param name="entityName"></param>, which matches for <param name="attributeName"></param> the value <param name="attributeMatchingValue"></param>
        /// </summary>
        /// <param name="oCRMSrv"></param>
        /// <param name="entityName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeMatchingValue"></param>
        /// <returns></returns>
        public static Nullable<Guid> GetRecordIDFromAttributeValue(this IOrganizationService oCRMSrv, string entityName,
                                                         string attributeName, object attributeMatchingValue) {
            var qe = new QueryByAttribute(entityName) {
                ColumnSet = new ColumnSet(new string[] { attributeName })
            };
            qe.AddAttributeValue(attributeName, attributeMatchingValue);

            var oRetunRecords = oCRMSrv.RetrieveMultiple(qe).Entities;
            if (oRetunRecords.Count > 0)
                return oRetunRecords.First().Id;
            else
                return null;
        }
        #endregion

        #region Method: IsEmailAddressValid (public - static - extension method)
        public static bool IsEmailAddressValid(this string email) {
            bool validEmail = false;
            if (!string.IsNullOrEmpty(email)) {
                try {
                    MailAddress em = new MailAddress(email);
                    validEmail = true;
                } catch (FormatException) {
                    validEmail = false;
                }
            }
            return validEmail;
        }
        #endregion

        #region Method: CustomerHasValidEmail (public - static)
        public static bool CustomerHasValidEmail(IOrganizationService svc, EntityReference customerRef) {
            // Check if user has an valid email address and allows emails

            if (customerRef == null || (!customerRef.LogicalName.ToLower().Equals("account") && !customerRef.LogicalName.ToLower().Equals("contact")))
                return false;

            Entity oCustomerEntity = svc.Retrieve(customerRef.LogicalName, customerRef.Id, new ColumnSet("emailaddress1", "donotemail"));

            // Check if field 'Do not allow Email' is set to Do Not Allow 
            if (oCustomerEntity.Contains("donotemail") && oCustomerEntity["donotemail"] != null) {
                if ((bool)oCustomerEntity["donotemail"] == true)
                    return false;
            }

            // Check if the customer has an email address
            if (!oCustomerEntity.Contains("emailaddress1") || oCustomerEntity["emailaddress1"] == null || string.IsNullOrWhiteSpace(oCustomerEntity["emailaddress1"].ToString()))
                return false;

            // If email address is a valid email address then return true
            return oCustomerEntity.Attributes["emailaddress1"].ToString().IsEmailAddressValid();
        }
        #endregion

        #region Method: Associate (public - static - extension method)
        /// <summary>
        /// Associate an entity with another entity
        /// </summary>
        /// <param name="oCRMSrv"></param>
        /// <param name="entityRef"></param>
        /// <param name="relatedEntityRef"></param>
        /// <param name="relationshipName"></param>
        public static void Associate(this EntityReference entityRef, IOrganizationService oCRMSrv, EntityReference relatedEntityRef, string relationshipName) {
            AssociateRequest req = new AssociateRequest {
                Target = relatedEntityRef,
                RelatedEntities = new EntityReferenceCollection { entityRef },
                Relationship = new Relationship(relationshipName)
            };

            if (string.Equals(entityRef.LogicalName, relatedEntityRef.LogicalName)) {
                req.Relationship.PrimaryEntityRole = EntityRole.Referenced;
            }

            oCRMSrv.Execute(req);
        }
        #endregion

        #region Method: GetTimeZone (public - static - extension method)
        public static TimeZoneInfo GetTimeZone(this IOrganizationService oCRMService, int timeZoneIndex) {
            var qa = new QueryByAttribute("timezonedefinition") {
                ColumnSet = new ColumnSet("standardname")
            };
            qa.AddAttributeValue("timezonecode", timeZoneIndex);
            return TimeZoneInfo.FindSystemTimeZoneById(oCRMService.RetrieveMultiple(qa).Entities.First().GetAttributeValue<string>("standardname"));
        }
        #endregion

        #region Method: RetrieveCurrentUsersTimeZoneSettings (public - static - extension method)
        public static int? RetrieveCurrentUsersTimeZoneSettings(this IOrganizationService oCRMService) {
            var userSettings = oCRMService.RetrieveMultiple(
            new QueryExpression("usersettings") {
                ColumnSet = new ColumnSet("localeid", "timezonecode"),
                Criteria = new FilterExpression {
                    Conditions = { new ConditionExpression("systemuserid", ConditionOperator.EqualUserId) }
                }
            });

            // if user settings were found for this user then return the timezone code
            if (userSettings != null && userSettings.Entities.Count > 0)
                return (int?)userSettings.Entities[0].Attributes["timezonecode"];

            return null;
        }
        #endregion

        #region Method: GetOptionsSetValues (public - static)
        public static Dictionary<int, string> GetOptionsSetValues(IOrganizationService service, string entityName, string attributeName) {
            var oList = new Dictionary<int, string>();
            var retrieveAttributeRequest = new RetrieveAttributeRequest {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            var reqRespMD = ((RetrieveAttributeResponse)(service.Execute(retrieveAttributeRequest))).AttributeMetadata;
            OptionMetadata[] optionList = null;
            if (reqRespMD is Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata) {
                var retrievedStatusAttributeMetadata = (StatusAttributeMetadata)reqRespMD;
                optionList = retrievedStatusAttributeMetadata.OptionSet.Options.ToArray();
            } else {
                var retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)reqRespMD;
                optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
            }

            // loop through the option set and add the ids and labels to the list
            foreach (OptionMetadata oMD in optionList) {
                if (oMD.Value.HasValue && !string.IsNullOrWhiteSpace(oMD.Label.UserLocalizedLabel.Label))
                    oList.Add(oMD.Value.Value, oMD.Label.UserLocalizedLabel.Label);
            }

            // return the populated list
            return oList;
        }
        #endregion

        #region Method: GetBoolManagedPropertyValue (public - static - extension method)
        /// <summary>
        /// returns a nullable bool from a bool manageed property
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityAttributeName"></param>
        /// <returns></returns>
        public static Nullable<bool> GetBoolManagedPropertyValue(this Entity entity, string entityAttributeName) {
            if (!entity.Attributes.Contains(entityAttributeName))
                return null;
            var oBoolManagedProp = entity.GetAttributeValue<BooleanManagedProperty>(entityAttributeName);
            if (oBoolManagedProp == null)
                return null;
            else
                return oBoolManagedProp.Value;
        }
        #endregion

        public static string GetDisplayLabel(this EntityMetadata oEntityMD) {
            var oEntityNameLabel = oEntityMD.DisplayName.LocalizedLabels.FirstOrDefault();
            if (oEntityNameLabel != null)
                return oEntityNameLabel.Label;
            return "";
        }

        public static bool GetValueOrDefault(this BooleanManagedProperty oBoolManProp, bool defaultValue = false) {
            if (oBoolManProp == null)
                return defaultValue;
            return oBoolManProp.Value;
        }
        
    } // Class: CRMUtilities

} // namespace: DotCyToolboxPlugins.Utils