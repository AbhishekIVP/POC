using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.RMarketWSO.FacilitySearchService;
using com.ivp.rad.RMarketWSO.GAMService;
using com.ivp.rad.RMarketWSO.TransactionService;
using System.Data;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using com.ivp.rad.dal;
using com.ivp.rad.configurationmanagement;
using System.Reflection;
using com.ivp.rad.cryptography;

namespace com.ivp.rad.RMarketWSO
{
    class RMarketUtils
    {
        static RRSAEncrDecr encDec = new RRSAEncrDecr();

        static IRLogger mLogger = RLogFactory.CreateLogger("RMarketUtils");

        public static GamServiceClient InitializeGAMService(int VendorPreferenceId = 1)
        {
            mLogger.Debug("start InitializeGAMService");
            GamServiceClient GAMClient = null;
            try
            {
                GAMClient = new GamServiceClient();
                DataTable clientDetails = FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    GAMClient.ClientCredentials.UserName.UserName = clientDetails.Rows[0]["username"].ToString();
                    GAMClient.ClientCredentials.UserName.Password = clientDetails.Rows[0]["password"].ToString();
                    mLogger.Debug("end InitializeGAMService");
                    return GAMClient;
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    mLogger.Debug("end InitializeGAMService");
                    return null;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }

        }

