/***************************************************************************************************
 * 
 *  This source forms a part of the IVP RAD Software System and is a copyright of 
 *  Indus Valley Partners (Pvt) Ltd.

 *  All rights are reserved with IVP. No part of this work may be reproduced, stored, 
 *  adopted or transmitted in any form or by any means including but not limiting to 
 *  electronic, mechanical, photographic, graphic, optic recording or otherwise, 
 *  translated in any language or computer language, without the prior written permission 
 *  of

 *  Indus Valley Partners (Pvt) Ltd
 *  Unit 7&8, Bldg 4
 *  Vardhman Trade Center
 *  Nehru Place Greens
 *  New Delhi - 19

 *  Copyright 2007-2008 Indus Valley Partners (Pvt) Ltd.
 * 
 * 
 * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            24-10-2008      Manoj          Initial Version
 * 1            04-11-2008      Nitin Saxena   Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using System.Xml;
using com.ivp.srm.vendorapi.bloombergServices.lite;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;
using System.Data;
using System.IO;
using com.ivp.srmcommon;
using System.Collections.Concurrent;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Config Loader for Vendor API
    /// </summary>
    public class RVendorConfigLoader
    {
        #region "Member Variables"
        private static ConcurrentDictionary<string,bool> INIT = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string,DateTime> TIMESTAMP = new ConcurrentDictionary<string, DateTime>();
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> mVendorConfig =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        public static Dictionary<string, Dictionary<RBbgMacroType, List<RMacroConfigInfo>>> mMacroConfig =
           new Dictionary<string, Dictionary<RBbgMacroType, List<RMacroConfigInfo>>>();
        private static Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>> mVendorConfigNew = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>();
        private static Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> mVendorHeadersgNew = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
        #endregion

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public static void LoadConfiguration(string clientName)
        {
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RefMDBVendorConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            DateTime date;
            try
            {
                if(!INIT.ContainsKey(clientName))
                    INIT.TryAdd(clientName,false);
                if (!TIMESTAMP.ContainsKey(clientName))
                    TIMESTAMP.TryAdd(clientName, DateTime.MinValue);

                date = Convert.ToDateTime(mDBConn.ExecuteQuery(@"SELECT MAX(last_modified_on) AS last_modified_on
                FROM(
                    SELECT last_modified_on FROM dbo.ivp_rad_vendor_management_master
                    UNION ALL
                    SELECT last_modified_on FROM dbo.ivp_rad_vendor_management_details
                    UNION ALL
                    SELECT last_modified_on FROM dbo.ivp_rad_vendor_management_headers
                )tab", RQueryType.Select).Tables[0].Rows[0][0]);

                if (!INIT[clientName] || TIMESTAMP[clientName] == null || TIMESTAMP[clientName] < date)
                {
                    if (!mVendorConfig.ContainsKey(clientName))
                        mVendorConfig.Add(clientName, new Dictionary<string, Dictionary<string, string>>());
                    TIMESTAMP[clientName] = DateTime.Now;
                    mLogger.Debug("Start -> Load Configuration for View Config");

                    List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("vendormanagement");
                    foreach (XmlDocument xmlDoc in lstConfig)
                    {
                        if (xmlDoc.HasChildNodes)
                        {
                            foreach (XmlNode node in xmlDoc.FirstChild.ChildNodes)
                            {
                                XmlNodeList nodeList = node.ChildNodes;
                                mVendorConfig[clientName][node.Name] = IterateChildNodes(nodeList, clientName);
                            }
                        }
                    }

                    DataSet ds = mDBConn.ExecuteQuery(@"SELECT vmas.vendor_management_id, CASE WHEN vdet.vendor_id = 1 THEN 'Bloomberg'
										                    WHEN vdet.vendor_id = 2 THEN 'Reuters' END AS vendor_name
								                    , key_real_name, key_value
                    FROM dbo.ivp_rad_vendor_management_master vmas
                    INNER JOIN dbo.ivp_rad_vendor_management_details vdet
                    ON vmas.vendor_management_id = vdet.vendor_management_id
                    WHERE is_active = 1 AND (vendor_id = 1 OR vendor_id = 2)

                    SELECT vmas.vendor_management_id, CASE WHEN vendor_id = 1 THEN 'Bloomberg'
										                    WHEN vendor_id = 2 THEN 'Reuters' END AS vendor_name
								                    , header_type_name, header_name, header_value
                    FROM dbo.ivp_rad_vendor_management_master vmas
                    INNER JOIN dbo.ivp_rad_vendor_management_headers vhead
                    ON vmas.vendor_management_id = vhead.vendor_management_id
                    INNER JOIN dbo.ivp_rad_vendor_management_header_type vhtyp
                    ON vhtyp.header_type_id = vhead.header_type_id
                    WHERE is_active = 1 AND (vendor_id = 1 OR vendor_id = 2)", RQueryType.Select);

                    mVendorConfigNew = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>();
                    mVendorHeadersgNew = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        if (!mVendorConfigNew.ContainsKey(clientName))
                            mVendorConfigNew.Add(clientName, new Dictionary<int, Dictionary<string, Dictionary<string, string>>>());
                        mVendorConfigNew[clientName] = ds.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["vendor_management_id"])).ToDictionary(x => x.Key, y => y.GroupBy(a => Convert.ToString(a["vendor_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(b => b.Key, c => c.ToDictionary(d => Convert.ToString(d["key_real_name"]), e => Convert.ToString(e["key_value"]), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase));
                    }
                    else
                    {
                        if (!mVendorConfigNew.ContainsKey(clientName))
                            mVendorConfigNew.Add(clientName, new Dictionary<int, Dictionary<string, Dictionary<string, string>>>());
                    }

                    if (ds != null && ds.Tables[1].Rows.Count > 0)
                    {
                        if (!mVendorHeadersgNew.ContainsKey(clientName))
                            mVendorHeadersgNew.Add(clientName, new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                        mVendorHeadersgNew[clientName] = ds.Tables[1].AsEnumerable().GroupBy(x => Convert.ToInt32(x["vendor_management_id"])).ToDictionary(x => x.Key, y => y.GroupBy(a => Convert.ToString(a["vendor_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(b => b.Key, c => c.GroupBy(d => Convert.ToString(d["header_type_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(e => e.Key, f => f.ToDictionary(g => Convert.ToString(g["header_name"]), h => Convert.ToString(h["header_value"]), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase));
                    }
                    else
                    {
                        if (!mVendorHeadersgNew.ContainsKey(clientName))
                            mVendorHeadersgNew.Add(clientName, new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                    }

                    INIT[clientName] = true;
                    mLogger.Debug("End -> Load Configuration for View Config");
                }

            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Iterates the child nodes.
        /// </summary>
        /// <param name="nodeList">The node list.</param>
        private static Dictionary<string, string> IterateChildNodes(XmlNodeList nodeList, string clientName)
        {
            Dictionary<string, string> dictChildren = null;
            dictChildren = new Dictionary<string, string>();
            foreach (XmlNode node in nodeList)
            {
                if (node.Name.ToLower().Equals("macroqualifiers"))
                {
                    if (node.HasChildNodes)
                        IterateMacroChildNodes(node.ChildNodes, clientName);
                }
                else if (node.Name.Equals("Fundamentals", StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else if (node.Name.Equals("BVAL", StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else if (node.Name.Equals("getcompanyheaders", StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else
                    dictChildren[node.Attributes[0].InnerText] = node.Attributes[1].InnerText;
            }
            return dictChildren;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Iterates the macro's child nodes.
        /// </summary>
        private static void IterateMacroChildNodes(XmlNodeList nodeList, string clientName)
        {
            if (!mMacroConfig.ContainsKey(clientName))
                mMacroConfig.Add(clientName, new Dictionary<RBbgMacroType, List<RMacroConfigInfo>>());
            List<RMacroConfigInfo> lstConfigInfo = null;
            foreach (XmlNode node in nodeList)
            {
                lstConfigInfo = new List<RMacroConfigInfo>();
                lstConfigInfo = PopulateMacroListInfo(node, node.Name.ToLower());
                if (node.Name.ToLower().Equals("primaryqualifiers"))
                    mMacroConfig[clientName][RBbgMacroType.Primary] = lstConfigInfo;
                if (node.Name.ToLower().Equals("secondaryqualifiers"))
                    mMacroConfig[clientName][RBbgMacroType.Secondary] = lstConfigInfo;
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Populates the macro list info.
        /// </summary>
        private static List<RMacroConfigInfo> PopulateMacroListInfo(XmlNode nodeList, string qualifierType)
        {
            List<RMacroConfigInfo> lstChildren = null;
            lstChildren = new List<RMacroConfigInfo>();
            RMacroConfigInfo macroConfigInfo = null;
            for (int count = 0; count < nodeList.ChildNodes.Count; count++)
            {
                macroConfigInfo = new RMacroConfigInfo();
                XmlNode node = nodeList.ChildNodes[count];
                if (node.Attributes.Count > 0)
                    macroConfigInfo.Name = node.Attributes[0].InnerText;
                if (node.Attributes.Count > 1)
                    macroConfigInfo.Value = node.Attributes[1].InnerText;
                if (qualifierType.Equals("secondaryqualifiers") &&
                            node.Attributes.Count > 2)
                {
                    macroConfigInfo.PQType = node.Attributes[2].InnerText;
                    if (node.HasChildNodes)
                    {
                        List<string> lstPQValues = new List<string>();
                        XmlNodeList nodePQList = node.ChildNodes;
                        for (int i = 0; i < nodePQList.Count; i++)
                        {
                            lstPQValues.Insert(i, nodePQList[i].ChildNodes[i].InnerText);
                        }
                        macroConfigInfo.PQValues = lstPQValues;
                    }
                }
                lstChildren.Insert(count, macroConfigInfo);
            }
            return lstChildren;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public static string GetVendorConfig(RVendorType vendorType, int VendorPreferenceId, string keyName)
        {
            string clientName = SRMMTConfig.GetClientName();

            LoadConfiguration(clientName);

            if (VendorPreferenceId == 0)
            {
                VendorPreferenceId = 1;
                mLogger.Debug("VendorPreferenceId not provided. Defaulted to Default preference");
            }
            if (!mVendorConfigNew.ContainsKey(clientName))
                mVendorConfigNew.Add(clientName, new Dictionary<int, Dictionary<string, Dictionary<string, string>>>());

            if (mVendorConfigNew != null && mVendorConfigNew[clientName].ContainsKey(VendorPreferenceId))
            {
                if (mVendorConfigNew[clientName][VendorPreferenceId].ContainsKey(vendorType.ToString()) && mVendorConfigNew[clientName][VendorPreferenceId][vendorType.ToString()].ContainsKey(keyName))
                    return mVendorConfigNew[clientName][VendorPreferenceId][vendorType.ToString()][keyName];
                else
                    return null;
            }
            else
                throw new Exception("VendorPreferenceId " + VendorPreferenceId + " is invalid.");
        }

        public static Dictionary<string, string> GetVendorHeaders(RVendorType vendorType, int VendorPreferenceId, string headerType)
        {
            string clientName = SRMMTConfig.GetClientName();

            LoadConfiguration(clientName);

            if (VendorPreferenceId == 0)
            {
                VendorPreferenceId = 1;
                mLogger.Debug("VendorPreferenceId not provided. Defaulted to Default preference");
            }
            if (mVendorHeadersgNew != null && mVendorHeadersgNew[clientName].ContainsKey(VendorPreferenceId) && mVendorHeadersgNew[clientName][VendorPreferenceId].ContainsKey(vendorType.ToString()) && mVendorHeadersgNew[clientName][VendorPreferenceId][vendorType.ToString()].ContainsKey(headerType))
                return mVendorHeadersgNew[clientName][VendorPreferenceId][vendorType.ToString()][headerType].ToDictionary(x => x.Key, y => y.Value, StringComparer.OrdinalIgnoreCase);
            else
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public static string GetDecryptToolPath(int VendorPreferenceId)
        {
            string decryptToolPath = GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.FTPDECRYPTTOOLPATH);

            if (!string.IsNullOrEmpty(decryptToolPath) && !Path.IsPathRooted(decryptToolPath))
            {
                decryptToolPath = RADConfigReader.GetServerPhysicalPath() + decryptToolPath;
                //string decryptToolPathTemp = RADConfigReader.GetServerPhysicalPath() + decryptToolPath;
                //if (File.Exists(decryptToolPathTemp))
                //    decryptToolPath = decryptToolPathTemp;
            }

            return decryptToolPath;
        }
    }
}
