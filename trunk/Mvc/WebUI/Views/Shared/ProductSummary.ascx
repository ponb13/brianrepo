<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DomainModel.Entities.Product>" %>
<div class="item">
    <h3>
        <%=Model.Name%></h3>
    <%=Model.Description%>
    <h4>
        <%=Model.Price.ToString("c")%></h4>
        
        
        <%using (Html.BeginForm("AddToCart", "Cart"))
          { %>
        <%=Html.Hidden("ProductId")%>
        <%=Html.Hidden("ReturnUrl", ViewContext.HttpContext.Request.Url.PathAndQuery)%>
        <input type="submit" value="+ add to cart" />
        <%} %>
</item>