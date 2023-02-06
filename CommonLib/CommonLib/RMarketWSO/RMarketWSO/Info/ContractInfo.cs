using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class ContractInfo
    {
        #region "Members"
        private int? _wSODataContractID;
        private string _contractName;
        private string _contractType;
        private string _rateOption;
        private Decimal? _globalAmount;
        private float? _allInRate;
        private float? _baseRate;
        private float? _spread;
        private DateTime? _startDate;
        private DateTime? _nextPaymentDate;
        private DateTime? _maturityDate;
        private Decimal? _interestDue;
        private Decimal? _interestReceived;
        private String _notes;
        private String _currencyType;
        private Decimal? _exchangeRate;
        private int? _frequency;
        private string _accrualBasis;
        private string _monthCount;
        private string _yearCount;
        #endregion

        #region "Properties"
        public int? ContractID { get { return _wSODataContractID; } set { _wSODataContractID = value; } }
        public string ContractName { get { return _contractName; } set { _contractName = value; } }
        public string ContractType { get { return _contractType; } set { _contractType = value; } }
        public string ContractRateOption { get { return _rateOption; } set { _rateOption = value; } }
        public Decimal? ContractGlobalAmount { get { return _globalAmount; } set { _globalAmount = value; } }
        public float? ContractAllInRate { get { return _allInRate; } set { _allInRate = value; } }
        public float? ContractBaseRate { get { return _baseRate; } set { _baseRate = value; } }
        public float? ContractSpread { get { return _spread; } set { _spread = value; } }
        public DateTime? ContractStartDate { get { return _startDate; } set { _startDate = value; } }
        public DateTime? ContractNextPaymentDate { get { return _nextPaymentDate; } set { _nextPaymentDate = value; } }
        public DateTime? ContractMaturityDate { get { return _maturityDate; } set { _maturityDate = value; } }
        public Decimal? ContractInterestDue { get { return _interestDue; } set { _interestDue = value; } }
        public Decimal? ContractInterestReceived { get { return _interestReceived; } set { _interestReceived = value; } }
        public String ContractNotes { get { return _notes; } set { _notes = value; } }
        public String ContractCurrencyType { get { return _currencyType; } set { _currencyType = value; } }
        public Decimal? ContractExchangeRate { get { return _exchangeRate; } set { _exchangeRate = value; } }
        public int? ContractFrequency { get { return _frequency; } set { _frequency = value; } }
        public string ContractAccrualBasis { get { return _accrualBasis; } set { _accrualBasis = value; } }
        public string ContractMonthCount { get { return _monthCount; } set { _monthCount = value; } }
        public string ContractYearCount { get { return _yearCount; } set { _yearCount = value; } }

        public string ContractBehavior { get; set; }
        public System.Nullable<System.DateTime> ContractReceiveDate { get; set; }
        public System.Nullable<bool> ContractIsReceived { get; set; }
        public System.Nullable<int> ContractAccrualBasisId { get; set; }
        public string ContractAccrualFeeType { get; set; }
        public System.Nullable<bool> ContractIsObservationShift { get; set; }
        public string ContractInterestMethod { get; set; }
        public System.Nullable<float> ContractSpreadAdjustment { get; set; }
        public System.Nullable<int> ContractLookBackDayOffset { get; set; }

        public int ContractFacilityId { get; set; } 
        public string ContractLoanX { get; set; }
        #endregion
    }

    public class MultiCurrencyInfo {
        private string _currencyType;
        private float? _limit;

        public string MultiCurrencyCurrencyType
        {
            get { return _currencyType; }
            set { _currencyType = value; }
        }

        public float? MultiCurrencyLimit
        {
            get { return _limit; }
            set { _limit = value; }
        }

    }
}