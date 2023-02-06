using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.DownstreamSystems
{
    class RMDownstreamSystemsInfo
    {
    }

    public class RM_DownstreamSystem_SheetNames
    {
        public const string Definition = "Definition";
        public const string Mapping = "Report Mapping";

    }
    public class RMReportSystemManagementInfo 
    {
        public int ReportSystemId { get; set; }

        public string ReportSystemName { get; set; }

        public string ReportSystemDescription { get; set; }

        public string AssemblyPath { get; set; }

        public string ClassName { get; set; }

        public string Version { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool RequireReportAttributeLevelAudit { get; set; }
    }
    public class RMReportSystemMappingInfo 
    {
        public int ReportSystemMapId { get; set; }
        public int ReportSystemId { get; set; }
        public int ReportId { get; set; }
        public int ReportAttributeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
