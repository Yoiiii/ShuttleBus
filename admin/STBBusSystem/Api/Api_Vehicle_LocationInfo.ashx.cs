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

    public class Api_Vehicle_LocationInfo : IHttpHandler
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
                case "GetLocationInfoDataGrid":
                    GetLocationInfoDataGrid();
                    break;
                case "LocationInfoSearch":
                    LocationInfoSearch();
                    break;
                case "LocationInfoAddSave":
                    LocationInfoAddSave();
                    break;
                case "LocationInfoEditSave":
                    LocationInfoEditSave();
                    break;
                case "LocationInfodelete":
                    LocationInfodelete();
                    break;
            };
             


        }

        /// <summary>
        /// GetLocationInfoDataGrid
        /// </summary>
        public void GetLocationInfoDataGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string GetDataGrid = Vehicle_LocationInfo.GetLocationInfoDataGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(GetDataGrid);
            Response.End();
        }

        /// <summary>
        /// LocationInfoSearch
        /// </summary>
        public void LocationInfoSearch()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string Location = Request.Form["Location"];
            string BeginDate = Request.Form["BeginDate"];
            string EndData = Request.Form["EndDate"];
            LocationInfoModel Model = new LocationInfoModel();
            Model.name = Location;
            Model.Create_Time = BeginDate;
            Model.Create_Time_E = EndData;
            string Json = Vehicle_LocationInfo.LocationInfoSearch(rows, page, Model);
            Response.ContentType = "Application/json";
            Response.Write(Json);
            Response.End();

        }

        /// <summary>
        /// LocationInfoAddSave
        /// </summary>
        public void LocationInfoAddSave()
        {
            LocationInfoModel Model = new LocationInfoModel();
            string LoginUserID = Request["LoginUserID"];
            Model.name = Request["Location"];
            Model.longitude = Request.Form["longitude"];
            Model.latitude = Request.Form["latitude"];
            Model.Radius = Request.Form["Radius"];
            Model.Create_Time = Request["Data"];
            string text = Vehicle_LocationInfo.LocationInfoAddSave(Model, LoginUserID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// LocationInfoEditSave
        /// </summary>
        public void LocationInfoEditSave()
        {

            LocationInfoModel Model = new LocationInfoModel();
            string LoginUserID = Request["LoginUserID"]; 
            string RID = Request["Data"];
            Model.name = Request["Location"];
            Model.longitude = Request.Form["longitude"];
            Model.latitude = Request.Form["latitude"];
            Model.Radius = Request.Form["Radius"];
            string text = Vehicle_LocationInfo.LocationInfoEditSave(Model, LoginUserID, RID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// LocationInfodelete
        /// </summary>
        public void LocationInfodelete()
        {
            string RidList = Request["Item"];
            string text = Vehicle_LocationInfo.LocationInfodelete(RidList);
            Response.ContentType = "Application/json";
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
