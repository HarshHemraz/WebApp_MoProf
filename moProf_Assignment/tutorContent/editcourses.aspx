<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="editcourses.aspx.cs" Inherits="moProf_Assignment.tutorContent.editcourses" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
     <div class="main-layout-container">
    
     <section class="add-courses-section">
          <uc1:sidebar runat="server" ID="ucSidebar" />
         <div class="container mt-5">
    <h2 class="mb-4 text-primary fw-bold">Courses Section</h2>
    
    <!-- Bootstrap Responsive Card Grid container -->
    <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
        
        <asp:Repeater ID="rptCourses" runat="server">
            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 shadow-sm hover-shadow transition-all">     
                        <img src='<%# string.IsNullOrEmpty(Eval("image").ToString()) ? "/images/placeholder.jpg" : "/images/" + Eval("image") %>' 
                             class="card-img-top object-fit-cover" style="height: 200px;" alt="Course Image">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="badge bg-secondary"><%# Eval("category") %></span>
                                <h5 class="text-success fw-bold m-0">$<%# string.Format("{0:N2}", Eval("c_price")) %></h5>
                            </div> 
                            <h5 class="card-title fw-bold text-dark"><%# Eval("c_name") %></h5>
                            <p class="card-text text-muted flex-grow-1"><%# Eval("c_desc") %></p>
                            <hr class="my-2 text-muted opacity-25">
                            <div class="small text-secondary mb-3">
                                <p class="m-0"><i class="bi bi-geo-alt-fill me-1 text-danger"></i><strong>Location:</strong> <%# Eval("location") %></p>
                                <p class="m-0"><i class="bi bi-clock-fill me-1 text-primary"></i><strong>Total time to complete course:</strong> <%# Eval("timetable") %></p>
                                <p class="m-0"><i class="bi bi-briefcase-fill me-1 text-warning"></i><strong>Tutor total experience: </strong> <%# Eval("experience") %></p>
                            </div>
                            
                            <a href="#" class="btn btn-outline-primary btn-sm w-100 mt-auto">Edit Course</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        </div>
             </div>
     </section>
 </div>
</asp:Content>
