using Npgsql;
using System;
using System.Configuration;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.tutorContent
{
    public partial class editcoursesadmin : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session check - only logged-in admins can manage all courses.
            // Assumes tblusers.role holds "Admin" and that your login flow
            // stores it in Session["Role"]. Adjust the session key/value if
            // your app names them differently.
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

                // Check for ID parameter to auto-load a modal
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int id))
                    {
                        string action = Request.QueryString["action"];

                        if (action == "view")
                        {
                            // "View Student Enrolled" link -> open the students modal for this course.
                            // The enrolled-students data was already bound for every course card
                            // inside rptCourses_ItemDataBound (called during BindCourseGrid above),
                            // so we just need to pop the right modal open.
                            string script = $"document.addEventListener('DOMContentLoaded', function() {{ var myModal = new bootstrap.Modal(document.getElementById('studentsModal_{id}')); myModal.show(); }});";
                            ClientScript.RegisterStartupScript(this.GetType(), "ShowStudentsModal", script, true);
                        }
                        else
                        {
                            // "Edit Course" link -> load the course fields and open the edit modal.
                            LoadCourse(id);
                            string script = $"document.addEventListener('DOMContentLoaded', function() {{ var myModal = new bootstrap.Modal(document.getElementById('editCourseModal_{id}')); myModal.show(); }});";
                            ClientScript.RegisterStartupScript(this.GetType(), "ShowEditModal", script, true);
                        }
                    }
                }
            }
        }

        // Bind ALL courses (across every tutor) to the repeater, including
        // the owning tutor's name so admins can tell them apart.
        private void BindCourseGrid()
        {
            string query = @"
                SELECT
                    c.c_id, c.c_name, c.c_desc, c.c_price, c.category, c.image,
                    c.timestable, c.location, c.experience,
                    u.firstname AS tutor_firstname,
                    u.lastname AS tutor_lastname
                FROM tblcourses c
                JOIN tbltutor t ON c.tutor_id = t.t_id
                JOIN tblusers u ON t.user_id = u.id
                ORDER BY c.c_name ASC;";

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

        // Fires once per course card as rptCourses binds. Loads and binds the
        // list of enrolled students into that card's nested rptEnrolledStudents.
        protected void rptCourses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView drv = e.Item.DataItem as DataRowView;
            if (drv == null) return;

            int cId = Convert.ToInt32(drv["c_id"]);

            Repeater rptEnrolledStudents = (Repeater)e.Item.FindControl("rptEnrolledStudents");
            Label lblNoStudents = (Label)e.Item.FindControl("lblNoStudents");

            DataTable dtStudents = GetEnrolledStudents(cId);

            if (dtStudents.Rows.Count > 0)
            {
                rptEnrolledStudents.DataSource = dtStudents;
                rptEnrolledStudents.DataBind();
                if (lblNoStudents != null) lblNoStudents.Visible = false;
            }
            else
            {
                rptEnrolledStudents.DataSource = null;
                rptEnrolledStudents.DataBind();
                if (lblNoStudents != null) lblNoStudents.Visible = true;
            }
        }

        // Returns the students enrolled (accepted booking) for a given course
        private DataTable GetEnrolledStudents(int courseId)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT u.firstname, u.lastname, u.email, br.booking_date
                FROM tblbookingrequest br
                JOIN tblstudent s ON br.s_id = s.s_id
                JOIN tblusers u ON s.user_id = u.id
                WHERE br.c_id = @cid AND br.isaccepted = true
                ORDER BY br.booking_date DESC;";

            using (var con = new NpgsqlConnection(conString))
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);

                try
                {
                    con.Open();
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<p style='color:red;'>Error loading enrolled students: " + ex.Message + "</p>");
                }
            }

            return dt;
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
