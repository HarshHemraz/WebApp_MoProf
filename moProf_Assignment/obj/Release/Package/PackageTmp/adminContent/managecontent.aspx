<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="managecontent.aspx.cs" Inherits="moProf_Assignment.adminContent.managecontent" %>
<%@ Register Src="~/usercontrol/adminsidebar.ascx" TagPrefix="uc1" TagName="adminsidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="row">
        <div class="col-md-2">
            <uc1:adminsidebar runat="server" ID="adminsidebar" />
        </div>
        <div class="col-md-10">
            <div class="content-wrapper p-4">
                <h2 class="mb-4 text-primary fw-bold">Manage Courses</h2>

                <div class="table-responsive">
                    <table class="table table-bordered table-hover align-middle">
                        <thead class="thead-dark">
                            <tr>
                                <th>Image</th>
                                <th>Course Name</th>
                                <th>Category</th>
                                <th>Tutor</th>
                                <th>Location</th>
                                <th>Price</th>
                                <th>Enrolled</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptCourses" runat="server" OnItemDataBound="rptCourses_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width:80px;">
                                            <img src='<%# string.IsNullOrEmpty(Eval("image").ToString()) ? "/images/placeholder.jpg" : "/CourseImages/" + Eval("image") %>'
                                                alt="Course Image" style="width:70px; height:50px; object-fit:cover;" class="rounded" />
                                        </td>
                                        <td><%# Eval("c_name") %></td>
                                        <td><span class="badge bg-secondary"><%# Eval("category") %></span></td>
                                        <td><%# Eval("tutor_firstname") %> <%# Eval("tutor_lastname") %></td>
                                        <td><%# Eval("location") %></td>
                                        <td class="text-success fw-bold">Rs<%# string.Format("{0:N2}", Eval("c_price")) %></td>
                                        <td><%# Eval("enrolled_count") %></td>
                                        <td style="white-space:nowrap;">
                                            <a href='managecontent.aspx?id=<%# Eval("c_id") %>&action=view' class="btn btn-outline-secondary btn-sm mb-1">View Students</a>
                                            <a href='managecontent.aspx?id=<%# Eval("c_id") %>&action=edit' class="btn btn-outline-primary btn-sm mb-1">Edit</a>
                                        </td>
                                    </tr>

                                    <!-- Edit Modal -->
                                    <div class="modal fade" id='editCourseModal_<%# Eval("c_id") %>' tabindex="-1" aria-labelledby="editCourseModalLabel" aria-hidden="true">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header">
                                                    <h5 class="modal-title">Edit Course Details</h5>
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

                                    <!-- Enrolled Students Modal -->
                                    <div class="modal fade" id='studentsModal_<%# Eval("c_id") %>' tabindex="-1" aria-labelledby="studentsModalLabel" aria-hidden="true">
                                        <div class="modal-dialog modal-lg modal-dialog-centered">
                                            <div class="modal-content">
                                                <div class="modal-header">
                                                    <h5 class="modal-title">Enrolled Students - <%# Eval("c_name") %></h5>
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
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <asp:Label ID="lblNoRecords" runat="server" CssClass="text-center d-block" Text="No courses found." Visible="false"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>