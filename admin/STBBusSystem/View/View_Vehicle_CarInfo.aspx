<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_Vehicle_CarInfo.aspx.cs" Inherits="View_Vehicle_CarInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="copyright" content="" />
<link href="<%=ResolveUrl("~/EasyUi/themes/default/easyui.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/icon.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/Ui.css")%>" type="text/css" rel="stylesheet" />
<script src="<%=ResolveUrl("~/EasyUi/jquery-1.10.2.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/jquery.easyui.min.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/locale/easyui-lang-zh_CN.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/datagrid-export.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/jquery.color.js")%>" type="text/javascript"></script>


</head>

<body>
    <div id="container">
        <div id="pageContent">
           
            <div class="panel-header_toorbar">
                <a id="add"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true">新增</a>
                <a style=" display:none;" id="edit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true">編輯</a>
                <a  style=" display:none;" id="delete" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true">删除</a>
                <a  style=" display:none;" id="view"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-magnifier',plain:true">查看</a>
                <a id="refresh" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-refresh',plain:true">刷新</a>
            </div>
            <div id="seachbar" class="easyui-accordion" style="width:100%;height:0;">
                <div id="search_bar" title="搜索" data-options="iconCls:'icon-search'" style="padding:45px;background-color:#eaf2ff;overflow: hidden;">
                
                     <div class="search-container">
                        <div class="search-content">
                            <div class="item">
                                 <input id="txt_carName" name="carName" label="汽車品牌"   type="text"/>
                            </div>
                             <div class="item">
                                 <input id="txt_carID"  name="carID" label="車牌號"   type="text"/>
                            </div>
                             <div class="item">
                                 <input id="txt_type"  name="type" label="車輛顏色"   type="text"/>
                            </div>
                             <div class="item">
                                 <input id="txt_seatNum" name="seatNum" label="汽車可載人數"  type="text"/>
                            </div>

                            <div class="item" style="float:left;width:450px;" >
                                 <input id="txt_Create_Date"  name="Create_Date" label="日期查詢" type="text" />To
                                 <input id="txt_Create_Date_T"  type="text"/>
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
   
        <div id="dialog" class="easyui-dialog"  data-options="closed:true,modal: true" style="width:300px;height:400px;padding:10px;background-color:#EAF2FF">
            <form id="dlg_From"  method="post">
                <div class="Form_item">
                    <input id="edit_carName" name="carName"  label="車輛品牌:">
                </div>
                <div class="Form_item">
                    <input id="edit_seatNum" name="seatNum"  label="車輛可載人數:">
                </div>
                <div class="Form_item">
                    <input id="edit_type" class="easyui-color" name="type"  label="車輛顏色:">
                    <a id="edit_type_clean" href="javascript:void(0)" class="easyui-linkbutton bott" data-options="iconCls:'icon-no'">清空</a>
                </div>
                <div class="Form_item">
                    <input id="edit_carID" name="carID"  required="true" label="車牌號:">
                </div>
                 <div class="Form_item">
                    <input id="edit_CreateDate" name="Create_Date"  label="創建時間:">
                </div>
                 <div class="Form_item">
                    <input id="edit_CreateUser" name="Create_User"  label="創建人:">
                </div>          
            </form>
          <%--  <div id="bottom_btn">
                <a id="Save" href="javascript:void(0)" class="easyui-linkbutton bott " style="width:80px">保存</a>
                <a id="Edit_Clear" href="javascript:void(0)" class="easyui-linkbutton bott" style="width:80px">取消</a>
            </div>--%>
        </div>

    </div>
    
    <div id="dgmenu" class="easyui-menu" style="width:120px;" data-options="onClick:gridMenuHandler">
            <div data-options="name:'add',iconCls:'icon-add'">新增</div>
            <div data-options="name:'edit',iconCls:'icon-edit'">編輯</div>
            <div data-options="name:'del',iconCls:'icon-remove'">删除</div>
            <div data-options="name:'view',iconCls:'icon-search'">查看</div>
  </div>
    
   <!-- #include file="../js/Vehicle_CarInfo.js.inc" -->
 
</body>
</html>
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>
  <style>
      .colorpicker {
    z-index: 9999;
}
  </style>



