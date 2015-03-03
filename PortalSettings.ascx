﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalSettings.ascx.cs" Inherits="dws.Modules.EditPortalSettings.View" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>


<div id="dnnPortalSettings">
	<div class="dnnFormExpandContent"><a href="#">ExpandAll</a></div>
	<div id="dnnEditPortalSettings">
		<div class="dnnForm dnnSiteSettings dnnClear" >
			<div class="ssbsContent dnnClear" >
				<asp:PlaceHolder runat="server" ID="plHolder" ></asp:PlaceHolder>
			</div>
		</div>
        <asp:PlaceHolder runat="server" ID="plResult" ></asp:PlaceHolder>
		<ul class="dnnActions dnnClear">
			<li>
				<asp:LinkButton id="cmdUpdate" OnClick="BtnSubmitClick" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" />
			</li>
			<li>
				<asp:PlaceHolder runat="server" ID="plHelp" ></asp:PlaceHolder>
			</li>
		</ul>
	</div>
</div>





<script>
$(document).ready(function () {
	$('#dnnEditPortalSettings').dnnPanels();
});

$('#dnnPortalSettings .dnnFormExpandContent a').dnnExpandAll({
	expandText: 'Expand All',
	collapseText: 'Collapse All',
	targetArea: '#dnnPortalSettings'
});
</script>


<script type="text/javascript">
	//initialize the 3 popup css class names - create more if needed
	var matchClass = ['popup1', 'popup2', 'popup3'];
	//Set your 3 basic sizes and other options for the class names above - create more if needed
	var popup1 = 'width=400,height=300,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=20,top=20';
	var popup2 = 'width=800,height=600,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=20,top=20';
	var popup3 = 'width=1000,height=750,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=20,top=20';

	//The pop-up function
	function tfpop() {
		var x = 0;
		var popClass;
		//Cycle through the class names
		while (x < matchClass.length) {
			popClass = "'." + matchClass[x] + "'";
			//Attach the clicks to the popup classes
			$(eval(popClass)).click(function () {
				//Get the destination URL and the class popup specs
				var popurl = $(this).attr('href');
				var popupSpecs = $(this).attr('class');
				//Create a "unique" name for the window using a random number
				var popupName = Math.floor(Math.random() * 10000001);
				//Opens the pop-up window according to the specified specs
				newwindow = window.open(popurl, popupName, eval(popupSpecs));
				return false;
			});
			x++;
		}
	}

	//Wait until the page loads to call the function
	$(function () {
		tfpop();
	});
</script>
