using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HrUpdater
{
    public class HrUpdateEngineDefinition
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DestinationPath { get; set; }
    }
}
