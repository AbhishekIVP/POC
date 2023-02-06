using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.rad.configurationmanagement;
using System.IO;
using System.Data;

namespace com.ivp.srm.vendorapi.Bloomberg.Fundamentals
{
    public class RBbgFundamentalController : IVendorForFundamentals
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgFundamentalController");
        static object obj = new object();
        public void GetFundamentals(RBbgFundamentalInfo secInfo)
        {
            string timeStamp = string.Empty;
            lock (obj)
            {
                //timeStamp = string.Format(string.Format(RCorpActionConstant.DATEFORMAT, RCorpActUtils.GetTargetDateTime()));
                timeStamp = RCorpActUtils.GetTargetDateTime();
            }
            try
            {
                mLogger.Debug("RBbgFundamentalController:GetFundamentals=> start getting fundamental data");
                RBbgFundamentalFTPHandler ftpHandler = new RBbgFundamentalFTPHandler();
                RBbgFundamentalUtils.RemoveDirtyEntries(secInfo);
                string requestId = Guid.NewGuid().ToString();
                secInfo.RequestIdentifier = requestId;
                if (secInfo.Instruments.Count > 0)
                {
                    string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.GETFUNDAMENTALSTRANSPORTNAME);

                    if (string.IsNullOrEmpty(secInfo.TransportName) && !string.IsNullOrEmpty(transportName))
                    {
                        secInfo.TransportName = transportName;
                    }

                    RBbgFundamentalUtils.InsertIntoDatabase(secInfo, requestId, timeStamp);
                    ftpHandler.GetFundamental(secInfo, timeStamp);
                }
                if (secInfo.ResultantData.Tables.Count > 0 && secInfo.ResultantData.Tables[0].Rows.Count > 0 && secInfo.ImmediateRequest)
                {
                    StringBuilder processedFields = new StringBuilder();
                    StringBuilder processedInstruments = new StringBuilder();
                    secInfo.ProcessedInstruments.ForEach(q => processedInstruments.Append(q + RCorpActionConstant.COMMA));
                    secInfo.ProcessedFields.ForEach(q => processedFields.Append(q + RCorpActionConstant.COMMA));
                    RVendorUtils.UpdateVendorHistory(processedInstruments.ToString(), processedFields.ToString(), timeStamp, RVendorStatusConstant.PASSED);
                }
                else
                {
                    RVendorUtils.UpdateVendorHistory(string.Empty, string.Empty, timeStamp, RVendorStatusConstant.FAILED);
                }
            }
            catch (Exception ex)
            {
                RVendorUtils.UpdateVendorHistory(string.Empty, string.Empty, timeStamp, RVendorStatusConstant.FAILED);
                mLogger.Error("RBbgFundamentalController:GetFundamentals=>" + ex.Message);
                throw ex;
            }
        }

        public DataSet GetResponse(string RequestId, string transportName, int VendorPreferenceId)
        {
            try
            {
                mLogger.Debug("RBbgFundamentalController:GetFundamentals=> start getting fundamental response data");

                string transportNameVendorPreference = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.GETFUNDAMENTALSTRANSPORTNAME);

                if (string.IsNullOrEmpty(transportName) && !string.IsNullOrEmpty(transportNameVendorPreference))
                {
                    transportName = transportNameVendorPreference;
                }

                List<RVendorHistoryInfo> historyList = RVendorUtils.GetVendorHistory(RequestId);
                RBbgFundamentalInfo info = new RBbgFundamentalInfo();
                PrepareInfo(historyList[0], info);
                RBbgFundamentalFTPHandler cntrl = new RBbgFundamentalFTPHandler();
                cntrl.GetResponse(transportName, historyList[0].TimeStamp, info);
                mLogger.Debug("RBbgFundamentalController:GetFundamentals=> end getting fundamental response data");
                return info.ResultantData;
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalController:GetFundamentals=> " + ex.ToString());
                throw ex;
            }
        }

        private void PrepareInfo(RVendorHistoryInfo vendorHistory, RBbgFundamentalInfo securityInfo)
        {
            List<RBbgInstrumentInfo> lstInstrument = null;
            List<RBbgFieldInfo> lstFields = null;
            securityInfo.TransportName = vendorHistory.TransportName;
            #region Instruments
            string[] instrumentsInfo = vendorHistory.VendorInstruments.Split(',');
            for (int i = 0; i < instrumentsInfo.Length; i++)
            {
                RBbgInstrumentInfo info = new RBbgInstrumentInfo();
                info.InstrumentID = instrumentsInfo[i];
                if (!lstInstrument.Contains(info))
                    lstInstrument.Add(info);
            }
            #endregion
            #region Fields
            string[] fieldsInfo = vendorHistory.VendorFields.Split(',');
            for (int i = 0; i < fieldsInfo.Length; i++)
            {
                RBbgFieldInfo info = new RBbgFieldInfo();
                info.Mnemonic = fieldsInfo[i];
                if (!lstFields.Contains(info))
                    lstFields.Add(info);
            }
            #endregion
            securityInfo.Instruments = lstInstrument;
            securityInfo.InstrumentFields = lstFields;
        }
    }
}
