namespace XamlColorSchemeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Web.Script.Serialization;

    internal class ColorSchemeGenerator
    {
        public void GenerateColorSchemeFiles(string inputFile)
        {
            var parameters = new JavaScriptSerializer().Deserialize<GeneratorParameters>(File.ReadAllText(inputFile, Encoding.UTF8));

            var templateDirectory = Path.GetDirectoryName(Path.GetFullPath(inputFile));
            var templateFile = Path.Combine(templateDirectory, parameters.TemplateFile);
            var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

            foreach (var variant in parameters.BaseColorSchemes)
            {
                foreach (var colorScheme in parameters.ColorSchemes)
                {
                    this.GenerateColorSchemeFile(parameters, templateDirectory, templateContent, variant, colorScheme);
                }
            }
        }

        public void GenerateColorSchemeFile(GeneratorParameters parameters, string templateDirectory, string templateContent, BaseColorScheme baseColorScheme, ColorScheme colorScheme)
        {
            var themeName = $"{baseColorScheme.Name}.{colorScheme.Name}";
            var themeDisplayName = $"{colorScheme.Name} ({baseColorScheme.Name})";
            var themeFilename = $"{themeName}.xaml";
            var themeTempFileName = $"{themeFilename}_{Guid.NewGuid()}.xaml";

            var themeFile = Path.Combine(templateDirectory, themeFilename);
            var themeTempFile = Path.Combine(Path.GetTempPath(), themeTempFileName);

            try
            {
                var fileContent = this.GenerateColorSchemeFileContent(parameters, baseColorScheme, colorScheme, templateContent, themeName, themeDisplayName);

                File.WriteAllText(themeTempFile, fileContent, Encoding.UTF8);

                Trace.WriteLine($"Comparing temp file \"{themeTempFile}\" to \"{themeFile}\"");

                if (File.Exists(themeFile) == false
                    || File.ReadAllText(themeFile) != File.ReadAllText(themeTempFile))
                {
                    File.Copy(themeTempFile, themeFile, true);

                    Trace.WriteLine($"Resource Dictionary saved to \"{themeFile}\".");
                }
                else
                {
                    Trace.WriteLine("New Resource Dictionary did not differ from existing file. No new file written.");
                }
            }
            finally
            {
                if (File.Exists(themeTempFile))
                {
                    File.Delete(themeTempFile);
                }
            }
        }

        public string GenerateColorSchemeFileContent(GeneratorParameters parameters, BaseColorScheme baseColorScheme, ColorScheme colorScheme, string templateContent, string themeName, string themeDisplayName)
        {
            templateContent = templateContent.Replace("{{ThemeName}}", themeName);
            templateContent = templateContent.Replace("{{ThemeDisplayName}}", themeDisplayName);
            templateContent = templateContent.Replace("{{BaseColorScheme}}", baseColorScheme.Name);
            templateContent = templateContent.Replace("{{ColorScheme}}", colorScheme.Name);

            foreach (var value in colorScheme.Values)
            {
                templateContent = templateContent.Replace($"{{{{{value.Key}}}}}", value.Value);
            }

            foreach (var value in baseColorScheme.Values)
            {
                templateContent = templateContent.Replace($"{{{{{value.Key}}}}}", value.Value);
            }

            foreach (var value in parameters.DefaultValues)
            {
                templateContent = templateContent.Replace($"{{{{{value.Key}}}}}", value.Value);
            }

            return templateContent;
        }
    }

    internal class GeneratorParameters
    {
        public string TemplateFile { get; set; }

        public Dictionary<string, string> DefaultValues { get; set; } = new Dictionary<string, string>();

        public BaseColorScheme[] BaseColorSchemes { get; set; }

        public ColorScheme[] ColorSchemes { get; set; }
    }

    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class BaseColorScheme
    {
        public string Name { get; set; }

        public Dictionary<string, string> Values { get; set; }
    }

    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class ColorScheme
    {
        public string Name { get; set; }

        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}