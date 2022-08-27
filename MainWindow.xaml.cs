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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
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
        }

        private void TransformDoc(object sender, RoutedEventArgs e)
        {
            Transformations.BannedWordCheck(BannedWordCheck, BannedWordPolicyChoice);
            
            WriteToFile(DataFactory.OutputData());
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {

        }
    }
}
