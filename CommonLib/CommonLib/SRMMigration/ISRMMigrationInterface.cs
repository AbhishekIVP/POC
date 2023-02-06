using com.ivp.common.migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace com.ivp.srmcommon
{
    public interface ISRMMigrationInterface
    {
        Dictionary<MigrationFeatureEnum, SRMCommonMigrationController> staticFileMetaData { get; set; }
        string DownloadMigrationConfiguration(List<MigrationFeatureInfo> features, List<object> selectedItems, int moduleID, string userName, SRMMigrationUserAction userAction, out string errorMessage, SRMWorkflowMigrationController obj);

        string SyncMigrationConfiguration(MigrationFeatureInfo featureInfo, int moduleID, string userName, string dateFormat, string dateTimeFormat, string timeFormat, out string errorMessage);

        List<CommonMigrationSelectionInfo> GetSelectableItemsForMigration(int moduleID, int feature, string userName);
    }
}
