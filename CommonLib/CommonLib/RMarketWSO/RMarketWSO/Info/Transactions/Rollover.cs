using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    internal class Rollover
    {
        [CustomAttributes(HeaderName = "globtranid", HasInnerProperty = false)]
        public string GlobTranId { get; set; }
        [CustomAttributes(HeaderName = "messagetype", HasInnerProperty = false)]
        public string MessageType { get; set; }
        [CustomAttributes(HeaderName = "effdt", HasInnerProperty = false)]
        public DateTime EffectiveDate { get; set; }
        [CustomAttributes(HeaderName = "notes", HasInnerProperty = false)]
        public string Notes { get; set; }
        [CustomAttributes(HeaderName = "longdescription", HasInnerProperty = false)]
        public string LongDescription { get; set; }
        [CustomAttributes(HeaderName = "shortdescription", HasInnerProperty = false)]
        public string ShortDescription { get; set; }
        [CustomAttributes(HeaderName = "trangroupid", HasInnerProperty = false)]
        public int TransactionGroupID { get; set; }

        [CustomAttributes(HeaderName = "RolloverData", HasInnerProperty = true)]
        public RolloverData RolloverData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
        [CustomAttributes(HeaderName = "ContractExisting", HasInnerProperty = true)]
        public ContractExisting ContractExisting { get; set; }
        [CustomAttributes(HeaderName = "ContractNew", HasInnerProperty = true)]
        public ContractNew ContractNew { get; set; }
        [CustomAttributes(HeaderName = "Capitalization", HasInnerProperty = true)]
        public Capitalization Capitalization { get; set; }
    }
    
    class RolloverData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
    }

    class Issuer
    {
        [CustomAttributes(HeaderName = "name", HasInnerProperty = false)]
        public string Name { get; set; }
        [CustomAttributes(HeaderName = "globissuerid", HasInnerProperty = false)]
        public string GlobalIssuerId { get; set; }
    }
    
    internal class Facility
    {
        [CustomAttributes(HeaderName = "name", HasInnerProperty = false)]
        public string Name { get; set; }
        [CustomAttributes(HeaderName = "globfacilityid", HasInnerProperty = false)]
        public string GlobalFacilityId { get; set; }
        [CustomAttributes(HeaderName = "cusip", HasInnerProperty = false)]
        public string Cusip { get; set; }
        [CustomAttributes(HeaderName = "lin", HasInnerProperty = false)]
        public string LIN { get; set; }
        [CustomAttributes(HeaderName = "isin", HasInnerProperty = false)]
        public string Isin { get; set; }
        [CustomAttributes(HeaderName = "loanx", HasInnerProperty = false)]
        public string Loanx { get; set; }
    }
    
    internal class ContractExisting
    {
        [CustomAttributes(HeaderName = "globalcontractid", HasInnerProperty = false)]
        public int GlobalContractID { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "contracttypeid", HasInnerProperty = false)]
        public int ContractTypeID { get; set; }
        [CustomAttributes(HeaderName = "contracttype", HasInnerProperty = false)]
        public string ContractType { get; set; }
        [CustomAttributes(HeaderName = "currtypeid", HasInnerProperty = false)]
        public int CurrencyTypeID { get; set; }
        [CustomAttributes(HeaderName = "currtype", HasInnerProperty = false)]
        public string CurrencyType { get; set; }
        [CustomAttributes(HeaderName = "fxrate", HasInnerProperty = false)]
        public double FXRate { get; set; }
        [CustomAttributes(HeaderName = "allinrate", HasInnerProperty = false)]
        public double AllInRate { get; set; }
        [CustomAttributes(HeaderName = "baserate", HasInnerProperty = false)]
        public double BaseRate { get; set; }
        [CustomAttributes(HeaderName = "spread", HasInnerProperty = false)]
        public double Spread { get; set; }
        [CustomAttributes(HeaderName = "rateoption", HasInnerProperty = false)]
        public string RateOption { get; set; }
        [CustomAttributes(HeaderName = "behaviorid", HasInnerProperty = false)]
        public int BehaviorId { get; set; }
        [CustomAttributes(HeaderName = "behavior", HasInnerProperty = false)]
        public string Behavior { get; set; }
        [CustomAttributes(HeaderName = "monthcntid", HasInnerProperty = false)]
        public int MonthCountID { get; set; }
        [CustomAttributes(HeaderName = "monthcnt", HasInnerProperty = false)]
        public string MonthCount { get; set; }
        [CustomAttributes(HeaderName = "yearcntid", HasInnerProperty = false)]
        public int YearCountID { get; set; }
        [CustomAttributes(HeaderName = "yearcnt", HasInnerProperty = false)]
        public string YearCount { get; set; }
        [CustomAttributes(HeaderName = "startdt", HasInnerProperty = false)]
        public DateTime StartDate { get; set; }
        [CustomAttributes(HeaderName = "nextpaydt", HasInnerProperty = false)]
        public DateTime NextPaymentDate { get; set; }
        [CustomAttributes(HeaderName = "matdt", HasInnerProperty = false)]
        public DateTime MaturityDate { get; set; }
        [CustomAttributes(HeaderName = "frequencyid", HasInnerProperty = false)]
        public int FrequencyID { get; set; }
        [CustomAttributes(HeaderName = "frequency", HasInnerProperty = false)]
        public string Frequency { get; set; }
        [CustomAttributes(HeaderName = "ReceiveInterest", HasInnerProperty = true)]
        public ReceiveInterest ReceiveInterest { get; set; }
    }

    internal class ContractNew
    {
        [CustomAttributes(HeaderName = "globalcontractid", HasInnerProperty = false)]
        public int GlobalContractID { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "contracttypeid", HasInnerProperty = false)]
        public int ContractTypeID { get; set; }
        [CustomAttributes(HeaderName = "contracttype", HasInnerProperty = false)]
        public string ContractType { get; set; }
        [CustomAttributes(HeaderName = "currtypeid", HasInnerProperty = false)]
        public int CurrencyTypeID { get; set; }
        [CustomAttributes(HeaderName = "currtype", HasInnerProperty = false)]
        public string CurrencyType { get; set; }
        [CustomAttributes(HeaderName = "fxrate", HasInnerProperty = false)]
        public double FXRate { get; set; }
        [CustomAttributes(HeaderName = "allinrate", HasInnerProperty = false)]
        public double AllInRate { get; set; }
        [CustomAttributes(HeaderName = "baserate", HasInnerProperty = false)]
        public double BaseRate { get; set; }
        [CustomAttributes(HeaderName = "spread", HasInnerProperty = false)]
        public double Spread { get; set; }
        [CustomAttributes(HeaderName = "rateoption", HasInnerProperty = false)]
        public string RateOption { get; set; }
        [CustomAttributes(HeaderName = "behaviorid", HasInnerProperty = false)]
        public int BehaviorId { get; set; }
        [CustomAttributes(HeaderName = "behavior", HasInnerProperty = false)]
        public string Behavior { get; set; }
        [CustomAttributes(HeaderName = "monthcntid", HasInnerProperty = false)]
        public int MonthCountID { get; set; }
        [CustomAttributes(HeaderName = "monthcnt", HasInnerProperty = false)]
        public string MonthCount { get; set; }
        [CustomAttributes(HeaderName = "yearcntid", HasInnerProperty = false)]
        public int YearCountID { get; set; }
        [CustomAttributes(HeaderName = "yearcnt", HasInnerProperty = false)]
        public string YearCount { get; set; }
        [CustomAttributes(HeaderName = "startdt", HasInnerProperty = false)]
        public DateTime StartDate { get; set; }
        [CustomAttributes(HeaderName = "nextpaydt", HasInnerProperty = false)]
        public DateTime NextPaymentDate { get; set; }
        [CustomAttributes(HeaderName = "matdt", HasInnerProperty = false)]
        public DateTime MaturityDate { get; set; }
        [CustomAttributes(HeaderName = "frequencyid", HasInnerProperty = false)]
        public int FrequencyID { get; set; }
        [CustomAttributes(HeaderName = "frequency", HasInnerProperty = false)]
        public string Frequency { get; set; }
    }

    internal class Contract
    {
        [CustomAttributes(HeaderName = "globalcontractid", HasInnerProperty = false)]
        public int GlobalContractID { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "contracttypeid", HasInnerProperty = false)]
        public int ContractTypeID { get; set; }
        [CustomAttributes(HeaderName = "contracttype", HasInnerProperty = false)]
        public string ContractType { get; set; }
        [CustomAttributes(HeaderName = "currtypeid", HasInnerProperty = false)]
        public int CurrencyTypeID { get; set; }
        [CustomAttributes(HeaderName = "currtype", HasInnerProperty = false)]
        public string CurrencyType { get; set; }
        [CustomAttributes(HeaderName = "fxrate", HasInnerProperty = false)]
        public double FXRate { get; set; }
        [CustomAttributes(HeaderName = "allinrate", HasInnerProperty = false)]
        public double AllInRate { get; set; }
        [CustomAttributes(HeaderName = "baserate", HasInnerProperty = false)]
        public double BaseRate { get; set; }
        [CustomAttributes(HeaderName = "spread", HasInnerProperty = false)]
        public double Spread { get; set; }
        [CustomAttributes(HeaderName = "rateoption", HasInnerProperty = false)]
        public string RateOption { get; set; }
        [CustomAttributes(HeaderName = "behaviorid", HasInnerProperty = false)]
        public int BehaviorId { get; set; }
        [CustomAttributes(HeaderName = "behavior", HasInnerProperty = false)]
        public string Behavior { get; set; }
        [CustomAttributes(HeaderName = "monthcntid", HasInnerProperty = false)]
        public int MonthCountID { get; set; }
        [CustomAttributes(HeaderName = "monthcnt", HasInnerProperty = false)]
        public string MonthCount { get; set; }
        [CustomAttributes(HeaderName = "yearcntid", HasInnerProperty = false)]
        public int YearCountID { get; set; }
        [CustomAttributes(HeaderName = "yearcnt", HasInnerProperty = false)]
        public string YearCount { get; set; }
        [CustomAttributes(HeaderName = "startdt", HasInnerProperty = false)]
        public DateTime StartDate { get; set; }
        [CustomAttributes(HeaderName = "nextpaydt", HasInnerProperty = false)]
        public DateTime NextPaymentDate { get; set; }
        [CustomAttributes(HeaderName = "matdt", HasInnerProperty = false)]
        public DateTime MaturityDate { get; set; }
        [CustomAttributes(HeaderName = "frequencyid", HasInnerProperty = false)]
        public int FrequencyID { get; set; }
        [CustomAttributes(HeaderName = "frequency", HasInnerProperty = false)]
        public string Frequency { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
    }

    internal class ReceiveInterest
    {
        [CustomAttributes(HeaderName = "makeadj", HasInnerProperty = false)]
        public bool MakeAdjustment { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "recdt", HasInnerProperty = false)]
        public DateTime ReceiveDate { get; set; }
        [CustomAttributes(HeaderName = "additionalamt", HasInnerProperty = false)]
        public double AdditionalAmount { get; set; }
    }

    internal class Capitalization
    {
        [CustomAttributes(HeaderName = "cappercent", HasInnerProperty = false)]
        public double CapitalizationPercent { get; set; }
        [CustomAttributes(HeaderName = "capprice", HasInnerProperty = false)]
        public double CapitalizedPrice { get; set; }
        [CustomAttributes(HeaderName = "isfee", HasInnerProperty = false)]
        public bool IsFee { get; set; }
        [CustomAttributes(HeaderName = "adjunf", HasInnerProperty = false)]
        public bool UnfundedContractAdjusted { get; set; }
        [CustomAttributes(HeaderName = "capamt", HasInnerProperty = false)]
        public double CapitalizationReceivedAdjustment { get; set; }
        [CustomAttributes(HeaderName = "isnewcontract", HasInnerProperty = false)]
        public bool IsNewContract { get; set; }
        [CustomAttributes(HeaderName = "Contract", HasInnerProperty = true)]
        public Contract Contract { get; set; }
    }
}
