#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using System.Collections.Generic;
using System.Linq;
using FTOptix.WebUI;
#endregion

public class UserEditorGroupsPanelLogic : BaseNetLogic
{
    public override void Start()
    {
        userVariable = Owner.GetVariable("User");
        editable = Owner.GetVariable("Editable");

        userVariable.VariableChange += UserVariable_VariableChange;
        editable.VariableChange += Editable_VariableChange;

        UpdateGroupsAndUser();

        BuildUIGroups();
        if (editable.Value)
            SetCheckedValues();
    }

    [ExportMethod]
    public void ApplyGroups(NodeId user)
    {
        if (editable.Value == false)
            return;

        if (user == null)
            return;

        if (groups == null)
            return;

        if (panel == null)
            return;

        var userNode = InformationModel.Get(user);
        if (userNode == null)
            return;

        var groupCheckBoxes = panel.Refs.GetObjects(OpcUa.ReferenceTypes.HasOrderedComponent, false);

        foreach (var groupCheckBoxNode in groupCheckBoxes)
        {
            var group = groups.Get(groupCheckBoxNode.BrowseName);
            if (group == null)
                return;

            bool userHasGroup = UserHasGroup(group.NodeId);

            if (groupCheckBoxNode.GetVariable("Checked").Value && !userHasGroup)
                userNode.Refs.AddReference(FTOptix.Core.ReferenceTypes.HasGroup, group);
            else if (!groupCheckBoxNode.GetVariable("Checked").Value && userHasGroup)
                userNode.Refs.RemoveReference(FTOptix.Core.ReferenceTypes.HasGroup, group.NodeId, false);
        }
    }

    private void Editable_VariableChange(object sender, VariableChangeEventArgs e)
    {
        UpdateGroupsAndUser();
        BuildUIGroups();

        if (e.NewValue)
            SetCheckedValues();
    }

    private void UserVariable_VariableChange(object sender, VariableChangeEventArgs e)
    {
        UpdateGroupsAndUser();
        if (editable.Value)
            SetCheckedValues();
        else
            BuildUIGroups();
    }

    private void UpdateGroupsAndUser()
    {
        if (userVariable.Value.Value != null)
            user = InformationModel.Get(userVariable.Value);

        groups = LogicObject.GetAlias("Groups");
    }

    private void BuildUIGroups()
    {
        if (groups == null)
            return;

        if (panel != null)
            panel.Delete();

        panel = InformationModel.MakeObject<ColumnLayout>("Container");
        panel.HorizontalAlignment = HorizontalAlignment.Stretch;

        foreach (var group in groups.Children)
        {
            if (editable.Value)
            {
                var groupCheckBox = InformationModel.MakeObject<Panel>(group.BrowseName, Optix_Training_Users_group_display2.ObjectTypes.GroupCheckbox);

                groupCheckBox.GetVariable("Group").Value = group.NodeId;
                groupCheckBox.GetVariable("User").SetDynamicLink(userVariable);
                groupCheckBox.HorizontalAlignment = HorizontalAlignment.Stretch;


                panel.Add(groupCheckBox);
                panel.Height += groupCheckBox.Height;
            }
            else if (UserHasGroup(group.NodeId))
            {
                var groupLabel = InformationModel.MakeObject<Panel>(group.BrowseName, Optix_Training_Users_group_display2.ObjectTypes.GroupLabel);
                groupLabel.GetVariable("Group").Value = group.NodeId;
                groupLabel.HorizontalAlignment = HorizontalAlignment.Stretch;

                panel.Add(groupLabel);
                panel.Height += groupLabel.Height;
            }

        }

        var scrollView = Owner.Get("ScrollView");
        if (scrollView != null)
            scrollView.Add(panel);
    }

    private void SetCheckedValues()
    {
        if (groups == null)
            return;

        if (panel == null)
            return;

        var groupCheckBoxes = panel.Refs.GetObjects(OpcUa.ReferenceTypes.HasOrderedComponent, false);

        foreach (var groupCheckBoxNode in groupCheckBoxes)
        {
            var group = groups.Get(groupCheckBoxNode.BrowseName);
            groupCheckBoxNode.GetVariable("Checked").Value = UserHasGroup(group.NodeId);
        }
    }

    private bool UserHasGroup(NodeId groupNodeId)
    {
        if (user == null)
            return false;
        var userGroups = user.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false);
        foreach (var userGroup in userGroups)
        {
            if (userGroup.NodeId == groupNodeId)
                return true;
        }
        return false;
    }

    private IUAVariable userVariable;
    private IUAVariable editable;

    private IUANode groups;
    private IUANode user;
    private ColumnLayout panel;
}
