using _01electronics_library;
using System;
using System.Collections.Generic;
//using System.Drawing;
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

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddReservationItemsPage.xaml
    /// </summary>
    public partial class AddReservationItemsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected MaterialReservation materialReservation;
        protected int viewAddCondition;
        protected int itemsCounter = 1;
        protected int isQuantity = 1;
        protected int isSerial = 2;
        protected int numberOfSelectedItems = 0;

        protected List<string> serialsList;

        protected List<KeyValuePair< BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>> selectedItemsGrids;

        protected MaterialEntryPermit materialEntry;
        protected List<MaterialEntryPermit> entryPermits;

        protected List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> genericBrands;
        protected List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL> genericModels;
        protected List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> genericProducts;
        protected List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> genericCategories;

        protected List<COMPANY_WORK_MACROS.BRAND_STRUCT> companyBrands;
        protected List<COMPANY_WORK_MACROS.MODEL_STRUCT> companyModels;
        protected List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> companyProducts;
        public List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrders;
        protected List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> companyCategories;
        protected List<BASIC_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT> listOfReservations;

        public AddReservationBasicInfoPage addReservationBasicInfoPage;

        public AddReservationItemsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref MaterialReservation mMaterialReservation, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            materialReservation = mMaterialReservation;
            viewAddCondition = mViewAddCondition;

            serialsList = new List<string>();
            selectedItemsGrids = new List<KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>>();
            materialEntry = new MaterialEntryPermit();
            entryPermits = new List<MaterialEntryPermit>();

            genericBrands = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();
            genericModels = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL>();
            genericProducts = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS>();
            genericCategories = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY>();

            companyBrands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
            companyModels = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();
            companyProducts = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            companyProducts = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();

            workOrders = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
            companyCategories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
            listOfReservations = new List<BASIC_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>();

            InitializeComponent();

            if (!materialEntry.InitializeMaterialEntryPermits(ref entryPermits))
                return;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTRA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FilterItems()
        {
            itemsGrid.Children.Clear();

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

            itemsGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(Itemsheader, itemsGrid.RowDefinitions.Count - 1);
            itemsGrid.Children.Add(Itemsheader);

            ScrollViewer scroll = new ScrollViewer();
            scroll.CanContentScroll = true;
            scroll.Height = 600;

            Grid itemsBody = new Grid();
            itemsBody.ShowGridLines = true;

            scroll.Content = itemsBody;

            itemsGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(scroll, itemsGrid.RowDefinitions.Count - 1);
            itemsGrid.Children.Add(scroll);

            if (genericCheckBox.IsChecked == true)
            {
                int previousSerial = 0;

                itemsCounter = 1;

                for (int i = 0; i < entryPermits.Count; i++)
                {
                    if (previousSerial == entryPermits[i].GetEntryPermitSerialid())
                        continue;

                    previousSerial = entryPermits[i].GetEntryPermitSerialid();

                    materialEntry.SetEntryPermitSerialid(entryPermits[i].GetEntryPermitSerialid());
                    if(!materialEntry.InitializeMaterialEntryPermit())
                        return;

                    for (int j = 0; j < materialEntry.GetItems().Count; j++)
                    {
                        if (categoryComboBox.SelectedIndex != -1)
                            if (genericCategories[categoryComboBox.SelectedIndex].category_id != materialEntry.GetItems()[j].genericCategory.category_id)
                                continue;

                        if (typeComboBox.SelectedIndex != -1)
                            if (genericProducts[typeComboBox.SelectedIndex].product_id != materialEntry.GetItems()[j].genericProduct.product_id)
                                continue;

                        if (brandComboBox.SelectedIndex != -1)
                            if (genericBrands[brandComboBox.SelectedIndex].brand_id != materialEntry.GetItems()[j].genericBrand.brand_id)
                                continue;

                        if (modelComboBox.SelectedIndex != -1)
                            if (modelComboBox.SelectedIndex != -1)
                                if (genericModels[modelComboBox.SelectedIndex].model_id != materialEntry.GetItems()[j].genericModel.model_id)
                                    continue;

                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });

                        Grid itemSubGrid = new Grid();
                        itemSubGrid.ShowGridLines = true;
                        itemSubGrid.Tag = entryPermits[i].GetEntryPermitSerialid().ToString() + " " + materialEntry.GetItems()[j].entry_permit_item_serial + " " + itemsCounter;
                        itemsCounter++;
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());

                        Label entryPermitSerialLabel = new Label();
                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");
                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();
                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        Grid.SetColumn(entryPermitSerialLabel, 0);
                        itemSubGrid.Children.Add(entryPermitSerialLabel);

                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;
                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;
                        ItemName.VerticalAlignment = VerticalAlignment.Top;
                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");
                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                        ItemName.Text = $"{materialEntry.GetItems()[j].genericCategory.category_name + "-" + materialEntry.GetItems()[j].genericProduct.product_name + "-" + materialEntry.GetItems()[j].genericBrand.brand_name + "-" + materialEntry.GetItems()[j].genericModel.model_name}";
                        Grid.SetColumn(ItemName, 1);
                        itemSubGrid.Children.Add(ItemName);

                        if (materialEntry.GetItems()[j].product_serial_number != "")
                        {
                            CheckBox chooseItemSerialCheckBox = new CheckBox();
                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;
                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                            chooseItemSerialCheckBox.Checked += OnCheckItemSerialCheckBox;
                            chooseItemSerialCheckBox.Unchecked += OnUnCheckItemSerialCheckBox;
                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");
                            Grid.SetColumn(chooseItemSerialCheckBox, 2);
                            itemSubGrid.Children.Add(chooseItemSerialCheckBox);

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentSerialWasChecked = selectedItemsGrids.Find(item => item.Key.entry_permit_serial == entryPermits[i].GetEntryPermitSerialid() &&
                                                                                            item.Key.entry_permit_item_serial == materialEntry.GetItems()[j].entry_permit_item_serial);
                            if (currentSerialWasChecked.Key.entry_permit_serial != 0)
                            {
                                chooseItemSerialCheckBox.IsChecked = true;
                                chooseItemSerialCheckBox.Tag = currentSerialWasChecked.Value;
                            }
                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "")
                        {
                            Grid quantityGrid = new Grid();
                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            TextBox quantityAvailableTextBox = new TextBox();
                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity;
                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");
                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;

                            TextBox quantityTextBox = new TextBox();
                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");
                            quantityTextBox.TextChanged += OnTextChangedQuantityTextBox;
                            quantityTextBox.IsEnabled = false;

                            CheckBox quantityCheckBox = new CheckBox();
                            quantityCheckBox.Width = 30;
                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");
                            quantityCheckBox.Checked += OnCheckQuantityCheckBox;
                            quantityCheckBox.Unchecked += OnUnCheckQuantityCheckBox;
                            Grid.SetColumn(quantityCheckBox, 0);
                            Grid.SetColumn(quantityAvailableTextBox, 1);
                            Grid.SetColumn(quantityTextBox, 2);

                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);
                            Grid.SetColumn(quantityGrid, 3);
                            itemSubGrid.Children.Add(quantityGrid);

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentSerialWasChecked = selectedItemsGrids.Find(item => item.Key.entry_permit_serial == entryPermits[i].GetEntryPermitSerialid() &&
                                                                                            item.Key.entry_permit_item_serial == materialEntry.GetItems()[j].entry_permit_item_serial);
                            if (currentSerialWasChecked.Key.entry_permit_serial != 0)
                            {
                                quantityCheckBox.IsChecked = true;
                                quantityCheckBox.Tag = currentSerialWasChecked.Value;
                                quantityTextBox.Text = currentSerialWasChecked.Key.quantity.ToString();
                            }
                        }

                        Grid.SetRow(itemSubGrid, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemSubGrid);

                    }

                }

            }

            else if (companyCheckBox.IsChecked == true)
            {
                int previousSerial = 0;

                itemsCounter = 1;

                for (int i = 0; i < entryPermits.Count; i++)
                {
                    if (previousSerial == entryPermits[i].GetEntryPermitSerialid())
                        continue;

                    previousSerial = entryPermits[i].GetEntryPermitSerialid();

                    materialEntry.SetEntryPermitSerialid(entryPermits[i].GetEntryPermitSerialid());
                    if(!materialEntry.InitializeMaterialEntryPermit())
                        return;

                    for (int j = 0; j < materialEntry.GetItems().Count; j++)
                    {
                        if (categoryComboBox.SelectedIndex != -1)
                            if (companyCategories[categoryComboBox.SelectedIndex].categoryId != materialEntry.GetItems()[j].companyCategory.categoryId)
                                continue;

                        if (typeComboBox.SelectedIndex != -1)
                            if (companyProducts[typeComboBox.SelectedIndex].typeId != materialEntry.GetItems()[j].companyProduct.typeId)
                                continue;

                        if (brandComboBox.SelectedIndex != -1)
                            if (companyBrands[brandComboBox.SelectedIndex].brandId != materialEntry.GetItems()[j].companyBrand.brandId)
                                continue;

                        if (modelComboBox.SelectedIndex != -1)
                            if (companyModels[modelComboBox.SelectedIndex].modelId != materialEntry.GetItems()[j].companyModel.modelId)
                                continue;

                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                        Grid itemSubGrid = new Grid();
                        itemSubGrid.ShowGridLines = true;
                        itemSubGrid.Tag = entryPermits[i].GetEntryPermitSerialid().ToString() + " " + materialEntry.GetItems()[j].entry_permit_item_serial + " " + itemsCounter;
                        itemsCounter++;
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        itemSubGrid.ColumnDefinitions.Add(new ColumnDefinition());

                        Label entryPermitSerialLabel = new Label();                        
                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");
                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();
                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                        Grid.SetColumn(entryPermitSerialLabel, 0);
                        itemSubGrid.Children.Add(entryPermitSerialLabel);
                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;

                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;
                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;
                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");
                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                        ItemName.Text = $"{materialEntry.GetItems()[j].companyProduct.typeName + "-" + materialEntry.GetItems()[j].companyBrand.brandName + "-" + materialEntry.GetItems()[j].companyModel.modelName}";
                        Grid.SetColumn(ItemName, 1);
                        itemSubGrid.Children.Add(ItemName);

                        if (materialEntry.GetItems()[j].product_serial_number != "")
                        {
                            CheckBox chooseItemSerialCheckBox = new CheckBox();
                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;
                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");
                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                            chooseItemSerialCheckBox.Checked += OnCheckItemSerialCheckBox;
                            chooseItemSerialCheckBox.Unchecked += OnUnCheckItemSerialCheckBox;
                            Grid.SetColumn(chooseItemSerialCheckBox, 2);
                            itemSubGrid.Children.Add(chooseItemSerialCheckBox);

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentSerialWasChecked = selectedItemsGrids.Find(item => item.Key.entry_permit_serial == entryPermits[i].GetEntryPermitSerialid() &&
                                                                                            item.Key.entry_permit_item_serial == materialEntry.GetItems()[j].entry_permit_item_serial);
                            if (currentSerialWasChecked.Key.entry_permit_serial != 0)
                            {
                                chooseItemSerialCheckBox.IsChecked = true;
                                chooseItemSerialCheckBox.Tag = currentSerialWasChecked.Value;
                            }
                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "")
                        {
                            Grid quantityGrid = new Grid();
                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            CheckBox quantityCheckBox = new CheckBox();
                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");
                            quantityCheckBox.Width = 30;
                            quantityCheckBox.Checked += OnCheckQuantityCheckBox;
                            quantityCheckBox.Unchecked += OnUnCheckQuantityCheckBox;

                            TextBox quantityAvailableTextBox = new TextBox();
                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity;
                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");
                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;

                            TextBox quantityTextBox = new TextBox();
                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");
                            quantityTextBox.TextChanged += OnTextChangedQuantityTextBox;
                            quantityTextBox.IsEnabled = false;

                            Grid.SetColumn(quantityCheckBox, 0);
                            Grid.SetColumn(quantityAvailableTextBox, 1);
                            Grid.SetColumn(quantityTextBox, 2);

                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);
                            Grid.SetColumn(quantityGrid, 3);
                            itemSubGrid.Children.Add(quantityGrid);

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentSerialWasChecked = selectedItemsGrids.Find(item => item.Key.entry_permit_serial == entryPermits[i].GetEntryPermitSerialid() &&
                                                                                            item.Key.entry_permit_item_serial == materialEntry.GetItems()[j].entry_permit_item_serial);
                            if (currentSerialWasChecked.Key.entry_permit_serial != 0)
                            {
                                quantityCheckBox.IsChecked = true;
                                quantityCheckBox.Tag = currentSerialWasChecked.Value;
                                quantityTextBox.Text = currentSerialWasChecked.Key.quantity.ToString();
                            }

                        }

                        Grid.SetRow(itemSubGrid, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemSubGrid);

                    }

                }

            }

            if(itemsBody.Children.Count == 0)
                itemsGrid.Children.Clear();

        }
        private void AddItemCard(CheckBox sender, int quantityOrSerial)
        {
            numberOfSelectedItems++;
            Grid itemSubGrid = new Grid();

            if (quantityOrSerial == isSerial)
            {
                itemSubGrid = sender.Parent as Grid;
            }
            else if (quantityOrSerial == isQuantity)
            {
                Grid quantityGrid = sender.Parent as Grid;
                TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;
                quantityTextBox.IsEnabled = true;

                itemSubGrid = quantityGrid.Parent as Grid;

            }

            bool currentSerialWasChecked = selectedItemsGrids.Exists(item => item.Key.entry_permit_serial == Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[0]) &&
                                                                            item.Key.entry_permit_item_serial == Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[1]));
            if (!currentSerialWasChecked)
            {
                numberOfSelectedItemsLabel.Content = numberOfSelectedItems.ToString();
                TextBlock itemName = itemSubGrid.Children[1] as TextBlock;

                Grid card = new Grid() { Margin = new Thickness(15, 30, 15, 30) };
                card.Height = 150;
                card.Background = Brushes.White;
                card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                card.RowDefinitions.Add(new RowDefinition());

                Grid header = new Grid() { };
                header.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                TextBlock itemHeader = new TextBlock();
                header.Children.Add(itemHeader);

                itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
                itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                itemHeader.FontSize = 16;
                itemHeader.Text = itemName.Text + " ," + sender.Content;
                itemHeader.Foreground = Brushes.White;
                itemHeader.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(header, 0);

                sender.Tag = card;

                BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT currentEntryPermitKeys = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                currentEntryPermitKeys.entry_permit_serial = Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[0]);
                currentEntryPermitKeys.entry_permit_item_serial = Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[1]);
                selectedItemsGrids.Add(new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(currentEntryPermitKeys, card));

                WrapPanel rfpOrOrderOrQuotation = new WrapPanel();

                Label itemLabel = new Label();
                itemLabel.Style = (Style)FindResource("tableItemLabel");
                itemLabel.Content = "Item*";
                itemLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


                ComboBox itemComboBox = new ComboBox();
                itemComboBox.Style = (Style)FindResource("comboBoxStyle");
                itemComboBox.SelectionChanged += OnSelChangedOrdersComboBox;
                itemComboBox.IsEditable = true;
                itemComboBox.IsTextSearchEnabled = true;
                itemComboBox.IsTextSearchCaseSensitive = false;
                FillItemsComboBox(ref itemComboBox);

                if (quantityOrSerial == isQuantity)
                {
                    itemComboBox.Text = "Enter required quantity";
                    itemComboBox.IsEnabled = false;
                }

                rfpOrOrderOrQuotation.Children.Add(itemLabel);
                rfpOrOrderOrQuotation.Children.Add(itemComboBox);
                Grid.SetRow(rfpOrOrderOrQuotation, 1);

                card.Children.Add(header);
                card.Children.Add(rfpOrOrderOrQuotation);

                selectedItemsWrapPanel.Children.Add(card);
            }
        }
        private void FillItemsComboBox(ref ComboBox itemsCombobox)
        {
            itemsCombobox.Items.Clear();

            if (materialReservation.GetRfpRequseterTeamId() != 0 && materialReservation.GetRfpSerial() != 0 && materialReservation.GetRfpVersion() != 0
                 && addReservationBasicInfoPage.selectedRFP != null)
            {                
                for(int i = 0; i < addReservationBasicInfoPage.rfpItems.Count(); i++)
                {
                    String itemName = String.Empty;
                    if ((addReservationBasicInfoPage.rfpItems[i].rfpItem.company_model.modelId != 0 ||
                        addReservationBasicInfoPage.rfpItems[i].rfpItem.company_brand.brandId != 0 ||
                        addReservationBasicInfoPage.rfpItems[i].rfpItem.company_product.typeId != 0 ||
                        addReservationBasicInfoPage.rfpItems[i].rfpItem.company_category.categoryId != 0) &&
                        (addReservationBasicInfoPage.rfpItems[i].rfpItem.item_status.status_id == COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED ||
                         addReservationBasicInfoPage.rfpItems[i].rfpItem.item_status.status_id == COMPANY_WORK_MACROS.RFP_QUOTED))
                    {
                        itemName = addReservationBasicInfoPage.rfpItems[i].rfpItem.company_category.category + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.company_product.typeName + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.company_brand.brandName + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.company_model.modelName;
                    }
                    else if ((addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_model.model_id != 0 ||
                             addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_brand.brand_id != 0 ||
                             addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_type.product_id != 0 ||
                             addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_category.category_id != 0) &&
                             (addReservationBasicInfoPage.rfpItems[i].rfpItem.item_status.status_id == COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED ||
                             addReservationBasicInfoPage.rfpItems[i].rfpItem.item_status.status_id == COMPANY_WORK_MACROS.RFP_QUOTED))
                    {
                        itemName = addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_category.category_name + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_type.product_name + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_brand.brand_name + " - " +
                                   addReservationBasicInfoPage.rfpItems[i].rfpItem.generic_product_model.model_name;
                    }
                    if (itemName != String.Empty)
                        itemsCombobox.Items.Add(itemName);
                }
            }
            else if(materialReservation.GetOrderSerial() != 0 && addReservationBasicInfoPage.selectedWorkOrder != null)
            {
                for (int i = 0; i < addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList().Count(); i++)
                {
                    String itemName = String.Empty;
                    if ((addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productModel.modelId != 0 ||
                        addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productBrand.brandId != 0 ||
                        addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productType.typeId != 0 ||
                        addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productCategory.categoryId != 0) &&
                        (addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].product_status.status_id == COMPANY_WORK_MACROS.OPEN_WORK_ORDER ||
                         addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].product_status.status_id == COMPANY_WORK_MACROS.PENDING_STOCK_RECEIVAL_WORK_ORDER_ITEM))
                    {
                        itemName = addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productCategory.category + " - " +
                                   addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productType.typeName + " - " +
                                   addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productBrand.brandName + " - " +
                                   addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[i].productModel.modelName;

                        if (itemName != String.Empty)
                            itemsCombobox.Items.Add(itemName);
                    }
                }
            }
            else if (materialReservation.GetQuotationOfferProposer() != 0 && materialReservation.GetQuotationOfferSerial() != 0 && materialReservation.GetQuotationOfferVersion() != 0
                 && addReservationBasicInfoPage.selectedQuotation != null)
            {
                for (int i = 0; i < addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList().Count(); i++)
                {
                    String itemName = String.Empty;
                    if ((addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productModel.modelId != 0 ||
                        addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productBrand.brandId != 0 ||
                        addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productType.typeId != 0 ||
                        addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productCategory.categoryId != 0))
                    {
                        itemName = addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productCategory.category + " - " +
                                   addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productType.typeName + " - " +
                                   addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productBrand.brandName + " - " +
                                   addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[i].productModel.modelName;

                        if (itemName != String.Empty)
                            itemsCombobox.Items.Add(itemName);
                    }
                }
            }
        }
        private void FillListOfReservations()
        {
            listOfReservations.Clear();

            for (int i = 0; i < selectedItemsGrids.Count; i++)
            {
                BASIC_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT currentItem = new BASIC_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();
                currentItem.entry_permit_serial = selectedItemsGrids[i].Key.entry_permit_serial;
                currentItem.entry_permit_item_serial = selectedItemsGrids[i].Key.entry_permit_item_serial;
                currentItem.quantity = selectedItemsGrids[i].Key.quantity;
                currentItem.serial_number = selectedItemsGrids[i].Key.serial_number;
                if (addReservationBasicInfoPage.selectedRFP != null)
                    currentItem.rfp_item_no = selectedItemsGrids[i].Key.item_number;
                else if (addReservationBasicInfoPage.selectedQuotation != null)
                    currentItem.quotation_product_no = selectedItemsGrids[i].Key.item_number;
                else if (addReservationBasicInfoPage.selectedWorkOrder != null)
                    currentItem.order_product_no = selectedItemsGrids[i].Key.item_number;

                listOfReservations.Add(currentItem);
            }
        }
        public void ClearItemsPage()
        {
            genericCheckBox.IsChecked = false;
            companyCheckBox.IsChecked = false;
            categoryComboBox.SelectedIndex = -1;
            itemsGrid.Children.Clear();
            serialsList.Clear();
            categoryComboBox.Text = "Category";
            typeComboBox.Text = "Type";
            brandComboBox.Text = "Brand";
            modelComboBox.Text = "Model";
            numberOfSelectedItems = 0;
            numberOfSelectedItemsLabel.Content = "0";
            selectedItemsWrapPanel.Children.Clear();
            selectedItemsGrids.Clear();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON TEXT CHANGE HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedQuantityTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox quantity = sender as TextBox;

            Grid quantityGrid = quantity.Parent as Grid;
            Grid itemSubGrid = quantityGrid.Parent as Grid;
            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentSerialWasChecked = selectedItemsGrids.Find(item => item.Key.entry_permit_serial == Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[0]) &&
                                                                            item.Key.entry_permit_item_serial == Convert.ToInt32(itemSubGrid.Tag.ToString().Split(' ')[1]));

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

            BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
            tempKey.Copy(currentSerialWasChecked.Key);
            tempKey.quantity = Convert.ToInt32(quantity.Text);

            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentSerialWasChecked.Value);
            selectedItemsGrids.Remove(currentSerialWasChecked);
            selectedItemsGrids.Add(tempItem);

            Grid currentSelectedItemGrid = tempItem.Value;
            WrapPanel bodyWrapPanel = currentSelectedItemGrid.Children[1] as WrapPanel;
            ComboBox itemComboBox = bodyWrapPanel.Children[1] as ComboBox;
            itemComboBox.IsEnabled = true;
            itemComboBox.Text = "";


            availableQuantity.Text = (Convert.ToInt32(availableQuantity.Tag) - Convert.ToInt32(quantity.Text)).ToString();

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON BUTTON CLICK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            addReservationBasicInfoPage.addReservationItemsPage = this;
            NavigationService.Navigate(addReservationBasicInfoPage);
        }
        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            FillListOfReservations();
            if (!materialReservation.IssueMultipleReservations(listOfReservations))
                return;
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow parent = this.Parent as NavigationWindow;
            parent.Close();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON MOUSE LEFT BUTTON DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseLeftButtonDownBasicInfoLabel(object sender, MouseButtonEventArgs e)
        {
            addReservationBasicInfoPage.addReservationItemsPage = this;
            NavigationService.Navigate(addReservationBasicInfoPage);
        }
        private void OnMouseLeftButtonDownItemsInfoLabel(object sender, MouseButtonEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON CHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckCompanyCheckBox(object sender, RoutedEventArgs e)
        {
            categoryComboBox.SelectedIndex = -1;
            categoryComboBox.Text = "Category";
            typeComboBox.Text = "Type";
            brandComboBox.Text = "Brand";
            modelComboBox.Text = "Model";
            itemsGrid.Children.Clear();
            categoryComboBox.IsEnabled = true;
            genericCheckBox.IsChecked = false;
            categoryComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            specsComboBox.Visibility = Visibility.Visible;

            companyCategories.Clear();
            categoryComboBox.Items.Clear();

            if (!commonQueries.GetProductCategories(ref companyCategories))
                return;

            companyCategories.ForEach(a => categoryComboBox.Items.Add(a.category));
        }
        private void OnCheckGenericCheckBox(object sender, RoutedEventArgs e)
        {
            categoryComboBox.SelectedIndex = -1;
            categoryComboBox.Text = "Category";
            typeComboBox.Text = "Type";
            brandComboBox.Text = "Brand";
            modelComboBox.Text = "Model";
            itemsGrid.Children.Clear();
            categoryComboBox.IsEnabled = true;
            companyCheckBox.IsChecked = false;
            categoryComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            specsComboBox.Visibility = Visibility.Collapsed;

            genericCategories.Clear();
            categoryComboBox.Items.Clear();

            if (!commonQueries.GetGenericProductCategories(ref genericCategories))
                return;

            genericCategories.ForEach(a => categoryComboBox.Items.Add(a.category_name));
        }
        private void OnCheckQuantityCheckBox(object sender, RoutedEventArgs e)
        {
            AddItemCard(sender as CheckBox, isQuantity);
        }
        private void OnCheckItemSerialCheckBox(object sender, RoutedEventArgs e)
        {
            AddItemCard(sender as CheckBox, isSerial);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUnCheckCompanyCheckBox(object sender, RoutedEventArgs e)
        {
            itemsGrid.Children.Clear();
            specsComboBox.Visibility = Visibility.Collapsed;
            
            if(genericCheckBox.IsChecked == false)
                categoryComboBox.IsEnabled = false;
        }
        private void OnUnCheckGenericCheckBox(object sender, RoutedEventArgs e)
        {
            itemsGrid.Children.Clear();

            if (companyCheckBox.IsChecked == false)
            {
                categoryComboBox.IsEnabled = false;
                specsComboBox.Visibility = Visibility.Collapsed;
            }
        }
        private void OnUnCheckQuantityCheckBox(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems--;

            numberOfSelectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox = sender as CheckBox;

            Grid quantityGrid = chooseItemCheckBox.Parent as Grid;

            TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;
            quantityTextBox.IsEnabled = false;
            quantityTextBox.Text = "";

            Grid card = chooseItemCheckBox.Tag as Grid;

            WrapPanel orderOrRfp = card.Children[1] as WrapPanel;
            ComboBox itemComboBox = orderOrRfp.Children[1] as ComboBox;

            if (itemComboBox.SelectedIndex != -1)
            {
                if (quantityTextBox.Text != "")
                    addReservationBasicInfoPage.serialProducts[itemComboBox.SelectedIndex] -= int.Parse(quantityTextBox.Text);
            }

            selectedItemsGrids.Remove(selectedItemsGrids.Find(item => item.Value == card));
            selectedItemsWrapPanel.Children.Remove(card);            
        }
        private void OnUnCheckItemSerialCheckBox(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems--;

            numberOfSelectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox = sender as CheckBox;

            Grid card = chooseItemCheckBox.Tag as Grid;

            WrapPanel orderOrRfp = card.Children[1] as WrapPanel;
            ComboBox itemComboBox = orderOrRfp.Children[1] as ComboBox;

            if (itemComboBox.SelectedIndex != -1)
            {
                addReservationBasicInfoPage.serialProducts[itemComboBox.SelectedIndex]--;
            }

            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> itemToBeRemoved = selectedItemsGrids.Find(item => item.Value == card);
            selectedItemsGrids.Remove(itemToBeRemoved);
            selectedItemsWrapPanel.Children.Remove(card);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTION CHANGE HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangeCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
            typeComboBox.Items.Clear();
            brandComboBox.Items.Clear();
            modelComboBox.Items.Clear();
            //selectedItemsWrapPanel.Children.Clear();
            //numberOfSelectedItemsLabel.Content = "0";
            //numberOfSelectedItems = 0;

            if (genericCheckBox.IsChecked == true && categoryComboBox.SelectedIndex != -1)
            {
                genericProducts.Clear();

                if(!commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id))
                    return;

                typeComboBox.IsEnabled = true;
                genericProducts.ForEach(a => typeComboBox.Items.Add(a.product_name));
                typeComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if(companyCheckBox.IsChecked == true && categoryComboBox.SelectedIndex != -1)
            {
                companyProducts.Clear();

                if(!commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].categoryId))
                    return;

                typeComboBox.IsEnabled = true;
                companyProducts.ForEach(a => typeComboBox.Items.Add(a.typeName));
                typeComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if(categoryComboBox.SelectedIndex == -1 || categoryComboBox.SelectedItem == null)
            {
                typeComboBox.IsEnabled = false;
            }

            FilterItems();
        }
        private void OnSelChangeTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
            brandComboBox.Items.Clear();
            modelComboBox.Items.Clear();

            if (genericCheckBox.IsChecked == true && typeComboBox.SelectedIndex != -1)
            {
                genericBrands.Clear();

                if(!commonQueries.GetGenericProductBrands(genericProducts[typeComboBox.SelectedIndex].product_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands))
                    return;

                brandComboBox.IsEnabled = true;
                genericBrands.ForEach(a => brandComboBox.Items.Add(a.brand_name));
                brandComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if (companyCheckBox.IsChecked == true && typeComboBox.SelectedIndex != -1)
            {
                companyBrands.Clear();
             
                if(!commonQueries.GetProductBrands(companyProducts[typeComboBox.SelectedIndex].typeId, ref companyBrands))
                    return;

                brandComboBox.IsEnabled = true;
                companyBrands.ForEach(a => brandComboBox.Items.Add(a.brandName));
                brandComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if (typeComboBox.SelectedIndex == -1 || typeComboBox.SelectedItem == null)
            {
                brandComboBox.IsEnabled = false;
            }

            FilterItems();
        }
        private void OnSelChangeBrandComboBox(object sender, SelectionChangedEventArgs e)
        {
            modelComboBox.Items.Clear();

            if (genericCheckBox.IsChecked == true && brandComboBox.SelectedIndex != -1)
            {
                genericModels.Clear();

                if (!commonQueries.GetGenericBrandModels(genericProducts[typeComboBox.SelectedIndex].product_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels))
                    return;

                modelComboBox.IsEnabled = true;
                genericModels.ForEach(a => modelComboBox.Items.Add(a.model_name));
                modelComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if (companyCheckBox.IsChecked == true && brandComboBox.SelectedIndex != -1)
            {
                companyModels.Clear();

                if(!commonQueries.GetCompanyModels(companyProducts[typeComboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels))
                    return;

                modelComboBox.IsEnabled = true;
                companyModels.ForEach(a => modelComboBox.Items.Add(a.modelName));
                modelComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else if (brandComboBox.SelectedIndex == -1 || brandComboBox.SelectedItem == null)
            {
                modelComboBox.IsEnabled = false;
            }

            FilterItems();
        }
        private void OnSelChangeModelComboBox(object sender, SelectionChangedEventArgs e)
        {
            specsComboBox.IsEnabled = true;
            specsComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            if (modelComboBox.SelectedIndex == -1 || modelComboBox.SelectedItem == null)
            {
                specsComboBox.IsEnabled = false;
            }
            FilterItems();
        }
        private void OnSelChangeSpecsComboBox(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedOrdersComboBox(object sender, SelectionChangedEventArgs e)
        {
            if(addReservationBasicInfoPage.selectedWorkOrder != null)
            {
                ComboBox orderItemsComboBox = sender as ComboBox;

                if (addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].has_serial_number == true)
                {
                    addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex]++;

                    if (addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Order Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }

                    WrapPanel parentCard = orderItemsComboBox.Parent as WrapPanel;
                    Grid card = parentCard.Parent as Grid;
                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);
                    BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                    Grid headerGrid = currentItem.Value.Children[0] as Grid;
                    TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                    tempKey.Copy(currentItem.Key);
                    tempKey.quantity = Convert.ToInt32(1);
                    tempKey.serial_number = headerTextBlock.Text.ToString().Split(',')[1];
                    tempKey.item_number = addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productNumber;

                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                    selectedItemsGrids.Remove(currentItem);
                    selectedItemsGrids.Add(tempItem);

                }
                else
                {
                    WrapPanel orderCard = orderItemsComboBox.Parent as WrapPanel;

                    Grid card = orderCard.Parent as Grid;
                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);

                    if (currentItem.Key.quantity != 0)
                    {
                        addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex] += currentItem.Key.quantity;
                    }

                    if (addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Order Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }

                    BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                    Grid headerGrid = currentItem.Value.Children[0] as Grid;
                    TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                    tempKey.Copy(currentItem.Key);
                    tempKey.item_number = addReservationBasicInfoPage.selectedWorkOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productNumber;

                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                    selectedItemsGrids.Remove(currentItem);
                    selectedItemsGrids.Add(tempItem);
                }

            }
            else if(addReservationBasicInfoPage.selectedRFP != null)
            { 
                ComboBox rfpItemsComboBox = sender as ComboBox;

                if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.company_model.modelName != "")
                {
                    if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.company_model.has_serial_number == true)
                    {
                        if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_quantity < addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                        {
                            System.Windows.Forms.MessageBox.Show("Rfp Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;
                        }
                        else
                        {
                            addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;

                            WrapPanel parentCard = rfpItemsComboBox.Parent as WrapPanel;
                            Grid card = parentCard.Parent as Grid;
                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);
                            BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                            Grid headerGrid = currentItem.Value.Children[0] as Grid;
                            TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                            tempKey.Copy(currentItem.Key);
                            tempKey.quantity = Convert.ToInt32(1);
                            tempKey.serial_number = headerTextBlock.Text.ToString().Split(',')[1];
                            tempKey.item_number = addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_number;

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                            selectedItemsGrids.Remove(currentItem);
                            selectedItemsGrids.Add(tempItem);
                        }
                    }
                    else
                    {
                        WrapPanel rfpCard = rfpItemsComboBox.Parent as WrapPanel;

                        Grid card = rfpCard.Parent as Grid;

                        KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);

                        if (currentItem.Key.quantity != 0)
                        {
                            addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex] += currentItem.Key.quantity;
                        }

                        if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_quantity < addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                        {
                            System.Windows.Forms.MessageBox.Show("Rfp Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;

                        }

                        BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                        Grid headerGrid = currentItem.Value.Children[0] as Grid;
                        TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                        tempKey.Copy(currentItem.Key);
                        tempKey.item_number = addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_number;

                        KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                        selectedItemsGrids.Remove(currentItem);
                        selectedItemsGrids.Add(tempItem);
                    }
                }
                else
                {
                    if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.generic_product_model.has_serial_number == true)
                    {
                        if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_quantity < addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                        {
                            System.Windows.Forms.MessageBox.Show("Rfp Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;
                            WrapPanel parentCard = rfpItemsComboBox.Parent as WrapPanel;
                            Grid card = parentCard.Parent as Grid;
                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);
                            BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                            Grid headerGrid = currentItem.Value.Children[0] as Grid;
                            TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                            tempKey.Copy(currentItem.Key);
                            tempKey.quantity = Convert.ToInt32(1);
                            tempKey.serial_number = headerTextBlock.Text.ToString().Split(',')[1];
                            tempKey.item_number = addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_number;

                            KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                            selectedItemsGrids.Remove(currentItem);
                            selectedItemsGrids.Add(tempItem);
                        }
                    }
                    else
                    {
                        WrapPanel rfpCard = rfpItemsComboBox.Parent as WrapPanel;

                        Grid card = rfpCard.Parent as Grid;
                        KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);

                        if (currentItem.Key.quantity != 0)
                        {
                            addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex] += currentItem.Key.quantity;
                        }

                        if (addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_quantity < addReservationBasicInfoPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                        {
                            System.Windows.Forms.MessageBox.Show("Rfp Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;
                        }

                        BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                        Grid headerGrid = currentItem.Value.Children[0] as Grid;
                        TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                        tempKey.Copy(currentItem.Key);
                        tempKey.item_number = addReservationBasicInfoPage.rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_number;

                        KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                        selectedItemsGrids.Remove(currentItem);
                        selectedItemsGrids.Add(tempItem);
                    }
                }
            }
            else if (addReservationBasicInfoPage.selectedQuotation != null)
            {
                ComboBox orderItemsComboBox = sender as ComboBox;

                if (addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[orderItemsComboBox.SelectedIndex].has_serial_number == true)
                {
                    addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex]++;

                    if (addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Quotation Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }

                    WrapPanel parentCard = orderItemsComboBox.Parent as WrapPanel;
                    Grid card = parentCard.Parent as Grid;
                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);
                    BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                    Grid headerGrid = currentItem.Value.Children[0] as Grid;
                    TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                    tempKey.Copy(currentItem.Key);
                    tempKey.quantity = Convert.ToInt32(1);
                    tempKey.serial_number = headerTextBlock.Text.ToString().Split(',')[1];
                    tempKey.item_number = addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[orderItemsComboBox.SelectedIndex].productNumber;

                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                    selectedItemsGrids.Remove(currentItem);
                    selectedItemsGrids.Add(tempItem);
                }
                else
                {
                    WrapPanel orderCard = orderItemsComboBox.Parent as WrapPanel;

                    Grid card = orderCard.Parent as Grid;
                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> currentItem = selectedItemsGrids.Find(tmp => tmp.Value == card);

                    if (currentItem.Key.quantity != 0)
                    {
                        addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex] += currentItem.Key.quantity;
                    }

                    if (addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < addReservationBasicInfoPage.serialProducts[orderItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Quotation Item Quantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }

                    BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT tempKey = new BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT();
                    Grid headerGrid = currentItem.Value.Children[0] as Grid;
                    TextBlock headerTextBlock = headerGrid.Children[0] as TextBlock;
                    tempKey.Copy(currentItem.Key);
                    tempKey.item_number = addReservationBasicInfoPage.selectedQuotation.GetOfferProductsList()[orderItemsComboBox.SelectedIndex].productNumber;

                    KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid> tempItem = new KeyValuePair<BASIC_STRUCTS.ENTRY_PERMIT_MIN_STRUCT, Grid>(tempKey, currentItem.Value);
                    selectedItemsGrids.Remove(currentItem);
                    selectedItemsGrids.Add(tempItem);
                }

            }
        }
    }
}
