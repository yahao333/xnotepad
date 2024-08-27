using NotesApp.ViewModels;
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
using System.Windows.Shapes;

namespace NotesApp.Views
{
    /// <summary>
    /// NoteListPage.xaml 的交互逻辑
    /// </summary>
    public partial class NoteListPage : Window
    {
        public NoteListPage(NoteListViewModel noteListViewModel)
        {
            InitializeComponent();

            DataContext = noteListViewModel;
        }

        private void ListItemTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                ListViewItem listViewItem = FindAncestor<ListViewItem>(textBox);
                if (listViewItem != null)
                {
                    listViewItem.IsSelected = true;
                }
            }
        }
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
