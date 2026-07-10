using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class postannouncement : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in tutors can post announcements
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
            string query = @"
                SELECT a_id, a_title, post_date, is_active, expiry_date, messages
                FROM tblannouncement
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
                        rptAnnouncements.DataSource = dt;
                        rptAnnouncements.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading announcements: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string title = txtTitle.Text.Trim();
            string message = txtMessage.Text.Trim();
            bool isActive = chkActive.Checked;

            DateTime? expiryDate = null;
            if (!string.IsNullOrWhiteSpace(txtExpiry.Text) && DateTime.TryParse(txtExpiry.Text, out DateTime parsedExpiry))
            {
                expiryDate = parsedExpiry;
            }

            string query = @"
                INSERT INTO tblannouncement (a_title, messages, expiry_date, is_active)
                VALUES (@title, @messages, @expiry, @isactive);";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@messages", message);
                cmd.Parameters.AddWithValue("@expiry", (object)expiryDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@isactive", isActive);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Announcement posted successfully.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;

                    // Clear the form
                    txtTitle.Text = "";
                    txtMessage.Text = "";
                    txtExpiry.Text = "";
                    chkActive.Checked = true;
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error posting announcement: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }

            LoadAnnouncements();
        }

        protected void rptAnnouncements_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int aId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Delete")
            {
                DeleteAnnouncement(aId);
            }
            else if (e.CommandName == "ToggleActive")
            {
                ToggleActive(aId);
            }

            LoadAnnouncements();
        }

        private void DeleteAnnouncement(int aId)
        {
            string query = "DELETE FROM tblannouncement WHERE a_id = @id;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@id", aId);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMessage.Text = "Announcement deleted.";
                    lblMessage.ForeColor = System.Drawing.Color.Gray;
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error deleting announcement: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void ToggleActive(int aId)
        {
            string query = "UPDATE tblannouncement SET is_active = NOT is_active WHERE a_id = @id;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@id", aId);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error updating status: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}