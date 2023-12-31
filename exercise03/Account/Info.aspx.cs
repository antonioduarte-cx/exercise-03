﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ASPLab.Account
{
    public partial class Info : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["isLoggedIn"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                DisplayAccountInfo();
                //DisplayAccountInfo_Fixed();
            }
        }

        

        public void DisplayAccountInfo()
        {
            StringBuilder html = new StringBuilder();

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var conn = new SqlConnection(constr))
            {
                conn.Open();

                using (var cmd = new SqlCommand(@" select * from users where id=@account_id", conn))
                {
                    SqlParameter accountId = new SqlParameter("account_id", SqlDbType.Int);
                    accountId.Value = Request.QueryString["account_id"];
                    cmd.Parameters.Add(accountId);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows && dr.Read())
                    {
                        html.Append("Username: " + dr["username"] + "<br/>");
                        html.Append("Secret: " + dr["secret"]);
                    }
                    else
                    {
                        html.Append("<b style='color:red'>Invalid User id</b>");
                    }
                    AccountInfoPage.Controls.Add(new Literal { Text = html.ToString() });
                }
            }
        }

        public void DisplayAccountInfo_Fixed()
        {
            StringBuilder html = new StringBuilder();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var conn = new SqlConnection(constr))
            {
                conn.Open();

                //Validating Part to make sure the user authorized to access the object:
                using (var cmd = new SqlCommand(@" select * from users where id=@account_id and username=@username", conn))
                {
                    SqlParameter accountId = new SqlParameter("account_id", SqlDbType.Int);
                    accountId.Value = Request.QueryString["account_id"];
                    cmd.Parameters.Add(accountId);

                    SqlParameter usernameParam = new SqlParameter("username", SqlDbType.NVarChar);
                    usernameParam.Value = Session["username"].ToString();
                    cmd.Parameters.Add(usernameParam);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (!dr.HasRows)
                    {
                        html.Append("<b style='color:red'>You are not authorized</b>");
                        AccountInfoPage.Controls.Add(new Literal { Text = html.ToString() });
                        return;
                    }

                }
            }

            //Getting the Data:
            using (var conn = new SqlConnection(constr))
            {
                conn.Open();

                using (var cmd = new SqlCommand(@" select * from users where id=@account_id and username=@username", conn))
                {
                    SqlParameter accountId = new SqlParameter("account_id", SqlDbType.Int);
                    accountId.Value = Request.QueryString["account_id"];
                    cmd.Parameters.Add(accountId);

                    SqlParameter usernameParam = new SqlParameter("username", SqlDbType.NVarChar);
                    usernameParam.Value = Session["username"].ToString();
                    cmd.Parameters.Add(usernameParam);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        html.Append("Username: " + dr["username"] + "<br/>");
                        html.Append("Secret: " + dr["secret"]);
                    }
                    else
                    {
                        html.Append("<b style='color:red'>Invalid User id</b>");
                    }
                    AccountInfoPage.Controls.Add(new Literal { Text = html.ToString() });
                }
            }
        }
        protected void File_Load(object sender, EventArgs e)
        {	
			//if(Request.QueryString["filename"] == null)
        	//{
				DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Downloads"));
	        	int i = 0;
	        	
	        	foreach(FileInfo fi in di.GetFiles())
	        	{
	            	HyperLink HL = new HyperLink();
	            	HL.ID = "HyperLink" + i++;
	            	HL.Text = fi.Name;
	            	HL.NavigateUrl = Request.FilePath + "?filename="+fi.Name;
	            	ContentPlaceHolder cph = (ContentPlaceHolder)this.Master.FindControl("BodyContentPlaceholder");
	            	cph.Controls.Add(HL);
	            	cph.Controls.Add(new LiteralControl("<br/>"));
	        	}
        	//}
        	//else
        	//{
        		string filename = Request.QueryString["filename"];
        		if(filename != null)
        		{
                    try
                    {
                        ResponseFile(Request, Response, filename, MapPath("~/Downloads/" + filename), 100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        lblStatus.Text = "File not found: " + filename;   
                    }
                }
        	//}
        }
    }
}