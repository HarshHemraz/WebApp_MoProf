using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class adminpanel : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in admins can view analytics.
            // Assumes tblusers.role holds "Admin" and your login flow stores
            // it in Session["Role"]. Adjust the session key/value if your
            // app names them differently.
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (Session["Role"] != null && Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSummaryStats();
                LoadCoursesBySubject();
                LoadBookingsByStatus();
                LoadStudentGrowth();
            }
        }

        private int GetScalarInt(string query)
        {
            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        private void LoadSummaryStats()
        {
            try
            {
                lblTotalStudents.Text = GetScalarInt("SELECT COUNT(*) FROM tblstudent;").ToString();
                lblTotalTutors.Text = GetScalarInt("SELECT COUNT(*) FROM tbltutor;").ToString();
                lblTotalCourses.Text = GetScalarInt("SELECT COUNT(*) FROM tblcourses;").ToString();
                lblTotalBookings.Text = GetScalarInt("SELECT COUNT(*) FROM tblbookingrequest;").ToString();
            }
            catch (Exception ex)
            {
                Response.Write("<p style='color:red;'>Error loading summary stats: " + ex.Message + "</p>");
            }
        }

        // Pie chart: how many courses exist per subject/category
        private void LoadCoursesBySubject()
        {
            List<string> labels = new List<string>();
            List<int> data = new List<int>();

            string query = @"
                SELECT category, COUNT(*) AS cnt
                FROM tblcourses
                GROUP BY category
                ORDER BY cnt DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            labels.Add(reader["category"]?.ToString() ?? "Uncategorized");
                            data.Add(Convert.ToInt32(reader["cnt"]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<p style='color:red;'>Error loading course subjects: " + ex.Message + "</p>");
                }
            }

            var serializer = new JavaScriptSerializer();
            litSubjectLabels.Text = serializer.Serialize(labels);
            litSubjectData.Text = serializer.Serialize(data);
            lblNoSubjectData.Visible = labels.Count == 0;
        }

        // Doughnut chart: booking requests broken down by Pending / Accepted / Rejected
        private void LoadBookingsByStatus()
        {
            List<string> labels = new List<string> { "Pending", "Accepted", "Rejected" };
            List<int> data = new List<int> { 0, 0, 0 };

            string query = @"
                SELECT
                    COUNT(*) FILTER (WHERE isaccepted IS NULL) AS pending,
                    COUNT(*) FILTER (WHERE isaccepted = true) AS accepted,
                    COUNT(*) FILTER (WHERE isaccepted = false) AS rejected
                FROM tblbookingrequest;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data[0] = Convert.ToInt32(reader["pending"]);
                            data[1] = Convert.ToInt32(reader["accepted"]);
                            data[2] = Convert.ToInt32(reader["rejected"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<p style='color:red;'>Error loading booking status: " + ex.Message + "</p>");
                }
            }

            var serializer = new JavaScriptSerializer();
            litStatusLabels.Text = serializer.Serialize(labels);
            litStatusData.Text = serializer.Serialize(data);
            lblNoBookingData.Visible = data[0] == 0 && data[1] == 0 && data[2] == 0;
        }

        // Line chart: new student sign-ups per month for the last 6 months
        private void LoadStudentGrowth()
        {
            List<string> labels = new List<string>();
            List<int> data = new List<int>();

            string query = @"
                SELECT
                    TO_CHAR(DATE_TRUNC('month', createdat), 'Mon YYYY') AS month_label,
                    DATE_TRUNC('month', createdat) AS month_sort,
                    COUNT(*) AS cnt
                FROM tblstudent
                WHERE createdat >= NOW() - INTERVAL '6 months'
                GROUP BY DATE_TRUNC('month', createdat), TO_CHAR(DATE_TRUNC('month', createdat), 'Mon YYYY')
                ORDER BY month_sort;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            labels.Add(reader["month_label"].ToString());
                            data.Add(Convert.ToInt32(reader["cnt"]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<p style='color:red;'>Error loading student growth: " + ex.Message + "</p>");
                }
            }

            var serializer = new JavaScriptSerializer();
            litGrowthLabels.Text = serializer.Serialize(labels);
            litGrowthData.Text = serializer.Serialize(data);
            lblNoGrowthData.Visible = labels.Count == 0;
        }
    }
}
