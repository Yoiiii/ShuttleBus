using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System;
using LitJson;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Text;
using System.Web.Extensions;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Generic;
//using System.Net.Http;
using System.Drawing;
using System.Drawing.Imaging;

namespace WeChatWS.QYWeixin
{
    public class QYWeixinHelper
    {
        static string corpid = System.Configuration.ConfigurationManager.AppSettings["corpid"].ToString();
        static string corpsecret = System.Configuration.ConfigurationManager.AppSettings["secret"].ToString();
        static string messageSendURI = System.Configuration.ConfigurationManager.AppSettings["messageSendURL"].ToString();
        static string getAccessTokenUrl = System.Configuration.ConfigurationManager.AppSettings["getAccessTokenUrl"].ToString();
        //static string PictureName = System.Configuration.ConfigurationManager.AppSettings["PictureName"].ToString();
        static string _SQLServer = "10.6.0.126";
        static string _SQLLoginname = "WeChat_AU";
        static string _SQLPasswork = "20190402";
        static string _SQLDatabase = "WeChat";

        #region----企业微信發送方法/獲取基本參數//20190606
        /// <summary>
        /// 获取企业号的accessToken
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        static string GetQYAccessToken(string corpid, string corpsecret)
        {
            //string getAccessTokenUrl = System.Configuration.ConfigurationManager.AppSettings["getAccessTokenUrl"].ToString();
            string accessToken = "";

            string respText = "";

            //获取josn数据
            string url = string.Format(getAccessTokenUrl, corpid, corpsecret);
            Console.WriteLine(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream resStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.Default);
                respText = reader.ReadToEnd();
                resStream.Close();
            }

            try
            {
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
                //通过键access_token获取值
                accessToken = respDic["access_token"].ToString();
            }
            catch (Exception) { }

            return accessToken;
        }

