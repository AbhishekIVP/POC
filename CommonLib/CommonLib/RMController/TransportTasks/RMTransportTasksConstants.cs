using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace com.ivp.common.TransportTasks
{
    public class RMTransportTasksConstants
    {
        public class RM_TransportTask_SheetNames
        {
            public const string CustomClasses = "Custom Classes";
            public const string Definition = "Definition";

        }
        public const int TASK_TYPE_ID = 1;
        public enum CallType
        {
            None = 0,
            PRE = 1,
            POST = 2
        }
        public enum ClassType
        {
            None = 0,
            ScriptExecutable = 1,
            CustomClass = 2
        }
    }
    public class TransportTaskPk
    {
        public string taskName { get; set; }
        public string transportType { get; set; }
        public string RemoteFile { get; set; }
        public string RemoteFileLoc { get; set; }
        public string LocalFile { get; set; }
    }
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMTransportTaskInfo : RMBaseInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the transport master id.
        /// </summary>
        /// <value>The transport master id.</value>
        public int TransportMasterId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the transport details id.
        /// </summary>
        /// <value>The transport details id.</value>
        public int TransportDetailsId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the transport.
        /// </summary>
        /// <value>The name of the transport.</value>
        public string TransportName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the remote file.
        /// </summary>
        /// <value>The name of the remote file.</value>
        public string RemoteFileName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the remote file location.
        /// </summary>
        /// <value>The remote file location.</value>
        public string RemoteFileLocation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the local file.
        /// </summary>
        /// <value>The name of the local file.</value>
        public string LocalFileName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the local file location.
        /// </summary>
        /// <value>The local file location.</value>
        public string LocalFileLocation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [use default path].
        /// </summary>
        /// <value><c>true</c> if [use default path]; otherwise, <c>false</c>.</value>
        public bool UseDefaultPath { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the file date.
        /// </summary>
        /// <value>The type of the file date.</value>
        public string FileDateType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom date.
        /// </summary>
        /// <value>The custom date.</value>
        public string CustomDate { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the file date days.
        /// </summary>
        /// <value>The file date days.</value>
        public int FileDateDays { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [custom call exists].
        /// </summary>
        /// <value><c>true</c> if [custom call exists]; otherwise, <c>false</c>.</value>
        public bool CustomCallExists { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the GPG pass phrase.
        /// </summary>
        /// <value>The GPG pass phrase.</value>
        public string GpgPassPhrase { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the GPG user.
        /// </summary>
        /// <value>The name of the GPG user.</value>
        public string GpgUserName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [extract all].
        /// </summary>
        /// <value><c>true</c> if [extract all]; otherwise, <c>false</c>.</value>
        public bool ExtractAll { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RMTransportTaskInfo"/> is status.
        /// </summary>
        /// <value><c>true</c> if status; otherwise, <c>false</c>.</value>
        public bool State { get; set; }
    }
    class UpdateTaskDependentIDInfo
    {
        public const string TASK_MASTER_ID = "task_master_id";
        public const string DEPENDENT_ID = "dependent_id";
    }
}
