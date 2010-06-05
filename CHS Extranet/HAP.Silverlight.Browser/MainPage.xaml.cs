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
using System.Windows.Browser;

namespace HAP.Silverlight.Browser
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            loaded = false;
            activeItems = new List<BrowserItem>();
            Filter = new List<string>();
            ViewMode = Browser.ViewMode.Tile;
            CurrentItem = new BItem("My Computer", "", "", "", BType.Drive, "", false);
            newfolder.Visibility = System.Windows.Visibility.Collapsed;
            WebClient driveclient = new WebClient();
            driveclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(driveclient_DownloadStringCompleted);
            driveclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + "/extranet/api/mycomputer/listdrives"), false);
        }

        private bool loaded;
        public List<string> Filter { get; set; }
        private BItem CurrentItem { get; set; }
        private List<BItem> Drives { get; set; }
        public ViewMode ViewMode { get; set; }

        delegate void ClearItemsDelegate();
        delegate void AddItemDelegate(BrowserItem item, bool Resort);
        public void AddItem(BrowserItem item, bool Resort)
        {
            contentPan.Children.Add(item);
            if (Resort) item_ReSort(this, true);
        }
        public void ClearItems()
        {
            contentPan.Children.Clear();
            activeItems.Clear();
        }

        private void driveclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DownloadStringCompletedEventHandler(driveclient_DownloadStringCompleted2), sender, e);
        }
        private void driveclient_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            if (loaded) ClearItems();
            foreach (string s in e.Result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.StartsWith("FILTER"))
                {
                    if (!Filter.Contains(s.Remove(0, 6)))
                        Filter.Add(s.Remove(0, 6));
                }
                else
                {
                    string[] ss = s.Split(new char[] { ',' });
                    BItem bitem = new BItem(ss[0], ss[1], "", "Drive", BType.Drive, ss[2], bool.Parse(ss[3]));
                    BrowserItem item = new BrowserItem(bitem);
                    if (!loaded)
                    {
                        HAPTreeNode titem = new HAPTreeNode();
                        titem.Cursor = Cursors.Hand;
                        titem.Header = bitem.Name;
                        titem.BItem = bitem;
                        titem.Expanded += new RoutedEventHandler(HAPTreeNode_Expanded);
                        titem.Selected += new RoutedEventHandler(HAPTreeNode_Selected);
                        HAPTreeNode ttitem = new HAPTreeNode();
                        ttitem.Header = "Loading...";
                        titem.Items.Add(ttitem);
                        treeView1.Items.Add(titem);
                    }
                    item.MouseEnter += new MouseEventHandler(item_MouseEnter);
                    item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                    item.MouseLeave += new MouseEventHandler(item_MouseLeave);
                    item.Activate += new EventHandler(item_Activate);
                    item.ReSort += new ResortHandler(item_ReSort);
                    item.DirectoryChange += new ChangeDirectoryHandler(item_DirectoryChange);
                    contentPan.Children.Add(item);
                    item.Mode = ViewMode;
                }
            }
            if (!loaded)
            {
                string p = HtmlPage.Document.DocumentUri.AbsoluteUri;
                if (p.Contains('#'))
                {
                    p = p.Remove(0, p.IndexOf('#') + 1);
                    if (p.Length == 0) loaded = true;
                    else
                    {
                        string s = "/extranet/api/mycomputer/list/" + p.Split(new char[] { '/' })[0];
                        if (p.Split(new char[] { '/' }).Length > 1) GetTreeNode(s, treeView1.Items).IsExpanded = true;
                        else
                        {
                            loaded = true;
                            treeView1.SelectItem(GetTreeNode(s, treeView1.Items));
                        }
                    }
                }
                else loaded = true;
            }
        }

        private bool reload = false;
        private HAPTreeNode tempnode;

        private void HAPTreeNode_Selected(object sender, RoutedEventArgs e)
        {
            reload = true;
            CurrentItem = ((HAPTreeNode)treeView1.SelectedItem).BItem;
            ((HAPTreeNode)treeView1.SelectedItem).IsExpanded = true;
            WebClient listclient = new WebClient();
            listclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(listclient_DownloadStringCompleted);
            listclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + CurrentItem.Path));
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + HtmlPage.Document.DocumentUri.LocalPath + "#" + CurrentItem.Path.ToLower().Remove(0, 30)));
        }

        private void HAPTreeNode_Expanded(object sender, RoutedEventArgs e)
        {
            if (!reload)
            {
                tempnode = sender as HAPTreeNode;
                WebClient listclient = new WebClient();
                listclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(treereloadclient_DownloadStringCompleted);
                listclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + tempnode.BItem.Path));
            }
        }

        private HAPTreeNode GetTreeNode(string path, ItemCollection items)
        {
            HAPTreeNode item = new HAPTreeNode();
            item.Header = "E";
            foreach (HAPTreeNode i in items)
                if (i.Header.ToString() == "Loading...") break;
                else if (i.BItem.Path.ToLower() == path.ToLower())
                {
                    item = i;
                    break;
                }
                else if (i.HasItems)
                {
                    item = GetTreeNode(path, i.Items);
                    if (item.Header.ToString() != "E") break;
                }
            return item;
        }

        private void item_DirectoryChange(object sender, BItem e)
        {
            CurrentItem = e;
            HAPTreeNode i = GetTreeNode(e.Path, treeView1.Items);
            if (i.Header.ToString() != "E") treeView1.SelectItem(i);
            else MessageBox.Show("Error: " + e.Path);
        }

        private void computer_DirectoryChange(object sender, BItem e)
        {
            CurrentItem = new BItem("My Computer", "", "", "", BType.Drive, "", false);
            if (treeView1.SelectedItem != null) ((HAPTreeNode)treeView1.SelectedItem).IsSelected = false;
            WebClient listclient = new WebClient();
            listclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(driveclient_DownloadStringCompleted);
            listclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + e.Path));
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + HtmlPage.Document.DocumentUri.LocalPath + "#"));
        }

        private void listclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DownloadStringCompletedEventHandler(listclient_DownloadStringCompleted2), sender, e);
        }
        private void listclient_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            ClearItems();
            tempnode = ((HAPTreeNode)treeView1.SelectedItem);
            tempnode.Items.Clear();
            if (e.Result.StartsWith("ERROR")) MessageBox.Show("An Error Occured");
            else foreach (string s in e.Result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] ss = s.Split(new char[] { ',' });
                BrowserItem item = new BrowserItem(new BItem(ss[0], ss[1], ss[2], ss[3], (ss[3] == "File Folder" ? BType.Folder : (ss[3] == "Drive" ? BType.Drive : BType.File)), ss[4], bool.Parse(ss[5])));
                item.Activate += new EventHandler(item_Activate);
                item.MouseEnter += new MouseEventHandler(item_MouseEnter);
                if (CurrentItem.CanWrite)
                {
                    item.ItemDrop += new DragEventHandler(item_Drop);
                    item.DragEnter += new DragEventHandler(item_DragEnter);
                    item.DragLeave += new DragEventHandler(item_DragLeave);
                    item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                }
                item.MouseLeave += new MouseEventHandler(item_MouseLeave);
                item.ReSort += new ResortHandler(item_ReSort);
                if (item.Data.BType == BType.Drive) item.DirectoryChange += new ChangeDirectoryHandler(computer_DirectoryChange);
                else item.DirectoryChange += new ChangeDirectoryHandler(item_DirectoryChange);
                if (item.Data.BType == BType.Folder && item.Data.Name != "..") UpdateTree(item.Data);
                contentPan.Children.Add(item);
                item.Mode = ViewMode;
            }
            reload = false;
        }

        private void treereloadclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DownloadStringCompletedEventHandler(treereloadclient_DownloadStringCompleted2), sender, e);
        }
        private void treereloadclient_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            tempnode.Items.Clear();
            if (e.Result.StartsWith("ERROR")) MessageBox.Show("An Error Occured");
            else foreach (string s in e.Result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] ss = s.Split(new char[] { ',' });
                    BrowserItem item = new BrowserItem(new BItem(ss[0], ss[1], ss[2], ss[3], (ss[3] == "File Folder" ? BType.Folder : (ss[3] == "Drive" ? BType.Drive : BType.File)), ss[4], bool.Parse(ss[5])));
                    if (item.Data.BType == BType.Folder && item.Data.Name != "..") UpdateTree(item.Data);
                }
            if (!loaded)
            {
                string p = HtmlPage.Document.DocumentUri.AbsoluteUri;
                p = p.Replace("%20", " ").Remove(0, p.IndexOf('#') + 1);
                p = p.Remove(0, tempnode.BItem.Path.Remove(0, 30).Length + 1);
                if (p.Split(new char[] { '/' }).Length > 1)
                    GetTreeNode(tempnode.BItem.Path + "/" + p.Split(new char[] { '/' })[0], tempnode.Items).IsExpanded = true;
                else
                {
                    loaded = true;
                    Dispatcher.BeginInvoke(new SelectHandler(SelectUpdatedNode), tempnode.BItem.Path + "/" + p.Split(new char[] { '/' })[0]);
                } 
            }
            reload = false;
        }

        private void SelectUpdatedNode(string path)
        {
            treeView1.SelectItem(GetTreeNode(path, treeView1.Items));//.IsSelected = true;
        }

        private void UpdateTree(BItem bitem)
        {
            HAPTreeNode titem = new HAPTreeNode();
            titem.Cursor = Cursors.Hand;
            titem.Header = bitem.Name;
            titem.BItem = bitem;
            titem.Expanded += new RoutedEventHandler(HAPTreeNode_Expanded);
            titem.Selected += new RoutedEventHandler(HAPTreeNode_Selected);
            HAPTreeNode ttitem = new HAPTreeNode();
            ttitem.Header = "Loading...";
            titem.Items.Add(ttitem);
            tempnode.Items.Add(titem);
        }

        private void barContextButton1_Click(object sender, EventArgs e)
        {
            ViewMode = barContextButton1.Mode;
            if (ViewMode == ViewMode.List)
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
                item.Mode = ViewMode;
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
                    item_ReSort(this, false);
                    ((BrowserItem)sender).Active = true;
                    if (((BrowserItem)sender).Active) activeItems.Add((BrowserItem)sender);
                }
            }
            else { selectall = false; rightclick = false; }
        }

        private bool selectall = false;
        private void organiseButton1_SelectAllClick(object sender, RoutedEventArgs e)
        {
            item_ReSort(this, false);
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
                if (activeItems.Count(I => I.IsRename == true) > 0) item_ReSort(this, true);
                else item_ReSort(this, false);
            } else rightclick = false;
        }

        private void contentPan_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeItems.Count == 1 && activeItems[0].Data.Name != ".." && activeItems[0].Data.Name != "My Computer")
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = RightClickRename.IsEnabled = RightClickDelete.IsEnabled = CurrentItem.CanWrite;
                RightClickZIP.Visibility = RightClickFolder.Visibility = RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (activeItems.Count > 1 && activeItems.Where(I => I.Data.Name == "My Computer" || I.Data.Name == "..").Count() == 0)
            {
                organiseButton1.IsDelete = RightClickDelete.IsEnabled = CurrentItem.CanWrite;
                organiseButton1.IsRename = RightClickRename.IsEnabled = false;
                RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = RightClickZIP.Visibility = CurrentItem.CanWrite ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = false;
                RightClickRename.IsEnabled = RightClickDelete.IsEnabled = false;
                RightClickUNZIP.Visibility = RightClickZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = CurrentItem.CanWrite ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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
            item_ReSort(this, true);
            int count = 0;
            foreach (BrowserItem i in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.Folder))
                if (i.Data.Name.StartsWith("New Folder")) count++;
            BrowserItem item = new BrowserItem(new BItem("New Folder" + (count == 0 ? "" : " " + count), "/extranet/images/icons/folder.png", "", "Folder", BType.Folder, CurrentItem.Path + "/New Folder" + (count == 0 ? "" : " " + (count + 1)), true));
            item.ReSort += new ResortHandler(item_ReSort);
            item.Activate += new EventHandler(item_Activate);
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            contentPan.Children.Add(item);
            scroller.ScrollToBottom();
            item.Mode = ViewMode;
            item.Active = true;
            item.IsRename = true;
            activeItems.Add(item);
        }

        private void item_ReSort(object sender, bool resort)
        {
            foreach (BrowserItem item in activeItems)
                if (item.IsRename)
                    if (item.Save(false) == 0) item.Active = false;
                    else return;
                else item.Active = false;
            activeItems.Clear();
            if (resort)
            {
                List<BrowserItem> drives = new List<BrowserItem>();
                foreach (BrowserItem item in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.Drive))
                    drives.Add(item);
                drives.Sort();
                List<BrowserItem> folders = new List<BrowserItem>();
                foreach (BrowserItem item in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.Folder))
                    folders.Add(item);
                folders.Sort();
                List<BrowserItem> files = new List<BrowserItem>();
                foreach (BrowserItem item in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == BType.File))
                    files.Add(item);
                files.Sort();
                contentPan.Children.Clear();
                foreach (BrowserItem item in drives) contentPan.Children.Add(item);
                foreach (BrowserItem item in folders) contentPan.Children.Add(item);
                foreach (BrowserItem item in files) contentPan.Children.Add(item);
                if (sender is BrowserItem) scroller.ScrollIntoView((BrowserItem)sender);
            }
        }

        private void RightClickDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ERG");
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
            //MessageBox.Show(string.Join("\n", e.Data.GetFormats(false)));
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

        private void organiseButton1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeItems.Count == 1 && activeItems[0].Data.Name != ".." && activeItems[0].Data.Name != "My Computer")
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = RightClickRename.IsEnabled = RightClickDelete.IsEnabled = CurrentItem.CanWrite;
                RightClickZIP.Visibility = RightClickFolder.Visibility = RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (activeItems.Count > 1 && activeItems.Where(I => I.Data.Name == "My Computer" || I.Data.Name == "..").Count() == 0)
            {
                organiseButton1.IsDelete = RightClickDelete.IsEnabled = CurrentItem.CanWrite;
                organiseButton1.IsRename = RightClickRename.IsEnabled = false;
                RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = RightClickZIP.Visibility = CurrentItem.CanWrite ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = false;
                RightClickRename.IsEnabled = RightClickDelete.IsEnabled = false;
                RightClickUNZIP.Visibility = RightClickZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = CurrentItem.CanWrite ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
    }

    public enum ViewMode { List, SmallIcon, Icon, LargeIcon, Tile }
    public enum MouseMode { NoGo, Move, Normal, Upload }
}

public delegate void ResortHandler(object sender, bool resort);
public delegate void SelectHandler(string path);