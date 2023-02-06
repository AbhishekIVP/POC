using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Collections;
using System.Xml.XPath;
using com.ivp.rad.RMarketWSO.TransactionService;

namespace com.ivp.rad.RMarketWSO
{
    class RTransactionUtils
    {
        internal static void FillRollover(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["Rollover"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);
           
            FillBaseData(resultantData.Tables["Rollover"], rolloverData, doc,"WSO_ROLLOVER");
            
            FillDataTag(resultantData.Tables["Rollover"], rolloverData, doc,"WSO_ROLLOVER/DATA");

            FillIssuerTag(resultantData.Tables["Rollover"], rolloverData, doc, "WSO_ROLLOVER/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["Rollover"], rolloverData, doc, "WSO_ROLLOVER/DATA/FACILITY");
            
            FillContractExistingTag(resultantData.Tables["Rollover"], rolloverData, doc, "WSO_ROLLOVER/DATA/CONTRACT_EXISTING");

            FillContractNewTag(resultantData.Tables["Rollover"], rolloverData, doc, "WSO_ROLLOVER/DATA/CONTRACT_NEW");

            FillCapitalizationTag(resultantData.Tables["Rollover"], rolloverData, doc, "WSO_ROLLOVER/DATA/CAPITALIZATION");

            resultantData.Tables["Rollover"].Rows.Add(rolloverData);
        }

        internal static void FillCommitmentIncrease(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["CommitmentIncrease"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["CommitmentIncrease"], rolloverData, doc, "WSO_COMMITMENT_INCREASE");
            
            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["CommitmentIncrease"], rolloverData, doc, "WSO_COMMITMENT_INCREASE/DATA");

            FillIssuerTag(resultantData.Tables["CommitmentIncrease"], rolloverData, doc, "WSO_COMMITMENT_INCREASE/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["CommitmentIncrease"], rolloverData, doc, "WSO_COMMITMENT_INCREASE/DATA/FACILITY");

            resultantData.Tables["CommitmentIncrease"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_COMMITMENT_INCREASE/DATA/CONTRACT_EXISTING");
        }

        internal static void FillCommitmentReduction(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["CommitmentReduction"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["CommitmentReduction"], rolloverData, doc, "WSO_COMMITMENT_REDUCTION");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["CommitmentReduction"], rolloverData, doc, "WSO_COMMITMENT_REDUCTION/DATA");

            FillIssuerTag(resultantData.Tables["CommitmentReduction"], rolloverData, doc, "WSO_COMMITMENT_REDUCTION/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["CommitmentReduction"], rolloverData, doc, "WSO_COMMITMENT_REDUCTION/DATA/FACILITY");

            resultantData.Tables["CommitmentReduction"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_COMMITMENT_REDUCTION/DATA/CONTRACT_EXISTING");
        }

        internal static void FillPaydownFullPaydownEarly(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["PaydownFullPaydownEarly"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["PaydownFullPaydownEarly"], rolloverData, doc, "WSO_PAYDOWN");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["PaydownFullPaydownEarly"], rolloverData, doc, "WSO_PAYDOWN/DATA");

            FillIssuerTag(resultantData.Tables["PaydownFullPaydownEarly"], rolloverData, doc, "WSO_PAYDOWN/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["PaydownFullPaydownEarly"], rolloverData, doc, "WSO_PAYDOWN/DATA/FACILITY");

            resultantData.Tables["PaydownFullPaydownEarly"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_PAYDOWN/DATA/CONTRACT_EXISTING");
        }

        internal static void FillPaydownPartial(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["PaydownPartial"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["PaydownPartial"], rolloverData, doc, "WSO_PAYDOWN_PARTIAL");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["PaydownPartial"], rolloverData, doc, "WSO_PAYDOWN_PARTIAL/DATA");

            FillIssuerTag(resultantData.Tables["PaydownPartial"], rolloverData, doc, "WSO_PAYDOWN_PARTIAL/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["PaydownPartial"], rolloverData, doc, "WSO_PAYDOWN_PARTIAL/DATA/FACILITY");

            FillContractNewTag(resultantData.Tables["PaydownPartial"], rolloverData, doc, "WSO_PAYDOWN_PARTIAL/DATA/CONTRACT_NEW"); 
            
            resultantData.Tables["PaydownPartial"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_PAYDOWN_PARTIAL/DATA/CONTRACT_EXISTING");
        }

        internal static void FillCombine(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["Combine"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE/DATA");

            FillIssuerTag(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE/DATA/FACILITY");

            FillContractNewTag(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE/DATA/CONTRACT_NEW");

            FillCapitalizationTag(resultantData.Tables["Combine"], rolloverData, doc, "WSO_COMBINE/DATA/CAPITALIZATION");

            resultantData.Tables["Combine"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_COMBINE/DATA/CONTRACT_EXISTING");
        }

        internal static void FillConvertContract(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["ConvertContract"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION/DATA");

            FillIssuerTag(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION/DATA/FACILITY");

            FillContractNewTag(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION/DATA/CONTRACT_NEW");

            FillCapitalizationTag(resultantData.Tables["ConvertContract"], rolloverData, doc, "WSO_CONVERSION/DATA/CAPITALIZATION");

            resultantData.Tables["ConvertContract"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_CONVERSION/DATA/CONTRACT_EXISTING");
        }

        internal static void FillReceiveInterimInterest(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["ReceiveInterimInterest"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST");

            FillDataTag(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST/DATA");

            FillIssuerTag(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST/DATA/CONTRACT_EXISTING");

            FillCapitalizationTag(resultantData.Tables["ReceiveInterimInterest"], rolloverData, doc, "WSO_RECEIVE_INTERIM_INTEREST/DATA/CAPITALIZATION");

            resultantData.Tables["ReceiveInterimInterest"].Rows.Add(rolloverData);
        }

        internal static void FillReceiveOldInterest(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["ReceiveOldInterest"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST");

            FillDataTag(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST/DATA");

            FillIssuerTag(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST/DATA/CONTRACT_EXISTING");

            FillCapitalizationTag(resultantData.Tables["ReceiveOldInterest"], rolloverData, doc, "WSO_RECEIVE_OLD_INTEREST/DATA/CAPITALIZATION");

            resultantData.Tables["ReceiveOldInterest"].Rows.Add(rolloverData);
        }

        internal static void FillRollback(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["Rollback"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["Rollback"], rolloverData, doc, "WSO_ROLLBACK");

            FillDataTag(resultantData.Tables["Rollback"], rolloverData, doc, "WSO_ROLLBACK/DATA");

            resultantData.Tables["Rollback"].Rows.Add(rolloverData);
        }

        internal static void FillSplitContract(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["SplitContract"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT/DATA");

            FillIssuerTag(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT/DATA/CONTRACT_EXISTING");

            FillCapitalizationTag(resultantData.Tables["SplitContract"], rolloverData, doc, "WSO_SPLIT/DATA/CAPITALIZATION");

            resultantData.Tables["SplitContract"].Rows.Add(rolloverData);

            FillContractNewTableTag(resultantData.Tables["ContractNew"], globalTranId, doc, "WSO_SPLIT/DATA/CONTRACT_NEW");
        }

        internal static void FillFacilityIncrease(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["FacilityIncrease"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["FacilityIncrease"], rolloverData, doc, "WSO_FACILITY_INCREASE");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["FacilityIncrease"], rolloverData, doc, "WSO_FACILITY_INCREASE/DATA");

            FillIssuerTag(resultantData.Tables["FacilityIncrease"], rolloverData, doc, "WSO_FACILITY_INCREASE/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["FacilityIncrease"], rolloverData, doc, "WSO_FACILITY_INCREASE/DATA/FACILITY");

            resultantData.Tables["FacilityIncrease"].Rows.Add(rolloverData);

            FillContractNewTableTag(resultantData.Tables["ContractNew"], globalTranId, doc, "WSO_FACILITY_INCREASE/DATA/CONTRACT_NEW");

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_FACILITY_INCREASE/DATA/CONTRACT_EXISTING");
        }

        internal static void FillFXAdjustment(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["FXAdjustment"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["FXAdjustment"], rolloverData, doc, "WSO_FX_ADJUSTMENT");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["FXAdjustment"], rolloverData, doc, "WSO_FX_ADJUSTMENT/DATA");

            FillIssuerTag(resultantData.Tables["FXAdjustment"], rolloverData, doc, "WSO_FX_ADJUSTMENT/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["FXAdjustment"], rolloverData, doc, "WSO_FX_ADJUSTMENT/DATA/FACILITY");

            resultantData.Tables["FXAdjustment"].Rows.Add(rolloverData);

            FillContractNewTableTag(resultantData.Tables["ContractNew"], globalTranId, doc, "WSO_FX_ADJUSTMENT/DATA/CONTRACT_NEW");

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_FX_ADJUSTMENT/DATA/CONTRACT_EXISTING");
        }

        internal static void FillBorrow(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["Borrow"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["Borrow"], rolloverData, doc, "WSO_BORROW");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["Borrow"], rolloverData, doc, "WSO_BORROW/DATA");

            FillIssuerTag(resultantData.Tables["Borrow"], rolloverData, doc, "WSO_BORROW/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["Borrow"], rolloverData, doc, "WSO_BORROW/DATA/FACILITY");

            FillContractNewTag(resultantData.Tables["Borrow"], rolloverData, doc, "WSO_BORROW/DATA/CONTRACT_NEW");

            resultantData.Tables["Borrow"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_BORROW/DATA/CONTRACT_EXISTING");
        }

        internal static void FillBorrowEarly(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["BorrowEarly"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["BorrowEarly"], rolloverData, doc, "WSO_BORROW_EARLY");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["BorrowEarly"], rolloverData, doc, "WSO_BORROW_EARLY/DATA");

            FillIssuerTag(resultantData.Tables["BorrowEarly"], rolloverData, doc, "WSO_BORROW_EARLY/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["BorrowEarly"], rolloverData, doc, "WSO_BORROW_EARLY/DATA/FACILITY");

            resultantData.Tables["BorrowEarly"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_BORROW_EARLY/DATA/CONTRACT_EXISTING");
        }

        internal static void FillBorrowNew(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["BorrowNew"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["BorrowNew"], rolloverData, doc, "WSO_BORROW_NEW");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["BorrowNew"], rolloverData, doc, "WSO_BORROW_NEW/DATA");

            FillIssuerTag(resultantData.Tables["BorrowNew"], rolloverData, doc, "WSO_BORROW_NEW/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["BorrowNew"], rolloverData, doc, "WSO_BORROW_NEW/DATA/FACILITY");

            FillContractNewTag(resultantData.Tables["BorrowNew"], rolloverData, doc, "WSO_BORROW_NEW/DATA/CONTRACT_NEW");

            resultantData.Tables["BorrowNew"].Rows.Add(rolloverData);

            FillContractExistingTableTag(resultantData.Tables["ContractExisting"], globalTranId, doc, "WSO_BORROW_NEW/DATA/CONTRACT_EXISTING");
        }

        internal static void FillProcessReceivable(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["ProcessReceivable"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["ProcessReceivable"], rolloverData, doc, "WSO_PROCESS_RECEIVABLE");

            FillDataTag(resultantData.Tables["ProcessReceivable"], rolloverData, doc, "WSO_PROCESS_RECEIVABLE/DATA");

            FillIssuerTag(resultantData.Tables["ProcessReceivable"], rolloverData, doc, "WSO_PROCESS_RECEIVABLE/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["ProcessReceivable"], rolloverData, doc, "WSO_PROCESS_RECEIVABLE/DATA/FACILITY");

            resultantData.Tables["ProcessReceivable"].Rows.Add(rolloverData);
        }
        
        internal static void FillAccrualAdjustment(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["AccrualAdjustment"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["AccrualAdjustment"], rolloverData, doc, "WSO_ACCRUAL_ADJUSTMENT");

            FillDataTag(resultantData.Tables["AccrualAdjustment"], rolloverData, doc, "WSO_ACCRUAL_ADJUSTMENT/DATA");

            FillIssuerTag(resultantData.Tables["AccrualAdjustment"], rolloverData, doc, "WSO_ACCRUAL_ADJUSTMENT/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["AccrualAdjustment"], rolloverData, doc, "WSO_ACCRUAL_ADJUSTMENT/DATA/FACILITY");

            resultantData.Tables["AccrualAdjustment"].Rows.Add(rolloverData);
        }

        internal static void FillMiscellaneousFees(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["MiscellaneousFees"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["MiscellaneousFees"], rolloverData, doc, "WSO_MISCELLANEOUS_FEES");

            FillDataTag(resultantData.Tables["MiscellaneousFees"], rolloverData, doc, "WSO_MISCELLANEOUS_FEES/DATA");

            FillIssuerTag(resultantData.Tables["MiscellaneousFees"], rolloverData, doc, "WSO_MISCELLANEOUS_FEES/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["MiscellaneousFees"], rolloverData, doc, "WSO_MISCELLANEOUS_FEES/DATA/FACILITY");

            resultantData.Tables["MiscellaneousFees"].Rows.Add(rolloverData);
        }

        internal static void FillRestructuring(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["Restructuring"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["Restructuring"], rolloverData, doc, "WSO_RESTRUCTURING");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["Restructuring"], rolloverData, doc, "WSO_RESTRUCTURING/DATA");

            FillIssuerTag(resultantData.Tables["Restructuring"], rolloverData, doc, "WSO_RESTRUCTURING/DATA/ISSUER");

            resultantData.Tables["Restructuring"].Rows.Add(rolloverData);

            FillFacilityTableTag(resultantData.Tables["FacilityNew"], globalTranId, doc, "WSO_RESTRUCTURING/DATA/FACILITY_NEW");

            FillFacilityTableTag(resultantData.Tables["FacilityExisting"], globalTranId, doc, "WSO_RESTRUCTURING/DATA/FACILITY_EXISTING");
        }

        internal static void FillIssuerUpdate(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["IssuerUpdate"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["IssuerUpdate"], rolloverData, doc, "WSO_ISSUER_UPDATE");

            FillDataTag(resultantData.Tables["IssuerUpdate"], rolloverData, doc, "WSO_ISSUER_UPDATE/DATA");

            FillIssuerTag(resultantData.Tables["IssuerUpdate"], rolloverData, doc, "WSO_ISSUER_UPDATE/DATA/ISSUER");

            FillIssuerTag(resultantData.Tables["IssuerUpdate"], rolloverData, doc, "WSO_ISSUER_UPDATE/DATA/ISSUER_EXISTING");

            resultantData.Tables["IssuerUpdate"].Rows.Add(rolloverData);
        }

        internal static void FillBankDealUpdate(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rolloverData = resultantData.Tables["BankDealUpdate"].NewRow();
            rolloverData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["BankDealUpdate"], rolloverData, doc, "WSO_BANK_DEAL_UPDATE");

            string globalTranId = rolloverData["globtranid"].ToString();

            FillDataTag(resultantData.Tables["BankDealUpdate"], rolloverData, doc, "WSO_BANK_DEAL_UPDATE/DATA");

            FillIssuerTag(resultantData.Tables["BankDealUpdate"], rolloverData, doc, "WSO_BANK_DEAL_UPDATE/DATA/ISSUER");
            
            FillBankDealTag(resultantData.Tables["BankDealUpdate"], rolloverData, doc, "WSO_BANK_DEAL_UPDATE/DATA/BANK_DEAL","BankDeal");

            FillBankDealTag(resultantData.Tables["BankDealUpdate"], rolloverData, doc, "WSO_BANK_DEAL_UPDATE/DATA/BANK_DEAL_EXISTING", "BankDealExisting");

            resultantData.Tables["BankDealUpdate"].Rows.Add(rolloverData);

            FillFacilityTableTag(resultantData.Tables["Facility"], globalTranId, doc, "WSO_BANK_DEAL_UPDATE/DATA/FACILITY");
        }

        internal static void FillBuy(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow buyData = resultantData.Tables["Buy"].NewRow();
            buyData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["Buy"], buyData, doc, "WSO_BUY");

            FillDataTag(resultantData.Tables["Buy"], buyData, doc, "WSO_BUY/DATA");

            FillIssuerTag(resultantData.Tables["Buy"], buyData, doc, "WSO_BUY/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["Buy"], buyData, doc, "WSO_BUY/DATA/FACILITY");

            resultantData.Tables["Buy"].Rows.Add(buyData);
        }

        internal static void FillCloseContract(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow closeContractData = resultantData.Tables["CloseContract"].NewRow();
            closeContractData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["CloseContract"], closeContractData, doc, "WSO_CLOSE_CONTRACT");

            FillDataTag(resultantData.Tables["CloseContract"], closeContractData, doc, "WSO_CLOSE_CONTRACT/DATA");

            FillIssuerTag(resultantData.Tables["CloseContract"], closeContractData, doc, "WSO_CLOSE_CONTRACT/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["CloseContract"], closeContractData, doc, "WSO_CLOSE_CONTRACT/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["CloseContract"], closeContractData, doc, "WSO_CLOSE_CONTRACT/DATA/CONTRACT_EXISTING");

            resultantData.Tables["CloseContract"].Rows.Add(closeContractData);
        }

        internal static void FillUpdateContract(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow updateContractData = resultantData.Tables["CloseContract"].NewRow();
            updateContractData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT");

            FillDataTag(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT/DATA");

            FillIssuerTag(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT/DATA/CONTRACT_EXISTING");

            FillContractUpdatingTag(resultantData.Tables["UpdateContract"], updateContractData, doc, "WSO_CORRECT_CONTRACT/DATA/CONTRACT_CORRECTED");

            resultantData.Tables["UpdateContract"].Rows.Add(updateContractData);
        }

        internal static void FillRateChange(DataSet resultantData, Transaction transaction, int facilityId)
        {
            DataRow rateChangeData = resultantData.Tables["RateChange"].NewRow();
            rateChangeData["Identifier Value"] = facilityId;
            XmlDocument docs = new XmlDocument();
            docs.LoadXml(transaction.Content);
            XDocument doc = XDocument.Parse(docs.OuterXml);

            FillBaseData(resultantData.Tables["RateChange"], rateChangeData, doc, "WSO_RATE_CHANGE");

            FillDataTag(resultantData.Tables["RateChange"], rateChangeData, doc, "WSO_RATE_CHANGE/DATA");

            FillIssuerTag(resultantData.Tables["RateChange"], rateChangeData, doc, "WSO_RATE_CHANGE/DATA/ISSUER");

            FillFacilityTag(resultantData.Tables["RateChange"], rateChangeData, doc, "WSO_RATE_CHANGE/DATA/FACILITY");

            FillContractExistingTag(resultantData.Tables["RateChange"], rateChangeData, doc, "WSO_RATE_CHANGE/DATA/CONTRACT_EXISTING");

            resultantData.Tables["RateChange"].Rows.Add(rateChangeData);
        }

        private static void FillFacilityTableTag(DataTable dataTable, string globalTranId, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                DataRow newRow = dataTable.NewRow();
                newRow["globtranid"] = globalTranId;
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (dataTable.Columns.Contains(enumerator.Current.Name.ToString()))
                        newRow[enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
                var childCollection =node.Descendants("SECID");
                childCollection.Cast<XElement>().ToList().ForEach(childNode =>
                {
                    if (dataTable.Columns.Contains(childNode.Attribute("type").Value))
                        newRow[childNode.Attribute("type").Value] = childNode.Attribute("value").Value;
                });

                dataTable.Rows.Add(newRow);
            });
        }
        
        private static void FillContractNewTableTag(DataTable dataTable, string globalTranId, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                DataRow newRow = dataTable.NewRow();
                newRow["globtranid"] = globalTranId; 
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (dataTable.Columns.Contains(enumerator.Current.Name.ToString()))
                        newRow[enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
                
                dataTable.Rows.Add(newRow);
            });
        }
        
        private static void FillContractExistingTableTag(DataTable dataTable, string globalTranId, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                DataRow newRow = dataTable.NewRow();
                newRow["globtranid"] = globalTranId; 
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (dataTable.Columns.Contains(enumerator.Current.Name.ToString()))
                        newRow[enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
               var childCollection = node.Descendants("RECEIVE_INTEREST");
               childCollection.ToList().ForEach(child =>
               {
                   var enumerator1 = node.Attributes().GetEnumerator();
                   while (enumerator1.MoveNext())
                   {
                       if (dataTable.Columns.Contains("ContractExisting ReceiveInterest " + enumerator.Current.Name.ToString()))
                           newRow["ContractExisting ReceiveInterest " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                   }
               });
               dataTable.Rows.Add(newRow);
            });
        }

        private static void FillBaseData(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains(enumerator.Current.Name.ToString()))
                        rolloverData[enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillDataTag(DataTable resultantTable, DataRow rolloverData, XDocument doc,string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains(resultantTable.TableName + "Data " + enumerator.Current.Name.ToString()))
                        rolloverData[resultantTable.TableName + "Data " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillIssuerTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("Issuer " + enumerator.Current.Name.ToString()))
                        rolloverData["Issuer " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillBankDealTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath, string tag)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains(tag + " " + enumerator.Current.Name.ToString()))
                        rolloverData[tag + " " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillIssuerExistingTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("IssuerExisting " + enumerator.Current.Name.ToString()))
                        rolloverData["IssuerExisting " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillFacilityTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("Facility " + enumerator.Current.Name.ToString()))
                        rolloverData["Facility " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/SECID");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                if (resultantTable.Columns.Contains("Facility " + node.Attribute("type").Value))
                    rolloverData["Facility " + node.Attribute("type").Value] = node.Attribute("value").Value;
            });
        }

        private static void FillContractExistingTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("ContractExisting " + enumerator.Current.Name.ToString()))
                        rolloverData["ContractExisting " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/RECEIVE_INTEREST");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("ContractExisting ReceiveInterest " + enumerator.Current.Name.ToString()))
                        rolloverData["ContractExisting ReceiveInterest " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillContractUpdatingTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("ContractUpdating " + enumerator.Current.Name.ToString()))
                        rolloverData["ContractUpdating " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/RECEIVE_INTEREST");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("ContractUpdating ReceiveInterest " + enumerator.Current.Name.ToString()))
                        rolloverData["ContractUpdating ReceiveInterest " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillContractNewTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("ContractNew " + enumerator.Current.Name.ToString()))
                        rolloverData["ContractNew " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });
        }

        private static void FillCapitalizationTag(DataTable resultantTable, DataRow rolloverData, XDocument doc, string xPath)
        {
            IEnumerable collection = (IEnumerable)doc.XPathEvaluate(xPath);
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("Capitalization " + enumerator.Current.Name.ToString()))
                        rolloverData["Capitalization " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/CONTRACT");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("Capitalization Contract " + enumerator.Current.Name.ToString()))
                        rolloverData["Capitalization Contract " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/CONTRACT/FACILITY");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                var enumerator = node.Attributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (resultantTable.Columns.Contains("Capitalization Contract Facility " + enumerator.Current.Name.ToString()))
                        rolloverData["Capitalization Contract Facility " + enumerator.Current.Name.ToString()] = enumerator.Current.Value;
                }
            });

            collection = (IEnumerable)doc.XPathEvaluate(xPath + "/CONTRACT/FACILITY/SECID");
            collection.Cast<XElement>().ToList().ForEach(node =>
            {
                if (resultantTable.Columns.Contains("Capitalization Contract Facility " + (node.Attribute("type").Value)))
                    rolloverData["Capitalization Contract Facility " + node.Attribute("type").Value] = node.Attribute("value").Value;
            });
        }






    }
}
