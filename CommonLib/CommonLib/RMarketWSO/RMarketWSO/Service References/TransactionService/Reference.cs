//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace com.ivp.rad.RMarketWSO.TransactionService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TransactionResults", Namespace="http://markit.com/Markit/Transaction/2012/06")]
    [System.SerializableAttribute()]
    public partial class TransactionResults : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Nullable<System.Guid> GroupIdField;
        
        private com.ivp.rad.RMarketWSO.TransactionService.Transaction[] TransactionsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Nullable<System.Guid> GroupId {
            get {
                return this.GroupIdField;
            }
            set {
                if ((this.GroupIdField.Equals(value) != true)) {
                    this.GroupIdField = value;
                    this.RaisePropertyChanged("GroupId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public com.ivp.rad.RMarketWSO.TransactionService.Transaction[] Transactions {
            get {
                return this.TransactionsField;
            }
            set {
                if ((object.ReferenceEquals(this.TransactionsField, value) != true)) {
                    this.TransactionsField = value;
                    this.RaisePropertyChanged("Transactions");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Transaction", Namespace="http://markit.com/Markit/Transaction/2012/06")]
    [System.SerializableAttribute()]
    public partial class Transaction : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int IdField;
        
        private string TypeField;
        
        private string ContentField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public string Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GeneralServiceFault", Namespace="http://markit.com/Markit/Transaction/2012/06")]
    [System.SerializableAttribute()]
    public partial class GeneralServiceFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Guid idField;
        
        private string messageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Guid id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string message {
            get {
                return this.messageField;
            }
            set {
                if ((object.ReferenceEquals(this.messageField, value) != true)) {
                    this.messageField = value;
                    this.RaisePropertyChanged("message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", ConfigurationName="TransactionService.TransactionService", SessionMode=System.ServiceModel.SessionMode.NotAllowed)]
    public interface TransactionService {
        
        // CODEGEN: Generating message contract since the wrapper name (GetTransactionsByFacilityRequest) of message GetTransactionsByFacilityRequest does not match the default value (GetTransactionsByFacility)
        [System.ServiceModel.OperationContractAttribute(Action="http://markit.com/Markit/Transaction/2012/06/GetTransactionsByFacility", ReplyAction="http://markit.com/Markit/Transaction/2012/06/TransactionService/GetTransactionsBy" +
            "FacilityResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(com.ivp.rad.RMarketWSO.TransactionService.GeneralServiceFault), Action="http://markit.com/Markit/Transaction/2012/06/TransactionService/GetTransactionsBy" +
            "FacilityGeneralServiceFaultFault", Name="GeneralServiceFault")]
        com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityResponse GetTransactionsByFacility(com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityRequest request);
        
        // CODEGEN: Generating message contract since the wrapper name (ConfirmTransactionsRequest) of message ConfirmTransactionsRequest does not match the default value (ConfirmTransactions)
        [System.ServiceModel.OperationContractAttribute(Action="http://markit.com/Markit/Transaction/2012/06/ConfirmTransactions", ReplyAction="http://markit.com/Markit/Transaction/2012/06/TransactionService/ConfirmTransactio" +
            "nsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(com.ivp.rad.RMarketWSO.TransactionService.GeneralServiceFault), Action="http://markit.com/Markit/Transaction/2012/06/TransactionService/ConfirmTransactio" +
            "nsGeneralServiceFaultFault", Name="GeneralServiceFault")]
        com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsResponse ConfirmTransactions(com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsRequest request);
        
        // CODEGEN: Generating message contract since the wrapper name (RejectTransactionsRequest) of message RejectTransactionsRequest does not match the default value (RejectTransactions)
        [System.ServiceModel.OperationContractAttribute(Action="http://markit.com/Markit/Transaction/2012/06/RejectTransactions", ReplyAction="http://markit.com/Markit/Transaction/2012/06/TransactionService/RejectTransaction" +
            "sResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(com.ivp.rad.RMarketWSO.TransactionService.GeneralServiceFault), Action="http://markit.com/Markit/Transaction/2012/06/TransactionService/RejectTransaction" +
            "sGeneralServiceFaultFault", Name="GeneralServiceFault")]
        com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsResponse RejectTransactions(com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsRequest request);
        
        // CODEGEN: Generating message contract since the wrapper name (SetDownloadStartDateRequest) of message SetDownloadStartDateRequest does not match the default value (SetDownloadStartDate)
        [System.ServiceModel.OperationContractAttribute(Action="http://markit.com/Markit/Transaction/2012/06/SetDownloadStartDate", ReplyAction="http://markit.com/Markit/Transaction/2012/06/TransactionService/SetDownloadStartD" +
            "ateResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(com.ivp.rad.RMarketWSO.TransactionService.GeneralServiceFault), Action="http://markit.com/Markit/Transaction/2012/06/TransactionService/SetDownloadStartD" +
            "ateGeneralServiceFaultFault", Name="GeneralServiceFault")]
        com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateResponse SetDownloadStartDate(com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetTransactionsByFacilityRequest", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class GetTransactionsByFacilityRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public int WSODataFacilityId;
        
        public GetTransactionsByFacilityRequest() {
        }
        
        public GetTransactionsByFacilityRequest(int WSODataFacilityId) {
            this.WSODataFacilityId = WSODataFacilityId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetTransactionsByFacilityResponse", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class GetTransactionsByFacilityResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public string Message;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=1)]
        public com.ivp.rad.RMarketWSO.TransactionService.TransactionResults Results;
        
        public GetTransactionsByFacilityResponse() {
        }
        
        public GetTransactionsByFacilityResponse(string Message, com.ivp.rad.RMarketWSO.TransactionService.TransactionResults Results) {
            this.Message = Message;
            this.Results = Results;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ConfirmTransactionsRequest", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class ConfirmTransactionsRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public System.Guid GroupId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=1)]
        public int WSODataFacilityId;
        
        public ConfirmTransactionsRequest() {
        }
        
        public ConfirmTransactionsRequest(System.Guid GroupId, int WSODataFacilityId) {
            this.GroupId = GroupId;
            this.WSODataFacilityId = WSODataFacilityId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ConfirmTransactionsResponse", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class ConfirmTransactionsResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public string Message;
        
        public ConfirmTransactionsResponse() {
        }
        
        public ConfirmTransactionsResponse(string Message) {
            this.Message = Message;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RejectTransactionsRequest", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class RejectTransactionsRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public System.Guid GroupId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=1)]
        public int WSODataFacilityId;
        
        public RejectTransactionsRequest() {
        }
        
        public RejectTransactionsRequest(System.Guid GroupId, int WSODataFacilityId) {
            this.GroupId = GroupId;
            this.WSODataFacilityId = WSODataFacilityId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RejectTransactionsResponse", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class RejectTransactionsResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public string Message;
        
        public RejectTransactionsResponse() {
        }
        
        public RejectTransactionsResponse(string Message) {
            this.Message = Message;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SetDownloadStartDateRequest", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class SetDownloadStartDateRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public System.DateTime StartDate;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=1)]
        public int WSODataFacilityId;
        
        public SetDownloadStartDateRequest() {
        }
        
        public SetDownloadStartDateRequest(System.DateTime StartDate, int WSODataFacilityId) {
            this.StartDate = StartDate;
            this.WSODataFacilityId = WSODataFacilityId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SetDownloadStartDateResponse", WrapperNamespace="http://markit.com/Markit/Transaction/2012/06", IsWrapped=true)]
    public partial class SetDownloadStartDateResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://markit.com/Markit/Transaction/2012/06", Order=0)]
        public string Message;
        
        public SetDownloadStartDateResponse() {
        }
        
        public SetDownloadStartDateResponse(string Message) {
            this.Message = Message;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TransactionServiceChannel : com.ivp.rad.RMarketWSO.TransactionService.TransactionService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TransactionServiceClient : System.ServiceModel.ClientBase<com.ivp.rad.RMarketWSO.TransactionService.TransactionService>, com.ivp.rad.RMarketWSO.TransactionService.TransactionService {
        
        public TransactionServiceClient() {
        }
        
        public TransactionServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TransactionServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransactionServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransactionServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityResponse com.ivp.rad.RMarketWSO.TransactionService.TransactionService.GetTransactionsByFacility(com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityRequest request) {
            return base.Channel.GetTransactionsByFacility(request);
        }
        
        public string GetTransactionsByFacility(int WSODataFacilityId, out com.ivp.rad.RMarketWSO.TransactionService.TransactionResults Results) {
            com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityRequest inValue = new com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityRequest();
            inValue.WSODataFacilityId = WSODataFacilityId;
            com.ivp.rad.RMarketWSO.TransactionService.GetTransactionsByFacilityResponse retVal = ((com.ivp.rad.RMarketWSO.TransactionService.TransactionService)(this)).GetTransactionsByFacility(inValue);
            Results = retVal.Results;
            return retVal.Message;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsResponse com.ivp.rad.RMarketWSO.TransactionService.TransactionService.ConfirmTransactions(com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsRequest request) {
            return base.Channel.ConfirmTransactions(request);
        }
        
        public string ConfirmTransactions(System.Guid GroupId, int WSODataFacilityId) {
            com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsRequest inValue = new com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsRequest();
            inValue.GroupId = GroupId;
            inValue.WSODataFacilityId = WSODataFacilityId;
            com.ivp.rad.RMarketWSO.TransactionService.ConfirmTransactionsResponse retVal = ((com.ivp.rad.RMarketWSO.TransactionService.TransactionService)(this)).ConfirmTransactions(inValue);
            return retVal.Message;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsResponse com.ivp.rad.RMarketWSO.TransactionService.TransactionService.RejectTransactions(com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsRequest request) {
            return base.Channel.RejectTransactions(request);
        }
        
        public string RejectTransactions(System.Guid GroupId, int WSODataFacilityId) {
            com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsRequest inValue = new com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsRequest();
            inValue.GroupId = GroupId;
            inValue.WSODataFacilityId = WSODataFacilityId;
            com.ivp.rad.RMarketWSO.TransactionService.RejectTransactionsResponse retVal = ((com.ivp.rad.RMarketWSO.TransactionService.TransactionService)(this)).RejectTransactions(inValue);
            return retVal.Message;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateResponse com.ivp.rad.RMarketWSO.TransactionService.TransactionService.SetDownloadStartDate(com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateRequest request) {
            return base.Channel.SetDownloadStartDate(request);
        }
        
        public string SetDownloadStartDate(System.DateTime StartDate, int WSODataFacilityId) {
            com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateRequest inValue = new com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateRequest();
            inValue.StartDate = StartDate;
            inValue.WSODataFacilityId = WSODataFacilityId;
            com.ivp.rad.RMarketWSO.TransactionService.SetDownloadStartDateResponse retVal = ((com.ivp.rad.RMarketWSO.TransactionService.TransactionService)(this)).SetDownloadStartDate(inValue);
            return retVal.Message;
        }
    }
}
