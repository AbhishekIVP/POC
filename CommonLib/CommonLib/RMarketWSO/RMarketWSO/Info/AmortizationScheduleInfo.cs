using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class AmortizationScheduleInfo
    {
        private string _scheduleName;
        private int?  _wSODataAmortizationID;
        private int? _wSOAmortizationID;
        private DateTime? _scheduleDate;
        private DateTime? _endDate;


        public string AmortizationScheduleName
        {
            get { return _scheduleName; }
            set { _scheduleName = value; }
        }

        public int? AmortizationScheduleDataID
        {
            get { return _wSODataAmortizationID; }
            set { _wSODataAmortizationID = value; }
        }

        public int? AmortizationScheduleID
        {
            get { return _wSOAmortizationID; }
            set { _wSOAmortizationID = value; }
        }

        public DateTime? AmortizationScheduleDate
        {
            get { return _scheduleDate; }
            set { _scheduleDate = value; }
        }

        public DateTime? AmortizationScheduleEndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        public int AmortizationScheduleFacilityId { get; set; }

        public List<AmortizationInfo> AmortizationInfo { get; set; }
    }
}