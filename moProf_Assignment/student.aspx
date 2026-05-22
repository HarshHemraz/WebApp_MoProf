<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="student.aspx.cs" Inherits="moProf_Assignment.student" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

        <section class="login d-flex align-items-center "  >
       
    <div class="container loginform" >
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div class="login-box p-4">

                    

                    <div class="text-center mb-4">
                      
                        <h3 class=" focus-ring animate-fade-in" style="animation: fadeIn 5s forwards; padding-bottom: 0.8rem;">
  Welcome Student
</h3>
                        <p>Please sign in to your peaceful area</p>
                    </div>

                    <div class="passemailtxt">
                    <div class="mb-3" >

                        <asp:TextBox ID="emailtxt" class="form-control" runat="server" placeholder="Email Address"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ForeColor="Red" runat="server" ErrorMessage="       *Email is required." ControlToValidate="emailtxt"></asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="mb-3">
                        <asp:TextBox ID="passwordtxt" class="form-control" runat="server" placeholder="Password" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ForeColor="Red" runat="server" ErrorMessage=" *Password is required." ControlToValidate="passwordtxt"></asp:RequiredFieldValidator>


                    </div>

                        </div>
                    
                    <div class="form-check text-center" ">

                        <asp:CheckBox ID="CheckBox1" type="checkbox"  
                            runat="server" />

                        <asp:Label ID="Label1" class="form-check-label" runat="server" Text="Remember me" 
                            ></asp:Label>

                       


                    </div>
                    <div class="text-center w-100">

                    <asp:Button ID="Button1" class="btn  btn-custom w-50 text-white" OnClick="Login_Click" runat="server" Text="Login" />

                   </div>
                    
                    <div class="text-center mt-3">
                        <a href="#" class="text-decoration-none">Forgot password?</a>
                        <br>
                        <a href="#" class="text-decoration-none">Create new account</a>
                    </div>
                </div>
            </div>
        </div>
    </div>


            </section>

    



</asp:Content>
