namespace VwSer
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vwSerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.vsSerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // vwSerProcessInstaller
            // 
            this.vwSerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            // 
            // vsSerInstaller
            // 
            this.vsSerInstaller.Description = "File syncronization service";
            this.vsSerInstaller.DisplayName = "CAVISync";
            this.vsSerInstaller.ServiceName = "CAVISync";
            this.vsSerInstaller.StartType = System.ServiceProcess.ServiceStartMode.Manual;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.vwSerProcessInstaller,
            this.vsSerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller vwSerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller vsSerInstaller;
    }
}