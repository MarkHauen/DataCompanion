using ExcelReader.DataTools;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ExcelReader.MyIO;

namespace ExcelReader
{
    public partial class MainWindow : Window
    {
        Config currentConfig;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            currentConfig = InitializeData();
            HideInitialComponents();
            PopulateBannedWords();
            PopulateBannedWordPolicies();
        }

        private void PopulateBannedWordPolicies()
        {
            BannedWordPolicyChoice.Items.Add("Redact");
            BannedWordPolicyChoice.Items.Add("Delete Row");
            BannedWordPolicyChoice.SelectedIndex = 1;
        }

        private void LoadDoc(object sender, RoutedEventArgs e)
        {
            DataFactory.BuildData(GetFileContents(FileNameTextBox.Text));
            EnableInitialComponents();
        }

        private void TransformDoc(object sender, RoutedEventArgs e)
        {
            Transformations.BannedWordCheck(BannedWordCheck, BannedWordPolicyChoice);
            WriteToCSV(DataFactory.OutputData());
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {

        }

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
        }

        private void EnableInitialComponents()
        {
            ChooseConfigLabel.Visibility = Visibility.Visible;
            ConfigChoice.Visibility = Visibility.Visible;
            SaveConfigButton.Visibility = Visibility.Visible;
            SaveConfigNameLabel.Visibility = Visibility.Visible;
            SaveConfigNameTextBox.Visibility = Visibility.Visible;
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

            ConfigChoice.Items.Add(currentConfig.name);
            ConfigChoice.SelectedIndex = 0;
            ColumnChoice.ItemsSource = currentConfig.columns;
            ColumnChoice.SelectedIndex = 0;
            MaxLengthChoice.Text = currentConfig.maxLengths.ElementAt(0).ToString();
            MinLengthChoice.Text = currentConfig.minLengths.ElementAt(0).ToString();
        }
    }
}
