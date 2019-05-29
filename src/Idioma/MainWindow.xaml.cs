using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Idioma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoThing_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GenerateDefault_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Generate
            using (System.Windows.Forms.SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog() { Filter = "TGA (*.tga)|*.tga" })
            {
                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FontGenerator.GenerateSpriteSheetFontThing(this.FontSelection.SelectedItem.ToString(), saveFile.FileName, Convert.ToInt32(this.ScaleFontSize.Text), Convert.ToInt32(this.TopOffsetSize.Text), Convert.ToInt32(this.RightOffsetSize.Text));
                    MessageBox.Show("The font has been saved!", "Idioma", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ScaleFontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Loop through fonts
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (System.Drawing.FontFamily family in fonts.Families)
            {
                this.FontSelection.Items.Add(family.Name);
            }
            this.FontSelection.SelectedIndex = 0;
        }
    }
}
