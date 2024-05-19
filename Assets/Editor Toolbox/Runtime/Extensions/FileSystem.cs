using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides extension methods for file system operations.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Combines and expands the specified file paths.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns>The combined and expanded file path.</returns>
        public static string CombineExpandPaths(string path1, string path2)
        {
            return Path.GetFullPath(Path.Combine(path1, path2));
        }

        /// <summary>
        /// Gets the editor project path.
        /// </summary>
        public static readonly string EditorProjectPath = Application.dataPath;

        /// <summary>
        /// Gets the Unity project temporary folder path.
        /// </summary>
        public static readonly string UnityProjectTempFolder = Application.temporaryCachePath;

        /// <summary>
        /// Gets the user profile path.
        /// </summary>
        public static string UserPath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        /// <summary>
        /// Gets the desktop path.
        /// </summary>
        public static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// Retrieves the most recent file in the specified folder.
        /// </summary>
        /// <param name="folderPath">The path of the folder.</param>
        /// <returns>The file path of the most recent file, or null if no files are found or an error occurs.</returns>
        public static string MostRecentFile(string folderPath)
        {
            try
            {
                var files = Directory.GetFiles(folderPath);
                var mostRecentFile = files.OrderByDescending(File.GetLastWriteTime).FirstOrDefault();
                return mostRecentFile;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error retrieving most recent file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cleans the specified directory by deleting and recreating it.
        /// </summary>
        /// <param name="folderPath">The path of the directory to clean.</param>
        public static void CleanDirectory(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    Thread.Sleep(100); // Recursive deletes seem to happen async... better way to do this..?
                }

                Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error cleaning directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Copies a directory and its contents to a new location.
        /// </summary>
        /// <param name="sourceDirectory">The path of the source directory.</param>
        /// <param name="destinationDirectory">The path of the destination directory.</param>
        /// <param name="overwrite">Specifies whether to overwrite existing files.</param>
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = false)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Debug.LogError($"Source directory does not exist: {sourceDirectory}");
                    return;
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                var files = Directory.GetFiles(sourceDirectory);
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var destFilePath = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destFilePath, overwrite);
                }

                var directories = Directory.GetDirectories(sourceDirectory);
                foreach (var directory in directories)
                {
                    var dirName = Path.GetFileName(directory);
                    var destDirPath = Path.Combine(destinationDirectory, dirName);
                    CopyDirectory(directory, destDirPath, overwrite);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error copying directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Moves a directory and its contents to a new location.
        /// </summary>
        /// <param name="sourceDirectory">The path of the source directory.</param>
        /// <param name="destinationDirectory">The path of the destination directory.</param>
        public static void MoveDirectory(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Debug.LogError($"Source directory does not exist: {sourceDirectory}");
                    return;
                }

                if (Directory.Exists(destinationDirectory))
                {
                    CleanDirectory(destinationDirectory);
                }

                Directory.Move(sourceDirectory, destinationDirectory);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error moving directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the specified directory if it exists.
        /// </summary>
        /// <param name="folderPath">The path of the directory to delete.</param>
        public static void DeleteDirectoryIfExists(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deleting directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the specified directory if it does not exist.
        /// </summary>
        /// <param name="folderPath">The path of the directory to create.</param>
        public static void CreateDirectoryIfNotExists(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        public static bool FileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error checking file existence: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified file if it exists.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        public static void DeleteFileIfExists(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deleting file: {ex.Message}");
            }
        }
    }
}