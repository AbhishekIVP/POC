using com.ivp.rad.common;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    public class SRMCommonZip
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMCommonZip");
        string basePath;
        public bool Zip(string folderToCompress, string zipFileName, bool
             dllRecurse = false)
        {
            //logger.Debug("Start ==> AddFolderToArchive");
            bool result = false;
            
            try
            {
                if (Directory.Exists(folderToCompress))
                {
                    string dllFile;
                    if (dllRecurse == false)
                        dllFile = "7z.dll";
                    else
                        dllFile = "7z64.dll";
                    mLogger.Error("folderToCompress --> " + folderToCompress + Environment.NewLine + "archiveName --> " + zipFileName);
                    try
                    {
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dllFile))
                        {
                            SevenZipCompressor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dllFile);
                        }

                        else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + dllFile))
                        {
                            SevenZipCompressor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + dllFile);
                        }
                    }
                    catch {

                    }
                    
                    
                    SevenZipCompressor compressor = new SevenZipCompressor();
                    compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
                    compressor.DirectoryStructure = true;
                    compressor.PreserveDirectoryRoot = true;
                    //compressor.ZipEncryptionMethod = ZipEncryptionMethod.Aes256;
                    if (File.Exists(zipFileName))
                        File.Delete(zipFileName);
                    compressor.CompressionMode = CompressionMode.Create;
                    try
                    {
                        compressor.CompressDirectory(folderToCompress, zipFileName);
                    }
                    catch {
                        if (dllRecurse)
                            throw;
                        else {
                            mLogger.Error("Recurse called");
                            return Zip(folderToCompress, zipFileName, true);
                        }
                    }
                    //UnZip(zipFileName, folderToCompress + "Unzip");
                    result = true;
                }
            }
            catch (SevenZipCompressionFailedException sevenZipCompFailedEx)
            {
                result = false;
                mLogger.Error("Failed to add folder to archive. Exception : " + sevenZipCompFailedEx.ToString());
            }
            catch (SevenZipException sevenZipEx)
            {
                result = false;
                mLogger.Error("Failed to add folder to archive. Exception : " + sevenZipEx.ToString());
            }
            catch (Exception ex)
            {
                result = false;
                mLogger.Error("Failed to add folder to archive. Exception : " + ex.ToString());
            }
            return result;
        }

        public bool UnZip(string zipFileLocation, string targetLocation, bool dllRecurse = false)
        {
            mLogger.Error("zipFileLocation --> " + zipFileLocation + Environment.NewLine + "targetLocation --> " + targetLocation);
            try
            {
                string dllFile;
                if (dllRecurse == false)
                    dllFile = "7z.dll";
                else
                    dllFile = "7z64.dll";
                if (Directory.Exists(targetLocation))
                        Directory.Delete(targetLocation, true);
                    Directory.CreateDirectory(targetLocation);
                try
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dllFile))
                    {
                        SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dllFile);
                    }

                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + dllFile))
                    {
                        SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + dllFile);
                    }
                }
                catch {

                }
                try {
                    SevenZipExtractor sze = new SevenZipExtractor(zipFileLocation);
                    sze.ExtractArchive(targetLocation);
                }
                catch
                {
                    if (dllRecurse)
                        throw;
                    else {
                        return UnZip(zipFileLocation, targetLocation, true);
                        mLogger.Error("Recurse called");
                    }
                }

            }
            catch (SevenZipExtractionFailedException ex)
            {
                mLogger.Error("Failed to Unzip file. Error : " + ex.ToString());
                return false;
            }
            catch (SevenZipException ex)
            {
                mLogger.Error("Failed to Unzip file. Error : " + ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                mLogger.Error("Failed to Unzip file. Error : " + ex.ToString());
                return false;
            }
            return true;
        }

    }
}
