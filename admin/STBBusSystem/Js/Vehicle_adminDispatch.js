

var ApiUrl="../Api/";
var url = ApiUrl+"Api_Vehicle_adminDispatch.ashx?t="+Math.random();
var params = {};
params['Action'] = 'GetdriverInfo';
$.getJSON(url,params, function (data) {
        DriverInfo = data;
    });   
params['Action'] = 'Getstep';
$.getJSON(url,params, function (data) {
        septInfo = data;
    });   
params['Action'] = 'GetpassengerInfo';
$.getJSON(url,params, function (data) {
        PassengerInfo = data;
    });   

$(function(){
   
  
    Init();//初始化
    var bodystate="";
    $("#search_bar").css("height",'0');
//    $("#seachbar").accordion('getSelected').panel('collapse');
    
    //按鍵操作
    $(document).keyup(function(event){
            switch(event.keyCode) {
            case 27:
                $('#dialog').dialog('close')
                $('#ChoseRetdialog').dialog('close')
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
    
    //作廢
    $("#remove").bind('click',function(){
        removeData()
    });
    
    //導入WorkFlow數據
    $("#Import_WorkFlow").bind('click',function(){
        Import_WorkFlow();    
    
    });
    
    //合併wf派車單
    $("#Merge").bind("click",function(){
        Merge();
        
    })
    
    
    //刷新
    $('#refresh').bind('click',function(){
        //window.location.reload();  
        Refresh();
//        $('#DateGrid').datagrid('toExcel','dg.xls')
    })
})


//加載層
    function load() {
        $("<div class=\"datagrid-mask\"></div>").css({ display: "block",zIndex:"20000", width: "100%", height: $(window).height() }).appendTo("body");
        $("<div class=\"datagrid-mask-msg\"></div>").html("請稍後。。。").appendTo("body").css({ display: "block",zIndex:"20000", height: "35px", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
    }
    
    //取消加载层  
    function disLoad() {
        $(".datagrid-mask").remove();
        $(".datagrid-mask-msg").remove();
    }



    //初始化
    function Init(){


        $("#txt_driverID").combobox({
            labelAlign:"right",
            width:"200px",
            labelPosition:"before",
            valueField: 'value',
            textField: 'text',
            panelHeight:"200",
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            queryParams: {Action: 'GetdriverID'},
            method:'Post'
        })

        $("#txt_CreateDate").datebox({
            labelAlign:"right",
            width:"200px",  
            labelPosition:"before"
        })

        $("#txt_status").combobox({
            labelAlign:"right",
            width:"200px",
            labelPosition:"before",
            valueField: 'value',
            textField: 'text',
            panelHeight:'120',
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            queryParams: {Action: 'Getstatus'},
            method:'Post'
        })
        
        $("#txt_carID").textbox({
            labelAlign:"right",
            width:"200px",
            labelPosition:"before"
        })
        
        $("#txt_CreateDate_T").datebox({
            labelAlign:"right",
            width:"120px",  
            labelPosition:"before"
        })

        $("#edit_planID").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
            
        })
        
        $("#edit_status").combobox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
            valueField: 'value',
            textField: 'text',
            panelHeight:'120',
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            queryParams: {Action: 'Getstatus'},
            method:'Post',
        })

        $("#edit_driverID").combobox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
            valueField: 'value',
            textField: 'text',
            panelHeight:'120',
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            queryParams: {Action: 'GetdriverID'},
            method:'Post',
            onChange: function(rst){
                if(DriverInfo!=null){
                    for(var i=0;i<DriverInfo.length;i++){
                        if(DriverInfo[i].Id==rst)
                        {
                            $("#edit_driverTel").textbox('setValue',DriverInfo[i].Tel);
           
                            $("#edit_carID").combogrid('setValue',DriverInfo[i].Carid);
                        }
                    } 
                }
            }
        })

        $("#edit_driverTel").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before"
        })

        $("#edit_carID").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
        })

        $("#edit_go").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
        })

        $("#edit_step").combobox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
            valueField: 'value',
            textField: 'text',
            panelHeight:'120',
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            queryParams: {Action: 'Getstep'},
            method:'Post'
        })

        $("#edit_time").datetimebox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before",
            showSeconds:false,
        })

        $("#edit_CreateDate").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before", 
        })

        $("#edit_CreateUser").textbox({
            labelAlign:"right",
            width:"250px",
            labelPosition:"before", 
        })

        GetGridData();//初始化Grid
        stepGrid();
        PassengerGrid();
        GetMainGrid()
        GetDtlGrid()
    }
    
    $('#txt_carID').combogrid({
        width:'95%',
        panelWidth:"300",
        panelHeight:"180",
        labelPosition:'before',
        method:"Post",
        idField: 'license',
        textField: 'license',
        url:ApiUrl+'Api_Vehicle_DriverInfo.ashx',
        queryParams: {Action: 'GetCarInfo'},
        
        columns:[[
            {field:'carname',title:'汽车品牌',width: '35%', align: 'center'},
            {field:'capacity',title:'可载人数',width: '30%', align: 'center'},
            {field:'license',title:'车牌号',width: '35%', align: 'center'}
        ]],
        onShowPanel:function(){
            $('#txt_carID').combogrid('grid').datagrid('reload');
    　　
        },
        onSelect: function (recordidex) {
                    var record = $("#txt_carID").combogrid("grid").datagrid("getSelected");
                    //setPatient(record);
                },
         keyHandler:{
            up: function() {
            },
            down: function() {
            },
            enter: function() {
                var pClosed = $("#txt_carID").combogrid("panel").panel("options").closed;
                        if (!pClosed) {
                            $("#txt_carID").combogrid("hidePanel");
                        }
                        var record = $("#txt_carID").combogrid("grid").datagrid("getSelected");
                        if (record == null || record == undefined) 
                                return
                        else {
                           // setPatient(record)
                        }
                        
            
            },
            query: function(q) {
                //动态搜索
               $('#txt_carID').combogrid("grid").datagrid("reload", {'license': q});
               $('#txt_carID').combogrid("setValue", q);
            }
        }
    });
    
   //车牌下拉
   $('#edit_carID').combogrid({
    width:'95%',
    panelWidth:"300",
    panelHeight:"180",
    labelPosition:'before',
    method:"Post",
    idField: 'license',
    textField: 'license',
    url:ApiUrl+'Api_Vehicle_DriverInfo.ashx',
    queryParams: {Action: 'GetCarInfo'},
    
    columns:[[
        {field:'carname',title:'汽车品牌',width: '35%', align: 'center'},
        {field:'capacity',title:'可载人数',width: '30%', align: 'center'},
        {field:'license',title:'车牌号',width: '35%', align: 'center'}
    ]],
    onShowPanel:function(){
        $('#edit_carID').combogrid('grid').datagrid('reload');
　　
    },
    onSelect: function (recordidex) {
                var record = $("#edit_carID").combogrid("grid").datagrid("getSelected");
                //setPatient(record);
            },
     keyHandler:{
        up: function() {
        },
        down: function() {
        },
        enter: function() {
            var pClosed = $("#edit_carID").combogrid("panel").panel("options").closed;
                    if (!pClosed) {
                        $("#edit_carID").combogrid("hidePanel");
                    }
                    var record = $("#edit_carID").combogrid("grid").datagrid("getSelected");
                    if (record == null || record == undefined) 
                            return
                    else {
                       // setPatient(record)
                    }
                    
        
        },
        query: function(q) {
            //动态搜索
           $('#edit_carID').combogrid("grid").datagrid("reload", {'license': q});
           $('#edit_carID').combogrid("setValue", q);
        }
    }
});


    //綁定主檔Grid
    function GetGridData()
    {  
        $("#DateGrid").datagrid({
            title: '詳情',
            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            method:"Post",
            queryParams: {Action: 'GetPlanDataGrid'},
            idField: 'planID',
            fit:false,
            pagination: true,//表示在datagrid设置分页       
            rownumbers: true,
            singleSelect: false,
            striped: true,
            resizeHandle:"both",
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
                { field: 'planID', title: '派車單號', width: '13%', align: 'left', sortable: true },
                { field: 'carID', title: '車牌號', width: '9%', align: 'center', sortable: true },
                { field: 'drivername', title: '司機', width: '9%', align: 'center', sortable: true },
                { field: 'driverTel', title: '司機電話', width: '11%', align: 'center', sortable: true },
                // { field: 'descLocation', title: '線路', width: '10%', align: 'center', sortable: true },
                { field: 'locationIDname', title: '出發點', width: '10%', align: 'center', sortable: true },
                { field: 'destinationIDname', title: '終點', width: '10%', align: 'center', sortable: true },
//                { field: 'stepname', title: '當前站點', width: '9%', align: 'center', sortable: true },
                { field: 'statusname', title: '狀態', width: '8%', align: 'center', sortable: true },
                { field: 'time', title: '出發時間', width: '11%', align: 'center', sortable: true,formatter: function (date) {
                                    var pa = /.*\((.*)\)/;
                                    var unixtime = date.match(pa)[1].substring(0,10);
                                    return getTime(unixtime);
                    }
                },
                { field: 'LastUpdDate',  title: '更新日期', width: '10%', align: 'center', sortable: true,formatter: function (date) {
                                    var pa = /.*\((.*)\)/;
                                    var unixtime = date.match(pa)[1].substring(0,10);
                                    return getTime(unixtime);
                                }},
                { field: 'LastUpdUser', title: '更新人', width: '10%', align: 'center', sortable: true },
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

    //站點Grid  
    function stepGrid()
    {  
       
        $("#stepGrid").datagrid({
            title: '途徑站',
            idField: 'Id',
            fit:false,    
            toolbar:'#tb_site',  
            rownumbers: true,
            singleSelect: true,
            striped: true,
            resizeHandle:"right",
            nowrap: true,
            collapsible: true,
            fitColumns: true,
            remoteSort: false,
         
            pageSize:10,
           // height: height,
            onClickRow: steponClickRow,
            columns: [[
                { field: 'RID', hidden: 'true' },
                { field: 'PlanRID', hidden: 'true' },
                { field: 'sequence',  hidden: 'true'  },
                 { field: 'atime',  hidden: 'true'  },
                  { field: 'ltime',  hidden: 'true'  },
                { field: 'locationID', title: '途徑站',width:100, align: 'center',
                    editor:{ 
                        type:'combobox',
							options:{
								valueField:'value',
								textField:'text',
                                url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                                method:"Post",
                                panelHeight:'120',
                                queryParams: {Action: 'Getstep'},
                                required:true,
                                onSelect:function(rst){
                                    debugger;
                                    var index = getstepGridRowIndex(this)
                                    var stepGrid=$('#stepGrid').datagrid("getData").rows;
                                    var step = $("#stepGrid").datagrid('getEditor', { index: index, field: 'locationID'});
                                    var step_name =step.target.textbox("getValue");
                                    console.log(step_name)
 
                                    // if(stepGrid.length>1)
                                    // {
                                    //     for(var i=0;i<stepGrid.length;i++)
                                    //     {   
                                    //         if(rst.value==step_name)
                                    //         {
                                    //             continue;
                                    //         }
                                    //         if(rst.value==stepGrid[i].locationID)
                                    //         {
                                    //             $.messager.alert("操作提示","不能選擇已有的站點","info");
                                    //             $('#stepGrid').datagrid('cancelEdit',index).datagrid('deleteRow', index);
                                    //             break;
                                    //         } 
                                    //    }  
                                    // }
                                }
                            },
                        },
                        formatter: function(value,row,index){
                            for(var i=0;i<septInfo.length;i++){
                                if(septInfo[i].value==value)
                                   return septInfo[i].text
                            }
                            
                        },
                },
            ]]
                　
                
                
         });
        }
//乘客Grid  
function PassengerGrid()
{  
     $("#PassengerGrid").datagrid({
          title: '乘客',
          idField: 'Id',
          fit:false,    
          toolbar:'#tb_desc',  
          rownumbers: true,
          singleSelect: true,
          striped: true,
          resizeHandle:"right",
          nowrap: true,
          collapsible: true,
          fitColumns: true,
          remoteSort: false,
          pageSize:10,
          height: window.innerHeight,
          onClickRow: PassengeronClickRow,
          columns: [[
            { field: 'RID', hidden: 'false' },
            { field: 'PlanRID', hidden: 'true' },
            { field: 'passengerID', title: '乘客', width: '14%', align: 'center',
                editor:{ 
                    type:'combobox',
                    options:{
                        valueField:'value',
                        textField:'text',
                        url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                        method:"Post",
                        panelHeight:'120',
                        queryParams: {Action: 'Getpassenger'},
                        required:true,
                        onSelect:function(rst){
                            debugger;
                            var index = getPassengerGridRowIndex(this)
                            var PassengerGrid=$('#PassengerGrid').datagrid("getData").rows;
                            var Passenger = $("#PassengerGrid").datagrid('getEditor', { index: index, field: 'passengerID'});
                            var Passenger_name =Passenger.target.textbox("getValue");
                            console.log(Passenger_name)
                            debugger;
                            if(PassengerGrid.length>1)
                            {
                                var bit=0;
                                for(var i=0;i<PassengerGrid.length;i++)
                                {   
                                    if(rst.value==Passenger_name)
                                    {
                                        continue;
                                    }
                                    else if(rst.value==PassengerGrid[i].passengerID)
                                    {
                                        $.messager.alert("操作提示","不能選擇已有的乘客","info");
                                        $('#PassengerGrid').datagrid('cancelEdit',index).datagrid('deleteRow', index);
                                        break;
                                    } 
                                    bit++;
                                    if(bit==PassengerGrid.length){
                                        var Passenger_Tel = $("#PassengerGrid").datagrid('getEditor', { index: index, field: 'passengerTel'});
                                        for(var i=0;i<PassengerInfo.length;i++){
                                            if(PassengerInfo[i].userid==rst.value)
                                                Passenger_Tel.target.textbox("setValue", PassengerInfo[i].tel);
                                        }
                                    }                                
                                }
                                  
                            }else
                            {
                                var Passenger_Tel = $("#PassengerGrid").datagrid('getEditor', { index: index, field: 'passengerTel'});
                                for(var i=0;i<PassengerInfo.length;i++){
                                    if(PassengerInfo[i].userid==rst.value)
                                        Passenger_Tel.target.textbox("setValue", PassengerInfo[i].tel);
                                }
                                
                            }

                        }
                    },
                },
                formatter: function(value,row,index){
                debugger;
                    for(var i=0;i<PassengerInfo.length;i++){
                        if(PassengerInfo[i].userid==value)
                         return PassengerInfo[i].name
                    }
                    
                },
            },
            { field: 'passengerTel', title: '電話', width: '14%', align: 'center',
                editor:{ 
                    type:'textbox',
                    options:{
                    },
                }
            },
            { field: 'locationID', title: '上車點', width: '8%', align: 'center', 
                editor:{ 
                    type:'combobox',
                        options:{
                            valueField:'value',
                            textField:'text',
                            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                            method:"Post",
                            panelHeight:'120',
                            queryParams: {Action: 'Getstep'},
                            required:true,
                        },
                    },
                    formatter: function(value,row,index){
                        for(var i=0;i<septInfo.length;i++){
                            if(septInfo[i].value==value)
                                return septInfo[i].text
                        }

                    },
            },
           
            { field: 'destinationID', title: '下車點', width: '8%', align: 'center', 
                editor:{ 
                    type:'combobox',
                        options:{
                            valueField:'value',
                            textField:'text',
                            url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                            method:"Post",
                            panelHeight:'120',
                            queryParams: {Action: 'Getstep'},
                            required:true,

                        },
                    },
                    formatter: function(value,row,index){
                        for(var i=0;i<septInfo.length;i++){
                            if(septInfo[i].value==value)
                                return septInfo[i].text
                        }
                    },
            },
            { field: 'time', title: '預計上車時間', width: '18%', align: 'center', 
          
            editor:{ 
                type:'datetimebox',
                    options:{
                        required:true,
                          showSeconds:false,

                    }
                }
//               formatter: function (date) {
//               
//                                    if(date!=null){
//                                        return Test(date) 
//                                    }
//                                   
//                                   
//                    }
//                        
                
            },




            { field: 'remark', title: '備註', width: '25%', align: 'left' ,
                editor:{ 
                    type:'textbox',
                    options:{
                    },
                }
            },
            { field: 'status', title: '状态', width: '14%', align: 'center',
            editor:{ 
                type:'combobox',
                    options:{
                        valueField:'value',
                        textField:'text',
                        url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                        method:"Post",
                        panelHeight:'120',
                        queryParams: {Action: 'Getpassengerstatus'},
                        required:true,
                    },
                },
            formatter: function(value,row,index){
                    if (value=="1"){
                        return "未上車";
                    } else if(value=="2") {
                        return "已上車";
                    }
                    }
                },
          ]],
     });
}

//選擇乘客主表(M_Plan)
function GetMainGrid(){
    $("#MainGrid").datagrid({
        title:"行程",
        url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
        method:"Post",
        queryParams: {Action: 'GetMainGrid'},
        idField: 'planID',
        fit:false,
        pagination: true,//表示在datagrid设置分页       
        rownumbers: true,
        singleSelect: true,
        striped: true,
        resizeHandle:"both",
        nowrap: true,
        collapsible: true,
        fitColumns: true,
        remoteSort: false,
        pageSize:5,
        height: 250,
        loadMsg: "正在努力加载数据，请稍后...",
        onLoadSuccess: function (data) {
            if (data.total == 0) {
            }
              //如果通过调用reload方法重新加载数据有数据时显示出分页导航容器
            else $(this).closest('div.datagrid-wrap').find('div.datagrid-pager').show();
        },
        columns: [[
            { field: 'RID', hidden: 'true' },
            { field: 'planID', title: '派車單號', width: '15%', align: 'left', sortable: true },
            { field: 'carID', title: '車牌號', width: '15%', align: 'center', sortable: true },
            { field: 'drivername', title: '司機', width: '15%', align: 'center', sortable: true },
            //{ field: 'driverTel', title: '司機電話', width: '11%', align: 'center', sortable: true },
            { field: 'locationIDname', title: '出發點', width: '15%', align: 'center', sortable: true },
            { field: 'destinationIDname', title: '終點', width: '15%', align: 'center', sortable: true },
//                { field: 'stepname', title: '當前站點', width: '9%', align: 'center', sortable: true },
           // { field: 'statusname', title: '狀態', width: '8%', align: 'center', sortable: true },
            { field: 'time', title: '出發時間', width: '18%', align: 'center', sortable: true,formatter: function (date) {
                                var pa = /.*\((.*)\)/;
                                var unixtime = date.match(pa)[1].substring(0,10);
                                return getTime(unixtime);
                }
            },
            { field: 'CreateDate', hidden: 'true' },
            { field: 'CreateUser', hidden: 'true' },
            
        ]],
        //選擇
        onSelect: function (index, row) {
            var rowsData = $('#MainGrid').datagrid('getSelections');
            console.log(row)
            $.ajax({
                type:"Post",
                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                data: { "Action":"GetMainDtlGridData","PlanID":row.planID},
                async: false,
                dataType: "json",
                success: function (result) {
                    console.log(result);
                    $('#MainDtlGrid').datagrid("uncheckAll");
                    $('#MainDtlGrid').datagrid('loadData', { total: 0, rows: [] });  
                    $('#MainDtlGrid').datagrid('loadData',result); 
                },error:function(){
                    console.log("ajax Error!")
                }
            })

        },
    });
}
    
//選擇乘客Dtl表(D_PassergerList)
function GetDtlGrid(){
    $("#MainDtlGrid").datagrid({
        title:"乘客",
        idField: 'RID',
        fit:false,
        pagination: false,//表示在datagrid设置分页       
        rownumbers: true,
        singleSelect: false,
        striped: true,
        resizeHandle:"both",
        nowrap: true,
        collapsible: true,
        fitColumns: true,
        remoteSort: false,   
        pageSize:10,
        height:250,
       // onClickRow: PassengeronClickRow,
        columns: [[
        { field: 'RID', hidden: 'false' },
        { field: 'PlanRID', hidden: 'true' },
        { field: 'passengerIDtest', title: '乘客', width: '15%', align: 'center' },
        { field: 'passengerTel', title: '電話', width: '15%', align: 'center'},
        { field: 'returnTime', title: '返程時間', width: '15%', align: 'center'},
        
        //{ field: 'locationIDtest', title: '上車點', width: '20%', align: 'center' },
        //{ field: 'destinationIDtest', title: '下車點', width: '20%', align: 'center'},
       // { field: 'time', title: '預計上車時間', width: '18%', align: 'center' },
        { field: 'remark', title: '備註', width: '55%', align: 'left'   },
        //{ field: 'status', title: '状态', width: '14%', align: 'center'},
    
    ]],
   });
}
    
function formatDate(dt) {
    var year = dt.getFullYear();
    var month = dt.getMonth() + 1;
    var date = dt.getDate();
    var hour = dt.getHours();
    var minute = dt.getMinutes();
    var second = dt.getSeconds();
    return year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
}
function Test(time) {
    if(time.indexOf("/Date")!=-1)
    var t= time.slice(6, 19)
    var NewDtime = new Date(parseInt(t));
    return formatDate(NewDtime);
}

    //合併派車單
    function Merge(){
        var rowsData = $('#DateGrid').datagrid('getSelections');
        if(rowsData.length>1){
            var length=rowsData.length;
            var carID=0;
            var timeID=0;
            var car="";
            var time="";
            for(var i=0;i<rowsData.length;i++){
                car=rowsData[i].carID;
                if(rowsData[i+1]!=null){
                    if(rowsData[i].carID==rowsData[i+1].carID)
                        carID=carID+1;
                }else{
                    if(rowsData[i].carID==car)
                        carID=carID+1;
                }
            }
            for(var i=0;i<rowsData.length;i++){
                time=rowsData[i].time;
                if(rowsData[i+1]!=null){
                    if(rowsData[i].time==rowsData[i+1].time)
                        timeID=timeID+1;
                }else{
                    if(rowsData[i].time==time)
                        timeID=timeID+1;
                }
            } 

            if(length==carID){
                if(length==timeID)
                    $.ajax({
                        type:"Post",
                        url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                        data: { "Action":"MergePlan","MergeList": JSON.stringify(rowsData)},
                        success: function (rst) {
                            $.messager.alert("成功","合併成功","info");
                            $('#DateGrid').datagrid("uncheckAll");
                            ClearTest();
                            Refresh();
                            HideBtn();
                            
                        },
                        error:function(){
                            console.log("ajax error!")
                        }
                    })
                else
                $.messager.alert("無法合併","所選單號存在不同出發時間","info");
            }else{
                $.messager.alert("無法合併","所選單號存在不同車牌號","info");
            }
        }else{
            $.messager.alert("提示","合併需要選擇兩條派車單","info");
        }
 
    }
     
     //搜索按鈕
     function Search(){
        $('#DateGrid').datagrid('load', {
            Action:"PlanSearch",
            driverID: $("#txt_driverID").combobox('getValue'),
            carID: $("#txt_carID").combobox('getValue'),
            status: $("#txt_status").combobox('getValue'),
            BeginDate:$("#txt_CreateDate").datebox('getValue'),
            EndDate:$("#txt_CreateDate_T").datebox('getValue'),      
        });
        $('#DateGrid').datagrid("uncheckAll");
     }
     
     //清除按鈕
     function Clear(){
        $("#txt_driverID").textbox("setValue","")
        $("#txt_carID").textbox("setValue","")
        $("#txt_status").combobox("setValue","")
        $("#txt_CreateDate").datebox("setValue","")
        $("#txt_CreateDate_T").datebox("setValue","")
        
     }
     
     //刷新
     function Refresh(){
            $('#DateGrid').datagrid('reload');  
            $('#DateGrid').datagrid("uncheckAll");
     
            HideBtn();
            
     }
     
     //導入WorkFlow數據
     function Import_WorkFlow(){
        
         $.ajax({
            type:"Post",
            url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
            data: { "Action":"GetWorkFlow_Import" },
            async: false,
            dataType: "json",
            success: function (result) {
                if(result!=null){
                   $.messager.alert("WorkFlow派車數據","已導入"+result[0].Column1+"條","info");
                    Refresh();
                    HideBtn();
                }
            
            },error:function(){
                console.log("Ajax.Error!")
            
            }
         
         })
     
     
     }
     
     //新增
     function add(){
        $('#DateGrid').datagrid("uncheckAll");
        ClearTest();
        var GetDate=getCurrentDate(3);
        var ApplyNo=""; 
        bodystate="add";   
        $("#tb_site").show()    
        $("#tb_desc").show()   
        
        
        showdialog("add")
        var height=$("#from_table").height();
        $("#stepGrid").datagrid({height:height})
        enable();
        $('#stepGrid').datagrid('loadData', { total: 0, rows: [] }); 
        $('#PassengerGrid').datagrid('loadData', { total: 0, rows: [] });  

        $("#edit_planID").textbox('setValue', "自動獲取");
        $("#edit_CreateUser").textbox('setValue', LoginUserID);
        $("#edit_CreateDate").textbox('setValue', GetDate);
        $("#edit_status").combobox('setValue', "0");
        $("#edit_step").textbox('setValue', "自動獲取");
        

		
    }
	 //编辑
	 function edit(){
        var rowsData = $('#DateGrid').datagrid('getSelections');
        if(rowsData.length==1)
        {
            var rows = $('#DateGrid').datagrid('getSelections');
            var count=rows.length;  
            showdialog("edit");
            ClearTest();
            enable();
            var height=$("#from_table").height();
             $("#stepGrid").datagrid({height:height})
            
            $("#edit_status").textbox('readonly',false);
            $("#edit_status").textbox('textbox').css('background-color','white');
            
            
            $("#tb_site").show()    
            $("#tb_desc").show()    
            bodystate="edit";
            
            $('#stepGrid').datagrid('loadData', { total: 0, rows: [] });
            $('#PassengerGrid').datagrid('loadData', { total: 0, rows: [] });

            var CreateDate=GetDateTime(rowsData[0].CreateDate);
            var time=GetDateTime(rowsData[0].time);

            $("#edit_planID").textbox('setValue',rowsData[0].planID)
        
            $("#edit_status").combobox('setValue',rowsData[0].status)
            $("#edit_driverID").combobox('setValue',rowsData[0].driverID)
            $("#edit_carID").textbox('setValue',rowsData[0].carID)
            $("#edit_driverTel").textbox('setValue',rowsData[0].driverTel)
            if(rowsData[0].step!=0)
                $("#edit_step").combobox('setValue',rowsData[0].step)
            else
                $("#edit_step").combobox('setValue',"")
                
            $("#edit_time").textbox('setValue',time)
            $("#edit_CreateDate").textbox('setValue',CreateDate)
            $("#edit_CreateUser").textbox('setValue',rowsData[0].CreateUser);

            $.ajax({
                type: "Post",
                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                data: { "Action":"GetstepGrid","planID":rowsData[0].planID },
                dataType: "json",
                success: function (result) {
                debugger;
                    console.log(result);
                        $('#stepGrid').datagrid('loadData',result); 
                        var start="";
                        var end="";
                        var stepGrid=$("#stepGrid").datagrid("getData").rows;
                        var len=stepGrid.length-1;
                        for(var i=0;i<stepGrid.length;i++){
                            if(stepGrid[i].sequence==0){
                                for(var j=0;j<septInfo.length;j++){
                                    if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                                        start=septInfo[j].text;
                                    }
                                }
                            }
                            if(stepGrid[i].sequence==len){
                                for(var j=0;j<septInfo.length;j++){
                                    if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                                        end=septInfo[j].text;
                                    }
                                }
                            }
                        }
                        $("#edit_go").textbox("setValue",start+">"+end)
                },
                
                error: function () {
                    console.log("Ajax Error!");
                }
            });


            $.ajax({
                type: "Post",
                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                data: { "Action":"GetPassengerGrid","planID":rowsData[0].planID },
                dataType: "json",
                success: function (result) {
                        console.log(result);
                        debugger;
                        for(var i=0;i<result.length;i++){
                            for(var j=0;j<PassengerInfo.length;j++){
                                if(result[i].passengerID==PassengerInfo[j].userid){
                                    result[i].passengerTel=PassengerInfo[j].tel;
                                }
                             
                            }
                            var date=result[i].time
                            var pa = /.*\((.*)\)/;
                            var unixtime = date.match(pa)[1].substring(0,10);
                            result[i].time= getTime(unixtime);
                        }
                        $('#PassengerGrid').datagrid('loadData',result); 
                       
                },
                error: function () {
                    console.log("Ajax Error!");
                }
            });
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
                var rows = $('#DateGrid').datagrid('getSelections');
                var count=rows.length;  
                var ridArr=rows[0].planID;
                    for(var i=1;i<count;i++){
                        ridArr+=","+rows[i].RID;
                }
                
                $.ajax({
                    type:"Post",
                    url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                    data: { "Action":"CheckPlanDelete","Item":ridArr },
                    dataType:"text",
                    success:function(result){
                        debugger;
                        if(result=="success"){
                            $.ajax({
                                type: "Post",
                                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                                data: { "Action":"Plandelete","Item":ridArr },
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
                        }else{
                        
                            $.messager.alert("操作提示","無法刪除選擇的單","info");
                        }
                    },error:function(){
                        console.log("Ajax Error!")
                    }
                
                });
            }
	    });
	   }
    }
    
  	 function removeData(){
        var rowsData = $('#DateGrid').datagrid('getSelections');
        debugger;
        var Rowcount=rowsData.length;  
	    if(Rowcount==0){
	          $.messager.alert("操作提示","请选择至少一条数据进行作廢","info");
	    }else{
        $.messager.confirm('Confirm','確定要作廢選中的數據?',function(e){
        if (e){        
               var rows = $('#DateGrid').datagrid('getSelections');
                var count=rows.length;  
                var ridArr=rows[0].planID;
                for(var i=1;i<count;i++){
                        ridArr+=","+rows[i].RID;
                }
                $.ajax({
                    type:"Post",
                    url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                    data: { "Action":"CheckPlanDelete","Item":ridArr },
                    dataType:"text",
                    success:function(result){
                        debugger;
                        if(result=="success"){
                            $.ajax({
                                type: "Post",
                                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                                data: { "Action":"Planremove","Item":ridArr },
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
                        }else{
                        
                            $.messager.alert("操作提示","無法刪除選擇的單","info");
                        }
                    },error:function(){
                        console.log("Ajax Error!")
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
            showdialog("view")
            disable();
            ClearTest();
           
            var height=$("#from_table").height();
            $("#stepGrid").datagrid({height:height})
            $("#tb_desc").hide()
            $("#tb_site").hide()
            bodystate="view";
            $('#stepGrid').datagrid('loadData', { total: 0, rows: [] });
            $('#PassengerGrid').datagrid('loadData', { total: 0, rows: [] });

            var CreateDate=GetDateTime(rowsData[0].CreateDate);
            var time=GetDateTime(rowsData[0].time);

            $("#edit_planID").textbox('setValue',rowsData[0].planID)
            if(rowsData[0].step!=0)
                $("#edit_step").combobox('setValue',rowsData[0].step)
            else
                $("#edit_step").combobox('setValue',"")
      
                
            $("#edit_status").combobox('setValue',rowsData[0].status)
            $("#edit_driverID").combobox('setValue',rowsData[0].driverID)
            $("#edit_carID").textbox('setValue',rowsData[0].carID)
            $("#edit_driverTel").textbox('setValue',rowsData[0].driverTel)
//            $("#edit_step").combobox('setValue',rowsData[0].step)
            $("#edit_time").textbox('setValue',time)
            $("#edit_CreateDate").textbox('setValue',CreateDate)
            $("#edit_CreateUser").textbox('setValue',rowsData[0].CreateUser);

            $.ajax({
                type: "Post",
                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                data: { "Action":"GetstepGrid","planID":rowsData[0].planID },
                dataType: "json",
                success: function (result) {
                    console.log(result);
                        $('#stepGrid').datagrid('loadData',result); 
                        var start="";
                        var end="";
                        var stepGrid=$("#stepGrid").datagrid("getData").rows;
                        var len=stepGrid.length-1;
                        for(var i=0;i<stepGrid.length;i++){
                            if(stepGrid[i].sequence==0){
                                for(var j=0;j<septInfo.length;j++){
                                    if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                                        start=septInfo[j].text;
                                    }
                                }
                            }
                            if(stepGrid[i].sequence==len){
                                for(var j=0;j<septInfo.length;j++){
                                    if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                                        end=septInfo[j].text;
                                    }
                                }
                            }
                        }
                        $("#edit_go").textbox("setValue",start+">"+end)
                },
                
                error: function () {
                    console.log("Ajax Error!");
                }
            });


            $.ajax({
                type: "Post",
                url: ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                data: { "Action":"GetPassengerGrid","planID":rowsData[0].planID },
                dataType: "json",
                success: function (result) {
                    console.log(result);
                         for(var i=0;i<result.length;i++){
                            for(var j=0;j<PassengerInfo.length;j++){
                                if(result[i].passengerID==PassengerInfo[j].userid){
                                    result[i].passengerTel=PassengerInfo[j].tel;
                                }
                            }
                            var date=result[i].time
                            var pa = /.*\((.*)\)/;
                            var unixtime = date.match(pa)[1].substring(0,10);
                            result[i].time= getTime(unixtime);
                        }
                         $('#PassengerGrid').datagrid('loadData',result); 
                
                },
                error: function () {
                    console.log("Ajax Error!");
                }
            });

	    }else{
	         $.messager.alert("操作提示","请选择一条数据进行查看","info");
	    }
	   
	 
	 }
		
		
		
	//dialog操作框
	function showdialog(state){
	
	    var state_text="";
	    var iconCls="";
	    var buttom_text="";
        var action="";
        var editstate="";
	    
	    if(state=="add")
	    {
	        state_text="新增";
	        iconCls="icon-add";
	        buttom_text="提交";
	        action="PlanAddSave";
	        buttons_iconCls="icon-ok";
	        
	    }else if(state=="edit")
	    {
	        state_text="编辑";
	        iconCls="icon-edit";
	        buttom_text="提交";
	        action="PlanEditSave";
	        buttons_iconCls="icon-ok";
	        
	    }else{
	        state_text="查看"
	        iconCls="icon-search";
	        buttom_text="编辑";
	        buttons_iconCls="icon-edit";
	    }
	    
	    $('#dialog').dialog({
        title:state_text,
        width: window.innerWidth-150,
        height:window.innerHeight-20,
        left:"5%",
        right:"40%",
        top:"2%",
        closed: false,
        cache: false,
        zIndex:10000,
        modal: true,
        iconCls: iconCls,
        buttons: [
        {
            text:buttom_text,
            iconCls:buttons_iconCls,
            handler: function () {
                    stepconfirm();
                    Passengerconfirm();
                    editstate="submit";
                    if(state=="view"){
                        $('#dialog').dialog('close')
                        edit();
                    }else{
                        load() ; 
                        Submit(action,editstate);
                         
                    }
                    
                }
                
        }, 
        {
            text: '取消',
            iconCls: 'icon-cancel',
            handler: function () {
              $('#dialog').dialog('close')
         }
        }
        ]

        
        });
     }
    
     //表單提交操作
     function Submit(action,editstate){
         debugger;
        if(stependEditing() && PassengerendEditing() ){
         
        var PassengerGrid=$('#PassengerGrid').datagrid("getData").rows; 
        var stepGrid=$("#stepGrid").datagrid("getData").rows;
        if(stepGrid.length>0){
            for(var i=0;i<stepGrid.length;i++){
                stepGrid[i].sequence=i;
            }
        }
        
        
        
        if(PassengerGrid.length<1 || stepGrid.length<1)
        {
              disLoad(); 
            $.messager.alert("操作提示","途徑站點表,乘客表不能為空，請檢查","info")
        }else
        {
            var Is_setp=checkSetp(PassengerGrid,stepGrid);
            if(Is_setp==true){
            
                debugger;
             
        
            var data = $('#dlg_From').serializeArray();
                var obj = {};
                $.each(data,function(i,v){
                    obj[v.name] = v.value;
                }) ;

            var FormDataJson=JSON.stringify(obj) 
            var PassengerDataJson=JSON.stringify(PassengerGrid)  
            var stepDataJson=JSON.stringify(stepGrid)  

                    if($("#dlg_From").form('enableValidation').form('validate')) {          
                    $('#dlg_From').form('submit', {
                        url:ApiUrl+"Api_Vehicle_adminDispatch.ashx",
                        onSubmit: function(param){
                            param.Action = action;
                            param.LoginUserID=LoginUserID;
                            param.PassengerData=PassengerDataJson;
                            param.stepData=stepDataJson;
                            param.FormData=FormDataJson;
                            param.editstate=editstate;
                        },
                        success:function(result){
                            console.log(result)
                            disLoad();
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
                              disLoad();
                            $.messager.alert("操作提示","存儲更新发生错误","info")
                        
                         }
                    });      
                }else{
                     disLoad();
                      $.messager.alert("操作提示","請輸入必輸項","info");
                }
            } else{
                 disLoad();
                 $.messager.alert("操作提示","請檢查乘客上下車站點與途徑站點是否匹配","info");
                
            }
        }
        }
    }
    
    //檢查乘客站點與站點資料是否匹配
    function checkSetp(Passengersetp,step){
            var PassengersetpLenght=Passengersetp.length;
            var locationLength=0;
            var destinationLength=0;
            
            for(var i=0;i<Passengersetp.length;i++){
                 for(var j=0;j<step.length;j++){
                    var passengerLocationID=Passengersetp[i].locationID.toString();
                    var stepLocationID=step[j].locationID.toString();
                    if(passengerLocationID==stepLocationID){
                            locationLength++
                            break;
                        }
                 }
            }
            
            for(var i=0;i<Passengersetp.length;i++){
                 for(var j=0;j<step.length;j++){
                    var destinationLocationID=Passengersetp[i].destinationID.toString();
                    var stepLocationID=step[j].locationID.toString();
                    if(destinationLocationID==stepLocationID){
                            destinationLength++
                            break;
                        }
                 }
            }
            console.log(locationLength)
            console.log(destinationLength)
            if(locationLength==PassengersetpLenght && destinationLength==PassengersetpLenght)
                return true;
            else
                return false;
        
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
        if (item.name == "remove") {
            removeData();
        }        
        if (item.name == "edit") {
            edit()
        }
    }

        //siteGrid编辑事件
        var stepeditIndex = null;
        function stependEditing() {
            if (stepeditIndex == null) { return true }
            if ($('#stepGrid').datagrid('validateRow', stepeditIndex)) {
                $('#stepGrid').datagrid('endEdit', stepeditIndex);
                stepeditIndex = null;
                return true;
            } else {
                $.messager.alert("操作提示","請輸入必輸項在操作","info");
                return false;
            }
        }
        // stepGrid行点击事件
        function steponClickRow(index, row) {
            if(bodystate!="view")
            {
            //     var rows = $('#DateGrid').datagrid('getSelections');
            //     if(rows.length>0){
            //     if(rows[0].status==0 ){
            //     debugger;
            //    //if (stepeditIndex != index) {
            //         if (stependEditing()) {
            //             $("#stepGrid").datagrid("selectRow", index).datagrid("beginEdit", index);
            //             stepeditIndex = index;
            //         } else {
            //             $("#stepGrid").datagrid("selectRow", stepeditIndex);
            //         }
            //     }else{
            //         $.messager.alert("操作提示","當前單正在進行中無法修改站點","info");
                    
            //         }
                // }else{
                    if (stependEditing()) {
                        $("#stepGrid").datagrid("selectRow", index).datagrid("beginEdit", index);
                        stepeditIndex = index;
                    } else {
                        $("#stepGrid").datagrid("selectRow", stepeditIndex);
                    }
                // }
      
            }
        }

        //stepGrid新增一行
		function stepappend(){
		var rows = $('#DateGrid').datagrid('getSelections');
        // if(rows.length>0){
        // if(rows[0].status==0 ){
		// 	if (stependEditing()){
        //         stepeditIndex = $("#stepGrid").datagrid("getRows").length;
        //         var sequence=stepeditIndex;
		// 		$('#stepGrid').datagrid('appendRow',{
        //             RID:guid(),
        //             PlanRID:"",
        //             planID:"",
        //             sequence:sequence,
        //         });
        //         $("#stepGrid").datagrid("selectRow", stepeditIndex).datagrid("beginEdit", stepeditIndex);
        //     }
        //     }else{
        //          $.messager.alert("操作提示","當前單正在進行中無法新增站點","info");
        //     }
        // }else{
            if (stependEditing()){
                stepeditIndex = $("#stepGrid").datagrid("getRows").length;
                var sequence=stepeditIndex;
				$('#stepGrid').datagrid('appendRow',{
                    RID:guid(),
                    PlanRID:"",
                    planID:"",
                    sequence:sequence,
                });
                $("#stepGrid").datagrid("selectRow", stepeditIndex).datagrid("beginEdit", stepeditIndex);
            }
        // }
           
    }

        //stepGrid移除一行
		function stepremoveit(){
        var rows = $('#DateGrid').datagrid('getSelections');
        // if(rows.length>0){
        // if(rows[0].status==0){
		// 	if (stepeditIndex == null) { return }
        //     $('#stepGrid').datagrid('cancelEdit', stepeditIndex).datagrid('deleteRow', stepeditIndex);
        //     stepeditIndex = null;
        //     }else{
        //         $.messager.alert("操作提示","當前單正在進行中無法刪除站點","info");
        //     }
        // }else{
            if (stepeditIndex == null) { return }
            $('#stepGrid').datagrid('cancelEdit', stepeditIndex).datagrid('deleteRow', stepeditIndex);
            stepeditIndex = null;
        // }
    }
        
        //stepGrid确定按钮
        function stepconfirm(){
            var rows = $('#stepGrid').datagrid('getSelected');
            var index = $('#stepGrid').datagrid('getRowIndex',rows)
            $('#stepGrid').datagrid('endEdit', index);
            var start="";
            var end="";
            var stepGrid=$("#stepGrid").datagrid("getData").rows;
            var len=stepGrid.length-1;
            for(var i=0;i<stepGrid.length;i++){
                if(stepGrid[i].sequence==0){
                    for(var j=0;j<septInfo.length;j++){
                        if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                            start=septInfo[j].text;
                        }
                    }
                }
                if(stepGrid[i].sequence==len){
                    for(var j=0;j<septInfo.length;j++){
                        if(parseInt(stepGrid[i].locationID)==septInfo[j].value){
                            end=septInfo[j].text;
                        }
                    }
                }
            }
            $("#edit_go").textbox("setValue",start+">"+end)

        }

        //获取stepGrid行号
        function getstepGridRowIndex(target)
        {
            var rows = $('#stepGrid').datagrid('getSelected');
            var index = $('#stepGrid').datagrid('getRowIndex',rows)
            return index;
        }



        //PassengerGrid编辑事件
        var PassengereditIndex = null;
        function PassengerendEditing() {
            if (PassengereditIndex == null) { return true }
            if ($('#PassengerGrid').datagrid('validateRow', PassengereditIndex)) {
                $('#PassengerGrid').datagrid('endEdit', PassengereditIndex);
                PassengereditIndex = null;
                return true;
            } else {
                $.messager.alert("操作提示","請輸入乘客表必輸項","info");
                return false;
            }
        }
        // PassengerGrid行点击事件
        function PassengeronClickRow(index, row) {
            if(bodystate!="view")
            {
            //if (PassengereditIndex != index) {
                    if (PassengerendEditing()) {
                        $("#PassengerGrid").datagrid("selectRow", index).datagrid("beginEdit", index);
                        PassengereditIndex = index;
                    } else {
                        $("#PassengerGrid").datagrid("selectRow", PassengereditIndex);
                    }
                //}
            }
        }

        //PassengerGrid新增一行
        function Passengerappend(){
            if (PassengerendEditing()){
            PassengereditIndex = $("#PassengerGrid").datagrid("getRows").length;
                var sequence=stepeditIndex+1;
                $('#PassengerGrid').datagrid('appendRow',{
                    RID:guid(),
                    PlanRID:"",
                    status:"1",
                     
                });
                $("#PassengerGrid").datagrid("selectRow", PassengereditIndex).datagrid("beginEdit", PassengereditIndex);
            }
        }

        //PassengerGrid移除一行
        function Passengerremoveit(){
            if (PassengereditIndex == null) { return }
            $('#PassengerGrid').datagrid('cancelEdit', PassengereditIndex).datagrid('deleteRow', PassengereditIndex);
            PassengereditIndex = null;
        }
        
        //PassengerGrid确定按钮
        function Passengerconfirm(){
            var rows = $('#PassengerGrid').datagrid('getSelected');
            var index = $('#PassengerGrid').datagrid('getRowIndex',rows)
            $('#PassengerGrid').datagrid('endEdit', index);
        }

        //获取PassengerGrid行号
        function getPassengerGridRowIndex(target)
        {
            var rows = $('#PassengerGrid').datagrid('getSelected');
            var index = $('#PassengerGrid').datagrid('getRowIndex',rows)
            return index;
        }

        //選擇回程乘客
        function ChoseRetPassenger(){
            if(PassengerendEditing()){
            $('#ChoseRetdialog').dialog({
                title:"選擇回程乘客",
                width: 800,
                height:600,
                left:"10%",
                right:"40%",
                top:"2%",
                closed: false,
                cache: false,
                zIndex:10000,
                modal: true,
                //iconCls: 
                buttons: [
                {
                    text:"插入",
                    iconCls:"icon-add",
                    handler: function () {
                        var selectRow=$("#MainDtlGrid").datagrid("getSelections")
                        console.log(selectRow)
                        var PassengerGrid=$("#PassengerGrid").datagrid("getData").rows;
                        var ReturnTime=getCurrentDate(1)+" "+"17:15"

                        if(selectRow.length>1){

                            var IsSame=false;
                            for(var i=0;i<PassengerGrid.length;i++){
                                for(var j=0;j<selectRow.length;j++){
                                    if(PassengerGrid[i].passengerID==selectRow[j].passengerID){
                                        IsSame=true;
                                        $.messager.alert("操作提示","不能插入乘客表已有的乘客","info");
                                        return;
                                    }
                                }
                            }
                            if(IsSame==false){


                            for(var i=0;i<selectRow.length;i++){
                                $('#PassengerGrid').datagrid('appendRow',{
                                    RID:guid(),
                                    PlanRID:"",
                                    status:"1",
                                    destinationID:"5",
                                    time:ReturnTime,
                                    passengerID:selectRow[i].passengerID,
                                    passengerTel:selectRow[i].passengerTel,
                                    remark:selectRow[i].remark,
                                });

                            }
                            $('#MainDtlGrid').datagrid("uncheckAll");
                        }


                        }else{
                            var IsSame=false;
                            for(var i=0;i<PassengerGrid.length;i++){
                                if(PassengerGrid[i].passengerID==selectRow[0].passengerID){
                                    IsSame=true;
                                    $.messager.alert("操作提示","不能插入乘客表已有的乘客","info");
                                    break;
                                }
                            }

                            if(IsSame==false){
                                $('#PassengerGrid').datagrid('appendRow',{
                                    RID:guid(),
                                    PlanRID:"",
                                    status:"1",
                                    destinationID:"5",
                                    time:ReturnTime,
                                    passengerID:selectRow[0].passengerID,
                                    passengerTel:selectRow[0].passengerTel,
                                    remark:selectRow[0].remark,
                                });
                                $('#MainDtlGrid').datagrid("uncheckAll");
                            }
                        }
                  
                        




                    }
                        
                }, 
                {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                      $('#ChoseRetdialog').dialog('close')
                 }
                }
                ]
                });

                $('#MainGrid').datagrid('reload');  
                $('#MainGrid').datagrid("uncheckAll");
                $('#MainDtlGrid').datagrid("uncheckAll");
                $('#MainDtlGrid').datagrid('loadData', { total: 0, rows: [] });  

            } 
        }




        
        //设置Grid值
        function setValueToEditor(editor, value) {
            switch (editor.type) {
                case "combobox":
                    editor.target.combobox("setValue", value);
                    break;
                case "combotree":
                    editor.target.combotree("setValue", value);
                    break;
                case "textbox":
                    editor.target.textbox("setValue", value);
                    break;
                case "numberbox":
                    editor.target.numberbox("setValue", value);
                    break;
                case "datebox":
                    editor.target.datebox("setValue", value);
                    break;
                case "datetimebox":
                    editor.target.datebox("setValue", value);
                    break;
                default:
                    editor.html = value;
                    break;
            }
        }
    

     //enable
   function enable(){
        
        $("#edit_driverID").combobox('readonly',false);
        $("#edit_driverTel").textbox('readonly',false);
        $("#edit_carID").combobox('readonly',false);
        $("#edit_time").textbox('readonly',false);
        $("#edit_driverID").combobox('textbox').css('background-color',"white");
        $("#edit_driverTel").textbox('textbox').css('background-color','white');
        $("#edit_carID").combobox('textbox').css('background-color','white');
        $("#edit_time").textbox('textbox').css('background-color','white');


        $("#edit_CreateUser").textbox('readonly',true);
        $("#edit_CreateDate").textbox('readonly',true);
        $("#edit_planID").textbox('readonly',true);
        $("#edit_go").textbox('readonly',true);
        $("#edit_status").combobox('readonly',true);
        $("#edit_step").textbox('readonly',true);
      
        $("#edit_CreateUser").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_CreateDate").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_planID").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_go").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_status").combobox('textbox').css('background-color','#EBEEEF');
        $("#edit_step").textbox('textbox').css('background-color','#EBEEEF');
     
     }
     
     //disable
   function disable(){

        $("#edit_driverID").combobox('readonly',true);
        $("#edit_driverTel").textbox('readonly',true);
        $("#edit_carID").combobox('readonly',true);
        $("#edit_time").textbox('readonly',true);
        $("#edit_driverID").combobox('textbox').css('background-color',"#EBEEEF");
        $("#edit_driverTel").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_carID").combobox('textbox').css('background-color','#EBEEEF');
        $("#edit_time").textbox('textbox').css('background-color','#EBEEEF');

        $("#edit_CreateUser").textbox('readonly',true);
        $("#edit_CreateDate").textbox('readonly',true);
        $("#edit_planID").textbox('readonly',true);
        $("#edit_go").textbox('readonly',true);
        $("#edit_status").combobox('readonly',true);
        $("#edit_step").textbox('readonly',true);
     
        $("#edit_CreateUser").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_CreateDate").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_planID").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_go").textbox('textbox').css('background-color','#EBEEEF');
        $("#edit_status").combobox('textbox').css('background-color','#EBEEEF');
        $("#edit_step").textbox('textbox').css('background-color','#EBEEEF');
 
    }
     
   //显示按钮
   function ShowBtn(){
     $("#edit").show(100);
     $("#view").show(100);
     $("#remove").show(100);
    
   }
   //隐藏按钮
   function HideBtn(){
     $("#edit").hide(100);
     $("#view").hide(100);
     $("#remove").hide(100);
   
   } 
   
   //清空文本
   function ClearTest(){
       $("#dlg_From").form("clear");
   }
     
   //日期格式化  
   function GetDateTime(date) {
        var pa = /.*\((.*)\)/;
        var unixtime = date.match(pa)[1].substring(0,10);
        return getTime(unixtime);  
     }

   //生成GUID
   function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
        return v.toString(16);
        });
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
         
         return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d) + ' ' + (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) ;
        //return  (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) ;
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
      }else if(format==3)
            time = year + "-" + month + "-" + date+ " " + hour + ":" + minu ;
	    return time;
      }
   