using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Humanizer;
using Newtonsoft.Json.Linq;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class FontAwesomeIcons
{
    public List<FontAwesomeIconEntry> Icons { get; set; }
}

public class FontAwesomeIconEntry
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Unicode { get; set; }
    public string Created { get; set; }
    public List<string> Aliases { get; set; }
    public List<string> Filter { get; set; }
    public List<string> Categories { get; set; }
}

public class PackIconData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Data { get; set; }
}

public class IconConverter
{
    public void StartConvertion()
    {
        GetMaterialDesignIconsAndGeneratePackIconData();
        GetModernUIIconsAndGeneratePackIconData();
        GetFontAwesomeIconsAndGeneratePackIconData();
        GetEntypoIconsAndGeneratePackIconData();
    }

    private void GetMaterialDesignIconsAndGeneratePackIconData()
    {
        Console.WriteLine("Downloading Material Design icon data...");
        var nameDataMaterialPairs = GetPackIconData(GetSourceData("https://materialdesignicons.com/api/package/38EF63D0-4744-11E4-B3CF-842B2B6CFE1B")).ToList();
        Console.WriteLine("Got "  + nameDataMaterialPairs.Count + " Items");

        Console.WriteLine("Updating PackIconMaterialKind...");
        var newEnumSource = UpdatePackIconKind("PackIconMaterialKind.template.cs", nameDataMaterialPairs);
        Write(newEnumSource, "PackIconMaterialKind.cs");
        Console.WriteLine("Updating PackIconMaterialDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconMaterialDataFactory.template.cs", "PackIconMaterialKind", nameDataMaterialPairs);
        Write(newDataFactorySource, "PackIconMaterialDataFactory.cs");

        Console.WriteLine("Material Design done!");
        Console.WriteLine();
    }

    private void GetModernUIIconsAndGeneratePackIconData()
    {
        Console.WriteLine("Downloading Modern UI icon data...");
        var nameDataModernPairs = GetPackIconData(GetSourceData("https://materialdesignicons.com/api/package/DFFB9B7E-C30A-11E5-A4E9-842B2B6CFE1B")).ToList();
        var nameDataOldModernPairs = GetNameDataOldModernPairs(GetSourceData("http://modernuiicons.com/icons/package")).ToList();
        Console.WriteLine("Got " + nameDataModernPairs.Count + " Items");

        Console.WriteLine("Updating PackIconModernKind...");
        var newEnumSource = UpdatePackIconKind("PackIconModernKind.template.cs", nameDataModernPairs);
        Write(newEnumSource, "PackIconModernKind.cs");
        Console.WriteLine("Updating PackIconModernDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconModernDataFactory.template.cs", "PackIconModernKind", nameDataModernPairs);
        Write(newDataFactorySource, "PackIconModernDataFactory.cs");

        Console.WriteLine("Modern UI done!");
        Console.WriteLine();
    }

    private void GetFontAwesomeIconsAndGeneratePackIconData()
    {
        Console.WriteLine("Downloading FontAwesome icon data...");

        var faSVG = "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/master/src/assets/font-awesome/fonts/fontawesome-webfont.svg";
        var faIcons = "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/master/src/icons.yml";

        var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);
        //FontAwesomeConfig config = deserializer.Deserialize<FontAwesomeConfig>(GetSourceStream(faRoot + "_config.yml"));
        FontAwesomeIcons allIcons = deserializer.Deserialize<FontAwesomeIcons>(GetSourceStream(faIcons));
        if (null == allIcons || allIcons.Icons.Count <= 0) {
            Console.WriteLine("Could not find any Font-Awesome icon!");
            return;
        }
        var allIconsDict = allIcons.Icons.ToDictionary(i => i.Unicode, i => i);
        Console.WriteLine("Found " + allIconsDict.Count + " icons");

        var svgStream = GetSourceData(faSVG)
            .Replace("\"&#x", "\"")
            .Replace(";\"", "\"");

        var xmlDoc = XDocument.Parse(svgStream);
        var elements = xmlDoc
            .Root
            .Elements("{http://www.w3.org/2000/svg}defs")
            .Elements("{http://www.w3.org/2000/svg}font")
            .Elements()
            .ToList();

