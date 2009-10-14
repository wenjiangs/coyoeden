<%@ Page Language="C#" MasterPageFile="~/themes/admin/site.master" AutoEventWireup="true" ValidateRequest="False" CodeFile="Categories.aspx.cs" Inherits="admin_Pages_Categories" %>
<asp:Content ID="Content0" ContentPlaceHolderID="cphHead" Runat="Server"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">

  <asp:Label ID="lblNewCategory" runat ="server" AssociatedControlID="txtNewCategory" Text="Title" /><br />
  <asp:TextBox runat="Server" ID="txtNewCategory" Width="200" /><br />
  <asp:Label ID="lblNewNewDescription" runat ="server" AssociatedControlID="txtNewNewDescription" Text="Description" /><br />
  <asp:TextBox runat="Server" ID="txtNewNewDescription" Width="400" TextMode="MultiLine" Rows="4" /><br />
  <asp:Label ID="lblNewParent" runat ="server" AssociatedControlID="ddlNewParent" Text="Parent" /><br />
  <asp:DropDownList ID="ddlNewParent" width="200" runat="server" /><br /><br />
  <asp:Button runat="server" ID="btnAdd" ValidationGroup="new" />
  <asp:CustomValidator runat="Server" ID="valExist" ValidationGroup="new" ControlToValidate="txtNewCategory" ErrorMessage="The category already exist" Display="dynamic" />
  <asp:RequiredFieldValidator runat="Server" ValidationGroup="new" ControlToValidate="txtNewCategory" ErrorMessage="Please enter a valid name" /><br /><hr />


  <asp:GridView runat="server" ID="grid" CssClass="category" 
    GridLines="None"
    AutoGenerateColumns="False" 
    AlternatingRowStyle-CssClass="alt" 
    AutoGenerateDeleteButton="True" 
    AutoGenerateEditButton="True">
    <Columns>      
      <asp:TemplateField HeaderText="<%$ Resources:labels, name %>">
        <ItemTemplate>
          <%# Server.HtmlEncode(Eval("title").ToString()) %>
        </ItemTemplate>
        <EditItemTemplate>
          <asp:TextBox runat="server" ID="txtTitle" Text='<%# Eval("title") %>' />
        </EditItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="<%$ Resources:labels, description %>">
       <ItemTemplate>
          <%# Server.HtmlEncode(Eval("description").ToString())%>
        </ItemTemplate>
        <EditItemTemplate>
          <asp:TextBox runat="server" MaxLength="255" ID="txtDescription" Text='<%# Eval("description") %>'  />
        </EditItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Parent">
        <ItemTemplate>
          <%# GetParentTitle(Container.DataItem) %>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:DropDownList ID="ddlParent" runat="server" />
        </EditItemTemplate>
      </asp:TemplateField>
    </Columns>
      <AlternatingRowStyle CssClass="alt" />
  </asp:GridView>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphFooter" Runat="Server"></asp:Content>