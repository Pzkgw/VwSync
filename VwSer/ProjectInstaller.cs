﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace VwSer
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        

        public ProjectInstaller()
        {
            InitializeComponent();
            //this.vwSerProcessInstaller.Username = "GI\\bogdan.visoiu";
            //this.vwSerProcessInstaller.Password = "Parola32167";
        }


        //public void Install()
        //{
        //    foreach(Installer i in Installers)
        //    {
        //        try
        //        {
        //            IDictionary stateSaver = new Hashtable();
        //            i.Install(stateSaver);
        //            i.Commit(stateSaver);

        //        }
        //        catch { }
        //    }
        //}


        //public override void Install(System.Collections.IDictionary stateSaver)
        //{
        //    base.Install(stateSaver);
        //    RegistrationServices regSrv = new RegistrationServices();
        //    regSrv.RegisterAssembly(base.GetType().Assembly,
        //      AssemblyRegistrationFlags.SetCodeBase);
        //}

        //public override void Uninstall(System.Collections.IDictionary savedState)
        //{
        //    base.Uninstall(savedState);
        //    RegistrationServices regSrv = new RegistrationServices();
        //    regSrv.UnregisterAssembly(base.GetType().Assembly);
        //}



    }
}
