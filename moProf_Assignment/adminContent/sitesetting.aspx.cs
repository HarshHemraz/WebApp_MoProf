using System;
using System.Collections.Generic;
using System.Configuration;
using Npgsql;

namespace moProf_Assignment.adminContent
{
    public partial class sitesettings : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            var settings = GetAllSettings();

            chkRegistrationsEnabled.Checked = settings.TryGetValue("registrations_enabled", out var reg) && reg == "true";
            chkMaintenanceMode.Checked = settings.TryGetValue("maintenance_mode", out var maint) && maint == "true";
            txtSupportEmail.Text = settings.TryGetValue("support_email", out var email) ? email : "";
        }

        private Dictionary<string, string> GetAllSettings()
        {
            var result = new Dictionary<string, string>();
            string query = "SELECT setting_key, setting_value FROM tblsettings;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader["setting_key"].ToString()] = reader["setting_value"].ToString();
                    }
                }
            }

            return result;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSetting("registrations_enabled", chkRegistrationsEnabled.Checked ? "true" : "false");
                SaveSetting("maintenance_mode", chkMaintenanceMode.Checked ? "true" : "false");
                SaveSetting("support_email", txtSupportEmail.Text.Trim());

                lblMessage.Text = "Settings saved successfully.";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error saving settings: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
            }
        }

        // Upsert: update if the key exists, insert if it doesn't
        private void SaveSetting(string key, string value)
        {
            string query = @"
                INSERT INTO tblsettings (setting_key, setting_value)
                VALUES (@key, @value)
                ON CONFLICT (setting_key)
                DO UPDATE SET setting_value = @value;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@value", value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}