TradeFileMonitor Project
This project is a Windows application designed to monitor files in a specific directory and display their content in the user interface. The application supports CSV, TXT, and XML file formats. It monitors file changes and updates the GUI in real-time.

Main Components
1. MainWindow.xaml.cs
MainWindow.xaml.cs is the code-behind file for the main window of the application and manages user interactions.

Properties:

_fileMonitorService: The file monitoring service.
_loaderFactory: The file loader factory class.
Methods:

InitializeFileMonitorService(): Initializes the file monitoring service. It prompts the user to select a directory and starts monitoring files in that directory.
OnChangeDirectoryClick(): When the user selects a new directory, it stops monitoring the current directory and starts monitoring files in the new directory.
OnChangeIntervalClick(): Changes the monitoring interval.
OnExitClick(): Closes the application.
OnLoadFileClick(): Opens the file loading dialog, loads the selected file, and updates the GUI.
LoadDataFromFileAsync(): Asynchronously loads the file content and converts it into a list of DataRecord objects.
LoadData(): Converts the file content into a DataTable and displays it in the GUI.
ShowErrorMessage(): Displays error messages.
2. FileMonitorService.cs
FileMonitorService.cs is the class responsible for file monitoring and interval management.

Properties:

_directoryPath: The path of the directory being monitored.
_loaderFactory: The file loader factory class.
_fileListView: The ListView where the file list is displayed.
_timer: The timer managing the monitoring interval.
_fileTimestamps: A dictionary tracking the last modification timestamps of files.
Methods:

StartMonitoring(): Starts monitoring.
StopMonitoring(): Stops monitoring.
ChangeInterval(): Changes the monitoring interval.
OnTimedEvent(): Event triggered at specific intervals to check files.
UpdateFileListView(): Updates the ListView.
3. DataRecord.cs
DataRecord.cs represents the data read from files.

Properties:

Date: Date.
Open: Opening price.
High: Highest price.
Low: Lowest price.
Close: Closing price.
Volume: Trading volume.
4. Loader Classes (XmlLoader.cs, TxtLoader.cs, CsvLoader.cs)
These classes are used to load data from different file types.

XmlLoader.cs: Loads XML files and converts them into DataRecord objects.
TxtLoader.cs: Loads TXT files and converts them into DataRecord objects.
CsvLoader.cs: Loads CSV files and converts them into DataRecord objects.
5. LoaderFactory.cs
LoaderFactory.cs creates the appropriate loader class based on the file extension.

Methods:

GetLoader(): Returns the appropriate ILoader instance based on the file extension.
6. FileProcessor.cs
FileProcessor.cs manages file monitoring and updating processes.

Methods:

LoadNewFilesAsync(): Checks for and processes new files.
ProcessFileIfModifiedAsync(): Checks if a file has been modified and processes it if necessary.
ProcessFileAsync(): Processes a file and updates its content.
UpdateFileListViewAsync(): Updates the ListView.
7. FileExtensionHelper.cs
FileExtensionHelper.cs determines file extensions and converts them to the FileExtension enum.

8. FileExtension.cs
FileExtension.cs defines supported file extensions (CSV, TXT, XML, Unknown).

9. ErrorMessages.cs
ErrorMessages.cs defines and manages error messages for the application.


![Example Image](https://imgur.com/ZxUuMrj.png)



## Images

### Image 1
![Image 1](https://i.imgur.com/qoQQenf.png)

### Image 2
![Image 2](https://i.imgur.com/gcuHOfE.png)

### Image 3
![Image 3](https://i.imgur.com/qRwLjrQ.png)

### Image 4
![Image 4](https://i.imgur.com/fegOPQh.png)

### Image 5
![Image 5](https://i.imgur.com/ZXFqXmP.png)

### Image 6
![Image 6](https://i.imgur.com/SV050vk.png)

### Image 7
![Image 7](https://i.imgur.com/g6lkJUH.png)

### Image 8
![Image 8](https://i.imgur.com/E0RQLny.png)

### Image 9
![Image 9](https://i.imgur.com/vX2VWar.png)

### Image 10
![Image 10](https://i.imgur.com/0XQZVmj.png)

### Image 11
![Image 11](https://i.imgur.com/fLNax0V.png)

### Image 12
![Image 12](https://i.imgur.com/XkHUpu6.png)

### Image 13
![Image 13](https://i.imgur.com/bJkiyaN.png)

### Image 14
![Image 14](https://i.imgur.com/v9fsn7A.png)

