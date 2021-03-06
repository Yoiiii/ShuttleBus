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
using System.Data.Common;

namespace STBBusSystem.Server
{
    
    /// <summary>
    /// Summary description for Vehicle_adminDispatch
    /// </summary>
    public class Vehicle_adminDispatch
    {
        public Vehicle_adminDispatch()
        {

        }

        /// <summary>
        /// 獲取主檔
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetPlanDataGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {
                var list = db.vw_Plan.Where(t => true).OrderByDescending(t => t.time).ThenByDescending(t => t.planID).Skip((page - 1) * row).Take(row);
                var total = db.vw_Plan.Count();
                string JsonGrid = JsonConvert.SerializeObject(new {total= total, rows=list });
                return JsonGrid;

            }
        }

        /// <summary>
        /// 獲取選擇乘客主檔
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetMainGrid(int row, int page)
        {
            using (var db = new VehicleEntities())
            {
                var listModel = db.vw_Plan.Where(t => true).OrderByDescending(t => t.time).ThenByDescending(t => t.planID).Skip((page - 1) * row).Take(row);
                var list = listModel.Where(t => t.destinationID == "1");
                var total = db.vw_Plan.Count();
                string JsonGrid = JsonConvert.SerializeObject(new {total= total, rows=list });
                return JsonGrid;

            }
        }

        

