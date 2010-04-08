<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteMaster.Master"
    Inherits="System.Web.Mvc.ViewPage<DomainModel.Entities.Cart>" %>

<%@ Import Namespace="DomainModel.Entities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Cart</h2>
    <%
        foreach (var line in Model.Lines)
        {%>
    <p>
        <%=line.Product.Name %></p>
    <p>
        <%=line.Quantity %></p>
    <p>
        <%=Model.ComputeTotalValue() %></p>
    <p>
        -------------------</p>
    <%
        using (Html.BeginForm("RemoveFromCart", "Cart"))
        {%>
    <%=Html.Hidden("productId", line.Product.ProductId) %>
    <%=Html.Hidden("returnUrl", ViewData["returnUrl"]) %>
    <input type="submit" value="Remove" />
    <% }
        } %>
    <a href="<%= Html.Encode(ViewData["returnUrl"])%>">Continue Shopping</a>
    
    <%=Html.ActionLink("Check out now", "CheckOut") %>
</asp:Content>
