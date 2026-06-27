using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
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



            }
            bindData();
        }


        private void bindData() {
            string xmlpath = Server.MapPath("/App_Data/categoryList.xml");
            if (File.Exists(xmlpath)) {
                using (DataSet ds = new DataSet()) {
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
            if (ddlCategory.SelectedValue != "0")
            {
                // Access values via SelectedItem or SelectedValue properties
                string selectedText = ddlCategory.SelectedItem.Text;
                string selectedValue = ddlCategory.SelectedValue;


            }
            else
            {

            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string cName = txtcoursename.Text.Trim();
            string cdesc = txtcrsdesc.Text.Trim();
            string fileupload = Path.GetFileName(fileUploadImage.FileName);
            string category = ddlCategory.Text.Trim();
            
            string location = locationtxt.Text.Trim();
            string experience = exptxt.Text.Trim();
            string c_time = rxtTime.Text.Trim();

            if (!double.TryParse(feetxt.Text.Trim(), out double fee))
            {
                Response.Write("<script>alert('Please enter a valid numeric fee.');</script>");
                return;
            }

            string query = "INSERT INTO tblcourses (c_name, c_desc, c_price, category, image, timetable, location, experience) VALUES (@c_name, @c_desc, @c_price, @category, @image, @timetable, @location, @experience);";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@c_name", cName);
                    cmd.Parameters.AddWithValue("@c_desc", cdesc);
                    cmd.Parameters.AddWithValue("@c_price", fee);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@image", fileupload);
                    cmd.Parameters.AddWithValue("@timetable", c_time);
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.Parameters.AddWithValue("@experience", experience);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Response.Write("Success, course added successfully");
                        }
                        else
                        {
                            Response.Write("Failed to insert data");
                        }

                    }
                    catch (Exception ex)
                    {

                        Response.Write(ex.Message);
                    }


                }
            }
        }
    }
}