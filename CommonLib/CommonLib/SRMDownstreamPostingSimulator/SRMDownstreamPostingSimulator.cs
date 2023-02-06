using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.customclass;
using System.Data;
using System.Threading;

namespace SRMDownstreamPostingSimulator
{
    public class SRMDownstreamPostingSimulator : RExternalSystem
    {
        public override List<RExternalSystemStatusInfo> Execute(object input)
        {
            DataSet ds = (DataSet)input;
            List<RExternalSystemStatusInfo> lstStatus = new List<RExternalSystemStatusInfo>();
            bool hasStatusColumn = false;
            bool hasDelayColumn = false;
            int delayTime = 5000;
            string internalIdColumnName = string.Empty;

            if (ds.Tables[0].Columns.Contains("entity_code"))
            {
                internalIdColumnName = "entity_code";
            }
            else if (ds.Tables[0].Columns.Contains("Entity Code"))
            {
                internalIdColumnName = "Entity Code";
            }
            else if (ds.Tables[0].Columns.Contains("sec_id"))
            {
                internalIdColumnName = "sec_id";
            }
            else if (ds.Tables[0].Columns.Contains("Security Id"))
            {
                internalIdColumnName = "Security Id";
            }

            if (ds.Tables[0].Columns.Contains("PostingStatus"))
            {
                hasStatusColumn = true;
            }
            if (ds.Tables[0].Columns.Contains("PostingDelay"))
            {
                hasDelayColumn = true;
            }

            Random rand = new Random();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RExternalSystemStatusInfo info = new RExternalSystemStatusInfo();

                if (hasDelayColumn && !string.IsNullOrEmpty(Convert.ToString(dr["PostingDelay"])))
                {
                    if (Convert.ToInt32(dr["PostingDelay"]) > delayTime)
                        delayTime = Convert.ToInt32(dr["PostingDelay"]);
                }
                info.ExternalSystemId = string.Empty;
                info.InternalSystemId = Convert.ToString(dr[internalIdColumnName]);

                if (hasStatusColumn && !string.IsNullOrEmpty(Convert.ToString(dr["PostingStatus"]))
                    && (Convert.ToString(dr["PostingStatus"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) || Convert.ToString(dr["PostingStatus"]).Equals("FAILURE", StringComparison.OrdinalIgnoreCase)))
                {
                    if (Convert.ToString(dr["PostingStatus"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        info.Status = RExternalSystemStatus.SUCCESS;
                        info.StatusMessage = "Posting successful";
                    }
                    else
                    {
                        info.Status = RExternalSystemStatus.FAILURE;
                        info.StatusMessage = "Posting failed";
                    }
                }
                else
                {
                    if ((rand.Next() % 2) == 0)
                    {
                        info.Status = RExternalSystemStatus.SUCCESS;
                        info.StatusMessage = "Posting successful";
                    }
                    else
                    {
                        info.Status = RExternalSystemStatus.FAILURE;
                        info.StatusMessage = "Posting failed";
                    }
                }

                lstStatus.Add(info);
            }

            Thread.Sleep(delayTime);
            return lstStatus;
        }
    }
}
