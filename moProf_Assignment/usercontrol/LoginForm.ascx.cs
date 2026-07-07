using Npgsql;
using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.UI.WebControls;

namespace moProf_Assignment.usercontrol
{
    public partial class LoginForm : System.Web.UI.UserControl
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string txtemail = emailtxt.Text.Trim();
            string txtpassword = passwordtxt.Text.Trim();

            string query = "SELECT firstname, lastname, role FROM tblusers WHERE email = @email AND password = @pass;";

            using (var con = new NpgsqlConnection(conString))
            {
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
                                string userFname = reader["firstname"].ToString();
                                string userLname = reader["lastname"].ToString();
                                string userRole = reader["role"].ToString().Trim();

                                // DEBUGGING: Display the raw role value
                                Response.Write($"<div style='background:yellow;padding:10px;margin:10px;border:1px solid black;'>");
                                Response.Write($"<strong>DEBUG INFO:</strong><br/>");
                                Response.Write($"Raw Role from DB: '{reader["role"].ToString()}'<br/>");
                                Response.Write($"Trimmed Role: '{userRole}'<br/>");
                                Response.Write($"Role Length: {userRole.Length}<br/>");
                                Response.Write($"Role in lowercase: '{userRole.ToLowerInvariant()}'<br/>");
                                Response.Write($"Equals 'student' (OrdinalIgnoreCase): {string.Equals(userRole, "student", StringComparison.OrdinalIgnoreCase)}<br/>");
                                Response.Write($"Equals 'student' (after ToLower): {userRole.ToLowerInvariant() == "student"}<br/>");
                                Response.Write($"</div>");

                                // Set session variables
                                Session["UserEmail"] = txtemail;
                                Session["UserFirstName"] = userFname;
                                Session["UserLastName"] = userLname;
                                Session["UserRole"] = userRole;

                                // Normalize role
                                string normalizedRole = userRole.ToLowerInvariant().Trim();

                                // Determine redirect URL
                                string redirectUrl = null;

                                // Try multiple comparison methods
                                if (normalizedRole == "student" ||
                                    string.Equals(userRole, "student", StringComparison.OrdinalIgnoreCase) ||
                                    userRole.ToLowerInvariant().Contains("student"))
                                {
                                    redirectUrl = "~/studentContent/studentpanel.aspx";
                                    Response.Write($"<div style='background:green;color:white;padding:5px;'>DEBUG: Student role matched! Redirecting to: {redirectUrl}</div>");
                                }
                                else if (normalizedRole == "tutor" ||
                                         string.Equals(userRole, "tutor", StringComparison.OrdinalIgnoreCase))
                                {
                                    redirectUrl = "~/tutorContent/tutorpanel.aspx";
                                }
                                else if (normalizedRole == "admin" ||
                                         string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    redirectUrl = "~/adminContent/adminpanel.aspx";
                                }

                                if (!string.IsNullOrEmpty(redirectUrl))
                                {
                                    Response.Redirect(redirectUrl, false);
                                    Context.ApplicationInstance.CompleteRequest();
                                    return;
                                }
                                else
                                {
                                    Response.Write($"<p style='color:orange; font-weight:bold;'>Login successful, but role '{userRole}' is unassigned.</p>");
                                }
                            }
                            else
                            {
                                Response.Write("<p style='color:red; font-weight:bold;'>Invalid Email or Password. Please try again.</p>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<p style='color:red; font-weight:bold;'>Login Error: " + HttpUtility.HtmlEncode(ex.Message) + "</p>");
                    }
                }
            }
        }

        protected void createAcct_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/registerpage.aspx");
        }
    }
}
    