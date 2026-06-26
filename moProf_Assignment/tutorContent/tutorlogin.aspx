<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tutorlogin.aspx.cs" Inherits="moProf_Assignment.tutor" %>

<%@ Register Src="~/usercontrol/LoginForm.ascx" TagPrefix="uc1" TagName="LoginForm" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">

    <uc1:LoginForm  runat="server" id="LoginForm" />
   

</asp:Content>
