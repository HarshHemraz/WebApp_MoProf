<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addcourses.aspx.cs" Inherits="moProf_Assignment.tutorContent.addcourses" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">


    <div class="main-layout-container ">

        <uc1:sidebar runat="server" ID="sidebar" />

        <section class="add-courses">
            <h2>Add Courses</h2>
            <div class="form-card">
                <asp:Label
                    ID="lblCourseName"
                    Text="Course Name:"
                    runat="server" />
                <asp:TextBox
                    ID="TextBox2"
                    runat="server" />
                <br />
                <asp:FileUpload ID="fileUploadImage" runat="server"  onchange="triggerUpload()"  />




<asp:Image ID="imgDisplay" runat="server" Visible="false" Width="300px" />
                <asp:TextBox
                    ID="lblCourseDesc"
                    runat="server" />
                <br />
                <asp:Label
    ID="Label2"
    Text="Course Name:"
    runat="server" />
<asp:TextBox
    ID="TextBox1"
    runat="server" />
<br />
<asp:Label
    ID="Label3"
    Text="Course Desc:"
    runat="server" />
<asp:TextBox
    ID="TextBox3"
    runat="server" />


                <asp:Button
                    ID="btnSubmit"
                    Text="Submit"
                    runat="server" />
            </div>
        </section>
    </div>
</asp:Content>
