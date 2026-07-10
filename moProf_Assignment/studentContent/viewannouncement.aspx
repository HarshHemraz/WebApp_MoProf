<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="viewannouncement.aspx.cs" Inherits="moProf_Assignment.studentContent.viewannouncement" %>

<%@ Register Src="~/usercontrol/Studentnavbar.ascx" TagPrefix="uc1" TagName="Studentnavbar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <uc1:Studentnavbar runat="server" ID="Studentnavbar" />

    <div class="main-layout-container">
        <section class="add-courses-section">
            <div class="container mt-5" style="max-width: 900px;">

                <h2 class="mb-4 text-primary fw-bold">Announcements</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3 text-muted"></asp:Label>

                <asp:Repeater ID="rptAnnouncements" runat="server">
                    <ItemTemplate>
                        <div class="card mb-3 shadow-sm">
                            <div class="card-body">
                                <h5 class="card-title fw-bold mb-1"><%# Eval("a_title") %></h5>
                                <p class="text-muted small mb-2">
                                    Posted: <%# Eval("post_date", "{0:MMM dd, yyyy HH:mm}") %>
                                    <%# Eval("expiry_date") != DBNull.Value ? " &nbsp;|&nbsp; Expires: " + Convert.ToDateTime(Eval("expiry_date")).ToString("MMM dd, yyyy") : "" %>
                                </p>
                                <p class="card-text"><%# Eval("messages") %></p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </section>
    </div>

</asp:Content>
