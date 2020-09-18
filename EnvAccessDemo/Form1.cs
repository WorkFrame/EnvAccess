using System;
using System.Windows.Forms;
using NetEti.ApplicationEnvironment;

namespace NetEti.DemoApplications
{
    /// <summary>
    /// Demo für EnvAccess<br></br>
    /// Holt Umgebungsvariablen und zeigt sie an.
    /// </summary>
    /// <remarks>
    /// File: Form1.cs
    /// Autor: Erik Nagel, NetEti
    ///
    /// 08.03.2012 Erik Nagel: erstellt
    /// </remarks>
    public partial class Form1 : Form
    {
        EnvAccess envAccess;

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            envAccess = new EnvAccess();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.listBox1.Items.Add(String.Format("{0}: {1}", "IsNetworkDeployed", envAccess.GetStringValue("IsNetworkDeployed", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "ClickOnceData", envAccess.GetStringValue("ClickOnceData", "???")));
            //foreach (Environment.SpecialFolder sf in (Environment.SpecialFolder[])Enum.GetValues(typeof(Environment.SpecialFolder)))
            //{
            //    this.listBox1.Items.Add(sf.ToString() + ": " + Environment.GetFolderPath(sf));
            //}
            this.listBox1.Items.Add(String.Format("{0}: {1}", "TEMP", envAccess.GetStringValue("TEMP", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "COMMANDLINE", envAccess.GetStringValue("COMMANDLINE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "CURRENTDIRECTORY", envAccess.GetStringValue("CURRENTDIRECTORY", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "APPLICATIONROOTPATH", envAccess.GetStringValue("APPLICATIONROOTPATH", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "APPLICATIONGUID", envAccess.GetStringValue("APPLICATIONGUID", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "LOCALAPPLICATIONDATA", envAccess.GetStringValue("LOCALAPPLICATIONDATA", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "EXITCODE", envAccess.GetStringValue("EXITCODE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "HASSHUTDOWNSTARTED", envAccess.GetStringValue("HASSHUTDOWNSTARTED", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "MACHINENAME", envAccess.GetStringValue("MACHINENAME", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "NEWLINE", envAccess.GetStringValue("NEWLINE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "OSVERSION", envAccess.GetStringValue("OSVERSION", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PROCESSORCOUNT", envAccess.GetStringValue("PROCESSORCOUNT", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "APPLICATIONDATA", envAccess.GetStringValue("APPLICATIONDATA", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "COMMONAPPLICATIONDATA", envAccess.GetStringValue("COMMONAPPLICATIONDATA", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "COMMONPROGRAMFILES", envAccess.GetStringValue("COMMONPROGRAMFILES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "COOKIES", envAccess.GetStringValue("COOKIES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "DESKTOP", envAccess.GetStringValue("DESKTOP", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "DESKTOPDIRECTORY", envAccess.GetStringValue("DESKTOPDIRECTORY", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "FAVORITES", envAccess.GetStringValue("FAVORITES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "HISTORY", envAccess.GetStringValue("HISTORY", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "INTERNETCACHE", envAccess.GetStringValue("INTERNETCACHE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "MYCOMPUTER", envAccess.GetStringValue("MYCOMPUTER", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "MYDOCUMENTS", envAccess.GetStringValue("MYDOCUMENTS", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "MYMUSIC", envAccess.GetStringValue("MYMUSIC", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "MYPICTURES", envAccess.GetStringValue("MYPICTURES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PERSONAL", envAccess.GetStringValue("PERSONAL", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PROGRAMFILES", envAccess.GetStringValue("PROGRAMFILES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PROGRAMS", envAccess.GetStringValue("PROGRAMS", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "RECENT", envAccess.GetStringValue("RECENT", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "SENDTO", envAccess.GetStringValue("SENDTO", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "STARTMENU", envAccess.GetStringValue("STARTMENU", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "STARTUP", envAccess.GetStringValue("STARTUP", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "SYSTEM", envAccess.GetStringValue("SYSTEM", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "TEMPLATES", envAccess.GetStringValue("TEMPLATES", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "STACKTRACE", envAccess.GetStringValue("STACKTRACE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "SYSTEMDIRECTORY", envAccess.GetStringValue("SYSTEMDIRECTORY", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "TICKCOUNT", envAccess.GetStringValue("TICKCOUNT", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "USERDOMAINNAME", envAccess.GetStringValue("USERDOMAINNAME", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "USERINTERACTIVE", envAccess.GetStringValue("USERINTERACTIVE", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "USERNAME", envAccess.GetStringValue("USERNAME", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "VERSION", envAccess.GetStringValue("VERSION", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "WORKINGSET", envAccess.GetStringValue("WORKINGSET", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "FRAMEWORKVERSIONMAJOR", envAccess.GetStringValue("FRAMEWORKVERSIONMAJOR", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PRODUCTNAME", envAccess.GetStringValue("PRODUCTNAME", "???")));
            this.listBox1.Items.Add(String.Format("{0}: {1}", "PROGRAMVERSION", envAccess.GetStringValue("PROGRAMVERSION", "???")));
        }
    }
}
