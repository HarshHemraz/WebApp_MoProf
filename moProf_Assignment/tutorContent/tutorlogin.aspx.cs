using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace moProf_Assignment
{
    public partial class tutor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
                {
                LoginForm.WelcomeMessage = "Welcome Tutor";

            }

        }

        protected void Login_Click(object sender, EventArgs e)
        {
            
        }
    }
}