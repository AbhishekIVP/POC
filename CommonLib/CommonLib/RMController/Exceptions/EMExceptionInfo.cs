using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.Exceptions
{
    public class EMManageExceptionsInfo
    {
        public int EntityTypeId;
        public string UserName;
        public int TaskMasterId;
        public DateTime CreatedOn;
        public List<EMExceptionInfo> ExceptionInfo;
        public bool InsertInactive;
    }
    public class EMExceptionInfo
    {
        public string EntityCode;
        public List<int> AttributesModified;
        public List<EMExceptionDetailsInfo> ExceptionInfo;
    }

    public class EMExceptionDetailsInfo
    {
        public int ExceptionTypeId;
        public int AttributeId;
        public string DBValue;
        public int LegEntityTypeId = -1;
        public string AdditionalInfo;
        public List<EMExceptionDependentValues> DependentValues;

    }

    public class EMExceptionDependentValues
    {
        public int DependentId;
        public int DependentTypeId;
        public string DependentValue;
    }
}
