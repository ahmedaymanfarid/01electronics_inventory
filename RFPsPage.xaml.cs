using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _01electronics_library;
using _01electronics_windows_library;
using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;


namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for RFPsPage.xaml
    /// </summary>
    public partial class RFPsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        private List<PROCUREMENT_STRUCTS.RFP_MAX_STRUCT> rfps;
        private List<BASIC_STRUCTS.STATUS_STRUCT> rfpStatuses;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> rfpRequestors;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> tmpRequestors;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> tmpRequestorsTeamsList;
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> assignees;
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT> siteEngineers;
        private List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT> workOrders;
        private List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT> maintContracts;
        private List<PROJECT_MACROS.PROJECT_STRUCT> companyProjects;
        private List<BASIC_STRUCTS.STATUS_STRUCT> rfpItemsStatus;
        private List<BASIC_STRUCTS.STATUS_STRUCT> rfp_status;
        private List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT> workFormList;
        private List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT> filteredWorkFormList;
        private List<int> workFormId;
        private BASIC_STRUCTS.FOLDER_STRUCT rfpsFolder;
        private List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpMappedItems;

        private Expander currentExpander;
        private Expander previousExpander;

        private int selectedYear;

        private FTPServer fTPServer;

        private StackPanel currentRFPCategoryStackPanel;
        private StackPanel previousRFPCategoryStackPanel;

        private StackPanel currentRFPFolderStackPanel;
        private StackPanel previousRFPFolderStackPanel;

        private StackPanel currentFileStackPanel;
        private StackPanel previousFileStackPanel;

        private StackPanel currentQoutPOFolderStackPanel;
        private StackPanel previousQoutPOFolderStackPanel;

        private StackPanel currentQoutPOFileStackPanel;
        private StackPanel previousQoutPOFileStackPanel;

        private List<StackPanel> teamsPageStackPanel;
        private List<StackPanel> rfpFoldersPageStackPanel;
        private List<StackPanel> quotationPOFoldersPageStackPanel;
        private int viewAddCondition;


        private String errorMessage;

        //private List<BASIC_STRUCTS.FOLDER_STRUCT> rfpFolders;

        private const int TEAMS_PAGE = 1;
        private const int RFP_FOLFDERS_PAGE = 2;
        private const int FILES_PAGE = 3;

        private int previousPage;

        private BASIC_STRUCTS.FOLDER_STRUCT selectedCategoryFolder;
        private BASIC_STRUCTS.FOLDER_STRUCT selectedRFPFolder;

        //private String selectedRFPIdFolder;
        private String selectedQuotationPOFolder;
        bool canEdit;
        bool canDelete;

        private BackgroundWorker folderWatcherProcess;
        private FileSystemWatcher systemWatcher;
        private WindowsFileSystem windowsFileSystem;

        public RFPsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            fTPServer = new FTPServer();

            InitializeComponent();
            
            windowsFileSystem = new WindowsFileSystem();

            rfps = new List<PROCUREMENT_STRUCTS.RFP_MAX_STRUCT>();
            rfpStatuses = new List<BASIC_STRUCTS.STATUS_STRUCT>();

            rfpRequestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            tmpRequestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            tmpRequestorsTeamsList = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            assignees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            workOrders = new List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT>();
            maintContracts = new List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT>();
            companyProjects = new List<PROJECT_MACROS.PROJECT_STRUCT>();
            workFormList = new List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT>();
            filteredWorkFormList = new List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT>();
            workFormId = new List<int>();
            rfpsFolder = new BASIC_STRUCTS.FOLDER_STRUCT();

            rfpItemsStatus = new List<BASIC_STRUCTS.STATUS_STRUCT>();
            rfp_status = new List<BASIC_STRUCTS.STATUS_STRUCT>();
            rfpMappedItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
            siteEngineers = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT>();

            //rfpFolders = new List<BASIC_STRUCTS.FOLDER_STRUCT>();
            teamsPageStackPanel = new List<StackPanel>();
            rfpFoldersPageStackPanel = new List<StackPanel>();
            quotationPOFoldersPageStackPanel = new List<StackPanel>();
            canEdit = false;
            canDelete = false;
            viewAddCondition = COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION;
            GetRFPStatuses();
            GetRFPs();
            GetRFPsFolders();
            GetRFPsRequestors();
            GetSiteEngineer();

            CheckLoggedInUser();

            //InitializeBackgroundWorker();
            InitializeSystemWatcher();

            InitializeYearCombo();
            SetYearComboBox();
            InitializeWorkFormCombo();
            InitializeStatusCombo();
            InitializeAssignedOfficerCombo();
            InitializeItemsStatusCombo();
            
            
            
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();

            //if (rfpRequestors.Find(x1 => x1.employee_id == loggedInUser.GetEmployeeId()).employee_id == 0)
              //  AddRFPBtn.IsEnabled = false;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// BACKGROUND FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void OnWatcherProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }

        protected void BackgroundWatch(object sender, DoWorkEventArgs e)
        {
        }

        protected void OnWatchBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GETTER FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetRFPStatuses()
        {
            if (!commonQueries.GetRFPStatuses(ref rfpStatuses))
                return false;

            return true;
        }
        private bool GetRFPs()
        {
            if (!commonQueries.GetRFPs(ref rfps))
                return false;

            return true;
        }
        private void GetRFPsFolders()
        {
            rfpsFolder = windowsFileSystem.GetDirectoryFilesAndFolders(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\RFPS");
        }
        private bool GetRFPsRequestors()
        {
            if (!commonQueries.GetRFPRequestors(ref tmpRequestorsTeamsList))
                return false;

            return true;
        }
        private bool GetSiteEngineer()
        {
            if (!commonQueries.GetProjectAssignees(ref siteEngineers))
                return false;

            return true;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INTIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeBackgroundWorker()
        {
            folderWatcherProcess = new BackgroundWorker();

            folderWatcherProcess.DoWork += BackgroundWatch;
            folderWatcherProcess.ProgressChanged += OnWatcherProgressChanged;
            folderWatcherProcess.RunWorkerCompleted += OnWatchBackgroundComplete;
            folderWatcherProcess.WorkerReportsProgress = true;
        }
        private void InitializeSystemWatcher()
        {
            systemWatcher = new FileSystemWatcher(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\RFPS");
            //backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += BackgroundStart;
            //backgroundWorker.ProgressChanged += OnBackgroundWorkerProgressChanged;
            //backgroundWorker.RunWorkerCompleted += OnBackgroundWorkerComplete;
            //backgroundWorker.WorkerReportsProgress = true;

            systemWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            systemWatcher.Changed += OnUpdatedRFPsFolder;
            systemWatcher.Error += OnErrorRFPsFolder;

            systemWatcher.Filter = "*.*";
            systemWatcher.IncludeSubdirectories = true;
            systemWatcher.EnableRaisingEvents = true;

        }

        private void InitializeYearCombo()
        {
            for (int year = BASIC_MACROS.CRM_START_YEAR; year <= DateTime.Now.Year; year++)
                yearComboBox.Items.Add(year);
        }

        private void InitializeRequestorCombo()
        {
            requestorComboBox.Items.Clear();
            tmpRequestors.Clear();

            tmpRequestorsTeamsList = tmpRequestorsTeamsList
                              .OrderBy(o => o.employee_name).ToList();

            for (int i = 0; i < tmpRequestorsTeamsList.Count; i++)
            {
                if (i != 0 && tmpRequestorsTeamsList[i].employee_id != tmpRequestorsTeamsList[i - 1].employee_id)
                {
                    requestorComboBox.Items.Add(tmpRequestorsTeamsList[i].employee_name);
                    tmpRequestors.Add(tmpRequestorsTeamsList[i]);
                }
                else if (i == 0)
                {
                    requestorComboBox.Items.Add(tmpRequestorsTeamsList[i].employee_name);
                    tmpRequestors.Add(tmpRequestorsTeamsList[i]);
                }
            }

            tmpRequestorsTeamsList = tmpRequestorsTeamsList
                              .OrderBy(o => o.requestor_team.team_name).ToList();
        }

        private bool InitializeAssignedOfficerCombo()
        {

            if (!commonQueries.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID, ref assignees))
                return false;

            for (int j = 0; j < assignees.Count; j++)
                assignedOfficerComboBox.Items.Add(assignees[j].employee_name);

            return true;
        }

        private void InitializeTeamCombo()
        {
            teamComboBox.Items.Clear();
            rfpRequestors.Clear();

            for (int i = 0; i < tmpRequestorsTeamsList.Count; i++)
            {
                //if (i != 0 && tmpRequestorsTeamsList[i].requestor_team.team_name != tmpRequestorsTeamsList[i - 1].requestor_team.team_name)
                //{
                //teamComboBox.Items.Add(tmpRequestorsTeamsList[i].requestor_team.team_name);
                //rfpRequestors.Add(tmpRequestorsTeamsList[i]);
                //}
                //else if (i == 0)
                //{
                //teamComboBox.Items.Add(tmpRequestorsTeamsList[i].requestor_team.team_name);
                //rfpRequestors.Add(tmpRequestorsTeamsList[i]);
                //}
                if (!teamComboBox.Items.Contains(tmpRequestorsTeamsList[i].requestor_team.team_name))
                {
                    teamComboBox.Items.Add(tmpRequestorsTeamsList[i].requestor_team.team_name);
                    rfpRequestors.Add(tmpRequestorsTeamsList[i]);
                }

            }
        }


        private void InitializeWorkFormCombo()
        {
            if (!commonQueries.GetWorkForm(ref workFormList))
                return;
            workFormComboBox.Items.Add("Work Order");

            COMPANY_WORK_MACROS.WORK_FORM_STRUCT currentWorkForm = new COMPANY_WORK_MACROS.WORK_FORM_STRUCT();
            currentWorkForm.work_form_id = 1;
            currentWorkForm.work_form_name = "Work Order";
            filteredWorkFormList.Add(currentWorkForm);

            workFormComboBox.Items.Add("Maintenace Contract");
            currentWorkForm = new COMPANY_WORK_MACROS.WORK_FORM_STRUCT();
            currentWorkForm.work_form_id = 2;
            currentWorkForm.work_form_name = "Maintenace Contract";
            filteredWorkFormList.Add(currentWorkForm);

            workFormComboBox.Items.Add("Project");
            currentWorkForm = new COMPANY_WORK_MACROS.WORK_FORM_STRUCT();
            currentWorkForm.work_form_id = 3;
            currentWorkForm.work_form_name = "Project";
            filteredWorkFormList.Add(currentWorkForm);

            workFormComboBox.Items.Add("Stock");
            currentWorkForm = new COMPANY_WORK_MACROS.WORK_FORM_STRUCT();
            currentWorkForm.work_form_id = 4;
            currentWorkForm.work_form_name = "Stock";
            filteredWorkFormList.Add(currentWorkForm);
        }

        private void InitializeWorkOrderCombo()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return;

            orderComboBox.Items.Clear();
            orderCheckBox.Content = "Orders";

            for (int i = 0; i < workOrders.Count; i++)
            {
                orderComboBox.Items.Add(workOrders[i].order_id);
            }
        }
        private void InitializeMaintContractCombo()
        {
            if (!commonQueries.GetMaintenanceContracts(ref maintContracts))
                return;

            orderComboBox.Items.Clear();
            orderCheckBox.Content = "Contracts";

            for (int i = 0; i < maintContracts.Count; i++)
            {
                orderComboBox.Items.Add(maintContracts[i].contract_id);
            }
        }
        private void InitializeProjectsCombo()
        {
            if (!commonQueries.GetCompanyProjects(ref companyProjects))
                return;

            orderComboBox.Items.Clear();
            orderCheckBox.Content = "Projects";

            for (int i = 0; i < companyProjects.Count; i++)
            {
                orderComboBox.Items.Add(companyProjects[i].project_name);
            }

        }
        private void InitializeStockComboBox()
        {
            orderComboBox.Items.Clear();
            orderCheckBox.Content = "Stock";


            for (int i = workFormList.Count - 2; i < workFormList.Count; i++)
            {
                orderComboBox.Items.Add(workFormList[i].work_form_name);
                workFormId.Add(workFormList[i].work_form_id);
            }
            orderComboBox.Items.Add(workFormList[0].work_form_name);
            workFormId.Add(workFormList[0].work_form_id);
        }
        private bool InitializeStatusCombo()
        {
            statusComboBox.Items.Clear();

            if (!GetRFPStatuses())
                return false;

            for (int i = 0; i < rfp_status.Count; i++)
                statusComboBox.Items.Add(rfp_status[i].status_name);

            return true;
        }

        private bool InitializeItemsStatusCombo()
        {
            itemStatusComboBox.Items.Clear();

            if (!GetRFPStatuses())
                return false;

            for (int i = 0; i < rfpItemsStatus.Count; i++)
                itemStatusComboBox.Items.Add(rfpItemsStatus[i].status_name);

            itemStatusComboBox.SelectedIndex = -1;

            return true;
        }


        private void InitializeRFPsGrid()
        {
            rfpsGrid.Children.Clear();
            rfpsGrid.RowDefinitions.Clear();
            rfpsGrid.ColumnDefinitions.Clear();

            Label rfpIdHeader = new Label();
            rfpIdHeader.Content = "RFP ID";
            rfpIdHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label rfpRequestorHeader = new Label();
            rfpRequestorHeader.Content = "Requestor";
            rfpRequestorHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label rfpAssigneeHeader = new Label();
            rfpAssigneeHeader.Content = "Assignee";
            rfpAssigneeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderContractProjectHeader = new Label();
            orderContractProjectHeader.Content = "Order/Contract/Project";
            orderContractProjectHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label rfpStatusHeader = new Label();
            rfpStatusHeader.Content = "RFP Status";
            rfpStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label notesHeader = new Label();
            notesHeader.Content = "RFP Notes";
            notesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label rfpItemsHeader = new Label();
            rfpItemsHeader.Content = "Items";
            rfpItemsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
            rfpsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            rfpsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

            Grid.SetRow(rfpIdHeader, 0);
            Grid.SetColumn(rfpIdHeader, 0);
            rfpsGrid.Children.Add(rfpIdHeader);

            Grid.SetRow(rfpRequestorHeader, 0);
            Grid.SetColumn(rfpRequestorHeader, 1);
            rfpsGrid.Children.Add(rfpRequestorHeader);

            Grid.SetRow(rfpAssigneeHeader, 0);
            Grid.SetColumn(rfpAssigneeHeader, 2);
            rfpsGrid.Children.Add(rfpAssigneeHeader);

            Grid.SetRow(orderContractProjectHeader, 0);
            Grid.SetColumn(orderContractProjectHeader, 3);
            rfpsGrid.Children.Add(orderContractProjectHeader);

            Grid.SetRow(rfpStatusHeader, 0);
            Grid.SetColumn(rfpStatusHeader, 4);
            rfpsGrid.Children.Add(rfpStatusHeader);

            Grid.SetRow(notesHeader, 0);
            Grid.SetColumn(notesHeader, 5);
            rfpsGrid.Children.Add(notesHeader);

            Grid.SetRow(rfpItemsHeader, 0);
            Grid.SetColumn(rfpItemsHeader, 6);
            rfpsGrid.Children.Add(rfpItemsHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < rfps.Count; i++)
            {
                DateTime currentRFP = DateTime.Parse(rfps[i].issue_date.ToString());


                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    String tempId = rfps[i].rfp_id;

                    if (tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {

                    }
                    else
                        continue;
                }
                if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1 && currentRFP.Year != selectedYear)
                    continue;

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1 && rfps[i].rfp_status_id != rfpStatuses[statusComboBox.SelectedIndex].status_id)
                    continue;

                if (itemStatusCheckBox.IsChecked == true && itemStatusComboBox.SelectedIndex != -1 && rfps[i].rfps_items.FindIndex(x => x.item_status.status_id == rfpItemsStatus[itemStatusComboBox.SelectedIndex].status_id) == -1)
                    continue;

                if (workFormCheckBox.IsChecked == true && workFormComboBox.SelectedIndex != -1 && rfps[i].work_form != workFormComboBox.SelectedIndex)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 0 && rfps[i].order_serial != workOrders[orderComboBox.SelectedIndex].order_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 1 && rfps[i].order_serial != maintContracts[orderComboBox.SelectedIndex].contract_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 2 && rfps[i].project_serial != companyProjects[orderComboBox.SelectedIndex].project_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 3 && rfps[i].work_form != workFormId[orderComboBox.SelectedIndex])
                    continue;

                if (requestorCheckBox.IsChecked == true && requestorComboBox.SelectedIndex != -1 && rfps[i].requestor_id != tmpRequestors[requestorComboBox.SelectedIndex].employee_id)
                    continue;

                if (teamCheckBox.IsChecked == true && teamComboBox.SelectedIndex != -1 && rfps[i].requestor_team_id != rfpRequestors[teamComboBox.SelectedIndex].requestor_team.team_id)
                    continue;

                if (assignedOfficerCheckBox.IsChecked == true && assignedOfficerComboBox.SelectedIndex != -1 && rfps[i].assigned_officer_id != assignees[assignedOfficerComboBox.SelectedIndex].employee_id)
                    continue;

                rfpsGrid.RowDefinitions.Add(new RowDefinition());

                Label rfpIdLabel = new Label();
                rfpIdLabel.Content = rfps[i].rfp_id;
                rfpIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(rfpIdLabel, currentRowNumber);
                Grid.SetColumn(rfpIdLabel, 0);
                rfpsGrid.Children.Add(rfpIdLabel);

                Label requestorLabel = new Label();
                requestorLabel.Content = rfps[i].requestor_name;
                requestorLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(requestorLabel, currentRowNumber);
                Grid.SetColumn(requestorLabel, 1);
                rfpsGrid.Children.Add(requestorLabel);

                Label assigneeLabel = new Label();
                assigneeLabel.Content = rfps[i].assigned_officer_name;
                assigneeLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(assigneeLabel, currentRowNumber);
                Grid.SetColumn(assigneeLabel, 2);
                rfpsGrid.Children.Add(assigneeLabel);


                Label orderContractProjectLabel = new Label();
                if (rfps[i].work_form == 0)
                    orderContractProjectLabel.Content = "01 Electronics";
                else if (rfps[i].work_form == 1)
                    orderContractProjectLabel.Content = "Order ID: " + rfps[i].order_serial;
                else if (rfps[i].work_form == 2)
                    orderContractProjectLabel.Content = "Contract ID: " + rfps[i].maint_contract_id;
                else if (rfps[i].work_form == 3)
                    orderContractProjectLabel.Content = "Project Name: " + rfps[i].project_name;
                else if (rfps[i].work_form == 4)
                    orderContractProjectLabel.Content = "Stock: " + rfps[i].work_form_name;
                else if (rfps[i].work_form == 5)
                    orderContractProjectLabel.Content = "Stock: " + rfps[i].work_form_name;

                orderContractProjectLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(orderContractProjectLabel, currentRowNumber);
                Grid.SetColumn(orderContractProjectLabel, 3);
                rfpsGrid.Children.Add(orderContractProjectLabel);

                Label statusLabel = new Label();
                statusLabel.Content = rfps[i].rfp_status;
                statusLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(statusLabel, currentRowNumber);
                Grid.SetColumn(statusLabel, 4);
                rfpsGrid.Children.Add(statusLabel);

                TextBlock notesLabel = new TextBlock();
                notesLabel.Text = rfps[i].notes;
                notesLabel.Style = (Style)FindResource("tableSubItemTextblock");
                notesLabel.Width = 200;



                Grid.SetRow(notesLabel, currentRowNumber);
                Grid.SetColumn(notesLabel, 5);
                rfpsGrid.Children.Add(notesLabel);

                Grid itemsGrid = new Grid();
                itemsGrid.ShowGridLines = true;

                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });

                itemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                Label itemDescriptionHeader = new Label();
                itemDescriptionHeader.Content = "Item Description";
                itemDescriptionHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(itemDescriptionHeader, 0);
                Grid.SetColumn(itemDescriptionHeader, 0);
                itemsGrid.Children.Add(itemDescriptionHeader);

                Label itemVendorsHeader = new Label();
                itemVendorsHeader.Content = "Item Vendors";
                itemVendorsHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(itemVendorsHeader, 0);
                Grid.SetColumn(itemVendorsHeader, 1);
                itemsGrid.Children.Add(itemVendorsHeader);

                Label itemQuantityHeader = new Label();
                itemQuantityHeader.Content = "Item Quantity";
                itemQuantityHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(itemQuantityHeader, 0);
                Grid.SetColumn(itemQuantityHeader, 2);
                itemsGrid.Children.Add(itemQuantityHeader);

                Label unitHeader = new Label();
                unitHeader.Content = "Item Unit";
                unitHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(unitHeader, 0);
                Grid.SetColumn(unitHeader, 3);
                itemsGrid.Children.Add(unitHeader);

                Label itemStatusHeader = new Label();
                itemStatusHeader.Content = "Item Status";
                itemStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(itemStatusHeader, 0);
                Grid.SetColumn(itemStatusHeader, 4);
                itemsGrid.Children.Add(itemStatusHeader);

                Label itemNotesHeader = new Label();
                itemNotesHeader.Content = "Item Notes";
                itemNotesHeader.Style = (Style)FindResource("tableSubHeaderItem");

                Grid.SetRow(itemNotesHeader, 0);
                Grid.SetColumn(itemNotesHeader, 5);
                itemsGrid.Children.Add(itemNotesHeader);

                int currentItemRow = 1;

                for (int j = 0; j < rfps[i].rfps_items.Count; j++)
                {
                    itemsGrid.RowDefinitions.Add(new RowDefinition());

                    TextBlock itemDescription = new TextBlock();
                    itemDescription.Text = rfps[i].rfps_items[j].item_description;
                    itemDescription.Style = (Style)FindResource("tableSubItemTextblock");


                    Grid.SetRow(itemDescription, currentItemRow);
                    Grid.SetColumn(itemDescription, 0);
                    itemsGrid.Children.Add(itemDescription);

                    Grid vendorsGrid = new Grid();
                    vendorsGrid.ShowGridLines = true;

                    for (int k = 0; k < rfps[i].rfps_items[j].item_vendors.Count; k++)
                    {
                        vendorsGrid.RowDefinitions.Add(new RowDefinition());

                        TextBlock vendor = new TextBlock();
                        vendor.Text = rfps[i].rfps_items[j].item_vendors[k].vendor_name;
                        vendor.Width = 150;
                        vendor.Style = (Style)FindResource("tableSubItemTextblock");

                        Grid.SetRow(vendor, k);
                        vendorsGrid.Children.Add(vendor);
                    }

                    Grid.SetRow(vendorsGrid, currentItemRow);
                    Grid.SetColumn(vendorsGrid, 1);
                    itemsGrid.Children.Add(vendorsGrid);

                    Label quantity = new Label();
                    quantity.Content = rfps[i].rfps_items[j].item_quantity;
                    quantity.Style = (Style)FindResource("tableSubItemLabel");

                    Grid.SetRow(quantity, currentItemRow);
                    Grid.SetColumn(quantity, 2);
                    itemsGrid.Children.Add(quantity);

                    Label unit = new Label();
                    unit.Content = rfps[i].rfps_items[j].measure_unit;
                    unit.Style = (Style)FindResource("tableSubItemLabel");

                    Grid.SetRow(unit, currentItemRow);
                    Grid.SetColumn(unit, 3);
                    itemsGrid.Children.Add(unit);

                    Label status = new Label();
                    status.Content = rfps[i].rfps_items[j].item_status.status_name;
                    status.Style = (Style)FindResource("tableSubItemLabel");

                    Grid.SetRow(status, currentItemRow);
                    Grid.SetColumn(status, 4);
                    itemsGrid.Children.Add(status);

                    TextBlock notes = new TextBlock();
                    notes.Text = rfps[i].rfps_items[j].item_notes;
                    notes.Style = (Style)FindResource("tableSubItemTextblock");
                    notes.Width = 200;

                    Grid.SetRow(notes, currentItemRow);
                    Grid.SetColumn(notes, 5);
                    itemsGrid.Children.Add(notes);

                    currentItemRow++;
                }

                Grid.SetRow(itemsGrid, currentRowNumber);
                Grid.SetColumn(itemsGrid, 6);
                rfpsGrid.Children.Add(itemsGrid);

                currentRowNumber++;

            }
        }

        private void InitializeRFPsFolderView()
        {
            rfpsFoldersStackPanel.Children.Clear();

            for (int i = 0; i < rfpsFolder.sub_folders.Count; i++)
            {
                if (teamCheckBox.IsChecked == true && teamComboBox.SelectedItem != rfpsFolder.sub_folders[i].folder_name)
                    continue;

                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                stackPanel.Orientation = Orientation.Horizontal;
                //stackPanel.Tag = tmpRequestorsTeamsList[i].team_id;
                stackPanel.MouseLeftButtonDown += OnClickRFPCategoryFolder;
                stackPanel.Margin = new Thickness(12, 0, 0, 0);

                Image folderImage = new Image();
                folderImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                folderImage.Height = 50;
                folderImage.Width = 50;

                Label teamName = new Label();
                teamName.Style = (Style)FindResource("stackPanelItemHeader");
                teamName.Content = rfpsFolder.sub_folders[i].folder_name;

                stackPanel.Children.Add(folderImage);
                stackPanel.Children.Add(teamName);

                rfpsFoldersStackPanel.Children.Add(stackPanel);
            }


        }

        private void InitializeRFPsStackPanel()
        {
            rfpsStackPanel.Children.Clear();


            for (int i = 0; i < rfps.Count; i++)
            {

                DateTime currentRFP = DateTime.Parse(rfps[i].issue_date.ToString());


                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    String tempId = rfps[i].rfp_id;

                    if (tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {

                    }
                    else
                        continue;
                }

                if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1 && currentRFP.Year != selectedYear)
                    continue;

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1 && rfps[i].rfp_status_id != rfpStatuses[statusComboBox.SelectedIndex].status_id)
                    continue;

                if (itemStatusCheckBox.IsChecked == true && itemStatusComboBox.SelectedIndex != -1 && rfps[i].rfps_items.FindIndex(x => x.item_status.status_id == rfpItemsStatus[itemStatusComboBox.SelectedIndex].status_id) == -1)
                    continue;

                if (workFormCheckBox.IsChecked == true && workFormComboBox.SelectedIndex != -1 && rfps[i].work_form != filteredWorkFormList[workFormComboBox.SelectedIndex].work_form_id && rfps[i].work_form > 0 && rfps[i].work_form < 4)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 0 && rfps[i].order_serial != workOrders[orderComboBox.SelectedIndex].order_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 1 && rfps[i].maintenance_contract_serial != maintContracts[orderComboBox.SelectedIndex].contract_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 2 && rfps[i].project_serial != companyProjects[orderComboBox.SelectedIndex].project_serial)
                    continue;

                if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 3 && rfps[i].work_form != workFormId[orderComboBox.SelectedIndex])
                    continue;

                if (requestorCheckBox.IsChecked == true && requestorComboBox.SelectedIndex != -1 && rfps[i].requestor_id != tmpRequestors[requestorComboBox.SelectedIndex].employee_id)
                    continue;

                if (teamCheckBox.IsChecked == true && teamComboBox.SelectedIndex != -1 && rfps[i].requestor_team_id != rfpRequestors[teamComboBox.SelectedIndex].requestor_team.team_id)
                    continue;

                if (assignedOfficerCheckBox.IsChecked == true && assignedOfficerComboBox.SelectedIndex != -1 && rfps[i].assigned_officer_id != assignees[assignedOfficerComboBox.SelectedIndex].employee_id)
                    continue;

                Grid mainGrid = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

                StackPanel mainStackPanel = new StackPanel();
                mainStackPanel.Orientation = Orientation.Vertical;

                Label rfpIdLabel = new Label();
                rfpIdLabel.Content = rfps[i].rfp_id;
                rfpIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label rfpRequestorLabel = new Label();
                rfpRequestorLabel.Content = "Requested By: " + rfps[i].requestor_name;
                rfpRequestorLabel.Style = (Style)FindResource("stackPanelItemBody");


                Label rfpAssignedOfficerLabel = new Label();
                rfpAssignedOfficerLabel.Content = "Assigned Officer: " + rfps[i].assigned_officer_name;
                rfpAssignedOfficerLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label rfpWorkFormLabel = new Label();
                rfpWorkFormLabel.Style = (Style)FindResource("stackPanelItemBody");

                if (rfps[i].work_form == 1)
                {
                    rfpWorkFormLabel.Content = "Order ID: " + rfps[i].order_serial;
                }
                else if (rfps[i].work_form == 2)
                {
                    rfpWorkFormLabel.Content = "Contract ID: " + rfps[i].maint_contract_id;
                }
                else if (rfps[i].work_form == 3)
                {
                    rfpWorkFormLabel.Content = "Project Name: " + rfps[i].project_name;
                }
                else if (rfps[i].work_form == 4 || rfps[i].work_form == 5 || rfps[i].work_form == 0)
                    rfpWorkFormLabel.Content = "Other: " + rfps[i].work_form_name;
                //else if ()
                //    rfpWorkFormLabel.Content = "Stock: " + rfps[i].work_form_name;
                //else if (rfps[i].work_form == 0)
                //{
                //    rfpWorkFormLabel.Content = "01 Electronics";
                //}

                mainStackPanel.Children.Add(rfpIdLabel);
                mainStackPanel.Children.Add(rfpRequestorLabel);
                mainStackPanel.Children.Add(rfpAssignedOfficerLabel);
                mainStackPanel.Children.Add(rfpWorkFormLabel);

                //ScrollViewer itemsScrollViewer = new ScrollViewer();
                //itemsScrollViewer.Height = 80;
                //itemsScrollViewer.Margin = new Thickness(12);
                Grid itemsGrid = new Grid();
                itemsGrid.Margin = new Thickness(12);
                int counter = 0;


                for (int j = 0; j < rfps[i].rfps_items.Count; j++)
                {
                    if (itemStatusCheckBox.IsChecked == true && rfps[i].rfps_items[j].item_status.status_id != rfpItemsStatus[itemStatusComboBox.SelectedIndex].status_id)
                        continue;

                    if (counter == 0)
                    {
                        // itemsScrollViewer.Content = itemsGrid;
                        itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(85) });
                        itemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });

                        mainStackPanel.Children.Add(itemsGrid);

                        counter++;
                    }

                    //Label itemNumberLabel = new Label();
                    //itemNumberLabel.Content = j + 1 + "- ";
                    //itemNumberLabel.Style = (Style)FindResource("stackPanelItemBody");

                    TextBlock itemDescriptionLabel = new TextBlock();
                    itemDescriptionLabel.Text = rfps[i].rfps_items[j].item_description;
                    itemDescriptionLabel.Style = (Style)FindResource("stackPanelItemBodyTextBlock");
                    itemDescriptionLabel.Width = 200;



                    Label rfpItemStatusLabel = new Label();
                    rfpItemStatusLabel.Style = (Style)FindResource("BorderIconItemTextLabel");
                    rfpItemStatusLabel.Content = rfps[i].rfps_items[j].item_status.status_name;
                    rfpItemStatusLabel.FontSize = 13;

                    Border itemBorderIcon = new Border();
                    itemBorderIcon.Style = (Style)FindResource("BorderItemIcon");
                    itemBorderIcon.Width = 70;
                    itemBorderIcon.Height = 25;
                    itemBorderIcon.CornerRadius = new CornerRadius(15);
                    itemBorderIcon.BorderThickness = new Thickness(1);

                    if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_PENDING)
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD000"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD000"));
                    }
                    else if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_QUOTED)
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E03C00"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E03C00"));
                    }
                    else if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_PO)
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8C08"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8C08"));
                    }
                    else if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));

                    }
                    else if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_SITE)
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                    }
                    else
                    {
                        itemBorderIcon.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                        itemBorderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                    }

                    itemBorderIcon.Child = rfpItemStatusLabel;

                    itemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                    itemsGrid.Children.Add(itemDescriptionLabel);
                    Grid.SetRow(itemDescriptionLabel, itemsGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(itemDescriptionLabel, 0);

                    itemsGrid.Children.Add(itemBorderIcon);
                    Grid.SetRow(itemBorderIcon, itemsGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(itemBorderIcon, 1);



                }
                for (int j = 0; j < rfps[i].rfps_items.Count; j++)
                {
                    if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_PENDING)
                    {
                        if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_PENDING)
                        {
                            canEdit = true;
                            canDelete = true;

                        }
                        else
                        {
                            canEdit = false;
                            canDelete = false;
                            break;
                        }
                    }
                    else
                    {
                        canEdit = false;
                        canDelete = false;
                        break;
                    }
                }
                for (int j = 0; j < rfps[i].rfps_items.Count; j++)
                {
                    if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED || rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_PENDING)
                    {
                        if (rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_PENDING || rfps[i].rfps_items[j].item_status.status_id == COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED)
                        {

                            canDelete = true;

                        }
                        else
                        {

                            canDelete = false;
                            break;
                        }
                    }
                    else
                    {


                        canDelete = false;
                        break;
                    }
                }
                Label rfpStatusLabel = new Label();
                rfpStatusLabel.Content = rfps[i].rfp_status;
                rfpStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");
                borderIcon.VerticalAlignment = VerticalAlignment.Top;
                borderIcon.Margin = new Thickness(0, 12, 0, 0);
                borderIcon.Width = 100.00;

                if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_PENDING)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD000"));
                }
                else if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_QUOTED)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E03C00"));
                }
                else if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_PO)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8C08"));
                }
                else if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF620D"));
                }
                else if (rfps[i].rfp_status_id == COMPANY_WORK_MACROS.RFP_AT_SITE)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfpStatusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                expander.Margin = new Thickness(12);

                StackPanel expanderStackPanel = new StackPanel();
                expanderStackPanel.Orientation = Orientation.Vertical;

                BrushConverter brushConverter = new BrushConverter();

                Button viewButton = new Button();
                viewButton.Click += OnBtnClickView;
                viewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                viewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                viewButton.Content = "View";

                //Button reviseButton = new Button();
                //reviseButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //reviseButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //reviseButton.Click += OnBtnClickRevise;
                //reviseButton.Content = "Revise";



                Button editButton = new Button();
                editButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                editButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                editButton.Click += OnBtnClickEdit;
                editButton.Content = "Map RFP";

                //Button cancelButton = new Button();
                //cancelButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //cancelButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //cancelButton.Click += OnBtnClickCancel;
                //cancelButton.Content = "Cancel";

                //Button viewQuotaition = new Button();
                //viewQuotaition.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //viewQuotaition.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //viewQuotaition.Click += OnBtnClickviewQuotaition;
                //viewQuotaition.Content = "View Quotaition";

                //Button viewPO = new Button();
                //viewPO.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //viewPO.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //viewPO.Click += OnBtnClickviewPO;
                //viewPO.Content = "View PO";

                //Button changeAssigne = new Button();
                //changeAssigne.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //changeAssigne.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //changeAssigne.Click += OnBtnClickChangeAssignee;
                //changeAssigne.Content = "Change Assignee";


                //Button ApproveButton = new Button();
                //ApproveButton.Background = (Brush)Brushes.Green;
                //ApproveButton.Foreground = (Brush)brushConverter.ConvertFrom("#FFF");
                //ApproveButton.Click += OnBtnClickApprove;
                //ApproveButton.Content = "Approve";

                //Button deleteRFP = new Button();
                //deleteRFP.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                //deleteRFP.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                //deleteRFP.Click += OnButtonClickDeleteRFP;
                //deleteRFP.Content = "Delete RFP";

                //if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID && rfps[i].rfps_items.FindIndex(x1 => x1.item_availablilty_status.status_id == COMPANY_WORK_MACROS.RFP_PENDING_STOCK_RECEIVAL) != -1)
                //   expanderStackPanel.Children.Add(ApproveButton);
                //if (siteEngineers.FindIndex(x => x == loggedInUser.GetEmployeeId()) != -1 && rfps[i].rfps_items.FindIndex(x1 => x1.item_availablilty_status.status_id == COMPANY_WORK_MACROS.RFP_PENDING_SITE_RECIEVAL) != -1)
                //    expanderStackPanel.Children.Add(ApproveButton);

                expanderStackPanel.Children.Add(viewButton);

                //expanderStackPanel.Children.Add(viewQuotaition);
                //expanderStackPanel.Children.Add(viewPO);

                //if (rfpRequestors.FindIndex(x1 => x1.employee_id == loggedInUser.GetEmployeeId()) != -1)
                    //expanderStackPanel.Children.Add(reviseButton);

                //if ((loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION) || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
                //{
                    //expanderStackPanel.Children.Add(changeAssigne);
                //}


                // if (loggedInUser.GetEmployeeId() != rfps[i].requestor_id && rfps[i].rfps_items.FindIndex(x1 => x1.status_name.status_id != 1) == -1 && rfps[i].rfps_items.FindIndex(x1 => x1.item_availablilty_status.status_id != 1) == -1)
                if (canEdit)
                {
                    expanderStackPanel.Children.Add(editButton);
                }
                //if (canDelete)
                //{
                //    expanderStackPanel.Children.Add(deleteRFP);
                //}

                //if (!loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()))
                //    expanderStackPanel.Children.Add(cancelButton);
                expander.Content = expanderStackPanel;

                mainGrid.Children.Add(mainStackPanel);
                Grid.SetColumn(mainStackPanel, 0);

                mainGrid.Children.Add(borderIcon);
                Grid.SetColumn(borderIcon, 1);

                mainGrid.Children.Add(expander);
                Grid.SetColumn(expander, 2);

                rfpsStackPanel.Children.Add(mainGrid);
            }

        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SYSTEM WATCHER HANDLER FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async void OnUpdatedRFPsFolder(object sender, FileSystemEventArgs e)
        {
            GetRFPsFolders();
            
            Dispatcher.Invoke(new Action(() => InitializeRFPsFolderView()));
        }

        private void OnErrorRFPsFolder(object sender, ErrorEventArgs e)
        {
            //PrintException(e.GetException());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GUI CLICK HANDLER FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickRFPCategoryFolder(object sender, MouseButtonEventArgs e)
        {
            previousRFPCategoryStackPanel = currentRFPCategoryStackPanel;
            currentRFPCategoryStackPanel = (StackPanel)sender;

            BrushConverter brushConverter = new BrushConverter();

            if (previousRFPCategoryStackPanel != null && previousRFPCategoryStackPanel != currentRFPCategoryStackPanel)
            {
                previousRFPCategoryStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)previousRFPCategoryStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)previousRFPCategoryStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                previousRFPCategoryStackPanel.Children.Clear();
                previousRFPCategoryStackPanel.Children.Add(currentImage);
                previousRFPCategoryStackPanel.Children.Add(currentLabel);
            }

            if (currentRFPCategoryStackPanel != null)
            {
                currentRFPCategoryStackPanel.Background = (Brush)brushConverter.ConvertFrom("#105A97");

                Label currentLabel = (Label)currentRFPCategoryStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                //selectedCategoryFolder = currentLabel.Content.ToString();
                selectedCategoryFolder = rfpsFolder.sub_folders.Find(category_item => category_item.folder_name == ((Label)currentRFPCategoryStackPanel.Children[1]).Content.ToString());

                Image currentImage = (Image)currentRFPCategoryStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\selected_folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentRFPCategoryStackPanel.Children.Clear();
                currentRFPCategoryStackPanel.Children.Add(currentImage);
                currentRFPCategoryStackPanel.Children.Add(currentLabel);
            }

            if (currentRFPCategoryStackPanel == previousRFPCategoryStackPanel && currentRFPCategoryStackPanel != null)
            {
                currentRFPCategoryStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)currentRFPCategoryStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)currentRFPCategoryStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentRFPCategoryStackPanel.Children.Clear();
                currentRFPCategoryStackPanel.Children.Add(currentImage);
                currentRFPCategoryStackPanel.Children.Add(currentLabel);

                teamsPageStackPanel.Clear();

                for (int i = 0; i < rfpsFoldersStackPanel.Children.Count; i++)
                {
                    teamsPageStackPanel.Add((StackPanel)rfpsFoldersStackPanel.Children[i]);
                }

                folderViewScrollViewer.Content = null;
                rfpsFoldersStackPanel.Children.Clear();

                Image backImage = new Image();
                backImage = new Image { Source = new BitmapImage(new Uri(@"Icons\back_icon.png", UriKind.Relative)) };
                backImage.Height = 25;
                backImage.Width = 25;
                backImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                backImage.Margin = new Thickness(10);
                backImage.MouseLeftButtonDown += OnClickBackTeams;

                StackPanel imageStackPanel = new StackPanel();

                imageStackPanel.Children.Add(backImage);

                rfpsFoldersStackPanel.Children.Add(imageStackPanel);

                //BASIC_STRUCTS.FOLDER_STRUCT currentSelectedRFPCategory = ;

                for (int i = 0; i < selectedCategoryFolder.sub_folders.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.Tag = i;
                    stackPanel.MouseLeftButtonDown += OnClickRFPFolder;
                    stackPanel.Margin = new Thickness(12, 0, 0, 0);

                    Image fileImage = new Image();
                    fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                    fileImage.Height = 50;
                    fileImage.Width = 50;

                    Label folderName = new Label();
                    folderName.Style = (Style)FindResource("stackPanelItemHeader");
                    folderName.Content = selectedCategoryFolder.sub_folders[i].folder_name;

                    stackPanel.Children.Add(fileImage);
                    stackPanel.Children.Add(folderName);

                    rfpsFoldersStackPanel.Children.Add(stackPanel);
                }


                //for (int i = 0; i < rfpsFolder.sub_folders.Count; i++)
                //{
                //    if (rfpFolders[i].selectedRfp.requestor_team_id == int.Parse(currentRFPCategoryStackPanel.Tag.ToString()))
                //    {
                //        DateTime currentRFP = DateTime.Parse(rfpFolders[i].selectedRfp.issue_date.ToString());
                //    
                //    
                //        if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                //        {
                //            String tempId = rfpFolders[i].selectedRfp.rfp_id;
                //    
                //            if (tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                //          {
                //    
                //          }
                //            else
                //                continue;
                //        }
                //
                //    
                //        if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1 && currentRFP.Year != selectedYear)
                //            continue;
                //    
                //        if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.rfp_status_id != rfpStatuses[statusComboBox.SelectedIndex].id)
                //            continue;
                //    
                //        if (itemStatusCheckBox.IsChecked == true && itemStatusComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.rfps_items.FindIndex(x => x.status_name.status_id == rfpItemsStatus[itemStatusComboBox.SelectedIndex].status_id) == -1)
                //            continue;
                //    
                //        if (workFormCheckBox.IsChecked == true && workFormComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.work_form != workFormComboBox.SelectedIndex)
                //            continue;
                //    
                //        if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 1 && rfpFolders[i].selectedRfp.order_serial != workOrders[orderComboBox.SelectedIndex].order_serial)
                //            continue;
                //    
                //        if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 2 && rfpFolders[i].selectedRfp.maintenance_contract_serial != maintContracts[orderComboBox.SelectedIndex].contract_serial)
                //            continue;
                //    
                //        if (orderCheckBox.IsChecked == true && orderComboBox.SelectedIndex != -1 && workFormComboBox.SelectedIndex == 3 && rfpFolders[i].selectedRfp.project_serial != companyProjects[orderComboBox.SelectedIndex].project_serial)
                //            continue;
                //    
                //        if (requestorCheckBox.IsChecked == true && requestorComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.requestor_id != rfpRequestors[requestorComboBox.SelectedIndex].employee_id)
                //            continue;
                //    
                //        if (teamCheckBox.IsChecked == true && teamComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.requestor_team_id != rfpRequestors[teamComboBox.SelectedIndex].team_id)
                //            continue;
                //    
                //        if (assignedOfficerCheckBox.IsChecked == true && assignedOfficerComboBox.SelectedIndex != -1 && rfpFolders[i].selectedRfp.assigned_officer_id != assignees[assignedOfficerComboBox.SelectedIndex].employee_id)
                //            continue;
                //    
                //        StackPanel stackPanel = new StackPanel();
                //        stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //        stackPanel.Orientation = Orientation.Horizontal;
                //        stackPanel.Tag = i;
                //        stackPanel.MouseLeftButtonDown += OnClickRFPFolder;
                //        stackPanel.Margin = new Thickness(12, 0, 0, 0);
                //    
                //        Image fileImage = new Image();
                //        fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                //        fileImage.Height = 50;
                //        fileImage.Width = 50;
                //    
                //        Label fileName = new Label();
                //        fileName.Style = (Style)FindResource("stackPanelItemHeader");
                //        fileName.Content = rfpFolders[i].fileName;
                //    
                //        stackPanel.Children.Add(fileImage);
                //        stackPanel.Children.Add(fileName);
                //    
                //        rfpsFoldersStackPanel.Children.Add(stackPanel);
                //    }
                //}

                folderViewScrollViewer.Content = rfpsFoldersStackPanel;
                folderViewScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        }

        private void OnClickRFPFolder(object sender, MouseButtonEventArgs e)
        {
            previousRFPFolderStackPanel = currentRFPFolderStackPanel;
            currentRFPFolderStackPanel = (StackPanel)sender;

            BrushConverter brushConverter = new BrushConverter();

            if (previousRFPFolderStackPanel != null && previousRFPFolderStackPanel != currentRFPFolderStackPanel)
            {
                previousRFPFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)previousRFPFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)previousRFPFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                previousRFPFolderStackPanel.Children.Clear();
                previousRFPFolderStackPanel.Children.Add(currentImage);
                previousRFPFolderStackPanel.Children.Add(currentLabel);
            }

            if (currentRFPFolderStackPanel != null)
            {
                currentRFPFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#105A97");

                Label currentLabel = (Label)currentRFPFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Image currentImage = (Image)currentRFPFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\selected_folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentRFPFolderStackPanel.Children.Clear();
                currentRFPFolderStackPanel.Children.Add(currentImage);
                currentRFPFolderStackPanel.Children.Add(currentLabel);

                //selectedRFPIdFolder = currentLabel.Content.ToString();
                selectedRFPFolder = selectedCategoryFolder.sub_folders.Find(rfp_folder => rfp_folder.folder_name == ((Label)currentRFPFolderStackPanel.Children[1]).Content.ToString());
            }

            if (currentRFPFolderStackPanel == previousRFPFolderStackPanel)
            {
                currentRFPFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)currentRFPFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)currentRFPFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentRFPFolderStackPanel.Children.Clear();
                currentRFPFolderStackPanel.Children.Add(currentImage);
                currentRFPFolderStackPanel.Children.Add(currentLabel);

                rfpFoldersPageStackPanel.Clear();

                for (int i = 0; i < rfpsFoldersStackPanel.Children.Count; i++)
                {
                    rfpFoldersPageStackPanel.Add((StackPanel)rfpsFoldersStackPanel.Children[i]);
                }

                rfpsFoldersStackPanel.Children.Clear();

                Image backImage = new Image();
                backImage = new Image { Source = new BitmapImage(new Uri(@"Icons\back_icon.png", UriKind.Relative)) };
                backImage.Height = 25;
                backImage.Width = 25;
                backImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                backImage.Margin = new Thickness(10);
                backImage.MouseLeftButtonDown += OnClickBackRFPFiles;

                StackPanel imageStackPanel = new StackPanel();
                imageStackPanel.Children.Add(backImage);

                rfpsFoldersStackPanel.Children.Add(imageStackPanel);

                //String path = BASIC_MACROS.RFP_FILES_PATH + selectedCategoryFolder + "/" + currentLabel.Content.ToString() + "/";
                //
                //if (!fTPServer.ListFilesInFolder(path, ref rfpFiles, ref errorMessage))
                //{
                //    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                for (int i = 0; i < selectedRFPFolder.sub_folders.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.Tag = i;

                    stackPanel.Margin = new Thickness(12, 0, 0, 0);

                    Image fileImage = new Image();

                    fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                    stackPanel.MouseLeftButtonDown += OnClickQuotationPOFolder;

                    fileImage.Height = 50;
                    fileImage.Width = 50;

                    Label fileName = new Label();
                    fileName.Style = (Style)FindResource("stackPanelItemHeader");
                    fileName.Content = selectedRFPFolder.sub_folders[i].folder_name;

                    stackPanel.Children.Add(fileImage);
                    stackPanel.Children.Add(fileName);

                    rfpsFoldersStackPanel.Children.Add(stackPanel);
                }
                for (int i = 0; i < selectedRFPFolder.files.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.Tag = i;

                    stackPanel.Margin = new Thickness(12, 0, 0, 0);

                    Image fileImage = new Image();

                    fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };
                    stackPanel.MouseLeftButtonDown += OnClickRFPFile;

                    fileImage.Height = 50;
                    fileImage.Width = 50;

                    Label fileName = new Label();
                    fileName.Style = (Style)FindResource("stackPanelItemHeader");
                    fileName.Content = selectedRFPFolder.files[i];

                    stackPanel.Children.Add(fileImage);
                    stackPanel.Children.Add(fileName);

                    rfpsFoldersStackPanel.Children.Add(stackPanel);
                }

                if (selectedRFPFolder.sub_folders.Count == 0 && selectedRFPFolder.files.Count == 0)
                {
                    Label fileName = new Label();
                    fileName.Style = (Style)FindResource("stackPanelItemHeader");
                    fileName.Content = "This directory is currenty empty, upload files to view";
                    fileName.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    rfpsFoldersStackPanel.Children.Add(fileName);
                }



            }
        }

        private void OnClickRFPFile(object sender, MouseButtonEventArgs e)
        {
            previousFileStackPanel = currentFileStackPanel;
            currentFileStackPanel = (StackPanel)sender;

            BrushConverter brushConverter = new BrushConverter();

            if (currentQoutPOFolderStackPanel != null)
            {
                currentQoutPOFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)currentQoutPOFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)currentQoutPOFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentQoutPOFolderStackPanel.Children.Clear();
                currentQoutPOFolderStackPanel.Children.Add(currentImage);
                currentQoutPOFolderStackPanel.Children.Add(currentLabel);

                previousQoutPOFolderStackPanel = null;
                currentQoutPOFolderStackPanel = null;
            }

            if (previousFileStackPanel != null && previousFileStackPanel != currentFileStackPanel)
            {
                previousFileStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)previousFileStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)previousFileStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                previousFileStackPanel.Children.Clear();
                previousFileStackPanel.Children.Add(currentImage);
                previousFileStackPanel.Children.Add(currentLabel);
            }

            if (currentFileStackPanel != null)
            {
                currentFileStackPanel.Background = (Brush)brushConverter.ConvertFrom("#105A97");

                Label currentLabel = (Label)currentFileStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Image currentImage = (Image)currentFileStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\selected_pdf_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentFileStackPanel.Children.Clear();
                currentFileStackPanel.Children.Add(currentImage);
                currentFileStackPanel.Children.Add(currentLabel);
            }

            if (currentFileStackPanel == previousFileStackPanel)
            {
                Label currentFileName = (Label)currentFileStackPanel.Children[1];

                string selectedFile = selectedRFPFolder.files.Find(rfp_file => rfp_file == currentFileName.Content.ToString());

                //PROCUREMENT_STRUCTS.RFP_MAX_STRUCT tempRFP = new PROCUREMENT_STRUCTS.RFP_MAX_STRUCT();
                //String serverFilePath = BASIC_MACROS.RFP_FILES_PATH + selectedCategoryFolder + "/" + selectedRFPIdFolder + "/" + currentFileName.Content.ToString();

                //Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics");

                //if (!fTPServer.DownloadFile(serverFilePath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics/" + currentFileName.Content.ToString(), ref errorMessage))
                //{
                //    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                using (System.Diagnostics.Process openPDF = new System.Diagnostics.Process())
                {
                    openPDF.StartInfo.FileName = selectedFile;
                    openPDF.Start();
                }
            }

        }

        private void OnClickQuotationPOFolder(object sender, MouseButtonEventArgs e)
        {
            previousQoutPOFolderStackPanel = currentQoutPOFolderStackPanel;
            currentQoutPOFolderStackPanel = (StackPanel)sender;

            BrushConverter brushConverter = new BrushConverter();

            if (currentFileStackPanel != null)
            {
                currentFileStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)currentFileStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)currentFileStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentFileStackPanel.Children.Clear();
                currentFileStackPanel.Children.Add(currentImage);
                currentFileStackPanel.Children.Add(currentLabel);

                previousFileStackPanel = null;
                currentFileStackPanel = null;
            }

            if (previousQoutPOFolderStackPanel != null && previousQoutPOFolderStackPanel != currentQoutPOFolderStackPanel)
            {
                previousQoutPOFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)previousQoutPOFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)previousQoutPOFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                previousQoutPOFolderStackPanel.Children.Clear();
                previousQoutPOFolderStackPanel.Children.Add(currentImage);
                previousQoutPOFolderStackPanel.Children.Add(currentLabel);
            }

            if (currentQoutPOFolderStackPanel != null)
            {
                currentQoutPOFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#105A97");

                Label currentLabel = (Label)currentQoutPOFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Image currentImage = (Image)currentQoutPOFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\selected_folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentQoutPOFolderStackPanel.Children.Clear();
                currentQoutPOFolderStackPanel.Children.Add(currentImage);
                currentQoutPOFolderStackPanel.Children.Add(currentLabel);


            }

            if (currentQoutPOFolderStackPanel == previousQoutPOFolderStackPanel)
            {
                currentQoutPOFolderStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)currentQoutPOFolderStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)currentQoutPOFolderStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                selectedQuotationPOFolder = currentLabel.Content.ToString();

                currentQoutPOFolderStackPanel.Children.Clear();
                currentQoutPOFolderStackPanel.Children.Add(currentImage);
                currentQoutPOFolderStackPanel.Children.Add(currentLabel);

                quotationPOFoldersPageStackPanel.Clear();

                for (int i = 0; i < rfpsFoldersStackPanel.Children.Count; i++)
                {
                    quotationPOFoldersPageStackPanel.Add((StackPanel)rfpsFoldersStackPanel.Children[i]);
                }

                rfpsFoldersStackPanel.Children.Clear();

                Image backImage = new Image();
                backImage = new Image { Source = new BitmapImage(new Uri(@"Icons\back_icon.png", UriKind.Relative)) };
                backImage.Height = 25;
                backImage.Width = 25;
                backImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                backImage.Margin = new Thickness(10);
                backImage.MouseLeftButtonDown += OnClickBackQuotationPOStackPanel;

                StackPanel imageStackPanel = new StackPanel();
                imageStackPanel.Children.Add(backImage);

                rfpsFoldersStackPanel.Children.Add(imageStackPanel);

                List<String> quotationPOFiles = new List<string>();

                //String path = BASIC_MACROS.RFP_FILES_PATH + selectedCategoryFolder + "/" + selectedRFPIdFolder + "/" + currentLabel.Content.ToString() + "/";
                //
                //if (!fTPServer.ListFilesInFolder(path, ref quotationPOFiles, ref errorMessage))
                //{
                //    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                for (int i = 0; i < quotationPOFiles.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.Tag = i;

                    stackPanel.Margin = new Thickness(12, 0, 0, 0);

                    Image fileImage = new Image();

                    fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };
                    stackPanel.MouseLeftButtonDown += OnClickQuotationPOFile;

                    fileImage.Height = 50;
                    fileImage.Width = 50;

                    Label fileName = new Label();
                    fileName.Style = (Style)FindResource("stackPanelItemHeader");
                    fileName.Content = quotationPOFiles[i];

                    stackPanel.Children.Add(fileImage);
                    stackPanel.Children.Add(fileName);

                    rfpsFoldersStackPanel.Children.Add(stackPanel);
                }

                if (quotationPOFiles.Count == 0)
                {
                    Label fileName = new Label();
                    fileName.Style = (Style)FindResource("stackPanelItemHeader");
                    fileName.Content = "This directory is currenty empty, upload files to view";
                    fileName.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    rfpsFoldersStackPanel.Children.Add(fileName);
                }
            }
        }

        private void OnClickQuotationPOFile(object sender, MouseButtonEventArgs e)
        {
            previousQoutPOFileStackPanel = currentQoutPOFileStackPanel;
            currentQoutPOFileStackPanel = (StackPanel)sender;

            BrushConverter brushConverter = new BrushConverter();

            if (previousQoutPOFileStackPanel != null && previousQoutPOFileStackPanel != currentFileStackPanel)
            {
                previousQoutPOFileStackPanel.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Label currentLabel = (Label)previousQoutPOFileStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");

                Image currentImage = (Image)previousQoutPOFileStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                previousQoutPOFileStackPanel.Children.Clear();
                previousQoutPOFileStackPanel.Children.Add(currentImage);
                previousQoutPOFileStackPanel.Children.Add(currentLabel);
            }

            if (currentQoutPOFileStackPanel != null)
            {
                currentQoutPOFileStackPanel.Background = (Brush)brushConverter.ConvertFrom("#105A97");

                Label currentLabel = (Label)currentQoutPOFileStackPanel.Children[1];
                currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FFFFFF");

                Image currentImage = (Image)currentQoutPOFileStackPanel.Children[0];
                currentImage = new Image { Source = new BitmapImage(new Uri(@"Icons\selected_pdf_icon.png", UriKind.Relative)) };
                currentImage.Height = 50;
                currentImage.Width = 50;

                currentQoutPOFileStackPanel.Children.Clear();
                currentQoutPOFileStackPanel.Children.Add(currentImage);
                currentQoutPOFileStackPanel.Children.Add(currentLabel);
            }

            if (currentQoutPOFileStackPanel == previousQoutPOFileStackPanel)
            {
                Label currentFileName = (Label)currentQoutPOFileStackPanel.Children[1];

                string selectedFile = selectedRFPFolder.files.Find(rfp_file => rfp_file == currentFileName.Content.ToString());

                //PROCUREMENT_STRUCTS.RFP_MAX_STRUCT tempRFP = new PROCUREMENT_STRUCTS.RFP_MAX_STRUCT();
                //String serverFilePath = BASIC_MACROS.RFP_FILES_PATH + selectedCategoryFolder + "/" + selectedRFPIdFolder + "/" + selectedQuotationPOFolder + "/" + currentFileName.Content.ToString();

                //Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics");

                //if (!fTPServer.DownloadFile(serverFilePath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics/" + currentFileName.Content.ToString(), ref errorMessage))
                //{
                //    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                //using (System.Diagnostics.Process openPDF = new System.Diagnostics.Process())
                //{
                //    openPDF.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics/" + currentFileName.Content.ToString();
                //    openPDF.Start();
                //}

                using (System.Diagnostics.Process openPDF = new System.Diagnostics.Process())
                {
                    openPDF.StartInfo.FileName = selectedFile;
                    openPDF.Start();
                }
            }
        }

        private void OnClickBackQuotationPOStackPanel(object sender, MouseButtonEventArgs e)
        {
            currentQoutPOFolderStackPanel = null;
            previousQoutPOFolderStackPanel = null;
            currentQoutPOFileStackPanel = null;
            previousQoutPOFileStackPanel = null;

            rfpsFoldersStackPanel.Children.Clear();

            for (int i = 0; i < quotationPOFoldersPageStackPanel.Count; i++)
            {
                rfpsFoldersStackPanel.Children.Add(quotationPOFoldersPageStackPanel[i]);
            }
        }

        private void OnClickBackRFPFiles(object sender, MouseButtonEventArgs e)
        {
            currentRFPFolderStackPanel = null;
            currentFileStackPanel = null;
            previousRFPFolderStackPanel = null;
            previousFileStackPanel = null;

            rfpsFoldersStackPanel.Children.Clear();

            for (int i = 0; i < rfpFoldersPageStackPanel.Count; i++)
            {
                rfpsFoldersStackPanel.Children.Add(rfpFoldersPageStackPanel[i]);
            }
        }

        private void OnClickBackTeams(object sender, MouseButtonEventArgs e)
        {
            currentRFPCategoryStackPanel = null;
            previousRFPCategoryStackPanel = null;
            currentFileStackPanel = null;
            previousFileStackPanel = null;

            rfpsFoldersStackPanel.Children.Clear();

            for (int i = 0; i < teamsPageStackPanel.Count; i++)
            {
                rfpsFoldersStackPanel.Children.Add(teamsPageStackPanel[i]);
            }
        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            if (currentExpander != null)
                previousExpander = currentExpander;

            currentExpander = (Expander)sender;

            if (previousExpander != currentExpander && previousExpander != null)
                previousExpander.IsExpanded = false;

            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            expanderColumn.Width = new GridLength(120);
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            currentExpander.Margin = new Thickness(12);
            expanderColumn.Width = new GridLength(50);
        }

        private void CheckLoggedInUser()
        {

            if (loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()) == false && loggedInUser.GetEmployeeDepartmentId() != COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID && loggedInUser.GetEmployeeTeamId()==COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
            {
                int index = -1;

                for (int i = 0; i < tmpRequestorsTeamsList.Count; i++)
                {
                    //if (loggedInUser.GetEmployeeId() == tmpRequestorsTeamsList[i].employee_id)
                   // {
                        index = i;
                        teamComboBox.Items.Add(tmpRequestorsTeamsList[i].requestor_team.team_name);
                        rfpRequestors.Add(tmpRequestorsTeamsList[i]);
                        requestorComboBox.Items.Add(tmpRequestorsTeamsList[index].employee_name);

                    // }
                }
                //if (index != -1)
                //{
                //    requestorComboBox.Items.Add(tmpRequestorsTeamsList[index].employee_name);
                //    tmpRequestors.Add(tmpRequestorsTeamsList[index]);
                //}
                if (teamComboBox.Items.Count == 1)
                {
                    teamCheckBox.IsChecked = true;
                    teamCheckBox.IsEnabled = false;
                    teamComboBox.SelectedIndex = 0;
                    teamComboBox.IsEnabled = false;
                }
                else
                {
                    teamCheckBox.IsChecked = true;
                    teamCheckBox.IsEnabled = false;
                    teamComboBox.SelectedIndex = 0;
                    requestorComboBox.IsEnabled = false;
                }
                //requestorComboBox.SelectedIndex = 0;
                //requestorCheckBox.IsChecked = true;
                //requestorCheckBox.IsEnabled = false;
                //requestorComboBox.IsEnabled = false;



                //index = rfpRequestors.FindIndex(x => x.team_id == loggedInUser.GetEmployeeTeamId());
                //
                // teamCheckBox.IsChecked = true;
                // requestorCheckBox.IsChecked = true;
                //
                // if (index != -1)
                // {
                //     teamComboBox.SelectedIndex = index;
                //     requestorComboBox.SelectedIndex = index;
                // }



            }
            else
            {
                InitializeTeamCombo();
                InitializeRequestorCombo();
            }
            //if(loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID)
            //{
            //    assignedOfficerCheckBox.IsChecked = true;
            //    assignedOfficerCheckBox.IsEnabled = false;
            //    assignedOfficerComboBox.IsEnabled = false;
            //    assignedOfficerComboBox.SelectedIndex = assignees.FindIndex(x => x.employee_id == loggedInUser.GetEmployeeId());
            //}

            //if (rfpRequestors.Find(x1 => x1.employee_id == loggedInUser.GetEmployeeId()).employee_id == 0)
            //    AddRFPBtn.IsEnabled = false;

            //if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_MANAGEMENT_TEAM_ID)
         //   if (loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()))
              //  AddRFPBtn.IsEnabled = false;

          //  if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID)
               // AddRFPBtn.IsEnabled = true;
            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
            {
                //int index = -1;

                //index = quotationAvabilityStatus.FindIndex(x => x.status_id == COMPANY_WORK_MACROS.RFP_PENDING_STOCK_RECEIVAL);

                //AvabilityStatusCheckBox.IsChecked = true;
                //
                //
                //if (index != -1)
                //{
                //    AvabilityStatusComboBox.SelectedIndex = index;
                //
                //}
                //
                //AvabilityStatusCheckBox.IsEnabled = false;
                //AvabilityStatusComboBox.IsEnabled = false;
                // AddRFPBtn.IsEnabled = false;
                InitializeTeamCombo();
                InitializeRequestorCombo();
                teamCheckBox.IsEnabled = true;
                teamComboBox.IsEnabled = true;
                requestorCheckBox.IsEnabled = true;
                requestorComboBox.IsEnabled = false;
                teamCheckBox.IsChecked = true;
                teamComboBox.SelectedIndex = 0;
                requestorCheckBox.IsChecked = false;
            }




        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddFolderStackPanel(ref StackPanel parentStackPanel, ref StackPanel folderStackPanel, String folderName)
        {
            folderStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            folderStackPanel.Orientation = Orientation.Horizontal;
            //folderStackPanel.Tag = i;

           // if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID) { }
               // AddRFPBtn.IsEnabled = true;


            folderStackPanel.Margin = new Thickness(12, 0, 0, 0);

            Image fileImage = new Image();

            fileImage = new Image { Source = new BitmapImage(new Uri(@"Icons\folder_icon.png", UriKind.Relative)) };
            folderStackPanel.MouseLeftButtonDown += OnClickQuotationPOFolder;

            fileImage.Height = 50;
            fileImage.Width = 50;

            Label fileName = new Label();
            fileName.Style = (Style)FindResource("stackPanelItemHeader");
            fileName.Content = folderName;

            folderStackPanel.Children.Add(fileImage);
            folderStackPanel.Children.Add(fileName);

            parentStackPanel.Children.Add(folderStackPanel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetYearComboBox()
        {
            yearCheckBox.IsChecked = true;
            yearComboBox.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void OnBtnClickedAddRFP(object sender, RoutedEventArgs e)
        {
            RFP selectedRfp = new RFP();
            int viewAddCondition = 1;
            AddRFPWindow addRFPWindow = new AddRFPWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref viewAddCondition, ref selectedRfp, false);
            addRFPWindow.Closed += OnClosedAddRFPWindow;
            addRFPWindow.Show();
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString(
                ));
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;

            int viewAddCondition = COMPANY_WORK_MACROS.RFP_VIEW_CONDITION;
            AddRFPWindow addRFPWindow = new AddRFPWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref viewAddCondition, ref selectedRfp, false);
            addRFPWindow.Closed += OnClosedAddRFPWindow;
            addRFPWindow.Show();
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString());
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;
            selectedRfp.CancelRFP();
            if (!GetRFPs())
                return;
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnBtnClickRevise(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString());
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;

            int viewAddCondition = COMPANY_WORK_MACROS.RFP_REVISE_CONDITION;
            AddRFPWindow addRFPWindow = new AddRFPWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref viewAddCondition, ref selectedRfp, false);
            addRFPWindow.Closed += OnClosedAddRFPWindow;
            addRFPWindow.Show();
        }

        private void OnBtnClickEdit(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString());
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;

            int viewAddCondition = COMPANY_WORK_MACROS.RFP_EDIT_CONDITION;
            AddRFPWindow addRFPWindow = new AddRFPWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref viewAddCondition, ref selectedRfp, false);
            addRFPWindow.Closed += OnClosedAddRFPWindow;
            addRFPWindow.Show();
        }
        
        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            //ExcelExport export = new ExcelExport(rfpsGrid);
        }

        private void OnBtnClickApprove(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString());
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;

            ApproveRFPItemsWindow aprroveRFPItemsWindow = new ApproveRFPItemsWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedRfp);
            aprroveRFPItemsWindow.Closed += OnClosedAddRFPWindow;
            aprroveRFPItemsWindow.Show();
        }

        private void OnClosedAddRFPWindow(object sender, EventArgs e)
        {
            if (!GetRFPs())
                return;

            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnBtnClickviewQuotaition(object sender, RoutedEventArgs e)
        {

        }
        
        private void OnBtnClickviewPO(object sender, RoutedEventArgs e)
        {

        }
        
        private void OnBtnClickChangeAssignee(object sender, RoutedEventArgs e)
        {

        }
        
        private void OnButtonClickDeleteRFP(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel buttonStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)buttonStackPanel.Parent;
            Grid currentGrid = (Grid)currentExpander.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Children[0];
            Label rfpIdLabel = (Label)currentStackPanel.Children[0];

            PROCUREMENT_STRUCTS.RFP_MAX_STRUCT currentRFP = rfps.Find(x1 => x1.rfp_id == rfpIdLabel.Content.ToString());
            RFP selectedRfp = new RFP();

            if (!selectedRfp.InitializeRFP(currentRFP.requestor_team_id, currentRFP.rfp_serial, currentRFP.rfp_version))
                return;

            if (!commonQueries.GetRfpItemsMapping(selectedRfp.GetRFPSerial(), selectedRfp.GetRFPVersion(), selectedRfp.GetRFPRequestorTeamId(), ref rfpMappedItems))
                return;

            for (int i = 0; i < selectedRfp.rfpItems.Count; i++)
            {
                PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT foundItem = rfpMappedItems.Find(f => f.rfp_item_number == selectedRfp.rfpItems[i].rfp_item_number);

                if (foundItem.rfp_item_number != 0)
                {
                    PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = new PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT();
                    rfpItem.item_vendors = new List<PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT>();

                    rfpItem.product_category = foundItem.product_category;
                    rfpItem.product_type = foundItem.product_type;
                    rfpItem.product_brand = foundItem.product_brand;
                    rfpItem.product_model = foundItem.product_model;
                    rfpItem.product_specs = foundItem.product_specs;

                    rfpItem.product_category = foundItem.product_category;
                    rfpItem.product_type = foundItem.product_type;
                    rfpItem.product_brand = foundItem.product_brand;
                    rfpItem.product_model = foundItem.product_model;

                    rfpItem.itemSelected = selectedRfp.rfpItems[i].itemSelected;
                    rfpItem.item_description = selectedRfp.rfpItems[i].item_description;
                    rfpItem.measure_unit = selectedRfp.rfpItems[i].measure_unit;
                    rfpItem.measure_unit_id = selectedRfp.rfpItems[i].measure_unit_id;
                    rfpItem.item_notes = selectedRfp.rfpItems[i].item_notes;
                    rfpItem.rfp_item_number = selectedRfp.rfpItems[i].rfp_item_number;
                    rfpItem.item_quantity = selectedRfp.rfpItems[i].item_quantity;
                    rfpItem.item_status = selectedRfp.rfpItems[i].item_status;
                    
                    
                    for (int j = 0; j < selectedRfp.rfpItems[i].item_vendors.Count; j++)
                    {
                        PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT brand = new PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT();
                        brand.vendor_id = selectedRfp.rfpItems[i].item_vendors[j].vendor_id;
                        brand.vendor_name = selectedRfp.rfpItems[i].item_vendors[j].vendor_name;
                        rfpItem.item_vendors.Add(brand);
                    }
                    selectedRfp.rfpItems.RemoveAt(i);
                    selectedRfp.rfpItems.Insert(i, rfpItem);

                }
            }
            if (!selectedRfp.DeleteRFP())
                return;
            if (!GetRFPs())
                return;

            InitializeRFPsStackPanel();
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION/TEXT CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedRequestorCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedAssignedOfficerCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedTeamCombo(object sender, SelectionChangedEventArgs e)
        {

            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedWorkFormCombo(object sender, SelectionChangedEventArgs e)
        {
            //if (workFormComboBox.SelectedIndex == 0)
            //{
            //    orderCheckBox.IsChecked = false;
            //    orderCheckBox.IsEnabled = false;
            //    orderComboBox.Items.Clear();
            //}
            if (workFormComboBox.SelectedIndex == 0)
            {
                orderCheckBox.IsEnabled = true;
                orderCheckBox.IsChecked = true;
                InitializeWorkOrderCombo();
            }
            else if (workFormComboBox.SelectedIndex == 1)
            {
                orderCheckBox.IsEnabled = true;
                orderCheckBox.IsChecked = true;
                InitializeMaintContractCombo();
            }
            else if (workFormComboBox.SelectedIndex == 2)
            {
                orderCheckBox.IsEnabled = true;
                orderCheckBox.IsChecked = true;
                InitializeProjectsCombo();
            }
            else if (workFormComboBox.SelectedIndex == 3)
            {
                orderCheckBox.IsEnabled = true;
                orderCheckBox.IsChecked = true;
                InitializeStockComboBox();
            }
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }
        private void OnSelChangedOrdersCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        private void OnSelChangedItemStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }
        private void AvabilityStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFPsStackPanel();
            InitializeRFPsGrid();
            InitializeRFPsFolderView();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECK/UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void OnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        private void OnUnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = false;
            searchTextBox.Text = "";
        }

        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = true;
            SetYearComboBox();
        }

        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = false;
            yearComboBox.SelectedIndex = -1;
        }


        private void OnCheckRequestorCheckBox(object sender, RoutedEventArgs e)
        {
            requestorComboBox.IsEnabled = true;

            requestorComboBox.SelectedIndex = rfpRequestors.FindIndex(x1 => x1.requestor_team.team_id == loggedInUser.GetEmployeeTeamId());
            if (requestorComboBox.SelectedIndex == -1)
                requestorComboBox.SelectedIndex = 0;
        }

        private void OnUncheckRequestorCheckBox(object sender, RoutedEventArgs e)
        {
            requestorComboBox.IsEnabled = false;
            requestorComboBox.SelectedIndex = -1;
        }

        private void OnCheckAssignedOfficerCheckBox(object sender, RoutedEventArgs e)
        {
            assignedOfficerComboBox.IsEnabled = true;
            assignedOfficerComboBox.SelectedIndex = 0;
        }

        private void OnUncheckAssignedOfficerCheckBox(object sender, RoutedEventArgs e)
        {
            assignedOfficerComboBox.IsEnabled = false;
            assignedOfficerComboBox.SelectedIndex = -1;
        }

        private void OnCheckTeamCheckBox(object sender, RoutedEventArgs e)
        {
            teamComboBox.IsEnabled = true;
            teamComboBox.SelectedIndex = rfpRequestors.FindIndex(x1 => x1.requestor_team.team_id == loggedInUser.GetEmployeeTeamId());
            if (teamComboBox.SelectedIndex == -1)
                teamComboBox.SelectedIndex = 0;
        }

        private void OnUncheckTeamCheckBox(object sender, RoutedEventArgs e)
        {
            teamComboBox.IsEnabled = false;
            teamComboBox.SelectedIndex = -1;
        }

        private void OnCheckWorkFormCheckBox(object sender, RoutedEventArgs e)
        {
            workFormComboBox.IsEnabled = true;
            workFormComboBox.SelectedIndex = 0;
        }

        private void OnUncheckWorkFormCheckBox(object sender, RoutedEventArgs e)
        {
            workFormComboBox.IsEnabled = false;
            workFormComboBox.SelectedIndex = -1;
            orderCheckBox.IsChecked = false;
            orderCheckBox.IsEnabled = false;
        }
        private void OnCheckOrderCheckBox(object sender, RoutedEventArgs e)
        {
            orderComboBox.IsEnabled = true;
        }

        private void OnUnCheckOrderCheckBox(object sender, RoutedEventArgs e)
        {
            orderComboBox.IsEnabled = false;
            orderComboBox.SelectedIndex = -1;
        }

        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;
            statusComboBox.SelectedIndex = 0;
        }

        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = false;
            statusComboBox.SelectedIndex = -1;
        }

        private void OnCheckItemStatusCheckBox(object sender, RoutedEventArgs e)
        {
            itemStatusComboBox.IsEnabled = true;
            itemStatusComboBox.SelectedIndex = 0;
        }

        private void OnUnCheckItemStatusCheckBox(object sender, RoutedEventArgs e)
        {
            itemStatusComboBox.IsEnabled = false;
            itemStatusComboBox.SelectedIndex = -1;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedDashboard(object sender, MouseButtonEventArgs e)
        {
            DashboardPage dashboardPagePage = new DashboardPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(dashboardPagePage);

        }
        private void OnButtonClickedRFPs(object sender, MouseButtonEventArgs e)
        {
            RFPsPage rfpPage = new RFPsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(rfpPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }
        private void OnButtonClickedCompanyProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage categoriesPage = new CategoriesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(categoriesPage);
        }
        private void OnButtonClickedGenericProducts(object sender, MouseButtonEventArgs e)
        {
            GenericProductsPage genericProductsPage = new GenericProductsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(genericProductsPage);
        }
        private void OnButtonClickedStoreLocations(object sender, MouseButtonEventArgs e)
        {
            WarehouseLocationsPage genericProductsPage = new WarehouseLocationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(genericProductsPage);
        }
        private void OnButtonClickedEntryPermits(object sender, MouseButtonEventArgs e)
        {
            EntryPermitPage entryPermitPage = new EntryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(entryPermitPage);

        }
        private void OnButtonClickedReleasePermits(object sender, MouseButtonEventArgs e)
        {
            ReleasePermitPage releasePermitPage = new ReleasePermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(releasePermitPage);
        }
        private void OnButtonClickedStockAvailability(object sender, MouseButtonEventArgs e)
        {
            StockAvailabilityPage stockAvailabilityPage = new StockAvailabilityPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);

            this.NavigationService.Navigate(stockAvailabilityPage);
        }
        private void OnButtonClickedRentryPermit(object sender, MouseButtonEventArgs e)
        {
            RentryPermitPage rentryPermitPage = new RentryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(rentryPermitPage);
        }
        private void OnButtonClickedReservations(object sender, MouseButtonEventArgs e)
        {
            ReservationsPage reservationsPage = new ReservationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(reservationsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEW TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            stackViewScrolllViewer.Visibility = Visibility.Visible;
            gridViewScrollViewer.Visibility = Visibility.Collapsed;
            folderViewScrollViewer.Visibility = Visibility.Collapsed;

            listViewBorder.Style = (Style)FindResource("selectedMainTabBorder");
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            tableViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            folderViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            folderViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            gridViewScrollViewer.Visibility = Visibility.Visible;
            stackViewScrolllViewer.Visibility = Visibility.Collapsed;
            folderViewScrollViewer.Visibility = Visibility.Collapsed;

            listViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            tableViewBorder.Style = (Style)FindResource("selectedMainTabBorder");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            folderViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            folderViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
        }

        private void OnClickFolderView(object sender, MouseButtonEventArgs e)
        {
            gridViewScrollViewer.Visibility = Visibility.Collapsed;
            stackViewScrolllViewer.Visibility = Visibility.Collapsed;
            folderViewScrollViewer.Visibility = Visibility.Visible;

            listViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            tableViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            folderViewBorder.Style = (Style)FindResource("selectedMainTabBorder");
            folderViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
        }
    }
}