        /// <summary>
        /// 獲取站點
        /// </summary>
        /// <returns></returns>
        public static string Getstep()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_Location
                          select new
                          {
                              value = hdr.locationID,
                              text = hdr.name
                          };
                return JsonConvert.SerializeObject(rst);
            }
        }


        /// <summary>
        /// 獲取司機
        /// </summary>
        /// <returns></returns>
        public static string GetdriverID()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_UserMaster.Where(t => t.type == "driver")
                          select new
                          {
                              value = hdr.userid,
                              text = hdr.username
                          };
                return JsonConvert.SerializeObject(rst);
            }

        }

        /// <summary>
        /// 獲取司機數據
        /// </summary>
        /// <returns></returns>
        public static string GetdriverInfo()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_UserMaster.Where(t => t.type == "driver")
                          select new
                          {
                              Id = hdr.userid,
                              Name = hdr.name,
                              Tel=hdr.tel,
                              Carid=hdr.text3,
                              //Company=hdr.Driver_Company
                          };
                return JsonConvert.SerializeObject(rst);
            }
        }

        /// <summary>
        /// 獲取乘客
        /// </summary>
        /// <returns></returns>
        public static string Getpassenger()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_UserMaster.Where(t => t.type == "passenger" || t.type=="Admin")
                          select new
                          {
                              value = hdr.userid,
                              text = hdr.name,
                          };
                return JsonConvert.SerializeObject(rst);
            }
        }

        /// <summary>
        /// 獲取乘客數據
        /// </summary>
        /// <returns></returns>
        public static string GetpassengerInfo()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_UserMaster.Where(t => t.type == "passenger" || t.type == "Admin")
                          select new
                          {
                              name=hdr.name,
                              userid = hdr.userid,
                              tel=hdr.tel
                          };
                return JsonConvert.SerializeObject(rst);
            }
        }

        /// <summary>
        /// 獲取Plan司機狀態
        /// </summary>
        /// <returns></returns>
        public static string Getstatus()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_SystemTable.Where(t => t.Field == "PlanState")
                          select new
                          {
                             value=hdr.Dataid,
                             text=hdr.Text
                          };
                return JsonConvert.SerializeObject(rst.OrderBy(t=>t.value));
            }
        }

        /// <summary>
        /// 獲取乘客狀態
        /// </summary>
        /// <returns></returns>
        public static string Getpassengerstatus()
        {
            using (var db = new VehicleEntities())
            {
                var rst = from hdr in db.T_SystemTable.Where(t => t.Field == "PassengerState")
                          select new
                          {
                              value = hdr.Dataid,
                              text = hdr.Text
                          };
                return JsonConvert.SerializeObject(rst.OrderBy(t=>t.value));
            }
        }

        /// <summary>
        /// 獲取站點表
        /// </summary>
        /// <returns></returns>
        public static string GetstepGrid(string planID)
        {
            using (var db = new VehicleEntities())
            {
                var rst = db.D_RoadList.Where(t => t.planID == planID);
                return JsonConvert.SerializeObject(rst.OrderBy(t=>t.sequence));
            }
        }

         /// <summary>
        /// 獲取乘客表
        /// </summary>
        /// <returns></returns>
        public static string GetPassengerGrid(string planID)
        {
            using (var db = new VehicleEntities())
            {
                var rst = db.D_PassengerList.Where(t => t.planID == planID);
                return JsonConvert.SerializeObject(rst.OrderBy(t=>t.locationID).ThenBy(t=>t.destinationID));
            }
        }
        


        /// <summary>
        /// 主檔搜索
        /// </summary>
        /// <param name="row"></param>
        /// <param name="page"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static string PlanSearch(int row, int page, PlanSearchModel Model)
        {
            DateTime? Begin_Date;
            DateTime? End_Date;
            int? status;
            if (Model.BeginDate != "")
            {
                Begin_Date = Convert.ToDateTime(Model.BeginDate);
            }
            else
            {

                Begin_Date = null;
            }
            if (Model.EndData != "")
            {
                End_Date = Convert.ToDateTime(Model.EndData);
            }
            else
            {

                End_Date = null;
            }
            if (Model.status != "")
            {
                status = Convert.ToInt32(Model.status);
            }
            else
            {

                status = null;
            }


                
            using (var db = new VehicleEntities())
            {          
                var Searchlist = from hdr in db.vw_Plan.Where(p =>
                                        (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                     && (p.LastUpdDate <= End_Date || End_Date == null)
                                     && (p.carID==Model.carID || string.IsNullOrEmpty(Model.carID))
                                     && (p.driverID==Model.driverID || string.IsNullOrEmpty(Model.driverID))
                                     && (p.status == status || string.IsNullOrEmpty(Model.status))
                                     ).OrderByDescending(p => p.LastUpdDate).Skip((page - 1) * row).Take(row)
                          select hdr;
                var AllList = from hdr in db.vw_Plan.Where(p =>
                                       (p.LastUpdDate >= Begin_Date || Begin_Date == null)
                                    && (p.LastUpdDate <= End_Date || End_Date == null)
                                    && (p.carID.Contains(Model.carID) || string.IsNullOrEmpty(Model.carID))
                                    && (p.driverID.Contains(Model.driverID) || string.IsNullOrEmpty(Model.driverID))
                                    && (p.status == status || string.IsNullOrEmpty(Model.status))
                                     ).OrderByDescending(p => p.LastUpdDate)
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
        public static string PlanAddSave(PlanModel PlanModel, List<PassengerModel> PassengerModel, List<stepModel> stepModel, string LoginUserID)
        {
            var ToDay = DateTime.Now.ToString("yyyy-MM-dd");
            using (var db = new VehicleEntities())
            {
                var Rid = Guid.NewGuid().ToString();
                var PlanID=GetPlanNo();
                var stepLen_b = stepModel.Count;
                var stepLen = stepLen_b - 1;
                string LocationID ="" ;
                string DestinationID ="";

                foreach (var item in stepModel)
                {
                    if (item.sequence == 0)
                        LocationID = item.locationID;
                    if (item.sequence == stepLen)
                        DestinationID = item.locationID;
                }

                //存儲主檔
                M_Plan Plan = new M_Plan();
                Plan.RID = Rid;
                Plan.planID = PlanID;
                Plan.carID = PlanModel.carID;
                Plan.driverID = PlanModel.driverID;
                Plan.driverTel = PlanModel.driverTel;
                Plan.locationID = LocationID;
                Plan.destinationID = DestinationID;
                Plan.time =Convert.ToDateTime(PlanModel.time);
                Plan.status = Convert.ToInt32(PlanModel.status);
                Plan.step = 0;
                Plan.CreateDate = Convert.ToDateTime(PlanModel.CreateDate);
                Plan.CreateUser = LoginUserID;
                Plan.LastUpdDate = Convert.ToDateTime(PlanModel.CreateDate);
                Plan.LastUpdUser = LoginUserID;
                Plan.valid = "1";
                Plan.IsSend = "0";
                Plan.batch = PlanID;
                db.AddToM_Plan(Plan);

                //存儲站點表
                foreach (var item in stepModel)
                {
                    D_RoadList RoadList = new D_RoadList();
                    RoadList.RID = item.RID;
                    RoadList.PlanRID = Rid;
                    RoadList.planID = PlanID;
                    RoadList.locationID =Convert.ToInt32( item.locationID);
                    RoadList.sequence = item.sequence;
                    db.AddToD_RoadList(RoadList);
                }

                //存儲乘客表
                foreach (var item in PassengerModel)
                {
                    D_PassengerList PassengerList = new D_PassengerList();
                    PassengerList.RID = item.RID;
                    PassengerList.PlanRID = Rid;
                    PassengerList.planID = PlanID;
                    PassengerList.passengerID = item.passengerID;
                    PassengerList.time = Convert.ToDateTime(item.time);
                    PassengerList.status = item.status;
                    PassengerList.remark = item.remark;
                    PassengerList.returnTime = "";
                    PassengerList.locationID = item.locationID;
                    PassengerList.destinationID = item.destinationID;

                    db.AddToD_PassengerList(PassengerList);
                }

                var YearText = DateTime.Now.ToString("yyyyMMdd").Substring(2, 2);
                int? number = null;
                var List = db.T_SystemApply.Where(t => t.FunctionName == "PlanNo" && t.DateTime == YearText);
                if (List.Count() > 0)
                {
                    var num = List.Max(t => t.Number);
                    //number = List.OrderByDescending(t => t.Create_Date).First().Number;
                    number = num + 1;
                }
                else
                {
                    number = 1;
                }

                //存儲單號記錄
                T_SystemApply SystemApply = new T_SystemApply();
                SystemApply.Create_Date = DateTime.Now;
                SystemApply.DateTime = PlanModel.CreateDate.ToString("yyyy-MM-dd").Substring(2,2);
                SystemApply.Number = number;
                SystemApply.FunctionName = "PlanNo";
                SystemApply.RID = Guid.NewGuid().ToString();
                db.AddToT_SystemApply(SystemApply);

                int success = db.SaveChanges();
                if (success > 0)
                {
                    //在這寫
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "提交保存成功";
                    WeChatWS.WeChatWSSoapClient ws = new WeChatWS.WeChatWSSoapClient();
                    ws.seedsmsToDriver(Rid);
                    using (var plan = new VehicleEntities())
                    {
                        var rst = plan.M_Plan.Where(t => t.RID == Rid).FirstOrDefault();
                        rst.IsSend = "1";
                        plan.SaveChanges();
                    }
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
        public static string PlanEditSave(PlanModel PlanModel, List<PassengerModel> PassengerModel, List<stepModel> stepModel, string LoginUserID)
        {
            using (var db = new VehicleEntities())
            {
                DateTime LastUpdDate=DateTime.Now;
                string LocationID = "";
                string DestinationID = "";
                var stepLen = stepModel.Count-1;
                var RID = "";
                foreach (var item in stepModel)
                {
                    if (item.sequence == 0)
                        LocationID = item.locationID;
                    if (item.sequence == stepLen)
                        DestinationID = item.locationID;
                }

                var PlanList = db.M_Plan.Where(t => t.planID == PlanModel.planID);
                if (PlanList.Count() > 1)
                {

                    foreach (var item in PlanList)
                    {
                        var itemRID = item.RID;
                        item.LastUpdDate = LastUpdDate;
                        item.locationID =LocationID;
                        item.destinationID = DestinationID;
              

                        var PassengerList = db.D_PassengerList.Where(t => t.PlanRID == item.RID);
                        if (PassengerList.Count() > 0)
                        {
                            item.carID = PlanModel.carID;
                            item.driverTel = PlanModel.driverTel;
                            item.driverID = PlanModel.driverID;
                            item.locationID = LocationID;
                            item.destinationID = DestinationID;
                            item.status = Convert.ToInt32(PlanModel.status);
                            item.time = Convert.ToDateTime(PlanModel.time);
                            item.LastUpdUser = LoginUserID;

                            var PlanID = item.planID;
                            RID = item.RID;

                            //站點表
                            var RoadList = db.D_RoadList.Where(t => t.planID == PlanID);

                            foreach (var del in RoadList)
                            {
                                db.DeleteObject(del);
                            }


                            foreach (var setpitem in stepModel)
                            {

                                D_RoadList RoadListitem = new D_RoadList();
                                RoadListitem.RID = Guid.NewGuid().ToString();
                                RoadListitem.PlanRID = RID;
                                RoadListitem.planID = PlanID;
                                RoadListitem.locationID = Convert.ToInt32(setpitem.locationID);
                                RoadListitem.sequence = setpitem.sequence;
                                if (setpitem.atime != null)
                                    RoadListitem.atime = Convert.ToDateTime(setpitem.atime);
                                if (setpitem.ltime != null)
                                    RoadListitem.ltime = Convert.ToDateTime(setpitem.ltime);
                                db.AddToD_RoadList(RoadListitem);
                            }


                            //乘客表
                            var PassengerListModel = db.D_PassengerList.Where(t => t.PlanRID == RID);
                            foreach (var del in PassengerList)
                            {
                                db.DeleteObject(del);
                            }

                            foreach (var Passengeritem in PassengerModel)
                            {
                                D_PassengerList PassengerListitem = new D_PassengerList();
                                PassengerListitem.RID = Guid.NewGuid().ToString();
                                PassengerListitem.PlanRID = RID;
                                PassengerListitem.planID = PlanID;
                                PassengerListitem.returnTime = Passengeritem.returnTime;
                                PassengerListitem.passengerID = Passengeritem.passengerID;
                                PassengerListitem.time = Convert.ToDateTime(Passengeritem.time);
                                PassengerListitem.status = Passengeritem.status;
                                PassengerListitem.remark = Passengeritem.remark;
                                PassengerListitem.locationID = Passengeritem.locationID;
                                PassengerListitem.destinationID = Passengeritem.destinationID;
                                if (Passengeritem.confirmTime != null)
                                    PassengerListitem.confirmTime = Convert.ToDateTime(Passengeritem.confirmTime);
                                db.AddToD_PassengerList(PassengerListitem);
                            }



                        }
                    }







                }
                else
                {





                    RID = db.M_Plan.Where(t => t.planID == PlanModel.planID).FirstOrDefault().RID;



                    //主檔
                    var Plan = db.M_Plan.Where(t => t.RID == RID).FirstOrDefault();
                    Plan.carID = PlanModel.carID;
                    Plan.driverTel = PlanModel.driverTel;
                    Plan.driverID = PlanModel.driverID;
                    Plan.locationID = LocationID;
                    Plan.destinationID = DestinationID;
                    Plan.status = Convert.ToInt32(PlanModel.status);
                    Plan.time = Convert.ToDateTime(PlanModel.time);
                    Plan.LastUpdDate = LastUpdDate;
                    Plan.LastUpdUser = LoginUserID;



                    //站點表
                    var RoadList = db.D_RoadList.Where(t => t.PlanRID == RID);
                    foreach (var del in RoadList)
                    {
                        db.DeleteObject(del);
                    }

                    foreach (var item in stepModel)
                    {

                        D_RoadList RoadListitem = new D_RoadList();
                        RoadListitem.RID = Guid.NewGuid().ToString();
                        RoadListitem.PlanRID = RID;
                        RoadListitem.planID = Plan.planID;
                        RoadListitem.locationID = Convert.ToInt32(item.locationID);
                        RoadListitem.sequence = item.sequence;
                        if (item.atime != null)
                            RoadListitem.atime = Convert.ToDateTime(item.atime);
                        if (item.ltime != null)
                            RoadListitem.ltime = Convert.ToDateTime(item.ltime);
                        db.AddToD_RoadList(RoadListitem);
                    }

                    //乘客表
                    var PassengerList = db.D_PassengerList.Where(t => t.PlanRID == RID);
                    foreach (var del in PassengerList)
                    {
                        db.DeleteObject(del);
                    }

                    foreach (var item in PassengerModel)
                    {
                        D_PassengerList PassengerListitem = new D_PassengerList();
                        PassengerListitem.RID = Guid.NewGuid().ToString();
                        PassengerListitem.PlanRID = RID;
                        PassengerListitem.planID = Plan.planID;
                        PassengerListitem.passengerID = item.passengerID;
                        PassengerListitem.time = Convert.ToDateTime(item.time);
                        PassengerListitem.status = item.status;
                        PassengerListitem.returnTime = item.returnTime;
                        PassengerListitem.remark = item.remark;
                        PassengerListitem.locationID = item.locationID;
                        PassengerListitem.destinationID = item.destinationID;
                        if (item.confirmTime != null)
                            PassengerListitem.confirmTime = Convert.ToDateTime(item.confirmTime);
                        db.AddToD_PassengerList(PassengerListitem);
                    }
                }



                int success = db.SaveChanges();
                if (success > 0)
                {

                    var IsSend = "";
                    //在這寫
                    using (var plan = new VehicleEntities())
                    {
                        var SendPlan = plan.M_Plan.Where(t => t.RID == RID).FirstOrDefault();
                        IsSend = SendPlan.IsSend;

                        if (IsSend != "1")
                        {
                            WeChatWS.WeChatWSSoapClient ws = new WeChatWS.WeChatWSSoapClient();
                            ws.seedsmsToDriver(RID);
                            SendPlan.IsSend = "1";

                            plan.SaveChanges();
                        };

                    }
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "提交保存成功";
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "保存失败" });
                }
            }
        }
        
        /// <summary>
        /// 刪除存儲
        /// </summary>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public static string Plandelete(string deleteItem)
        {
            string[] ridArr = deleteItem.Split(',');
            using (var db = new VehicleEntities())
            {
                foreach (var ds in ridArr)
                {
                    string planID = ds;
                    var Planitem = db.M_Plan.FirstOrDefault(p => p.planID == planID && p.valid=="1");
                    db.DeleteObject(Planitem);

                    var PlanRid = Planitem.RID;

                    var RoadListitem = db.D_RoadList.Where(p => p.PlanRID == PlanRid);
                    foreach (var item in RoadListitem)
                    {
                        db.DeleteObject(item);
                    }
                    var PassengerListitem = db.D_PassengerList.Where(p => p.PlanRID == PlanRid);
                    foreach (var item in PassengerListitem)
                    {
                        db.DeleteObject(item);
                    }
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
        /// <summary>
        /// 作廢
        /// </summary>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public static string Planremove(string removeItem)
        {
            string[] ridArr = removeItem.Split(',');
            using (var db = new VehicleEntities())
            {
                foreach (var ds in ridArr)
                {
                    string planID = ds;
                    var Planitem = db.M_Plan.FirstOrDefault(p => p.planID == planID && p.valid == "1");
                    Planitem.valid = "0";
                }
                int success = db.SaveChanges();
                if (success > 0)
                {
                    ViewModel.Result result = new Result();
                    result.success = true;
                    result.message = "作廢成功";
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new ViewModel.Result() { success = true, message = "作廢失败" });
                }

            }
        }

        /// <summary>
        /// 獲取編號
        /// </summary>
        /// <returns></returns>
        public static string GetPlanNo()
        {
            using (var db = new VehicleEntities())
            {
                string ToDay = DateTime.Now.ToString("yyyy-MM-dd");
                var FristText = db.T_SystemTable.Where(t => t.Field == "PlanNo").FirstOrDefault().Dataid;
                var secondText = DateTime.Now.ToString("yyyyMMdd").Substring(2,2);
                var thirdText = "";
                string PlanNo = "";
                var rst = db.T_SystemApply.Where(t => t.FunctionName == "PlanNo" && t.DateTime == secondText);
                if (rst.Count() > 0)
                {
                    var number = rst.Max(t => t.Number);
                    number = number + 1;
                    if (number < 10)
                    {
                        thirdText = "0000" + number.ToString();
                    }
                    else if (number > 9 && number < 100)
                    {
                        thirdText = "000" + number.ToString();
                    }
                    else if (number > 99 && number < 1000)
                    {
                        thirdText = "00" + number.ToString();
                    }
                    else if (number > 999 && number < 10000)
                    {
                        thirdText = "0" + number.ToString();
                    }
                    else
                    {
                        thirdText = number.ToString();
                    }
                }
                else
                {
                    thirdText = "00001";
                }

                PlanNo = FristText + secondText + thirdText;
                return PlanNo;
            }
        }

        /// <summary>
        /// 導入WorkFlow派車數據
        /// </summary>
        /// <returns></returns>
        public static string GetWorkFlow_Import()
        {
            SqlCommand cmd = null;
            DataSet ds = null;
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            cmd = new SqlCommand("usp_GetWorkFlowRecord", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            ds = new DataSet();
            sda.Fill(ds);
            ds.Tables[0].TableName = "TotalUpdate";
            ds.AcceptChanges();
            var TotalUpdate = ds.Tables[0];
            return JsonConvert.SerializeObject(TotalUpdate);
        }

        /// <summary>
        /// 合併wf派車單
        /// </summary>
        /// <returns></returns>
        public static string MergePlan(List<MergePlanModel> M_PlanModel)
        {
            string rst = "success";
            var Now = DateTime.Now;
            int success=0;

            List<M_Plan> PlanList = new List<M_Plan>();
            List<D_PassengerList> PassengerList = new List<D_PassengerList>();
            var PlanID = GetPlanNo();
            var LastUpdDate = "";
            var CreateDate = "";
            var NewRid = Guid.NewGuid().ToString();
            using (var db = new VehicleEntities())
            {
                foreach (var item in M_PlanModel)
                {
                    var changePlanModel = db.M_Plan.Where(t => t.planID == item.PlanID).FirstOrDefault();
                    LastUpdDate = changePlanModel.LastUpdDate.ToString();
                    CreateDate = changePlanModel.CreateDate.ToString();
                }


                foreach (var item in M_PlanModel)
                {
                    var changePlanModel = db.M_Plan.Where(t => t.planID == item.PlanID).FirstOrDefault();

                    M_Plan Planitem = new M_Plan();
                    Planitem.RID = Guid.NewGuid().ToString();

                    Planitem.batch = changePlanModel.batch;
                    Planitem.carID = changePlanModel.carID;
                    Planitem.LastUpdUser = changePlanModel.LastUpdUser;
                    Planitem.CreateUser = changePlanModel.CreateUser;
                    Planitem.driverID = changePlanModel.driverID;
                    Planitem.driverTel = changePlanModel.driverTel;
                    Planitem.time = changePlanModel.time;
                    Planitem.status = changePlanModel.status;
                    Planitem.step = changePlanModel.step;
                    Planitem.LastUpdDate = Convert.ToDateTime(LastUpdDate);
                    Planitem.CreateDate = Convert.ToDateTime(CreateDate);
                    Planitem.planID = PlanID;
                    Planitem.valid = "0";
                    PlanList.Add(Planitem);




                    var PassengerModel = db.D_PassengerList.Where(t => t.planID == item.PlanID);
                    foreach (var Passger_Item in PassengerModel)
                    {
                        D_PassengerList itemList = new D_PassengerList();
                        itemList.RID = Guid.NewGuid().ToString();
                        itemList.planID = PlanID;
                        itemList.locationID = Passger_Item.locationID;
                        itemList.confirmTime = Passger_Item.confirmTime;
                        itemList.destinationID = Passger_Item.destinationID;
                        itemList.passengerID = Passger_Item.passengerID;
                        itemList.returnTime = Passger_Item.returnTime;
                        itemList.PlanRID = NewRid;
                        itemList.remark = Passger_Item.remark;
                        itemList.status = Passger_Item.status;
                        itemList.time = Passger_Item.time;

                        PassengerList.Add(itemList);
                    }
                }

                foreach (var item in M_PlanModel)
                {
                    var PlanModel = db.M_Plan.Where(t => t.planID == item.PlanID).FirstOrDefault();
                    db.DeleteObject(PlanModel);

                    var PassengerModel = db.D_PassengerList.Where(t => t.planID == item.PlanID);
                    foreach (var PassengerItem in PassengerModel)
                    {
                        if (item.PlanID == PassengerItem.planID)
                            db.DeleteObject(PassengerItem);
                    }
                }



                foreach (var addPlanitem in PlanList)
                {
                    db.AddToM_Plan(addPlanitem);
                }

                foreach (var adPassengerditem in PassengerList)
                {
                    db.AddToD_PassengerList(adPassengerditem);
                }





                var YearText = DateTime.Now.ToString("yyyyMMdd").Substring(2, 2);
                int? number = null;
                var List = db.T_SystemApply.Where(t => t.FunctionName == "PlanNo" && t.DateTime == YearText);
                if (List.Count() > 0)
                {
                    var num = List.Max(t => t.Number);
                    //number = List.OrderByDescending(t => t.Create_Date).First().Number;
                    number = num + 1;
                }
                else
                {
                    number = 1;
                }
                //存儲單號記錄
                T_SystemApply SystemApply = new T_SystemApply();
                SystemApply.Create_Date = DateTime.Now;
                SystemApply.DateTime = Now.ToString("yyyy-MM-dd").Substring(2, 2);
                SystemApply.Number = number;
                SystemApply.FunctionName = "PlanNo";
                SystemApply.RID = Guid.NewGuid().ToString();
                db.AddToT_SystemApply(SystemApply);

                success = db.SaveChanges();
               

            }

            var modelRID = "";
            using (var db = new VehicleEntities())
            {
                var model = db.M_Plan.Where(t => t.planID == PlanID).FirstOrDefault();
                modelRID = model.RID;
                var demodel = db.D_PassengerList.Where(t => t.planID == PlanID);
                foreach (var deitem in demodel)
                {
                    deitem.PlanRID = modelRID;
                }
                db.SaveChanges();
            }

            using (var db = new VehicleEntities())
            {
                var modelPlanvalid = db.M_Plan.Where(t => t.RID == modelRID).FirstOrDefault();
                modelPlanvalid.valid = "1";
                modelPlanvalid.IsSend = "0";
                db.SaveChanges();
            }

            if (success > 0)
            {
                return rst;
            }
            else
            {
                rst="fail";
                return rst;
            }
            
        }


        /// <summary>
        /// 返回乘客列表
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        public static string GetMainDtlGridData(string PlanID)
        {
            using (var db = new VehicleEntities())
            {
                var MainModel = db.M_Plan.Where(t => t.planID == PlanID  && t.valid=="1");
                var MainRID = MainModel.FirstOrDefault().RID;
                var DtlModel = db.vw_PassengerList.Where(t => t.PlanRID == MainRID);
                return JsonConvert.SerializeObject(DtlModel.OrderBy(t=>t.locationID).ThenBy(t=>t.destinationID));

            }
        }


        /// <summary>
        /// 檢查刪除單是否處於未開始狀態
        /// </summary>
        /// <returns></returns>
        public static string CheckPlanDelete(string deleteItem)
        {

            string rst = "";
            string[] ridArr = deleteItem.Split(',');
            using (var db = new VehicleEntities())
            {
                foreach (var ds in ridArr)
                {

                    var PlanID=ds;
                    var planModel = db.M_Plan.Where(t => t.planID == PlanID && t.valid=="1" ).FirstOrDefault();
                    if (planModel.status == 0)
                        rst = "success";
                    else
                    {
                        rst = "fail";
                        break;
                    }
                }
            }
            return rst;

        }


    }


}