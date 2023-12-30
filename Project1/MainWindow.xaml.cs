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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PaintButton.Click += PaintButton_Click;
        }
        private void PaintButton_Click(object sender, RoutedEventArgs e)
        {
            FormValidationLabel.Visibility = Visibility.Collapsed;
            FormTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            TrianglePolygon.Visibility = TriangleRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            RectanglePolygon.Visibility = RectangleRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            CircleElipse.Visibility = CircleRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            EllipseElipse.Visibility = ElipsRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            TextBorder.Visibility = TextRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            FuncImage.Visibility = FormRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            if (TextRadioButton.IsChecked == true)
            {
                ShowTextBlock.Text = TextTextBox.Text;
            }
            if (FormRadioButton.IsChecked == true)
            {
                if (!Calculator.isValid(FormTextBox.Text))
                {
                    FormValidationLabel.Visibility = Visibility.Visible;
                    FormTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                    FormTextBox.Focus();
                    return;
                }
                var calc = Calculator.GetDelegate<Func<double, double>>(FormTextBox.Text);
                double w = DrawAreaCanvas.ActualWidth, h = DrawAreaCanvas.ActualHeight;
                GeometryGroup geometryGroup = new GeometryGroup(), xy = new();
                xy.Children.Add(new LineGeometry(new Point(-w / 2, 0), new Point(w / 2, 0)));
                xy.Children.Add(new LineGeometry(new Point(0, -h / 2), new Point(0, h / 2)));
                double y;
                for (double x = -w / 2; x < w / 2; x += 0.1)
                {
                    y = calc(x/10)*10;
                    if (y > -h / 2 && y < h / 2)
                        geometryGroup.Children.Add(new RectangleGeometry(new Rect(x, y, 1, 1)));
                }
                GeometryDrawing geometryDrawing = new GeometryDrawing(),xydr=new();
                xydr.Geometry = xy;
                xydr.Pen = new Pen(new SolidColorBrush(Colors.Gray), 2);
                xydr.Pen.DashStyle=DashStyles.Dash;
                geometryDrawing.Geometry = geometryGroup;
                geometryDrawing.Brush = new SolidColorBrush(Colors.Black);
                geometryDrawing.Pen = new Pen(new SolidColorBrush(Colors.Black), 1);
                DrawingGroup draw = new DrawingGroup();
                draw.Children.Add(geometryDrawing);
                draw.Children.Add(xydr);
                DrawingImage drawingImage=new DrawingImage(draw);
                drawingImage.Freeze();
                FuncImage.Source = drawingImage;
            }
        }
    }
}
