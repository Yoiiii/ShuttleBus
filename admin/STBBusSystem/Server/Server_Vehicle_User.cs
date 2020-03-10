using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using STBBusSystem.DB;
using ViewModel;
using System.Data.Linq;
using Newtonsoft.Json;

namespace STBBusSystem.Server
{

    /// <summary>
    /// Summary description for Vehicle_User
    /// </summary>
    public class Vehicle_User
    {
        public Vehicle_User()
        {

        }

        /// <summary>
        /// GetUserDataGrid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetUserDataGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {

                var list = db.T_UserMaster.Where(t => true).OrderByDescending(t=>t.username).Skip((page - 1) * row).Take(row);
                var total = db.T_UserMaster.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = list });
                return JsonGrid;

            }
        }

        /// <summary>
        /// 獲取用戶類型
        /// </summary>
        /// <returns></returns>
        public static string GetUserType()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_UserMaster
                          select new
                          {
                              value = hdr.type,
                              text = hdr.type
                          };
                return JsonConvert.SerializeObject(rst.Distinct());
            }
        }

      /// <summary>
      /// 搜索
      /// </summary>
      /// <param name="row"></param>
      /// <param name="page"></param>
      /// <param name="Model"></param>
      /// <returns></returns>
        public static string UserSearch(int row, int page, UserModel Model)
        {
            using (var db = new VehicleEntities())
            {
                var Searchlist = from hdr in db.T_UserMaster.Where(p =>
                                     (p.userid.Contains(Model.userid) || string.IsNullOrEmpty(Model.userid))
                                     && (p.username.Contains(Model.username) || string.IsNullOrEmpty(Model.username))
                                      && (p.name.Contains(Model.name) || string.IsNullOrEmpty(Model.name))
                                      && (p.type.Contains(Model.type) || string.IsNullOrEmpty(Model.type))
                                     ).OrderByDescending(p => p.username).Skip((page - 1) * row).Take(row)
                                 select hdr;
                var AllList = from hdr in db.T_UserMaster.Where(p =>
                                    (p.userid.Contains(Model.userid) || string.IsNullOrEmpty(Model.userid))
                                     && (p.username.Contains(Model.username) || string.IsNullOrEmpty(Model.username))
                                      && (p.name.Contains(Model.name) || string.IsNullOrEmpty(Model.name))
                                      && (p.type.Contains(Model.type) || string.IsNullOrEmpty(Model.type))
                                     ).OrderByDescending(p => p.username)
                              select hdr;
                var total = AllList.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = Searchlist });
                return JsonGrid;
            }
        }

        /// <summary>
        /// 新增存儲
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <returns></returns>
        public static string UserAddSave(UserModel Model)
        {
            using (var db = new VehicleEntities())
            {
                T_UserMaster List = new T_UserMaster();
                List.name = Model.name;
                List.userid = Model.userid;
                List.username = Model.username;
                List.type = Model.type;
                List.tel = Model.tel;
                List.text1 = "CN";
                List.RID = Guid.NewGuid().ToString();
                db.AddToT_UserMaster(List);
                int success = db.SaveChanges();
                if (success > 0)
                {
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "保存成功";
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "保存失败" });
                }

            }

        }

        /// <summary>
        /// 更新存儲
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <param name="RID"></param>
        /// <returns></returns>
        public static string UserEditSave(UserModel Model, string RID)
        {
            using (var db = new VehicleEntities())
            {
                var List = db.T_UserMaster.Where(t => t.RID == RID).FirstOrDefault();
                List.name = Model.name;
                List.username = Model.username;
                List.type = Model.type;
                List.userid = Model.userid;
                List.tel = Model.tel;
                int success = db.SaveChanges();
                if (success > 0)
                {
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "更新成功";
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "更新失败" });
                }
            }
        }

        /// <summary>
        /// LocationInfodelete
        /// </summary>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public static string LocationInfodelete(string deleteItem)
        {
            string[] ridArr = deleteItem.Split(',');
            using (var db = new VehicleEntities())
            {
                foreach (var ds in ridArr)
                {
                    string Rid = ds;
                    var model = db.T_Location.FirstOrDefault(p => p.RID == Rid);
                    db.DeleteObject(model);
                }
                int success = db.SaveChanges();
                if (success > 0)
                {
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "刪除成功";
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "刪除失败" });
                }

            }
        }

    }
}