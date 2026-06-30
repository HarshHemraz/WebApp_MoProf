using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;


namespace moProf_Assignment
{
    public partial class student : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoginForm.WelcomeMessage = "Welcome Student";

            }

        }
       
           
          
       
    }
}