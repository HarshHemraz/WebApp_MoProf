using System;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.adminContent
{
    public partial class managestudents : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStudents(null);
            }
        }

        private void LoadStudents(string searchTerm)
        {
            string query = @"SELECT id, firstname, lastname, email, ""dateCreated"" 
                              FROM tblusers 
                              WHERE role = 'student' ";

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
                                studentsRepeater.DataSource = dt;
                                studentsRepeater.DataBind();
                                studentsRepeater.Visible = true;
                                lblNoRecords.Visible = false;
                            }
                            else
                            {
                                studentsRepeater.Visible = false;
                                lblNoRecords.Visible = true;
                                lblNoRecords.Text = string.IsNullOrWhiteSpace(searchTerm)
                                    ? "No students found."
                                    : "No students found matching \"" + searchTerm + "\".";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error loading students: " + ex.Message, "danger");
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadStudents(txtSearch.Text);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadStudents(null);
        }

        protected void studentsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string studentId = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "EditStudent":
                    Response.Redirect("~/adminContent/editstudent.aspx?id=" + studentId);
                    break;

                case "DeleteStudent":
                    DeleteStudent(studentId);
                    break;
            }
        }

        private void DeleteStudent(string studentId)
        {
            string query = "DELETE FROM tblusers WHERE id = @id AND role = 'student';";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(studentId));

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Student deleted successfully.", "success");
                            LoadStudents(txtSearch.Text);
                        }
                        else
                        {
                            ShowMessage("Student not found.", "danger");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error deleting student: " + ex.Message, "danger");
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