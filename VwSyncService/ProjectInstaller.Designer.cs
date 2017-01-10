namespace VwSyncService
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
            this.serviceProcessInstallerSync = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerSync = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerSync
            // 
            this.serviceProcessInstallerSync.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstallerSync.Password = null;
            this.serviceProcessInstallerSync.Username = null;
            // 
            // serviceInstallerSync
            // 
            this.serviceInstallerSync.ServiceName = "ServiceSync";
            this.serviceInstallerSync.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerSync,
            this.serviceInstallerSync});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerSync;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSync;
    }
}