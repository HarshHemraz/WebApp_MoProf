using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
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
            string query = @"SELECT id, firstname, lastname, email, ""dateCreated"", status 
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
                case "AcceptTutor":
                    UpdateTutorStatus(tutorId, "accepted");
                    break;

                case "DenyTutor":
                    UpdateTutorStatus(tutorId, "denied");
                    break;

                case "DeleteTutor":
                    DeleteTutor(tutorId);
                    break;
            }
        }

        // Shared handler for both Accept and Deny — updates status and emails the tutor
        private void UpdateTutorStatus(string tutorId, string newStatus)
        {
            string selectQuery = "SELECT firstname, email FROM tblusers WHERE id = @id AND role = 'tutor';";
            string firstName = null;
            string email = null;

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(selectQuery, con))
            {
                cmd.Parameters.AddWithValue("@id", Guid.Parse(tutorId));

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            firstName = reader["firstname"].ToString();
                            email = reader["email"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error looking up tutor: " + ex.Message, "danger");
                    return;
                }
            }

            if (email == null)
            {
                ShowMessage("Tutor not found.", "danger");
                return;
            }

            string updateQuery = "UPDATE tblusers SET status = @status WHERE id = @id AND role = 'tutor';";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", Guid.Parse(tutorId));

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        bool emailSent = SendStatusEmail(email, firstName, newStatus);

                        string statusLabel = newStatus == "accepted" ? "accepted" : "denied";
                        ShowMessage(
                            emailSent
                                ? $"Tutor registration {statusLabel} and notified by email."
                                : $"Tutor registration {statusLabel}, but the notification email could not be sent.",
                            emailSent ? "success" : "danger");

                        LoadTutors(txtSearch.Text);
                    }
                    else
                    {
                        ShowMessage("Tutor not found.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error updating tutor status: " + ex.Message, "danger");
                }
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

        // Renders a colored badge for the Status column
        protected string GetStatusBadge(string status)
        {
            switch (status)
            {
                case "accepted":
                    return "<span class='badge bg-success'>Accepted</span>";
                case "denied":
                    return "<span class='badge bg-danger'>Denied</span>";
                default:
                    return "<span class='badge bg-warning text-dark'>Pending</span>";
            }
        }

        private bool SendStatusEmail(string toEmail, string firstName, string status)
        {
            try
            {
                string host = ConfigurationManager.AppSettings["SmtpHost"];
                int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string user = ConfigurationManager.AppSettings["SmtpUser"];
                string pass = ConfigurationManager.AppSettings["SmtpPass"];

                string subject;
                string body;

                if (status == "accepted")
                {
                    subject = "Your tutor registration has been approved";
                    body = $"Hi {firstName},\n\nGood news! Your tutor registration has been reviewed and approved. You can now log in and start creating courses.\n\nWelcome aboard!";
                }
                else
                {
                    subject = "Update on your tutor registration";
                    body = $"Hi {firstName},\n\nWe're sorry to let you know that your tutor registration was not approved at this time. If you believe this was a mistake, please contact support.\n\nThank you for your interest.";
                }

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = true;

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(user, "moProf");
                        mail.To.Add(toEmail);
                        mail.Subject = subject;
                        mail.Body = body;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SMTP ERROR: " + ex.ToString());
                return false;
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