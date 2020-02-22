using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using winsw.Configuration;
using winsw.Native;
using WMI;
using YamlDotNet.Serialization;
using System.Linq;
using System.Collections;

namespace winsw
{
    public class ServiceDescriptorYAML
    {
        //Defauls values for configurations
        public static DefaultWinSWSettings Defaults { get; } = new DefaultWinSWSettings();


        protected readonly string dom;

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

        private readonly YAMLConfig configurations;

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

            BaseName = baseName + ".yaml";
            BasePath = Path.Combine(d.FullName, BaseName);

            //load yaml file
            using (var reader = new StreamReader(BasePath))
            {
                dom = reader.ReadToEnd();
            }

            //Initialize the Deserailizer
            var deserializer = new DeserializerBuilder().Build();

            //deserialize the yaml
            configurations = deserializer.Deserialize<YAMLConfig>(dom);

            // register the base directory as environment variable so that future expansions can refer to this.
            Environment.SetEnvironmentVariable("BASE", d.FullName);

            // ditto for ID
            Environment.SetEnvironmentVariable("SERVICE_ID", configurations.Id);

            // New name
            Environment.SetEnvironmentVariable(WinSWSystem.ENVVAR_NAME_EXECUTABLE_PATH, ExecutablePath);

            // Also inject system environment variables
            Environment.SetEnvironmentVariable(WinSWSystem.ENVVAR_NAME_SERVICE_ID, configurations.Id);

            Console.WriteLine(SingleElement("Id"));

        }

        //Add constructors matching to SERviceDescriptor class
        /*public static ServiceDescriptorYAML FromYAML(string yaml)
        {
            return null;
        }*/

        private string SingleElement(string tagName)
        {
            return SingleElement(tagName, false)!;
        }

        private string? SingleElement(string tagName, bool optional)
        {
            var node = configurations.GetType().GetProperty(tagName).GetValue(configurations, null);
            if (node == null && !optional)
                throw new InvalidDataException(tagName + " is missing in configurations YAML");

            return node == null ? null : node.ToString();
        }

        private bool SingleBoolElement(string tagName, bool defaultValue)
        {
            var node = configurations.GetType().GetProperty(tagName).GetValue(configurations, null);

            return node == null ? defaultValue : bool.Parse(node.ToString());
        }

        private int SingleIntElement(string tagName, int defaultValue)
        {
            var node = configurations.GetType().GetProperty(tagName).GetValue(configurations, null);

            return node == null ? defaultValue : int.Parse(node.ToString());
        }

        private TimeSpan SingleTimeSpanElement(string tagName, TimeSpan defaultValue)
        {
            var node = SingleElement(tagName, true);

            return node == null ? defaultValue : ParseTimeSpan(node); 
        }

        private TimeSpan ParseTimeSpan(string v)
        {
            v = v.Trim();
            foreach (var s in Suffix)
            {
                if (v.EndsWith(s.Key))
                {
                    return TimeSpan.FromMilliseconds(int.Parse(v.Substring(0, v.Length - s.Key.Length).Trim()) * s.Value);
                }
            }

            return TimeSpan.FromMilliseconds(int.Parse(v));
        }

        private static readonly Dictionary<string, long> Suffix = new Dictionary<string, long>
        {
            { "ms",     1 },
            { "sec",    1000L },
            { "secs",   1000L },
            { "min",    1000L * 60L },
            { "mins",   1000L * 60L },
            { "hr",     1000L * 60L * 60L },
            { "hrs",    1000L * 60L * 60L },
            { "hour",   1000L * 60L * 60L },
            { "hours",  1000L * 60L * 60L },
            { "day",    1000L * 60L * 60L * 24L },
            { "days",   1000L * 60L * 60L * 24L }
        };



        public string Executable => SingleElement("executable");
       
        public bool HideWindow => SingleBoolElement("hidewindow", Defaults.HideWindow);

        public string? StopExecutable => SingleElement("stopexecutable", true);


        public string Arguments
        {
            get
            {
                string? arguments = AppendTags("argument", null);
                if(arguments == null)
                {
                    var argumentsNode = configurations.GetType().GetProperty("arguments").GetValue(configurations, null).ToString();
                    
                    if(argumentsNode == null)
                    {
                        return Defaults.Arguments;
                    }

                    return argumentsNode;
                }
                else
                {
                    return arguments;
                }
            }
        }

        public string? Startarguments => AppendTags("startargument", Defaults.Startarguments);

        public string? Stoparguments => AppendTags("stopargument", Defaults.Stoparguments);

        public string WorkingDirectory
        {
            get
            {
                var wd = SingleElement("workingdirectory", true);
                return string.IsNullOrEmpty(wd) ? Defaults.WorkingDirectory : wd!;
            }
        }

        public string ExtensionsConfiguration => configurations.GetType().GetProperty("extensions").GetValue(configurations, null).ToString();

        private string? AppendTags(string tagName, string? defaultValue = null)
        {
            var argumentNode = configurations.GetType().GetProperty(tagName);
            if(argumentNode == null)
            {
                return defaultValue;
            }

            var arguments = new StringBuilder();
            var argumentNodeList = (IList<string>)configurations.GetType().GetProperty(tagName).GetValue(configurations, null);

            foreach(var argument in argumentNodeList)
            {
                arguments.Append(' ');


                //When creating YAML configurations file, should write it in the manner of write environment details as
                //they can be replaced in ExpandEnvironmentVariables() method.
                string token = Environment.ExpandEnvironmentVariables(argument);

                if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    // for backward compatibility, if the argument is already quoted, leave it as is.
                    // in earlier versions we didn't handle quotation, so the user might have worked
                    // around it by themselves
                }
                else
                {
                    if (token.Contains(" "))
                    {
                        arguments.Append('"').Append(token).Append('"');
                        continue;
                    }
                }

                arguments.Append(token);
            }

            return arguments.ToString();
        }
    }
}
