using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{
    public class AttributeSetupInfo
    {

        public string AttrName { get; set; }
        public string AttrDescription { get; set; }
        public string DataType { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceAttribute { get; set; }
        public string ReferenceLegAttribute { get; set; }
        public string ReferenceDisplayAttribute { get; set; }
        public string Length { get; set; }
        public bool Mandatory { get; set; }
        public bool IsCloneable { get; set; }
        public string[] Tags { get; set; }
        public string LookupType { get; set; }
        public string LookupAttribute { get; set; }
        public string DefaultValue { get; set; }
        public string SearchViewPosition { get; set; }
        public bool Encrypted { get; set; }
        public bool VisibleInSearch { get; set; }
        public string RestrictedChar { get; set; }

    }

}

