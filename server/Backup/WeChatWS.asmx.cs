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


namespace WeChatWS
{
    /// <summary>
    /// Summary description for GPSService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
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

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 小程序調用登錄獲取登錄態
        /// </summary>
        /// <param name="code">wx.login()傳回的code</param>
        [WebMethod]
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
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj);
                Context.Response.End();
            }
        }

        /// <summary>
        /// 初次使用用户绑定微信
        /// </summary>
        /// <param name="userID">工号</param>
        /// <param name="tel">联系电话</param>
        /// <param name="code">微信登录code</param>
        [WebMethod]
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
                Context.Response.End();
            }
        }
        /// <summary>
        /// 根据openid获取用户信息
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
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
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse(str);
                    Context.Response.Write(jObj1);
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
        [WebMethod]
        public void GetTravel(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT PL.RID, PL.PlanRID,L.name AS location,L2.name AS destination,PL.confirmTime, CONVERT(VARCHAR(19),PL.time,120)AS time,P.carID,U.username ,U.tel,P.status,P.step,PL.passengerID,P.latitude,P.longitude,P.gpsTime " +
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
                           "AND '" + stime + "' < PL.Time ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
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
                string sSQL2 = "SELECT L.name AS location, R.sequence, R.atime,R.ltime " +
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
                string sSQL3 = "SELECT U.username ,L.name AS location,L2.name AS destination,PL.confirmTime " +
                               "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                               "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                               "ON U.userID=PL.passengerID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                               "ON L.locationID=PL.locationID " +
                               "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                               "ON L2.locationID=PL.destinationID " +
                               "WHERE PL.PlanRID='" + PlanRID + "' and PL.passengerID != '" + passengerID + "' ORDER BY L.name,L2.name,U.username ";
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
                    string ForecastTime = duration(jObj2["latitude"].ToString(), jObj2["longitude"].ToString(), Job["latitude"].ToString(), Job["longitude"].ToString());
                    DateTime t2 = t1.AddSeconds(Convert.ToInt64(ForecastTime));
                    string Time = t2.ToString("yyyy-MM-dd HH:mm:ss");
                    Job.Add("ForecastTime", Time);

                }
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(Job);
                Context.Response.End();
            }
        }

        /// <summary>
        /// 获取当前任务
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetMission(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT P.RID, P.carID,L.name AS location,L2.name AS destination,P.time,U.username ,U.tel,P.status,P.step,R.locationID,L3.name " +
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
                           "WHERE P.driverID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "') AND P.status!= 3  AND '" + stime + "' < P.Time ";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JObject Job = JObject.Parse(str);
                // JArray Jar = JArray.Parse(str);
                //foreach (JObject items in Jar)
                //{//}
                string PlanRID = Job["RID"].ToString();
                string sSQL2 = "SELECT L.name AS location, R.sequence, R.atime,R.ltime " +
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
                string sSQL3 = "SELECT U.username ,L.name AS location,L2.name AS destination,PL.confirmTime,U.tel,PL.RID " +
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
                Context.Response.End();
            }
        }

        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }



        /// <summary>
        /// 乘客撤销上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]
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
                               "SET confirmTime=NULL " +
                               "WHERE RID='" + plsID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }


        [WebMethod]
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
                               "SET confirmTime='" + nowTime + "'" +
                               "WHERE RID='" + RID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }

        [WebMethod]
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
            if (jObj["atime"].ToString() == "")
            {
                double distance = GetDistance(Convert.ToDouble(jObj["longitude"].ToString()), Convert.ToDouble(jObj["latitude"].ToString()), Convert.ToDouble(longitude), Convert.ToDouble(latitude));
                //计算经纬度、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、、
                if (distance <= 300)
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
            }
            if (row1 == 1 && row4 == 1)
            {
                JObject jObj1 = JObject.Parse("{result:\"success\"}");
                jObj1.Add("arrival", jObj["name"].ToString());
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else if (row1 == 1)
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
        }

        [WebMethod]
        public void driverSubmit(string sopenid, string rid,string confirmType)
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
            }
        }

        [WebMethod]
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
                               "SET status='1' " +
                               "WHERE RID='" + rid + "'";
                row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
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

                    }
                    else
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                      "SET atime='" + nowTime + "' " +
                                      "WHERE PlanRID ='" + rid + "' and sequence=" + jObj["step"].ToString();
                        row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                        if (row2 == 1)
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
            }

        }




        [WebMethod]
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
            }
        }


        [WebMethod]
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
                               "SET confirmTime=NULL " +
                               "WHERE RID='" + RID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }

        [WebMethod]
        public void dirverConfirm3(string sopenid, string rid)
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
                if (step == no)
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET status='2' " +
                               "WHERE RID='" + rid + "'";
                    row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }
                else
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                   "SET status='1' ,step='" + step2 + "'" +
                                   "WHERE RID='" + rid + "'";
                    row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }

                if (row == 1)
                {
                    if (step != no)
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET ltime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }

                    if (jObj["step"].ToString() != "0")
                    {
                        string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                      "SET atime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row3 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }

                    if (row2 == 1 || row3 == 1)
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
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else
            {
                if (step == no)
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                               "SET status='2' " +
                               "WHERE RID='" + rid + "'";
                    row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }
                else
                {
                    string sSQL2 = "UPDATE [WeChat].[dbo].[M_Plan] " +
                                   "SET status='1' ,step='" + step2 + "'" +
                                   "WHERE RID='" + rid + "'";
                    row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                }
                if (row == 1)
                {

                    if (step != no)
                    {
                        string sSQL3 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                       "SET ltime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row2 = DbHelperSQL.ExecuteSql(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }
                    if (jObj["step"].ToString() != "0")
                    {
                        string sSQL4 = "UPDATE [WeChat].[dbo].[D_RoadList] " +
                                      "SET atime='" + nowTime + "' " +
                                       "WHERE PlanRID ='" + rid + "' and sequence='" + jObj["step"].ToString() + "'";
                        row3 = DbHelperSQL.ExecuteSql(sSQL4, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    }

                    if (row2 == 1 || row3 == 1)
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

                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
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
        [WebMethod]
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
            Context.Response.End();
        }
        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]
        public void GetTravelList(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            //string RID = Guid.NewGuid().ToString();
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            DateTime dt1 = DateTime.Now;
            dt1 = dt1.AddDays(30);
            string etime = dt1.ToString("yyyy-MM-dd") + " 23:59";
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
                          "WHERE PL.passengerID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "')   AND '" + stime + "' < P.Time  AND P.Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                Context.Response.Write(Jar);
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


        //////////////////////////////////////////////////////////

        /// <summary>
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetTravelList2(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT PL.PlanRID,P.carID, L.name AS location,L2.name AS destination,P.status, PL.time " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[M_Plan] AS P  " +
                          "ON P.RID=PL.PlanRID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U  " +
                          "ON U.userID=P.driverID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L   " +
                          "ON L.locationID=PL.locationID  " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2   " +
                          "ON L2.locationID=PL.destinationID  " +
                          "WHERE PL.passengerID=(SELECT userID FROM [WeChat].[dbo].[T_UserMaster] WHERE openID='" + openid + "')   AND '" + stime + "' < P.Time  AND P.Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                /*foreach (JObject items in Jar)
                {
                    string planID = items["id"].ToString();
                    string sSQL2 = "SELECT R.id,R.sequence,L.name AS location, R.atime,R.ltime " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE planID ='" + planID + "' ORDER BY sequence";
                    DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                    if (str2 == "")
                    {
                        JObject job2 = JObject.Parse("{roadlist:\"\"}");
                    }
                    else
                    {
                        JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                        items.Add("roadlist", jar);
                    }
                }*/
                Context.Response.Write(Jar);
                Context.Response.End();
            }



            /*DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            JArray Jar = JArray.Parse(ConvertJson.ToJson(ds.Tables[0]));
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
           if (str == "")
           {
               JObject jObj1 = JObject.Parse("{result:\"null\"}");
               Context.Response.Write(jObj1);
               Context.Response.End();
           }
           else
           {
               //JObject jObj1 = JObject.Parse(str);

               foreach (JObject items in Jar)
               {
               string sSQL2 = "SELECT R.id,R.sequence,L.name AS location, R.atime,R.ltime " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE planID ='" +jObj1["id"].ToString() + "' ORDER BY sequence";
               DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
               string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
               if (str2 == "")
               {
                   JObject job2 = JObject.Parse("{roadlist:\"\"}");
               }
               else
               {
                   JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                   jObj1.Add("roadlist", jar);
               }
               }
               Context.Response.Write(jObj1);
               Context.Response.End();
           }
           DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
           string str = ConvertJson.ToJson(ds.Tables[0]);
           if (str == ""|| str=="]")
           {
               JObject jObj1 = JObject.Parse("{result:\"null\"}");
               Context.Response.Write(jObj1);
               Context.Response.End();
           }
           else
           {
               JArray Jar = JArray.Parse(str);
               //string planID = jObj1["PlanID"].ToString();
               //string sSQL2 = "select COUNT(*)as num from [WeChat].[dbo].PassengerList where PlanID='" + planID + "'";
               //DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
               //JObject jObj2 = JObject.Parse(ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0])));
               //jObj1.Add("Num", jObj2["num"].ToString());
               Context.Response.Write(Jar);
               Context.Response.End();
           }*/
        }



        /// <summary>
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetMissionList(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = " select P.planID AS id,P.carID ,L1.name AS location,L2.name AS destination, p.status ,P.type ,P.time " +
                           "from [WeChat] . [dbo] .[M_Plan] AS P " +
                           "INNER JOIN [WeChat] .[dbo] .[T_Location] AS L1 " +
                           "on P.locationID=L1.locationID " +
                           "INNER JOIN [WeChat] .[dbo] .[T_Location] AS L2 " +
                           "on P.destinationID=L2.locationID " +
                           "where driverID =(select userid from [WeChat].[dbo].[T_UserMaster] where openID='" + openid + "') and '" + stime + "' < P.Time and P.Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "" || str == "]")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                foreach (JObject items in Jar)
                {
                    string planID = items["id"].ToString();
                    string sSQL2 = "SELECT R.id,R.sequence,L.name AS location, R.atime,R.ltime " +
                              "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                              "on R.locationID=L.locationID  " +
                              "WHERE planID ='" + planID + "' ORDER BY sequence";
                    DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                    if (str2 == "")
                    {
                        JObject job2 = JObject.Parse("{roadlist:\"\"}");
                    }
                    else
                    {
                        JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                        items.Add("roadlist", jar);
                    }

                    string sSQL3 = "select PL.passengerID,PL.id AS plsid,U.username ,u.tel,pl.confirmTime,l.name " +
                                   "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                                   "LEFT JOIN [WeChat].[dbo] .[T_UserMaster] AS U " +
                                   "ON U.userID =PL.passengerID " +
                                   "INNER JOIN [WeChat] .[dbo] .[T_Location] AS L " +
                                   "on PL.locationID=L.locationID " +
                                   "where planID ='" + planID + "'";
                    DataSet ds3 = DbHelperSQL.Query(sSQL3, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str3 = ConvertJson.ToJObject(ConvertJson.ToJson(ds3.Tables[0]));
                    if (str3 == "")
                    {
                        JObject job3 = JObject.Parse("{passengerlist:\"\"}");
                    }
                    else
                    {
                        JArray jar2 = JArray.Parse(ConvertJson.ToJson(ds3.Tables[0]));
                        items.Add("passengerlist", jar2);
                    }
                }
                Context.Response.Write(Jar);
                Context.Response.End();
            }

        }

        /// <summary>
        /// 司机确认到站
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]
        public void driverConfirm(string sopenid, string plID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID,U.userID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.OPENID ='" + openid + "' " +
                          "WHERE PL.PLANDI ='" + plID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "'" +
                               "WHERE planid='" + plID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }
        /// <summary>
        /// 司机确认乘客上车
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="plsID">行車計劃id</param>
        [WebMethod]
        public void driverConfirm3(string plid, string passengerID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID,U.userID,PL.ID" +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "WHERE pl.planid ='" + plid + "' AND PL.passengerID='" + passengerID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "'" +
                               "WHERE id='" + jObj["id"].ToString() + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
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
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }


        /// <summary>  
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetTravelInfo(string sopenid, string planid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.id,PL.planID,P.carID,L1.name AS location,L2.name AS destination,PL.time,P.status,P.type,L3.name AS start,L4.name AS final ,PL.confirmTime " +
                          "FROM [WeChat] .[dbo] .[D_PassengerList]  AS PL  " +
                          "LEFT JOIN [WeChat] . [dbo] .[M_Plan] AS P  " +
                          "ON P.planID=PL.planID " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L1  " +
                          "on PL.locationID=L1.locationID  " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L2 " +
                          "on PL.destinationID=L2.locationID  " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L3  " +
                          "on P.locationID=L3.locationID  " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L4 " +
                          "on P.destinationID=L4.locationID  " +
                          "WHERE PL.planID='" + planid + "' and PL.passengerID=(SELECT USERID FROM [WeChat] .[dbo] .[T_UserMaster] WHERE openID='" + openid + "') ";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse(str);
                //foreach (JObject items in Jar)
                //{
                string sSQL2 = "SELECT R.id,R.sequence,L.name AS location, R.atime,R.ltime " +
                               "FROM [WeChat].[dbo].[D_RoadList] AS R " +
                               "LEFT JOIN [WeChat] .[dbo] .[T_Location] AS L  " +
                               "on R.locationID=L.locationID  " +
                               "WHERE planID ='" + planid + "' ORDER BY sequence";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                if (str2 == "")
                {
                    JObject job2 = JObject.Parse("{roadlist:\"\"}");
                }
                else
                {
                    JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                    jObj1.Add("roadlist", jar);
                }
                //}
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }

        /// <summary>
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetPlan(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT P.CarID,P.DriverID,P.Status,P.Time,P.Type,C.SeatNum,L.name,L2.name AS destination,P.PlanID " +
                          "FROM [WeChat].[dbo].[M_Plan] AS P " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.OPENID='" + openid + "'AND P.DRIVERID =U.USERID " +
                          "LEFT JOIN [WeChat].[dbo].[Car] AS C " +
                          "ON C.CARID =P.CARID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                          "ON L.LocationID=P.LocationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location]  AS L2 " +
                          "ON P.destinationID=L2.locationID " +
                          "WHERE '" + stime + "' < Time and Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse(str);
                string planID = jObj1["PlanID"].ToString();
                string sSQL2 = "select COUNT(*)as num from [WeChat].[dbo].PassengerList where PlanID='" + planID + "'";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                JObject jObj2 = JObject.Parse(ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0])));
                jObj1.Add("Num", jObj2["num"].ToString());
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }


        /// <summary>
        /// 获取行车计划
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetPlanList(string sopenid)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT ROW_NUMBER() OVER (ORDER BY P.Time ASC) AS NO,P.CarID,P.DriverID,P.Status,P.Time,P.Type,C.SeatNum,L.name,L2.name AS destination,P.PlanID,(select COUNT(*)as num from [WeChat].[dbo].PassengerList where PlanID=P.PlanID) AS mun " +
                          "FROM [WeChat].[dbo].[M_Plan] AS P " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.OPENID='" + openid + "'AND P.DRIVERID =U.USERID " +
                          "LEFT JOIN [WeChat].[dbo].[Car] AS C " +
                          "ON C.CARID =P.CARID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                          "ON L.LocationID=P.LocationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                          "ON L2.LocationID=P.destinationID " +
                          "WHERE '" + stime + "' < Time and Time < '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJson(ds.Tables[0]);
            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JArray Jar = JArray.Parse(str);
                foreach (JObject items in Jar)
                {
                    string planID = items["PlanID"].ToString();
                    string sSQL2 = "SELECT  PL.time,PL.confirmTime,PL.id,U.username ,U.tel,L.name AS location,L2.name AS destination,PL.remark " +
                                  "FROM PassengerList AS pl " +
                                  "LEFT JOIN [WeChat].[dbo] .[T_UserMaster] AS U " +
                                  "ON U.userID =PL.passengerID " +
                                  "LEFT JOIN [WeChat].[dbo].[M_Plan]  AS P " +
                                  "ON PL.planID =P.planID " +
                                  "LEFT JOIN [WeChat] .[dbo] .[T_Location]  AS L " +
                                  "ON L.locationID=PL.locationID " +
                                  "LEFT JOIN [WeChat] .[dbo] .[T_Location]  AS L2 " +
                                  "ON L2.locationID=PL.destinationID " +
                                  "where PL.planID ='" + planID + "'";
                    DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                    string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                    if (str2 == "")
                    {
                        JObject job2 = JObject.Parse("{passengerList:\"\"}");
                    }
                    else
                    {
                        JArray jar = JArray.Parse(ConvertJson.ToJson(ds2.Tables[0]));
                        items.Add("passengerList", jar);
                    }
                }
                Context.Response.Write(Jar);
                Context.Response.End();
            }
        }


        /// <summary>
        /// 乘客获取乘坐車輛信息
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetCarInfo(string sopenid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT P.planID, P.carID,P.status ,L.name AS location,L2.name AS destination,U2.name,P.time,PL.confirmTime " +
                          "FROM [WeChat].[dbo].[D_PassengerList]AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.OPENID='" + openid + "' " +
                          "LEFT JOIN [WeChat].[dbo].[M_Plan]  AS P " +
                          "ON P.Time <= '" + etime + "' and P.Time >='" + nowTime + "' " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location]  AS L " +
                          "ON L.LocationID=P.LocationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location]  AS L2 " +
                          "ON P.destinationID=L2.locationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U2 " +
                          "ON p.driverID=u2.userID " +
                          "WHERE  P.planID=PL.planID AND U.userID=PL.passengerID  ";//P.status!=3
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse(str);
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }

        /// <summary>
        /// 乘客获取乘坐車輛信息
        /// </summary>
        /// <param name="sopenid">加密的openid</param>
        [WebMethod]
        public void GetCarInfo2(string sopenid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT ROW_NUMBER() OVER (ORDER BY P.Time ASC) AS NO ,P.carID,U2.name,L.name AS location,L2.name AS destination, p.time,pl.confirmTime,pl.id " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.openID ='" + openid + "' " +
                          "LEFT JOIN [WeChat].[dbo].[M_Plan] AS P " +
                          "ON P.planID=PL.planID " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U2 " +
                          "ON U2.userID=P.driverID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                          "ON L.locationID=PL.locationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                          "ON L2.locationID=PL.destinationID " +
                          "WHERE U.userID=PL.passengerID and P.time> '" + stime + "' and P.time< '" + etime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JArray jar = JArray.Parse(ConvertJson.ToJson(ds.Tables[0]));
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jar);
                Context.Response.End();
            }
        }

        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">乘客的加密的openid</param>
        /// <param name="planID">行車計劃id</param>
        [WebMethod]
        public void passengerConfirm6(string sopenid, string planID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT PL.confirmTime,PL.PlanID,U.userID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] AS PL " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON U.OPENID ='" + openid + "' " +
                          "WHERE planID ='" + planID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL2 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "'" +
                               "WHERE passengerID='" + jObj["userID"].ToString() + "'AND PlanID='" + jObj["PlanID"].ToString() + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(planID);
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }


        /// <summary>
        /// 司機更新行程
        /// </summary>
        /// <param name="sopenid">乘客的加密openid</param>
        /// <param name="planID">行程計劃id</param>
        [WebMethod]
        public void submit(string sopenid, string planID)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT status " +
                          "FROM [WeChat].[dbo].[M_Plan] " +
                          "WHERE PlanID ='" + planID + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["status"].ToString() == "1")
            {
                string sSQL1 = "UPDATE [WeChat].[dbo].[M_Plan]" +
                               "SET status='2'" +
                               "WHERE PlanID ='" + planID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    //updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else if (jObj["status"].ToString() == "2")
            {
                string sSQL1 = "UPDATE [WeChat].[dbo].[M_Plan]" +
                               "SET status='3'" +
                               "WHERE PlanID ='" + planID + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    //updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
        }

        /// <summary>
        /// 獲取該車輛乘客列表
        /// </summary>
        /// <param name="sopenid">司機的加密openid</param>
        [WebMethod]
        public void getPassenger(string sopenid)
        {

            string stime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT P.planID FROM [WeChat].[dbo].[M_Plan] AS P LEFT JOIN [WeChat].[dbo] .[T_UserMaster] AS U ON U.userID =P.driverID WHERE U.openID='" + openid + "' AND '" + etime + "'> P.time AND P.time >'" + stime + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string strja = "";
            JArray ja = new JArray();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string planId = row[0].ToString();
                string sSQL2 = "SELECT PL.time,PL.confirmTime,PL.id,U.username ,U.tel,L.name " +
                          "FROM PassengerList AS pl " +
                          "LEFT JOIN [WeChat].[dbo] .[T_UserMaster] AS U " +
                          "ON U.userID =PL.passengerID " +
                          "LEFT JOIN [WeChat].[dbo].[M_Plan]  AS P " +
                          "ON PL.planID =P.planID " +
                          "LEFT JOIN [WeChat] .[dbo] .[T_Location]  AS L " +
                          "ON L.locationID=P.locationID " +
                          "where PL.planID ='" + planId + "'";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                strja = strja + ConvertJson.ToJson(ds2.Tables[0]);
            }
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Context.Response.Write(ja);
            Context.Response.End();
        }

        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">加密的乘客openid</param>
        /// <param name="plid">乘客乘車計劃id</param>
        [WebMethod]
        public void driverConfirm1(string sopenid, string plid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT confirmTime,PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] " +
                          "WHERE id ='" + plid + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL1 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "'" +
                               "WHERE id='" + plid + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }


        /// <summary>
        /// 乘客確認上車
        /// </summary>
        /// <param name="sopenid">加密的乘客openid</param>
        /// <param name="plid">乘客乘車計劃id</param>
        [WebMethod]
        public void driverConfirm2(string sopenid, string plid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL = "SELECT confirmTime,PlanID " +
                          "FROM [WeChat].[dbo].[D_PassengerList] " +
                          "WHERE id ='" + plid + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            JObject jObj = JObject.Parse(str);//{"confirmTime": "", "PlanID": "XJslvuSiwXKAQseG","userID": "S91014"}
            if (jObj["confirmTime"].ToString() == "")
            {
                string sSQL1 = "UPDATE [WeChat].[dbo].[D_PassengerList]" +
                               "SET confirmTime='" + nowTime + "',REMARK= N'不乘車'" +
                               "WHERE id='" + plid + "'";
                int row = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    updateStatus(jObj["PlanID"].ToString());
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"faill\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }

        [WebMethod]
        public int UpdateGPSData(string sSID, string sRemarks, string sqlType)
        {
            string sSQL = "EXEC usp_UpdateGPSData @sid,@Remarks,@sqlType ";
            SqlParameter[] cmdParms = {
                    new SqlParameter("@SID", SqlDbType.VarChar),  
                new SqlParameter("@Remarks", SqlDbType.Text),
                new SqlParameter("@sqlType", SqlDbType.VarChar),
            };

            cmdParms[0].Value = sSID;
            cmdParms[1].Value = sRemarks;
            cmdParms[2].Value = sqlType;

            int iR = DbHelperSQL.ExecuteSql(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase, cmdParms);

            return iR;
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

        /// <summary>
        /// 新增用戶
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        [WebMethod]
        public void AddUser(string userID, string name, string departmentID, string passWord, string tel, string type)
        {
            string sSQL = "select UserID,tel from [WeChat].[dbo].[T_UserMaster] " +
                          "where UserID='" + userID + "' and tel='" + tel + "'";
            DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str = ConvertJson.ToJObject(ConvertJson.ToJson(ds.Tables[0]));
            if (str == "")
            {
                string sSQL1 = "INSERT INTO [WeChat].[dbo].[T_UserMaster] (userID,name,departmentID,passWord,tel,type) " +
                               "VALUES ('" + userID + "','" + name + "','" + departmentID + "','" + passWord + "','" + tel + "','" + type + "')";
                int row = DbHelperSQL.ExecuteSql(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 1)
                {
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
        }
        /// <summary>
        /// 扫描获取车辆信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        [WebMethod]
        public void GetCarInfoByCarID(string carID, string sopenid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT P.planID,U.username ,L.name AS location,L2.name AS destination,p.status,p.type,p.time,p.carID " +
                          "FROM [WeChat].[dbo].[M_Plan] AS P " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L " +
                          "ON  P.locationID = L.locationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_Location] AS L2 " +
                          "ON  P.destinationID=L2.locationID " +
                          "LEFT JOIN [WeChat].[dbo].[T_UserMaster] AS U " +
                          "ON p.driverID=U.userID " +
                          "where carID='" + carID + "' and time <'" + etime + "' and time >'" + nowTime + "'";
            DataSet ds1 = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str1 = ConvertJson.ToJObject(ConvertJson.ToJson(ds1.Tables[0]));
            if (str1 == "")
            {
                JObject jObj1 = JObject.Parse("{result:\"null\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
            else
            {
                JObject jObj1 = JObject.Parse(str1);
                string sSQL2 = "SELECT PL.passengerID,PL.confirmTime " +
                              "FROM [WeChat] .[dbo] .[D_PassengerList]  AS PL " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_UserMaster]  AS U " +
                              "ON U.userID=PL.passengerID " +
                              "WHERE planID='" + jObj1["planID"].ToString() + "' AND U.openID ='" + openid + "'";
                DataSet ds2 = DbHelperSQL.Query(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                string str2 = ConvertJson.ToJObject(ConvertJson.ToJson(ds2.Tables[0]));
                if (str2 == "")
                {
                    jObj1.Add("check", "fail");
                }
                else
                {
                    jObj1.Add("check", "success");
                    jObj1.Add("confirmTime", JObject.Parse(str2)["confirmTime"].ToString());
                }
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
            }
        }
        /// <summary>
        /// 扫描上车
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        [WebMethod]
        public void IntoCarByCarID(string planID, string sopenid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string etime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            string openid = Cryptography.Decrypt(sopenid);
            string sSQL1 = "SELECT PL.passengerID " +
                              "FROM [WeChat] .[dbo] .[D_PassengerList]  AS PL " +
                              "LEFT JOIN [WeChat] .[dbo] .[T_UserMaster]  AS U " +
                              "ON U.userID=PL.passengerID " +
                              "WHERE planID='" + planID + "' AND U.openID ='" + openid + "'"; ;
            DataSet ds1 = DbHelperSQL.Query(sSQL1, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
            string str1 = ConvertJson.ToJObject(ConvertJson.ToJson(ds1.Tables[0]));
            if (str1 == "")
            {
                string plid = newCode("PL").ToString();
                string sSQL2 = "INSERT INTO [WeChat].[dbo].[D_PassengerList]  " +
                               "VALUES ('" + plid + "', (SELECT userID FROM [WeChat] .[dbo] .[T_UserMaster] WHERE openID ='" + openid + "') , '" + planID + "', null, '" + nowTime + "',null ," +
                               "(SELECT locationID FROM [WeChat] .[dbo].[M_Plan] WHERE planID = '" + planID + "') , " +
                               "(SELECT destinationID FROM [WeChat] .[dbo].[M_Plan] WHERE planID = '" + planID + "'))";
                //"INSERT INTO [WeChat].[dbo].[D_PassengerList]  VALUES "+"('"+plid+"', (SELECT userID FROM [WeChat] .[dbo] .[T_UserMaster] WHERE openID ='" + openid + "') , '" + planID + "',null,'" + nowTime + "',null)";
                int row = DbHelperSQL.ExecuteSql(sSQL2, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                if (row == 0)
                {
                    JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
                else
                {
                    updateStatus(planID);
                    JObject jObj1 = JObject.Parse("{result:\"success\"}");
                    Context.Response.ContentType = "application/json; charset=utf-8";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jObj1);
                    Context.Response.End();
                }
            }
            else
            {
                JObject jObj1 = JObject.Parse("{result:\"fail\"}");
                Context.Response.ContentType = "application/json; charset=utf-8";
                Context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                Context.Response.Write(jObj1);
                Context.Response.End();
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
