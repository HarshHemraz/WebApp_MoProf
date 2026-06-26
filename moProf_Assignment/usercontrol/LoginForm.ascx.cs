using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using Npgsql;
using System.Security.Principal;
using System.Linq.Expressions;

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

                // Setup the toggle button display text

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

                               
                                Session["UserEmail"] = txtemail;
                                Session["UserFirstName"] = userFname;
                                Session["UserLastName"] = userLname;
                                Session["UserRole"] = userRole;

                                
                                if (string.Equals(userRole, "student", StringComparison.OrdinalIgnoreCase))
                                {
                                    Response.Redirect("/studentContent/studentpanel.aspx", false);
                                    Context.ApplicationInstance.CompleteRequest();
                                }
                                else if (string.Equals(userRole, "tutor", StringComparison.OrdinalIgnoreCase))
                                {
                                    Response.Redirect("/tutorContent/tutorpanel.aspx", false);
                                    Context.ApplicationInstance.CompleteRequest();
                                }
                                else
                                {
                                    Response.Write("<p style='color:orange; font-weight:bold;'>Login successful, but role is unassigned.</p>");
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
