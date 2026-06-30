<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="studentpanel.aspx.cs" Inherits="moProf_Assignment.studentpanel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="container mt-4">
        <h2 class="mb-4">Available Courses</h2>
        
        <!-- Course Grid Loop -->
        <div class="row">
            <asp:Repeater ID="rptCourses" runat="server">
                <ItemTemplate>
                    <div class="col-md-4 mb-4">
                        <div class="card h-100 shadow-sm">
                            <!-- Image Fallback handling for text type images -->
                            <img src='<%# Eval("image") != DBNull.Value && !string.IsNullOrEmpty(Eval("image").ToString()) ? ResolveUrl("~/images/" + Eval("image")) : ResolveUrl("~/images/default-course.jpg") %>' class="card-img-top" alt="Course Image" style="height: 180px; object-fit: cover;">
                            
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title text-primary"><%# Eval("c_name") %></h5>
                                <span class="badge bg-secondary mb-2 align-self-start"><%# Eval("category") %></span>
                                
                                <!-- Safely truncate description if text length > 100 -->
                                <p class="card-text text-muted flex-grow-1">
                                    <%# Eval("c_desc") != DBNull.Value && Eval("c_desc").ToString().Length > 100 ? Eval("c_desc").ToString().Substring(0, 100) + "..." : Eval("c_desc") %>
                                </p>
                                
                                <ul class="list-unstyled my-2 small text-dark">
                                    <li><strong>📍 Location:</strong> <%# Eval("location") %></li>
                                    <li><strong>🕒 Schedule:</strong> <%# Eval("timestable") %></li>
                                    <li><strong>⭐ Experience:</strong> <%# Eval("experience") %></li>
                                    <li><strong>👥 Enrolled:</strong> <%# Eval("no_student") ?? 0 %></li>
                                </ul>

                                <div class="mt-auto pt-3 border-top d-flex justify-content-between align-items-center">
                                    <span class="h5 text-success mb-0">$<%# string.Format("{0:N2}", Eval("c_price")) %></span>
                                    <asp:LinkButton ID="lnkViewDetails" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="lnkViewDetails_Click" CommandArgument='<%# Eval("c_id") %>'>View Details</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Pagination Controls -->
        <div class="d-flex justify-content-between align-items-center mt-4 pt-3 border-top">
            <div>
                <span class="text-muted">Page <asp:Label ID="lblCurrentPage" runat="server" FontWeight="Bold" /> of <asp:Label ID="lblTotalPages" runat="server" FontWeight="Bold" /></span>
            </div>
            <div class="btn-group">
                <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click" CssClass="btn btn-primary btn-sm">⏮️ Previous</asp:LinkButton>
                <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click" CssClass="btn btn-primary btn-sm">Next ⏭️</asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>