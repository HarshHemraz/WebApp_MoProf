<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="postannouncement.aspx.cs" Inherits="moProf_Assignment.tutorContent.postannouncement" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">

   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <div class="main-layout-container">
        <section class="add-courses-section">
             <uc1:sidebar runat="server" ID="sidebar" />
            <div class="container mt-5" style="max-width: 900px;">

                <h2 class="mb-4 text-primary fw-bold">Post Announcement</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Font-Size="Medium"></asp:Label>

                <!-- New Announcement Form -->
                <div class="card shadow-sm mb-5">
                    <div class="card-body">
                        <div class="mb-3">
                            <asp:Label ID="Label1" runat="server" Text="Title" AssociatedControlID="txtTitle" CssClass="form-label fw-bold" />
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Announcement title" />
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                                ErrorMessage="Title is required." CssClass="text-danger small" Display="Dynamic"
                                ValidationGroup="postAnnouncement" />
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="Label2" runat="server" Text="Message" AssociatedControlID="txtMessage" CssClass="form-label fw-bold" />
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Full announcement content" />
                            <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage"
                                ErrorMessage="Message is required." CssClass="text-danger small" Display="Dynamic"
                                ValidationGroup="postAnnouncement" />
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="Label3" runat="server" Text="Expiry Date (optional)" AssociatedControlID="txtExpiry" CssClass="form-label fw-bold" />
                                <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-6 mb-3 d-flex align-items-end">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" CssClass="form-check-input" />
                                    <asp:Label ID="Label4" runat="server" Text="Active" AssociatedControlID="chkActive" CssClass="form-check-label fw-bold" />
                                </div>
                            </div>
                        </div>

                        <asp:Button ID="btnPost" runat="server" Text="Post Announcement" CssClass="btn btn-primary"
                            ValidationGroup="postAnnouncement" OnClick="btnPost_Click" />
                    </div>
                </div>

                <!-- Existing Announcements -->
                <h4 class="mb-3 fw-bold">Your Announcements</h4>

                <asp:Repeater ID="rptAnnouncements" runat="server" OnItemCommand="rptAnnouncements_ItemCommand">
                    <ItemTemplate>
                        <div class="card mb-3">
                            <div class="card-body">
                                <div class="d-flex justify-content-between align-items-start">
                                    <div>
                                        <h5 class="card-title mb-1"><%# Eval("a_title") %></h5>
                                        <p class="mb-1 text-muted small">
                                            Posted: <%# Eval("post_date", "{0:MMM dd, yyyy HH:mm}") %>
                                            <%# Eval("expiry_date") != DBNull.Value ? " &nbsp;|&nbsp; Expires: " + Convert.ToDateTime(Eval("expiry_date")).ToString("MMM dd, yyyy") : "" %>
                                        </p>
                                        <p class="card-text"><%# Eval("messages") %></p>
                                    </div>
                                    <span class='badge <%# (bool)Eval("is_active") ? "bg-success" : "bg-secondary" %>'>
                                        <%# (bool)Eval("is_active") ? "Active" : "Inactive" %>
                                    </span>
                                </div>

                                <div class="mt-2 text-end">
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# (bool)Eval("is_active") ? "Deactivate" : "Activate" %>'
                                        CssClass="btn btn-outline-secondary btn-sm me-2"
                                        CommandName="ToggleActive" CommandArgument='<%# Eval("a_id") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-outline-danger btn-sm"
                                        CommandName="Delete" CommandArgument='<%# Eval("a_id") %>'
                                        OnClientClick="return confirm('Delete this announcement?');" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </section>
    </div>

</asp:Content>
