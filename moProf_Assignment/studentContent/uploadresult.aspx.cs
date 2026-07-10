using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class StudentResultUpload : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string currentUserEmail = "";
        private string currentUserId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in via session
            if (Session["UserEmail"] == null)
            {
                Response.Redirect("~/studentContent/studentlogin.aspx");
                return;
            }

            currentUserEmail = Session["UserEmail"].ToString();

            // Verify user is a student
            if (Session["UserRole"] != null)
            {
                string role = Session["UserRole"].ToString().ToLowerInvariant().Trim();
                if (role != "student")
                {
                    Response.Write("<script>alert('Access Denied. Only students can access this page.');</script>");
                    Response.Redirect("~/studentContent/studentlogin.aspx");
                    return;
                }
            }

            // IMPORTANT: this must run on EVERY request (including postbacks),
            // because currentUserId is a plain field and does not survive
            // across postbacks on its own. Previously this only ran when
            // !IsPostBack, so on Save/Update currentUserId was always empty.
            GetUserIdByEmail();

            if (string.IsNullOrEmpty(currentUserId))
            {
                ShowMessage("Unable to identify user. Please login again.", "danger");
                return;
            }

            // Only load/populate the form fields on the initial GET,
            // not on every postback (otherwise we'd overwrite the user's
            // in-progress edits every time they click Save).
            if (!IsPostBack)
            {
                LoadStudentInfo();
                LoadStudentResult();
            }
        }

        private void GetUserIdByEmail()
        {
            try
            {
                // Case-insensitive search using ILIKE (PostgreSQL case-insensitive)
                string query = "SELECT id, email FROM tblusers WHERE email ILIKE @email";

                using (var con = new NpgsqlConnection(conString))
                {
                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@email", currentUserEmail.Trim());
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentUserId = reader["id"].ToString();
                            }
                            else
                            {
                                currentUserId = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error retrieving user: " + ex.Message, "danger");
                currentUserId = "";
            }
        }

        private void LoadStudentInfo()
        {
            try
            {
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return;
                }

                // Cast userId to UUID
                string query = @"
                    SELECT firstname, lastname, email 
                    FROM tblusers 
                    WHERE id = @userId::uuid";

                using (var con = new NpgsqlConnection(conString))
                {
                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["firstname"].ToString() + " " + reader["lastname"].ToString();
                                if (string.IsNullOrWhiteSpace(reader["lastname"].ToString()))
                                {
                                    fullName = reader["firstname"].ToString();
                                }
                                lblStudentName.Text = fullName;
                                lblStudentEmail.Text = reader["email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading student information: " + ex.Message, "danger");
            }
        }

        private void LoadStudentResult()
        {
            try
            {
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return;
                }

                // Cast userId to UUID
                string query = @"
                    SELECT s_id, grade, schoolname, preferredsubjects, 
                           totalbookings, totalspent, updatedat, createdat, result_image
                    FROM tblstudent
                    WHERE user_id = @userId::uuid";

                using (var con = new NpgsqlConnection(conString))
                {
                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Load existing data
                                txtGrade.Text = reader["grade"]?.ToString() ?? "";
                                txtSchoolName.Text = reader["schoolname"]?.ToString() ?? "";
                                txtPreferredSubjects.Text = reader["preferredsubjects"]?.ToString() ?? "";
                                txtTotalBookings.Text = reader["totalbookings"]?.ToString() ?? "0";
                                txtTotalSpent.Text = reader["totalspent"]?.ToString() ?? "0.00";

                                // Load image if exists
                                string imagePath = reader["result_image"]?.ToString();
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    imgResult.ImageUrl = ResolveUrl("~/uploads/" + imagePath);
                                    imgResult.Visible = true;
                                    lblFileName.Text = "Current file: " + imagePath;
                                    lblFileName.Visible = true;
                                    ViewState["CurrentImage"] = imagePath;
                                }

                                // Show last updated time
                                if (reader["updatedat"] != DBNull.Value)
                                {
                                    lblUpdatedAt.Text = Convert.ToDateTime(reader["updatedat"]).ToString("MMM dd, yyyy HH:mm");
                                }
                                else
                                {
                                    lblUpdatedAt.Text = "Not updated yet";
                                }

                                // Store s_id for update (as integer)
                                ViewState["StudentId"] = reader["s_id"].ToString();
                            }
                            else
                            {
                                // No existing record, set default values
                                txtTotalBookings.Text = "0";
                                txtTotalSpent.Text = "0.00";
                                lblUpdatedAt.Text = "No record found. Create new?";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading student results: " + ex.Message, "danger");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            // Check if we have a valid user ID
            if (string.IsNullOrEmpty(currentUserId))
            {
                ShowMessage("User not identified. Please login again.", "danger");
                return;
            }

            try
            {
                // Validate inputs
                string grade = txtGrade.Text.Trim();
                string schoolName = txtSchoolName.Text.Trim();
                string preferredSubjects = txtPreferredSubjects.Text.Trim();
                int totalBookings = string.IsNullOrEmpty(txtTotalBookings.Text) ? 0 : Convert.ToInt32(txtTotalBookings.Text);
                decimal totalSpent = string.IsNullOrEmpty(txtTotalSpent.Text) ? 0 : Convert.ToDecimal(txtTotalSpent.Text);

                // Handle file upload
                string fileName = "";
                if (fileUpload.HasFile)
                {
                    // Validate file size (5MB max)
                    if (fileUpload.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        ShowMessage("File size exceeds 5MB limit.", "danger");
                        return;
                    }

                    // Validate file extension
                    string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
                    if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                    {
                        ShowMessage("Invalid file format. Please upload JPG, PNG, PDF, DOC, or DOCX files.", "danger");
                        return;
                    }

                    // Create uploads folder if it doesn't exist
                    string uploadPath = Server.MapPath("~/uploads/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate unique filename
                    fileName = Guid.NewGuid().ToString() + fileExtension;
                    string fullPath = Path.Combine(uploadPath, fileName);

                    // Save file
                    fileUpload.SaveAs(fullPath);

                    // Delete old image if exists
                    if (ViewState["CurrentImage"] != null)
                    {
                        string oldPath = Path.Combine(uploadPath, ViewState["CurrentImage"].ToString());
                        if (File.Exists(oldPath))
                        {
                            File.Delete(oldPath);
                        }
                    }
                }
                else
                {
                    // If no new file uploaded, keep existing
                    if (ViewState["CurrentImage"] != null)
                    {
                        fileName = ViewState["CurrentImage"].ToString();
                    }
                }

                bool isUpdate = ViewState["StudentId"] != null && !string.IsNullOrEmpty(ViewState["StudentId"].ToString());

                if (isUpdate)
                {
                    // UPDATE existing record - convert studentId to integer
                    string query = @"
                        UPDATE tblstudent 
                        SET grade = @grade, 
                            schoolname = @schoolName, 
                            preferredsubjects = @preferredSubjects, 
                            totalbookings = @totalBookings, 
                            totalspent = @totalSpent,
                            updatedat = @updatedAt";

                    // Add image to query if uploaded
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        query += ", result_image = @resultImage";
                    }

                    query += " WHERE s_id = @studentId::int AND user_id = @userId::uuid";

                    using (var con = new NpgsqlConnection(conString))
                    {
                        using (var cmd = new NpgsqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@grade", grade);
                            cmd.Parameters.AddWithValue("@schoolName", schoolName);
                            cmd.Parameters.AddWithValue("@preferredSubjects", string.IsNullOrEmpty(preferredSubjects) ? DBNull.Value : (object)preferredSubjects);
                            cmd.Parameters.AddWithValue("@totalBookings", totalBookings);
                            cmd.Parameters.AddWithValue("@totalSpent", totalSpent);
                            cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@studentId", ViewState["StudentId"].ToString());
                            cmd.Parameters.AddWithValue("@userId", currentUserId);

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                cmd.Parameters.AddWithValue("@resultImage", fileName);
                            }

                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                ShowMessage("Student results updated successfully!", "success");
                                lblUpdatedAt.Text = DateTime.UtcNow.ToString("MMM dd, yyyy HH:mm");
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    ViewState["CurrentImage"] = fileName;
                                    imgResult.ImageUrl = ResolveUrl("~/uploads/" + fileName);
                                    imgResult.Visible = true;
                                    lblFileName.Text = "Current file: " + fileName;
                                    lblFileName.Visible = true;
                                }
                            }
                            else
                            {
                                ShowMessage("No changes were made. Please try again.", "warning");
                            }
                        }
                    }
                }
                else
                {
                    // INSERT new record
                    string query = @"
                        INSERT INTO tblstudent 
                        (grade, schoolname, preferredsubjects, totalbookings, totalspent, createdat, updatedat, user_id, result_image)
                        VALUES 
                        (@grade, @schoolName, @preferredSubjects, @totalBookings, @totalSpent, @createdAt, @updatedAt, @userId::uuid, @resultImage)
                        RETURNING s_id";

                    using (var con = new NpgsqlConnection(conString))
                    {
                        using (var cmd = new NpgsqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@grade", grade);
                            cmd.Parameters.AddWithValue("@schoolName", schoolName);
                            cmd.Parameters.AddWithValue("@preferredSubjects", string.IsNullOrEmpty(preferredSubjects) ? DBNull.Value : (object)preferredSubjects);
                            cmd.Parameters.AddWithValue("@totalBookings", totalBookings);
                            cmd.Parameters.AddWithValue("@totalSpent", totalSpent);
                            cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@userId", currentUserId);
                            cmd.Parameters.AddWithValue("@resultImage", string.IsNullOrEmpty(fileName) ? DBNull.Value : (object)fileName);

                            con.Open();
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                ViewState["StudentId"] = result.ToString();
                                ShowMessage("Student results saved successfully!", "success");
                                lblUpdatedAt.Text = DateTime.UtcNow.ToString("MMM dd, yyyy HH:mm");
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    ViewState["CurrentImage"] = fileName;
                                    imgResult.ImageUrl = ResolveUrl("~/uploads/" + fileName);
                                    imgResult.Visible = true;
                                    lblFileName.Text = "Current file: " + fileName;
                                    lblFileName.Visible = true;
                                }
                            }
                            else
                            {
                                ShowMessage("Failed to save results. Please try again.", "danger");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving student results: " + ex.Message, "danger");
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Clear all fields
            txtGrade.Text = "";
            txtSchoolName.Text = "";
            txtPreferredSubjects.Text = "";
            txtTotalBookings.Text = "0";
            txtTotalSpent.Text = "0.00";

            // Reload the original data if exists
            LoadStudentResult();

            // Clear message
            pnlMessage.Visible = false;
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            // Set alert type
            pnlMessage.CssClass = "alert alert-" + type;
        }
    }
}