using System;
using System.Configuration;
using Npgsql;

namespace moProf_Assignment
{
    public partial class registerpage : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    using (var con = new NpgsqlConnection(conString))
                    {
                        con.Open();
                        //Response.Write("Database Connection success" );
                    }
                }
                catch (Exception ex)
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

            string userQuery = "INSERT INTO tblusers (firstName, lastName, email, password, role, \"rememberSession\") VALUES (@Fname, @Lname, @email, @pass, @role, @remberMe) RETURNING id;";

            using (var con = new NpgsqlConnection(conString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        Guid newUserId;

                        using (var cmd = new NpgsqlCommand(userQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Fname", txtfname);
                            cmd.Parameters.AddWithValue("@Lname", txtlname);
                            cmd.Parameters.AddWithValue("@email", txtemail);
                            cmd.Parameters.AddWithValue("@pass", txtpassword);
                            cmd.Parameters.AddWithValue("@role", role);
                            cmd.Parameters.AddWithValue("@remberMe", rememberme);

                            newUserId = (Guid)cmd.ExecuteScalar();
                        }

                        if (role == "student")
                        {
                            string studentQuery = @"
                                INSERT INTO tblstudent (user_id)
                                VALUES (@user_id)";

                            using (var studentCmd = new NpgsqlCommand(studentQuery, con, transaction))
                            {
                                studentCmd.Parameters.AddWithValue("@user_id", newUserId);
                                studentCmd.ExecuteNonQuery();
                            }
                        }
                        else if (role == "tutor")
                        {
                            string tutorQuery = @"
                                INSERT INTO tbltutor (user_id)
                                VALUES (@user_id)";

                            using (var tutorCmd = new NpgsqlCommand(tutorQuery, con, transaction))
                            {
                                tutorCmd.Parameters.AddWithValue("@user_id", newUserId);
                                tutorCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        // Store as Guid, not string, so downstream (Guid)Session["UserID"] casts work
                        Session["UserID"] = newUserId;

                        if (role == "student")
                        {
                            Response.Redirect("/studentContent/studentlogin.aspx");
                        }
                        else if (role == "tutor")
                        {
                            Response.Redirect("/tutorContent/tutorlogin.aspx");
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