//function callfunc() {

//    $.ajax({
//        type: "POST",
//        url: "http://localhost:59727/Service2.svc/FetchTaskManagerValues",
//        contentType: 'application/json; charset=utf-8',
//        dataType: "json",
//        success: function (msg) {

//            for (var i = 0; i < msg.d.length; i++)
//                console.log("PID--> " + msg.d[i].Key + " PNAME--> " + msg.d[i].Value.processName + " MEMORY USED--> " + msg.d[i].Value.memoryUsed);
//            console.log("Total Number of Processes: " + msg.d.length);
//            console.log("-----------------------------------------------------------");
//            interval = setTimeout(callfunc, 10000);
//        },
//        error: function (msg) {
//            console.log(msg);
//        }
//    });

//}

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
    $('#btnShowServices').unbind('click').bind('click',function(){
        $('#disableTaskPg').css('width',$(document).width());
        $('#disableTaskPg').css('height',$(document).height()-30);
        $('#disableTaskPg').css('display','');
        $('#divShowServices').css('display','');
        //  $('#divShowServices').css("left", $(document).width() + "px");
        $('#divShowServices').css("width", "0px");
        $('#divShowServices').animate({ "right":"0px" ,"width":"1200px"}, 700, function () {
            SMopenServiceSetup.initShowServiceSection();
        });
    });
    $('#iframeCloseBtn').unbind('click').bind('click',function(){
        $('#divShowServices').hide({
            done:function(){
                $('#disableTaskPg').css('display','none');
            }
        });
      
        // SMopenServiceSetup.initShowServiceSection();
    });
    
}
$(document).ready(function () {
    SMopenServiceSetup.attachHandler();
});



