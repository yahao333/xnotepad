using NotesApp.Models;
using NotesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotesApp.Views
{
    /// <summary>
    /// CardListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CardListWindow : Window
    {
        public CardListWindow(Folder folder)
        {
            InitializeComponent();

            var vm = new CardListViewModel(folder);
            vm.RequestReturnIndexWindowHandle = requestReturnIndexWindow;
            DataContext = vm;

            this.Loaded += CardListWindow_Loaded;
        }

        private void requestReturnIndexWindow()
        {
            IndexWindow indexWindow = new IndexWindow();
            indexWindow.Show();

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as CardListViewModel).SaveCommand.Execute(null);
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ButtonState == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
        }

        private void ResizeRight_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void DragResize(WindowResizeEdge direction)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private enum WindowResizeEdge
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8
        }

        private void ResizeBottomRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragResize(WindowResizeEdge.BottomRight);
            }
        }

        private void CardListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AttachToRightSide();
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
