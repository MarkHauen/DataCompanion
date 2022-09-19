using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static ExcelReader.MyIO;

namespace ExcelReader
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            FirstTimeSetup();
            CreateDBConnection();
            InitializeComponent();
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            FileNameChoice.ItemsSource = PopulateAvailableFiles();
            HideInitialComponents();
            PopulateBannedWords();
        }

        private void LoadDoc(object sender, RoutedEventArgs e)
        {
            if(FileNameChoice.SelectedItem == null)
            {
                FileNameChoice.ItemsSource = PopulateAvailableFiles();
                return;
            }
            MyData.BuildData(GetFileContents($"input\\{FileNameChoice.SelectedItem}"));
            EnableConfiguationBox();
        }

        private void NewConfig(object sender, RoutedEventArgs e)
        {
            PopulateNewTable(Sanitize(SaveConfigNameTextBox.Text));
            if (!ConfigChoice.Items.Contains(Sanitize(SaveConfigNameTextBox.Text)))
            {
                ConfigChoice.Items.Add(Sanitize(SaveConfigNameTextBox.Text));
                ConfigChoice.SelectedIndex = ConfigChoice.Items.Count - 1;
            }
            else
            {
                ConfigChoice.SelectedItem = Sanitize(SaveConfigNameTextBox.Text);
            }
            EnableConfigurationComponents();
            RepopulateConfigruationComponents();
        }

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            PopulateNewTable(Sanitize(ConfigChoice.Text));
            EnableConfigurationComponents();
            RepopulateConfigruationComponents();
        }

        private void DeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            DeleteTable(ConfigChoice.Text);
            ConfigChoice.Items.Remove(ConfigChoice.Text);
        }

        private void TransformDoc(object sender, RoutedEventArgs e)
        {
            foreach (int column in GetBannedWordColumns(ConfigChoice.Text))
            {
                Transformations.BannedWordCheck(BannedWordPolicyChoice, column);
            }
            foreach (int column in GetMinLengthColumns(ConfigChoice.Text))
            {
                MyData.RunEnsureMinLength(column, GetColumnsMinLength(ConfigChoice.Text, column.ToString()), GetColumnsFillChar(ConfigChoice.Text, column.ToString()));
            }
            foreach (int column in GetMaxLengthColumns(ConfigChoice.Text))
            {
                MyData.RunEnsureMaxLength(column, GetColumnsMaxLength(ConfigChoice.Text, column.ToString()));
            }
            WriteToCSV(MyData.OutputData());
        }

        ///////////////////////////////////////////////////

        private void BannedWordCheck_Checked(object sender, RoutedEventArgs e)
        {
            UpdateBannedPolicy(ConfigChoice.Text, b2is(BannedWordCheck), ColumnChoice.Text);
        }

        private void ColumnChoice_SelectionChanged(object sender, EventArgs e)
        {
            List<int> columnConfig = GetColumnConfig(ConfigChoice.Text, ColumnChoice.Text);
            MaxLengthChoice.Text = columnConfig[0].ToString();
            MinLengthChoice.Text = columnConfig[1].ToString();
            FillCharChoice.SelectedIndex = columnConfig[2];
            BannedWordCheck.IsChecked = i2b(columnConfig[3]);
        }

        private void MinLengthChoice_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMinLengthPolicy(ConfigChoice.Text, MinLengthChoice.Text, ColumnChoice.Text);
        }

        private void MaxLengthChoice_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMaxLengthPolicy(ConfigChoice.Text, MaxLengthChoice.Text, ColumnChoice.Text);
        }

        private void FillCharChoice_SelectionChanged(object sender, EventArgs e)
        {
            UpdateFillChar(ConfigChoice.Text, FillCharChoice.SelectedIndex.ToString(), ColumnChoice.Text);
        }

        private void EnableDelete_Checked(object sender, RoutedEventArgs e)
        {
            if (DeleteConfigButton.Visibility == Visibility.Hidden)
            {
                DeleteConfigButton.Visibility = Visibility.Visible;
            }
            else
            {
                DeleteConfigButton.Visibility = Visibility.Hidden;
            }
        }

        ///////////////////////////////////////////////////

        private void HideInitialComponents()
        {
            ChooseConfigLabel.Visibility = Visibility.Hidden;
            ConfigChoice.Visibility = Visibility.Hidden;
            SaveConfigButton.Visibility = Visibility.Hidden;
            SaveConfigNameLabel.Visibility = Visibility.Hidden;
            SaveConfigNameTextBox.Visibility = Visibility.Hidden;
            TransformDocButton.Visibility = Visibility.Hidden;
            ColumnChoiceLabel.Visibility = Visibility.Hidden;
            ColumnChoice.Visibility = Visibility.Hidden;
            BannedWordLabel.Visibility = Visibility.Hidden;
            BannedWordCheck.Visibility = Visibility.Hidden;
            BannedWordPolicyChoice.Visibility = Visibility.Hidden;
            MinLengthLabel.Visibility = Visibility.Hidden;
            MinLengthChoice.Visibility = Visibility.Hidden;
            MaxLenthLabel.Visibility = Visibility.Hidden;
            MaxLengthChoice.Visibility = Visibility.Hidden;
            FillCharLabel.Visibility = Visibility.Hidden;
            FillCharChoice.Visibility = Visibility.Hidden;
            Border1.Visibility = Visibility.Hidden;
            Border2.Visibility = Visibility.Hidden;
            LoadConfigButton.Visibility = Visibility.Hidden;
            DeleteConfigButton.Visibility = Visibility.Hidden;
            EnableDeleteCheckbox.Visibility = Visibility.Hidden;

            BannedWordPolicyChoice.SelectedIndex = 0;
        }

        private void EnableConfiguationBox()
        {
            ChooseConfigLabel.Visibility = Visibility.Visible;
            ConfigChoice.Visibility = Visibility.Visible;
            SaveConfigButton.Visibility = Visibility.Visible;
            SaveConfigNameLabel.Visibility = Visibility.Visible;
            SaveConfigNameTextBox.Visibility = Visibility.Visible;
            Border1.Visibility = Visibility.Visible;
            LoadConfigButton.Visibility = Visibility.Visible;
            EnableDeleteCheckbox.Visibility = Visibility.Visible;

            foreach (string tableName in GetTableNames())
            {
                ConfigChoice.Items.Add(tableName);
            }
            ConfigChoice.SelectedIndex = 0;
        }

        private void EnableConfigurationComponents()
        {
            if (TransformDocButton.Visibility == Visibility.Hidden)
            {
                TransformDocButton.Visibility = Visibility.Visible;
                ColumnChoiceLabel.Visibility = Visibility.Visible;
                ColumnChoice.Visibility = Visibility.Visible;
                BannedWordLabel.Visibility = Visibility.Visible;
                BannedWordCheck.Visibility = Visibility.Visible;
                BannedWordPolicyChoice.Visibility = Visibility.Visible;
                MinLengthLabel.Visibility = Visibility.Visible;
                MinLengthChoice.Visibility = Visibility.Visible;
                MaxLenthLabel.Visibility = Visibility.Visible;
                MaxLengthChoice.Visibility = Visibility.Visible;
                FillCharLabel.Visibility = Visibility.Visible;
                FillCharChoice.Visibility = Visibility.Visible;
                Border2.Visibility = Visibility.Visible;
            }
        }

        private void RepopulateConfigruationComponents()
        {
            ColumnChoice.ItemsSource = GetColumns(ConfigChoice.Text);
            ColumnChoice.SelectedItem = 0;
            List<int> columnConfig = GetColumnConfig(ConfigChoice.Text, ColumnChoice.Text);
            MaxLengthChoice.Text = columnConfig[0].ToString();
            MinLengthChoice.Text = columnConfig[1].ToString();
            FillCharChoice.SelectedIndex = columnConfig[2];
            BannedWordCheck.IsChecked = i2b(columnConfig[3]);
        }

        private static string b2is(CheckBox box)
        {
            return box.IsChecked.Value ? "1" : "0";
        }

        private static bool i2b(int i)
        {
            return i != 0;
        }

        private static string Sanitize(string input)
        {
            if (input == null || input.Length == 0)
            {
                return "Default_Table";
            }
            return Regex.Replace(input, @"\s+", "")
                .Replace("-", "")
                .Replace(".", "")
                .Replace(":", "")
                .Replace("_", "")
                .Replace("!", "")
                .Replace("@", "")
                .Replace("#", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("^", "")
                .Replace("&", "")
                .Replace("(", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("*", "")
                .Replace("+", "")
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("=", "")
                .Replace("\"", "")
                .Replace("'", "")
                .Replace("`", "")
                .Replace("~", "");
        }

        
    }
}
