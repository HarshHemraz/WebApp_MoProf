using System;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.studentContent
{
    public partial class coursecontent : System.Web.UI.Page
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get course ID from query string
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int courseId))
                    {
                        LoadCourseData(courseId);
                    }
                    else
                    {
                        lblMessage.Text = "Invalid course ID.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblMessage.Text = "Please login to view courses";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void LoadCourseData(int courseId)
        {
            string query = @"
                SELECT 
                    c.c_id,
                    c.c_name,
                    c.c_desc,
                    c.c_price,
                    c.category,
                    c.image,
                    c.timestable,
                    c.location,
                    c.experience,
                    c.tutor_id,
                    u.id as user_id,
                    u.firstname,
                    u.lastname,
                    u.email,
                    t.t_exp,
                    t.total_student,
                    t.total_review,
                    t.""isAvailable"" as availability,
                    t.created_at,
                    t.updated_at
                FROM tblcourses c
                LEFT JOIN tbltutor t ON c.tutor_id = t.t_id
                LEFT JOIN tblusers u ON t.user_id = u.id
                WHERE c.c_id = @courseId";

            using (var con = new NpgsqlConnection(conString))
            {
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);

                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                rptCourseDetails.DataSource = dt;
                                rptCourseDetails.DataBind();
                            }
                            else
                            {
                                lblMessage.Text = "Course not found.";
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error loading course: " + ex.Message;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }

        // Helper method to generate image HTML with default fallback
        protected string GetImageHtml(object imageObj, object courseNameObj)
        {
            string imageName = imageObj?.ToString();
            string courseName = courseNameObj?.ToString() ?? "Course";
            string defaultImage = ResolveUrl("~/images/default-course.jpg");

            if (string.IsNullOrEmpty(imageName))
            {
                // Return default image if no image name in database
                return $@"
                    <img src='{defaultImage}' 
                         alt='{courseName}' 
                         class='course-image' />";
            }

            // Build the image path
            string imagePath = ResolveUrl("~/images/" + imageName);

            return $@"
                <img src='{imagePath}' 
                     alt='{courseName}' 
                     class='course-image'
                     onerror='this.src=""{defaultImage}""' />";
        }

        // Check if tutor is assigned
        protected bool IsTutorAssigned(object firstName, object lastName)
        {
            string fname = firstName?.ToString();
            string lname = lastName?.ToString();
            return !string.IsNullOrEmpty(fname) && !string.IsNullOrEmpty(lname);
        }

        // Get "No Tutor" message HTML
        protected string GetNoTutorMessageHtml()
        {
            return @"
                <div class='no-tutor-message'>
                    <span class='icon'>👤</span>
                    <div class='title'>No Tutor Assigned Yet</div>
                    <div class='subtitle'>This course doesn't have a tutor assigned at the moment.</div>
                    <div class='subtitle' style='margin-top: 10px;'>
                        <small>Please check back later for tutor information.</small>
                    </div>
                </div>";
        }

        // Get Tutor Details HTML
        protected string GetTutorDetailsHtml(object dataItem)
        {
            var row = (DataRowView)dataItem;

            string fname = row["firstname"]?.ToString() ?? "";
            string lname = row["lastname"]?.ToString() ?? "";
            string email = row["email"]?.ToString() ?? "";
            string exp = row["t_exp"]?.ToString() ?? "0";
            string students = row["total_student"]?.ToString() ?? "0";
            string reviews = row["total_review"]?.ToString() ?? "0";

            // Handle availability
            string availHtml = "<span class='text-muted'>Not specified</span>";
            if (row["availability"] != DBNull.Value)
            {
                try
                {
                    bool isAvailable = Convert.ToBoolean(row["availability"]);
                    availHtml = isAvailable ?
                        "<span class='text-success'>✅ Available</span>" :
                        "<span class='text-danger'>❌ Not Available</span>";
                }
                catch { }
            }

            // Handle date
            string dateStr = "Not available";
            if (row["created_at"] != DBNull.Value)
            {
                try
                {
                    dateStr = Convert.ToDateTime(row["created_at"]).ToString("MMM dd, yyyy");
                }
                catch { }
            }

            return $@"
                <div class='row'>
                    <div class='col-md-6'>
                        <p><strong>Name:</strong> {fname} {lname}</p>
                        <p><strong>Email:</strong> {email}</p>
                        <p><strong>Experience:</strong> {exp} years</p>
                    </div>
                    <div class='col-md-6'>
                        <p><strong>Total Students:</strong> {students}</p>
                        <p><strong>Total Reviews:</strong> {reviews}</p>
                        <p><strong>Availability:</strong> {availHtml}</p>
                        <p><small class='text-muted'>Tutor since: {dateStr}</small></p>
                    </div>
                </div>";
        }

        // Safe string handler for NULL values
        protected string GetSafeString(object value)
        {
            return value?.ToString() ?? "Not specified";
        }


        protected void btnBooking_Click(object sender, EventArgs e)
        {
            try
            {
                string bookingDate = txtBookingDate.Text;
                string message = txtMessage.Text;

                // === ADD THIS: check user is logged in ===
                if (Session["UserID"] == null)
                {
                    lblMessage.Text = "Please log in to book a course.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // === REPLACE THIS LINE ===
                // OLD: int studentId = Convert.ToInt32(Session["s_id"]);
                // with:
                Guid userId = (Guid)Session["UserID"];

                int courseId = Convert.ToInt32(Request.QueryString["id"]);

                using (NpgsqlConnection conn = new NpgsqlConnection(conString))
                {
                    conn.Open();

                    // === ADD THIS: look up the student's s_id using the logged-in user's Guid ===
                    int studentId;
                    using (NpgsqlCommand cmdLookup = new NpgsqlCommand(
                        "SELECT s_id FROM tblstudent WHERE user_id = @userId", conn))
                    {
                        cmdLookup.Parameters.AddWithValue("@userId", userId);
                        var result = cmdLookup.ExecuteScalar();

                        if (result == null)
                        {
                            lblMessage.Text = "No student profile found for this account.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                        studentId = Convert.ToInt32(result);
                    }

                    // === everything below this is UNCHANGED from your original code ===
                    string sql = @"INSERT INTO tblbookingrequest
                   (isaccepted, req_date, messages, booking_date, s_id, c_id)
                   VALUES
                   (@isaccepted, @req_date, @messages, @booking_date, @s_id, @c_id)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@isaccepted", false);
                        cmd.Parameters.AddWithValue("@req_date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@messages", message);
                        cmd.Parameters.AddWithValue("@booking_date", DateTime.Parse(bookingDate));
                        cmd.Parameters.AddWithValue("@s_id", studentId);
                        cmd.Parameters.AddWithValue("@c_id", courseId);

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "msg",
                    "alert('Booking request submitted successfully!');", true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

    }


}