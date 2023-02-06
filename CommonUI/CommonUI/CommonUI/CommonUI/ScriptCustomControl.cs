using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace com.ivp.common.CommonUI
{
    public class ScriptCustomControl : WebControl, IScriptControl
    {
        private string _descriptorID = "";
        private string _targetClientComponent = "";
        private List<string> _scriptPath = new List<string>();
        private object _info = "";
        private object _controlInfo = "";
        private string _infoName = "";
        private string _controlInfoName = "";

        public string DescriptorID
        {
            get { return _descriptorID; }
            set { _descriptorID = value; }
        }
        public string TargetClientComponent
        {
            get { return _targetClientComponent; }
            set { _targetClientComponent = value; }
        }
        public List<string> ScriptPath
        {
            get { return _scriptPath; }
            set { _scriptPath = value; }
        }
        public object Info
        {
            get { return _info; }
            set { _info = value; }
        }
        public object ControlInfo
        {
            get { return _controlInfo; }
            set { _controlInfo = value; }
        }
        public string InfoName
        {
            get { return _infoName; }
            set { _infoName = value; }
        }
        public string ControlInfoName
        {
            get { return _controlInfoName; }
            set { _controlInfoName = value; }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                if (sm == null)
                    throw new HttpException("A ScriptManager control must exist on the current page.");
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                if (sm == null)
                    throw new HttpException("A ScriptManager control must exist on the current page.");
                sm.RegisterScriptControl(this);
            }
            base.OnPreRender(e);
        }
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptComponentDescriptor descriptor = new ScriptComponentDescriptor
                (_targetClientComponent);
            if (!string.IsNullOrEmpty(_descriptorID))
                descriptor.ID = _descriptorID;
            if (_info != null && !string.IsNullOrEmpty(_infoName))
                descriptor.AddProperty(_infoName, _info);
            if (_controlInfo != null && !string.IsNullOrEmpty(_controlInfoName))
                descriptor.AddProperty(_controlInfoName, _controlInfo);
            yield return descriptor;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            List<ScriptReference> lstObjScriptReference = new List<ScriptReference>();
            foreach (string scriptPath in _scriptPath)
            {
                ScriptReference objScriptReference = new ScriptReference(scriptPath);
                lstObjScriptReference.Add(objScriptReference);
            }
            return lstObjScriptReference;
        }
    }
}
