using System;
using System.Web;

namespace ViewModel
{

    public class CarInfoModel
    {
        public string RID { get; set; }

        public string carName { get; set; }

        public string seatNum { get; set; }

        public string carID { get; set; }

        public string type { get; set; }

        public string Create_Time { get; set; }

        public string Create_Time_E { get; set; }

    }

    public class DriveInfoModel
    {
        public string RID { get; set; }

        public string Driver_Name { get; set; }

        public string Driver_Number { get; set; }

        public string Driver_Tel { get; set; }

        public string CarInfo { get; set; }

    }



    public class LocationInfoModel
    {
        public string RID { get; set; }

        public string locationID { get; set; }

        public string name { get; set; }

        public string longitude { get; set; }

        public string latitude { get; set; }

        public string Radius { get; set; }

        public string Create_Time { get; set; }

        public string Create_Time_E { get; set; }
    }


   


    public class Result
    {
        public string message {  set;  get; }
        public bool success {  set;  get; }
    }







    #region ¦æ¬F¬£¨®


    public class PlanSearchModel
    {
        public string driverID { set; get; }
        public string carID { set; get; }
        public string status { set; get; }
        public string BeginDate { set; get; }
        public string EndData { set; get; }
    }

    public class PlanModel
    {
        public DateTime CreateDate { set; get; }
        public string CreateUser { set; get; }
        public string carID { set; get; }
        public string driverID { set; get; }
        public string driverTel { set; get; }
        public string planID { set; get; }
        public string status { set; get; }
        public string step { set; get; }
        public string time { set; get; }
    }

    public class PassengerModel
    {
        public string PlanRID { set; get; }
        public string RID { set; get; }
        public string destinationID { set; get; }
        public string locationID { set; get; }
        public string passengerID { set; get; }
        public string passengerTel { set; get; }
        public string remark { set; get; }
        public int status { set; get; }
        public string time { set; get; }
        public string confirmTime { set; get; }
        public string returnTime { set; get; }
    }


    public class stepModel
    {
        public string PlanRID { set; get; }
        public string RID { set; get; }
        public string locationID { set; get; }
        public string planID { set; get; }
        public int sequence { set; get; }
        public string atime { set; get; }
        public string ltime { set; get; }
    }


    public class MergePlanModel
    {
        public string PlanID { set; get; }
        public string time { set; get; }
        public string carID { set; get; }
    }

    public class UserModel
    {
        public string username { set; get; }
        public string userid { set; get; }
        public string name { set; get; }
        public string type { set; get; }
        public string tel { set; get; }
    }

    #endregion




}
