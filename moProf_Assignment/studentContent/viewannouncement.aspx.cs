using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.studentContent
{
    public partial class viewannouncement : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in students can view announcements
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAnnouncements();
            }
        }

        private void LoadAnnouncements()
        {
            // Only show announcements that are active and either have no
            // expiry date or haven't expired yet.
            string query = @"
                SELECT a_id, a_title, post_date, expiry_date, messages
                FROM tblannouncement
                WHERE is_active = true
                  AND (expiry_date IS NULL OR expiry_date > NOW())
                ORDER BY post_date DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            rptAnnouncements.DataSource = dt;
                            rptAnnouncements.DataBind();
                            lblMessage.Text = "";
                        }
                        else
                        {
                            rptAnnouncements.DataSource = null;
                            rptAnnouncements.DataBind();
                            lblMessage.Text = "No announcements at this time.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading announcements: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}