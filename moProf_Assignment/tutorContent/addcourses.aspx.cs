using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace moProf_Assignment.tutorContent
{
    public partial class addcourses : System.Web.UI.Page
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
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Database Connection Failed: " + ex.Message);
                }
                bindData();
            }

            
        }

        private void bindData()
        {
            string xmlpath = Server.MapPath("/App_Data/categoryList.xml");

            if (File.Exists(xmlpath))
            {
                using (DataSet ds = new DataSet())
                {
                    ds.ReadXml(xmlpath);

                    ddlCategory.DataSource = ds;
                    ddlCategory.DataValueField = "Id";
                    ddlCategory.DataTextField = "Name";
                    
                    ddlCategory.DataBind();
                }

                ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", "0"));
            }
        }
     
        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // optional
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string cName = txtcoursename.Text.Trim();
            string cdesc = txtcrsdesc.Text.Trim();
            string category = ddlCategory.SelectedItem.Text;
            string location = locationtxt.Text.Trim();
            string experience = exptxt.Text.Trim();
            string c_time = rxtTime.Text.Trim();

            if (!double.TryParse(feetxt.Text.Trim(), out double fee))
            {
                Response.Write("<script>alert('Please enter a valid numeric fee.');</script>");
                return;
            }

            if (!fileUploadImage.HasFile)
            {
                Response.Write("<script>alert('Please select an image.');</script>");
                return;
            }

            // Get file name
            string fileName = Path.GetFileName(fileUploadImage.FileName);

            // Save folder
            string folderPath = Server.MapPath("~/CourseImages/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Save image file
            fileUploadImage.SaveAs(Path.Combine(folderPath, fileName));

            // STORE ONLY FILE NAME (as you requested)
            string imageName = fileName;

            string query = @"INSERT INTO tblcourses
                            (c_name, c_desc, c_price, category, image, timestable, location, experience)
                            VALUES
                            (@c_name, @c_desc, @c_price, @category, @image, @timetable, @location, @experience);";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@c_name", cName);
                    cmd.Parameters.AddWithValue("@c_desc", cdesc);
                    cmd.Parameters.AddWithValue("@c_price", fee);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@image", imageName);
                    cmd.Parameters.AddWithValue("@timetable", c_time);
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.Parameters.AddWithValue("@experience", experience);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        Response.Write("<script>alert('Course added successfully!');</script>");

                        txtcoursename.Text = "";
                        txtcrsdesc.Text = "";
                        feetxt.Text = "";
                        locationtxt.Text = "";
                        exptxt.Text = "";
                        rxtTime.Text = "";
                        ddlCategory.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('" + ex.Message.Replace("'", "") + "');</script>");
                    }
                }
            }
        }
    }
}