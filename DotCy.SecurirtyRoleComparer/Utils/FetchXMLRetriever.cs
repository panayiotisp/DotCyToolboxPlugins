using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


namespace DotCyToolboxPlugins.Utils {

    public static class FetchXMLRetriever {

        public sealed class AggregateTypes {
            private AggregateTypes() { }

            public static readonly string count = "count";
            public static readonly string avg = "avg";
            public static readonly string countcolumn = "countcolumn";
            public static readonly string max = "max";
            public static readonly string min = "min";
            public static readonly string sum = "sum";
        }


        #region Method: FetchAggregate (public - static - overloaded)
        public static AliasedValue FetchAggregate(string entityName, string fieldName, string aType, IOrganizationService service) {
            string fetchXml = null;
            try {
                fetchXml = String.Format(@"<fetch distinct='false' mapping='logical' aggregate='true' no-lock='true'> 
                                            <entity name='{0}'> 
                                                <attribute name='{1}' alias='aValue' aggregate='{2}' /> 
                                            </entity> 
                                        </fetch>", entityName, fieldName, aType);
                AliasedValue x = null;
                EntityCollection eCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                foreach (var c in eCollection.Entities) {
                    x = (AliasedValue)c["aValue"];
                }

                return x;
            } catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> exFalt) {
                throw;
            } catch (System.Exception) {
                throw;
            }
        }

        public static AliasedValue FetchAggregate(string fetchXml, IOrganizationService service, string aliasName = "aValue") {
            try {
                AliasedValue x = null;
                EntityCollection eCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                var oEntity = eCollection.Entities.FirstOrDefault();
                if (oEntity != null) {
                    x = oEntity.GetAttributeValue<AliasedValue>(aliasName);
                }
                return x;
            } catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> exFalt) {
                throw;
            } catch (System.Exception) {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// returns all the aggregate values in a fetchxml, indexed by aliased name
        /// </summary>
        /// <param name="fetchXml"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static Dictionary<string, AliasedValue> FetchAllAliasesForAggregate(string fetchXml, IOrganizationService service) {
            try {
                EntityCollection eCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                var oEntity = eCollection.Entities.FirstOrDefault();
                if (oEntity != null) {
                    return oEntity.Attributes.Where(x => x.Value is AliasedValue).ToDictionary(x => x.Key, x => oEntity.GetAttributeValue<AliasedValue>(x.Key));
                }
                return new Dictionary<string, AliasedValue>();
            } catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> exFalt) {
                throw;
            } catch (System.Exception) {
                throw;
            }
        }

        #region Method: FetchAll (public - static)
        public static Dictionary<Guid, Entity> FetchAll(string fetchXml, IOrganizationService service) {
            try {
                Dictionary<Guid, Entity> rCollection = new Dictionary<Guid, Entity>();

                // Define the fetch attributes.
                // Set the number of records per page to retrieve.
                int fetchCount = 5000;
                // Initialize the page number.
                int pageNumber = 1;

                // Specify the current paging cookie. For retrieving the first page, 
                // pagingCookie should be null.
                string pagingCookie = null;

                while (true) {
                    // Build fetchXml string with the placeholders.
                    string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                    // Execute the fetch query and get the xml result.
                    RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest {
                        Query = new FetchExpression(xml)
                    };

                    EntityCollection pageCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;

                    foreach (var c in pageCollection.Entities) {
                        rCollection.Add(c.Id, c);
                    }

                    // Check for more records, if it returns 1.
                    if (pageCollection.MoreRecords) {
                        // Increment the page number to retrieve the next page.
                        pageNumber++;
                    } else {
                        // If no more records in the result nodes, exit the loop.
                        break;
                    }
                }

                return rCollection;
            } catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> exFalt) {
                throw;
            } catch (System.Exception ex) {
                throw;
            }
        }
        #endregion

        #region Method: FetchAllAsList (public - static)
        public static List<Entity> FetchAllAsList(string fetchXml, IOrganizationService service, int topX = 0, int fetchCount = 5000, int pageNumber = 1) {
            try {
                var rCollection = new List<Entity>();

                // default to topx if the fetchcount is greater
                if (topX > 0 && fetchCount > topX)
                    fetchCount = topX;

                // Specify the current paging cookie. For retrieving the first page, 
                // pagingCookie should be null.
                string pagingCookie = null;

                while (true) {
                    // Build fetchXml string with the placeholders.
                    string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                    // Execute the fetch query and get the xml result.
                    var fetchRequest1 = new RetrieveMultipleRequest {
                        Query = new FetchExpression(xml)
                    };

                    var pageCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;

                    foreach (var c in pageCollection.Entities) {
                        rCollection.Add(c);
                    }

                    // Check for more records, if it returns 1.
                    if (pageCollection.MoreRecords && (topX == 0 || topX > rCollection.Count)) {
                        // Increment the page number to retrieve the next page.
                        pageNumber++;
                    } else {
                        // If no more records in the result nodes, exit the loop.
                        break;
                    }
                }

                return rCollection;
            } catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> exFalt) {
                throw;
            } catch (System.Exception ex) {
                throw;
            }
        }
        #endregion

        #region Method: CreateXml (static)
        private static string CreateXml(string xml, string cookie, int page, int count) {
            using (StringReader stringReader = new StringReader(xml)) {
                using (XmlTextReader reader = new XmlTextReader(stringReader)) {

                    // Load document
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);

                    XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

                    if (cookie != null) {
                        XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                        pagingAttr.Value = cookie;
                        attrs.Append(pagingAttr);
                    }

                    XmlAttribute pageAttr = doc.CreateAttribute("page");
                    pageAttr.Value = System.Convert.ToString(page);
                    attrs.Append(pageAttr);

                    XmlAttribute countAttr = doc.CreateAttribute("count");
                    countAttr.Value = System.Convert.ToString(count);
                    attrs.Append(countAttr);

                    var sb = new StringBuilder(1024);
                    using (StringWriter stringWriter = new StringWriter(sb)) {

                        using (var writer = new XmlTextWriter(stringWriter)) {
                            doc.WriteTo(writer);
                            writer.Close();
                        }

                        return sb.ToString();
                    }
                }
            }
        }
        #endregion

    } // Class: FetchXMLRetriever

} // namespace: DotCyToolboxPlugins.Utils
