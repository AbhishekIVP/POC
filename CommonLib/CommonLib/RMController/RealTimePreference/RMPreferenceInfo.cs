using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMPreferenceInfo : RMBaseInfo
    {
        public int PreferenceID { get; set; }
        public string PreferenceName { get; set; }
        public int DataSourceID { get; set; }
        public string VendorDetails { get; set; }
        public string DataSourceName { get; set; }
    }
}
