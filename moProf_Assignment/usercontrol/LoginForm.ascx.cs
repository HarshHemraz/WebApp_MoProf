using Npgsql;
using OtpNet;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI.WebControls;

namespace moProf_Assignment.usercontrol
{
    public partial class LoginForm : System.Web.UI.UserControl
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private const int MaxLoginAttempts = 3;
        private const int MaxOtpAttempts = 3;
        private const int OtpStepSeconds = 300; // 5 minute validity window
        private const int OtpDigits = 6;

        public string CurrentFormRole { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    using (var con = new NpgsqlConnection(conString))
                    {
                        con.Open();
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Database Connection Failed: " + ex.Message);
                }

                // Always start on the password step for a fresh page visit
                ShowLoginStep();
            }
        }

        public string WelcomeMessage
        {
            get { return welcomeHeading.InnerText; }
            set { welcomeHeading.InnerText = value; }
        }

        public string getEmail
        {
            get { return emailtxt.Text.Trim(); }
        }

        public string getPassword
        {
            get { return passwordtxt.Text.Trim(); }
        }

        // ---------- Step 1: email + password ----------

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string txtemail = emailtxt.Text.Trim();
            string txtpassword = passwordtxt.Text.Trim();

            int loginAttempts = Session["LoginAttempts"] != null ? (int)Session["LoginAttempts"] : 0;

            if (loginAttempts >= MaxLoginAttempts)
            {
                lblMessage.Text = "Account has been temporarily blocked due to too many failed login attempts. Please try again later.";
                lblMessage.CssClass = "errrormsg";
                return;
            }

            string query = "SELECT id, firstname, lastname, role, \"otpSecret\", \"isFrozen\" FROM tblusers WHERE email = @email AND password = @pass;";
            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@email", txtemail);
                cmd.Parameters.AddWithValue("@pass", txtpassword);

                try
                {
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool isFrozen = reader["isFrozen"] is bool fr && fr;

                            if (isFrozen)
                            {
                                reader.Close();
                                lblMessage.Text = "This account has been frozen by an administrator. Please contact support.";
                                lblMessage.CssClass = "errrormsg";
                                return;
                            }

                            Session["LoginAttempts"] = 0;

                            Guid userId = (Guid)reader["id"];
                            string userFname = reader["firstname"].ToString();
                            string userLname = reader["lastname"].ToString();
                            string userRole = reader["role"].ToString().Trim();
                            string otpSecret = reader["otpSecret"] as string;

                            reader.Close();

                            // Generate a secret on first-ever login if the user doesn't have one yet
                            if (string.IsNullOrEmpty(otpSecret))
                            {
                                byte[] newKey = KeyGeneration.GenerateRandomKey(20);
                                otpSecret = Base32Encoding.ToString(newKey);
                                SaveOtpSecret(userId, otpSecret);
                            }

                            // Stash pending (unauthenticated) login info until OTP is confirmed
                            Session["PendingUserID"] = userId;
                            Session["PendingUserEmail"] = txtemail;
                            Session["PendingUserFirstName"] = userFname;
                            Session["PendingUserLastName"] = userLname;
                            Session["PendingUserRole"] = userRole;
                            Session["PendingOtpSecret"] = otpSecret;
                            Session["OtpAttempts"] = 0;

                            byte[] secretKeyBytes = Base32Encoding.ToBytes(otpSecret);
                            var totp = new Totp(secretKeyBytes, step: OtpStepSeconds, totpSize: OtpDigits);
                            string otpCode = totp.ComputeTotp();

                            bool sent = SendOtpEmail(txtemail, otpCode);

                            ShowOtpStep();

                            lblMessage.Text = sent
                                ? "We've sent a verification code to your email."
                                : "Password correct, but the verification email could not be sent. Try resending the code.";
                            lblMessage.CssClass = sent ? "text-success" : "errrormsg";
                        }
                        else
                        {
                            loginAttempts++;
                            Session["LoginAttempts"] = loginAttempts;

                            int attemptsLeft = MaxLoginAttempts - loginAttempts;

                            lblMessage.CssClass = "errrormsg";
                            lblMessage.Text = attemptsLeft > 0
                                ? $"Invalid Email or Password. You have {attemptsLeft} attempt(s) left before your account is temporarily blocked."
                                : "Invalid Email or Password. Account has been temporarily blocked due to too many failed login attempts.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Login Error: " + HttpUtility.HtmlEncode(ex.Message);
                    lblMessage.CssClass = "errrormsg";
                }
            }
        }

        // ---------- Step 2: OTP entry ----------

        protected void verifyOtpBtn_Click(object sender, EventArgs e)
        {
            if (Session["PendingUserID"] == null)
            {
                lblOtpMessage.Text = "Your session expired. Please log in again.";
                lblOtpMessage.CssClass = "errrormsg";
                ShowLoginStep();
                return;
            }

            int otpAttempts = Session["OtpAttempts"] != null ? (int)Session["OtpAttempts"] : 0;

            if (otpAttempts >= MaxOtpAttempts)
            {
                lblOtpMessage.Text = "Too many incorrect codes. Please log in again.";
                lblOtpMessage.CssClass = "errrormsg";
                ClearPendingLogin();
                ShowLoginStep();
                return;
            }

            string enteredCode = otpTxt.Text.Trim();
            string otpSecret = Session["PendingOtpSecret"] as string;

            byte[] secretKeyBytes = Base32Encoding.ToBytes(otpSecret);
            var totp = new Totp(secretKeyBytes, step: OtpStepSeconds, totpSize: OtpDigits);

            bool isValid = totp.VerifyTotp(enteredCode, out long timeStepMatched,
                new VerificationWindow(previous: 1, future: 1));

            if (isValid)
            {
                // Finalize the real session now that OTP is confirmed
                Guid userId = (Guid)Session["PendingUserID"];
                string userRole = Session["PendingUserRole"] as string;

                Session["UserID"] = userId;
                Session["UserEmail"] = Session["PendingUserEmail"];
                Session["UserFirstName"] = Session["PendingUserFirstName"];
                Session["UserLastName"] = Session["PendingUserLastName"];
                Session["UserRole"] = userRole;

                ClearPendingLogin();

                string normalizedRole = userRole?.ToLowerInvariant().Trim();
                string redirectUrl = null;

                if (normalizedRole == "student")
                {
                    redirectUrl = "~/studentContent/studentpanel.aspx";
                }
                else if (normalizedRole == "tutor")
                {
                    redirectUrl = "~/tutorContent/tutorpanel.aspx";
                }
                else if (normalizedRole == "admin")
                {
                    redirectUrl = "~/adminContent/adminpanel.aspx";
                }

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    Response.Redirect(redirectUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    lblOtpMessage.Text = $"Login successful, but role '{HttpUtility.HtmlEncode(userRole)}' is unassigned.";
                    lblOtpMessage.CssClass = "text-warning";
                }
            }
            else
            {
                otpAttempts++;
                Session["OtpAttempts"] = otpAttempts;
                int attemptsLeft = MaxOtpAttempts - otpAttempts;

                lblOtpMessage.CssClass = "errrormsg";
                if (attemptsLeft > 0)
                {
                    lblOtpMessage.Text = $"Invalid or expired code. You have {attemptsLeft} attempt(s) left.";
                }
                else
                {
                    lblOtpMessage.Text = "Too many incorrect codes. Please log in again.";
                    ClearPendingLogin();
                    ShowLoginStep();
                }
            }
        }

        protected void resendOtpBtn_Click(object sender, EventArgs e)
        {
            string email = Session["PendingUserEmail"] as string;
            string otpSecret = Session["PendingOtpSecret"] as string;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpSecret))
            {
                lblOtpMessage.Text = "Your session expired. Please log in again.";
                lblOtpMessage.CssClass = "errrormsg";
                ShowLoginStep();
                return;
            }

            byte[] secretKeyBytes = Base32Encoding.ToBytes(otpSecret);
            var totp = new Totp(secretKeyBytes, step: OtpStepSeconds, totpSize: OtpDigits);
            string otpCode = totp.ComputeTotp();

            bool sent = SendOtpEmail(email, otpCode);

            lblOtpMessage.Text = sent ? "A new code has been sent to your email." : "Could not send email right now. Please try again shortly.";
            lblOtpMessage.CssClass = sent ? "text-success" : "errrormsg";
        }

        protected void backToLoginBtn_Click(object sender, EventArgs e)
        {
            ClearPendingLogin();
            ShowLoginStep();
        }

        protected void createAcct_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/registerpage.aspx");
        }

        // ---------- helpers ----------

        private void ShowLoginStep()
        {
            pnlLoginStep.Visible = true;
            pnlOtpStep.Visible = false;
        }

        private void ShowOtpStep()
        {
            pnlLoginStep.Visible = false;
            pnlOtpStep.Visible = true;
        }

        private void ClearPendingLogin()
        {
            Session.Remove("PendingUserID");
            Session.Remove("PendingUserEmail");
            Session.Remove("PendingUserFirstName");
            Session.Remove("PendingUserLastName");
            Session.Remove("PendingUserRole");
            Session.Remove("PendingOtpSecret");
            Session.Remove("OtpAttempts");
        }

        private void SaveOtpSecret(Guid userId, string secretBase32)
        {
            string query = "UPDATE tblusers SET \"otpSecret\" = @secret WHERE id = @id;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@secret", secretBase32);
                cmd.Parameters.AddWithValue("@id", userId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool SendOtpEmail(string toEmail, string otpCode)
        {
            try
            {
                string host = ConfigurationManager.AppSettings["SmtpHost"];
                int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string user = ConfigurationManager.AppSettings["SmtpUser"];
                string pass = ConfigurationManager.AppSettings["SmtpPass"];

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = true;

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(user, "moProf");
                        mail.To.Add(toEmail);
                        mail.Subject = "Your login verification code";
                        mail.Body = $"Your verification code is: {otpCode}\nThis code is valid for {OtpStepSeconds / 60} minutes.";
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
    }
    }
