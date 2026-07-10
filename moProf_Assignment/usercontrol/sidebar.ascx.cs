using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment.usercontrol
{
    public partial class sidebar : System.Web.UI.UserControl
    {
        public string AnnouncementText
        {
            get { return studentresult.Text; }
            set { studentresult.Text = value; }
        }
        public string AnnouncementPostBackUrl
        {
            get { return studentresult.PostBackUrl; }
            set { studentresult.PostBackUrl = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void manageTutors_Click(object sender, EventArgs e) { }
        protected void manageStudents_Click(object sender, EventArgs e) { }
        protected void manageCourses_Click(object sender, EventArgs e) { }

    }
}