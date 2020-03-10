<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_Vehicle_adminDispatch.aspx.cs" Inherits="STBBusSystem.View.View_Vehicle_adminDispatch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="copyright" content="" />

<link href="<%=ResolveUrl("~/EasyUi/themes/default/easyui.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/icon.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/Ui.css")%>" type="text/css" rel="stylesheet" />
<script src="<%=ResolveUrl("~/EasyUi/jquery-1.7.2.min.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/jquery.easyui.min.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/locale/easyui-lang-zh_CN.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/datagrid-export.js")%>" type="text/javascript"></script>


</head>
<style>
    .Form_item{
        padding: 6px;
        margin-top:18px;
        
    }
    #ApplyInfo>.Form_item{
        float:left; 
    }
    #ApplyDriver>.Form_item{
        float:left; 
    }
    #ApplyRegister>.Form_item{
        float:left; 
    }
    #ApplyNow>.Form_item{
        float:left; 
    }
    #ApplyState>.Form_item{
        float:left; 
    }
    

</style>
<body>
    <div id="container">
        <div id="pageContent">
            <div class="panel-header_toorbar">
                <a id="add"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true">新增</a>
                
                <a style=" display:none;" id="edit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true">編輯</a>
                <%--<a  style=" display:none;" id="delete" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true">删除</a>--%>
                <a  style=" display:none;" id="remove" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true">作廢</a>
                <a  style=" display:none;" id="view"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-magnifier',plain:true">查看</a>
                <a id="refresh" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-refresh',plain:true">刷新</a>
                <a id="Merge" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-export',plain:true">合併派車單</a>
                <a id="Import_WorkFlow" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-export',plain:true">WorkFlow數據導入</a>
                
            </div>
            <div id="seachbar" class="easyui-accordion" style="width:100%;height:0;">
                <div id="search_bar" title="搜索" data-options="iconCls:'icon-search'" style="padding:45px;background-color:#eaf2ff;overflow: hidden;">
                    <div class="search-container" style=" width:100%;">
                        <div class="search-content">
                            <div class="item">
                                <input id="txt_driverID" name="driverID" date-bind="aaa" label="司機" type="text"/>
                            </div>
                            <div class="item">
                                <input id="txt_carID"  name="carID" label="車牌號"  type="text"/>
                            </div>
                            <div class="item">
                                <input id="txt_status"  name="status" label="狀態"  type="text"/>
                            </div>
                            
                            <div class="item" style="float:left;width:450px;" >
                                <input id="txt_CreateDate"  name="CreateDate" label="日期查詢" type="text" />To
                                <input id="txt_CreateDate_T"  type="text"/>
                            </div>
                        </div>
                        <div class="button-bar">
                            <div class="item">
                                <a id="search" style="margin-right:10px" href="javascript:void(0)" class="easyui-linkbutton bott" data-options="iconCls:'icon-search'">搜索</a>
                            </div>
                            <div class="item">
                                <a id="clear" href="javascript:void(0)" class="easyui-linkbutton bott" data-options="iconCls:'icon-no'">清除</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="Grid" style="margin-top: 3px;bottom: 0; height: 100%; width:100%"  >
            <table id="DateGrid" width="100%" data-options="iconCls:'icon-layout'" ></table>
        </div>
    <div id="dialog" class="easyui-dialog"  data-options="closed:true,modal: true" style=" padding:10px;background-color:#EAF2FF">
            
            <div style="width:100%">
                <div style="bottom: 0;width:30%;float:left;height:100%"  >
                    <table id="stepGrid" width="100%"  data-options="iconCls:'icon-layout'"></table>
                </div>    
                <div id="from_table" style="width:70%;float:left;" >
                    <form id="dlg_From"   method="post">
                        <div id="ApplyRegister" >
                            <div class="Form_item">
                                <input id="edit_CreateUser" name="CreateUser"  label="創建人:">
                            </div> 
                            <div class="Form_item">
                                <input id="edit_CreateDate" name="CreateDate"  label="創建時間:">
                            </div>
                        </div>
                        <div style="clear: both"></div>                    
                        <div id="ApplyInfo" >
                            <div class="Form_item">
                                <input  id="edit_planID" name="planID"  label="派車單號:">
                            </div>
                            <div class="Form_item">
                                <input  id="edit_go" name="go" >
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                        <div id="ApplyDriver">
                            <div class="Form_item">
                                <input id="edit_driverID"  required=true name="driverID"  label="司機姓名:">
                            </div>
                            <div class="Form_item">
                                <input id="edit_driverTel"  required=true name="driverTel"  label="聯繫方式:">
                            </div>
                        </div>
                        <div style="clear: both"></div>     
                        <div id="ApplyState">  
                            <div class="Form_item">
                                <input id="edit_carID" required=true name="carID"  label="車牌號:">
                            </div>
                            <div class="Form_item">
                                    <input id="edit_status" name="status"  label="狀態:">
                            </div>
                        </div>   
                        <div style="clear: both"></div>   
                        <div id="ApplyNow">
                            <div class="Form_item">
                                    <input id="edit_time" required=true  name="time"  label="出發時間:">
                            </div>
                            <div class="Form_item" style="display:none">
                                <input id="edit_step" name="step"  label="當前站點:">
                            </div>
                        </div> 

                    </form>
                </div>
            </div>
            <div style="bottom: 0;margin-top:5%" >
                    <table id="PassengerGrid" width="100%"  data-options="iconCls:'icon-layout'"></table>
            </div> 
            
            
            
            
            
        </div>
        <div id="ChoseRetdialog" class="easyui-dialog"  data-options="closed:true,modal: true" style=" padding:10px;background-color:#EAF2FF">
            <table id="MainGrid" width="100%" ></table>
            <table id="MainDtlGrid" width="100%"  ></table>
        </div>

    </div>
    
    <div id="dgmenu" class="easyui-menu" style="width:120px;" data-options="onClick:gridMenuHandler">
            <div data-options="name:'add',iconCls:'icon-add'">新增</div>
            <div data-options="name:'edit',iconCls:'icon-edit'">編輯</div>
            <div data-options="name:'remove',iconCls:'icon-remove'">作废</div>
            <div style="display:none" data-options="name:'del',iconCls:'icon-remove'">删除</div>
            <div data-options="name:'view',iconCls:'icon-search'">查看</div>
            
            
  </div>
 <div id="tb_site" style="height:auto;width:auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-add',plain:true" onclick="stepappend()">新增</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-minus',plain:true" onclick="stepremoveit()">刪除</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-tick',plain:true" onclick="stepconfirm()">確認</a>
 </div>
 <div id="tb_desc" style="height:auto;width:auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-add',plain:true" onclick="Passengerappend()">新增</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-minus',plain:true" onclick="Passengerremoveit()">刪除</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-tick',plain:true" onclick="Passengerconfirm()">確認</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-bullet-add',plain:true" onclick="ChoseRetPassenger()">選擇回程乘客</a>
 </div>
    
   <!-- #include file="../js/Vehicle_adminDispatch.js.inc" -->
 
</body>
</html>
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>

