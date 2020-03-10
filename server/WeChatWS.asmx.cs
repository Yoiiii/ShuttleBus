using System;
using System.Data;
using System.Web;
using System.Net;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Web.Script.Services;


namespace WeChatWS
{
    /// <summary>
    /// Summary description for GPSService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class WeChatWS : System.Web.Services.WebService
    {
        private string _SQLServer = "10.6.0.126";
        private string _SQLLoginname = "WeChat_AU";
        private string _SQLPasswork = "20190402";
        private string _SQLDatabase = "WeChat";
        private string appid = "wxa0c71ba9986f5816";//"wx993ca26a9925bc1b";
        private string secret = "3ec70b2dcc430b73c727490c655ac216";//"2fc0ddd7fcb5ae8e64b05715d8001b80";
        private string txmapkey = "XOABZ-E5XKS-M65OB-6264O-JXRNK-GIBQF";
        private string txmapSK = "JlI60yXQ7oqDvCsbKgvp5vILof0HTHru";

        [WebMethod(Description = "Hello World")]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]//測試是否可以正常發送
        public string seedsms(string rid)
        {
            return QYWeixin.QYWeixinHelper.SendList(rid);

        }
        [WebMethod]//測試是否可以正常發送
        public void seedsmsToDriver(string rid)
        {
            QYWeixin.QYWeixinHelper.SendToDriver(rid);

        }
        [WebMethod]//測試是否可以正常發送
        public string seedsmsToAdmin(string rid)
        {
            return QYWeixin.QYWeixinHelper.SendToAdmin(rid);

        }