class SearchBar extends React.Component {
    handleChange() {
        this.props.onUserInput(this.refs.filterTextInput.value);
        this.props.showFilter = false;
        this.setState({
             
        });
    }
    render() {
        return (
          <div className='tableUpperHeader'>

            <input type="text" className='searchInput' placeholder="Search process name..." value={this.props.filterText} ref="filterTextInput" onChange={this.handleChange.bind(this)}/>

                </div>

        );
    }

}
class FilterSearchBar extends React.Component {
    handleChange() {
        this.props.onUserInput(this.refs.filterTextInputFilterBox.value);
        this.props.showFilter = false;
        this.setState({
             
        });
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
//class CheckboxDiv extends React.Component { 
//    handleChange() {
//        this.props.onUserInput(this.refs.filterTextInput.value);
//    }
//     handleOutsideClick(e) {
//    // ignore clicks on the component itself
//    if (this.node.contains(e.target)) {
//      return;
//    }
    
//    this.handleClick();
//  }
//    render() {
//        return (
//   <input type="checkbox"  onChange={this.handleChange.bind(this, index)}/>
//        );
//        }

//        }
class FilerDiv extends React.Component { 
 
    constructor(props) {
        super(props);
        this.state={};
        this.state.childShowFilter = false;
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
        //this.state.childShowFilter = true;
        this.props.showFilter = false;
        this.setState({
             
        });
        //  this.props.showFilter = false;
        
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
        var handleToShowFilter=this.props.handleToShowFilter;
        return (
            <div id="filterDivId" className='filterDivCls' style={{display: handleToShowFilter( this.props.showFilter) ? 'block' : 'none',left:this.props.filteredPos[0].left  , top:this.props.filteredPos[1].top ,position:'absolute',maxHeight:'230px'}}>  
                
             
                
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
    <div className='filterCategoryRowCss'>{ (data.Key == '' ||data.Key == null)? 'blank': data.Key}</div></div>
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
        //this.state = {
        //    data: []
        //}
        this.state = {};
        this.state.data = [];
        this.state.filteredData = [];
        this.state.finalOutputdata = [];
        this.state.headerProperties = [];
        this.state.headerProperties.push({"keyName":"processName" ,"sortDir":"ASC","showFilter":"false"});
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
        path = window.location.protocol + '//' + window.location.host;
        pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });
    }
    getDataFromServer(){
        this.setState({
            loading:true
        });
        $.ajax({
            type:"POST",
            dataType:"json",
            url:path + "/BaseUserControls/Service/CommonService.svc/FetchTaskManagerValues",
            success: function(msg) {
                this.showResult(msg.d);
                //                setTimeout(this.getDataFromServer.bind(this),10000);
            }.bind(this),
            error: function(xhr, status, err) {
                //    console.error(this.props.url, status, err.toString());
            }.bind(this)
        });
    }
    componentDidMount() {
      
        this.getDataFromServer();
    }
    showResult(response) {
      
        this.setState({
            data: response,finalOutputdata :response,loading:false
        });
        this.onSortChange(null,this.state.sortKey);
    }
    handleUserInput(filterText) {
        this.setState({filterText: filterText});
    };
    handleToShowParentFilter(changedFilterState){
        this.state.showFilter= changedFilterState;
        return changedFilterState;
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
        this.state.showFilter = false;
        this.setState({sortKey: sortKey}); 
        if(this.state.sortDir==='ASC'){
            if(sortKey == 'processId' || sortKey == 'CPUUsage'||sortKey == 'memoryUsed'){
                
                this.state.finalOutputdata.sort(function(a,b) { 
                    if(parseInt(b.Value[sortKey]) > parseInt(a.Value[sortKey])) 
                        return -1;
                    if(parseInt(b.Value[sortKey]) < parseInt(a.Value[sortKey]))
                        return 1;
                    else return 0;
                });
            }
            if(sortKey == 'processName')
                this.state.finalOutputdata.sort((a,b) => a.Key.localeCompare(b.Key));
            else if(sortKey == 'userName')
            {
                this.state.finalOutputdata.sort(function(a,b){
                    if( a.Value[sortKey]) return b.Value[sortKey] ? a.Value[sortKey].localeCompare(b.Value[sortKey]) : -1;
                    else if(b.Value[sortKey]) return a.Value[sortKey] ? b.Value[sortKey].localeCompare(a.Value[sortKey]) : 1;
                }); 
            }
        }
        else{
            if(sortKey == 'processId' || sortKey == 'CPUUsage'||sortKey == 'memoryUsed'){
                this.state.finalOutputdata.sort(function(a,b) { 
                    if(parseInt(a.Value[sortKey]) > parseInt(b.Value[sortKey]))
                        return -1;
                    if(parseInt(b.Value[sortKey]) > parseInt(a.Value[sortKey]))
                        return 1;
                    else return 0;
                });
            }
            if(sortKey == 'processName')
                this.state.finalOutputdata.reverse((a,b) => a.Key.localeCompare(b.Key));
            else if(sortKey == 'userName')
            {
                this.state.finalOutputdata.reverse(function(a,b){
                    if( a.Value[sortKey]) return b.Value[sortKey] ? a.Value[sortKey].localeCompare(b.Value[sortKey]) : -1;
                    else if(b.Value[sortKey]) return a.Value[sortKey] ? b.Value[sortKey].localeCompare(a.Value[sortKey]) : 1;
                
                }); 
            }
            // this.state.finalOutputdata.reverse((a,b) => a.Value[sortKey].localeCompare(b.Value[sortKey]));
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
        if(headerName != 'processName'){
            for( i=0; i<l; i++) {
                if( flags[collObj[i].Value[headerName]]) continue;
                flags[collObj[i].Value[headerName]] = true;
                outputPID.push({'Key':collObj[i].Value[headerName],'Checked':true,'selectAllChecked':true}); // checked = true is good if the dataset is already filtered                
            }
         
        }
        else
        {       
            for( i=0; i<l; i++) {
                if( flags[collObj[i].Key]) continue;
                flags[collObj[i].Key] = true;
                outputPID.push({'Key':collObj[i].Key,'Checked':true,'selectAllChecked':true});                 
            }
        }
        //mark the checked in the whole data and rest of them as unchecked
        if(collObj == this.state.data)
        {if(headerName != 'processName')
            var obj1= this.state.finalOutputdata.map(x=>x.Value[headerName]);
        else
            var obj1= this.state.finalOutputdata.map(x=>x.Key);

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
    onSubmitSelection(headerName){
        this.state.showFilter = false;
        if(headerName != 'processName'){
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
        }
        else{
            if(this.state.collectionFilters.filter(  x=>x.filterName == headerName).length==0){
                var flags = [], outputPID = [], l = this.state.finalOutputdata.length, i;
                for( i=0; i<l; i++) {
                    if( flags[this.state.finalOutputdata[i].Key]) continue;
                    flags[this.state.finalOutputdata[i].Key] = true;
                    outputPID.push({'Key':this.state.finalOutputdata[i].Key,'Checked':true,'selectAllChecked':true});                 
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
        }
        //array containing keys with checked true
        var obj1= this.state.filteredData.filter(x=> x.Checked==true).map(x=>x.Key);
        if(headerName == 'processName')
            this.state.finalOutputdata=  this.state.data.filter(x=>obj1.indexOf(x.Key) >=0);
        else
            this.state.finalOutputdata=  this.state.data.filter(x=>obj1.indexOf(x.Value[headerName]) >=0);
        this.setState(this.state.finalOutputdata);
    };
   
    render() {
        return (
            
          <div>
               {(this.state.loading)?
                         (<div> <div> <img  src={'../App_Themes/Aqua/images/ajax-working.gif'} style={{borderWidth: '0px', position:'absolute', left:this.state.width /2 + 200+ 'px'}}/></div> <div id='disableDiv' className='alertBG' style={{zIndex: '9999', height:$(document).height(),width:$(document).width()}} align='center'></div></div>) : ''}
            <FilerDiv data={this.state.data} filterTextFilterBox={this.state.filterTextFilterBox} collectionFilters={this.state.collectionFilters} filteredData={this.state.filteredData} filteredPos={this.state.filteredPos} showFilter={this.state.showFilter} handleToShowFilter={this.handleToShowParentFilter.bind(this)} onSubmitSelection={this.onSubmitSelection.bind(this)} />
           
                <SearchBar filterText={this.state.filterText} onUserInput={this.handleUserInput.bind(this)}/>
                    <div style={{height: this.state.height+'px'}} className='contentDivClass'>
                    <table className='tableCls'>
                      <thead>
                     <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'processId')}>process Id <span onClick={e=>this.onFilterIconClick(e,'processId')} className='filterIconCls' id="divFilterProcessId"></span></th>
                        <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'processName')}>Process Name <span onClick={e=>this.onFilterIconClick(e,'processName')} className='filterIconCls' id="divFilterProcessName"></span></th>
        
                         <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'CPUUsage')}> CPU Usage <span onClick={e=>this.onFilterIconClick(e,'CPUUsage')}  className='filterIconCls' id="divFilterCPUUsage"></span></th>
                         <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'memoryUsed')}>Memory Usage <span onClick={e=>this.onFilterIconClick(e,'memoryUsed')}  className='filterIconCls' id="divFilterMemoryUsed"></span></th>
                          <th className='tableHeaderCss' onClick={e => this.onSortChange(e, 'userName')}>User Name <span onClick={e=>this.onFilterIconClick(e,'userName')}  className='filterIconCls' id="divFilterUserName"></span></th>
                      </thead>
                      <tbody style={{height: this.state.height+'px'}} className='mostly-customized-scrollbar'>
                      {this.state.finalOutputdata.map((data, i) => 
                      {
                          if(this.state.filterText != '')
                          {
                              if (data.Key.toLowerCase().indexOf(this.state.filterText.toLowerCase()) === -1) {
                                  return;
                              }
                              else{
                                  return (
                                    <tr className='tableRows' key={i}>
                                     <td className='tableTdCls'>{data.Value.processId}</td>
                                    <td className='tableTdCls'>{data.Key}</td>
                      
                                       <td className='tableTdCls'>{data.Value.CPUUsage}</td>
                                    <td className='tableTdCls'>{data.Value.memoryUsed}   </td>
                                    <td className='tableTdCls'>{data.Value.userName}   </td>

                                  </tr>
                                          )
                              }
                      }
                      else
                      {
                                   return (
                                      <tr className='tableRows' key={i}>
                                   <td  className='tableTdCls'>{data.Value.processId}</td>
                                  <td className='tableTdCls'>{data.Key}</td>
                      
                                     <td className='tableTdCls'>{data.Value.CPUUsage}</td>
                                  <td className='tableTdCls'>{data.Value.memoryUsed}   </td>
                                  <td className='tableTdCls'>{data.Value.userName}   </td>

                                </tr>
                                       )
}
})
}
</tbody>
                             </table>
                             </div>
                           </div>
    )
}
}


ReactDOM.render(
  <App />, 
  document.getElementById("app")
);
