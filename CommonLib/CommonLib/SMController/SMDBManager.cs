using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using com.ivp.commom.commondal;
using com.ivp.rad.dal;
using System.Data;

namespace com.ivp.common
{
    public class SMDBManager
    {

        public DataTable GetSecTypeAndAttributeDetails(List<SRMSecurityAttributeDetailsInfo> lstInput, RDBConnectionManager mDBCon = null)
        {
            DataTable table = null;

            DataSet result = null;
            XElement securityXML = new XElement("root", from sec in lstInput
                                                        select new XElement("sec",
                                                        new XAttribute("si", sec.SecurityTypeId),
                                                        new XAttribute("ai", sec.SecurityAttributeId == null ? "" : sec.SecurityAttributeId),
                                                        new XAttribute("an", sec.SecurityAttributeRealName == null ? "" : sec.SecurityAttributeRealName)
                                               ));

            string query = " EXEC SECM_GetAttributeDetailsAdvanced '" + securityXML.ToString() + "' ";

            if (mDBCon != null)
                result = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);

            else
                result = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.SecMaster_Connection);

            if (result != null && result.Tables.Count > 0 && result.Tables[0] != null && result.Tables[0].Rows.Count > 0)
            {
                table = result.Tables[0];

            }

            return table;
        }

       
    }

}
