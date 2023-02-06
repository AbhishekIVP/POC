using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi
{
    public class RMacroConfigInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the qualifier type.
        /// </summary>
        public string PQType { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the qualifier values.
        /// </summary>
        public List<string> PQValues { get; set; }
    }
}
