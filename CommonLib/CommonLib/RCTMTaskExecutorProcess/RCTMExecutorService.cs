//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RCTMExecutorService
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TaskExecutorResponse", Namespace="http://schemas.datacontract.org/2004/07/RCTMExecutorService")]
    public partial class TaskExecutorResponse : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int ExitCodeField;
        
        private string MessageField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ExitCode
        {
            get
            {
                return this.ExitCodeField;
            }
            set
            {
                this.ExitCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IRCTMExecutorService")]
public interface IRCTMExecutorService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRCTMExecutorService/TriggerTask", ReplyAction="http://tempuri.org/IRCTMExecutorService/TriggerTaskResponse")]
    RCTMExecutorService.TaskExecutorResponse TriggerTask(int chainId, int flowId);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRCTMExecutorService/TriggerTask", ReplyAction="http://tempuri.org/IRCTMExecutorService/TriggerTaskResponse")]
    System.Threading.Tasks.Task<RCTMExecutorService.TaskExecutorResponse> TriggerTaskAsync(int chainId, int flowId);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IRCTMExecutorServiceChannel : IRCTMExecutorService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class RCTMExecutorServiceClient : System.ServiceModel.ClientBase<IRCTMExecutorService>, IRCTMExecutorService
{
    
    public RCTMExecutorServiceClient()
    {
    }
    
    public RCTMExecutorServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public RCTMExecutorServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public RCTMExecutorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public RCTMExecutorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public RCTMExecutorService.TaskExecutorResponse TriggerTask(int chainId, int flowId)
    {
        return base.Channel.TriggerTask(chainId, flowId);
    }
    
    public System.Threading.Tasks.Task<RCTMExecutorService.TaskExecutorResponse> TriggerTaskAsync(int chainId, int flowId)
    {
        return base.Channel.TriggerTaskAsync(chainId, flowId);
    }
}
