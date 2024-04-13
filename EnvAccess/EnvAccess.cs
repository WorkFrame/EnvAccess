using Microsoft.Win32;
using NetEti.Globals;
using System.Runtime.InteropServices;

namespace NetEti.ApplicationEnvironment
{
    /// <summary>
    /// Zugriffe auf das Environment und auf einige Application-Properties.<br></br>
    ///           Implementiert IGetStringValue.
    /// </summary>
    /// <remarks>
    /// File: EnvAccess.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt
    /// 01.05.2020 Erik Nagel: auf DotNet Core 3.0 Verträglichkeit angepasst:
    /// Zugriffe nicht mehr über System.IO und System.Windows.Forms.Application, sondern über
    /// System.Reflection.Assembly.GetEntryAssembly().
    /// </remarks>
    public class EnvAccess : IGetStringValue
    {
        private string[]? _activationData;

        #region public members

        /// <summary>
        /// Umstellung auf .net standard 2.0.
        /// Ersetzt die Kommandozeilen-Argumente für ClickOnce-Installationen unter .net standard 2.0.
        /// </summary>
        public string[]? ActivationData
        {
            get
            {
                return this._activationData;
            }
        }

        #region IGetStringValue Members

        /// <summary>
        /// Liefert genau einen Wert zu einem Key. Wenn es keinen Wert zu dem
        /// Key gibt, wird defaultValue zurückgegeben.
        /// </summary>
        /// <param name="key">Der Zugriffsschlüssel (string)</param>
        /// <param name="defaultValue">Das default-Ergebnis (string)</param>
        /// <returns>Der Ergebnis-String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1302:DoNotHardcodeLocaleSpecificStrings", Justification = "SENDTO is a programmer-user-literal")]
        public string? GetStringValue(string key, string? defaultValue)
        {
            ClickOnceInfo clickOnceInfo = new ClickOnceInfo();
            this._activationData = clickOnceInfo.ActivationData;
            string? rtn = null;
            System.Reflection.Assembly? assembly;
            switch (key.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
            {
                case "ISNETWORKDEPLOYED":
                    // rtn = System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed.ToString(); break;
                    rtn = clickOnceInfo.IsNetworkDeployed.ToString(); break;

                case "CLICKONCEDATA":
                    // if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    if (clickOnceInfo.IsNetworkDeployed)
                    {
                        // string clickOnceDataDirectory = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory;
                        string? clickOnceDataDirectory = clickOnceInfo.DataDirectory;
                        if (!String.IsNullOrEmpty(clickOnceDataDirectory))
                        {
                            rtn = clickOnceDataDirectory;
                        }
                    }
                    break;
                case "APPLICATIONROOTPATH":
                    // rtn = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath); break;
                    assembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        rtn = Path.GetDirectoryName(assembly.Location);
                    }
                    else
                    {
                        rtn = Directory.GetCurrentDirectory();
                    }
                    break;
                case "APPLICATIONGUID":
                    assembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        // Achtung: die Guid wird aus einer zusätzlichen Datei "AssemblyInfo.cs" gelesen.
                        // In .Net Core 7 muss die AssemblyInfo.cs manuell erzeugt werden:
                        // Projekt - Hinzufügen/Neues Element/Allgemein/Assemblyinformationsdatei
                        object[] attributes = assembly
                            .GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                        if (attributes != null && attributes.Length > 0)
                        {
                            rtn = ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
                        }
                        // Hinweis: eine Lösung über eine in EnvAccessDemo.csproj eingebettete Guid
                        //          konnte ich bisher nicht herausfinden (02.03.2023 Nagel).
                    }
                    break;
                case "LOCALAPPDATA":
                case "LOCALAPPLICATIONDATA":
                    rtn = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (String.IsNullOrEmpty(rtn))
                    {
                        rtn = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // vor Vista
                    }
                    break;
                case "COMMANDLINE": rtn = Environment.CommandLine; break;
                case "CURRENTDIRECTORY": rtn = Environment.CurrentDirectory; break;
                case "EXITCODE": rtn = Environment.ExitCode.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "HASSHUTDOWNSTARTED": rtn = Environment.HasShutdownStarted.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "MACHINENAME": rtn = Environment.MachineName; break;
                case "NEWLINE": rtn = Environment.NewLine; break;
                case "OSVERSION": rtn = Environment.OSVersion.ToString(); break;
                case "OSVERSIONMAJOR": rtn = Environment.OSVersion.Version.Major.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "PROCESSORCOUNT": rtn = Environment.ProcessorCount.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "APPDATA":
                case "APPLICATIONDATA": rtn = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); break;
                case "COMMONAPPLICATIONDATA": rtn = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); break;
                case "COMMONPROGRAMFILES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles); break;
                case "COOKIES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Cookies); break;
                case "DESKTOP": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); break;
                case "DESKTOPDIRECTORY": rtn = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); break;
                case "FAVORITES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Favorites); break;
                case "ISFRAMEWORKASSEMBLY": rtn = IsFrameworkAssembly().ToString(); break;
                case "FRAMEWORKVERSIONMAJOR": rtn = EnvAccess.getFrameworkVersionMajor().ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "HISTORY": rtn = Environment.GetFolderPath(Environment.SpecialFolder.History); break;
                case "INTERNETCACHE": rtn = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache); break;
                case "MYCOMPUTER": rtn = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer); break;
                case "MYDOCUMENTS": rtn = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); break;
                case "MYMUSIC": rtn = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); break;
                case "MYPICTURES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); break;
                case "PERSONAL": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Personal); break;
                case "PROGRAMFILES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); break;
                case "PROGRAMS": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Programs); break;
                case "RECENT": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Recent); break;
                case "SENDTO": rtn = Environment.GetFolderPath(Environment.SpecialFolder.SendTo); break;
                case "STARTMENU": rtn = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu); break;
                case "STARTUP": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Startup); break;
                case "SYSTEM": rtn = Environment.GetFolderPath(Environment.SpecialFolder.System); break;
                case "TEMPLATES": rtn = Environment.GetFolderPath(Environment.SpecialFolder.Templates); break;
                case "STACKTRACE": rtn = Environment.StackTrace; break;
                case "SYSTEMDIRECTORY": rtn = Environment.SystemDirectory; break;
                case "TICKCOUNT": rtn = Environment.TickCount.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "USERDOMAINNAME": rtn = Environment.UserDomainName; break;
                case "USERINTERACTIVE": rtn = Environment.UserInteractive.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "USERNAME": rtn = Environment.UserName; break;
                case "VERSION": rtn = Environment.Version.ToString(); break;
                case "WORKINGSET": rtn = Environment.WorkingSet.ToString(System.Globalization.CultureInfo.CurrentCulture); break;
                case "PRODUCTNAME":
                    //case "PRODUCTNAME": rtn = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; break;
                    assembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        // rtn = System.Windows.Forms.Application.ProductName; break;
                        rtn = assembly.GetName().Name; break;
                    }
                    else
                    {
                        rtn = null;
                    }
                    break;
                case "PROGRAMVERSION":
                    //case "PROGRAMVERSION": rtn = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); break;
                    // rtn = System.Windows.Forms.Application.ProductVersion; break;
                    assembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        rtn = assembly.GetName()?.Version?.ToString();
                    }
                    else
                    {
                        rtn = null;
                    }
                    break;
                default:
                    rtn = Environment.GetEnvironmentVariable(key); break;
            }
            if (!key.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Equals("NEWLINE") && (String.IsNullOrEmpty(rtn) || (rtn.Trim().Length == 0)))
            {
                rtn = defaultValue;
            }
            else
            {
                if (rtn == null || rtn.Length == 0)
                {
                    rtn = defaultValue;
                }
            }
            return rtn;
        }

        /// <summary>
        /// Liefert ein string-Array zu einem Key. Wenn es keinen Wert zu dem
        /// Key gibt, wird defaultValue zurückgegeben.
        /// </summary>
        /// <param name="key">Der Zugriffsschlüssel (string)</param>
        /// <param name="defaultValues">Das default-Ergebnis (string[])</param>
        /// <returns>Das Ergebnis-String-Array</returns>
        public string?[]? GetStringValues(string key, string?[]? defaultValues)
        {
            string? rtn = GetStringValue(key, null);
            if (rtn != null)
            {
                return new string[] { rtn };
            }
            else
            {
                return defaultValues;
            }
        }

        /// <summary>
        /// Liefert einen beschreibenden Namen dieses StringValueGetters,
        /// z.B. Name plus ggf. Quellpfad.
        /// </summary>
        public string Description { get; set; }

        #endregion

        /// <summary>
        /// Standard-Konstruktor.
        /// </summary>
        public EnvAccess()
        {
            this.Description = "Environment";
        }

        #endregion public members

        #region private members

        /// <summary>
        /// Ermittelt die höchste installierte Version des .Net-Frameworks.
        /// </summary>
        /// <returns>Die höchste installierte Major-Version des .Net-Frameworks als Integer</returns>
        private static int getFrameworkVersionMajor()
        {
            int maxVersion = 0;
            if (OperatingSystem.IsWindows())
            {
                RegistryKey localMaschine = Registry.LocalMachine;
                RegistryKey? registryKey = localMaschine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                // string highestVersion = "0";
                if (registryKey != null)
                {
                    int tmpVersion;
                    foreach (string valueName in registryKey.GetSubKeyNames())
                    {
                        if (Int32.TryParse((valueName.Split(new char[] { '.' })[0]).Replace("v", ""), out tmpVersion) && tmpVersion >= maxVersion)
                        {
                            maxVersion = tmpVersion;
                            // highestVersion = valueName;
                        }
                    }
                }
            }
            // return highestVersion;
            return maxVersion;
        }

        /// <summary>
        /// Liefert true, wenn das Programm ein .Net-Framework-Programm ist.
        /// </summary>
        /// <returns>True, wenn das Programm ein .Net-Framework-Programm ist</returns>
        private static bool IsFrameworkAssembly()
        {
            return RuntimeInformation.FrameworkDescription.Contains(".NET Framework");
        }

        #endregion private members

    }
}
