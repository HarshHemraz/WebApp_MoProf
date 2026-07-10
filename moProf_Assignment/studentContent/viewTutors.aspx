<%@ Page Title="Tutor Panel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TutorPanel.aspx.cs" Inherits="moProf_Assignment.TutorPanel" %>

<%@ Register Src="~/usercontrol/Studentnavbar.ascx" TagPrefix="uc1" TagName="Studentnavbar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <uc1:Studentnavbar runat="server" ID="Studentnavbar" />
    <div class="container mt-4">

        <h2 class="mb-4">Our Tutors</h2>

        <!-- Tutor Grid Loop -->
        <div class="row">
            <asp:Repeater ID="rptTutors" runat="server">
                <ItemTemplate>
                    <div class="col-md-4 mb-4">
                        <div class="card h-100 shadow-sm">
                            <!-- Tutor Card Header -->
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0"><%# Eval("firstname") %> <%# Eval("lastname") %></h5>
                            </div>

                            <div class="card-body d-flex flex-column">
                                <!-- Role Badge -->
                                <span class="badge bg-info mb-2 align-self-start">Tutor</span>

                                <!-- Tutor Details -->
                                <ul class="list-unstyled my-2 small text-dark">
                                    <li><strong>🎯 Total Students:</strong> <%# Eval("total_student") %></li>
                                    <li><strong>⭐ Total Reviews:</strong> <%# Eval("total_review") %></li>
                                    <li><strong>📅 Joined:</strong> <%# Convert.ToDateTime(Eval("created_at")).ToString("MMM dd, yyyy") %></li>
                                    <li>
                                        <strong>📊 Status:</strong>
                                        <span class='<%# Convert.ToBoolean(Eval("isAvailable")) ? "text-success" : "text-danger" %>'>
                                            <%# Convert.ToBoolean(Eval("isAvailable")) ? "Available" : "Not Available" %>
                                        </span>
                                    </li>
                                </ul>

                           
<!-- Experience Display -->
<div class="mt-2">
    <strong>💼 Experience:</strong>
    <div class="progress">
        <div class="progress-bar bg-success" role="progressbar" 
             style='<%# "width: " + (Convert.ToInt32(Eval("t_exp")) >= 10 ? 100 : (Convert.ToInt32(Eval("t_exp")) * 10)).ToString() + "%;" %>' 
             aria-valuenow='<%# Eval("t_exp") %>' 
             aria-valuemin="0" 
             aria-valuemax="10">
            <%# Eval("t_exp") %> years
        </div>
    </div>
</div>

                                <div class="mt-auto pt-3 border-top d-flex justify-content-between align-items-center">
                                    <span class="text-muted small"></span>
                                    <asp:LinkButton ID="lnkViewProfile" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="lnkViewProfile_Click" CommandArgument='<%# Eval("user_id") %>'>View Profile</asp:LinkButton>
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
                <span class="text-muted">Page
                    <asp:Label ID="lblCurrentPage" runat="server" FontWeight="Bold" />
                    of
                    <asp:Label ID="lblTotalPages" runat="server" FontWeight="Bold" /></span>
            </div>
            <div class="btn-group">
                <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click" CssClass="btn btn-primary btn-sm">⏮️ Previous</asp:LinkButton>
                <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click" CssClass="btn btn-primary btn-sm">Next ⏭️</asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
