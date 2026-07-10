using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserInfo();
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                if (Session["UserEmail"] != null)
                {
                    string firstName = Session["UserFirstName"]?.ToString() ?? "";
                    string lastName = Session["UserLastName"]?.ToString() ?? "";

                    string displayName = firstName;
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        displayName = firstName + " " + lastName;
                    }

                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = Session["UserEmail"].ToString();
                    }

                    lblUserName.Text = displayName;
                }
                else
                {
                    lblUserName.Text = "Hello, Guest";
                }
            }
            catch
            {
                lblUserName.Text = "Guest";
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/studentContent/studentlogin.aspx");
        }
    }
}