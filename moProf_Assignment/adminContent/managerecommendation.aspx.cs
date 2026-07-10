using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class managerecommendations : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in tutors can manage recommendations
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRecommendations();
            }
        }

        private void LoadRecommendations()
        {
            string query = @"
                SELECT r.r_id, r.recommendation_title, r.recommendation_type, r.description,
                       r.confidence_score, r.status, r.createdat, u.firstname, u.lastname
                FROM tblrecommendation r
                JOIN tblusers u ON r.user_id = u.id
                ORDER BY
                    CASE WHEN r.status = 'Pending' THEN 0 ELSE 1 END,
                    r.createdat DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            rptRecommendations.DataSource = dt;
                            rptRecommendations.DataBind();
                            lblNoRecommendations.Visible = false;
                        }
                        else
                        {
                            rptRecommendations.DataSource = null;
                            rptRecommendations.DataBind();
                            lblNoRecommendations.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading recommendations: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void rptRecommendations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int rId = Convert.ToInt32(e.CommandArgument);
            string newStatus = e.CommandName == "Approve" ? "Approved" : "Rejected";

            string query = @"
                UPDATE tblrecommendation
                SET status = @status, updatedat = NOW()
                WHERE r_id = @rId;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@rId", rId);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMessage.Text = $"Recommendation {newStatus.ToLower()}.";
                    lblMessage.ForeColor = newStatus == "Approved" ? System.Drawing.Color.Green : System.Drawing.Color.Gray;
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error updating recommendation: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }

            LoadRecommendations();
        }

        protected string GetStatusBadgeClass(object statusObj)
        {
            string status = statusObj?.ToString() ?? "";

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
