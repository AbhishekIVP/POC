/***************************************************************************************************
 * 
 *  This source forms a part of the IVP RAD Software System and is a copyright of 
 *  Indus Valley Partners (Pvt) Ltd.

 *  All rights are reserved with IVP. No part of this work may be reproduced, stored, 
 *  adopted or transmitted in any form or by any means including but not limiting to 
 *  electronic, mechanical, photographic, graphic, optic recording or otherwise, 
 *  translated in any language or computer language, without the prior written permission 
 *  of

 *  Indus Valley Partners (Pvt) Ltd
 *  Unit 7&8, Bldg 4
 *  Vardhman Trade Center
 *  Nehru Place Greens
 *  New Delhi - 19

 *  Copyright 2007-2008 Indus Valley Partners (Pvt) Ltd.
 * 
 * 
 * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            24-10-2008      Nitin Saxena    Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Vendor Constants
    /// </summary>
    internal class RVendorConstant
    {
        #region class constant members
        internal const string CERTIFICATEPATH = "certificatepath";
        internal const string PASSWORD = "password";
        internal const string WEBSERVICEURLLITE = "webserviceurllite";
        internal const string WEBSERVICEURLHEAVY = "webserviceurlheavy";
        internal const string SERVERURI = "serveruri";
        internal const string API_AUTH_SVC_NAME = "authorizationapi";
        internal const string REF_DATA_SVC_NAME = "referencedataapi";
        internal const string SAPI_VALIDATE_SESSION = "sapivalidatesession";
        internal const string SERVERHOST = "serverhost";
        internal const string SERVERPORT = "serverport";
        internal const string MAXSECLITE = "maxseclite";
        internal const string MAXSECHEAVY = "maxsecheavy";
        internal const string MAXSECFTP = "maxsecftp";
        internal const string MAXSECSERVER = "maxsecserver";
        internal const string MAXFIELDSLITE = "maxfieldslite";
        internal const string MAXFIELDSHEAVY = "maxfieldsheavy";
        internal const string MAXFIELDSFTP = "maxfieldsftp";
        internal const string MAXFIELDSSERVER = "maxfieldsserver";
        internal const string USERID = "userid";
        internal const string REPLYFILENAME = "replyfilename";
        internal const string REQUESTFILENAME = "requestfilename";
        internal const string WORKINGDIRECTORY = "workingdirectory";
        internal const string FTPDECRPYPTKEY = "ftpdecryptkey";
        internal const string FTPDECRPYPTFILENAME = "ftpdecryptfilename";
        internal const string FTPDECRYPTTOOLPATH = "ftpdecrypttoolpath";
        internal const string FTPFOLDERNAME = "ftpfoldername";
        internal const string FTPDECRYPTCOMMAND = "ftpdecryptcommand";
        internal const string FTPTIMEZONE = "ftptimezone";
        internal const string SERVICEURL = "serviceurl";
        internal const string INPUT_DIRECTORY = "inputdirectory";
        internal const string COPY_DIRECTORY = "copydirectory";
        internal const string REPORTS_DIRECTORY = "reportsdirectory";
        internal const string NOT_AVAILABLE = "Not Available";
        internal const string FLD_UNKNOWN = "FLD UNKNOWN";
        internal const string NOT_PROCESSED = "NOT PROCESSED";
        internal const string USER_NUMBER = "USERNUMBER";
        internal const string WS = "WS";
        internal const string CREDIT_RISK = "CREDITRISK";
        internal const string FUNDAMENTALS = "FUNDAMENTALS";
        internal const string ESTIMATES = "ESTIMATES";
        internal const string CLOSING_VALUES = "CLOSINGVALUES";
        internal const string DERIVED = "DERIVED";
        internal const string SKIP_PCS = "SKIP_PCS";
        internal const string SERIAL_NUMBER = "SN";
        internal const string CORPSTRUCT = "CORPSTRUCT";
        internal const string PRICING_SOURCE = "PRICING_SOURCE";
        internal const string PROGRAMFLAG = "PROGRAMFLAG";
        internal const string HISTORICAL = "HISTORICAL";
        internal const string USER_ID = "userid";
        internal const string SAPIUUID = "uuid";
        internal const string IPADDRESS = "ipAddress";
        internal const string SECURITIESELEMENTFORSAPI = "securities";
        internal const string FIELDSELEMENTFORSAPI = "fields";
        internal const string ELEMENTEIDSFORSAPI = "returnEids";
        internal const string OPERATIONNAMEREFDATA = "ReferenceDataRequest";
        internal const string SECURITYERROR = "securityError";
        internal const string NOTAVIALABLE = "not available";

        internal const string MANAGEDSERVERHOST = "managedbpipeserverhost";
        internal const string MANAGEDSERVERPORT = "managedbpipeserverport";
        internal const string MANAGEDAUTHENTICATION = "managedauthentication";
        internal const string MANAGEDAPPNAME = "managedappname";

        internal const string GLOBALSERVERHOST = "globalapiserverhost";
        internal const string GLOBALSERVERPORT = "globalapiserverport";
        internal const string GLOBALAUTHENTICATION = "globalapiauthentication";
        internal const string GLOBALAPPNAME = "globalapiappname";

        internal const string ENABLE_BBG_AUDIT = "enablebbgaudit";

        internal const string REPORT_TEMPLATE_NAME = "ReportTemplateName";
        internal const string INPUT_LIST_NAME = "InputListName";
        internal const string REPORT_TEMPLATE_TAG = "ReportTemplateTag";
        internal const string SCHEDULE_NAME = "ScheduleName";
        internal const string SCHEDULE_TAG = "ScheduleTag";

        internal const string TRANSPORTNAME = "transportname";
        internal const string GETHISTORYTRANSPORTNAME = "gethistorytransportname";
        internal const string GETFUNDAMENTALSTRANSPORTNAME = "getfundamentalstransportname";
        internal const string GETACTIONSTRANSPORTNAME = "getactionstransportname";
        #endregion
    }

    /// <summary>
    /// Class for Vendor Error Constants
    /// </summary>
    internal class RVendorErrorConstant
    {
        internal const string MAXSECEXCEED = "maxsecexceed";
        internal const string MAXFIELDSEXCEED = "maxfieldsexceed";
    }

    /// <summary>
    /// Bloomberg Vendor Constant for Per Security Heavy Request
    /// </summary>
    internal class RVendorHeavyStatusConstant
    {
        internal const int DATANOTAVAILABLE = 100;
        internal const int SUCCESS = 0;
        internal const int REQUESTERROR = 200;
        internal const int POLLINTERVAL = 100;
    }

    internal class RVendorStatusConstant
    {
        internal const string PASSED = "PASSED";
        internal const string FAILED = "FAILED";
        internal const string PENDING = "PENDING";
        internal const string PROCESSED = "PROCESSED";
        internal const string REQUEST_REGISTERED = "REQUEST REGISTERED";
    }

    internal class RBbgBulkListConstant
    {
        internal const string BULKLISTMAPID = "bulklist_mapping_id";
        internal const string REQUESTFIELD = "request_field";
        internal const string OUTPUTFIELDS = "output_fields";
        internal const string CREATEDBY = "created_by";
        internal const string MARKETSECTOR = "market_sector";
    }

    internal class RBbgUUIDConstant
    {
        internal const string SAPIUUID = "uuid";
        internal const string USERNAME = "user_name";
        internal const string EMRSID = "emrsid";
    }
    internal class RCorpActionConstant
    {
        internal const string COMMA = ",";
        internal const string DATEFORMAT = "{0:MMddyyHHmmssfff}";
        internal const string REQ = "req";
        internal const string RES = "res";
        internal const string DOTREQ = ".req";
        internal const string DOTRES = ".res";
        internal const string DECRYPT="decrypt";
        internal const string DOTTXT = ".txt";
        internal const string BACKSLASH = "//";
        internal const string DOTRESDOTGZ = ".res.gz";
        internal const string WHITESPACE = " ";
        internal const string DOTGZ = ".gz";
        internal const string SINGLEQOUTE = "\"";
        internal const string GETHISTORYFTP = "GetHistory-FTP";
        internal const string CORPACTIONFTP = "CorpAction-FTP";
        internal const string BLOOMBERG = "Bloomberg";
        internal const string NAME = "name";
        internal const string INSTRUMENT = "Instrument";
        internal const string MARKET_SECTOR = "Market_Sector";
        internal const string FIELD = "Field";
        internal const string CORP_ACTION = "Corp_Action";
        internal const string BLOOMBERG_COMPANY_ID = "Bloomberg Company ID";
        internal const string PIPE = "|";
        internal const string NA = "N.A.";
        internal const string STARTOFDATA = "START-OF-DATA";
        internal const string ENDOFDATA = "END-OF-DATA";
        internal const string STARTOFFILE = "START-OF-FILE";
        internal const string ENDOFFILE = "END-OF-FILE";
        internal const string IS_VALID = "Is_Valid";
        internal const string DATEOFELEMENT = "DateOfElement";
        internal const string STARTOFFIELDS = "START-OF-FIELDS";
        internal const string ENDOFFIELDS = "END-OF-FIELDS";
        internal const string FIRMNAME = "FIRMNAME=";
        internal const string DATERANGE = "DATERANGE=";
        internal const string REPLYFILENAME = "REPLYFILENAME=";
        internal const string YYYYMMDD = "yyyyMMdd";
        internal const string VALUE = "value";
        internal const string INSTRUMENTNAME = "Instrument Name";
        internal const string BLOOMBERGSECURITYID = "Bloomberg Security ID";
        internal const string RCODE = "Rcode";
        internal const string ACTIONID = "Action ID";
        internal const string MNEMONIC = "Mnemonic";
        internal const string FLAG = "Flag";
        internal const string COMPANYNAME = "Company Name";
        internal const string SECIDTYPE = "SecID type";
        internal const string SECID = "SecID";
        internal const string CURRENCY = "Currency";
        internal const string BLOOMBERGUNIQUEID = "Bloomberg Unique ID";
        internal const string ANNDATE = "Ann-date";
        internal const string EFFDATE = "Eff-date";
        internal const string AMDDATE = "Amd-date";
        internal const string BLOOMBERGGLOBALID = "Bloomberg Global ID";
        internal const string BLOOMBERGGLOBALCOMPANYID = "Bloomberg Global Company ID";
        internal const string BLOOMBERGSECURITYIDNUMBERDESCRIPTION = "Bloomberg Security ID Number Description";
        internal const string FEEDSOURCE = "Feed Source";
    }
}
