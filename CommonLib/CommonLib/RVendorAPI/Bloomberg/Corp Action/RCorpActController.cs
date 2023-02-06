using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
                                System.Collections.Generic.
                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using System.Data;

namespace com.ivp.srm.vendorapi.bloomberg
{
    public class RCorpActController : IVendorForCorpAction
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        public void GetCorpAction(RCorpActInfo corpActInfo)
        {
            string timeStamp = string.Format(string.Format(RCorpActionConstant.DATEFORMAT, RCorpActUtils.GetTargetDateTime()));
            var dictHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //call ftp method to get data
            try
            {
                mLogger.Debug("RCorpActController:GetCorpAction=> start getting corp action");
                RCorpActFtpHandler ftpHandler = new RCorpActFtpHandler();
                RCorpActHeavyHandler heavyHandler = new RCorpActHeavyHandler();
                RCorpActUtils.RemoveDirtyEntries(corpActInfo);
                string requestId = Guid.NewGuid().ToString();
                if (corpActInfo.SecurityIds.Count > 0)
                {
                    string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.GETACTIONSTRANSPORTNAME);

                    if (corpActInfo.RequestType == RBbgCorpActRequestType.FTP && string.IsNullOrEmpty(corpActInfo.TransportName) && !string.IsNullOrEmpty(transportName))
                    {
                        corpActInfo.TransportName = transportName;
                    }

                    RCorpActUtils.InsertIntoHistory(corpActInfo, requestId, timeStamp);
                    if (corpActInfo.RequestType == RBbgCorpActRequestType.FTP)
                        ftpHandler.GetCorpAction(corpActInfo, timeStamp, ref dictHeaders);
                    else if (corpActInfo.RequestType == RBbgCorpActRequestType.Heavy)
                        heavyHandler.GetCorpAction(corpActInfo, timeStamp, ref dictHeaders);
                }
                if (corpActInfo.ImmediateResponse)
                {
                    if (corpActInfo.HasError)
                    {
                        RCorpActUtils.UpdateVendorHistory(string.Empty, corpActInfo.RequestIdentifier, RVendorStatusConstant.FAILED);
                        RVendorUtils.UpdateBBGAuditCorpActionStatus(timeStamp);
                    }
                    else
                    {
                        StringBuilder processedInstruments = new StringBuilder();
                        if (corpActInfo.CorpActResultantXml.Elements().Count() > 0)
                        {
                            foreach (KeyValuePair<string, List<string>> key in corpActInfo.ProcessedInstruments)
                            {
                                processedInstruments.Append(key.Key + "=>");
                                key.Value.ForEach(q => processedInstruments.Append(q + RCorpActionConstant.COMMA));
                                processedInstruments.Append(";");
                            }
                        }
                        RCorpActUtils.UpdateVendorHistory(processedInstruments.ToString(), corpActInfo.RequestIdentifier, RVendorStatusConstant.PASSED);
                        RBbgUtils.UpdateBBGAuditCorpAction(corpActInfo.RequestIdentifier, corpActInfo.ProcessedInstruments);
                    }
                }
                else
                {
                    RCorpActUtils.UpdateVendorHistory(string.Empty, timeStamp, RVendorStatusConstant.PENDING);
                    RVendorUtils.UpdateBBGAuditCorpActionStatus(timeStamp);
                }
                mLogger.Debug("RCorpActController:GetCorpAction=> End getting corp action");
            }
            catch (Exception ex)
            {
                RCorpActUtils.UpdateVendorHistory(string.Empty, timeStamp, RVendorStatusConstant.FAILED);
                RVendorUtils.UpdateBBGAuditCorpActionStatus(timeStamp);
                mLogger.Error("RCorpActController:GetCorpAction=>" + ex.Message);
                throw ex;
            }
        }

        public void GetCorpActionResponse(RCorpActInfo corpActInfo)
        {
            try
            {
                mLogger.Debug("RCorpActController:GetCorpActionResponse=> start getting corp action response");
                string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.GETACTIONSTRANSPORTNAME);

                if (string.IsNullOrEmpty(corpActInfo.TransportName) && !string.IsNullOrEmpty(transportName))
                {
                    corpActInfo.TransportName = transportName;
                }

                RCorpActFtpHandler ftpHandler = new RCorpActFtpHandler();
                ftpHandler.GetResponse(corpActInfo);
                if (corpActInfo.HasError)
                {
                    RCorpActUtils.UpdateVendorHistory(string.Empty, corpActInfo.RequestIdentifier, RVendorStatusConstant.FAILED);
                    RVendorUtils.UpdateBBGAuditCorpActionStatus(corpActInfo.RequestIdentifier);
                }
                else
                {
                    StringBuilder processedInstruments = new StringBuilder();
                    if (corpActInfo.CorpActResultantXml.Elements().Count() > 0)
                    {
                        foreach (KeyValuePair<string, List<string>> key in corpActInfo.ProcessedInstruments)
                        {
                            processedInstruments.Append(key.Key + "=>");
                            key.Value.ForEach(q => processedInstruments.Append(q + RCorpActionConstant.COMMA));
                            processedInstruments.Append(";");
                        }
                    }
                    RCorpActUtils.UpdateVendorHistory(processedInstruments.ToString(), corpActInfo.RequestIdentifier, RVendorStatusConstant.PASSED);
                    RBbgUtils.UpdateBBGAuditCorpAction(corpActInfo.RequestIdentifier, corpActInfo.ProcessedInstruments);
                }
                mLogger.Debug("RCorpActController:GetCorpActionResponse=> End getting corp action response");
            }
            catch (Exception ex)
            {
                RCorpActUtils.UpdateVendorHistory(string.Empty, corpActInfo.RequestIdentifier, RVendorStatusConstant.FAILED);
                RVendorUtils.UpdateBBGAuditCorpActionStatus(corpActInfo.RequestIdentifier);
                mLogger.Error("RCorpActController:GetCorpAction=>" + ex.Message);
                throw ex;
            }
        }
    }
}
