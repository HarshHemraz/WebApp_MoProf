<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginForm.ascx.cs" Inherits="moProf_Assignment.usercontrol.LoginForm" %>
<section class="login d-flex align-items-center ">
    <div class="container loginform pt-5">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div class="login-box p-4">
                    <div class="text-center mt-4">
                        <h3 id="welcomeHeading" runat="server"
                            class="focus-ring animate-fade-in"
                            style="animation: fadeIn 5s forwards; padding-bottom: 0.8rem;">
                            Welcome
                        </h3>
                    </div>

                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3"></asp:Label>
                    <asp:Label ID="lblOtpMessage" runat="server" CssClass="d-block mb-3"></asp:Label>

                    <!-- Step 1: Email + Password -->
                    <asp:Panel ID="pnlLoginStep" runat="server">
                        <p>Please sign in to your peaceful area</p>
                        <div class="passemailtxt">
                            <div class="mb-3">
                                <asp:TextBox ID="emailtxt" class="form-control" runat="server" placeholder="Email Address"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ForeColor="Red" runat="server" ErrorMessage="*Email is required." CssClass="errrormsg" ControlToValidate="emailtxt"></asp:RequiredFieldValidator>
                            </div>
                            <div class="mb-3">
                                <asp:TextBox ID="passwordtxt" class="form-control" runat="server" placeholder="Password" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ForeColor="Red" runat="server" ErrorMessage="*Password is required." CssClass="errrormsg" ControlToValidate="passwordtxt"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-check text-center " style="padding-bottom: 1rem">
                            <asp:CheckBox ID="CheckBox1" type="checkbox" runat="server" />
                            <asp:Label ID="Label1" class="form-check-label" runat="server" Text="Remember me"></asp:Label>
                        </div>
                        <div class="text-center w-100">
                            <asp:Button ID="loginBtn" OnClick="loginBtn_Click" class="btn btn-custom w-100 text-white" runat="server" Text="Login" />
                        </div>
                        <div class="text-center mt-3 d-flex flex-column">
                            <a href="#" class="hovereffect text-decoration-none">Forgot password?</a>
                            <br>
                            <asp:LinkButton ID="createAcct" OnClick="createAcct_Click" runat="server" CssClass="hovereffect text-decoration-none mt-2" CausesValidation="false">Don't have an account? Create Account</asp:LinkButton>
                        </div>
                    </asp:Panel>

                    <!-- Step 2: OTP verification -->
                    <asp:Panel ID="pnlOtpStep" runat="server" Visible="false">
                        <p>Enter the 6-digit code sent to your email.</p>
                        <div class="mb-3">
                            <asp:TextBox ID="otpTxt" class="form-control" runat="server" placeholder="Enter code" MaxLength="6"></asp:TextBox>
                        </div>
                        <div class="text-center w-100">
                            <asp:Button ID="verifyOtpBtn" OnClick="verifyOtpBtn_Click" class="btn btn-custom w-100 text-white" runat="server" Text="Verify Code" />
                        </div>
                        <div class="text-center mt-3 d-flex flex-column">
                            <asp:LinkButton ID="resendOtpBtn" runat="server" OnClick="resendOtpBtn_Click" CausesValidation="false" CssClass="hovereffect text-decoration-none">Resend code</asp:LinkButton>
                            <asp:LinkButton ID="backToLoginBtn" runat="server" OnClick="backToLoginBtn_Click" CausesValidation="false" CssClass="hovereffect text-decoration-none mt-2">Back to login</asp:LinkButton>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</section>