using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.ivp.rad.RMarketWSO
{
    public class AgentBankInfo
    {
        #region "Members"
        private string _markitAgentBankID;
        private string _agentBankMember;
        private string _agentContact;
        private string _agentPhone;
        private string _agentMail;
        #endregion

        #region "Properties"

        public string AgentBankID
        {
            get { return _markitAgentBankID; }
            set { _markitAgentBankID = value; }
        }

        public string AgentBankMember
        {
            get { return _agentBankMember; }
            set { _agentBankMember = value; }
        }

        public string AgentBankContact
        {
            get { return _agentContact; }
            set { _agentContact = value; }
        }

        public string AgentBankPhone
        {
            get { return _agentPhone; }
            set { _agentPhone = value; }
        }

        public string AgentBankEmail
        {
            get { return _agentMail; }
            set { _agentMail = value; }
        }

        

        #endregion
    }
}