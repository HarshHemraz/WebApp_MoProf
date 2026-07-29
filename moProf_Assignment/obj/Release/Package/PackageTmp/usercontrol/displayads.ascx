<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="displayads.ascx.cs" Inherits="moProf_Assignment.usercontrol.displayads" %>

<div class="ads-container d-flex flex-row w-75 justify-content-center gap-5" >
    <asp:Repeater ID="adsRepeater"  runat="server">
        <ItemTemplate>
            <div class="ad-box mb-3">
                <a href='<%# Eval("LinkUrl") %>' target="_blank" rel="noopener noreferrer">
                    <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Title") %>' class="img-fluid ad-image" />
                </a>
                <p class="ad-caption text-center mt-1"><%# Eval("Title") %></p>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
