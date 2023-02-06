using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO
{
    public class CallSchedulePayInfo
    {
        private int? _order;
        private DateTime? _date;
        private decimal? _price;

        public int? CallSchedulePayOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public DateTime? CallSchedulePayDate
        {
            get { return _date; }
            set { _date = value; }
        }

        public decimal? CallSchedulePayPrice
        {
            get { return _price; }
            set { _price = value; }
        }
    }
}
