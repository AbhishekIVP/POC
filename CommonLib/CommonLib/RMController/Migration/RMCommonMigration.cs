using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.Migration
{
    public class RMCommonMigration
    {
        public void FailDependencies(Dictionary<string, IEnumerable<ObjectRow>> dependencies, string errorMessage, bool setNotProcessed)
        {
            foreach (KeyValuePair<string, IEnumerable<ObjectRow>> kvp in dependencies.Where(d => d.Value != null && d.Value.Count() > 0))
            {
                kvp.Value.ToList().ForEach(row =>
                {
                    SetFailedRow(row, new List<string>() { errorMessage }, setNotProcessed);
                });
            }
        }

        public void SetFailedRow(ObjectRow row, List<string> remarks, bool setNotProcessed)
        {
            row[RMCommonConstants.MIGRATION_COL_STATUS] = setNotProcessed ? RMCommonConstants.MIGRATION_NOT_PROCESSED : RMCommonConstants.MIGRATION_FAILED;

            bool addSeparator = !string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS]).Trim());
            int remCount = 0;

            remarks.ForEach(rem => {
                remCount++;
                if (remCount > 1)
                    addSeparator = true;

                row[RMCommonConstants.MIGRATION_COL_REMARKS] = Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS]) +
                (addSeparator ? RMCommonConstants.MIGRATION_REMARKS_SEPARATOR : string.Empty) + rem;
            });
        }

        public void SetPassedRow(ObjectRow row, string remarks)
        {
            row[RMCommonConstants.MIGRATION_COL_STATUS] = RMCommonConstants.MIGRATION_SUCCESS;
            row[RMCommonConstants.MIGRATION_COL_REMARKS] = remarks;
        }

        public void SetInSyncRow(ObjectRow row, string remarks)
        {
            row[RMCommonConstants.MIGRATION_COL_STATUS] = RMCommonConstants.MIGRATION_IN_SYNC;
            row[RMCommonConstants.MIGRATION_COL_REMARKS] = remarks;
        }
    }
}
