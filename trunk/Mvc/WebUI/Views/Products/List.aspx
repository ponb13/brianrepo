<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteMaster.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<DomainModel.Entities.Product>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Products
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=string.IsNullOrEmpty((string)ViewData["Category"]) ? "All Products" : Html.Encode(ViewData["Category"]) %>
    <h2>
        List</h2>
    <%=Html.PageLinks((int)ViewData["CurrentPage"], (int)ViewData["TotalPages"], i => Url.Action("List", new { page = i, category = ViewData["Category"] }))%>
    <% foreach (var product in Model) %>
    <%{%>
            <%Html.RenderPartial("ProductSummary", product);%>
    <%}%>
    
</asp:Content>
