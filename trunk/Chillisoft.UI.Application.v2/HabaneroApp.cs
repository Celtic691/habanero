using System;
using System.IO;
using System.Xml;
using Chillisoft.Bo.ClassDefinition.v2;
using Chillisoft.Bo.Loaders.v2;
using Chillisoft.Db.v2;
using Chillisoft.Generic.v2;
//using Chillisoft.UI.Misc.v2;
using Chillisoft.UI.Misc.v2;
using log4net;
using log4net.Config;

namespace Chillisoft.UI.Application.v2
{
    /// <summary>
    /// Provides a template for a standard Habanero application, including
    /// standard fields and initialisations.  Specific details covered are:
    /// <ul>
    /// <li>The class definitions that define how the data is represented
    /// and limited</li>
    /// <li>The database configuration, connection and settings</li>
    /// <li>A logger to record debugging and error messages</li>
    /// <li>An exception notifier to communicate exceptions to the user</li>
    /// <li>Automatic version upgrades when an application is out-of-date</li>
    /// <li>A synchronisation controller</li>
    /// </ul>
    /// To set up and launch an application:
    /// <ol>
    /// <li>Instantiate the application with the constructor</li>
    /// <li>Specify any individual settings as required</li>
    /// <li>Call the Startup() method to launch the application</li>
    /// </ol>
    /// </summary>
    public class HabaneroApp
    {
        private readonly string itsAppName;
        private readonly string itsAppVersion;
        private static ILog log;
        private ApplicationVersionUpgrader itsApplicationVersionUpgrader;
        private string itsClassDefsPath = "";
        private string itsClassDefsFileName = "ClassDefs.xml";
        private DatabaseConfig itsDatabaseConfig;
        private IExceptionNotifier itsExceptionNotifier;
        private SynchronisationController itsSynchronisationController;
        private ISettingsStorer itsSettingsStorer;
        private bool itsLoadClassDefs = true;

        /// <summary>
        /// Constructor to initialise a new application with basic application
        /// information.  Use the Startup() method to launch the application.
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <param name="appVersion">The application version</param>
        public HabaneroApp(string appName, string appVersion) {
            itsAppName = appName;
            itsAppVersion = appVersion;
        }

        /// <summary>
        /// Sets the class definition path.  The class definitions specify
        /// the format and limitations of the data.
        /// </summary>
        public string ClassDefsPath {
            set { itsClassDefsPath = value; }
        }

        /// <summary>
        /// Sets the class definition file name.  The class definitions specify
        /// the format and limitations of the data.
        /// </summary>
        public string ClassDefsFileName {
            set { itsClassDefsFileName = value; }
        }

        /// <summary>
        /// Sets the database configuration object, which contains basic 
        /// connection information along with the database vendor name 
        /// (eg. MySQL, Oracle).
        /// </summary>
        public DatabaseConfig DatabaseConfig {
            set { itsDatabaseConfig = value; }
        }

        /// <summary>
        /// Sets the version upgrader, which carries
        /// out upgrades on an installed application to upgrade it to newer
        /// versions.
        /// </summary>
        public ApplicationVersionUpgrader ApplicationVersionUpgrader {
            set { itsApplicationVersionUpgrader = value; }
        }

        /// <summary>
        /// Sets the exception notifier, which is used to inform the
        /// user of exceptions encountered.
        /// </summary>
        public IExceptionNotifier ExceptionNotifier {
            set { itsExceptionNotifier = value; }
        }

        /// <summary>
        /// Sets the synchronisation controller, which implements a
        /// synchronisation strategy for the application.
        /// </summary>
        public SynchronisationController SynchronisationController {
            set { itsSynchronisationController = value; }
        }

        /// <summary>
        /// Sets the settings storer, which stores database settings
        /// </summary>
        public ISettingsStorer SettingsStorer {
            set { itsSettingsStorer = value; }
        }

        /// <summary>
        /// Sets the class definitions to the object specified
        /// </summary>
        public bool LoadClassDefs {
            set { itsLoadClassDefs = value; }
        }

        /// <summary>
        /// Gets the loader for the xml class definitions
        /// </summary>
        /// <returns>Returns the loader</returns>
        private XmlClassDefsLoader GetXmlClassDefsLoader()
        {
            try
            {
                return new XmlClassDefsLoader(new StreamReader(itsClassDefsFileName).ReadToEnd(), itsClassDefsPath);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Unable to find Class Definitions file. " +
                      "This file contains all the class definitions that match " +
                      "objects to database tables. Ensure that you have a classdefs.xml file " +
                      "and that the file is being copied to your output directory (eg. bin/debug).", ex);
            }
        }

        /// <summary>
        /// Launches the application, initialising the logger, the database
        /// configuration and connection, the class definitions, the exception
        /// notifier and the synchronisation controller.  This method also
        /// carries out any version upgrades using the 
        /// ApplicationVersionUpgrader, if specified.
        /// </summary>
        /// <returns>Returns true if launched successfully, false if not. A
        /// failed launch will result in error messages being sent to the log
        /// with further information about the failure.</returns>
        public bool Startup() {
            try {
                if (itsExceptionNotifier == null) itsExceptionNotifier = new FormExceptionNotifier();
                GlobalRegistry.UIExceptionNotifier = itsExceptionNotifier;

                if (itsSynchronisationController == null) itsSynchronisationController = new NullSynchronisationController();
                GlobalRegistry.SynchronisationController = itsSynchronisationController;

                GlobalRegistry.ApplicationName = itsAppName;
                GlobalRegistry.ApplicationVersion = itsAppVersion;

                try
                {
                    DOMConfigurator.Configure();
                }
                catch (Exception ex)
                {
                    throw new XmlException("There was an error reading the XML configuration file. " +
                        "Check that all custom configurations, such as DatabaseConfig, are well-formed, " +
                        "spelt correctly and have been declared correctly in configSections.  See the " +
                        "Habanero tutorial for example usage or see official " +
                        "documentation on configuration files if the error is not resolved.", ex);
                }
                log = LogManager.GetLogger("HabaneroApp");

                log.Debug("---------------------------------------------------------------------");
                log.Debug(itsAppName + "v" + itsAppVersion + " starting");
                log.Debug("---------------------------------------------------------------------");

                if (itsDatabaseConfig == null) itsDatabaseConfig = DatabaseConfig.ReadFromConfigFile();
                DatabaseConnection.CurrentConnection = itsDatabaseConfig.GetDatabaseConnection();

                if (itsSettingsStorer == null) itsSettingsStorer = new DatabaseSettingsStorer();
                GlobalRegistry.SettingsStorer = itsSettingsStorer;

                if (itsApplicationVersionUpgrader != null) itsApplicationVersionUpgrader.Upgrade();

                if (itsLoadClassDefs) ClassDef.LoadClassDefs(GetXmlClassDefsLoader());
            }
            catch (Exception ex) {
                string errorMessage = "There was a problem starting the application.";
                if (log != null && log.Logger.IsEnabledFor(log4net.spi.Level.ERROR))
                {
                    log.Error("---------------------------------------------" +
                        Environment.NewLine + ExceptionUtil.GetCategorizedExceptionString(ex, 0));
                    errorMessage += " Please look at the log file for details of the problem.";
                }
                GlobalRegistry.UIExceptionNotifier.Notify(
                    new UserException(errorMessage, ex),
                    "Problem in Startup:", "Problem in Startup");
                //MessageBox.Show("An error happened on program startup : " + Environment.NewLine + ex.Message + Environment.NewLine + "Please see log file for details.");
                return false;
            }
            return true;
        }
    }
}