using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for SurveyMaker.xaml
    /// </summary>
    public partial class SurveyMaker : Window
    {
        List<Grid> QuestionGrids = new List<Grid>();
        public SurveyMaker()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addNewImageSet();
        }
        private void addNewImageSet()
        {
            Grid imageGrid = new Grid();
            QuestionGrids.Add(imageGrid);
            imageGrid.Background = new SolidColorBrush(Color.FromRgb(0xb1, 0xb1, 0xb1));

            for (int i = 0; i < 5; i++)
            {
                RowDefinition row = new RowDefinition();
                imageGrid.RowDefinitions.Add(row);
            }
            int currRow = 0;
            Label titleLabel = new Label();
            titleLabel.Content = "Image Set 1";
            titleLabel.FontSize = 16;
            titleLabel.FontWeight = FontWeights.Bold;
            titleLabel.Foreground = Brushes.White;

            imageGrid.Children.Add(titleLabel);


            Label addLabel = new Label();
            addLabel.SetValue(Grid.RowProperty, ++currRow);
            addLabel.Content = "Add Image";
            imageGrid.Children.Add(addLabel);

            Image openPic2 = new Image();
            imageGrid.Children.Add(openPic2);
            openPic2 = addIcon("sunset.jpg");

            Grid imagePathGrid = new Grid();
            addImagePath(imagePathGrid);


            //RowDefinition row = new RowDefinition();
            //MainGrid.RowDefinitions.Add(row);

            imageGrid.Children.Add(imagePathGrid);
            MainGrid.Children.Add(imageGrid);
            imagePathGrid.SetValue(Grid.RowProperty, ++currRow);

        }
        private void addImagePath( Grid container)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            TextBox tb = new TextBox();
            tb.Width = 400;
            tb.Height = 25;
            tb.IsReadOnly = true;
            stack.Children.Add(tb);

            Image openPic = new Image();
            stack.Children.Add(openPic);
            openPic = addIcon("open.jpg");
            openPic.Width = 35;
            openPic.Height = 35;
            
            openPic.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            openPic.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            openPic.Stretch = Stretch.None;


            Image openPic2 = new Image();
            stack.Children.Add(openPic2);
            openPic2 = addIcon("sunset.jpg");
            openPic2.Width = 35;
            openPic2.Height = 35;

            openPic2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            openPic2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            openPic2.Stretch = Stretch.None;

            container.Children.Add(stack);
            
        }
        private Image addIcon(string path)
        {
            //TODO: protect against unfound files
            Image img = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(path, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            return img;
        }
    }
}
