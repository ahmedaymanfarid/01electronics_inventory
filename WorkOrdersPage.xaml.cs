﻿using System;
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
using _01electronics_library;
using _01electronics_inventory;
using _01electronics_windows_library;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for WorkOrdersPage.xaml
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        private WorkOrder selectedWorkOrder;
        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrders = new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
        private List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrdersAfterFiltering = new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> productTypes = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> brandTypes = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
        private List<BASIC_STRUCTS.STATUS_STRUCT> orderStatuses = new List<BASIC_STRUCTS.STATUS_STRUCT>();

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

        public WorkOrdersPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            if (!GetWorkOrders())
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

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetWorkOrders()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return false;
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INTIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

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
            if (!commonQueries.GetWorkOrderStatus(ref orderStatuses))
                return;

            for (int i = 0; i < orderStatuses.Count; i++)
            {
                statusComboBox.Items.Add(orderStatuses[i].status_name);
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
        private void EnableViewButton()
        {
            //viewButton.IsEnabled = true;
        }

        //private void DisableReviseButton()
        //{
        //    reviseButton.IsEnabled = false;
        //}
        //private void EnableReviseButton()
        //{
        //    reviseButton.IsEnabled = true;
        //}

        private void SetDefaultSettings()
        {
            DisableViewButton();
            DisableComboBoxes();
            ResetComboBoxes();

            yearCheckBox.IsChecked = true;
            yearCheckBox.IsEnabled = false;

            //if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            //{
            //    salesCheckBox.IsChecked = false;
            //    salesCheckBox.IsEnabled = true;
            //    salesComboBox.IsEnabled = false;
            //
            //    preSalesCheckBox.IsChecked = false;
            //    preSalesCheckBox.IsEnabled = true;
            //    preSalesComboBox.IsEnabled = false;
            //}
            //else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            //{
            //    salesCheckBox.IsChecked = false;
            //    salesCheckBox.IsEnabled = true;
            //    salesComboBox.IsEnabled = false;
            //
            //    preSalesCheckBox.IsChecked = false;
            //    preSalesCheckBox.IsEnabled = true;
            //    preSalesComboBox.IsEnabled = false;
            //
            //    addButton.IsEnabled = false;
            //}
            //else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            //{
            //    preSalesCheckBox.IsChecked = false;
            //    preSalesCheckBox.IsEnabled = true;
            //    preSalesComboBox.IsEnabled = false;
            //
            //    salesCheckBox.IsChecked = false;
            //    salesCheckBox.IsEnabled = true;
            //    salesComboBox.IsEnabled = false;
            //}
            //else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            //{
            //    salesCheckBox.IsChecked = true;
            //    salesCheckBox.IsEnabled = false;
            //    salesComboBox.IsEnabled = false;
            //
            //    preSalesCheckBox.IsChecked = false;
            //    preSalesCheckBox.IsEnabled = true;
            //    preSalesComboBox.IsEnabled = false;
            //
            //    addButton.IsEnabled = false;
            //}
            //else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            //{
            //    preSalesCheckBox.IsChecked = false;
            //    preSalesCheckBox.IsEnabled = true;
            //    preSalesComboBox.IsEnabled = false;
            //
            //    salesCheckBox.IsChecked = false;
            //    salesCheckBox.IsEnabled = true;
            //    salesComboBox.IsEnabled = false;
            //}
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


        private bool SetWorkOrdersStackPanel()
        {
            workOrdersStackPanel.Children.Clear();

            workOrdersAfterFiltering.Clear();

            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;
                bool assigneeCondition;

                if (selectedPreSales == workOrders[i].offer_proposer_id || (selectedPreSales == workOrders[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.type_id == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brand_id == selectedBrand)
                        brandCondition |= true;

                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    String tempId = workOrders[i].order_id;
                    String tempCompanyName = workOrders[i].company_name;
                    String tempContactName = workOrders[i].contact_name;


                    if (tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tempCompanyName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tempContactName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {

                    }
                    else
                        continue;
                }


                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentWorkOrderDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && workOrders[i].order_status_id != selectedStatus)
                    continue;

                workOrdersAfterFiltering.Add(workOrders[i]);

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;


                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOrders[i].order_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = workOrders[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOrders[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOrders[i].company_name + " - " + workOrders[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT> temp = workOrders[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT tempType1 = temp[j].productType;
                    PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    productTypeAndBrandLabel.Content += tempType1.product_name + " - " + tempBrand1.brand_name;

                    if (j != temp.Count() - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOrders[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.CANCELLED_WORK_ORDER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                else if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.CLOSED_WORK_ORDER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }

                borderIcon.Child = rfqStatusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);

                ListBox listBox = new ListBox();
                listBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                listBox.SelectionChanged += new SelectionChangedEventHandler(OnSelChangedListBox);

                ListBoxItem viewButton = new ListBoxItem();
                viewButton.Content = "View";
                viewButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                listBox.Items.Add(viewButton);

                ListBoxItem addMissionButton = new ListBoxItem();
                addMissionButton.Content = "Add Mission";
                addMissionButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                listBox.Items.Add(addMissionButton);

                ListBoxItem editButton = new ListBoxItem();
                editButton.Content = "Edit Order";
                editButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.DOCUMENT_CONTROL_TEAM_ID ||
                    (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) ||
                    loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.DISPATCH_TEAM_ID)
                    listBox.Items.Add(editButton);


                //listBox.Items.Add(addCollectionButton);


                if (workOrders[i].order_status_id != COMPANY_WORK_MACROS.CANCELLED_WORK_ORDER && workOrders[i].order_status_id != COMPANY_WORK_MACROS.CLOSED_WORK_ORDER && (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID || (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)))
                {
                    if ((loggedInUser.GetEmployeeId() == workOrders[i].offer_proposer_id || loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION) 
                         && workOrders[i].order_status_id == COMPANY_WORK_MACROS.COMMISSIONED_WORK_ORDER_ITEM)
                    {
                        ListBoxItem confirmOrderButton = new ListBoxItem();
                        confirmOrderButton.Content = "Close Order";
                        confirmOrderButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        listBox.Items.Add(confirmOrderButton);
                    }
                }


                if (workOrders[i].order_status_id != COMPANY_WORK_MACROS.CLOSED_WORK_ORDER && workOrders[i].order_status_id != COMPANY_WORK_MACROS.CANCELLED_WORK_ORDER && (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID || (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)))
                {
                    if (loggedInUser.GetEmployeeId() == workOrders[i].offer_proposer_id || loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
                    {
                        ListBoxItem cancelOrderButton = new ListBoxItem();
                        cancelOrderButton.Content = "Cancel Order";
                        cancelOrderButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        listBox.Items.Add(cancelOrderButton);
                    }
                }

                expander.Content = listBox;

                fullStackPanel.Children.Add(offerIdLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);
                fullStackPanel.Children.Add(contractTypeLabel);

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

                workOrdersStackPanel.Children.Add(grid);
            }

            return true;
        }

        private bool SetWorkOrdersGrid()
        {

            workOrdersGrid.Children.Clear();
            workOrdersGrid.RowDefinitions.Clear();
            workOrdersGrid.ColumnDefinitions.Clear();

            int counter = 0;

            Decimal ordersTotalPrice = 0;

            Label orderIdHeader = new Label();
            orderIdHeader.Content = "Order ID";
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

            Label orderContractTypeHeader = new Label();
            orderContractTypeHeader.Content = "Contract Type";
            orderContractTypeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderStatusHeader = new Label();
            orderStatusHeader.Content = "Order Status";
            orderStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProjectHeader = new Label();
            orderProjectHeader.Content = "Order Project";
            orderProjectHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProjectLocationsHeader = new Label();
            orderProjectLocationsHeader.Content = "Project Locations";
            orderProjectLocationsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderTotalPriceHeader = new Label();
            orderTotalPriceHeader.Content = "Total Price";
            orderTotalPriceHeader.Style = (Style)FindResource("tableSubHeaderItem");

            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());

            workOrdersGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(orderIdHeader, 0);
            Grid.SetColumn(orderIdHeader, 0);
            workOrdersGrid.Children.Add(orderIdHeader);

            Grid.SetRow(orderSalesHeader, 0);
            Grid.SetColumn(orderSalesHeader, 1);
            workOrdersGrid.Children.Add(orderSalesHeader);

            Grid.SetRow(orderPreSalesHeader, 0);
            Grid.SetColumn(orderPreSalesHeader, 2);
            workOrdersGrid.Children.Add(orderPreSalesHeader);

            Grid.SetRow(orderCompanyContactHeader, 0);
            Grid.SetColumn(orderCompanyContactHeader, 3);
            workOrdersGrid.Children.Add(orderCompanyContactHeader);

            Grid.SetRow(orderProductsHeader, 0);
            Grid.SetColumn(orderProductsHeader, 4);
            workOrdersGrid.Children.Add(orderProductsHeader);

            Grid.SetRow(orderContractTypeHeader, 0);
            Grid.SetColumn(orderContractTypeHeader, 5);
            workOrdersGrid.Children.Add(orderContractTypeHeader);

            Grid.SetRow(orderStatusHeader, 0);
            Grid.SetColumn(orderStatusHeader, 6);
            workOrdersGrid.Children.Add(orderStatusHeader);

            Grid.SetRow(orderProjectHeader, 0);
            Grid.SetColumn(orderProjectHeader, 7);
            workOrdersGrid.Children.Add(orderProjectHeader);

            Grid.SetRow(orderProjectLocationsHeader, 0);
            Grid.SetColumn(orderProjectLocationsHeader, 8);
            workOrdersGrid.Children.Add(orderProjectLocationsHeader);

            Grid.SetRow(orderTotalPriceHeader, 0);
            Grid.SetColumn(orderTotalPriceHeader, 9);
            workOrdersGrid.Children.Add(orderTotalPriceHeader);



            int currentRowNumber = 1;


            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;

                bool assigneeCondition;

                if (selectedPreSales == workOrders[i].offer_proposer_id || (selectedPreSales == workOrders[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.type_id == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brand_id == selectedBrand)
                        brandCondition |= true;


                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    bool containsID = workOrders[i].order_id.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsCompanyName = workOrders[i].company_name.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsContactName = workOrders[i].contact_name.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (containsID || containsCompanyName || containsContactName)
                    {

                    }
                    else
                        continue;
                }

                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentWorkOrderDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && workOrders[i].order_status_id != selectedStatus)
                    continue;

                RowDefinition currentRow = new RowDefinition();
                workOrdersGrid.RowDefinitions.Add(currentRow);

                Label orderIdLabel = new Label();
                orderIdLabel.Content = workOrders[i].order_id;
                orderIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(orderIdLabel, currentRowNumber);
                Grid.SetColumn(orderIdLabel, 0);
                workOrdersGrid.Children.Add(orderIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = workOrders[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                workOrdersGrid.Children.Add(salesLabel);

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOrders[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                workOrdersGrid.Children.Add(preSalesLabel);

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOrders[i].company_name + " - " + workOrders[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(companyAndContactLabel);
                Grid.SetRow(companyAndContactLabel, currentRowNumber);
                Grid.SetColumn(companyAndContactLabel, 3);


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


                List<PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT> temp = workOrders[i].products;

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

                workOrdersGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOrders[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 5);


                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(rfqStatusLabel);
                Grid.SetRow(rfqStatusLabel, currentRowNumber);
                Grid.SetColumn(rfqStatusLabel, 6);

                Label rfqProjectLabel = new Label();
                rfqProjectLabel.Content = workOrders[i].project_name;
                if (rfqProjectLabel.Content.ToString() == "")
                    rfqProjectLabel.Content = "N/A";
                rfqProjectLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(rfqProjectLabel);
                Grid.SetRow(rfqProjectLabel, currentRowNumber);
                Grid.SetColumn(rfqProjectLabel, 7);

                if (workOrders[i].project_locations.Count == 0)
                {
                    Label locationLabel = new Label();
                    locationLabel.Content = "N/A";
                    locationLabel.Style = (Style)FindResource("tableSubItemLabel");

                    workOrdersGrid.Children.Add(locationLabel);
                    Grid.SetRow(locationLabel, currentRowNumber);
                    Grid.SetColumn(locationLabel, 8);
                }

                else
                {
                    Grid projectLocationsGrid = new Grid();
                    projectLocationsGrid.ShowGridLines = true;
                    projectLocationsGrid.RowDefinitions.Add(new RowDefinition());
                    projectLocationsGrid.RowDefinitions.Add(new RowDefinition());
                    projectLocationsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    projectLocationsGrid.ColumnDefinitions.Add(new ColumnDefinition());


                    Label rowColumnHeaderProjectLocations = new Label();
                    rowColumnHeaderProjectLocations.Style = (Style)FindResource("tableSubHeaderItem");

                    projectLocationsGrid.Children.Add(rowColumnHeaderProjectLocations);
                    Grid.SetRow(rowColumnHeaderProjectLocations, 0);
                    Grid.SetColumn(rowColumnHeaderProjectLocations, 0);

                    Label locationHeader = new Label();
                    locationHeader.Content = "Location";
                    locationHeader.Style = (Style)FindResource("tableSubHeaderItem");

                    projectLocationsGrid.Children.Add(locationHeader);
                    Grid.SetRow(locationHeader, 0);
                    Grid.SetColumn(locationHeader, 1);

                    Label locationNumberHeader = new Label();
                    locationNumberHeader.Content = "Type";
                    locationNumberHeader.Style = (Style)FindResource("tableSubHeaderItem");

                    projectLocationsGrid.Children.Add(locationNumberHeader);
                    Grid.SetRow(locationNumberHeader, 1);
                    Grid.SetColumn(locationNumberHeader, 0);


                    for (int k = 0; k < workOrders[i].project_locations.Count; k++)
                    {
                        if (k == 0)
                        {
                            Label numberLabel = new Label();
                            numberLabel.Content = (k + 1).ToString();
                            numberLabel.Style = (Style)FindResource("tableSubItemLabel");

                            projectLocationsGrid.Children.Add(numberLabel);
                            Grid.SetRow(numberLabel, k + 1);
                            Grid.SetColumn(numberLabel, 0);

                            Label locationLabel = new Label();
                            locationLabel.Content = workOrders[i].project_locations[k].site_location.district.district_name + ", " + workOrders[i].project_locations[k].site_location.state_governorate.state_name + ", " + workOrders[i].project_locations[k].site_location.city.city_name + ", " + workOrders[i].project_locations[k].site_location.country.country_name;
                            locationLabel.Style = (Style)FindResource("tableSubItemLabel");

                            projectLocationsGrid.Children.Add(locationLabel);
                            Grid.SetRow(locationLabel, 1);
                            Grid.SetColumn(locationLabel, 1);
                        }
                        else
                        {
                            projectLocationsGrid.RowDefinitions.Add(new RowDefinition());

                            Label numberLabel = new Label();
                            numberLabel.Content = (k + 1).ToString();
                            numberLabel.Style = (Style)FindResource("tableSubItemLabel");

                            projectLocationsGrid.Children.Add(numberLabel);
                            Grid.SetRow(numberLabel, k + 1);
                            Grid.SetColumn(numberLabel, 0);

                            Label locationLabel = new Label();
                            locationLabel.Content = workOrders[i].project_locations[k].site_location.district.district_name + ", " + workOrders[i].project_locations[k].site_location.state_governorate.state_name + ", " + workOrders[i].project_locations[k].site_location.city.city_name + ", " + workOrders[i].project_locations[k].site_location.country.country_name;
                            locationLabel.Style = (Style)FindResource("tableSubItemLabel");

                            projectLocationsGrid.Children.Add(locationLabel);
                            Grid.SetRow(locationLabel, k + 1);
                            Grid.SetColumn(locationLabel, 1);
                        }
                    }

                    workOrdersGrid.Children.Add(projectLocationsGrid);
                    Grid.SetRow(projectLocationsGrid, currentRowNumber);
                    Grid.SetColumn(projectLocationsGrid, 8);

                }

                Label totalPriceLabel = new Label();
                totalPriceLabel.Content = workOrders[i].total_price;
                totalPriceLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(totalPriceLabel);
                Grid.SetRow(totalPriceLabel, currentRowNumber);
                Grid.SetColumn(totalPriceLabel, 9);

                ordersTotalPrice += workOrders[i].total_price;



                //currentRow.MouseLeftButtonDown += OnBtnClickWorkorderItem;

                currentRowNumber++;


            }

            currentRowNumber++;

            workOrdersGrid.RowDefinitions.Add(new RowDefinition());

            Label tempLabel0 = new Label();
            Label tempLabel1 = new Label();
            Label tempLabel2 = new Label();
            Label tempLabel3 = new Label();

            Grid tempGrid4 = new Grid();

            tempGrid4.RowDefinitions.Add(new RowDefinition());
            tempGrid4.RowDefinitions.Add(new RowDefinition());

            tempGrid4.ColumnDefinitions.Add(new ColumnDefinition());
            tempGrid4.ColumnDefinitions.Add(new ColumnDefinition());
            tempGrid4.ColumnDefinitions.Add(new ColumnDefinition());
            tempGrid4.ColumnDefinitions.Add(new ColumnDefinition());
            Label header0 = new Label();
            Label header1 = new Label();
            Label header2 = new Label();
            Label header3 = new Label();
            Label value0 = new Label();
            Label value1 = new Label();
            Label value2 = new Label();
            Label value3 = new Label();

            tempGrid4.Children.Add(header0);
            tempGrid4.Children.Add(header1);
            tempGrid4.Children.Add(header2);
            tempGrid4.Children.Add(header3);
            tempGrid4.Children.Add(value0);
            tempGrid4.Children.Add(value1);
            tempGrid4.Children.Add(value2);
            tempGrid4.Children.Add(value3);


            Label tempLabel5 = new Label();
            Label tempLabel6 = new Label();
            Label tempLabel7 = new Label();
            Label tempLabel8 = new Label();

            workOrdersGrid.Children.Add(tempLabel0);
            Grid.SetRow(tempLabel0, currentRowNumber);
            Grid.SetColumn(tempLabel0, 0);

            workOrdersGrid.Children.Add(tempLabel1);
            Grid.SetRow(tempLabel1, currentRowNumber);
            Grid.SetColumn(tempLabel1, 1);

            workOrdersGrid.Children.Add(tempLabel2);
            Grid.SetRow(tempLabel2, currentRowNumber);
            Grid.SetColumn(tempLabel2, 2);

            workOrdersGrid.Children.Add(tempLabel3);
            Grid.SetRow(tempLabel3, currentRowNumber);
            Grid.SetColumn(tempLabel3, 3);

            workOrdersGrid.Children.Add(tempGrid4);
            Grid.SetRow(tempGrid4, currentRowNumber);
            Grid.SetColumn(tempGrid4, 4);

            workOrdersGrid.Children.Add(tempLabel5);
            Grid.SetRow(tempLabel5, currentRowNumber);
            Grid.SetColumn(tempLabel5, 5);

            workOrdersGrid.Children.Add(tempLabel6);
            Grid.SetRow(tempLabel6, currentRowNumber);
            Grid.SetColumn(tempLabel6, 6);

            workOrdersGrid.Children.Add(tempLabel7);
            Grid.SetRow(tempLabel7, currentRowNumber);
            Grid.SetColumn(tempLabel7, 7);

            workOrdersGrid.Children.Add(tempLabel8);
            Grid.SetRow(tempLabel8, currentRowNumber);
            Grid.SetColumn(tempLabel8, 8);

            Label ordersTotalPriceLabel = new Label();
            ordersTotalPriceLabel.Content = ordersTotalPrice;
            ordersTotalPriceLabel.Style = (Style)FindResource("tableSubItemLabel");

            workOrdersGrid.Children.Add(ordersTotalPriceLabel);
            Grid.SetRow(ordersTotalPriceLabel, currentRowNumber);
            Grid.SetColumn(ordersTotalPriceLabel, 9);

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            ////DisableReviseButton();

            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;



            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;



            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].type_id;
            else
                selectedProduct = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brand_id;
            else
                selectedBrand = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = orderStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }


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

            //for (int i = 0; i < salesEmployeesList.Count; i++)
            //    if (loggedInUser.GetEmployeeId() == salesEmployeesList[i].employee_id)
            //        salesComboBox.SelectedIndex = i;
        }
        private void OnCheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.IsEnabled = true;


            preSalesComboBox.SelectedIndex = 0;

            //for (int i = 0; i < preSalesEmployeesList.Count; i++)
            //    if (loggedInUser.GetEmployeeId() == preSalesEmployeesList[i].employee_id)
            //        preSalesComboBox.SelectedIndex = i;
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

        private void OnUnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = false;
            searchTextBox.Text = null;
        }


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
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            viewAddCondition = COMPANY_WORK_MACROS.ORDER_ADD_CONDITION;

            selectedWorkOrder = new WorkOrder();

            //WorkOrderWindow workOrderWindow = new WorkOrderWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedWorkOrder, viewAddCondition, false);
            //
            //workOrderWindow.Closed += OnClosedWorkOrderWindow;
            //workOrderWindow.Show();
        }

        //private void OnBtnClickView(object sender, RoutedEventArgs e)
        //{
        //    Quotation selectedWorkOffer = new Quotation(sqlDatabase);
        //
        //    commonQueries.GetEmployeeTeam(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].sales_person_id, ref salesPersonTeam);
        //
        //    int viewAddCondition = COMPANY_WORK_MACROS.QUOTATION_VIEW_CONDITION;
        //    WorkOfferWindow viewOffer = new WorkOfferWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedWorkOffer, viewAddCondition, false);
        //    viewOffer.Show();
        //}

        //private void OnBtnClickWorkOfferItem(object sender, RoutedEventArgs e)
        //{
        //    EnableViewButton();
        //    //EnableReviseButton();
        //    previousSelectedOrderItem = currentSelectedOrderItem;
        //    currentSelectedOrderItem = (Grid)sender;
        //    BrushConverter brush = new BrushConverter();
        //
        //    if (previousSelectedOrderItem != null)
        //    {
        //        previousSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        //
        //        StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedOrderItem.Children[0];
        //        Border previousSelectedBorder = (Border)previousSelectedOrderItem.Children[1];
        //        Label previousStatusLabel = (Label)previousSelectedBorder.Child;
        //
        //        foreach (Label childLabel in previousSelectedStackPanel.Children)
        //            childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
        //
        //        if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFA500");
        //        else if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#008000");
        //        else
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FF0000");
        //
        //        previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
        //    }
        //
        //    currentSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#105A97");
        //
        //    StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedOrderItem.Children[0];
        //    Border currentSelectedBorder = (Border)currentSelectedOrderItem.Children[1];
        //    Label currentStatusLabel = (Label)currentSelectedBorder.Child;
        //
        //    foreach (Label childLabel in currentSelectedStackPanel.Children)
        //        childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
        //
        //    currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        //    currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
        //}

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClosedWorkOrderWindow(object sender, EventArgs e)
        {
            if (!GetWorkOrders())
                return;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(workOrdersGrid);
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
                else if (currentItem.Content.ToString() == "Add Mission")
                {
                    OnBtnClickAddMission();
                }
                else if (currentItem.Content.ToString() == "Edit Order")
                {
                    OnBtnClickEditOrder();
                }
                tempListBox.SelectedIndex = -1;
            }
        }
        private void OnBtnClickView()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;

            WorkOrder workOrder = new WorkOrder();

            //workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);
            //WorkOrderWindow workOrderWindow = new WorkOrderWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref workOrder, viewAddCondition, false);
            //
            //workOrderWindow.Show();
        }
        private void OnBtnClickAddMission()
        {
            //AddMissionWindow missionWindow = new AddMissionWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial, 0, workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_id);
            //missionWindow.Show();
        }
        private void OnBtnClickEditOrder()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION;

            WorkOrder workOrder = new WorkOrder();

            //workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);
            //WorkOrderWindow workOrderWindow = new WorkOrderWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref workOrder, viewAddCondition, false);
            //
            //workOrderWindow.Show();
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
    }
}
