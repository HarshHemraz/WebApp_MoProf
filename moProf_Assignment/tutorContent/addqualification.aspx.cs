using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class addqualification : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (Session["Role"] != null && Session["Role"].ToString() != "tutor")
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadQualifications();
            }
        }

        // Resolves the logged-in user's tutor_id (t_id) from tbltutor via their UserID
        private int? GetTutorId()
        {
            Guid userId = (Guid)Session["UserID"];
            string query = "SELECT t_id FROM tbltutor WHERE user_id = @userId;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
        }

        private void LoadQualifications()
        {
            int? tutorId = GetTutorId();
            if (tutorId == null)
            {
                ShowMessage("Could not find your tutor profile.", "danger");
                return;
            }

            string query = @"SELECT q_id, degree_title, institution, field_of_study, year_obtained, certificate_file, ""dateAdded""
                              FROM tblqualification
                              WHERE t_id = @tid
                              ORDER BY ""dateAdded"" DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@tid", tutorId.Value);

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            rptQualifications.DataSource = dt;
                            rptQualifications.DataBind();
                            rptQualifications.Visible = true;
                            lblNoRecords.Visible = false;
                        }
                        else
                        {
                            rptQualifications.Visible = false;
                            lblNoRecords.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error loading qualifications: " + ex.Message, "danger");
                }
            }
        }

        protected void btnAddQualification_Click(object sender, EventArgs e)
        {
            int? tutorId = GetTutorId();
            if (tutorId == null)
            {
                ShowMessage("Could not find your tutor profile.", "danger");
                return;
            }

            string degree = txtDegree.Text.Trim();
            string institution = txtInstitution.Text.Trim();
            string field = txtField.Text.Trim();
            int? year = null;

            if (int.TryParse(txtYear.Text.Trim(), out int parsedYear))
            {
                year = parsedYear;
            }

            string certFileName = null;
            if (fileUploadCert.HasFile)
            {
                try
                {
                    certFileName = Guid.NewGuid() + Path.GetExtension(fileUploadCert.FileName);
                    string folderPath = Server.MapPath("~/QualificationFiles/");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    fileUploadCert.SaveAs(Path.Combine(folderPath, certFileName));
                }
                catch (Exception fileEx)
                {
                    ShowMessage("File upload error: " + fileEx.Message, "danger");
                    return;
                }
            }

            string query = @"INSERT INTO tblqualification 
                (t_id, degree_title, institution, field_of_study, year_obtained, certificate_file) 
                VALUES (@tid, @degree, @institution, @field, @year, @cert);";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@tid", tutorId.Value);
                cmd.Parameters.AddWithValue("@degree", degree);
                cmd.Parameters.AddWithValue("@institution", institution);
                cmd.Parameters.AddWithValue("@field", string.IsNullOrEmpty(field) ? (object)DBNull.Value : field);
                cmd.Parameters.AddWithValue("@year", year.HasValue ? (object)year.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@cert", certFileName != null ? (object)certFileName : DBNull.Value);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ShowMessage("Qualification added successfully.", "success");

                    txtDegree.Text = "";
                    txtInstitution.Text = "";
                    txtField.Text = "";
                    txtYear.Text = "";

                    LoadQualifications();
                }
                catch (Exception ex)
                {
                    ShowMessage("Error adding qualification: " + ex.Message, "danger");
                }
            }
        }

        protected void rptQualifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteQualification")
            {
                int qId = Convert.ToInt32(e.CommandArgument);
                DeleteQualification(qId);
            }
        }

        private void DeleteQualification(int qId)
        {
            int? tutorId = GetTutorId();
            if (tutorId == null) return;

            // Restrict delete to qualifications owned by the current tutor
            string query = "DELETE FROM tblqualification WHERE q_id = @qid AND t_id = @tid;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@qid", qId);
                cmd.Parameters.AddWithValue("@tid", tutorId.Value);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Qualification deleted.", "success");
                    }
                    else
                    {
                        ShowMessage("Qualification not found or not yours to delete.", "danger");
                    }

                    LoadQualifications();
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting qualification: " + ex.Message, "danger");
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