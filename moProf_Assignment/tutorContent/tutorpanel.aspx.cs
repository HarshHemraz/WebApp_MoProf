using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class tutormainpage : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - ensures only logged-in tutors can view this page
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadBookings();
            }
        }

        private void LoadBookings()
        {
            Guid userId = (Guid)Session["UserID"];

            string query = @"
                SELECT
                    br.br_id,
                    br.isaccepted,
                    br.req_date,
                    br.messages,
                    br.booking_date,
                    c.c_name,
                    u.firstname,
                    u.lastname,
                    u.email
                FROM tblbookingrequest br
                JOIN tblcourses c ON br.c_id = c.c_id
                JOIN tbltutor t ON c.tutor_id = t.t_id
                JOIN tblstudent s ON br.s_id = s.s_id
                JOIN tblusers u ON s.user_id = u.id
                WHERE t.user_id = @userId
                ORDER BY br.req_date DESC";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            rptBookings.DataSource = dt;
                            rptBookings.DataBind();
                            lblMessage.Text = "";
                        }
                        else
                        {
                            rptBookings.DataSource = null;
                            rptBookings.DataBind();
                            lblMessage.Text = "No booking requests found.";
                            lblMessage.ForeColor = System.Drawing.Color.Gray;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading bookings: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected string GetStatusHtml(object isAcceptedObj)
        {
            if (isAcceptedObj == null || isAcceptedObj == DBNull.Value)
                return "<span class='text-warning'>⏳ Pending</span>";

            bool isAccepted = Convert.ToBoolean(isAcceptedObj);
            return isAccepted
                ? "<span class='text-success'>✅ Accepted</span>"
                : "<span class='text-danger'>❌ Not Approved</span>";
        }

        protected bool IsPending(object isAcceptedObj)
        {
            return isAcceptedObj == null || isAcceptedObj == DBNull.Value;
        }

        protected void rptBookings_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int brId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Delete")
            {
                DeleteBooking(brId);
                LoadBookings();
                return;
            }

            if (e.CommandName == "ViewResult")
            {
                ShowStudentResult(brId);
                LoadBookings(); // keep the list rebound so the repeater still renders
                return;
            }

            bool accept = e.CommandName == "Accept";
            bool wasAlreadyAccepted = false;

            string studentEmail = "";
            string studentName = "";
            string courseName = "";

            using (var con = new NpgsqlConnection(conString))
            {
                con.Open();

                // Check current status BEFORE updating, so we know if this is a fresh approval
                string checkSql = "SELECT isaccepted FROM tblbookingrequest WHERE br_id = @brId";
                using (var checkCmd = new NpgsqlCommand(checkSql, con))
                {
                    checkCmd.Parameters.AddWithValue("@brId", brId);
                    var result = checkCmd.ExecuteScalar();
                    wasAlreadyAccepted = result != null && result != DBNull.Value && (bool)result;
                }

                // Now update
                string sql = "UPDATE tblbookingrequest SET isaccepted = @accept WHERE br_id = @brId";
                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@accept", accept);
                    cmd.Parameters.AddWithValue("@brId", brId);
                    cmd.ExecuteNonQuery();
                }

                // Only fetch email info if this is a NEW approval (not already accepted before)
                if (accept && !wasAlreadyAccepted)
                {
                    string infoQuery = @"
                        SELECT u.email, u.firstname, u.lastname, c.c_name
                        FROM tblbookingrequest br
                        JOIN tblstudent s ON br.s_id = s.s_id
                        JOIN tblusers u ON s.user_id = u.id
                        JOIN tblcourses c ON br.c_id = c.c_id
                        WHERE br.br_id = @brId";

                    using (var cmd = new NpgsqlCommand(infoQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@brId", brId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                studentEmail = reader["email"].ToString();
                                studentName = reader["firstname"].ToString() + " " + reader["lastname"].ToString();
                                courseName = reader["c_name"].ToString();
                            }
                        }
                    }
                }
            }

            if (accept && !wasAlreadyAccepted && !string.IsNullOrEmpty(studentEmail))
            {
                SendBookingApprovedEmail(studentEmail, studentName, courseName);
            }

            LoadBookings();
        }

        private void DeleteBooking(int brId)
        {
            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand("DELETE FROM tblbookingrequest WHERE br_id = @brId", con))
            {
                cmd.Parameters.AddWithValue("@brId", brId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Booking request deleted.";
            lblMessage.ForeColor = System.Drawing.Color.Gray;
        }

        private void SendBookingApprovedEmail(string toEmail, string studentName, string courseName)
        {
            try
            {
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
                string smtpPass = ConfigurationManager.AppSettings["SmtpPass"];

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpUser, "moProf Tutoring");
                mail.To.Add(toEmail);
                mail.Subject = "Your booking request has been approved!";
                mail.Body = $@"
Hi {studentName},
 
Good news! Your booking request for the course '{courseName}' has been approved by the tutor.
 
You can log in to your account to view the details.
 
Thanks,
moProf Team";
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Booking updated, but email notification failed: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Orange;
            }
        }

        private void ShowStudentResult(int brId)
        {
            DataTable dtResult = new DataTable();

            string query = @"
                SELECT
                    s.s_id,
                    s.grade,
                    s.schoolname,
                    s.preferredsubjects,
                    s.totalbookings,
                    s.totalspent,
                    s.updatedat,
                    s.createdat,
                    s.user_id,
                    s.result_image
                FROM tblbookingrequest br
                JOIN tblstudent s ON br.s_id = s.s_id
                WHERE br.br_id = @brId";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@brId", brId);
                con.Open();
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dtResult);
                }
            }

            if (dtResult.Rows.Count > 0)
            {
                DataRow row = dtResult.Rows[0];

                lblStudentID.Text = row["s_id"]?.ToString() ?? "-";
                lblGrade.Text = row["grade"]?.ToString() ?? "-";
                lblSchoolName.Text = row["schoolname"]?.ToString() ?? "-";
                lblPreferredSubjects.Text = row["preferredsubjects"]?.ToString() ?? "-";
                lblTotalBookings.Text = row["totalbookings"]?.ToString() ?? "0";
                lblTotalSpent.Text = row["totalspent"] != DBNull.Value
                    ? Convert.ToDecimal(row["totalspent"]).ToString("N2")
                    : "0.00";
                lblUpdatedAt.Text = row["updatedat"] != DBNull.Value
                    ? Convert.ToDateTime(row["updatedat"]).ToString("dd/MM/yyyy HH:mm")
                    : "-";
                lblCreatedAt.Text = row["createdat"] != DBNull.Value
                    ? Convert.ToDateTime(row["createdat"]).ToString("dd/MM/yyyy HH:mm")
                    : "-";
                lblUserID.Text = row["user_id"]?.ToString() ?? "-";

                string resultImage = row["result_image"]?.ToString();
                if (!string.IsNullOrEmpty(resultImage))
                {
                    imgResult.ImageUrl = ResolveUrl("~/images/" + resultImage);
                    imgResult.Visible = true;
                    lblNoResult.Visible = false;
                }
                else
                {
                    imgResult.Visible = false;
                    lblNoResult.Visible = true;
                    lblNoResult.Text = "No academic result image uploaded.";
                }
            }
            else
            {
                lblStudentID.Text = "-";
                lblGrade.Text = "-";
                lblSchoolName.Text = "-";
                lblPreferredSubjects.Text = "-";
                lblTotalBookings.Text = "0";
                lblTotalSpent.Text = "0.00";
                lblUpdatedAt.Text = "-";
                lblCreatedAt.Text = "-";
                lblUserID.Text = "-";
                imgResult.Visible = false;
                lblNoResult.Visible = true;
                lblNoResult.Text = "No student record found for this booking.";
            }

            // Works because the button that triggered this is inside an UpdatePanel
            // (async postback) — see the .aspx. On a plain full postback this still
            // runs, but it only visibly "pops" the modal reliably under UpdatePanel.
            ScriptManager.RegisterStartupScript(this, GetType(), "showResultModal_" + brId,
                "var resultModalEl = document.getElementById('resultModal');" +
                "if (resultModalEl && window.bootstrap) {" +
                "  var resultModal = bootstrap.Modal.getOrCreateInstance(resultModalEl);" +
                "  resultModal.show();" +
                "}", true);
        }
    }
}