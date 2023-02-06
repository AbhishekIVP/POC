//Not Used

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.IO;
//using System.Threading;
//using System.Configuration;
//using System.Collections;

//namespace com.ivp.rad.RCommonTaskManager
//{
//    class RCTMFileWatcher
//    {

// //private static IPLogger mLogger = PLogManager.CreateLogger("PEyeOfProvidence");
//        const int _timerSecs = 25;
//        const int _resetSetting = 17;

//        private class WatchedFileMetaData
//        {
//            public Regex FileRegex { get; private set; }

//            public int PreDefBatchId { get; private set; }

//            public WatchedFileMetaData(string pattern, int preDefBatchId)
//            {
//                PreDefBatchId = preDefBatchId;
//                FileRegex = new Regex(pattern, RegexOptions.IgnoreCase);
//            }
//        }

//        private class FileEventInfo
//        {
//            public DateTime FirstEventTime { get; set; }

//            public long Size { get; set; }

//            public string FullFilePath { get; set; }

//            public int TaskStartCounter { get; set; }
//        }

//        private Dictionary<string, WatchedFileMetaData[]> _allWatchedFileInfo;
//        private FileSystemWatcher[] _directoryWatchers;
//        CTMService taskmanager = null;
//        private int _isDisposed;
//        private Timer _signalHashPoller;
//        private int _resetCounter;
        
//        private readonly int _safetyLimit;
//        private readonly Dictionary<string, FileEventInfo> _signalHash;

//        public RCTMFileWatcher(CTMService taskmanager)
//        {
//            this.taskmanager = taskmanager;
//            _signalHash = new Dictionary<string, FileEventInfo>();
//            _safetyLimit = int.Parse(ConfigurationManager.AppSettings["SAFETY_TIMEOUT"]);
//        }            

//        public void StartWatching()
//        {
//            InitWatchers(false);
//            _signalHashPoller = new Timer(CheckFiles, null, _timerSecs * 1000, _timerSecs * 1000);

//            if (_directoryWatchers == null) return;

//            lock (_directoryWatchers)
//            {
//                if (_directoryWatchers == null) return;

//                for (int i = 0; i < _directoryWatchers.Length; i++)
//                {
//                    if (_directoryWatchers[i] != null)
//                    {
//                        _directoryWatchers[i].EnableRaisingEvents = true;
//                        //mLogger.Debug(string.Format("Eye Watching - {0}", _directoryWatchers[i].Path));
//                    }
//                }
//            }
//        }

//        public void StopWatchingAndDispose()
//        {
//            if (Interlocked.Exchange(ref _isDisposed, 1) == 1) return;
//            DisposeAllWatchers();
//            _signalHashPoller.Dispose();
//        }

//        private void InitWatchers(bool startWatching)
//        {
//            try
//            {
//                if (_allWatchedFileInfo != null)
//                {
//                    lock (((ICollection)_allWatchedFileInfo).SyncRoot)
//                    {
//                        _allWatchedFileInfo = new Dictionary<string, WatchedFileMetaData[]>();
//                    }
//                }
//                else
//                {
//                    _allWatchedFileInfo = new Dictionary<string, WatchedFileMetaData[]>();
//                }
//                var watches = RCTMUtils.GetAllToBeWatched();
//                var allEyes = from w in watches

//                              select new
//                              {
//                                  DirPath = w.filewatcherInfo.SearchFolder.TrimEnd('\\').ToLower().Trim(),
//                                  w.chainId,
//                                  FileRegex = w.filewatcherInfo.FileRegex
//                              };

//                var dirList = allEyes.Select(x => x.DirPath).Distinct().ToArray();

//                //mLogger.Debug(string.Format("Eye Init. Directories Count - {0}", dirList.Length));

//                _directoryWatchers = new FileSystemWatcher[dirList.Length];

//                for (int i = 0; i < dirList.Length; i++)
//                {
//                    try
//                    {
//                        var currDir = dirList[i];
//                        // Create a new FileSystemWatcher and set its properties.
//                        _directoryWatchers[i] = new FileSystemWatcher(currDir)
//                        {
//                            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.DirectoryName | NotifyFilters.CreationTime,
//                            IncludeSubdirectories = true
//                        };
//                        // Watch for changes in Creation and LastWrite times, and 
//                        // Add event handlers.
//                        _directoryWatchers[i].Changed += OnFileCreatedChanged;
//                        _directoryWatchers[i].Created += OnFileCreatedChanged;
//                        _directoryWatchers[i].Renamed += OnFileRenamed;
//                        _directoryWatchers[i].Error += OnWatcherError;

//                        var specificFiles = allEyes.Where(x => x.DirPath.Equals(currDir))
//                                            .Select(x => new WatchedFileMetaData(x.FileRegex, x.chainId)).ToArray();

