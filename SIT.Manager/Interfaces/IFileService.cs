﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIT.Manager.Services;

public interface IFileService
{
    Task CopyDirectory(string source, string destination, IProgress<double>? progress = null);
    Task CopyFileAsync(string source, string destination, CancellationToken cancellationToken = default);
    /// <summary>
    /// Downloads a file and report progress if enabled
    /// </summary>
    /// <param name="fileName">The name of the file to be downloaded.</param>
    /// <param name="filePath">The path (not including the filename) to download to.</param>
    /// <param name="fileUrl">The URL to download from.</param>
    /// <param name="progress">Report progress of the download</param>
    /// <returns></returns>
    Task<bool> DownloadFile(string fileName, string filePath, string fileUrl, IProgress<double> progress);
    /// <summary>
    /// Extracts a Zip archive
    /// </summary>
    /// <param name="filePath">The file to extract</param>
    /// <param name="destination">The destination to extract to</param>
    /// <param name="progress">Optional report progress of the archive extraction</param>
    /// <returns></returns>
    Task ExtractArchive(string filePath, string destination, IProgress<double>? progress = null);
    /// <summary>
    /// Open the system file manager at the path requested, if the directory doesn't exist then do nothing
    /// </summary>
    /// <param name="path">Path to open file manager at</param>
    Task OpenDirectoryAsync(string path);
    /// <summary>
    /// Open the requested file in the default system handler, if the file doesn't exist do nothing.
    /// </summary>
    /// <param name="path">The path of the file to open</param>
    Task OpenFileAsync(string path);
    /// <summary>
    /// Ensure that the file at the target path has executable permissions
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns></returns>
    Task SetFileAsExecutable(string filePath);
}
