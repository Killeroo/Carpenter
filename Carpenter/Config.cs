﻿using System;

namespace Carpenter
{
    /// <summary>
    /// Stores configurable values for the Carpenter library
    /// </summary>
    public static class Config
    {
        public const float kVersion = 3.0f;
        public const string kVersionOption = "carpenter_version";
        public const string kSchemaFileName = "SCHEMA";
        public const string kSiteFileName = "SITE";
        public const string kTemplateFilename = "template.html";
        public const string kGeneratedPreviewPostfix = "_preview";
        public const string kDefaultGeneratedFilename = "index.html";
        public const string kTemplateImageWidthToken = "%WIDTH";
        public const string kTemplateImageHeightToken = "%HEIGHT";
        public const string kGeneratedComment = "<!-- Generated by Carpenter, Static Website Generator (https://github.com/Killeroo/Carpenter), at {0} UTC -->";

        public const string kTagRegexPattern = @"(?<=TAG:).*\w+";
    }
}