//                        lock (((ICollection)_allWatchedFileInfo).SyncRoot)
//                        {
//                            _allWatchedFileInfo.Add(currDir, specificFiles);
//                        }
//                        if (startWatching)
//                        {
//                            var temp = new DirectoryInfo(currDir);
//                            temp.Refresh();

//                            var files = temp.GetFiles();

//                            var ofiles = files.OrderByDescending(p => p.Name);

//                            foreach (var entry in ofiles)
//                            {
//                                //var tempTime = DateTime.UtcNow.Subtract(entry.LastWriteTimeUtc).TotalSeconds;

//                                //mLogger.Debug(string.Format("Filewatcher re-init. Write time diff for File {0} - {1}", entry.FullName, tempTime));
//                                //mLogger.Debug(string.Format("Filewatcher re-init. Write time diff for File {0}", entry.FullName));

//                                //if (tempTime < (_resetSetting * _timerSecs))
//                                ProcessFileEvent(entry.FullName, "File Found at Re-Init : ");
//                            }
//                            _directoryWatchers[i].EnableRaisingEvents = true;
//                        }

//                        //mLogger.Info("FileWatcher created for the path - " + currDir);
//                    }
//                    catch (Exception ex)
//                    {
//                       // mLogger.Error("Error Initializing" + ex.ToString());
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//               // mLogger.Fatal("Error Initializing. Check config settings." + e.ToString());
//            }
//            finally
//            {
//            }
//        }

//        private void CheckFiles(object dummy)
//        {
//            var tempList = new List<FileEventInfo>();

//            if (_resetCounter++ == _resetSetting)
//            {
//                _resetCounter = 0;
//                DisposeAllWatchers();
//                InitWatchers(true);
//            }

//            lock (((IDictionary)_signalHash).SyncRoot)
//            {
//                var allEventInfo = _signalHash.Values.ToArray();

//                foreach (var entry in allEventInfo)
//                {
//                    try
//                    {
//                        var fi = new FileInfo(entry.FullFilePath);

//                        fi.Refresh();

//                        if (false == fi.Exists)
//                        {
//                            _signalHash.Remove(entry.FullFilePath);
//                            continue;
//                        }

//                        //current size == old size && timedelta > safety time && CanOpenExclusive && TaskNotStarted
//                        if (fi.Length == entry.Size && (DateTime.Now - fi.LastWriteTime).TotalSeconds > _safetyLimit
//                            && CanOpenExclusive(entry.FullFilePath) && entry.TaskStartCounter == 0)
//                        {
//                            tempList.Add(entry);

//                            entry.TaskStartCounter++;

//                           // mLogger.Debug(string.Format("Added to Pending Queue: {0}", entry.FullFilePath));
//                        }

//                        if (entry.TaskStartCounter > 0) entry.TaskStartCounter++;

//                        //new job on same filename cannot be started until it has been removed once = 8*timer secs later
//                        if (entry.TaskStartCounter == (_resetSetting + 2)) _signalHash.Remove(entry.FullFilePath);
//                    }
//                    catch (Exception ex)
//                    {
//                        _signalHash.Remove(entry.FullFilePath);
//                       // mLogger.Warn(string.Format("Exclusive lock exception hence removed from Signal Queue: {0} ", entry.FullFilePath) + ex);
//                    }
//                }
//            }

//            //make sure first file to come gets fired off first
//            tempList.Sort((a, b) => a.FirstEventTime.CompareTo(b.FirstEventTime));
          
//            foreach (var entry in tempList)
//            {
//                var fi = new FileInfo(entry.FullFilePath);

//                fi.Refresh();

//                List<WatchedFileMetaData> matchList;

//                // ReSharper disable PossibleNullReferenceException - Already Checked if File Exists. Directory name cannot be null....
//                if (!FileMatchesRegex(fi.Name, fi.DirectoryName.TrimEnd('\\').ToLower(), out matchList)) continue;
//                // ReSharper restore PossibleNullReferenceException
                
//                foreach (var item in matchList)
//                {
//                    try
//                    {  
//                        string environmentVar = "filenames=";
//                        environmentVar += entry.FullFilePath;
//                        taskmanager.TriggerChainByFilewatcher(item.PreDefBatchId, environmentVar);
//                        //TODO: call task
//                        //taskmanager.Batch(item.PreDefBatchId).
//                        //    WithParameter(new PTaskParam(PConstants.FILE_NAME, entry.FullFilePath, typeof(String))).
//                        //    WithParameter(new PTaskParam(PConstants.FILE_NAME_SSIS_TASK, entry.FullFilePath, typeof(String))).
//                        //    Begin(BatchRunMode.Fresh);

//                       // mLogger.Info(string.Format("Succesfully queued task request for file - {0}, Pre Def Id - {1}", entry.FullFilePath, item.PreDefBatchId));
//                    }
//                    catch (Exception ex)
//                    {
//                       // mLogger.Error(string.Format("Exception while trying to make request for File - {0}, Pre Def Id - {1}", entry.FullFilePath, item.PreDefBatchId) + ex);
//                    }
//                }
//            }
           
