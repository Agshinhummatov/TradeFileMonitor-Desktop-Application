using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Models;
using TradeFileMonitor.Services;

namespace TradeFileMonitor
{
    public partial class MainWindow : System.Windows.Window
    {
        private FileMonitorService _fileMonitorService;
        private LoaderFactory _loaderFactory; 

        public MainWindow()
        {
            InitializeComponent();
            _loaderFactory = new LoaderFactory(); 
            string csvDirectoryPath = @"C:\Users\akshin.hummatov\Desktop\CSVFiles";
            _fileMonitorService = new FileMonitorService(csvDirectoryPath, _loaderFactory, fileListView);
        }

        private void OnChangeDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                try
                {
                    _fileMonitorService.StopMonitoring(); 
                    _fileMonitorService = new FileMonitorService(selectedPath, _loaderFactory, fileListView);
                    _fileMonitorService.StartMonitoring();
                }
                catch (ArgumentException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnChangeIntervalClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(intervalTextBox.Text, out int newInterval) && newInterval > 0)
            {
                _fileMonitorService.ChangeInterval(newInterval);
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter a valid range.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    DataTable dataTable = LoadData(filePath);
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    // Dosyayı fileListView'e de ekleyin
                    var dataRecords = LoadDataFromFile(filePath);
                    _fileMonitorService.UpdateFileListView(dataRecords);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<DataRecord> LoadDataFromFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var loader = _loaderFactory.GetLoader(extension);
            return loader?.Load(filePath)?.ToList() ?? new List<DataRecord>();
        }

        private DataTable LoadData(string filePath)
        {
            var dataTable = new DataTable();
            var fileExtension = Path.GetExtension(filePath).ToLower();

            if (fileExtension == ".csv")
            {
                using (var reader = new StreamReader(filePath))
                {
                    var headers = reader.ReadLine().Split(',');
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine().Split(',');
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (fileExtension == ".txt")
            {
                using (var reader = new StreamReader(filePath))
                {
                    var headers = reader.ReadLine().Split(';');
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine().Split(';');
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (fileExtension == ".xml")
            {
                var xdoc = XDocument.Load(filePath);
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Open");
                dataTable.Columns.Add("High");
                dataTable.Columns.Add("Low");
                dataTable.Columns.Add("Close");
                dataTable.Columns.Add("Volume");

                var dataRecords = xdoc.Descendants("value").Select(node =>
                {
                    var row = dataTable.NewRow();
                    row["Date"] = node.Attribute("date")?.Value;
                    row["Open"] = node.Attribute("open")?.Value;
                    row["High"] = node.Attribute("high")?.Value;
                    row["Low"] = node.Attribute("low")?.Value;
                    row["Close"] = node.Attribute("close")?.Value;
                    row["Volume"] = node.Attribute("volume")?.Value;
                    return row;
                });

                foreach (var row in dataRecords)
                {
                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                throw new NotSupportedException("File type not supported.");
            }

            return dataTable;
        }

        private void intervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(intervalTextBox.Text, out int interval) && interval > 0)
            {
                _fileMonitorService.ChangeInterval(interval);
            }
            else
            {
                System.Windows.MessageBox.Show("Interval must be a positive number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


}
