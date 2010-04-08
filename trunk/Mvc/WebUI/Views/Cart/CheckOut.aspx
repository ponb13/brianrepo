<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteMaster.Master"
    Inherits="System.Web.Mvc.ViewPage<DomainModel.Entities.Cart>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CheckOut
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        CheckOut</h2>
    <%using (Html.BeginForm())%>
    <%{ %>
    <div>
        Name:
        <%=Html.TextBox("Name")%></div>
    <div>
        Line1:
        <%=Html.TextBox("Line1")%></div>
    <div>
        Line2:
        <%=Html.TextBox("Line2")%></div>
    <div>
        Line3:
        <%=Html.TextBox("Line3")%></div>
    <div>
        City:
        <%=Html.TextBox("City")%></div>
    <div>
        State:
        <%=Html.TextBox("State")%></div>
    <div>
        Zip:
        <%=Html.TextBox("Zip")%></div>
    <div>
        Country:
        <%=Html.TextBox("Country")%></div>
    <div>
        Gift Wrap;
        <%=Html.CheckBox("GiftWrap") %></div>
    <input type="submit" value="CompleteOrder" />
    <%} %>
</asp:Content>
