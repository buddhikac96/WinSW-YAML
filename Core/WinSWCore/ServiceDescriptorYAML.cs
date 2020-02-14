using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using winsw.Configuration;
using winsw.Native;
using WMI;

namespace winsw
{
    class ServiceDescriptorYAML : IWinSWConfiguration
    {
        //confugurations

        public string Id => throw new NotImplementedException();

        public string Caption => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public string Executable => throw new NotImplementedException();


        public bool HideWindow => throw new NotImplementedException();

        public bool AllowServiceAcountLogonRight => throw new NotImplementedException();

        public string? ServiceAccountPassword => throw new NotImplementedException();

        public string ServiceAccountUser => throw new NotImplementedException();

        public List<SC_ACTION> FailureActions => throw new NotImplementedException();

        public TimeSpan ResetFailureAfter => throw new NotImplementedException();

        public string Arguments => throw new NotImplementedException();

        public string? Startarguments => throw new NotImplementedException();

        public string? StopExecutable => throw new NotImplementedException();

        public string? Stoparguments => throw new NotImplementedException();

        public string WorkingDirectory => throw new NotImplementedException();

        public ProcessPriorityClass Priority => throw new NotImplementedException();

        public TimeSpan StopTimeout => throw new NotImplementedException();

        public bool StopParentProcessFirst => throw new NotImplementedException();

        public StartMode StartMode => throw new NotImplementedException();

        public string[] ServiceDependencies => throw new NotImplementedException();

        public TimeSpan WaitHint => throw new NotImplementedException();

        public TimeSpan SleepTime => throw new NotImplementedException();

        public bool Interactive => throw new NotImplementedException();

        public string LogDirectory => throw new NotImplementedException();

        public string LogMode => throw new NotImplementedException();

        public List<Download> Downloads => throw new NotImplementedException();

        public Dictionary<string, string> EnvironmentVariables => throw new NotImplementedException();

        public bool BeepOnShutdown => throw new NotImplementedException();

        public XmlNode? ExtensionsConfiguration => throw new NotImplementedException();



        //Defauls values for configurations
        public static DefaultWinSWSettings Defaults { get; } = new DefaultWinSWSettings();

        /// <summary>
        /// Where did we find the configuration file?
        ///
        /// This string is "c:\abc\def\ghi" when the configuration XML is "c:\abc\def\ghi.xml"
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The file name portion of the configuration file.
        ///
        /// In the above example, this would be "ghi".
        /// </summary>
        public string BaseName { get; set; }

        public virtual string ExecutablePath => Defaults.ExecutablePath;

        public ServiceDescriptorYAML()
        {
            string p = ExecutablePath;
            string baseName = Path.GetFileNameWithoutExtension(p);

            if (baseName.EndsWith(".vshost"))
                baseName = baseName.Substring(0, baseName.Length - 7);

            DirectoryInfo d = new DirectoryInfo(Path.GetDirectoryName(p));
            while (true)
            {
                if (File.Exists(Path.Combine(d.FullName, baseName + ".yaml")))
                    break;

                if (d.Parent == null)
                    throw new FileNotFoundException("Unable to locate " + baseName + ".yaml file within executable directory or any parents");

                d = d.Parent;
            }

            BaseName = baseName;
            BasePath = Path.Combine(d.FullName, BaseName);

            //deserialize the YAML here

        }

    }
}
