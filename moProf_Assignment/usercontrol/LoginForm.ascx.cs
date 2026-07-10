using Npgsql;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
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

            string query = "SELECT id, firstname, lastname, role FROM tblusers WHERE email = @email AND password = @pass;";

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
                                Guid userId = (Guid)reader["id"];
                                string userFname = reader["firstname"].ToString();
                                string userLname = reader["lastname"].ToString();
                                string userRole = reader["role"].ToString().Trim();

                                Session["UserID"] = userId;
                                Session["UserEmail"] = txtemail;
                                Session["UserFirstName"] = userFname;
                                Session["UserLastName"] = userLname;
                                Session["UserRole"] = userRole;

                                // REMOVED: FormsAuthentication.SetAuthCookie(...) — not usable with authentication mode="None"

                                string normalizedRole = userRole.ToLowerInvariant().Trim();
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
                                    return;
                                }
                                else
                                {
                                    Response.Write($"<p style='color:orange; font-weight:bold;'>Login successful, but role '{HttpUtility.HtmlEncode(userRole)}' is unassigned.</p>");
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