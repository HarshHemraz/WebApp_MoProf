<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tutormainapge.aspx.cs" Inherits="moProf_Assignment.tutormainapge" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent2" runat="server">
     <!-- SIDEBAR -->

  <aside id="sidebar" class="sidebar">
    <div class="logo-area">
     <a href="index.html" class="d-inline-flex"><img src="./assets/images/logo-icon.svg" alt="" width="24">
        <span class="logo-text ms-2"> <img src="./assets/images/logo.svg" alt=""></span>
      </a>
    </div>
    <ul class="nav flex-column">
      <li class="px-4 py-2"><small class="nav-text">Main</small></li>
      <li><a class="nav-link active" href="index.html"><i class="ti ti-home"></i><span
            class="nav-text">Dashboard</span></a></li>
      <li><a class="nav-link" href="inventory.html"><i class="ti ti-box-seam"></i><span
            class="nav-text">Inventory</span></a></li>
      <li><a class="nav-link" href="create-product.html"><i class="ti ti-plus"></i><span class="nav-text">Add
            Product</span></a></li>
    <li><a class="nav-link" href="reports.html"><i class="ti ti-receipt"></i><span class="nav-text">Reports</span></a>
      </li>
    <li><a class="nav-link" href="404-error.html"><i class="ti ti-alert-circle"></i><span class="nav-text">404 Error</span></a>
      </li>
      <li><a class="nav-link" href="docs.html"><i class="ti ti-file-text"></i><span class="nav-text">Docs</span></a></li>


      <li class="px-4 pt-4 pb-2"><small class="nav-text">Account</small></li>
      <li><a class="nav-link" href="signin.html"><i class="ti ti-logout"></i><span class="nav-text">Log in</span></a>
      </li>
      <li><a class="nav-link" href="signup.html"><i class="ti ti-user-plus"></i><span class="nav-text">Sign
            up</span></a></li>
    </ul>

  </aside>






</asp:Content>
