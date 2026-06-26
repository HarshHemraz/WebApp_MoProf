<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="registerpage.aspx.cs" Inherits="moProf_Assignment.registerpage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
     <section class="login align-items-center pb-5 pt-3 ">
     <div class="container loginform">
         <div class="row justify-content-center">
             <div class="col-md-5">
                 <div class="login-box p-4">
                     <div class="text-center mt-4"">
                         <h3 class=" focus-ring animate-fade-in" style="animation: fadeIn 5s forwards; padding-bottom: 0.8rem;">Welcome new user</h3>
                         <p>Please enter the following details:</p>
                     </div>

                     <div class="passemailtxt">
                         <div class="mb-3">

                             
                             <asp:TextBox ID="fname" class="form-control" runat="server" placeholder="First Name"></asp:TextBox>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ForeColor="Red" runat="server" 
                                 ErrorMessage="*First name is required." CssClass="errrormsg" ControlToValidate="fname"></asp:RequiredFieldValidator>
                             
                             <asp:TextBox ID="lname" class="form-control" runat="server" placeholder="Last Name"></asp:TextBox>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ForeColor="Red" runat="server" 
                                 ErrorMessage="*Last name is required." CssClass="errrormsg" ControlToValidate="lname"></asp:RequiredFieldValidator>
                             <asp:TextBox ID="emailtxt" class="form-control" runat="server" placeholder="Email Address"></asp:TextBox>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ForeColor="Red" runat="server" 
                                 ErrorMessage="*Email is required." CssClass="errrormsg" ControlToValidate="emailtxt"></asp:RequiredFieldValidator>
                         
                             <asp:TextBox ID="passwordtxt" class="form-control" runat="server" placeholder="Password" TextMode="Password"></asp:TextBox>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ForeColor="Red" runat="server" 
                                 ErrorMessage="*Password is required." CssClass="errrormsg" ControlToValidate="passwordtxt"></asp:RequiredFieldValidator>

                             <asp:TextBox ID="rpassword" class="form-control" runat="server" placeholder="Repeat Password" TextMode="Password"></asp:TextBox>
                             <div>
                             <asp:CompareValidator ID="matchPass" ControlToValidate="rpassword" ControlToCompare="passwordtxt" 
                                 Operator="Equal" Type="String" ForeColor="Red" CssClass="errrormsg"  runat="server" ErrorMessage="*Password does not match!"></asp:CompareValidator>
                                 </div>
                             <div>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ForeColor="Red" runat="server" 
ErrorMessage="*Password is required." CssClass="errrormsg" ControlToValidate="rpassword"></asp:RequiredFieldValidator>
                         </div>
                             </div>
                         

                     </div>


                     <div class="radio-options">
                     <asp:Label ID="Label2" runat="server" Text="Register as:" CssClass="d-block text-center "></asp:Label>

                   
                     <div class="radio-btn">
                     <asp:RadioButtonList ID="RegisterOption"   RepeatDirection="Horizontal" runat="server">
                         <asp:ListItem Text="Student" CssClass="register-option" Value="student"></asp:ListItem>
                         <asp:ListItem Text="Tutors" CssClass="register-option"  Value="tutor"></asp:ListItem>
                     </asp:RadioButtonList>
                         
                         </div>
                         <asp:RequiredFieldValidator ID="rovOption" runat="server" ControlToValidate="RegisterOption"
    ErrorMessage="*Please select your desired option" ForeColor="Red" CssClass="d-block text-center "></asp:RequiredFieldValidator>

                         </div>
                     <div class="form-check text-center " style="padding-bottom: 1rem">
                         <asp:CheckBox ID="checkbxRemeberMe" type="checkbox"
                             runat="server" />
                         <asp:Label ID="Label1" class="form-check-label" runat="server" Text="Remember me"></asp:Label>
                     </div>
                     <div class="text-center w-100">

                         <asp:Button ID="registerBtn" class="btn  btn-custom w-100 text-white" OnClick="registerBtn_Click"   Text="Register" runat="server" />
                     </div>
                     <div class="text-center mt-3">
                         <a href="#" class="hovereffect text-decoration-none">Forgot password?</a>
                         <br>
                         <a href="/tutorContent/tutorlogin.aspx" class="hovereffect text-decoration-none">Already have an account?</a>
                        
                     </div>
                 </div>
             </div>
         </div>
     </div>
        
 </section>








</asp:Content>
