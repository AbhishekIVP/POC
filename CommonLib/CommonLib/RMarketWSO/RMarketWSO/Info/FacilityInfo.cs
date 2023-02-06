using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class FacilityInfo
    {
        #region "Members"
        private int? _wsoFacilityID;
        private string _facilityName;
        private string _facilityType;
        private string _country;
        private string _purpose;
        private DateTime? _maturityDate;
        private double? _originalCommitment;
        private string _facilityCurrency;
        private string _facilityAbbrevName;
        private float? _statedSpread;
        private bool? _activityTracked;
        private bool? _caCertified;
        private DateTime? _lastUpdated;
        private string _guarantor;
        private bool? _isFixed;
        private bool? _tradesWithAccrued;
        private bool? _isSynthetic;
        private bool? _isDelayedDraw;
        private bool? _isDIP;
        private bool? _isGuaranteed;
        private bool? _isLCCreditLinked;
        private bool? _hasLCSublimit;
        private decimal? _lCSublimit;
        private string _seniority;
        private string _lienType;
        private decimal? _issuePrice;
        private string _paymentFrequency;
        private float? _facilityMinAssignment;
        private float? _minHold;
        private bool? _issuingBankConsent;
        private string _collateral;
        private string _notes;
        private double? _currentCommitment;
        private double? _currentOutstanding;
        private DateTime? _firstPaymentDate;
        private bool? _isPositionClosed;
        private bool? _isDefault;

        #endregion

        #region "Properties"

        public int? FacilityID
        {
            get { return _wsoFacilityID; }
            set { _wsoFacilityID = value; }
        }

        public string FacilityName
        {
            get { return _facilityName; }
            set { _facilityName = value; }
        }

        public string FacilityType
        {
            get { return _facilityType; }
            set { _facilityType = value; }
        }

        public string FacilityCountry
        {
            get { return _country; }
            set { _country = value; }
        }

        public string FacilityPurpose
        {
            get { return _purpose; }
            set { _purpose = value; }
        }

        public DateTime? FacilityMaturityDate
        {
            get { return _maturityDate; }
            set { _maturityDate = value; }
        }

        public double? FacilityOriginalCommitment
        {
            get { return _originalCommitment; }
            set { _originalCommitment = value; }
        }

        public string FacilityCurrency
        {
            get { return _facilityCurrency; }
            set { _facilityCurrency = value; }
        }

        public string FacilityAbbrevName
        {
            get { return _facilityAbbrevName; }
            set { _facilityAbbrevName = value; }
        }



        public float? FacilityStatedSpread
        {
            get { return _statedSpread; }
            set { _statedSpread = value; }
        }

        public bool? FacilityActivityTracked
        {
            get { return _activityTracked; }
            set { _activityTracked = value; }
        }


        public bool? FacilityCaCertified
        {
            get { return _caCertified; }
            set { _caCertified = value; }
        }

        public DateTime? FacilityLastUpdated
        {
            get { return _lastUpdated; }
            set { _lastUpdated = value; }
        }

        public String FacilityGuarantor
        {
            get { return _guarantor; }
            set { _guarantor = value; }
        }
        public bool? FacilityIsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }
        public bool? FacilityTradesWithAccrued
        {
            get { return _tradesWithAccrued; }
            set { _tradesWithAccrued = value; }
        }
        public bool? FacilityIsSynthetic
        {
            get { return _isSynthetic; }
            set { _isSynthetic = value; }
        }
        public bool? FacilityIsDelayedDraw
        {
            get { return _isDelayedDraw; }
            set { _isDelayedDraw = value; }
        }
        public bool? FacilityIsDIP
        {
            get { return _isDIP; }
            set { _isDIP = value; }
        }
        public bool? FacilityIsGuaranteed
        {
            get { return _isGuaranteed; }
            set { _isGuaranteed = value; }
        }
        public bool? FacilityIsLCCreditLinked
        {
            get { return _isLCCreditLinked; }
            set { _isLCCreditLinked = value; }
        }
        public bool? FacilityHasLCSublimit
        {
            get { return _hasLCSublimit; }
            set { _hasLCSublimit = value; }
        }
        public decimal? FacilityLCSublimit
        {
            get { return _lCSublimit; }
            set { _lCSublimit = value; }
        }
        public string FacilitySeniority
        {
            get { return _seniority; }
            set { _seniority = value; }
        }
        public string FacilityLienType
        {
            get { return _lienType; }
            set { _lienType = value; }
        }
        public decimal? FacilityIssuePrice
        {
            get { return _issuePrice; }
            set { _issuePrice = value; }
        }
        public string FacilityPaymentFrequency
        {
            get { return _paymentFrequency; }
            set { _paymentFrequency = value; }
        }
        public float? FacilityMinAssignment
        {
            get { return _facilityMinAssignment; }
            set { _facilityMinAssignment = value; }
        }
        public float? FacilityMinHold
        {
            get { return _minHold; }
            set { _minHold = value; }
        }
        public bool? FacilityIssuingBankConsent
        {
            get { return _issuingBankConsent; }
            set { _issuingBankConsent = value; }
        }
        public string FacilityCollateral
        {
            get { return _collateral; }
            set { _collateral = value; }
        }


        public string FacilityNotes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public double? FacilityCurrentCommitment
        {
            get { return _currentCommitment; }
            set { _currentCommitment = value; }
        }

        public double? FacilityCurrentOutstanding
        {
            get { return _currentOutstanding; }
            set { _currentOutstanding = value; }
        }

        public DateTime? FacilityFirstPaymentDate
        {
            get { return _firstPaymentDate; }
            set { _firstPaymentDate = value; }
        }

        public bool? FacilityIsPositionClosed
        {
            get { return _isPositionClosed; }
            set { _isPositionClosed = value; }
        }

        public bool? FacilityIsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }

        public System.Nullable<System.DateTime> FacilitySpringingMaturityDate { get; set; }
        public System.Nullable<System.DateTime> FacilityExpirationDate { get; set; }
        public System.Nullable<System.DateTime> FacilityDateDefaulted { get; set; }
        public System.Nullable<bool> FacilityIsDateConfirmed { get; set; }
        public System.Nullable<decimal> FacilityExitFee { get; set; }
        public System.Nullable<bool> FacilityIsSubscribed { get; set; }
        public System.Nullable<bool> FacilityAllowsFeeAccrual { get; set; }
        public System.Nullable<bool> FacilityAllowsPIKAccrual { get; set; }
        public System.Nullable<bool> FacilityTradesWithAccruedFee { get; set; }
        public System.Nullable<bool> FacilityTradesWithAccruedPIK { get; set; }
        public string FacilityIsCovLite { get; set; }
        public System.Nullable<bool> FacilityIsPIK { get; set; }
        public System.Nullable<bool> FacilityCashPIKToggle { get; set; }
        public System.Nullable<int> FacilityCashPIKBD { get; set; }
        public System.Nullable<bool> FacilityIsABL { get; set; }
        public System.Nullable<bool> FacilityIsMulticurrency { get; set; }
        public System.Nullable<bool> FacilityIsRestructured { get; set; }
        public System.Nullable<bool> FacilityIsCapFloorVerified { get; set; }
        public string FacilityStatedSpreadType { get; set; }
        public System.Nullable<bool> FacilityIsAssetQC { get; set; }
        public System.Nullable<System.DateTime> FacilityAssetQCDate { get; set; }
        public System.Nullable<System.DateTime> FacilityLaunchDate { get; set; }
        public System.Nullable<System.DateTime> FacilityAssetEffectiveDate { get; set; }
        public System.Nullable<System.DateTime> FacilityReplacedDate { get; set; }
        public string FacilityReplacedReason { get; set; }
        #endregion
    }

    public class SecurityIDFieldInfo
    {
        public string SPMID { get; set; }
        public string MAID { get; set; }
        public string LIN { get; set; }
        public string FIGI { get; set; }
        public string BBUD { get; set; }
        public string LoanX { get; set; }
        public string ISIN { get; set; }
        public string CUSIP { get; set; }
        public string SPMID03 { get; set; }
        public string MAID01 { get; set; }
    }

    public class SpreadFieldInfo
    {
        private string _type;
        private string _value;

        public string SpreadFieldType
        {
            get { return _type; }
            set { _type = value; }
        }

        public string SpreadFieldValue
        {
            get { return _value; }
            set { _value = value; }
        }

    }

    public class SICLevelInfo
    {
        private string _type;
        private string _name;
        private string _code;
        private DateTime? _startDate;


        public string SICLevelType
        {
            get { return _type; }
            set { _type = value; }
        }

        public string SICLevelName
        {
            get { return _name; }
            set { _name = value; }
        }

        public string SICLevelCode
        {
            get { return _code; }
            set { _code = value; }
        }

        public DateTime? SICLevelStartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

    }


}