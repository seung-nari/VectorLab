using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VectorLab.Views
{
    public partial class LabelInputDialog : Window
    {
        // 입력된 라벨 이름
        public string LabelName { get; private set; } = "default";

        public LabelInputDialog()
        {
            InitializeComponent();
            PlaceholderText.Visibility = Visibility.Visible;
        }

        private void LabelNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(LabelNameTextBox.Text))
                PlaceholderText.Visibility = Visibility.Visible;
            else
                PlaceholderText.Visibility = Visibility.Collapsed;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            LabelName = LabelNameTextBox.Text;

            if(LabelName == "") LabelName = "Default";

            DialogResult = true;
        }
    }
}
