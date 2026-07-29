<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="adminpanel.aspx.cs" Inherits="moProf_Assignment.adminpanel" %>

<%@ Register Src="~/usercontrol/adminsidebar.ascx" TagPrefix="uc1" TagName="adminsidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

   

    <div class="main-layout-container">
        <section class="add-courses-section">
             <uc1:adminsidebar runat="server" ID="adminsidebar" />
            <div class="container mt-4">
                <h2 class="mb-4 text-primary fw-bold">Admin Dashboard</h2>

                <!-- Summary Cards -->
                <div class="row g-3 mb-4">
                    <div class="col-6 col-md-3">
                        <div class="card text-white bg-primary shadow-sm h-100">
                            <div class="card-body">
                                <h6 class="card-title mb-1">Total Students</h6>
                                <h2 class="fw-bold mb-0"><asp:Label ID="lblTotalStudents" runat="server" Text="0" /></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="card text-white bg-success shadow-sm h-100">
                            <div class="card-body">
                                <h6 class="card-title mb-1">Total Tutors</h6>
                                <h2 class="fw-bold mb-0"><asp:Label ID="lblTotalTutors" runat="server" Text="0" /></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="card text-white bg-warning shadow-sm h-100">
                            <div class="card-body">
                                <h6 class="card-title mb-1">Total Courses</h6>
                                <h2 class="fw-bold mb-0"><asp:Label ID="lblTotalCourses" runat="server" Text="0" /></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="card text-white bg-info shadow-sm h-100">
                            <div class="card-body">
                                <h6 class="card-title mb-1">Total Bookings</h6>
                                <h2 class="fw-bold mb-0"><asp:Label ID="lblTotalBookings" runat="server" Text="0" /></h2>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Charts -->
                <div class="row g-4">
                    <div class="col-lg-6">
                        <div class="card shadow-sm h-100">
                            <div class="card-body">
                                <h5 class="card-title fw-bold mb-3">Courses by Subject</h5>
                                <asp:Label ID="lblNoSubjectData" runat="server" CssClass="text-muted" Visible="false" Text="No course data yet." />
                                <canvas id="chartCoursesBySubject" height="260"></canvas>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6">
                        <div class="card shadow-sm h-100">
                            <div class="card-body">
                                <h5 class="card-title fw-bold mb-3">Bookings by Status</h5>
                                <asp:Label ID="lblNoBookingData" runat="server" CssClass="text-muted" Visible="false" Text="No booking data yet." />
                                <canvas id="chartBookingsByStatus" height="260"></canvas>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12">
                        <div class="card shadow-sm">
                            <div class="card-body">
                                <h5 class="card-title fw-bold mb-3">New Student Sign-ups (Last 6 Months)</h5>
                                <asp:Label ID="lblNoGrowthData" runat="server" CssClass="text-muted" Visible="false" Text="No student sign-up data yet." />
                                <canvas id="chartStudentGrowth" height="120"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </section>
    </div>

    <!-- Chart.js -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.0/chart.umd.min.js"></script>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {

            // ----- Courses by Subject (pie) -----
            var subjectLabels = <asp:Literal ID="litSubjectLabels" runat="server" Text="[]" />;
            var subjectData = <asp:Literal ID="litSubjectData" runat="server" Text="[]" />;

            if (subjectLabels.length > 0) {
                new Chart(document.getElementById('chartCoursesBySubject'), {
                    type: 'pie',
                    data: {
                        labels: subjectLabels,
                        datasets: [{
                            data: subjectData,
                            backgroundColor: ['#0d6efd', '#198754', '#ffc107', '#0dcaf0', '#dc3545', '#6f42c1', '#fd7e14', '#20c997']
                        }]
                    },
                    options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
                });
            }

            // ----- Bookings by Status (doughnut) -----
            var statusLabels = <asp:Literal ID="litStatusLabels" runat="server" Text="[]" />;
            var statusData = <asp:Literal ID="litStatusData" runat="server" Text="[]" />;

            if (statusData.reduce(function (a, b) { return a + b; }, 0) > 0) {
                new Chart(document.getElementById('chartBookingsByStatus'), {
                    type: 'doughnut',
                    data: {
                        labels: statusLabels,
                        datasets: [{
                            data: statusData,
                            backgroundColor: ['#ffc107', '#198754', '#dc3545']
                        }]
                    },
                    options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
                });
            }

            // ----- New Student Sign-ups (line) -----
            var growthLabels = <asp:Literal ID="litGrowthLabels" runat="server" Text="[]" />;
            var growthData = <asp:Literal ID="litGrowthData" runat="server" Text="[]" />;

            if (growthLabels.length > 0) {
                new Chart(document.getElementById('chartStudentGrowth'), {
                    type: 'line',
                    data: {
                        labels: growthLabels,
                        datasets: [{
                            label: 'New Students',
                            data: growthData,
                            borderColor: '#0d6efd',
                            backgroundColor: 'rgba(13,110,253,0.15)',
                            fill: true,
                            tension: 0.3
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: { legend: { display: false } },
                        scales: { y: { beginAtZero: true, ticks: { precision: 0 } } }
                    }
                });
            }
        });
    </script>

</asp:Content>
