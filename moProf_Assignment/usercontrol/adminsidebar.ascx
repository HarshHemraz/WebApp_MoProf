<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="adminsidebar.ascx.cs" Inherits="moProf_Assignment.usercontrol.adminsidebar" %>
<aside id="sidebar" class="sidebar" style=" background-color: rgb(23, 23, 23); height: 120vh">    
    <ul class="nav flex-column">
        <asp:Image ID="logoImg" runat="server" CssClass="imagelogo img-fluid w-50 rounded-circle d-block p-lg-4 pt-5" ImageUrl="/images/prof1.jpg" AlternateText="Company Logo" />
        <asp:Label ID="dashboardlbl" runat="server" CssClass="text-side" ForeColor="White" Font-Size="Large" Text="Dashboard"></asp:Label>
        
        <asp:LinkButton ID="manageStudents" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/adminContent/managestudents.aspx">
            ⦾ Manage Students
        </asp:LinkButton>
        
        <asp:LinkButton ID="manageTutors" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/adminContent/managetutors.aspx">
            ⦾ Manage Tutors
        </asp:LinkButton>
         <asp:LinkButton ID="manageContent" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/adminContent/managecontent.aspx">
     ⦾ Manage Content
 </asp:LinkButton>
        
        <asp:LinkButton ID="editCourses" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/adminContent/editcoursesadmin.aspx">
    ⦾ Manage Courses
</asp:LinkButton>
        
        <li class="px-4 pt-4 pb-2 pt-5"></li>
        
        <asp:Label ID="accountmanagement" runat="server" CssClass="text-side" ForeColor="White" Font-Size="Large" Text="Account Management"></asp:Label>
        
        <asp:LinkButton ID="editAcct" runat="server" CssClass="links-side" CausesValidation="false" PostBackUrl="~/adminContent/editadmin.aspx">
            ⦾ Edit Account
        </asp:LinkButton>
    </ul>
</aside>