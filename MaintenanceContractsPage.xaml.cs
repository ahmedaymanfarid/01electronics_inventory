using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for MaintenanceContractsPage.xaml
    /// </summary>
    public partial class MaintenanceContractsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        private MaintenanceContract selectedMaintContract;
        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> businessDevelopmentEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MAX_STRUCT> maintContracts = new List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MAX_STRUCT>();
        private List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MAX_STRUCT> maintContractsAfterFiltering = new List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MAX_STRUCT>();
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> productTypes = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> brandTypes = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
        private List<BASIC_STRUCTS.STATUS_STRUCT> contractStatuses = new List<BASIC_STRUCTS.STATUS_STRUCT>();

        private int selectedYear;
        private int selectedQuarter;

        private int selectedSales;
        private int selectedPreSales;

        private int selectedProduct;
        private int selectedBrand;
        private int selectedStatus;
        private int salesPersonTeam;

        int viewAddCondition;

        private Grid currentGrid;
        private Expander currentExpander;
        private Expander previousExpander;

        public MaintenanceContractsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            selectedMaintContract = new MaintenanceContract();

            InitializeComponent();


            if (!GetMaintenanceContracts())
                return;
            
            InitializeYearsComboBox();
            InitializeQuartersComboBox();
            InitializeStatusComboBox();
            
            if (!InitializeSalesComboBox())
                return;
            
            if (!InitializePreSalesComboBox())
                return;
            
            if (!InitializeProductsComboBox())
                return;
            
            if (!InitializeBrandsComboBox())
                return;
            
            SetDefaultSettings();
            
            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
           
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INTIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool GetMaintenanceContracts()
        {
            if (!commonQueries.GetMaintenanceContracts(ref maintContracts))
                return false;
            return true;
        }
        private void InitializeYearsComboBox()
        {
            for (int year = BASIC_MACROS.CRM_START_YEAR; year <= DateTime.Now.Year; year++)
                yearComboBox.Items.Add(year);
        }
        private void InitializeQuartersComboBox()
        {
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterComboBox.Items.Add(commonFunctions.GetQuarterName(i + 1));

        }

        private bool InitializeSalesComboBox()
        {
            if (!commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref salesEmployeesList))
                return false;

            if (!commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID, ref businessDevelopmentEmployeesList))
                return false;

            for (int i = 0; i < businessDevelopmentEmployeesList.Count(); i++)
            {
                salesEmployeesList.Add(businessDevelopmentEmployeesList[i]);
            }

            if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT loggedInUserStruct = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                loggedInUserStruct.employee_id = loggedInUser.GetEmployeeId();
                loggedInUserStruct.employee_name = loggedInUser.GetEmployeeName();
                loggedInUserStruct.department.department_id = loggedInUser.GetEmployeeDepartmentId();
                loggedInUserStruct.department.department_name = loggedInUser.GetEmployeeDepartment();
                loggedInUserStruct.team.team_id = loggedInUser.GetEmployeeTeamId();
                loggedInUserStruct.team.team_name = loggedInUser.GetEmployeeTeam();

                salesEmployeesList.Add(loggedInUserStruct);
            }

            for (int i = 0; i < salesEmployeesList.Count; i++)
                salesComboBox.Items.Add(salesEmployeesList[i].employee_name);

            return true;
        }
        private bool InitializePreSalesComboBox()
        {
            if (!commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployeesList))
                return false;

            if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT loggedInUserStruct = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                loggedInUserStruct.employee_id = loggedInUser.GetEmployeeId();
                loggedInUserStruct.employee_name = loggedInUser.GetEmployeeName();
                loggedInUserStruct.department.department_id = loggedInUser.GetEmployeeDepartmentId();
                loggedInUserStruct.department.department_name = loggedInUser.GetEmployeeDepartment();
                loggedInUserStruct.team.team_id = loggedInUser.GetEmployeeTeamId();
                loggedInUserStruct.team.team_name = loggedInUser.GetEmployeeTeam();

                preSalesEmployeesList.Add(loggedInUserStruct);
            }

            for (int i = 0; i < preSalesEmployeesList.Count; i++)
                preSalesComboBox.Items.Add(preSalesEmployeesList[i].employee_name);

            return true;
        }

        private bool InitializeProductsComboBox()
        {
            if (!commonQueries.GetCompanyProducts(ref productTypes))
                return false;

            for (int i = 0; i < productTypes.Count; i++)
                productComboBox.Items.Add(productTypes[i].product_name);

            return true;
        }

        private bool InitializeBrandsComboBox()
        {

            if (!commonQueries.GetCompanyBrands(ref brandTypes))
                return false;

            for (int i = 0; i < brandTypes.Count; i++)
                brandComboBox.Items.Add(brandTypes[i].brand_name);

            brandComboBox.IsEnabled = false;
            return true;
        }

        private void InitializeStatusComboBox()
        {
            if (!commonQueries.GetMaintenanceContractsStatus(ref contractStatuses))
                return;

            for (int i = 0; i < contractStatuses.Count; i++)
            {
                statusComboBox.Items.Add(contractStatuses[i].status_name);
            }

            statusComboBox.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ResetComboBoxes()
        {
            yearComboBox.SelectedIndex = -1;
            quarterComboBox.SelectedIndex = -1;

            salesComboBox.SelectedIndex = -1;
            preSalesComboBox.SelectedIndex = -1;

            productComboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;

            statusComboBox.SelectedIndex = -1;
        }
        private void DisableComboBoxes()
        {
            yearComboBox.IsEnabled = false;
            quarterComboBox.IsEnabled = false;
            salesComboBox.IsEnabled = false;
            preSalesComboBox.IsEnabled = false;
            productComboBox.IsEnabled = false;
            brandComboBox.IsEnabled = false;
            statusComboBox.IsEnabled = false;
        }
        private void DisableViewButton()
        {
            //viewButton.IsEnabled = false;
        }

        private void SetDefaultSettings()
        {
            DisableViewButton();
            ////DisableReviseButton();
            DisableComboBoxes();
            ResetComboBoxes();

            yearCheckBox.IsChecked = true;
            yearCheckBox.IsEnabled = false;

            if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;

            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;

                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID || loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID)
            {
                salesCheckBox.IsChecked = true;
                salesCheckBox.IsEnabled = false;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;

            }
            else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;

                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetYearComboBox()
        {
            yearComboBox.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;
        }
        private void SetQuarterComboBox()
        {
            if (yearComboBox.SelectedIndex == DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR)
                quarterComboBox.SelectedIndex = commonFunctions.GetCurrentQuarter() - 1;
            else
                quarterComboBox.SelectedIndex = 0;
        }

        private bool SetMaintContractsStackPanel()
        {
            maintContractsStackPanel.Children.Clear();

            maintContractsAfterFiltering.Clear();

            for (int i = 0; i < maintContracts.Count; i++)
            {
                DateTime currentMaintContractDate = DateTime.Parse(maintContracts[i].issue_date);

                bool salesPersonCondition = selectedSales != maintContracts[i].sales_person_id;
                bool assigneeCondition;

                if (selectedPreSales == maintContracts[i].maintenance_contract_proposer_id || (selectedPreSales == maintContracts[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productType.type_id == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productBrand.brand_id == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentMaintContractDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentMaintContractDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && maintContracts[i].maintenance_contract_status_id != selectedStatus)
                    continue;

                maintContractsAfterFiltering.Add(maintContracts[i]);

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;


                Label offerIdLabel = new Label();
                offerIdLabel.Content = maintContracts[i].maintenance_contract_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = maintContracts[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintContracts[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintContracts[i].company_name + " -" + maintContracts[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT> temp = maintContracts[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT tempType1 = temp[j].productType;
                    PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    productTypeAndBrandLabel.Content += tempType1.product_name + " -" + tempBrand1.brand_name;

                    if (j != temp.Count() - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label contractStatusLabel = new Label();
                contractStatusLabel.Content = maintContracts[i].maintenance_contract_status;
                contractStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");


                if (maintContracts[i].maintenance_contract_status_id == COMPANY_WORK_MACROS.NEW_MAINTENANCE_CONTRACT || maintContracts[i].maintenance_contract_status_id == COMPANY_WORK_MACROS.PENDING_RENEWAL_MAINTENANCE_CONTRACT)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (maintContracts[i].maintenance_contract_status_id == COMPANY_WORK_MACROS.CLOSED_MAINTENANCE_CONTRACT || maintContracts[i].maintenance_contract_status_id == COMPANY_WORK_MACROS.RENEWED_MAINTENANCE_CONTRACT || maintContracts[i].maintenance_contract_status_id == COMPANY_WORK_MACROS.NEWLY_RENEWED_MAINTENANCE_CONTRACT)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = contractStatusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.HorizontalAlignment = HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);

                ListBox listBox = new ListBox();
                listBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                listBox.SelectionChanged += new SelectionChangedEventHandler(OnSelChangedListBox);

                ListBoxItem viewButton = new ListBoxItem();
                viewButton.Content = "View";
                viewButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                listBox.Items.Add(viewButton);

                ListBoxItem editContractButton = new ListBoxItem();
                editContractButton.Content = "Edit Contract";
                editContractButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID || 
                   (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION) ||
                   (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) ||
                   loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.DOCUMENT_CONTROL_TEAM_ID || (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)||
                    loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.DISPATCH_TEAM_ID)
                    listBox.Items.Add(editContractButton);

                ListBoxItem showHistoryButton = new ListBoxItem();
                showHistoryButton.Content = "Show History";
                showHistoryButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                listBox.Items.Add(showHistoryButton);

                expander.Content = listBox;

                fullStackPanel.Children.Add(offerIdLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);

                Grid grid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                ColumnDefinition column3 = new ColumnDefinition();
                column2.MaxWidth = 95;
                column3.MaxWidth = 50;

                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                grid.ColumnDefinitions.Add(column3);

                grid.Children.Add(fullStackPanel);
                grid.Children.Add(borderIcon);
                grid.Children.Add(expander);

                Grid.SetColumn(fullStackPanel, 0);
                Grid.SetColumn(borderIcon, 1);
                Grid.SetColumn(expander, 2);

                maintContractsStackPanel.Children.Add(grid);
            }

            return true;
        }

        private bool SetMaintContractsGrid()
        {

            maintContractsGrid.Children.Clear();
            maintContractsGrid.RowDefinitions.Clear();
            maintContractsGrid.ColumnDefinitions.Clear();

            int counter = 0;

            Label orderIdHeader = new Label();
            orderIdHeader.Content = "MaintContract ID";
            orderIdHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderSalesHeader = new Label();
            orderSalesHeader.Content = "Sales Engineer";
            orderSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderPreSalesHeader = new Label();
            orderPreSalesHeader.Content = "Pre-Sales Engineer";
            orderPreSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderCompanyContactHeader = new Label();
            orderCompanyContactHeader.Content = "Contact Info";
            orderCompanyContactHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProductsHeader = new Label();
            orderProductsHeader.Content = "Products";
            orderProductsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderStatusHeader = new Label();
            orderStatusHeader.Content = "MaintContract Status";
            orderStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            maintContractsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(orderIdHeader, 0);
            Grid.SetColumn(orderIdHeader, 0);
            maintContractsGrid.Children.Add(orderIdHeader);

            Grid.SetRow(orderSalesHeader, 0);
            Grid.SetColumn(orderSalesHeader, 1);
            maintContractsGrid.Children.Add(orderSalesHeader);

            Grid.SetRow(orderPreSalesHeader, 0);
            Grid.SetColumn(orderPreSalesHeader, 2);
            maintContractsGrid.Children.Add(orderPreSalesHeader);

            Grid.SetRow(orderCompanyContactHeader, 0);
            Grid.SetColumn(orderCompanyContactHeader, 3);
            maintContractsGrid.Children.Add(orderCompanyContactHeader);

            Grid.SetRow(orderProductsHeader, 0);
            Grid.SetColumn(orderProductsHeader, 4);
            maintContractsGrid.Children.Add(orderProductsHeader);

            Grid.SetRow(orderStatusHeader, 0);
            Grid.SetColumn(orderStatusHeader, 5);
            maintContractsGrid.Children.Add(orderStatusHeader);

            int currentRowNumber = 1;


            for (int i = 0; i < maintContracts.Count; i++)
            {
                DateTime currentMaintContractDate = DateTime.Parse(maintContracts[i].issue_date);

                bool salesPersonCondition = selectedSales != maintContracts[i].sales_person_id;

                bool assigneeCondition;

                if (selectedPreSales == maintContracts[i].maintenance_contract_proposer_id || (selectedPreSales == maintContracts[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productType.type_id == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productBrand.brand_id == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentMaintContractDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentMaintContractDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && maintContracts[i].maintenance_contract_status_id != selectedStatus)
                    continue;

                RowDefinition currentRow = new RowDefinition();
                maintContractsGrid.RowDefinitions.Add(currentRow);

                Label orderIdLabel = new Label();
                orderIdLabel.Content = maintContracts[i].maintenance_contract_id;
                orderIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(orderIdLabel, currentRowNumber);
                Grid.SetColumn(orderIdLabel, 0);
                maintContractsGrid.Children.Add(orderIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = maintContracts[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                maintContractsGrid.Children.Add(salesLabel);

                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintContracts[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                maintContractsGrid.Children.Add(preSalesLabel);

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintContracts[i].company_name + " - " + maintContracts[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(companyAndContactLabel, currentRowNumber);
                Grid.SetColumn(companyAndContactLabel, 3);
                maintContractsGrid.Children.Add(companyAndContactLabel);


                Grid productGrid = new Grid();
                productGrid.ShowGridLines = true;


                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());

                productGrid.RowDefinitions.Add(new RowDefinition());


                Label rowColumnHeader = new Label();
                rowColumnHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(rowColumnHeader);
                Grid.SetRow(rowColumnHeader, 0);
                Grid.SetColumn(rowColumnHeader, 0);

                Label typeHeader = new Label();
                typeHeader.Content = "Type";
                typeHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(typeHeader);
                Grid.SetRow(typeHeader, 0);
                Grid.SetColumn(typeHeader, 1);


                Label brandHeader = new Label();
                brandHeader.Content = "Brand";
                brandHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(brandHeader);
                Grid.SetRow(brandHeader, 0);
                Grid.SetColumn(brandHeader, 2);


                Label modelHeader = new Label();
                modelHeader.Content = "Model";
                modelHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(modelHeader);
                Grid.SetRow(modelHeader, 0);
                Grid.SetColumn(modelHeader, 3);


                List<PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT> temp = maintContracts[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT tempType1 = temp[j].productType;
                    PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT tempBrand1 = temp[j].productBrand;
                    PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT tempModel1 = temp[j].productModel;

                    if (tempType1.type_id == 0)
                        continue;

                    productGrid.RowDefinitions.Add(new RowDefinition());

                    int tempNumber = j + 1;
                    Label productNumberHeader = new Label();
                    productNumberHeader.Content = "Product" + " " + tempNumber;
                    productNumberHeader.Style = (Style)FindResource("tableSubHeaderItem");

                    productGrid.Children.Add(productNumberHeader);
                    Grid.SetRow(productNumberHeader, j + 1);
                    Grid.SetColumn(productNumberHeader, 0);

                    Label type = new Label();
                    type.Content = tempType1.product_name;
                    type.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(type);
                    Grid.SetRow(type, j + 1);
                    Grid.SetColumn(type, 1);

                    Label brand = new Label();
                    brand.Content = tempBrand1.brand_name;
                    brand.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(brand);
                    Grid.SetRow(brand, j + 1);
                    Grid.SetColumn(brand, 2);

                    Label model = new Label();
                    model.Content = tempModel1.model_name;
                    model.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(model);
                    Grid.SetRow(model, j + 1);
                    Grid.SetColumn(model, 3);
                }

                maintContractsGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);

                Label contractStatusLabel = new Label();
                contractStatusLabel.Content = maintContracts[i].maintenance_contract_status;
                contractStatusLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintContractsGrid.Children.Add(contractStatusLabel);
                Grid.SetRow(contractStatusLabel, currentRowNumber);
                Grid.SetColumn(contractStatusLabel, 5);


                //currentRow.MouseLeftButtonDown += OnBtnClickWorkorderItem;

                currentRowNumber++;


            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            ////DisableReviseButton();

            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;



            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;



            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].type_id;
            else
                selectedProduct = 0;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brand_id;
            else
                selectedBrand = 0;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = contractStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = true;

            SetYearComboBox();
        }

        private void OnCheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.IsEnabled = true;

            SetQuarterComboBox();
        }

        private void OnCheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.IsEnabled = true;


            salesComboBox.SelectedIndex = 0;

            for (int i = 0; i < salesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == salesEmployeesList[i].employee_id)
                    salesComboBox.SelectedIndex = i;
        }
        private void OnCheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.IsEnabled = true;


            preSalesComboBox.SelectedIndex = 0;

            for (int i = 0; i < preSalesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == preSalesEmployeesList[i].employee_id)
                    preSalesComboBox.SelectedIndex = i;
        }

        private void OnCheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.IsEnabled = true;


            productComboBox.SelectedIndex = 0;
        }

        private void OnCheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.IsEnabled = true;


            brandComboBox.SelectedIndex = 0;
        }

        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;


            statusComboBox.SelectedIndex = 0;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.SelectedItem = null;
            yearComboBox.IsEnabled = false;

        }


        private void OnUncheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.SelectedItem = null;
            quarterComboBox.IsEnabled = false;

        }


        private void OnUncheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.SelectedItem = null;
            salesComboBox.IsEnabled = false;

        }
        private void OnUncheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.SelectedItem = null;
            preSalesComboBox.IsEnabled = false;

        }

        private void OnUncheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.SelectedItem = null;
            productComboBox.IsEnabled = false;

        }

        private void OnUncheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.SelectedItem = null;
            brandComboBox.IsEnabled = false;

        }

        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.SelectedItem = null;
            statusComboBox.IsEnabled = false;

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
        private void OnButtonClickedRecievalNotes(object sender, MouseButtonEventArgs e)
        {
            RecievalNotePage recievalNotePage = new RecievalNotePage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(recievalNotePage);
        }
        private void OnClosedMaintContractWindow(object sender, EventArgs e)
        {
            if (!GetMaintenanceContracts())
                return;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }
        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(maintContractsGrid);
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
            //expanderColumn.Width = new GridLength(Width = 120);
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            expanderColumn.MaxWidth = 120;
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            currentExpander.VerticalAlignment = VerticalAlignment.Center;
            expanderColumn.MaxWidth = 50;
        }

        private void OnSelChangedListBox(object sender, SelectionChangedEventArgs e)
        {
            ListBox tempListBox = (ListBox)sender;
            if (tempListBox.SelectedItem != null)
            {
                ListBoxItem currentItem = (ListBoxItem)tempListBox.Items[tempListBox.SelectedIndex];
                Expander currentExpander = (Expander)tempListBox.Parent;

                currentGrid = (Grid)currentExpander.Parent;

                if (currentItem.Content.ToString() == "View")
                {
                    OnBtnClickView();
                }
                else if (currentItem.Content.ToString() == "Edit Contract")
                {
                    OnBtnClickEdit();
                }
                else if (currentItem.Content.ToString() == "Show History")
                {
                    OnBtnClickShowHistory();
                }
                tempListBox.SelectedIndex = -1;
            }
        }
        private void OnBtnClickView()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION;

            MaintenanceContract MaintContract = new MaintenanceContract();

            MaintContract.InitializeMaintenanceContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].maintenance_contract_serial, maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].maintenance_contract_version);
            MaintenanceContractsWindow workOrderWindow = new MaintenanceContractsWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref MaintContract, viewAddCondition, false);
            
            workOrderWindow.Show();
        }
        private void OnBtnClickShowHistory()
        {
            int contractSerial = maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].maintenance_contract_serial;
            MaintContractHistoryWindow workOrderWindow = new MaintContractHistoryWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, contractSerial);
            
            workOrderWindow.Closed += OnClosedMaintContractWindow;
            workOrderWindow.Show();
        }
        private void OnBtnClickEdit()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION;

            MaintenanceContract MaintContract = new MaintenanceContract();

            MaintContract.InitializeMaintenanceContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].maintenance_contract_serial, maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].maintenance_contract_version);

            MaintenanceContractsWindow workOrderWindow = new MaintenanceContractsWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref MaintContract, viewAddCondition, false);
            
            workOrderWindow.Closed += OnClosedMaintContractWindow;
            workOrderWindow.Show();
        }

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Visible;
            gridScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Collapsed;
            gridScrollViewer.Visibility = Visibility.Visible;
        }

       
    }
}
