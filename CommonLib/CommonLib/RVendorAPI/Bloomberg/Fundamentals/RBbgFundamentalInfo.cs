using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;
using System.Data;

namespace com.ivp.srm.vendorapi.Bloomberg.Fundamentals
{
    public class RBbgFundamentalInfo
    {
        public RBbgFundamentalInfo()
        {
            Instruments = new List<RBbgInstrumentInfo>();
            InstrumentFields = new List<RBbgFieldInfo>();
            ProcessedInstruments = new List<string>();
            ProcessedFields = new List<string>();
            ResultantData = new DataSet();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instruments.
        /// </summary>
        /// <value>The instruments.</value>
        public List<RBbgInstrumentInfo> Instruments { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instrument fields.
        /// </summary>
        /// <value>The instrument fields.</value>
        public List<RBbgFieldInfo> InstrumentFields { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the transport.
        /// </summary>
        /// <value>The name of the transport.</value>
        public string TransportName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the market sector.
        /// </summary>
        /// <value>The market sector.</value>
        public RBbgMarketSector MarketSector { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the instrument identifier.
        /// </summary>
        /// <value>The type of the instrument identifier.</value>
        public RBbgInstrumentIdType InstrumentIdentifierType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        public int ModuleId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestIdentifier { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [immediate request].
        /// </summary>
        public bool ImmediateRequest { get; set; }

        public DataSet ResultantData { get; set; }

        public List<string> ProcessedInstruments { get; set; }

        public List<string> ProcessedFields { get; set; }

        public string UserLogin { get; set; }

        public RPeriodicity Periodicity{get;set;}

        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value; DateProvided = true;
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value; DateProvided = true;
            }
        }


        internal bool DateProvided { get; set; }
        public int VendorPreferenceId { get; set; }
    }

    public enum RPeriodicity
    {
        //yearly
        y,
        //quaterly
        q,
        //semi-annually
        s,
        //all
        a,
        //cumulative quarter
        c
    }
}
