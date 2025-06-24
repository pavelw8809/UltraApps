using Calculator.Services;
using Calculator.ViewModel;
using IronOcr;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

                if (bitmap != null)
                {
                    var ocr = new IronTesseract();
                    string text;
                    using var input = new OcrInput();
                    input.LoadImage(bitmap);
                    input.EnhanceResolution(300);
                    input.ToGrayScale();
                    input.DeNoise();
                    var result = ocr.Read(input);
                    text = result.Text;

                    var checkNumbers = Regex.Matches(text, @"\d+([.,]\d+)?").Cast<Match>().Select(m => double.Parse(m.Value.Replace(',', '.'), CultureInfo.InvariantCulture)).ToList();

                    if (DataContext is CalculatorViewModel vm)
                    {
                        string scannedExpression = String.Join('+', checkNumbers);
                        vm.SetScannedExpression(scannedExpression);
                    }
                }
            }
        }

        private BitmapSource CaptureScreenArea(Rect rect)
        {
            using var screenBmp = new Bitmap((int)rect.Width, (int)rect.Height);
            using var g = Graphics.FromImage(screenBmp);
            g.CopyFromScreen((int)rect.X, (int)rect.Y, 0, 0, screenBmp.Size);

            return Imaging.CreateBitmapSourceFromHBitmap(
                screenBmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
        }
    }
}