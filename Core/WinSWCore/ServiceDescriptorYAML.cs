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

        public string Executable => SingleElement("executable");
       
        public bool HideWindow => SingleBoolElement("hidewindow", Defaults.HideWindow);

        public string? StopExecutable => SingleElement("stopexecutable", true);
    }

}
