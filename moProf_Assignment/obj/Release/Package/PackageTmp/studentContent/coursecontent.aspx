<%@ Page Title="Course Content" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="coursecontent.aspx.cs" Inherits="moProf_Assignment.studentContent.coursecontent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <div class="container mt-4">
        <h2>Course Details</h2>
        <hr />

        <asp:Repeater ID="rptCourseDetails" runat="server">
            <ItemTemplate>
                <!-- Course Card -->
                <div class="card mb-4">
                    <div class="card-body">
                        <div class="row">
                            <!-- Course Image -->
                            <div class="col-md-4">
                                <%# GetImageHtml(Eval("image"), Eval("c_name")) %>
                            </div>

                            <!-- Course Details -->
                            <div class="col-md-8">
                                <h3 class="card-title"><%# Eval("c_name") %></h3>
                                <p class="card-text"><%# Eval("c_desc") %></p>
                                <ul class="list-group list-group-flush">
                                    <li class="list-group-item"><strong>Price:</strong> Rs<%# Eval("c_price") %></li>
                                    <li class="list-group-item"><strong>Category:</strong> <%# GetSafeString(Eval("category")) %></li>
                                    <li class="list-group-item"><strong>Location:</strong> <%# GetSafeString(Eval("location")) %></li>
                                    <li class="list-group-item"><strong>Experience Required:</strong> <%# GetSafeString(Eval("experience")) %></li>
                                    <li class="list-group-item"><strong>Schedule:</strong> <%# GetSafeString(Eval("timestable")) %></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Tutor Details Section -->
                <div class="card mb-4">
                    <div class="card-body">
                        <div class="row">
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0 text-center">👨‍🏫 About the Tutor</h5>
                            </div>
                            <div class="card-body">
                                <%# IsTutorAssigned(Eval("firstname"), Eval("lastname")) ? 
                                    GetTutorDetailsHtml(Container.DataItem) : 
                                    GetNoTutorMessageHtml() %>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Back Button -->
                <div class="text-center mt-3 mb-5">
               <button type="button" class="btn btn-primary"
    data-bs-toggle="modal"
    data-bs-target="#bookingModal">
    Register to Course 📋
</button>
                    <a href="javascript:history.back()" class="btn btn-secondary">⬅ Back to Courses</a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <!-- Booking Request Modal -->
<div class="modal fade" id="bookingModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">

            <div class="modal-header">
                <h5 class="modal-title">Booking Request</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>

            <div class="modal-body">

                <div class="mb-3">
                    <label>Booking Date</label>
                    <asp:TextBox ID="txtBookingDate"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Date"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label>Message</label>
                    <asp:TextBox ID="txtMessage"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="4"></asp:TextBox>
                </div>

            </div>

            <div class="modal-footer">
                <button type="button"
                    class="btn btn-secondary"
                    data-bs-dismiss="modal">
                    Cancel
                </button>

                <asp:Button ID="btnBooking"
                    runat="server"
                    Text="Submit Request"
                    CssClass="btn btn-success"
                    OnClick="btnBooking_Click"/>
            </div>

        </div>
    </div>
</div>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-center txt-mes" Font-Size="Large"></asp:Label>
    </div>

    <style>
        .card {
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);

        }

        .card-header {
            border-radius: 10px 10px 0 0;
        }

        .list-group-item {
            border: none;
            padding: 8px 0;
        }

            .list-group-item:last-child {
                border-bottom: none;
            }

        .bg-primary {
            background-color: #007bff !important;
        }

        .course-image {
            max-height: 250px;
            width: 100%;
            object-fit: cover;
            border-radius: 8px;
        }
        .txt-mes {
            margin:0;
            
        }

        .no-image {
            background-color: #f8f9fa;
            border: 2px dashed #ddd;
            border-radius: 8px;
            padding: 40px 20px;
            text-align: center;
            color: #999;
        }

        .text-center {
            text-align: center;
            
        }

        .text-muted {
            color: #6c757d;
        }

        .text-success {
            color: #28a745;
        }

        .text-danger {
            color: #dc3545;
        }

        .no-tutor-message {
            text-align: center;
            padding: 20px;
        }

            .no-tutor-message .icon {
                font-size: 48px;
                display: block;
                margin-bottom: 10px;
            }

            .no-tutor-message .title {
                font-size: 18px;
                font-weight: bold;
                color: #333;
            }

            .no-tutor-message .subtitle {
                color: #666;
                margin-top: 5px;
            }
    </style>
</asp:Content>
