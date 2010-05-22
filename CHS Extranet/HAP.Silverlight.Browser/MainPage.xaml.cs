using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Controls.Theming;

namespace HAP.Silverlight.Browser
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            activeItems = new List<BrowserItem>();
            BrowserItem item = new BrowserItem(new BItem("Folder 1", "/HAP.Silverlight.Browser;component/folder.png", "", "Folder", BType.Folder, "/N/Folder"));
            item.Activate += new EventHandler(item_Activate);
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            item.ItemDrop += new DragEventHandler(item_Drop);
            item.DragEnter += new DragEventHandler(item_DragEnter);
            item.DragLeave += new DragEventHandler(item_DragLeave);
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            item.ReSort += new EventHandler(item_ReSort);
            contentPan.Children.Add(item);
            item = new BrowserItem(new BItem("File 1", "/HAP.Silverlight.Browser;component/file.png", "0 bytes", "File", BType.File, "/N/File"));
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            item.Activate += new EventHandler(item_Activate);
            item.ReSort += new EventHandler(item_ReSort);
            contentPan.Children.Add(item);
        }

        private void barContextButton1_Click(object sender, EventArgs e)
        {
            if (barContextButton1.Mode == ViewMode.List)
            {
                contentPan.Orientation = Orientation.Vertical;
                scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else
            {
                contentPan.Orientation = Orientation.Horizontal;
                scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            foreach (BrowserItem item in contentPan.Children)
                item.Mode = barContextButton1.Mode;
        }

        private List<BrowserItem> activeItems;

        private void item_Activate(object sender, EventArgs e)
        {
            candrag = drag = false;  mousemode = MouseMode.Normal;
            if (!selectall && !rightclick)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (activeItems.Contains((BrowserItem)sender))
                    {
                        activeItems.Remove((BrowserItem)sender);
                        ((BrowserItem)sender).Active = false;
                    }
                    else activeItems.Add((BrowserItem)sender);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (activeItems.Count > 0)
                    {
                        int index1 = contentPan.Children.IndexOf(activeItems[activeItems.Count - 1]);
                        int index2 = contentPan.Children.IndexOf((BrowserItem)sender);
                        if (index1 < index2) for (int x = index1 + 1; x <= index2; x++)
                            {
                                BrowserItem i = contentPan.Children[x] as BrowserItem;
                                i.Active = true;
                                activeItems.Add(i);
                            }
                        else for (int x = index2; x <= index1; x++)
                            {
                                BrowserItem i = contentPan.Children[x] as BrowserItem;
                                i.Active = true;
                                activeItems.Add(i);
                            }
                    }
                    else activeItems.Add((BrowserItem)sender);
                }
                else
                {
                    item_ReSort(this, new EventArgs());
                    ((BrowserItem)sender).Active = true;
                    if (((BrowserItem)sender).Active) activeItems.Add((BrowserItem)sender);
                }
            }
            else { selectall = false; rightclick = false; }
        }

        private bool selectall = false;
        private void organiseButton1_SelectAllClick(object sender, RoutedEventArgs e)
        {
            item_ReSort(this, new EventArgs());
            foreach (BrowserItem item in contentPan.Children)
            {
                item.Active = true;
                activeItems.Add(item);
            }
            selectall = true;
        }

        private bool rightclick = false;
        private void contentPan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            if (!rightclick)
            {
                item_ReSort(this, new EventArgs());
            } else rightclick = false;
        }

        private void contentPan_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeItems.Count == 1)
            {
                BrowserItem item = activeItems[0];
                RightClickRename.Visibility = RightClickDelete.Visibility = System.Windows.Visibility.Visible;
                RightClickZIP.Visibility = RightClickFolder.Visibility = RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (activeItems.Count > 1)
            {
                RightClickRename.Visibility = RightClickUNZIP.Visibility = RightClickFolder.Visibility = System.Windows.Visibility.Collapsed;
                RightClickZIP.Visibility = RightClickDelete.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                RightClickRename.Visibility = RightClickDelete.Visibility = RightClickUNZIP.Visibility = RightClickZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void RightClickRename_Click(object sender, RoutedEventArgs e)
        {
            activeItems[0].IsRename = true;
            rightclick = true;
        }

        private void newfolder_Click(object sender, RoutedEventArgs e)
        {
            if (sender == RightClickFolder) rightclick = true;
            item_ReSort(this, new EventArgs());
            int count = 0;
            foreach (BrowserItem i in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.Folder))
                if (i.Data.Name.StartsWith("New Folder")) count++;
            BrowserItem item = new BrowserItem(new BItem("New Folder" + (count == 0 ? "" : " " + count), "/HAP.Silverlight.Browser;component/folder.png", "", "Folder", BType.Folder, "/N/New Folder" + (count == 0 ? "" : (count + 1).ToString())));
            item.ReSort += new EventHandler(item_ReSort);
            item.Activate += new EventHandler(item_Activate);
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            contentPan.Children.Add(item);
            item.Mode = barContextButton1.Mode;
            item.Active = true;
            item.IsRename = true;
            activeItems.Add(item);
        }

        private void item_ReSort(object sender, EventArgs e)
        {
            foreach (BrowserItem item in activeItems)
                if (item.IsRename)
                    if (item.Save(false) == 0) item.Active = false;
                    else return;
                else item.Active = false;
            activeItems.Clear();
            List<BrowserItem> folders = new List<BrowserItem>();
            foreach (BrowserItem item in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.Folder))
                folders.Add(item);
            folders.Sort();
            List<BrowserItem> files = new List<BrowserItem>();
            foreach (BrowserItem item in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.File))
                files.Add(item);
            files.Sort();
            contentPan.Children.Clear();
            foreach (BrowserItem item in folders) contentPan.Children.Add(item);
            foreach (BrowserItem item in files) contentPan.Children.Add(item);
        }

        private void RightClickDelete_Click(object sender, RoutedEventArgs e)
        {
            rightclick = true;
            foreach (BrowserItem i in activeItems)
            {
                if (i.IsRename)
                    if (i.Save(false) == 0) i.Active = false;
                    else return;
                else i.Delete();
                contentPan.Children.Remove(i);
            }
            activeItems.Clear();
        }

        private bool candrag = false;
        private bool drag = false;
        private MouseMode mousemode = MouseMode.Normal;

        private void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            candrag = true;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag && mousemode == MouseMode.NoGo)
            {
                nomove.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
                nomove.Visibility = System.Windows.Visibility.Visible;
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (drag && mousemode == MouseMode.Move)
            {
                movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
                movetooltip.Visibility = System.Windows.Visibility.Visible;
                nomove.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                movetooltip.Visibility = nomove.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            mousemode = MouseMode.Normal;
            drag = false;
            movetooltip.Visibility = nomove.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (candrag) {
                if (!activeItems.Contains((BrowserItem)sender))
                {
                    foreach (BrowserItem item in activeItems)
                        if (item.IsRename)
                            if (item.Save(false) == 0) item.Active = false;
                            else return;
                        else item.Active = false;
                    activeItems.Clear();
                    ((BrowserItem)sender).Active = true;
                    activeItems.Add((BrowserItem)sender);
                }
                drag = true;
                mousemode = MouseMode.Normal; 
                candrag = false;
            }
            mousemode = MouseMode.NoGo;
        }

        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                if (((BrowserItem)sender).Data.BType == BType.Folder)
                {
                    if (activeItems.Contains((BrowserItem)sender)) mousemode = MouseMode.NoGo;
                    else
                    {
                        movetext.Text = "Move to";
                        movefoldertext.Text = ((BrowserItem)sender).Data.Name;
                        mousemode = MouseMode.Move;
                    }
                }
                else mousemode = MouseMode.NoGo;
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            mousemode = MouseMode.Normal;
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Refresh Treeview");
        }

        private void contentPan_DragEnter(object sender, DragEventArgs e)
        {
            mousemode = MouseMode.Upload;
            nomove.Visibility = System.Windows.Visibility.Visible;
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            movetext.Text = "Upload to";
            movefoldertext.Text = "This Folder";
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            movetext.Text = "Upload to";
            movefoldertext.Text = "This Folder";
            movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
            movetooltip.Visibility = System.Windows.Visibility.Visible;
            nomove.Visibility = System.Windows.Visibility.Collapsed;
            e.Handled = true;
        }

        private void contentPan_DragOver(object sender, DragEventArgs e)
        {
            movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
            movetooltip.Visibility = System.Windows.Visibility.Visible;
            nomove.Visibility = System.Windows.Visibility.Collapsed;
            e.Handled = true;
        }

        private void contentPan_DragLeave(object sender, DragEventArgs e)
        {
            nomove.Visibility = System.Windows.Visibility.Visible;
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void item_DragLeave(object sender, DragEventArgs e)
        {
            movefoldertext.Text = "This Folder";
            ((BrowserItem)sender).Leave();
        }

        private void item_DragEnter(object sender, DragEventArgs e)
        {
            mousemode = MouseMode.Upload;
            nomove.Visibility = System.Windows.Visibility.Visible;
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            movetext.Text = "Upload to";
            movefoldertext.Text = ((BrowserItem)sender).Data.Name;
            ((BrowserItem)sender).Hover();
            e.Handled = true;
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            nomove.Margin = movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            nomove.Visibility = movetooltip.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            nomove.Visibility = movetooltip.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void contentPan_Drop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            try
            {

                FileInfo[] files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                if (files == null) return;

                foreach (FileInfo file in files)
                {
                    //if (filters.Contains("*.*") || filters.ToLower().Contains(file.Extension.Trim(new char[] { '.' }).ToLower()))
                    //{
                    UploadItem ui = new UploadItem(file);
                    UploadQueue.Children.Add(ui);
                    //}
                    //else MessageBox.Show("File Type Not Allowed (" + file.Extension.ToLower() + ")", "This file is not allowed to be uploaded", MessageBoxButton.OK);
                }
            }
            catch { MessageBox.Show("The item you have dragged here is not supported"); }
            
        }

        private void item_Drop(object sender, DragEventArgs e)
        {
            ((BrowserItem)sender).Leave();
            //Upload to a specific folder
            if (e.Data == null) return;

            try
            {

                FileInfo[] files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                if (files == null) return;

                foreach (FileInfo file in files)
                {
                    //if (filters.Contains("*.*") || filters.ToLower().Contains(file.Extension.Trim(new char[] { '.' }).ToLower()))
                    //{
                    UploadItem ui = new UploadItem(file);
                    UploadQueue.Children.Add(ui);
                    //}
                    //else MessageBox.Show("File Type Not Allowed (" + file.Extension.ToLower() + ")", "This file is not allowed to be uploaded", MessageBoxButton.OK);
                }
            }
            catch { MessageBox.Show("The item you have dragged here is not supported"); }
        }

        private void helpButton1_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }
    }

    public enum ViewMode { List, SmallIcon, Icon, LargeIcon, Tile }
    public enum MouseMode { NoGo, Move, Normal, Upload }
}
