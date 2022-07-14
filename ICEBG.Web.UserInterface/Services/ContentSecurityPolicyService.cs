using System.Collections.Generic;
using System.IO;
using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace ICEBG.Web.UserInterface.Services;

/// <summary>
/// Manages static asset SHA hashes for use with CSP.
/// Temporarily using a nonce for blazor.server.js
/// </summary>
public class ContentSecurityPolicyService
{
    /// <summary>
    /// The Scoped nonce value.
    /// </summary>
    public readonly string nonceValue = "";


    /// <summary>
    /// Formatted nonce string.
    /// </summary>
    public string nonceString => $"'nonce-{nonceValue}'";


    /// <summary>
    /// Part of the CSP <c>script-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public readonly string scriptSrcPart = "'self'";


    /// <summary>
    /// Part of the CSP <c>style-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public readonly string styleSrcPart = "'self'";


    /// <summary>
    /// The CSP is to be applied only if this is true.
    /// </summary>
    public readonly bool applyContentSecurityPolicy = true;


    // Delimiters are for Linux and Windows respectively.
    private static readonly char[] kPathDelimiters = { '/', '\\' };

    private readonly Dictionary<string, string> fileHashes = new();


    public ContentSecurityPolicyService(IWebHostEnvironment env)
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        nonceValue = Convert.ToBase64String(bytes);

        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        string scriptSrcPart = "";
        string styleSrcPart = "";

        if (File.Exists(hashesFilePath) && !fileHashes.Any())
        {
            using StreamReader sr = new(hashesFilePath);

            while (sr.Peek() >= 0)
            {
                var csvSplit = (sr.ReadLine() ?? ",").Split(',');

                var fileName = csvSplit[0].Split(kPathDelimiters)[^1];
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

        applyContentSecurityPolicy = File.Exists(hashesFilePath) || !env.IsDevelopment();

        this.scriptSrcPart = (nonceString + " " + scriptSrcPart).Trim();
        this.styleSrcPart = styleSrcPart.Trim();
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
