﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<DomainModel.Entities.Order>>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>List</title>
</head>
<body>
    <div>
          
        <%foreach (var order in Model)%>
            <%{ %>
            <%Html.RenderPartial("OrderSummary", order);%>
                
            <% }%> 
        
    </div>
</body>
</html>
