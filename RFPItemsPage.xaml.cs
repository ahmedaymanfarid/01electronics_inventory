using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using _01electronics_windows_library;
using static _01electronics_library.PROCUREMENT_STRUCTS;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for RFPItemsPage.xaml
    /// </summary>
    public partial class RFPItemsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        List<int> oldBrandIds = new List<int>();
        EmailFormat emailFormat;
        List<string> emailTo;
        List<string> emailCC;
        private int itemNumber = 2;
        private Object obj;
        public RFPBasicInfoPage basicInfoPage;

        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> genericBrands;
        private List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> measureUnits;
        private List<BASIC_STRUCTS.STATUS_STRUCT> rfpItemsStatus;
        private List<RFP_ITEM_MAX_STRUCT> oldRFPsItemsList;
        private List<string> selectedSerials;
        private RFP rfp;

        private Object mailObject;
        private Object viewSerialsButton;
        private int viewAddCondition;
        bool uncheckAll = true;
        
        Button saveChangesButton;
        PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem;

        public RFPBasicInfoPage rfpBasicInfoPage;
        public RFPUploadFilesPage rfpUploadFilesPage;

        Stock stock;
        List<List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>> stockAvailableItemsListOfList;
        MaterialReservation reservation;
        private INVENTORY_STRUCTS.RESERVATION_RFP_MIN_STRUCT rfpInfo;
        List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT> reservedRFPSerialNumber;
        int serialCounter;

        public RFPItemsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref RFP mRFP, ref int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            viewAddCondition = mViewAddCondition;
            rfp = mRFP;

            InitializeComponent();

            saveChangesButton = new Button();
            
            mailObject = new Object();
            
            emailFormat = new EmailFormat();
            
            emailTo = new List<string>();
            emailCC = new List<string>();
            selectedSerials = new List<string>();
            genericBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            measureUnits = new List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT>();
            rfpItemsStatus = new List<BASIC_STRUCTS.STATUS_STRUCT>();
            viewSerialsButton = new Object();
            oldRFPsItemsList = new List<RFP_ITEM_MAX_STRUCT>();
            rfpItem = new RFP_ITEM_MAX_STRUCT();
            obj = new Object();
            stock = new Stock();
            stockAvailableItemsListOfList = new List<List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>>();
            reservation = new MaterialReservation();
            rfpInfo = new INVENTORY_STRUCTS.RESERVATION_RFP_MIN_STRUCT();
            reservedRFPSerialNumber = new List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>();

           
            InitializeMeasureUnits();
            InitializeRFPsItemsStatus();

            InitializeUIElements();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void InitializeUIElements()
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_ADD_CONDITION)
            {
                nextButton.IsEnabled = false;
            }
            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_ADD_CONDITION)
            {
                InitializeItemsGrid();
                
                if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                {
                    finishButton.Content = "Add PO";
                    finishButton.IsEnabled = false;
                    for (int i = 1; i < itemsGrid.Children.Count; i++)
                    {
                        Grid currentItemsGrid = (Grid)itemsGrid.Children[i];
                        Button button = (Button)currentItemsGrid.Children[1];
                        button.IsEnabled = false;
                    }
                }
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION && ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID)))
            {

                saveChangesButton = new Button();
                saveChangesButton.Style = (Style)FindResource("buttonStyle");
                saveChangesButton.Content = "Save Changes";
                saveChangesButton.Click += OnBtnClickFinish;
                saveChangesButton.IsEnabled = false;
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                buttonsGrid.Children.Add(saveChangesButton);
                Grid.SetColumn(saveChangesButton, buttonsGrid.ColumnDefinitions.Count - 1);
            }

        }
        private void SetUIElementsEdit(Grid currentGrid)
        {
            foreach (var var in currentGrid.Children)
            {
                if (var.GetType() == typeof(TextBox))
                {
                    TextBox currentTextBox = (TextBox)var;
                    currentTextBox.IsEnabled = true;
                }
                else if (var.GetType() == typeof(ComboBox))
                {
                    ComboBox currentComboBox = (ComboBox)var;
                    currentComboBox.IsEnabled = true;
                }
            }

        }
   
            private void InitializeItemsGrid()
            {
                itemsGrid.Children.Clear();
                //itemsGrid.RowDefinitions.Clear();

                itemNumber = rfp.rfpItems.Count;

                for (int j = itemsGrid.RowDefinitions.Count(); j > 1; j--)
                {
                    itemsGrid.RowDefinitions.RemoveAt(itemsGrid.RowDefinitions.Count - 1);
                }

                //itemsGrid.RowDefinitions.Add(new RowDefinition());

                itemsGrid.Children.Add(itemsHeaderGrid);
                if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                {
                    selectItemsCheckBox.Visibility = Visibility.Visible;
                }


                if (rfp.rfpItems.Count == 0 && viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                {
                    Grid tempGrid = new Grid();
                    Image addRowImage = new Image();
                    addRowImage.Source = new BitmapImage(new Uri(@"Icons\add_row_icon.jpg", UriKind.Relative));
                    addRowImage.HorizontalAlignment = HorizontalAlignment.Center;
                    addRowImage.VerticalAlignment = VerticalAlignment.Center;
                    addRowImage.Height = 40;
                    addRowImage.Width = 40;
                    addRowImage.MouseLeftButtonDown += OnClickAddRowImage;
                    addRowImage.Visibility=Visibility.Collapsed;
                    tempGrid.Children.Add(addRowImage);

                    itemsGrid.Children.Add(tempGrid);
                    itemsGrid.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(tempGrid, itemsGrid.RowDefinitions.Count - 1);
                }

                else
                {
                    for (int i = 0; i < rfp.rfpItems.Count(); i++)
                    {
                        ///////////////////////// copy items //////////////////////////////////
                        PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT oldItem = new PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT();
                        oldItem.Copy(rfp.rfpItems[i]);
                        oldRFPsItemsList.Add(oldItem);
                        ////////////////////////////////////////////////////////////////////////


                        itemsGrid.RowDefinitions.Add(new RowDefinition());

                        Grid subGrid = new Grid();
                        subGrid.ShowGridLines = true;
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(75) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });



                        Label itemNumberLabel = new Label();
                        itemNumberLabel.Content = rfp.rfpItems[i].rfp_item_number + "-";
                        itemNumberLabel.Style = (Style)FindResource("miniLabelStyle");

                        subGrid.Children.Add(itemNumberLabel);
                        Grid.SetColumn(itemNumberLabel, 0);

                        Button editButton = new Button();
                        TextBlock description = new TextBlock();
                        //description.Text = rfp.rfpItems[index].item_description;
                        description.Text = rfp.rfpItems[i].item_description;
                        description.TextWrapping = TextWrapping.Wrap;
                        description.FontSize = 16;

                        editButton.Style = (Style)FindResource("buttonStyle2");
                        editButton.Content = description;
                        editButton.Click += OnBtnClickAddDescriptionn;
                        editButton.Tag = rfp.rfpItems[i].rfp_item_number;
                        editButton.FontSize = 16;

                        TextBox descriptionTextBox = new TextBox();
                        descriptionTextBox.Style = (Style)FindResource("miniTextBoxStyle");
                        descriptionTextBox.TextWrapping = TextWrapping.Wrap;
                        descriptionTextBox.Visibility = Visibility.Collapsed;

                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                            descriptionTextBox.IsReadOnly = true;
                        else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
                        {
                            descriptionTextBox.IsReadOnly = true;
                        }

                        //descriptionTextBox.Text = rfp.rfpItems[i].item_description;
                        subGrid.Children.Add(editButton);
                        Grid.SetColumn(editButton, 1);

                        //subGrid.Children.Add(descriptionTextBox);
                        Grid.SetColumn(descriptionTextBox, 1);

                        Grid vendorsGrid = new Grid();

                        for (int k = 0; k < rfp.rfpItems[i].item_vendors.Count; k++)
                        {

                            //VENDOR_MIN_STRUCT oldBrand = new VENDOR_MIN_STRUCT();
                            //oldBrand.vendor_id = rfp.rfpItems[i].item_vendors[k].vendor_id;
                            //oldBrand.vendor_name = rfp.rfpItems[i].item_vendors[k].vendor_name;
                            //oldRFPsItemsList[i].item_vendors.Add(oldBrand);

                            vendorsGrid.RowDefinitions.Add(new RowDefinition());

                            Grid innerGrid = new Grid();
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });

                            ComboBox vendorComboBox = new ComboBox();
                            vendorComboBox.Style = (Style)FindResource("miniComboBoxStyle");

                            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                                vendorComboBox.IsEnabled = false;
                            else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID||loggedInUser.GetEmployeeTeamId()==COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID))
                            {
                                vendorComboBox.IsEnabled = false;
                            }
                            FillVendorsCombo(ref vendorComboBox);
                            //vendorComboBox.SelectedIndex = genericBrands.FindIndex(x1 => x1.vendor_id == rfp.rfpItems[i].item_vendors[k].vendor_id);
                            vendorComboBox.SelectedItem = rfp.rfpItems[i].item_vendors[k].vendor_name;

                            innerGrid.Children.Add(vendorComboBox);
                            Grid.SetColumn(vendorComboBox, 0);

                            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION && k == rfp.rfpItems[i].item_vendors.Count - 1 || viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                            {
                                // if (loggedinUser.GetEmployeeId() == rfp.GetRFPRequestor().GetEmployeeId())
                                // {
                                Image addVendorImage = new Image();
                                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                                addVendorImage.Height = 20;
                                addVendorImage.Width = 20;
                                addVendorImage.MouseLeftButtonDown += OnClickAddVendor;

                                innerGrid.Children.Add(addVendorImage);
                                Grid.SetColumn(addVendorImage, 2);
                                // }
                            }

                            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION || viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                            {
                                if (loggedInUser.GetEmployeeId() == rfp.GetRFPRequestor().GetEmployeeId())
                                {
                                    Image removeVendorImage = new Image();
                                    removeVendorImage.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
                                    removeVendorImage.Height = 20;
                                    removeVendorImage.Width = 20;
                                    removeVendorImage.MouseLeftButtonDown += OnClickRemoveVendor;

                                    innerGrid.Children.Add(removeVendorImage);
                                    Grid.SetColumn(removeVendorImage, 1);
                                }
                            }

                            vendorsGrid.Children.Add(innerGrid);
                            Grid.SetRow(innerGrid, k);
                        }
                    
                    if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                        {
                        //if (loggedinUser.GetEmployeeId() == rfp.GetRFPRequestor().GetEmployeeId())
                        //{
                        vendorsGrid.RowDefinitions.Add(new RowDefinition());

                        Grid innerGrid = new Grid();
                        innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                        innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });

                        ComboBox vendorComboBox = new ComboBox();
                        vendorComboBox.Style = (Style)FindResource("miniComboBoxStyle");

                        FillVendorsCombo(ref vendorComboBox);

                        innerGrid.Children.Add(vendorComboBox);
                        Grid.SetColumn(vendorComboBox, 0);

                        Image addVendorImage = new Image();
                        addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                        addVendorImage.Height = 20;
                        addVendorImage.Width = 20;
                        addVendorImage.MouseLeftButtonDown += OnClickAddVendor;

                        innerGrid.Children.Add(addVendorImage);
                        Grid.SetColumn(addVendorImage, 2);

                        vendorsGrid.Children.Add(innerGrid);
                        addVendorImage.Visibility = Visibility.Collapsed;
                        vendorComboBox.IsEnabled = false;
                          
                            //}
                        }

                        subGrid.Children.Add(vendorsGrid);
                        Grid.SetColumn(vendorsGrid, 2);

                        WrapPanel quantityWrapPanel = new WrapPanel();
                        quantityWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                        quantityWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                        TextBox quantityTextBox = new TextBox();
                        quantityTextBox.Style = (Style)FindResource("microTextBoxStyle");

                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                            quantityTextBox.IsReadOnly = true;
                        else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID || viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION))
                        {
                            quantityTextBox.IsReadOnly = true;
                        }

                        quantityTextBox.Text = rfp.rfpItems[i].item_quantity.ToString();

                        ComboBox unitComboBox = new ComboBox();
                        unitComboBox.Style = (Style)FindResource("filterComboBoxStyle");

                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                            unitComboBox.IsEnabled = false;
                        else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID || viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION))
                        {
                            unitComboBox.IsEnabled = false;
                        }

                        FillMeasureUnitsCombo(ref unitComboBox);
                        //unitComboBox.SelectedIndex = measureUnits.FindIndex(x1 => x1.measure_unit_id == rfp.rfpItems[i].item_measure_unit_id);
                        unitComboBox.SelectedItem = rfp.rfpItems[i].measure_unit;

                        quantityWrapPanel.Children.Add(quantityTextBox);
                        quantityWrapPanel.Children.Add(unitComboBox);

                        subGrid.Children.Add(quantityWrapPanel);
                        Grid.SetColumn(quantityWrapPanel, 3);

                        ComboBox statusComboBox = new ComboBox();
                        statusComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                        FillRFPsItemsStatusCombo(ref statusComboBox);

                        statusComboBox.SelectedItem = rfp.rfpItems[i].item_status.status_name;
                        statusComboBox.IsEnabled = false;

                        subGrid.Children.Add(statusComboBox);
                        Grid.SetColumn(statusComboBox, 7);

                        TextBox notesTextBox = new TextBox();
                        notesTextBox.Height = 70;
                        notesTextBox.Style = (Style)FindResource("miniTextBoxStyle");
                        notesTextBox.TextWrapping = TextWrapping.Wrap;
                        notesTextBox.Text = rfp.rfpItems[i].item_notes;

                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                            notesTextBox.IsReadOnly = true;
                        else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
                        {
                            notesTextBox.IsReadOnly = true;
                        }

                        subGrid.Children.Add(notesTextBox);
                        Grid.SetColumn(notesTextBox, 8);

                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                        {
                            // if (loggedinUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedinUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedinUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
                            // {
                            CheckBox currentCheckBox = new CheckBox();
                            currentCheckBox.Style = (Style)FindResource("miniCheckBoxStyle");
                            currentCheckBox.Checked += OnCheckItemCheckBox;
                            currentCheckBox.Unchecked += OnUncheckItemCheckBox;

                            subGrid.Children.Add(currentCheckBox);
                            Grid.SetColumn(currentCheckBox, 9);



                            //  }
                        }
                        int availableQantity = 0;
                        if (rfp.rfpItems[i].is_company_product)
                        {
                            if (!stock.GetAvailableQuantityItem(rfp.rfpItems[i].product_category.category_id, rfp.rfpItems[i].product_type.type_id, rfp.rfpItems[i].product_brand.brand_id, rfp.rfpItems[i].product_model.model_id, rfp.rfpItems[i].product_specs.spec_id, false, ref availableQantity))
                                return;
                        }
                        else if (rfp.rfpItems[i].product_category.category_id == 0 && rfp.rfpItems[i].product_category.category_name != null || rfp.rfpItems[i].product_category.category_id != 0)
                        {
                            if (!stock.GetAvailableQuantityItem(rfp.rfpItems[i].product_category.category_id, rfp.rfpItems[i].product_type.type_id, rfp.rfpItems[i].product_brand.brand_id, rfp.rfpItems[i].product_model.model_id, rfp.rfpItems[i].product_specs.spec_id, false, ref availableQantity))
                                return;
                        }
                       
                        TextBox availableQuantityLabel = new TextBox();
                        availableQuantityLabel.Style = (Style)FindResource("microTextBoxStyle");
                        availableQuantityLabel.Text = availableQantity.ToString();
                        availableQuantityLabel.IsEnabled = false;
                        subGrid.Children.Add(availableQuantityLabel);
                        Grid.SetColumn(availableQuantityLabel, 4);

                        TextBox reservredQuantity = new TextBox();
                        reservredQuantity.Style = (Style)FindResource("miniTextBoxStyle");
                        //reservredQuantity.TextChanged += OnTextChangedReservedQuantity;
                        reservredQuantity.IsEnabled = false;
                        subGrid.Children.Add(reservredQuantity);
                        Grid.SetColumn(reservredQuantity, 5);
                        InitializeRFPReserevedSerial(rfp.rfpItems[i].rfp_item_number);
                        Button serialButton = new Button();
                        serialButton.Tag = i;
                        TextBlock viewSerial = new TextBlock();
                        //description.Text = rfp.rfpItems[index].item_description;

                        viewSerial.TextWrapping = TextWrapping.Wrap;
                        viewSerial.FontSize = 16;
                        if (reservedRFPSerialNumber.Count != 0)
                        {
                            for (int k = 0; k < reservedRFPSerialNumber.Count; k++)
                            {
                                viewSerial.Text += reservedRFPSerialNumber[k].serial_number + '\n';
                            }
                        }
                        else
                        {
                            viewSerial.Text = "View Serials";
                        }
                        reservredQuantity.Text = reservedRFPSerialNumber.Count.ToString();


                        serialButton.Style = (Style)FindResource("buttonStyle2");
                        serialButton.Content = viewSerial;
                        serialButton.IsEnabled = false;
                        serialButton.Click += OnBtnClickViewsSerials;
                        subGrid.Children.Add(serialButton);
                        Grid.SetColumn(serialButton, 6);


                        if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                        {
                            serialButton.IsEnabled = false;

                        }

                        // if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION && i == rfp.rfpItems.Count() - 1)
                        // {
                        //
                        //     Grid tempGrid = new Grid();
                        //     Image addRowImage = new Image();
                        //     addRowImage.Source = new BitmapImage(new Uri(@"Icons\add_row_icon.jpg", UriKind.Relative));
                        //     addRowImage.HorizontalAlignment = HorizontalAlignment.Center;
                        //     addRowImage.VerticalAlignment = VerticalAlignment.Center;
                        //     addRowImage.Height = 40;
                        //     addRowImage.Width = 40;
                        //     addRowImage.MouseLeftButtonDown += OnClickAddRowImage;
                        //
                        //     tempGrid.Children.Add(addRowImage);
                        //
                        //     itemsGrid.Children.Add(tempGrid);
                        //     itemsGrid.RowDefinitions.Add(new RowDefinition());
                        //     Grid.SetRow(tempGrid, rfp.rfpItems[i].item_number + 1);
                        //
                        // }

                        itemsGrid.Children.Add(subGrid);
                        Grid.SetRow(subGrid, rfp.rfpItems[i].rfp_item_number);

                    }
                }
            }

        

        private void FillVendorsCombo(ref ComboBox currentCombo)
        {
            currentCombo.Items.Clear();

            for (int i = 0; i < genericBrands.Count; i++)
                currentCombo.Items.Add(genericBrands[i].brand_name);

        }
        private bool InitializeMeasureUnits()
        {
            if (!commonQueries.GetMeasureUnits(ref measureUnits))
                return false;

            measureUnitsCombo.Items.Clear();

            for (int i = 0; i < measureUnits.Count; i++)
                measureUnitsCombo.Items.Add(measureUnits[i].measure_unit);

            return true;
        }
        private void FillMeasureUnitsCombo(ref ComboBox currentCombo)
        {
            currentCombo.Items.Clear();

            for (int i = 0; i < measureUnits.Count; i++)
                currentCombo.Items.Add(measureUnits[i].measure_unit);

        }
        private bool InitializeRFPsItemsStatus()
        {
            if (!commonQueries.GetRFPStatuses(ref rfpItemsStatus))
                return false;

            itemStatusCombo.Items.Clear();

            for (int i = 0; i < rfpItemsStatus.Count; i++)
                itemStatusCombo.Items.Add(rfpItemsStatus[i].status_name);

            itemStatusCombo.SelectedIndex = 0;

            return true;
        }
        private void FillRFPsItemsStatusCombo(ref ComboBox currentCombo)
        {
            currentCombo.Items.Clear();

            for (int i = 0; i < rfpItemsStatus.Count; i++)
                currentCombo.Items.Add(rfpItemsStatus[i].status_name);

            currentCombo.SelectedIndex = 0;
            //currentCombo.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool GetUIValues()
        {
            //rfp.rfpItems.Clear();
            if (rfp.rfpItems.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Item #1 Description must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            for (int j = 0; j < rfp.rfpItems.Count; j++)
            {
                foreach (Grid currentGrid in itemsGrid.Children)
                {
                    if (Grid.GetRow(currentGrid) >= 1)
                    {
                        int counter = 1;
                        if (currentGrid.Children.Count != 1)
                        {

                            PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT tempItem = rfp.rfpItems[j];
                            tempItem.item_vendors = new List<PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT>();

                            Label itemNumberLabel = (Label)currentGrid.Children[0];
                            tempItem.rfp_item_number = int.Parse(itemNumberLabel.Content.ToString().Split('-')[0]);

                            Button description = (Button)currentGrid.Children[1];
                            TextBlock textBlock = description.Content as TextBlock;
                            if (textBlock.Text == "ADD")
                            {
                                System.Windows.Forms.MessageBox.Show("Item #" + tempItem.rfp_item_number + " Description must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                return false;
                            }
                            tempItem.item_description = textBlock.Text;

                            Grid vendorsGrid = (Grid)currentGrid.Children[2];

                            for (int i = 0; i < vendorsGrid.Children.Count; i++)
                            {
                                Grid innerGrid = (Grid)vendorsGrid.Children[i];
                                //if (innerGrid.Children.Count != 1)
                                //{
                                ComboBox vendor = (ComboBox)innerGrid.Children[0];
                                if (vendor.SelectedIndex != -1)
                                {
                                    PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT temp = new PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT();
                                    temp.vendor_id = genericBrands[vendor.SelectedIndex].brand_id;
                                    temp.vendor_name = genericBrands[vendor.SelectedIndex].brand_name;
                                    tempItem.item_vendors.Add(temp);
                                }
                                else
                                {

                                    //System.Windows.Forms.MessageBox.Show("Item #" + tempItem.rfp_item_number + " Brand #" + (i + 1) +
                                    //    " must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    //return false;
                                }
                                //}

                            }

                            WrapPanel quantityUnitWrapPanel = (WrapPanel)currentGrid.Children[3];
                            TextBox quantityTextBox = (TextBox)quantityUnitWrapPanel.Children[0];
                            ComboBox unitCombo = (ComboBox)quantityUnitWrapPanel.Children[1];

                            if (quantityTextBox.Text.ToString() == String.Empty)
                            {
                                System.Windows.Forms.MessageBox.Show("Item #" + tempItem.rfp_item_number +
                                    " Quantity must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                return false;
                            }

                            if (unitCombo.SelectedIndex == -1)
                            {
                                System.Windows.Forms.MessageBox.Show("Item #" + tempItem.rfp_item_number +
                                    " Measuring Unit must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                return false;
                            }

                            tempItem.item_quantity = Decimal.Parse(quantityTextBox.Text.ToString());
                            tempItem.measure_unit_id = measureUnits[unitCombo.SelectedIndex].measure_unit_id;
                            tempItem.measure_unit = measureUnits[unitCombo.SelectedIndex].measure_unit;

                            ComboBox itemStatusCombo = (ComboBox)currentGrid.Children[4];
                            tempItem.item_status.status_id = rfpItemsStatus[itemStatusCombo.SelectedIndex].status_id;
                            tempItem.item_status.status_name = rfpItemsStatus[itemStatusCombo.SelectedIndex].status_name;

                            TextBox notes = (TextBox)currentGrid.Children[5];
                            tempItem.item_notes = notes.Text;
                            tempItem.rfp_item_number = Int32.Parse(description.Tag.ToString());
                            rfp.rfpItems[j] = tempItem;
                            j++;
                            // rfp.rfpItems.Add(tempItem);
                        }
                        counter++;
                    }
                }

            }
            return true;
        }
        private void InitializeRFPReserevedSerial(int rfpItemNo)
        {
            if (!rfp.GetReservedSerialNumberRFP(rfpItemNo, ref reservedRFPSerialNumber))
                return;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickAddVendor(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;

            //int currentImageIndex = currentGrid.Children.IndexOf(currentWrapPanel);

            if (outerGrid.Children.Count == 1 && innerGrid.Children.Count == 1)
            {
                innerGrid.Children.Clear();

                ComboBox vendorComboBox = new ComboBox();
                vendorComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                FillVendorsCombo(ref vendorComboBox);

                Image removeVendorImage = new Image();
                removeVendorImage.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
                removeVendorImage.Height = 20;
                removeVendorImage.Width = 20;
                removeVendorImage.MouseLeftButtonDown += OnClickRemoveVendor;

                innerGrid.Children.Add(vendorComboBox);
                Grid.SetColumn(vendorComboBox, 0);

                innerGrid.Children.Add(removeVendorImage);
                Grid.SetColumn(removeVendorImage, 1);

                innerGrid.Children.Add(currentImage);
                Grid.SetColumn(currentImage, 2);
            }
            else
            {
                innerGrid.Children.Remove(currentImage);

                Grid addedGrid = new Grid();
                addedGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                addedGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                addedGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });

                ComboBox addedComboBox = new ComboBox();
                addedComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                FillVendorsCombo(ref addedComboBox);

                Image deleteIcon = new Image { Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative)) };
                deleteIcon.Width = 20;
                deleteIcon.Height = 20;
                deleteIcon.MouseLeftButtonDown += OnClickRemoveVendor;

                addedGrid.Children.Add(addedComboBox);
                Grid.SetColumn(addedComboBox, 0);

                addedGrid.Children.Add(deleteIcon);
                Grid.SetColumn(deleteIcon, 1);

                addedGrid.Children.Add(currentImage);
                Grid.SetColumn(currentImage, 2);

                outerGrid.RowDefinitions.Add(new RowDefinition());

                outerGrid.Children.Add(addedGrid);
                Grid.SetRow(addedGrid, outerGrid.RowDefinitions.Count - 1);
            }
        }
        private void OnClickAddRowImage(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid imageGrid = (Grid)currentImage.Parent;



            itemsGrid.Children.Remove(imageGrid);
            itemNumber = itemsGrid.Children.Count;

            Grid subGrid = new Grid();
            subGrid.ShowGridLines = true;
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(75) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            subGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label itemNumberLabel = new Label();
            itemNumberLabel.Content = itemNumber + "-";
            itemNumberLabel.Style = (Style)FindResource("miniLabelStyle");

            subGrid.Children.Add(itemNumberLabel);
            Grid.SetColumn(itemNumberLabel, 0);

            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.Text = "ADD";
            Button addDescriptionButton = new Button();
            addDescriptionButton.Style = (Style)FindResource("buttonStyle2");
            addDescriptionButton.Content = descriptionTextBlock;
            addDescriptionButton.Tag = itemNumber;
            addDescriptionButton.Click += OnBtnClickAddDescriptionn;
            TextBox descriptionTextBox = new TextBox();
            descriptionTextBox.Style = (Style)FindResource("miniTextBoxStyle");
            descriptionTextBox.TextWrapping = TextWrapping.Wrap;

            subGrid.Children.Add(addDescriptionButton);
            Grid.SetColumn(addDescriptionButton, 1);

            subGrid.Children.Add(descriptionTextBox);
            Grid.SetColumn(descriptionTextBox, 1);
            descriptionTextBox.Visibility = Visibility.Collapsed;

            Grid vendorsGrid = new Grid();

            vendorsGrid.RowDefinitions.Add(new RowDefinition());

            Grid innerGrid = new Grid();
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });

            ComboBox vendorComboBox = new ComboBox();
            vendorComboBox.Style = (Style)FindResource("miniComboBoxStyle");
            FillVendorsCombo(ref vendorComboBox);

            Image addVendorImage = new Image();
            addVendorImage.Width = 20;
            addVendorImage.Height = 20;
            addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
            addVendorImage.MouseLeftButtonDown += OnClickAddVendor;


            innerGrid.Children.Add(vendorComboBox);
            Grid.SetColumn(vendorComboBox, 0);
            innerGrid.Children.Add(addVendorImage);
            Grid.SetColumn(addVendorImage, 2);

            vendorsGrid.Children.Add(innerGrid);
            Grid.SetRow(innerGrid, 0);

            subGrid.Children.Add(vendorsGrid);
            Grid.SetColumn(vendorsGrid, 2);

            WrapPanel quantityWrapPanel = new WrapPanel();
            quantityWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            quantityWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            TextBox quantityTextBox = new TextBox();
            quantityTextBox.Style = (Style)FindResource("microTextBoxStyle");

            ComboBox unitComboBox = new ComboBox();
            unitComboBox.Style = (Style)FindResource("filterComboBoxStyle");
            FillMeasureUnitsCombo(ref unitComboBox);

            quantityWrapPanel.Children.Add(quantityTextBox);
            quantityWrapPanel.Children.Add(unitComboBox);

            subGrid.Children.Add(quantityWrapPanel);
            Grid.SetColumn(quantityWrapPanel, 3);

            ComboBox statusComboBox = new ComboBox();
            statusComboBox.Style = (Style)FindResource("miniComboBoxStyle");
            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                statusComboBox.IsEnabled = false;
            else
                statusComboBox.IsEnabled = true;
            FillRFPsItemsStatusCombo(ref statusComboBox);

            subGrid.Children.Add(statusComboBox);
            Grid.SetColumn(statusComboBox, 4);

            TextBox notesTextBox = new TextBox();
            notesTextBox.TextWrapping = TextWrapping.Wrap;
            notesTextBox.Style = (Style)FindResource("miniTextBoxStyle");

            subGrid.Children.Add(notesTextBox);
            Grid.SetColumn(notesTextBox, 5);

            Image deleteIcon = new Image { Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative)) };
            deleteIcon.Height = 40;
            deleteIcon.Width = 40;
            deleteIcon.Margin = new Thickness(10);
            deleteIcon.MouseLeftButtonDown += OnClickRemoveItem;
            subGrid.Children.Add(deleteIcon);
            Grid.SetColumn(deleteIcon, 6);

            itemsGrid.Children.Add(subGrid);
            Grid.SetRow(subGrid, itemNumber);

            itemsGrid.RowDefinitions.Add(new RowDefinition());

            itemsGrid.Children.Add(imageGrid);
            Grid.SetRow(imageGrid, itemNumber + 1);

            itemNumber++;
        }
        private void OnClickRemoveVendor(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;

            int index = outerGrid.Children.IndexOf(innerGrid);

            if (outerGrid.Children.Count == 1)
            {
                innerGrid.Children.Clear();
                Image addVendorImage = new Image();
                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                addVendorImage.Height = 20;
                addVendorImage.Width = 20;
                addVendorImage.MouseLeftButtonDown += OnClickAddVendor;

                innerGrid.Children.Add(addVendorImage);
                Grid.SetColumn(addVendorImage, 2);
            }
            else if (index == outerGrid.Children.Count - 1 && outerGrid.Children.Count != 0)
            {
                outerGrid.Children.Remove(innerGrid);

                Grid previousInnerGrid = (Grid)outerGrid.Children[index - 1];
                Image plusIcon = (Image)innerGrid.Children[2];
                innerGrid.Children.Remove(plusIcon);
                previousInnerGrid.Children.Add(plusIcon);
                Grid.SetColumn(plusIcon, 2);

                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
            else
            {
                for (int i = index; i < outerGrid.Children.Count - 1; i++)
                {
                    if (i == index)
                        Grid.SetRow(innerGrid, outerGrid.RowDefinitions.Count - 1);
                    else
                        Grid.SetRow(outerGrid.Children[i], i - 1);

                }

                outerGrid.Children.Remove(innerGrid);

                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
        }
        private void OnClickRemoveItem(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Button descriptionButton = innerGrid.Children[1] as Button;
            int itemNo = int.Parse(descriptionButton.Tag.ToString());
            int index1 = itemsGrid.Children.IndexOf(innerGrid);

            if (index1 == itemsGrid.Children.Count - 2)
            {

                for (int i = 0; i < rfp.rfpItems.Count; i++)
                {
                    if (itemNo == rfp.rfpItems[i].rfp_item_number)
                    {
                        rfp.rfpItems.RemoveAt(i);
                        break;
                    }

                }
                itemsGrid.Children.Remove(innerGrid);
                itemsGrid.RowDefinitions.RemoveAt(itemsGrid.RowDefinitions.Count - 1);

            }
            else
            {
                for (int i = index1; i < itemsGrid.Children.Count - 1; i++)
                {
                    if (i == index1)
                        Grid.SetRow(innerGrid, itemsGrid.RowDefinitions.Count - 1);
                    else
                        Grid.SetRow(itemsGrid.Children[i], i - 1);
                }
                for (int i = 0; i < rfp.rfpItems.Count; i++)
                {
                    if (itemNo == rfp.rfpItems[i].rfp_item_number)
                    {
                        rfp.rfpItems.RemoveAt(i);
                        break;
                    }

                }
                itemsGrid.Children.Remove(innerGrid);
                itemsGrid.RowDefinitions.RemoveAt(itemsGrid.RowDefinitions.Count - 1);
            }

            itemNumber--;
            FixItemNumbers();

        }
        private void FixItemNumbers()
        {
            List<Grid> tempGridList = new List<Grid>();

            tempGridList.Add(itemsHeaderGrid);

            int addRowGridIndex = 0;
            for (int j = 1; j < itemsGrid.Children.Count; j++)
            {
                Grid currentGrid = (Grid)itemsGrid.Children[j];
                if (currentGrid.Children.Count != 1)
                    tempGridList.Add(currentGrid);
                else
                    addRowGridIndex = j;
            }

            tempGridList.Add((Grid)itemsGrid.Children[addRowGridIndex]);

            itemsGrid.Children.Clear();
            itemsGrid.RowDefinitions.Clear();

            for (int k = 0; k < tempGridList.Count; k++)
            {
                if (k == 0)
                    itemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                else
                    itemsGrid.RowDefinitions.Add(new RowDefinition());
                itemsGrid.Children.Add(tempGridList[k]);
                Grid.SetRow(itemsGrid.Children[itemsGrid.Children.Count - 1], k);
            }

            for (int i = 1; i <= itemNumber; i++)
            {
                Grid currentGrid = (Grid)itemsGrid.Children[i];
                if (currentGrid.Children.Count != 1)
                {
                    Label itemNumberLabel = (Label)currentGrid.Children[0];
                    itemNumberLabel.Content = i + "-";
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedDescriptionTextBox(object sender, TextChangedEventArgs e)
        {

        }
        private void OnTextChangedVendorsTextBox(object sender, TextChangedEventArgs e)
        {

        }
        private void OnTextChangedQuantityTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox currentQuantityTextBox = (TextBox)sender;

            if (integrityChecks.CheckInvalidCharacters(currentQuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && currentQuantityTextBox.Text != "")
            {

            }
            else
            {
                if (currentQuantityTextBox.Text.Length > 1)
                {
                    currentQuantityTextBox.Text = currentQuantityTextBox.Text.Substring(0, currentQuantityTextBox.Text.Length - 1);
                    currentQuantityTextBox.Select(currentQuantityTextBox.Text.Length, 0);
                }
                else
                    currentQuantityTextBox.Text = "";
            }
        }
        private void OnSelChangedUnitComboBox(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnTextChangedNotesTextBox(object sender, TextChangedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CHECK\UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckSelectAllCheckBox(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < itemsGrid.Children.Count; i++)
            {
                Grid currentGrid = (Grid)itemsGrid.Children[i];
                try
                {
                    CheckBox currentCheckBox = (CheckBox)currentGrid.Children[currentGrid.Children.Count - 1];
                    currentCheckBox.IsChecked = true;
                }
                catch
                {

                }
            }
        }
        private void OnUnCheckSelectAllCheckBox(object sender, RoutedEventArgs e)
        {
            if (uncheckAll)
            {

                for (int i = 1; i < itemsGrid.Children.Count; i++)
                {
                    Grid currentGrid = (Grid)itemsGrid.Children[i];
                    try
                    {
                        CheckBox currentCheckBox = (CheckBox)currentGrid.Children[currentGrid.Children.Count - 1];
                        currentCheckBox.IsChecked = false;
                    }
                    catch
                    {

                    }
                }
                uncheckAll = true;
            }
        }
        private void OnCheckItemCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            Label itemNumberLabel = (Label)currentGrid.Children[0];
            currentGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDEDED"));
            rfp.SetItemSelectionStatus(int.Parse(itemNumberLabel.Content.ToString().Split('-')[0]), true);
            bool isAllSelected = false;
            for (int i = 0; i < rfp.GetRFPItemList().Count; i++)
            {
                if (rfp.GetRFPItemList()[i].itemSelected)
                {
                    isAllSelected = true;
                }
                else
                {
                    isAllSelected = false;
                    break;
                }
            }
            if (isAllSelected)
            {
                selectItemsCheckBox.IsChecked = true;
            }
            else
            {
                uncheckAll = false;
                selectItemsCheckBox.IsChecked = false;
                uncheckAll = true;
            }

            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION))
            {
                addQuotationButton.IsEnabled = true;
                finishButton.IsEnabled = true;
            }
        }
        private void OnUncheckItemCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            Label itemNumberLabel = (Label)currentGrid.Children[0];

            uncheckAll = false;
            selectItemsCheckBox.IsChecked = false;
            uncheckAll = true;

            currentGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            rfp.SetItemSelectionStatus(int.Parse(itemNumberLabel.Content.ToString().Split('-')[0]), false);

            if (rfp.GetRFPItemList().FindIndex(x1 => x1.itemSelected == true) != -1)
            {
                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID && loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION))
                {
                    addQuotationButton.IsEnabled = true;
                    finishButton.IsEnabled = true;
                }
            }
            else
            {
                addQuotationButton.IsEnabled = false;
                finishButton.IsEnabled = false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// NAVIGATION HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            rfpBasicInfoPage.rfpItemsPage = this;
            rfpBasicInfoPage.rfpUploadFilesPage = rfpUploadFilesPage;
            NavigationService.Navigate(rfpBasicInfoPage);
        }
        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            rfpBasicInfoPage.rfpItemsPage = this;
            rfpBasicInfoPage.rfpUploadFilesPage = rfpUploadFilesPage;
            NavigationService.Navigate(rfpBasicInfoPage);
        }
        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
            {
                rfpUploadFilesPage.rfpBasicInfoPage = rfpBasicInfoPage;
                rfpUploadFilesPage.rfpItemsPage = this;
                NavigationService.Navigate(rfpUploadFilesPage);
            }
        }
        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            finishButton.Content = "Loading..";
            finishButton.IsEnabled = false;
            if (rfp.GetRFPIssueDate().ToString().Contains("1/1/0001"))
            {
                System.Windows.Forms.MessageBox.Show("RFP Issue Date must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                finishButton.Content = "Finish";
                finishButton.IsEnabled = true;
                return;
            }
            else if (rfp.GetRFPDeadlineDate().ToString().Contains("01/01/0001"))
            {
                System.Windows.Forms.MessageBox.Show("RFP Deadline Date must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                finishButton.Content = "Finish";
                finishButton.IsEnabled = true;
                return;
            }
            else if (rfp.GetRFPRequestor().GetEmployeeId() == 0)
            {
                System.Windows.Forms.MessageBox.Show("RFP Requestor must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                finishButton.Content = "Finish";
                finishButton.IsEnabled = true;
                return;
            }
            else if (rfp.GetRFPAssignedOfficer().GetEmployeeId() == 0)
            {
                System.Windows.Forms.MessageBox.Show("RFP Assignee must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                finishButton.Content = "Finish";
                finishButton.IsEnabled = true;
                return;
            }
            else if (rfp.GetWorkForm() == 0 && (rfp.GetWorkFormName() == string.Empty || rfp.GetWorkFormName() == null))
            {
                System.Windows.Forms.MessageBox.Show("None of the work form checkboxees are selected ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                finishButton.Content = "Finish";
                finishButton.IsEnabled = true;
                return;
            }
            else if (!GetUIValues())
                return;
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.RFP_ADD_CONDITION)
                {
                    if (!rfp.IssueNewRfp())
                        return;
                    else
                    {

                        emailTo.Clear();
                        emailCC.Clear();
                        if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID, ref emailTo))
                            return;
                        if (!commonQueries.GetInventoryTeamBusinessEmails(ref emailTo))
                            return;
                        if (!commonQueries.GetSpecificDepartmentBusinessEmailsForManagers(rfp.GetRFPRequestor().GetEmployeeDepartmentId(), rfp.GetRFPRequestor().GetEmployeePositionId(), ref emailCC))
                            return;
                        if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID, ref emailCC))
                            return;
                        if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID, ref emailCC))
                            return;
                        if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.BOARD_OF_DIRECTORS, ref emailCC))
                            return;
                        emailCC.Add(rfp.GetRFPRequestor().GetEmployeeBusinessEmail());
                        string subject = "";
                        string header = "NEW RFP ADDED !";
                        SubjectMaker(ref subject, viewAddCondition);
                        object mailObject = rfp as object;
                        emailFormat.SendEmail(ref emailTo, ref emailCC, subject, header, ref mailObject, BASIC_MACROS.IS_RFP);

                    }
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.RFP_REVISE_CONDITION)
                {
                    if (!rfp.ReviseRfp())
                        return;
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                {
                    rfpInfo.rfpSerial = rfp.GetRFPSerial();
                    rfpInfo.rfpVersion = rfp.GetRFPVersion();
                    rfpInfo.rfpRequestorTeam = rfp.GetRFPRequestorTeamId();
                   

                    reservation.SetRFPInfo(rfpInfo);
                    reservation.SetReservationStatusId(COMPANY_WORK_MACROS.PENDING_RESERVATION);
                    reservation.SetReservationDate(DateTime.Now);
                    reservation.SetReservedBy(loggedInUser.GetEmployeeId());
                    if (!rfp.EditRfp())
                        return;

                    if (!rfp.EditRFPItems(ref oldRFPsItemsList))
                        return;

                    for (int i = 0; i < stockAvailableItemsListOfList.Count; i++)
                    {
                        if (!reservation.IssueMultipleReservations(stockAvailableItemsListOfList[i]))
                            return;
                    }

                    emailTo.Clear();
                    emailCC.Clear();
                    if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID, ref emailTo))
                        return;
                    if (!commonQueries.GetInventoryTeamBusinessEmails(ref emailTo))
                        return;
                    if (!commonQueries.GetSpecificDepartmentBusinessEmailsForManagers(rfp.GetRFPRequestor().GetEmployeeDepartmentId(), rfp.GetRFPRequestor().GetEmployeePositionId(), ref emailCC))
                        return;
                    if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID, ref emailCC))
                        return;
                    if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID, ref emailCC))
                        return;
                    if (!commonQueries.GetSpecificDepartmentBusinessEmails(COMPANY_ORGANISATION_MACROS.BOARD_OF_DIRECTORS, ref emailCC))
                        return;
                    emailCC.Add(rfp.GetRFPRequestor().GetEmployeeBusinessEmail());
                    string subject = "";
                    string header = "RFP IS UPDATED !";
                    SubjectMaker(ref subject, viewAddCondition);
                    object mailObject = rfp as object;
                    emailFormat.SendEmail(ref emailTo, ref emailCC, subject, header, ref mailObject, BASIC_MACROS.IS_RFP);
                }

                if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                {
                    PurchaseOrder purchaseOrder = new PurchaseOrder();
                    ////if (!rfp.UpdateRfpItemMapping())
                    ////    return;
                    if (!purchaseOrder.InitializeRFP(rfp.GetRFPRequestor().GetEmployeeTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                        return;
                    if (!purchaseOrder.InitializeProcurementOfficer(loggedInUser.GetEmployeeId()))
                        return;

                    List<PROCUREMENT_STRUCTS.PO_ITEM_STRUCT> tempPOItems = new List<PROCUREMENT_STRUCTS.PO_ITEM_STRUCT>();

                    int itemNumber = 1;

                    for (int i = 1; i < itemsGrid.Children.Count; i++)
                    {
                        Grid currentItemsGrid = (Grid)itemsGrid.Children[i];
                        CheckBox currentItemCheckBox = (CheckBox)currentItemsGrid.Children[currentItemsGrid.Children.Count - 1];
                        Label currentItemNumberLabel = (Label)currentItemsGrid.Children[0];

                        if (currentItemCheckBox.IsChecked == true)
                        {
                            PROCUREMENT_STRUCTS.PO_ITEM_STRUCT tempPOItem = new PROCUREMENT_STRUCTS.PO_ITEM_STRUCT();
                            tempPOItem.rfp_item.rfp_item_number = rfp.rfpItems[i - 1].rfp_item_number;
                            tempPOItem.rfp_item.item_quantity = rfp.rfpItems[i - 1].item_quantity;
                            tempPOItem.rfp_item.measure_unit = rfp.rfpItems[i - 1].measure_unit;
                            tempPOItem.rfp_item.measure_unit_id = rfp.rfpItems[i - 1].measure_unit_id;
                            tempPOItem.po_item_no = itemNumber;
                            itemNumber += 1;

                            tempPOItems.Add(tempPOItem);
                        }
                    }

                    purchaseOrder.SetPOItems(tempPOItems);

                    int viewAddConditionPO = COMPANY_WORK_MACROS.PO_ADD_CONDITION;

                  //  POWindow pOWindow = new POWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref purchaseOrder, viewAddConditionPO, false);
                 //   pOWindow.Show();

                }

                if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                {
                    //viewAddCondition = COMPANY_WORK_MACROS.RFP_EDIT_CONDITION;
                    //RFPUploadFilesPage rFPUploadFilesPage = new RFPUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref viewAddCondition);
                    //NavigationService.Navigate(rFPUploadFilesPage);
                    viewAddCondition = COMPANY_WORK_MACROS.PO_VIEW_CONDITION;

                   // AddRFPWindow rfpWindow = new AddRFPWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref viewAddCondition, ref rfp, true);
                   // rfpWindow.Show();

                    NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                    currentWindow.Close();
                }
            }
        }

        private void OnBtnClickAddQuotation(object sender, RoutedEventArgs e)
        {
            IncomingQuotations incomingQuotation = new IncomingQuotations();

            if (!incomingQuotation.InitializeRFP(rfp.GetRFPRequestor().GetEmployeeTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                return;
            if (!incomingQuotation.InitializeEmployeeInfo(loggedInUser.GetEmployeeId()))
                return;

            List<PROCUREMENT_STRUCTS.INCOMING_QUOTATIONS_ITEM_MIN_STRUCT> tempQuotationItems = new List<PROCUREMENT_STRUCTS.INCOMING_QUOTATIONS_ITEM_MIN_STRUCT>();

            int itemNumber = 1;

            for (int i = 1; i < itemsGrid.Children.Count; i++)
            {
                Grid currentItemsGrid = (Grid)itemsGrid.Children[i];
                CheckBox currentItemCheckBox = (CheckBox)currentItemsGrid.Children[currentItemsGrid.Children.Count - 1];
                Label currentItemNumberLabel = (Label)currentItemsGrid.Children[0];

                if (currentItemCheckBox.IsChecked == true)
                {
                    PROCUREMENT_STRUCTS.INCOMING_QUOTATIONS_ITEM_MIN_STRUCT tempQuotationItem = new PROCUREMENT_STRUCTS.INCOMING_QUOTATIONS_ITEM_MIN_STRUCT();
                    tempQuotationItem.rfp_item_number = rfp.rfpItems[i - 1].rfp_item_number;
                    tempQuotationItem.item_quantity = rfp.rfpItems[i - 1].item_quantity;
                    tempQuotationItem.measure_unit = rfp.rfpItems[i - 1].measure_unit;
                    tempQuotationItem.measure_unit_id = rfp.rfpItems[i - 1].measure_unit_id;
                    tempQuotationItem.rfp_item_number = itemNumber;
                    itemNumber += 1;

                    tempQuotationItems.Add(tempQuotationItem);
                }
            }

            incomingQuotation.SetIncomingQuotationItems(tempQuotationItems);

            int viewAddConditionQuotation = COMPANY_WORK_MACROS.INCOMIN_QUOTATION_ADD_CONDITION;

           // IncomingQuotationsWindow quotationWindow = new IncomingQuotationsWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref incomingQuotation, ref viewAddConditionQuotation);
           // quotationWindow.Show();
        }
        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
            {
                rfpUploadFilesPage.rfpBasicInfoPage = rfpBasicInfoPage;
                rfpUploadFilesPage.rfpItemsPage = this;
                NavigationService.Navigate(rfpUploadFilesPage);
            }
        }
        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            GetUIValues();

            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
            {
                for (int i = 0; i < rfp.rfpItems.Count; i++)
                {
                    //if (rfp.rfpItems[i].item_availablilty_status.status_id == COMPANY_WORK_MACROS.RFP_CANCELLED_AVAILABILITY_STATUS)
                    //{
                    //    if (!rfp.UpdateRFPItemStatus(COMPANY_WORK_MACROS.RFP_CANCELLED, rfp.rfpItems[i].rfp_item_number))
                    //        return;
                    //}
                }

                //if (!rfp.CheckRFPStatus())
                //    return;
            }

            NavigationWindow parentWindow = (NavigationWindow)this.Parent;
            parentWindow.Close();
        }

        //private void OnBtnClickAddDescription(object sender, RoutedEventArgs e)
        //{
        //    Button addButtonn = (Button)sender;
        //    int itemNo = Int32.Parse(addButtonn.Tag.ToString());

        //    bool edit = false;
        //    mailObject = null;
        //    mailObject = (Object)addButtonn;

        //    for (int i = 0; i < rfp.rfpItems.Count; i++)
        //    {
        //        if (rfp.rfpItems[i].rfp_item_number == itemNo)
        //        {
        //            edit = true;
        //            break;
        //        }
        //    }
        //    if (edit)
        //    {

        //        rfpItem = rfp.rfpItems[itemNo-1];
        //        rfpItem.rfp_item_number = itemNo;
        //        RFPItemDescriptionWindow rfpItemWindow = new RFPItemDescriptionWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref rfpItem,viewAddCondition);
        //        rfpItemWindow.Show();
        //        rfpItemWindow.Closed += OnCloseRFPItemWindow;
        //    }
        //    else
        //    {
        //        rfpItem = new PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT();
        //        rfpItem.rfp_item_number = itemNo;
        //        RFPItemDescriptionWindow rfpItemWindow = new RFPItemDescriptionWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref rfpItem , viewAddCondition);
        //        rfpItemWindow.Show();
        //        rfpItemWindow.Closed += OnCloseRFPItemWindow;
        //    }
        //}

        private void OnBtnClickAddDescriptionn(object sender, RoutedEventArgs e)
        {
            Button addButtonn = (Button)sender;
            int itemNo = Int32.Parse(addButtonn.Tag.ToString());
            Grid parent = (Grid)addButtonn.Parent;
            Button viewSerialButton = parent.Children[8] as Button;
            viewSerialButton.IsEnabled = true;
        
            mailObject = new object();
            mailObject = (Object)addButtonn;
            for (int i = 0; i < rfp.rfpItems.Count; i++)
            {
                if (rfp.rfpItems[i].rfp_item_number == itemNo)
                {
                    itemNo = i+1;
                    addButtonn.Tag = itemNo;
                    viewAddCondition = COMPANY_WORK_MACROS.RFP_EDIT_CONDITION;
                    break;
                }
            }
            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
            {

                PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = rfp.rfpItems[itemNo - 1];
                rfpItem.rfp_item_number = itemNo;
                RFPItemDescriptionWindow rfpItemWindow = new RFPItemDescriptionWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref rfpItem, viewAddCondition);
                rfpItemWindow.Show();
                rfpItemWindow.Closed += OnCloseRFPItemWindow;
            }
            else
            {
                PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = new PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT();
                rfpItem.rfp_item_number = itemNo;
                RFPItemDescriptionWindow rfpItemWindow = new RFPItemDescriptionWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref rfpItem, viewAddCondition);
                rfpItemWindow.Show();
                rfpItemWindow.Closed += OnCloseRFPItemWindow;
              
            }
        }
        private void OnBtnClickViewsSerials(object sender, RoutedEventArgs e)
        {
            Button viewSerialButton = (Button)sender;
            TextBlock serialsTextBlock = viewSerialButton.Content as TextBlock;
            viewSerialsButton = viewSerialButton as Object;
            selectedSerials.Clear();
            bool edit = false;
            if (serialsTextBlock.Text != "View Serials")
            {
                String[] serials = serialsTextBlock.Text.Split('\n');
                for (int i = 0; i < serials.Length; i++)
                    selectedSerials.Add(serials[i]);
                edit = true;
            }
            PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = rfp.rfpItems[Int32.Parse(viewSerialButton.Tag.ToString())];
            RFPItemSerialsWindow rFPItemSerials = new RFPItemSerialsWindow(ref rfp, ref stock, ref stockAvailableItemsListOfList, ref rfpItem, ref selectedSerials, ref serialCounter, ref loggedInUser, ref edit);
            rFPItemSerials.Closed += OnCloseRFPItemSerialsWindow;
            rFPItemSerials.Show();

        }

        private void OnCloseRFPItemSerialsWindow(object sender, EventArgs e)
        {
            Button addButton = (Button)mailObject;

            Button viewSerialButton = viewSerialsButton as Button;
            Grid viewSerialsButtonParent = viewSerialButton.Parent as Grid;
            TextBlock serialsTextBlock = viewSerialButton.Content as TextBlock;
            if (selectedSerials.Count != 0)
                serialsTextBlock.Text = string.Empty;
            else
                serialsTextBlock.Text = "View Serials";
            for (int i = 0; i < selectedSerials.Count; i++)
            {
                serialsTextBlock.Text += selectedSerials[i] + '\n';

            }
            TextBox reservedQuantityTextBox = viewSerialsButtonParent.Children[7] as TextBox;
            reservedQuantityTextBox.Text = selectedSerials.Count.ToString();

            TextBox availableQuantityTextBox = viewSerialsButtonParent.Children[6] as TextBox;
            availableQuantityTextBox.Text =(int.Parse(availableQuantityTextBox.Text)- selectedSerials.Count).ToString();
        }
        private void OnCloseRFPItemWindow(object sender, EventArgs e)
        {
            if (viewAddCondition==COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
                saveChangesButton.IsEnabled = true;

            Button addButton = (Button)mailObject;
            Grid addButtonParent = addButton.Parent as Grid;
            TextBox availableQuantityTextBox = addButtonParent.Children[6] as TextBox;
            TextBlock description = addButton.Content as TextBlock;
            int itemNo = Int32.Parse(addButton.Tag.ToString());
            if (oldRFPsItemsList.Count < rfp.rfpItems.Count || rfp.rfpItems.Count == 0)
            {
                description.Text = "ADD";
                description.TextWrapping = TextWrapping.Wrap;
                addButton.Content = description;
            }
            else
            {

                description.Text = rfp.rfpItems[itemNo - 1].item_description;
                description.TextWrapping = TextWrapping.Wrap;
                addButton.Content = description;
                int availbleQuanity = 0;
                if (rfp.rfpItems[itemNo - 1].is_company_product)
                {
                    if (!stock.GetAvailableQuantityItem(rfp.rfpItems[itemNo - 1].product_category.category_id, rfp.rfpItems[itemNo - 1].product_type.type_id, rfp.rfpItems[itemNo - 1].product_brand.brand_id, rfp.rfpItems[itemNo - 1].product_model.model_id, rfp.rfpItems[itemNo - 1].product_specs.spec_id, false, ref availbleQuanity))
                        return;
                }
                else
                {
                    if (!stock.GetAvailableQuantityItem(rfp.rfpItems[itemNo - 1].product_category.category_id, rfp.rfpItems[itemNo - 1].product_type.type_id, rfp.rfpItems[itemNo - 1].product_type.type_id, rfp.rfpItems[itemNo - 1].product_type.type_id, 0, true, ref availbleQuanity))
                        return;

                }
                availableQuantityTextBox.Text = availbleQuanity.ToString();
            }
        }
        private void SubjectMaker(ref string subject, int state)
        {
            if (state == COMPANY_WORK_MACROS.RFP_ADD_CONDITION)
            {
                if (rfp.GetRFPStatusID() == 1)
                    subject = "[" + "NEW" + "]" + " " + rfp.GetRFPId();
                else
                    subject = "[" + rfp.GetRFPStatus() + "]" + " " + rfp.GetRFPId();
                if (rfp.GetOrderSerial() != 0)
                    subject += "-WO-" + rfp.GetOrderSerial();
                else if (rfp.GetProjectSerial() != 0)
                    subject += "-PROJ-" + rfp.GetProjectName();
                else if (rfp.GetContractSerial() != 0)
                    subject += "-CONTRACT-" + rfp.GetContractSerial();
                else if (rfp.GetWorkForm() != 0)
                    subject += "-STOCK-" + rfp.GetWorkFormName();


            }
            else
            {
                subject = "[" + "UPDATED" + "]" + " " + rfp.GetRFPId();
                if (rfp.GetOrderSerial() != 0)
                    subject += "-WO-" + rfp.GetOrderSerial();
                else if (rfp.GetProjectSerial() != 0)
                    subject += "-PROJ-" + rfp.GetProjectName();
                else if (rfp.GetContractSerial() != 0)
                    subject += "-CONTRACT-" + rfp.GetContractSerial();
                else if (rfp.GetWorkForm() != 0)
                    subject += "-STOCK-" + rfp.GetWorkFormName();


            }
        }
      

        private void OnTextChangedReservedQuantity(object sender, TextChangedEventArgs e)
        {

        }
    }

}



