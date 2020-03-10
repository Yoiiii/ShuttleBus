<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 2.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shuttle Bus System</title>
</head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="copyright" content="" />

<link href="<%=ResolveUrl("~/EasyUi/themes/default/easyui.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/icon.css")%>" type="text/css" rel="stylesheet" />
<link href="<%=ResolveUrl("~/css/Ui.css")%>" type="text/css" rel="stylesheet" />
<script src="<%=ResolveUrl("~/EasyUi/jquery-1.7.2.min.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/jquery.easyui.min.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/locale/easyui-lang-zh_CN.js")%>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/EasyUi/datagrid-export.js")%>" type="text/javascript"></script>

   <body class="easyui-layout">
	<!-- begin of header -->
	<div class="Ui-header" data-options="region:'north',border:false,split:true">
    	<div class="Ui-header-left">
        	<h1>穿梭車系統(Shuttle Bus System)</h1>
        </div>
        <div data-options="region:'north',border:false" style="height:70px;">
        <div style="margin-right:15px">
            <div class="User_Contant">
                <div class="sett">
                    <span class="User"></span>
                  <%--  <span class="Posi"></span>
                    <span class="dep"></span>--%>
                </div>
                <div style="clear:both;"></div>
                <div class="Header_Contant">
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-help'" id="Help">幫助</a> 
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" id="Sign_out">退出登錄</a> 
  <%--                  <a href="javascript:void(0)" class="easyui-linkbutton" id="lang">CN</a> --%>
               </div>
          </div>
        </div>
    </div>
    
    </div>
    <!-- end of header -->
    <!-- begin of sidebar -->
	<div class="Ui-sidebar" data-options="region:'west',split:true,border:true,title:'菜單'"> 
    	<div class="easyui-accordion" data-options="border:false,fit:true"> 
    	  <div title="行程管理" data-options="iconCls:'icon-car'">  	
    			<ul id="Car_Mang" class="easyui-tree Ui-side-tree">
    			  <li iconCls="icon-car-add"><a href="javascript:void(0)" iconCls="icon-car-add" data-link="View/View_Vehicle_adminDispatch.aspx" iframe="0">派車登記</a></li>
                </ul>
            </div>
        	<div  title="信息管理" data-options="iconCls:'icon-application-cascade'">  	
    			<ul id="Car_Info" class="easyui-tree Ui-side-tree">
    			     <li iconCls="icon-script-add"><a href="javascript:void(0)" iconCls="icon-script-add" data-link="View/View_Vehicle_User.aspx" iframe="0">用戶信息登記</a></li>
                </ul>
            </div>
           <%-- <div  title="報表管理" data-options="iconCls:'icon-text-padding-top'">  	
    			<ul id="Statement"  class="easyui-tree Ui-side-tree">
                </ul>
            </div>--%>
        </div>
    </div>	
    <!-- end of sidebar -->    
    <!-- begin of main -->
    <div class="Ui-main" data-options="region:'center'">
        <div id="Ui-tabs" class="easyui-tabs" data-options="border:false,fit:true">  
            <div id="Index" title="首頁" data-options="href:'other/Index.html',closable:false,iconCls:'icon-tip',cls:'pd3'"></div>
        </div>
    </div>
    <!-- end of main --> 
    <!-- begin of footer -->
	<div class="Ui-footer" data-options="region:'south',border:true,split:true">
    </div>
    <!-- end of footer -->  
</body>
</html>

<script src="<%=ResolveUrl("~/js/Index.js")%>" type="text/javascript"></script>
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>
