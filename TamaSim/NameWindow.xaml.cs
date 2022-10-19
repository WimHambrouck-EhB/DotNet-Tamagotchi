using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TamaSim
{
    /// <summary>
    /// Interaction logic for NameWindow.xaml. 
    /// Layout van venster gebaseerd op: https://www.wpf-tutorial.com/dialogs/creating-a-custom-input-dialog/
    /// </summary>
    public partial class NameWindow : Window
    {
        public string TamaName { get; set; }
        public NameWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Gelieve een naam in te voeren!", "Ongeldige invoer.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                TamaName = txtName.Text;
                DialogResult = true;
                Close();
            }            
        }
    }
}
