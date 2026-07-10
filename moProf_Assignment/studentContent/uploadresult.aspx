<%@ Page Title="Student Result Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentResultUpload.aspx.cs" Inherits="moProf_Assignment.StudentResultUpload" %>

<%@ Register Src="~/usercontrol/Studentnavbar.ascx" TagPrefix="uc1" TagName="Studentnavbar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    <uc1:Studentnavbar runat="server" ID="Studentnavbar" />

    <div class="container mt-4">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="card shadow">
                    <div class="card-header bg-primary text-white">
                        <h4 class="mb-0">Upload Academic Results</h4>
                    </div>
                    <div class="card-body">
                        <!-- Success/Error Messages -->
                        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert" role="alert">
                            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                        </asp:Panel>

                        <!-- Student Information (Read-only) -->
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Student Name:</label>
                                <asp:Label ID="lblStudentName" runat="server" CssClass="form-control-plaintext" Text=""></asp:Label>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Email:</label>
                                <asp:Label ID="lblStudentEmail" runat="server" CssClass="form-control-plaintext" Text=""></asp:Label>
                            </div>
                        </div>

                        <hr />

                        <!-- Image Upload Section -->
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label for="fileUpload" class="form-label fw-bold">Upload Result Document/Image</label>
                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
                                <small class="text-muted">Supported formats: JPG, PNG, PDF, DOC, DOCX (Max size: 5MB)</small>

                                <!-- Display existing image -->
                                <asp:Image ID="imgResult" runat="server" CssClass="img-fluid mt-2" Style="max-height: 200px;" Visible="false" />
                                <asp:Label ID="lblFileName" runat="server" CssClass="text-muted d-block mt-1" Visible="false"></asp:Label>
                            </div>
                        </div>

                        <hr />

                        <!-- Result Form -->
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label for="txtGrade" class="form-label fw-bold">Grade / GPA <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control" placeholder="e.g., A, B+, 85%, 3.5 GPA" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvGrade" runat="server" ControlToValidate="txtGrade"
                                    ErrorMessage="Grade is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label for="txtSchoolName" class="form-label fw-bold">School / Institution <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtSchoolName" runat="server" CssClass="form-control" placeholder="Enter your school name" MaxLength="200"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" ControlToValidate="txtSchoolName"
                                    ErrorMessage="School name is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label for="txtPreferredSubjects" class="form-label fw-bold">Preferred Subjects</label>
                                <asp:TextBox ID="txtPreferredSubjects" runat="server" CssClass="form-control" placeholder="e.g., Mathematics, Science, English" MaxLength="500" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                <small class="text-muted">Separate multiple subjects with commas</small>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtTotalBookings" class="form-label fw-bold">Total Bookings</label>
                                <asp:TextBox ID="txtTotalBookings" runat="server" CssClass="form-control" type="number" min="0" Text="0"></asp:TextBox>
                                <asp:RangeValidator ID="rvTotalBookings" runat="server" ControlToValidate="txtTotalBookings"
                                    ErrorMessage="Please enter a valid number" CssClass="text-danger" Display="Dynamic"
                                    MinimumValue="0" MaximumValue="999999" Type="Integer" ValidationGroup="Submit"></asp:RangeValidator>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtTotalSpent" class="form-label fw-bold">Total Spent ($)</label>
                                <asp:TextBox ID="txtTotalSpent" runat="server" CssClass="form-control" type="number" step="0.01" min="0" Text="0.00"></asp:TextBox>
                                <asp:RangeValidator ID="rvTotalSpent" runat="server" ControlToValidate="txtTotalSpent"
                                    ErrorMessage="Please enter a valid amount" CssClass="text-danger" Display="Dynamic"
                                    MinimumValue="0" MaximumValue="999999.99" Type="Double" ValidationGroup="Submit"></asp:RangeValidator>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label class="form-label fw-bold">Last Updated</label>
                                <asp:Label ID="lblUpdatedAt" runat="server" CssClass="form-control-plaintext" Text="Not updated yet"></asp:Label>
                            </div>
                        </div>

                        <hr />

                        <!-- Buttons -->
                        <div class="d-flex justify-content-between">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save Results" CssClass="btn btn-success btn-lg"
                                OnClick="btnSubmit_Click" ValidationGroup="Submit" />
                            <asp:Button ID="btnReset" runat="server" Text="Clear Form" CssClass="btn btn-secondary btn-lg"
                                OnClick="btnReset_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <style>
        .form-control-plaintext {
            padding-top: calc(0.375rem + 1px);
            padding-bottom: calc(0.375rem + 1px);
            margin-bottom: 0;
        }

        .alert {
            border-radius: 0.25rem;
        }

        .card {
            border-radius: 0.5rem;
        }
    </style>
</asp:Content>
