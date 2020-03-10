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
using STBBusSystem.Server;
using System.Collections.Generic;
using ViewModel;

namespace STBBusSystem.Api
{

    public class Api_Vehicle_adminDispatch : IHttpHandler
    {
        protected HttpContext _ctx;
        private String Action = "";
        private String result = "";
        private HttpRequest Request;
        private HttpResponse Response;
        private HttpSessionState Session;
        private string SearchAction;
        private int page;
        private int rows;

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
                case "GetPlanDataGrid":
                    GetPlanDataGrid();
                    break;
                case "PlanSearch":
                    PlanSearch();
                    break;
                case "PlanAddSave":
                    PlanAddSave();
                    break;
                case "PlanEditSave":
                    PlanEditSave();
                    break;
                case "Plandelete":
                    Plandelete();
                    break;
                case "Planremove":
                    Planremove(); 
                    break;                
                case "Getstep":
                    Getstep();
                    break;
                case "GetdriverID":
                    GetdriverID();
                    break;
                case "GetdriverInfo":
                    GetdriverInfo();
                    break;
                case "Getpassenger":
                    Getpassenger();
                    break;
                case "GetpassengerInfo":
                    GetpassengerInfo();
                    break;
                case "Getstatus":
                    Getstatus();
                    break;
                case "Getpassengerstatus":
                    Getpassengerstatus();
                    break;
                case "GetstepGrid":
                    GetstepGrid();
                    break;
                case "GetPassengerGrid":
                    GetPassengerGrid();
                    break;
                case "GetWorkFlow_Import":
                    GetWorkFlow_Import();
                    break;
                case "MergePlan":
                    MergePlan();
                    break;
                case "GetMainDtlGridData":
                    GetMainDtlGridData();
                    break;
                case "GetMainGrid":
                    GetMainGrid();
                    break;
                case "CheckPlanDelete":
                    CheckPlanDelete();
                    break;
            };



        } 

        /// <summary>
        /// 獲取主檔
        /// </summary>
        public void GetPlanDataGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            result = Vehicle_adminDispatch.GetPlanDataGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取選擇乘客主檔
        /// </summary>
        public void GetMainGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            result = Vehicle_adminDispatch.GetMainGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }


        /// <summary>
        /// 獲取站點
        /// </summary>
        public void Getstep()
        {
            result = Vehicle_adminDispatch.Getstep();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取司機
        /// </summary>
        public void GetdriverID()
        {
            result = Vehicle_adminDispatch.GetdriverID();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取司機數據
        /// </summary>
        public void GetdriverInfo()
        {
            result = Vehicle_adminDispatch.GetdriverInfo();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取乘客
        /// </summary>
        public void Getpassenger()
        {
            result = Vehicle_adminDispatch.Getpassenger();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取乘客數據
        /// </summary>
        public void GetpassengerInfo()
        {
            result = Vehicle_adminDispatch.GetpassengerInfo();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取Plan司機狀態
        /// </summary>
        public void Getstatus()
        {
            result = Vehicle_adminDispatch.Getstatus();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取乘客狀態
        /// </summary>
        public void Getpassengerstatus()
        {
            result = Vehicle_adminDispatch.Getpassengerstatus();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取站點表
        /// </summary>
        public void GetstepGrid()
        {
            string planID = Request["planID"];
            result = Vehicle_adminDispatch.GetstepGrid(planID);
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 獲取乘客表
        /// </summary>
        public void GetPassengerGrid()
        {
            string planID = Request["planID"];
            result = Vehicle_adminDispatch.GetPassengerGrid(planID);
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 主檔搜索
        /// </summary>
        public void PlanSearch()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string driverID = Request.Form["driverID"];
            string carID = Request.Form["carID"];
            string status = Request.Form["status"];
            string BeginDate = Request.Form["BeginDate"];
            string EndData = Request.Form["EndDate"];
            PlanSearchModel Model = new PlanSearchModel();
            Model.carID = carID;
            Model.driverID = driverID;
            Model.status =status;
            Model.BeginDate = BeginDate;
            Model.EndData = EndData;
            string Json = Vehicle_adminDispatch.PlanSearch(rows, page, Model);
            Response.ContentType = "Application/json";
            Response.Write(Json);
            Response.End();

        }
        /// <summary>
        /// 新增存儲
        /// </summary>
        public void PlanAddSave()
        {
            string LoginUserID = Request["LoginUserID"];
            var PlanJson = Request["FormData"];
            var PassengerJson = Request["PassengerData"];
            var stepJson = Request["stepData"];
            var PlanModel = JsonConvert.DeserializeObject<PlanModel>(PlanJson);
            var PassengerModel = JsonConvert.DeserializeObject<List<PassengerModel>>(PassengerJson);
            var stepModel = JsonConvert.DeserializeObject<List<stepModel>>(stepJson);
            string text = Vehicle_adminDispatch.PlanAddSave(PlanModel, PassengerModel, stepModel, LoginUserID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// 更新存儲
        /// </summary>
        public void PlanEditSave()
        {
            string LoginUserID = Request["LoginUserID"];
            var PlanJson = Request["FormData"];
            var PassengerJson = Request["PassengerData"];
            var stepJson = Request["stepData"];
            var PlanModel = JsonConvert.DeserializeObject<PlanModel>(PlanJson);
            var PassengerModel = JsonConvert.DeserializeObject<List<PassengerModel>>(PassengerJson);
            var stepModel = JsonConvert.DeserializeObject<List<stepModel>>(stepJson);
            string text = Vehicle_adminDispatch.PlanEditSave(PlanModel, PassengerModel, stepModel, LoginUserID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// 刪除存儲
        /// </summary>
        public void Plandelete()
        {
            string RidList = Request["Item"];
            string text = Vehicle_adminDispatch.Plandelete(RidList);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// 刪除存儲
        /// </summary>
        public void Planremove()
        {
            string RidList = Request["Item"];
            string text = Vehicle_adminDispatch.Planremove(RidList);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// WorkFlow數據導入
        /// </summary>
        public void GetWorkFlow_Import()
        {
            result = Vehicle_adminDispatch.GetWorkFlow_Import();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 合併wf派車單
        /// </summary>
        public void MergePlan()
        {
            var List = Request["MergeList"];
            var MergeList = JsonConvert.DeserializeObject<List<MergePlanModel>>(List);
            result = Vehicle_adminDispatch.MergePlan(MergeList);
            Response.ContentType = "Application/text";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 返回乘客列表
        /// </summary>
        public void GetMainDtlGridData()
        {
            string PlanID = Request["PlanID"];
            result = Vehicle_adminDispatch.GetMainDtlGridData(PlanID);
            Response.ContentType = "Application/text";
            Response.Write(result);
            Response.End();
        }




        /// <summary>
        /// 檢查刪除單是否處於未開始狀態
        /// </summary>
        public void CheckPlanDelete()
        {
            string RidList = Request["Item"];
            string text = Vehicle_adminDispatch.CheckPlanDelete(RidList);
            Response.ContentType = "Application/text";
            Response.Write(text);
            Response.End();
        }















        /// <summary>
        /// GetModelFromRequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestName"></param>
        /// <returns></returns>
        protected T GetModelFromRequest<T>(string requestName)
        {
            return this.GetModelsFromRequest<T>(requestName);
        }
        /// <summary>
        /// 从request form中获取 'models'参数中的值并转为某类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetModelsFromRequest<T>(string requestName)
        {
            //if (this._ctx.Request.Form == null
            //    || this._ctx.Request.Form[requestName] == null)
            //    return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(requestName);
            }
            catch
            {
                return default(T);
            }
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
