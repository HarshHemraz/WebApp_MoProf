using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using Npgsql;
using System.Security.Principal;
using System.Linq.Expressions;


namespace moProf_Assignment
{
    public partial class registerpage : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                try
                { 
                    using (var con = new NpgsqlConnection(conString))
                    {
                        con.Open();
                        //Response.Write("Database Connection success" );

                    }
                } catch (Exception ex)
                {
                    Response.Write("Database Connection Failed: " + ex.Message);
                }

               
        }
    }
        protected void registerBtn_Click(object sender, EventArgs e)
        {
            

            String txtfname = fname.Text.Trim();
            String txtlname = lname.Text.Trim();
            String txtemail = emailtxt.Text.Trim();
            String txtpassword = passwordtxt.Text.Trim();
            String role = RegisterOption.SelectedValue;
            bool rememberme = checkbxRemeberMe.Checked;

            string query = "INSERT INTO tblusers (firstname, lastname, email, password, role, \"rememberSession\") VALUES (@Fname,@Lname, @email, @pass, @role, @remberMe);";
            


            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con)) 
                {
                    cmd.Parameters.AddWithValue("@Fname", txtfname);
                    cmd.Parameters.AddWithValue("@Lname", txtlname);
                    cmd.Parameters.AddWithValue("@email", txtemail);
                    cmd.Parameters.AddWithValue("@pass", txtpassword);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@remberMe", rememberme);

                    try { 
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                            if (role == "student") {
                                Response.Redirect("/studentContent/studentlogin.aspx");
                            } else if (role == "tutor")
                            {
                                Response.Redirect("/tutorContent/tutorlogin.aspx");
                            }
                            
                       
                        
                    } else
                    {
                        Response.Write("Registration Unsuccessful!");
                    }

                }
                    catch (PostgresException ex)
                    {
                        if( ex.SqlState == "23505") {
                            Response.Write("Email already exists, please login using your email.");
                        } 
                       
                    }
                    catch (Exception ex)
                    {

                        Response.Write(ex.Message);
                    }
                } 
                


        }
    }




}
    }
