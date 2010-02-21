<%@ Page Language="C#" AutoEventWireup="true" CodeFile="error404.aspx.cs" Inherits="error404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
  <div class="post error404">
    <h1>Ooops! I can't find the page you're looking for</h1>
    <div id="divSearchEngine" runat="server" visible="False" class="search">
      <p>
        You did a search on <strong><a href="<%=Server.HtmlEncode(Request.UrlReferrer.ToString()) %>"><%=Server.HtmlEncode(Request.UrlReferrer.Host)%></a></strong>
        for <strong><%=SearchTerm %></strong>. However, their index seems to be out of date.
      </p>
      <h2>All is not lost!</h2>
      <p>I think that the following pages on my site will be able to help you:</p>
      <div id="divSearchResult" runat="server" class="searchresults" />
    </div>
    
    <div id="divExternalReferrer" runat="server" visible="False">
      <p>
        You were incorrectly referred to this page by: 
        <a href="javascript:history.go(-1)"><%=Server.HtmlEncode(Request.UrlReferrer.Host)%></a> 
      </p>
      
      <p>I suggest you try one of the links below:</p>
      <ul>
        <li><a href="archive.aspx"><%=Resources.labels.archive %></a></li>
        <li><a href="<%=SystemX.Web.Utils.RelativeWebRoot %>">Home page</a></li>
      </ul>
      
      <p>You can also try to <strong>search for the page you were looking for</strong>:</p>
      <vs:SearchBox runat="server" />
      
      <p>I'm sorry for the inconvenience</p>
    </div>
    
    <div id="divInternalReferrer" runat="server" visible="False">
      <p>
        This one's down to me! Please accept my apologies for this - I'll see to it
        that the developer responsible for this broken link is given 20 lashes 
        (but only after he or she has fixed this problem).
      </p>
      
      <p>You can also try to <strong>search for the page you were looking for</strong>:</p>
      <vs:SearchBox ID="SearchBox2" runat="server" /><br /><br />
    </div>
    
    <div id="divDirectHit" runat="server" visible="False">
      <p>You might find one of the following links useful:</p>
      <ul>
        <asp:placeholder runat="server" id="phSearchResult" />
        <li><a href="archive.aspx"><%=Resources.labels.archive %></a></li>
        <li><a href="<%=SystemX.Web.Utils.RelativeWebRoot %>">Home page</a></li>
      </ul>
      
      <p>You can also try to <strong>search for the page you were looking for</strong>:</p>
      <vs:SearchBox ID="SearchBox1" runat="server" />
      
      <hr />
      
      <p><strong>You may not be able to find the page you were after because of:</strong></p>
      <ol type="a">
        <li>An <strong>out-of-date bookmark/favorite</strong></li>
        <li>A search engine that has an <strong>out-of-date listing for us</strong></li>
        <li>A <strong>miss-typed address</strong></li>
      </ol>
    </div>
  </div>
</asp:Content>