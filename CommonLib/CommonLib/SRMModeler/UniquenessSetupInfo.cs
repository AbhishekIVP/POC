using com.ivp.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{

    public class SectypeInfo
    {
        public int SectypeID { get; set; }
        public string SectypeName { get; set; }
    }

    public class UniquenessSetupKeyInfo
    {
        public UniquenessSetupKeyInfo()
        {
            this.SelectedLeg = new UniquenessSetupLegInfo();
            this.SelectedAttributes = new List<UniquenessSetupAttributeInfo>();
            this.SelectedLegAttributes = new List<UniquenessSetupAttributeInfo>();
        }

        public int KeyID { get; set; }
        public string KeyName { get; set; }
        public bool IsMaster { get; set; }
        public bool IsAcrossSecurities { get; set; }
        public List<int> SelectedSectypes { get; set; }
        public UniquenessSetupLegInfo SelectedLeg { get; set; }
        public List<UniquenessSetupAttributeInfo> SelectedAttributes { get; set; }
        public List<UniquenessSetupAttributeInfo> SelectedLegAttributes { get; set; }
        public bool CheckInDrafts { get; set; }
        public bool CheckInWorkflows { get; set; }
        public bool NullAsUnique { get; set; }
    }


    public class UniquenessSetupLegInfo
    {
        public string LegIDs { get; set; }
        public string LegName { get; set; }
        public string AreAdditionalLegs { get; set; }
    }


    public class UniquenessSetupAttributeInfo
    {
        public string AttributeIDs { get; set; }
        public string AttributeName { get; set; }
        public string AreAdditionalLegAttributes { get; set; }
    }

    public class UniquenessSetupCommonMasterAttributesOutputInfo
    {
        public UniquenessSetupCommonMasterAttributesOutputInfo()
        {
            this.CommonMasterAttributesList = new List<UniquenessSetupAttributeInfo>();
        }

        public List<UniquenessSetupAttributeInfo> CommonMasterAttributesList { get; set; }
        public int KeyID { get; set; }
    }

    public class UniquenessSetupCommonLegAttributesOutputInfo
    {
        public UniquenessSetupCommonLegAttributesOutputInfo()
        {
            this.CommonLegAttributesList = new List<UniquenessSetupAttributeInfo>();
        }

        public List<UniquenessSetupAttributeInfo> CommonLegAttributesList { get; set; }
        public int KeyID { get; set; }
    }

    public class UniquenessSetupOutputObject
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<KeyValuePair<string, List<SRMDuplicateInfo>>> uniquenessFailurePopupInfo { get; set; }
    }
}
