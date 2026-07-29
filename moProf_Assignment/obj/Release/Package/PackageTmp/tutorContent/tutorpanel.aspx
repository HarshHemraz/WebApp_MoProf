<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tutorpanel.aspx.cs" Inherits="moProf_Assignment.tutorContent.tutormainpage" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <div class="main-layout-container">
        <section class="add-courses-section">
            <uc1:sidebar runat="server" ID="sidebar" />

            <div class="bookings-card w-50 p-5 d-flex justify-content-center flex-column ">
                <h2>Booking Requests</h2>
                <hr />

                <asp:Label ID="lblMessage" runat="server" CssClass="text-center d-block" Font-Size="Medium"></asp:Label>

                <%-- Wrapping everything in an UpdatePanel turns the button clicks into
                     async (AJAX) postbacks instead of full-page reloads. This is what
                     kills the "just refreshes" feeling, and it's also what makes
                     ScriptManager.RegisterStartupScript reliably pop the modal,
                     since the modal's bootstrap.Modal(...).show() call runs right
                     after the async update completes instead of racing a full
                     page/script reload. --%>
                <asp:UpdatePanel ID="upBookings" runat="server" UpdateMode="Always">
                    <ContentTemplate>

                        <asp:Repeater ID="rptBookings" runat="server" OnItemCommand="rptBookings_ItemCommand">
                            <ItemTemplate>
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-8">
                                                <h5 class="card-title"><%# Eval("c_name") %></h5>
                                                <p class="mb-1"><strong>Student:</strong> <%# Eval("firstname") %> <%# Eval("lastname") %></p>
                                                <p class="mb-1"><strong>Email:</strong> <%# Eval("email") %></p>
                                                <p class="mb-1"><strong>Requested Date:</strong> <%# Eval("booking_date", "{0:MMM dd, yyyy}") %></p>
                                                <p class="mb-1"><strong>Message:</strong> <%# Eval("messages") %></p>
                                                <p class="mb-1">
                                                    <strong>Status:</strong>
                                                    <%# GetStatusHtml(Eval("isaccepted")) %>
                                                </p>
                                            </div>
                                            <div class="col-md-12 text-end align-self-center">

                                                <asp:Button ID="btnAccept" runat="server" Text="Accept" CssClass="btn btn-success btn-sm me-2"
                                                    CommandName="Accept" CommandArgument='<%# Eval("br_id") %>' />
                                                <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-danger btn-sm me-2"
                                                    CommandName="Reject" CommandArgument='<%# Eval("br_id") %>' />
                                                <asp:Button ID="btnViewResult" runat="server" Text="View Academic Result" CssClass="btn btn-info btn-sm me-2"
                                                    CommandName="ViewResult" CommandArgument='<%# Eval("br_id") %>' />
                                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-outline-secondary btn-sm mt-2"
                                                    CommandName="Delete" CommandArgument='<%# Eval("br_id") %>'
                                                    OnClientClick="return confirm('Are you sure you want to delete this booking request?');" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <!-- Academic Result Modal -->
                        <style>
                            #resultModal.modal.show {
                                display: flex !important;
                                align-items: center;
                                justify-content: center;
                                padding-bottom:30rem;
                            }

                            #resultModal .modal-dialog {
                                margin: 0 30rem;
                            }
                        </style>
                        <div class="modal fade" id="resultModal" tabindex="-1">
                            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">Student Academic Result</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="table-responsive">
                                            <table class="table table-bordered table-striped">
                                                <tr>
                                                    <th style="width: 40%">Student ID</th>
                                                    <td>
                                                        <asp:Label ID="lblStudentID" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Grade</th>
                                                    <td>
                                                        <asp:Label ID="lblGrade" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>School Name</th>
                                                    <td>
                                                        <asp:Label ID="lblSchoolName" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Preferred Subjects</th>
                                                    <td>
                                                        <asp:Label ID="lblPreferredSubjects" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Total Bookings</th>
                                                    <td>
                                                        <asp:Label ID="lblTotalBookings" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Total Spent (RM)</th>
                                                    <td>
                                                        <asp:Label ID="lblTotalSpent" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Updated At</th>
                                                    <td>
                                                        <asp:Label ID="lblUpdatedAt" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Created At</th>
                                                    <td>
                                                        <asp:Label ID="lblCreatedAt" runat="server" Text="-" /></td>
                                                </tr>
                                                <tr>
                                                    <th>User ID</th>
                                                    <td>
                                                        <asp:Label ID="lblUserID" runat="server" Text="-" /></td>
                                                </tr>
                                            </table>
                                        </div>

                                        <div class="text-center mt-3">
                                            <asp:Image ID="imgResult" runat="server" CssClass="img-fluid" Style="max-height: 400px;" Visible="false" />
                                            <asp:Label ID="lblNoResult" runat="server" Text="No academic result image uploaded." CssClass="text-muted" Visible="false" />
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </section>
    </div>

</asp:Content>
