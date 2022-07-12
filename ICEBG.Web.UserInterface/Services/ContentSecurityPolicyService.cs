using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ICEBG.Web.UserInterface.Services;

public class ContentSecurityPolicyService
{
    /// <summary>
    /// Part of the CSP <c>script-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public string ScriptSrcPart { get; private set; } = $"'self'";


    /// <summary>
    /// Part of the CSP <c>style-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public string StyleSrcPart { get; private set; } = $"'self'";


    public readonly bool ApplyContentSecurityPolicy = true;


    // Delimiters are for Linux and Windows respectively.
    private static readonly char[] kPathDelimiters = { '/', '\\' };

    private readonly Dictionary<string, string> fileHashes = new();

    public ContentSecurityPolicyService(IWebHostEnvironment env)
    {
        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        string scriptSrcPart = "";
        string styleSrcPart = "";

        if (File.Exists(hashesFilePath) && !fileHashes.Any())
        {
            using StreamReader sr = new(hashesFilePath);

            while (sr.Peek() >= 0)
            {
                var csvSplit = (sr.ReadLine() ?? ",").Split(',');

                var fileName = csvSplit[0].Split('\\')[^1];
                var extension = csvSplit[0].Split('.')[^1].ToLower();
                var hashString = $"sha256-{csvSplit[1]}";

                fileHashes[fileName] = hashString;

                if (extension == "js")
                {
                    scriptSrcPart += $"'sha256-{csvSplit[1]}' ";
                }
                else if (extension == "css")
                {
                    styleSrcPart += $"'sha256-{csvSplit[1]}' ";
                }
            }
        }

        ApplyContentSecurityPolicy = File.Exists(hashesFilePath) || !env.IsDevelopment();

        ScriptSrcPart = scriptSrcPart.Trim();
        StyleSrcPart = styleSrcPart.Trim();
    }


    /// <summary>
    /// Returns the hash string for a given file name to be used in a script of style's <c>integrity="[value]"</c> tag.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetFileHashString(string fileName)
    {
        if (fileHashes.TryGetValue(fileName, out var hash))
        {
            return hash;
        }

        return "";
    }
}
