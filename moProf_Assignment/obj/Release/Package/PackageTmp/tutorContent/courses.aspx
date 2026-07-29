<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="courses.aspx.cs" Inherits="moProf_Assignment.tutorContent.courses" %>

<%@ Register Src="~/usercontrol/sidebar.ascx" TagPrefix="uc1" TagName="sidebar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
    
<uc1:sidebar runat="server" ID="sidebar" />

</asp:Content>