        public static FacilitySearchServiceClient InitializeFacilityService(int VendorPreferenceId = 1)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchServiceClient();
                DataTable clientDetails = FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    FacilityServiceClient.ClientCredentials.UserName.UserName = clientDetails.Rows[0]["username"].ToString();
                    FacilityServiceClient.ClientCredentials.UserName.Password = clientDetails.Rows[0]["password"].ToString();
                    mLogger.Debug("end InitializeFacilityService");
                    return FacilityServiceClient;
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    mLogger.Debug("end InitializeFacilityService");
                    return null;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static FacilitySearchService2013.FacilitySearchServiceClient InitializeFacilityService2013(int VendorPreferenceId = 1)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchService2013.FacilitySearchServiceClient();
                DataTable clientDetails = FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    FacilityServiceClient.ClientCredentials.UserName.UserName = clientDetails.Rows[0]["username"].ToString();
                    FacilityServiceClient.ClientCredentials.UserName.Password = clientDetails.Rows[0]["password"].ToString();
                    mLogger.Debug("end InitializeFacilityService");
                    return FacilityServiceClient;
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    mLogger.Debug("end InitializeFacilityService");
                    return null;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static FacilitySearchService2012.FacilitySearchServiceClient InitializeFacilityService2012(int VendorPreferenceId = 1)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchService2012.FacilitySearchServiceClient();
                DataTable clientDetails = FetchCredentials(VendorPreferenceId);
                if (clientDetails != null && clientDetails.Rows.Count > 0)
                {
                    FacilityServiceClient.ClientCredentials.UserName.UserName = clientDetails.Rows[0]["username"].ToString();
                    FacilityServiceClient.ClientCredentials.UserName.Password = clientDetails.Rows[0]["password"].ToString();
                    mLogger.Debug("end InitializeFacilityService");
                    return FacilityServiceClient;
                }
                else
                {
                    mLogger.Debug("no connectivity details found");
                    mLogger.Debug("end InitializeFacilityService");
                    return null;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static FacilitySearchServiceClient InitializeFacilityService(string username, string password)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchServiceClient();
                FacilityServiceClient.ClientCredentials.UserName.UserName = username;
                FacilityServiceClient.ClientCredentials.UserName.Password = password;
                mLogger.Debug("end InitializeFacilityService");
                return FacilityServiceClient;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static FacilitySearchService2013.FacilitySearchServiceClient InitializeFacilityService2013(string username, string password)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchService2013.FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchService2013.FacilitySearchServiceClient();
                FacilityServiceClient.ClientCredentials.UserName.UserName = username;
                FacilityServiceClient.ClientCredentials.UserName.Password = password;
                mLogger.Debug("end InitializeFacilityService");
                return FacilityServiceClient;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static FacilitySearchService2012.FacilitySearchServiceClient InitializeFacilityService2012(string username, string password)
        {
            mLogger.Debug("start InitializeFacilityService");
            try
            {
                FacilitySearchService2012.FacilitySearchServiceClient FacilityServiceClient = new FacilitySearchService2012.FacilitySearchServiceClient();
                FacilityServiceClient.ClientCredentials.UserName.UserName = username;
                FacilityServiceClient.ClientCredentials.UserName.Password = password;
                mLogger.Debug("end InitializeFacilityService");
                return FacilityServiceClient;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public static TransactionServiceClient InitializeTransactionService(string username, string password)
        {
            mLogger.Debug("start InitializeTransactionService");
            try
            {
                TransactionServiceClient TransactionServiceClient = new TransactionServiceClient();
                TransactionServiceClient.ClientCredentials.UserName.UserName = username;
                TransactionServiceClient.ClientCredentials.UserName.Password = password;
                mLogger.Debug("end InitializeTransactionService");
                return TransactionServiceClient;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        internal static DataTable FetchCredentials(int VendorPreferenceId = 1)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("FetchCredentials=> start FetchCredentials");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RefMDBVendorConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                DataSet data = mDBConn.ExecuteQuery(string.Format(@"DECLARE @user_name VARCHAR(MAX), @password VARCHAR(MAX), @vendor_management_id INT = {0}

                IF NOT EXISTS (SELECT 1 FROM dbo.ivp_rad_vendor_management_master WHERE is_active = 1 AND vendor_management_id = @vendor_management_id)
	                SELECT @vendor_management_id = 1

                SELECT @user_name = key_value
                FROM dbo.ivp_rad_vendor_management_master vmas
                INNER JOIN dbo.ivp_rad_vendor_management_details vdet
                ON vmas.vendor_management_id = vdet.vendor_management_id
                WHERE is_active = 1 AND vmas.vendor_management_id = @vendor_management_id AND vendor_id = 3 AND key_real_name = 'username'

                SELECT @password = key_value
                FROM dbo.ivp_rad_vendor_management_master vmas
                INNER JOIN dbo.ivp_rad_vendor_management_details vdet
                ON vmas.vendor_management_id = vdet.vendor_management_id
                WHERE is_active = 1 AND vmas.vendor_management_id = @vendor_management_id AND vendor_id = 3 AND key_real_name = 'password'

                SELECT @user_name AS UserName, @password AS [Password]", VendorPreferenceId), RQueryType.Select);
                if (data != null && data.Tables.Count > 0 && data.Tables[0] != null && data.Tables[0].Rows.Count > 0)
                {
                    string password = Convert.ToString(data.Tables[0].Rows[0]["Password"]);
                    if (!string.IsNullOrEmpty(password))
                    {
                        try
                        {
                            password = encDec.DoDecrypt(password);
                            data.Tables[0].Rows[0]["Password"] = password;
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error("FetchCredentials=> " + ex.ToString());
                        }
                    }

                }
                mLogger.Debug("FetchCredentials=> end FetchCredentials");
                return data.Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (mDBConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static void UpdateSubscriptionStatus(string Keys, bool isSubscribed, string user)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("FetchCredentials=> start FetchCredentials");
                RHashlist list = new RHashlist();
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                list.Add("facilityId", Keys);
                if (isSubscribed)
                    list.Add("isSubscribed", 1);
                else
                    list.Add("isSubscribed", 0);
                list.Add("User", user);
                mDBConn.ExecuteQuery("RAD:UpdateSubscriptionStatus", list);
                mLogger.Debug("FetchCredentials=> end FetchCredentials");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (mDBConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static ContractInfo GetCloneContract(ContractInfo contractInfo)
        {
            ContractInfo contract = new ContractInfo();
            contract.ContractAccrualBasis = Convert.ToString(contractInfo.ContractAccrualBasis);
            contract.ContractAllInRate = (float?)contractInfo.ContractAllInRate;
            contract.ContractBaseRate = (float?)contractInfo.ContractBaseRate;
            contract.ContractName = (string)contractInfo.ContractName;
            contract.ContractType = (string)contractInfo.ContractType;
            contract.ContractCurrencyType = (string)contractInfo.ContractCurrencyType;
            contract.ContractExchangeRate = (decimal?)contractInfo.ContractExchangeRate;
            contract.ContractFrequency = (int?)contractInfo.ContractFrequency;
            contract.ContractGlobalAmount = (decimal?)contractInfo.ContractGlobalAmount;
            contract.ContractInterestDue = (decimal?)contractInfo.ContractInterestDue;
            contract.ContractInterestReceived = (decimal?)contractInfo.ContractInterestReceived;
            contract.ContractMaturityDate = (DateTime?)contractInfo.ContractMaturityDate;
            contract.ContractMonthCount = (string)contractInfo.ContractMonthCount;
            contract.ContractNextPaymentDate = (DateTime?)contractInfo.ContractNextPaymentDate;
            contract.ContractNotes = (string)contractInfo.ContractNotes;
            contract.ContractRateOption = (string)contractInfo.ContractRateOption;
            contract.ContractSpread = (float?)contractInfo.ContractSpread;
            contract.ContractStartDate = (DateTime?)contractInfo.ContractStartDate;
            contract.ContractID = (int?)contractInfo.ContractID;
            contract.ContractYearCount = (string)contractInfo.ContractYearCount;
            contract.ContractBehavior = contractInfo.ContractBehavior;
            contract.ContractReceiveDate = contractInfo.ContractReceiveDate;
            contract.ContractIsReceived = contractInfo.ContractIsReceived;
            contract.ContractAccrualBasisId = contractInfo.ContractAccrualBasisId;
            contract.ContractAccrualFeeType = contractInfo.ContractAccrualFeeType;
            contract.ContractIsObservationShift = contractInfo.ContractIsObservationShift;
            contract.ContractInterestMethod = contractInfo.ContractInterestMethod;
            contract.ContractSpreadAdjustment = contractInfo.ContractSpreadAdjustment;
            contract.ContractLookBackDayOffset = contractInfo.ContractLookBackDayOffset;
            contract.ContractFacilityId = contractInfo.ContractFacilityId;
            contract.ContractLoanX = contractInfo.ContractLoanX;
            return contract;
        }

        internal static AmortizationScheduleInfo CloneAmortizationSchedule(RMarkitAmortizationSchedule amortizationResults, int facilityId)
        {
            mLogger.Debug("begin CloneAmortizationSchedule");
            AmortizationScheduleInfo amortizationScheduleInfo = new AmortizationScheduleInfo();
            amortizationScheduleInfo.AmortizationScheduleEndDate = (DateTime?)amortizationResults.endDate;
            amortizationScheduleInfo.AmortizationScheduleDate = (DateTime?)amortizationResults.scheduleDate;
            amortizationScheduleInfo.AmortizationScheduleName = (string)amortizationResults.name;
            amortizationScheduleInfo.AmortizationScheduleID = (int?)amortizationResults.globalAmortizationID;
            amortizationScheduleInfo.AmortizationScheduleDataID = (int?)amortizationResults.wsoAmortizationID;
            amortizationScheduleInfo.AmortizationScheduleFacilityId = facilityId;

            RMarkitAmortization[] amortizations = amortizationResults.payments;
            List<AmortizationInfo> amortizationss = new List<AmortizationInfo>();
            if (amortizations != null)
            {
                foreach (RMarkitAmortization amortization in amortizations)
                {
                    AmortizationInfo amortizationInfo = new AmortizationInfo();
                    amortizationInfo.AmortizationAmount = (decimal?)amortization.amount;
                    amortizationInfo.AmortizationDate = (DateTime?)amortization.date;
                    amortizationInfo.AmortizationNotes = (string)amortization.notes;
                    amortizationInfo.AmortizationPrice = (decimal?)amortization.price;
                    amortizationInfo.AmortizationReceived = (bool?)amortization.isReceived;
                    amortizationInfo.AmortizationFacilityId = facilityId;
                    amortizationss.Add(amortizationInfo);
                }
            }
            amortizationScheduleInfo.AmortizationInfo = amortizationss;
            mLogger.Debug("end; CloneAmortizationSchedule");
            return amortizationScheduleInfo;
        }

        internal static AmortizationScheduleInfo GetCloneAmortizeScheduleInfo(AmortizationScheduleInfo amortizationResults)
        {
            AmortizationScheduleInfo amortizationScheduleInfo = new AmortizationScheduleInfo();
            amortizationScheduleInfo.AmortizationScheduleEndDate = (DateTime?)amortizationResults.AmortizationScheduleEndDate;
            amortizationScheduleInfo.AmortizationScheduleDate = (DateTime?)amortizationResults.AmortizationScheduleDate;
            amortizationScheduleInfo.AmortizationScheduleName = (string)amortizationResults.AmortizationScheduleName;
            amortizationScheduleInfo.AmortizationScheduleID = (int?)amortizationResults.AmortizationScheduleID;
            amortizationScheduleInfo.AmortizationScheduleDataID = (int?)amortizationResults.AmortizationScheduleDataID;
            amortizationScheduleInfo.AmortizationScheduleFacilityId = amortizationResults.AmortizationScheduleFacilityId;

            List<AmortizationInfo> amortizations = amortizationResults.AmortizationInfo;
            List<AmortizationInfo> amortizationss = new List<AmortizationInfo>();
            if (amortizations != null)
            {
                foreach (AmortizationInfo amortization in amortizations)
                {
                    AmortizationInfo amortizationInfo = new AmortizationInfo();
                    amortizationInfo.AmortizationAmount = (decimal?)amortization.AmortizationAmount;
                    amortizationInfo.AmortizationDate = (DateTime?)amortization.AmortizationDate;
                    amortizationInfo.AmortizationNotes = (string)amortization.AmortizationNotes;
                    amortizationInfo.AmortizationPrice = (decimal?)amortization.AmortizationPrice;
                    amortizationInfo.AmortizationReceived = (bool?)amortization.AmortizationReceived;
                    amortizationInfo.AmortizationFacilityId = amortization.AmortizationFacilityId;
                    amortizationss.Add(amortizationInfo);
                }
            }
            amortizationScheduleInfo.AmortizationInfo = amortizationss;
            return amortizationScheduleInfo;
        }

        internal static void CreateTableSchema(Type type, DataTable dataTable)
        {
            PropertyInfo[] properties = type.GetProperties();
            string headerName = string.Empty;
            string tagName = string.Empty;
            properties.ToList<PropertyInfo>().ForEach(delegate (PropertyInfo property)
            {
                {
                    if (property.PropertyType == typeof(List<AmortizationInfo>))
                    {
                    }
                    else
                    {
                        dataTable.Columns.Add(property.Name);
                    }
                }
            });
        }

        internal static void GetAgentBankInfo(RMarkitAgentBank agentBank, string RequestInfo, DataTable agentData)
        {
            try
            {
                mLogger.Debug("start fill agentbank data");
                string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                DataRow dr = agentData.NewRow();
                dr["AgentBankMember"] = agentBank.name;
                dr["AgentBankID"] = agentBank.markitAgentBankId;
                dr["AgentBankContact"] = agentBank.contact;
                dr["AgentBankEmail"] = agentBank.email;
                dr["AgentBankPhone"] = agentBank.phone;
                dr["Identifier"] = data[0];
                dr["Identifier Value"] = data[1];
                agentData.Rows.Add(dr);
                mLogger.Debug("end fill agentbank data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetBankDealInfo(RMarkitBankDeal BD, string RequestInfo, DataTable bankDealData)
        {
            try
            {
                mLogger.Debug("start fill bankdeal data");
                string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                DataRow dr = bankDealData.NewRow();
                dr["BankDealDataID"] = (int?)BD.wsoDataBankDealId;
                dr["BankDealID"] = (string)BD.markitBankDealId;
                dr["BankDealName"] = (string)BD.name;
                dr["BankDealStreetName"] = (string)BD.streetName;
                dr["BankDealGlobalAmount"] = (decimal?)BD.globalAmount;
                dr["BankDealCreditDate"] = BD.creditDate == null ? "" : BD.creditDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                dr["BankDealCurrencyType"] = (string)BD.currency;
                dr["BankDealUnderwriter"] = (string)BD.underwriter;
                dr["BankDealPrepaymentNotes"] = (string)BD.prepaymentNotes;
                dr["BankDealMinAssignment"] = (decimal?)BD.minAssignment;
                dr["BankDealIsCovLite"] = (bool?)BD.isCovLite;
                dr["BankDealDefaultNotes"] = (string)BD.defaultNotes;
                dr["BankDealAgentConsent"] = (bool?)BD.agentConsent;
                dr["BankDealBorrowerConsent"] = (bool?)BD.borrowerConsent;
                dr["BankDealAssignmentFee"] = (decimal?)BD.assignmentFee;
                dr["BankDealMinHold"] = (decimal?)BD.minimumHold;
                dr["BankDealPublicLoan"] = (string)BD.publicLoan;
                dr["BankDealCanAffiliate"] = (bool?)BD.canAffiliate;
                dr["BankDealDescription"] = BD.description;
                dr["BankDealIsCAReceived"] = BD.isCAReceived;
                dr["BankDealPublicSource"] = BD.publicSource;
                dr["BankDealFaxAgent"] = BD.faxAgent;
                dr["BankDealFaxAgentContact"] = BD.faxAgentContact;
                dr["Identifier"] = data[0];
                dr["Identifier Value"] = data[1];
                bankDealData.Rows.Add(dr);
                mLogger.Debug("end fill bankdeal data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetBankDealSponserInfo(string[] bankDealSponsor, string RequestInfo, DataTable BankDealSponser)
        {
            try
            {
                mLogger.Debug("start fill BankDealSponser data");

                if (bankDealSponsor != null && bankDealSponsor.Count() > 0)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    DataRow dr = BankDealSponser.NewRow();
                    dr["BankDealSponserLevelPrimarySponsor"] = bankDealSponsor[0];
                    if (bankDealSponsor.Count() > 1)
                        dr["BankDealSponserLevelSponsor2"] = bankDealSponsor[1];
                    if (bankDealSponsor.Count() > 2)
                        dr["BankDealSponserLevelSponsor3"] = bankDealSponsor[2];
                    dr["Identifier"] = data[0];
                    dr["Identifier Value"] = data[1];
                    BankDealSponser.Rows.Add(dr);
                }
                mLogger.Debug("end fill BankDealSponser data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetFacilityInfo(RMarkitFacility facility, string RequestInfo, DataTable Facility)
        {
            try
            {
                mLogger.Debug("start fill Facility data");
                string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                DataRow dr = Facility.NewRow();
                dr["FacilityID"] = (int?)facility.wsoDataFacilityId;
                dr["FacilityName"] = (string)facility.name;
                dr["FacilityType"] = (string)facility.type;
                dr["FacilityCountry"] = (string)facility.country;
                dr["FacilityPurpose"] = (string)facility.purpose;
                dr["FacilityMaturityDate"] = (DateTime?)facility.maturityDate == null ? "" : facility.maturityDate.Value.ToString("yyyyMMdd hh:mm:ss");
                dr["FacilityOriginalCommitment"] = (double?)facility.originalCommitment;
                dr["FacilityCurrency"] = (string)facility.currency;
                dr["FacilityAbbrevName"] = (string)facility.abbrevName;
                dr["FacilityStatedSpread"] = (float?)facility.statedSpread;
                dr["FacilityActivityTracked"] = (bool?)facility.activityTracked;
                dr["FacilityCaCertified"] = (bool?)facility.caVerified;
                dr["FacilityLastUpdated"] = (DateTime?)facility.lastUpdated == null ? "" : facility.lastUpdated.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                dr["FacilityGuarantor"] = (string)facility.guarantor;
                dr["FacilityIsFixed"] = (bool?)facility.isFixed;
                dr["FacilityTradesWithAccrued"] = (bool?)facility.tradesWithAccrued;
                dr["FacilityIsSynthetic"] = (bool?)facility.isSynthetic;
                dr["FacilityIsDelayedDraw"] = (bool?)facility.isDelayedDraw;
                dr["FacilityIsDIP"] = (bool?)facility.isDip;
                dr["FacilityIsGuaranteed"] = (bool?)facility.isGuaranteed;
                dr["FacilityIsLCCreditLinked"] = (bool?)facility.isLcCreditLinked;
                dr["FacilityHasLCSublimit"] = (bool?)facility.hasLcSublimit;
                dr["FacilityLCSublimit"] = (decimal?)facility.lcSublimit;
                dr["FacilitySeniority"] = (string)facility.seniority;
                dr["FacilityLienType"] = (string)facility.lienType;
                dr["FacilityIssuePrice"] = (decimal?)facility.issuePrice;
                dr["FacilityPaymentFrequency"] = (string)facility.paymentFrequency;
                dr["FacilityCollateral"] = (string)facility.collateral;
                dr["FacilityMinAssignment"] = (float?)facility.minAssignment;
                dr["FacilityMinHold"] = (float?)facility.minHold;
                dr["FacilityIssuingBankConsent"] = (bool?)facility.issuingBankConsent;
                dr["FacilityNotes"] = (string)facility.notes;
                dr["FacilityCurrentCommitment"] = (double?)facility.currentCommitment;
                dr["FacilityCurrentOutstanding"] = (double?)facility.currentOutstanding;
                dr["FacilityFirstPaymentDate"] = (DateTime?)facility.firstPayment == null ? "" : facility.firstPayment.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                dr["FacilityIsPositionClosed"] = (bool?)facility.isPositionClosed;
                dr["FacilityIsDefault"] = (bool?)facility.isDefault;
                dr["FacilitySpringingMaturityDate"] = facility.springingMaturityDate;
                dr["FacilityExpirationDate"] = facility.expirationDate;
                dr["FacilityDateDefaulted"] = facility.dateDefaulted;
                dr["FacilityIsDateConfirmed"] = facility.isDateConfirmed;
                dr["FacilityExitFee"] = facility.exitFee;
                dr["FacilityIsSubscribed"] = facility.isSubscribed;
                dr["FacilityAllowsFeeAccrual"] = facility.allowsFeeAccrual;
                dr["FacilityAllowsPIKAccrual"] = facility.allowsPIKAccrual;
                dr["FacilityTradesWithAccruedFee"] = facility.tradesWithAccruedFee;
                dr["FacilityTradesWithAccruedPIK"] = facility.tradesWithAccruedPIK;
                dr["FacilityIsCovLite"] = facility.isCovLite;
                dr["FacilityIsPIK"] = facility.isPIK;
                dr["FacilityCashPIKToggle"] = facility.cashPIKToggle;
                dr["FacilityCashPIKBD"] = facility.cashPIKBD;
                dr["FacilityIsABL"] = facility.isABL;
                dr["FacilityIsMulticurrency"] = facility.isMulticurrency;
                dr["FacilityIsRestructured"] = facility.isRestructured;
                dr["FacilityIsCapFloorVerified"] = facility.isCapFloorVerified;
                dr["FacilityStatedSpreadType"] = facility.statedSpreadType;
                dr["FacilityIsAssetQC"] = facility.isAssetQC;
                dr["FacilityAssetQCDate"] = facility.assetQCDate;
                dr["FacilityLaunchDate"] = facility.launchDate;
                dr["FacilityAssetEffectiveDate"] = facility.assetEffectiveDate;
                dr["FacilityReplacedDate"] = facility.replacedDate;
                dr["FacilityReplacedReason"] = facility.replacedReason;
                dr["Identifier"] = data[0];
                dr["Identifier Value"] = data[1];
                Facility.Rows.Add(dr);
                mLogger.Debug("end fill Facility data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetSecurityInfo(RMarkitSecurity[] SecurityData, string RequestInfo, DataTable Security)
        {
            try
            {
                mLogger.Debug("start fill GetSecurityInfo data");
                if (SecurityData != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    DataRow dr = Security.NewRow();
                    dr["Identifier"] = data[0];
                    dr["Identifier Value"] = data[1];
                    foreach (var sec in SecurityData)
                    {
                        if (Security.Columns.Contains(sec.type))
                            dr[sec.type] = sec.value;
                    }
                    Security.Rows.Add(dr);
                }
                mLogger.Debug("end fill GetSecurityInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetSpreadInfo(RMarkitSpread[] spread, string RequestInfo, DataTable Spread)
        {
            try
            {
                mLogger.Debug("start fill GetSpreadInfo data");
                if (spread != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var sp in spread)
                    {
                        DataRow dr = Spread.NewRow();
                        dr["SpreadFieldType"] = sp.type;
                        dr["SpreadFieldValue"] = sp.value;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        Spread.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetSpreadInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetSICInfo(RMarkitSIC[] sIC, string RequestInfo, DataTable SIC)
        {
            try
            {
                mLogger.Debug("start fill GetSpreadInfo data");
                if (sIC != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var sic in sIC)
                    {
                        DataRow dr = SIC.NewRow();
                        dr["SICLevelType"] = sic.type;
                        dr["SICLevelName"] = sic.name;
                        dr["SICLevelCode"] = sic.code;
                        dr["SICLevelStartDate"] = sic.startDate == null ? "" : sic.startDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        SIC.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetSpreadInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetIssuerInfo(RMarkitFacilityResponse issuer, string RequestInfo, DataTable Issuer)
        {
            try
            {
                mLogger.Debug("start fill GetIssuerInfo data");
                string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                DataRow dr = Issuer.NewRow();
                dr["IssuerDataID"] = (int?)issuer.wsoDataIssuerId;
                dr["IssuerID"] = (string)issuer.markitIssuerId;
                dr["IssuerName"] = (string)issuer.name;
                dr["IssuerAbbrevName"] = (string)issuer.abbrevName;
                dr["IssuerCountry"] = (string)issuer.country;
                dr["MoodyIssuerID"] = (string)issuer.moodyIssuerId;
                dr["SPIssuerID"] = (string)issuer.spIssuerId;
                dr["IssuerHasPublicEquity"] = (bool?)issuer.hasPublicEquity;
                dr["IssuerEquityID"] = (string)issuer.equityId;
                dr["IssuerNotes"] = (string)issuer.notes;
                dr["IssuerState"] = (string)issuer.state;
                dr["IssuerJurisdiction"] = (string)issuer.jurisdiction;
                dr["IssuerParentAffiliate"] = (string)issuer.parentAffiliate;
                dr["IssuerMarkitTicker"] = (string)issuer.markitTicker;
                dr["IssuerFitchId"] = (string)issuer.fitchId;
                dr["Identifier"] = data[0];
                dr["Identifier Value"] = data[1];
                Issuer.Rows.Add(dr);
                mLogger.Debug("end fill GetIssuerInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetRateOptionInfo(RMarkitRateOption[] rateOption, string RequestInfo, DataTable RateOption)
        {
            try
            {
                mLogger.Debug("start fill GetRateOptionInfo data");
                if (rateOption != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ro in rateOption)
                    {
                        DataRow dr = RateOption.NewRow();
                        dr["RateOptionBehavior"] = (string)ro.behavior;
                        dr["RateOptionDateOffset"] = (int)ro.offset;
                        dr["RateOptionEndDate"] = (DateTime?)ro.endDate == null ? "" : ro.endDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                        dr["RateOptionFirstPayDatetime"] = (DateTime)ro.firstPayDate == null ? "" : ro.firstPayDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                        dr["RateOptionFrequency"] = (string)ro.frequency;
                        dr["RateOptionMonthCount"] = (string)ro.monthCount;
                        dr["RateOptionName"] = (string)ro.name;
                        dr["RateOptionSpread"] = (float)ro.spread;
                        if (ro.startDate.HasValue)
                            dr["RateOptionStartDate"] = (DateTime)ro.startDate;
                        dr["RateOptionYearCount"] = (string)ro.yearCount;
                        dr["RateOptionIsObservationShift"] = ro.isObservationShift;
                        dr["RateOptionInterestMethod"] = ro.interestMethod;
                        dr["RateOptionCompoundMethod"] = ro.compoundMethod;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        RateOption.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetRateOptionInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetRateLimitInfo(RMarkitRateLimit[] rateLimit, string RequestInfo, DataTable RateLimit)
        {
            try
            {
                mLogger.Debug("start fill GetRateLimitInfo data");
                if (rateLimit != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ro in rateLimit)
                    {
                        DataRow dr = RateLimit.NewRow();
                        dr["RateLimitRateOption"] = (string)ro.rateOption;
                        dr["RateLimitLimitType"] = (string)ro.limitType;
                        dr["RateLimitRateType"] = (string)ro.rateType;
                        dr["RateLimitLimit"] = (float?)ro.limit;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        RateLimit.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetRateLimitInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetRatingInfo(RMarkitRating[] rating, string RequestInfo, DataTable Rating)
        {
            try
            {
                mLogger.Debug("start fill GetRatingInfo data");
                if (rating != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var rs in rating)
                    {
                        foreach (var ro in rs.types)
                        {
                            DataRow dr = Rating.NewRow();
                            dr["RatingSource"] = (string)rs.name;
                            dr["RatingType"] = (string)ro.name;
                            dr["RatingName"] = (string)ro.value;
                            dr["RatingStartDate"] = (DateTime?)ro.startDate == null ? "" : ro.startDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                            dr["Identifier"] = data[0];
                            dr["Identifier Value"] = data[1];
                            Rating.Rows.Add(dr);
                        }
                    }
                }
                mLogger.Debug("end fill GetRatingInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetSponsorInfo(string[] sponsor, string RequestInfo, DataTable Sponsor)
        {
            try
            {
                mLogger.Debug("start fill GetSponsorInfo data");
                if (sponsor != null)
                {
                    int sponsorOrder = 1;
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var rs in sponsor)
                    {
                        DataRow dr = Sponsor.NewRow();
                        dr["SponsorOrder"] = sponsorOrder++;
                        dr["SponsorName"] = rs;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        Sponsor.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetSponsorInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetCallSchedulePayInfo(RMarkitCallScheduleDetail callScheduleFeePay, string RequestInfo, DataTable CallSchedulePay)
        {
            try
            {
                mLogger.Debug("start fill GetCallSchedulePayInfo data");
                if (callScheduleFeePay != null && callScheduleFeePay.lines != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ro in callScheduleFeePay.lines)
                    {
                        DataRow dr = CallSchedulePay.NewRow();
                        dr["CallSchedulePayOrder"] = (int?)ro.Order;
                        dr["CallSchedulePayDate"] = (DateTime?)ro.date == null ? "" : ro.date.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                        dr["CallSchedulePayPrice"] = (decimal?)ro.price;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        CallSchedulePay.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetCallSchedulePayInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetMultiCurrencyInfo(RMarkitMultiCurrency[] multiCurrency, string RequestInfo, DataTable MultiCurrency)
        {
            try
            {
                mLogger.Debug("start fill GetMultiCurrencyInfo data");
                if (multiCurrency != null)
                {
                    string[] data = RequestInfo.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ro in multiCurrency)
                    {
                        DataRow dr = MultiCurrency.NewRow();
                        dr["MultiCurrencyCurrencyType"] = (string)ro.type;
                        dr["MultiCurrencyLimit"] = (float?)ro.limit;
                        dr["Identifier"] = data[0];
                        dr["Identifier Value"] = data[1];
                        MultiCurrency.Rows.Add(dr);
                    }
                }
                mLogger.Debug("end fill GetMultiCurrencyInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetContractInfo(ContractInfo contract, DataTable Contract, Dictionary<int, KeyValuePair<string, string>> DictFacilityIdVsIdentifierDetails)
        {
            try
            {
                mLogger.Debug("start fill GetContractInfo data");
                DataRow dr = Contract.NewRow();
                dr["ContractAccrualBasis"] = Convert.ToString(contract.ContractAccrualBasis);
                dr["ContractAllInRate"] = (float?)contract.ContractAllInRate;
                dr["ContractBaseRate"] = (float?)contract.ContractBaseRate;
                dr["ContractName"] = (string)contract.ContractName;
                dr["ContractType"] = (string)contract.ContractType;
                dr["ContractCurrencyType"] = (string)contract.ContractCurrencyType;
                dr["ContractExchangeRate"] = (decimal?)contract.ContractExchangeRate;
                dr["ContractFrequency"] = (int?)contract.ContractFrequency;
                dr["ContractGlobalAmount"] = (decimal?)contract.ContractGlobalAmount;
                dr["ContractInterestDue"] = (decimal?)contract.ContractInterestDue;
                dr["ContractInterestReceived"] = (decimal?)contract.ContractInterestReceived;
                dr["ContractMaturityDate"] = (DateTime?)contract.ContractMaturityDate == null ? "" : contract.ContractMaturityDate.Value.ToString("yyyyMMdd hh:mm:ss"); ;
                dr["ContractMonthCount"] = (string)contract.ContractMonthCount;
                dr["ContractNextPaymentDate"] = (DateTime?)contract.ContractNextPaymentDate == null ? "" : contract.ContractNextPaymentDate.Value.ToString("yyyyMMdd hh:mm:ss"); ; ;
                dr["ContractNotes"] = (string)contract.ContractNotes;
                dr["ContractRateOption"] = (string)contract.ContractRateOption;
                dr["ContractSpread"] = (float?)contract.ContractSpread;
                dr["ContractStartDate"] = (DateTime?)contract.ContractStartDate == null ? "" : contract.ContractStartDate.Value.ToString("yyyyMMdd hh:mm:ss"); ; ;
                dr["ContractID"] = (int?)contract.ContractID;
                dr["ContractYearCount"] = (string)contract.ContractYearCount;
                dr["ContractBehavior"] = contract.ContractBehavior;
                dr["ContractReceiveDate"] = contract.ContractReceiveDate;
                dr["ContractIsReceived"] = contract.ContractIsReceived;
                dr["ContractAccrualBasisId"] = contract.ContractAccrualBasisId;
                dr["ContractAccrualFeeType"] = contract.ContractAccrualFeeType;
                dr["ContractIsObservationShift"] = contract.ContractIsObservationShift;
                dr["ContractInterestMethod"] = contract.ContractInterestMethod;
                dr["ContractSpreadAdjustment"] = contract.ContractSpreadAdjustment;
                dr["ContractLookBackDayOffset"] = contract.ContractLookBackDayOffset;
                dr["ContractFacilityId"] = contract.ContractFacilityId;
                dr["ContractLoanX"] = contract.ContractLoanX;
                if (DictFacilityIdVsIdentifierDetails != null && DictFacilityIdVsIdentifierDetails.ContainsKey(contract.ContractFacilityId))
                {
                    dr["Identifier"] = DictFacilityIdVsIdentifierDetails[contract.ContractFacilityId].Key;
                    dr["Identifier Value"] = DictFacilityIdVsIdentifierDetails[contract.ContractFacilityId].Value;
                }
                else
                {
                    dr["Identifier"] = RMarketIdentifierType.WSODataFacilityID.ToString();
                    dr["Identifier Value"] = contract.ContractFacilityId.ToString();
                }
                Contract.Rows.Add(dr);
                mLogger.Debug("end fill GetContractInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetAmortizationDetailInfo(AmortizationScheduleInfo amor, DataTable Amortization, Dictionary<int, KeyValuePair<string, string>> DictFacilityIdVsIdentifierDetails)
        {
            try
            {
                mLogger.Debug("start fill GetAmortizationDetailInfo data");
                foreach (var amortize in amor.AmortizationInfo)
                {
                    DataRow dr = Amortization.NewRow();
                    dr["AmortizationAmount"] = (decimal?)amortize.AmortizationAmount;
                    dr["AmortizationDate"] = (DateTime?)amortize.AmortizationDate == null ? "" : amortize.AmortizationDate.Value.ToString("yyyyMMdd hh:mm:ss"); ; ;
                    dr["AmortizationNotes"] = (string)amortize.AmortizationNotes;
                    dr["AmortizationPrice"] = (decimal?)amortize.AmortizationPrice;
                    dr["AmortizationReceived"] = (bool?)amortize.AmortizationReceived;
                    if (DictFacilityIdVsIdentifierDetails != null && DictFacilityIdVsIdentifierDetails.ContainsKey(amortize.AmortizationFacilityId))
                    {
                        dr["Identifier"] = DictFacilityIdVsIdentifierDetails[amortize.AmortizationFacilityId].Key;
                        dr["Identifier Value"] = DictFacilityIdVsIdentifierDetails[amortize.AmortizationFacilityId].Value;
                    }
                    else
                    {
                        dr["Identifier"] = RMarketIdentifierType.WSODataFacilityID.ToString();
                        dr["Identifier Value"] = amortize.AmortizationFacilityId.ToString();
                    }
                    Amortization.Rows.Add(dr);
                }
                mLogger.Debug("end fill GetAmortizationDetailInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void GetAmortizationScheduleInfo(AmortizationScheduleInfo amor, DataTable AmortizationSchedule, Dictionary<int, KeyValuePair<string, string>> DictFacilityIdVsIdentifierDetails)
        {
            try
            {
                mLogger.Debug("start fill GetAmortizationScheduleInfo data");
                DataRow dr = AmortizationSchedule.NewRow();
                dr["AmortizationScheduleEndDate"] = (DateTime?)amor.AmortizationScheduleEndDate == null ? "" : amor.AmortizationScheduleEndDate.Value.ToString("yyyyMMdd hh:mm:ss"); ; ;
                dr["AmortizationScheduleDate"] = (DateTime?)amor.AmortizationScheduleDate == null ? "" : amor.AmortizationScheduleDate.Value.ToString("yyyyMMdd hh:mm:ss"); ; ;
                dr["AmortizationScheduleName"] = (string)amor.AmortizationScheduleName;
                dr["AmortizationScheduleID"] = (int?)amor.AmortizationScheduleID;
                dr["AmortizationScheduleDataID"] = (int?)amor.AmortizationScheduleDataID;
                if (DictFacilityIdVsIdentifierDetails != null && DictFacilityIdVsIdentifierDetails.ContainsKey(amor.AmortizationScheduleFacilityId))
                {
                    dr["Identifier"] = DictFacilityIdVsIdentifierDetails[amor.AmortizationScheduleFacilityId].Key;
                    dr["Identifier Value"] = DictFacilityIdVsIdentifierDetails[amor.AmortizationScheduleFacilityId].Value;
                }
                else
                {
                    dr["Identifier"] = RMarketIdentifierType.WSODataFacilityID.ToString();
                    dr["Identifier Value"] = amor.AmortizationScheduleFacilityId.ToString();
                }
                AmortizationSchedule.Rows.Add(dr);
                mLogger.Debug("end fill GetAmortizationScheduleInfo data");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        internal static void InsertDataInToDatabase(DataSet dataSet)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("InsertDataInToDatabase=> start InsertDataInToDatabase");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                DataTable facilityData = dataSet.Tables[RMarketRequestType.Facility.ToString()];
                facilityData.Columns.Remove("Identifier Value");
                facilityData.Columns.Remove("Identifier");
                mDBConn.ExecuteQuery("delete from dbo.ivp_rad_WSO_unsubscribed_facilities", RQueryType.Delete);
                mDBConn.ExecuteBulkCopy("ivp_rad_WSO_unsubscribed_facilities", facilityData);
                mLogger.Debug("InsertDataInToDatabase=> end InsertDataInToDatabase");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (mDBConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static void UpdateDatabase(string requestedData, string requestTypes, string user, string identifier)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("UpdateDatabase=> start UpdateDatabase");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                RHashlist list = new RHashlist();
                list.Add("requestedData", requestedData);
                list.Add("requestTypes", requestTypes);
                list.Add("user", user);
                list.Add("idetifier", identifier);
                mDBConn.ExecuteQuery("RAD:InsertMarkitRequestDetails", list);
                mLogger.Debug("UpdateDatabase=> end UpdateDatabase");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (mDBConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static void UpdateDatabase(string identifier, string status, string description)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("UpdateDatabase=> start UpdateDatabase");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                RHashlist list = new RHashlist();
                list.Add("identifier", identifier);
                list.Add("status", status);
                list.Add("description", description);
                mDBConn.ExecuteQuery("RAD:UpdateMarkitRequestDetails", list);
                mLogger.Debug("UpdateDatabase=> end UpdateDatabase");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (mDBConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static void CreateTransactionTableSchema(Type tranType, DataTable resultantData, string parentPropertyName = "")
        {
            try
            {
                mLogger.Debug("create transaction table structure");
                bool flag;
                if (tranType.FullName.Contains("Generic"))
                {
                    return;
                    //   tranType = tranType.GetProperties().Last<PropertyInfo>().PropertyType;
                }
                PropertyInfo[] properties = tranType.GetProperties();
                string headerName = string.Empty;
                properties.ToList<PropertyInfo>().ForEach(delegate (PropertyInfo property)
                {
                    if (property.GetCustomAttributes(false).Count<object>() > 0)
                    {
                        flag = ((CustomAttributes)property.GetCustomAttributes(false).GetValue(0)).HasInnerProperty;
                        headerName = ((CustomAttributes)property.GetCustomAttributes(false).GetValue(0)).HeaderName;
                        if (flag)
                        {
                            CreateTransactionTableSchema(property.PropertyType, resultantData, parentPropertyName + headerName + " ");
                        }
                        else
                        {
                            resultantData.Columns.Add(parentPropertyName + headerName);
                        }
                    }
                });
                mLogger.Debug("end create transaction table structure");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        private static object CopyArrayData(object source, Type destinationType)
        {
            if (source == null)
                return null;

            Type destinationChildType = destinationType.GetElementType();
            var destination = Array.CreateInstance(destinationChildType, 0);
            try
            {
                object[] sourceArr = source as object[];
                if (sourceArr != null && sourceArr.Count() > 0)
                {
                    List<object> lstDestinationChild = new List<object>();
                    foreach (var item in sourceArr)
                    {
                        object destinationChild = CopyData(item, destinationChildType);
                        if (destinationChild != null)
                            lstDestinationChild.Add(destinationChild);
                    }

                    if (lstDestinationChild.Count > 0)
                    {
                        destination = Array.CreateInstance(destinationChildType, lstDestinationChild.Count);

                        Array.Copy(lstDestinationChild.ToArray(), destination, lstDestinationChild.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return destination;
        }

        public static object CopyData(object source, Type destinationType)
        {
            try
            {
                if (source == null)
                    return null;

                Type sType = source.GetType();
                if (sType.IsArray)
                {
                    return CopyArrayData(source, destinationType);
                }
                else
                {
                    try
                    {
                        object destination = null;
                        try
                        {
                            if (!destinationType.Name.Equals("ExtensionDataObject", StringComparison.OrdinalIgnoreCase))
                                destination = Activator.CreateInstance(destinationType);
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }

                        PropertyInfo[] secinfo = sType.GetProperties();
                        foreach (PropertyInfo propinfo in secinfo)
                        {
                            PropertyInfo targetProperty = destinationType.GetProperty(propinfo.Name);
                            if (targetProperty != null)
                            {
                                if (((propinfo.PropertyType.IsPrimitive || propinfo.PropertyType.Name == "String") && propinfo.PropertyType.Name == targetProperty.PropertyType.Name) || (propinfo.PropertyType.Name == "Nullable`1" && propinfo.PropertyType.FullName == targetProperty.PropertyType.FullName))
                                    targetProperty.SetValue(destination, propinfo.GetValue(source, null), null);
                                else if (propinfo.PropertyType.IsClass || propinfo.PropertyType.IsArray)
                                {
                                    object sourceChild = propinfo.GetValue(source, null);

                                    if (sourceChild == null)
                                        continue;
                                    object destinationChild = CopyData(sourceChild, targetProperty.PropertyType);
                                    if (destinationChild != null)
                                        targetProperty.SetValue(destination, destinationChild, null);
                                }
                                else
                                    mLogger.Error("Property type not matched for ==> targetProperty.PropertyType.FullName -> " + targetProperty.PropertyType.FullName + ", propinfo.PropertyType.FullName -> " + propinfo.PropertyType.FullName + ", propinfo.Name -> " + propinfo.Name + ", DestinationType -> " + destinationType.ToString() + ", SourceType -> " + sType.ToString());
                            }
                        }
                        return destination;
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error(ex.ToString());
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }
    }
}
