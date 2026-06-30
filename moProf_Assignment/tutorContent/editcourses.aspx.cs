using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class editcourses : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseGrid();

                // Test database connection
                try
                {
                    using (var con = new NpgsqlConnection(conString))
                    {
                        con.Open();
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<p style='color:red;'>Connection Error: " + ex.Message + "</p>");
                }

                // Check for ID parameter to auto-load modal data
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int id))
                    {
                        LoadCourse(id);
                        string script = $"document.addEventListener('DOMContentLoaded', function() {{ var myModal = new bootstrap.Modal(document.getElementById('editCourseModal_{id}')); myModal.show(); }});";
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowEditModal", script, true);
                    }
                }
            }
        }

        // Bind courses to repeater
        private void BindCourseGrid()
        {
            string query = @"SELECT c_id, c_name, c_desc, c_price, category, image, timestable, location, experience 
                           FROM tblcourses 
                           ORDER BY c_name ASC;";

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

        // Load course data for editing
        private void LoadCourse(int id)
        {
            string query = @"SELECT c_name, c_desc, c_price, location, experience, timestable 
                           FROM tblcourses 
                           WHERE c_id = @id;";

            using (NpgsqlConnection con = new NpgsqlConnection(conString))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    try
                    {
                        con.Open();
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                foreach (RepeaterItem item in rptCourses.Items)
                                {
                                    Button btn = (Button)item.FindControl("Button1");

                                    if (btn != null && btn.CommandArgument == id.ToString())
                                    {
                                        TextBox txtcoursename = (TextBox)item.FindControl("txtcoursename");
                                        TextBox txtcrsdesc = (TextBox)item.FindControl("txtcrsdesc");
                                        TextBox feetxt = (TextBox)item.FindControl("feetxt");
                                        TextBox locationtxt = (TextBox)item.FindControl("locationtxt");
                                        TextBox exptxt = (TextBox)item.FindControl("exptxt");
                                        TextBox rxtTime = (TextBox)item.FindControl("rxtTime");

                                        if (txtcoursename != null) txtcoursename.Text = reader["c_name"].ToString();
                                        if (txtcrsdesc != null) txtcrsdesc.Text = reader["c_desc"].ToString();
                                        if (feetxt != null) feetxt.Text = reader["c_price"].ToString();
                                        if (locationtxt != null) locationtxt.Text = reader["location"].ToString();
                                        if (exptxt != null) exptxt.Text = reader["experience"].ToString();
                                        if (rxtTime != null) rxtTime.Text = reader["timestable"].ToString();

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<p style='color:red;'>Data Loading Error: " + ex.Message + "</p>");
                    }
                }
            }
        }

        // Save course updates
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string courseId = btn.CommandArgument;

            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            if (item != null)
            {
                TextBox txtCourseName = (TextBox)item.FindControl("txtcoursename");
                TextBox txtCrsDesc = (TextBox)item.FindControl("txtcrsdesc");
                TextBox txtFee = (TextBox)item.FindControl("feetxt");
                TextBox txtLocation = (TextBox)item.FindControl("locationtxt");
                TextBox txtExp = (TextBox)item.FindControl("exptxt");
                TextBox txtTime = (TextBox)item.FindControl("rxtTime");
                FileUpload fileUpload = (FileUpload)item.FindControl("fileUploadImage");

                string updatedName = txtCourseName != null ? txtCourseName.Text : "";
                string updatedDesc = txtCrsDesc != null ? txtCrsDesc.Text : "";
                string updatedFee = txtFee != null ? txtFee.Text : "0";
                string updatedLocation = txtLocation != null ? txtLocation.Text : "";
                string updatedExp = txtExp != null ? txtExp.Text : "";
                string updatedTime = txtTime != null ? txtTime.Text : "";

                UpdateCourseInDatabase(courseId, updatedName, updatedDesc, updatedFee, updatedLocation, updatedExp, updatedTime, fileUpload);
                BindCourseGrid();
            }
        }

        // Update course in database
        private void UpdateCourseInDatabase(string id, string name, string desc, string fee, string location, string exp, string time, FileUpload fileUpload)
        {
            decimal priceValue = 0;
            decimal.TryParse(fee, out priceValue);

            string imageName = null;
            if (fileUpload != null && fileUpload.HasFile)
            {
                try
                {
                    imageName = Path.GetFileName(fileUpload.FileName);
                    string folderPath = Server.MapPath("~/images/");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    fileUpload.SaveAs(Path.Combine(folderPath, imageName));
                }
                catch (Exception fileEx)
                {
                    Response.Write("<p style='color:red;'>Image Save Error: " + fileEx.Message + "</p>");
                }
            }

            string query = @"UPDATE tblcourses 
                             SET c_name = @name, c_desc = @desc, c_price = @price, 
                                 location = @location, experience = @exp, timestable = @time"
                             + (imageName != null ? ", image = @image" : "") +
                             " WHERE c_id = @id;";

            using (NpgsqlConnection con = new NpgsqlConnection(conString))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", desc);
                    cmd.Parameters.AddWithValue("@price", priceValue);
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.Parameters.AddWithValue("@exp", exp);
                    cmd.Parameters.AddWithValue("@time", time);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));

                    if (imageName != null)
                    {
                        cmd.Parameters.AddWithValue("@image", imageName);
                    }

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string successScript = "alert('Course details updated successfully!');";
                            ClientScript.RegisterStartupScript(this.GetType(), "UpdateSuccess", successScript, true);
                        }
                        else
                        {
                            Response.Write("<p style='color:red;'>Course not found or you don't have permission to update it.</p>");
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<p style='color:red;'>Database Update Error: " + ex.Message + "</p>");
                    }
                }
            }
        }

        // Delete course
        protected void dltbtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string courseId = btn.CommandArgument;

            if (!string.IsNullOrEmpty(courseId) && int.TryParse(courseId, out int id))
            {
                string query = "DELETE FROM tblcourses WHERE c_id = @id;";

                using (var con = new NpgsqlConnection(conString))
                {
                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        try
                        {
                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                BindCourseGrid();
                            }
                            else
                            {
                                Response.Write("<p style='color:red;'>Course not found.</p>");
                            }
                        }
                        catch (Exception ex)
                        {
                            Response.Write("<p style='color:red;'>Delete Error: " + ex.Message + "</p>");
                        }
                    }
                }
            }
        }
    }
}