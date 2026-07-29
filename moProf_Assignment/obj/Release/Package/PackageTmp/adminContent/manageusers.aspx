<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="manageusers.aspx.cs" Inherits="moProf_Assignment.adminContent.manageusers" %>
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
                <h2 class="mb-4">Manage Users</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Visible="false"></asp:Label>

                <div class="row mb-3">
                    <div class="col-md-4">
                        <div class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                placeholder="Search by name or email..."></asp:TextBox>
                            <div class="input-group-append">
                                <asp:Button ID="btnSearch" runat="server" Text="Search"
                                    CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                                <asp:Button ID="btnClearSearch" runat="server" Text="Clear"
                                    CssClass="btn btn-secondary" OnClick="btnClearSearch_Click"
                                    CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:DropDownList ID="ddlRoleFilter" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlRoleFilter_SelectedIndexChanged">
                            <asp:ListItem Text="All Roles" Value="" />
                            <asp:ListItem Text="Student" Value="student" />
                            <asp:ListItem Text="Tutor" Value="tutor" />
                            <asp:ListItem Text="Admin" Value="admin" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="usersRepeater" runat="server" OnItemCommand="usersRepeater_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-bordered table-hover">
                                <thead class="thead-dark">
                                    <tr>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                        <th>Date Created</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("firstname") %></td>
                                <td><%# Eval("lastname") %></td>
                                <td><%# Eval("email") %></td>
                                <td><%# Eval("role") %></td>
                                <td><%# Convert.ToDateTime(Eval("dateCreated")).ToString("yyyy-MM-dd") %></td>
                                <td>
                                    <%# (bool)Eval("isFrozen")
                                         ? "<span style='background-color:#dc3545; color:#ffffff; padding:4px 10px; border-radius:12px; font-size:0.85rem; font-weight:600;'>Frozen</span>"
        : "<span style='background-color:#28a745; color:#ffffff; padding:4px 10px; border-radius:12px; font-size:0.85rem; font-weight:600;'>Active</span>" %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnToggleFreeze" runat="server" CommandName="ToggleFreeze"
                                        CommandArgument='<%# Eval("id") + "|" + Eval("isFrozen") %>'
                                        CssClass='<%# (bool)Eval("isFrozen") ? "btn btn-sm btn-success" : "btn btn-sm btn-warning" %>'
                                        OnClientClick='<%# (bool)Eval("isFrozen")
                                            ? "return confirm(\"Unfreeze this account?\");"
                                            : "return confirm(\"Freeze this account? The user will not be able to log in.\");" %>'>
                                        <%# (bool)Eval("isFrozen") ? "Unfreeze" : "Freeze" %>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <asp:Label ID="lblNoRecords" runat="server" CssClass="text-center d-block" Text="No users found." Visible="false"></asp:Label>
            </div>
        </div>
    </div>

</asp:Content>