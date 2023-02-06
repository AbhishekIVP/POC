using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.SecMaster
{
    public class RMSectypeAPI
    {
        public Dictionary<string, SecurityTypeMasterInfo> GetSectypeAttributes(bool requireCurveAttributes)
        {
            return new SMCommonController().GetSectypeAttributes(requireCurveAttributes);
        }
        public void MassageSecTypeAndAttributes(IEnumerable<ObjectRow> dtInput, SRMSecTypeMassageInputInfo inputInfo, RDBConnectionManager mDBCon = null)
        {
            new SMCommonController().MassageSecTypeAndAttributes(dtInput, inputInfo, mDBCon);
        }

        public Dictionary<string, AttrInfo> FetchCommonAttributes(bool requireCurveAttributes)
        {
            return new SMCommonController().FetchCommonAttributes(requireCurveAttributes);
        }

    }
}
