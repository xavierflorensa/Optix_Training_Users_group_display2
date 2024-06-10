#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.Core;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    public override void Start()
    {
        // Mostrar el usuari logat
        var usuario = Session.User;
        var user = Session.User.BrowseName;
        var mylabel = Project.Current.Get<Label>("UI/MainWindow/Label1");
        mylabel.Text = user.ToString();
        //Mostra el grup del usuari
        var userGroups = usuario.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false);
        foreach (var group in userGroups)
        {
            Log.Info(group.BrowseName);
            Project.Current.GetVariable("Model/Locales/UserGroups").Value = group.BrowseName;

        }

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
    [ExportMethod]
    
    public void Update()
    {
       Actualitza();
       Log.Info("Updating: ");
    }
    
     private void Actualitza()
    {
       // Mostrar el usuari logat
       var usuario = Session.User;
       var user = Session.User.BrowseName;
       var mylabel = Project.Current.Get<Label>("UI/MainWindow/Label1");
       mylabel.Text = user.ToString();
       //Mostra el grup del usuari
       var userGroups = usuario.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false);
       foreach (var group in userGroups)
       {
            Log.Info(group.BrowseName);
            var mylabel_group = Project.Current.Get<Label>("UI/MainWindow/Label2");
            mylabel_group.Text = group.BrowseName.ToString();
            Project.Current.GetVariable("Model/Locales/UserGroups").Value = group.BrowseName;
       }
    }

}
