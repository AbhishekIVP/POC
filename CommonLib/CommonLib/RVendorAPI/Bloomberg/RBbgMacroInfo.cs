using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Macro Class for Bloomberg.
    /// </summary>
    public class RBbgMacroInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the qualifier type.
        /// </summary>
        public string QualifierType { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the qualifier value.
        /// </summary>
        public string QualifierValue { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the qualifier operator.
        /// </summary>
        public RBbgOperatorType QualifierOperator { get; set; }
    }
}
