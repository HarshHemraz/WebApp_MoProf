<%@ Page Title="Site Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sitesettings.aspx.cs" Inherits="moProf_Assignment.adminContent.sitesettings" %>
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
                <h2 class="mb-4">Site Settings</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Visible="false"></asp:Label>

                <div class="card p-4" style="max-width: 600px;">

                    <h5 class="mb-3">User Registrations</h5>
                    <div class="form-check form-switch mb-3 mx-3">
                        <asp:CheckBox ID="chkRegistrationsEnabled" runat="server" />
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="chkRegistrationsEnabled"
                            CssClass="form-check-label" Text="Allow new user registrations" />
                    </div>

                    <hr />

                    <h5 class="mb-3">Site Availability</h5>
                    <div class="form-check form-switch mb-3 mx-3">
                        <asp:CheckBox ID="chkMaintenanceMode" runat="server" />
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="chkMaintenanceMode"
                            CssClass="form-check-label" Text="Enable maintenance mode (blocks all non-admin access)" />
                    </div>

                    <hr />

                    <h5 class="mb-3">Contact Information</h5>
                    <div class="mb-3">
                        <asp:Label ID="Label4" runat="server" Text="Support email address" CssClass="form-label" />
                        <asp:TextBox ID="txtSupportEmail" runat="server" CssClass="form-control" placeholder="support@yourdomain.com" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtSupportEmail"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="*Enter a valid email"
                            ForeColor="Red" CssClass="errrormsg" Display="Dynamic" />
                    </div>

                    <asp:Button ID="btnSave" runat="server" Text="Save Settings"
                        CssClass="btn btn-primary mt-2" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>