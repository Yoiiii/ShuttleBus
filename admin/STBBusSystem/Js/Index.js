
$(function() {

    CheckLoginName();
//    ShowFunctionList();
    
    
        
    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    //设置cookie    
    function setCookie(name, value, seconds) {
        seconds = seconds || 0;   //seconds有值就直接赋值，没有为0
        var expires = "";
        if (seconds != 0) {      //设置cookie生存时间    
            var date = new Date();
            date.setTime(date.getTime() + (seconds * 1000));
            expires = "; expires=" + date.toGMTString();
        }
        document.cookie = name + "=" + escape(value) + expires + "; path=/";   //转码并赋值    
    }



    //檢查是否登錄過    
    function CheckLoginName() {
        if (LoginUserID != "") {
            $.ajax({
                type: "Post",
                url: "./Api/Api_Vehicle_Index.ashx",
                data: {"Action":"GetUserInfo","LoginUserID":LoginUserID},
                dataType:"text",
                success:function(result){
                    var Resultdata = eval('(' + result + ')');  
                    $(".User").html(LoginUserID);
//                    $(".dep").html(Resultdata.Dep);
//                    $(".Posi").html(Resultdata.Position); 
                },
                error:function(){
                    console.log("Ajax Error!")
                },
            })
        } else {
            load();
            $(".datagrid-mask-msg").html("沒有登錄，正在退出")
            Sign_out();
        }
    }

    //清除cookie    
    function clearCookie(name) {
        setCookie(name, "", -1);
    }

    //退出
    function Sign_out() {
        window.location.href = "Login.aspx";
    }

    
    $("#Sign_out").click(function() {
        load();
        $(".datagrid-mask-msg").html("正在退出")
        clearCookie("NameID")
        Sign_out();

    })
    
          MenuShow();
    
        $('.Ui-side-tree a').bind("click", function() {
        var title = $(this).text();
        var url = $(this).attr('data-link');
        var iconCls = $(this).attr('iconCls');
        var iframe = $(this).attr('iframe') == 1 ? true : false;
        addTab(title, url, iconCls, true);
    });
    


    
  

  
    
})

  //加載層
    function load() {
        $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
        $("<div class=\"datagrid-mask-msg\"></div>").html("請稍後。。。").appendTo("body").css({ display: "block", height: "35px", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
    }
    
    //取消加载层  
    function disLoad() {
        $(".datagrid-mask").remove();
        $(".datagrid-mask-msg").remove();
    }



//獲取菜單
function MenuShow()
{
    var Car_Mang=$("#Car_Mang");//車連管理模塊
    var Car_Info=$("#Car_Info");//信息管理模塊
    var Statement=$("#Statement");//報表管理模塊
    
     if (LoginUserID != "") {
            $.ajax({
                type: "Post",
                url: "./Api/Api_Vehicle_Index.ashx",
                data: {"Action":"MenuShow","LoginUserID":LoginUserID},
                async:false,
                success:function(result){
                     console.log(result)
                     var Menu = eval('(' + result + ')');  
                     console.log(Menu)
                     Car_Info.append(Menu.Car_Info)
                     Car_Mang.append(Menu.Car_Mang)
                     Statement.append(Menu.Statement) 
//                    $("body [id]").each(function(){  
//                        var ids = $(this).attr("id");  
//                        if( $("body [id="+ids+"]").length >= 2 ){  
//                            $("#"+ids).hide();
//                        }  
//                    });  

//                     if(Menu.Car_Info=="" || Menu.Statement=="")
//                     {
//                        $(".easyui-accordion.accordion.accordion-noborder.easyui-fluid").find(".panel.panel-htop").eq(1).hide();
//                        $(".easyui-accordion.accordion.accordion-noborder.easyui-fluid").find(".panel.panel-htop").eq(2).hide();
//                        $(".panel-body.accordion-body").css("height",1000)
//                     }
                },
                error:function(){
                    console.log("Ajax Error!")
                },
            })
        } else {
            load();
            $(".datagrid-mask-msg").html("沒有登錄，正在退出")
            Sign_out();
        }
        
        
        

}

    /**
    * Name 添加菜单选项
    * Param title 名称
    * Param href 链接
    * Param iconCls 图标样式
    * Param iframe 链接跳转方式（true为iframe，false为href）
    */
    function addTab(title, href, iconCls, iframe) {
        var tabPanel = $('#Ui-tabs');
        if (!tabPanel.tabs('exists', title)) {
            var content = '<iframe scrolling="auto" frameborder="0"  src="' + href + '" style="width:100%;height:100%;"></iframe>';
            if (iframe) {
                tabPanel.tabs('add', {
                    title: title,
                    content: content,
                    iconCls: iconCls,
                    fit: true,
                    cls: 'pd3',
                    closable: true
                });
            }
            else {
                tabPanel.tabs('add', {
                    title: title,
                    href: href,
                    iconCls: iconCls,
                    fit: true,
                    cls: 'pd3',
                    closable: true
                });
            }
        }
        else {
            tabPanel.tabs('select', title);
        }
    }
    /**
    * Name 移除菜单选项
    */
    function removeTab() {
        var tabPanel = $('#Ui-tabs');
        var tab = tabPanel.tabs('getSelected');
        if (tab) {
            var index = tabPanel.tabs('getTabIndex', tab);
            tabPanel.tabs('close', index);
        }
    }