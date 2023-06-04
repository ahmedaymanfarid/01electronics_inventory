using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using static _01electronics_library.PROCUREMENT_STRUCTS;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Controls.ComboBox;
using Page = System.Windows.Controls.Page;
using TextBox = System.Windows.Controls.TextBox;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddReleasePermitItemPage.xaml
    /// </summary>
    public partial class AddReleasePermitItemPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        MaintenanceContract maintenanceContract;

        int itemsCounter=1;
        int numberOfSelectedItems = 0;

        AddReleasePermitWindow parentWindow;

        List<string> serials;

        private MaterialEntryPermit materialEntry;
        private AddReleasePermitPage addReleasePermitPage;
        private ReleasePermitUploadFilesPage ReleasePermitUploadFilesPage;
        private List<INVENTORY_STRUCTS.ENTRY_PERMIT_MIN_STRUCT> entryPermits;


        private List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> genericCategories;
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> genericProducts;
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> genericBrands;
        private List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> genericModels;

        private List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> companyCategories;
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> companyProducts;
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> companyBrands;
        private List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> companyModels;

        private List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT> workOrders;

        private List<BASIC_STRUCTS.ADDRESS_STRUCT> workOrdersLocations;

        private List<BASIC_STRUCTS.ADDRESS_STRUCT> maintenanceContractLocations;

        private List<BASIC_STRUCTS.ADDRESS_STRUCT> companyProjectLocations;

        private List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT> specs;


        public AddReleasePermitItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow mReleasePermitWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            materialEntry = new MaterialEntryPermit();

            genericCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            genericProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();

            genericBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            genericModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            companyCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            companyProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            companyBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            companyModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            workOrders = new List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT>();

            workOrdersLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();

            maintenanceContractLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();
            companyProjectLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();
            
            specs = new List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT>();
            genericCategories= new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            serials = new List<string>();
            entryPermits= new List<INVENTORY_STRUCTS.ENTRY_PERMIT_MIN_STRUCT>();
            maintenanceContract = new MaintenanceContract();
            if (!commonQueries.GetEntryPermits(ref entryPermits))
                return;
            
            parentWindow = mReleasePermitWindow;

            InitializeStock();

            commonQueries.GetWorkOrders(ref workOrders);

            LocationsWrapPanel.Margin = new Thickness(0,15,0,0);


            if (parentWindow.isView == true) {
                finishButton.IsEnabled = false;
                addRecieval.Visibility = Visibility.Visible;
                addReEntry.Visibility = Visibility.Visible;



            }
        }

        public void InitializeStock() {

            Home.Children.Clear();
            Home.RowDefinitions.Clear();

            WrapPanel choicePanel = new WrapPanel();

            Label choice = new Label();

            choice.Style = (Style)FindResource("tableItemLabel");


            choice.Content = "Choose type";
            

            choice.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox choiceComboBox = new ComboBox();


            choiceComboBox.Style = (Style)FindResource("comboBoxStyle");

            choiceComboBox.Items.Add("Generic");
            choiceComboBox.Items.Add("Company");
            choiceComboBox.SelectionChanged += OnChoiceComboBoxSelectionChanged;

            Home.RowDefinitions.Add(new RowDefinition());
            choicePanel.Children.Add(choice);
            choicePanel.Children.Add(choiceComboBox);
            Home.Children.Add(choicePanel);

            Grid.SetRow(choicePanel, Home.RowDefinitions.Count-1);




            WrapPanel GenericCategoryPanel = new WrapPanel();

            Label genericCategoryLabel = new Label();

            genericCategoryLabel.Content = "Generic Category";

            genericCategoryLabel.Style = (Style)FindResource("tableItemLabel");

            genericCategoryLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox genericCategoryComboBox = new ComboBox();

            genericCategoryComboBox.IsEnabled = false;
              
            genericCategories.Clear();
            commonQueries.GetGenericProductCategories(ref genericCategories);

            genericCategories.ForEach(a => genericCategoryComboBox.Items.Add(a.category_name));
            genericCategoryComboBox.Style = (Style)FindResource("comboBoxStyle");

            genericCategoryComboBox.SelectionChanged += OnGenericCategoryComboBoxSelectionChanged;


            GenericCategoryPanel.Children.Add(genericCategoryLabel);
            GenericCategoryPanel.Children.Add(genericCategoryComboBox);
            Home.Children.Add(GenericCategoryPanel);
            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(GenericCategoryPanel, Home.RowDefinitions.Count - 1);





            WrapPanel GenericProductPanel = new WrapPanel();

            Label genericProductLabel = new Label();

            genericProductLabel.Content = "Generic Product";

            genericProductLabel.Style = (Style)FindResource("tableItemLabel");

            genericProductLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox genericProductComboBox = new ComboBox();

            genericProductComboBox.IsEnabled = false;

            genericProductComboBox.Style = (Style)FindResource("comboBoxStyle");

            genericProductComboBox.SelectionChanged += OnGenericProductComboBoxSelectionChanged;


            GenericProductPanel.Children.Add(genericProductLabel);
            GenericProductPanel.Children.Add(genericProductComboBox);
            Home.Children.Add(GenericProductPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(GenericProductPanel, Home.RowDefinitions.Count - 1);






            WrapPanel GenericBrandPanel = new WrapPanel();

            Label genericBrandLabel = new Label();

            genericBrandLabel.Content = "Generic Brand";

            genericBrandLabel.Style = (Style)FindResource("tableItemLabel");

            genericBrandLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox genericBrandComboBox = new ComboBox();

            genericBrandComboBox.IsEnabled = false;

            genericBrandComboBox.Style = (Style)FindResource("comboBoxStyle");

            genericBrandComboBox.SelectionChanged += OnGenericBrandComboBoxSelectionChanged;


            GenericBrandPanel.Children.Add(genericBrandLabel);
            GenericBrandPanel.Children.Add(genericBrandComboBox);
            Home.Children.Add(GenericBrandPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(GenericBrandPanel, Home.RowDefinitions.Count - 1);




            WrapPanel GenericModelPanel = new WrapPanel();

            Label genericModelLabel = new Label();

            genericModelLabel.Content = "Generic Model";

            genericModelLabel.Style = (Style)FindResource("tableItemLabel");

            genericModelLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox genericModelComboBox = new ComboBox();

            genericModelComboBox.IsEnabled = false;

            genericModelComboBox.Style = (Style)FindResource("comboBoxStyle");

            genericModelComboBox.SelectionChanged += OnGenericModelComboBoxSelectionChanged;


            GenericModelPanel.Children.Add(genericModelLabel);
            GenericModelPanel.Children.Add(genericModelComboBox);
            Home.Children.Add(GenericModelPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(GenericModelPanel, Home.RowDefinitions.Count - 1);


            /////////////////////////////////////////////////////
            ///



            WrapPanel companyCategoryPanel = new WrapPanel();

            Label companyCategoryLabel = new Label();

            companyCategoryLabel.Content = "Company Category";

            companyCategoryLabel.Style = (Style)FindResource("tableItemLabel");

            companyCategoryLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox companyCategoryComboBox = new ComboBox();

            companyCategoryComboBox.IsEnabled = false;

            companyCategories.Clear();
            commonQueries.GetProductCategories(ref companyCategories);

            companyCategories.ForEach(a => companyCategoryComboBox.Items.Add(a.category_name));
            companyCategoryComboBox.Style = (Style)FindResource("comboBoxStyle");

            companyCategoryComboBox.SelectionChanged += OnCompanyCategoryComboBoxSelectionChanged;


            companyCategoryPanel.Children.Add(companyCategoryLabel);
            companyCategoryPanel.Children.Add(companyCategoryComboBox);
            Home.Children.Add(companyCategoryPanel);
            Home.RowDefinitions.Add(new RowDefinition());


            Grid.SetRow(companyCategoryPanel, Home.RowDefinitions.Count - 1);





            WrapPanel companyProductPanel = new WrapPanel();

            Label companyProductLabel = new Label();

            companyProductLabel.Content = "Company Product";

            companyProductLabel.Style = (Style)FindResource("tableItemLabel");

            companyProductLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox companyProductComboBox = new ComboBox();

            companyProductComboBox.IsEnabled = false;

            companyProductComboBox.Style = (Style)FindResource("comboBoxStyle");

            companyProductComboBox.SelectionChanged += OnCompanyProductComboBoxSelectionChanged;


            companyProductPanel.Children.Add(companyProductLabel);
            companyProductPanel.Children.Add(companyProductComboBox);
            Home.Children.Add(companyProductPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(companyProductPanel, Home.RowDefinitions.Count - 1);






            WrapPanel companyBrandPanel = new WrapPanel();

            Label companyBrandLabel = new Label();

            companyBrandLabel.Content = "Company Brand";

            companyBrandLabel.Style = (Style)FindResource("tableItemLabel");

            companyBrandLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox companyBrandComboBox = new ComboBox();

            companyBrandComboBox.IsEnabled = false;

            companyBrandComboBox.Style = (Style)FindResource("comboBoxStyle");

            companyBrandComboBox.SelectionChanged += OnCompanyBrandComboBoxSelectionChanged;


            companyBrandPanel.Children.Add(companyBrandLabel);
            companyBrandPanel.Children.Add(companyBrandComboBox);
            Home.Children.Add(companyBrandPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(companyBrandPanel, Home.RowDefinitions.Count - 1);


            WrapPanel companyModelPanel = new WrapPanel();

            Label companyModelLabel = new Label();

            companyModelLabel.Content = "Company Model";

            companyModelLabel.Style = (Style)FindResource("tableItemLabel");


            companyModelLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox companyModelComboBox = new ComboBox();

            companyModelComboBox.IsEnabled = false;

            companyModelComboBox.Style = (Style)FindResource("comboBoxStyle");

            companyModelComboBox.SelectionChanged += OnCompanyModelComboBoxSelectionChanged;


            companyModelPanel.Children.Add(companyModelLabel);
            companyModelPanel.Children.Add(companyModelComboBox);
            Home.Children.Add(companyModelPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(companyModelPanel, Home.RowDefinitions.Count - 1);





            WrapPanel companySpecsPanel = new WrapPanel();

            Label companySpecsLabel = new Label();

            companySpecsLabel.Content = "Company Specs";

            companySpecsLabel.Style = (Style)FindResource("tableItemLabel");


            companySpecsLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            ComboBox companySpecsComboBox = new ComboBox();

            companySpecsComboBox.IsEnabled = false;

            companySpecsComboBox.Style = (Style)FindResource("comboBoxStyle");

            companySpecsComboBox.SelectionChanged += CompanySpecsComboBox_SelectionChanged;


            companySpecsPanel.Children.Add(companySpecsLabel);
            companySpecsPanel.Children.Add(companySpecsComboBox);
            Home.Children.Add(companySpecsPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(companySpecsPanel, Home.RowDefinitions.Count - 1);





            WrapPanel SelectedItemsPanel = new WrapPanel();

            Label selectedItemsLabel = new Label();

            selectedItemsLabel.Content = "Number of selected Items";

            selectedItemsLabel.Style = (Style)FindResource("tableItemLabel");

            selectedItemsLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


            Label NumberOfItemsSelected = new Label();
            NumberOfItemsSelected.Content = "0";

            NumberOfItemsSelected.Style = (Style)FindResource("tableItemLabel");

            NumberOfItemsSelected.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            SelectedItemsPanel.Children.Add(selectedItemsLabel);
            SelectedItemsPanel.Children.Add(NumberOfItemsSelected);
            Home.Children.Add(SelectedItemsPanel);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(SelectedItemsPanel, Home.RowDefinitions.Count - 1);




            Grid items = new Grid();

            Home.Children.Add(items);

            Home.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(items, Home.RowDefinitions.Count - 1);


        }

        private void CompanySpecsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FilterItems() {


            Grid items = Home.Children[Home.Children.Count - 1] as Grid;
            items.Background = Brushes.White;

            items.Children.Clear();
            items.RowDefinitions.Clear();

          

            Grid Itemsheader = new Grid();

            Itemsheader.ShowGridLines = true;
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());


            Label entryPermitSerialId = new Label();

            entryPermitSerialId.Style = (Style)FindResource("tableHeaderItem");

            entryPermitSerialId.Content = "SERIAL ID";

            Grid.SetColumn(entryPermitSerialId, 0);

            Itemsheader.Children.Add(entryPermitSerialId);



            Label itemNameLabel = new Label();

            itemNameLabel.Style = (Style)FindResource("tableHeaderItem");

            itemNameLabel.Content = "ITEM NAME";

            Grid.SetColumn(itemNameLabel, 1);

            Itemsheader.Children.Add(itemNameLabel);


            Label serialNumber = new Label();

            serialNumber.Style = (Style)FindResource("tableHeaderItem");

            serialNumber.Content = "SERIAL NUMBER";

            Grid.SetColumn(serialNumber, 2);

            Itemsheader.Children.Add(serialNumber);





            Label quantity = new Label();

            quantity.Style = (Style)FindResource("tableHeaderItem");

            quantity.Content = "Quantity";


            Grid.SetColumn(quantity, 3);

            Itemsheader.Children.Add(quantity);




            items.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(Itemsheader, items.RowDefinitions.Count - 1);
            items.Children.Add(Itemsheader);


            ScrollViewer scroll = new ScrollViewer();
            scroll.CanContentScroll = true;
            scroll.Height = 600;
            Grid itemsBody = new Grid();

            itemsBody.ShowGridLines = true;

            scroll.Content = itemsBody;

            items.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(scroll, items.RowDefinitions.Count - 1);
            items.Children.Add(scroll);



            WrapPanel choicePanel = Home.Children[0] as WrapPanel;

            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;


            WrapPanel genericCategoryPanel=Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox=genericCategoryPanel.Children[1] as ComboBox;


            WrapPanel genericProductPanel = Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;



            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;



            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



            WrapPanel companyCategoryPanel = Home.Children[5] as WrapPanel;

            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;


            WrapPanel companyProductPanel = Home.Children[6] as WrapPanel;

            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;



            WrapPanel companyBrandPanel = Home.Children[7] as WrapPanel;

            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;



            WrapPanel companyModelPanel = Home.Children[8] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            WrapPanel companySpecsPanel = Home.Children[9] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;


            if (choiceComboBox.SelectedIndex == 0) {

                int previousSerial = 0;

                itemsCounter=1;

                for (int i = 0; i < entryPermits.Count; i++)
                {
                    if (previousSerial == entryPermits[i].entry_permit_serial)
                        continue;

                    previousSerial = entryPermits[i].entry_permit_serial;


                    materialEntry.SetEntryPermitSerialid(entryPermits[i].entry_permit_serial);
                    materialEntry.InitializeMaterialEntryPermit();



                    for (int j = 0; j < materialEntry.GetItems().Count; j++) {

                        
                        if (genericCategoryComboBox.SelectedIndex != -1)
                        {

                            if (genericCategories[genericCategoryComboBox.SelectedIndex].category_id != materialEntry.GetItems()[j].product_category.category_id)
                                continue;
                        }


                        if (genericProductComboBox.SelectedIndex != -1)
                        {

                            if (genericProducts[genericProductComboBox.SelectedIndex].type_id != materialEntry.GetItems()[j].product_type.type_id)
                                continue;
                        }


                        if (genericBrandComboBox.SelectedIndex != -1)
                        {

                            if (genericBrands[genericBrandComboBox.SelectedIndex].brand_id != materialEntry.GetItems()[j].product_brand.brand_id)
                                continue;
                        }


                        if (genericModelComboBox.SelectedIndex != -1)
                        {
                            if(genericModelComboBox.SelectedIndex!=-1)
                            if (genericModels[genericModelComboBox.SelectedIndex].model_id != materialEntry.GetItems()[j].product_model.model_id)
                                continue;
                        }


                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(60)});


                        Grid itemm = new Grid();

                        itemm.ShowGridLines = true;

                        itemm.Tag = entryPermits[i].entry_permit_serial.ToString()+" "+materialEntry.GetItems()[j].entry_permit_item_serial+" "+itemsCounter;

                        itemsCounter++;

                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());




                        Label entryPermitSerialLabel = new Label();


                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();

                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));



                        Grid.SetColumn(entryPermitSerialLabel, 0);

                        itemm.Children.Add(entryPermitSerialLabel);


                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;



                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;

                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;

                        ItemName.VerticalAlignment = VerticalAlignment.Top;


                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");

                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


                        ItemName.Text = $"{materialEntry.GetItems()[j].product_category.category_name + "-" + materialEntry.GetItems()[j].product_type.product_name + "-" + materialEntry.GetItems()[j].product_brand.brand_name + "-" + materialEntry.GetItems()[j].product_model.model_name}";


                        Grid.SetColumn(ItemName, 1);

                        itemm.Children.Add(ItemName);


                        if (materialEntry.GetItems()[j].product_serial_number != "") {

                           
                            CheckBox chooseItemSerialCheckBox = new CheckBox();

                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;

                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                            chooseItemSerialCheckBox.Checked += ChooseItemSerialCheckBoxChecked;

                            chooseItemSerialCheckBox.Unchecked += ChooseItemSerialCheckBoxUnchecked;



                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");


                            Grid.SetColumn(chooseItemSerialCheckBox, 2);

                            itemm.Children.Add(chooseItemSerialCheckBox);



                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "") {



                            Grid quantityGrid = new Grid();

                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                            TextBox quantityAvailableTextBox = new TextBox();

                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity- materialEntry.GetItems()[j].released_quantity;


                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;


                            TextBox quantityTextBox = new TextBox();

                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;


                            CheckBox quantityCheckBox = new CheckBox();

                            quantityCheckBox.Width = 30;

                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");


                            quantityCheckBox.Checked += QuantityCheckBoxChecked;

                            quantityCheckBox.Unchecked += QuantityCheckBoxUnchecked;

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;

                            Grid.SetColumn(quantityCheckBox, 0);

                            Grid.SetColumn(quantityAvailableTextBox, 1);

                            Grid.SetColumn(quantityTextBox, 2);


                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);






                            Grid.SetColumn(quantityGrid, 3);

                            itemm.Children.Add(quantityGrid);




                        }



                        Grid.SetRow(itemm, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemm);

                    }

                 

                }


            }

            else if (choiceComboBox.SelectedIndex == 1) {
                int previousSerial = 0;

                itemsCounter = 1;

                for (int i=0;i<entryPermits.Count;i++)
                {
                    if (previousSerial == entryPermits[i].entry_permit_serial)
                        continue;

                    previousSerial = entryPermits[i].entry_permit_serial;


                    materialEntry.SetEntryPermitSerialid(entryPermits[i].entry_permit_serial);
                    materialEntry.InitializeMaterialEntryPermit();




                    for (int j = 0; j < materialEntry.GetItems().Count; j++)
                    {


                        if (companyCategoryComboBox.SelectedIndex != -1) {


                            if (companyCategories[companyCategoryComboBox.SelectedIndex].category_id != materialEntry.GetItems()[j].product_category.category_id)
                                continue;

                        }

                        if (companyProductComboBox.SelectedIndex != -1)
                        {

                            if (companyProducts[companyProductComboBox.SelectedIndex].type_id != materialEntry.GetItems()[j].product_type.type_id)
                                continue;
                        }


                        if (companyBrandComboBox.SelectedIndex != -1)
                        {

                            if (companyBrands[companyBrandComboBox.SelectedIndex].brand_id != materialEntry.GetItems()[j].product_brand.brand_id)
                                continue;
                        }


                        if (companyModelComboBox.SelectedIndex != -1)
                        {

                            if (companyModels[companyModelComboBox.SelectedIndex].model_id != materialEntry.GetItems()[j].product_model.model_id)
                                continue;
                        }


                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60)});


                        Grid itemm = new Grid();

                        itemm.ShowGridLines = true;

                        itemm.Tag = entryPermits[i].entry_permit_serial.ToString()+ " " +materialEntry.GetItems()[j].entry_permit_item_serial+" "+ itemsCounter;


                        itemsCounter++;

                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());



                        Label entryPermitSerialLabel = new Label();


                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();


                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));



                        Grid.SetColumn(entryPermitSerialLabel, 0);

                        itemm.Children.Add(entryPermitSerialLabel);


                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;


                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;
                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;


                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");

                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                        ItemName.Text = $"{materialEntry.GetItems()[j].product_type.product_name + "-" + materialEntry.GetItems()[j].product_brand.brand_name + "-" + materialEntry.GetItems()[j].product_model.model_name}";


                        Grid.SetColumn(ItemName, 1);

                        itemm.Children.Add(ItemName);



                        if (materialEntry.GetItems()[j].product_serial_number != "")
                        {


                            CheckBox chooseItemSerialCheckBox = new CheckBox();

                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;



                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");

                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                            chooseItemSerialCheckBox.Checked += ChooseItemSerialCheckBoxChecked;

                            chooseItemSerialCheckBox.Unchecked += ChooseItemSerialCheckBoxUnchecked;



                            Grid.SetColumn(chooseItemSerialCheckBox, 2);

                            itemm.Children.Add(chooseItemSerialCheckBox);



                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "")
                        {

                            Grid quantityGrid = new Grid();
                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;

                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                            TextBox quantityAvailableTextBox = new TextBox();

                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity- materialEntry.GetItems()[j].released_quantity;


                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;


                            TextBox quantityTextBox = new TextBox();

                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;


                            CheckBox quantityCheckBox = new CheckBox();

                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");

                            quantityCheckBox.Width = 30;

                            quantityCheckBox.Checked += QuantityCheckBoxChecked;

                            quantityCheckBox.Unchecked += QuantityCheckBoxUnchecked;

                                


                            Grid.SetColumn(quantityCheckBox, 0);

                            Grid.SetColumn(quantityAvailableTextBox, 1);

                            Grid.SetColumn(quantityTextBox, 2);


                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);



                            Grid.SetColumn(quantityGrid, 3);

                            itemm.Children.Add(quantityGrid);



                        }



                        Grid.SetRow(itemm, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemm);



                    }



                }

            }



            LocationsWrapPanel.Children.Clear();
        }


        /// ////////////////////////////////////////////       
        //ON CHECKS FUNCTIONS
        /// ////////////////////////////////////////////       
        private void QuantityCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems--;


            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox = sender as CheckBox;

           Grid quantityGrid= chooseItemCheckBox.Parent as Grid;

           TextBox quantityTextBox= quantityGrid.Children[2] as TextBox;

            quantityTextBox.Text = "";

            Grid card = chooseItemCheckBox.Tag as Grid;

            WrapPanel locationsWrapPanel = card.Parent as WrapPanel;
            locationsWrapPanel.Children.Remove(card);


            WrapPanel checkBoxesPanel=  card.Children[1] as WrapPanel;

            CheckBox rfpCheckBox=  checkBoxesPanel.Children[0] as CheckBox;

            CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;

           Grid orderOrRfp= card.Children[2] as Grid;

            if (orderCheckBox.IsChecked == true)
            {

                ComboBox orderItems = orderOrRfp.Children[0] as ComboBox;

                if (orderItems.SelectedIndex != -1)
                {

                    if (quantityTextBox.Text != "")
                        addReleasePermitPage.serialProducts[orderItems.SelectedIndex] -= int.Parse(quantityTextBox.Text);
                }
            }


            else
            {

                ComboBox rfpItems = orderOrRfp.Children[0] as ComboBox;

                if (rfpItems.SelectedIndex != -1)
                {

                    if (quantityTextBox.Text != "")
                        addReleasePermitPage.serialProducts[rfpItems.SelectedIndex] -= int.Parse(quantityTextBox.Text);
                }

            }


        }

        private void QuantityCheckBoxChecked(object sender, RoutedEventArgs e)
        {

            numberOfSelectedItems++;

            CheckBox quantityCheckBox = sender as CheckBox;


            Grid quantityGrid = quantityCheckBox.Parent as Grid;

            Grid item= quantityGrid.Parent as Grid;



            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();

            TextBlock itemName = item.Children[1] as TextBlock;


            Grid card = new Grid() { Margin = new Thickness(15, 30, 15, 30) };
            card.Height = 500;

            card.Background = Brushes.White;


            card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());


            Grid header = new Grid() { };
            header.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            TextBlock itemHeader = new TextBlock();

            header.Children.Add(itemHeader);

            itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
            itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            itemHeader.FontSize = 16;

            itemHeader.Text = itemName.Text + " ," + quantityCheckBox.Content;

            itemHeader.Foreground = Brushes.White;

            itemHeader.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(header, 0);


            quantityCheckBox.Tag = card;



            WrapPanel checkBoxesPanel = new WrapPanel();

            CheckBox rfpCheckBox = new CheckBox();

            rfpCheckBox.Style = (Style)FindResource("checkBoxStyle");

            rfpCheckBox.Content = "RFP";

            rfpCheckBox.Checked += RfpCheckBoxChecked;


            rfpCheckBox.HorizontalAlignment = HorizontalAlignment.Left;


            rfpCheckBox.Tag = item;



            CheckBox orderCheckBox = new CheckBox();

            orderCheckBox.Style = (Style)FindResource("checkBoxStyle");

            orderCheckBox.Content = "WorkOrder";
            orderCheckBox.Checked += OrderCheckBoxChecked;

            orderCheckBox.HorizontalAlignment = HorizontalAlignment.Right;

            orderCheckBox.Tag = item;



            checkBoxesPanel.Children.Add(rfpCheckBox);

            checkBoxesPanel.Children.Add(orderCheckBox);


            Grid.SetRow(checkBoxesPanel, 1);
            checkBoxesPanel.Visibility = Visibility.Collapsed;


            Grid rfpOrOrder = new Grid() { Margin = new Thickness(0, -40, 0, 0) };


            Grid.SetRow(rfpOrOrder, 2);


            card.Children.Add(header);
            card.Children.Add(checkBoxesPanel);
            card.Children.Add(rfpOrOrder);

            LocationsWrapPanel.Children.Add(card);


            if (addReleasePermitPage.rfpChecked.IsChecked == true)
            {

                rfpCheckBox.IsChecked = true;
            }

            else if (addReleasePermitPage.orderChecked.IsChecked == true)
            {

                orderCheckBox.IsChecked = true;

            }


        }

        private void ChooseItemSerialCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

            numberOfSelectedItems--;


            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox=sender as CheckBox;

            Grid card= chooseItemCheckBox.Tag as Grid;


            Grid rfpOrOrder= card.Children[2] as Grid;

            ComboBox items=  rfpOrOrder.Children[0] as ComboBox;

            if(items.SelectedIndex!=-1)
            addReleasePermitPage.serialProducts[items.SelectedIndex]--;

           WrapPanel locationsWrapPanel= card.Parent as WrapPanel;
            locationsWrapPanel.Children.Remove(card);
        }

        private void ChooseItemSerialCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems++;

             CheckBox itemSerialCheckBox =sender as CheckBox;


             Grid item= itemSerialCheckBox.Parent as Grid;



             WrapPanel selectedItemsPanel=Home.Children[Home.Children.Count - 2] as WrapPanel;

             Label selectedItemsLabel=selectedItemsPanel.Children[1] as Label;

             selectedItemsLabel.Content = numberOfSelectedItems.ToString();

             TextBlock itemName=  item.Children[1] as TextBlock;


            Grid card = new Grid() { Margin=new Thickness(15,30,15,30)};
            card.Height = 500;

            card.Background = Brushes.White;


            card.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(60)});
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());


            Grid header = new Grid() {};
            header.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            TextBlock itemHeader = new TextBlock();

            header.Children.Add(itemHeader);

            itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
            itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            itemHeader.FontSize = 16;

            itemHeader.Text = itemName.Text+" ,"+ itemSerialCheckBox.Content;

            itemHeader.Foreground = Brushes.White;

            itemHeader.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(header, 0);

          
            itemSerialCheckBox.Tag=card;



            WrapPanel checkBoxesPanel = new WrapPanel();

            CheckBox rfpCheckBox = new CheckBox();

            rfpCheckBox.Style = (Style)FindResource("checkBoxStyle");

            rfpCheckBox.Content = "RFP";

            rfpCheckBox.Checked += RfpCheckBoxChecked;


            rfpCheckBox.HorizontalAlignment = HorizontalAlignment.Left;



            CheckBox orderCheckBox = new CheckBox();

            orderCheckBox.Style = (Style)FindResource("checkBoxStyle");

            orderCheckBox.Content = "WorkOrder";
            orderCheckBox.Checked += OrderCheckBoxChecked;

            orderCheckBox.HorizontalAlignment = HorizontalAlignment.Right;



            checkBoxesPanel.Children.Add(rfpCheckBox);

            checkBoxesPanel.Children.Add(orderCheckBox);


            checkBoxesPanel.Visibility = Visibility.Collapsed;


            Grid.SetRow(checkBoxesPanel, 1);


            Grid rfpOrOrder = new Grid() { Margin=new Thickness(0,-40,0,0)};


            Grid.SetRow(rfpOrOrder, 2);


            card.Children.Add(header);
            card.Children.Add(checkBoxesPanel);
            card.Children.Add(rfpOrOrder);

            LocationsWrapPanel.Children.Add(card);


            if (addReleasePermitPage.rfpChecked.IsChecked == true) {

                rfpCheckBox.IsChecked = true;
            }

            else if (addReleasePermitPage.orderChecked.IsChecked == true) {

                orderCheckBox.IsChecked = true;
            
            }

        }

        private void RfpCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox rfpCheckBox = sender as CheckBox;


            WrapPanel checkPanel = rfpCheckBox.Parent as WrapPanel;


            CheckBox orderCheckBox=  checkPanel.Children[1] as CheckBox;

            orderCheckBox.IsChecked = false;

            Grid card = checkPanel.Parent as Grid;


            Grid order = card.Children[2] as Grid;

            order.Children.Clear();
            order.RowDefinitions.Clear();


            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());




            ComboBox rfpsItemsComboBox = new ComboBox();

            rfpsItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
            rfpsItemsComboBox.IsEnabled = true;

            rfpsItemsComboBox.SelectionChanged += RfpsItemsComboBoxSelectionChanged;



            ComboBox locations = new ComboBox();
            locations.Style = (Style)FindResource("comboBoxStyle");
            locations.IsEnabled = true;
            locations.Visibility = Visibility.Visible;





            TextBox projectName = new TextBox();
            projectName.Style = (Style)FindResource("filterTextBoxStyle");
            projectName.IsReadOnly = true;
            projectName.Visibility = Visibility.Visible;

            projectName.Width = 200;




            TextBox identityTextBox = new TextBox();
            identityTextBox.Style = (Style)FindResource("filterTextBoxStyle");
            identityTextBox.IsReadOnly = true;
            identityTextBox.Visibility = Visibility.Visible;



            Grid textBoxesGrid = new Grid();

            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());


            Grid.SetColumn(identityTextBox, 0);
            Grid.SetColumn(projectName, 1);

            textBoxesGrid.Children.Add(identityTextBox);
            textBoxesGrid.Children.Add(projectName);



            Grid.SetRow(rfpsItemsComboBox, 0);

            Grid.SetRow(locations, 1);

            Grid.SetRow(textBoxesGrid, 2);




            order.Children.Add(rfpsItemsComboBox);

            order.Children.Add(locations);

            order.Children.Add(textBoxesGrid);


            ComboBox rfpSerialsComboBox= addReleasePermitPage.rfpPanel.Children[1] as ComboBox;

            if (addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
            {

                WorkOrder workOrder = new WorkOrder();
                workOrder.InitializeWorkOrderInfo(addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                workOrdersLocations.Clear();
                workOrder.GetProjectLocations(ref workOrdersLocations);


                workOrdersLocations.ForEach(a => locations.Items.Add(a.district.district_name + " ," + a.city.city_name + " ," + a.state_governorate.state_name + " ," + a.country.country_name));

                identityTextBox.Text = workOrder.GetOrderID();

                projectName.Text = workOrder.GetprojectName();

            }

            else if (addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
            {
                MaintenanceContract maintenanceContract = new MaintenanceContract();
                maintenanceContract.InitializeMaintenanceContractInfo(addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), addReleasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                maintenanceContractLocations.ForEach(a => locations.Items.Add(a.district.district_name + " ," + a.city.city_name + " ," + a.state_governorate.state_name + " ," + a.country.country_name));

                identityTextBox.Text = maintenanceContract.GetMaintContractID();

                projectName.Text = maintenanceContract.GetMaintContractProjectName();


            }
            else if (addReleasePermitPage.rfp.GetProjectSerial() != 0)
            {

                commonQueries.GetCompanyProjectLocations(addReleasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                companyProjectLocations.ForEach(a => locations.Items.Add(a.district.district_name + " ," + a.city.city_name + " ," + a.state_governorate.state_name + " ," + a.country.country_name));

                identityTextBox.Text = "PROJECT";

                projectName.Text = addReleasePermitPage.rfp.GetProjectName();



            }


            else
            {

                identityTextBox.Text = "PROJECT";

                projectName.Text = "01ELECTRONICS";
            }



            rfpsItemsComboBox.Items.Clear();

            for (int i = 0; i < addReleasePermitPage.rfpItems.Count; i++)
            {
                //String itemName = rfpItemsDescription[i].item_description;
                String itemName = String.Empty;
                if (addReleasePermitPage.rfpItems[i].product_model.model_name != "" &&addReleasePermitPage.rfpItems[i].product_model.model_name != null)
                {
                    itemName = addReleasePermitPage.rfpItems[i].product_category.category_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_type.product_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_brand.brand_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_model.model_name;
                }
                else if (addReleasePermitPage.rfpItems[i].product_model.model_name != "" && addReleasePermitPage.rfpItems[i].product_model.model_name != null)
                {
                    itemName = addReleasePermitPage.rfpItems[i].product_category.category_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_type.product_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_brand.brand_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_model.model_name;
                }
                if (itemName != string.Empty)
                    rfpsItemsComboBox.Items.Add(itemName);
            }


        }

        private void RfpsItemsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox rfpItemsComboBox=sender as ComboBox;



            if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.model_name != "")
            {


                if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == true)
                {
                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("RfpItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }

                    else {

                        addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;


                    }



                }

                else {

                    Grid rfpCard = rfpItemsComboBox.Parent as Grid;

                    Grid card = rfpCard.Parent as Grid;

                    WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                    CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                    Grid item = rfpCheckBox.Tag as Grid;

                    Grid quantityGrid = item.Children[2] as Grid;

                    if (quantityGrid != null)
                    {

                        TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                        if (availableQuantityTextBox.Text != "")
                            addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                    }

                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("RfpItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }


                }

            }

            else
            {

                if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == true)
                {
                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("RfpItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                        return;

                    }
                    else
                    {

                        addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;


                    }



                }


                else
                {

                    Grid rfpCard = rfpItemsComboBox.Parent as Grid;

                    Grid card = rfpCard.Parent as Grid;

                    WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                    CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                    Grid item = rfpCheckBox.Tag as Grid;

                    Grid quantityGrid = item.Children[2] as Grid;

                    if (quantityGrid != null)
                    {

                        TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                        if (availableQuantityTextBox.Text != "")
                            addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                    }

                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("RfpItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }


                }



            }

        }

        private void OrderCheckBoxChecked(object sender, RoutedEventArgs e)
        {

            CheckBox orderCheckBox = sender as CheckBox;


            WrapPanel checkPanel = orderCheckBox.Parent as WrapPanel;

            Grid card = checkPanel.Parent as Grid;


            CheckBox rfpCheckBox = checkPanel.Children[0] as CheckBox;

            rfpCheckBox.IsChecked = false;


            Grid order = card.Children[2] as Grid;

            order.Children.Clear();
            order.RowDefinitions.Clear();


            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());


            ComboBox orderItemsComboBox = new ComboBox();
            orderItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
            orderItemsComboBox.IsEnabled = true;

            orderItemsComboBox.SelectionChanged += OnOrderItemsComboBoxSelectionChanged;


            ComboBox orderLocationsComboBox = new ComboBox();
            orderLocationsComboBox.Style = (Style)FindResource("comboBoxStyle");
            orderLocationsComboBox.IsEnabled = true;

         
            Grid.SetRow(orderItemsComboBox, 0);

            Grid.SetRow(orderLocationsComboBox, 1);



            order.Children.Add(orderItemsComboBox);

            order.Children.Add(orderLocationsComboBox);



            workOrdersLocations.Clear();
            addReleasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);


            workOrdersLocations.ForEach(a => orderLocationsComboBox.Items.Add(a.district.district_name + " ," + a.city.city_name + " ," + a.state_governorate.state_name + " ," + a.country.country_name));


            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = addReleasePermitPage.workOrder.GetOrderProductsList();

            orderItemsComboBox.Items.Clear();

            for (int i = 0; i < workOrderProducts.Length; i++)
            {

                orderItemsComboBox.Items.Add(workOrderProducts[i].product_category.category_name + " ," + workOrderProducts[i].productType.product_name + " ," + workOrderProducts[i].productBrand.brand_name + " ," + workOrderProducts[i].productModel.model_name);
            }

        }

        private void OnOrderItemsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox orderItemsComboBox= sender as ComboBox;

            if (addReleasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].has_serial_number == true)
            {
                addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex]++;


                if (addReleasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                {
                     System.Windows.Forms.MessageBox.Show("OrderItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }

            else {


                Grid orderCard = orderItemsComboBox.Parent as Grid;

                Grid card = orderCard.Parent as Grid;

                WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;

                Grid item = orderCheckBox.Tag as Grid;

                Grid quantityGrid = item.Children[2] as Grid;

                if (quantityGrid != null)
                {

                    TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                    if (availableQuantityTextBox.Text != "")
                        addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                }

                if (addReleasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                {
                    System.Windows.Forms.MessageBox.Show("OrderItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    return;

                }

               

            }


        }


        /// ////////////////////////////////////////////       
        //ON SELECTION CHANGED FUNCTIONS
        /// ////////////////////////////////////////////       
        private void OnOrdersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox ordersComboBox= sender as ComboBox;

            if (ordersComboBox.SelectedIndex == -1)
                return;


            WorkOrder workOrder = new WorkOrder();


            Grid order = ordersComboBox.Parent as Grid;

            ComboBox orderItemsComboBox = order.Children[1] as ComboBox;
            orderItemsComboBox.IsEnabled = true;


            ComboBox orderLocationComboBox = order.Children[2] as ComboBox;

            orderLocationComboBox.Items.Clear();

            workOrder.InitializeWorkOrderInfo(workOrders[ordersComboBox.SelectedIndex].order_serial);

            workOrdersLocations.Clear();
            workOrder.GetProjectLocations(ref workOrdersLocations);


            workOrdersLocations.ForEach(a => orderLocationComboBox.Items.Add(a.district.district_name + " ," + a.city.city_name + " ," + a.state_governorate.state_name + " ," + a.country.country_name));

            orderLocationComboBox.IsEnabled = true;

            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[]workOrderProducts=workOrder.GetOrderProductsList();

            orderItemsComboBox.Items.Clear();

            for (int i = 0; i < workOrderProducts.Length; i++) {

                orderItemsComboBox.Items.Add(workOrderProducts[i].product_category.category_name + " ," + workOrderProducts[i].productType.product_name + " ," + workOrderProducts[i].productBrand.brand_name + " ," + workOrderProducts[i].productModel.model_name);
            }

        }


        private void OnCompanyModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyModelComboBox= sender as ComboBox;


            WrapPanel companyModelPanel=companyModelComboBox.Parent as WrapPanel;
            Grid home= companyModelPanel.Parent as Grid;
            WrapPanel companyBrandPanel=home.Children[7] as WrapPanel;

            ComboBox companyBrandComboBox= companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyProductPanel = home.Children[6] as WrapPanel;

            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;

            WrapPanel companyCategoryPanel = home.Children[5] as WrapPanel;

            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;



            WrapPanel companySpecsPanel = home.Children[9] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;

            if(companyModelComboBox.SelectedIndex!=-1)
            commonQueries.GetModelSpecsNames(companyCategories[companyCategoryComboBox.SelectedIndex].category_id, companyProducts[companyProductComboBox.SelectedIndex].type_id, companyBrands[companyBrandComboBox.SelectedIndex].brand_id, companyModels[companyModelComboBox.SelectedIndex].model_id, ref specs);

            companySpecsComboBox.Items.Clear();
            specs.ForEach(a => companySpecsComboBox.Items.Add(a.spec_name));
            FilterItems();

        }

        private void OnCompanyBrandComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyBrandComboBox = sender as ComboBox;

            WrapPanel companyBrandPanel = companyBrandComboBox.Parent as WrapPanel;

            Grid home = companyBrandPanel.Parent as Grid;

            WrapPanel companyproductPanel = home.Children[6] as WrapPanel;

            ComboBox companyProductComboBox = companyproductPanel.Children[1] as ComboBox;



            WrapPanel companyModelPanel = home.Children[8] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            companyModelComboBox.Items.Clear();

            if (companyBrandComboBox.SelectedIndex == -1)
                return;

            companyModels.Clear();
            commonQueries.GetCompanyModels(companyProducts[companyProductComboBox.SelectedIndex], companyBrands[companyBrandComboBox.SelectedIndex], ref companyModels);


            companyModels.ForEach(a => companyModelComboBox.Items.Add(a.model_name));

            FilterItems();
        }

        private void OnCompanyProductComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox companyProductComboBox=sender as ComboBox;

            WrapPanel companyProductPanel = companyProductComboBox.Parent as WrapPanel;

            Grid home=companyProductPanel.Parent as Grid;

            WrapPanel companyCategoryPanel=home.Children[5] as WrapPanel;

            ComboBox companyCategoryComboBox=companyCategoryPanel.Children[1] as ComboBox;


            WrapPanel companyBrandPanel = home.Children[7] as WrapPanel;

            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyModelPanel = home.Children[8] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            companyBrandComboBox.Items.Clear();
            companyModelComboBox.Items.Clear();


            if (companyProductComboBox.SelectedIndex == -1)
                return;

            companyBrands.Clear();
            commonQueries.GetProductBrands(companyProducts[companyProductComboBox.SelectedIndex].type_id, ref companyBrands);

            companyBrands.ForEach(a => companyBrandComboBox.Items.Add(a.brand_name));

            FilterItems();

        }

        private void OnCompanyCategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                ComboBox companyCategoryComboBox=sender as ComboBox;

                WrapPanel companyCategoryPanel=companyCategoryComboBox.Parent as WrapPanel;

                Grid home= companyCategoryPanel.Parent as Grid;

                WrapPanel companyProductPanel= home.Children[6] as WrapPanel;
                ComboBox companyProductComboBox=  companyProductPanel.Children[1] as ComboBox;


                WrapPanel companyBrandPanel = home.Children[7] as WrapPanel;
                ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;



                WrapPanel companyModelPanel = home.Children[8] as WrapPanel;
                ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            companyProductComboBox.Items.Clear();
            companyBrandComboBox.Items.Clear();
            companyModelComboBox.Items.Clear();

            if (companyCategoryComboBox.SelectedIndex == -1)
                return;

            companyProducts.Clear();

            commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[companyCategoryComboBox.SelectedIndex].category_id);

            companyProductComboBox.Items.Clear();

            companyProducts.ForEach(a => companyProductComboBox.Items.Add(a.product_name));

            FilterItems();

        }

        private void OnGenericModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            FilterItems();
        }

        private void OnGenericBrandComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox genericBrandCombo = sender as ComboBox;
            WrapPanel genericBrandpanel = genericBrandCombo.Parent as WrapPanel;

            Grid Home = genericBrandpanel.Parent as Grid;

            WrapPanel generiModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = generiModelPanel.Children[1] as ComboBox;


            WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;



            WrapPanel genericProductPanel = Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;



            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;

            genericModelComboBox.Items.Clear();

            genericModels.Clear();

            if (genericBrandComboBox.SelectedIndex == -1)
                return;

            commonQueries.GetGenericBrandModels(genericProducts[genericProductComboBox.SelectedIndex].type_id, genericBrands[genericBrandComboBox.SelectedIndex].brand_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericModels);


            genericModels.ForEach(a => genericModelComboBox.Items.Add(a.model_name));


            FilterItems();
        }

        private void OnGenericProductComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox genericProductCombo = sender as ComboBox;
            WrapPanel genericProductpanel = genericProductCombo.Parent as WrapPanel;

            Grid Home = genericProductpanel.Parent as Grid;

            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;



            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



            WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;


            genericModelComboBox.Items.Clear();
            genericBrandComboBox.Items.Clear();


            if (genericProductCombo.SelectedIndex == -1)
                return;

                genericBrands.Clear();


            commonQueries.GetGenericProductBrands(genericProducts[genericProductCombo.SelectedIndex].type_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericBrands);


            genericBrands.ForEach(a => genericBrandComboBox.Items.Add(a.brand_name));




            FilterItems();


        }

        private void OnGenericCategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox genericCategoryCombo=sender as ComboBox;
            WrapPanel genericCategorypanel= genericCategoryCombo.Parent as WrapPanel;

            Grid Home=genericCategorypanel.Parent as Grid;

            WrapPanel genericProductPanel=Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox=genericProductPanel.Children[1] as ComboBox;


            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;

            genericProducts.Clear();

            genericProductComboBox.Items.Clear();
            genericBrandComboBox.Items.Clear();
            genericModelComboBox.Items.Clear();

            if (genericCategoryCombo.SelectedIndex == -1)
                return;

            commonQueries.GetGenericProducts(ref genericProducts, genericCategories[genericCategoryCombo.SelectedIndex].category_id);

      

            genericProducts.ForEach(a => genericProductComboBox.Items.Add(a.product_name));





            FilterItems();


        }

        private void OnChoiceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox choiceComboBox = sender as ComboBox;
            WrapPanel choicePanel = choiceComboBox.Parent as WrapPanel;

            Grid Home = choicePanel.Parent as Grid;

            WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;

            WrapPanel genericProductPanel = Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;


            WrapPanel companyCategoryPanel = Home.Children[5] as WrapPanel;

            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;


            WrapPanel companyProductPanel = Home.Children[6] as WrapPanel;

            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;


            WrapPanel companyBrandPanel = Home.Children[7] as WrapPanel;

            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;


            WrapPanel companyModelPanel = Home.Children[8] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            WrapPanel companySpecsPanel = Home.Children[9] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;




            if (choiceComboBox.SelectedIndex == 0)
            {

                genericCategoryComboBox.IsEnabled = true;
                genericProductComboBox.IsEnabled = true;
                genericBrandComboBox.IsEnabled = true;
                genericModelComboBox.IsEnabled = true;

                companyCategoryComboBox.IsEnabled = false;
                companyCategoryComboBox.SelectedIndex = -1;
                companyProductComboBox.IsEnabled = false;
                companyBrandComboBox.IsEnabled = false;
                companyModelComboBox.IsEnabled = false;
                companySpecsComboBox.IsEnabled = false;


            }

            else            
            {

                genericCategoryComboBox.IsEnabled = false;
                genericCategoryComboBox.SelectedIndex = -1;
                genericProductComboBox.IsEnabled = false;
                genericBrandComboBox.IsEnabled = false;
                genericModelComboBox.IsEnabled = false;


                companyCategoryComboBox.IsEnabled = true;
                companyProductComboBox.IsEnabled = true;
                companyBrandComboBox.IsEnabled = true;
                companyModelComboBox.IsEnabled = true;
                companySpecsComboBox.IsEnabled = true;

            }




        }

        /// ////////////////////////////////////////////       
        //TEXT CHANGE FUNCTIONS
        ////////////////////////////////////////////////
        private void QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox quantity = sender as TextBox;

            Grid quantityGrid = quantity.Parent as Grid;
            TextBox availableQuantity = quantityGrid.Children[1] as TextBox;

            for (int i = 0; i < quantity.Text.Length; i++)
            {


                if (!char.IsDigit(quantity.Text[i]))
                {

                    quantity.Text = "";

                    return;
                }
            }

            if (quantity.Text == "")
            {
                availableQuantity.Text = availableQuantity.Tag.ToString();

                return;

            }

            if (int.Parse(quantity.Text) > Convert.ToInt32(availableQuantity.Tag))
            {

                System.Windows.Forms.MessageBox.Show("The available quantity is not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                availableQuantity.Text = availableQuantity.Tag.ToString();
                quantity.Text = "";

                return;

            }


            availableQuantity.Text = (Convert.ToInt32(availableQuantity.Tag) - Convert.ToInt32(quantity.Text)).ToString();

        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {

            this.NavigationService.Navigate(addReleasePermitPage);


        }

        private void OnNextButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (ReleasePermitUploadFilesPage != null)
                this.NavigationService.Navigate(ReleasePermitUploadFilesPage);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {

            parentWindow.Close();

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {

            if (addReleasePermitPage.ReleaseDatePicker.DisplayDate.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show("release date is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            if (addReleasePermitPage.SerialIdTextBox.Text == "") {

                System.Windows.Forms.MessageBox.Show("Serial id is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }


            if (!FillReleasePermitItems())
                return;

            int status = 0;

            ///////////////////////////////////////////////////////////////////////
            ///YOU NEED TO SET ORDER / RFP STATUS, AND SET RELEASE PERMIT STATUS
            ///////////////////////////////////////////////////////////////////////
            if (addReleasePermitPage.orderChecked.IsChecked == true)
            {
                if (addReleasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && addReleasePermitPage.contactComboBox.SelectedIndex != -1)
                    status = COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL;
                else
                    status = COMPANY_WORK_MACROS.ORDER_EMPLOYEE_CUSTODY;
            }
            else
                status = COMPANY_WORK_MACROS.RFP_PENDING_SITE_RECEIVAL;


            addReleasePermitPage.materialReleasePermit.GetReleaseItems().ForEach(a => a.release_permit_item_status = status);


            if (addReleasePermitPage.orderChecked.IsChecked == true)
            {
                if (addReleasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && addReleasePermitPage.contactComboBox.SelectedIndex != -1)
                    addReleasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL);
                else
                    addReleasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.ORDER_EMPLOYEE_CUSTODY);
            }

            else
                addReleasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.RFP_PENDING_SITE_RECEIVAL);


            if (!addReleasePermitPage.materialReleasePermit.IssueNewMaterialRelease(ref serials,ref addReleasePermitPage.rfpItems))
                return;


            ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, this.addReleasePermitPage, parentWindow, ref addReleasePermitPage.materialReleasePermit);

            ReleasePermitUploadFilesPage.addReleasePermitItemPage = this;
            NavigationService.Navigate(ReleasePermitUploadFilesPage);


            parentWindow.func1.Invoke(addReleasePermitPage.materialReleasePermit.GetReleaseSerial());


        }

        private void BasicInfoLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(addReleasePermitPage);
        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (ReleasePermitUploadFilesPage != null)
                this.NavigationService.Navigate(ReleasePermitUploadFilesPage);
        }

        private bool FillReleasePermitItems() {



            addReleasePermitPage.materialReleasePermit.GetReleaseItems().Clear();

            Grid items = Home.Children[Home.Children.Count - 1] as Grid;

            ScrollViewer scroll = items.Children[1] as ScrollViewer;

            Grid itemsBody = scroll.Content as Grid;

            serials.Clear();

            int counter = 0;

            for (int i = 0; i < itemsBody.Children.Count; i++)
            {

                counter++;

                INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM releaseItem = new INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM();

                Grid item = itemsBody.Children[i] as Grid;

                bool isSerial = true;


                CheckBox productSerialCheckBox1 = item.Children[2] as CheckBox;


                if (productSerialCheckBox1 == null)
                    isSerial = false;




                if (isSerial == true)
                {

                    CheckBox productSerialCheckBox = item.Children[2] as CheckBox;

                    if (productSerialCheckBox.IsChecked == true)
                    {


                        serials.Add(productSerialCheckBox.Content.ToString());


                        releaseItem.release_permit_item_serial = counter;

                        releaseItem.entry_permit_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        releaseItem.entry_permit_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);
                        releaseItem.released_quantity_release = 0;



                        Grid location = productSerialCheckBox.Tag as Grid;


                        WrapPanel checkBoxesPanel = location.Children[1] as WrapPanel;

                        CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                        CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;



                        if (rfpCheckBox.IsChecked == true)
                        {


                            Grid rfpLocationGrid = location.Children[2] as Grid;



                            ComboBox rfpItems = rfpLocationGrid.Children[0] as ComboBox;

                            ComboBox locationsComboBox = rfpLocationGrid.Children[1] as ComboBox;

                            //Grid identityTextBoxGrid = rfpLocationGrid.Children[2] as Grid;


                            releaseItem.rfp_info.rfpRequestorTeam = addReleasePermitPage.rfp.GetRFPRequestorTeamId();


                            releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL;

                            releaseItem.rfp_info.rfpSerial = addReleasePermitPage.rfp.GetRFPSerial();
                            releaseItem.rfp_info.rfpVersion = addReleasePermitPage.rfp.GetRFPVersion();





                            if (rfpItems.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show($"you have to choose an rfp item:(if there are no items please contact the inventory to map the items)", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                                return false;
                            }

                            releaseItem.rfp_item_number = addReleasePermitPage.rfpItems[rfpItems.SelectedIndex].rfp_item_number;


                            if (addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
                            {
                                WorkOrder workOrder = new WorkOrder();


                                workOrder.InitializeWorkOrderInfo(addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                                workOrdersLocations.Clear();
                                workOrder.GetProjectLocations(ref workOrdersLocations);

                                releaseItem.order_serial = workOrder.GetOrderSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                    releaseItem.order_project_serial = workOrder.GetprojectSerial();


                                }

                            }

                            else if (addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
                            {


                                maintenanceContract.InitializeMaintenanceContractInfo(addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), addReleasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                                releaseItem.contract_serial = maintenanceContract.GetMaintContractSerial();
                                releaseItem.contract_version = maintenanceContract.GetMaintContractVersion();

                                releaseItem.contract_project_serial = maintenanceContract.GetprojectSerial();


                                maintenanceContractLocations.Clear();

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.contract_location_id = maintenanceContractLocations[locationsComboBox.SelectedIndex].location_id;


                                }

                            }


                            else if (addReleasePermitPage.rfp.GetProjectSerial() != 0)
                            {


                                companyProjectLocations.Clear();

                                commonQueries.GetCompanyProjectLocations(addReleasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                                releaseItem.company_project_serial = addReleasePermitPage.rfp.GetProjectSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {
                                    releaseItem.company_project_location = companyProjectLocations[locationsComboBox.SelectedIndex].location_id;


                                }


                            }


                        }


                        else if (orderCheckBox.IsChecked == true)
                        {


                            if (addReleasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && addReleasePermitPage.contactComboBox.SelectedIndex != -1)
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                            else
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL;

                            Grid orderLocationGrid = location.Children[2] as Grid;

                            ComboBox orderItemsComboBox = orderLocationGrid.Children[0] as ComboBox;


                            ComboBox locationsComboBox = orderLocationGrid.Children[1] as ComboBox;



                            WrapPanel ordersPanel = addReleasePermitPage.mainPanel.Children[1] as WrapPanel;

                            ComboBox ordersComboBox = ordersPanel.Children[0] as ComboBox;

                            if (ordersComboBox.SelectedIndex != -1)
                            {




                                releaseItem.workOrder_serial = addReleasePermitPage.workOrder.GetOrderSerial();

                                releaseItem.order_serial = addReleasePermitPage.workOrder.GetOrderSerial();


                                releaseItem.order_project_serial = addReleasePermitPage.workOrder.GetprojectSerial();


                                if (orderItemsComboBox.SelectedIndex != -1)
                                {

                                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = addReleasePermitPage.workOrder.GetOrderProductsList();

                                    releaseItem.workOrder_product_number = workOrderProducts[orderItemsComboBox.SelectedIndex].productNumber;


                                    if (workOrderProducts[orderItemsComboBox.SelectedIndex].productQuantity < addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                                    {

                                        System.Windows.Forms.MessageBox.Show("exceeded the max allowed quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        return false;
                                    }

                                }

                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("You have to choose workOrder Item ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return false;

                                }


                                workOrdersLocations.Clear();

                                addReleasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);

                                if (locationsComboBox.SelectedIndex != -1)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                }

                            }

                            else
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose workOrder", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return false;

                            }

                        }


                        else
                        {
                            System.Windows.Forms.MessageBox.Show($"rfp or workOrder should be checked on item number {item.Tag.ToString().Split(' ')[2]}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return false;
                        }

                        addReleasePermitPage.materialReleasePermit.AddReleaseItem(releaseItem);
                        addReleasePermitPage.materialReleasePermit.UpdateIsReleasedInfo(releaseItem.entry_permit_serial, releaseItem.entry_permit_item_serial, true);



                    }

                }



                else
                {

                    Grid quantityGrid = item.Children[2] as Grid;
                    CheckBox quantityCheckBox = quantityGrid.Children[0] as CheckBox;
                    TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;


                    if (quantityCheckBox.IsChecked == true)
                    {

                        Grid card = quantityCheckBox.Tag as Grid;


                        releaseItem.release_permit_item_serial = counter;

                        releaseItem.entry_permit_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        releaseItem.entry_permit_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);

                        if (quantityTextBox.Text != "")
                            releaseItem.released_quantity_release = int.Parse(quantityTextBox.Text);


                        Grid location = quantityCheckBox.Tag as Grid;


                        WrapPanel checkBoxesPanel = location.Children[1] as WrapPanel;

                        CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                        CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;



                        if (rfpCheckBox.IsChecked == true)
                        {

                            releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL;

                            Grid rfpLocationGrid = location.Children[2] as Grid;

                            ComboBox rfpItems = rfpLocationGrid.Children[0] as ComboBox;

                            ComboBox locationsComboBox = rfpLocationGrid.Children[1] as ComboBox;





                            releaseItem.rfp_info.rfpRequestorTeam = addReleasePermitPage.rfp.GetRFPRequestorTeamId();

                            releaseItem.rfp_info.rfpSerial = addReleasePermitPage.rfp.GetRFPSerial();

                            releaseItem.rfp_info.rfpVersion = addReleasePermitPage.rfp.GetRFPVersion();

                            if (rfpItems.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show($"you have to choose an rfp item:(if there are no items please contact the inventory to map the items)", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                                return false;
                            }


                            releaseItem.rfp_item_number = addReleasePermitPage.rfpItems[rfpItems.SelectedIndex].rfp_item_number;


                            if (addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
                            {

                                WorkOrder workOrder = new WorkOrder();


                                workOrder.InitializeWorkOrderInfo(addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                                workOrdersLocations.Clear();
                                workOrder.GetProjectLocations(ref workOrdersLocations);

                                releaseItem.order_serial = workOrder.GetOrderSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                    releaseItem.order_project_serial = workOrder.GetprojectSerial();


                                }




                            }
                            else if (addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
                            {


                                maintenanceContract.InitializeMaintenanceContractInfo(addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), addReleasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                                releaseItem.contract_serial = maintenanceContract.GetMaintContractSerial();
                                releaseItem.contract_version = maintenanceContract.GetMaintContractVersion();

                                releaseItem.contract_project_serial = maintenanceContract.GetprojectSerial();


                                maintenanceContractLocations.Clear();

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.contract_location_id = maintenanceContractLocations[locationsComboBox.SelectedIndex].location_id;


                                }



                            }
                            else
                            {

                                companyProjectLocations.Clear();

                                commonQueries.GetCompanyProjectLocations(addReleasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                                releaseItem.company_project_serial = addReleasePermitPage.rfp.GetProjectSerial();
                                if (locationsComboBox.Items.Count != 0)
                                {
                                    releaseItem.company_project_location = companyProjectLocations[locationsComboBox.SelectedIndex].location_id;


                                }


                            }


                        }


                        else if (orderCheckBox.IsChecked == true)
                        {

                            if (addReleasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && addReleasePermitPage.contactComboBox.SelectedIndex != -1)
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                            else
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL;

                            Grid orderLocationGrid = location.Children[2] as Grid;


                            ComboBox orderItemsComboBox = orderLocationGrid.Children[0] as ComboBox;


                            ComboBox locationsComboBox = orderLocationGrid.Children[1] as ComboBox;


                            WrapPanel ordersPanel = addReleasePermitPage.mainPanel.Children[1] as WrapPanel;

                            ComboBox ordersComboBox = ordersPanel.Children[0] as ComboBox;


                            if (ordersComboBox.SelectedIndex != -1)
                            {

                                releaseItem.workOrder_serial = addReleasePermitPage.workOrder.GetOrderSerial();

                                releaseItem.order_serial = addReleasePermitPage.workOrder.GetOrderSerial();


                                releaseItem.order_project_serial = addReleasePermitPage.workOrder.GetprojectSerial();


                                if (orderItemsComboBox.SelectedIndex != -1)
                                {

                                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = addReleasePermitPage.workOrder.GetOrderProductsList();

                                    releaseItem.workOrder_product_number = workOrderProducts[orderItemsComboBox.SelectedIndex].productNumber;


                                    if (workOrderProducts[orderItemsComboBox.SelectedIndex].productQuantity < addReleasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                                    {

                                        System.Windows.Forms.MessageBox.Show("exceeded the max allowed quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        return false;
                                    }

                                }

                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("You have to choose workOrder Item ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return false;
                                }


                                workOrdersLocations.Clear();

                                addReleasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);

                                if (locationsComboBox.SelectedIndex != -1)
                                {
                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;
                                }

                            }


                            else
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose workOrder", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return false;

                            }



                        }


                        else
                        {

                            System.Windows.Forms.MessageBox.Show($"rfp or workOrder should be checked on item number {item.Tag.ToString().Split(' ')[2]}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return false;

                        }

                        addReleasePermitPage.materialReleasePermit.AddReleaseItem(releaseItem);
                        addReleasePermitPage.materialReleasePermit.UpdateReleasedQuantity(int.Parse(quantityTextBox.Text), releaseItem.entry_permit_serial, releaseItem.entry_permit_item_serial);
                    }

                }


            }
            return true;

        }

      
    }
}
