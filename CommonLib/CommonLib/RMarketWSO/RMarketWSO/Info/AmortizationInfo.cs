using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class AmortizationInfo
    {
        private DateTime? _date;
        private decimal? _amount;
        private decimal? _price;
        private bool? _received;
        private string _notes;


        public DateTime? AmortizationDate
        { get { return _date; } set { _date = value; } }

        public decimal? AmortizationAmount
        { get { return _amount; } set { _amount = value; } }

        public decimal? AmortizationPrice
        { get { return _price; } set { _price = value; } }

        public bool? AmortizationReceived
        { get { return _received; } set { _received = value; } }
        public string AmortizationNotes
        { get { return _notes; } set { _notes = value; } }
        public int AmortizationFacilityId { get; set; }
        
    }


   
}