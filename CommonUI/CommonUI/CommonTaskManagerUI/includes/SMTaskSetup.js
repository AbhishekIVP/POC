var smTaskSetup = (function () {
    var smTaskSetup;
    function SMTaskSetupClass() {
    }
    smTaskSetup = new SMTaskSetupClass();

    var groups = {};
    var taskNameSearchData = {};
    var viewModelObj;
    var isApplyBindingTrue = 'false';

    //var selectedGroup = 0;
    //var mainGroupsObj = function () {
    //    var self = this;
    //    self.groups = ko.observableArray();
    //    self.tasks = ko.observableArray();
    //}

    SMTaskSetupClass.prototype.DeleteGroup = function DeleteGroup(_chainId) {
        var targetGroupObj = null;
        if (viewModelObj.groups().length > 0) {
            viewModelObj.groups().forEach(function (el) {
                if (el.chain_id() == _chainId)
                    targetGroupObj = el;
            });
            if (targetGroupObj != null)
                viewModelObj.groups.remove(targetGroupObj);
            refillGroup(viewModelObj);
        }
    }

    SMTaskSetupClass.prototype.DeleteTask = function DeleteTask(_chainId, _flowId) {

        var targetTaskObj = null;
        if (viewModelObj.groups().length > 0) {
            viewModelObj.groups().forEach(function (e1) {
                if (e1.chain_id() == _chainId) {
                    e1.children().forEach(function (e2) {
                        if (e2.flow_id() == _flowId) //removeTask
                            targetTaskObj = e2;
                    });
                    if (targetTaskObj != null)
                        e1.children.remove(targetTaskObj);
                }
            });
            refillGroup(viewModelObj);
        }
    }

    function applySlimScroll(containerSelector, bodySelector) {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        if (scrollBodyContainer.height() < scrollBody.height()) {
            scrollBody.smslimscroll({ height: (scrollBodyContainer.height() - 40) + 'px', alwaysVisible: false, position: 'right', size: '6px', distance: '2px' });
        }
    }

    SMTaskSetupClass.prototype.GetNewGroup = function () {
        //var arrGroups = viewModelObj.groups();
        newGroup = { chain_name: "Untitled Group", next_scheduled_time: null, chain_id: '12@3', subscribe_id: '12@3', moduleId: '12@3', chainIdentifier: '12@3', children: [{ flow_id: '12@3', chain_id: '12@3', chain_name: '12@3', childrenRow: false, task_name: '12@3', subscribe_id: '12@3', is_muted: true, isVisible: true, module_name: '12@3', task_type_name: '12@3' }] };
        //arrGroups.unshift(ko.mapping.fromJS(newGroup));
        if (viewModelObj.groups().length > 0) {
            viewModelObj.groups.unshift(ko.mapping.fromJS(newGroup));
        }
        else {
            viewModelObj.groups = ko.observableArray();
            viewModelObj.groups.push(ko.mapping.fromJS(newGroup));
        }

        refillGroup(viewModelObj);
    };

    SMTaskSetupClass.prototype.DeleteNewGroup = function () {

        var targetGroupObj = viewModelObj.groups()[0];
        if (targetGroupObj.chain_name() == 'Untitled Group') {
            viewModelObj.filteredGroups.remove(targetGroupObj);
            refillGroup(viewModelObj);
        }
    };

    SMTaskSetupClass.prototype.GetFilteredGroup = function () {
        var sText = $('#gridSearch').val().trim().toLowerCase();
        var moduleNameDropDown = $('#gridModuleSelect').val().trim().toLowerCase();
        var taskTypeDropDown = $('#gridTaskTypeSelect').val().trim().toLowerCase();
        var arr = viewModelObj.groups();
        var flag = false;

        if (sText !== "search by group name or task name...") {
            arr = arr.filter(function (ele, index) {
                if (ele.chain_name().toLowerCase().indexOf(sText) !== -1) {
                    return ele;
                }
                else {
                    flag = false;
                    ele.children().forEach(function (child, i) {
                        if (child.task_name().toLowerCase().indexOf(sText) !== -1) {
                            flag = true;
                            return;
                        }
                    });
                    if (flag) {
                        return ele;
                    }
                }
            });
        }
        if (moduleNameDropDown !== "all")//dropdown1
        {
            arr = arr.filter(function (ele, index) {
                flag = false;
                ele.children().forEach(function (child, i) {
                    if (child.module_name().toLowerCase().indexOf(moduleNameDropDown) !== -1) {
                        flag = true;
                        return;
                    }
                });
                if (flag) {
                    return ele;
                }
            });
        }
        if (taskTypeDropDown !== "all") {//dropdown2
            arr = arr.filter(function (ele, index) {
                flag = false;
                ele.children().forEach(function (child, i) {
                    if (child.task_type_name().toLowerCase().indexOf(taskTypeDropDown) !== -1) {
                        flag = true;
                        return;
                    }
                });
                if (flag) {
                    return ele;
                }
            });
        }

        viewModelObj.filteredGroups(arr);
        if (arr.length > 0) {
            viewModelObj.selectedGroup(arr[0]);
            $('#SMeditable').css('width', ((arr[0].chain_name().length + 1) * 8) + 'px');  //dynamic input text element
            smtaskManagerMain.GenerateGraph(ko.mapping.toJS(arr[0].children()[0].chain_id()));
        }
    }

    SMTaskSetupClass.prototype.InitializeGrid = function InitializeGrid(_groups) {
        groups = _groups;
        var taskContainer = {};
        var temp;
        var dateFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);

        if (groups.length > 0) {
            groups.forEach(function (group, i) {
                group.isSelected = false;
                if (i == 0) {
                    group.isSelected = true;
                }
                // group.childrenRow = false;  // making the custom attr from ascx.
                group.chainIdentifier = ko.computed(function () {
                    return group.chain_id;
                });

                if (taskNameSearchData.hasOwnProperty(group.chain_name)) {
                    taskNameSearchData[group.chain_name].push(group.chain_id);
                }
                else {
                    taskNameSearchData[group.chain_name] = [group.chain_id];
                }
                if (group.next_scheduled_time)
                    group.next_scheduled_time = Date.parseInvariant(group.next_scheduled_time, "yyyy-MM-dd HH:mm:ss").formatCTM(dateFormat + ' tfh:nn:ss');

                //group.isSelected = ko.observable(false);
                temp = JSON.parse(JSON.stringify(group));
                group.children.unshift(temp);
                group.children.forEach(function (child, j) {
                    child.isVisible = true;
                    child.childrenRow = true;
                    if (j == 0) {
                        child.childrenRow = false;
                        child.children.length = 0;
                    }

                    if (taskNameSearchData.hasOwnProperty(child.task_name)) {
                        taskNameSearchData[child.task_name].push(group.chain_id);
                    }
                    else {
                        taskNameSearchData[child.task_name] = [group.chain_id];
                    }
                });
            });
        }
        //var temp;
        //groups.forEach(function (group, i) {

        //    group.children.forEach(function (child, j) {
        //        if (j == 0) {

        //        }
        //    });
        //});

        //taskContainer.groups = ko.observableArray(groups);
        //taskContainer.tasks = ko.observableArray(tasks);
        //taskContainer.selectedGroup = ko.observable(groups[0]);
        //taskContainer.isSelected = ko.observable(groups[0]);
        //taskContainer.selectedGroupName = ko.observable(tasks[0].groupName);
        taskContainer.groups = groups;
        //taskContainer.tasks = tasks;
        //taskContainer.taskIds = ko.observableArray();
        //for (var i = 0; i < taskContainer.tasks[0].tasksArray.length; i++) {
        //    taskContainer.taskIds.push(taskContainer.tasks[0].tasksArray[i].id);
        //}
        //taskContainer.selectedGroup = tasks[0];
        viewModelObj = ko.mapping.fromJS(taskContainer);
        viewModelObj.selectedGroup = ko.observable(viewModelObj.groups()[0]);
        viewModelObj.filteredGroups = ko.observableArray(viewModelObj.groups());
        //smtaskManagerMain.GenerateGraph(ko.mapping.toJS(viewModelObj.groups()[0].children()[0].chain_id()));

        viewModelObj.onClickGroupItem = function (obj, event) {
            viewModelObj.selectedGroup(obj);
            $('#SMeditable').css('width', ((obj.chain_name().length + 1) * 8) + 'px');  //dynamic input text element

            smtaskManagerMain.GenerateGraph(ko.mapping.toJS(obj.children()[0].chain_id()));
            if ($('.SMTaskNameSetup_Scrollable').length > 0)
                applySlimScroll('.SMTaskNameSetup_Scrollable', '.SMTaskNameSetup_TaskNamesBinding');
        }
        // for handling multiple bindings....
        //TaskBindingAreaContainer = $("#TaskBindingAreaContainer").html();
        //$("#TaskBindingAreaContainer").empty();
        //$("#TaskBindingAreaContainer").html(TaskBindingAreaContainer);

        //ko.cleanNode($("#TaskBindingArea")[0]);
        if (isApplyBindingTrue == 'false') {
            ko.applyBindings(viewModelObj, document.getElementById("TaskBindingArea"));
            isApplyBindingTrue = 'true';
        }
        else {
            refillGroup(viewModelObj);
        }

        var subHeight = $('#toolbar').outerHeight();
        $("#TaskBindingArea").height($(window).height() - subHeight - 30);
        $('.SMTaskGroupSetup_Scrollable').height($(window).height() - subHeight - 31);
        $('#SMTaskNamesContainer').height($(window).height() - subHeight - 31);

        if (groups.length > 0)
            $('#SMeditable').css('width', ((viewModelObj.groups()[0].chain_name().length + 1) * 8) + 'px');  //dynamic input text element

        $('#chainNameTxt').css('width', '140px');


        $('#SMTaskNameSetup_Task_Contents').height($('#SMTaskNamesContainer').height() - ($('.SMTaskNameSetup_TaskLevel_Contents').height() + 25)); // + $('#taskChart').height()
        $('.SMTaskNameSetup_Scrollable').height($('#SMTaskNameSetup_Task_Contents').height() - 50);
        //var left = $('.SMTaskGroupSetup_Scrollable').width() + 3;
        //$('#SMTaskNamesContainer').height($(window).height() - subHeight - 31); // for graph
        //$('#configureTasksPopup').height($(window).height() - subHeight - 31);
        $('#popupBodyExpand').height($(window).height() - subHeight - 30);


        if ($('.SMTaskGroupSetup_Scrollable').length > 0)
            applySlimScroll('.SMTaskGroupSetup_Scrollable', '.SMTaskGroupSetup_TaskGroupNames');
        if ($('.SMTaskNameSetup_Scrollable').length > 0)
            applySlimScroll('.SMTaskNameSetup_Scrollable', '.SMTaskNameSetup_TaskNamesBinding');

        // this task only
        $(".SMtriggerType[type='0']").click(function (e) {
            smtaskManagerMain.Manual_Task_Trigger();
        });
        // this task onwards
        $(".SMtriggerType[type='1']").click(function (e) {
            smtaskManagerMain.Task_Chain_Trigger();
        });


    }

    SMTaskSetupClass.prototype.SearchData = function SearchData(value) {
        var groupId = null;
        $.each(taskNameSearchData, function (key, object) {
            if (key.toLowerCase().indexOf(value.toLowerCase()) !== -1) {
                groupId = object[0];
                return;
            }
        });

        viewModelObj.groups().forEach(function (ele, index) {
            if (ele.chain_id() == groupId) {
                viewModelObj.selectedGroup(ele);
                return;
            }
        });
    }

    function refillGroup(groupviewModelObj) {
        if (groupviewModelObj.groups().length > 0) {
            viewModelObj.selectedGroup(groupviewModelObj.groups()[0]);
            viewModelObj.filteredGroups(groupviewModelObj.groups());
            $('#SMeditable').css('width', ((viewModelObj.groups()[0].chain_name().length + 1) * 8) + 'px');  //dynamic input text element
        }
    }

    Date.prototype.formatCTM = function (f) {
        if (!this.valueOf())
            return ' ';

        var d = this;

        return f.replace(/(yyyy|yy|mmmm|mmm|mm|m|dddd|ddd|dd|d|hh|tfh|nn|ss|fff|a\/p)/gi,
                    function ($1) {
                        switch ($1.toLowerCase()) {
                            case 'yyyy': return d.getFullYear();
                            case 'yy': return d.getYear();
                            case 'mmmm': return gsMonthNames[d.getMonth()];
                            case 'mmm': return gsMonthNames[d.getMonth()].substr(0, 3);
                            case 'mm': return (d.getMonth() + 1).zf(2);
                            case 'm': return (d.getMonth() + 1);
                            case 'dddd': return gsDayNames[d.getDay()];
                            case 'ddd': return gsDayNames[d.getDay()].substr(0, 3);
                            case 'dd': return d.getDate().zf(2);
                            case 'd': return d.getDate();
                            case 'hh': return ((h = d.getHours() % 12) ? h : 12).zf(2);
                            case 'tfh': return d.getHours().zf(2);
                            case 'nn': return d.getMinutes().zf(2);
                            case 'ss': return d.getSeconds().zf(2);
                            case 'fff': return d.getMilliseconds();
                            case 'a/p': return d.getHours() < 12 ? 'a' : 'p';
                        }
                    }
                );
    }
    String.prototype.zf = function (l) {
        var initial = '';
        for (var i = l; i > this.length; i--)
            initial = '0' + initial;
        return initial + this;
    }
    Number.prototype.zf = function (l) { return this.toString().zf(l); }




    return smTaskSetup;
})();