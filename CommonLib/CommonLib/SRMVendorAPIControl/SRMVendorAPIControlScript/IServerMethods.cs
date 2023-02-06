using System;

namespace com.ivp.srm.controls.vendor.script
{
    /// <summary>
    /// interface for the server side methods
    /// </summary>
    public interface IServerMethods
    {
        void RBbgRequestType();
        void RBbgInstrumentIdType();
        void RBbgMarketSector();

        void RReuterRequestType();
        void RWSOInstrumentIdType();
        void RReuterInstrumentIdType();
        void RReuterAssetTypes();

        void GetAllTransports();

        void GetRequestType(int vendorType, int licenseType);

        void BindApplicationSpecificData(string className, string methodName, string assembly, 
            string id, string nameSpace);

    }

    /// <summary>
    /// info class for all the server side method names
    /// </summary>
    public class ServerMethodNames
    {
        internal const string RBbgRequestType = "RBbgRequestType";
        internal const string RBbgInstrumentIdType = "RBbgInstrumentIdType";
        internal const string RBbgMarketSector = "RBbgMarketSector";
        internal const string BindShuttleForIdentifiers = "BindShuttleForIdentifiers";
        internal const string RReuterRequestType = "RReuterRequestType";
        internal const string RReuterInstrumentIdType = "RReuterInstrumentIdType";
        internal const string RReuterAssetTypes = "RReuterAssetTypes";
        internal const string RWSOInstrumentIdType = "RWSOInstrumentIdType";
        internal const string GetAllTransports = "GetAllTransports";
        internal const string GetRequestType = "GetRequestType";

       
    }
}