        /// <summary>
        /// Post数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        static string UploadMultimedia(string PictureName)//20190906
        {
            string access = GetQYAccessToken(corpid, corpsecret);
            string result = "";
            string wxurl = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token=" + access + "&type=file";
            string filepath = System.Web.HttpContext.Current == null ? Path.GetFullPath("..") + "\\Images" + PictureName : System.Web.HttpContext.Current.Server.MapPath("Images") + PictureName;
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                byte[] responseArray = myWebClient.UploadFile(wxurl, "POST", filepath);
                result = System.Text.Encoding.Default.GetString(responseArray, 0, responseArray.Length);
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(result);
                //通过键access_token获取值
                result = respDic["media_id"].ToString();
            }
            catch (Exception ex)
            {
                result = "Error:" + ex.Message;
            }
            return result;
        }
        public static bool SendText(string UserId, string Party, string content, string accessToken)//20190905        
        {

            string postUrl = "";
            string param = "";
            string postResult = "";

            bool Result = false;//20191009

            postUrl = string.Format(messageSendURI, accessToken);

            CorpSendText paramData = new CorpSendText(content);
            paramData.touser = UserId;
            paramData.toparty = Party;
            param = JsonConvert.SerializeObject(paramData);
            if (paramData.touser != null || paramData.toparty != null)
                //20191009
                if (paramData.touser != null)
                {
                    try
                    {
                        postResult = PostWebRequest(postUrl, param, Encoding.UTF8);
                        Result = true;
                    }
                    catch
                    {
                        postResult = "發送失敗";
                    }
                    finally
                    {
                        CreateLog(paramData.touser, param, postResult);

                    }
                }
            return Result;

        }
        public static bool SendImage(string UserId, string PictureName, string accessToken)
        {
            string postUrl = "";
            string param = "";
            string postResult = "";
            string description = "";
            bool Result = false;//20191009
            accessToken = GetQYAccessToken(corpid, corpsecret);
            postUrl = string.Format(messageSendURI, accessToken);

            description = UploadMultimedia(PictureName);
            CorpSendImge paramData = new CorpSendImge(description);
            paramData.touser = UserId;
            param = JsonConvert.SerializeObject(paramData);
            //20191009
            if (paramData.touser != null)
            {
                try
                {
                    postResult = PostWebRequest(postUrl, param, Encoding.UTF8);
                    Result = true;
                }
                catch
                {
                    postResult = "發送失敗";
                }
                finally
                {
                    CreateLog(paramData.touser, param, postResult);

                }
            }
            return Result;

        }
        //private static void CreateLog(string strlog)
        //{
        //    string str1 = "QYWeixin_log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        //    //BS CS应用日志自适应
        //    string path = System.Web.HttpContext.Current == null ? Path.GetFullPath("..") + "\\temp\\" : System.Web.HttpContext.Current.Server.MapPath("temp");
        //    try
        //    {
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        path = Path.Combine(path, str1);
        //        StreamWriter sw = File.AppendText(path);
        //        sw.WriteLine(strlog);
        //        sw.Flush();
        //        sw.Close();

        //    }
        //    catch
        //    {
        //    }
        //}
        private static void CreateLog(string touser, string param, string postResult)
        {
            string sSQL = "INSERT INTO [T_Weixin_SMS_log]([touser],[Result],[Msg],[SendTime])"
                          + "VALUES ('" + touser + "','" + postResult + "',N'" + param + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") + "')";
            int row = DbHelperSQL.ExecuteSql(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
        }
        #endregion----企业微信發送方法/獲取基本參數
        public static string SendList(string rid)
        {
            string sSQL = "select A.passengerID,A.status,A.locationID,A.time,B.planID,B.carID,B.driverID,C.username as DriverName,D.atime as ArriveTime,D.ltime,B.step,D.sequence,E.name as ArriveName"
                            + " FROM D_PassengerList A LEFT JOIN  M_Plan B ON A.planID=B.planID"
                            + " LEFT JOIN  T_UserMaster C ON B.driverID=C.userid"
                            + " LEFT JOIN D_RoadList D ON D.planID=A.planID AND D.locationID=A.locationID"
                            + " LEFT JOIN T_Location E ON A.locationID= E.locationID"
                            + " where B.step=D.sequence AND B.RID='" + rid + "' and D.atime<>'' and A.IsSend is NULL"
                            + " ORDER BY A.time,planID"
                            + " update A"
                            + " set A.IsSend='1'"
                            + " from D_PassengerList A,M_Plan B,D_RoadList C"
                            + " where A.PlanRID='" + rid + "'"
                            + " and A.PlanRID=b.RID and B.step=C.sequence and A.locationID=C.locationID "
                            + " AND C.PlanRID=A.PlanRID and A.IsSend is NULL";
            string UserId = "";
            string CarId = "";
            string DriverName = "";
            string ArriveName = "";
            string ArriveTime = "";
            string content = "";//20190906
            string PictureName = "";//20190906
            string Result = "發送失敗";//20191009
            PictureName = "/QRCodeToPassenger.png";
            try
            {
                string access = GetQYAccessToken(corpid, corpsecret);
                DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                DataTable List = new DataTable();
                List = ds.Tables[0];


                for (int i = 0; i < List.Rows.Count; i++)
                {
                    UserId = List.Rows[i]["passengerID"].ToString();
                    CarId = List.Rows[i]["carID"].ToString();
                    DriverName = List.Rows[i]["DriverName"].ToString();
                    ArriveName = List.Rows[i]["ArriveName"].ToString();
                    ArriveTime = Convert.ToDateTime(List.Rows[i]["ArriveTime"]).ToString("yyyy-MM-dd HH:mm");
                    content = "穿梭車到達提醒\n-----------------------\n車牌：" + CarId + " \n司機：" + DriverName + "\n到達站點：" + ArriveName + "\n到達時間：" + ArriveTime + "\n車輛已到達附近，請做好上車準備\n-----------------------\n請點擊下圖，長按識別圖中二維碼，進入小程序";//20190906
                    if (SendText(UserId, "", content, access) && SendImage(UserId, PictureName, access)) //20191009
                    {
                        Result = "發送成功";
                    }

                }
            }
            catch (Exception e)
            {

            }
            return Result;

        }
        public static string SendToDriver(string rid)
        {
            string sSQL = "select planID,driverID,carID,name,time,stroke"
                            + " FROM v_CarRoute"
                            + " where RID='" + rid + "'";
            string UserId = "";
            string CarId = "";
            string PlanID = "";
            string DriverName = "";
            string Route = "";
            string Time = "";
            string content = "";
            string PictureName = "";//20190906
            string Result = "發送失敗";//20191009
            PictureName = "/QRCodeToPassenger.png";
            try
            {
                string access = GetQYAccessToken(corpid, corpsecret);
                DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                DataTable List = new DataTable();
                List = ds.Tables[0];


                for (int i = 0; i < List.Rows.Count; i++)
                {
                    UserId = List.Rows[i]["driverID"].ToString();
                    PlanID = List.Rows[i]["planID"].ToString();
                    DriverName = List.Rows[i]["name"].ToString();
                    CarId = List.Rows[i]["carID"].ToString();
                    Route = List.Rows[i]["stroke"].ToString();
                    Route = Route.Replace(",", ">").ToString().Trim();
                    Time = Convert.ToDateTime(List.Rows[i]["time"]).ToString("yyyy-MM-dd HH:mm");
                    content = "穿梭車派車任務提醒\n-----------------------\n派車單號：" + PlanID + "\n車牌：" + CarId + " \n司機：" + DriverName + "\n路線：" + Route + "\n出發時間：" + Time + "\n-----------------------\n請點擊下圖，長按識別圖中二維碼，進入小程序，進行查看";
                    //content = "穿梭車派車任務提醒\n-----------------------\n派車單號：" + PlanID + "\n車牌：" + CarId + " \n司機：" + DriverName + "\n路線：" + Route + "\n出發時間：" + Time ;
                    if (SendText(UserId, "", content, access) && SendImage(UserId, PictureName, access)) //20191009
                    {
                        Result = "發送成功";
                    }

                }

            }
            catch (Exception e)
            {

            }
            return Result;
        }
        public static string SendToAdmin(string rid)
        {
            string sSQL = "select planID,driverID,carID,name,time,stroke"
                            + " FROM v_CarRoute"
                            + " where RID='" + rid + "'";
            string UserId = "";
            string CarId = "";
            string PlanID = "";
            string DriverName = "";
            string Route = "";
            string Time = "";
            string content = "";
            string Party = "13";
            string Result = "發送失敗";//20191009
            try
            {
                string access = GetQYAccessToken(corpid, corpsecret);
                DataSet ds = DbHelperSQL.Query(sSQL, _SQLServer, _SQLLoginname, _SQLPasswork, _SQLDatabase);
                DataTable List = new DataTable();
                List = ds.Tables[0];


                for (int i = 0; i < List.Rows.Count; i++)
                {
                    UserId = List.Rows[i]["driverID"].ToString();
                    PlanID = List.Rows[i]["planID"].ToString();
                    DriverName = List.Rows[i]["name"].ToString();
                    CarId = List.Rows[i]["carID"].ToString();
                    Route = List.Rows[i]["stroke"].ToString();
                    Route = Route.Replace(",", ">").ToString().Trim();
                    Time = Convert.ToDateTime(List.Rows[i]["time"]).ToString("yyyy-MM-dd HH:mm");
                    content = "穿梭車派車任務提醒\n-----------------------\n派車單號：" + PlanID + "\n車牌：" + CarId + " \n司機：" + DriverName + "\n路線：" + Route + "\n出發時間：" + Time + "\n-----------------------\n司機已確認";
                   
                    if (SendText("", Party, content, access)) //20191009
                    {
                        Result = "發送成功";
                    }


                }

            }
            catch (Exception e)
            {

            }
            return Result;


        }
    }
}
