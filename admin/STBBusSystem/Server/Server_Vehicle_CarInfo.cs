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
    /// Summary description for Vehicle_CaiInfo
    /// </summary>
    public class Vehicle_CarInfo
    {
        public Vehicle_CarInfo()
        {

        }

        /// <summary>
        /// GetCarInfoDataGrid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetCarInfoDataGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {

                var list = db.T_Car.Where(t =>true).OrderByDescending(t => t.LastUpdDate).Skip((page - 1) * row).Take(row);
                var total = db.T_Car.Count();
                string JsonGrid = JsonConvert.SerializeObject(new {total= total, rows=list });
                return JsonGrid;

            }
        }
        

        /// <summary>
        /// CarInfoSearch
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static string CarInfoSearch(int row, int page, CarInfoModel Model)
        {
            DateTime? Begin_Date;
            DateTime? End_Date;
            int seatNum=0;
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
            if (Model.seatNum != "")
            {
                seatNum = Convert.ToInt32(Model.seatNum);
            }
                
            using (var db = new VehicleEntities())
            {          
                var Searchlist = from hdr in db.T_Car.Where(p =>
                                        (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                     && (p.LastUpdDate <= End_Date || End_Date == null)
                                     && (p.carID.Contains(Model.carID) || string.IsNullOrEmpty(Model.carID))
                                     && (p.carName.Contains(Model.carName) || string.IsNullOrEmpty(Model.carName))
                                     && (p.seatNum == seatNum || string.IsNullOrEmpty(Model.seatNum))
                                     && (p.seatNum!=0)
                                     ).OrderByDescending(p => p.LastUpdDate).Skip((page - 1) * row).Take(row)
                          select hdr;
                var AllList = from hdr in db.T_Car.Where(p =>
                                        (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                     && (p.LastUpdDate <= End_Date || End_Date == null)
                                     && (p.carID.Contains(Model.carID) || string.IsNullOrEmpty(Model.carID))
                                     && (p.carName.Contains(Model.carName) || string.IsNullOrEmpty(Model.carName))
                                     && (p.seatNum == seatNum || string.IsNullOrEmpty(Model.seatNum))
                                     && (p.seatNum != 0)
                                     ).OrderByDescending(p => p.LastUpdDate)
                          select hdr;
                var total = AllList.Count();
                string JsonGrid = JsonConvert.SerializeObject(new { total = total, rows = Searchlist });
                return JsonGrid;
            }
        }

        /// <summary>
        /// CarInfoAddSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <returns></returns>
        public static string CarInfoAddSave(CarInfoModel Model,string LoginUserID)
        {
            using (var db = new VehicleEntities())
            {
                T_Car List = new T_Car();
                List.CreateUser = LoginUserID;
                List.CreateDate =Convert.ToDateTime(Model.Create_Time);
                List.LastUpdDate = Convert.ToDateTime(Model.Create_Time);
                List.LastUpdUser = LoginUserID;
                List.seatNum =Convert.ToInt32( Model.seatNum);
                List.carID = Model.carID;
                List.carName = Model.carName;
                List.RID = Guid.NewGuid().ToString();
                db.AddToT_Car(List);
                int success=db.SaveChanges();
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
        /// CarInfoEditSave
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="LoginUserID"></param>
        /// <param name="RID"></param>
        /// <returns></returns>
        public static string CarInfoEditSave(CarInfoModel Model,string LoginUserID,string RID)
        {
            using (var db = new VehicleEntities())
            {
                var List = db.T_Car.Where(t => t.RID == RID).FirstOrDefault();
                List.LastUpdDate = DateTime.Now;
                List.LastUpdUser = LoginUserID;
                List.seatNum =Convert.ToInt32( Model.seatNum);
                List.carID = Model.carID;
                List.type = Model.type;
                List.carName = Model.carName;
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
        /// CarInfodelete
        /// </summary>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public static string CarInfodelete(string deleteItem)
        {
            string[] ridArr = deleteItem.Split(',');
            using (var db = new VehicleEntities())
            {
                foreach (var ds in ridArr)
                {
                    string Rid = ds;
                    var model = db.T_Car.FirstOrDefault(p => p.RID == Rid);
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