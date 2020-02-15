using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using winsw.Configuration;
using winsw.Native;
using WMI;

namespace winsw
{
    public class YamlConfig : IWinSWConfiguration
    {
        public string Id => throw new NotImplementedException();

        public string Caption => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public string Executable => throw new NotImplementedException();

        public string ExecutablePath => throw new NotImplementedException();

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
    }
}
