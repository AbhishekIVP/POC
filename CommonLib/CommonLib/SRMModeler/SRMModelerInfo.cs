using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{
    public class SRMModelerTypeInfo
    {
        public int EntityTypeId { get; set; }
        public string Type { get; set; }
        public string[] Tags { get; set; }
        public bool HasTags { get; set; }
    }
    public class SRMModelerConfigInfo
    {
       
        public string SecTypeName { get; set; }
        public bool IsVanilla { get; set; }
        public bool IsExotic { get; set; }
        public bool IsBasket { get; set; }
        public string SecTypeDesc { get; set; }
        public bool IsCreateScratch { get; set; }
        public bool IsCreateExisting { get; set; }
        public string DefaultDate { get; set; }
        public string[] Tags { get; set; }

        public string[] Users { get; set; }
        public string[] Groups { get; set; }

    }
    public class SRMModelerUserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string username { get; set; }
        public string email { get; set; }

    }
    public class SRMModelerGroupInfo
    {
        public string groupname { get; set; }
    }

}
