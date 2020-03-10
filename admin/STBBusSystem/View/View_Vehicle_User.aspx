<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_Vehicle_User.aspx.cs" Inherits="STBBusSystem.View.View_Vehicle_User" %>


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
<body>
<div id="container">
        <div id="pageContent">
           
            <div class="panel-header_toorbar">
                <a id="add"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true">新增</a>
                <a  style=" display:none;" id="edit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true">編輯</a>
              <%--  <a  style=" display:none;" id="delete" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true">删除</a>--%>
                <a  style=" display:none;" id="view"  href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-magnifier',plain:true">查看</a>
                <a id="refresh" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-refresh',plain:true">刷新</a>
            </div>
            <div id="seachbar" class="easyui-accordion" style="width:100%;height:0;">
                <div id="search_bar" title="搜索" data-options="iconCls:'icon-search'" style="padding:45px;background-color:#eaf2ff;overflow: hidden;">
                
                     <div class="search-container" style=" width:100%;">
                        <div class="search-content">
                            <div class="item">
                                 <input id="txt_username"  name="Searchusername" type="text" label="name:"/>
                            </div>
                            <div class="item">
                                 <input id="txt_userid"  name="Searchuserid" type="text" label="工號:"/>
                            </div>
                            <div class="item">
                                 <input id="txt_name"  name="Searchname" type="text" label="名字:"/>
                            </div>
                             <div class="item">
                                 <input id="txt_type"  name="Searchname" type="text" label="類型:"/>
                            </div>
                        </div>
                        <div class="button-bar" style=" position:absolute; right:10px; bottom:-30px">
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
            <table id="DateGrid" width="100%" data-options="iconCls:'icon-layout'"></table>
        </div>
   
        <div id="dialog" class="easyui-dialog"  data-options="closed:true,modal: true" style="width:300px;height:400px;padding:10px;background-color:#EAF2FF">
            <form id="dlg_From"  method="post">
                <div class="Form_item">
                    <input id="edit_userid" name="userid"  required="true" label="工號:">
                </div>
                <div class="Form_item">
                    <input id="edit_username" name="username"  required="true" label="name:">
                </div>
                <div class="Form_item">
                    <input id="edit_name" name="name"   label="名字:">
                </div>
                <div class="Form_item">
                    <input id="edit_tel" name="tel"    label="聯繫方式:">
                </div>
                <div class="Form_item">
                    <input id="edit_type" name="type"     label="類型:">
                </div>        
            </form>
          <%--  <div id="bottom_btn">
                <a id="Save" href="javascript:void(0)" class="easyui-linkbutton bott " style="width:80px">保存</a>
                <a id="Edit_Clear" href="javascript:void(0)" class="easyui-linkbutton bott" style="width:80px">取消</a>
            </div>--%>
        </div>

    <div id="dgmenu" class="easyui-menu" style="width:120px;" data-options="onClick:gridMenuHandler">
            <div data-options="name:'add',iconCls:'icon-add'">新增</div>
            <div data-options="name:'edit',iconCls:'icon-edit'">編輯</div>
            <%--<div data-options="name:'del',iconCls:'icon-remove'">删除</div>--%>
            <div data-options="name:'view',iconCls:'icon-search'">查看</div>
  </div>
    
   <!-- #include file="../js/Vehicle_User.js.inc" -->
 
</body>
</html>
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>

