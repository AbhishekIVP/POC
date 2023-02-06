/***************************************************************************************************
 * 
 *  This source forms a part of the IVP RAD Software System and is a copyright of 
 *  Indus Valley Partners (Pvt) Ltd.

 *  All rights are reserved with IVP. No part of this work may be reproduced, stored, 
 *  adopted or transmitted in any form or by any means including but not limiting to 
 *  electronic, mechanical, photographic, graphic, optic recording or otherwise, 
 *  translated in any language or computer language, without the prior written permission 
 *  of

 *  Indus Valley Partners (Pvt) Ltd
 *  Unit 7&8, Bldg 4
 *  Vardhman Trade Center
 *  Nehru Place Greens
 *  New Delhi - 19

 *  Copyright 2007-2008 Indus Valley Partners (Pvt) Ltd.
 * 
 * 
 * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            24-10-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Bloomberg Instrument Info
    /// </summary>
    public class RBbgInstrumentInfo:ICloneable
    {
        public RBbgInstrumentInfo()
        {
            Overrides = new Dictionary<string, object>();
        }

        private RBbgMarketSector _marketSector = RBbgMarketSector.None;
        private RBbgInstrumentIdType _interumentIdType;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instrument ID.
        /// </summary>
        /// <value>The instrument ID.</value>
        public string InstrumentID { get; set; }

        public string BVALPriceSourceValue { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the market sector.
        /// </summary>
        /// <value>The market sector.</value>
        public RBbgMarketSector MarketSector
        {
            get
            {
                return _marketSector;
            }
            set
            {
                _marketSector = value; 
                MarketSectorSpecified = true;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [market sector specified].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [market sector specified]; otherwise, <c>false</c>.
        /// </value>
        internal bool MarketSectorSpecified { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the instrument id.
        /// </summary>
        /// <value>The type of the instrument id.</value>
        public RBbgInstrumentIdType InstrumentIdType
        {
            get
            {
                return _interumentIdType;
            }
            set
            {
                _interumentIdType = value; InstrumentIdTypeSpecified = true;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [instrument id type specified].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [instrument id type specified]; otherwise, <c>false</c>.
        /// </value>
        internal bool InstrumentIdTypeSpecified { get; set; }

        public Dictionary<string, object> Overrides { get; set; }
        internal bool DateSpecified;
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value; DateSpecified = true;
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
                _endDate = value; DateSpecified = true;
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            RBbgInstrumentInfo info = new RBbgInstrumentInfo();
            info.InstrumentID = InstrumentID;
            info.InstrumentIdType = InstrumentIdType;
            info.InstrumentIdTypeSpecified = InstrumentIdTypeSpecified;
            info.MarketSector = MarketSector;
            info.MarketSectorSpecified = MarketSectorSpecified;
            foreach (var value in Overrides)
            {
                info.Overrides.Add(value.Key, value.Value);
            }
            return info;
        }

        #endregion
    }
}