        /// <summary>
        /// 小程序調用登錄獲取登錄態
        /// </summary>
        /// <param name="code">wx.login()傳回的code</param>
        [WebMethod]//登录
        //[ScriptMethod(UseHttpGet = true)]
        public void login(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + code + "&grant_type=authorization_code";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

            req.Method = "GET";

            using (WebResponse wr = req.GetResponse())
            {
                StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.Default);
                string sReturn = sr.ReadToEnd().Trim();
                JObject jObj = JObject.Parse(sReturn);
                string openid = jObj["openid"].ToString();
                string sessionKey = jObj["session_key"].ToString();
                string sopenid = Cryptography.Encrypt(openid);//返回加密的openid
                jObj.Add("sopenid", sopenid);
                //Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        /// <summary>
        /// 初次使用用户绑定微信
        /// </summary>
        /// <param name="userID">工号</param>
        /// <param name="tel">联系电话</param>
        /// <param name="code">微信登录code</param>
        [WebMethod]//绑定
        public void Binding(string userID, string tel, string code)
        {
            string sSQL = "SELECT * FROM [WeChat].[dbo].[T_UserMaster] WHERE tel='" + tel + "'and userid='" + userID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));

            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:false}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse(str);
                string openid = jObj1["openID"].ToString();
                if (openid == "")
                {
                    sSQL = "UPDATE [WeChat].[dbo].[T_UserMaster] SET openid = '" + getOpenID(code) + "' " +
                          "WHERE tel='" + tel + "'and userid='" + userID + "'";
                    int row = DbHelperSQL.ExecuteSql(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    if (row == 1)
                    {
                        jObj1 = JObject.Parse("{result:\"true\"}");
                    }
                    else
                    {
                        jObj1 = JObject.Parse("{result:\"false\"}");
                    }
                }
                else
                {
                    jObj1 = JObject.Parse("{result:0}");
                }
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取用戶信息
        public void GetLangauge(string sopenid, string langauge)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            try
            {
                string openid = Cryptography.Decrypt(sopenid);
                //string sSQL = "select UserID,name,Type from [WeChat].[dbo].[T_UserMaster] where OpenID='" + openid + "'";
                string sSQL = "select TagID,TagValue from [WeChat].[dbo].[M_LabelTranslation_Wechat] where LangCode='" + langauge + "'";
                DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str =ConvertJson.ToJson(ds.Tables[0]);
                if (str == "")
                {
                    JObject jObj1 = JObject.Parse("{result:\"null\"}");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JArray jObj1 = JArray.Parse(str);
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            catch (Exception e)
            {
                //JObject jObj1 = JObject.Parse("{err:\"\"}");
                // Context.Response.Write(jObj1);
                // Context.Response.End();
            }
        }

        /// <summary>
        /// 根据openid获取用户信息
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]//獲取用戶信息
        public void GetUserInfo(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            try
            {
                string openid = Cryptography.Decrypt(sopenid);
                //string sSQL = "select UserID,name,Type from [WeChat].[dbo].[T_UserMaster] where OpenID='" + openid + "'";
                string sSQL = "select userid,username AS name,type from [WeChat].[dbo].[T_UserMaster] where openID='" + openid + "'";
                DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
                if (str == "")
                {
                    JObject jObj1 = JObject.Parse("{result:\"null\"}");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse(str);
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            catch (Exception e)
            {
                //JObject jObj1 = JObject.Parse("{err:\"\"}");
                // Context.Response.Write(jObj1);
                // Context.Response.End();
            }
        }

        /// <summary>
        /// 获取當前行程
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]//獲取行程信息
        public void GetTravel(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT  TOP 1 PL.RID, PL.PlanRID,L.name AS location,L2.name AS destination,PL.confirmTime, CONVERT(VARCHAR(19),PL.time,120)AS time,P.carID,U.username ,U.tel,P.status,P.step,PL.passengerID,P.latitude,P.longitude,P.gpsTime " +
                           "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                           "LEFT JOIN [WeChat].[dbo].[M_Plan] AS P " +
                           "ON P.RID=PL.PlanRID " +
                           "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                           "ON U.userID=P.driverID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L  " +
                           "ON L.locationID=PL.locationID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2  " +
                           "ON L2.locationID=PL.destinationID " +
                           "WHERE PL.passengerID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "') " +
                           "AND P.status!= 3  " +
                           "AND '" + stime + "' < PL.Time order by PL.time ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject Job = JObject.Parse(str);

                // JArray Jar = JArray.Parse(str);
                //foreach (JObject items in Jar)
                //{//}
                string PlanRID = Job["PlanRID"].ToString();
                string passengerID = Job["passengerID"].ToString();
                string sSQL2 = "SELECT L.name AS location, R.sequence, CONVERT(VARCHAR(19),R.atime,120)AS atime,CONVERT(VARCHAR(19),R.ltime,120)AS ltime " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE PlanRID ='" + PlanRID + "' ORDER BY sequence";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                if (str2 == "")
                {
                    JArray jar = JArray.Parse("[]");
                    Job.Add("roadlist", jar);
                }
                else
                {
                    string sssss = ConvertJson.ToJson(ds2.Tables[0]);
                    JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                    Job.Add("roadlist", jar);
                }
                string sSQL3 = "SELECT U.username, U.name ,L.name AS location,L2.name AS destination,PL.confirmTime,R.sequence " +
                               "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                               "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                               "ON U.userID=PL.passengerID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                               "ON L.locationID=PL.locationID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                               "ON L2.locationID=PL.destinationID " +
                               "LEFT JOIN [WeChat].[dbo].[D_RoadList] AS R " +
                               "ON R.locationID =  PL.locationID AND PL.PlanRID=R.PlanRID " +
                               "WHERE PL.PlanRID='" + PlanRID + "' and PL.passengerID != '" + passengerID + "' ORDER BY R.sequence,L.name,L2.name,U.name ";
                DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                if (str3 == "")
                {
                    JArray jar2 = JArray.Parse("[]");
                    Job.Add("passengers", jar2);
                }
                else
                {
                    JArray jar2 = JArray.Parse(ConvertJson.ToJson(ds3.Tables[0]));
                    Job.Add("passengers", jar2);
                }


                string sSQL4 = "SELECT latitude,longitude " +
                              "FROM [WeChat].[dbo].[T_Location] " +
                              "WHERE name ='" + Job["location"].ToString() + "'";
                DataSet ds4 = DbHelperSQL.Query(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str4 = ConvertJson.ToJObject(ConvertJson.ToJson(ds4.Tables[0]));
                if (str4 != "" && Job["gpsTime"].ToString() != "" && Job["latitude"].ToString() != "" && Job["longitude"].ToString() != "")
                {
                    JObject jObj2 = JObject.Parse(str4);
                    DateTime t1 = Convert.ToDateTime(Job["gpsTime"].ToString());
                    DateTime nt = DateTime.Now;
                    string ForecastTime = duration(jObj2["latitude"].ToString(), jObj2["longitude"].ToString(), Job["latitude"].ToString(), Job["longitude"].ToString());
                    DateTime t2 = nt.AddSeconds(Convert.ToInt64(ForecastTime));
                    string Time = t2.ToString("yyyy-MM-dd HH:mm:ss");
                    Job.Add("ForecastTime", Time);

                }
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(Job);
                Context.Response.Flush();
                Context.Response.End();
            }
        }




        [WebMethod]//獲取地图信息
        public void GetMapInfo(string sopenid, string RID)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string openid = Cryptography.Decrypt(sopenid);
            DateTime now = DateTime.Now;
            string sSQL1 = "SELECT " +
                           "P.planID,P.carID ,P.longitude,P.latitude, CONVERT(VARCHAR(19),P.gpsTime,24)AS gpsTime,L.longitude AS Elongitude ,L.latitude AS Elatitude,L.name " +
                           "FROM [WeChat].[dbo].[M_Plan] AS P " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                           "on L.locationID =p.destinationID " +
                           "where p.RID ='" + RID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj = JObject.Parse(str);
                string ForecastTime = duration(jObj["latitude"].ToString(), jObj["longitude"].ToString(), jObj["Elatitude"].ToString(), jObj["Elongitude"].ToString());
                DateTime t2 = now.AddSeconds(Convert.ToInt64(ForecastTime));
                string Time = t2.ToString("HH:mm");
                jObj.Add("ForecastTime", Time);
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取地图信息
        public void GetChartInfo(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string openid = Cryptography.Decrypt(sopenid);
            DateTime now = DateTime.Now;
            DateTime startWeek = now.AddDays(1 - Convert.ToInt32(now.DayOfWeek.ToString("d")));  //本周周一  
            DateTime endWeek = startWeek.AddDays(6);  //本周周日  
            DateTime startMonth = now.AddDays(1 - now.Day);  //本月月初  
            DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);  //本月月末  

            DateTime.Now.AddDays(Convert.ToInt32 (1 - Convert.ToInt32(DateTime.Now.DayOfWeek)) - 7);
            DateTime.Now.AddDays(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)) - 7).AddDays(6);
            DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:01")).AddMonths(-1).ToShortDateString();
            DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 23:59")).AddDays(-1).ToShortDateString();
            string sSQL1 = "select COUNT(driverID)as count,driverID,U.name " +
                           "from [WeChat].[dbo].[M_Plan] AS P " +
                           "inner join [WeChat].[dbo].[T_UserMaster] AS U " +
                           "on P.driverID=U.userid " +
                           "where  P.valid='1' " +
                           "and '" + startWeek.AddDays(-7).ToString("yyyy-MM-dd 00:01") + "' < P.time " +
                           "and   '" + endWeek.AddDays(-7).ToString("yyyy-MM-dd 23:59") + "' > P.time " +
                           "group by P.driverID, U.name ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            string sSQL2 = "select COUNT(driverID)as count,driverID,U.name " +
                           "from [WeChat].[dbo].[M_Plan] AS P " +
                           "inner join [WeChat].[dbo].[T_UserMaster] AS U " +
                           "on P.driverID=U.userid " +
                           "where  P.valid='1' " +
                           "and '" + startMonth.AddMonths(-1).ToString("yyyy-MM-dd 00:01") + "' < P.time " +
                           "and   '" + endMonth.AddMonths(-1).ToString("yyyy-MM-dd 23:59") + "' > P.time " +
                           "group by P.driverID, U.name ";
            DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str2 = ConvertJson.ToJson(ds2.Tables[0]);
            string sSQL3 = "select COUNT(driverID)as count,driverID,U.name " +
                           "from [WeChat].[dbo].[M_Plan] AS P " +
                           "inner join [WeChat].[dbo].[T_UserMaster] AS U " +
                           "on P.driverID=U.userid " +
                           "where  P.valid='1' " +
                           "and '" + startMonth.ToString("yyyy-MM-dd 00:01") + "' < P.time " +
                           "and   '" + endMonth.ToString("yyyy-MM-dd 23:59") + "' > P.time " +
                           "group by P.driverID, U.name ";
            DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str3 = ConvertJson.ToJson(ds3.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse("{}");
                JArray Jar = JArray.Parse(str);
                JArray Jar2 = JArray.Parse(str2);
                JArray Jar3 = JArray.Parse(str3);
                jObj1.Add("lastWeek", Jar);
                jObj1.Add("thisMounth", Jar3);
                jObj1.Add("lastMounth", Jar2);
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }











            /*if (type == "w")
            {
                string sSQL1 = "select COUNT(driverID)as count,driverID,U.name " +
               "from [WeChat].[dbo].[M_Plan] AS P " +
               "inner join [WeChat].[dbo].[T_UserMaster] AS U " +
               "on P.driverID=U.userid " +
               "where  P.valid='1' " +
               "and '" + startWeek.AddDays(-7).ToString("yyyy-MM-dd 00:01") + "' < P.time " +
               "and   '" + endWeek.AddDays(-7).ToString("yyyy-MM-dd 23:59") + "' > P.time " +
               "group by P.driverID, U.name ";
                DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str = ConvertJson.ToJson(ds.Tables[0]);
                if (str == "" || str == "]")
                {
                    JObject jObj1 = JObject.Parse("{result:\"null\"}");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {

                    JArray Jar = JArray.Parse(str);
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(Jar);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else {
                string sSQL2 = "select COUNT(driverID)as count,driverID,U.name " +
                 "from [WeChat].[dbo].[M_Plan] AS P " +
                 "inner join [WeChat].[dbo].[T_UserMaster] AS U " +
                 "on P.driverID=U.userid " +
                 "where  P.valid='1' " +
                 "and '" + startMonth.AddMonths(-1).ToString("yyyy-MM-dd 00:01") + "' < P.time " +
                 "and   '" + endMonth.AddMonths(-1).ToString("yyyy-MM-dd 23:59") + "' > P.time " +
                 "group by P.driverID, U.name ";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJson(ds2.Tables[0]);

                if (str2 == "" || str2 == "]")
                {
                    JObject jObj1 = JObject.Parse("{result:\"null\"}");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {

                    JArray Jar = JArray.Parse(str2);
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(Jar);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }*/



  
        }



        /// <summary>
        /// 获取当前任务
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]//獲取任務
        public void GetMission(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT TOP 1 P.RID, P.planID ,P.carID,L.name AS location,L2.name AS destination,CONVERT(VARCHAR(19),P.time,120)AS time,U.username,U.name ,U.tel,P.status,P.step,R.locationID,L3.name AS nowlocation " +
                           "FROM [WeChat].[dbo].[M_Plan] AS P " +
                           "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                           "ON U.userID=P.driverID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                           "ON L.locationID=P.locationID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                           "ON L2.locationID=P.destinationID " +
                           "LEFT JOIN [WeChat].[dbo].[D_RoadList] AS R " +
                           "ON P.RID=R.PlanRID AND P.step=R.sequence " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L3 " +
                           "ON L3.locationID=R.locationID " +
                           "WHERE P.driverID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "') AND P.status!= 3 AND P.valid ='1' AND '" + stime + "' < P.Time ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject Job = JObject.Parse(str);
                // JArray Jar = JArray.Parse(str);
                //foreach (JObject items in Jar)
                //{//}
                string PlanRID = Job["RID"].ToString();
                string sSQL2 = "SELECT L.name AS location, R.sequence, R.atime,R.ltime , L.Radius " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE PlanRID ='" + PlanRID + "' ORDER BY sequence";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                if (str2 == "")
                {
                    JArray jar = JArray.Parse("[]");
                    Job.Add("roadlist", jar);
                }
                else
                {
                    string sssss = ConvertJson.ToJson(ds2.Tables[0]);
                    JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                    Job.Add("roadlist", jar);
                }
                string sSQL3 = "SELECT U.username, U.name , L.name AS location,L2.name AS destination,PL.confirmTime,U.tel,PL.RID " +
                               "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                               "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                               "ON U.userID=PL.passengerID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                               "ON L.locationID=PL.locationID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                               "ON L2.locationID=PL.destinationID " +
                               "WHERE PL.PlanRID='" + PlanRID + "' ORDER BY L.name,L2.name,U.username ";
                DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                if (str3 == "")
                {
                    JArray jar2 = JArray.Parse("[]");
                    Job.Add("passengers", jar2);
                }
                else
                {
                    JArray jar2 = JArray.Parse(ConvertJson.ToJson(ds3.Tables[0]));
                    Job.Add("passengers", jar2);
                }
                Context.Response.Write(Job);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]//乘客確認上車
        public void passengerConfirm(string sopenid, string plsID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "WHERE RID ='" + plsID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "' ,status=2 " +
                               "WHERE RID='" + plsID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        /// <summary>
        /// 乘客撤销上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]//乘客撤銷上車
        public void passengerCancel(string sopenid, string plsID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "WHERE RID ='" + plsID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() != "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList] " +
                               "SET confirmTime=NULL ,status=1 " +
                               "WHERE RID='" + plsID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//司機確認乘客上車
        public void dirverConfirm(string sopenid, string RID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "WHERE RID ='" + RID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "' , status = 2 " +
                               "WHERE RID='" + RID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }
        [WebMethod]//司機更新GPS定位
        public void driverStart(string sopenid, string rid)
        {
            string sSQL = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                       "SET status=1 " +
                                       "WHERE RID ='" + rid + "'";
            int row = DbHelperSQL.ExecuteSql(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            if (row == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"success\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }
        [WebMethod]//司機更新GPS定位
        public void driverUpdataGPS(string sopenid, string rid, string longitude, string latitude)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            int row4 = 0;
            string sSQL1 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET longitude='" + longitude + "' ,latitude='" + latitude + "' ,gpsTime='" + nowTime + "' " +
                               "WHERE RID='" + rid + "'";
            int row1 = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            ///取下一个站的经纬度计算距离

            string sSQL2 = "SELECT R.atime ,L.longitude ,L.latitude ,P.step ,L.name " +
                           "FROM [WeChat] .[dbo] .[M_Plan] AS P " +
                           "LEFT JOIN [WeChat] .[dbo] .[D_RoadList] AS R " +
                           "ON R.sequence =P.STEP AND R.PlanRID=P.RID " +
                           "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L " +
                           "ON R.locationID= L.LocationID " +
                           "WHERE P.RID ='" + rid + "' ";
            DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
            JObject jObj = JObject.Parse(str);
            int step = Convert.ToInt32(jObj["step"].ToString());
            /*if (jObj["atime"].ToString() == "")
            {
                double distance = GetDistance(Convert.ToDouble(jObj["longitude"].ToString()), Convert.ToDouble(jObj["latitude"].ToString()), Convert.ToDouble(longitude), Convert.ToDouble(latitude));
                //计算经纬度、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、
                if (distance <= 100)
                {
                    string sSQL3 = "SELECT COUNT(*) AS NO " +
                                   "FROM [WeChat].[dbo].[D_RoadList] " +
                                   "WHERE PLANRID ='" + rid + "'";
                    DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                    JObject jObj2 = JObject.Parse(str3);
                    int no = Convert.ToInt32(jObj2["NO"].ToString()) - 1;
                    string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                   "SET atime='" + nowTime + "' " +
                                   "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                    row4 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    if (step == no)
                    {
                        string sSQL5 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                       "SET status=2 " +
                                       "WHERE RID ='" + rid + "'";
                        int row5 = DbHelperSQL.ExecuteSql(sSQL5, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }
                }
            }*/
            if (row1 == 1 && row4 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"success\"}");
                jObj1.Add("arrival", jObj["name"].ToString());
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else if (row1 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"success\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//司機更新GPS定位
        public void driverUpdataGPSnew(string sopenid, string rid,string planID, string longitude, string latitude, int speed)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid); 
            string result = "";
            int row4 = 0;
            int row6 = 0;
            double distance = 0;
            string sSQL1 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET longitude='" + longitude + "' ,latitude='" + latitude + "' ,gpsTime='" + nowTime + "' " +
                               "WHERE RID='" + rid + "'";
            int row1 = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            //添加logs
            string Addlogs = "INSERT INTO [WeChat].[dbo].[M_logs]   (time, planID,latitude,longitude) VALUES ('" + nowTime + "', '" + planID + "','" + latitude + "','" + longitude + "') ";
            int rowLogs = DbHelperSQL.ExecuteSql(Addlogs, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);

            ///取下一个站的经纬度计算距离

            string sSQL2 = "SELECT R.atime, R.ltime ,L.longitude ,L.latitude ,P.step ,L.name ,L.Radius,P.status " +
                           "FROM [WeChat] .[dbo] .[M_Plan] AS P " +
                           "LEFT JOIN [WeChat] .[dbo] .[D_RoadList] AS R " +
                           "ON R.sequence =P.STEP AND R.PlanRID=P.RID " +
                           "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L " +
                           "ON R.locationID= L.LocationID " +
                           "WHERE P.RID ='" + rid + "' ";
            DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
            JObject jObj = JObject.Parse(str);
            int step = Convert.ToInt32(jObj["step"].ToString());
            int radius = Convert.ToInt32(jObj["Radius"].ToString());
            if (jObj["atime"].ToString() == "")
            {
                result = "Arrive";
                distance = GetDistance(Convert.ToDouble(jObj["longitude"].ToString()), Convert.ToDouble(jObj["latitude"].ToString()), Convert.ToDouble(longitude), Convert.ToDouble(latitude));
                if (distance <= radius)
                {
                    string sSQL3 = "SELECT COUNT(*) AS NO " +
                                   "FROM [WeChat].[dbo].[D_RoadList] " +
                                   "WHERE PLANRID ='" + rid + "'";
                    DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                    JObject jObj2 = JObject.Parse(str3);
                    int no = Convert.ToInt32(jObj2["NO"].ToString()) - 1;
                    string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                   "SET atime='" + nowTime + "' " +
                                   "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                    row4 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    if (step == no)
                    {
                        string sSQL5 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                       "SET status=3 " +
                                       "WHERE RID ='" + rid + "'";
                        int row5 = DbHelperSQL.ExecuteSql(sSQL5, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }
                    QYWeixin.QYWeixinHelper.SendList(rid);
                }
                /*else
                {
                    string sSQL7 = "SELECT L.longitude,L.latitude,L.Radius,R.sequence FROM [WeChat].[dbo].[D_RoadList] AS R " +
                                   "LEFT JOIN [WeChat].[dbo].[T_Location] AS L "+
                                   "ON R.locationID = L.locationID "+
                                   "WHERE PlanRID ='" + rid + "' AND sequence = (SELECT COUNT(*) AS NO FROM [WeChat].[dbo].[D_RoadList] " +
                                   "WHERE PlanRID ='" + rid + "') -1 ";
                    DataSet ds7 = DbHelperSQL.Query(sSQL7, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str7 = ConvertJson.ToJObject(ConvertJson.ToJson(ds7.Tables[0]));
                    JObject jObj7 = JObject.Parse(str7);
                    distance = GetDistance(Convert.ToDouble(jObj7["longitude"].ToString()), Convert.ToDouble(jObj7["latitude"].ToString()), Convert.ToDouble(longitude), Convert.ToDouble(latitude));
                    radius =Convert.ToInt32(jObj7["Radius"].ToString());
                    if (distance <= radius)
                    {
                        string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET atime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj7["sequence"].ToString() + "'";
                        row4 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);

                        string sSQL5 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                       "SET status=3 " +
                                       "WHERE RID ='" + rid + "'";
                        int row5 = DbHelperSQL.ExecuteSql(sSQL5, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);

                    }
                }*/
            }
            else if (jObj["ltime"].ToString() == "")
            {
                result = "Leave";
                if (jObj["status"].ToString() == "1")
                {
                    distance = GetDistance(Convert.ToDouble(jObj["longitude"].ToString()), Convert.ToDouble(jObj["latitude"].ToString()), Convert.ToDouble(longitude), Convert.ToDouble(latitude));
                    if (distance >= radius || speed >= 3)
                    {
                        string sSQL3 = "SELECT COUNT(*) AS NO " +
                                       "FROM [WeChat].[dbo].[D_RoadList] " +
                                       "WHERE PLANRID ='" + rid + "'";
                        DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                        JObject jObj2 = JObject.Parse(str3);
                        int no = Convert.ToInt32(jObj2["NO"].ToString()) - 1;
                        string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET ltime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row4 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (step != no)
                        {
                            step = step + 1;
                            string sSQL6 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                           "SET step=" + step + " " +
                                           "WHERE RID ='" + rid + "'";
                            row6 = DbHelperSQL.ExecuteSql(sSQL6, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        }
                    }
                }
            }
            if (row1 == 1 && row4 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"" + result + "\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else if (row1 == 1 && row6 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"" + result + "\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else if (row1 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"success\",\"distance\":" + distance + "}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }


        [WebMethod]//司機確認完成任務
        public void driverSubmit(string sopenid, string rid, string confirmType)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT status,step " +
                          "FROM [WeChat].[dbo].[M_Plan] " +
                          "WHERE RID ='" + rid + "'";

            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            int step = Convert.ToInt32(jObj["step"].ToString());
            int step2 = Convert.ToInt32(jObj["step"].ToString()) + 1;
            string sSQL1 = "SELECT COUNT(*) AS NO " +
                           "FROM [WeChat].[dbo].[D_RoadList] " +
                           "WHERE PLANRID ='" + rid + "'";
            DataSet ds2 = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
            JObject jObj2 = JObject.Parse(str2);
            int no = Convert.ToInt32(jObj2["NO"].ToString()) - 1;
            int row = 0;
            int row2 = 0;

            if (jObj["status"].ToString() == "2" && step == no)
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET status='3' " +
                               "WHERE RID='" + rid + "'";
                row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
"SET ltime='" + nowTime + "'  " +
"WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1 && row2 == 1)
                {
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
        }

        [WebMethod]//司機確認出發和到達
        public void driverConfirmA(string sopenid, string rid, string confirmType)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT status,step " +
                          "FROM [WeChat].[dbo].[M_Plan] " +
                          "WHERE RID ='" + rid + "'";

            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            int step = Convert.ToInt32(jObj["step"].ToString());
            int step2 = Convert.ToInt32(jObj["step"].ToString()) + 1;
            string sSQL1 = "SELECT COUNT(*) AS NO " +
                           "FROM [WeChat].[dbo].[D_RoadList] " +
                           "WHERE PLANRID ='" + rid + "'";
            DataSet ds2 = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
            JObject jObj2 = JObject.Parse(str2);
            int no = Convert.ToInt32(jObj2["NO"].ToString()) - 1;
            int row = 0;
            int row2 = 0;
            int row3 = 0;

            if (jObj["status"].ToString() == "0")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET status='1' ,startTime='" + nowTime + "' " +
                               "WHERE RID='" + rid + "'";
                row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else
            {
                if (confirmType == "A")
                {
                    if (step == no)
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET atime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        string sSQL4 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                        "SET status='2' " +
                                        "WHERE RID='" + rid + "'";
                        row3 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1 && row3 == 1)
                        {
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }

                    }
                    else
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                      "SET atime='" + nowTime + "' " +
                                      "WHERE PlanRID ='" + rid + "' and sequence=" + jObj["step"].ToString();
                        row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1)
                        {
                            QYWeixin.QYWeixinHelper.SendList(rid);
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                    }
                }
                else if (confirmType == "L")
                {
                    string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                   "SET ltime='" + nowTime + "'  " +
                   "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                    row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string sSQL4 = "UPDATE [WeChat].[dbo].[M_Plan] " +
               "SET step='" + step2 + "' " +
               "WHERE RID='" + rid + "'";
                    row3 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }

                if (row2 == 1 & row3 == 1)
                {
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }

        }

        [WebMethod]//司機撤回出發和到達
        public void driverCancel(string sopenid, string rid, string cancelType)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT status,step " +
                          "FROM [WeChat].[dbo].[M_Plan] " +
                          "WHERE RID ='" + rid + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            int step = Convert.ToInt32(jObj["step"].ToString());
            int step2 = Convert.ToInt32(jObj["step"].ToString()) - 1;
            int row = 0;
            int row2 = 0;
            int row3 = 0;
            int row4 = 0;

            if (jObj["status"].ToString() == "1")
            {
                if (step == 0)
                {
                    if (cancelType == "A")
                    {
                        string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                       "SET status='0' " +
                                       "WHERE RID='" + rid + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1)
                        {
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }

                    }
                    else if (cancelType == "L")
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                           "SET atime=NULL " +
                                           "WHERE PlanRID ='" + rid + "' and sequence='" + step + "'";
                        row3 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row3 == 1)
                        {
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }

                    }

                }
                /*else if (step == 1 && cancelType == "L")//
                {
                    string sSQL3 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                   "SET step='" + step2 + "' " +
                                   "WHERE RID='" + rid + "'";
                    row3 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    if ( row3 == 1)
                    {
                        JObject jObj1 = JObject.Parse("{result:\"success\"}");
                        Context.Response.ContentType = "application/json; charset=utf-8";
                        Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                        Context.Response.Write(jObj1);
                        Context.Response.End();
                    }
                    else
                    {
                        JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                        Context.Response.ContentType = "application/json; charset=utf-8";
                        Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                        Context.Response.Write(jObj1);
                        Context.Response.End();
                    }
                }*/
                else if (step == 1 && cancelType == "A")//A=撤销到達
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                   "SET step='" + step2 + "' " +
                                   "WHERE RID='" + rid + "'";
                    row2 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET ltime=NULL " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + step2 + "'";
                    row3 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    if (row2 == 1 & row3 == 1)
                    {
                        JObject jObj1 = JObject.Parse("{result:\"success\"}");
                        Context.Response.ContentType = "application/json; charset=utf-8";
                        Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                        Context.Response.Write(jObj1);
                        Context.Response.Flush();
                        Context.Response.End();
                    }
                    else
                    {
                        JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                        Context.Response.ContentType = "application/json; charset=utf-8";
                        Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                        Context.Response.Write(jObj1);
                        Context.Response.Flush();
                        Context.Response.End();
                    }
                }
                else
                {
                    if (cancelType == "A")
                    {
                        string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                  "SET step='" + step2 + "' " +
                                  "WHERE RID='" + rid + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                           "SET ltime=NULL " +
                                           "WHERE PlanRID ='" + rid + "' and sequence='" + step2 + "'";
                        row3 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1 & row3 == 1)
                        {
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }

                    }
                    else if (cancelType == "L")
                    {
                        string sSQL2 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                           "SET atime=NULL " +
                                           "WHERE PlanRID ='" + rid + "' and sequence='" + step + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1)
                        {
                            JObject jObj1 = JObject.Parse("{result:\"success\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        else
                        {
                            JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                            Context.Response.ContentType = "application/json; charset=utf-8";
                            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                            Context.Response.Write(jObj1);
                            Context.Response.Flush();
                            Context.Response.End();
                        }

                    }

                }
            }
            else if (jObj["status"].ToString() == "2")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET status='1' " +
                               "WHERE RID='" + rid + "'";
                row2 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string sSQL3 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET step='" + step2 + "' " +
                               "WHERE RID='" + rid + "'";
                row3 = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                               "SET atime=NULL " +
                               "WHERE PlanRID ='" + rid + "' and sequence='" + step + "'";
                row4 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row2 == 1 && row3 == 1 && row4 == 1)
                {
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
        }

        [WebMethod]//司機取消乘客確認上車
        public void dirverConfirm2(string sopenid, string RID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "WHERE RID ='" + RID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() != "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime=NULL , status =1 " +
                               "WHERE RID='" + RID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.Flush();
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取任務列表
        public void GetMissionList(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            //string RID = Guid.NewGuid().ToString();
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            DateTime dt1 = DateTime.Now;
            dt1 = dt1.AddDays(-30);
            string stime = dt1.ToString("yyyy-MM-dd") + " 00:00";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "select P.planID AS id,P.carID ,L1.name AS location,L2.name AS destination, p.status  ,CONVERT(VARCHAR(19),P.time,120)AS time " +
                           "from [WeChat] . [dbo] .[M_Plan] AS P " +
                           "INNER JOIN [WeChat] .[dbo] .[T_Location] AS L1 " +
                           "on P.locationID=L1.locationID " +
                           "INNER JOIN [WeChat] .[dbo] .[T_Location] AS L2 " +
                           "on P.destinationID=L2.locationID " +
                          "WHERE P.driverID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "')   AND P.valid ='1' AND '" + stime + "' < P.Time  AND P.Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
                Context.Response.Flush();
                Context.Response.End();
            }

        }

        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]//獲取行程列表
        public void GetTravelList(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            //string RID = Guid.NewGuid().ToString();
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            DateTime dt1 = DateTime.Now;
            dt1 = dt1.AddDays(-30);
            string stime = dt1.ToString("yyyy-MM-dd") + " 00:00";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.PlanRID,P.carID, L.name AS location,L2.name AS destination,P.status, CONVERT(VARCHAR(19),PL.time,120)AS time " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[M_Plan] AS P  " +
                          "ON P.RID=PL.PlanRID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U  " +
                          "ON U.userID=P.driverID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L   " +
                          "ON L.locationID=PL.locationID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2   " +
                          "ON L2.locationID=PL.destinationID  " +
                          "WHERE PL.passengerID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "')   AND '" + stime + "' < P.Time  AND P.Time < '" + etime + "' ORDER BY P.Time Desc";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
                Context.Response.Flush();
                Context.Response.End();
            }
        }


        /// <summary>
        /// 調用騰訊地圖api計算預計到達時間
        /// </summary>
        public string duration(string latitude1, string longitude1, string latitude2, string longitude2)
        {
            string url = "https://apis.map.qq.com" + "/ws/distance/v1/?from=" + latitude2 + "," + longitude2 + "&" + "key=" + txmapkey + "&mode=driving&" + "to=" + latitude1 + "," + longitude1;
            url = url.Replace(" ", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.Default);
                string sReturn = sr.ReadToEnd().Trim();
                JObject jObj = JObject.Parse(sReturn);
                string result = jObj["result"].ToString();
                string elements = jObj["result"]["elements"].ToString();
                string duration = jObj["result"]["elements"][0]["duration"].ToString();
                return duration;
            }
        }

        /// <summary>
        /// 獲取預計到達站點時間
        /// </summary>
        [WebMethod]//獲取預計到達時間
        public void GetForecastTime(string latitude, string longitude, string destination)
        {
            DateTime now = DateTime.Now;
            string sSQL = "SELECT latitude,longitude " +
                          "FROM [WeChat].[dbo].[T_Location] " +
                          "WHERE name ='" + destination + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);
            string ForecastTime = duration(latitude, longitude, jObj["latitude"].ToString(), jObj["longitude"].ToString());
            DateTime t2 = now.AddSeconds(Convert.ToInt64(ForecastTime));
            string Time = t2.ToString("HH:mm");
            JObject jObj1 = JObject.Parse("{ForecastTime:\"" + Time + "\"}");

            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jObj1);
            Context.Response.Flush();
            Context.Response.End();
        }

        /// <summary>
        /// 獲取預計全程時間
        /// </summary>
        [WebMethod]//獲取預計全程時間
        public void GetLineTime(string location, string destination, string time)
        {
            DateTime now = Convert.ToDateTime(time);
            string sSQL = "SELECT latitude,longitude " +
                          "FROM [WeChat].[dbo].[T_Location] " +
                          "WHERE name ='" + destination + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);
            string sSQL2 = "SELECT latitude,longitude " +
              "FROM [WeChat].[dbo].[T_Location] " +
              "WHERE name ='" + location + "'";
            DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
            JObject jObj2 = JObject.Parse(str2);
            string ForecastTime = duration(jObj2["latitude"].ToString(), jObj2["longitude"].ToString(), jObj["latitude"].ToString(), jObj["longitude"].ToString());
            DateTime t2 = now.AddSeconds(Convert.ToInt64(ForecastTime));
            string Time = t2.ToString("HH:mm");
            JObject jObj1 = JObject.Parse("{ForecastTime:\"" + Time + "\"}");

            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jObj1);
            Context.Response.Flush();
            Context.Response.End();
        }

        /// <summary>
        /// 獲取全程路线時間   
        /// </summary>
        [WebMethod]//獲取離開和到達站點時間
        public void GetAllLineTime(string RID)
        {
            string sSQL = "SELECT r.atime AS '到達時間' ,r.ltime AS '離開時間' ,l.name AS '站點' " +
                         "FROM dbo.D_RoadList AS R " +
                         "LEFT JOIN  dbo.T_Location AS L " +
                         "ON R.locationID=L.locationID " +
                         "WHERE R.PlanRID='" + RID + "' order by r.sequence ";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            JArray jar = JArray.Parse(str);


            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jar);
            Context.Response.Flush();
            Context.Response.End();
        }

        /// <summary>
        /// 獲取全程路线時間
        /// </summary>
        [WebMethod]//獲取經緯度
        public void GetLocation(string location)
        {
            string sSQL = "SELECT latitude,longitude " +
                          "FROM dbo.T_Location " +
                          "where name ='" + location + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            JArray jar = JArray.Parse(str);
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jar);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]//記錄司機消息推送
        public void dirverMessage(string sopenid, string fromid, string planRID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);

            string sSQL2 = "INSERT INTO [WeChat].[dbo].[T_Message] (fromID,openID,creatdate,status)  " +
                           "VALUES ('" + fromid + "' ,  '" + openid + "' ,'" + nowTime + "','A')";
            int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            if (row == 1)
            {
                QYWeixin.QYWeixinHelper.SendToAdmin(planRID);
                JObject jObj1 = JObject.Parse("{result:\"success\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        /// <summary>
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]//獲取所有任務信息
        public void GetALLMissionList(string sopenid, string driverID, string stime, string etime, string CarID)/////////////////////////////////////////////////////////
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            stime = stime + " 00:00";
            etime = etime + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "select P.planID AS id,P.carID ,L1.name AS location,L2.name AS destination, p.status,U.username  ,CONVERT(VARCHAR(19),P.time,120)AS time " +
                          "from [WeChat] . [dbo] .[M_Plan] AS P " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L1 " +
                          "on P.locationID=L1.locationID " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L2 " +
                          "on P.destinationID=L2.locationID " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_UserMaster] AS U " +
                          "ON P.driverID =U.userid " +
                          "WHERE   P.valid ='1' AND '" + stime + "' < P.Time  AND P.Time < '" + etime + "'";
            if (driverID != "ALL")
            {
                sSQL = sSQL + " AND P.driverID = '" + driverID + "' ";
            }
            if (CarID != "ALL")//全部")
            {
                sSQL = sSQL + " AND P.carID = '" + CarID + "' ";
            }
            sSQL = sSQL + " order by P.time desc";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取任務詳情信息
        public void GetMissionDetail(string sopenid, string planID)/////////////////////////////////////////////////////////
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string sSQL1 = "SELECT TOP 1 P.RID, P.carID,L.name AS location,L2.name AS destination,CONVERT(VARCHAR(19),P.time,120)AS time,CONVERT(VARCHAR(19),P.startTime,120)AS startTime,U.username,U.name ,U.tel,P.status,P.step,R.locationID,L3.name AS nowlocation " +
                           "FROM [WeChat].[dbo].[M_Plan] AS P " +
                           "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                           "ON U.userID=P.driverID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                           "ON L.locationID=P.locationID " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                           "ON L2.locationID=P.destinationID " +
                           "LEFT JOIN [WeChat].[dbo].[D_RoadList] AS R " +
                           "ON P.RID=R.PlanRID AND P.step=R.sequence " +
                           "LEFT JOIN [WeChat].[dbo].[T_Location] AS L3 " +
                           "ON L3.locationID=R.locationID " +
                           "WHERE P.planID='" + planID + "' and valid ='1' ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JObject Job = JObject.Parse(str);
                // JArray Jar = JArray.Parse(str);
                //foreach (JObject items in Jar)
                //{//}
                string PlanRID = Job["RID"].ToString();
                string sSQL2 = "SELECT L.name AS location, R.sequence,  CONVERT(VARCHAR(19),R.atime,120)AS atime,CONVERT(VARCHAR(19),R.ltime,120)AS ltime " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE PlanRID ='" + PlanRID + "' ORDER BY sequence";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                if (str2 == "")
                {
                    JArray jar = JArray.Parse("[]");
                    Job.Add("roadlist", jar);
                }
                else
                {
                    string sssss = ConvertJson.ToJson(ds2.Tables[0]);
                    JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                    Job.Add("roadlist", jar);
                }
                string sSQL3 = "SELECT U.username, U.name , L.name AS location,L2.name AS destination, CONVERT(VARCHAR(19),PL.confirmTime,120)AS confirmTime,U.tel,PL.RID " +
                               "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                               "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                               "ON U.userID=PL.passengerID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                               "ON L.locationID=PL.locationID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                               "ON L2.locationID=PL.destinationID " +
                               "WHERE PL.PlanRID='" + PlanRID + "' ORDER BY L.name,L2.name,U.username ";
                DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                if (str3 == "")
                {
                    JArray jar2 = JArray.Parse("[]");
                    Job.Add("passengers", jar2);
                }
                else
                {
                    JArray jar2 = JArray.Parse(ConvertJson.ToJson(ds3.Tables[0]));
                    Job.Add("passengers", jar2);
                }
                Context.Response.Write(Job);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取所有司機信息
        public void GetAllDriverList(string sopenid)/////////////////////////////////////////////////////////
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT userid,username FROM dbo.T_UserMaster WHERE type ='driver'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        [WebMethod]//獲取所有車輛信息
        public void GetAllCarList(string sopenid)/////////////////////////////////////////////////////////
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT carID FROM dbo.T_Car";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.Flush();
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
                Context.Response.Flush();
                Context.Response.End();
            }
        }

        //地球半径，单位米
        private const double EARTH_RADIUS = 6378137;
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

        /// <summary>
        /// 根據wx.login()傳回的code獲取用戶的openid
        /// </summary>
        /// <param name="code">wx.login()傳回的code</param>
        /// <returns></returns>
        public string getOpenID(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + code + "&grant_type=authorization_code";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

            req.Method = "GET";

            using (WebResponse wr = req.GetResponse())
            {
                StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.Default);
                string sReturn = sr.ReadToEnd().Trim();
                JObject jObj = JObject.Parse(sReturn);
                string openid = jObj["openid"].ToString();
                return openid;
            }
        }

        /// <summary>
        /// 更新行程計劃狀態
        /// </summary>
        /// <param name="planId">行車計劃ID</param>
        /// <returns></returns>
        public void updateStatus(string planId)
        {
            string sSQL = "SELECT status " +
                          "FROM [WeChat] .[dbo] .[M_Plan] " +
                          "WHERE planID='" + planId + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jo = JObject.Parse(str);
            if (jo["status"].ToString() == "0")
            {
                string sSQL1 = "SELECT confirmTime " +
                               "FROM [WeChat] .[dbo] .[D_PassengerList] " +
                                "WHERE planID='" + planId + "'";
                DataSet ds2 = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = "1";
                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[0].ToString() == "")
                    {
                        str2 = "0";
                    }
                }
                if (str2 == "1")
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan]" +
                                   "SET status='" + str2 + "'" +
                                   "WHERE planID='" + planId + "'";
                    int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }
            }
        }

        [WebMethod]
        public void Test()
        {
            string sSQL1 = "SELECT passengerID " +
                           "FROM [WeChat] .[dbo] .[D_PassengerList] ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str1 = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JArray jar = JArray.Parse(ConvertJson.ToJson(ds.Tables[0]));
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jar);
            Context.Response.Flush();
            Context.Response.End();

        }

        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构  
            foreach (DataRow row in rows)
                tmp.Rows.Add(row.ItemArray);  // 将DataRow添加到DataTable中  
            return tmp;
        }

        public string newCode(string type)
        {
            string code = "";
            string nowTime = DateTime.Now.ToString("yyyyMMdd");
            System.Random a = new Random(System.DateTime.Now.Millisecond); // use System.DateTime.Now.Millisecond as seed
            int RandKey = a.Next(1000);
            code = nowTime;
            if (type == "PL")
            {
                code = "PL" + code + RandKey.ToString("0000");
            }
            else if (type == "P")
            {
                code = "0P" + code + RandKey.ToString("0000");
            }
            return code;

        }
    }
}
