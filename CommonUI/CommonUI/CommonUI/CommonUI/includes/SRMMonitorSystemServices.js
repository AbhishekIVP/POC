var path = window.location.protocol + '//' + window.location.host;
var pathname = window.location.pathname.split('/');
$.each(pathname, function (ii, ee) {
    if ((ii !== 0) && (ii !== pathname.length - 1))
        path = path + '/' + ee;
});
var SMopenServiceSetup = {};
SMopenServiceSetup.initShowServiceSection = function(){
    $('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + "/SRMMonitorSystemServices.aspx");
}
SMopenServiceSetup.attachHandler = function () {
    $('#StartPoolId').unbind('click').bind('click',function(){
        $.ajax({
            type:"POST",
            dataType:"json",
            url:path + "/BaseUserControls/Service/CommonService.svc/startAppPool",
            success: function(msg) {
                console.log(msg.d);
            },
            error: function(xhr, status, err) {
                console.log(err.message);
            }
        });
    });
    $('#StopPoolId').unbind('click').bind('click',function(){
        $.ajax({
            type:"POST",
            dataType:"json",
            url:path + "/BaseUserControls/Service/CommonService.svc/stopAppPool",
            success: function(msg) {
                console.log(msg.d);
            },
            error: function(xhr, status, err) {
                console.log(err.message);
            }
        });
    });
  
    
}
$(document).ready(function () {
    SMopenServiceSetup.attachHandler();
});

class SearchBar extends React.Component {
    handleChange() {
        this.props.onUserInput(this.refs.filterTextInput.value);
    }
    render() {
        return (
          <div className='tableUpperHeader'>

            <input type="text" className='searchInput' placeholder="Search service name..." value={this.props.filterText} ref="filterTextInput" onChange={this.handleChange.bind(this)}/>

                </div>

        );
    }

}
class FilterSearchBar extends React.Component {
    handleChange() {
        this.props.onUserInput(this.refs.filterTextInputFilterBox.value);
    }
    render() {
        return (
          <div>
          <div style={{textAlign: 'center'}}>
            <i className="fa fa-search"></i>
            <input type="text" className='filterSearchInput' placeholder="Search..." value={this.props.filterTextFilterBox} ref="filterTextInputFilterBox" onChange={this.handleChange.bind(this)}/>
                </div>
                </div>

        );
}

}
class FilerDiv extends React.Component { 
 
    constructor(props) {
        super(props);
        // this.setWrapperRef = this.setWrapperRef.bind(this);
        //this.handleClickOutside = this.handleClickOutside.bind(this);
    }
    //componentDidMount() {
    //    document.addEventListener('mousedown', this.handleClickOutside);
    //}

    //componentWillUnmount() {
    //    document.removeEventListener('mousedown', this.handleClickOutside);
    //}
    handleChange(e,d) {
        var isChecked=this.props.filteredData[d].Checked;
        this.props.filteredData[d].Checked = !isChecked;
        if(isChecked) //was true earlier and now we are making it false
            this.props.filteredData.map(x=>x.selectAllChecked = false);
        //if all the checkboxes are set to be true then check the select All checkbox also
        if(this.props.filteredData.filter(x=>x.Checked == false).length<=0)
            this.props.filteredData.map(x=>x.selectAllChecked = true);
        this.setState({
             
        });
        //   this.props.onUserInput(this.refs.filterTextInput.value);
    };
    handleFilteredUserInput(filterTextFilterBox) {    
        this.props.filterTextFilterBox = filterTextFilterBox;
        this.setState({});
    };
    //setWrapperRef(node) {
    //    this.wrapperRef = node;
    //}
    //handleClickOutside(event) {
    //    if ($(event.target).parents('#filterDivId').length<=0 && $(event.target)[0].className!='filterIconCls') {
    //        this.props.showFilter = false; //$('#filterDivId').css('display','none')
    //    }
    //}
    callAppSubmitSelection(e){
        this.props.onSubmitSelection(this.props.filteredPos[2].headerName);
    };
    callRefreshSelection(e){
        //find out the first filter applied by getting first ele pushed in conditionalFilter
        if(this.props.filteredPos[2].headerName == this.props.collectionFilters[0].filterName)
        {
            var flags = [], outputPID = [], l = this.state.data.length, i;
            for( i=0; i<l; i++) {
                if( flags[this.state.data[i].Key]) continue;
                flags[this.props.data[i].Key] = true;
                outputPID.push({'Key':this.props.data[i].Key,'Checked':true});                 
            }
            this.props.filteredData = outputPID;
            this.props.collectionFilters[this.props.collectionFilters.length-1].setFilter=outputPID;
        }
     
        this.props.filteredData.map(x=>x.Checked = true);
        this.setState({
             
        });
    };
    callCloseSelection(e){
        this.props.showFilter = false;
        this.setState({
             
        });
    };
    selectAllSelection(e,d){
        this.props.filteredData[d].selectAllChecked = !this.props.filteredData[d].selectAllChecked;
        this.props.filteredData.map(x=>x.Checked = $(e.target).prop('checked'));
        this.props.filteredData.map(x=> x.Checked =this.props.filteredData[d].selectAllChecked);
        this.props.filteredData.map(x=>x.selectAllChecked =this.props.filteredData[d].selectAllChecked);
        this.setState({
             
        });
    };
    render() {
        return (
            <div id="filterDivId" className='filterDivCls' style={{display: this.props.showFilter ? 'block' : 'none',left:this.props.filteredPos[0].left  , top:this.props.filteredPos[1].top ,position:'absolute',maxHeight:'230px'}}>  
                
             
                
               <FilterSearchBar filterTextFilterBox={this.props.filterTextFilterBox} onUserInput={e=> this.handleFilteredUserInput(e)}/>
                   { this.props.filteredData.find(x=>x) != undefined ?
                       <div className='filterSearchDivCss'><div className='filterCategoryChckBoxRowCss'> <input type="checkbox"  checked={  this.props.filteredData.find(x=>x).selectAllChecked} onClick={e => this.selectAllSelection(e,0)}  value='Select All' name ='Select All'/></div> <div className='filterCategoryRowCss'>Select All</div> </div> : ''
}

                   <div className='mostly-customized-scrollbar' style={{overflowY: 'auto',width: '188px',height: '128px',background:'white'}} >
{ this.props.filteredData.map((data, i) => 

{
    if(this.props.filterTextFilterBox != '')
    {
        if (data.Key.toString().toLowerCase().indexOf(this.props.filterTextFilterBox.toString().toLowerCase()) === -1) 
        {
            return;
        }
        else{
            return(    
         <div>
            <div className='filterChkBxDivCss'> <div className='filterCategoryChckBoxRowCss'> <input type="checkbox"  checked={data.Checked} onClick={e => this.handleChange(e, i)}  value={data.Key} name ={data.Key}/> </div>
        <div className='filterCategoryRowCss'>{data.Key}</div> </div>
        
    </div>
                          )
                   }
    }
    else
    {

        return(    
            <div>          
          <div className='filterChkBxDivCss'><div className='filterCategoryChckBoxRowCss'> <input type="checkbox"  checked={data.Checked} onClick={e => this.handleChange(e, i)}  value={data.Key} name ={data.Key}/> </div>
    <div className='filterCategoryRowCss'>{data.Key == '' ? 'blank': data.Key}</div></div>
    </div>
                          )
}                            
})
}
</div>
<div className='filterMainDivOptionsCss'>
<div  className='filterDivOptionsCss'  onClick={e=>this.callAppSubmitSelection(e)} ><i className="fa fa-check-circle" style={{'color':'##999999a1'}}></i></div>
<div  className='filterDivOptionsCss'   onClick={e=>this.callRefreshSelection(e)}  ><i className="fa fa-refresh" style={{'color':'##999999a1'}}></i></div>
<div  className='filterDivOptionsCss'  onClick={e=>this.callCloseSelection(e)}  ><i className="fa fa-times-circle" style={{'color':'##999999a1'}}></i></div>
</div>
</div>
        );
}

}
class App extends React.Component {
    constructor(props) {
        super(props)
        this.state = {};
        this.state.data = [];
        this.state.filteredData = [];
        this.state.finalOutputdata = [];
        this.state.filterText ="";
        this.state.filterTextFilterBox ="";
        this.state.sortDir = "ASC";
        this.state.sortKey = "processName"; //processName
        this.state.width = $(document).width()-500;
        this.state.height =$(document).height()-100;
        this.state.filteredPos = [{"left":"0px"},{"right":"0px"},{"headerName":''}];
        this.state.showFilter = false;
        this.state.collectionFilters=[];
        this.state.loading = true;               
        this.state.showPopup = false;
      
    }
    getDataFromServer() {
        $.ajax({
            type: "POST",
            dataType: "json",
            url: path + "/BaseUserControls/Service/CommonService.svc/FetchSystemServicesValues",
            success: function (msg) {
                this.showResult(msg.d);
                // setTimeout(this.getDataFromServer.bind(this),10000);
            }.bind(this),
            error: function (xhr, status, err) {
                //    console.error(this.props.url, status, err.toString());
            }.bind(this)
        });
    }
    componentDidMount() {
     
        this.getDataFromServer();
     
    }
    showResult(response) {

        this.setState({
            data: response,finalOutputdata :response, loading:false
        });
      
    }
    handleUserInput(filterText) {
        this.setState({filterText: filterText});
    };
    onRefreshClick(e){
        this.setState({
            loading:true
        });
        this.getDataFromServer();
    }
    onSortChange(e,sortKey) {
        var tempSortDir= this.state.sortDir;
        if(e != null){
            var found = this.state.headerProperties.some(function (el) {
                if( el.keyName === sortKey){
                    el.sortDir = el.sortDir === 'ASC' ? 'DESC' : 'ASC';
                    tempSortDir =el.sortDir;
                    return true;
                }
                
            });
            this.state.sortDir= tempSortDir;
            if(!found)
            {
                this.state.headerProperties.push({"keyName" : sortKey,"sortDir":"ASC","showFilter":"false"});
                this.state.sortDir ="ASC";
            }
            //this.state.sortDir = (this.state.sortDir === 'ASC' ? 'DESC' : 'ASC');
        }
        this.setState({sortKey: sortKey}); 
        if(this.state.sortDir==='ASC'){
            if(sortKey == 'processId'){
                
                this.state.finalOutputdata.sort(function(a,b) { 
                    if(parseInt(b.Value[sortKey]) > parseInt(a.Value[sortKey])) 
                        return -1;
                    if(parseInt(b.Value[sortKey]) < parseInt(a.Value[sortKey]))
                        return 1;
                    else return 0;
                });
            }
            else //if(sortKey == 'serviceName')
                this.state.finalOutputdata.sort((a,b) => a.Key.localeCompare(b.Key));
            //else if(sortKey == 'userName')
            //    this.state.dafinalOutputdatata.sort((a,b) => a.Value[sortKey].localeCompare(b.Value[sortKey]));
        }
        else{
            if(sortKey == 'processId'){
                this.state.finalOutputdata.sort(function(a,b) { 
                    if(parseInt(a.Value[sortKey]) > parseInt(b.Value[sortKey]))
                        return -1;
                    if(parseInt(b.Value[sortKey]) > parseInt(a.Value[sortKey]))
                        return 1;
                    else return 0;
                });
            }
            else
                this.state.finalOutputdata.reverse((a,b) => a.Key.localeCompare(b.Key));
      
        }
        this.setState(this.state.finalOutputdata);
    };
    onFilterIconClick(e,headerName){
   
        var leftPos= $(e.target).offset().left;
        var topPos= $(e.target).offset().top;
        this.state.filteredPos=[{"left":leftPos+2 +"px"},{"top":topPos+30+"px"},{"headerName":headerName}]; //from e offset
        this.setState({
            showFilter: true
        });
        var collObj='';
        //if the filter icon of first pushed column ( where we had applied the filter for the first time is clicked) , it should show all the options of data in filter popup not just the filtered data else it will show the filtered options.
        if(this.state.collectionFilters.length>0 && this.state.collectionFilters[0].filterName == headerName)
            collObj = this.state.data;
        else
            collObj= this.state.finalOutputdata;
        
        var flags = [], outputPID = [], l = collObj.length, i;
        // if(headerName != 'serviceName'){
        for( i=0; i<l; i++) {
            if( flags[collObj[i].Value[headerName]]) continue;
            flags[collObj[i].Value[headerName]] = true;
            outputPID.push({'Key':collObj[i].Value[headerName],'Checked':true,'selectAllChecked':true}); // checked = true is good if the dataset is already filtered                
        }
         
        //}
        //else
        //{       
        //    for( i=0; i<l; i++) {
        //        if( flags[collObj[i].Key]) continue;
        //        flags[collObj[i].Key] = true;
        //        outputPID.push({'Key':collObj[i].Key,'Checked':true,'selectAllChecked':true});                 
        //    }
        //}
        //mark the checked in the whole data and rest of them as unchecked
        if(collObj == this.state.data)
        {
            var obj1= this.state.finalOutputdata.map(x=>x.Value[headerName]);
            outputPID.filter(x=> obj1.indexOf(x.Key) ==-1 ).map(y=> {y.Checked = false,y.selectAllChecked = false});
        }
    
    
        if(this.state.collectionFilters.filter(  x=>x.filterName == headerName).length==0)   
            this.state.collectionFilters.push({'filterName':headerName,'setFilter':outputPID});
        else
        {
            var alreadyExsitingObj= this.state.collectionFilters.filter(obj => {
                obj['filterName'] == headerName;
            }).map(x=>x.setFilter = outputPID);
        }
        this.state.filteredData = outputPID;
        e.stopPropagation();

    };
    onServiceRestart(e,updatedStatus){
        var thisObj = this;
        $('#panelError').css('display','none');
        this.setState({
            loading:true
        });
        var serviceName = e;
        $.ajax({
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data:JSON.stringify({"serviceName": serviceName }),
            url: path + "/BaseUserControls/Service/CommonService.svc/OnRestartService",
            success: function (msg) {
                if(msg.d != 'success')
                {
                    $('#panelError').css('display','');
                    $('#lblErrorText').text(msg.d);
                    $('#btnErrorOk').unbind('click').bind('click',function(){
                        thisObj.setState({
                            showPopup : false
                        });
                        App.prototype.getDataFromServer();
                        $('#panelError').css('display','none');
                    });
                    this.setState({
                        loading:false,showPopup : true
                    });
                }else
                { this.getDataFromServer();
                    $('#panelError').css('display','none');}
                //var servObj = this.state.data.filter(x=>x.Value.serviceName == serviceName);
                //if(updatedStatus == 'Restart')                
                //    servObj[0].Value.status = 'Started'
                //this.setState({
                   
                //});
           
                // setTimeout(this.getDataFromServer.bind(this),10000);
            }.bind(this),
            error: function (xhr, status, err) {
                var abc =1 ; 
                var cde = 2323;
                console.log('dewef');
            }.bind(this)
        });
    }
    onServiceStartStop(e,updatedStatus){
        var thisObj = this;
        $('#panelError').css('display','none');
        this.setState({
            loading:true
        });
        var serviceName = e;
        $.ajax({
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data:JSON.stringify({"serviceName": serviceName ,"existingStatus":updatedStatus}),
            url: path + "/BaseUserControls/Service/CommonService.svc/onStartStopService",
            success: function (msg) {
                if(msg.d != 'success')
                {
                    $('#panelError').css('display','');
                    $('#lblErrorText').text(msg.d);
                    $('#btnErrorOk').unbind('click').bind('click',function(){
                        App.prototype.getDataFromServer();
                        thisObj.setState({
                            showPopup : false
                        });
                        $('#panelError').css('display','none');
                    });
                    this.setState({
                        loading:false,showPopup:true
                    });
                }else
                {this.getDataFromServer();
                    $('#panelError').css('display','none');}
                //var servObj = this.state.data.filter(x=>x.Value.serviceName == serviceName);
                //if(updatedStatus == 'Running' || updatedStatus == 'Started')
                //    servObj[0].Value.status = 'Stopped'
                //else
                //    servObj[0].Value.status = 'Started'

                //this.setState({
                   
                //});
             
                // setTimeout(this.getDataFromServer.bind(this),10000);
            }.bind(this),
            error: function (xhr, status, err) {
                var abc =1 ; 
                var cde = 2323;
                console.log('dewef');
            }.bind(this)
        });
    }
    onSubmitSelection(headerName){
        if(this.state.collectionFilters.filter(  x=>x.filterName == headerName).length==0){
            var flags = [], outputPID = [], l = this.state.finalOutputdata.length, i;
            for( i=0; i<l; i++) {
                if( flags[this.state.finalOutputdata[i].Value[headerName]]) continue;
                flags[this.state.finalOutputdata[i].Value[headerName]] = true;
                outputPID.push({'Key':this.state.finalOutputdata[i].Value[headerName],'Checked':true,'selectAllChecked':true});                 
            }
            this.state.collectionFilters.push({'filterName':headerName,'setFilter':outputPID});
        }
        else
        {
            var alreadyExsitingObj= this.state.collectionFilters.filter(obj => {
                return obj['filterName'] == headerName;
            })
            outputPID= alreadyExsitingObj[0].setFilter;
        }

        //array containing keys with checked true
        var obj1= this.state.filteredData.filter(x=> x.Checked==true).map(x=>x.Key);
        this.state.finalOutputdata=  this.state.data.filter(x=>obj1.indexOf(x.Value[headerName]) >=0);
        this.setState(this.state.finalOutputdata);
    };
    render() {
        return (
            <div>
            <div  onClick={e => this.onRefreshClick(e)} className='RefreshCls'> Refresh </div>
            {(this.state.loading)?
                (<div> <div> <img  src={'../App_Themes/Aqua/images/ajax-working.gif'} style={{borderWidth: '0px', position:'absolute', left:this.state.width /2 + 200+ 'px'}}/></div> <div id='disableDiv' className='alertBG' style={{zIndex: '9999',height:$(document).height(),width:$(document).width()}} align='center'></div></div>) : ''}
{(this.state.showPopup)?
                (<div> <div id='disableDiv' className='alertBG' style={{zIndex: '9999',height:$(document).height(),width:$(document).width()}} align='center'></div></div>) : ''}

             <FilerDiv data={this.state.data} filterTextFilterBox={this.state.filterTextFilterBox} collectionFilters={this.state.collectionFilters} filteredData={this.state.filteredData} filteredPos={this.state.filteredPos} showFilter={this.state.showFilter} onSubmitSelection={this.onSubmitSelection.bind(this)} />
           
                 <SearchBar filterText={this.state.filterText} onUserInput={this.handleUserInput.bind(this)}/>
                     <div style={{height: this.state.height+'px'}} className='contentDivClass'>
                     <table className='tableCls'>
                       <thead>
                            <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'serviceName')}>Service Name <span onClick={e=>this.onFilterIconClick(e,'serviceName')} className='filterIconCls' id="divFilterServiceName"></span></th>
                      <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'processId')}>process Id <span onClick={e=>this.onFilterIconClick(e,'processId')} className='filterIconCls' id="divFilterProcessId"></span></th>
                          <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'status')}>Status<span onClick={e=>this.onFilterIconClick(e,'status')}  className='filterIconCls' id="divFilterstatus"></span></th> 
                            <th className='tableHeaderCss'></th> 
                              <th className='tableHeaderCss' ></th> 
                       </thead>
                       <tbody style={{height: this.state.height - 20+'px'}} className='mostly-customized-scrollbar'>

                       {this.state.finalOutputdata.map((data, i) => 
                       {
                           if(this.state.filterText != '')
                           {
                               if (data.Value.serviceName.toLowerCase().indexOf(this.state.filterText.toLowerCase()) === -1) {
                                   return;
                               }
                               else{
                                   
                                   return (
                                                         <tr className='tableRows' key={i}>
                                                     
                                                            <td className='tableTdCls'>{data.Value.serviceName}</td>
                                                                <td className='tableTdCls'>{data.Value.processId}</td>
                                                        
                                                           <td className='tableTdCls'>{data.Value.status}   </td>
                                       
                                                           <td className='tableTdCls statusCls' onClick={e => this.onServiceStartStop(data.Value.serviceName, data.Value.status)}> { (data.Value.status == 'Stopped')?
                            'Start' : (data.Value.status == 'Started' || data.Value.status == 'Running') ? 'Stop' : ''} </td>
                                       
    
                                                <td className='tableTdCls statusCls'  onClick={e => this.onServiceRestart(data.Value.serviceName, 'Restart')}>Restart</td>
                   </tr>
                                              )
                              }
                       }  else
                           {
                                 
                           return (
                                                 <tr className='tableRows' key={i}>
                                                     
                                                    <td className='tableTdCls'>{data.Value.serviceName}</td>
                                                        <td className='tableTdCls'>{data.Value.processId}</td>
                                                  
                                                   <td className='tableTdCls'>{data.Value.status}   </td>
                                       
                                                   <td className='tableTdCls statusCls' onClick={e => this.onServiceStartStop(data.Value.serviceName, data.Value.status)}> { (data.Value.status == 'Stopped')?
                    'Start' : (data.Value.status == 'Started' || data.Value.status == 'Running') ? 'Stop' : ''} </td>
                                       
    
                                        <td className='tableTdCls statusCls'  onClick={e => this.onServiceRestart(data.Value.serviceName, 'Restart')}>Restart</td>
           </tr>
                                              )
}




})
}

</tbody>
</table>
</div>
</div>
            )}
}

    ReactDOM.render(
      <App />, 
      document.getElementById("app")
    );
