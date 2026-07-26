<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addrecommendation.aspx.cs" Inherits="moProf_Assignment.studentContent.addrecommendation" %>

<%@ Register Src="~/usercontrol/Studentnavbar.ascx" TagPrefix="uc1" TagName="Studentnavbar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
    <uc1:Studentnavbar runat="server" ID="Studentnavbar" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <div class="main-layout-container">
        <section class="add-courses-section">
            <div class="container " >

                <h2 class="mb-4 text-primary fw-bold">Add Recommendation</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Font-Size="Medium"></asp:Label>

                
                <div class="card shadow-sm mb-5">
                    <div class="card-body">

                        <div class="mb-3">
                            <asp:Label ID="Label1" runat="server" Text="Title" AssociatedControlID="txtTitle" CssClass="form-label fw-bold" />
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="e.g. Great tutor for Mathematics" />
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                                ErrorMessage="Title is required." CssClass="text-danger small" Display="Dynamic"
                                ValidationGroup="addRecommendation" />
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="Label2" runat="server" Text="Recommendation Type" AssociatedControlID="ddlType" CssClass="form-label fw-bold" />
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Tutor" Value="Tutor" />
                                <asp:ListItem Text="Course" Value="Course" />
                                <asp:ListItem Text="Study Material" Value="Study Material" />
                                <asp:ListItem Text="Study Tips" Value="Study Tips" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="Label3" runat="server" Text="Description" AssociatedControlID="txtDescription" CssClass="form-label fw-bold" />
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"
                                placeholder="Tell us more about your recommendation" />
                            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                ErrorMessage="Description is required." CssClass="text-danger small" Display="Dynamic"
                                ValidationGroup="addRecommendation" />
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="Label4" runat="server" Text="How confident are you in this recommendation?" AssociatedControlID="rblConfidence" CssClass="form-label fw-bold" />
                            <asp:RadioButtonList ID="rblConfidence" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-4">
                                <asp:ListItem Text="Low" Value="33" />
                                <asp:ListItem Text="Medium" Value="66" Selected="True" />
                                <asp:ListItem Text="High" Value="100" />
                            </asp:RadioButtonList>
                        </div>

                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Recommendation" CssClass="btn btn-primary"
                            ValidationGroup="addRecommendation" OnClick="btnSubmit_Click" />
                    </div>
                </div>

               
                <h4 class="mb-3 fw-bold">Your Recommendations</h4>

                <asp:Repeater ID="rptMyRecommendations" runat="server">
                    <ItemTemplate>
                        <div class="card mb-3">
                            <div class="card-body">
                                <div class="d-flex justify-content-between align-items-start">
                                    <div>
                                        <h5 class="card-title mb-1"><%# Eval("recommendation_title") %></h5>
                                        <p class="mb-1 text-muted small">
                                            <span class="badge bg-light text-dark border"><%# Eval("recommendation_type") %></span>
                                            &nbsp;|&nbsp; Submitted: <%# Eval("createdat", "{0:MMM dd, yyyy HH:mm}") %>
                                        </p>
                                        <p class="card-text"><%# Eval("description") %></p>
                                    </div>
                                    <span class='badge <%# GetStatusBadgeClass(Eval("status").ToString()) %>'>
                                        <%# Eval("status") %>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoRecommendations" runat="server" CssClass="text-muted" Visible="false" Text="You haven't submitted any recommendations yet." />

            </div>
        </section>
    </div>

</asp:Content>
