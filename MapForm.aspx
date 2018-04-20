<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="MapForm.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="MapInfo.WebControls, Version=6.7.1.503, Culture=neutral, PublicKeyToken=0a9556cc66c0af57"
    Namespace="MapInfo.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="charset=UTF-8" />
    <title>Untitled Page</title>
<style type="text/css">
.mapCtrlStyle {
	position:absolute; 
	width:20px;
	height:18px; 
	top:0px;  
	left:0px;  
	border-color:#8DC2EE; 
	border-style:solid; 
	border-width:0px; 
	//cursor:pointer;
	font-size:1px;
	color:#ffffff;
}
.mapNormalStyle{
	position:absolute;
	z-index:997;
	background-color:#6180A3;
	-moz-opacity:0.9;
	filter:Alpha(Opacity=90);
	top:0px; 
	left:0px;
}

</style>    
</head>
<body style="overflow:hidden; margin-left:0; margin-right:0; margin-top:0; margin-bottom:0;" onresize="resize()">
   
    <form id="form1" runat="server">
    <div id="main" style="position:absolute; z-index:10; left:0px; top:0px; width:100%; height:100%; overflow:hidden;" >
		<cc1:MapControl ID="MapControl1" runat="server" MapAlias="Map1" BackColor="Transparent" BorderWidth="0px" BorderColor="#FF8000" Height="100%" Width="100%"/>
    </div>
    
<div style="position:absolute;z-index:11;" onmouseover="onMOverAlpha();"  onmouseout="onMOutAlpha();"  >    
      
        <div id="ctrlZoom" class="mapNormalStyle" style="left:0px;font-size:18px; width:30px;height:22px; cursor:pointer;  text-align:center; vertical-align: middle; font-weight: bold; color: white;" 
        onclick="ctrlZoom( );"  onmouseover="this.style.borderWidth = '1px';" onmouseout="this.style.borderWidth = '0px';" >
    	    &lt;&lt;
    	</div> 
		<div id="ctrlTop" class="mapNormalStyle" style="left:30px;width:280px; height:22px;  background-color:#6180A3;" >
		    
    	    <div class="mapCtrlStyle" style="left:5px;" >
	 		  <asp:ImageButton runat="server" id="btnzoomall" OnClick="btnZoomAll" ImageUrl="image/zoomall.gif" AlternateText="zoom all" />
			</div>
	 		<div class="mapCtrlStyle" style="left:30px;" >
	 			<cc1:PanTool        ID="PanTool1" runat="server" MapControlID="MapControl1" ActiveImageUrl="image/pan1.gif" InactiveImageUrl="image/pan.gif"/>
			</div>
			<div class="mapCtrlStyle" style="left:55px;" >
				<cc1:ZoomInTool     ID="ZoomInTool1" runat="server" MapControlID="MapControl1" ActiveImageUrl="image/zoomin1.gif" InactiveImageUrl="image/zoomin.gif"/>
			</div>
			<div class="mapCtrlStyle" style="left:80px;" >
				<cc1:ZoomOutTool    ID="ZoomOutTool1" runat="server" MapControlID="MapControl1" ActiveImageUrl="image/zoomout1.gif" InactiveImageUrl="image/zoomout.gif"/>
			</div>
	 		<div class="mapCtrlStyle" style="left:105px;" >
	 		   <asp:dropdownlist id="DropDownList1" width="120" height="22" runat="server" OnInit="InitMapPath"></asp:dropdownlist>
			</div> 	   
	 		<div class="mapCtrlStyle" style="left:225px;" >
		     <asp:Button runat="server" ID="btnChange" OnClick="ChangeMap" OnClientClick="clientclick" Text="change" />
		  </div>
		</div>		
	
</div>
   
    <div id="overviewMain" style="position:absolute; z-index:11; left:183px; width:208px; height:138px; border:1px solid #000000; background-color:#6180A3; top: 224px;">
	    <div id="overviewWHOLEAREA" style="position:relative; top:4px; left:4px;">   
  		    <cc1:MapControl ID="MapControl2" runat="server" MapAlias="Map2" ExportFormat="Jpeg" Height="130px" Width="200px" BorderColor="#6180A3" />
        </div> 
    </div>
    
    <div id="overviewBtn" onmouseout="this.style.backgroundColor='#bbbbbb';" onmouseover="this.style.backgroundColor='#6180A3';" onclick=" zoomOverView( ) "
     style="position:absolute; z-index:12; font-size:14px; cursor:pointer; width:15px; height:15px; top:343px; left:377px; border:2px solid #aaaaaa; background-color:#dddddd; vertical-align: middle; text-align: center;">
	-
	</div>	
	
    <script language="javascript" type="text/javascript" src="mapjs/MouseWheel.js"></script>   
    <script language="javascript" type="text/javascript" src="mapjs/OverviewMap.js"></script>   
    
    </form> 
    <script language="javascript" type="text/javascript" src="mapjs/func.js"></script>   
    <script language="javascript" type="text/javascript" src="mapjs/global.js"></script>   
    
</body>
</html>
