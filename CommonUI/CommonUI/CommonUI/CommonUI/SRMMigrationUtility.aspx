<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMMigrationUtility.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMMigrationUtility" 
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <script src="includes/fileUploadScript.js" type="text/javascript"></script>
    <script src="includes/SRMMigrationUtility.js" type="text/javascript"></script>
    <script src="includes/SRMMigrationFileUploadControl.js" type="text/javascript"></script>
    <link href="css/SRMMigrationUtility.css" rel="stylesheet" type="text/css"/>

    <iframe id="srmmigration-iframe" src="" class="srmmigration-display-none"></iframe>
    <div id="srmmigration-parent">
        <div id="srm-migration-top-section">
             <div id="srmmigration-buttons-parent">           
                <div class="srmmigration-button" id="srmmigration-export-master-button" data-bind="css: !isAtleastOneFeatureSelected() ? 'srmigration-disabled-button-class':'', attr:{'title': !isAtleastOneFeatureSelected() ? 'Select atleast one feature':'Click to download'}">
                    <div class="srmmigration-button-icon srmmigration-bulk-button-children">
                        <div class="srmmigration-button-download-master-icon"></div>
                    </div>
                    <div class="srmmigration-button-text-parent srmmigration-bulk-button-children">
                        <span class="srmmigration-button-text">DOWNLOAD</span>
                    </div>

                    <%--div to display download options--%>
                    <%--<div id="srmmigration-bulk-download-details" class="srmmigration-display-none">
                        <div class="srmmigration-selected-list-view-downloadtype-selection-parent srmmigration-bulk-download-children srmmigration-bulk-download-filetype">
                            <span class="srmmigration-filetype-text">File Type </span>
                            <div class="srmmigration-downloadtype-excel srmmigration-download-type-children">Excel</div>
                            <div class="srmmigration-downloadtype-xml srmmigration-download-type-children">XML</div>
                        </div>

                        <div id="srmmigration-bulk-download-details-button-parent" class="srmmigration-bulk-download-children">
                            <div id="srmmigration-bulk-download-details-button" data-bind="click: onBulkDownloadClick">
                                <span>Download</span>
                            </div>
                        </div>
                    </div>--%>

                     <div id="srmmigration-bulk-download-details" class="srmmigration-display-none">
                        <div id="srmmigration-bulk-download-excel-click" class="srmmigration-bulk-download-click-children-popup" data-bind="click: onBulkDownloadClick">
                            <div class="srmmigration-excel-icon"></div><span class="srmmigration-bulk-download-option-text">Excel</span>
                        </div>
                        <div id="srmmigration-bulk-download-xml-click" class="srmmigration-bulk-download-click-children-popup"  data-bind="click: onBulkDownloadClick">
                            <div class="srmmigration-xml-icon"></div><span class="srmmigration-bulk-download-option-text">XML</span>
                        </div>
                    </div>
                </div>

                <div class="srmmigration-button" id="srmmigration-difference-master-button" data-bind="css: !privilegeForCompare() ? 'srmigration-disabled-button-class':'', attr:{'title': !privilegeForCompare() ? 'You do not have the privilege.':'Click to compare.'}">
                    <div class="srmmigration-button-icon srmmigration-bulk-button-children srmmigration-button-icon-adjust-margin">
                        <div class="srmmigration-button-downloaddiff-master-icon"></div>
                    </div>
                    <div class="srmmigration-button-text-parent srmmigration-bulk-button-children">
                        <span class="srmmigration-button-text">COMPARE</span>
                    </div>
                    <%--<div id="srmmigration-bulk-upload-placeholder-download-diff" class=""></div>--%>
                    <div class="fa fa-caret-down srmmigration-modules-item-arrow-bulk" data-bind="visible: isBulkDownloadDiffClicked"></div>
                </div>

                <div class="srmmigration-button" id="srmmigration-import-master-button"  data-bind="css: !privilegeForUpload() ? 'srmigration-disabled-button-class' : '', attr: { 'title': !privilegeForUpload() ? 'You do not have the privilege.' : 'Click to upload.' }">
                    <div class="srmmigration-button-icon srmmigration-bulk-button-children ">
                        <div class="srmmigration-button-upload-master-icon"></div>
                    </div>
                    <div class="srmmigration-button-text-parent srmmigration-bulk-button-children">
                        <span class="srmmigration-button-text">UPLOAD</span>
                    </div>
                    
                    <div class="fa fa-caret-down srmmigration-modules-item-arrow-bulk" data-bind="visible: isBulkUploadClicked"></div>
                </div>
                   
                

                
            </div>
            <div id="srmmigration-bulk-upload-placeholder-upload" class=""></div>
            <div id="srmmigration-bulk-upload-placeholder-download-diff" class=""></div>
           
        </div>

        <div id="migration-utility-bottom-section">

            <div id="srmmigration-module-list-parent">
                <div id="srmmigration-modules-list" data-bind="foreach: moduleList">
                    <div class="srmmigration-modules-item">
                        <div class="srmmigration-modules-content">
                            <div class="srmmigration-modules-item-top" data-bind="click: onSelectionClick, attr: { title: isSelected() ? 'Click to unselect':'Click to select'}">
                                <div class="srmmigration-modules-item-top-icon-div srmmigration-modules-item-top-children ">
                                    <div class="fa srmmigration-circle-div-top" data-bind="css: !isSelected() ?'':'fa-check'" ></div>
                                </div>
                                <div class="srmmigration-modules-item-top-text-parent srmmigration-modules-item-top-children">
                                    <span class="srmmigration-modules-item-top-text" data-bind="text: moduleName"></span>
                                </div>
                            </div>

                            <div class="srmmigration-module-separator-div">
                            </div>

                            <div class="srmmigration-modules-item-bottom">
                                <div class="srmmigration-modules-item-bottom-icon-div">
                                    <div class="srmmigration-button-download-icon srmmigration-modules-item-bottom-icons" actionType="Download" data-bind="click: onClickSelectionPopup, css: isDownloadClicked()?'srmmigration-button-download-icon-selected':''"  title="Click to download">
                                    </div>
                                    <div class="fa fa-caret-down srmmigration-modules-item-arrow" data-bind="visible: isDownloadClicked"></div>
                                </div>

                                <div class="srmmigration-modules-item-bottom-icon-div">
                                    <div class="srmmigration-button-downloaddiff-icon srmmigration-modules-item-bottom-icons" actionType="Diff" data-bind="click: $parent.privilegeForCompare() ? onClickUploadDiv : function () { return false; }, css: isDownloadDiffClicked() ? 'srmmigration-button-downloaddiff-icon-selected' : '', attr: { title: $parent.privilegeForCompare() ? 'Click to compare' : 'You do not have privilege' }, style: { cursor: $parent.privilegeForCompare() ? 'pointer !important' : 'default !important' }" >
                                    </div>
                                    <div class="fa fa-caret-down srmmigration-modules-item-arrow" data-bind="visible: isDownloadDiffClicked"></div>
                                </div>

                                <div class="srmmigration-modules-item-bottom-icon-div">
                                    <div class="srmmigration-button-upload-icon srmmigration-modules-item-bottom-icons" actionType="Upload" data-bind="click: $parent.privilegeForUpload() ? onClickUploadDiv : function () { return false; }, css: isUploadClicked() ? 'srmmigration-button-upload-icon-selected' : '', attr: { title: $parent.privilegeForUpload() ? 'Click to upload' : 'You do not have privilege' }, style: { cursor: $parent.privilegeForUpload() ? 'pointer !important' : 'default !important' }">
                                    </div>
                                    <div class="fa fa-caret-down srmmigration-modules-item-arrow" data-bind="visible: isUploadClicked" ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="srmmigration-download-selection-placeholder">
                <div id="srmmigration-selected-list-view-parent" class="srmmigration-display-none srmmigration-selection-placeholder-children">
                    <div class="srmmigration-selected-list-view-top-section srmmigration-selected-list-view-children">

                        <div class="srmmigration-search-checkbox-parent srmmigration-selected-list-view-top-section-children">
                            <input type="text" class="srmmigration-search-inputbox" data-bind="value: searchText, valueUpdate: 'afterkeydown'" placeholder="Enter search text" />
                            <div class="fa fa-search srmmigration-search-icon"></div>
                        </div>

                        <div class="srmmigration-selected-list-view-selection-button-parent srmmigration-selected-list-view-top-section-children">
                            <div class="srmmigration-selected-list-view-selection-button" data-bind="click: SelectAllCheckboxItems">
                                <span class="srmmigration-selected-list-view-selection-button-text">Select All</span>
                            </div>
                            <div class="srmmigration-selected-list-view-selection-button">
                                <span class="srmmigration-selected-list-view-selection-button-text" data-bind="click: UnSelectAllCheckboxItems">Clear Selection</span>
                            </div>
                        </div>
                    </div>

                    <div class="srmmigration-close-list-view-parent srmmigration-close-x">X</div>

                    <div class="srmmigration-action-text-on-click"><span></span></div>
                
                    <div class="srmmigration-selected-list-view srmmigration-selected-list-view-children srmmigration-common-search-divs" data-bind="foreach: checkboxList, visible: isSearchResultAvailableForCheckboxes()">
                        <div class="srmmigration-selected-list-item" data-bind="click: onSelectionClick, visible: isVisible">
                            <div class="srmmigration-selected-list-item-text-parent" data-bind="attr: {'itemId' : id}">
                                <span class = "srmmigration-selected-list-item-text" data-bind="text: itemName"></span>
                            </div>
                             <div class="srmmigration-selected-list-item-icon-parent fa fa-check srmmigration-circle-div" data-bind="visible: isSelected">
                            </div>
                        </div>
                    </div>

                    <div class="srmmigration-no-search-result-exists srmmigration-common-search-divs" data-bind="visible: !isSearchResultAvailableForCheckboxes()">
                        <div class="srmmigration-no-search-result-icon"></div>
                        <div class="srmmigration-no-search-result-text">No items found matching the search criteria</div>
                    </div>
                
                    <div class="srmmigration-selected-list-view-download-button-parent srmmigration-selected-list-view-children">
                        <div class="srmmigration-selected-list-view-downloadtype-selection-parent srmmigration-margin-for-toggle-filetype">
                            <span class="srmmigration-filetype-text">File Type </span>
                            <div class="srmmigration-downloadtype-excel srmmigration-download-type-children">Excel</div>
                            <div class="srmmigration-downloadtype-xml srmmigration-download-type-children">XML</div>
                        </div>

                        <div class="srmmigration-selected-list-view-download-button" data-bind="click: !isAtleastOneCheckboxSelected() ? 'return false;' : OnDownloadCheckboxClick, css: !isAtleastOneCheckboxSelected() ? 'srmigration-disabled-button-class' : '', attr: { 'title': !isAtleastOneCheckboxSelected() ? 'Select atleast one.' : 'Click to download.' }">
                            <span class="srmmigration-selected-list-view-download-button-text">Download</span>
                        </div>
                    </div>
                
                </div>
            </div>

            <div id="srmmigration-upload-selection-placeholder">

                <div id="srmmigration-upload-section" class="srmmigration-display-none srmmigration-selection-placeholder-children"> 
                
                    <%--RAD FILE UPLOAD NOT USED ANYMORE--%>
                    <%--<div id="SRMMigrationFileUpload" style="width: 687px; margin-left: 5px;">
                        <div style="width: 100%; cursor: pointer; margin: 0px auto; border: 0px;" id="SRMMigrationUploadWidget">
                            <div class="SRMMigrationLabeledInput" style="width: 97%; border: 0px; padding: 0px; text-indent: 8px;"
                                id="SRMMigrationFileUploadAttachement">
                                Click here or drop a file to upload
                            </div>
                        </div>
                     </div>--%>

                    <%--CUSTOM FILE UPLOAD--%>
                    <div class="PlaceholderSpaceDiv"></div>
                
                    <div class="srmmigration-close-list-view-parent srmmigration-close-x">X</div>

                    <div class="srmmigration-action-text-on-click"><span></span></div>

                    <div class="srmmigration-file-upload-div" CurrentFilesLoaded="" isReadOnly="false" directUploadFile="true">
                        FILE UPLOAD DIV
                    </div>
                
                    <div id="srm-migration-upload-helpers" class="srmmigration-display-none">
                        <div class="srmmigration-upload-require-missing-table srm-migration-upload-helpers-children">
                            <span class="srmmigration-requiremissing-text">Include Missing Configurations in uploaded file</span>
                            <div class="srmmigration-upload-require-missing-yes srmmigration-upload-require-missing-table-children">Yes</div>
                            <div class="srmmigration-upload-require-missing-no srmmigration-upload-require-missing-table-children">No</div>
                        </div>

                        <div class="srmmigration-selected-list-view-downloadtype-selection-parent srm-migration-upload-helpers-children">
                            <span class="srmmigration-filetype-text">File Type </span>
                            <div class="srmmigration-downloadtype-excel srmmigration-download-type-children">Excel</div>
                            <div class="srmmigration-downloadtype-xml srmmigration-download-type-children">XML</div>
                        </div>

                        <div id="srm-migration-upload-listview-button-parent" class="srm-migration-upload-helpers-children">
                            <div id="srm-migration-upload-listview-button" data-bind="click: OnUploadCheckboxClick">
                                <span>Upload</span>
                            </div>
                        </div>

                        <div id ="srm-migration-upload-listview-button-parent-close" class="srm-migration-upload-helpers-children">
                            <div id="srm-migration-upload-listview-button-close">Reset</div>
                        </div>
                    </div>

                    <div class="PlaceholderSpaceDiv"></div>
                </div>
            </div>
        </div>

        <%--//error popup--%>
        <div class="srmmigration">
            <div class="message-popup-container srmmigration-display-none">
                <div class="message-popup">
                    <div class="popup-title-bar">
                        <div class="popup-title">
                            <div class="title-image"></div>
                            <div class="title-text"></div>
                        </div>
                        <div class="srmmigration-close-popup popup-close-btn srmmigration-close-x">X</div>
                    </div>
                    <div class="popup-content"></div>
                    <div class="popup-content-upload-error srmmigration-display-none">
                        <div class= "srmmigration-upload-error-div-parent">
                            <div class="srmmigration-upload-error-div-items-header">
                                <div class="feature-name items">Feature</div>
                                <div class="status items">Status</div>
                                <div class="error-message items">Error</div>
                                <div class="download items">Download Status</div>
                            </div>
                            
                            <div class ="srmmigration-upload-error-div-items-parent" data-bind="foreach: errorPopupList">
                                <div class ="srmmigration-upload-error-div-item">
                                    <div class="feature-name items" data-bind="text: FeatureDisplayName"></div>
                                    <div class="status items" data-bind="text: SyncStatus, css: !(SyncStatus == 'Failure') ? 'success-color' : 'failure-color'"></div>
                                    <div class="error-message items" data-bind="text: ErrorMsg, attr: { 'title': ErrorMsg }"></div>
                                    <div class="download items">
                                        <div class="download-feature-icon" data-bind="click: downloadFeatureSheet, visible: IsDownloadable">
                                    </div>
                                </div>
                            </div>   
                        </div>
                        <div class= "srmmigration-upload-error-download-zip-parent" data-bind="visible: errorPopupList().length>1">
                            <div class="srmmigration-download-all-button" data-bind="click: downloadAllZip">Download All</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>


<asp:Content ID="contentBottom" ContentPlaceHolderID="SRMContentPlaceHolderBottom" runat="server">
    <div id="disableDivForPopup" class="alertBG" style="display: none; z-index: 100;background-color: black !important" align="center"></div>
</asp:Content>