<%@ Page Title="Edit Account" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="editacct.aspx.cs" Inherits="moProf_Assignment.tutorContent.editacct" %>
<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="main-layout-container">
        <section class="add-courses-section">
            <uc1:sidebar runat="server" ID="Sidebar1" />

            <div class="form-card">
                <h2>Edit Account</h2>

                <div class="mb-3">
                    <label>First Name</label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Last Name</label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Total Experience (Years)</label>
                    <asp:TextBox ID="txtExperience" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                </div>

                <hr />

                <h4>Change Password</h4>

                <div class="mb-3">
                    <label>Current Password</label>
                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>New Password</label>
                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Confirm Password</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnUpdate" runat="server"
                        Text="Update Account"
                        CssClass="btn btn-primary"
                        OnClick="btnUpdate_Click" />

                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        CssClass="btn btn-secondary ms-2"
                        CausesValidation="false"
                        PostBackUrl="~/tutorContent/dashboard.aspx" />
                </div>

                <br />

                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </div>
        </section>
    </div>
</asp:Content>