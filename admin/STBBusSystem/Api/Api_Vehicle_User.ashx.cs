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

    public class Api_Vehicle_User : IHttpHandler
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
                case "GetUserDataGrid":
                    GetUserDataGrid();
                    break;
                case "UserSearch":
                    UserSearch();
                    break;
                case "UserAddSave":
                    UserAddSave();
                    break;
                case "UserEditSave":
                    UserEditSave();
                    break;
                case "GetUserType":
                    GetUserType();
                    break;
            };
             


        }

        /// <summary>
        /// 獲取主檔
        /// </summary>
        public void GetUserDataGrid()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string GetDataGrid = Vehicle_User.GetUserDataGrid(rows, page);
            Response.ContentType = "Application/json";
            Response.Write(GetDataGrid);
            Response.End();
        }

        /// <summary>
        /// 獲取用戶類型
        /// </summary>
        public void GetUserType()
        {
            result = Vehicle_User.GetUserType();
            Response.ContentType = "Application/json";
            Response.Write(result);
            Response.End();
            
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void UserSearch()
        {
            rows = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            page = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            string username = Request.Form["username"];
            string userid = Request.Form["userid"];
            string name = Request.Form["name"];
            string type = Request.Form["type"];
            UserModel Model = new UserModel();
            Model.username = username;
            Model.userid = userid;
            Model.name = name;
            Model.type = type;
            string Json = Vehicle_User.UserSearch(rows, page, Model);
            Response.ContentType = "Application/json";
            Response.Write(Json);
            Response.End();

        }

        /// <summary>
        /// 新增存儲
        /// </summary>
        public void UserAddSave()
        {
            UserModel Model = new UserModel();
            Model.userid = Request.Form["userid"];
            Model.username = Request.Form["username"];
            Model.name = Request.Form["name"];
            Model.tel = Request.Form["tel"];
            Model.type = Request.Form["type"];
            string text = Vehicle_User.UserAddSave(Model);
            Response.ContentType = "Application/json";
            Response.Write(text);
            Response.End();

        }

        /// <summary>
        /// 更新存儲
        /// </summary>
        public void UserEditSave()
        {

            UserModel Model = new UserModel();
            string RID = Request["Data"];
            Model.userid = Request.Form["userid"];
            Model.username = Request.Form["username"];
            Model.name = Request.Form["name"];
            Model.tel = Request.Form["tel"];
            Model.type = Request.Form["type"];
            string text = Vehicle_User.UserEditSave(Model, RID);
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
