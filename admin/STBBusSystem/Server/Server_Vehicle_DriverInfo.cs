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
    /// Summary description for Vehicle_DriverInfo
    /// </summary>
    public class Vehicle_DriverInfo
    {
        public Vehicle_DriverInfo()
        {

        }

        /// <summary>
        /// GetDriveInfoDataGrid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetDriveInfoDataGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {

                var list = db.T_UserMaster.Where(t => t.type == "driver").OrderByDescending(t => t.username).Skip((page - 1) * row).Take(row);
                var total = db.T_UserMaster.Where(t => t.type == "driver").Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = list });
                return JsonGrid;

            }
        }

       /// <summary>
        /// DriverInfoSearch
       /// </summary>
       /// <param name="row"></param>
       /// <param name="page"></param>
       /// <param name="Model"></param>
       /// <returns></returns>
        public static string DriverInfoSearch(int row, int page, DriveInfoModel Model)
        {
           

            using (var db = new VehicleEntities())
            {
                var Searchlist = from hdr in db.T_UserMaster.Where(p =>
                                      (p.username.Contains( Model.Driver_Name) || string.IsNullOrEmpty(Model.Driver_Name))
                                     && (p.userid.Contains(Model.Driver_Number) || string.IsNullOrEmpty(Model.Driver_Number))
                                     && (p.tel.Contains(Model.Driver_Tel) || string.IsNullOrEmpty(Model.Driver_Tel))
                                     && p.type == "driver"
                                     ).OrderBy(p => p.name).Skip((page - 1) * row).Take(row)
                                 select hdr;
                var AllList = from hdr in db.T_UserMaster.Where(p =>
                                      (p.username.Contains( Model.Driver_Name) || string.IsNullOrEmpty(Model.Driver_Name))
                                     && (p.userid.Contains(Model.Driver_Number) || string.IsNullOrEmpty(Model.Driver_Number))
                                     && (p.tel.Contains( Model.Driver_Tel) || string.IsNullOrEmpty(Model.Driver_Tel))
                                     && p.type == "driver"
                                     ).OrderBy(p => p.name)
                              select hdr;
                var total = AllList.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = Searchlist });
                return JsonGrid;
            }
        }

        /// <summary>
        /// DriverInfoAddSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <returns></returns>
        public static string DriverInfoAddSave(DriveInfoModel Model, string LoginUserID)
        {
            using (var db = new VehicleEntities())
            {
                string CarInfoRID="";
                //if (Model.CarInfo != "")
                //{

                //    CarInfoRID = db.Car.Where(p => p.Car_License == Model.CarInfo).FirstOrDefault().RID;
                //}
                //DateTime Dt = Convert.ToDateTime(Model.Create_Time);
                //DriverInfo List = new DriverInfo();
                //List.Update_Time = Dt;
                //List.Update_User = LoginUserID;
                //List.Create_User = LoginUserID;
                //List.Create_Time = Dt;
                //List.Driver_Name = Model.Driver_Name;
                //List.Driver_Number = Model.Driver_Number;
                //List.Driver_Company = Model.Driver_Company;
                //List.Driver_Tel = Model.Driver_Tel;
                //List.RID = Guid.NewGuid().ToString();
                //List.CarInfoRID = CarInfoRID;
                //db.AddToDriverInfo(List);
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
        /// DriverInfoEditSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <param name="RID"></param>
        /// <returns></returns>
        public static string DriverInfoEditSave(DriveInfoModel Model, string LoginUserID, string RID)
        {
            using (var db = new VehicleEntities())
            {  
                var List = db.T_UserMaster.Where(t => t.RID == RID).FirstOrDefault();
                List.username = Model.Driver_Name;
                List.tel = Model.Driver_Tel;
                List.userid = Model.Driver_Number;
                List.text3 =Model.CarInfo;
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
        /// DriverInfodelete
        /// </summary>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public static string DriverInfodelete(string deleteItem)
        {
            string[] ridArr = deleteItem.Split(',');
            string result="null";
            //using (var db = new VehicleEntities())
            //{
            //    foreach (var ds in ridArr)
            //    {
            //        string Rid = ds;
            //        var model = db.DriverInfo.FirstOrDefault(p => p.RID == Rid);
            //        db.DeleteObject(model);
            //    }
            //    int success = db.SaveChanges();
            //    if (success > 0)
            //    {
            //        ViewModel.Result result = new Result();
            //        result.success = true;
            //        result.message = "刪除成功";
            //        return JsonConvert.SerializeObject(result);
            //    }
            //    else
            //    {
            //        return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "刪除失败" });
            //    }

            //}
            return result;
        }

        /// <summary>
        /// 獲取車輛信息
        /// </summary>
        /// <returns></returns>
        public static string GetCarInfo()
        {
            using (var db = new VehicleEntities())
            {
                var rst =
                    from hdr in db.T_Car
                    select new { order=hdr.LastUpdDate, carname = hdr.carName, capacity = hdr.seatNum, license = hdr.carID };
                return JsonConvert.SerializeObject(rst.OrderByDescending(t=>t.order));
            }

        }

        /// <summary>
        /// 獲取公司信息
        /// </summary>
        /// <returns></returns>
        public static string GetCompany()
        {
            using (var db = new VehicleEntities())
            {
                var rst =
                    from hdr in db.T_SystemTable.Where(t => t.Field == "company" && t.Dataid!=" ")
                    select new { text = hdr.Text, value = hdr.Dataid };
                return JsonConvert.SerializeObject(rst);
            }
        }

    }
}