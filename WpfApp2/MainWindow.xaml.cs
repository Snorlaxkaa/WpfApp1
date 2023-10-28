using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        string takeout = "無外帶選項";

        public MainWindow()
        {
            InitializeComponent();
            AddNewDrink(drinks);
            DisplayDrinkMenu(drinks);
        }

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
                    if (tokens.Length != 2)
                    {
                        MessageBox.Show("無效的CSV格式");
                        return;
                    }
                    string drinkName = tokens[0];
                    if (!int.TryParse(tokens[1], out int price))
                    {
                        MessageBox.Show("無效的價格格式");
                        return;
                    }
                    myDrinks.Add(drinkName, price);
                }
            }
        }

        private void DisplayDrinkMenu(Dictionary<string, int> myDrinks)
        {
            foreach (var drink in myDrinks)
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };

                var cb = new CheckBox
                {
                    Content = $"{drink.Key} : {drink.Value}元",
                    Width = 200,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18,
                    Foreground = Brushes.Blue,
                    Margin = new Thickness(5)
                };

                var sl = new Slider
                {
                    Width = 100,
                    Value = 0,
                    Minimum = 0,
                    Maximum = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true
                };

                var lb = new Label
                {
                    Width = 50,
                    Content = "0",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18,
                    Foreground = Brushes.Red
                };

                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);

                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder(orders);
            DisplayOrder(orders);
            SaveOrderToFile(orders);
        }

        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[1] as Slider;
                string drinkName = cb.Content.ToString().Split(':')[0].Trim();
                int quantity = Convert.ToInt32(sl.Value);
                if (cb.IsChecked == true && quantity != 0)
                {
                    myOrders.Add(drinkName, quantity);
                }
            }
        }

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
                displayTextBlock.Inlines.Add(new Run() { Text = $"飲料品項{i}： {drinkName} X {quantity}杯 = {price * quantity}元 \n", FontSize = 16 });
                i++;
            }
            displayTextBlock.Inlines.Add(new Run() { Text = $"飲料總金額 = {total}元 \n", FontSize = 16 });
        }

        private void SaveOrderToFile(Dictionary<string, int> myOrders)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                    {
                        writer.WriteLine("飲料訂單明細");
                        writer.WriteLine("外帶選項: " + takeout);
                        writer.WriteLine("====================");
                        foreach (var item in myOrders)
                        {
                            writer.WriteLine($"飲品: {item.Key}, 數量: {item.Value}, 單價: {drinks[item.Key]}, 總價: {item.Value * drinks[item.Key]}");
                        }
                        writer.WriteLine("====================");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("儲存訂單時發生錯誤: " + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
