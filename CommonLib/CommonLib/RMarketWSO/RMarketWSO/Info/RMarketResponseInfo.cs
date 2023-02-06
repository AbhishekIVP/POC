using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.ivp.rad.RMarketWSO.FacilitySearchService;

namespace com.ivp.rad.RMarketWSO
{
    public class RMarketResponseInfo
    {
        public RMarketResponseInfo()
        {
            FacilityData = new DataSet();
        }
        public DataSet FacilityData { get; set; }

        public string Message { get; set; }
        public RStatus Status { get; set; }
    }

    public class RMarketRequestInfo
    {
        public RMarketRequestInfo()
        {
            SecurityDetails = new Dictionary<string, RMarketIdentifierType>();
            RequestType = new List<RMarketRequestType>();
            TranDetails = new List<TransactionDetails>();
            User = "";
        }

        public Dictionary<string, RMarketIdentifierType> SecurityDetails { get; set; }
        public List<RMarketRequestType> RequestType { get; set; }
        public bool IsImmediate { get; set; }
        public string CachedFileName { get; set; }
        public string User { get; set; }
        internal string requestIdentifier { get; set; }
        public List<TransactionDetails> TranDetails { get; set; }
        public int VendorPreferenceId { get; set; }
    }

    public class TransactionDetails
    {
        public int facilityId { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    internal class CustomAttributes : Attribute
    {
        public string HeaderName { get; set; }
        public int Tag { get; set; }
        public bool HasInnerProperty { get; set; }
    }

    public enum RMarketIdentifierType
    {
        WSODataFacilityID,
        ISIN,
        CUSIP,
        LoanX,
        LIN
    }

    public enum RStatus
    {
        PASSED,
        FAILED,
        PROCESSED
    }

    public enum RMarketRequestType
    {
        Facility,
        AgentBank,
        BankDeal,
        Amortization,
        AmortizationSchedule,
        Issuer,
        RateOption,
        Transaction,
        BankDealSponser,
        CallSchedulePay,
        Contract,
        MultiCurrency,
        SecurityID,
        Spread,
        SIC,
        RateLimit,
        Rating,
        Sponsor
    }

    public class RMarkitFacilityResponse
    {
        public RMarkitBankDeal bankDeal { get; set; }
        public System.Nullable<int> wsoDataIssuerId { get; set; }
        public string markitIssuerId { get; set; }
        public string moodyIssuerId { get; set; }
        public string spIssuerId { get; set; }
        public string equityId { get; set; }
        public string name { get; set; }
        public string abbrevName { get; set; }
        public string country { get; set; }
        public System.Nullable<bool> hasPublicEquity { get; set; }

        //New fields
        public string notes { get; set; }
        public string state { get; set; }
        public string jurisdiction { get; set; }
        public string parentAffiliate { get; set; }
        public string markitTicker { get; set; }
        public RMarkitMarkitTier[] markitTiers { get; set; }
        public string fitchId { get; set; }
    }

    public class RMarkitMarkitTier
    {
        public int index { get; set; }
        public string tier { get; set; }
        public int code { get; set; }
    }

    public class RMarkitBankDeal
    {
        public RMarkitFacility facility { get; set; }
        public RMarkitAgentBank agentBank { get; set; }
        public string[] sponsor { get; set; }
        public string markitBankDealId { get; set; }
        public System.Nullable<int> wsoDataBankDealId { get; set; }
        public string name { get; set; }
        public string streetName { get; set; }
        public string underwriter { get; set; }
        public string prepaymentNotes { get; set; }
        public System.Nullable<decimal> globalAmount { get; set; }
        public System.Nullable<decimal> minAssignment { get; set; }
        public System.Nullable<System.DateTime> creditDate { get; set; }
        public System.Nullable<bool> isCovLite { get; set; }
        public string currency { get; set; }
        public string defaultNotes { get; set; }
        public System.Nullable<bool> agentConsent { get; set; }
        public System.Nullable<bool> borrowerConsent { get; set; }
        public System.Nullable<decimal> assignmentFee { get; set; }
        public System.Nullable<decimal> minimumHold { get; set; }
        public string publicLoan { get; set; }
        public System.Nullable<bool> canAffiliate { get; set; }

        //New fields
        public string description { get; set; }
        public System.Nullable<bool> isCAReceived { get; set; }
        public string publicSource { get; set; }
        public string faxAgent { get; set; }
        public string faxAgentContact { get; set; }

        //not added
        //identifiers
        //syndicationAgent
    }

    public class RMarkitFacility
    {
        public RMarkitSecurity[] identifiers { get; set; }
        public RMarkitSpread[] spreads { get; set; }
        public RMarkitMultiCurrency[] currencyTypes { get; set; }
        public RMarkitRateOption[] rateOptions { get; set; }
        public RMarkitRateLimit[] rateLimits { get; set; }
        public RMarkitCallScheduleDetail callSchedule { get; set; }
        public RMarkitRating[] Ratings { get; set; }
        public RMarkitSIC[] SICs { get; set; }
        public System.Nullable<int> wsoDataFacilityId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string notes { get; set; }
        public string guarantor { get; set; }
        public string purpose { get; set; }
        public System.Nullable<System.DateTime> maturityDate { get; set; }
        public System.Nullable<double> originalCommitment { get; set; }
        public System.Nullable<double> currentCommitment { get; set; }
        public System.Nullable<double> currentOutstanding { get; set; }
        public System.Nullable<System.DateTime> firstPayment { get; set; }
        public string currency { get; set; }
        public System.Nullable<bool> isSynthetic { get; set; }
        public System.Nullable<bool> isDelayedDraw { get; set; }
        public System.Nullable<bool> isDip { get; set; }
        public System.Nullable<bool> isGuaranteed { get; set; }
        public System.Nullable<bool> isLcCreditLinked { get; set; }
        public System.Nullable<bool> isPositionClosed { get; set; }
        public System.Nullable<bool> isFixed { get; set; }
        public System.Nullable<bool> tradesWithAccrued { get; set; }
        public string abbrevName { get; set; }
        public System.Nullable<bool> hasLcSublimit { get; set; }
        public System.Nullable<decimal> lcSublimit { get; set; }
        public string seniority { get; set; }
        public string lienType { get; set; }
        public System.Nullable<decimal> issuePrice { get; set; }
        public string paymentFrequency { get; set; }
        public System.Nullable<float> statedSpread { get; set; }
        public System.Nullable<decimal> minAssignment { get; set; }
        public System.Nullable<decimal> minHold { get; set; }
        public string classification { get; set; }
        public System.Nullable<bool> issuingBankConsent { get; set; }
        public string collateral { get; set; }
        public System.Nullable<bool> isDefault { get; set; }
        public System.Nullable<bool> activityTracked { get; set; }
        public System.Nullable<bool> caVerified { get; set; }
        public System.Nullable<System.DateTime> lastUpdated { get; set; }

        //New fields
        public System.Nullable<System.DateTime> springingMaturityDate { get; set; }
        public System.Nullable<System.DateTime> expirationDate { get; set; }
        public System.Nullable<System.DateTime> dateDefaulted { get; set; }
        public System.Nullable<bool> isDateConfirmed { get; set; }
        public System.Nullable<decimal> exitFee { get; set; }
        public System.Nullable<bool> isSubscribed { get; set; }
        public System.Nullable<bool> allowsFeeAccrual { get; set; }
        public System.Nullable<bool> allowsPIKAccrual { get; set; }
        public System.Nullable<bool> tradesWithAccruedFee { get; set; }
        public System.Nullable<bool> tradesWithAccruedPIK { get; set; }
        public string isCovLite { get; set; }
        public System.Nullable<bool> isPIK { get; set; }
        public System.Nullable<bool> cashPIKToggle { get; set; }
        public System.Nullable<int> cashPIKBD { get; set; }
        public System.Nullable<bool> isABL { get; set; }
        public System.Nullable<bool> isMulticurrency { get; set; }
        public System.Nullable<bool> isRestructured { get; set; }
        public System.Nullable<bool> isCapFloorVerified { get; set; }
        public string statedSpreadType { get; set; }
        public System.Nullable<bool> isAssetQC { get; set; }
        public System.Nullable<System.DateTime> assetQCDate { get; set; }
        public System.Nullable<System.DateTime> launchDate { get; set; }
        public System.Nullable<System.DateTime> assetEffectiveDate { get; set; }
        public System.Nullable<System.DateTime> replacedDate { get; set; }
        public string replacedReason { get; set; }

        //not added
        //paymentFrequencyId
    }

    public class RMarkitSecurity
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class RMarkitRateOption
    {
        public string name { get; set; }
        public string behavior { get; set; }
        public System.Nullable<float> spread { get; set; }
        public string frequency { get; set; }
        public System.Nullable<int> offset { get; set; }
        public System.Nullable<System.DateTime> startDate { get; set; }
        public System.Nullable<System.DateTime> endDate { get; set; }
        public System.Nullable<System.DateTime> firstPayDate { get; set; }
        public string monthCount { get; set; }
        public string yearCount { get; set; }

        //New fields
        public System.Nullable<bool> isObservationShift { get; set; }
        public string interestMethod { get; set; }
        public string compoundMethod { get; set; }
    }

    public class RMarkitRateLimit
    {
        public string rateOption { get; set; }
        public string limitType { get; set; }
        public string rateType { get; set; }
        public System.Nullable<float> limit { get; set; }
    }

    public class RMarkitMultiCurrency
    {
        public string type { get; set; }
        public System.Nullable<float> limit { get; set; }
    }

    public class RMarkitRating
    {
        public System.Nullable<int> WSODataFacilityID { get; set; }
        public string name { get; set; }
        public RMarkitRatingRow[] types { get; set; }

        //New fields
        public System.Nullable<int> id { get; set; }
    }

    public class RMarkitRatingRow
    {
        public System.Nullable<int> id { get; set; }
        public string name { get; set; }
        public bool isProvisional { get; set; }
        public string value { get; set; }
        public System.Nullable<System.DateTime> startDate { get; set; }
    }

    public class RMarkitSIC
    {
        public System.Nullable<int> id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public System.Nullable<System.DateTime> startDate { get; set; }
    }

    public class RMarkitSpread
    {
        public string type { get; set; }
        public System.Nullable<float> value { get; set; }
    }

    public class RMarkitCallScheduleDetail
    {
        public RMarkitDatePeriod hardCall { get; set; }
        public RMarkitDatePeriod makeWhole { get; set; }
        public RMarkitCallSchedule[] lines { get; set; }
    }

    public class RMarkitDatePeriod
    {
        public System.Nullable<System.DateTime> start { get; set; }
        public System.Nullable<System.DateTime> end { get; set; }
    }

    public class RMarkitCallSchedule
    {
        public System.Nullable<int> Order { get; set; }
        public System.Nullable<System.DateTime> date { get; set; }
        public System.Nullable<decimal> price { get; set; }
    }

    public class RMarkitAgentBank
    {
        public string markitAgentBankId { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }


    public class RMarkitAmortizationSchedule
    {
        public System.Nullable<int> wsoAmortizationID { get; set; }
        public System.Nullable<int> globalAmortizationID { get; set; }
        public string name { get; set; }
        public System.Nullable<System.DateTime> scheduleDate { get; set; }
        public System.Nullable<System.DateTime> endDate { get; set; }
        public RMarkitAmortization[] payments { get; set; }
    }

    public class RMarkitAmortization
    {
        public System.Nullable<System.DateTime> date { get; set; }
        public System.Nullable<decimal> amount { get; set; }
        public System.Nullable<bool> isReceived { get; set; }
        public System.Nullable<decimal> price { get; set; }
        public string notes { get; set; }
    }


    public class RMarkitContract
    {
        public System.Nullable<int> wsoDataContractId { get; set; }
        public string contractName { get; set; }
        public string contractType { get; set; }
        public string rateOption { get; set; }
        public System.Nullable<decimal> globalAmount { get; set; }
        public System.Nullable<float> allInRate { get; set; }
        public System.Nullable<float> baseRate { get; set; }
        public System.Nullable<float> spread { get; set; }
        public System.Nullable<System.DateTime> startDate { get; set; }
        public System.Nullable<System.DateTime> nextPaymentDate { get; set; }
        public System.Nullable<System.DateTime> maturityDate { get; set; }
        public System.Nullable<decimal> interestDue { get; set; }
        public System.Nullable<decimal> interestReceived { get; set; }
        public string notes { get; set; }
        public string currencyType { get; set; }
        public System.Nullable<decimal> exchangeRate { get; set; }
        public System.Nullable<int> frequency { get; set; }
        public string accrualBasis { get; set; }
        public string monthCount { get; set; }
        public string yearCount { get; set; }

        //New fields
        public string behavior { get; set; }
        public System.Nullable<System.DateTime> receiveDate { get; set; }
        public System.Nullable<bool> isReceived { get; set; }
        public System.Nullable<int> accrualBasisId { get; set; }
        public string accrualFeeType { get; set; }
        public System.Nullable<bool> isObservationShift { get; set; }
        public string interestMethod { get; set; }
        public System.Nullable<float> spreadAdjustment { get; set; }
        public System.Nullable<int> lookBackDayOffset { get; set; }
    }

    public static class RMarkitResponseParser
    {
        public static RMarkitFacilityResponse GetFacilityResponseFromSOAP(FacilitySearchResults results)
        {
            RMarkitFacilityResponse markitResponse = new RMarkitFacilityResponse();

            if (results != null && results.Issuer != null)
            {
                markitResponse.wsoDataIssuerId = results.Issuer.WSODataIssuerID;
                markitResponse.markitIssuerId = results.Issuer.MarkitIssuerID;
                markitResponse.moodyIssuerId = results.Issuer.MoodyIssuerID;
                markitResponse.spIssuerId = results.Issuer.SPIssuerID;
                markitResponse.equityId = results.Issuer.EquityID;
                markitResponse.name = results.Issuer.Name;
                markitResponse.abbrevName = results.Issuer.AbbrevName;
                markitResponse.country = results.Issuer.Country;
                markitResponse.hasPublicEquity = results.Issuer.HasPublicEquity;

                if (results.Issuer.BankDeal != null)
                {
                    markitResponse.bankDeal = new RMarkitBankDeal();

                    markitResponse.bankDeal.markitBankDealId = results.Issuer.BankDeal.MarkitBankDealID;
                    markitResponse.bankDeal.wsoDataBankDealId = results.Issuer.BankDeal.WSODataBankDealID;
                    markitResponse.bankDeal.name = results.Issuer.BankDeal.Name;
                    markitResponse.bankDeal.streetName = results.Issuer.BankDeal.StreetName;
                    markitResponse.bankDeal.underwriter = results.Issuer.BankDeal.Underwriter;
                    markitResponse.bankDeal.prepaymentNotes = results.Issuer.BankDeal.PrepaymentNotes;
                    markitResponse.bankDeal.globalAmount = results.Issuer.BankDeal.GlobalAmount;
                    markitResponse.bankDeal.minAssignment = results.Issuer.BankDeal.MinAssignment;
                    markitResponse.bankDeal.creditDate = results.Issuer.BankDeal.CreditDate;
                    markitResponse.bankDeal.isCovLite = results.Issuer.BankDeal.IsCovLite;
                    markitResponse.bankDeal.currency = results.Issuer.BankDeal.CurrencyType;
                    markitResponse.bankDeal.defaultNotes = results.Issuer.BankDeal.DefaultNotes;
                    markitResponse.bankDeal.agentConsent = results.Issuer.BankDeal.AgentConsent;
                    markitResponse.bankDeal.borrowerConsent = results.Issuer.BankDeal.BorrowerConsent;
                    markitResponse.bankDeal.assignmentFee = results.Issuer.BankDeal.AssignmentFee;
                    markitResponse.bankDeal.minimumHold = results.Issuer.BankDeal.MinHold;
                    markitResponse.bankDeal.publicLoan = results.Issuer.BankDeal.PublicLoan;
                    markitResponse.bankDeal.canAffiliate = results.Issuer.BankDeal.CanAffiliate;

                    if (results.Issuer.BankDeal.BankDealSponsor != null)
                    {
                        markitResponse.bankDeal.sponsor = new List<string> { results.Issuer.BankDeal.BankDealSponsor.PrimarySponsor, results.Issuer.BankDeal.BankDealSponsor.Sponsor2, results.Issuer.BankDeal.BankDealSponsor.Sponsor3 }.ToArray();
                    }

                    if (results.Issuer.BankDeal.Facility != null)
                    {
                        markitResponse.bankDeal.facility = new RMarkitFacility();

                        markitResponse.bankDeal.facility.wsoDataFacilityId = results.Issuer.BankDeal.Facility.WSODataFacilityID;
                        markitResponse.bankDeal.facility.name = results.Issuer.BankDeal.Facility.Name;
                        markitResponse.bankDeal.facility.type = results.Issuer.BankDeal.Facility.Type;
                        markitResponse.bankDeal.facility.country = results.Issuer.BankDeal.Facility.Country;
                        markitResponse.bankDeal.facility.countryCode = results.Issuer.BankDeal.Facility.CountryCode;
                        markitResponse.bankDeal.facility.notes = results.Issuer.BankDeal.Facility.Notes;
                        markitResponse.bankDeal.facility.guarantor = results.Issuer.BankDeal.Facility.Guarantor;
                        markitResponse.bankDeal.facility.purpose = results.Issuer.BankDeal.Facility.Purpose;
                        markitResponse.bankDeal.facility.maturityDate = results.Issuer.BankDeal.Facility.MaturityDate;
                        markitResponse.bankDeal.facility.originalCommitment = results.Issuer.BankDeal.Facility.OriginalCommitment;
                        markitResponse.bankDeal.facility.currentCommitment = results.Issuer.BankDeal.Facility.CurrentCommitment;
                        markitResponse.bankDeal.facility.currentOutstanding = results.Issuer.BankDeal.Facility.CurrentOutstanding;
                        markitResponse.bankDeal.facility.firstPayment = results.Issuer.BankDeal.Facility.FirstPaymentDate;
                        markitResponse.bankDeal.facility.currency = results.Issuer.BankDeal.Facility.CurrencyType;
                        markitResponse.bankDeal.facility.isSynthetic = results.Issuer.BankDeal.Facility.IsSynthetic;
                        markitResponse.bankDeal.facility.isDelayedDraw = results.Issuer.BankDeal.Facility.IsDelayedDraw;
                        markitResponse.bankDeal.facility.isDip = results.Issuer.BankDeal.Facility.IsDIP;
                        markitResponse.bankDeal.facility.isGuaranteed = results.Issuer.BankDeal.Facility.IsGuaranteed;
                        markitResponse.bankDeal.facility.isLcCreditLinked = results.Issuer.BankDeal.Facility.IsLCCreditLinked;
                        markitResponse.bankDeal.facility.isPositionClosed = results.Issuer.BankDeal.Facility.IsPositionClosed;
                        markitResponse.bankDeal.facility.isFixed = results.Issuer.BankDeal.Facility.IsFixed;
                        markitResponse.bankDeal.facility.tradesWithAccrued = results.Issuer.BankDeal.Facility.TradesWithAccrued;
                        markitResponse.bankDeal.facility.abbrevName = results.Issuer.BankDeal.Facility.AbbrevName;
                        markitResponse.bankDeal.facility.hasLcSublimit = results.Issuer.BankDeal.Facility.HasLCSublimit;
                        markitResponse.bankDeal.facility.lcSublimit = results.Issuer.BankDeal.Facility.LCSublimit;
                        markitResponse.bankDeal.facility.seniority = results.Issuer.BankDeal.Facility.Seniority;
                        markitResponse.bankDeal.facility.lienType = results.Issuer.BankDeal.Facility.LienType;
                        markitResponse.bankDeal.facility.issuePrice = results.Issuer.BankDeal.Facility.IssuePrice;
                        markitResponse.bankDeal.facility.paymentFrequency = results.Issuer.BankDeal.Facility.PaymentFrequency;
                        markitResponse.bankDeal.facility.statedSpread = results.Issuer.BankDeal.Facility.StatedxIBORSpread;
                        markitResponse.bankDeal.facility.minAssignment = results.Issuer.BankDeal.Facility.MinAssignment;
                        markitResponse.bankDeal.facility.minHold = results.Issuer.BankDeal.Facility.MinHold;
                        markitResponse.bankDeal.facility.classification = results.Issuer.BankDeal.Facility.Classification;
                        markitResponse.bankDeal.facility.issuingBankConsent = results.Issuer.BankDeal.Facility.IssuingBankConsent;
                        markitResponse.bankDeal.facility.collateral = results.Issuer.BankDeal.Facility.Collateral;
                        markitResponse.bankDeal.facility.isDefault = results.Issuer.BankDeal.Facility.IsDefault;
                        markitResponse.bankDeal.facility.activityTracked = results.Issuer.BankDeal.Facility.ActivityTracked;
                        markitResponse.bankDeal.facility.caVerified = results.Issuer.BankDeal.Facility.CAVerified;
                        markitResponse.bankDeal.facility.lastUpdated = results.Issuer.BankDeal.Facility.LastUpdate;

                        if (results.Issuer.BankDeal.Facility.Securities != null && results.Issuer.BankDeal.Facility.Securities.Count() > 0)
                        {
                            List<RMarkitSecurity> identifiers = new List<RMarkitSecurity>();

                            foreach (var item in results.Issuer.BankDeal.Facility.Securities)
                            {
                                RMarkitSecurity newItem = new RMarkitSecurity();
                                newItem.type = item.Type;
                                newItem.value = item.Value;
                                identifiers.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.identifiers = identifiers.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.Spreads != null && results.Issuer.BankDeal.Facility.Spreads.Count() > 0)
                        {
                            List<RMarkitSpread> spreads = new List<RMarkitSpread>();

                            foreach (var item in results.Issuer.BankDeal.Facility.Spreads)
                            {
                                RMarkitSpread newItem = new RMarkitSpread();
                                newItem.type = item.Type;
                                newItem.value = item.Value;
                                spreads.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.spreads = spreads.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.CurrencyType != null && results.Issuer.BankDeal.Facility.CurrencyType.Count() > 0)
                        {
                            List<RMarkitMultiCurrency> currencyType = new List<RMarkitMultiCurrency>();

                            foreach (var item in results.Issuer.BankDeal.Facility.MultiCurrencies)
                            {
                                RMarkitMultiCurrency newItem = new RMarkitMultiCurrency();
                                newItem.type = item.CurrencyType;
                                newItem.limit = item.Limit;
                                currencyType.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.currencyTypes = currencyType.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.RateOptions != null && results.Issuer.BankDeal.Facility.RateOptions.Count() > 0)
                        {
                            List<RMarkitRateOption> rateOption = new List<RMarkitRateOption>();

                            foreach (var item in results.Issuer.BankDeal.Facility.RateOptions)
                            {
                                RMarkitRateOption newItem = new RMarkitRateOption();
                                newItem.name = item.Name;
                                newItem.behavior = item.Behavior;
                                newItem.spread = item.Spread;
                                newItem.frequency = item.Frequency;
                                newItem.offset = item.DateOffset;
                                newItem.startDate = item.StartDate;
                                newItem.endDate = item.EndDate;
                                newItem.firstPayDate = item.FirstPayDate;
                                newItem.monthCount = item.MonthCount;
                                newItem.yearCount = item.YearCount;
                                rateOption.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.rateOptions = rateOption.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.RateLimits != null && results.Issuer.BankDeal.Facility.RateLimits.Count() > 0)
                        {
                            List<RMarkitRateLimit> rateLimit = new List<RMarkitRateLimit>();

                            foreach (var item in results.Issuer.BankDeal.Facility.RateLimits)
                            {
                                RMarkitRateLimit newItem = new RMarkitRateLimit();
                                newItem.rateOption = item.RateOption;
                                newItem.limitType = item.LimitType;
                                newItem.rateType = item.RateType;
                                newItem.limit = item.Limit;
                                rateLimit.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.rateLimits = rateLimit.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.CallScheduleDetail != null)
                        {
                            markitResponse.bankDeal.facility.callSchedule = new RMarkitCallScheduleDetail();

                            markitResponse.bankDeal.facility.callSchedule.hardCall = new RMarkitDatePeriod()
                            {
                                start = results.Issuer.BankDeal.Facility.CallScheduleDetail.HardCallStart,
                                end = results.Issuer.BankDeal.Facility.CallScheduleDetail.HardCallEnd
                            };
                            markitResponse.bankDeal.facility.callSchedule.makeWhole = new RMarkitDatePeriod()
                            {
                                start = results.Issuer.BankDeal.Facility.CallScheduleDetail.MakeWholeStart,
                                end = results.Issuer.BankDeal.Facility.CallScheduleDetail.MakeWholeEnd
                            };

                            if (results.Issuer.BankDeal.Facility.CallScheduleDetail.CallSchedules != null && results.Issuer.BankDeal.Facility.CallScheduleDetail.CallSchedules.Count() > 0)
                            {
                                List<RMarkitCallSchedule> callSchedule = new List<RMarkitCallSchedule>();

                                foreach (var item in results.Issuer.BankDeal.Facility.CallScheduleDetail.CallSchedules)
                                {
                                    RMarkitCallSchedule newItem = new RMarkitCallSchedule();
                                    newItem.Order = item.Order;
                                    newItem.date = item.Date;
                                    newItem.price = item.Price;
                                    callSchedule.Add(newItem);
                                }

                                markitResponse.bankDeal.facility.callSchedule.lines = callSchedule.ToArray();
                            }
                        }
                        if (results.Issuer.BankDeal.Facility.Ratings != null && results.Issuer.BankDeal.Facility.Ratings.Count() > 0)
                        {
                            List<RMarkitRating> ratingSource = new List<RMarkitRating>();

                            foreach (var itemSource in results.Issuer.BankDeal.Facility.Ratings.GroupBy(x => x.Source))
                            {
                                RMarkitRating newItem = new RMarkitRating();
                                newItem.WSODataFacilityID = results.Issuer.BankDeal.Facility.WSODataFacilityID;
                                newItem.name = itemSource.Key;

                                List<RMarkitRatingRow> rating = new List<RMarkitRatingRow>();

                                foreach (var item in itemSource)
                                {
                                    RMarkitRatingRow newItemRating = new RMarkitRatingRow();
                                    newItemRating.id = item.ID;
                                    newItemRating.name = item.Type;
                                    newItemRating.isProvisional = item.IsProvisional;
                                    newItemRating.value = item.Name;
                                    newItemRating.startDate = item.StartDate;
                                    rating.Add(newItemRating);
                                }

                                newItem.types = rating.ToArray();

                                ratingSource.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.Ratings = ratingSource.ToArray();
                        }
                        if (results.Issuer.BankDeal.Facility.SICs != null && results.Issuer.BankDeal.Facility.SICs.Count() > 0)
                        {
                            List<RMarkitSIC> SIC = new List<RMarkitSIC>();

                            foreach (var item in results.Issuer.BankDeal.Facility.SICs)
                            {
                                RMarkitSIC newItem = new RMarkitSIC();
                                newItem.id = item.ID;
                                newItem.type = item.Type;
                                newItem.name = item.Name;
                                newItem.code = item.Code;
                                newItem.startDate = item.StartDate;
                                SIC.Add(newItem);
                            }

                            markitResponse.bankDeal.facility.SICs = SIC.ToArray();
                        }
                    }

                    if (results.Issuer.BankDeal.AgentBankDetails != null)
                    {
                        markitResponse.bankDeal.agentBank = new RMarkitAgentBank();

                        markitResponse.bankDeal.agentBank.markitAgentBankId = results.Issuer.BankDeal.AgentBankDetails.MarkitAgentBankID;
                        markitResponse.bankDeal.agentBank.name = results.Issuer.BankDeal.AgentBankDetails.AgentBankMember;
                        markitResponse.bankDeal.agentBank.contact = results.Issuer.BankDeal.AgentBankDetails.AgentContact;
                        markitResponse.bankDeal.agentBank.phone = results.Issuer.BankDeal.AgentBankDetails.AgentPhone;
                        markitResponse.bankDeal.agentBank.email = results.Issuer.BankDeal.AgentBankDetails.AgentEmail;
                    }
                }
            }

            return markitResponse;
        }

        public static RMarkitAmortizationSchedule GetAmortizationResponseFromSOAP(AmortizationSchedule results)
        {
            RMarkitAmortizationSchedule markitResponse = new RMarkitAmortizationSchedule();

            if (results != null)
            {
                markitResponse.wsoAmortizationID = results.WSODataAmortizationId;
                markitResponse.globalAmortizationID = results.WSOAmortizationId;
                markitResponse.name = results.Name;
                markitResponse.scheduleDate = results.ScheduleDate;
                markitResponse.endDate = results.EndDate;

                if (results.Amortizations != null && results.Amortizations.Count() > 0)
                {
                    List<RMarkitAmortization> amortization = new List<RMarkitAmortization>();

                    foreach (var item in results.Amortizations)
                    {
                        RMarkitAmortization newItem = new RMarkitAmortization();
                        newItem.date = item.Date;
                        newItem.amount = item.Amount;
                        newItem.isReceived = item.Received;
                        newItem.price = item.Price;
                        newItem.notes = item.Notes;
                        amortization.Add(newItem);
                    }

                    markitResponse.payments = amortization.ToArray();
                }
            }

            return markitResponse;
        }
    }
}