//        }

//        private void DisposeAllWatchers()
//        {
//            if (_directoryWatchers == null) return;

//            lock (_directoryWatchers)
//            {
//                if (_directoryWatchers == null) return;

//                for (int i = 0; i < _directoryWatchers.Length; i++)
//                {
//                    if (_directoryWatchers[i] != null)
//                    {
//                        _directoryWatchers[i].EnableRaisingEvents = false;
//                       // mLogger.Debug(string.Format("Eye Stopped Watching - {0}", _directoryWatchers[i].Path));
//                        _directoryWatchers[i].Dispose();
//                    }
//                }
//            }

//            _directoryWatchers = null;
//        }

//        #region FileWatcher Event Handlers

//        private void ProcessFileEvent(string fullFilePath, string logMessagePrefix)
//        {
//            fullFilePath = fullFilePath.ToLower();
//            var fInfo = new FileInfo(fullFilePath);

//            if (false == fInfo.Exists) return;

//            lock (((IDictionary)_signalHash).SyncRoot)
//            {
//                try
//                {
//                    if (_signalHash.ContainsKey(fullFilePath) == false)
//                    {
//                        _signalHash.Add(fullFilePath, new FileEventInfo() { Size = fInfo.Length, FullFilePath = fullFilePath, FirstEventTime = DateTime.Now });
//                    }
//                    else
//                    {
//                        _signalHash[fullFilePath].Size = fInfo.Length;
//                    }
//                }
//                catch (Exception e)
//                {
//                   // mLogger.Fatal("True Exception! in adding file info to signal hash. Possible missed file event." + e.ToString());
//                }
//            }

//          //  mLogger.Debug(logMessagePrefix + fullFilePath);
//        }

//        private void OnFileRenamed(object source, RenamedEventArgs e)
//        {
//            ProcessFileEvent(e.FullPath, "File Renamed. New Name : ");
//        }

//        private void OnFileCreatedChanged(object source, FileSystemEventArgs e)
//        {
//            ProcessFileEvent(e.FullPath, "File Created : ");
//        }

//        private void OnWatcherError(object sender, ErrorEventArgs e)
//        {
//           // mLogger.Warn("Error in File Watcher. Could do a re-init. " + e.GetException().ToString());
//        }

//        #endregion

//        private bool FileMatchesRegex(string fileName, string directoryPath, out List<WatchedFileMetaData> matchList)
//        {
//            bool isMatch = false;
//            matchList = new List<WatchedFileMetaData>();

//          //  mLogger.Debug(string.Format("Now Checking regexes for - {0}\\{1}", directoryPath, fileName));

//            var directoryWatchInfo = SearchFile(directoryPath);

//            if (directoryWatchInfo == null && directoryWatchInfo.Count == 0)
//            {
//              //  mLogger.Fatal(string.Format("Call made to check regex for a path that is not watched!!! Direcotry given - {0}", directoryPath));
//                return false;
//            }

//            var matchData = directoryWatchInfo;

//            foreach (var entry in matchData.Where(entry => entry.FileRegex.IsMatch(fileName)))
//            {
//                matchList.Add(entry);
//                isMatch = true;
//              //  mLogger.Debug(string.Format("Regex matched : {0}", entry.FileRegex));
//            }

//            return isMatch;
//        }

//        private List<WatchedFileMetaData> SearchFile(string directoryName)
//        {
//            List<WatchedFileMetaData> objfilemetatdata = new List<WatchedFileMetaData>();
//            lock (((ICollection)_allWatchedFileInfo).SyncRoot)
//            {
//                foreach (var info in _allWatchedFileInfo)
//                {
//                    List<string> directory = Directory.GetDirectories(info.Key, "*", SearchOption.AllDirectories).ToList();
//                    directory.Add(info.Key);

//                    if (directory.Exists(d => d.ToLower().Equals(directoryName.ToLower())))
//                    {
//                        objfilemetatdata.AddRange(info.Value);
//                        //return info.Value;
//                    }

//                    //if (directoryName.StartsWith(info.Key.ToLower()))
//                    //{
//                    //    objfilemetatdata.AddRange(info.Value);
//                    //}
//                }
//            }
//            return objfilemetatdata;
//        }

//        private static bool CanOpenExclusive(string filePath)
//        {
//            var fi = new FileInfo(filePath);
//            FileStream fs = null;
//            try
//            {
//                fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None);
//                fs.Close();
//            }
//            catch (IOException)
//            {
//                return false;
//            }
//            finally
//            {
//                if (fs != null)
//                {
//                    fs.Dispose();
//                }
//            }
//            return true;
//        }
//    }
//}


