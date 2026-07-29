<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="moProf_Assignment.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <main class="flex-grow-1">


        <div class="container ">


            <!-- Hero section / responsive columns to demonstrate layout fluidity -->
            <div class="row align-items-center g-4">
                <div class="info col-lg-6">

                    <h1 class=" fw-bold text-primary">MoProf Web Application</h1>
                    <p class="lead">
                        <strong>Find the perfect tutor, anytime, anywhere.<br />

                        </strong>
                        <p class="para">MoProf connects students across Mauritius with qualified private tutors - eliminating distance, time, and cost barriers. Browse subjects, check tutor qualifications, and reserve your seat today.</p>

                    <p>Sign up now and start your learning journey today!</p>
                    <a class="btn btn-primary btn-lg mt-2" href="registerpage.aspx">Sign Up Now</a>
                </div>


               <%-- Carousel--%>

                <div class="carousel  col-lg-6" style="padding-top: 4rem;">
                     
                    <div class="card shadow-sm border-0 bg-light" >
                       <p class="carousel-title fw-bold text-primary"  style="font-size: 1.5rem; font-weight: bold;">Our top trending teachers</p>
                        <div id="cardCarousel" class="carousel slide" data-bs-ride="carousel">
                            
                            <div class="carousel-inner">
                                <div class="carousel-item active">
                                    <div class="d-flex justify-content-center">
                                        <div class="card" style="width: 400px;">
                                            <img class="card-img-top" src="images/prof1.jpg" alt="Card image">
                                            <div class="card-body">
                                                <h4 class="card-title">John Doe</h4>
                                                <p class="card-text">Mathematics Tutor - 5 years experience</p>
                                                <a href="#" class="btn btn-primary">See Profile</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="carousel-item">
                                    <div class="d-flex justify-content-center">
                                        <div class="card" style="width: 400px;">
                                            <img class="card-img-top" src="images/prof1.jpg" alt="Card image">
                                            <div class="card-body">
                                                <h4 class="card-title">Jane Smith</h4>
                                                <p class="card-text">Physics Tutor - 8 years experience</p>
                                                <a href="#" class="btn btn-primary">See Profile</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="carousel-item">
                                    <div class="d-flex justify-content-center">
                                        <div class="card" style="width: 400px;">
                                            <img class="card-img-top" src="images/prof1.jpg" alt="Card image">
                                            <div class="card-body">
                                                <h4 class="card-title">Mike Brown</h4>
                                                <p class="card-text">English Tutor - 3 years experience</p>
                                                <a href="#" class="btn btn-primary">See Profile</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </div>

                        <!-- Indicators (dots at bottom - optional) -->
                        <div class="text-center mt-3">
                            <button type="button" data-bs-target="#cardCarousel" data-bs-slide-to="0" class="active btn btn-sm btn-outline-secondary rounded-circle mx-1" style="width: 12px; height: 12px; padding: 0;"></button>
                            <button type="button" data-bs-target="#cardCarousel" data-bs-slide-to="1" class="btn btn-sm btn-outline-secondary rounded-circle mx-1" style="width: 12px; height: 12px; padding: 0;"></button>
                            <button type="button" data-bs-target="#cardCarousel" data-bs-slide-to="2" class="btn btn-sm btn-outline-secondary rounded-circle mx-1" style="width: 12px; height: 12px; padding: 0;"></button>
                        </div>
                    </div>
                </div>


            </div>

            <!-- Recommended courses, pulled live from tblcourses (most-booked first) -->
            <div class="row mt-4 ">

                <p class="fw-bold text-primary" style="font-size: 1.25rem; font-weight: bold;">Recommendations:</p>

                <asp:Repeater ID="rptRecommendedCourses" runat="server">
                    <ItemTemplate>
                        <div class="col-md-4">
                            <div class="card h-100 border shadow-sm course-card"
                                onclick='<%# "window.location.href=\"CourseDetails.aspx?id=" + Eval("c_id") + "\"" %>'
                                style="cursor: pointer; transition: transform 0.2s;">
                                <img src='<%# string.IsNullOrEmpty(Eval("image").ToString()) ? "images/prof1.jpg" : "/CourseImages/" + Eval("image") %>'
                                    class="card-img-top" style="height: 180px; object-fit: cover;" alt="Course Image" />
                                <div class="card-body">
                                    <span class="badge bg-primary mb-2"><%# Eval("category") %></span>

                                    <h5 class="card-title"><i class="bi bi-book"></i> <%# Eval("c_name") %></h5>

                                    <p class="text-muted small mb-2">By <%# Eval("firstname") %> <%# Eval("lastname") %></p>

                                    <p class="card-text small"><%# Eval("c_desc") %></p>

                                    <div class="d-flex justify-content-between align-items-center mt-3">
                                        <span class="badge bg-success">Rs<%# string.Format("{0:N2}", Eval("c_price")) %></span>
                                        <small class="text-muted"><i class="bi bi-geo-alt"></i> <%# Eval("location") %></small>
                                    </div>

                                    <div class="text-end mt-2">
                                        <small class="text-primary">Click to preview &rarr;</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoCourses" runat="server" CssClass="text-muted text-center" Visible="false" Text="No courses available yet." />

            </div>

            <!-- Student Recommendations, pulled live from tblrecommendation (approved only) -->
            <div class="row mt-4">

                <p class="fw-bold text-primary" style="font-size: 1.25rem; font-weight: bold;">What our students recommend</p>

                <asp:Repeater ID="rptRecommendations" runat="server">
                    <ItemTemplate>
                        <div class="col-md-4 mb-3">
                            <div class="card h-100 border shadow-sm">
                                <div class="card-body d-flex flex-column">
                                    <span class="badge bg-info text-dark align-self-start mb-2"><%# Eval("recommendation_type") %></span>

                                    <h5 class="card-title fw-bold"><%# Eval("recommendation_title") %></h5>

                                    <p class="card-text small text-muted flex-grow-1"><%# Eval("description") %></p>

                                    <div class="d-flex justify-content-between align-items-center mt-2">
                                        <small class="text-muted">By <%# Eval("firstname") %> <%# Eval("lastname") %></small>
                                        <small class="text-muted"><%# Eval("createdat", "{0:MMM dd, yyyy}") %></small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoRecommendations" runat="server" CssClass="text-muted text-center" Visible="false" Text="No recommendations yet." />

            </div>


            <!-- Additional demo content: list group to show scroll behavior when content is taller; footer remains bottom even with short content-->
            <div class="row mt-5">
                <div class="alertbox col-12">
                    <div class="alert alert-info d-flex align-items-center" style="left: 0.8rem;" role="alert">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-info-circle-fill me-2 flex-shrink-0" viewBox="0 0 16 16">
                            <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                        </svg>
                        <div>
                            <strong>Register now to have full access:</strong> Use the portal page to login and signup.
                        </div>
                    </div>
                </div>
            </div>

            <!-- Optional: some dummy text block to ensure responsive height demonstration -->
            <div class="row mt-3">
    <div class="col">
        <p class="text-muted small text-center">
            <i class="bi bi-shield-check"></i> Verified Tutors &nbsp;|&nbsp; 
            <i class="bi bi-lock"></i> Secure Payments &nbsp;|&nbsp; 
            <i class="bi bi-star"></i> 500+ Happy Students
        </p>
    </div>
</div>
        </div>
    </main>
</asp:Content>

