using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class index : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRecommendedCourses();
            }
        }

        // Pulls the top 3 courses ranked by number of accepted bookings
        // (most popular first). Falls back to newest courses if there are
        // no bookings yet, since COUNT(...) ties still sort by c_id DESC.
        private void LoadRecommendedCourses()
        {
            string query = @"
                SELECT
                    c.c_id,
                    c.c_name,
                    c.c_desc,
                    c.c_price,
                    c.category,
                    c.image,
                    c.location,
                    u.firstname,
                    u.lastname,
                    COUNT(br.br_id) FILTER (WHERE br.isaccepted = true) AS booking_count
                FROM tblcourses c
                JOIN tbltutor t ON c.tutor_id = t.t_id
                JOIN tblusers u ON t.user_id = u.id
                LEFT JOIN tblbookingrequest br ON br.c_id = c.c_id
                GROUP BY c.c_id, c.c_name, c.c_desc, c.c_price, c.category, c.image, c.location, u.firstname, u.lastname
                ORDER BY booking_count DESC, c.c_id DESC
                LIMIT 3;";

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
                            rptRecommendedCourses.DataSource = dt;
                            rptRecommendedCourses.DataBind();
                            lblNoCourses.Visible = false;
                        }
                        else
                        {
                            rptRecommendedCourses.DataSource = null;
                            rptRecommendedCourses.DataBind();
                            lblNoCourses.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    rptRecommendedCourses.DataSource = null;
                    rptRecommendedCourses.DataBind();
                    lblNoCourses.Visible = true;
                    lblNoCourses.Text = "Unable to load courses right now.";
                }
            }
        }
    }
}