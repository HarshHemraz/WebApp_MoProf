using System;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.adminContent
{
    public partial class managetutors : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTutors(null);
            }
        }

        private void LoadTutors(string searchTerm)
        {
            string query = @"SELECT id, firstname, lastname, email, ""dateCreated"" 
                              FROM tblusers 
                              WHERE role = 'tutor' ";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += @" AND (firstname ILIKE @search 
                                 OR lastname ILIKE @search 
                                 OR email ILIKE @search) ";
            }

            query += " ORDER BY \"dateCreated\" DESC;";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm.Trim() + "%");
                    }

                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                tutorsRepeater.DataSource = dt;
                                tutorsRepeater.DataBind();
                                tutorsRepeater.Visible = true;
                                lblNoRecords.Visible = false;
                            }
                            else
                            {
                                tutorsRepeater.Visible = false;
                                lblNoRecords.Visible = true;
                                lblNoRecords.Text = string.IsNullOrWhiteSpace(searchTerm)
                                    ? "No tutors found."
                                    : "No tutors found matching \"" + searchTerm + "\".";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error loading tutors: " + ex.Message, "danger");
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTutors(txtSearch.Text);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadTutors(null);
        }

        protected void tutorsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tutorId = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "EditTutor":
                    Response.Redirect("~/adminContent/edittutor.aspx?id=" + tutorId);
                    break;

                case "DeleteTutor":
                    DeleteTutor(tutorId);
                    break;
            }
        }

        private void DeleteTutor(string tutorId)
        {
            string query = "DELETE FROM tblusers WHERE id = @id AND role = 'tutor';";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(tutorId));

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Tutor deleted successfully.", "success");
                            LoadTutors(txtSearch.Text);
                        }
                        else
                        {
                            ShowMessage("Tutor not found.", "danger");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error deleting tutor: " + ex.Message, "danger");
                    }
                }
            }
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            if (type == "success")
                lblMessage.CssClass = "alert alert-success";
            else if (type == "danger")
                lblMessage.CssClass = "alert alert-danger";
            else
                lblMessage.CssClass = "alert alert-info";
            lblMessage.Visible = true;
        }
    }
}