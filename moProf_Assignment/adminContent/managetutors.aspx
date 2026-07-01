<%@ Page Title="Manage Tutors" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="managetutors.aspx.cs" Inherits="moProf_Assignment.adminContent.managetutors" %>
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
                    <h2 class="mb-4">Manage Tutors</h2>
                    
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Visible="false"></asp:Label>
                    
                    <div class="table-responsive">
                        <asp:Repeater ID="tutorsRepeater" runat="server" OnItemCommand="tutorsRepeater_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-bordered table-hover">
                                    <thead class="thead-dark">
                                        <tr>
                                            <th>First Name</th>
                                            <th>Last Name</th>
                                            <th>Email</th>
                                            <th>Date Created</th>
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
                                    <td><%# Convert.ToDateTime(Eval("dateCreated")).ToString("yyyy-MM-dd") %></td>
                                    <td>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditTutor" 
                                            CommandArgument='<%# Eval("id") %>' CssClass="btn btn-sm btn-primary">
                                            Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteTutor" 
                                            CommandArgument='<%# Eval("id") %>' CssClass="btn btn-sm btn-danger"
                                            OnClientClick="return confirm('Are you sure you want to delete this tutor?');">
                                            Delete
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
                    
                    <asp:Label ID="lblNoRecords" runat="server" CssClass="text-center d-block" Text="No tutors found." Visible="false"></asp:Label>
                </div>
            </div>
        </div>
    
</asp:Content>