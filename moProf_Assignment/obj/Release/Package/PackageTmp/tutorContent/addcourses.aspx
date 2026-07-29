<%@ Page Title="Add Courses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addcourses.aspx.cs" Inherits="moProf_Assignment.tutorContent.addcourses" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    

    <div class="main-layout-container">
       
        <section class="add-courses-section">
             <uc1:sidebar runat="server" ID="ucSidebar" />
            <div class="form-card">
                <h2>Add Courses</h2>
                <asp:Label ID="lblCourseName" Text="Course Name:" runat="server" />
                <asp:TextBox ID="txtcoursename" CssClass="txtField" Placeholder="Enter Course Name" runat="server" />
                <br />

                <asp:Label ID="Label1" Text="Course Description" runat="server" />
                <asp:TextBox ID="txtcrsdesc" CssClass="txtField" Placeholder="Enter Course Description" runat="server" />
                <br />

                <asp:Label ID="Label2" Text="Add Image:" runat="server" />
                <br />
                <asp:FileUpload ID="fileUploadImage" CssClass="txtField"  runat="server" onchange="previewSelectedImage(this);" />
                <br />
                <br />

                <asp:Image ID="imgDisplay" runat="server" Width="300px" Style="display: none;" />
                <br />

                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="txtField"
                    OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                </asp:DropDownList>
                <br />

                <asp:Label ID="Label3" Text="Fees: " runat="server" />
                <asp:TextBox ID="feetxt" CssClass="txtField" Placeholder="Enter Fee(Rs)" runat="server" />
                <br />
                <asp:Label ID="Label4" Text="Location " runat="server" />
                <asp:TextBox ID="locationtxt" CssClass="txtField" Placeholder="Enter Course Location" runat="server" />
                <br />
                <asp:Label ID="Label5" Text="Teaching Experience " runat="server" />
                <asp:TextBox ID="exptxt" CssClass="txtField" Placeholder="Teaching Experience"  runat="server" />
                <br />
                <asp:Label ID="Label6" Text="Time table " runat="server" />
                <asp:TextBox ID="rxtTime" CssClass="txtField" Placeholder="Enter time course" runat="server" />
                <br />

                <asp:Button ID="btnSubmit" Text="Submit" OnClick="btnSubmit_Click" runat="server" />
            </div>
        </section>
    </div>

    <script type="text/javascript">
        function previewSelectedImage(input) {
            var imgControl = document.getElementById('<%= imgDisplay.ClientID %>');
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    imgControl.src = e.target.result;
                    imgControl.style.display = 'block';
                }
                reader.readAsDataURL(input.files[0]);
            } else {
                imgControl.src = '';
                imgControl.style.display = 'none';
            }
        }
    </script>
</asp:Content>
