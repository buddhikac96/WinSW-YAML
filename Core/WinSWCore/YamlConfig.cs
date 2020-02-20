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
    public class YAMLConfig
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string executable { get; set; }
        public string priority { get; set; }
        public string stoptimeout { get; set; }
        public Boolean stopparentprocessfirst { get; set; }
        public string startmode { get; set; }
        public string waithint { get; set; }
        public string sleeptime { get; set; }
    }
}
