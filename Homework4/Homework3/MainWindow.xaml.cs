// 引入所需的命名空間
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Homework3
{
    // 主窗口類別
    public partial class MainWindow : Window
    {
        // 定義全局變數
        String shapeType = "Line";  // 要繪製的形狀類型（默認為線）
        Color strokeColor = Colors.Red;  // 描邊顏色（默認為紅色）
        Color fillColor = Colors.Yellow;  // 填充顏色（默認為黃色）
        int strokeThickness = 1;  // 線條粗細（默認為1）

        Point start, dest;  // 繪圖的起點和終點座標

        public MainWindow()
        {
            InitializeComponent();  // 初始化窗口組件
            // 初始化顏色選擇器
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        // 當用戶點擊形狀按鈕時觸發此事件
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;  // 將觸發事件的對象轉為RadioButton
            shapeType = targetRadioButton.Tag.ToString();  // 更新要繪製的形狀類型
        }

        // 當用戶改變線條粗細滑塊時觸發此事件
        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = Convert.ToInt32(strokeThicknessSlider.Value);  // 更新線條粗細
        }

        // 當鼠標在畫布上移動時觸發此事件
        private void myCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvas);  // 獲取當前鼠標位置
            DisplayStatus();  // 顯示狀態信息

            // 如果左鍵被按下，則更新當前繪製的形狀
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 計算形狀的原點、寬度和高度
                Point origin = new Point
                {
                    X = Math.Min(start.X, dest.X),
                    Y = Math.Min(start.Y, dest.Y)
                };
                double width = Math.Abs(dest.X - start.X);
                double height = Math.Abs(dest.Y - start.Y);

                // 根據要繪製的形狀類型來更新畫布上的形狀
                switch (shapeType)
                {
                    case "Line":
                        var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                        line.X2 = dest.X;
                        line.Y2 = dest.Y;
                        break;
                    case "Rectangle":
                        var rect = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                        rect.Width = width;
                        rect.Height = height;
                        rect.SetValue(Canvas.LeftProperty, origin.X);
                        rect.SetValue(Canvas.TopProperty, origin.Y);
                        break;
                    case "Ellipse":
                        var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                        ellipse.Width = width;
                        ellipse.Height = height;
                        ellipse.SetValue(Canvas.LeftProperty, origin.X);
                        ellipse.SetValue(Canvas.TopProperty, origin.Y);
                        break;
                }
            }
        }

        // 當用戶按下畫布上的左鍵時觸發此事件
        private void myCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            start = e.GetPosition(myCanvas);  // 獲取起點座標
            myCanvas.Cursor = Cursors.Cross;  // 更改鼠標指針形狀為十字

            // 根據要繪製的形狀類型，在畫布上創建新的形狀
            switch (shapeType)
            {
                case "Line":
                    var line = new Line
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = dest.X,
                        Y2 = dest.Y
                    };
                    myCanvas.Children.Add(line);
                    break;
                case "Rectangle":
                    var rect = new Rectangle
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        Fill = Brushes.LightGray,
                    };
                    myCanvas.Children.Add(rect);
                    rect.SetValue(Canvas.LeftProperty, start.X);
                    rect.SetValue(Canvas.TopProperty, start.Y);
                    break;
                case "Ellipse":
                    var ellipse = new Ellipse
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        Fill = Brushes.LightGray,
                    };
                    myCanvas.Children.Add(ellipse);
                    ellipse.SetValue(Canvas.LeftProperty, start.X);
                    ellipse.SetValue(Canvas.TopProperty, start.Y);
                    break;
            }
            DisplayStatus();  // 顯示狀態信息
        }

        // 顯示當前狀態（座標點和形狀數量）
        // 用於顯示畫布上的狀態資訊
        private void DisplayStatus()
        {
            // 統計畫布上線條（Line）的數量
            int lineCount = myCanvas.Children.OfType<Line>().Count();

            // 統計畫布上矩形（Rectangle）的數量
            int rectCount = myCanvas.Children.OfType<Rectangle>().Count();

            // 統計畫布上橢圓（Ellipse）的數量
            int ellipseCount = myCanvas.Children.OfType<Ellipse>().Count();

            // 更新座標標籤（coordinateLabel）的內容，顯示起始和終點座標
            coordinateLabel.Content = $"座標點: ({Math.Round(start.X)}, {Math.Round(start.Y)}) : ({Math.Round(dest.X)}, {Math.Round(dest.Y)})";
            // 更新形狀標籤（shapeLabel）的內容，顯示各種形狀的數量
            shapeLabel.Content = $"Line: {lineCount}, Rectangle: {rectCount}, Ellipse: {ellipseCount}";
        }


        // 當用戶更改描邊顏色時觸發此事件
        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = (Color)strokeColorPicker.SelectedColor;  // 更新描邊顏色
        }

        // 當用戶更改填充顏色時觸發此事件
        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = (Color)fillColorPicker.SelectedColor;  // 更新填充顏色
        }

        // 當用戶點擊清除菜單項時觸發此事件
        private void clearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();  // 清除所有形狀
            DisplayStatus();  // 更新狀態信息
        }

        // 當用戶釋放畫布上的左鍵時觸發此事件
        private void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 將新繪製的形狀的顏色和線條粗細設為用戶選擇的值
            // 根據所選的形狀類型（shapeType）來更新最後繪製的形狀的屬性
            switch (shapeType)
            {
                // 如果選擇的是線條（Line）
                case "Line":
                    // 從畫布上獲取最後一條線條
                    var line = myCanvas.Children.OfType<Line>().LastOrDefault();

                    // 設定線條的描邊顏色
                    line.Stroke = new SolidColorBrush(strokeColor);
                    // 設定線條的粗細
                    line.StrokeThickness = strokeThickness;
                    // 跳出switch
                    break;

                // 如果選擇的是矩形（Rectangle）
                case "Rectangle":
                    // 從畫布上獲取最後一個矩形
                    var rect = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                    // 設定矩形的描邊顏色
                    rect.Stroke = new SolidColorBrush(strokeColor);
                    // 設定矩形的填充顏色
                    rect.Fill = new SolidColorBrush(fillColor);
                    // 設定矩形的線條粗細
                    rect.StrokeThickness = strokeThickness;
                    // 跳出switch
                    break;

                // 如果選擇的是橢圓（Ellipse）
                case "Ellipse":
                    {
                        // 從畫布上獲取最後一個橢圓
                        var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                        // 設定橢圓的描邊顏色
                        ellipse.Stroke = new SolidColorBrush(strokeColor);
                        // 設定橢圓的游標
                        myCanvas.Cursor = Cursors.Arrow;  // 將鼠標指針更改為箭頭
                        break;
                    }
            }
        }
    }
}
            




