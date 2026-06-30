using System;
using System.Configuration;
using Npgsql;

namespace moProf_Assignment.tutorContent
{
    public partial class editacct : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/tutorContent/tutorlogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserData();
            }
        }

        private void LoadUserData()
        {
            try
            {
                // Get UserID from session as GUID
                Guid userId = (Guid)Session["UserID"];

                using (NpgsqlConnection con = new NpgsqlConnection(conString))
                {
                    con.Open();

                    string query = @"
                    SELECT
                        u.firstname,
                        u.lastname,
                        u.email,
                        u.password,
                        t.t_exp
                    FROM tblusers u
                    LEFT JOIN tbltutor t
                        ON u.id = t.user_id
                    WHERE u.id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFirstName.Text = reader["firstname"].ToString();
                                txtLastName.Text = reader["lastname"].ToString();
                                txtEmail.Text = reader["email"].ToString();

                                if (reader["t_exp"] != DBNull.Value)
                                    txtExperience.Text = reader["t_exp"].ToString();
                            }
                            else
                            {
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                                lblMessage.Text = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error loading data: " + ex.Message;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Get UserID from session as GUID
                Guid userId = (Guid)Session["UserID"];

                using (NpgsqlConnection con = new NpgsqlConnection(conString))
                {
                    con.Open();

                    NpgsqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // Update user table
                        string updateUser = @"
                        UPDATE tblusers
                        SET
                            firstname = @fname,
                            lastname = @lname,
                            email = @email
                        WHERE id = @id";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(updateUser, con))
                        {
                            cmd.Transaction = transaction;

                            cmd.Parameters.AddWithValue("@fname", txtFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@lname", txtLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@id", userId);

                            cmd.ExecuteNonQuery();
                        }

                        // Update tutor table
                        string updateTutor = @"
                        UPDATE tbltutor
                        SET t_exp = @exp
                        WHERE user_id = @id";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(updateTutor, con))
                        {
                            cmd.Transaction = transaction;

                            int exp = 0;
                            int.TryParse(txtExperience.Text, out exp);

                            cmd.Parameters.AddWithValue("@exp", exp);
                            cmd.Parameters.AddWithValue("@id", userId);

                            cmd.ExecuteNonQuery();
                        }

                        // Change password only if entered
                        if (!string.IsNullOrWhiteSpace(txtNewPassword.Text))
                        {
                            if (txtNewPassword.Text != txtConfirmPassword.Text)
                            {
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                                lblMessage.Text = "New passwords do not match.";
                                transaction.Rollback();
                                return;
                            }

                            string currentPassword = "";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(
                                "SELECT password FROM tblusers WHERE id = @id", con))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@id", userId);

                                currentPassword = cmd.ExecuteScalar().ToString();
                            }

                            if (currentPassword != txtCurrentPassword.Text)
                            {
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                                lblMessage.Text = "Current password is incorrect.";
                                transaction.Rollback();
                                return;
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(
                                "UPDATE tblusers SET password = @pass WHERE id = @id", con))
                            {
                                cmd.Transaction = transaction;

                                cmd.Parameters.AddWithValue("@pass", txtNewPassword.Text);
                                cmd.Parameters.AddWithValue("@id", userId);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Account updated successfully.";

                        // Reload the data to show updated values
                        LoadUserData();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Error: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}