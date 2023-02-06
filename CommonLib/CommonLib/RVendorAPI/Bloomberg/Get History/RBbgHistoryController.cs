using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using System.IO;
using System.Data;

namespace com.ivp.srm.vendorapi.bloomberg
{
    public class RBbgHistoryController : IVendorForGetHistory
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgHistoryController");
        public void GetHistory(RBbgHistoryInfo historyInfo)
        {
            //call ftp method to get data
            //string timeStamp = string.Format(string.Format(RCorpActionConstant.DATEFORMAT, RCorpActUtils.GetTargetDateTime()));
            string timeStamp = RCorpActUtils.GetTargetDateTime();
            var dictHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                mLogger.Debug("RBbgHistoryController:GetHistory=> start getting History data");
                RbbgHistoryFtpHandler ftpHandler = new RbbgHistoryFtpHandler();
                RBbgHistorySAPIHandler sapiHandler = new RBbgHistorySAPIHandler();
                RBbgHistoryUtils.RemoveDirtyEntries(historyInfo);
                string requestId = Guid.NewGuid().ToString();
                if (historyInfo.SecurityIds.Count > 0)
                {
                    string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.GETHISTORYTRANSPORTNAME);

                    if (historyInfo.RequestType == RHistoryRequestType.FTP && string.IsNullOrEmpty(historyInfo.TransportName) && !string.IsNullOrEmpty(transportName))
                    {
                        historyInfo.TransportName = transportName;
                    }

                    RBbgHistoryUtils.InsertIntoHistory(historyInfo, requestId, timeStamp);
                    if (historyInfo.RequestType == RHistoryRequestType.FTP)
                        ftpHandler.GetHistory(historyInfo, timeStamp, ref dictHeaders);

                    else
                        sapiHandler.GetHistory(historyInfo, timeStamp);
                }
                if (historyInfo.ResultantData.Tables.Count > 0 && historyInfo.ResultantData.Tables[0].Rows.Count > 0)
                {
                    StringBuilder processedFields = new StringBuilder();
                    StringBuilder processedInstruments = new StringBuilder();
                    historyInfo.ProcessedInstruments.ForEach(q => processedInstruments.Append(q + RCorpActionConstant.COMMA));
                    historyInfo.ProcessedFields.ForEach(q => processedFields.Append(q + RCorpActionConstant.COMMA));
                    RVendorUtils.UpdateVendorHistory(processedInstruments.ToString(), processedFields.ToString(), timeStamp, RVendorStatusConstant.PASSED);
                    RBbgUtils.UpdateBBGAuditHist(timeStamp, historyInfo.ProcessedInstruments, historyInfo.ProcessedFields);
                    if (!historyInfo.IsImmediate)
                    {
                        string guid = Guid.NewGuid().ToString();
                        historyInfo.RequestId = guid;
                        historyInfo.ResultantData.WriteXml(Path.Combine(RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY)), guid + ".xml"), System.Data.XmlWriteMode.WriteSchema);
                    }
                }
                else
                {
                    RVendorUtils.UpdateVendorHistory(string.Empty, string.Empty, timeStamp, RVendorStatusConstant.FAILED);
                    RVendorUtils.UpdateBBGAuditStatus(timeStamp);
                }
                mLogger.Debug("RBbgHistoryController:GetHistory=> End getting History data");
            }
            catch (Exception ex)
            {
                RVendorUtils.UpdateVendorHistory(string.Empty, string.Empty, timeStamp, RVendorStatusConstant.FAILED);
                mLogger.Error("RBbgHistoryController:GetHistory=>" + ex.Message);
                throw ex;
            }
        }
        public DataSet GetResponse(string RequestId, int VendorPreferenceId)
        {
            try
            {
                string path = Path.Combine(RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY)), RequestId + ".xml");
                DataSet resultantDataset = new DataSet();
                if (File.Exists(path))
                {
                    resultantDataset.ReadXml(path, XmlReadMode.ReadSchema);
                }
                return resultantDataset;
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryController:GetResponse=>" + ex.ToString());
                throw ex;
            }
        }
        public DataSet GetResponse(string RequestId)
        {
            return GetResponse(RequestId, 1);
        }
    }
}
