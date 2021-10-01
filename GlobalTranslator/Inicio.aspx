<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="GlobalTranslator.Inicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <br />
        <div style="text-align: center; background-color:lightyellow;">
            <br />
            <br />
            <asp:Label ID="lblNombreServidor" runat="server" Text="Nombre del servidor: "></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="txtServerName" runat="server" Text="ARIPAU03\SQL2017" Width="200px"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblNombreBase" runat="server" Text="Nombre de la base: "></asp:Label>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:TextBox ID="txtDataBaseName" runat="server" Text="Idiomas" Width="200px"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnSetDataBase" runat="server" Text="Setear Base de datos" OnClick="btnSetDataBase_Click" />
            <br />
            <asp:Label ID="lblSuccess" runat="server" Visible="False"></asp:Label>
            <br />
            <br />
        </div>
        <br />
        <div style="text-align: center; background-color:lightyellow;">
            <br />
            <br />
            <asp:Label ID="lblLngFrom" runat="server" Text="Del lenguaje: "></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="txtLngFrom" runat="server" Text="es" Width="26px"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblLngTo" runat="server" Text="Al lenguaje: "></asp:Label>
             &nbsp;&nbsp;&nbsp;&nbsp;
             <asp:TextBox ID="txtLngTo" runat="server" Text="en" Width="26px"></asp:TextBox>
            <br />
            <br />

            <asp:Button ID="btnTranslate" runat="server" Text="Traducir Texto" OnClick="btnTranslate_Click" />
            <br />
            <asp:Label ID="lblResponse" runat="server" Visible="False"></asp:Label>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
