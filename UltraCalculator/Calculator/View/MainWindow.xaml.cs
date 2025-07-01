using Calculator.Services;
using Calculator.ViewModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;

namespace Calculator.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseApp(object sender, RoutedEventArgs e) 
        {
            this.Close();
        }

        private void ReadDisplay(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new SelectionWindow();
            if (selectionWindow.ShowDialog() == true)
            {
                var selectedRectangle = selectionWindow.SelectedArea;

                var bitMapSource = ScreenshotLogic.CaptureScreenArea(selectedRectangle);
                var bitmap = ScreenshotLogic.BitmapSourceToBitmap(bitMapSource);
                var resizedBitmap = ScreenshotLogic.ResizeBitmap(bitmap, 2.0);

                if (bitmap != null)
                {
                    string ocrDataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OcrData");
                    using var engine = new TesseractEngine(ocrDataPath, "eng", EngineMode.Default);
                    using var img = PixConverter.ToPix(bitmap);
                    using var page = engine.Process(img);
                    string text = page.GetText();

                    var checkNumbers = Regex.Matches(text, @"\d+([.,]\d+)?").Cast<Match>().Select(m => double.Parse(m.Value.Replace(',', '.'), CultureInfo.InvariantCulture)).ToList();

                    if (DataContext is CalculatorViewModel vm)
                    {
                        string scannedExpression = String.Join('+', checkNumbers);
                        vm.SetScannedExpression(scannedExpression);
                    }
                }
            }
        }
    }
}