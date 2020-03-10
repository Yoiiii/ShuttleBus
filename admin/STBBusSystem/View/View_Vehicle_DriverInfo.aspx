<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_Vehicle_DriverInfo.aspx.cs" Inherits="View_Vehicle_DriverInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
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

<body>
    <div id="container">
        <div id="pageContent">
           
            <div class="panel-header_toorbar">
               <%-- <a id="add"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true">新增</a>--%>
                <a style=" display:none;" id="edit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true">編輯</a>
              <%--  <a  style=" display:none;" id="delete" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true">删除</a>--%>
                <a  style=" display:none;" id="view"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-magnifier',plain:true">查看</a>
                <a id="refresh" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-refresh',plain:true">刷新</a>
            </div>
            <div id="seachbar" class="easyui-accordion" style="width:100%;height:0;">
                <div id="search_bar" title="搜索" data-options="iconCls:'icon-search'" style="padding:45px;background-color:#eaf2ff;overflow: hidden;">
                
                     <div class="search-container">
                        <div class="search-content">
                            <div class="item">
                                 <input id="txt_Driver_Name" name="Driver_Name" label="司機姓名" type="text"/>
                            </div>
                            <div class="item">
                                 <input id="txt_Driver_Number" name="Driver_Number" label="司機工號"  type="text"/>
                            </div>
                             <div class="item">
                                 <input id="txt_Driver_Tel" name="Driver_Tel" label="聯繫電話" type="text"/>
                            </div>
                            <div style=" clear:both;"></div>
                          <%--   <div class="item">
                                <input id="txt_Driver_Company"  data-options="panelHeight:'auto'" name="txt_Driver_Company" label="所屬公司" >
                            </div>--%>
                        <%--    <div class="item" style="float:left;width:380px;" >
                                 <input id="txt_Create_Date"  name="Create_Date" label="日期查詢" type="text"/>To
                                 <input id="txt_Create_Date_T"   type="text"/>
                            </div>--%>
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
        
        <div style="margin-top: 3px;bottom: 0; height: 100%; width:100%"  >
            <table id="DateGrid"  data-options="iconCls:'icon-layout'"></table>
        </div>
   
        <div id="dialog" class="easyui-dialog"  data-options="closed:true,modal: true" style="width:400px;height:400px;padding:10px;background-color:#EAF2FF">
            <form id="dlg_From"  method="post">
                <div class="Form_item">
                    <input id="edit_Driver_Name" name="Driver_Name" required="true" label="司機姓名:">
                </div>
                <div class="Form_item">
                    <input id="edit_Driver_Number" name="Driver_Number" required="true" label="司機工號:">
                </div>
               
            <%--    <div class="Form_item">
                    <inpput id="edit_Driver_Company" data-options="panelHeight:'auto'" required="true" label='所屬公司:' name="Driver_Company" >
                </div>--%>
                <div class="Form_item">
                    <input id="edit_Driver_Tel" name="Driver_Tel"  required="true"  label="联系电话:">
                </div>
                <div class="Form_item">
                    <input id="edit_CarInfo"    name="CarInfo"  label="车牌号:">
		        </div>
       <%--           
                <div class="Form_item">
                    <input id="edit_CreateDate" name="Create_Date" label="創建時間:">
                </div>
                 <div class="Form_item">
                    <input id="edit_CreateUser" name="Create_User" label="創建人:">
                </div>     --%>     
            </form>
          <%--  <div id="bottom_btn">
                <a id="Save" href="javascript:void(0)" class="easyui-linkbutton bott " style="width:80px">保存</a>
                <a id="Edit_Clear" href="javascript:void(0)" class="easyui-linkbutton bott" style="width:80px">取消</a>
            </div>--%>
        </div>

    </div>
    
    <div id="dgmenu" class="easyui-menu" style="width:120px;" data-options="onClick:gridMenuHandler">
      <%--      <div data-options="name:'add',iconCls:'icon-add'">新增</div>--%>
            <div data-options="name:'edit',iconCls:'icon-edit'">編輯</div>
    <%--        <div data-options="name:'del',iconCls:'icon-remove'">删除</div>--%>
            <div data-options="name:'view',iconCls:'icon-search'">查看</div>
  </div>
    
   <!-- #include file="../js/Vehicle_DriverInfo.js.inc" -->
 
</body>
</html>
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>



