// JScript File


$(function(){

checkLoginUserID();
CheckLang("CN")

//加載層
function load() {  
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");  
    $("<div class=\"datagrid-mask-msg\"></div>").html("請稍後....").appendTo("body").css({ display: "block",height:"35px", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });  
}


//取消加载层  
function disLoad() {  
    $(".datagrid-mask").remove();  
    $(".datagrid-mask-msg").remove();  
}

//设置cookie    
function setCookie(name, value, seconds) {    
     seconds = seconds || 0;   //seconds有值就直接赋值，没有为0
     var expires = "";    
     if (seconds != 0 ) {      //设置cookie生存时间    
     var date = new Date();    
     date.setTime(date.getTime()+(seconds*1000));    
     expires = "; expires="+date.toGMTString();    
 }    
    document.cookie = name+"="+escape(value)+expires+"; path=/";   //转码并赋值    
}    

//取得cookie    
function getCookie(name) {    
     var nameEQ = name + "=";    
     var ca = document.cookie.split(';');    //把cookie分割成组    
     for(var i=0;i < ca.length;i++) {    
     var c = ca[i];                      //取得字符串    
     while (c.charAt(0)==' ') {          //判断一下字符串有没有前导空格    
         c = c.substring(1,c.length);      //有的话，从第二位开始取    
     }    
         if (c.indexOf(nameEQ) == 0) {       //如果含有我们要的name    
         return unescape(c.substring(nameEQ.length,c.length));    //解码并截取我们要值    
        }    
     }    
     return false;    
}  

//回車確認
$(document).keypress(function (e) {
        if (e.keyCode == 13)
           Submit();
})


//檢查是否登錄過
function checkLoginUserID(){
    if(LoginUserID!="")
    {
        window.location.href=("./Index.aspx?NameId="+LoginUserID);
    }
}

  

$("#User_Name").textbox({
    iconCls:'icon-man',
    iconAlign:'right',
    height:'35px'
});

$("#User_Password").textbox({
    height:'40px'
});

 
//登錄按鈕
$(".submit").click(function(){
    Submit();
   
});

//登錄
function Submit(){
    var UserName;
    var UserPassWord
    UserName=$("#User_Name").textbox('getValue');
    UserPassWord=$("#User_Password").textbox('getValue');
    
    
    if(UserName!="" && UserPassWord!="")
    {     
         $.ajax({
            type: "Post",
            url: "./Api/Api_Vehicle_Index.ashx",
            data: { "Action": "Authenticate_users","Username": UserName, "Password": UserPassWord, },
            dataType: "text",
            success: function (data) {
               console.log(data);
               if(data=="Success"){
                    load();
                    
                    setCookie("NameID",UserName.toUpperCase(),999999999)
                    window.location.href=("Index.aspx")
                    
               }else if(data=="Fail"){
                    $.messager.alert('Warning','登錄失敗，請檢查密碼是否正確');
               }else{
                    $.messager.alert('Warning','登錄失敗，用戶名不存在');
               }
              
                 
            },
            error: function () {
                console.log("Ajax Error!");
            }
        });
    }
    else
    {
        $.messager.alert('Warning','無法登陸，請檢查有沒輸入帳號密碼');
    }
    
}
    
//function GetText(CNText, ENText) {
//    if (Lang=="CN") {
//        return CNText;
//    }
//    else {
//        return ENText;
//    }
//}

//中英文
function CheckLang(language) {
    if (language == "CN") {
        $(".Title").html("登錄");
        $(".txt_Lang").html("語言");
        $(".txt_UserName").html("用戶名");
        $(".txt_Password").html("密碼");
        $(".submit").html("登錄");
        
        

    }
    else {
        $(".Title").html("Login");
        $(".txt_Lang").html("Lang")
        $(".txt_UserName").html("Name");
        $(".txt_Password").html("Password");
        $(".submit").html("Login");

    }
}























})