using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Login : System.Web.UI.Page
{
    public String LoginUserID;
    public String Lang;
    protected void Page_Load(object sender, EventArgs e)
    {
        getCookieNameID("NameID");
        getCookieLang("Lang");
    }

    public string getCookieNameID(string strName)
    {
        HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
        if (Cookie != null)
        {
            LoginUserID = Cookie.Value.ToString();
            return LoginUserID;

        }
        else
        {
            return null;
        }
    }

    public string getCookieLang(string strName)
    {
        HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
        if (Cookie != null)
        {
            Lang = Cookie.Value.ToString();
            return Lang;

        }
        else
        {
            return null;
        }
    }
}
