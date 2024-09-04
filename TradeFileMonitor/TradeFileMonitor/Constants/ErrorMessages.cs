using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeFileMonitor.Constants
{
    public static class ErrorMessages
    {
        // General Error Messages
        public const string FileTypeNotSupported = "File type not supported.";
        public const string FileLoadFailed = "Failed to load file: {0}";
        public const string FileProcessingError = "Error processing file: {0}";
        public const string FileListViewUpdateError = "Error updating FileListView";
        public const string LoadNewFilesError = "Error in LoadNewFilesAsync";

        // Error Messages - XML ​​Loader
        public const string XmlLoadFailed = "Failed to load XML file: {0}";

        // Error Messages - TXT Loader
        public const string TxtLoadFailed = "Failed to load TXT file: {0}";

        // Error Messages - CSV Loader
        public const string CsvFileEmpty = "CSV file is empty.";
        public const string CsvHeaderMismatch = "CSV file header does not match expected format. Expected {0} columns, but got {1}.";
        public const string CsvRowMismatch = "CSV file row does not match expected format.";
        public const string CsvParsingError = "Error parsing line: {0}";

        // Error Messages - File Monitor Service
        public const string IntervalMustBePositive = "Interval must be greater than zero.";
    }
}
