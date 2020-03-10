using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.Linq;
using System.Data.SqlClient;
using STBBusSystem.DB;
using System.Collections.Generic;
using ViewModel;

namespace STBBusSystem.Api
{

    public class Api_Vehicle_Index : IHttpHandler
    {
        protected HttpContext _ctx;
        private String Action = "";
        private String result = "";
        private HttpRequest Request;
        private HttpResponse Response;
        private HttpSessionState Session;
        private string SearchAction;

        private string VS01001 = "<li><div id='_easyui_tree_1' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-script-add'></span><span class='tree-title'><a href='javascript:void(0)'  iconCls='icon-script-add' data-link='View/View_Vehicle_CarInfo.aspx' iframe='0'>車輛信息登記</a></span></div>";
        private string VS01002 = "<li><div id='_easyui_tree_2' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-script-add'></span><span class='tree-title'><a href='javascript:void(0)'  iconCls='icon-script-add' data-link='View/View_Vehicle_DriverInfo.aspx' iframe='0'>司機信息登記</a></span></div>";
        private string VS01003 = "<li><div id='_easyui_tree_3' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-script-add'></span><span class='tree-title'><a href='javascript:void(0)'  iconCls='icon-script-add' data-link='View/View_Vehicle_LocationInfo.aspx' iframe='0'>常用地點登記</a></span></div>";
        //private string VS02001 = "<li><div id='_easyui_tree_4' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-car-add'></span><span class='tree-title'><a href='javascript:void(0)' iconCls='icon-car-add' data-link='View/View_Vehicle_CarApplication.aspx' iframe='0'>派車申請</a></span></div>";
        //private string VS02002 = "<li><div id='_easyui_tree_5' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-user-business-boss'></span><span class='tree-title'><a href='javascript:void(0)' iconCls='icon-user-business-boss'  data-link='View/View_Vehicle_CarApproval.aspx' iframe='0'>批覈管理</a></span></div>";
        //private string VS02003 = "<li><div id='_easyui_tree_6' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-car-red'></span><span class='tree-title'><a href='javascript:void(0)' iconCls='icon-car-red' data-link='View/View_Vehicle_CarScheduling.aspx' iframe='0'>用車調度</a></span></div>";
        //private string VS02004 = "<li><div id='_easyui_tree_7' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-user-edit'></span><span class='tree-title'><a href='javascript:void(0)' iconCls='icon-user-edit' data-link='View/View_Vehicle_CarFeedback.aspx' iframe='0'>问题建議反馈</a></span></div>";
        //private string VS02005 = "<li><div id='_easyui_tree_8' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-money-add'></span><span class='tree-title'><a href='javascript:void(0)' iconCls='icon-money-add' data-link='View/View_Vehicle_CarCost.aspx' iframe='0'>行駛費用登記</a></span></div>";
        //private string VS03001 = "<li><div id='_easyui_tree_9' class='tree-node'><span class='tree-indent'></span><span class='tree-icon tree-file icon-script-add'></span><span class='tree-title'><a href='javascript:void(0)'  iconCls='icon-script-add' data-link='View/View_Vehicle_Report_CarApply.aspx' iframe='0'>申請虧總</a></li>";
        private string VS02001 = "";
        private string VS02002 = "";
        private string VS02003 = "";
        private string VS02004 = "";
        private string VS02005 = "";
        private string VS03001 = "";

        /// <summary>
        /// 菜單Model
        /// </summary>
        public class Menu
        {
            public string Car_Mang { set; get; }
            public string Car_Info { set; get; }
            public string Statement { set; get; }
        }


        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";

            Request = context.Request;
            Response = context.Response;
            Session = context.Session;
            context.Response.ContentType = "text/plain";
            Action = context.Request["Action"];

         
            switch (Action)
            {
                case "Authenticate_users":
                    Authenticate_users();
                    break;
                case "GetUserInfo":
                    GetUserInfo();
                    break;
                case "MenuShow":
                    MenuShow();
                    break;
            };



        }

        /// <summary>
        /// 驗證登錄
        /// </summary>
        public void Authenticate_users()
        {
            string Username = Request["Username"];
            string Password = Request["Password"];
            using (var db = new SSM2_PRODEntities())
            {
                var Select = db.M_User.Where(t=>t.UserID==Username).FirstOrDefault();
                if (Select!=null && Select.Password==Password)
                {
                    Select.CancelDate = DateTime.Now;
                    db.SaveChanges();
                    result = "Success";
                    Response.ContentType = "Application/json";
                    Response.Write(result);
                    Response.End();
                }
                else if (Select != null && Select.Password != Password)
                {
                    result = "Fail";
                    Response.ContentType = "Application/json";
                    Response.Write(result);
                    Response.End();
                }
                else
                {
                    result = "Error";
                    Response.ContentType = "Application/json";
                    Response.Write(result);
                    Response.End();
                }
            }
           
        }
        /// <summary>
        /// 獲取用戶信息
        /// </summary>
        public void GetUserInfo()
        {
            string LoginUserID = Request["LoginUserID"];
            using (var db = new VehicleEntities())
            {
                var List = db.user_master.Where(t => t.Uname== LoginUserID).FirstOrDefault();
                var json = JsonConvert.SerializeObject(List);
                Response.ContentType = "Application/text";
                Response.Write(json);
                Response.End();
            }
        }

