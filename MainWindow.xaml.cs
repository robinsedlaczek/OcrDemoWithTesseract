using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Ocr.Annotations;
using Tesseract;
using Rect = Tesseract.Rect;

namespace Ocr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _lastResult;
        private string _lastError;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string Error
        {
            get
            {
                return _lastError;
            }

            set
            {
                if (_lastError != value)
                {
                    _lastError = value;

                    OnPropertyChanged();
                }
            }
        }

        public string Result
        {
            get
            {
                return _lastResult; 
            }

            set
            {
                if (_lastResult != value)
                {
                    _lastResult = value;

                    OnPropertyChanged();
                }
            } 
        }

        private void OnChoseImageButtonClick(object sender, RoutedEventArgs e)
        {
            Error = string.Empty;
            Result = string.Empty;

            var source = LoadImage();

            if (source != null)
            {
                Image.Source = source;
                ScanImage(source);
            }
        }

        private void OnScanImageButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnScanRegionsButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ScanImage(BitmapImage source)
        {
            try
            {
                var enginePath = @"F:\Workbench\Test Projects\Ocr\Tesseract - Backup\Tesseract-OCR";

                var api = new TesseractEngine(enginePath, "eng", EngineMode.Default);

                // Open input image with leptonica library
                var image = Pix.LoadFromFile(source.UriSource.ToString());

                var page = api.Process(image);

                page.RegionOfInterest = new Rect(25, 25, 600, 400);
                var text = "--- Region 1 --------------------------------\n\n" + page.GetText().Trim() + "\n\n";

                page.RegionOfInterest = new Rect(30, 540, 640, 360);
                text += "--- Region 2 --------------------------------\n\n" + page.GetText().Trim() + "\n\n";

                page.RegionOfInterest = new Rect(760, 100, 620, 370);
                text += "--- Region 3 --------------------------------\n\n" + page.GetText().Trim() + "\n\n";

                page.RegionOfInterest = new Rect(760, 600, 650, 250);
                text += "--- Region 4 --------------------------------\n\n" + page.GetText().Trim() + "\n\n";

                Result = text;
            }
            catch (Exception exception)
            {
                var error = "Error when scanning image:" + Environment.NewLine + exception.Message;

                if (exception.InnerException != null)
                    error += Environment.NewLine + exception.InnerException.Message;

                Result = error;
            }
        }

        private BitmapImage LoadImage()
        {
            try
            {
                var dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".png";
                dlg.Filter = "Images (*.png, *.jpg)|*.png;*.jpg";

                bool? result = dlg.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    var filename = dlg.FileName.Trim();
                    FilePathTextBox.Text = filename;

                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(filename, UriKind.Relative);
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();

                    return source;
                }

                return null;
            }
            catch (Exception exception)
            {
                var error = "Error when loading image:" + Environment.NewLine + exception.Message;

                if (exception.InnerException != null)
                    error += Environment.NewLine + exception.InnerException.Message;

                Result = error;

                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
