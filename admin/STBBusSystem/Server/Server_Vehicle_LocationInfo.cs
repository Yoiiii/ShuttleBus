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
    /// Summary description for Vehicle_LocationInfo
    /// </summary>
    public class Vehicle_LocationInfo
    {
        public Vehicle_LocationInfo()
        {

        }

        /// <summary>
        /// GetLocationInfoDataGrid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetLocationInfoDataGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {

                var list = db.T_Location.Where(t => true).OrderByDescending(t => t.LastUpdDate).Skip((page - 1) * row).Take(row);
                var total = db.T_Location.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = list });
                return JsonGrid;

            }
        }

      /// <summary>
      /// LocationInfoSearch
      /// </summary>
      /// <param name="row"></param>
      /// <param name="page"></param>
      /// <param name="Model"></param>
      /// <returns></returns>
        public static string LocationInfoSearch(int row, int page, LocationInfoModel Model)
        {
            DateTime? Begin_Date;
            DateTime? End_Date;
            if (Model.Create_Time != "")
            {
                Begin_Date = Convert.ToDateTime(Model.Create_Time);
            }
            else
            {

                Begin_Date = null;
            }
            if (Model.Create_Time_E != "")
            {
                End_Date = Convert.ToDateTime(Model.Create_Time_E);
            }
            else
            {

                End_Date = null;
            }

            using (var db = new VehicleEntities())
            {
                var Searchlist = from hdr in db.T_Location.Where(p =>
                                        (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                     && (p.LastUpdDate <= End_Date || End_Date == null)
                                     && (p.name.Contains(Model.name) || string.IsNullOrEmpty(Model.name))
                                     ).OrderByDescending(p => p.LastUpdDate).Skip((page - 1) * row).Take(row)
                                 select hdr;
                var AllList = from hdr in db.T_Location.Where(p =>
                                        (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                     && (p.LastUpdDate <= End_Date || End_Date == null)
                                     && (p.name.Contains(Model.name) || string.IsNullOrEmpty(Model.name))
                                     ).OrderByDescending(p => p.LastUpdDate)
                              select hdr;
                var total = AllList.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = Searchlist });
                return JsonGrid;
            }
        }

        /// <summary>
        /// LocationInfoAddSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <returns></returns>
        public static string LocationInfoAddSave(LocationInfoModel Model, string LoginUserID)
        {
            using (var db = new VehicleEntities())
            {
                int LocationIDMax =Convert.ToInt32( db.T_Location.Select(t => t.locationID).Max());
                int LocationID = LocationIDMax + 1;
                T_Location List = new T_Location();
                List.CreateUser = LoginUserID;
                List.CreateDate = Convert.ToDateTime(Model.Create_Time);
                List.LastUpdDate = Convert.ToDateTime(Model.Create_Time);
                List.LastUpdUser = LoginUserID;
                List.name = Model.name;
                List.longitude = Model.longitude;
                List.latitude = Model.latitude;
                if (Model.Radius != "")
                    List.Radius = Convert.ToInt32(Model.Radius);
                List.locationID =  LocationID.ToString();
                List.RID = Guid.NewGuid().ToString();
                db.AddToT_Location(List);
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
        /// LocationInfoEditSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <param name="RID"></param>
        /// <returns></returns>
        public static string LocationInfoEditSave(LocationInfoModel Model, string LoginUserID, string RID)
        {
            using (var db = new VehicleEntities())
            {
                var List = db.T_Location.Where(t => t.RID == RID).FirstOrDefault();
                List.LastUpdDate = DateTime.Now;
                List.LastUpdUser = LoginUserID;
                List.name = Model.name;
                List.longitude = Model.longitude;
                List.latitude = Model.latitude;
                if (Model.Radius != "")
                    List.Radius = Convert.ToInt32(Model.Radius);
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