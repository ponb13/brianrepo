<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DomainModel.Entities.Cart>" %>
<%if (Model.Lines.Count > 0)
  { %>
<%=Model.Lines.Sum(l => l.Quantity) %>
items Total:
<%=Model.ComputeTotalValue() %>
<%=Html.ActionLink("Check out", "Index", "Cart", new {returnUrl = Request.Url.PathAndQuery},null )%>
<%} %>
