<%@ Page Title="PSP Program" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1>Pacific Northwest Permanent Sample Plot Program - Data Explorer</h1>
            </hgroup>
            <p>
                <b>Learn more about our research at the <a href="http://pnwpsp.forestry.oregonstate.edu/" style="color: #0026ff" title="Permanent Sample Plot Program" target="_blank">Permanent Sample Plot Program</a> website. 
                </b>
            </p>
        </div>
    </section>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <script src="Scripts/PSPDataViewer_scripts.js" type="text/javascript" language="JavaScript"></script>
    <ol class="round">
        <li class="one">
            <h2 align="center">Select stands to view and export&nbsp;&nbsp;
            <a href="#" onclick="helpAlert('selectHelp')">
                <asp:Image ID="img_help1" runat="server" ImageUrl="Images/help-about-icon.png" AlternateText="help" /></a>
            </h2>
            <table cellspacing="10px">
                <tr>
                    <td>
                        <asp:Label ID="lbl_selectIndividual" Text="Select stands individually:" runat="server" Font-Size="Large" Font-Bold="True"></asp:Label><br />

                        <style type="text/css">
                            label {
                                display: inline-block;
                            }
                        </style>
                        <div style="overflow: auto; width: 500px; height: 200px; background: white; border: 1px solid grey">
                            <asp:CheckBoxList ID="cbList_stands" runat="server" CellPadding="0" CellSpacing="0" RepeatLayout="Flow" CssClass="checkboxlist_nowrap">
                            </asp:CheckBoxList>
                        </div>
                    </td>
                    <td valign="top">
                        <asp:Label ID="lbl_select_by" Text="Choose stands by:" runat="server" Font-Size="Large" Font-Bold="True"></asp:Label><br />
                        <asp:DropDownList ID="ddl_select_by" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_select_by_change" Height="30px" Width="300px">
                            <asp:ListItem Value="default">--</asp:ListItem>
                            <asp:ListItem Value="Geo_loc">Geographic location</asp:ListItem>
                            <asp:ListItem Value="admin_unit">Administration unit</asp:ListItem>
                            <asp:ListItem Value="study_focus">Study focus</asp:ListItem>
                            <asp:ListItem Value="all_stands">All stands</asp:ListItem>
                        </asp:DropDownList><br />
                        <br />
                        <asp:DropDownList ID="ddl_select_class" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_select_class_SelectedIndexChanged" Height="30px" Width="300px">
                        </asp:DropDownList>

                        <br />
                        <br />
                        <asp:Button ID="btn_selectAll" runat="server" Text="Select All" CssClass="formButton" OnClick="btn_selectAll_Click" />&nbsp;
                        <asp:Button ID="btn_selectNone" runat="server" Text="Clear Selection" CssClass="formButton" OnClick="btn_selectNone_Click" />&nbsp;
                        <asp:Button ID="btn_downloadStandID" runat="server" Text="Download Stand Descriptors" CssClass="formButton" OnClick="btn_downloadStandID_Click" />
                        <a href="http://andlter.forestry.oregonstate.edu/data/attributes.aspx?dbcode=TV010&entnum=6" target="_blank">
                            <asp:Image ID="img_standID_metadata" runat="server" ImageUrl="Images/help-about-icon.png" AlternateText="help" /></a>

                    </td>
                </tr>
                <tr>
                    <p></p>
                </tr>
            </table>
        </li>
        <li class="two">
            <h2 align="center">Select data summary&nbsp;&nbsp;
            <a href="#" onclick="helpAlert('showDataHelp')">
                <asp:Image ID="img_help2" runat="server" ImageUrl="Images/help-about-icon.png" AlternateText="help" /></a>
            </h2>
            <table cellspacing="10px">
                <tr>
                    <td>
                        <asp:Label ID="lbl_fileContent" Text="File content preview (first 10 lines)" runat="server" Font-Size="Large"></asp:Label><br />
                        <asp:TextBox ID="tb_fileDump" Text="" Font-Names="Courier New" runat="server" Width="600px" Height="200px" Wrap="False" ReadOnly="True" TextMode="MultiLine" Font-Size="Small"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lbl_summaryMethod" Text="Data summary type and level" runat="server" Font-Size="Large" Font-Bold="True"></asp:Label>
                        <asp:RadioButtonList ID="radio_summaryMethod" runat="server" Width="270px" CssClass="rButtonList" RepeatLayout="Table" OnSelectedIndexChanged="radio_summaryMethod_SelectedIndexChanged" AutoPostBack="True">
                            <asp:ListItem Value="individualTrees">Individual trees (state)</asp:ListItem>
                            <asp:ListItem Value="plotState">Plot level (state)</asp:ListItem>
                            <asp:ListItem Value="plotChange">Plot level (change)</asp:ListItem>
                            <asp:ListItem Value="standState">Stand level (state)</asp:ListItem>
                            <asp:ListItem Value="standChange">Stand level (change)</asp:ListItem>
                        </asp:RadioButtonList>
                        <p>
                            <asp:Button ID="btn_download" Text="Download data (acknowledge as below)" runat="server" CssClass="formButton" OnClick="btn_download_Click" />
                        </p>
                    </td>
                </tr>
            </table>
        </li>
        <li class="three">
            <h2 align="center">Graph Stand-Level Data&nbsp;&nbsp;
                <a href="#" onclick="helpAlert('graphDataHelp')">
                <asp:Image ID="img_help3" runat="server" ImageUrl="Images/help-about-icon.png" AlternateText="help" /></a>
            </h2>
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btn_chartTPH" Text="Total Live TPH" runat="server" CssClass="formButton" OnClick="btn_chartTPH_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_chartBA" Text="Total Live Basal Area" runat="server" CssClass="formButton" OnClick="btn_chartBA_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_chartLiveBio" Text="Total Live Biomass" runat="server" CssClass="formButton" OnClick="btn_chartLiveBio_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_chartDeadBio" Text="Total Snag Biomass" runat="server" CssClass="formButton" OnClick="btn_chartDeadBio_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_NPP" Text="NPP" runat="server" CssClass="formButton" OnClick="btn_chartNPP_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btn_bySPP_TPH" Text="TPH by species" runat="server" CssClass="formButton" OnClick="btn_bySPP_TPH_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_bySPP_BA" Text="Basal Area by species" runat="server" CssClass="formButton" OnClick="btn_bySPP_BA_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_bySPP_LiveBio" Text="Live Biomass by species" runat="server" CssClass="formButton" OnClick="btn_bySPP_LiveBio_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btn_bySPP_DeadBio" Text="Snag Biomass by species" runat="server" CssClass="formButton" OnClick="btn_bySPP_DeadBio_Click" />
                    </td>
<%--                    <td>
                        <asp:Button ID="btn_bySPP_DBH" Text="DBH by species" runat="server" CssClass="formButton" OnClick="btn_bySPP_DBH_Click" />
                    </td>--%>
                </tr>
            </table>
            <div id="outChart">
               
                <asp:Chart ID="Chart1" runat="server" Width="860px" Height="510px">
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                            <AxisX Title="Year of Observation"></AxisX>
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
            <div style="text-align: center">To export this image right click and choose 'copy image' or 'save image as'. See notes for this step.</div>
        </li>
    </ol>
    <p>
        <b>Please use the following acknowledgment if you use the data in any presentation, report or manuscript: “Data were provided by the Pacific Northwest Permanent Sample Plot Program and the HJ Andrews Experimental Forest research program, funded in part by the National Science Foundation's Long-Term Ecological Research Program (DEB 08-23380), the US Forest Service Pacific Northwest Research Station, and Oregon State University.</b>
    </p>
</asp:Content>
