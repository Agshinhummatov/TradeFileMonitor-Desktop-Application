using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeFileMonitor.Constants
{
    public static class FileLoaderErrors
    {
        public const string EmptyFileMessage = "File is empty.";
        public const string InvalidHeaderMessage = "File header does not match expected format.";
        public const string ParsingErrorMessage = "Error parsing line";
        public const string UnsupportedFileTypeMessage = "File type not supported.";
        public const string NoDirectorySelectedMessage = "No directory selected. The application will now close.";
        public const string UnexpectedErrorMessage = "Unexpected error: {0}";
        public const string InvalidIntervalMessage = "Please enter a valid interval value.";
        public const string ErrorLoadingFileMessage = "Error loading file: {0}";
    }
}
