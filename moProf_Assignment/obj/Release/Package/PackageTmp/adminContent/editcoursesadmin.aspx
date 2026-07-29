<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="editcoursesadmin.aspx.cs" Inherits="moProf_Assignment.tutorContent.editcoursesadmin" %>

<%@ Register Src="~/usercontrol/adminsidebar.ascx" TagPrefix="uc1" TagName="adminsidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="main-layout-container">
        <uc1:adminsidebar runat="server" ID="adminsidebar" />
        <section class="add-courses-section">

            <div class="container mt-5">
                <h2 class="mb-4 text-primary fw-bold">Courses Section (Admin)</h2>

                <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
                    <asp:Repeater ID="rptCourses" runat="server" OnItemDataBound="rptCourses_ItemDataBound">
                        <ItemTemplate>
                            <div class="col">
                                <div class="card h-100 shadow-sm hover-shadow transition-all">
                                    <img src='<%# string.IsNullOrEmpty(Eval("image").ToString()) ? "/images/placeholder.jpg" : "/CourseImages/" + Eval("image") %>' class="card-img-top object-fit-cover" style="height: 200px;" alt="Course Image">

                                    <div class="card-body d-flex flex-column">
                                        <div class="d-flex justify-content-between align-items-center mb-2">
                                            <span class="badge bg-secondary"><%# Eval("category") %></span>
                                            <h5 class="text-success fw-bold m-0">Rs<%# string.Format("{0:N2}", Eval("c_price")) %></h5>
                                        </div>
                                        <h5 class="card-title fw-bold text-dark"><%# Eval("c_name") %></h5>
                                        <p class="card-text text-muted flex-grow-1"><%# Eval("c_desc") %></p>
                                        <hr class="my-2 text-muted opacity-25">
                                        <div class="small text-secondary mb-3">
                                            <p class="m-0"><i class="bi bi-person-fill me-1 text-primary"></i><strong>Tutor:</strong> <%# Eval("tutor_firstname") %> <%# Eval("tutor_lastname") %></p>
                                            <p class="m-0"><i class="bi bi-geo-alt-fill me-1 text-danger"></i><strong>Location:</strong> <%# Eval("location") %></p>
                                            <p class="m-0"><i class="bi bi-clock-fill me-1 text-primary"></i><strong>Total time to complete course:</strong> <%# Eval("timestable") %></p>
                                            <p class="m-0"><i class="bi bi-briefcase-fill me-1 text-warning"></i><strong>Tutor total experience: </strong><%# Eval("experience") %></p>
                                        </div>

                                        <!-- Redirect Link to drive Page_Load processing context -->
                                        <a href='editcoursesadmin.aspx?id=<%# Eval("c_id") %>&action=view' class="btn btn-outline-secondary btn-sm w-100 mt-auto my-2">View Student Enrolled</a>

                                        <a href='editcoursesadmin.aspx?id=<%# Eval("c_id") %>&action=edit' class="btn btn-outline-primary btn-sm w-100 mt-auto">Edit Course</a>

                                        <!-- Unique Modal Layout Per Card Block: Edit Course -->
                                        <div class="modal fade" id='editCourseModal_<%# Eval("c_id") %>' tabindex="-1" aria-labelledby="editCourseModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="editCourseModalLabel">Edit Course Details</h5>
                                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                    </div>
                                                    <div class="modal-body d-flex justify-content-center flex-column">
                                                        <div class="mb-3">
                                                            <asp:Label ID="lblCourseName" Text="Course Name:" runat="server" />
                                                            <asp:TextBox ID="txtcoursename" CssClass="form-control" Placeholder="Edit Course Name" runat="server" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label1" Text="Course Description" runat="server" />
                                                            <asp:TextBox ID="txtcrsdesc" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control" Placeholder="Enter Course Description" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label2" Text="Edit Add Image:" runat="server" />
                                                            <br />
                                                            <asp:FileUpload ID="fileUploadImage" CssClass="form-control" runat="server" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label3" Text="Fees: " runat="server" />
                                                            <asp:TextBox ID="feetxt" CssClass="form-control" Placeholder="Edit Fee(Rs)" runat="server" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label4" Text="Location " runat="server" />
                                                            <asp:TextBox ID="locationtxt" CssClass="form-control" Placeholder="Edit Course Location" runat="server" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label5" Text="Teaching Experience " runat="server" />
                                                            <asp:TextBox ID="exptxt" CssClass="form-control" Placeholder="Edit Teaching Experience" runat="server" />
                                                        </div>
                                                        <div class="mb-3">
                                                            <asp:Label ID="Label6" Text="Time table " runat="server" />
                                                            <asp:TextBox ID="rxtTime" CssClass="form-control" Placeholder="Edit course time" runat="server" />
                                                        </div>

                                                        <!-- Save Button tied to Data Update Event Context -->
                                                        <asp:Button ID="Button1"
                                                            runat="server"
                                                            Text="Save Changes"
                                                            CssClass="btn btn-success w-100"
                                                            CommandName="SaveChanges"
                                                            CommandArgument='<%# Eval("c_id") %>' OnClick="btnSave_Click" />

                                                        <asp:Button ID="dltbtn" Text="Delete Course" CssClass="btn btn-danger mt-1 w-100" runat="server"
                                                            OnClick="dltbtn_Click"
                                                            CommandArgument='<%# Eval("c_id") %>'
                                                            OnClientClick="return confirm('Do you want to delete this data?');" />

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Unique Modal Layout Per Card Block: Enrolled Students -->
                                        <div class="modal fade" id='studentsModal_<%# Eval("c_id") %>' tabindex="-1" aria-labelledby="studentsModalLabel" aria-hidden="true">
                                            <div class="modal-dialog modal-lg modal-dialog-centered">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="studentsModalLabel">Enrolled Students - <%# Eval("c_name") %></h5>
                                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                    </div>
                                                    <div class="modal-body">
                                                        <asp:Repeater ID="rptEnrolledStudents" runat="server">
                                                            <HeaderTemplate>
                                                                <div class="table-responsive">
                                                                    <table class="table table-striped table-bordered">
                                                                        <thead>
                                                                            <tr>
                                                                                <th>Name</th>
                                                                                <th>Email</th>
                                                                                <th>Booking Date</th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%# Eval("firstname") %> <%# Eval("lastname") %></td>
                                                                    <td><%# Eval("email") %></td>
                                                                    <td><%# Eval("booking_date", "{0:MMM dd, yyyy}") %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                        </tbody>
                                                                    </table>
                                                                </div>
                                                            </FooterTemplate>
                                                        </asp:Repeater>
                                                        <asp:Label ID="lblNoStudents" runat="server" Text="No students enrolled yet." CssClass="text-muted" Visible="false" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

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
