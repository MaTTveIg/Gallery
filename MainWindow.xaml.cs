using Microsoft.Win32;
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
using System.Xml.Linq;
using System.Windows.Forms;
using FileTools.Analyze;

namespace Gallery
{
    public partial class MainWindow : Window
    {
        private readonly string[] fileMasks =
        {
            "*.jpg",
            "*.jpeg",
            "*.png",
            "*.gif",
            "*.jfif",
        };

        private FolderBrowserDialog dlg;
        private Finder finder;
        private List<string> list = new List<string>();
        private Border? activeBoarder = null;
        private bool theme = true;

        private readonly Brush selectedBrush = new SolidColorBrush(Colors.Black);
        private readonly Brush commonBrush = new SolidColorBrush(Colors.Transparent);

        public MainWindow()
        {
            InitializeComponent();
            dlg = new FolderBrowserDialog();
            finder = new Finder()
            {
                FileMasks = fileMasks,
            };
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            list.Clear();
            mainPanel.Children.Clear();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                finder.FindFilesByMasks(dlg.SelectedPath);
                list.AddRange(finder.Container.Files.Select(f => f.FullName));
            }

            list.ForEach(path =>
            {
                Image img = new Image();
                img.Width = 156;
                img.Height = 156;
                img.Stretch = Stretch.UniformToFill;

                img.Source = new BitmapImage(new Uri(path));

                Border border = new Border()
                {
                    Child = img,

                    MinWidth = 150,
                    BorderThickness = new Thickness(3),
                };
                
                border.MouseDown += Border_MouseDown;

                mainPanel.Children.Add(border);
            });
        }

        private void SelectImage(object sender)
        {
            if (sender is Border border)
            {
                if (border is not null)
                {
                    if (activeBoarder is not null)
                        activeBoarder.BorderBrush = commonBrush;

                    activeBoarder = border;
                    border.BorderBrush = selectedBrush;

                    Image? image = border.Child as Image;
                    if (image is not null)
                        preImage.Source = image.Source;
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectImage(sender);
            }
        }

        private void FullScreenMode_Click(object sender, RoutedEventArgs e)
        {
            if (activeBoarder is null)
                return;
            Window1 windowImage = new Window1(list, mainPanel.Children.IndexOf(activeBoarder));
            if (theme is true)
                windowImage.gridMain.Background = new SolidColorBrush(Colors.WhiteSmoke);
            else windowImage.gridMain.Background = new SolidColorBrush(Colors.Gray);
            windowImage.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    
        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            theme = true;
            this.Background = new SolidColorBrush(Colors.White);
            mainBorder.Background = new SolidColorBrush(Colors.WhiteSmoke);
            menuThemes.Background = new SolidColorBrush(Colors.WhiteSmoke); ;
            mainMenu.Background = new SolidColorBrush(Colors.WhiteSmoke); ;
            MyScrollViewer.Background = new SolidColorBrush(Colors.White); ;
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            theme = false;
            this.Background = new SolidColorBrush(Colors.DimGray);
            mainBorder.Background = new SolidColorBrush(Colors.Gray);
            menuThemes.Background = new SolidColorBrush(Colors.Gray);
            mainMenu.Background = new SolidColorBrush(Colors.Gray);
            MyScrollViewer.Background = new SolidColorBrush(Colors.Gray);
        }
    }
}