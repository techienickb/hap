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
using System.Windows.Media.Imaging;
using HAP.Silverlight.Browser.service;
using System.ServiceModel;

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
            CurrentItem = new BItem(service.BType.Root, "My School Computer", service.AccessControlActions.None);
            newfolder.Visibility = System.Windows.Visibility.Collapsed;
            HAPTreeNode root = new HAPTreeNode();
            root.Data = CurrentItem;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            sp.Cursor = Cursors.Hand;
            Image img = new Image();
            img.Margin = new Thickness(0, 0, 4, 0);
            img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/mycomp.png", UriKind.Relative));
            sp.Children.Add(img);
            TextBlock tb = new TextBlock();
            tb.Text = CurrentItem.Name;
            sp.Children.Add(tb);
            root.Header = sp;
            treeView1.Items.Add(root);
            soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.ListCompleted += new EventHandler<ListCompletedEventArgs>(soap_ListCompleted);
            soap.ListDrivesCompleted += new EventHandler<ListDrivesCompletedEventArgs>(soap_ListDrivesCompleted);
            soap.NewFolderCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_NewFolderCompleted);
        }

        public apiSoapClient soap { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((HAPTreeNode)treeView1.Items[0]).IsExpanded = true;
            ((HAPTreeNode)treeView1.Items[0]).IsSelected = true;
            ((HAPTreeNode)treeView1.Items[0]).Selected += new RoutedEventHandler(root_Selected);
            try
            {
                this.AllowDrop = true;
            }
            catch 
            {
                if (HtmlPage.BrowserInformation.ProductName == "Firefox" && new Version(HtmlPage.BrowserInformation.ProductVersion) > new Version(3, 6, 3))
                    new FirefoxMessage().Show();
            }
            try
            {
                this.AllowDrop = false;
            }
            catch { }
            soap.ListDrivesAsync();
        }

        #region Variables

        private bool loaded;
        public List<string> Filter { get; set; }
        public string Info { get; set; }
        private BItem CurrentItem { get; set; }
        private List<BItem> Drives { get; set; }
        public ViewMode ViewMode { get; set; }
        private bool reload = false;
        private HAPTreeNode tempnode;
        private bool candrag = false;
        private bool drag = false;
        private MouseMode mousemode = MouseMode.Normal;
        private BItem uploadItem { get; set; }

        private bool selectall = false;
        private bool rightclick = false;
        private List<BrowserItem> activeItems;

        delegate void ClearItemsDelegate();
        delegate void AddItemDelegate(BrowserItem item, bool Resort);

        #endregion

        #region drive/list events

        private void SetDrop(bool val)
        {
            this.AllowDrop = contentPan.AllowDrop = RightClickFolder.IsEnabled = val;
            newfolder.Visibility = val ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void AddItem(BrowserItem item)
        {
            contentPan.Children.Add(item);
            item.Mode = ViewMode;
        }

        private void AddTreeItem(HAPTreeNode currentnode, HAPTreeNode newnode)
        {
            currentnode.Items.Add(newnode);
        }

        void soap_ListDrivesCompleted(object sender, ListDrivesCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new SetBool(SetDrop), CurrentItem.AccessControl == service.AccessControlActions.Change);
            if (loaded) Dispatcher.BeginInvoke(new Action(ClearItems));
            foreach (service.UploadFilter filter in e.Result.Filters)
            {
                string f = string.Format("{0} ({2})|{1}", filter.Name, filter.Filter, filter.Filter.Replace(";", ","));
                if (!Filter.Contains(f)) Filter.Add(f);
            }
            Info = string.Format("Name:{0}|Version:{1}", e.Result.HAPName, e.Result.HAPVerion);
            foreach (service.ComputerBrowserAPIItem i in e.Result.Items)
            {
                BItem bitem = new BItem(i);
                bitem.Changed += new BItemChangeHandler(bitem_Changed);
                bitem.Deleted += new BItemChangeHandler(bitem_Delete);
                BrowserItem item = new BrowserItem(bitem);
                if (!loaded)
                {
                    HAPTreeNode titem = new HAPTreeNode();
                    BStackPanel sp = new BStackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    sp.MouseLeftButtonUp += new MouseButtonEventHandler(HAPTreeNode_MouseLeftButtonUp);
                    sp.AllowDrop = bitem.AccessControl == service.AccessControlActions.Change;
                    if (sp.AllowDrop)
                    {
                        sp.DragEnter += new DragEventHandler(titem_DragEnter);
                        sp.DragLeave += new DragEventHandler(titem_DragLeave);
                        sp.Drop += new DragEventHandler(contentPan_Drop);
                        sp.DragOver += new DragEventHandler(sp_DragOver);
                    }
                    sp.Cursor = Cursors.Hand;
                    sp.MouseEnter += new MouseEventHandler(HAPTreeNode_MouseEnter);
                    sp.MouseLeave += new MouseEventHandler(HAPTreeNode_MouseLeave);
                    sp.Data = bitem;
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/netdrive.png", UriKind.Relative));
                    sp.Children.Add(img);
                    TextBlock tb = new TextBlock();
                    tb.Padding = new Thickness(4, 0, 0, 0);
                    tb.Text = bitem.Name;
                    sp.Children.Add(tb);
                    titem.Header = sp;
                    titem.Data = bitem;
                    titem.Expanded += new RoutedEventHandler(HAPTreeNode_Expanded);
                    titem.Unselected += new RoutedEventHandler(HAPTreeNode_Unselected);
                    titem.Selected += new RoutedEventHandler(HAPTreeNode_Selected);
                    HAPTreeNode ttitem = new HAPTreeNode();
                    ttitem.Header = "Loading...";
                    titem.Items.Add(ttitem);
                    Dispatcher.BeginInvoke(new AddTeeeHandler(AddTreeItem), (HAPTreeNode)treeView1.Items[0], titem);
                    //((HAPTreeNode)treeView1.Items[0]).Items.Add(titem);
                }
                item.KeyUp += new KeyEventHandler(item_KeyUp);
                item.MouseEnter += new MouseEventHandler(item_MouseEnter);
                item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                item.MouseLeave += new MouseEventHandler(item_MouseLeave);
                item.Activate += new EventHandler(item_Activate);
                item.ReSort += new ResortHandler(item_ReSort);
                item.DirectoryChange += new ChangeDirectoryHandler(item_DirectoryChange);
                Dispatcher.BeginInvoke(new AddItemHandler(AddItem), item);
                //contentPan.Children.Add(item);
                item.Mode = ViewMode;
            }
        if (!loaded) Dispatcher.BeginInvoke(new Action(ContinueLoad));
        }

        private void ContinueLoad()
        {
            string p = HtmlPage.Document.DocumentUri.AbsoluteUri;
            if (p.Contains('#'))
            {
                p = p.Remove(0, p.IndexOf('#') + 1);
                if (p.Length == 0) loaded = true;
                else
                {
                    string s = p.Split(new char[] { '/' })[0];
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

        private void sp_DragOver(object sender, DragEventArgs e)
        {
            movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
            if (((IBitem)sender).Data.AccessControl == service.AccessControlActions.Change)
            {
                movetooltip.Visibility = System.Windows.Visibility.Visible;
                nomove.Visibility = System.Windows.Visibility.Collapsed;
                ((StackPanel)sender).Background = Resources["hover"] as LinearGradientBrush;
            }
            else
            {
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                nomove.Visibility = System.Windows.Visibility.Visible;
            }
            e.Handled = true;
        }

        private void titem_DragEnter(object sender, DragEventArgs e)
        {
            if (((IBitem)sender).Data.AccessControl == service.AccessControlActions.Change)
            {
                mousemode = MouseMode.Upload;
                nomove.Visibility = System.Windows.Visibility.Visible;
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                movetext.Text = "Upload to";
                movefoldertext.Text = ((IBitem)sender).Data.Name;
                uploadItem = ((IBitem)sender).Data;
                e.Handled = true;
            }
        }

        private void titem_DragLeave(object sender, DragEventArgs e)
        {
            ((StackPanel)sender).Background = new SolidColorBrush(Colors.Transparent);
            mousemode = MouseMode.NoGo;
            uploadItem = CurrentItem;
            if (CurrentItem.AccessControl == service.AccessControlActions.Change) movefoldertext.Text = "This Folder";
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            nomove.Visibility = System.Windows.Visibility.Visible;
        }

        private void ClearNode()
        {
            tempnode = ((HAPTreeNode)treeView1.SelectedItem);
            tempnode.Items.Clear();
        }

        private void soap_ListCompleted(object sender, ListCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(ClearItems));
            Dispatcher.BeginInvoke(new SetBool(SetDrop), CurrentItem.AccessControl == service.AccessControlActions.Change);
            Dispatcher.BeginInvoke(new Action(ClearNode));

            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                foreach (service.ComputerBrowserAPIItem i in e.Result)
                {
                    BItem bitem = new BItem(i);
                    bitem.Changed += new BItemChangeHandler(bitem_Changed);
                    bitem.Deleted += new BItemChangeHandler(bitem_Delete);
                    BrowserItem item = new BrowserItem(bitem);
                    item.Activate += new EventHandler(item_Activate);
                    item.MouseEnter += new MouseEventHandler(item_MouseEnter);
                    if (CurrentItem.AccessControl == service.AccessControlActions.Change)
                    {
                        item.DragEnter += new DragEventHandler(item_DragEnter);
                        item.DragLeave += new DragEventHandler(item_DragLeave);
                        item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                        item.KeyUp += new KeyEventHandler(item_KeyUp);
                    }
                    item.MouseLeave += new MouseEventHandler(item_MouseLeave);
                    item.ReSort += new ResortHandler(item_ReSort);
                    //if (item.Data.BType == BType.Drive) item.DirectoryChange += new ChangeDirectoryHandler(computer_DirectoryChange);
                    //else 
                    item.DirectoryChange += new ChangeDirectoryHandler(item_DirectoryChange);
                    if (item.Data.BType == service.BType.Folder && item.Data.Name != "..") Dispatcher.BeginInvoke(new UpdateTree(UpdateTree), item.Data);
                    Dispatcher.BeginInvoke(new AddItemHandler(AddItem), item);
                }
            }
            reload = false;
        }

        #endregion

        #region Tree Events

        private void root_Selected(object sender, RoutedEventArgs e)
        {
            CurrentItem = ((HAPTreeNode)sender).Data;
            soap.ListDrivesAsync();
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "mycomputersl.aspx#"));
        }

        private void HAPTreeNode_Selected(object sender, RoutedEventArgs e)
        {
            reload = true;
            CurrentItem = ((HAPTreeNode)treeView1.SelectedItem).Data;
            ((HAPTreeNode)treeView1.SelectedItem).IsExpanded = true;
            if (CurrentItem.BType == service.BType.Folder)
            {
                StackPanel sp = ((HAPTreeNode)treeView1.SelectedItem).Header as StackPanel;
                Image img = sp.Children[0] as Image;
                img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/folderopen.png", UriKind.Relative));
            }
            soap.ListAsync(CurrentItem.Path);
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "mycomputersl.aspx#" + CurrentItem.Path));
        }

        private void HAPTreeNode_Expanded(object sender, RoutedEventArgs e)
        {
            if (!reload)
            {
                tempnode = sender as HAPTreeNode;
                if (tempnode.Data.BType == service.BType.Folder)
                {
                    StackPanel sp = tempnode.Header as StackPanel;
                    Image img = sp.Children[0] as Image;
                    img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/folderopen.png", UriKind.Relative));
                }
                apiSoapClient asoap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
                asoap.ListCompleted += new EventHandler<ListCompletedEventArgs>(asoap_ListCompleted);
                asoap.ListAsync(tempnode.Data.Path);
            }
        }

        private void HAPTreeNode_Collapsed(object sender, RoutedEventArgs e)
        {
            HAPTreeNode node = sender as HAPTreeNode;
            if (node.Data.BType == service.BType.Folder)
            {
                StackPanel sp = node.Header as StackPanel;
                Image img = sp.Children[0] as Image;
                img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/folderclosed.png", UriKind.Relative));
            }
        }

        private void HAPTreeNode_Unselected(object sender, RoutedEventArgs e)
        {
            HAPTreeNode node = sender as HAPTreeNode;
            if (node.Data.BType == service.BType.Folder && (node.HasItems && node.IsExpanded))
            {
                StackPanel sp = node.Header as StackPanel;
                Image img = sp.Children[0] as Image;
                img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/folderclosed.png", UriKind.Relative));
            }
        }

        private void HAPTreeNode_MouseEnter(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                if (((IBitem)sender).Data.BType == service.BType.Folder || ((IBitem)sender).Data.BType == service.BType.Drive)
                {
                    bool cont = false;
                    foreach (BrowserItem i in activeItems)
                        if (i.Data == ((IBitem)sender).Data) cont = true;
                    if (cont) mousemode = MouseMode.NoGo;
                    else
                    {
                        movetext.Text = "Move to";
                        movefoldertext.Text = ((IBitem)sender).Data.Name;
                        mousemode = MouseMode.Move;
                    }
                }
                else mousemode = MouseMode.NoGo;
            }
        }

        private void HAPTreeNode_MouseLeave(object sender, MouseEventArgs e)
        {
            mousemode = MouseMode.NoGo;
        }

        private void HAPTreeNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (drag && mousemode == MouseMode.Move)
            {
                foreach (BrowserItem item in activeItems)
                {
                    item.Active = false;
                    item.Move(true, ((IBitem)sender).Data.Path);
                }
                activeItems.Clear();
            }
            candrag = drag = false; mousemode = MouseMode.Normal;
        }

        private HAPTreeNode GetTreeNode(string path, ItemCollection items)
        {
            HAPTreeNode item = new HAPTreeNode();
            item.Header = "E";
            foreach (HAPTreeNode i in items)
                if (i.Header.ToString() == "Loading...") break;
                else if (i.Data.Path.ToLower() == path.ToLower())
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

        void asoap_ListCompleted(object sender, ListCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<ListCompletedEventArgs>(asoap_ListCompleted2), sender, e);
        }

        void asoap_ListCompleted2(object sender, ListCompletedEventArgs e)
        {
            tempnode.Items.Clear();
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else foreach (ComputerBrowserAPIItem i in e.Result)
            {
                BItem bitem = new BItem(i);
                bitem.Deleted += new BItemChangeHandler(bitem_Delete);
                bitem.Changed += new BItemChangeHandler(bitem_Changed);
                BrowserItem item = new BrowserItem(bitem);
                if (item.Data.BType == service.BType.Folder && item.Data.Name != "..") UpdateTree(item.Data);
            }
            if (!loaded)
            {
                string p = HtmlPage.Document.DocumentUri.AbsoluteUri;
                p = p.Replace("%20", " ").Remove(0, p.IndexOf('#') + 1);
                p = p.Remove(0, tempnode.Data.Path.Remove(0, 20).Length + 1);
                if (p.Split(new char[] { '/' }).Length > 1)
                    GetTreeNode(tempnode.Data.Path + "/" + p.Split(new char[] { '/' })[0], tempnode.Items).IsExpanded = true;
                else
                {
                    loaded = true;
                    SelectUpdatedNode(tempnode.Data.Path + "/" + p.Split(new char[] { '/' })[0]);
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
            BStackPanel sp = new BStackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            sp.MouseEnter += new MouseEventHandler(HAPTreeNode_MouseEnter);
            sp.MouseLeave += new MouseEventHandler(HAPTreeNode_MouseLeave);
            sp.MouseLeftButtonUp += new MouseButtonEventHandler(HAPTreeNode_MouseLeftButtonUp);
            sp.AllowDrop = bitem.AccessControl == service.AccessControlActions.Change;
            if (sp.AllowDrop)
            {
                sp.DragEnter += new DragEventHandler(titem_DragEnter);
                sp.DragLeave += new DragEventHandler(titem_DragLeave);
                sp.Drop += new DragEventHandler(contentPan_Drop);
                sp.DragOver += new DragEventHandler(sp_DragOver);
            }
            sp.Data = bitem;
            sp.Cursor = Cursors.Hand;
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("/HAP.Silverlight.Browser;component/folderclosed.png", UriKind.Relative));
            sp.Children.Add(img);
            TextBlock tb = new TextBlock();
            tb.Padding = new Thickness(4, 0, 0, 0);
            tb.Text = bitem.Name;
            sp.Children.Add(tb);
            titem.Header = sp;
            titem.Data = bitem;
            titem.Unselected += new RoutedEventHandler(HAPTreeNode_Unselected);
            titem.Expanded += new RoutedEventHandler(HAPTreeNode_Expanded);
            titem.Collapsed += new RoutedEventHandler(HAPTreeNode_Collapsed);
            titem.Selected += new RoutedEventHandler(HAPTreeNode_Selected);
            HAPTreeNode ttitem = new HAPTreeNode();
            ttitem.Header = "Loading...";
            titem.Items.Add(ttitem);
            tempnode.Items.Add(titem);
        }

        #endregion

        #region Bar/Context Events

        private void back_Click(object sender, RoutedEventArgs e)
        {
            DateTime expireDate = DateTime.Now.AddDays(1);
            string newCookie = "mycompv=html;path=/;expires=" + expireDate.ToString("R");
            HtmlPage.Document.SetProperty("cookie", newCookie);
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "mycomputer.aspx"));
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

        private void home_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "./"));
        }

        private void helpButton1_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.HAPV = Info;
            help.Show();
        }

        private void organiseButton1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeItems.Count == 1 && activeItems[0].Data.Name != ".." && activeItems[0].Data.Name != "My Computer")
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = RightClickRename.IsEnabled = RightClickDelete.IsEnabled = CurrentItem.AccessControl == service.AccessControlActions.Change;
                RightClickZIP.Visibility = RightClickFolder.Visibility = RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (activeItems.Count > 1 && activeItems.Where(I => I.Data.Name == "My Computer" || I.Data.Name == "..").Count() == 0)
            {
                organiseButton1.IsDelete = RightClickDelete.IsEnabled = CurrentItem.AccessControl == service.AccessControlActions.Change;
                organiseButton1.IsRename = RightClickRename.IsEnabled = false;
                RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = RightClickZIP.Visibility = CurrentItem.AccessControl == service.AccessControlActions.Change ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = false;
                RightClickRename.IsEnabled = RightClickDelete.IsEnabled = false;
                RightClickUNZIP.Visibility = RightClickZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = CurrentItem.AccessControl == service.AccessControlActions.Change ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        #endregion

        #region new folder

        private void newfolder_Click(object sender, RoutedEventArgs e)
        {
            if (sender == RightClickFolder) rightclick = true;
            item_ReSort(this, true);
            int count = 0;
            foreach (BrowserItem i in contentPan.Children.Where(I => ((BrowserItem)I).Data.BType == service.BType.Folder))
                if (i.Data.Name.StartsWith("New Folder")) count++;
            BItem bitem = new BItem(BType.Folder, "New Folder" + (count == 0 ? "" : " " + (count + 1)), AccessControlActions.Change);
            bitem.Path = CurrentItem.Path + "/New Folder" + (count == 0 ? "" : " " + (count + 1));
            bitem.Icon = "images/icons/Newfolder.png";
            bitem.Type = "File Folder";
            BrowserItem item = new BrowserItem(bitem);
            item.Activate += new EventHandler(item_Activate);
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            if (CurrentItem.AccessControl == service.AccessControlActions.Change)
            {
                item.DragEnter += new DragEventHandler(item_DragEnter);
                item.DragLeave += new DragEventHandler(item_DragLeave);
                item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                item.KeyUp += new KeyEventHandler(item_KeyUp);
            }
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            item.ReSort += new ResortHandler(item_ReSort);
            item.DirectoryChange += new ChangeDirectoryHandler(item_DirectoryChange);
            if (item.Data.BType == service.BType.Folder && item.Data.Name != "..") UpdateTree(item.Data);
            contentPan.Children.Add(item);
            scroller.ScrollIntoView(item);
            item.Mode = ViewMode;
            item.Active = true;
            item.IsRename = true;
            foreach (BrowserItem i in activeItems)
                if (i.IsRename)
                    if (i.Save(false) == 0) i.Active = false;
                    else return;
                else i.Active = false;
            activeItems.Clear();
            activeItems.Add(item);
            soap.NewFolderAsync(CurrentItem.Path, bitem.Name);
        }

        void soap_NewFolderCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
             Dispatcher.BeginInvoke(new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_NewFolderCompleted2), sender, e);
        }

        void soap_NewFolderCompleted2(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                contentPan.Children.Remove(activeItems[0]);
                activeItems.Clear();
                MessageBox.Show(e.Error.ToString());
            }
        }

        #endregion

        #region ItemEvents

        void bitem_Delete(BItem e)
        {
            try
            {
                HAPTreeNode item = GetTreeNode(e.Path, ((TreeViewItem)treeView1.Items[0]).Items);
                ((HAPTreeNode)item.Parent).Items.Remove(item);
            }
            catch { }
        }

        private void bitem_Changed(BItem e)
        {
            try
            {
                HAPTreeNode item = GetTreeNode(e.Path, ((TreeViewItem)treeView1.Items[0]).Items);
                StackPanel sp = item.Header as StackPanel;
                TextBlock tb = sp.Children[1] as TextBlock;
                tb.Text = e.Name;
            }
            catch { }
        }

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

        private void item_Activate(object sender, EventArgs e)
        {
            if (!selectall && !rightclick)
            {
                if (drag && mousemode == MouseMode.Move)
                {
                    foreach (BrowserItem item in activeItems)
                    {
                        item.Active = false;
                        item.Move(true, ((IBitem)sender).Data.Path);
                    }
                    activeItems.Clear();
                }
                candrag = drag = false; mousemode = MouseMode.Normal;
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

        private void item_DirectoryChange(object sender, BItem e)
        {
            CurrentItem = e;
            HAPTreeNode i = GetTreeNode(e.Path, treeView1.Items);
            if (i.Header.ToString() != "E") treeView1.SelectItem(i);
            else MessageBox.Show("Error: " + e.Path);
        }

        private void item_KeyUp(object sender, KeyEventArgs e)
        {
            BrowserItem item = ((BrowserItem)sender);
            if (item.Active && !item.IsRename)
            {
                if (e.Key == Key.Delete)
                {
                    if (MessageBox.Show("Are you sure you want to delete?", "Delete?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        item.Active = false;
                        item.Delete();
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter && item.Data.BType != service.BType.File)
                {
                    CurrentItem = item.Data;
                    HAPTreeNode i = GetTreeNode(item.Data.Path, treeView1.Items);
                    if (i.Header.ToString() != "E") treeView1.SelectItem(i);
                    else MessageBox.Show("Error: " + item.Data.Path);
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter && item.Data.BType == service.BType.File)
                {
                    HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, item.Data.Path));
                    e.Handled = true;
                }
            }
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

        private void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (BrowserItem item in activeItems) if (item.Data.Name == "..") { candrag = false; return; }
            if (((BrowserItem)sender).Data.Name == "..") { candrag = false; return; }
            candrag = true;
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (candrag)
            {
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
                if (((BrowserItem)sender).Data.BType == service.BType.Folder)
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

        private void item_DragLeave(object sender, DragEventArgs e)
        {
            uploadItem = CurrentItem;
            if (CurrentItem.AccessControl == service.AccessControlActions.Change) movefoldertext.Text = "This Folder";
            if (sender is BrowserItem) ((BrowserItem)sender).Leave();
        }

        private void item_DragEnter(object sender, DragEventArgs e)
        {
            if (CurrentItem.AccessControl == service.AccessControlActions.Change)
            {
                mousemode = MouseMode.Upload;
                nomove.Visibility = System.Windows.Visibility.Visible;
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                movetext.Text = "Upload to";
                uploadItem = ((IBitem)sender).Data;
                movefoldertext.Text = uploadItem.Name;
                if (sender is BrowserItem) ((BrowserItem)sender).Hover();
                e.Handled = true;
            }
        }

        #endregion

        #region Right Click Events

        private void contentPan_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeItems.Count == 1)
            {
                organiseButton1.IsDelete = organiseButton1.IsRename = RightClickRename.IsEnabled = RightClickDelete.IsEnabled = ((activeItems[0].Data.BType == service.BType.Drive) ? false : CurrentItem.AccessControl == service.AccessControlActions.Change);
                RightClickDownload.Visibility = activeItems[0].Data.BType == service.BType.File ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                RightClickUpload.Visibility = (activeItems[0].Data.BType == service.BType.Folder && activeItems[0].Data.AccessControl == service.AccessControlActions.Change) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                RightClickUpload.Header = "Upload to " + activeItems[0].Data.Name;
                if (activeItems[0].Data.Type == "Compressed (zipped) Folder")
                    RightClickUNZIP.Visibility = CurrentItem.AccessControl == service.AccessControlActions.Change ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                else RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickZIP.Visibility = RightClickFolder.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (activeItems.Count > 1)
            {
                organiseButton1.IsDelete = RightClickDelete.IsEnabled = CurrentItem.AccessControl == service.AccessControlActions.Change;
                organiseButton1.IsRename = RightClickRename.IsEnabled = false;
                RightClickDownload.Visibility = RightClickUpload.Visibility = RightClickUNZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = RightClickZIP.Visibility = CurrentItem.AccessControl == service.AccessControlActions.Change ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                RightClickUpload.Header = "Upload to " + CurrentItem.Name;
                organiseButton1.IsDelete = organiseButton1.IsRename = RightClickRename.IsEnabled = RightClickDelete.IsEnabled = false;
                RightClickDownload.Visibility = RightClickUNZIP.Visibility = RightClickZIP.Visibility = System.Windows.Visibility.Collapsed;
                RightClickFolder.Visibility = RightClickUpload.Visibility = CurrentItem.AccessControl == service.AccessControlActions.Change ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }


        private void RightClickUpload_Click(object sender, RoutedEventArgs e)
        {
            rightclick = true;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = string.Join("|", Filter.ToArray());
            dlg.Multiselect = true;
            if (dlg.ShowDialog().Value)
                foreach (FileInfo file in dlg.Files)
                    new UploadItem(file, activeItems.Count == 1 ? activeItems[0].Data : CurrentItem, ref UploadQueue, new RoutedEventHandler(ui_Uploaded));
        }

        private void RightClickDownload_Click(object sender, RoutedEventArgs e)
        {
            rightclick = true;
            HtmlPage.PopupWindow(new Uri(HtmlPage.Document.DocumentUri, activeItems[0].Data.Path), "_download", new HtmlPopupWindowOptions());
        }

        private void RightClickZIP_Click(object sender, RoutedEventArgs e)
        {
            rightclick = true;
            List<BItem> bitems = new List<BItem>();
            foreach (BrowserItem i in activeItems)
                bitems.Add(i.Data);
            ZipQuestion zq = new ZipQuestion(bitems.ToArray(), CurrentItem);
            zq.ZipQuestionComplete += new RoutedEventHandler(zipcompletedhandler);
            zq.Show();
        }

        private void RightClickUNZIP_Click(object sender, RoutedEventArgs e)
        {
            rightclick = true;
            UnZip uz = new UnZip(activeItems[0].Data, CurrentItem, new RoutedEventHandler(zipcompletedhandler));
            uz.Completed += new RoutedEventHandler(zipcompletedhandler);
            uz.Show();
        }

        private void zipcompletedhandler(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new RoutedEventHandler(zipcompletedhandler2), sender, e);
        }

        private void zipcompletedhandler2(object sender, RoutedEventArgs e)
        {
            soap.ListAsync(CurrentItem.Path);
            HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "mycomputersl.aspx#" + CurrentItem.Path));
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
            }
            activeItems.Clear();
        }

        private void RightClickRename_Click(object sender, RoutedEventArgs e)
        {
            activeItems[0].IsRename = true;
            rightclick = true;
        }

        #endregion

        #region Drag Events

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag && (mousemode == MouseMode.NoGo || mousemode == MouseMode.Move))
            {
                if (mousemode == MouseMode.NoGo)
                {
                    nomove.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
                    nomove.Visibility = System.Windows.Visibility.Visible;
                    movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (mousemode == MouseMode.Move)
                {
                    movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
                    movetooltip.Visibility = System.Windows.Visibility.Visible;
                    nomove.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (ViewMode == Browser.ViewMode.List)
                {
                    if (e.GetPosition(scroller).X < 10) scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - (10 - e.GetPosition(scroller).X));
                    else if (e.GetPosition(scroller).X > (scroller.ActualWidth - 10)) scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + (10 - (scroller.ActualHeight - e.GetPosition(scroller).X)));
                }
                else
                {
                    if (e.GetPosition(scroller).Y < 10) scroller.ScrollToVerticalOffset(scroller.VerticalOffset - (10 - e.GetPosition(scroller).Y));
                    else if (e.GetPosition(scroller).Y > (scroller.ActualHeight - 20)) scroller.ScrollToVerticalOffset(scroller.VerticalOffset + (10 - (scroller.ActualHeight - e.GetPosition(scroller).Y)));
                }
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

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            mousemode = MouseMode.Normal;
            movetooltip.Visibility = nomove.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void contentPan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            if (!rightclick)
            {
                if (activeItems.Count(I => I.IsRename == true) > 0) item_ReSort(this, true);
                else item_ReSort(this, false);
            }
            else rightclick = false;
        }

        private void contentPan_DragEnter(object sender, DragEventArgs e)
        {
            mousemode = MouseMode.Upload;
            nomove.Visibility = System.Windows.Visibility.Visible;
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            movetext.Text = "Upload to";
            movefoldertext.Text = "This Folder";
            uploadItem = CurrentItem;
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            //MessageBox.Show(string.Join("\n", e.Data.GetFormats(false)));
            movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
            if (CurrentItem.AccessControl == service.AccessControlActions.Change)
            {
                movetext.Text = "Upload to";
                movefoldertext.Text = "This Folder";
                movetooltip.Visibility = System.Windows.Visibility.Visible;
                nomove.Visibility = System.Windows.Visibility.Collapsed;
                uploadItem = CurrentItem;
            }
            else
            {
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                nomove.Visibility = System.Windows.Visibility.Visible;
                uploadItem = null;
            }
            e.Handled = true;
        }

        private void contentPan_DragOver(object sender, DragEventArgs e)
        {
            movetooltip.Margin = new Thickness(e.GetPosition(this).X + 10, e.GetPosition(this).Y + 10, 0, 0);
            if (CurrentItem.AccessControl == service.AccessControlActions.Change)
            {
                movetooltip.Visibility = System.Windows.Visibility.Visible;
                nomove.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                movetooltip.Visibility = System.Windows.Visibility.Collapsed;
                nomove.Visibility = System.Windows.Visibility.Visible;
            }
            e.Handled = true;
        }

        private void contentPan_DragLeave(object sender, DragEventArgs e)
        {
            nomove.Visibility = System.Windows.Visibility.Visible;
            movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            uploadItem = null;
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
            if (sender is StackPanel) ((StackPanel)sender).Background = new SolidColorBrush(Colors.Transparent);
            if (e.Data == null) return;
            if (uploadItem == null) return;

            try
            {

                FileInfo[] files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                if (files == null) return;

                foreach (FileInfo file in files)
                {
                    if (HasFilter(file.Extension)) { UploadItem ui = new UploadItem(file, uploadItem, ref UploadQueue, new RoutedEventHandler(ui_Uploaded)); }
                    else MessageBox.Show("File Type Not Allowed (" + file.Extension.ToLower() + ")", "This file is not allowed to be uploaded", MessageBoxButton.OK);
                }
            }
            catch { MessageBox.Show("The item you have dragged here is not supported"); }
            nomove.Visibility = movetooltip.Visibility = System.Windows.Visibility.Collapsed;
            drag = false;            
        }

        public void ui_Uploaded(object sender, RoutedEventArgs e)
        {
            UploadItem item = sender as UploadItem;
            if (CurrentItem == item.ParentData)
            {
                reload = true;
                soap.ListAsync(CurrentItem.Path);
                HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri, "mycomputersl.aspx#" + CurrentItem.Path));
            }
            UploadQueue.Children.Remove(item);
            if (UploadQueue.Children.Count > 0)
            {
                UploadItem i = UploadQueue.Children[0] as UploadItem;
                if (i.State == UploadItemState.Ready) i.Upload();
            }
        }

        #endregion

        private void contentPan_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) RightClickDelete_Click(RightClickDelete, new RoutedEventArgs());
        }

        private bool HasFilter(string extension)
        {
            foreach (string s in Filter)
                if (s.Contains("*.*")) return true;
                else if (s.ToLower().Contains(extension.ToLower())) return true;
            return false;
        }
    }
}