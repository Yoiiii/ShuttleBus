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

    public class Api_Vehicle_CarInfo : IHttpHandler
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
                case "GetCarInfoDataGrid":
                    GetCarInfoDataGrid();
                    break;
                case "CarInfoSearch":
                    CarInfoSearch();
                    break;
                case "CarInfoAddSave":
                    CarInfoAddSave();
                    break;
                case "CarInfoEditSave":
                    CarInfoEditSave();
                    break;
                case "CarInfodelete":
                    CarInfodelete();
                    break;
            };



        }


        /// <summary>
        /// GetCarInfoDataGrid
        /// </summary>
        public void GetCarInfoDataGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string GetDataGrid = Vehicle_CarInfo.GetCarInfoDataGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(GetDataGrid);
            Response.End();
        }
        /// <summary>
        /// CarInfoGridSearch
        /// </summary>
        public void CarInfoSearch()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string carName = Request.Form["carName"];
            string seatNum = Request.Form["seatNum"];
            string carID = Request.Form["carID"];
            string BeginDate = Request.Form["BeginDate"];
            string EndData = Request.Form["EndDate"];
            string type = Request.Form["type"];
            CarInfoModel Model = new CarInfoModel();
            Model.carName = carName;
            Model.seatNum = seatNum;
            Model.carID = carID;
            Model.type = type;
            Model.Create_Time = BeginDate;
            Model.Create_Time_E = EndData;
            string Json = Vehicle_CarInfo.CarInfoSearch(rows, page, Model);
            Response.ContentType = "Application/json";
            Response.Write(Json);
            Response.End();

        }
        /// <summary>
        /// CarInfoAddSave
        /// </summary>
        public void CarInfoAddSave()
        {
            CarInfoModel Model = new CarInfoModel();
            string LoginUserID = Request["LoginUserID"];
            Model.carID = Request["carID"];
            Model.Create_Time = Request["Data"];
            Model.carName = Request["carName"];
            Model.type = Request["type"];
            Model.seatNum = Request["seatNum"];
            string text = Vehicle_CarInfo.CarInfoAddSave(Model, LoginUserID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// CarInfoEditSave
        /// </summary>
        public void CarInfoEditSave()
        {

            CarInfoModel Model = new CarInfoModel();
            string LoginUserID = Request["LoginUserID"];
            string RID = Request["Data"];
            Model.carID = Request["carID"];
            Model.type = Request["type"];
            Model.carName = Request["carName"];
            Model.seatNum = Request["seatNum"];
            string text = Vehicle_CarInfo.CarInfoEditSave(Model, LoginUserID, RID);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();
        }

        /// <summary>
        /// CarInfodelete
        /// </summary>
        public void CarInfodelete()
        {
            string RidList = Request["Item"];
            string text = Vehicle_CarInfo.CarInfodelete(RidList);
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
