using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class BankDealInfo
    {
        #region "Members"
        private int? _wSODataBankDealID;
        private string _markitBankDealID;
        private string _bankDealName;
        private string _streetName;
        private decimal? _globalAmount;
        private DateTime? _creditDate;
        private string _currencyType;
        private string _underwriter;
        private string _prepaymentNotes;
        private decimal? _minAssignment;
        private bool? _isCovLite;
        private string _defaultNotes;
        private bool? _agentConsent;
        private bool? _borrowerConsent;
        private decimal? _assignmentFee;
        private decimal? _minHold;
        private string _publicLoan;
        private bool? _canAffiliate;

        #endregion

        #region "properties"
        public int? BankDealDataID
        {
            get { return _wSODataBankDealID; }
            set { _wSODataBankDealID = value; }
        }

        public string BankDealID
        {
            get { return _markitBankDealID; }
            set { _markitBankDealID = value; }
        }

        public string BankDealName
        {
            get { return _bankDealName; }
            set { _bankDealName = value; }
        }

        public string BankDealStreetName
        {
            get { return _streetName; }
            set { _streetName = value; }
        }

        public decimal? BankDealGlobalAmount
        {
            get { return _globalAmount; }
            set { _globalAmount = value; }
        }

        public DateTime? BankDealCreditDate
        {
            get { return _creditDate; }
            set { _creditDate = value; }
        }

        public string BankDealCurrencyType
        {
            get { return _currencyType; }
            set { _currencyType = value; }
        }


        public string BankDealUnderwriter
        {
            get { return _underwriter; }
            set { _underwriter = value; }
        }
        public string BankDealPrepaymentNotes
        {
            get { return _prepaymentNotes; }
            set { _prepaymentNotes = value; }
        }
        public decimal? BankDealMinAssignment
        {
            get { return _minAssignment; }
            set { _minAssignment = value; }
        }
        public bool? BankDealIsCovLite
        {
            get { return _isCovLite; }
            set { _isCovLite = value; }
        }
        public string BankDealDefaultNotes
        {
            get { return _defaultNotes; }
            set { _defaultNotes = value; }
        }
        public bool? BankDealAgentConsent
        {
            get { return _agentConsent; }
            set { _agentConsent = value; }
        }
        public bool? BankDealBorrowerConsent
        {
            get { return _borrowerConsent; }
            set { _borrowerConsent = value; }
        }
        public decimal? BankDealAssignmentFee
        {
            get { return _assignmentFee; }
            set { _assignmentFee = value; }
        }
        public decimal? BankDealMinHold
        {
            get { return _minHold; }
            set { _minHold = value; }
        }
        public string BankDealPublicLoan
        {
            get { return _publicLoan; }
            set { _publicLoan = value; }
        }
        public bool? BankDealCanAffiliate
        {
            get { return _canAffiliate; }
            set { _canAffiliate = value; }
        }

        public string BankDealDescription { get; set; }
        public System.Nullable<bool> BankDealIsCAReceived { get; set; }
        public string BankDealPublicSource { get; set; }
        public string BankDealFaxAgent { get; set; }
        public string BankDealFaxAgentContact { get; set; }
        #endregion
    }


    public class BankDealSponserLevelInfo
    {

        #region "Members"
        private string _primarySponsor;
        private string _sponsor2;
        private string _sponsor3;
        #endregion

        #region "properties"
        public string BankDealSponserLevelPrimarySponsor
        {
            get { return _primarySponsor; }
            set { _primarySponsor = value; }
        }

        public string BankDealSponserLevelSponsor2
        {
            get { return _sponsor2; }
            set { _sponsor2 = value; }
        }

        public string BankDealSponserLevelSponsor3
        {
            get { return _sponsor3; }
            set { _sponsor3 = value; }
        }

        #endregion

    }
}