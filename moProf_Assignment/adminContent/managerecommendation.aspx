<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="managerecommendations.aspx.cs" Inherits="moProf_Assignment.tutorContent.managerecommendations" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <div class="main-layout-container">
        <section class="add-courses-section">
            <uc1:sidebar runat="server" ID="sidebar" />
            <div class="container mt-5" style="max-width: 900px;">

                <h2 class="mb-4 text-primary fw-bold">Manage Student Recommendations</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Font-Size="Medium"></asp:Label>

                <asp:Repeater ID="rptRecommendations" runat="server" OnItemCommand="rptRecommendations_ItemCommand">
                    <ItemTemplate>
                        <div class="card mb-3">
                            <div class="card-body">
                                <div class="d-flex justify-content-between align-items-start">
                                    <div>
                                        <h5 class="card-title mb-1"><%# Eval("recommendation_title") %></h5>
                                        <p class="mb-1 text-muted small">
                                            <span class="badge bg-light text-dark border"><%# Eval("recommendation_type") %></span>
                                            &nbsp;|&nbsp; Submitted by <%# Eval("firstname") %> <%# Eval("lastname") %>
                                            &nbsp;|&nbsp; <%# Eval("createdat", "{0:MMM dd, yyyy HH:mm}") %>
                                            &nbsp;|&nbsp; Confidence: <%# Eval("confidence_score") %>%
                                        </p>
                                        <p class="card-text"><%# Eval("description") %></p>
                                    </div>
                                    <span class='badge <%# GetStatusBadgeClass(Eval("status")) %>'>
                                        <%# Eval("status") %>
                                    </span>
                                </div>

                                <div class="mt-2 text-end">
                                    <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-success btn-sm me-2"
                                        CommandName="Approve" CommandArgument='<%# Eval("r_id") %>'
                                        Visible='<%# Eval("status").ToString() != "Approved" %>' />
                                    <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-outline-danger btn-sm"
                                        CommandName="Reject" CommandArgument='<%# Eval("r_id") %>'
                                        Visible='<%# Eval("status").ToString() != "Rejected" %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoRecommendations" runat="server" CssClass="text-muted" Visible="false" Text="No recommendations submitted yet." />

            </div>
        </section>
    </div>

</asp:Content>
