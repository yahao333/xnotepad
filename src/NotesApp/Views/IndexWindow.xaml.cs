using NotesApp.Models;
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
    /// IndexWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IndexWindow : Window
    {
        public IndexWindow()
        {
            InitializeComponent();

            AttachToRightSide();

            var vm = new IndexViewModel(this);

            vm.cardListRequest = requestCardListHandle;

            DataContext = vm;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is Folder selectedItem)
            {
                var viewModel = DataContext as IndexViewModel;
                if (viewModel?.ItemDoubleClickCommand.CanExecute(selectedItem) == true)
                {
                    viewModel.ItemDoubleClickCommand.Execute(selectedItem);
                }
            }
        }

        private void requestCardListHandle(Folder folder)
        {
            CardListWindow cardListWindow = new CardListWindow(folder);
            cardListWindow.Show();
        }

        private void TagNameEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                Folder folder = GetFolderFromTextBox(textBox);
                folder.IsEditing = false;
            }
        }

        private void TagNameEditBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is IndexViewModel viewModel)
                {
                    var textBox = sender as TextBox;
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        Folder folder = GetFolderFromTextBox(textBox);

                        if (!viewModel.veridateTagName(folder))
                        {
                            MessageBox.Show($"错误的名称{folder.Name}");
                            return;
                        }

                        viewModel.SaveTagCommand.Execute(folder);
                        folder.IsEditing = false;
                    }

                }
            }
            else if (e.Key == Key.Escape)
            {
                if (DataContext is IndexViewModel viewModel)
                {
                    var textBox = sender as TextBox;
                    Folder folder = GetFolderFromTextBox(textBox);

                    if (!viewModel.veridateTagName(folder))
                    {
                        MessageBox.Show($"错误的名称{folder.Name}");
                        return;
                    }

                    folder.IsEditing = false;
                }
            }
        }

        private void TagNameEditBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is IndexViewModel viewModel)
            {
                var textBox = sender as TextBox;
                Folder folder = GetFolderFromTextBox(textBox);
                folder.IsEditing = true;
            }
        }
        private Folder GetFolderFromTextBox(TextBox textBox)
        {
            // Traverse the visual tree to find the ListViewItem
            var listViewItem = FindVisualParent<ListViewItem>(textBox);

            if (listViewItem != null)
            {
                // Get the data context (which is the Folder object) from the ListViewItem
                return listViewItem.DataContext as Folder;
            }

            return null;
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                {
                    return parent;
                }
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ButtonState == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
        }

        private void AttachToRightSide()
        {
            // 获取主屏幕的工作区
            var workingArea = System.Windows.SystemParameters.WorkArea;

            // 设置窗口宽度（可以根据需要调整）
            this.Width = 320;

            // 设置窗口高度为工作区高度
            this.Height = workingArea.Height;

            // 设置窗口左上角的X坐标，使其右边缘与屏幕右边缘对齐
            this.Left = workingArea.Right - this.Width;

            // 设置窗口左上角的Y坐标为工作区顶部
            this.Top = workingArea.Top;

            // 可选：如果你想留出一些空间给任务栏，可以稍微调整高度和顶部位置
            // this.Height -= 40; // 为任务栏预留空间
            // this.Top += 20; // 向下移动一点
        }
    }
}
