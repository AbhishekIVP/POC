using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class RateOptionInfo
    {
        #region "Members"
        private string _name;
        private string _behavior;
        private float _spread;
        private string _frequency;
        private int _dateOffset;
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime _firstPayDateime;
        private string _monthCount;
        private string _yearCount;
        #endregion

        #region "Properties"
        public string RateOptionName { get { return _name; } set { _name = value; } }
        public string RateOptionBehavior { get { return _behavior; } set { _behavior = value; } }
        public float RateOptionSpread { get { return _spread; } set { _spread = value; } }
        public string RateOptionFrequency { get { return _frequency; } set { _frequency = value; } }
        public int RateOptionDateOffset { get { return _dateOffset; } set { _dateOffset = value; } }
        public DateTime RateOptionStartDate { get { return _startDate; } set { _startDate = value; } }
        public DateTime RateOptionEndDate { get { return _endDate; } set { _endDate = value; } }
        public DateTime RateOptionFirstPayDatetime { get { return _firstPayDateime; } set { _firstPayDateime = value; } }
        public string RateOptionMonthCount { get { return _monthCount; } set { _monthCount = value; } }
        public string RateOptionYearCount { get { return _yearCount; } set { _yearCount = value; } }

        public System.Nullable<bool> RateOptionIsObservationShift { get; set; }
        public string RateOptionInterestMethod { get; set; }
        public string RateOptionCompoundMethod { get; set; }
        #endregion

    }


    public class RateLimitInfo
    {
        private string _rateOption;
        private string _limitType;
        private string _rateType;
        private float? _limit;

        public string RateLimitRateOption
        {
            get { return _rateOption; }
            set { _rateOption = value; }
        }

        public string RateLimitLimitType
        {
            get { return _limitType; }
            set { _limitType = value; }
        }

        public string RateLimitRateType
        {
            get { return _rateType; }
            set { _rateType = value; }
        }

        public float? RateLimitLimit
        {
            get { return _limit; }
            set { _limit = value; }
        }
    }


    public class RatingInfo
    {
        private string _source;
        private string _type;
        private string _name;
        private DateTime? _startDate;

        public string RatingSource
        {
            get { return _source; }
            set { _source = value; }
        }

        public string RatingType
        {
            get { return _type; }
            set { _type = value; }
        }

        public string RatingName
        {
            get { return _name; }
            set { _name = value; }
        }

        public DateTime? RatingStartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
    }


    public class SponsorInfo
    {
        private int _order;
        private string _name;

        public int SponsorOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public string SponsorName
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}