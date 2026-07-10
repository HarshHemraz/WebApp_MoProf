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
            if (!IsPostBack)
            {
                LoadTutor();
            }
        }

        private void LoadTutor()
        {
            string email = Request.QueryString["email"];

            if (string.IsNullOrEmpty(email))
            {
                lblMessage.Text = "Tutor not found.";
                return;
            }

            using (NpgsqlConnection con = new NpgsqlConnection(conString))
            {
                con.Open();

                string query = @"
                    SELECT u.firstname,
                           u.lastname,
                           u.email,
                           t.""isAvailable"",
                           t.t_exp
                    FROM tblusers u
                    JOIN tbltutor t
                    ON u.id = t.user_id
                    WHERE u.email = @email
                    AND u.role = 'tutor'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@email", email);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            txtFirstName.Text = dr["firstname"].ToString();
                            txtLastName.Text = dr["lastname"].ToString();
                            txtEmail.Text = dr["email"].ToString();
                            chkIsAvailable.Checked = Convert.ToBoolean(dr["isAvailable"]);
                            txtExperience.Text = dr["t_exp"].ToString();
                        }
                        else
                        {
                            lblMessage.Text = "Tutor not found.";
                        }
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "New passwords do not match.";
                return;
            }

            using (NpgsqlConnection con = new NpgsqlConnection(conString))
            {
                con.Open();

                string checkQuery = @"
                    SELECT id
                    FROM tblusers
                    WHERE email = @email
                    AND password = @password
                    AND role = 'tutor'";

                using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@password", txtCurrentPassword.Text.Trim());

                    object result = checkCmd.ExecuteScalar();

                    if (result == null)
                    {
                        lblMessage.Text = "Current password is incorrect.";
                        return;
                    }

                    string updateUserQuery = @"
                        UPDATE tblusers
                        SET firstname = @firstname,
                            lastname = @lastname,
                            email = @newemail,
                            password = @newpassword
                        WHERE id = @id";

                    using (NpgsqlCommand updateCmd = new NpgsqlCommand(updateUserQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@firstname", txtFirstName.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@lastname", txtLastName.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@newemail", txtEmail.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@newpassword", txtNewPassword.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@id", result);

                        int rows = updateCmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            int experience;
                            if (!int.TryParse(txtExperience.Text.Trim(), out experience))
                            {
                                experience = 0;
                            }

                            string updateTutorQuery = @"
                                UPDATE tbltutor
                                SET ""isAvailable"" = @available,
                                t_exp = @experience
                                WHERE user_id = @userid;";

                            using (NpgsqlCommand tutorCmd = new NpgsqlCommand(updateTutorQuery, con))
                            {
                                tutorCmd.Parameters.AddWithValue("@available", chkIsAvailable.Checked);
                                tutorCmd.Parameters.AddWithValue("@experience", experience);
                                tutorCmd.Parameters.AddWithValue("@userid", result);

                                tutorCmd.ExecuteNonQuery();
                            }

                            lblMessage.ForeColor = System.Drawing.Color.Green;
                            lblMessage.Text = "Account updated successfully.";
                        }
                        else
                        {
                            lblMessage.Text = "Update failed.";
                        }
                    }
                }
            }
        }
    }
}