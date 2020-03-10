<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Login</title>
<script type="text/javascript" src="EasyUi/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="EasyUi/jquery.easyui.min.js"></script>
<link rel="stylesheet" href="EasyUi/themes/default/easyui.css" type="text/css" />
<link rel="stylesheet" href="css/icon.css" type="text/css"/>
<script type="text/javascript" src="EasyUi/locale/easyui-lang-zh_CN.js"></script>
<script type="text/javascript" src="EasyUi/easyloader.js"></script>

</head>

 

<body>
	<div id="Login_Info">
	<form id="Login_Form">
	     <h1 class="Title"></h1>
	     <div class="middle">
	         <span class="txt_UserName"></span><br />
	         <input id="User_Name" class="easyui-textbox"  prompt='Username' /><br />
		     <span class="txt_Password"></span><br />
		     <input id="User_Password" class="easyui-passwordbox" prompt="Password" iconWidth="28" style="height:35px;" /><br />
		     <%--<span class="txt_Lang">--%></span><br /> 
		 </div>
         <%--<select id="lang" class="easyui-combobox" data-options="panelHeight:'auto'" name="lang" >
         <option value="CN">CN</option>
         <option value="EN">EN</option>
        </select>--%>
		 <a href="javacript:void(0);"  class="submit"></a>
	</form>
    </div>

	
</body>
</html>

<script type="text/javascript" src="js/Login.js?v=201904241405"></script>
<link rel="stylesheet" href="css/Login.css" type="text/css" />
<script type="text/javascript" defer="defer">
    var LoginUserID = "<%=LoginUserID%>";
    var Lang = "<%=Lang%>";
  </script>



