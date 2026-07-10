using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class TutorPanel : System.Web.UI.Page
    {
        // Reference connection string from web.config
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                    return 0;
                return (int)ViewState["CurrentPage"];
            }
            set { ViewState["CurrentPage"] = value; }
        }

        // Keeps the active search term across Prev/Next postbacks, so paging
        // doesn't silently drop the filter.
        private string SearchTerm
        {
            get
            {
                return ViewState["SearchTerm"] as string ?? "";
            }
            set { ViewState["SearchTerm"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentPage = 0;
                SearchTerm = "";
                BindTutorRepeater();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            CurrentPage = 0; // reset to first page whenever the search changes
            BindTutorRepeater();
        }

        private void BindTutorRepeater()
        {
            DataTable dtTutors = FetchAvailableTutors(SearchTerm);

            if (dtTutors == null || dtTutors.Rows.Count == 0)
            {
                // Handle no data case
                rptTutors.DataSource = null;
                rptTutors.DataBind();
                lblCurrentPage.Text = "0";
                lblTotalPages.Text = "0";
                lnkPrev.Enabled = false;
                lnkNext.Enabled = false;
                lblNoResults.Visible = true;
                return;
            }

            lblNoResults.Visible = false;

            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dtTutors.DefaultView;
            pds.AllowPaging = true;
            pds.PageSize = 6; // Show 6 tutors per page
            pds.CurrentPageIndex = CurrentPage;

            // Handle display logic for pagination metrics
            lblCurrentPage.Text = (CurrentPage + 1).ToString();
            lblTotalPages.Text = pds.PageCount == 0 ? "1" : pds.PageCount.ToString();

            // Disable pagination buttons when out of bounds
            lnkPrev.Enabled = !pds.IsFirstPage;
            lnkNext.Enabled = !pds.IsLastPage;

            lnkPrev.CssClass = pds.IsFirstPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";
            lnkNext.CssClass = pds.IsLastPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";

            rptTutors.DataSource = pds;
            rptTutors.DataBind();
        }

        private DataTable FetchAvailableTutors(string searchTerm)
        {
            DataTable dt = new DataTable();

            // Query with JOIN to get firstname and lastname from tblusers.
            // When a search term is supplied, filter by first/last name
            // (case-insensitive partial match).
            string query = @"
                SELECT 
                    t.t_id, 
                    t.t_exp, 
                    t.total_student, 
                    t.total_review, 
                    t.""isAvailable"", 
                    t.user_id, 
                    t.created_at, 
                    t.updated_at,
                    u.firstname,
                    u.lastname
                FROM 
                    tbltutor t
                INNER JOIN 
                    tblusers u ON t.user_id = u.id
                WHERE 
                    t.""isAvailable"" = true";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += @"
                    AND (u.firstname ILIKE @search OR u.lastname ILIKE @search OR (u.firstname || ' ' || u.lastname) ILIKE @search)";
            }

            query += @"
                ORDER BY 
                    t.created_at DESC";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    }

                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<p style='color:red; font-weight:bold;'>Failed to load tutor information: " + ex.Message + "</p>");
                    }
                }
            }
            return dt;
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            CurrentPage -= 1;
            BindTutorRepeater();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            CurrentPage += 1;
            BindTutorRepeater();
        }

        // Helper method to get experience width - returns string with % sign
        public string GetExperienceWidth(object expValue)
        {
            try
            {
                int experience = Convert.ToInt32(expValue);
                // Cap at 100% and ensure it's not negative
                experience = Math.Max(0, Math.Min(100, experience));
                return experience.ToString() + "%";
            }
            catch
            {
                return "0%";
            }
        }

        protected void lnkViewProfile_Click(object sender, EventArgs e)
        {
            // Get the button that was clicked
            LinkButton btn = (LinkButton)sender;

            // Get the user ID from the button's CommandArgument
            string userId = btn.CommandArgument;

            // Check if we have a valid ID
            if (!string.IsNullOrEmpty(userId))
            {
                // Redirect with the user ID
                Response.Redirect("~/tutorContent/tutorprofile.aspx?id=" + userId);
            }
            else
            {
                // If no ID, show error
                Response.Write("<script>alert('No tutor ID found!');</script>");
            }
        }
    }
}
