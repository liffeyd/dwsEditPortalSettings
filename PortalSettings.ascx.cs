/*
' Copyright (c) 2015  dws.ie
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI;
using DotNetNuke.UI.UserControls;


namespace dws.Modules.EditPortalSettings
{
    using System.Collections.Generic;



    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Load Portal Settings for all portals. The settings loaded are specific to the 
    /// Schedule Task to delete and optionally remove unauthorised users. 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : EditPortalSettingsModuleBase        //, IActionable
    {
    private  List<string> TextBoxIdCollection
    {
        get
        {
            var collection = this.ViewState["TextBoxIdCollection"] as List<string>;
            return collection ?? new List<string>();
        }
        set
        {
            this.ViewState["TextBoxIdCollection"] = value;
        }
}



        protected void Page_Load(object sender, EventArgs e)
        {
            this.AddControlsToPage();
        }

        /// <summary>
        /// Add controls to page for each portal in the DNN instance.
        /// THere are 4 settings for each portal used by a scheduled task to delete / purge users:
        ///     Checkbox to denote if users should be "auto" deleted.
        ///     Textbox for the timespan after a user registers before they are auto deleted, id not authorised.
        ///     
        ///     Checkbox to denote if deleted users should be "auto" removed.
        ///     Textbox for the timespan after a user registers before they are auto removed.
        /// 
        ///     DNN flags a user as deleted thus allowing for deleted users to be restored. 
        ///     Once deleted users are purged (removed from database) they cannot be restored.
        /// </summary>
        private void AddControlsToPage()

        {
            try
            {

                const string StartFieldSet = "<fieldset>";
                const string EndFieldSet = "</fieldset>";

                const string StartFormItem = "<div class='dnnFormItem'>";
                const string EndFormItem = "</div>";

                const string SrcLabelControl = "~/../controls/LabelControl.ascx";

                const string DefaultDeleteTimeSpan = "60";
                const string DefaultPurgeTimespan = "7";

                const string SettingAutoDeleteUsers = "AutoDeleteUsers";
                const string SettingAutoDeleteUsersTimeSpan = "AutoDeleteUsersTimeSpan";
                const string SettingAutoPurgeUsers = "AutoPurgeUsers";
                const string SettingAutoPurgeUsersTimeSpan = "AutoPurgeUsersTimeSpan";

                // TODO
                // Check current user. 
                // If host user show all portals
                // If admin show current portal
                // else show a message


                var portals = new List<PortalInfo>();


                var currentUserInfo = base.UserInfo;
                if (currentUserInfo.IsSuperUser)
                {
                    // list portals by portal id
                    portals = PortalController.Instance.GetPortals().Cast<PortalInfo>().OrderBy(o => o.PortalID).ToList();
                }
                else if (currentUserInfo.IsInRole("Administrators"))
                {
                    portals.Add(PortalController.Instance.GetPortal(base.PortalId));
                }
                else
                {
                    this.Controls.Clear();
                    this.Controls.Add(new LiteralControl { Text = this.LocalizeString("UnauthorisedUser") });
                    return;
                }


                


                // loop through portals collection and add controls, in a fieldset, for each portal.
                foreach (var portal in portals)
                {
                    // Get portal settings used by scheduled task to delete / purge users.
                    var autoDeleteUsers = PortalController.GetPortalSettingAsBoolean(SettingAutoDeleteUsers, portal.PortalID, false);
                    var deleteTimeSpan = PortalController.GetPortalSetting(
                        SettingAutoDeleteUsersTimeSpan,
                        portal.PortalID,
                        DefaultDeleteTimeSpan);

                    var autoRemoveUsers = PortalController.GetPortalSettingAsBoolean(SettingAutoPurgeUsers, portal.PortalID, false);
                    var removeDeletedUsersTimeSpan = PortalController.GetPortalSetting(
                        SettingAutoPurgeUsersTimeSpan,
                        portal.PortalID,
                        DefaultPurgeTimespan);


                    // dnn Panels are used to allow Expand / Collapse of settings for each portal.
                    // http://www.dnnsoftware.com/wiki/page/dnnpanels-jquery-plugin
                    var sectionHead = new LiteralControl
                    {
                        Text =
                            string.Format(
                                "<h2 class='dnnFormSectionHead' ID='pnlHeader_{2}'><a href='' class='dnnLabelCollapsed'>{0} ({2}): {1}</a></h2>",
                                this.LocalizeString("lblPortalTitle"),
                                portal.PortalName,
                                portal.PortalID)
                    };
                    this.plHolder.Controls.Add(sectionHead);


                    //******************************************************************************************
                    // Start fieldset
                    this.plHolder.Controls.Add(new LiteralControl { Text = StartFieldSet });

                    // Auto Delete Checkbox & label
                    // Start form item
                    this.plHolder.Controls.Add(new LiteralControl { Text = StartFormItem });

                    using (var lblAutoDelete = ControlUtilities.LoadControl<LabelControl>(this, SrcLabelControl))
                    {
                        lblAutoDelete.ID = String.Format("lblAutoDelete{0}", portal.PortalID);
                        lblAutoDelete.ResourceKey = "lblAutoDeleteUnauthorisedUsers";

                        this.plHolder.Controls.Add(lblAutoDelete);
                    }

                    var chkAutoDelete = new CheckBox
                                            {
                                                ID = String.Format("chkAutoDeleteUsers{0}", portal.PortalID),
                                                Checked = autoDeleteUsers
                                            };
                    this.plHolder.Controls.Add(chkAutoDelete);

                    this.plHolder.Controls.Add(new LiteralControl { Text = EndFormItem });




                    // Auto delete time span textbox & label
                    this.plHolder.Controls.Add(new LiteralControl { Text = StartFormItem });

                    using (var lblAutoDeleteTimeSpan = ControlUtilities.LoadControl<LabelControl>(this, SrcLabelControl))
                    {
                        lblAutoDeleteTimeSpan.ResourceKey = "lblAutoDeleteTimeSpan";
                        lblAutoDeleteTimeSpan.ID = String.Format("lblAutoDeleteTimeSpan{0}", portal.PortalID);
                        lblAutoDeleteTimeSpan.CssClass = "dnnFormRequired";

                        this.plHolder.Controls.Add(lblAutoDeleteTimeSpan);
                    }

                    var txtAutoDeleteTimeSpan = new TextBox
                    {
                        ID = String.Format("txtAutoDeleteTimeSpan{0}", portal.PortalID),
                        MaxLength = 4,
                        Width = 40,
                        Text = deleteTimeSpan
                    };
                    this.plHolder.Controls.Add(txtAutoDeleteTimeSpan);


                    //ToDo
                    // figure out why valdation doesn't work. It seems the event doesn't fire because hte Text field has a value.
                    var rfvAutoDeleteTimeSpan = new RequiredFieldValidator
                    {
                        ID = String.Format("rfvAutoDeleteTimeSpan{0}",portal.PortalID),
                        ErrorMessage = this.LocalizeString("RequiredTimeSpan"),
                        ControlToValidate = String.Format("txtAutoDeleteTimeSpan{0}", portal.PortalID),
                        CssClass = "dnnFormMessage dnnFormError"
                        //Operator = ValidationCompareOperator.DataTypeCheck, 
                        //Type = ValidationDataType.Integer

                    };
                    this.plHolder.Controls.Add(rfvAutoDeleteTimeSpan);

                    //var rngAutoDeleteTimeSpan = new RangeValidator
                    //{
                    //    ID = String.Format("rngAutoDeleteTimeSpan{0}", portal.PortalID),
                    //    CssClass = "dnnFormMessage dnnFormError",
                    //    ErrorMessage = this.LocalizeString("RangeTimeSpan"),
                    //    ControlToValidate = String.Format("txtAutoDeleteTimeSpan{0}", portal.PortalID),
                    //    Type = ValidationDataType.Integer, MinimumValue = "1", MaximumValue = "99"
                    //};

                    //this.plHolder.Controls.Add(rngAutoDeleteTimeSpan);


                    this.plHolder.Controls.Add(new LiteralControl { Text = EndFormItem });


                    // Start form item
                    this.plHolder.Controls.Add(new LiteralControl { Text = StartFormItem });

                    using (var lblAutoPurgeUsers = ControlUtilities.LoadControl<LabelControl>(this, SrcLabelControl))
                    {
                        lblAutoPurgeUsers.ResourceKey = "lblAutoPurgeUsers";
                        lblAutoPurgeUsers.ID = String.Format("lblAutoPurgeUsers{0}", portal.PortalID);

                        this.plHolder.Controls.Add(lblAutoPurgeUsers);
                    }

                    var chkAutoPurgeUsers = new CheckBox
                                                {
                                                    ID = String.Format("chkAutoPurgeUsers{0}", portal.PortalID),
                                                    Checked = autoRemoveUsers
                                                };
                    this.plHolder.Controls.Add(chkAutoPurgeUsers);

                    this.plHolder.Controls.Add(new LiteralControl { Text = EndFormItem });



                    this.plHolder.Controls.Add(new LiteralControl { Text = StartFormItem });

                    using (var lblAutoPurgeTimeSpan = ControlUtilities.LoadControl<LabelControl>(this, SrcLabelControl))
                    {
                        lblAutoPurgeTimeSpan.ID = String.Format("lblAutoPurgeTimeSpan{0}", portal.PortalID);
                        lblAutoPurgeTimeSpan.ResourceKey = "lblAutoPurgeTimeSpan";
                        lblAutoPurgeTimeSpan.CssClass = "dnnFormRequired";

                        this.plHolder.Controls.Add(lblAutoPurgeTimeSpan);
                    }

                    var txtAutoPurgeTimeSpan = new TextBox
                                                   {
                                                       ID = String.Format("txtAutoPurgeTimeSpan{0}", portal.PortalID),
                                                       MaxLength = 4,
                                                       Width = 40,
                                                       Text = removeDeletedUsersTimeSpan,
                                                       CssClass = "dnnFormRequired"
                                                   };
                    this.plHolder.Controls.Add(txtAutoPurgeTimeSpan);

                    //ToDo
                    // See above ...

                    //var requiredAutoPurgeTimeSpan = new RequiredFieldValidator
                    //{
                    //    ID = String.Format("requiredAutoPurgeTimeSpan{0}",portal.PortalID),
                    //    CssClass = "dnnFormMessage dnnFormError",
                    //    ErrorMessage = this.LocalizeString("RequiredTimeSpan"),
                    //    ControlToValidate = String.Format("txtAutoPurgeTimeSpan{0}",portal.PortalID)
                    //};
                    //this.plHolder.Controls.Add(requiredAutoPurgeTimeSpan);


                    //var compareAutoPurgeTimeSpan = new CompareValidator
                    //{
                    //    ID = String.Format("rfvAutoPurgeTimeSpan{0}", portal.PortalID),
                    //    CssClass = "dnnFormMessage dnnFormError",
                    //    ErrorMessage = this.LocalizeString("RequiredTimeSpan"),
                    //    ControlToValidate = String.Format("txtAutoPurgeTimeSpan{0}", portal.PortalID),
                    //    Operator = ValidationCompareOperator.DataTypeCheck,
                    //    Type = ValidationDataType.Integer
                    //};
                    //this.plHolder.Controls.Add(compareAutoPurgeTimeSpan);




                    this.plHolder.Controls.Add(new LiteralControl { Text = EndFormItem });

                    // End fieldset
                    this.plHolder.Controls.Add(new LiteralControl { Text = EndFieldSet });
                }

                this.plHelp.Controls.Add(new LiteralControl
                {
                    Text = String.Format("<a href='/Desktopmodules/dwsEditPortalSettings/documentation/documentation.html' class='popup2'>{0}</a>", this.LocalizeString("Help"))
                });
               
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSubmit control to update PortalSettings.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void BtnSubmitClick(object sender, EventArgs e)
        {
            try
            {
                foreach (Control ctr in this.plHolder.Controls)
                {           
                    string strPortalId;
                    int portalId;
                    string id;

                    var box = ctr as TextBox;
                    if (box != null)
                    {
                        var txtValue = box.Text;
                        id = box.ID;
                    
                        const string AutoDeleteTimeSpan = "txtAutoDeleteTimeSpan";
                        if (id.Contains(AutoDeleteTimeSpan))
                        {
                            if (id.Contains(AutoDeleteTimeSpan))
                            {
                                strPortalId = id.Substring(AutoDeleteTimeSpan.Length);

                                if (int.TryParse(strPortalId, out portalId))
                                {
                                    PortalController.UpdatePortalSetting(portalId, "AutoDeleteUsersTimeSpan", txtValue);
                                }
                            }
                        }

                        const string AutoPurgeTimeSpan = "txtAutoPurgeTimeSpan";
                        if (id.Contains(AutoPurgeTimeSpan))
                        {
                            if (id.Contains(AutoPurgeTimeSpan))
                            {
                                strPortalId = id.Substring(AutoPurgeTimeSpan.Length);
                            
                                if (int.TryParse(strPortalId, out portalId))
                                {
                                    PortalController.UpdatePortalSetting(portalId, "AutoPurgeUsersTimeSpan", txtValue);
                                }
                            }
                        }

                    }

                    var chk = ctr as CheckBox;
                    if (chk != null)
                    {
                        var chkValue = chk.Checked;
                        id = chk.ID;

                        const string AutoDelete = "chkAutoDeleteUsers";
                        if (id.Contains(AutoDelete))
                        {
                            strPortalId = id.Substring(AutoDelete.Length);
                        
                            if (int.TryParse(strPortalId, out portalId))
                            {
                                PortalController.UpdatePortalSetting(portalId, "AutoDeleteUsers", chkValue ? "true" : "false");
                            }
                        }

                        const string AutoPurgeUsers = "chkAutoPurgeUsers";
                        if (id.Contains(AutoPurgeUsers))
                        {
                            strPortalId = id.Substring(AutoPurgeUsers.Length);
                            if (int.TryParse(strPortalId, out portalId))
                            {
                                PortalController.UpdatePortalSetting(portalId, "AutoPurgeUsers", chkValue ? "true" : "false");
                            }
                        }
                    }
                }

                this.plResult.Controls.Add(new LiteralControl
                {
                    Text = String.Format(this.LocalizeString("SettingsUpdatedAt"), DateTime.Now.TimeOfDay.ToString())
                });


                this.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());

            }
            catch (Exception ex)
            {
                this.plResult.Controls.Add(new LiteralControl
                {
                    Text = ex.Message
                });
                throw;
            }
        }
    }
}