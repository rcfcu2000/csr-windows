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

namespace csr_windows.Client.Views.Customer
{
    /// <summary>
    /// CustomerBottomAskAIView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerBottomInputAIView : UserControl
    {
        public CustomerBottomInputAIView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // Shift + Enter: Insert new line
                int caretIndex = textBox.CaretIndex;
                textBox.Text = textBox.Text.Insert(caretIndex, "\n");
                textBox.CaretIndex = caretIndex + 1;
                e.Handled = true; // Mark event as handled
            }
            else if (e.Key == Key.Enter)
            {
                // Enter: Trigger button click
                button.Command.Execute(null);
                e.Handled = true; // Mark event as handled
            }
        }
    }
}