        ///// <summary>
        ///// 功能權限顯示
        ///// </summary>
        //public void GetShowFunctionList()
        //{
        //    string LoginUserID = Request["LoginUserID"];
        //    using (var db = new VehicleEntities())
        //    {
        //        var rst = from hdr in db.Permissions.Where(p => p.LoginID == LoginUserID)
        //                  select new { FunctionTree = hdr.FunctionTreeID, FunctionListName = hdr.FunctionNameID, IsShow = hdr.IsShow };
        //        result = JsonConvert.SerializeObject(rst);
        //        Response.ContentType = "Application/text";
        //        Response.Write(result);
        //        Response.End();
        //    }
        //}

        /// <summary>
        /// 獲取菜單
        /// </summary>
        public void MenuShow()
        {
           
            string LoginUserID = Request["LoginUserID"];
            Menu result = new Menu();
            string Car_Mang = "";
            string Car_Info = "";
            string Statement = "";
            using (var db = new SSM2_PRODEntities())
            {
                var LoginRole = db.M_UserRole.Where(t => t.UserID == LoginUserID);
                var SystemFunc = db.M_SystemFunc.Where(t => t.SystemCode == "VEHICLESYSTEM");
                var SystemFuncAccessRights=db.M_SystemFuncAccessRights.Where(t=>t.SystemCode=="VEHICLESYSTEM");
                var SystemRole = db.M_SystemFuncAccessRights.Where(t => t.SystemCode == "VEHICLESYSTEM").Select(t => t.RoleID).Distinct();
                foreach (var RoleID in SystemRole)
                {
                    foreach (var lr in LoginRole.Where(t => t.RoleID == RoleID))
                    {
                        foreach (var Func in SystemFunc.Where(t => t.FuncTagID == "VS01"))
                        {
                            foreach (var FuncAccessRights in SystemFuncAccessRights.Where(t => t.FuncCode == Func.FuncCode && t.RoleID == RoleID))
                            {

                                if (FuncAccessRights.FuncCode == "VS01001")
                                {
                                    Car_Info = Car_Info + VS01001;
                                }
                                if (FuncAccessRights.FuncCode == "VS01002")
                                {
                                    Car_Info = Car_Info + VS01002;
                                }
                                if (FuncAccessRights.FuncCode == "VS01003")
                                {
                                    Car_Info = Car_Info + VS01003;
                                }
                            }
                        }
                        foreach (var Func in SystemFunc.Where(t => t.FuncTagID == "VS02"))
                        {
                            foreach (var FuncAccessRights in SystemFuncAccessRights.Where(t => t.FuncCode == Func.FuncCode && t.RoleID == RoleID))
                            {
                                if (FuncAccessRights.FuncCode == "VS02001")
                                {
                                    Car_Mang = Car_Mang + VS02001;
                                }
                                if (FuncAccessRights.FuncCode == "VS02002")
                                {
                                    Car_Mang = Car_Mang + VS02002;
                                }
                                if (FuncAccessRights.FuncCode == "VS02003")
                                {
                                    Car_Mang = Car_Mang + VS02003;
                                }
                                if (FuncAccessRights.FuncCode == "VS02004")
                                {
                                    Car_Mang = Car_Mang + VS02004;
                                }
                                if (FuncAccessRights.FuncCode == "VS02005")
                                {
                                    Car_Mang = Car_Mang + VS02005;
                                }
                            }
                        }
                        foreach (var Func in SystemFunc.Where(t => t.FuncTagID == "VS03"))
                        {
                            foreach (var FuncAccessRights in SystemFuncAccessRights.Where(t => t.FuncCode == Func.FuncCode && t.RoleID == RoleID))
                            {
                                if (FuncAccessRights.FuncCode == "VS03001")
                                {
                                    Statement = Statement + VS03001;
                                }

                            }
                        }

                    }
                }
                
                result.Car_Mang = Car_Mang;
                result.Car_Info = Car_Info;
                result.Statement = Statement;
            }

            Response.ContentType = "Application/text";
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }



     








        












        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}
