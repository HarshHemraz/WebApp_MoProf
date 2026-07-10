using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class studentpanel : System.Web.UI.Page
    {
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

        // NEW: stores the active search term across postbacks
        private string SearchTerm
        {
            get
            {
                return ViewState["SearchTerm"] == null ? "" : ViewState["SearchTerm"].ToString();
            }
            set { ViewState["SearchTerm"] = value; }
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
            DataTable dtCourses = FetchAvailableCourses(SearchTerm);

            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dtCourses.DefaultView;
            pds.AllowPaging = true;
            pds.PageSize = 6;
            pds.CurrentPageIndex = CurrentPage;

            lblCurrentPage.Text = (CurrentPage + 1).ToString();
            lblTotalPages.Text = pds.PageCount == 0 ? "1" : pds.PageCount.ToString();

            lnkPrev.Enabled = !pds.IsFirstPage;
            lnkNext.Enabled = !pds.IsLastPage;

            lnkPrev.CssClass = pds.IsFirstPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";
            lnkNext.CssClass = pds.IsLastPage ? "btn btn-secondary btn-sm disabled" : "btn btn-primary btn-sm";

            rptCourses.DataSource = pds;
            rptCourses.DataBind();
        }

        // MODIFIED: added optional searchTerm parameter, defaults to no filter if empty
        private DataTable FetchAvailableCourses(string searchTerm = "")
        {
            DataTable dt = new DataTable();

            string query = @"SELECT c_id, c_name, c_desc, c_price, no_student, category, image, location, timestable, experience 
                             FROM tblcourses 
                             WHERE isavailable = TRUE ";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += @" AND (c_name ILIKE @search OR c_desc ILIKE @search OR category ILIKE @search OR location ILIKE @search) ";
            }

            query += " ORDER BY created_at DESC;";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm.Trim() + "%");
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

        // NEW: handles the Search button click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            CurrentPage = 0; // reset to page 1 whenever a new search runs
            BindCourseRepeater();
        }

        protected void lnkViewDetails_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string courseId = btn.CommandArgument;

            if (!string.IsNullOrEmpty(courseId))
            {
                Response.Redirect("~/studentContent/coursecontent.aspx?id=" + courseId);
            }
            else
            {
                Response.Write("<script>alert('No course ID found!');</script>");
            }
        }
    }
}