using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.studentContent
{
    public partial class addrecommendation : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in students can add recommendations
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadMyRecommendations();
            }
        }

        // tblrecommendation stores both student_id (tblstudent.s_id) and
        // user_id (tblusers.id). Session only holds the user id, so we look
        // up the matching student row here.
        private int? GetStudentId(Guid userId)
        {
            string query = "SELECT s_id FROM tblstudent WHERE user_id = @userId;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
            }
        }

        private void LoadMyRecommendations()
        {
            Guid userId = (Guid)Session["UserID"];

            string query = @"
                SELECT r_id, recommendation_title, recommendation_type, description, confidence_score, status, createdat
                FROM tblrecommendation
                WHERE user_id = @userId
                ORDER BY createdat DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            rptMyRecommendations.DataSource = dt;
                            rptMyRecommendations.DataBind();
                            lblNoRecommendations.Visible = false;
                        }
                        else
                        {
                            rptMyRecommendations.DataSource = null;
                            rptMyRecommendations.DataBind();
                            lblNoRecommendations.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading your recommendations: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Guid userId = (Guid)Session["UserID"];
            int? studentId = GetStudentId(userId);

            if (studentId == null)
            {
                lblMessage.Text = "Could not find your student profile. Please contact support.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string title = txtTitle.Text.Trim();
            string type = ddlType.SelectedValue;
            string description = txtDescription.Text.Trim();
            int confidenceScore = int.Parse(rblConfidence.SelectedValue);

            string query = @"
                INSERT INTO tblrecommendation
                    (student_id, user_id, recommendation_title, recommendation_type, description, confidence_score, status, createdat, updatedat)
                VALUES
                    (@studentId, @userId, @title, @type, @description, @confidence, @status, NOW(), NOW());";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId.Value);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@confidence", confidenceScore);
                cmd.Parameters.AddWithValue("@status", "Pending");

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Recommendation submitted successfully.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;

                    // Reset the form
                    txtTitle.Text = "";
                    txtDescription.Text = "";
                    ddlType.SelectedIndex = 0;
                    rblConfidence.SelectedValue = "66";
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error submitting recommendation: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }

            LoadMyRecommendations();
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Approved":
                    return "bg-success";
                case "Rejected":
                    return "bg-danger";
                case "Pending":
                default:
                    return "bg-warning text-dark";
            }
        }
    }
}
