using System;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.adminContent
{
    public partial class manageusers : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers(null, null);
            }
        }

        private void LoadUsers(string searchTerm, string roleFilter)
        {
            string query = @"SELECT id, firstname, lastname, email, role, ""dateCreated"", ""isFrozen""
                              FROM tblusers
                              WHERE 1=1 ";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += @" AND (firstname ILIKE @search
                                 OR lastname ILIKE @search
                                 OR email ILIKE @search) ";
            }

            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                query += " AND role = @role ";
            }

            query += " ORDER BY \"dateCreated\" DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm.Trim() + "%");
                }
                if (!string.IsNullOrWhiteSpace(roleFilter))
                {
                    cmd.Parameters.AddWithValue("@role", roleFilter);
                }

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            usersRepeater.DataSource = dt;
                            usersRepeater.DataBind();
                            usersRepeater.Visible = true;
                            lblNoRecords.Visible = false;
                        }
                        else
                        {
                            usersRepeater.Visible = false;
                            lblNoRecords.Visible = true;
                            lblNoRecords.Text = string.IsNullOrWhiteSpace(searchTerm)
                                ? "No users found."
                                : "No users found matching \"" + searchTerm + "\".";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error loading users: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text, ddlRoleFilter.SelectedValue);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlRoleFilter.SelectedValue = "";
            LoadUsers(null, null);
        }

        protected void ddlRoleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text, ddlRoleFilter.SelectedValue);
        }

        protected void usersRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ToggleFreeze")
            {
                string[] parts = e.CommandArgument.ToString().Split('|');
                string userId = parts[0];
                bool currentlyFrozen = bool.Parse(parts[1]);

                ToggleFreeze(userId, !currentlyFrozen);
            }
        }

        private void ToggleFreeze(string userId, bool newFrozenState)
        {
           
            if (Session["UserID"] != null && Session["UserID"].ToString() == userId && newFrozenState)
            {
                ShowMessage("You cannot freeze your own account while logged in.", "danger");
                LoadUsers(txtSearch.Text, ddlRoleFilter.SelectedValue);
                return;
            }

            string query = "UPDATE tblusers SET \"isFrozen\" = @frozen WHERE id = @id;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@frozen", newFrozenState);
                cmd.Parameters.AddWithValue("@id", Guid.Parse(userId));

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage(newFrozenState ? "Account frozen successfully." : "Account unfrozen successfully.", "success");
                    }
                    else
                    {
                        ShowMessage("User not found.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error updating account status: " + ex.Message, "danger");
                }
            }

            LoadUsers(txtSearch.Text, ddlRoleFilter.SelectedValue);
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = type == "success" ? "alert alert-success"
                                 : type == "danger" ? "alert alert-danger"
                                 : "alert alert-info";
            lblMessage.Visible = true;
        }
    }
}