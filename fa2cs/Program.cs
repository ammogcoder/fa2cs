﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fa2cs.Helpers;

namespace fa2cs
{
    class MainClass
    {
        public const string Endpoint = "https://fontawesome.com/cheatsheet";

        public const string FontAwesomeIconsAssemblyFileName = "FontAwesome.IconCodes.dll";

        public const string FontAwesomeIconsAssemblyDocsFileName = "FontAwesome.IconCodes.xml";

        public static async Task Main(string[] args)
        {
            var exportPath = AssemblyHelper.EntryAssemblyDirectory;
            if (args != null && args.Any())
            {
                exportPath = args.First();
            }

            var outputPath = Path.Combine(exportPath, "fa2cs-output");

            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            Directory.CreateDirectory(outputPath);

            var downloader = new FontAwesomeDownloader();
            var codeWriter = new CodeWriter();

            var icons = await downloader.DownloadIconCodes(Endpoint);

            var code = codeWriter.Write(icons);

            var codeFiles = new List<string>()
            {
                code,
                ResourcesHelper.ReadResourceContent("AssemblyInfoTemplate.txt"),
            };

            AssemblyEmitter.EmitAssembly(codeFiles, outputPath);

            File.WriteAllText(Path.Combine(outputPath, "readme.txt"), ResourcesHelper.ReadResourceContent("readme.txt"));
            File.WriteAllText(Path.Combine(exportPath, "FontAwesomeIcons.cs"), code);

            OpenFileHelper.OpenAndSelect(exportPath);
        }
    }
}
