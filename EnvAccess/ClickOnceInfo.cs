using System.Globalization;
using System.Xml.Linq;

namespace Framework.ClickOnce
{
	/// <summary>
	/// This is a partial replacement for ApplicationDeployment which is not available in .NET 6
	/// We use a custom Launcher.exe which will set various "CLICKONCE_*" local environment variables
	/// Thanks to Simon Hewitt (simmotech) for this helpful workaround.
	/// https://github.com/simmotech/Net6ClickOnce
	/// Wichtig: Das ganze funktioniert nur, wenn die App über die modifizierte "launcher.exe"
	/// aus dem Projekt ...Net6ClickOnce-master\ClickOnceLauncher gestartet wird.
	/// Ansonsten werden die zusätzlichen Environment-Variablen nicht gesetzt und Defauts zurückgegeben.
	/// </summary>
	/// <remarks>
	/// 23.02.2023 Erik Nagel: übernommen.
	/// </remarks>
	public class ClickOnceInfo
	{
		/// <summary>
		/// Standard-Konstruktor - füllt Properties aus dem (von Launcher.exe modifizierten) Environment.
		/// </summary>
		public ClickOnceInfo()
		{
			BaseDirectory = AppContext.BaseDirectory;
			TargetFrameworkName = String.IsNullOrEmpty(AppContext.TargetFrameworkName) ? "unknown" : AppContext.TargetFrameworkName;

            if (Environment.GetEnvironmentVariable("CLICKONCE_ISNETWORKDEPLOYED") == bool.TrueString)
			{
                IsNetworkDeployed = true;
            }
            CurrentVersion = new Version("0.0.0.0");
            if (Environment.GetEnvironmentVariable("CLICKONCE_CURRENTVERSION") is {} currentVersionString && Version.TryParse(currentVersionString, out var currentVersion))
			{
				CurrentVersion = currentVersion;
			}
            if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATEDVERSION") is {} updatedVersionString && Version.TryParse(updatedVersionString, out var updatedVersion))
			{
				UpdatedVersion = updatedVersion;
			}

			if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATELOCATION") is {} updateLocationString && Uri.TryCreate(updateLocationString, UriKind.RelativeOrAbsolute, out var updateLocation))
			{
				UpdateLocation = updateLocation;
				if (UpdateLocation != null)
				{
                    ApplicationName = UpdateLocation.Segments[1].Replace(".application", null, StringComparison.OrdinalIgnoreCase);
                }
            }

			if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATEDAPPLICATIONFULLNAME") is {} updatedApplicationFullName)
			{
				UpdatedApplicationFullName = updatedApplicationFullName;
			}

			if (Environment.GetEnvironmentVariable("CLICKONCE_TIMEOFLASTUPDATECHECK") is {} timeOfLastUpdateCheckString && DateTime.TryParse(timeOfLastUpdateCheckString, null, DateTimeStyles.RoundtripKind, out var timeOfLastUpdateCheck))
			{
				TimeOfLastUpdateCheck = timeOfLastUpdateCheck;
			}

			if (Environment.GetEnvironmentVariable("CLICKONCE_ACTIVATIONURI") is {} activationUriString && Uri.TryCreate(activationUriString, UriKind.RelativeOrAbsolute, out var activationUri))
			{
				ActivationUri = activationUri;
			}

			if (Environment.GetEnvironmentVariable("CLICKONCE_DATADIRECTORY") is {} dataDirectory)
			{
				DataDirectory = dataDirectory;
			}

			// Not 100%e sure what this is but it is mentioned at https://github.com/dotnet/deployment-tools/pull/135 and https://github.com/dotnet/deployment-tools/issues/113
			// so we can include it. Think it might be about passing command line arguments and/or FileAssociation arguments.
			if (Environment.GetEnvironmentVariable("CLICKONCE_ACTIVATIONDATA_1") is {} activationDataItem)
			{
				var items = new List<string>();
				var index = 1;

				do
				{
					items.Add(activationDataItem);

					activationDataItem = Environment.GetEnvironmentVariable($"CLICKONCE_ACTIVATIONDATA_{++index}");
				}
				while (activationDataItem != null);

				ActivationData = items.ToArray();
			}
		}

        /// <summary>
        /// Das Verzeichnis, in dem die Applikation gestartet wurde als absoluter Pfad.
        /// </summary>
		public string BaseDirectory { get; init; }

        /// <summary>
        /// RuntimeInformation.FrameworkDescription.
        /// </summary>
        public string TargetFrameworkName { get; init; }

        /// <summary>
        /// True, wenn die Anwendung per ClickOnce installiert wurde.
        /// </summary>
		public bool IsNetworkDeployed { get; init; }

        /// <summary>
        /// Die aktuelle ProgrammVersion = Application.ProductVersion
        /// </summary>
		public Version CurrentVersion { get; init; }

