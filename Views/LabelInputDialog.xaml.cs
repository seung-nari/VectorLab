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
        public string LabelName { get; private set; } = "";

        public LabelInputDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            LabelName = LabelNameTextBox.Text;

            if(LabelName == "") LabelName = "default";

            DialogResult = true;
        }
    }
}
