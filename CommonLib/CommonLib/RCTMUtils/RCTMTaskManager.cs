using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace com.ivp.rad.RCTMUtils
{
    public abstract class RCTMTaskManager
    {
        public abstract void RunJob(TaskInfo info, string clientName);

        public abstract void Cancel(TaskInfo info, string clientName);

        public abstract List<string> getCalendarNames(string clientName);

        public abstract List<string> getSubscribeMailIds(string clientName);



        public abstract void DeleteTaskStatusFromClient(int clientTaskStatusId, string clientName);
        public abstract void UndoTask(int clientTaskStatusId, string clientName);

        public virtual void flowAdded(List<int> clientTaskMasterId, string clientName) { }
        public virtual void flowDeleted(List<int> clientTaskMasterId, string clientName) { }

        public abstract List<int> getUnsyncdTasksClientTaskStatusIds(List<int> clientTaskStatusIds, string clientName);

        public virtual List<string> getPrivilegeList(string pageId, string username, string clientName) { return null; }

        public virtual List<int> isSecureToTrigger(int taskMasterId, string clientName) { return new List<int>(); }

        public virtual DataTable SyncStatus(List<int> ctmStatusId, string clientName) { return new DataTable(); }

        public virtual void KillInprogressTask(List<int> ctmStatusId, string clientName) { }
    }
}
