using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TradeFileMonitor.Constants;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Helpers
{
    public static class FileProcessor
    {
        public static async Task LoadNewFilesAsync(string directoryPath, ILoaderFactory loaderFactory, ListView fileListView, ConcurrentDictionary<string, DateTime> fileTimestamps)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath);
                var tasks = files.Select(file => ProcessFileIfModifiedAsync(file, loaderFactory, fileListView, fileTimestamps));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                HandleError(ex, ErrorMessages.LoadNewFilesError);
            }
        }

        private static async Task ProcessFileIfModifiedAsync(string file, ILoaderFactory loaderFactory, ListView fileListView, ConcurrentDictionary<string, DateTime> fileTimestamps)
        {
            var lastModified = File.GetLastWriteTime(file);

            if (fileTimestamps.TryGetValue(file, out var previousModified) && lastModified <= previousModified)
            {
                return; // Skip if file has not changed
            }

            fileTimestamps[file] = lastModified;
            await ProcessFileAsync(file, loaderFactory, fileListView);
        }

        private static async Task ProcessFileAsync(string file, ILoaderFactory loaderFactory, ListView fileListView)
        {
            try
            {
                var dataRecords = await LoadDataFromFileAsync(file, loaderFactory);

                if (dataRecords.Any())
                {
                    await UpdateFileListViewAsync(dataRecords, fileListView);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, string.Format(ErrorMessages.FileProcessingError, file));
            }
        }

        private static async Task<IEnumerable<DataRecord>> LoadDataFromFileAsync(string filePath, ILoaderFactory loaderFactory)
        {
            var extension = Path.GetExtension(filePath);
            var fileExtension = FileExtensionHelper.ParseFileExtension(extension);
            var loader = loaderFactory.GetLoader(fileExtension);

            try
            {
                var dataRecords = await loader.LoadAsync(filePath);
                return dataRecords.ToList();
            }
            catch (Exception ex)
            {
                HandleError(ex, string.Format(ErrorMessages.FileLoadFailed, filePath));
                throw;
            }
        }

        public static async Task UpdateFileListViewAsync(IEnumerable<DataRecord> dataRecords, ListView fileListView)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    fileListView.Items.Clear();
                    foreach (var record in dataRecords)
                    {
                        fileListView.Items.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex, ErrorMessages.FileListViewUpdateError);
                }
            });
        }

        private static void HandleError(Exception ex, string message)
        {
            // You can use a logging framework here instead of Console
            Console.WriteLine($"{message}: {ex.Message}");
        }
    }
}
