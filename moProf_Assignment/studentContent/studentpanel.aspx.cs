using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class studentpanel : System.Web.UI.Page
    {
        // Reference connection string from web.config exactly like editcourses page
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentPage = 0;
                BindCourseRepeater();
            }
        }

        private void BindCourseRepeater()
        {
            DataTable dtCourses = FetchAvailableCourses();

            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dtCourses.DefaultView;
            pds.AllowPaging = true;
            pds.PageSize = 6; // Set total course items visible per page
            pds.CurrentPageIndex = CurrentPage;

            // Handle display logic for pagination metrics
            lblCurrentPage.Text = (CurrentPage + 1).ToString();
            lblTotalPages.Text = pds.PageCount == 0 ? "1" : pds.PageCount.ToString();

            // Disable pagination buttons when out of bounds
            lnkPrev.Enabled = !pds.IsFirstPage;
            lnkNext.Enabled = !pds.IsLastPage;

            lnkPrev.CssClass = pds.IsFirstPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";
            lnkNext.CssClass = pds.IsLastPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";

            rptCourses.DataSource = pds;
            rptCourses.DataBind();
        }

        private DataTable FetchAvailableCourses()
        {
            DataTable dt = new DataTable();

            // PostgreSQL query verifying specific schema layout
            string query = @"SELECT c_id, c_name, c_desc, c_price, no_student, category, image, location, timestable, experience 
                             FROM tblcourses 
                             WHERE isavailable = TRUE 
                             ORDER BY created_at DESC;";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
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
                        Response.Write("<p style='color:red; font-weight:bold;'>Failed to load student course view: " + ex.Message + "</p>");
                    }
                }
            }
            return dt;
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            CurrentPage -= 1;
            BindCourseRepeater();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            CurrentPage += 1;
            BindCourseRepeater();
        }

        protected void lnkViewDetails_Click(object sender, EventArgs e)
        {
            // Get the button that was clicked
            LinkButton btn = (LinkButton)sender;

            // Get the course ID from the button's CommandArgument
            string courseId = btn.CommandArgument;

            // Check if we have a valid ID
            if (!string.IsNullOrEmpty(courseId))
            {
                // Redirect with the course ID
                Response.Redirect("~/studentContent/coursecontent.aspx?id=" + courseId);
            }
            else
            {
                // If no ID, show error
                Response.Write("<script>alert('No course ID found!');</script>");
            }
        }
    }
}