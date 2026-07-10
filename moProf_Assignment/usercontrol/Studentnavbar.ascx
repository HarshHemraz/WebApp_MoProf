<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Studentnavbar.ascx.cs" Inherits="moProf_Assignment.usercontrol.studentnavbar" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark w-100 d-flex align-items-center flex-grow-1">
  
    <!-- mx-auto centers this flexible container horizontally -->
    <div class="d-flex align-items-center mx-auto">
 
        <asp:LinkButton ID="viewTutors" runat="server" CssClass="links-side text-decoration-none text-white px-2" CausesValidation="false" PostBackUrl="/studentContent/viewTutors.aspx">
            ⦾ View Tutors
        </asp:LinkButton>
          
        <asp:LinkButton ID="viewAnnouncement" runat="server" CssClass="links-side text-decoration-none text-white px-2" CausesValidation="false" PostBackUrl="~/studentContent/viewannouncement.aspx">
            ⦾ View Announcement
        </asp:LinkButton>
        
        <asp:LinkButton ID="viewCourses" runat="server" CssClass="links-side text-decoration-none text-white px-2" CausesValidation="false" PostBackUrl="~/studentContent/studentpanel.aspx">
            ⦾ View Courses
        </asp:LinkButton>

        <asp:LinkButton ID="viewRecommendations" runat="server" CssClass="links-side text-decoration-none text-white px-2" CausesValidation="false" PostBackUrl="~/studentContent/addrecommendation.aspx">
            ⦾ Add Recommendations 
        </asp:LinkButton>
        
        <asp:LinkButton ID="uploadResult" runat="server" CssClass="links-side text-decoration-none text-white px-2" CausesValidation="false" PostBackUrl="~/studentContent/uploadresult.aspx">
            ⦾ Upload Academic Result
        </asp:LinkButton>
    </div>

</nav>