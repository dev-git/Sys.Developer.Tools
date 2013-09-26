using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Collections.ObjectModel;

using Sys.Developer.Tools.Fcd;
using Sys.Developer.Tools.Fcd.ServiceContracts;
using Sys.Developer.Tools.Wpf.ViewModel;

namespace Sys.Developer.Tools.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FacadeService facadeService = new FacadeService();
        private SqlServerInstanceDto sqlServerInstanceDto = new SqlServerInstanceDto();
        private TableDto tableDto = new TableDto();
        private IEnumerable<SqlServerInstanceDto> sqlServerInstances = null;
        private const string masterConnString = "server={0};initial catalog={1};user id=sa;password=Passw0rd;";
        private const String PosrunPath = @"C:\InfinityPOS\posrun.ini";
        private SqlServerTreeViewModel ssvm = null;

        private ContextMenu TableListMenu = null;
        private bool CanLoadData = false;

        private ObservableCollection<TableViewModel> tvm = null;

        public MainWindow()
        {
            InitializeComponent();

            LoadSqlServers(true);

            LoadPosrun();
        }

        private void LoadPosrun()
        {
            if (File.Exists(PosrunPath))
            {
                txtPosrun.Clear();
                string[] lines = File.ReadAllLines(PosrunPath);
                foreach (string line in lines)
                {
                    txtPosrun.Text += String.Format("{0}\n", line);
                }
            }
        }

        private void LoadSqlServers(bool localOnly)
        {
            AddLog(String.Format("Loading {0} servers...", localOnly ? "local" : "all"));
            lstSqlServer.Items.Clear();
            trvSqlDatabase.Items.Clear();

            sqlServerInstances = GetSqlServerInstances(facadeService, localOnly);
            //IEnumerable<SqlServerInstanceDto> sqlSerInst = GetSqlServerInstances(facadeService, localOnly);
            foreach (SqlServerInstanceDto ssi in sqlServerInstances)
            {
                lstSqlServer.Items.Add(ssi.Name);
                //trvSqlDatabase.Items.Add(ssi.Name);

            }
        }

        public IEnumerable<SqlServerInstanceDto> GetSqlServerInstances(FacadeService facade, bool localOnly)
        {
            
            try
            {
                sqlServerInstances = facade.GetSqlServerInstances(localOnly);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed");
            }

            return sqlServerInstances;
        }

        private void chkLocalOnly_Checked(object sender, RoutedEventArgs e)
        {
            //CheckBox chk = sender as CheckBox;
            //LoadSqlServers(Convert.ToBoolean(chk.IsChecked));
        }

        private void chkLocalOnly_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            LoadSqlServers(Convert.ToBoolean(chk.IsChecked));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string[] posRun = txtPosrun.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                File.WriteAllLines(PosrunPath, posRun);
                LoadPosrun();
                AddLog("Updating posrun.ini.");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void lstSqlServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddLog(String.Format("Selecting {0}.", lstSqlServer.SelectedItem.ToString()));

            // Update the tree
            UpdateTree(lstSqlServer.SelectedItem.ToString());

            if (tabControl1.SelectedIndex == 1)
            {

                AddLog(String.Format("Getting S_this for {0}.", lstSqlServer.SelectedItem.ToString()));
                facadeService.GetTableData(lstSqlServer.SelectedItem.ToString(), "S_This");
            }
            else
            {
                // Console.WriteLine(String.Format("{0}: Selecting {1}.", DateTime.Now.ToLongTimeString(), lstSqlServer.SelectedItem.ToString()));
                // AddLog(String.Format("Selecting {0}.", lstSqlServer.SelectedItem.ToString()));
            }
        }

        private void AddLog(string message)
        {
            lstLog.Items.Insert(0, String.Format("{0}: {1}.", DateTime.Now.ToLongTimeString(), message));
        }

        private void trvSqlDatabase_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if (e.GetType() == ViewModel.DatabaseViewModel)
            //MessageBox.Show(((TreeViewItem)e.NewValue).Header.ToString());
            if (e.NewValue.GetType().Name == "SqlServerViewModel")
            {
                // Get the database
            } 
            else if (e.NewValue.GetType().Name == "DatabaseViewModel")
            {
                // Get the tables
                DatabaseViewModel dvm = (DatabaseViewModel)e.NewValue;

                // Todo: Try/catch here
                // Get the indexes
                int serverIndex = ssvm.FirstGeneration.IndexOf(dvm.Parent);
                int dbIndex = ssvm.FirstGeneration[serverIndex].Children.IndexOf(dvm);

                ssvm.FirstGeneration[serverIndex].Children[dbIndex].IsExpanded = true;

                tvm = new ObservableCollection<TableViewModel>((from child in facadeService.GetTableList(dvm.Parent.Name, dvm.Name, true) 
                    select new TableViewModel(child, dvm)).ToList<TableViewModel>());
            
                // Todo: Get the correct indexes here.
                if (ssvm.FirstGeneration[serverIndex].Children[dbIndex].Children.Count == 0)
                {
                    AddLog(String.Format("Getting table list for {0}.{1}.", dvm.Parent.Name, dvm.Name));

                    foreach (TableViewModel tm in tvm)
                    {
                        ssvm.FirstGeneration[serverIndex].Children[dbIndex].Children.Add(tm);
                    }

                    AddContextMenu(dvm.Parent.Name, dvm.Name);
                }
            }
            else if (e.NewValue.GetType().Name == "TableViewModel")
            {
                // Get the table data
                GetTableData((TableViewModel)e.NewValue, String.Empty, false);
            }
            CanLoadData = true;
        }

        /// <summary>
        /// Gets the table data.
        /// </summary>
        /// <param name="tableViewModel">The table view model.</param>
        /// <param name="filter">The filter.</param>
        private void GetTableData(TableViewModel tableViewModel, string filter, bool lookForJoin)
        {
            if (CanLoadData)
            {
                TableData td = new TableData(facadeService, tableViewModel.Parent.Parent.Name, tableViewModel.Parent.Name, tableViewModel.Name, filter, lookForJoin);
                AddLog(String.Format("Getting table data for {0}.{1}.{2}", tableViewModel.Parent.Parent.Name, tableViewModel.Parent.Name, tableViewModel.Name));
                td.Show();
            }
        }

        /// <summary>
        /// Updates the tree.
        /// </summary>
        /// <param name="sqlServer">The SQL server.</param>
        private void UpdateTree(string sqlServer)
        {
            try
            {
                SqlServerInstanceDto ssid = sqlServerInstances.SingleOrDefault(s => s.Name == sqlServer);

                if (ssid != null)
                {
                    IEnumerable<DatabaseDto> d = facadeService.GetDatabaseList(ssid.Name);
                    ssid.DataBase = d.ToList();

                    //ViewModel.SqlServerViewModel ssvm = new ViewModel.SqlServerViewModel(ssid);
                    ssvm = new SqlServerTreeViewModel(ssid);

                    trvSqlDatabase.Items.Clear();
                    trvSqlDatabase.DataContext = ssvm;
                }
                else
                {
                    // Add the new SQL Server
                    SqlServerInstanceDto newSsid = new SqlServerInstanceDto();
                    newSsid.Name = sqlServer;
                    
                    // Save the server to file
                    facadeService.SetSqlServerInstance(newSsid);

                    sqlServerInstances = sqlServerInstances.Concat(new[] { newSsid });

                    UpdateTree(sqlServer);
                }
            }
            catch (ApplicationException aex)
            {
                AddLog(String.Format("{2}\n{1}-{0}.", lstSqlServer.SelectedItem.ToString(), aex.Message, aex.InnerException));
            }
            catch (Exception ex)
            {
                AddLog(String.Format("Failed to connect to server: {0} because {1}.", lstSqlServer.SelectedItem.ToString(), ex.Message));
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtServer.Text))
            {
                AddLog(String.Format("Selecting {0}.", txtServer.Text));

                 //Save server

                UpdateTree(txtServer.Text);



                
            }
        }

        private void AddTableToList_Click(object sender, RoutedEventArgs e)
        {
           
            //TreeViewItem selectedItem = trvSqlDatabase.SelectedItem as TreeViewItem;
            //AddContextMenu(null, null);
        }

        private void AddContextMenu(string serverInstance, string dbName)
        {
            if (TableListMenu == null)
            {
                trvSqlDatabase.ContextMenu.Items.Clear();

                IEnumerable<TableDto> tableDto = facadeService.GetTableList(serverInstance, dbName, false);
                TableListMenu = new ContextMenu();

                foreach (TableDto t in tableDto)
                {
                    MenuItem mu = new MenuItem();
                    mu.Header = t.TableName;
                    mu.Click += new RoutedEventHandler(mu_Click);

                    TableListMenu.Items.Add(mu);

                    trvSqlDatabase.ContextMenu = TableListMenu;
                }
            }
        }

        private void mu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mu = sender as MenuItem;
            
            if (tvm != null)
            {
                TableDto tabDto = new TableDto();
                
                tabDto.TableName = mu.Header.ToString();
                
                TableViewModel tab = new TableViewModel(tabDto, ssvm.FirstGeneration[0].Children[0]);
 
                // Todo: Get the correct indexes here.
                ssvm.FirstGeneration[0].Children[0].Children.Add(tab);

                // Remove the item rom the menu
                TableListMenu.Items.Remove(mu);
            }
        }

        private void trvSqlDatabase_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;

            if (item.Header.GetType().Name == "DatabaseViewModel")
            {
                // Add the table list
                if (TableListMenu != null)
                {
                    trvSqlDatabase.ContextMenu = TableListMenu;
                }
            }
            else if (item.Header.GetType().Name == "TableViewModel")
            {
                // Remove the old list 
                trvSqlDatabase.ContextMenu.Items.Remove(TableListMenu);

                ContextMenu cm = new ContextMenu();

                MenuItem mu11 = new MenuItem();
                mu11.Header = "Get bottom 100 joined";
                mu11.Click += new RoutedEventHandler(mu11_Click);
                cm.Items.Add(mu11);

                MenuItem mu1 = new MenuItem();
                mu1.Header = "Get bottom 100";
                mu1.Click += new RoutedEventHandler(mu1_Click);
                cm.Items.Add(mu1);

                MenuItem mu2 = new MenuItem();
                mu2.Header = "Get top 100";
                mu2.Click += new RoutedEventHandler(mu2_Click);
                cm.Items.Add(mu2);

                trvSqlDatabase.ContextMenu = cm;

                CanLoadData = false;
                item.IsSelected = true;
            }

            //XmlElement selectedElement = (XmlElement)item.Header;
        }

        void mu11_Click(object sender, RoutedEventArgs e)
        {
            GetTableData((TableViewModel)trvSqlDatabase.SelectedItem, "desc", true);
        }

        void mu2_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void mu1_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
    }
}
