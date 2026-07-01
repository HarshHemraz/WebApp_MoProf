<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sidebar.ascx.cs" Inherits="moProf_Assignment.usercontrol.sidebar" %>
    <aside id="sidebar" class="sidebar" style=" background-color: rgb(23, 23, 23); height: 120vh">
             
        <ul class="nav flex-column">
            <asp:Image ID="logoImg" runat="server" CssClass="imagelogo img-fluid w-50 rounded-circle d-block p-lg-4 pt-5" ImageUrl="/images/prof1.jpg" AlternateText="Company Logo" />
            <asp:Label ID="dashboardlbl" class="form-check-label" CssClass="text-side" ForeColor="White" Font-Size="Large" runat="server" Text="Dashboard"></asp:Label>
            <asp:LinkButton ID="analytics" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/tutorContent/courses.aspx">
⦾ Analytics
            </asp:LinkButton>
            <asp:LinkButton ID="courses" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/tutorContent/addcourses.aspx">
          ⦾ Add Courses
            </asp:LinkButton>
            <asp:LinkButton ID="editcourse" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/tutorContent/editcourses.aspx">
   ⦾ Edit Courses
            </asp:LinkButton>
            <asp:LinkButton ID="studentresult" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/tutorContent/studentresult.aspx">
   ⦾ Student Result
            </asp:LinkButton>

            <li class="px-4 pt-4 pb-2 pt-5"></li>
            <asp:Label ID="accountmanagement" class="form-check-label" CssClass="text-side " ForeColor="White" Font-Size="Large" runat="server" Text="Account Management"></asp:Label>
            <asp:LinkButton ID="editAcct" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/tutorContent/editacct.aspx">
           ⦾ Edit Account
            </asp:LinkButton>
            

        </ul>

    </aside>