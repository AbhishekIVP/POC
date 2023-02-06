using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class SRMSecTypeMassageInputInfo
    {
        public SRMSecTypeMassageInputInfo()
        {
            SecTypeAttributeRealNameColumn = string.Empty;
        }
        public string SecTypeIDColumn { get; set; }
        public string SecTypeAttributeIDColumn { get; set; }
        public string SecTypeAttributeRealNameColumn { get; set; }
        public string SecTypeDisplayNameColumn { get; set; }
        public string SecTypeAttributeDisplayNameColumn { get; set; }
    }


    public class SRMSecurityAttributeDetailsInfo
    {
        public SRMSecurityAttributeDetailsInfo()
        {
            SecurityAttributeRealName = string.Empty;
        }
        public int SecurityTypeId { get; set; }
        public string SecurityTypeName { get; set; }
        public string SecurityAttributeId { get; set; }
        public string SecurityAttributeRealName { get; set; }
        public string SecurityAttributeDisplayName { get; set; }

    }
}
