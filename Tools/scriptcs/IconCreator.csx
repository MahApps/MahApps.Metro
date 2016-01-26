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

Console.WriteLine("Convert Material Design Icons to MahApps.Metro PackIconMaterialKind");

public class IconConverter
{
    public void StartConvertion()
    {
        Console.WriteLine("Downloading Material Design icon data...");
        var nameDataMaterialPairs = GetNameDataPairs(GetSourceData("https://materialdesignicons.com/api/package/38EF63D0-4744-11E4-B3CF-842B2B6CFE1B")).ToList();
        Console.WriteLine("Items: " + nameDataMaterialPairs.Count);

        Console.WriteLine("Downloading Modern UI icon data...");
        var nameDataModernPairs = GetNameDataPairs(GetSourceData("https://materialdesignicons.com/api/package/DFFB9B7E-C30A-11E5-A4E9-842B2B6CFE1B")).ToList();
        var nameDataOldModernPairs = GetNameDataOldModernPairs(GetSourceData("http://modernuiicons.com/icons/package")).ToList();
        Console.WriteLine("Items: " + nameDataModernPairs.Count);

        Console.WriteLine("Updating PackIconMaterialKind...");
        var newEnumSource = UpdatePackIconKind("PackIconMaterialKind.template.cs", nameDataMaterialPairs);
        Write(newEnumSource, "PackIconMaterialKind.cs");
        Console.WriteLine("Updating PackIconMaterialDataFactory...");
        var newDataFactorySource = UpdatePackIconDataFactory("PackIconMaterialDataFactory.template.cs", "PackIconMaterialKind", nameDataMaterialPairs);
        Write(newDataFactorySource, "PackIconMaterialDataFactory.cs");

        Console.WriteLine("Updating PackIconModernKind...");
        newEnumSource = UpdatePackIconKind("PackIconModernKind.template.cs", nameDataModernPairs);
        Write(newEnumSource, "PackIconModernKind.cs");
        Console.WriteLine("Updating PackIconModernDataFactory...");
        newDataFactorySource = UpdatePackIconDataFactory("PackIconModernDataFactory.template.cs", "PackIconModernKind", nameDataModernPairs);
        Write(newDataFactorySource, "PackIconModernDataFactory.cs");

        Console.WriteLine("Done!");
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
            Console.WriteLine("Got.");
            return iconData;
        }
    }

    private void Write(string content, string filename)
    {
        File.WriteAllText(Path.Combine(@"..\..\MahApps.Metro\Controls\IconPacks", filename), content, Encoding.UTF8);
    }
}

var iconConverter = new IconConverter();
iconConverter.StartConvertion();

Console.WriteLine("...finished");
