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

public class IconConverter
{
    public void StartConvertion()
    {
        GetMaterialDesignIconsAndGeneratePackIconData();
        GetModernUIIconsAndGeneratePackIconData();
        GetFontAwesomeIconsAndGeneratePackIconData();
    }

    private void GetMaterialDesignIconsAndGeneratePackIconData()
    {
        Console.WriteLine("Downloading Material Design icon data...");
        var nameDataMaterialPairs = GetNameDataPairs(GetSourceData("https://materialdesignicons.com/api/package/38EF63D0-4744-11E4-B3CF-842B2B6CFE1B")).ToList();
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
        var nameDataModernPairs = GetNameDataPairs(GetSourceData("https://materialdesignicons.com/api/package/DFFB9B7E-C30A-11E5-A4E9-842B2B6CFE1B")).ToList();
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

        var iconTuples = new List<Tuple<string, string>>();
        foreach (var xElement in elements)
        {
            var unicode = (string)xElement.Attribute("unicode");
            var data = (string)xElement.Attribute("d");
            if (null == unicode || null == data) continue;
            FontAwesomeIconEntry iconEntry;
            if (allIconsDict.TryGetValue(unicode, out iconEntry))
            {
                var name = GetName(iconEntry.Id);
                iconTuples.Add(new Tuple<string, string>(name, data));
            }
        }
        iconTuples = iconTuples.OrderBy(t => t.Item1, StringComparer.InvariantCultureIgnoreCase).ToList();
        Console.WriteLine("Got " + iconTuples.Count + " Items");

        Console.WriteLine("Updating PackIconFontAwesomeKind...");
        var newEnumSource = UpdatePackIconKind("PackIconFontAwesomeKind.template.cs", iconTuples);
        Write(newEnumSource, "PackIconFontAwesomeKind.cs");
        Console.WriteLine("Updating PackIconFontAwesomeDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconFontAwesomeDataFactory.template.cs", "PackIconFontAwesomeKind", iconTuples);
        Write(newDataFactorySource, "PackIconFontAwesomeDataFactory.cs");

        Console.WriteLine("FontAwesome done!");
        Console.WriteLine();
    }

    private IEnumerable<Tuple<string, string>> GetNameDataPairs(string sourceData)
    {
        var jObject = JObject.Parse(sourceData);
        return jObject["icons"].Select(t => GetNameDataPair(GetName(t["name"].ToString()), t["data"].ToString().Trim()));
    }

    private Tuple<string, string> GetNameDataPair(string name, string data)
    {
        /*if (string.IsNullOrEmpty(data))
        {
            Console.Write("Uuuups, found empty data part for: '{0}'", name);
            if (name == "ControlStop") data = "M0,0 L28.5,0 L28.5,28.5 L0,28.5 z";
            else if (name == "DebugStop") data = "M0,0 L28,0 L28,28 L0,28 z";
            else if (name == "MoonFull") data = "M44.3278,22.1639 C44.3278,34.404684 34.404684,44.3278 22.1639,44.3278 C9.923116,44.3278 0,34.404684 0,22.1639 C0,9.923116 9.923116,0 22.1639,0 C34.404684,0 44.3278,9.923116 44.3278,22.1639 z";
            if (string.IsNullOrEmpty(data))
            {
                Console.Write(" ...and can not be used!!!");
            }
            else
            {
                Console.Write(" ...and can set to '{0}'", data);
            }
            Console.WriteLine();
        }*/
        return new Tuple<string, string>(name, data);
    }

    private IEnumerable<Tuple<string, string>> GetNameDataOldModernPairs(string sourceData)
    {
        var jToken = JToken.Parse(sourceData);
        return jToken.Children()
                     .Select(t => new Tuple<string, string>(GetName(t["name"].ToString()).Replace(".", "-").Underscore().Pascalize(),
                                                            GetSvgData(GetName(t["name"].ToString()).Replace(".", "-").Underscore().Pascalize(), t["svg"].ToString())));
    }

    private string GetName(string name)
    {
        var oldname = name;
        if (name.EndsWith("-o") || name.Contains("-o-"))
        {
            name = name.Replace("-o", "-outline");
        }
        name = name.Underscore().Pascalize();
        if (name.Length > 0 && Char.IsNumber(name[0]))
        {
            return '_' + name;
        }
        //Console.WriteLine("Converting name '{0}' -> '{1}", oldname, name);
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

    private string UpdatePackIconKind(string sourceFile, IEnumerable<Tuple<string, string>> nameDataPairs)
    {
        // line 15
        var allLines = File.ReadAllLines(sourceFile).ToList();
        allLines.InsertRange(15, nameDataPairs.Where(t => !string.IsNullOrEmpty(t.Item2)).Select(t => string.Format("        {0},", t.Item1)).ToArray());
        return string.Join(Environment.NewLine, allLines);
    }

    private string UpdatePackIconDataFactory(string sourceFile, string enumKind, IEnumerable<Tuple<string, string>> nameDataPairs)
    {
        // { PackIconMaterialKind.AutoGenerated, "data in here" },
        // line 12
        var allLines = File.ReadAllLines(sourceFile).ToList();
        //var insert = string.Join(",", nameDataPairs.Select(t => string.Format("{{{0}.{1}, \"{2}\"}}", enumKind, t.Item1, t.Item2)).ToArray());
        //allLines.Insert(12, insert);
        allLines.InsertRange(14, nameDataPairs.Where(t => !string.IsNullOrEmpty(t.Item2)).Select(t => string.Format("                       {{{0}.{1}, \"{2}\"}},", enumKind, t.Item1, t.Item2)).ToArray());
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
            var iconData = sr.ReadToEnd();
            //Console.WriteLine("Got.");
            return iconData;
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
