using System;
using System.IO;

namespace GDExtensionGenerator
{
    public static class Program
    {
        private static int Main(string[] args)
        {
            string outputDirectory = null, apiFile = null;
            int parseType = 0;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-a":
                    case "--api":
                        if (i == args.Length - 1)
                            break;
                        apiFile = args[++i];
                        break;
                    case "-o":
                    case "--output":
                        if (i == args.Length - 1)
                            break;
                        outputDirectory = args[++i];
                        break;
                    case "-h":
                    case "--header":
                        parseType = 1;
                        break;
                    case "-v":
                    case "--variant":
                        parseType = 2;
                        break;
                }
            }

            outputDirectory = Path.GetFullPath(outputDirectory);

            if (outputDirectory == null)
                return Error("Missing output directory. Please use the `-o` argument.");

            if (parseType < 2)
            {
                apiFile = Path.GetFullPath(apiFile);

                if (apiFile == null || (!apiFile.EndsWith(parseType == 1 ? "gdextension_interface.h" : "extension_api.json")))
                {
                    if (parseType != 1)
                        return Error("Missing `extension_api.json` file. Please use the `-a` argument.");
                    else
                        return Error("Missing `gdextension_interface.h`. Please use the `-a` argument.");
                }

                if (!File.Exists(apiFile))
                    return Error($"The api file '{apiFile}' does not exist.");
            }

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            Directory.CreateDirectory(outputDirectory);

            if (parseType < 2)
                Console.WriteLine($"Parsing '{apiFile}'...");
            using FileStream fs = parseType < 2 ? File.OpenRead(apiFile) : null;
            using StreamReader r = fs != null ? new StreamReader(fs) : null;

            return parseType switch
            {
                0 => JsonGenerator.GenerateApi(r, outputDirectory),
                1 => HeaderGenerator.GenerateApi(r, outputDirectory),
                2 => VariantGenerator.GenerateApi(outputDirectory),
                _ => throw new NotImplementedException("I don't know how you managed this.")
            };
        }
    }
}