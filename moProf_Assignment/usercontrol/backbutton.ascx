<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="backbutton.ascx.cs" Inherits="moProf_Assignment.usercontrol.backbutton" %>

<asp:Button ID="btnBack" runat="server"
    Text="Back" CssClass="back-btn"
    OnClientClick="history.back(); return false;" />