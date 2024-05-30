﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PeNet;
using PeNet.Header.Resource;
using SIT.Manager.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SIT.Manager.Services;

public partial class VersionService(ILogger<VersionService> logger) : IVersionService
{
    private readonly ILogger<VersionService> _logger = logger;

    [GeneratedRegex("[0]{1,}\\.[0-9]{1,2}\\.[0-9]{1,2}\\.[0-9]{1,2}\\-[0-9]{1,5}")]
    private static partial Regex EFTVersionRegex();

    [GeneratedRegex("[1]{1,}\\.[0-9]{1,2}\\.[0-9]{1,5}\\.[0-9]{1,5}")]
    private static partial Regex SITVersionRegex();

    public string GetFileProductVersionString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        // Use the first traditional / recommended method first
        string fileVersion = FileVersionInfo.GetVersionInfo(filePath).ProductVersion ?? string.Empty;

        // If the above doesn't return anything attempt to read the executable itself
        if (string.IsNullOrEmpty(fileVersion))
        {
            PeFile peHeader = new(filePath);
            StringFileInfo? stringFileInfo = peHeader.Resources?.VsVersionInfo?.StringFileInfo;
            if (stringFileInfo != null)
            {
                StringTable? fileinfoTable = stringFileInfo.StringTable.Length != 0 ? stringFileInfo.StringTable[0] : null;
                fileVersion = fileinfoTable?.ProductVersion ?? string.Empty;
            }
        }

        return fileVersion;
    }

    public string GetSptAkiVersion(string path)
    {
        // TODO fix this when installed on linux
        string filePath = Path.Combine(path, "Aki.Server.exe");
        string fileVersion = GetFileProductVersionString(filePath);
        if (string.IsNullOrEmpty(fileVersion))
        {
            _logger.LogWarning("Check SPT AKI Version: File did not exist at {filePath}", filePath);
        }
        else
        {
            _logger.LogInformation("SPT AKI Version is now {fileVersion}", fileVersion);
        }
        return fileVersion;
    }

    public string GetEFTVersion(string path)
    {
        string eftFilename = "EscapeFromTarkov.exe";

        string filePath = path;
        if (Path.GetFileName(path) != eftFilename)
        {
            filePath = Path.Combine(path, "EscapeFromTarkov.exe");
        }

        string fileVersion = GetFileProductVersionString(filePath);
        if (string.IsNullOrEmpty(fileVersion))
        {
            _logger.LogWarning("CheckEFTVersion: File did not exist at {filePath}", filePath);
        }
        else
        {
            fileVersion = EFTVersionRegex().Match(fileVersion).Value.Replace("-", ".");
            _logger.LogInformation("EFT Version is now {fileVersion}", fileVersion);
        }
        return fileVersion;
    }

    public string GetSITVersion(string path)
    {
        string filePath = Path.Combine(path, "BepInEx", "plugins", "StayInTarkov.dll");
        string fileVersion = GetFileProductVersionString(filePath);
        if (string.IsNullOrEmpty(fileVersion))
        {
            _logger.LogWarning("CheckSITVersion: File did not exist at {filePath}", filePath);
        }
        else
        {
            fileVersion = SITVersionRegex().Match(fileVersion).Value.ToString();
            _logger.LogInformation("SIT Version is now: {fileVersion}", fileVersion);
        }
        return fileVersion;
    }

    public string GetSitModVersion(string path)
    {
        string filePath = Path.Combine(path, "user", "mods", "SITCoop", "package.json");
        string fileVersion = string.Empty;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            dynamic data = JObject.Parse(json);
            try
            {
                fileVersion = data.version;
            }
            catch
            {
                fileVersion = string.Empty;
            }
        }
        return fileVersion;
    }
}
