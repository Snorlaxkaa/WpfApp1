using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // 字典，用於儲存飲料名稱和價格
        Dictionary<string, int> drinks = new Dictionary<string, int>();

        // 字典，用於儲存訂單，包括飲料名稱和數量
        Dictionary<string, int> orders = new Dictionary<string, int>();

        // 用於儲存外帶選項的變數
        string takeout = "";

        public MainWindow()
        {
            InitializeComponent();

            // 將飲料項目添加到 'drinks' 字典中
            AddNewDrink(drinks);

            // 在UI上顯示飲料菜單
            DisplayDrinkMenu(drinks);
        }

        // 在UI上顯示飲料菜單
        private void DisplayDrinkMenu(Dictionary<string, int> myDrinks)
        {
            foreach (var drink in myDrinks)
            {
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // 為每種飲料創建一個核取方塊
                var cb = new CheckBox
                {
                    Content = $"{drink.Key} : {drink.Value}元",
                    Width = 200,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18,
                    Foreground = Brushes.Blue,
                    Margin = new Thickness(5)
                };

                // 創建一個滑塊以選擇數量
                var sl = new Slider
                {
                    Width = 100,
                    Value = 0,
                    Minimum = 0,
                    Maximum = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true
                };

                // 創建一個標籤以顯示所選數量
                var lb = new Label
                {
                    Width = 50,
                    Content = "0",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18,
                    Foreground = Brushes.Red
                };

                // 將UI元素添加到StackPanel
                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                // 數據綁定以將滑塊值和標籤內容關聯起來
                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);

                // 將StackPanel添加到主UI StackPanel
                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        // 從文件中添加新的飲料到 'drinks' 字典
        private void AddNewDrink(Dictionary<string, int> myDrinks)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV文件|*.csv|文本文件|*.txt|所有文件|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = Convert.ToInt32(tokens[1]);
                    myDrinks.Add(drinkName, price);
                }
            }
        }

        // "訂購"按鈕點擊事件處理程序
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // 將選擇的飲料放入 'orders' 字典
            PlaceOrder(orders);

            // 在UI上顯示訂單詳細信息
            DisplayOrder(orders);

            SaveOrderToFile(orders);
        }

        // 在UI上顯示訂單詳細信息
        private void DisplayOrder(Dictionary<string, int> myOrders)
        {
            displayTextBlock.Inlines.Clear();
            Run titleString = new Run
            {
                Text = "您所訂購的飲品為 ",
                FontSize = 16,
                Foreground = Brushes.Blue
            };

            Run takeoutString = new Run
            {
                Text = $"{takeout}",
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            // 添加標題和外帶信息到顯示中
            displayTextBlock.Inlines.Add(titleString);
            displayTextBlock.Inlines.Add(takeoutString);
            displayTextBlock.Inlines.Add(new Run() { Text = " ，訂購明細如下: \n", FontSize = 16 });

            double total = 0.0;
            double sellPrice = 0.0;
            string discountString = "";

            int i = 1;
            foreach (var item in myOrders)
            {
                string drinkName = item.Key;
                int quantity = myOrders[drinkName];
                int price = drinks[drinkName];
                total += price * quantity;
                // 顯示每個訂購的項目及其詳細信息
                displayTextBlock.Inlines.Add(new Run() { Text = $"飲料品項{i}： {drinkName} X {quantity}杯，每杯{price}元，總共{price * quantity}元\n" });
                i++;
            }

            // 計算並顯示折扣和總價
            if (total >= 500)
            {
                discountString = "訂購滿500元以上者打8折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                discountString = "訂購滿300元以上者打85折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                discountString = "訂購滿200元以上者打9折";
                sellPrice = total * 0.9;
            }
            else
            {
                discountString = "訂購未滿200元以上者不打折";
                sellPrice = total;
            }

            // 顯示訂單摘要
            Italic summaryString = new Italic(new Run
            {
                Text = $"本次訂購總共{myOrders.Count}項，{discountString}，售價{sellPrice}元",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red
            });
            displayTextBlock.Inlines.Add(summaryString);
        }
        private void SaveOrderToFile(Dictionary<string, int> myOrders)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件|*.txt|所有文件|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                    {
                        writer.WriteLine("訂單明細:");
                        foreach (var item in myOrders)
                        {
                            string drinkName = item.Key;
                            int quantity = item.Value;
                            int price = drinks[drinkName];
                            writer.WriteLine($"{drinkName} X {quantity}杯，每杯{price}元，總共{price * quantity}元");
                        }
                        writer.Close();
                        MessageBox.Show("訂單已成功儲存到檔案: " + filename, "訂單儲存成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("儲存訂單時發生錯誤: " + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        // 將選擇的飲料放入 'orders' 字典
        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[1] as Slider;
                string drinkName = cb.Content.ToString().Substring(0, 4);
                int quantity = Convert.ToInt32(sl.Value);

                if (cb.IsChecked == true && quantity != 0)
                {
                    myOrders.Add(drinkName, quantity);
                }
            }
        }

        // 用於選擇外帶的單選按鈕事件處理程序
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true) takeout = rb.Content.ToString();
            //MessageBox.Show(takeout);
        }
    }
}
