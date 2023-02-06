using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{
    public class AttributeLegControllerInfo
    {
        public string LegName { get; set; }
        public int LegId { get; set; }
        public bool HasUnderlier { get; set; }
        public string UnderlierSecTypeName { get; set; }
        public bool MultipleInfo { get; set; }
        public string PrimaryKey { get; set; }
        public bool IsAdditionalLeg { get; set; }

    }
}
