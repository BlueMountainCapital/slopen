using System;
using System.Collections.Generic;
using System.IO;
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

namespace SlnOpener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string RootPath = @"e:\source\rlee_BC323_bmc\src";

        public MainWindow()
        {
            var args = Environment.GetCommandLineArgs();
            var rootPath = args.Skip(1).FirstOrDefault();
            var refresh = args.Skip(2).Contains("refresh");
            if (rootPath == null)
                rootPath = RootPath;

            try
            {
                if (refresh)
                    Properties.Settings.Default.RefreshFileCache(rootPath);
                DataContext = new SolutionsViewModel(rootPath);
            }
            catch(DirectoryNotFoundException exception)
            {
                MessageBox.Show("Error: " + exception.ToString());
            }

            InitializeComponent();

            Loaded += (s, e) =>
            {
                SearchBox.Focus();
                SearchBox.Focusable = true;
                Keyboard.Focus(SearchBox);
            };

            SearchBox.GotFocus += (s, e) =>
            {
                SearchBox.SelectAll();
            };
        }
    }
}
