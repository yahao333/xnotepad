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

namespace NotesApp.UserControls
{
    /// <summary>
    /// TagListItem.xaml 的交互逻辑
    /// </summary>
    public partial class TagListItem : UserControl
    {
        private string _originalText;

        public TagListItem()
        {
            InitializeComponent();
        }


        private void EditableText_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //EditableText.Visibility = Visibility.Collapsed;
            //TextBoxEdit.Visibility = Visibility.Visible;
            //TextBoxEdit.Focus();
            if (e.ClickCount == 2)
            {
                _originalText = EditableText.Text;

                EditableText.Visibility = Visibility.Collapsed;
                TextBoxEdit.Visibility = Visibility.Visible;
                TextBoxEdit.Focus();
            }
        }

        private void TextBoxEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxEdit.Visibility = Visibility.Collapsed;
            EditableText.Visibility = Visibility.Visible;

            CancelEdit();
        }

        private void TextBoxEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateText();
            }
            else if (e.Key == Key.Escape)
            {
                CancelEdit();
            }
        }

        private void UpdateText()
        {
            TextBoxEdit.Visibility = Visibility.Collapsed;
            EditableText.Visibility = Visibility.Visible;
            BindingExpression binding = TextBoxEdit.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }

        private void CancelEdit()
        {
            TextBoxEdit.Text = _originalText;
            TextBoxEdit.Visibility = Visibility.Collapsed;
            EditableText.Visibility = Visibility.Visible;
        }
    }
}
