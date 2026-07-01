<%@ Page Title="Manage Students" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="managestudents.aspx.cs" Inherits="moProf_Assignment.adminContent.managestudents" %>
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
                    <h2 class="mb-4">Manage Students</h2>
                    
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Visible="false"></asp:Label>
                    
                    <div class="table-responsive">
                        <asp:Repeater ID="studentsRepeater" runat="server" OnItemCommand="studentsRepeater_ItemCommand">
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
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditStudent" 
                                            CommandArgument='<%# Eval("id") %>' CssClass="btn btn-sm btn-primary">
                                            Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteStudent" 
                                            CommandArgument='<%# Eval("id") %>' CssClass="btn btn-sm btn-danger"
                                            OnClientClick="return confirm('Are you sure you want to delete this student?');">
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
                    
                    <asp:Label ID="lblNoRecords" runat="server" CssClass="text-center d-block" Text="No students found." Visible="false"></asp:Label>
                </div>
            </div>
        </div>
    
</asp:Content>