using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{

    public class IssuerInfo
    {
        #region "Members"
        private int? _wSODataIssuerID;
        private string _markitIssuerID;
        private string _issuerName;
        private string _issuerAbbrevName;
        private string _issuerCountry;
        private string _moodyIssuerID;
        private string _SPIssuerID;
        private bool? _hasPublicEquity;
        private string _equityID;
        #endregion

        #region "Properties"
        public int? IssuerDataID
        {
            get { return _wSODataIssuerID; }
            set { _wSODataIssuerID = value; }
        }

        public string IssuerID
        {
            get { return _markitIssuerID; }
            set { _markitIssuerID = value; }
        }


        public string IssuerName
        {
            get { return _issuerName; }
            set { _issuerName = value; }
        }


        public string IssuerAbbrevName
        {
            get { return _issuerAbbrevName; }
            set { _issuerAbbrevName = value; }
        }


        public string IssuerCountry
        {
            get { return _issuerCountry; }
            set { _issuerCountry = value; }
        }

        public string MoodyIssuerID
        {
            get { return _moodyIssuerID; }
            set { _moodyIssuerID = value; }
        }

        public string SPIssuerID
        {
            get { return _SPIssuerID; }
            set { _SPIssuerID = value; }
        }

        public bool? IssuerHasPublicEquity
        {
            get { return _hasPublicEquity; }
            set { _hasPublicEquity = value; }
        }

        public string IssuerEquityID
        {
            get { return _equityID; }
            set { _equityID = value; }
        }

        public string IssuerNotes { get; set; }
        public string IssuerState { get; set; }
        public string IssuerJurisdiction { get; set; }
        public string IssuerParentAffiliate { get; set; }
        public string IssuerMarkitTicker { get; set; }
        public string IssuerFitchId { get; set; }
        #endregion
    }
}