        /// <summary>
        /// Die neue ProgrammVersion.
        /// </summary>
		public Version? UpdatedVersion { get; init; }

        /// <summary>
        /// Die Update-Url für das Programm.
        /// </summary>
        public Uri? UpdateLocation { get; init; }

		/// <summary>
		/// Qualifizierter Name der aktualisierten Applikation.
		/// </summary>
		public string? UpdatedApplicationFullName { get; init; }

		/// <summary>
		/// Zeitpunkt der letzten Update-Prüfung.
		/// </summary>
		public DateTime TimeOfLastUpdateCheck { get; init; }

		/// <summary>
		/// Url für die Aktivierung.
		/// </summary>
		public Uri? ActivationUri { get; init; }

        /// <summary>
        /// Verzeichnis, in dem die Installationsdaten bei einer
        /// ClickOnce-Installation (EnvAccess:ISNETWORKDEPLOYED = true) liegen.
        /// </summary>
		public string? DataDirectory { get; init; }

		/// <summary>
		/// Ersetzt die Kommandozeilen-Argumente.
		/// </summary>
		public string[]? ActivationData { get; init; }

        /// <summary>
        /// Application.ProductName
        /// </summary>
		public string? ApplicationName { get; init; }

        /// <summary>
        /// Holt vom remote Server die neueste Versionsinformation.
        /// </summary>
        /// <returns>Task&lt;ClickOnceUpdateInfo&gt;</returns>
        public async Task<ClickOnceUpdateInfo?> GetLatestVersionInfo()
		{
			if (!IsNetworkDeployed) return null;

			// TODO: Not tested as yet
			if (UpdateLocation?.Segments[0] != null && UpdateLocation.Segments[0].StartsWith("http", StringComparison.OrdinalIgnoreCase))
			{
				using var client = new HttpClient { BaseAddress = UpdateLocation };
				await using var stream = await client.GetStreamAsync(UpdateLocation);

				return await ReadServerManifest(stream);
			}

			if (UpdateLocation != null && UpdateLocation.IsFile)
			{
				await using var stream = File.OpenRead(UpdateLocation.LocalPath);

				return await ReadServerManifest(stream);
			}

			return null;
		}

        // Based on code from https://github.com/derskythe/WpfSettings/blob/master/PureManApplicationDevelopment/PureManClickOnce.cs
        async Task<ClickOnceUpdateInfo> ReadServerManifest(Stream stream)
		{
			XNamespace nsV1 = "urn:schemas-microsoft-com:asm.v1";
			XNamespace nsV2 = "urn:schemas-microsoft-com:asm.v2";

			var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

			var xmlElement = xmlDoc.Descendants(nsV1 + "assemblyIdentity").FirstOrDefault();
			if (xmlElement == null) throw new ClickOnceDeploymentException($"Invalid manifest document for {ApplicationName}.application");

			var version = xmlElement.Attribute("version")?.Value;
			if (string.IsNullOrEmpty(version)) throw new ClickOnceDeploymentException("Version info is empty!");

			// Minimum version is optional
			var minimumVersion = xmlDoc.Descendants(nsV2 + "deployment").FirstOrDefault()?.Attribute("minimumRequiredVersion")?.Value;

			return new ClickOnceUpdateInfo
				   {
					   CurrentVersion = CurrentVersion,
					   LatestVersion = new Version(version),
					   MinimumVersion = string.IsNullOrEmpty(minimumVersion) ? null : new Version(minimumVersion)
				   };
		}
	}

	/// <summary>
	/// Klasse mit ClickOnce-Updateinformationen.
	/// </summary>
	public class ClickOnceUpdateInfo
	{
		/// <summary>
		/// aktuell installierte Version.
		/// </summary>
		public Version? CurrentVersion { get; init; }

		/// <summary>
		/// Neueste verfügbare Version.
		/// </summary>
		public Version? LatestVersion { get; init; }

		/// <summary>
		/// Geforderte Mindestversion.
		/// </summary>
		public Version? MinimumVersion { get; init; }

		/// <summary>
		/// True, wenn ein Update bereitsteht.
		/// </summary>
		public bool IsUpdateAvailable
		{
			get { return LatestVersion > CurrentVersion; }
		}

		/// <summary>
		/// True, wenn ein Update bereitsteht und zwingend erforderlich ist.
		/// </summary>
		public bool IsMandatoryUpdate
		{
			get { return IsUpdateAvailable && MinimumVersion != null && MinimumVersion > CurrentVersion; }
		}
	}

	/// <summary>
	/// Spezieller Exception-Typ für ClickOnce-Exceptions.
	/// </summary>
	public class ClickOnceDeploymentException: Exception
	{
        /// <summary>
        /// Konstruktor - übernimmt einen Meldungstext.
        /// </summary>
        /// <param name="message">Ein Meldungstext für die Exception.</param>
        public ClickOnceDeploymentException(string message): base(message)
		{}
	}
}