        var iconDataList = new List<PackIconData>();
        foreach (var xElement in elements)
        {
            var unicode = (string)xElement.Attribute("unicode");
            var data = (string)xElement.Attribute("d");
            if (null == unicode || null == data) continue;
            FontAwesomeIconEntry iconEntry;
            if (allIconsDict.TryGetValue(unicode, out iconEntry))
            {
                var name = GetName(iconEntry.Id, true);
                var aliases = string.Empty;
                if (iconEntry.Aliases != null && iconEntry.Aliases.Count > 0)
                {
                    aliases = string.Format(" ({0})", string.Join(", ", iconEntry.Aliases.Select(a => GetName(a, true))));
                }
                iconDataList.Add(new PackIconData() { Name = name, Description = iconEntry.Name + aliases, Data = data });
            }
        }
        iconDataList = iconDataList.OrderBy(d => d.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
        Console.WriteLine("Got " + iconDataList.Count + " Items");

        Console.WriteLine("Updating PackIconFontAwesomeKind...");
        var newEnumSource = UpdatePackIconKind("PackIconFontAwesomeKind.template.cs", iconDataList);
        Write(newEnumSource, "PackIconFontAwesomeKind.cs");
        Console.WriteLine("Updating PackIconFontAwesomeDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconFontAwesomeDataFactory.template.cs", "PackIconFontAwesomeKind", iconDataList);
        Write(newDataFactorySource, "PackIconFontAwesomeDataFactory.cs");

        Console.WriteLine("FontAwesome done!");
        Console.WriteLine();
    }

