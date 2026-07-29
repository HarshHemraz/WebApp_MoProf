<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addqualification.aspx.cs" Inherits="moProf_Assignment.tutorContent.addqualification" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="row">
        <div class="col-md-2">
            
<uc1:sidebar runat="server" ID="sidebar" />
        </div>
        <div class="col-md-10">
            <div class="content-wrapper p-4">
                <h2 class="mb-4 text-primary fw-bold">My Qualifications</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Visible="false"></asp:Label>

                <!-- Add Qualification Form -->
                <div class="card p-4 mb-4 shadow-sm">
                    <h5 class="mb-3">Add New Qualification</h5>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <asp:Label ID="Label1" Text="Degree / Certification Title" runat="server" CssClass="form-label" />
                            <asp:TextBox ID="txtDegree" runat="server" CssClass="form-control" placeholder="e.g. BSc Computer Science" />
                            <asp:RequiredFieldValidator ID="rfvDegree" runat="server" ControlToValidate="txtDegree"
                                ErrorMessage="*Required" ForeColor="Red" CssClass="errrormsg" Display="Dynamic" />
                        </div>

                        <div class="col-md-6">
                            <asp:Label ID="Label2" Text="Institution" runat="server" CssClass="form-label" />
                            <asp:TextBox ID="txtInstitution" runat="server" CssClass="form-control" placeholder="e.g. University of Mauritius" />
                            <asp:RequiredFieldValidator ID="rfvInstitution" runat="server" ControlToValidate="txtInstitution"
                                ErrorMessage="*Required" ForeColor="Red" CssClass="errrormsg" Display="Dynamic" />
                        </div>

                        <div class="col-md-6">
                            <asp:Label ID="Label3" Text="Field of Study" runat="server" CssClass="form-label" />
                            <asp:TextBox ID="txtField" runat="server" CssClass="form-control" placeholder="e.g. Mathematics" />
                        </div>

                        <div class="col-md-6">
                            <asp:Label ID="Label4" Text="Year Obtained" runat="server" CssClass="form-label" />
                            <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" placeholder="e.g. 2020" />
                            <asp:RegularExpressionValidator ID="revYear" runat="server" ControlToValidate="txtYear"
                                ValidationExpression="^(19|20)\d{2}$" ErrorMessage="*Enter a valid year" ForeColor="Red"
                                CssClass="errrormsg" Display="Dynamic" />
                        </div>

                        <div class="col-md-12">
                            <asp:Label ID="Label5" Text="Certificate (optional, PDF/Image)" runat="server" CssClass="form-label" />
                            <asp:FileUpload ID="fileUploadCert" runat="server" CssClass="form-control" />
                        </div>

                        <div class="col-12">
                            <asp:Button ID="btnAddQualification" runat="server" Text="Add Qualification"
                                CssClass="btn btn-primary" OnClick="btnAddQualification_Click" />
                        </div>
                    </div>
                </div>

                <!-- Existing Qualifications Table -->
                <h5 class="mb-3">Existing Qualifications</h5>
                <div class="table-responsive">
                    <table class="table table-bordered table-hover align-middle">
                        <thead class="thead-dark">
                            <tr>
                                <th>Degree / Title</th>
                                <th>Institution</th>
                                <th>Field of Study</th>
                                <th>Year</th>
                                <th>Certificate</th>
                                <th>Date Added</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptQualifications" runat="server" OnItemCommand="rptQualifications_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("degree_title") %></td>
                                        <td><%# Eval("institution") %></td>
                                        <td><%# Eval("field_of_study") %></td>
                                        <td><%# Eval("year_obtained") %></td>
                                        <td>
                                            <%# Eval("certificate_file") != DBNull.Value && !string.IsNullOrEmpty(Eval("certificate_file").ToString())
                                                ? "<a href='/QualificationFiles/" + Eval("certificate_file") + "' target='_blank'>View</a>"
                                                : "<span class='text-muted'>None</span>" %>
                                        </td>
                                        <td><%# Convert.ToDateTime(Eval("dateAdded")).ToString("yyyy-MM-dd") %></td>
                                        <td>
                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteQualification"
                                                CommandArgument='<%# Eval("q_id") %>' CssClass="btn btn-sm btn-danger"
                                                OnClientClick="return confirm('Delete this qualification?');">
                                                Delete
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <asp:Label ID="lblNoRecords" runat="server" CssClass="text-center d-block" Text="No qualifications added yet." Visible="false"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>