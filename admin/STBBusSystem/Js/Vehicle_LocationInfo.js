﻿
$(function(){
        
    Init();//初始化
    
    $("#search_bar").css("height",'0');
//    $("#seachbar").accordion('getSelected').panel('collapse');
    
    //按鍵操作
    $(document).keyup(function(event){
            switch(event.keyCode) {
            case 27:
                $('#dialog').dialog('close')
                break;

        }
    }); 
    
      
    $("#seachbar").accordion({
        onSelect:function(){
            console.log("open")
            var height=window.innerHeight-180;
            $("#DateGrid").datagrid({height:height})
        
        },
        onUnselect:function(){
            console.log("close")
            var height=window.innerHeight-90;
            $("#DateGrid").datagrid({height:height})
            
            
            
        }
    
    
    })
    
    //搜索頁面清除
    $("#clear").bind('click',function(){
        Clear();
    })
    
    //搜索
    $("#search").bind('click',function(){
        Search();
    
    })
    
    //新增
    $('#add').bind('click', function(){
		add();
		
    });  
    
    //编辑
    $('#edit').bind('click', function(){
		edit();
		
    });  
    
    //查看
     $('#view').bind('click', function(){
		view();
		
    });  
    
    //刪除
    $("#delete").bind('click',function(){
        deleteData()
    });
    
    
    //刷新
    $('#refresh').bind('click',function(){
        Refresh();     
    })
        
   
   
})
    //初始化
    function Init(){
        $("#txt_Location").textbox({
            labelAlign:"right",
            width:"200px",
            labelPosition:"before",
        })
        
   
        
        $("#txt_Create_Date").datebox({
            labelAlign:"right",
            width:"200px",
            labelPosition:"before"
        })
        
        $("#txt_Create_Date_T").datebox({
            labelAlign:"right",
            width:"120px",
            labelPosition:"before"
        })
        
        $("#edit_Location").textbox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before"
        })
        
        
        $("#edit_longitude").textbox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before",
        })
        $("#edit_latitude").textbox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before",
        })
        
        $("#edit_Radius").textbox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before",
        })
        
        
        
         $("#edit_CreateDate").datebox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before"
        })
        
         $("#edit_CreateUser").textbox({
            labelAlign:"left",
            width:"100%",
            labelPosition:"before"
        })
        
         GetGridData();//獲取Grid數據

    }

    
    //綁定Grid
    function GetGridData()
    {  
         $("#DateGrid").datagrid({
              title: '詳情',
              url:"../Api/Api_Vehicle_LocationInfo.ashx",
              method:"Post",
              queryParams: {Action: 'GetLocationInfoDataGrid'},
              idField: 'Id',
              rownumbers: 'true',
              fit:false,
              pagination: true,//表示在datagrid设置分页       
              rownumbers: true,
              singleSelect: false,
              striped: true,
              resizeHandle:"right",
              nowrap: true,
              collapsible: true,
              fitColumns: true,
              remoteSort: false,
              pageSize:10,
              height: window.innerHeight-180,
              loadMsg: "正在努力加载数据，请稍后...",
              onLoadSuccess: function (data) {
                if (data.total == 0) {
                }
                  //如果通过调用reload方法重新加载数据有数据时显示出分页导航容器
                else $(this).closest('div.datagrid-wrap').find('div.datagrid-pager').show();
              },
              columns: [[
//                { field: 'ck',
//                 checkbox: true 
//                 },
                { field: 'RID', hidden: 'true' },
                { field: 'locationID',hidden: 'true'},
                { field: 'longitude',hidden: 'true'},
                { field: 'latitude',hidden: 'true'},
                { field: 'name', title: '地点名称', width: '40%', align: 'center', sortable: true },
                { field: 'LastUpdDate', title: '更新日期', width: '30%', align: 'center', sortable: true,formatter: function (date) {
                                    var pa = /.*\((.*)\)/;
                                    var unixtime = date.match(pa)[1].substring(0,10);
                                    return getTime(unixtime);
                                }},
                { field: 'LastUpdUser', title: '更新人', width: '29%', align: 'center', sortable: true },
                { field: 'CreateDate', hidden: 'true' },
                { field: 'CreateUser', hidden: 'true' },
                
              ]],
            //選擇
            onSelect: function (index, row) {
                 var rowsData = $('#DateGrid').datagrid('getSelections');
                 if(rowsData.length>0)
                 {
                    ShowBtn();
                 }
            },
            //不選擇
            onUnselect: function (index, row) {
                var rowsData = $('#DateGrid').datagrid('getSelections');
                 if(rowsData.length==0)
                 {
                    HideBtn();
                 }
            },
            //右鍵菜單
            onRowContextMenu:function(e, rowIndex, rowData) {
                var rowsData = $('#DateGrid').datagrid('getSelections');
                if(rowsData.length>0){
                    $('#DateGrid').datagrid("uncheckAll");
                }
                if(rowData!=null){
                    e.preventDefault();//阻止冒泡
                    $(this).datagrid('selectRow', rowIndex);
                    $('#dgmenu').menu('show', { left: e.pageX, top: e.pageY });
                }
                
            }
         });
      }
      
     
     //搜索按鈕
     function Search(){
        $('#DateGrid').datagrid('load', {
            Action:"LocationInfoSearch",
            Location: $("#txt_Location").textbox('getValue'),
            BeginDate:$("#txt_Create_Date").textbox('getValue'),
            EndDate:$("#txt_Create_Date_T").textbox('getValue'),
        });
        $('#DateGrid').datagrid("uncheckAll");
     }
     
     //清除按鈕
     function Clear(){
        $("#txt_Location").textbox("setValue","")
        $("#txt_Create_Date").textbox("setValue","")
        $("#txt_Create_Date_T").textbox("setValue","")
        
        
     }
     
     //刷新
     function Refresh(){
            $('#DateGrid').datagrid('reload');  
            $('#DateGrid').datagrid("uncheckAll");
            HideBtn();
            
     }
     
     //新增
     function add(){
            var GetDate=getCurrentDate(2);
    //      $('#dialog').dialog('open')
		    $("#edit_CreateUser").textbox('setValue', LoginUserID);
		    $("#edit_CreateDate").textbox('setValue', GetDate);
		    $("#edit_CreateUser").textbox('disable');
		    $("#edit_CreateDate").textbox('disable');
		    ClearTest();
		    enable();
		    showdialog("add",GetDate)
		
		}
	 
	 //编辑
	 function edit(){
	    var rowsData = $('#DateGrid').datagrid('getSelections');
	    if(rowsData.length==1)
	    {
	         var DateTime=GetDateTime(rowsData[0].LastUpdDate);
	         var editRID=rowsData[0].RID;
	         $("#edit_Location").textbox('setValue',rowsData[0].name);
	         $("#edit_CreateUser").textbox('setValue',rowsData[0].LastUpdUser);
	         $("#edit_longitude").textbox('setValue',rowsData[0].longitude);
	         $("#edit_latitude").textbox('setValue',rowsData[0].latitude);
	         $("#edit_Radius").textbox('setValue',rowsData[0].Radius);
		     $("#edit_CreateDate").textbox('setValue',DateTime );
		     enable();
		     showdialog("edit",editRID);
	    }else{
	         $.messager.alert("操作提示","请选择一条数据进行编辑","info");
	    }
	    
	 
	 }
	 
	 //删除
	 function deleteData(){
	    var rowsData = $('#DateGrid').datagrid('getSelections');
	    var Rowcount=rowsData.length;
	    if(Rowcount==0){
	          $.messager.alert("操作提示","请选择至少一条数据进行删除","info");
	    }else{
        $.messager.confirm('Confirm','確定要刪除選中的數據?',function(e){
        if (e){             
	        var ridArr=rowsData[0].RID;
	        for(var i=1;i<Rowcount;i++){
	            ridArr+=","+rowsData[i].RID;
	        }
            $.ajax({
            type: "Post",
            url: "../Api/Api_Vehicle_LocationInfo.ashx",
            data: { "Action":"LocationInfodelete","Item":ridArr },
            dataType: "text",
            success: function (result) {
                var Resultdata = eval('(' + result + ')');  
                if (Resultdata.success) {
                    $.messager.alert("操作提示",Resultdata.message,"info");
                    Refresh();
                    HideBtn();
                 }
            },
            error: function () {
                console.log("Ajax Error!");
            }
         });
	    }
	    });
	   }
    }
	 
	 //查看
	 function view(){
	    var rowsData = $('#DateGrid').datagrid('getSelections');
	    if(rowsData.length==1)
	    {
	         var DateTime=GetDateTime(rowsData[0].LastUpdDate);
	         var editRID=rowsData[0].RID;
	         $("#edit_Location").textbox('setValue',rowsData[0].name);
	         $("#edit_CreateUser").textbox('setValue',rowsData[0].LastUpdUser);
	         $("#edit_longitude").textbox('setValue',rowsData[0].longitude);
	         $("#edit_latitude").textbox('setValue',rowsData[0].latitude);
		     $("#edit_CreateDate").textbox('setValue',DateTime );
		     disable()
		     showdialog("view")

	    }else{
	         $.messager.alert("操作提示","请选择一条数据进行查看","info");
	    }
	   
	 
	 }
		
		
		
	//dialog操作框
	function showdialog(state,data){
	
	    var state_text="";
	    var iconCls="";
	    var buttom_text="";
	    var action="";
	    
	    if(state=="add")
	    {
	        state_text="新增";
	        iconCls="icon-add";
	        buttom_text="保存";
	        action="LocationInfoAddSave";
	        buttons_iconCls="icon-ok";
	        
	    }else if(state=="edit")
	    {
	        state_text="编辑";
	        iconCls="icon-edit";
	        buttom_text="保存";
	        action="LocationInfoEditSave";
	        buttons_iconCls="icon-ok";
	        
	    }else{
	        state_text="查看"
	        iconCls="icon-search";
	        buttom_text="编辑";
	        buttons_iconCls="icon-edit";
	    }
	    
	    $('#dialog').dialog({
        title:state_text,
        width: 350,
        height:380,
        left:"25%",
        right:"15%",
        top:"10%",
        closed: false,
        cache: false,
        zIndex:10000,
        modal: true,
        iconCls: iconCls,
        buttons: [{
            text:buttom_text,
            iconCls:buttons_iconCls,
            handler: function () {
                    if(state=="view"){
                        $('#dialog').dialog('close')
                        edit();
                    }else{
                        Submit(action,data);
                    }
                    
                }
                
        }, 
        {
            text: '取消',
            iconCls: 'icon-cancel',
            handler: function () {
              $('#dialog').dialog('close')
         }
        }]
        });
     }
    
     //表單提交操作
     function Submit(action,data){
         if($("#dlg_From").form('enableValidation').form('validate')) { 
                     $('#dlg_From').form('submit', {
                        url:"../Api/Api_Vehicle_LocationInfo.ashx",
                        onSubmit: function(param){
                            param.Action = action;
                            param.LoginUserID=LoginUserID;
                            param.Data=data
                        },
                        success:function(result){
                           var Resultdata = eval('(' + result + ')');  
                                    if (Resultdata.success) {
                                        $.messager.alert("操作提示",Resultdata.message,"info");
                                        $('#dialog').dialog('close')
                                        $('#DateGrid').datagrid("uncheckAll");
                                        ClearTest();
                                        Refresh();
                                        HideBtn();
                         }
                        },error:function(){
                             $.messager.alert("发生错误")
                        
                      }
              });    
            }
     }

 
    //右键快捷键事件处理
    function gridMenuHandler(item) {
        var row = $('#DateGrid').datagrid('getSelected');
         if (item.name == "add") {
            add()
        }
        if (item.name == "view") {
             view();
        }
        if (item.name == "del") {
             deleteData();
        }
        if (item.name == "edit") {
            edit()
        }
    }

 
 
 
 
 
     
     //enable
   function enable(){
         $("#edit_CreateUser").textbox('disable');
	     $("#edit_CreateDate").textbox('disable');
	     $("#edit_Location").textbox('enable');
	     $("#edit_longitude").textbox('enable');
	     $("#edit_latitude").textbox('enable');
	     
     }
     
     //disable
   function disable(){
         $("#edit_CreateUser").textbox('disable');
	     $("#edit_CreateDate").textbox('disable');
	     $("#edit_Location").textbox('disable');
	     $("#edit_longitude").textbox('disable');
	     $("#edit_latitude").textbox('disable');
     }
     
   //显示按钮
   function ShowBtn(){
     $("#edit").show(100);
     $("#view").show(100);
     $("#delete").show(100);
    
   }
   //隐藏按钮
   function HideBtn(){
     $("#edit").hide(100);
     $("#view").hide(100);
     $("#delete").hide(100);
   
   } 
   
   //清空文本
   function ClearTest(){
        $("#edit_Location").textbox('setValue', "");
        $("#edit_longitude").textbox('setValue', "");
        $("#edit_latitude").textbox('setValue', "");
        $("#edit_latitude").textbox('setValue', "");
        $("#edit_Radius").textbox('setValue', "");
   }
     
   //日期格式化  
   function GetDateTime(date) {
        var pa = /.*\((.*)\)/;
        var unixtime = date.match(pa)[1].substring(0,10);
        return getTime(unixtime);  
     }

   //日期格式化
   function getTime(unixtime) {
        var ts = arguments[0] || 0;
        var t, y, m, d, h, i, s;
        t = ts ? new Date(ts * 1000) : new Date();
        y = t.getFullYear();
        m = t.getMonth() + 1;
        d = t.getDate();
        h = t.getHours();
        i = t.getMinutes();
        s = t.getSeconds();
         
         return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d) + ' ' + (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) + ':' + (s < 10 ? '0' + s : s);
     }
     
     //獲取時間
    function getCurrentDate(format) {
	      var now = new Date();
	      var year = now.getFullYear(); //得到年份
	      var month = now.getMonth();//得到月份
	      var date = now.getDate();//得到日期
	      var day = now.getDay();//得到周几
	      var hour = now.getHours();//得到小时
	      var minu = now.getMinutes();//得到分钟
	      var sec = now.getSeconds();//得到秒
	      month = month + 1;
	      if (month < 10) month = "0" + month;
	      if (date < 10) date = "0" + date;
	      if (hour < 10) hour = "0" + hour;
	      if (minu < 10) minu = "0" + minu;
	      if (sec < 10) sec = "0" + sec;
	      var time = "";
	  //精确到天
	  if(format==1){
		    time = year + "-" + month + "-" + date;
	  }
	  //精确到分
	    else if(format==2){
		    time = year + "-" + month + "-" + date+ " " + hour + ":" + minu + ":" + sec;
	  }
	    return time;
      }
      