    public void GetEntypoIconsAndGeneratePackIconData()
    {
    	Console.WriteLine("Create Entypo+ icon data...");

    	var iconSourceFolder = ".\\Entypo+";
    	var allSVGFiles = new List<string>();
    	ProcessDirectory(iconSourceFolder, allSVGFiles);
    	Console.WriteLine("Found " + allSVGFiles.Count + " icons");

    	var iconDataList = new List<PackIconData>();
    	foreach (var fileName in allSVGFiles)
    	{
    		var svgData = File.ReadAllText(fileName);
    		var xmlDoc = XDocument.Parse(svgData);
    		var id = Path.GetFileNameWithoutExtension(fileName);
    		var name = GetName(id, true);
    		//var id = ((string)xmlDoc.Root.Attribute("id")).Pascalize().Pascalize().Underscore().Dasherize();
	        var paths = xmlDoc.Root.Descendants("{http://www.w3.org/2000/svg}path");
	        if (paths.Count() > 1)
	        {
	        	Console.WriteLine("Too many path data in " + name + " -> " + id);
	        	continue;
	        }
	        var data = (string)paths.First().Attribute("d");
	        if (data == null)
	        {
	        	Console.WriteLine("No path for " + name + " -> " + id);
	        }
	        if (fileName.Contains("Entypo+ Social Extension"))
	        {
	        	id = id + " (Social Extension)";
	        }
	        iconDataList.Add(new PackIconData() { Name = name, Description = id, Data = data });
    	}
        iconDataList = iconDataList.OrderBy(d => d.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
        Console.WriteLine("Got " + iconDataList.Count + " Items");

        Console.WriteLine("Updating PackIconEntypoKind...");
        var newEnumSource = UpdatePackIconKind("PackIconEntypoKind.template.cs", iconDataList);
        Write(newEnumSource, "PackIconEntypoKind.cs");
        Console.WriteLine("Updating PackIconEntypoDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconEntypoDataFactory.template.cs", "PackIconEntypoKind", iconDataList);
        Write(newDataFactorySource, "PackIconEntypoDataFactory.cs");

    	Console.WriteLine("Entypo+ done!");
        Console.WriteLine();
    }

    public static void ProcessDirectory(string targetDirectory, IList<string> allFiles) 
    {
        // Process the list of files found in the directory.
        string [] fileEntries = Directory.GetFiles(targetDirectory);
        foreach(string fileName in fileEntries)
        {
        	var extension = Path.GetExtension(fileName).ToLower();
        	if (extension != ".svg") continue;
        	allFiles.Add(fileName);
        }
        // Recurse into subdirectories of this directory.
        string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach(string subdirectory in subdirectoryEntries)
        {
            ProcessDirectory(subdirectory, allFiles);
        }
    }

    private IEnumerable<PackIconData> GetPackIconData(string sourceData)
    {
        var jObject = JObject.Parse(sourceData);
        return jObject["icons"].Select(t => {
                var aliases = string.Join(", ", t["aliases"].Values<string>().Select(a => GetName(a)));
                if (!string.IsNullOrEmpty(aliases))
                {
                    aliases = string.Format(" ({0})", aliases);
                }
                return new PackIconData() { Name = GetName(t["name"].ToString()), Description = t["name"].ToString() + aliases, Data = t["data"].ToString().Trim() };
            });
    }

    private IEnumerable<Tuple<string, string>> GetNameDataOldModernPairs(string sourceData)
    {
        var jToken = JToken.Parse(sourceData);
        return jToken.Children()
                     .Select(t => new Tuple<string, string>(GetName(t["name"].ToString()).Replace(".", "-").Underscore().Pascalize(),
                                                            GetSvgData(GetName(t["name"].ToString()).Replace(".", "-").Underscore().Pascalize(), t["svg"].ToString())));
    }

    private string GetName(string name, bool replaceOutline = false)
    {
        if (replaceOutline && (name.EndsWith("-o") || name.Contains("-o-")))
        {
            name = name.Replace("-o", "-outline");
        }
        name = name.Replace("+", "Plus").Replace("%", "Percent").Underscore().Pascalize();
        if (name.Length > 0 && Char.IsNumber(name[0]))
        {
            return '_' + name;
        }
        return name;
    }

    private string GetSvgData(string name, string svg)
    {
        svg = string.Format("<svg>{0}</svg>", svg);
        //Console.WriteLine("Try get data for {0}...", name);
        //Console.WriteLine(svg);

        // Encode the XML string in a UTF-8 byte array
        byte[] encodedString = Encoding.UTF8.GetBytes(svg);

        // Put the byte array into a stream and rewind it to the beginning
        MemoryStream ms = new MemoryStream(encodedString);
        ms.Flush();
        ms.Position = 0;

        // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
        var xmlDoc = XDocument.Load(ms);
        var elements = xmlDoc.Element("svg").Elements().ToList();
        if (elements.Count > 1)
        {
            Console.WriteLine("Uuuups, can not use path data for {0}", name);
            return string.Empty;
        }

        var data = (string)(elements.First().Attribute("d"));
        if (string.IsNullOrEmpty(data))
        {
            Console.WriteLine("Uuuups, can not use path data for {0}", name);
            //Console.WriteLine("Data {0}", data);
            return string.Empty;
        }

        return data;
    }

    private string UpdatePackIconKind(string sourceFile, IEnumerable<PackIconData> iconDataList)
    {
        // line 17
        var allLines = File.ReadAllLines(sourceFile).ToList();
        allLines.InsertRange(17, iconDataList.Where(d => !string.IsNullOrEmpty(d.Name) && !string.IsNullOrEmpty(d.Data))
                                             .Select(d => string.Format("        [Description(\"{0}\")] {1},", d.Description, d.Name)).ToArray());
        return string.Join(Environment.NewLine, allLines);
    }

    private string UpdatePackIconDataFactory(string sourceFile, string enumKind, IEnumerable<PackIconData> iconDataList)
    {
        // line 14
        var allLines = File.ReadAllLines(sourceFile).ToList();
        allLines.InsertRange(14, iconDataList.Where(d => !string.IsNullOrEmpty(d.Name) && !string.IsNullOrEmpty(d.Data))
                                             .Select(d => string.Format("                       {{{0}.{1}, \"{2}\"}},", enumKind, d.Name, d.Data)).ToArray());
        return string.Join(Environment.NewLine, allLines);
    }

    private string GetSourceData(string url)
    {
        var webRequest = WebRequest.CreateDefault(new Uri(url));
        webRequest.Credentials = CredentialCache.DefaultCredentials;
        if (webRequest.Proxy != null)
        {
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
        }
        using (var sr = new StreamReader(webRequest.GetResponse().GetResponseStream()))
        {
            return sr.ReadToEnd();
        }
    }

    private StreamReader GetSourceStream(string url)
    {
        var webRequest = WebRequest.CreateDefault(new Uri(url));
        webRequest.Credentials = CredentialCache.DefaultCredentials;
        if (webRequest.Proxy != null)
        {
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
        }
        return new StreamReader(webRequest.GetResponse().GetResponseStream());
    }

    private void Write(string content, string filename)
    {
        File.WriteAllText(Path.Combine(@"..\..\MahApps.Metro\Controls\IconPacks", filename), content, Encoding.UTF8);
    }
}

Console.WriteLine("Magic icon converter startet...");

var iconConverter = new IconConverter();
iconConverter.StartConvertion();
Console.WriteLine();

Console.WriteLine("...finished.");