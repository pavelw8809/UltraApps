using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calculator.View
{
    public partial class SelectionWindow : Window
    {
        private Point startPoint;
        public Rect SelectedArea { get; private set; }

        public SelectionWindow()
        {
            InitializeComponent();
            SelectionCanvas.MouseLeftButtonDown += SelectionWindow_MouseLeftButtonDown;
            SelectionCanvas.MouseMove += SelectionWindow_MouseMove;
            SelectionCanvas.MouseLeftButtonUp += SelectionWindow_MouseLeftButtonUp;
            Loaded += (s, e) => SelectionCanvas.Focus();
        }

        private void SelectionWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(SelectionCanvas);
            SelectionRectangle.Width = 0;
            SelectionRectangle.Height = 0;
            Canvas.SetLeft(SelectionRectangle, startPoint.X);
            Canvas.SetTop(SelectionRectangle, startPoint.Y);
            SelectionRectangle.Visibility = Visibility.Visible;
            SelectionCanvas.CaptureMouse();
        }

        private void SelectionWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!SelectionCanvas.IsMouseCaptured) return;

            var pos = e.GetPosition(SelectionCanvas);
            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);
            var w = Math.Abs(pos.X - startPoint.X);
            var h = Math.Abs(pos.Y - startPoint.Y);

            Canvas.SetLeft(SelectionRectangle, x);
            Canvas.SetTop(SelectionRectangle, y);
            SelectionRectangle.Width = w;
            SelectionRectangle.Height = h;
        }

        private void SelectionWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) { 
            ReleaseMouseCapture();
            var x = Canvas.GetLeft(SelectionRectangle);
            var y = Canvas.GetTop(SelectionRectangle);
            var w = SelectionRectangle.Width;
            var h = SelectionRectangle.Height;
            SelectedArea = new Rect(x-5, y-5, w+10, h+10);
            DialogResult = true;
            Close();
        }
    }
}
