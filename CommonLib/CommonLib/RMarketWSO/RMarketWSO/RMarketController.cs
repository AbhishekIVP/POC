using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.RMarketWSO.GAMService;
using System.Configuration;
using com.ivp.rad.RMarketWSO.FacilitySearchService;
using System.Threading.Tasks;
using System.Data;
using com.ivp.rad.RMarketWSO.TransactionService;
using System.Collections;
using com.ivp.rad.configurationmanagement;
using System.IO;
using com.ivp.rad.RMarketWSO.Info.Transactions;
using com.ivp.srmcommon;
using Newtonsoft.Json;

namespace com.ivp.rad.RMarketWSO
{
    internal class RMarketController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMarketController");

        List<string> allowedMarkitVersions = new List<string> { "2012", "2013", "2015", "Rest" };

        Dictionary<string, int> DictIdentifierVsFacilityId { get; set; }
        Dictionary<int, string> DictFacilityIdVsLoanX { get; set; }
        Dictionary<int, KeyValuePair<string, string>> DictFacilityIdVsIdentifierDetails { get; set; }

        internal int VendorPreferenceId { get; set; }
        internal Dictionary<int, bool> SubscribeFacility(List<int> facilityId, string user)
        {
            mLogger.Debug("MarkitWSO -> Entering SubscribeFacility method");
            try
            {
                Dictionary<int, bool> result = new Dictionary<int, bool>();

                var subscribedFacilityIds = GetAllSubscribedFacilities();
                if (subscribedFacilityIds == null)
                    subscribedFacilityIds = new List<int>();

                var preSubscribedFacilities = facilityId.Intersect(subscribedFacilityIds).ToList();
                var notSubscribedFacilities = facilityId.Except(preSubscribedFacilities).ToList();

                string markitVersion = GetMarkitVersion();
                if (notSubscribedFacilities.Count > 0)
                {
                    if (!markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                    {
                        GamServiceClient GAMClient = RMarketUtils.InitializeGAMService(VendorPreferenceId);
                        if (GAMClient != null)
                        {
                            List<GAMService.SecurityId> listToBeSubscribedSecIDs = new List<GAMService.SecurityId>();
                            foreach (int facility in notSubscribedFacilities)
                            {
                                result.Add(facility, false);
                                List<SecurityTypeValuePair> ListSecValuePairs = new List<SecurityTypeValuePair>() { new SecurityTypeValuePair() { SecurityIdType = "WSODataFacilityId", SecurityIdValue = facility.ToString() } };
                                listToBeSubscribedSecIDs.Add(new GAMService.SecurityId() { SecurityTypeValuePairs = ListSecValuePairs.ToArray() });
                            }
                            SubscribedFacility[] subscribedFacilities;
                            GAMClient.FacilitySubscribe(listToBeSubscribedSecIDs.ToArray(), out subscribedFacilities);
                            for (int count = 0; count < subscribedFacilities.Length; count++)
                            {
                                result[(int)subscribedFacilities[count].WsoDataFacilityId] = true;
                            }
                        }
                    }
                    else
                    {
                        string username = string.Empty;
                        string password = string.Empty;
                        DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                        if (clientDetails != null && clientDetails.Rows.Count > 0)
                        {
                            username = clientDetails.Rows[0]["username"].ToString();
                            password = clientDetails.Rows[0]["password"].ToString();

                            string markitRestGAMURL = GetMarkitRestURL("MarkitRestGAMURL");

                            foreach (int facility in notSubscribedFacilities)
                            {
                                try
                                {
                                    SRMCommon.RestAPICaller(markitRestGAMURL + "/Subscribe/" + facility, SRMRestMethod.POST, null, username, password);

                                    result[facility] = true;
                                }
                                catch (Exception exx)
                                {
                                    mLogger.Error("MarkitWSO -> Error in Subscribing Facility ! Error is : " + exx.ToString());

                                    result[facility] = false;
                                }
                            }
                        }
                        else
                        {
                            mLogger.Debug("no connectivity details found");

                            foreach (int facility in notSubscribedFacilities)
                            {
                                result.Add(facility, false);
                            }
                        }
                    }

                    RMarketUtils.UpdateSubscriptionStatus(string.Join(",", result.Where(x => x.Value).Select(x => x.Key)), true, user);
                }

                if (preSubscribedFacilities.Count > 0)
                {
                    foreach (int facility in preSubscribedFacilities)
                    {
                        result.Add(facility, true);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                mLogger.Error("MarkitWSO -> Error in SubscribeFacility method ! Error is : " + e.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("MarkitWSO -> Leaving SubscribeFacility method ");
            }
        }

        private static string GetMarkitRestURL(string MarkitRestURLType)
        {
            string MarkitBaseURLKey = "MarkitRestBaseURL";

            string markitRestBaseURL = string.Empty;
            if (ConfigurationManager.AppSettings[MarkitBaseURLKey] != null)
                markitRestBaseURL = Convert.ToString(ConfigurationManager.AppSettings[MarkitBaseURLKey]);

            string markitRestURL = string.Empty;
            if (ConfigurationManager.AppSettings[MarkitRestURLType] != null)
                markitRestURL = Convert.ToString(ConfigurationManager.AppSettings[MarkitRestURLType]);

            if (string.IsNullOrEmpty(markitRestURL))
                throw new Exception("Markit Rest url not provided in app setting - " + MarkitRestURLType);
            if (string.IsNullOrEmpty(markitRestBaseURL))
                throw new Exception("Markit Rest url not provided in app setting - " + MarkitBaseURLKey);
            return markitRestBaseURL + "/" + markitRestURL;
        }

        internal Dictionary<int, bool> UnSubscribeFacility(List<int> facilityId, string user)
        {
            mLogger.Debug("MarkitWSO -> Entering UnSubscribeFacility method");
            try
            {
                Dictionary<int, bool> result = new Dictionary<int, bool>();

                var subscribedFacilityIds = GetAllSubscribedFacilities();
                if (subscribedFacilityIds == null)
                    subscribedFacilityIds = new List<int>();

                var preSubscribedFacilities = facilityId.Intersect(subscribedFacilityIds).ToList();
                var notSubscribedFacilities = facilityId.Except(preSubscribedFacilities).ToList();

                string markitVersion = GetMarkitVersion();
                if (preSubscribedFacilities.Count > 0)
                {
                    if (!markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                    {
                        GamServiceClient GAMClient = RMarketUtils.InitializeGAMService(VendorPreferenceId);
                        if (GAMClient != null)
                        {
                            List<GAMService.SecurityId> listToBeUnSubscribedSecIDs = new List<GAMService.SecurityId>();
                            foreach (int facility in preSubscribedFacilities)
                            {
                                result.Add(facility, false);
                                List<SecurityTypeValuePair> ListSecValuePairs = new List<SecurityTypeValuePair>() { new SecurityTypeValuePair() { SecurityIdType = "WSODataFacilityId", SecurityIdValue = facility.ToString() } };
                                listToBeUnSubscribedSecIDs.Add(new GAMService.SecurityId() { SecurityTypeValuePairs = ListSecValuePairs.ToArray() });
                            }
                            UnsubscribedFacility[] unSubscribedFacilities;
                            GAMClient.FacilityUnsubscribe(listToBeUnSubscribedSecIDs.ToArray(), out unSubscribedFacilities);
                            for (int count = 0; count < unSubscribedFacilities.Length; count++)
                            {
                                result[(int)unSubscribedFacilities[count].WsoDataFacilityId] = true;
                            }
                        }
                    }
                    else
                    {
                        string username = string.Empty;
                        string password = string.Empty;
                        DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                        if (clientDetails != null && clientDetails.Rows.Count > 0)
                        {
                            username = clientDetails.Rows[0]["username"].ToString();
                            password = clientDetails.Rows[0]["password"].ToString();

                            string markitRestGAMURL = GetMarkitRestURL("MarkitRestGAMURL");

                            foreach (int facility in preSubscribedFacilities)
                            {
                                try
                                {
                                    SRMCommon.RestAPICaller(markitRestGAMURL + "/Subscribe/" + facility, SRMRestMethod.DELETE, null, username, password);

                                    result[facility] = true;
                                }
                                catch (Exception exx)
                                {
                                    mLogger.Error("MarkitWSO -> Error in UnSubscribing Facility ! Error is : " + exx.ToString());

                                    result[facility] = false;
                                }
                            }
                        }
                        else
                        {
                            mLogger.Debug("no connectivity details found");

                            foreach (int facility in preSubscribedFacilities)
                            {
                                result.Add(facility, false);
                            }
                        }
                    }

                    RMarketUtils.UpdateSubscriptionStatus(string.Join(",", result.Where(x => x.Value).Select(x => x.Key)), false, user);
                }

                if (notSubscribedFacilities.Count > 0)
                {
                    foreach (int facility in notSubscribedFacilities)
                    {
                        result.Add(facility, true);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                mLogger.Error("MarkitWSO -> Error in UnSubscribeFacility method ! Error is : " + e.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("MarkitWSO -> Leaving UnSubscribeFacility method ");
            }
        }

        internal RMarketResponseInfo FetchFacilityData(RMarketRequestInfo requestInfo)
        {
            try
            {
                mLogger.Debug("MarkitWSO -> start FetchFacilityData method ");
                RMarketResponseInfo response = new RMarketResponseInfo();
                int WSOSingleRequestLimit = Convert.ToInt32(ConfigurationManager.AppSettings["WSOSingleRequestLimit"]);
                Dictionary<string, RMarkitFacilityResponse> FacilityDetails = new Dictionary<string, RMarkitFacilityResponse>();
                List<AmortizationScheduleInfo> AmortizationDetails = new List<AmortizationScheduleInfo>();
                List<ContractInfo> ContractDetails = new List<ContractInfo>();
                Dictionary<int, TransactionResults> TransactionDetails = new Dictionary<int, TransactionResults>();
                //RMarketUtils.UpdateDatabase
                AddDatabaseEntry(requestInfo);

                bool allPrimariesFacilityId = true;
                if (requestInfo != null && requestInfo.SecurityDetails != null && requestInfo.SecurityDetails.Any(x => !x.Value.Equals(RMarketIdentifierType.WSODataFacilityID)))
                    allPrimariesFacilityId = false;

                string markitVersion = GetMarkitVersion();
                if ((requestInfo.RequestType.Where(x => x != RMarketRequestType.Amortization && x != RMarketRequestType.AmortizationSchedule && x != RMarketRequestType.Transaction).Count() > 0) || (!allPrimariesFacilityId && !markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase)))
                    INIT(requestInfo, FacilityDetails);

                foreach (RMarketRequestType reqType in requestInfo.RequestType)
                {
                    switch (reqType)
                    {
                        case RMarketRequestType.AgentBank:
                            FillAgentBankInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.BankDeal:
                            FillBankDealInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.BankDealSponser:
                            FillBankDealSponserInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Facility:
                            FillFacilityInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.SecurityID:
                            FillSecurityInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Spread:
                            FillSpreadInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.SIC:
                            FillSICInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Issuer:
                            FillIssuerInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.RateOption:
                            FillRateOptionInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.RateLimit:
                            FillRateLimitInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Rating:
                            FillRatingInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.CallSchedulePay:
                            FillCallSchedulePayInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.MultiCurrency:
                            FillMultiCurrencyInfo(FacilityDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Contract:
                            if (ContractDetails == null || ContractDetails.Count == 0)
                                FillContractData(requestInfo, ContractDetails);
                            FillContractDataInfo(ContractDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Amortization:
                            if (AmortizationDetails == null || AmortizationDetails.Count == 0)
                                FillAmortizationData(requestInfo, AmortizationDetails);
                            FillAmortizationDataInfo(AmortizationDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.AmortizationSchedule:
                            if (AmortizationDetails == null || AmortizationDetails.Count == 0)
                                FillAmortizationData(requestInfo, AmortizationDetails);
                            FillAmortizationScheduleDataInfo(AmortizationDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Transaction:
                            if (TransactionDetails == null || TransactionDetails.Count == 0)
                                FillTransactionData(requestInfo, TransactionDetails);
                            FillTransactionData(TransactionDetails, response.FacilityData);
                            break;
                        case RMarketRequestType.Sponsor:
                            FillSponsorInfo(FacilityDetails, response.FacilityData);
                            break;
                    }
                }
                if (FacilityDetails.Count == 0 && ContractDetails.Count == 0 && AmortizationDetails.Count == 0 && TransactionDetails.Count == 0)
                    response.Status = RStatus.FAILED;
                else
                    response.Status = RStatus.PASSED;
                UpdateDatabaseEntry(requestInfo.requestIdentifier, response.Status, "");
                if (requestInfo.IsImmediate == false)
                {
                    string workingDirectory = RADConfigReader.GetServerPhysicalPath() + "VendorWorkingDirectory";
                    string path = Path.Combine(workingDirectory, requestInfo.CachedFileName);
                    if (!Directory.Exists(workingDirectory))
                    {
                        Directory.CreateDirectory(workingDirectory);
                    }
                    response.FacilityData.WriteXml(path, XmlWriteMode.WriteSchema);
                }
                mLogger.Debug("MarkitWSO -> end FetchFacilityData method ");
                return response;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RMarketResponseInfo response = new RMarketResponseInfo();
                response.FacilityData = null;
                response.Status = RStatus.FAILED;
                UpdateDatabaseEntry(requestInfo.requestIdentifier, response.Status, ex.ToString());
                return response;
            }
        }

        private void FillTransactionData(Dictionary<int, TransactionResults> TransactionDetails, DataSet resultantData)
        {
            foreach (var transactionCol in TransactionDetails)
            {
                FillTransactionData(resultantData, transactionCol);
            }
        }

        private void FillTransactionData(DataSet resultantData, KeyValuePair<int, TransactionResults> transactionCol)
        {
            int facilityId = transactionCol.Key;

            Type type = null;
            DataTable ContractExisting = new DataTable("ContractExisting");
            ContractExisting.Columns.Add("globtranid");
            type = (new ContractExisting()).GetType();
            RMarketUtils.CreateTransactionTableSchema(type, ContractExisting);
            resultantData.Tables.Add(ContractExisting);

            DataTable ContractNew = new DataTable("ContractNew");
            ContractNew.Columns.Add("globtranid");
            type = (new ContractNew()).GetType();
            RMarketUtils.CreateTransactionTableSchema(type, ContractNew);
            resultantData.Tables.Add(ContractNew);

            DataTable FacilityExisting = new DataTable("FacilityExisting");
            FacilityExisting.Columns.Add("globtranid");
            type = (new com.ivp.rad.RMarketWSO.Info.Transactions.Facility()).GetType();
            RMarketUtils.CreateTransactionTableSchema(type, FacilityExisting);
            resultantData.Tables.Add(FacilityExisting);

            DataTable FacilityNew = new DataTable("FacilityNew");
            FacilityNew.Columns.Add("globtranid");
            type = (new com.ivp.rad.RMarketWSO.Info.Transactions.Facility()).GetType();
            RMarketUtils.CreateTransactionTableSchema(type, FacilityNew);
            resultantData.Tables.Add(FacilityNew);

            DataTable Facility = new DataTable("Facility");
            Facility.Columns.Add("globtranid");
            type = (new com.ivp.rad.RMarketWSO.Info.Transactions.Facility()).GetType();
            RMarketUtils.CreateTransactionTableSchema(type, Facility);
            resultantData.Tables.Add(Facility);


            foreach (var transaction in transactionCol.Value.Transactions)
            {
                if (!resultantData.Tables.Contains(transaction.Type))
                {
                    DataTable transactionTable = new DataTable(transaction.Type);
                    DataColumn dc = new DataColumn("Identifier");
                    dc.DefaultValue = RMarketIdentifierType.WSODataFacilityID.ToString();
                    transactionTable.Columns.Add(dc);
                    transactionTable.Columns.Add("Identifier Value");

                    switch (transaction.Type)
                    {
                        case "Rollover":
                            type = (new Rollover()).GetType();
                            break;
                        case "CommitmentIncrease":
                            type = (new CommitmentIncrease()).GetType();
                            break;
                        case "CommitmentReduction":
                            type = (new CommitmentReduction()).GetType();
                            break;
                        case "Paydown Full, Paydown Early":
                            type = (new PaydownFullPaydownEarly()).GetType();
                            break;
                        case "PaydownPartial":
                            type = (new PaydownPartial()).GetType();
                            break;
                        case "Combine":
                            type = (new Combine()).GetType();
                            break;
                        case "ConvertContract":
                            type = (new ConvertContract()).GetType();
                            break;
                        case "ReceiveInterimInterest":
                            type = (new ReceiveInterimInterest()).GetType();
                            break;
                        case "ReceiveOldInterest":
                            type = (new ReceiveOldInterest()).GetType();
                            break;
                        case "Rollback":
                            type = (new Rollback()).GetType();
                            break;
                        case "SplitContract":
                            type = (new SplitContract()).GetType();
                            break;
                        case "FacilityIncrease":
                            type = (new FacilityIncrease()).GetType();
                            break;
                        case "FXAdjustment":
                            type = (new FXAdjustment()).GetType();
                            break;
                        case "Borrow":
                            type = (new Borrow()).GetType();
                            break;
                        case "BorrowEarly":
                            type = (new BorrowEarly()).GetType();
                            break;
                        case "BorrowNew":
                            type = (new BorrowNew()).GetType();
                            break;
                        case "ProcessReceivable":
                            type = (new ProcessReceivable()).GetType();
                            break;
                        case "AccrualAdjustment":
                            type = (new AccrualAdjustment()).GetType();
                            break;
                        case "MiscellaneousFees":
                            type = (new MiscellaneousFees()).GetType();
                            break;
                        case "Restructuring":
                            type = (new Restructuring()).GetType();
                            break;
                        case "IssuerUpdate":
                            type = (new IssuerUpdate()).GetType();
                            break;
                        case "BankDealUpdate":
                            type = (new BankDealUpdate()).GetType();
                            break;
                        case "Buy":
                            type = (new Buy()).GetType();
                            break;
                        case "Close Contract":
                            type = (new CloseContract()).GetType();
                            break;
                        case "Update Contract":
                            type = (new UpdateContract()).GetType();
                            break;
                        case "Rate Change":
                            type = (new RateChange()).GetType();
                            break;

                    }
                    RMarketUtils.CreateTransactionTableSchema(type, transactionTable);
                    resultantData.Tables.Add(transactionTable);
                }
                FillTransactionData(resultantData, transaction, facilityId);
            }
        }

        private void FillTransactionData(DataSet resultantData, Transaction transaction, int facilityId)
        {
            switch (transaction.Type)
            {
                case "Rollover":
                    RTransactionUtils.FillRollover(resultantData, transaction, facilityId);
                    break;
                case "CommitmentIncrease":
                    RTransactionUtils.FillCommitmentIncrease(resultantData, transaction, facilityId);
                    break;
                case "CommitmentReduction":
                    RTransactionUtils.FillCommitmentReduction(resultantData, transaction, facilityId);
                    break;
                case "Paydown Full, Paydown Early":
                    RTransactionUtils.FillPaydownFullPaydownEarly(resultantData, transaction, facilityId);
                    break;
                case "PaydownPartial":
                    RTransactionUtils.FillPaydownPartial(resultantData, transaction, facilityId);
                    break;
                case "Combine":
                    RTransactionUtils.FillCombine(resultantData, transaction, facilityId);
                    break;
                case "ConvertContract":
                    RTransactionUtils.FillConvertContract(resultantData, transaction, facilityId);
                    break;
                case "ReceiveInterimInterest":
                    RTransactionUtils.FillReceiveInterimInterest(resultantData, transaction, facilityId);
                    break;
                case "ReceiveOldInterest":
                    RTransactionUtils.FillReceiveOldInterest(resultantData, transaction, facilityId);
                    break;
                case "Rollback":
                    RTransactionUtils.FillRollback(resultantData, transaction, facilityId);
                    break;
                case "SplitContract":
                    RTransactionUtils.FillSplitContract(resultantData, transaction, facilityId);
                    break;
                case "FacilityIncrease":
                    RTransactionUtils.FillFacilityIncrease(resultantData, transaction, facilityId);
                    break;
                case "FXAdjustment":
                    RTransactionUtils.FillFXAdjustment(resultantData, transaction, facilityId);
                    break;
                case "Borrow":
                    RTransactionUtils.FillBorrow(resultantData, transaction, facilityId);
                    break;
                case "BorrowEarly":
                    RTransactionUtils.FillBorrowEarly(resultantData, transaction, facilityId);
                    break;
                case "BorrowNew":
                    RTransactionUtils.FillBorrowNew(resultantData, transaction, facilityId);
                    break;
                case "ProcessReceivable":
                    RTransactionUtils.FillProcessReceivable(resultantData, transaction, facilityId);
                    break;
                case "AccrualAdjustment":
                    RTransactionUtils.FillAccrualAdjustment(resultantData, transaction, facilityId);
                    break;
                case "MiscellaneousFees":
                    RTransactionUtils.FillMiscellaneousFees(resultantData, transaction, facilityId);
                    break;
                case "Restructuring":
                    RTransactionUtils.FillRestructuring(resultantData, transaction, facilityId);
                    break;
                case "IssuerUpdate":
                    RTransactionUtils.FillIssuerUpdate(resultantData, transaction, facilityId);
                    break;
                case "BankDealUpdate":
                    RTransactionUtils.FillBankDealUpdate(resultantData, transaction, facilityId);
                    break;
                case "Buy":
                    RTransactionUtils.FillBuy(resultantData, transaction, facilityId);
                    break;
                case "Rate Change":
                    RTransactionUtils.FillRateChange(resultantData, transaction, facilityId);
                    break;
                case "Update Contract":
                    RTransactionUtils.FillUpdateContract(resultantData, transaction, facilityId);
                    break;
                case "Close Contract":
                    RTransactionUtils.FillCloseContract(resultantData, transaction, facilityId);
                    break;
            }
        }

        private void UpdateDatabaseEntry(string identifier, RStatus rStatus, string description)
        {
            try
            {
                mLogger.Debug("being UpdateDatabaseEntry");
                RMarketUtils.UpdateDatabase(identifier, rStatus.ToString(), description);
                mLogger.Debug("end UpdateDatabaseEntry");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        private void AddDatabaseEntry(RMarketRequestInfo requestInfo)
        {
            try
            {
                mLogger.Debug("being AddDatabaseEntry");
                Guid guid = Guid.NewGuid();
                requestInfo.requestIdentifier = guid.ToString();
                string requestTypes = string.Join(",", requestInfo.RequestType);
                var identifierGroup = requestInfo.SecurityDetails.GroupBy(q => q.Value);
                StringBuilder requestedData = new StringBuilder();
                foreach (var grp in identifierGroup)
                {
                    requestedData.Append(grp.Key.ToString())
                        .Append(" => ");
                    grp.ToList().ForEach(q => requestedData.Append(q.Key).Append(","));
                }
                RMarketUtils.UpdateDatabase(requestedData.ToString(), requestTypes, requestInfo.User, requestInfo.requestIdentifier);
                mLogger.Debug("end AddDatabaseEntry");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        private void FillAmortizationScheduleDataInfo(List<AmortizationScheduleInfo> AmortizationDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillAmortizationScheduleDataInfo");
            DataTable AmortizationSchedule = new DataTable("AmortizationSchedule");
            AmortizationSchedule.Columns.Add("Identifier");
            AmortizationSchedule.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(AmortizationScheduleInfo), AmortizationSchedule);
            if (AmortizationDetails != null)
            {
                foreach (var amor in AmortizationDetails)
                {
                    if (amor != null)
                        RMarketUtils.GetAmortizationScheduleInfo(amor, AmortizationSchedule, DictFacilityIdVsIdentifierDetails);
                }
            }
            dataSet.Tables.Add(AmortizationSchedule);
            mLogger.Debug("end FillAmortizationScheduleDataInfo");
        }

        private void FillAmortizationDataInfo(List<AmortizationScheduleInfo> AmortizationDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillAmortizationDataInfo");
            DataTable Amortization = new DataTable("Amortization");
            Amortization.Columns.Add("Identifier");
            Amortization.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(AmortizationInfo), Amortization);
            if (AmortizationDetails != null)
            {
                foreach (var amor in AmortizationDetails)
                {
                    if (amor != null)
                        RMarketUtils.GetAmortizationDetailInfo(amor, Amortization, DictFacilityIdVsIdentifierDetails);
                }
            }
            dataSet.Tables.Add(Amortization);
            mLogger.Debug("end FillAmortizationDataInfo");
        }

        private void FillContractDataInfo(List<ContractInfo> ContractDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillContractDataInfo");
            DataTable Contract = new DataTable("Contract");
            Contract.Columns.Add("Identifier");
            Contract.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(ContractInfo), Contract);
            if (ContractDetails != null)
            {
                foreach (var contract in ContractDetails)
                {
                    if (contract != null)
                        RMarketUtils.GetContractInfo(contract, Contract, DictFacilityIdVsIdentifierDetails);
                }
            }
            dataSet.Tables.Add(Contract);
            mLogger.Debug("end FillContractDataInfo");
        }

        private void FillMultiCurrencyInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillMultiCurrencyInfo");
            DataTable MultiCurrency = new DataTable("MultiCurrency");
            MultiCurrency.Columns.Add("Identifier");
            MultiCurrency.Columns.Add("Identifier Value");
            //MultiCurrency.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(MultiCurrencyInfo), MultiCurrency);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.currencyTypes != null)
                        RMarketUtils.GetMultiCurrencyInfo(facilityInfo.Value.bankDeal.facility.currencyTypes, facilityInfo.Key, MultiCurrency);
                }
            }
            dataSet.Tables.Add(MultiCurrency);
            mLogger.Debug("start FillMultiCurrencyInfo");
        }

        private void FillCallSchedulePayInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillCallSchedulePayInfo");
            DataTable CallSchedulePay = new DataTable("CallSchedulePay");
            CallSchedulePay.Columns.Add("Identifier");
            CallSchedulePay.Columns.Add("Identifier Value");
            //CallSchedulePay.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(CallSchedulePayInfo), CallSchedulePay);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.callSchedule != null)
                        RMarketUtils.GetCallSchedulePayInfo(facilityInfo.Value.bankDeal.facility.callSchedule, facilityInfo.Key, CallSchedulePay);
                }
            }
            dataSet.Tables.Add(CallSchedulePay);
            mLogger.Debug("start FillCallSchedulePayInfo");
        }

        private void FillRatingInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillRatingInfo");
            DataTable Rating = new DataTable("Rating");
            Rating.Columns.Add("Identifier");
            Rating.Columns.Add("Identifier Value");
            //Rating.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(RatingInfo), Rating);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.Ratings != null)
                        RMarketUtils.GetRatingInfo(facilityInfo.Value.bankDeal.facility.Ratings, facilityInfo.Key, Rating);
                }
            }
            dataSet.Tables.Add(Rating);
            mLogger.Debug("start FillRatingInfo");
        }

        private void FillSponsorInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillSponsorInfo");
            DataTable sponsor = new DataTable("Sponsor");
            sponsor.Columns.Add("Identifier");
            sponsor.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(SponsorInfo), sponsor);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.sponsor != null)
                        RMarketUtils.GetSponsorInfo(facilityInfo.Value.bankDeal.sponsor, facilityInfo.Key, sponsor);
                }
            }
            dataSet.Tables.Add(sponsor);
            mLogger.Debug("start FillSponsorInfo");
        }

        private void FillRateLimitInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillRateLimitInfo");
            DataTable RateLimit = new DataTable("RateLimit");
            RateLimit.Columns.Add("Identifier");
            RateLimit.Columns.Add("Identifier Value");
            //RateLimit.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(RateLimitInfo), RateLimit);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.rateLimits != null)
                        RMarketUtils.GetRateLimitInfo(facilityInfo.Value.bankDeal.facility.rateLimits, facilityInfo.Key, RateLimit);
                }
            }
            dataSet.Tables.Add(RateLimit);
            mLogger.Debug("start FillRateLimitInfo");
        }

        private void FillRateOptionInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillRateOptionInfo");
            DataTable RateOption = new DataTable("RateOption");
            RateOption.Columns.Add("Identifier");
            RateOption.Columns.Add("Identifier Value");
            //RateOption.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(RateOptionInfo), RateOption);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.rateOptions != null)
                        RMarketUtils.GetRateOptionInfo(facilityInfo.Value.bankDeal.facility.rateOptions, facilityInfo.Key, RateOption);
                }
            }
            dataSet.Tables.Add(RateOption);
            mLogger.Debug("start FillRateOptionInfo");
        }

        private void FillIssuerInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillIssuerInfo");
            DataTable Issuer = new DataTable("Issuer");
            Issuer.Columns.Add("Identifier");
            Issuer.Columns.Add("Identifier Value");
            //Issuer.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(IssuerInfo), Issuer);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null)
                        RMarketUtils.GetIssuerInfo(facilityInfo.Value, facilityInfo.Key, Issuer);
                }
            }
            dataSet.Tables.Add(Issuer);
            mLogger.Debug("start FillIssuerInfo");
        }

        private void FillSICInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillSICInfo");
            DataTable SIC = new DataTable("SIC");
            SIC.Columns.Add("Identifier");
            SIC.Columns.Add("Identifier Value");
            //SIC.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(SICLevelInfo), SIC);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.SICs != null)
                        RMarketUtils.GetSICInfo(facilityInfo.Value.bankDeal.facility.SICs, facilityInfo.Key, SIC);
                }
            }
            dataSet.Tables.Add(SIC);
            mLogger.Debug("start FillSICInfo");
        }

        private void FillSpreadInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillSpreadInfo");
            DataTable Spread = new DataTable("Spread");
            Spread.Columns.Add("Identifier");
            Spread.Columns.Add("Identifier Value");
            //Spread.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(SpreadFieldInfo), Spread);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.spreads != null)
                        RMarketUtils.GetSpreadInfo(facilityInfo.Value.bankDeal.facility.spreads, facilityInfo.Key, Spread);
                }
            }
            dataSet.Tables.Add(Spread);
            mLogger.Debug("start FillSpreadInfo");
        }

        private void FillSecurityInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillSecurityInfo");
            DataTable Security = new DataTable("SecurityID");
            Security.Columns.Add("Identifier");
            Security.Columns.Add("Identifier Value");
            //Security.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(SecurityIDFieldInfo), Security);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null && facilityInfo.Value.bankDeal.facility.identifiers != null)
                        RMarketUtils.GetSecurityInfo(facilityInfo.Value.bankDeal.facility.identifiers, facilityInfo.Key, Security);
                }
            }
            dataSet.Tables.Add(Security);
            mLogger.Debug("start FillSecurityInfo");
        }

        private void FillFacilityInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillFacilityInfo");
            DataTable Facility = new DataTable("Facility");
            Facility.Columns.Add("Identifier");
            Facility.Columns.Add("Identifier Value");
            // Facility.Columns.Add(FacilityDetails.Keys.ElementAt(0).Split(new string[]{"~~"},StringSplitOptions.RemoveEmptyEntries)[0]);
            RMarketUtils.CreateTableSchema(typeof(FacilityInfo), Facility);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.facility != null)
                        RMarketUtils.GetFacilityInfo(facilityInfo.Value.bankDeal.facility, facilityInfo.Key, Facility);
                }
            }
            dataSet.Tables.Add(Facility);
            mLogger.Debug("start FillFacilityInfo");
        }

        private void FillBankDealSponserInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillBankDealSponserInfo");
            DataTable BankDealSponser = new DataTable("BankDealSponser");
            BankDealSponser.Columns.Add("Identifier");
            BankDealSponser.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(BankDealSponserLevelInfo), BankDealSponser);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.sponsor != null)
                        RMarketUtils.GetBankDealSponserInfo(facilityInfo.Value.bankDeal.sponsor, facilityInfo.Key, BankDealSponser);
                }
            }
            dataSet.Tables.Add(BankDealSponser);
            mLogger.Debug("start FillBankDealSponserInfo");
        }

        private void FillBankDealInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillBankDealInfo");
            DataTable bankDeal = new DataTable("BankDeal");
            bankDeal.Columns.Add("Identifier");
            bankDeal.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(BankDealInfo), bankDeal);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null)
                        RMarketUtils.GetBankDealInfo(facilityInfo.Value.bankDeal, facilityInfo.Key, bankDeal);
                }
            }
            dataSet.Tables.Add(bankDeal);
            mLogger.Debug("end FillBankDealInfo");
        }

        private void FillAgentBankInfo(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, DataSet dataSet)
        {
            mLogger.Debug("start FillAgentBankInfo");
            DataTable agentData = new DataTable("AgentBank");
            agentData.Columns.Add("Identifier");
            agentData.Columns.Add("Identifier Value");
            RMarketUtils.CreateTableSchema(typeof(AgentBankInfo), agentData);
            if (FacilityDetails != null)
            {
                foreach (var facilityInfo in FacilityDetails)
                {
                    if (facilityInfo.Value != null && facilityInfo.Value.bankDeal != null && facilityInfo.Value.bankDeal.agentBank != null)
                        RMarketUtils.GetAgentBankInfo(facilityInfo.Value.bankDeal.agentBank, facilityInfo.Key, agentData);
                }
            }
            dataSet.Tables.Add(agentData);
            mLogger.Debug("end FillAgentBankInfo");
        }

        private void FillTransactionData(RMarketRequestInfo requestInfo, Dictionary<int, TransactionResults> tranResults)
        {
            try
            {
                mLogger.Debug("begin FillTransactionData");
                double NumberOfLoops;
                bool flagRoundNumber = false;
                int NumberLeft = 0;
                int requestPerLoop = 1000;
                if (ConfigurationManager.AppSettings["requestPerLoop"] != null)
                    requestPerLoop = Convert.ToInt32(ConfigurationManager.AppSettings["requestPerLoop"]);
                if (requestInfo.TranDetails.Count % requestPerLoop == 0)
                {
                    NumberOfLoops = requestInfo.SecurityDetails.Count / requestPerLoop;
                    flagRoundNumber = true;
                }
                else
                {
                    NumberOfLoops = Math.Ceiling((double)requestInfo.TranDetails.Count / requestPerLoop);
                    NumberLeft = requestInfo.TranDetails.Count % requestPerLoop;
                }
                string username = string.Empty;
                string password = string.Empty;
                DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    username = clientDetails.Rows[0]["username"].ToString();
                    password = clientDetails.Rows[0]["password"].ToString();
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    return;
                }
                for (int count = 0; count < NumberOfLoops; count++)
                {

                    Dictionary<int, TransactionResults> tranRes = new Dictionary<int, TransactionResults>();
                    if (count == NumberOfLoops - 1 && !flagRoundNumber)
                        tranRes = GetTransactionDetails(requestInfo.TranDetails, count, NumberLeft, requestPerLoop, username, password);
                    else
                        tranRes = GetTransactionDetails(requestInfo.TranDetails, count, requestPerLoop, requestPerLoop, username, password);

                    foreach (var tran in tranRes)
                    {
                        tranResults.Add(tran.Key, tran.Value);
                    }
                }
                mLogger.Debug("end FillAmortizationData");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                tranResults.Clear();
                throw;
            }
        }

        private Dictionary<int, TransactionResults> GetTransactionDetails(List<TransactionDetails> dictionary, int count, int NumberLeft, int loopSize, string username, string password)
        {
            try
            {
                mLogger.Debug("begin GetTransactionDetails");
                Dictionary<int, TransactionResults> listTranInfo = new Dictionary<int, TransactionResults>();
                Task[] tasks = new Task[NumberLeft];

                for (int i = 0; i < NumberLeft; i++)
                {
                    int j = i;
                    tasks[j] = new Task(() => { GetTranDetails(dictionary[(count * loopSize) + j], listTranInfo, username, password); });
                    tasks[j].Start();
                }
                Task.WaitAll(tasks);
                mLogger.Debug("end GetTransactionDetails");
                return listTranInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        private void GetTranDetails(TransactionDetails transactionDetails, Dictionary<int, TransactionResults> listTranInfo, string username, string password)
        {
            TransactionServiceClient TransactionServiceClient = null;
            try
            {
                mLogger.Debug("begin GetTranDetails for value=> " + transactionDetails.facilityId);
                TransactionResults results = new TransactionResults();
                TransactionServiceClient = RMarketUtils.InitializeTransactionService(username, password);
                if (TransactionServiceClient != null)
                {
                    if (transactionDetails.TransactionDate != null)
                        TransactionServiceClient.SetDownloadStartDate((DateTime)transactionDetails.TransactionDate, transactionDetails.facilityId);
                    TransactionServiceClient.GetTransactionsByFacility(transactionDetails.facilityId, out results);
                }
                if (results != null)
                {
                    lock (((ICollection)listTranInfo).SyncRoot)
                        listTranInfo.Add(transactionDetails.facilityId, results);
                }
                else
                {
                    mLogger.Error("null value recieved from MARKIT for transaction = >  " + transactionDetails.facilityId);
                }
                mLogger.Debug("end GetTranDetails for value=> " + transactionDetails.facilityId);
            }
            catch (Exception ex)
            {
                mLogger.Error("value=> " + transactionDetails.facilityId);
                mLogger.Error(ex.ToString());
            }
            finally
            {
                if (TransactionServiceClient != null)
                    TransactionServiceClient.Close();
            }
        }

        private void FillAmortizationData(RMarketRequestInfo requestInfo, List<AmortizationScheduleInfo> amortizationData)
        {
            try
            {
                mLogger.Debug("begin FillAmortizationData");
                double NumberOfLoops;
                bool flagRoundNumber = false;
                int NumberLeft = 0;
                int requestPerLoop = 1000;
                if (ConfigurationManager.AppSettings["requestPerLoop"] != null)
                    requestPerLoop = Convert.ToInt32(ConfigurationManager.AppSettings["requestPerLoop"]);
                if (requestInfo.SecurityDetails.Count % requestPerLoop == 0)
                {
                    NumberOfLoops = requestInfo.SecurityDetails.Count / requestPerLoop;
                    flagRoundNumber = true;
                }
                else
                {
                    NumberOfLoops = Math.Ceiling((double)requestInfo.SecurityDetails.Count / requestPerLoop);
                    NumberLeft = requestInfo.SecurityDetails.Count % requestPerLoop;
                }
                string username = string.Empty;
                string password = string.Empty;
                DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    username = clientDetails.Rows[0]["username"].ToString();
                    password = clientDetails.Rows[0]["password"].ToString();
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    return;
                }
                for (int count = 0; count < NumberOfLoops; count++)
                {

                    List<AmortizationScheduleInfo> listAmottizeInfo = new List<AmortizationScheduleInfo>();

                    if (count == NumberOfLoops - 1 && !flagRoundNumber)
                        listAmottizeInfo = GetAmotizationDetails(requestInfo.SecurityDetails, count, NumberLeft, requestPerLoop, username, password);
                    else
                        listAmottizeInfo = GetAmotizationDetails(requestInfo.SecurityDetails, count, requestPerLoop, requestPerLoop, username, password);

                    foreach (var amortize in listAmottizeInfo)
                    {
                        AmortizationScheduleInfo cont = RMarketUtils.GetCloneAmortizeScheduleInfo(amortize);
                        amortizationData.Add(cont);
                    }
                }
                mLogger.Debug("end FillAmortizationData");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                amortizationData.Clear();
                throw;
            }
        }

        private List<AmortizationScheduleInfo> GetAmotizationDetails(Dictionary<string, RMarketIdentifierType> WSODataFacilityID, int count, int NumberLeft, int loopSize, string username, string password)
        {
            try
            {
                mLogger.Debug("begin GetAmotizationDetails");
                List<AmortizationScheduleInfo> listAmortizeInfo = new List<AmortizationScheduleInfo>();
                Task[] tasks = new Task[NumberLeft];

                for (int i = 0; i < NumberLeft; i++)
                {
                    int j = i;
                    string key = WSODataFacilityID.Keys.ElementAt((count * loopSize) + j);
                    tasks[j] = new Task(() => { GetAmorDetails(key, WSODataFacilityID[key], listAmortizeInfo, username, password); });
                    tasks[j].Start();
                }
                Task.WaitAll(tasks);
                mLogger.Debug("end GetAmotizationDetails");
                return listAmortizeInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

        }

        private void GetAmorDetails(string value, RMarketIdentifierType rMarketIdentifierType, List<AmortizationScheduleInfo> listAmortizeInfo, string username, string password)
        {
            FacilitySearchServiceClient FacilityServiceClient = null;
            FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient2013 = null;
            FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient2012 = null;

            string markitVersion = GetMarkitVersion();
            try
            {
                int facilityId = 0;
                if (rMarketIdentifierType == RMarketIdentifierType.WSODataFacilityID)
                    facilityId = Convert.ToInt32(value);
                else if (DictIdentifierVsFacilityId != null && DictIdentifierVsFacilityId.ContainsKey(rMarketIdentifierType.ToString() + "~~" + value))
                    facilityId = DictIdentifierVsFacilityId[rMarketIdentifierType.ToString() + "~~" + value];
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    string markitRestFacilityURL = GetMarkitRestURL("MarkitRestFacilityURL");

                    var facilityIdStr = FetchFacilityId(value, username, password, markitRestFacilityURL, rMarketIdentifierType);
                    if (!int.TryParse(facilityIdStr, out facilityId))
                    {
                        mLogger.Debug("Not able to identify facility id for identifier - " + value + ", identifier type - " + rMarketIdentifierType.ToString());
                        return;
                    }
                }
                else
                {
                    mLogger.Debug("Not able to identify facility id for identifier - " + value + ", identifier type - " + rMarketIdentifierType.ToString());
                    return;
                }

                mLogger.Debug("begin GetAmorDetails for value=> " + value + ", facility id - " + facilityId);
                bool isRest = false;
                AmortizationSchedule amortizationResults = new AmortizationSchedule();
                RMarkitAmortizationSchedule amortizationResultsRest = new RMarkitAmortizationSchedule();
                if (markitVersion == "2013")
                {
                    FacilitySearchService2013.AmortizationSchedule amortizationResults2013 = new FacilitySearchService2013.AmortizationSchedule();
                    FacilityServiceClient2013 = RMarketUtils.InitializeFacilityService2013(username, password);
                    if (FacilityServiceClient2013 != null)
                        FacilityServiceClient2013.GetAmortizationByFacilityId(facilityId, out amortizationResults2013);

                    amortizationResults = (AmortizationSchedule)RMarketUtils.CopyData(amortizationResults2013, typeof(AmortizationSchedule));
                }
                else if (markitVersion == "2012")
                {
                    FacilitySearchService2012.AmortizationSchedule amortizationResults2012 = new FacilitySearchService2012.AmortizationSchedule();
                    FacilityServiceClient2012 = RMarketUtils.InitializeFacilityService2012(username, password);
                    if (FacilityServiceClient2012 != null)
                        FacilityServiceClient2012.GetAmortizationByFacilityId(facilityId, out amortizationResults2012);

                    amortizationResults = (AmortizationSchedule)RMarketUtils.CopyData(amortizationResults2012, typeof(AmortizationSchedule));
                }
                else if (markitVersion == "2015")
                {
                    FacilityServiceClient = RMarketUtils.InitializeFacilityService(username, password);
                    if (FacilityServiceClient != null)
                        FacilityServiceClient.GetAmortizationByFacilityId(facilityId, out amortizationResults);
                }
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    isRest = true;

                    string markitRestAmortizationURL = GetMarkitRestURL("MarkitRestAmortizationURL");

                    var amortizationResultsStr = SRMCommon.RestAPICaller(markitRestAmortizationURL + "/" + facilityId, SRMRestMethod.GET, null, username, password);

                    if (!string.IsNullOrEmpty(amortizationResultsStr))
                        amortizationResultsRest = JsonConvert.DeserializeObject<RMarkitAmortizationSchedule>(amortizationResultsStr);
                }
                //else
                //    throw new Exception("cannot obtain service object");

                if (amortizationResults != null && !isRest)
                    amortizationResultsRest = RMarkitResponseParser.GetAmortizationResponseFromSOAP(amortizationResults);

                if (amortizationResultsRest != null)
                {
                    AmortizationScheduleInfo schedule = RMarketUtils.CloneAmortizationSchedule(amortizationResultsRest, facilityId);
                    lock (((ICollection)listAmortizeInfo).SyncRoot)
                        listAmortizeInfo.Add(schedule);
                }
                else
                {
                    mLogger.Error("null value recieved from MARKIT for amortize = >  " + value + ", facility id - " + facilityId);
                }
                mLogger.Debug("end GetAmorDetails for value=> " + value + ", facility id - " + facilityId);
            }
            catch (Exception ex)
            {
                mLogger.Error("value=> " + value);
                mLogger.Error(ex.ToString());
            }
            finally
            {
                if (FacilityServiceClient != null)
                    FacilityServiceClient.Close();
                if (FacilityServiceClient2012 != null)
                    FacilityServiceClient2012.Close();
                if (FacilityServiceClient2013 != null)
                    FacilityServiceClient2013.Close();
            }
        }

        private void FillContractData(RMarketRequestInfo requestInfo, List<ContractInfo> contractDetails)
        {
            try
            {
                mLogger.Debug("begin FillContractData");
                double NumberOfLoops;
                bool flagRoundNumber = false;
                int NumberLeft = 0;
                int requestPerLoop = 1000;
                if (ConfigurationManager.AppSettings["requestPerLoop"] != null)
                    requestPerLoop = Convert.ToInt32(ConfigurationManager.AppSettings["requestPerLoop"]);
                if (requestInfo.SecurityDetails.Count % requestPerLoop == 0)
                {
                    NumberOfLoops = requestInfo.SecurityDetails.Count / requestPerLoop;
                    flagRoundNumber = true;
                }
                else
                {
                    NumberOfLoops = Math.Ceiling((double)requestInfo.SecurityDetails.Count / requestPerLoop);
                    NumberLeft = requestInfo.SecurityDetails.Count % requestPerLoop;
                }
                string username = string.Empty;
                string password = string.Empty;
                DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    username = clientDetails.Rows[0]["username"].ToString();
                    password = clientDetails.Rows[0]["password"].ToString();
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    return;
                }
                for (int count = 0; count < NumberOfLoops; count++)
                {

                    List<ContractInfo> listContractInfo = new List<ContractInfo>();

                    if (count == NumberOfLoops - 1 && !flagRoundNumber)
                        listContractInfo = GetContractDetails(requestInfo.SecurityDetails, count, requestPerLoop, NumberLeft, username, password);
                    else
                        listContractInfo = GetContractDetails(requestInfo.SecurityDetails, count, requestPerLoop, requestPerLoop, username, password);

                    foreach (var contract in listContractInfo)
                    {
                        ContractInfo cont = RMarketUtils.GetCloneContract(contract);
                        contractDetails.Add(cont);
                    }
                }
                mLogger.Debug("end FillContractData");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                contractDetails.Clear();
                throw;
            }
        }

        private void INIT(RMarketRequestInfo requestInfo, Dictionary<string, RMarkitFacilityResponse> FacilityDetails)
        {
            if (FacilityDetails == null || FacilityDetails.Count == 0)
            {
                FillFacilityDetails(FacilityDetails, requestInfo);
            }
        }

        private void FillFacilityDetails(Dictionary<string, RMarkitFacilityResponse> FacilityDetails, RMarketRequestInfo requestInfo)
        {
            try
            {
                mLogger.Debug("begin FillFacilityDetails");
                double NumberOfLoops;
                bool flagRoundNumber = false;
                int NumberLeft = 0;
                int requestPerLoop = 100;
                if (ConfigurationManager.AppSettings["requestPerLoop"] != null)
                    requestPerLoop = Convert.ToInt32(ConfigurationManager.AppSettings["requestPerLoop"]);
                if (requestInfo.SecurityDetails.Count % requestPerLoop == 0)
                {
                    NumberOfLoops = requestInfo.SecurityDetails.Count / requestPerLoop;
                    flagRoundNumber = true;
                }
                else
                {
                    NumberOfLoops = Math.Ceiling((double)requestInfo.SecurityDetails.Count / requestPerLoop);
                    NumberLeft = requestInfo.SecurityDetails.Count % requestPerLoop;
                }
                string username = string.Empty;
                string password = string.Empty; DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    username = clientDetails.Rows[0]["username"].ToString();
                    password = clientDetails.Rows[0]["password"].ToString();
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                }
                for (int count = 0; count < NumberOfLoops; count++)
                {
                    Dictionary<string, RMarkitFacilityResponse> listFacInfo = new Dictionary<string, RMarkitFacilityResponse>();

                    if (count == NumberOfLoops - 1 && !flagRoundNumber)
                        listFacInfo = GetFacilityDetails(requestInfo.SecurityDetails, count, requestPerLoop, NumberLeft, username, password, requestInfo.RequestType);
                    else
                        listFacInfo = GetFacilityDetails(requestInfo.SecurityDetails, count, requestPerLoop, requestPerLoop, username, password, requestInfo.RequestType);

                    foreach (var facility in listFacInfo.Keys)
                    {
                        FacilityDetails.Add(facility, listFacInfo[facility]);
                    }
                }
                mLogger.Debug("end FillFacilityDetails");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                FacilityDetails.Clear();
                throw;
            }
        }

        internal Dictionary<string, RMarkitFacilityResponse> GetFacilityDetails(Dictionary<string, RMarketIdentifierType> WSODataFacilityID, int count, int number, int loopSize, string username, string password, List<RMarketRequestType> RequestType)
        {
            try
            {
                mLogger.Debug("begin GetFacilityDetails");
                Dictionary<string, RMarkitFacilityResponse> listFacInfo = new Dictionary<string, RMarkitFacilityResponse>();
                Task[] tasks = new Task[loopSize];

                for (int i = 0; i < loopSize; i++)
                {
                    FacilityInfo facInfo = new FacilityInfo();
                    int j = i;
                    string key = WSODataFacilityID.Keys.ElementAt((count * number) + j);
                    tasks[j] = new Task(() => { GetFacDetails(key, WSODataFacilityID[key], listFacInfo, username, password, RequestType); });
                    tasks[j].Start();
                }
                Task.WaitAll(tasks);
                mLogger.Debug("end GetFacilityDetails");
                return listFacInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

        }

        internal void GetFacDetails(string value, RMarketIdentifierType identifier, Dictionary<string, RMarkitFacilityResponse> list, string username, string password, List<RMarketRequestType> RequestType)
        {
            FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchServiceClient();
            FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient2013 = new FacilitySearchService2013.FacilitySearchServiceClient();
            FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient2012 = new FacilitySearchService2012.FacilitySearchServiceClient();

            string markitVersion = GetMarkitVersion();
            try
            {
                mLogger.Debug("start GetFacDetails for value=> " + value);
                bool isRest = false;
                FacilitySearchResults results = new FacilitySearchResults();
                RMarkitFacilityResponse markitResponse = new RMarkitFacilityResponse();
                //FacilityServiceClient =  RMarketUtils.InitializeFacilityService(username,password);
                if (markitVersion == "2013")
                {
                    FacilityServiceClient2013.ClientCredentials.UserName.UserName = username;
                    FacilityServiceClient2013.ClientCredentials.UserName.Password = password;
                    List<FacilitySearchService2013.SecurityId> listOfSecurityIds2013 = new List<FacilitySearchService2013.SecurityId>() {
                    new FacilitySearchService2013.SecurityId() { SecurityIdName = identifier.ToString(), SecurityIdValue =value} };

                    FacilitySearchService2013.FacilitySearchResults results2013 = new FacilitySearchService2013.FacilitySearchResults();
                    if (FacilityServiceClient2013 != null)
                        FacilityServiceClient2013.GetFacilityBySecurityId(listOfSecurityIds2013.ToArray(), out results2013);

                    results = (FacilitySearchResults)RMarketUtils.CopyData(results2013, typeof(FacilitySearchResults));

                    if (results2013 != null && results2013.Issuer != null && results2013.Issuer.BankDeal != null && results2013.Issuer.BankDeal.Facility != null && results2013.Issuer.BankDeal.Facility.CallSchedule != null && results2013.Issuer.BankDeal.Facility.CallSchedule.Count() > 0)
                    {
                        if (results == null)
                            results = new FacilitySearchResults();
                        if (results.Issuer == null)
                            results.Issuer = new FacilitySearchService.Issuer();
                        if (results.Issuer.BankDeal == null)
                            results.Issuer.BankDeal = new FacilitySearchService.BankDeal();
                        if (results.Issuer.BankDeal.Facility == null)
                            results.Issuer.BankDeal.Facility = new FacilitySearchService.Facility();
                        if (results.Issuer.BankDeal.Facility.CallScheduleDetail == null)
                            results.Issuer.BankDeal.Facility.CallScheduleDetail = new CallScheduleDetail();
                        results.Issuer.BankDeal.Facility.CallScheduleDetail.CallSchedules = (CallSchedule[])RMarketUtils.CopyData(results2013.Issuer.BankDeal.Facility.CallSchedule, typeof(CallSchedule[]));
                    }
                }
                else if (markitVersion == "2012")
                {
                    FacilityServiceClient2012.ClientCredentials.UserName.UserName = username;
                    FacilityServiceClient2012.ClientCredentials.UserName.Password = password;
                    List<FacilitySearchService2012.SecurityId> listOfSecurityIds2012 = new List<FacilitySearchService2012.SecurityId>() {
                    new FacilitySearchService2012.SecurityId() { SecurityIdName = identifier.ToString(), SecurityIdValue =value} };

                    FacilitySearchService2012.FacilitySearchResults results2012 = new FacilitySearchService2012.FacilitySearchResults();
                    if (FacilityServiceClient2012 != null)
                        FacilityServiceClient2012.GetFacilityBySecurityId(listOfSecurityIds2012.ToArray(), out results2012);

                    results = (FacilitySearchResults)RMarketUtils.CopyData(results2012, typeof(FacilitySearchResults));

                    if (results2012 != null && results2012.Issuer != null && results2012.Issuer.BankDeal != null && results2012.Issuer.BankDeal.Facility != null)
                    {
                        if (results2012.Issuer.BankDeal.Facility.MinAssignment.HasValue)
                            results.Issuer.BankDeal.Facility.MinAssignment = (decimal)results2012.Issuer.BankDeal.Facility.MinAssignment.Value;
                        if (results2012.Issuer.BankDeal.Facility.MinHold.HasValue)
                            results.Issuer.BankDeal.Facility.MinHold = (decimal)results2012.Issuer.BankDeal.Facility.MinHold.Value;

                        if (results2012.Issuer.BankDeal.Facility.CallSchedule != null && results2012.Issuer.BankDeal.Facility.CallSchedule.Count() > 0)
                        {
                            if (results == null)
                                results = new FacilitySearchResults();
                            if (results.Issuer == null)
                                results.Issuer = new FacilitySearchService.Issuer();
                            if (results.Issuer.BankDeal == null)
                                results.Issuer.BankDeal = new FacilitySearchService.BankDeal();
                            if (results.Issuer.BankDeal.Facility == null)
                                results.Issuer.BankDeal.Facility = new FacilitySearchService.Facility();
                            if (results.Issuer.BankDeal.Facility.CallScheduleDetail == null)
                                results.Issuer.BankDeal.Facility.CallScheduleDetail = new CallScheduleDetail();
                            results.Issuer.BankDeal.Facility.CallScheduleDetail.CallSchedules = (CallSchedule[])RMarketUtils.CopyData(results2012.Issuer.BankDeal.Facility.CallSchedule, typeof(CallSchedule[]));
                        }
                    }
                }
                else if (markitVersion == "2015")
                {
                    FacilityServiceClient.ClientCredentials.UserName.UserName = username;
                    FacilityServiceClient.ClientCredentials.UserName.Password = password;
                    List<FacilitySearchService.SecurityId> listOfSecurityIds = new List<FacilitySearchService.SecurityId>() {
                    new FacilitySearchService.SecurityId() { SecurityIdName = identifier.ToString(), SecurityIdValue =value} };

                    if (FacilityServiceClient != null)
                        FacilityServiceClient.GetFacilityBySecurityId(listOfSecurityIds.ToArray(), out results);
                }
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    isRest = true;

                    string markitRestFacilityURL = GetMarkitRestURL("MarkitRestFacilityURL");

                    var facilityId = value;
                    if (identifier != RMarketIdentifierType.WSODataFacilityID)
                        facilityId = FetchFacilityId(value, username, password, markitRestFacilityURL, identifier);

                    var markitResponseStr = SRMCommon.RestAPICaller(markitRestFacilityURL + "/Detail/" + facilityId, SRMRestMethod.GET, null, username, password);

                    if (!string.IsNullOrEmpty(markitResponseStr))
                        markitResponse = JsonConvert.DeserializeObject<RMarkitFacilityResponse>(markitResponseStr);

                    if (RequestType.Contains(RMarketRequestType.Rating))
                    {
                        var markitRatingResponseStr = SRMCommon.RestAPICaller(markitRestFacilityURL + "/Detail/Ratings/" + facilityId, SRMRestMethod.GET, null, username, password);

                        if (!string.IsNullOrEmpty(markitRatingResponseStr))
                        {
                            if (markitResponse == null)
                                markitResponse = new RMarkitFacilityResponse();
                            if (markitResponse.bankDeal == null)
                                markitResponse.bankDeal = new RMarkitBankDeal();
                            if (markitResponse.bankDeal.facility == null)
                                markitResponse.bankDeal.facility = new RMarkitFacility();
                            markitResponse.bankDeal.facility.Ratings = JsonConvert.DeserializeObject<RMarkitRating[]>(markitRatingResponseStr);
                        }
                    }

                    if (RequestType.Contains(RMarketRequestType.SIC))
                    {
                        var markitSICResponseStr = SRMCommon.RestAPICaller(markitRestFacilityURL + "/Detail/SICs/" + facilityId, SRMRestMethod.GET, null, username, password);

                        if (!string.IsNullOrEmpty(markitSICResponseStr))
                        {
                            if (markitResponse == null)
                                markitResponse = new RMarkitFacilityResponse();
                            if (markitResponse.bankDeal == null)
                                markitResponse.bankDeal = new RMarkitBankDeal();
                            if (markitResponse.bankDeal.facility == null)
                                markitResponse.bankDeal.facility = new RMarkitFacility();
                            markitResponse.bankDeal.facility.SICs = JsonConvert.DeserializeObject<RMarkitSIC[]>(markitSICResponseStr);
                        }
                    }
                }

                if (results != null && !isRest)
                    markitResponse = RMarkitResponseParser.GetFacilityResponseFromSOAP(results);

                if (markitResponse != null)
                {
                    lock (((IDictionary)list).SyncRoot)
                    {
                        list.Add(identifier.ToString() + "~~" + value, markitResponse);

                        if (DictIdentifierVsFacilityId == null)
                            DictIdentifierVsFacilityId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                        if (DictFacilityIdVsLoanX == null)
                            DictFacilityIdVsLoanX = new Dictionary<int, string>();
                        if (DictFacilityIdVsIdentifierDetails == null)
                            DictFacilityIdVsIdentifierDetails = new Dictionary<int, KeyValuePair<string, string>>();

                        if (markitResponse.bankDeal != null && markitResponse.bankDeal.facility != null && markitResponse.bankDeal.facility.wsoDataFacilityId.HasValue)
                        {
                            DictIdentifierVsFacilityId[identifier.ToString() + "~~" + value] = markitResponse.bankDeal.facility.wsoDataFacilityId.Value;
                            DictFacilityIdVsIdentifierDetails[markitResponse.bankDeal.facility.wsoDataFacilityId.Value] = new KeyValuePair<string, string>(identifier.ToString(), value);

                            if (markitResponse.bankDeal.facility.identifiers != null)
                            {
                                var loanxIdObject = markitResponse.bankDeal.facility.identifiers.Where(x => x.type.Equals("LoanX")).FirstOrDefault();

                                if (loanxIdObject != null)
                                    DictFacilityIdVsLoanX[markitResponse.bankDeal.facility.wsoDataFacilityId.Value] = loanxIdObject.value;
                            }
                        }
                    }
                }
                else
                {
                    mLogger.Error("null value recieved from MARKIT for facility = >  " + value);
                }
                mLogger.Debug("end GetFacDetails for value=> " + value);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                if (FacilityServiceClient != null)
                    FacilityServiceClient.Close();
                if (FacilityServiceClient2012 != null)
                    FacilityServiceClient2012.Close();
                if (FacilityServiceClient2013 != null)
                    FacilityServiceClient2013.Close();
            }

        }

        private string FetchFacilityId(string identifierValue, string username, string password, string markitRestFacilityURL, RMarketIdentifierType identifier)
        {
            var facilityIdStr = SRMCommon.RestAPICaller(markitRestFacilityURL + "/Identifiers/FacilityLookup/" + identifierValue, SRMRestMethod.GET, null, username, password);

            if (DictIdentifierVsFacilityId == null)
                DictIdentifierVsFacilityId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            int facilityId;
            if (int.TryParse(facilityIdStr, out facilityId))
            {
                DictIdentifierVsFacilityId[identifier.ToString() + "~~" + identifierValue] = facilityId;
            }

            return facilityIdStr;
        }

        internal List<ContractInfo> GetContractDetails(Dictionary<string, RMarketIdentifierType> WSODataFacilityID, int count, int number, int loopSize, string username, string password)
        {
            try
            {
                mLogger.Debug("begin GetContractDetails");
                List<ContractInfo> listContractInfo = new List<ContractInfo>();
                Task[] tasks = new Task[loopSize];

                for (int i = 0; i < loopSize; i++)
                {
                    int j = i;
                    string key = WSODataFacilityID.Keys.ElementAt((count * number) + j);
                    tasks[j] = new Task(() => { GetConDetails(key, WSODataFacilityID[key], listContractInfo, username, password); });
                    tasks[j].Start();
                }
                Task.WaitAll(tasks);
                mLogger.Debug("end GetContractDetails");
                return listContractInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

        }

        internal void GetConDetails(string value, RMarketIdentifierType identifier, List<ContractInfo> list, string username, string password)
        {
            FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient2013 = null;
            FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient2012 = null;

            string markitVersion = GetMarkitVersion();
            try
            {
                string loanX = string.Empty;
                int facilityId = 0;
                if (identifier == RMarketIdentifierType.WSODataFacilityID)
                    facilityId = Convert.ToInt32(value);
                else if (DictIdentifierVsFacilityId != null && DictIdentifierVsFacilityId.ContainsKey(identifier.ToString() + "~~" + value))
                    facilityId = DictIdentifierVsFacilityId[identifier.ToString() + "~~" + value];
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    string markitRestFacilityURL = GetMarkitRestURL("MarkitRestFacilityURL");

                    var facilityIdStr = FetchFacilityId(value, username, password, markitRestFacilityURL, identifier);
                    if (!int.TryParse(facilityIdStr, out facilityId))
                    {
                        mLogger.Debug("Not able to identify facility id for identifier - " + value + ", identifier type - " + identifier.ToString());
                        return;
                    }
                }
                else
                {
                    mLogger.Debug("Not able to identify facility id for identifier - " + value + ", identifier type - " + identifier.ToString());
                    return;
                }

                if (DictFacilityIdVsLoanX != null && DictFacilityIdVsLoanX.ContainsKey(facilityId))
                    loanX = DictFacilityIdVsLoanX[facilityId];

                mLogger.Debug("start GetConDetails for value=> " + value + ", facility id - " + facilityId);
                if (markitVersion == "2012")
                {
                    FacilitySearchService2012.Contract[] listContracts = null;
                    FacilityServiceClient2012 = RMarketUtils.InitializeFacilityService2012(username, password);
                    if (FacilityServiceClient2012 != null)
                        FacilityServiceClient2012.GetContractsByFacilityId(DateTime.Now, facilityId, out listContracts);
                    if (listContracts != null)
                    {
                        foreach (FacilitySearchService2012.Contract contract in listContracts)
                        {

                            ContractInfo contractInfo = new ContractInfo();
                            contractInfo.ContractAccrualBasis = Convert.ToString(contract.AccrualBasis);
                            contractInfo.ContractAllInRate = (float?)contract.AllInRate;
                            contractInfo.ContractBaseRate = (float?)contract.BaseRate;
                            contractInfo.ContractName = (string)contract.ContractName;
                            contractInfo.ContractType = (string)contract.ContractType;
                            contractInfo.ContractCurrencyType = (string)contract.CurrencyType;
                            contractInfo.ContractExchangeRate = (decimal?)contract.ExchangeRate;
                            contractInfo.ContractFrequency = (int?)contract.Frequency;
                            contractInfo.ContractGlobalAmount = (decimal?)contract.GlobalAmount;
                            contractInfo.ContractInterestDue = (decimal?)contract.InterestDue;
                            contractInfo.ContractInterestReceived = (decimal?)contract.InterestReceived;
                            contractInfo.ContractMaturityDate = (DateTime?)contract.MaturityDate;
                            contractInfo.ContractMonthCount = (string)contract.MonthCount;
                            contractInfo.ContractNextPaymentDate = (DateTime?)contract.NextPaymentDate;
                            contractInfo.ContractNotes = (string)contract.Notes;
                            contractInfo.ContractRateOption = (string)contract.RateOption;
                            contractInfo.ContractSpread = (float?)contract.Spread;
                            contractInfo.ContractStartDate = (DateTime?)contract.StartDate;
                            contractInfo.ContractID = (int?)contract.WSODataContractID;
                            contractInfo.ContractYearCount = (string)contract.YearCount;
                            contractInfo.ContractFacilityId = facilityId;
                            contractInfo.ContractLoanX = loanX;
                            lock (((ICollection)list).SyncRoot)
                                list.Add(contractInfo);
                        }
                    }
                    else
                    {
                        mLogger.Error("null value recieved from MARKIT for contract = >  " + value + ", facility id - " + facilityId);
                    }
                }
                else if (markitVersion == "2013" || markitVersion == "2015")
                {
                    FacilitySearchService2013.Contract[] listContracts = null;
                    FacilityServiceClient2013 = RMarketUtils.InitializeFacilityService2013(username, password);
                    if (FacilityServiceClient2013 != null)
                        FacilityServiceClient2013.GetContractsByFacilityId(DateTime.Now, facilityId, out listContracts);
                    if (listContracts != null)
                    {
                        foreach (FacilitySearchService2013.Contract contract in listContracts)
                        {

                            ContractInfo contractInfo = new ContractInfo();
                            contractInfo.ContractAccrualBasis = Convert.ToString(contract.AccrualBasis);
                            contractInfo.ContractAllInRate = (float?)contract.AllInRate;
                            contractInfo.ContractBaseRate = (float?)contract.BaseRate;
                            contractInfo.ContractName = (string)contract.ContractName;
                            contractInfo.ContractType = (string)contract.ContractType;
                            contractInfo.ContractCurrencyType = (string)contract.CurrencyType;
                            contractInfo.ContractExchangeRate = (decimal?)contract.ExchangeRate;
                            contractInfo.ContractFrequency = (int?)contract.Frequency;
                            contractInfo.ContractGlobalAmount = (decimal?)contract.GlobalAmount;
                            contractInfo.ContractInterestDue = (decimal?)contract.InterestDue;
                            contractInfo.ContractInterestReceived = (decimal?)contract.InterestReceived;
                            contractInfo.ContractMaturityDate = (DateTime?)contract.MaturityDate;
                            contractInfo.ContractMonthCount = (string)contract.MonthCount;
                            contractInfo.ContractNextPaymentDate = (DateTime?)contract.NextPaymentDate;
                            contractInfo.ContractNotes = (string)contract.Notes;
                            contractInfo.ContractRateOption = (string)contract.RateOption;
                            contractInfo.ContractSpread = (float?)contract.Spread;
                            contractInfo.ContractStartDate = (DateTime?)contract.StartDate;
                            contractInfo.ContractID = (int?)contract.WSODataContractID;
                            contractInfo.ContractYearCount = (string)contract.YearCount;
                            contractInfo.ContractFacilityId = facilityId;
                            contractInfo.ContractLoanX = loanX;
                            lock (((ICollection)list).SyncRoot)
                                list.Add(contractInfo);
                        }
                    }
                    else
                    {
                        mLogger.Error("null value recieved from MARKIT for contract = >  " + value + ", facility id - " + facilityId);
                    }
                }
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    string markitRestContractsURL = GetMarkitRestURL("MarkitRestContractsURL");

                    var contractsResultsStr = SRMCommon.RestAPICaller(markitRestContractsURL + "/Detail/" + facilityId, SRMRestMethod.GET, null, username, password);

                    if (!string.IsNullOrEmpty(contractsResultsStr))
                    {
                        RMarkitContract[] contractResultsRest = JsonConvert.DeserializeObject<RMarkitContract[]>(contractsResultsStr);

                        foreach (RMarkitContract contract in contractResultsRest)
                        {

                            ContractInfo contractInfo = new ContractInfo();
                            contractInfo.ContractAccrualBasis = Convert.ToString(contract.accrualBasis);
                            contractInfo.ContractAllInRate = (float?)contract.allInRate;
                            contractInfo.ContractBaseRate = (float?)contract.baseRate;
                            contractInfo.ContractName = (string)contract.contractName;
                            contractInfo.ContractType = (string)contract.contractType;
                            contractInfo.ContractCurrencyType = (string)contract.currencyType;
                            contractInfo.ContractExchangeRate = (decimal?)contract.exchangeRate;
                            contractInfo.ContractFrequency = (int?)contract.frequency;
                            contractInfo.ContractGlobalAmount = (decimal?)contract.globalAmount;
                            contractInfo.ContractInterestDue = (decimal?)contract.interestDue;
                            contractInfo.ContractInterestReceived = (decimal?)contract.interestReceived;
                            contractInfo.ContractMaturityDate = (DateTime?)contract.maturityDate;
                            contractInfo.ContractMonthCount = (string)contract.monthCount;
                            contractInfo.ContractNextPaymentDate = (DateTime?)contract.nextPaymentDate;
                            contractInfo.ContractNotes = (string)contract.notes;
                            contractInfo.ContractRateOption = (string)contract.rateOption;
                            contractInfo.ContractSpread = (float?)contract.spread;
                            contractInfo.ContractStartDate = (DateTime?)contract.startDate;
                            contractInfo.ContractID = (int?)contract.wsoDataContractId;
                            contractInfo.ContractYearCount = (string)contract.yearCount;
                            contractInfo.ContractBehavior = contract.behavior;
                            contractInfo.ContractReceiveDate = contract.receiveDate;
                            contractInfo.ContractIsReceived = contract.isReceived;
                            contractInfo.ContractAccrualBasisId = contract.accrualBasisId;
                            contractInfo.ContractAccrualFeeType = contract.accrualFeeType;
                            contractInfo.ContractIsObservationShift = contract.isObservationShift;
                            contractInfo.ContractInterestMethod = contract.interestMethod;
                            contractInfo.ContractSpreadAdjustment = contract.spreadAdjustment;
                            contractInfo.ContractLookBackDayOffset = contract.lookBackDayOffset;
                            contractInfo.ContractFacilityId = facilityId;
                            contractInfo.ContractLoanX = loanX;
                            lock (((ICollection)list).SyncRoot)
                                list.Add(contractInfo);
                        }
                    }
                    else
                    {
                        mLogger.Error("null value recieved from MARKIT for contract = >  " + value + ", facility id - " + facilityId);
                    }
                }
                mLogger.Debug("end GetConDetails for value=> " + value + ", facility id - " + facilityId);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                //throw;
            }
            finally
            {
                if (FacilityServiceClient2013 != null)
                    FacilityServiceClient2013.Close();
                if (FacilityServiceClient2012 != null)
                    FacilityServiceClient2012.Close();
            }
        }

        internal List<int> GetAllSubscribedFacilities()
        {
            mLogger.Debug("MarkitWSO -> start GetAllSubscribedFacilities method ");

            string markitVersion = GetMarkitVersion();
            if (!markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
            {
                GamServiceClient GAMClient = RMarketUtils.InitializeGAMService(VendorPreferenceId);
                if (GAMClient != null)
                {
                    mLogger.Debug("MarkitWSO -> end GetAllSubscribedFacilities method ");
                    return GAMClient.GetAllSubscribed().ToList();
                }
            }
            else
            {
                List<int> facilityIDS = null;

                string username = string.Empty;
                string password = string.Empty;
                DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    username = clientDetails.Rows[0]["username"].ToString();
                    password = clientDetails.Rows[0]["password"].ToString();

                    string markitRestGAMURL = GetMarkitRestURL("MarkitRestGAMURL");

                    var facilityIDSStr = SRMCommon.RestAPICaller(markitRestGAMURL + "/Subscribed", SRMRestMethod.GET, null, username, password);

                    if (!string.IsNullOrEmpty(facilityIDSStr))
                        facilityIDS = JsonConvert.DeserializeObject<List<int>>(facilityIDSStr);
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                }

                return facilityIDS;
            }
            mLogger.Debug("MarkitWSO -> end GetAllSubscribedFacilities method ");
            return null;
        }

        private string GetMarkitVersion()
        {
            string markitVersion = "2013";
            bool IsMarkitAPIVersion2013 = true;
            if (ConfigurationManager.AppSettings["IsMarkitAPIVersion2013"] != null)
                IsMarkitAPIVersion2013 = Convert.ToBoolean(ConfigurationManager.AppSettings["IsMarkitAPIVersion2013"]);

            if (!IsMarkitAPIVersion2013)
                markitVersion = "2015";
            else
            {
                string markitAPIVersion = string.Empty;
                if (ConfigurationManager.AppSettings["MarkitAPIVersion"] != null)
                    markitAPIVersion = Convert.ToString(ConfigurationManager.AppSettings["MarkitAPIVersion"]);

                if (!string.IsNullOrEmpty(markitAPIVersion) && allowedMarkitVersions.Contains(markitAPIVersion))
                    markitVersion = markitAPIVersion;
            }

            return markitVersion;
        }

        internal Dictionary<string, RMarketIdentifierType> GetAllAvailableFacilities()
        {
            FacilitySearchServiceClient FacilityServiceClient = null;
            FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient2013 = null;
            FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient2012 = null;

            string markitVersion = GetMarkitVersion();
            try
            {
                mLogger.Debug("MarkitWSO -> start GetAllAvailableFacilities method ");
                Dictionary<string, RMarketIdentifierType> allFacilities = new Dictionary<string, RMarketIdentifierType>();
                if (markitVersion == "2013")
                    FacilityServiceClient2013 = RMarketUtils.InitializeFacilityService2013(VendorPreferenceId);
                else if (markitVersion == "2012")
                    FacilityServiceClient2012 = RMarketUtils.InitializeFacilityService2012(VendorPreferenceId);
                else if (markitVersion == "2015")
                    FacilityServiceClient = RMarketUtils.InitializeFacilityService(VendorPreferenceId);
                string message = string.Empty;
                int[] facilityIDS = null;
                if ((FacilityServiceClient != null && markitVersion == "2015") || (FacilityServiceClient2013 != null && markitVersion == "2013") || (FacilityServiceClient2012 != null && markitVersion == "2012"))
                {
                    if (markitVersion == "2012")
                        facilityIDS = FacilityServiceClient2012.GetAvailableFacilities(out message);
                    else if (markitVersion == "2015")
                        facilityIDS = FacilityServiceClient.GetAvailableFacilities(out message);
                    else
                        facilityIDS = FacilityServiceClient2013.GetAvailableFacilities(out message);
                }
                else if (markitVersion.Equals("Rest", StringComparison.OrdinalIgnoreCase))
                {
                    string username = string.Empty;
                    string password = string.Empty;
                    DataTable clientDetails = RMarketUtils.FetchCredentials(VendorPreferenceId);
                    if (clientDetails != null && clientDetails.Rows.Count > 0)
                    {
                        username = clientDetails.Rows[0]["username"].ToString();
                        password = clientDetails.Rows[0]["password"].ToString();
                    }
                    else
                    {
                        mLogger.Debug("no connectivity details found");
                        return allFacilities;
                    }

                    string markitRestGAMURL = GetMarkitRestURL("MarkitRestGAMURL");

                    var facilityIDSStr = SRMCommon.RestAPICaller(markitRestGAMURL + "/Available", SRMRestMethod.GET, null, username, password);

                    if (!string.IsNullOrEmpty(facilityIDSStr))
                        facilityIDS = JsonConvert.DeserializeObject<int[]>(facilityIDSStr);
                }
                if (facilityIDS != null)
                {
                    foreach (int fac in facilityIDS)
                    {
                        allFacilities.Add(fac.ToString(), RMarketIdentifierType.WSODataFacilityID);
                    }
                }
                mLogger.Debug("MarkitWSO -> end GetAllAvailableFacilities method ");
                return allFacilities;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
            finally
            {
                if (FacilityServiceClient != null)
                    FacilityServiceClient.Close();
                if (FacilityServiceClient2012 != null)
                    FacilityServiceClient2012.Close();
                if (FacilityServiceClient2013 != null)
                    FacilityServiceClient2013.Close();
            }
        }

        internal void FetchAllFacilityDetails()
        {
            RMarketRequestInfo info = new RMarketRequestInfo();
            info.IsImmediate = true;
            info.RequestType = new List<RMarketRequestType>() { RMarketRequestType.Facility };
            info.SecurityDetails = GetAllAvailableFacilities();
            RMarketResponseInfo resultData = FetchFacilityData(info);
            if (resultData.Status == RStatus.PASSED)
                RMarketUtils.InsertDataInToDatabase(resultData.FacilityData);
            else
            {
                mLogger.Error("Error while fetching data from MARKIT");
            }
        }

        internal RMarketResponseInfo FetchFacilityData(string FileName)
        {
            try
            {
                mLogger.Debug("MarkitWSO -> start FetchFacilityData method ");
                RMarketResponseInfo response = new RMarketResponseInfo();
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() + "VendorWorkingDirectory";
                string path = Path.Combine(workingDirectory, FileName);
                response.FacilityData.ReadXml(path);
                response.Status = RStatus.PASSED;
                mLogger.Debug("MarkitWSO -> start FetchFacilityData method ");
                return response;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RMarketResponseInfo response = new RMarketResponseInfo();
                response.FacilityData = null;
                response.Status = RStatus.FAILED;
                return response;
            }
        }
    }
}
