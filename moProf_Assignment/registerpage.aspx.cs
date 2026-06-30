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

            // Using tbusers table with correct column names
            string userQuery = "INSERT INTO tblusers (firstName, lastName, email, password, role, \"rememberSession\") VALUES (@Fname, @Lname, @email, @pass, @role, @remberMe) RETURNING id;";

            using (var con = new NpgsqlConnection(conString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new NpgsqlCommand(userQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Fname", txtfname);
                            cmd.Parameters.AddWithValue("@Lname", txtlname);
                            cmd.Parameters.AddWithValue("@email", txtemail);
                            cmd.Parameters.AddWithValue("@pass", txtpassword);
                            cmd.Parameters.AddWithValue("@role", role);
                            cmd.Parameters.AddWithValue("@remberMe", rememberme);

                            

                            Guid newUserId = (Guid)cmd.ExecuteScalar();

                       
                            transaction.Commit();

                           
                            Session["UserID"] = newUserId.ToString();

                           
                            if (role == "student")
                            {
                                Response.Redirect("/studentContent/studentlogin.aspx");
                            }
                            else if (role == "tutor")
                            {
                                Response.Redirect("/tutorContent/tutorlogin.aspx");
                            }

                        }
                    }
                    catch (PostgresException ex)
                    {
                        transaction.Rollback();
                        if (ex.SqlState == "23505") // Unique constraint violation
                        {
                            Response.Write("Email already exists, please login using your email.");
                        }
                        else
                        {
                            Response.Write("Database error: " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Response.Write("Error: " + ex.Message);
                    }
                }
            }
        }



    }
    }
