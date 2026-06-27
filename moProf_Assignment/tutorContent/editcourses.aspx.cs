using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class editcourses : System.Web.UI.Page
    {
        String conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    using (var con = new NpgsqlConnection(conString))
                    {
                        con.Open();
                        //Response.Write("Database Connection success" );

                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Database Connection Failed: " + ex.Message);
                }
            }
            BindCourseGrid();

            }

        private void BindCourseGrid()
        {
      
            string query = "SELECT c_name, c_desc, c_price, category, image, timetable, location, experience FROM tblcourses ORDER BY c_name ASC;";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    try
                    {
                        con.Open();

                       
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            
                            rptCourses.DataSource = dt;
                            rptCourses.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                       
                        Response.Write("<p style='color:red; font-weight:bold;'>Failed to load courses: " + ex.Message + "</p>");
                    }
                }
            }
        }
    }
}
    
