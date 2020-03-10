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

    public class Api_Vehicle_DriverInfo : IHttpHandler
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
                case "GetDriverInfoDataGrid":
                    GetDriverInfoDataGrid();
                    break;
                case "DriverInfoSearch":
                    DriverInfoSearch();
                    break;
                case "DriverInfoAddSave":
                    DriverInfoAddSave();
                    break;
                case "DriverInfoEditSave":
                    DriverInfoEditSave();
                    break;
                case "DriverInfodelete":
                    DriverInfodelete();
                    break;
                case "GetCarInfo":
                    GetCarInfo();
                    break;
                case "GetCompany":
                    GetCompany();
                    break;
            };
             


        }

        /// <summary>
        /// GetDriverInfoDataGrid
        /// </summary>
        public void GetDriverInfoDataGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string GetDataGrid = Vehicle_DriverInfo.GetDriveInfoDataGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(GetDataGrid);
            Response.End();
        }

        /// <summary>
        /// DriverInfoSearch
        /// </summary>
        public void DriverInfoSearch()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string DriverName = Request.Form["DriverName"];
            string DriverNumber = Request.Form["DriverNumber"];
            string DriverTel = Request.Form["DriverTel"];
            DriveInfoModel Model = new DriveInfoModel();
            Model.Driver_Name = DriverName;
            Model.Driver_Number = DriverNumber;
            Model.Driver_Tel = DriverTel;

            string Json = Vehicle_DriverInfo.DriverInfoSearch(rows, page, Model);
            Response.ContentType = "Application/json";
            Response.Write(Json);
            Response.End();

        }

        /// <summary>
        /// DriverInfoAddSave
        /// </summary>
        public void DriverInfoAddSave()
        {
            DriveInfoModel Model = new DriveInfoModel();
            string LoginUserID = Request["LoginUserID"];
            Model.Driver_Name = Request["Driver_Name"];
            Model.Driver_Number = Request["Driver_Number"];
            Model.Driver_Tel = Request["Driver_Tel"];
            Model.CarInfo = Request["CarInfo"];

            string text = Vehicle_DriverInfo.DriverInfoAddSave(Model, LoginUserID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// DriverInfoEditSave
        /// </summary>
        public void DriverInfoEditSave()
        {

            DriveInfoModel Model = new DriveInfoModel();
            string LoginUserID = Request["LoginUserID"];
            string RID = Request["Data"];
            Model.Driver_Name = Request["Driver_Name"];
            Model.Driver_Number = Request["Driver_Number"];
            Model.Driver_Tel = Request["Driver_Tel"];
            Model.CarInfo = Request["CarInfo"];
            string text = Vehicle_DriverInfo.DriverInfoEditSave(Model, LoginUserID, RID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// DriverInfodelete
        /// </summary>
        public void DriverInfodelete()
        {
            string RidList = Request["Item"];
            string text = Vehicle_DriverInfo.DriverInfodelete(RidList);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// 獲取車輛信息
        /// </summary>
        public void GetCarInfo()
        {
            string CarJson = Vehicle_DriverInfo.GetCarInfo();
            Response.ContentType = "Application/json";
            Response.Write(CarJson);
            Response.End();
        }

        /// <summary>
        /// 獲取公司信息
        /// </summary>
        public void GetCompany()
        {
            string company=Vehicle_DriverInfo.GetCompany();
            Response.ContentType="Application/json";
            Response.Write(company);
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
