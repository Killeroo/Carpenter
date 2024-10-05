using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    /// <summary>
    /// Stores customizable values for the Carpenter library
    /// </summary>
    public static class Config
    {
        public const float kVersion = 3.0f;
        public const string kSchemaFileName = "SCHEMA";
        public const string kSiteFileName = "SITE";
        public const string kTemplateFilename = "template.html";
        public const string kGeneratedPreviewPostfix = "_preview";
        public const string kDefaultGeneratedFilename = "index.html"
    }
}
