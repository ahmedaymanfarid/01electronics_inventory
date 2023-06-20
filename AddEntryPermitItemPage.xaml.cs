using _01electronics_library;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static _01electronics_library.BASIC_STRUCTS;
using static _01electronics_library.PROCUREMENT_STRUCTS;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddEntryPermitItemPage.xaml
    /// </summary>
    public partial class AddEntryPermitItemPage : Page
    {

        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        int viewAddCondition;
        private AddEntryPermitWindow entryPermitWindow;
        public AddEntryPermitPage addEntryPermitPage;

        public MaterialEntryPermit oldMaterialEntryPermit = null;

        public EntryPermitUploadFilesPage EntryPermitUploadFilesPage=null;

        List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> genericCategories;
        List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> genericProducts;
        List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> genericBrands;
        List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> genericModels;

        List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> companyCategories;
        List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> companyProducts;
        List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> companyBrands;
        List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> companyModels;

        List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies;
        List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requestors;

        List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT> rfps;

        List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems;

        List<INVENTORY_STRUCTS.STOCK_TYPES> stockTypes;

        List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT> specs;

        List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT> materialReservations;


        public AddEntryPermitItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddEntryPermitWindow mEntryPermitWindow,int mViewAddCondition, ref MaterialEntryPermit moldMaterialEntryPermit) 
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            entryPermitWindow = mEntryPermitWindow;
            
            oldMaterialEntryPermit = moldMaterialEntryPermit;

            genericCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            genericProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            genericBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            genericModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            companyCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            companyProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            companyBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            companyModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
            requestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();

            rfps = new List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT>();

            rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();

            stockTypes = new List<INVENTORY_STRUCTS.STOCK_TYPES>();

            specs = new List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT>();

            materialReservations = new List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>();

            InitializeComponent();
            GetStockTypes();
            GetCurrency();
            GetRFPRequestorTeam();
            CheckViewOrEditOrAdd();


           
        }

        public void CheckViewOrEditOrAdd()
        {
           if(viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            {
                if(oldMaterialEntryPermit.GetItems().Count!=0)
                {
                    for(int i=0;i<oldMaterialEntryPermit.GetItems().Count;i++)
                    {
                        InitializeNewCard();
                    }
                    for(int i=0;i<itemsWrapPanel.Children.Count;i++)
                    {
                        Border itemBorder = itemsWrapPanel.Children[i] as Border;
                        StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                        Label header = mainStackPanel.Children[0] as Label;
                        header.Content = "Item " + (i + 1);
                        ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                        StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                        for(int j = 0; j < oldMaterialEntryPermit.GetItems().Count;j++)
                        {
                            CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;
                           
                            rfpCheckBox.IsEnabled = false;
                            if (oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpSerial != 0)
                            {
                                rfpCheckBox.IsChecked = true;

                                WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                                rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                                ComboBoxItem rfpRequestorItem = new ComboBoxItem();


                                WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                                rfpIdWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                                rfpIdComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpID;
                                rfpIdComboBox.IsEnabled = false;

                                WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                                rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[2] as ComboBox;
                                rfpDescriptionComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfp_items[0].item_description;
                                rfpDescriptionComboBox.IsEnabled = true;


                            }
                            WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                            ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;
                            choiceComboBox.IsEnabled = false;
                            if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                            {
                                choiceComboBox.SelectedIndex = 1;
                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                            }

                            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem categoryItem = new ComboBoxItem();
                            categoryItem.Content = oldMaterialEntryPermit.GetItems()[i].product_category.category_name;
                            categoryItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_category.category_id;
                            categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(categoryItem.Tag));
                            categoryComboBox.IsEnabled = false;

                            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                            ComboBox  typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem typeItem = new ComboBoxItem();
                            typeItem.Content = oldMaterialEntryPermit.GetItems()[i].product_type.product_name;
                            typeItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_type.type_id;
                            typeComboBox.SelectedItem = typeComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(typeItem.Tag));
                            typeComboBox.IsEnabled = false;

                            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem brandItem = new ComboBoxItem();
                            brandItem.Content = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_name;
                            brandItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_id;
                            brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().First(c=>c.Tag.Equals(brandItem.Tag));
                            brandComboBox.IsEnabled = false;

                            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem modelItem = new ComboBoxItem();
                            modelItem.Content = oldMaterialEntryPermit.GetItems()[i].product_model.model_name;
                            modelItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_model.model_id;
                            modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(modelItem.Tag));
                            modelComboBox.IsEnabled = false;

                            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                            ComboBox  specsComboBox = specsWrapPanel.Children[1] as ComboBox;


                            if (oldMaterialEntryPermit.GetItems()[i].is_company_product==true)
                            {
                                ComboBoxItem specsItem = new ComboBoxItem();
                                specsItem.Content = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_name;
                                specsItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_id;
                                specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().First(c=>c.Tag.Equals(specsItem.Tag));

                                specsWrapPanel.Visibility = Visibility.Visible;
                                specsComboBox.IsEnabled = false;
                               
                            }
                            else
                            {
                               
                                specsWrapPanel.Visibility = Visibility.Collapsed;
                            }

                            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                            startSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                            startSerialTextBox.IsEnabled = false;

                            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                            TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                            endSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                            endSerialTextBox.IsEnabled = false;

                            Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                            generateSerials.IsEnabled = false;

                            WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                            quantityTextBox.Text = oldMaterialEntryPermit.GetItems()[i].quantity.ToString();
                            quantityTextBox.IsEnabled = false;

                            WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                            TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                            priceTextBox.Text = oldMaterialEntryPermit.GetItems()[i].item_price.ToString();
                            priceTextBox.IsEnabled = false;

                            WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                            ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem currencyItem = new ComboBoxItem();
                            currencyItem.Content = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyName;
                            currencyItem.Tag = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyId;
                            currencyComboBox.SelectedItem = currencyComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(currencyItem.Tag));
                            currencyComboBox.IsEnabled = false;

                            WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                            ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem stocktypeItem = new ComboBoxItem();
                            stocktypeItem.Content = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_name;
                            stocktypeItem.Tag = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_id;
                            stockTypeComboBox.SelectedItem = stockTypeComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(stocktypeItem.Tag));
                            stockTypeComboBox.IsEnabled = false;

                        }
                    }
                }

            }
           else if(viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION)
            {
                if (oldMaterialEntryPermit.GetItems().Count != 0)
                {
                    for (int i = 0; i < oldMaterialEntryPermit.GetItems().Count; i++)
                    {
                        InitializeNewCard();
                    }
                    for (int i = 0; i < itemsWrapPanel.Children.Count; i++)
                    {
                        Border itemBorder = itemsWrapPanel.Children[i] as Border;
                        StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                        Label header = mainStackPanel.Children[0] as Label;
                        header.Content = "Item " + (i + 1);
                        ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                        StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                        for (int j = 0; j < oldMaterialEntryPermit.GetItems().Count; j++)
                        {
                            CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;

                            //rfpCheckBox.IsEnabled = false;
                            if (oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpSerial != 0)
                            {
                                rfpCheckBox.IsChecked = true;

                                WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                                rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                                ComboBoxItem rfpRequestorItem = new ComboBoxItem();


                                WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                                rfpIdWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                                rfpIdComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpID;
                                //rfpIdComboBox.IsEnabled = false;

                                WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                                rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                                ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[2] as ComboBox;
                                rfpDescriptionComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfp_items[0].item_description;
                                //rfpDescriptionComboBox.IsEnabled = true;


                            }
                            WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                            ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;
                            //choiceComboBox.IsEnabled = false;
                            if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                            {
                                choiceComboBox.SelectedIndex = 1;
                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                            }

                            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem categoryItem = new ComboBoxItem();
                            categoryItem.Content = oldMaterialEntryPermit.GetItems()[i].product_category.category_name;
                            categoryItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_category.category_id;
                            categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(categoryItem.Tag));
                           // categoryComboBox.IsEnabled = false;

                            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem typeItem = new ComboBoxItem();
                            typeItem.Content = oldMaterialEntryPermit.GetItems()[i].product_type.product_name;
                            typeItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_type.type_id;
                            typeComboBox.SelectedItem = typeComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(typeItem.Tag));
                           // typeComboBox.IsEnabled = false;

                            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem brandItem = new ComboBoxItem();
                            brandItem.Content = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_name;
                            brandItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_id;
                            brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(brandItem.Tag));
                           // brandComboBox.IsEnabled = false;

                            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem modelItem = new ComboBoxItem();
                            modelItem.Content = oldMaterialEntryPermit.GetItems()[i].product_model.model_name;
                            modelItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_model.model_id;
                            modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(modelItem.Tag));
                          //  modelComboBox.IsEnabled = false;

                            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;


                            if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                            {
                                ComboBoxItem specsItem = new ComboBoxItem();
                                specsItem.Content = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_name;
                                specsItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_id;
                                specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(specsItem.Tag));

                                specsWrapPanel.Visibility = Visibility.Visible;
                             //   specsComboBox.IsEnabled = false;

                            }
                            else
                            {

                                specsWrapPanel.Visibility = Visibility.Collapsed;
                            }

                            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                            startSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                           // startSerialTextBox.IsEnabled = false;

                            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                            TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                            endSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                           // endSerialTextBox.IsEnabled = false;

                            Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                           // generateSerials.IsEnabled = false;

                            WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                            quantityTextBox.Text = oldMaterialEntryPermit.GetItems()[i].quantity.ToString();
                           // quantityTextBox.IsEnabled = false;

                            WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                            TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                            priceTextBox.Text = oldMaterialEntryPermit.GetItems()[i].item_price.ToString();
                            // priceTextBox.IsEnabled = false;

                            WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                            ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem currencyItem = new ComboBoxItem();
                            currencyItem.Content = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyName;
                            currencyItem.Tag = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyId;
                            currencyComboBox.SelectedItem = currencyComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(currencyItem.Tag));
                          
                            WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                            ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                            ComboBoxItem stocktypeItem = new ComboBoxItem();
                            stocktypeItem.Content = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_name;
                            stocktypeItem.Tag = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_id;
                            stockTypeComboBox.SelectedItem = stockTypeComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(stocktypeItem.Tag));
                     


                        }
                    }
                    addNewItemButton();
                }
            }
            else
            {
               // itemsWrapPanel.Children.RemoveAt(itemsWrapPanel.Children.Count - 1);
                InitializeNewCard();
                addNewItemButton();
            }
        }
        public void InitializeNewCard()
        {
            ///////////// CREATE CARD ///////////////
            
            var border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.CornerRadius = new CornerRadius(10);
            // border.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            border.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
            border.Width = 500;
            border.Height = 500;
            border.Margin = new Thickness(10);
            border.Effect = new DropShadowEffect { ShadowDepth = 2, BlurRadius = 5, Color = Colors.LightGray };

            var stackPanel = new StackPanel();
            stackPanel.Background = Brushes.Transparent;
            stackPanel.Margin = new Thickness(10);
            border.Child = stackPanel;

            var header = new Label();
            header.Content = $"Item {itemsWrapPanel.Children.Count+1}";
            header.Style = (Style)FindResource("GridItem");
            header.HorizontalAlignment = HorizontalAlignment.Stretch;
            header.Foreground = Brushes.White;
            header.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
            header.HorizontalContentAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(header);

            var scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.Height = 450;
           // scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            stackPanel.Children.Add(scrollViewer);

            var itemsStackPanel = new StackPanel();
            itemsStackPanel.Background = Brushes.Transparent;
            itemsStackPanel.Margin = new Thickness(10);
            itemsStackPanel.Height = 560;
            scrollViewer.Content= itemsStackPanel;

            var rfpcheckBox = new CheckBox();
            rfpcheckBox.Style = (Style)FindResource("checkBoxStyle");
            rfpcheckBox.HorizontalAlignment = HorizontalAlignment.Left;
            rfpcheckBox.Checked += OnCheckRFPCheckBox;
            rfpcheckBox.Unchecked += OnUnCheckRFPCheckBox;
            rfpcheckBox.Content = "RFP";

            var requestorLabel = new Label();
            requestorLabel.Style = (Style)FindResource("labelStyle");
            requestorLabel.Content = "Requestor Team";
            requestorLabel.Width = 140;
            requestorLabel.Margin = new Thickness(0);
            requestorLabel.Name = "requestorLabel";

            var requestorComboBox = new ComboBox();
            requestorComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            requestorComboBox.IsEnabled = false;
            requestorComboBox.Name = "requestorComboBox";
            requestorComboBox.SelectionChanged += OnSelChangedRfpRequestorTeamComboBox;

            var requestorWrapPanel = new WrapPanel();
            requestorWrapPanel.Visibility = Visibility.Collapsed;
            requestorWrapPanel.Children.Add(requestorLabel);
            requestorWrapPanel.Children.Add(requestorComboBox);

            var rfpIdLabel = new Label();
            rfpIdLabel.Style = (Style)FindResource("labelStyle");
            rfpIdLabel.Content = "RFP ID";
            rfpIdLabel.Width = 140;
            rfpIdLabel.Margin = new Thickness(0);
            rfpIdLabel.Name = "rfpIdLabel";

            var rfpIdComboBox = new ComboBox();
            rfpIdComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            rfpIdComboBox.IsEnabled = false;
            rfpIdComboBox.Name = "rfpIdComboBox";
            rfpIdComboBox.SelectionChanged += OnSelChangedRfpIdComboBox;

            var rfpIdWrapPanel = new WrapPanel();
            rfpIdWrapPanel.Visibility = Visibility.Collapsed;
            rfpIdWrapPanel.Children.Add(rfpIdLabel);
            rfpIdWrapPanel.Children.Add(rfpIdComboBox);

            var itemDescriptionTextBlock = new TextBlock();
            itemDescriptionTextBlock.Style = (Style)FindResource("tableSubItemTextblock");
            itemDescriptionTextBlock.TextAlignment = TextAlignment.Left;
            itemDescriptionTextBlock.Background = Brushes.Transparent;
            itemDescriptionTextBlock.Margin = new Thickness(3, 0, 0, 0);
            itemDescriptionTextBlock.TextWrapping = TextWrapping.Wrap;
            itemDescriptionTextBlock.Text = "RFP Item Description";
            itemDescriptionTextBlock.Width = 137;
            itemDescriptionTextBlock.Name = "itemDescriptionTextBlock";

            var rfpItemDescriptionComboBox = new ComboBox();
            rfpItemDescriptionComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            rfpItemDescriptionComboBox.IsEnabled = false;
            rfpItemDescriptionComboBox.SelectionChanged += OnSelChangedRfpItemDescriptionComboBox;

            var rfpItemDescriptionWrapPanel = new WrapPanel();
            rfpItemDescriptionWrapPanel.Visibility = Visibility.Collapsed;
            rfpItemDescriptionWrapPanel.Children.Add(itemDescriptionTextBlock);
            rfpItemDescriptionWrapPanel.Children.Add(rfpItemDescriptionComboBox);

            var choiceLabel = new Label();
            choiceLabel.Style = (Style)FindResource("labelStyle");
            choiceLabel.Margin = new Thickness(0);
            choiceLabel.Background = Brushes.Transparent;
            choiceLabel.Content = "Choose";
            choiceLabel.Width = 140;
            choiceLabel.Name = "choiseLabel";

            var choiseComboBox = new ComboBox();
            choiseComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            choiseComboBox.IsEnabled = true;
            choiseComboBox.Name = "choiseComboBox";
            choiseComboBox.SelectionChanged += OnSelChangedChoiceComboBox;
            choiseComboBox.Items.Add("Generic Products");
            choiseComboBox.Items.Add("Company Products");

            var choiseWrapPanel = new WrapPanel();
            choiseWrapPanel.Visibility = Visibility.Visible;
            choiseWrapPanel.Children.Add(choiceLabel);
            choiseWrapPanel.Children.Add(choiseComboBox);

            var companyCategoryLabel = new Label();
            companyCategoryLabel.Style = (Style)FindResource("labelStyle");
            companyCategoryLabel.Margin = new Thickness(0);
            companyCategoryLabel.Background = Brushes.Transparent;
            companyCategoryLabel.Content = "Category";
            companyCategoryLabel.Width = 140;
            companyCategoryLabel.Name = "companyCategoryLabel";

            var companyCategoryComoBox = new ComboBox();
            companyCategoryComoBox.Style = (Style)FindResource("comboBoxStyleCard2");
            companyCategoryComoBox.IsEnabled = true;
            companyCategoryComoBox.Name = "companyCategoryComoBox";
            companyCategoryComoBox.SelectionChanged +=OnSelChangedCategoryComboBox;


            var companyCategoryWrapPanel = new WrapPanel();
            //companyCategoryWrapPanel.Visibility = Visibility.Collapsed;
            companyCategoryWrapPanel.Children.Add(companyCategoryLabel);
            companyCategoryWrapPanel.Children.Add(companyCategoryComoBox);

            var productCategoryLabel = new Label();
            productCategoryLabel.Style = (Style)FindResource("labelStyle");
            productCategoryLabel.Margin = new Thickness(0);
            productCategoryLabel.Background = Brushes.Transparent;
            productCategoryLabel.Content = "Product";
            productCategoryLabel.Width = 140;
            productCategoryLabel.Name = "productCategoryLabel";

            var productCategoryComboBox = new ComboBox();
            productCategoryComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            productCategoryComboBox.IsEnabled = true;
            productCategoryComboBox.Name = "productCategoryComboBox";
            productCategoryComboBox.SelectionChanged += OnSelChangedProductComboBox;

            var productCategoryWrapPanel = new WrapPanel();
           // productCategoryWrapPanel.Visibility = Visibility.Collapsed;
            productCategoryWrapPanel.Children.Add(productCategoryLabel);
            productCategoryWrapPanel.Children.Add(productCategoryComboBox);

            var brandLabel = new Label();
            brandLabel.Style = (Style)FindResource("labelStyle");
            brandLabel.Margin = new Thickness(0);
            brandLabel.Background = Brushes.Transparent;
            brandLabel.Content = "Brand";
            brandLabel.Width = 140;
            brandLabel.Name = "brandLabel";

            var brandComboBox = new ComboBox();
            brandComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            brandComboBox.IsEnabled = false;
            brandComboBox.Name = "brandComboBox";
            brandComboBox.SelectionChanged += OnSelChangedBrandComboBox;

            var brandWrapPanel = new WrapPanel();
           // brandWrapPanel.Visibility = Visibility.Collapsed;
            brandWrapPanel.Children.Add(brandLabel);
            brandWrapPanel.Children.Add(brandComboBox);

            var modelLabel = new Label();
            modelLabel.Style = (Style)FindResource("labelStyle");
            modelLabel.Margin = new Thickness(0);
            modelLabel.Background = Brushes.Transparent;
            modelLabel.Content = "Model";
            modelLabel.Width = 140;
            modelLabel.Name = "modelLabel";

            var modelComboBox = new ComboBox();
            modelComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            modelComboBox.IsEnabled = false;
            modelComboBox.Name = "modelComboBox";
            modelComboBox.SelectionChanged += OnSelChangedModelComboBox;

            var modelWrapPanel = new WrapPanel();
           // modelWrapPanel.Visibility = Visibility.Collapsed;
            modelWrapPanel.Children.Add(modelLabel);
            modelWrapPanel.Children.Add(modelComboBox);

            var specsLabel = new Label();
            specsLabel.Style = (Style)FindResource("labelStyle");
            specsLabel.Margin = new Thickness(0);
            specsLabel.Background = Brushes.Transparent;
            specsLabel.Content = "Specs";
            specsLabel.Width = 140;
            specsLabel.Name = "specsLabel";
           // specsLabel.Visibility = Visibility.Collapsed;

            var specsComboBox = new ComboBox();
            specsComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            specsComboBox.IsEnabled = false;
            specsComboBox.Name = "specsComboBox";
           

            var specsWrapPanel = new WrapPanel();
            specsWrapPanel.Visibility = Visibility.Collapsed;
            specsWrapPanel.Children.Add(specsLabel);
            specsWrapPanel.Children.Add(specsComboBox);

            var startSerialWrapPanel = new WrapPanel();

            var startSerialLabel = new Label();
            startSerialLabel.Style = (Style)FindResource("labelStyle");
            startSerialLabel.Margin = new Thickness(0);
            startSerialLabel.Background = Brushes.Transparent;
            startSerialLabel.Content = "Start Serial";
            startSerialLabel.Width = 140;
            startSerialLabel.Name = "startSerialLabel";

            var startSerialTextBox = new TextBox();
            startSerialTextBox.Style = (Style)FindResource("textBoxStyle");
            startSerialTextBox.Name = "startSerialTextBox";
            startSerialTextBox.Margin = new Thickness(0);
            startSerialTextBox.Width = 240;

            startSerialWrapPanel.Children.Add(startSerialLabel);
            startSerialWrapPanel.Children.Add(startSerialTextBox);

            // Create the second WrapPanel control
            var endSerialWrapPanel = new WrapPanel();

            var endSerialLabel = new Label();
            endSerialLabel.Style = (Style)FindResource("labelStyle");
            endSerialLabel.Margin = new Thickness(0);
            endSerialLabel.Background = Brushes.Transparent;
            endSerialLabel.Content = "End Serial";
            endSerialLabel.Width = 140;
            endSerialLabel.Name = "endSerialLabel";

            var endSerialTextBox = new TextBox();
            endSerialTextBox.Style = (Style)FindResource("textBoxStyle");
            endSerialTextBox.Name = "endSerialTextBox";
            endSerialTextBox.Margin = new Thickness(0);
            endSerialTextBox.Width = 240;

            var generateButton = new Button();
            generateButton.Style = (Style)FindResource("buttonStyle2");
            generateButton.Width = 50;
            generateButton.Content = "generate";
            generateButton.FontSize = 10;
            generateButton.Margin = new Thickness(10,0,0,0);
            generateButton.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
            generateButton.Foreground = Brushes.White;
            generateButton.Click += OnButtonClickGenenrateSerials;

            endSerialWrapPanel.Children.Add(endSerialLabel);
            endSerialWrapPanel.Children.Add(endSerialTextBox);
            endSerialWrapPanel.Children.Add(generateButton);

            var quantityLabel = new Label();
            quantityLabel.Style = (Style)FindResource("labelStyle");
            quantityLabel.Margin = new Thickness(0);
            quantityLabel.Background = Brushes.Transparent;
            quantityLabel.Content = "Quantity";
            quantityLabel.Width = 140;
            quantityLabel.Name = "quantityLabel";

            var quantityTextBox = new TextBox();
            quantityTextBox.Style = (Style)FindResource("textBoxStyle");
            quantityTextBox.Margin = new Thickness(0);
            quantityTextBox.Width = 240;
           // quantityTextBox.IsEnabled = false;
            quantityTextBox.Name = "quantityTextBox";

            var quantityWrapPanel = new WrapPanel();
           // quantityWrapPanel.Visibility = Visibility.Collapsed;
            quantityWrapPanel.Children.Add(quantityLabel);
            quantityWrapPanel.Children.Add(quantityTextBox);

            var priceLabel = new Label();
            priceLabel.Style = (Style)FindResource("labelStyle");
            priceLabel.Margin = new Thickness(0);
            priceLabel.Background = Brushes.Transparent;
            priceLabel.Content = "Price";
            priceLabel.Width = 140;
            priceLabel.Name = "priceLabel";

            var priceTextBox = new TextBox();
            priceTextBox.Style = (Style)FindResource("textBoxStyle");
            priceTextBox.Margin = new Thickness(0);
            priceTextBox.Width = 240;
           // priceTextBox.IsEnabled = false;
            priceTextBox.Name = "priceTextBox";

            var priceWrapPanel = new WrapPanel();
            priceWrapPanel.Children.Add(priceLabel);
            priceWrapPanel.Children.Add(priceTextBox);

            var currencyLabel = new Label();
            currencyLabel.Style = (Style)FindResource("labelStyle");
            currencyLabel.Margin = new Thickness(0);
            currencyLabel.Background = Brushes.Transparent;
            currencyLabel.Content = "Currency";
            currencyLabel.Width = 140;
            currencyLabel.Name = "currencyLabel";

            var currencyComboBox = new ComboBox();
            currencyComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
          //  currencyComboBox.IsEnabled = false;
            currencyComboBox.Name = "currencyComboBox";
            FillCurrencyComboBox(currencyComboBox);

            var currencyWrapPanel = new WrapPanel();
            currencyWrapPanel.Children.Add(currencyLabel);
            currencyWrapPanel.Children.Add(currencyComboBox);

            var stockTypeLabel = new Label();
            stockTypeLabel.Style = (Style)FindResource("labelStyle");
            stockTypeLabel.Margin = new Thickness(0);
            stockTypeLabel.Background = Brushes.Transparent;
            stockTypeLabel.Content = "Stock Type";
            stockTypeLabel.Width = 140;
            stockTypeLabel.Name = "stockTypeLabel";
         

            var stockTypeComboBox = new ComboBox();
            stockTypeComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
           // stockTypeComboBox.IsEnabled = false;
            stockTypeComboBox.Name = "stockTypeComboBox";
            FillStockTypesComboBox(stockTypeComboBox);

            var stockTypeWrapPanel = new WrapPanel();
            stockTypeWrapPanel.Margin = new Thickness(0,0,0,20);
            stockTypeWrapPanel.Children.Add(stockTypeLabel);
            stockTypeWrapPanel.Children.Add(stockTypeComboBox);

             var generatedSerialsStackPanel = new StackPanel();
             //generatedSerialsStackPanel.Visibility = Visibility.Collapsed;
             //generatedSerialsStackPanel.Height = 100;
             
             var generatedSerialsScrollViewer = new ScrollViewer();
             generatedSerialsScrollViewer.Content = generatedSerialsStackPanel;
             generatedSerialsScrollViewer.Height = 100;
             generatedSerialsScrollViewer.Visibility = Visibility.Collapsed;
             generatedSerialsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            itemsWrapPanel.Children.Add(border);
            itemsStackPanel.Children.Add(rfpcheckBox);
            itemsStackPanel.Children.Add(requestorWrapPanel);
            itemsStackPanel.Children.Add(rfpIdWrapPanel);
            itemsStackPanel.Children.Add(rfpItemDescriptionWrapPanel);
            itemsStackPanel.Children.Add(choiseWrapPanel);
            itemsStackPanel.Children.Add(companyCategoryWrapPanel);
            itemsStackPanel.Children.Add(productCategoryWrapPanel);
            itemsStackPanel.Children.Add(brandWrapPanel);
            itemsStackPanel.Children.Add(modelWrapPanel);
            itemsStackPanel.Children.Add(specsWrapPanel);
            itemsStackPanel.Children.Add(startSerialWrapPanel);
            itemsStackPanel.Children.Add(endSerialWrapPanel);
            itemsStackPanel.Children.Add(quantityWrapPanel);
            itemsStackPanel.Children.Add(priceWrapPanel);
            itemsStackPanel.Children.Add(currencyWrapPanel);
            itemsStackPanel.Children.Add(stockTypeWrapPanel);
            itemsStackPanel.Children.Add(generatedSerialsScrollViewer);
            if (itemsWrapPanel.Children.Count > 1 && viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            {
                RemoveItemButton(ref itemsStackPanel);
            }


        }
        private void addNewItemButton()
        {
            var addNewItemButton = new Button();
            addNewItemButton.Content = "ADD NEW ITEM";
            addNewItemButton.Width = 200;
            addNewItemButton.Style = (Style)FindResource("buttonBrowseStyle");
            addNewItemButton.Click += OnButtonClickAddNewCard;
            addNewItemButton.Margin = new Thickness(10);

            itemsWrapPanel.Children.Add(addNewItemButton);
        }
        private void RemoveItemButton(ref StackPanel itemsStackPanel)
        {
            var removeItemButton = new Button();
            removeItemButton.Content = "REMOVE";
            removeItemButton.Width = 100;
            removeItemButton.Style = (Style)FindResource("buttonBrowseStyle");
            removeItemButton.Click += OnButtonClickRemoveItem; 
            removeItemButton.Margin = new Thickness(0,40,0,0);
            removeItemButton.Background = Brushes.Red;

            itemsStackPanel.Children.Add(removeItemButton);
        }

      

        //private void OnTextChangedQuantityTextBox(object sender, TextChangedEventArgs e)
        //{


        //   TextBox quantityTextBox=sender as TextBox;

        //   WrapPanel quantityPanel= quantityTextBox.Parent as WrapPanel;

        //   Grid card= quantityPanel.Parent as Grid;

        //   WrapPanel itemsPanel=  card.Children[3] as WrapPanel;


        //    WrapPanel rfpPanel = card.Children[1] as WrapPanel;

        //    CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;



        //    ComboBox itemsComboBox = itemsPanel.Children[1] as ComboBox;

        //    if (quantityTextBox.Text == "")
        //        return;

        //    if (quantityTextBox.Text == "0") 
        //    {
        //        System.Windows.Forms.MessageBox.Show("quantity cannot be 0 !", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //        quantityTextBox.Text = "";
        //        quantityTextBox.Focusable = true;
        //        return;
        //    }
                

        //       bool isLetter  =  quantityTextBox.Text.ToList().Exists(a=>char.IsLetter(a));

        //    if (isLetter == true) {
        //        quantityTextBox.Text = "";

        //        return;
        //    }

        //    if (int.Parse(quantityTextBox.Text) > Convert.ToInt32(itemsComboBox.Tag)&& rfpCheckBox.IsChecked==true) {


        //        System.Windows.Forms.MessageBox.Show("quantity cannot be more than rfp items quantity!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

        //        quantityTextBox.Text = "";
        //        return;
            
        //    }
        //}

       

        /// <summary>
        /// /////////////////////////////////////////// ON CHECK/UNCHECK ///////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnCheckRFPCheckBox(object sender, RoutedEventArgs e)
        {

            CheckBox rfpCheckBox = sender as CheckBox;
            StackPanel itemsStackPanel = rfpCheckBox.Parent as StackPanel;
            WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            WrapPanel rfpItemDescription = itemsStackPanel.Children[3] as WrapPanel;

            rfpRequestorTeamWrapPanel.Visibility = Visibility.Collapsed;
            rfpIdWrapPanel.Visibility = Visibility.Collapsed;
            rfpItemDescription.Visibility = Visibility.Collapsed;

            ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;
            ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
            ComboBox rfpItemDescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

            rfpRequestorTeamComboBox.Items.Clear();
            rfpRequestorTeamComboBox.SelectedIndex = -1;

            rfpIdComboBox.Items.Clear();
            rfpIdComboBox.SelectedIndex = -1;
            rfpIdComboBox.IsEnabled = false;

            rfpItemDescriptionComboBox.Items.Clear();
            rfpItemDescriptionComboBox.SelectedIndex = -1;
            rfpItemDescription.IsEnabled = false;


            //  //CREATE A NEW FUNCTION TO DISABLE THE GUI AND CALL IT HERE

            //  CheckBox rfpCheckBox = sender as CheckBox;

            //  WrapPanel rfpPanel = rfpCheckBox.Parent as WrapPanel;


            //  Grid card = rfpPanel.Parent as Grid;
            //  WrapPanel requestorPanel = card.Children[2] as WrapPanel;

            //  ScrollViewer scroll = card.Children[22] as ScrollViewer;
            //  Grid serialsGrid= scroll.Content as Grid;

            //  serialsGrid.Children.Clear();


            //  ComboBox requstorComboBox = requestorPanel.Children[1] as ComboBox;

            //  requstorComboBox.IsEnabled = false;
            //  requstorComboBox.SelectedIndex = -1;


            //ComboBox serialComboBox=requestorPanel.Children[2] as ComboBox;


            //  serialComboBox.IsEnabled = false;
            //  serialComboBox.SelectedIndex = -1;

            //  WrapPanel itemDescriptionPanel = card.Children[3] as WrapPanel;

            //  WrapPanel choicePanel = card.Children[4] as WrapPanel;


            //  ComboBox itemDescriptionComboBox = itemDescriptionPanel.Children[1] as ComboBox;

            //  ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;


            //  choiceComboBox.IsEnabled = true;
            //  itemDescriptionComboBox.IsEnabled = false;
            //  itemDescriptionComboBox.SelectedIndex = -1;


        }

        private void OnCheckRFPCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox rfpCheckBox = sender as CheckBox;
            StackPanel itemsStackPanel = rfpCheckBox.Parent as StackPanel;
            WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            WrapPanel rfpItemDescription = itemsStackPanel.Children[3] as WrapPanel;

            rfpRequestorTeamWrapPanel.Visibility = Visibility.Visible;
            rfpIdWrapPanel.Visibility = Visibility.Visible;
            rfpItemDescription.Visibility = Visibility.Visible;

            ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;
            ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
            ComboBox rfpItemDescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

            rfpRequestorTeamComboBox.IsEnabled = true;
            for(int i =0;i<requestors.Count;i++)
            {
                ComboBoxItem requestorTeamItem = new ComboBoxItem();
                
                requestorTeamItem.Content = requestors[i].requestor_team.team_name;
                requestorTeamItem.Tag = requestors[i].requestor_team.team_id;
                if (rfpRequestorTeamComboBox.Items.Cast<ComboBoxItem>().Any(f => f.Tag.Equals(requestorTeamItem.Tag)))
                    continue;
                rfpRequestorTeamComboBox.Items.Add(requestorTeamItem);
            }

            //CheckBox rfpCheckBox=sender as CheckBox;

            //WrapPanel rfpPanel=rfpCheckBox.Parent as WrapPanel;


            //Grid card=rfpPanel.Parent as Grid;
            //WrapPanel requestorPanel=card.Children[2] as WrapPanel;

            //ComboBox requstorComboBox=requestorPanel.Children[1] as ComboBox;


            //  WrapPanel choicePanel = card.Children[4] as WrapPanel;
            // ComboBox choiceComboBox= choicePanel.Children[1] as ComboBox;


            //  WrapPanel GenericCategoryPanel = card.Children[5] as WrapPanel;
            //  ComboBox genericCategoryComboBox = GenericCategoryPanel.Children[1] as ComboBox;



            //  WrapPanel GenericProductPanel = card.Children[6] as WrapPanel;
            //  ComboBox genericProductComboBox = GenericProductPanel.Children[1] as ComboBox;



            //  WrapPanel genricBrandPanel = card.Children[7] as WrapPanel;
            //  ComboBox genericBrandComboBox = genricBrandPanel.Children[1] as ComboBox;


            //  WrapPanel genericModelPanel = card.Children[8] as WrapPanel;
            //  ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;





            //  WrapPanel companyCategoryPanel = card.Children[9] as WrapPanel;
            //  ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;



            //  WrapPanel companyProductPanel = card.Children[10] as WrapPanel;
            //  ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;



            //  WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;
            //  ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;


            //  WrapPanel companyModelPanel = card.Children[12] as WrapPanel;
            //  ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            //  requstorComboBox.IsEnabled = true;
            //  choiceComboBox.IsEnabled = false;
            //  choiceComboBox.SelectedIndex = -1;


            //  genericCategoryComboBox.IsEnabled = false;
            //  genericCategoryComboBox.SelectedIndex = -1;

            //  genericProductComboBox.IsEnabled = false;
            //  genericProductComboBox.Items.Clear();

            //  genericBrandComboBox.IsEnabled = false;
            //  genericBrandComboBox.Items.Clear();

            //  genericModelComboBox.IsEnabled = false;
            //  genericModelComboBox.Items.Clear();


            //  companyCategoryComboBox.IsEnabled = false;
            //  companyCategoryComboBox.SelectedIndex = -1;

            //  companyProductComboBox.IsEnabled = false;
            //  companyProductComboBox.Items.Clear();


            //  companyBrandComboBox.IsEnabled = false;
            //  companyBrandComboBox.Items.Clear();


            //  companyModelComboBox.IsEnabled = false;
            //  companyModelComboBox.Items.Clear();






        }
        /// <summary>
        /// ////////////////////////////////// ON SELECTION CHANGED /////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelChangedCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
           
            ComboBox categoryComboBox= sender as ComboBox;
            WrapPanel categoryWrapPanel = categoryComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = categoryWrapPanel.Parent as StackPanel;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;

            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;
            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

            typeComboBox.Items.Clear();
            brandComboBox.Items.Clear();
            modelComboBox.Items.Clear();
            specsComboBox.Items.Clear();

            genericProducts.Clear();
            companyProducts.Clear();
            if (categoryComboBox.SelectedIndex != -1)
            {
                if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 0)
                {
                    typeComboBox.IsEnabled = true;
                    brandComboBox.IsEnabled = false;
                    modelComboBox.IsEnabled = false;
                    if (!commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id))
                        return;

                    for (int i = 0; i < genericProducts.Count; i++)
                    {
                        ComboBoxItem product = new ComboBoxItem();
                        product.Content = genericProducts[i].product_name;
                        product.Tag = genericProducts[i].type_id;
                        typeComboBox.Items.Add(product);
                    }
                }
                else if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 1)
                {
                    typeComboBox.IsEnabled = true;
                    brandComboBox.IsEnabled = false;
                    modelComboBox.IsEnabled = false;
                    specsComboBox.IsEnabled = false;

                    if (!commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].category_id))
                        return;

                    PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT findOther = companyProducts.Find(f => f.type_id == 0);
                    if (findOther.product_name==null)
                    {
                        findOther.type_id = 0;
                        findOther.product_name = "Others";
                        companyProducts.Add(findOther);
                    }
                    for (int i = 0; i < companyProducts.Count; i++)
                    {
                        ComboBoxItem product = new ComboBoxItem();
                        product.Content = companyProducts[i].product_name;
                        product.Tag = companyProducts[i].type_id;
                        typeComboBox.Items.Add(product);
                    }
                   
                }
            }
           //WrapPanel categoryPanel= companyCategoryComboBox.Parent as WrapPanel;

           //Grid card= categoryPanel.Parent as Grid;

           //WrapPanel companyProductPanel= card.Children[10] as WrapPanel;

           //ComboBox companyProductComboBox=companyProductPanel.Children[1] as ComboBox;


           // WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;

           // ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;
             

           // WrapPanel companyModelPanel = card.Children[12] as WrapPanel;

           // ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;



           // WrapPanel companySpecsPanel = card.Children[13] as WrapPanel;

           // ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;



           // WrapPanel stockTypesPanel = card.Children[18] as WrapPanel;


           // companySpecsComboBox.Items.Clear();
           // companySpecsComboBox.IsEnabled = false;

        }
        private void OnSelChangedProductComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox typeComboBox = sender as ComboBox;
            WrapPanel typeWrapPanel = typeComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = typeWrapPanel.Parent as StackPanel;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;

            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;
            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;


            brandComboBox.Items.Clear();
            modelComboBox.Items.Clear();
            specsComboBox.Items.Clear();

            genericBrands.Clear();
            companyBrands.Clear();

            if (typeComboBox.SelectedIndex != -1)
            {
                if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 0)
                {

                    brandComboBox.IsEnabled = true;
                    modelComboBox.IsEnabled = false;
                    if (!commonQueries.GetGenericProductBrands(genericProducts[typeComboBox.SelectedIndex].type_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands))
                        return;

                    for (int i = 0; i < genericBrands.Count; i++)
                    {
                        ComboBoxItem brand = new ComboBoxItem();
                        brand.Content = genericBrands[i].brand_name;
                        brand.Tag = genericBrands[i].brand_id;
                        brandComboBox.Items.Add(brand);
                    }
                }
                else if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 1)
                {

                    brandComboBox.IsEnabled = true;
                    modelComboBox.IsEnabled = false;
                    specsComboBox.IsEnabled = false;

                    if (!commonQueries.GetProductBrands(companyProducts[typeComboBox.SelectedIndex].type_id, ref companyBrands))
                        return;

                    for (int i = 0; i < companyBrands.Count; i++)
                    {
                        ComboBoxItem brand = new ComboBoxItem();
                        brand.Content = companyBrands[i].brand_name;
                        brand.Tag = companyBrands[i].brand_id;
                        brandComboBox.Items.Add(brand);
                    }
                }
            }
        }
        private void OnSelChangedBrandComboBox(object sender, SelectionChangedEventArgs e)
        {

            ComboBox brandComboBox = sender as ComboBox;
            WrapPanel brandWrapPanel = brandComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = brandWrapPanel.Parent as StackPanel;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;

            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;
            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;



            modelComboBox.Items.Clear();
            specsComboBox.Items.Clear();

            genericModels.Clear();
            companyModels.Clear();

            if (brandComboBox.SelectedIndex != -1)
            {
                if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 0)
                {


                    modelComboBox.IsEnabled = true;
                    if (!commonQueries.GetGenericBrandModels(genericProducts[typeComboBox.SelectedIndex].type_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels))
                        return;

                    for (int i = 0; i < genericModels.Count; i++)
                    {
                        ComboBoxItem model = new ComboBoxItem();
                        model.Content = genericModels[i].model_name;
                        model.Tag = genericModels[i].model_id;
                        modelComboBox.Items.Add(model);
                    }
                }
                else if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 1)
                {


                    modelComboBox.IsEnabled = true;
                    specsComboBox.IsEnabled = false;

                    if (!commonQueries.GetCompanyModels(companyProducts[typeComboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels))
                        return;

                    for (int i = 0; i < companyModels.Count; i++)
                    {
                        ComboBoxItem model = new ComboBoxItem();
                        model.Content = companyModels[i].model_name;
                        model.Tag = companyModels[i].model_id;
                        modelComboBox.Items.Add(model);
                    }
                }
            }

        }
        private void OnSelChangedModelComboBox(object sender, SelectionChangedEventArgs e)
        {

            ComboBox modelComboBox = sender as ComboBox;
            WrapPanel modelWrapPanel = modelComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = modelWrapPanel.Parent as StackPanel;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;

            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;
            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;



          
            specsComboBox.Items.Clear();

            genericModels.Clear();
            specs.Clear();

            if (modelComboBox.SelectedIndex != -1)
            {
                if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 1)
                { 
                    specsComboBox.IsEnabled = true;
                    specsWrapPanel.Visibility = Visibility.Visible;
                    if (!commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id,companyProducts[typeComboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs))
                        return;

                    for (int i = 0; i < specs.Count; i++)
                    {
                        ComboBoxItem specss = new ComboBoxItem();
                        specss.Content = specs[i].spec_name;
                        specss.Tag = specs[i].spec_id;
                        specsComboBox.Items.Add(specss);
                    }
                }
            }

        }
        private void OnSelChangedChoiceComboBox(object sender, SelectionChangedEventArgs e)
        {


            ComboBox choiceComboBox = sender as ComboBox;
            WrapPanel choiceWrapPanel = choiceComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = choiceWrapPanel.Parent as StackPanel;

            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;

            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

            categoryComboBox.Items.Clear();
            companyCategories.Clear();
            genericCategories.Clear();

            if (choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex==0 )
            {
                if (!commonQueries.GetGenericProductCategories(ref genericCategories))
                    return;

                for(int i =0;i<genericCategories.Count;i++)
                {
                    ComboBoxItem category = new ComboBoxItem();
                    category.Content = genericCategories[i].category_name;
                    category.Tag = genericCategories[i].category_id;
                    categoryComboBox.Items.Add(category);
                }
            }
            else if(choiceComboBox.SelectedIndex != -1 && choiceComboBox.SelectedIndex == 1)
            {
                if (!commonQueries.GetProductCategories(ref companyCategories))
                    return;

                for (int i = 0; i < companyCategories.Count; i++)
                {
                    ComboBoxItem category = new ComboBoxItem();
                    category.Content = companyCategories[i].category_name;
                    category.Tag = companyCategories[i].category_id;
                    categoryComboBox.Items.Add(category);
                }
            }


        }
        private void OnSelChangedRfpItemDescriptionComboBox(object sender, SelectionChangedEventArgs e)
        {

            ComboBox rfpItemsComboBox = sender as ComboBox;

            if (rfpItemsComboBox.SelectedIndex == -1)
                return;

            WrapPanel rfpItemsPanel = rfpItemsComboBox.Parent as WrapPanel;


            Grid card = rfpItemsPanel.Parent as Grid;

            WrapPanel rfpRequsterPanel = card.Children[2] as WrapPanel;

            ComboBox serialComboBox = rfpRequsterPanel.Children[2] as ComboBox;

            WrapPanel startSerialPanel = card.Children[14] as WrapPanel;
            WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

            WrapPanel stockTypePanel = card.Children[19] as WrapPanel;

            WrapPanel validityDatePanel = card.Children[20] as WrapPanel;


            Button generateSerialsButton = card.Children[21] as Button;





            commonQueries.GetRfpItemsMapping(rfps[serialComboBox.SelectedIndex].rfpSerial, rfps[serialComboBox.SelectedIndex].rfpVersion, rfps[serialComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems);



            rfpItemsComboBox.Tag = rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity;

            if (rfpItems[rfpItemsComboBox.SelectedIndex].product_category.category_name != "")
            {


                if (rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == false)
                {

                    startSerialPanel.Visibility = Visibility.Collapsed;
                    endSerialPanel.Visibility = Visibility.Collapsed;

                    stockTypePanel.Visibility = Visibility.Visible;
                    validityDatePanel.Visibility = Visibility.Visible;

                    generateSerialsButton.Visibility = Visibility.Collapsed;

                }

                else
                {

                    startSerialPanel.Visibility = Visibility.Visible;
                    endSerialPanel.Visibility = Visibility.Visible;
                    stockTypePanel.Visibility = Visibility.Collapsed;

                    validityDatePanel.Visibility = Visibility.Collapsed;
                    generateSerialsButton.Visibility = Visibility.Visible;




                }

            }

            else
            {


                if (rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == false)
                {

                    startSerialPanel.Visibility = Visibility.Collapsed;
                    endSerialPanel.Visibility = Visibility.Collapsed;
                    stockTypePanel.Visibility = Visibility.Visible;
                    validityDatePanel.Visibility = Visibility.Visible;

                    generateSerialsButton.Visibility = Visibility.Collapsed;




                }


                else
                {

                    startSerialPanel.Visibility = Visibility.Visible;
                    endSerialPanel.Visibility = Visibility.Visible;

                    stockTypePanel.Visibility = Visibility.Collapsed;
                    validityDatePanel.Visibility = Visibility.Collapsed;

                    generateSerialsButton.Visibility = Visibility.Visible;




                }


            }


        }
        private void OnSelChangedRfpIdComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox rfpIdComboBox = sender as ComboBox;
            WrapPanel rfpIdWrapPanel = rfpIdComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = rfpIdWrapPanel.Parent as StackPanel;

            WrapPanel itemDescriptionWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
            ComboBox itemDescriptionComboBox = itemDescriptionWrapPanel.Children[1] as ComboBox;

            WrapPanel requestorTeamWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            ComboBox requestorTeamComboBox = requestorTeamWrapPanel.Children[1] as ComboBox;


            //ComboBox serialComboBox=sender as ComboBox;

            //WrapPanel serialPanel=serialComboBox.Parent as WrapPanel;

            //Grid card= serialPanel.Parent as Grid;

            //ComboBox requsterComboBox= serialPanel.Children[1] as ComboBox;

            rfps.Clear();

            if (rfpIdComboBox.SelectedIndex == -1)
                return;

            //SEPARATE GET DATA FUNCTIONS FROM GUI FUNCTIONS
            //COMMON QUERIES
            //IF NOT RETURN
            ComboBoxItem requestorTeam = requestorTeamComboBox.SelectedItem as ComboBoxItem;
            ComboBoxItem rfpId = rfpIdComboBox.SelectedItem as ComboBoxItem;
            PROCUREMENT_STRUCTS.RFP_MIN_STRUCT rfpItem = (RFP_MIN_STRUCT)rfpId.Tag;
            if (!commonQueries.GetTeamRFPs(ref rfps, Int32.Parse(requestorTeam.Tag.ToString())))
                return;

            rfpItems.Clear();

            if (!commonQueries.GetRfpItemsMapping(rfpItem.rfpSerial, rfpItem.rfpVersion, rfpItem.rfpRequestorTeam, ref rfpItems))
                return;


            for (int i = 0; i < rfpItems.Count; i++)
            {

                if (rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED)
                {

                    rfpItems.RemoveAt(i);
                    i--;
                }
            }


            //WrapPanel descriptionPanel= card.Children[3] as WrapPanel;

            //ComboBox itemDescription= descriptionPanel.Children[1] as ComboBox;

            itemDescriptionComboBox.Items.Clear();
            itemDescriptionComboBox.IsEnabled = true;



            if (rfpItems.Count == 0)
            {

                System.Windows.Forms.MessageBox.Show("RFP Items are not mapped!.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                return;
            }

            for (int i = 0; i < rfpItems.Count; i++)
            {


                if (rfpItems[i].product_category.category_id == 0)
                {
                    itemDescriptionComboBox.Items.Add(rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);


                }

                else
                {


                    itemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);



                }

            }


        }
        private void OnSelChangedRfpRequestorTeamComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox requestorComboBox = sender as ComboBox;
            WrapPanel requestorTeamWrapPanel = requestorComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = requestorTeamWrapPanel.Parent as StackPanel;
            WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            WrapPanel rfpItemDescription = itemsStackPanel.Children[3] as WrapPanel;


            ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
            ComboBox rfpItemDescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

            if (requestorComboBox.SelectedIndex != -1)
            {
                ComboBoxItem requestorTeamItem = requestorComboBox.SelectedItem as ComboBoxItem;
                rfps.Clear();
                rfpIdComboBox.Items.Clear();
                rfpItemDescriptionComboBox.Items.Clear();
                rfpIdComboBox.IsEnabled = true;
                if (!commonQueries.GetTeamRFPs(ref rfps, Int32.Parse(requestorTeamItem.Tag.ToString())))
                    return;
                for (int i = 0; i < rfps.Count; i++)
                {
                    ComboBoxItem rfpId = new ComboBoxItem();
                    rfpId.Content = rfps[i].rfpID;
                    rfpId.Tag = rfps[i];
                    rfpIdComboBox.Items.Add(rfpId);
                }
            }

            //  ComboBox rfpRequesterComboBoxBox = sender as ComboBox;

            //  WrapPanel rfpRequsetorPanel = rfpRequesterComboBoxBox.Parent as WrapPanel;


            //  Grid card = rfpRequsetorPanel.Parent as Grid;

            //ComboBox serialComboBox= rfpRequsetorPanel.Children[2] as ComboBox;

            //  if (rfpRequesterComboBoxBox.SelectedIndex == -1)
            //      return;
            //  rfps.Clear();
            //  serialComboBox.Items.Clear();
            //  commonQueries.GetTeamRFPs(ref rfps, requestors[rfpRequesterComboBoxBox.SelectedIndex].requestor_team.team_id);

            //  rfps.ForEach(a => serialComboBox.Items.Add(a.rfpID));
            //  serialComboBox.IsEnabled = true;

        }
        /// <summary>
        /// ////////////////////////////////////// ON BUTTON CLICK //////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClickGenenrateSerials(object sender, RoutedEventArgs e)
        {
            Button generateSerialsButton = sender as Button;
            WrapPanel endSerialWrapPanel = generateSerialsButton.Parent as WrapPanel;
            StackPanel itemsStackPanel = endSerialWrapPanel.Parent as StackPanel;

            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;

            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
            TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;

            CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;

           // Grid card= generateButton.Parent as Grid;


           // WrapPanel rfpPanel = card.Children[1] as WrapPanel;

           //CheckBox rfpCheckBox= rfpPanel.Children[1] as CheckBox;

           // WrapPanel startSerialWrapPanel=card.Children[14] as WrapPanel;


           //     TextBox startSerialTextBox=startSerialWrapPanel.Children[1] as TextBox;

           // WrapPanel endSerialWrapPanel = card.Children[15] as WrapPanel;


           //     TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;


           // WrapPanel quantityWrapPanel = card.Children[16] as WrapPanel;


           // TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;



            if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
            {

                System.Windows.Forms.MessageBox.Show("Start Serial and end Serial cannot be null", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


            }

            else if (startSerialTextBox.Text != "" && endSerialTextBox.Text != "" && quantityTextBox.Text != "")
            {

                List<int> count = new List<int>();
                string number = "";
                string number2 = "";


                if (startSerialTextBox.Text.Length != endSerialTextBox.Text.Length)
                {

                    System.Windows.Forms.MessageBox.Show("The Size of the Start Serial and End Serial doesnt Match", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                for (int i = 0; i < startSerialTextBox.Text.Length; i++)
                {

                    if (char.IsDigit(startSerialTextBox.Text[i]) == true)
                    {

                        count.Add(i);


                    }

                }


                for (int i = 0; i < startSerialTextBox.Text.Length; i++)
                {

                    if (char.IsDigit(startSerialTextBox.Text[i]) == true)
                        number += startSerialTextBox.Text[i];



                }


                for (int i = 0; i < endSerialTextBox.Text.Length; i++)
                {

                    if (char.IsDigit(endSerialTextBox.Text[i]) == true)
                        number2 += endSerialTextBox.Text[i];



                }


                for (int i = 0; i < startSerialTextBox.Text.Length; i++)
                {

                    if (char.IsLetter(startSerialTextBox.Text[i]))
                    {

                        if (endSerialTextBox.Text[i] != startSerialTextBox.Text[i])
                        {

                            System.Windows.Forms.MessageBox.Show("The Start Serial and end Serial doesnt match!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;

                        }

                    }
                }


                if (int.Parse(quantityTextBox.Text) != (int.Parse(number2) - int.Parse(number)) + 1)
                {

                    System.Windows.Forms.MessageBox.Show("The quantity dosent match the range!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                //ScrollViewer serialsScroll = card.Children[22] as ScrollViewer;
                //Grid serialsGrid = serialsScroll.Content as Grid;

                //serialsGrid.RowDefinitions.Clear();
                //serialsGrid.ColumnDefinitions.Clear();
                //serialsGrid.Children.Clear();

                ScrollViewer serialsGeneratedScrollViewer = itemsStackPanel.Children[16] as ScrollViewer;
                serialsGeneratedScrollViewer.Visibility = Visibility.Visible;
                StackPanel generatedSerialsStackPanel = serialsGeneratedScrollViewer.Content as StackPanel;
                generatedSerialsStackPanel.Children.Clear();


                for (int i = 0; i < (int.Parse(number2) - int.Parse(number)) + 1; i++)
                {

                    WrapPanel wrapPanel = new WrapPanel();

                
                    Label serialNumber = new Label();
                    serialNumber.Style = (Style)FindResource("labelStyle");
                    serialNumber.Width = 140;
                    serialNumber.Margin = new Thickness(0);
                    serialNumber.Content = $"Serial {i + 1}";
                    serialNumber.HorizontalContentAlignment = HorizontalAlignment.Left;

                    TextBox textSerial = new TextBox();
                    textSerial.Style =  (Style)FindResource("textBoxStyle");
                    textSerial.Margin = new Thickness(0,0,10,0);
                    textSerial.Width = 110;







                    //TextBox productCodeTextBox = new TextBox();
                    //productCodeTextBox.Style = (Style)FindResource("miniTextBoxStyle");

                    //productCodeTextBox.Text = "Product Code";

                    //productCodeTextBox.Foreground = Brushes.Gray;

                    //productCodeTextBox.MouseEnter += OnProductCodeTextBoxMouseEnter;
                    //productCodeTextBox.MouseLeave += OnProductCodeTextBoxMouseLeave;



                    ComboBox stockTypeCombobox = new ComboBox();
                    stockTypeCombobox.Style = (Style)FindResource("comboBoxStyleCard2");
                    stockTypeCombobox.Width = 120;
                    stockTypeCombobox.Margin = new Thickness(0);
                    stockTypes.ForEach(a => stockTypeCombobox.Items.Add(a.stock_type_name));
                    



                    bool foundnum = false;

                    for (int j = 0; j < startSerialTextBox.Text.Length; j++)
                    {

                        if (foundnum == false)
                        {

                            if (count.Exists(a => a == j))
                            {

                                textSerial.Text += int.Parse(number) + i;
                                foundnum = true;
                            }

                            else
                            {

                                textSerial.Text += startSerialTextBox.Text[j];

                            }

                        }

                        else
                        {
                            if (j != startSerialTextBox.Text.Length)
                            {
                                if (char.IsLetter(startSerialTextBox.Text[j]))
                                    textSerial.Text += startSerialTextBox.Text[j];

                            }
                        }



                    }

                    wrapPanel.Children.Add(serialNumber);
                    wrapPanel.Children.Add(textSerial);
                    wrapPanel.Children.Add(stockTypeCombobox);


                    if (rfpCheckBox.IsChecked == true) {

                        DatePicker vailidityDatePicker = new DatePicker();


                        vailidityDatePicker.Style = (Style)FindResource("minidatePickerStyle");


                        DateTime returnDateTime = new DateTime();
                        commonQueries.GetTodaysDate(ref returnDateTime);

                        vailidityDatePicker.SelectedDate = returnDateTime;
                        wrapPanel.Children.Add(vailidityDatePicker);


                    }


                    generatedSerialsStackPanel.Children.Add(wrapPanel);
                    serialsGeneratedScrollViewer.Visibility = Visibility.Visible;

                }

            }


            else if (startSerialTextBox.Text != "" && quantityTextBox.Text == "1" && endSerialTextBox.Text == "") {

                ScrollViewer serialsGeneratedScrollViewer = itemsStackPanel.Children[16] as ScrollViewer;
                StackPanel generatedSerialsStackPanel = serialsGeneratedScrollViewer.Content as StackPanel;
                generatedSerialsStackPanel.Children.Clear();
                //serialsGrid.RowDefinitions.Clear();
                //serialsGrid.ColumnDefinitions.Clear();
                //serialsGrid.Children.Clear();


                WrapPanel wrapPanel = new WrapPanel();

                Label serialNumber = new Label();
                serialNumber.Style = (Style)FindResource("labelStyle");
                serialNumber.Width = 140;
                serialNumber.Margin = new Thickness(0);
                serialNumber.Content = $"Serial 1";


                TextBox textSerial = new TextBox();

                textSerial.Style = (Style)FindResource("textBoxStyle");
                textSerial.Margin = new Thickness(0,0,10,0);
                textSerial.Width = 110;
                //serialsGrid.RowDefinitions.Add(new RowDefinition());



                //textSerial.Text = startSerialTextBox.Text;


                //Grid.SetRow(wrapPanel, serialsGrid.RowDefinitions.Count - 1);


                wrapPanel.Children.Add(serialNumber);
                wrapPanel.Children.Add(textSerial);

                generatedSerialsStackPanel.Children.Add(wrapPanel);
                serialsGeneratedScrollViewer.Visibility = Visibility.Visible;

            }
            





        }
        private void OnButtonClickCancel(object sender, RoutedEventArgs e)
        {
            entryPermitWindow.Close();
        }
        private void OnButtonClickFinish(object sender, RoutedEventArgs e)
        {

            if (addEntryPermitPage.WareHouseCombo.SelectedIndex == -1 || addEntryPermitPage.TransactionDatePicker.Text == ""|| addEntryPermitPage.entryPermitIdTextBox.Text=="") {

                System.Windows.Forms.MessageBox.Show("WareHouse or transaction date or entryPermitId is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            addEntryPermitPage.materialEntryPermit.GetItems().Clear();
            materialReservations.Clear();


            int rfpItemCount = 1;

            for (int i = 0; i < itemsWrapPanel.Children.Count-1; i++)
            {

                Border itemBorder = itemsWrapPanel.Children[i] as Border;
                StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                Label header = mainStackPanel.Children[0] as Label;

                ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;


                CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;
                //rfpCheckBox.IsChecked = true;


                WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;



                WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                rfpIdWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;



                WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[1] as ComboBox;


                WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;


                WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;



                WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;



                WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;



                WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;



                WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;



                WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;



                Button generateSerials = endSerialWrapPanel.Children[2] as Button;


                WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;



                WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;



                WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;


                WrapPanel stockWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                ComboBox stockComboBox = stockWrapPanel.Children[1] as ComboBox;

                ScrollViewer generatedSerialsScrollViewer = itemsStackPanel.Children[16] as ScrollViewer;
                StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;
                //if the checkbox is unchecked
                INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                materialEntryItem.entry_permit_item_serial = i + 1;
                if (rfpCheckBox.IsChecked == false)
                {

                    //if the choice is generic
                    if (choiceComboBox.SelectedIndex == 0)
                    {


                        if (modelComboBox.SelectedIndex == -1)
                        {

                            System.Windows.Forms.MessageBox.Show("You have to choose a model", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                            return;
                        }

                        else
                        {

                            if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
                            {



                                // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                                materialEntryItem.product_category.category_id = genericCategories[categoryComboBox.SelectedIndex].category_id;

                                genericProducts.Clear();

                                commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id);

                                materialEntryItem.product_type.type_id = genericProducts[typeComboBox.SelectedIndex].type_id;

                                genericBrands.Clear();

                                commonQueries.GetGenericProductBrands(genericProducts[typeComboBox.SelectedIndex].type_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands);

                                materialEntryItem.product_brand.brand_id = genericBrands[brandComboBox.SelectedIndex].brand_id;

                                genericModels.Clear();

                                commonQueries.GetGenericBrandModels(genericProducts[typeComboBox.SelectedIndex].type_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels);


                                materialEntryItem.product_model.model_id = genericModels[modelComboBox.SelectedIndex].model_id;

                                materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                if(currencyComboBox.SelectedIndex !=-1)
                                materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return;
                                }

                                materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                                commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typeComboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                                materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;



                                //if (stockTypeComboBoxMain.SelectedIndex == -1)
                                //{

                                //System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                //return;

                                //}

                                //materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_id;
                                //materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_name;
                                //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;




                                addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                            }

                            else
                            {
                                int count = 1;

                                for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                                {


                                    //  INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                                    materialEntryItem.product_category.category_id = genericCategories[categoryComboBox.SelectedIndex].category_id;

                                    genericProducts.Clear();

                                    commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id);

                                    materialEntryItem.product_type.type_id = genericProducts[typeComboBox.SelectedIndex].type_id;

                                    genericBrands.Clear();

                                    commonQueries.GetGenericProductBrands(genericProducts[typeComboBox.SelectedIndex].type_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands);

                                    materialEntryItem.product_brand.brand_id = genericBrands[brandComboBox.SelectedIndex].brand_id;

                                    genericModels.Clear();

                                    commonQueries.GetGenericBrandModels(genericProducts[typeComboBox.SelectedIndex].type_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels);


                                    materialEntryItem.product_model.model_id = genericModels[modelComboBox.SelectedIndex].model_id;
                                    materialEntryItem.quantity = 0;

                                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                    if (currencyComboBox.SelectedIndex != -1)
                                    {
                                        materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                                    }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;
                                    }

                                    commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typeComboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                                    materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                    materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;




                                    materialEntryItem.entry_permit_item_serial = count;

                                    count++;

                                    WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                                    TextBox serialText = serialPanel.Children[0] as TextBox;
                                    ComboBox stockTypeComboBox = serialPanel.Children[1] as ComboBox;

                                    if (stockTypeComboBox.SelectedIndex == -1)
                                    {

                                        System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;

                                    }

                                    materialEntryItem.product_serial_number = serialText.Text;



                                    materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                                    materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                                    //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;



                                    addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                                }




                            }



                        }


                    }

                    //if the choice is company

                    else
                    {

                        if (specsComboBox.SelectedIndex == -1)
                        {

                            System.Windows.Forms.MessageBox.Show("You have to choose a spec", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;
                        }


                        else
                        {

                            if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
                            {

                                ///INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();


                                materialEntryItem.is_company_product = true;
                                materialEntryItem.product_category.category_id = companyCategories[categoryComboBox.SelectedIndex].category_id;
                                companyProducts.Clear();

                                commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].category_id);

                                materialEntryItem.product_type.type_id = companyProducts[typeComboBox.SelectedIndex].type_id;

                                companyBrands.Clear();

                                commonQueries.GetProductBrands(companyProducts[typeComboBox.SelectedIndex].type_id, ref companyBrands);

                                materialEntryItem.product_brand.brand_id = companyBrands[brandComboBox.SelectedIndex].brand_id;

                                companyModels.Clear();

                                commonQueries.GetCompanyModels(companyProducts[typeComboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels);


                                materialEntryItem.product_model.model_id = companyModels[modelComboBox.SelectedIndex].model_id;

                                specs.Clear();


                                if (priceTextBox.Text != "")
                                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);
                                if(currencyComboBox.SelectedIndex != -1)
                                materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return;
                                }

                                materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                                commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typeComboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                                materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                                if (stockComboBox.SelectedIndex == -1)
                                {

                                    System.Windows.Forms.MessageBox.Show("You have to choose the stock Type ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return;

                                }

                                if(stockComboBox.SelectedIndex !=-1)
                                {
                                    materialEntryItem.stock_type.stock_type_id = stockTypes[stockComboBox.SelectedIndex].stock_type_id;
                                    materialEntryItem.stock_type.stock_type_name = stockTypes[stockComboBox.SelectedIndex].stock_type_name;
                                }
                                
                                //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;


                                addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                            }

                            else
                            {
                                int count = 1;

                                for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                                {

                                    // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                                    materialEntryItem.is_company_product = true;
                                    materialEntryItem.product_category.category_id = companyCategories[categoryComboBox.SelectedIndex].category_id;

                                    companyProducts.Clear();

                                    commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].category_id);

                                    materialEntryItem.product_type.type_id = companyProducts[typeComboBox.SelectedIndex].type_id;

                                    companyBrands.Clear();

                                    commonQueries.GetProductBrands(companyProducts[typeComboBox.SelectedIndex].type_id, ref companyBrands);

                                    materialEntryItem.product_brand.brand_id = companyBrands[brandComboBox.SelectedIndex].brand_id;

                                    companyModels.Clear();

                                    commonQueries.GetCompanyModels(companyProducts[typeComboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels);


                                    materialEntryItem.product_model.model_id = companyModels[modelComboBox.SelectedIndex].model_id;

                                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                    if(currencyComboBox.SelectedIndex != -1)
                                    materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;
                                    }

                                    commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typeComboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                                    materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                    materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                                    materialEntryItem.quantity = 0;



                                    materialEntryItem.entry_permit_item_serial = count;

                                    count++;

                                    WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                                    TextBox serialText = serialPanel.Children[0] as TextBox;
                                    ComboBox stockTypeComBoBox = serialPanel.Children[1] as ComboBox;

                                    if (stockTypeComBoBox.SelectedIndex == -1)
                                    {

                                        System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;

                                    }
                                     if(stockComboBox.SelectedIndex != -1)
                                    {
                                        materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_id;
                                        materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_name;
                                    }
                                    
                                    //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                                    materialEntryItem.product_serial_number = serialText.Text;

                                    addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                                }




                            }


                        }



                    }


                }

                //if the checkbox is checked

                else
                {


                    //////////////////////////////////////////////////
                    //to implement rfp items after the mapping is done
                    //////////////////////////////////////////////////

                    if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "" && rfpCheckBox.IsChecked==true)
                    {

                        // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                        materialEntryItem.rfp_info.rfpRequestorTeam = requestors[rfpRequestorsComboBox.SelectedIndex].requestor_team.team_id;

                        rfps.Clear();

                        commonQueries.GetTeamRFPs(ref rfps, requestors[rfpRequestorsComboBox.SelectedIndex].requestor_team.team_id);

                        materialEntryItem.rfp_info.rfpSerial = rfps[rfpIdComboBox.SelectedIndex].rfpSerial;
                        materialEntryItem.rfp_info.rfpVersion = rfps[rfpIdComboBox.SelectedIndex].rfpVersion;

                        rfpItems.Clear();

                        commonQueries.GetRfpItemsMapping(rfps[rfpIdComboBox.SelectedIndex].rfpSerial, rfps[rfpIdComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                        materialEntryItem.rfp_item_number = rfpItems[rfpDescriptionComboBox.SelectedIndex].rfp_item_number;
                        materialEntryItem.entry_permit_item_serial = rfpItemCount;


                        if (priceTextBox.Text != "")
                            materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                        if(currencyComboBox.SelectedIndex != -1)
                        materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;
                        }

                        materialEntryItem.quantity = int.Parse(quantityTextBox.Text);

                        INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT materialReservation = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();


                        //materialReservation.hold_until = validityDate.DisplayDate;

                        materialReservation.reserved_by_id = loggedInUser.GetEmployeeId();

                        materialReservation.rfp_serial = materialEntryItem.rfp_info.rfpSerial;

                        materialReservation.rfp_version = materialEntryItem.rfp_info.rfpVersion;

                        materialReservation.rfp_item_no = materialEntryItem.rfp_item_number;

                        materialReservation.rfp_requestor_team = materialEntryItem.rfp_info.rfpRequestorTeam;

                        materialReservation.quantity = materialEntryItem.quantity;

                        materialReservation.entry_permit_item_serial = rfpItemCount;

                        materialReservations.Add(materialReservation);


                        if (rfpItems[rfpDescriptionComboBox.SelectedIndex].is_company_product == true)
                        {


                            materialEntryItem.product_category.category_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_category.category_id;
                            materialEntryItem.product_type.type_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_type.type_id;
                            materialEntryItem.product_brand.brand_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_brand.brand_id;
                            materialEntryItem.product_model.model_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_model.model_id;
                            materialEntryItem.product_specs.spec_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_specs.spec_id;


                        }

                        else
                        {

                            materialEntryItem.product_category.category_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_category.category_id;
                            materialEntryItem.product_type.type_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_type.type_id;
                            materialEntryItem.product_brand.brand_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_brand.brand_id;
                            materialEntryItem.product_model.model_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_model.model_id;


                        }


                        if (stockComboBox.SelectedIndex == -1)
                        {

                            System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;

                        }

                        if(stockComboBox.SelectedIndex !=-1)
                        {
                            materialEntryItem.stock_type.stock_type_id = stockTypes[stockComboBox.SelectedIndex].stock_type_id;
                            materialEntryItem.stock_type.stock_type_name = stockTypes[stockComboBox.SelectedIndex].stock_type_name;
                        }
                       


                        addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                        rfpItemCount++;


                    }

                    else
                    {

                        for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                        {


                            // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                            materialEntryItem.rfp_info.rfpRequestorTeam = requestors[rfpRequestorsComboBox.SelectedIndex].requestor_team.team_id;

                            rfps.Clear();

                            commonQueries.GetTeamRFPs(ref rfps, requestors[rfpRequestorsComboBox.SelectedIndex].requestor_team.team_id);

                            materialEntryItem.rfp_info.rfpSerial = rfps[rfpIdComboBox.SelectedIndex].rfpSerial;
                            materialEntryItem.rfp_info.rfpVersion = rfps[rfpIdComboBox.SelectedIndex].rfpVersion;

                            rfpItems.Clear();

                            commonQueries.GetRfpItemsMapping(rfps[rfpIdComboBox.SelectedIndex].rfpSerial, rfps[rfpIdComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                            materialEntryItem.rfp_item_number = rfpItems[rfpDescriptionComboBox.SelectedIndex].rfp_item_number;


                            if (priceTextBox.Text != "")
                                materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                            if(currencyComboBox.SelectedIndex !=-1)
                            materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return;
                            }
                            materialEntryItem.quantity = 1;



                            materialEntryItem.entry_permit_item_serial = rfpItemCount;


                            INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT materialReservation = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();


                            //  materialReservation.hold_until = validityDate.DisplayDate;

                            materialReservation.reserved_by_id = loggedInUser.GetEmployeeId();

                            materialReservation.rfp_serial = materialEntryItem.rfp_info.rfpSerial;

                            materialReservation.rfp_version = materialEntryItem.rfp_info.rfpVersion;

                            materialReservation.rfp_item_no = materialEntryItem.rfp_item_number;

                            materialReservation.rfp_requestor_team = materialEntryItem.rfp_info.rfpRequestorTeam;

                            materialReservation.quantity = materialEntryItem.quantity;

                            materialReservation.entry_permit_item_serial = rfpItemCount;

                            materialReservations.Add(materialReservation);

                            rfpItemCount++;


                            WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                            TextBox serialText = serialPanel.Children[0] as TextBox;

                            ComboBox stockTypeComBoBox = serialPanel.Children[1] as ComboBox;

                            if (stockTypeComBoBox.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return;

                            }

                            materialEntryItem.product_serial_number = serialText.Text;


                            materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_id;
                            materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_name;
                            //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                            if (rfpItems[rfpDescriptionComboBox.SelectedIndex].product_category.category_name == "")
                            {
                                materialEntryItem.product_category.category_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_category.category_id;
                                materialEntryItem.product_type.type_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_type.type_id;
                                materialEntryItem.product_brand.brand_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_brand.brand_id;
                                materialEntryItem.product_model.model_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_model.model_id;
                                materialEntryItem.product_specs.spec_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_specs.spec_id;



                            }

                            else
                            {

                                materialEntryItem.product_category.category_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_category.category_id;
                                materialEntryItem.product_type.type_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_type.type_id;
                                materialEntryItem.product_brand.brand_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_brand.brand_id;
                                materialEntryItem.product_model.model_id = rfpItems[rfpDescriptionComboBox.SelectedIndex].product_model.model_id;


                            }


                            addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                        }


                    }

                }

            }


                if (viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION)
                {
                    if (!addEntryPermitPage.materialEntryPermit.IssueNewEntryPermit())
                        return;


                    if (materialReservations.Count != 0)
                    {

                        for (int i = 0; i < materialReservations.Count; i++)
                        {
                            INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservationItem = materialReservations[i];
                            reservationItem.entry_permit_serial = addEntryPermitPage.materialEntryPermit.GetEntryPermitSerial();

                            materialReservations[i] = reservationItem;
                        }

                        MaterialReservation reservation = new MaterialReservation();
                        reservation.SetReservationDate(Convert.ToDateTime(addEntryPermitPage.TransactionDatePicker.SelectedDate));

                        reservation.SetReservedBy(loggedInUser.GetEmployeeId());
                        reservation.SetRfpSerial(materialReservations[0].rfp_serial);

                        reservation.SetReservationStatusId(COMPANY_WORK_MACROS.PENDING_RESERVATION);


                        if (!reservation.IssueMultipleReservations(materialReservations))
                            return;
                    }

                }

                else

                {
                    addEntryPermitPage.materialEntryPermit.UpdateMaterialEntryPermit(oldMaterialEntryPermit);
                }

                addEntryPermitPage.WareHouseCombo.IsEnabled = false;
                addEntryPermitPage.TransactionDatePicker.IsEnabled = false;

            for (int i = 0; i < itemsWrapPanel.Children.Count-1; i++)
            {

                Border itemBorder = itemsWrapPanel.Children[i] as Border;
                StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                Label header = mainStackPanel.Children[0] as Label;
                ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;
                rfpCheckBox.IsEnabled = false;

                WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                rfpRequestorsComboBox.IsEnabled = false;

                WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                rfpIdWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                rfpIdComboBox.IsEnabled = false;

                WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[1] as ComboBox;
                rfpDescriptionComboBox.IsEnabled = true;



                WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;


                WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                categoryComboBox.IsEnabled = false;

                WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                typeComboBox.IsEnabled = false;

                WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                brandComboBox.IsEnabled = false;

                WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                modelComboBox.IsEnabled = false;

                WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                specsComboBox.IsEnabled = false;
                

                WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                startSerialTextBox.IsEnabled = false;

                WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                endSerialTextBox.IsEnabled = false;

                Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                generateSerials.IsEnabled = false;

                WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                quantityTextBox.IsEnabled = false;

                WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                priceTextBox.IsEnabled = false;

                WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                currencyComboBox.IsEnabled = false;

                WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                stockTypeComboBox.IsEnabled = false;
            }


                EntryPermitUploadFilesPage = new EntryPermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, addEntryPermitPage, entryPermitWindow, ref addEntryPermitPage.materialEntryPermit);


                this.NavigationService.Navigate(EntryPermitUploadFilesPage);

                this.nextButton.IsEnabled = true;
                this.finishButton.IsEnabled = false;

            
        }
        private void OnButtonClickAddNewCard (object sender, RoutedEventArgs e)
        {
            itemsWrapPanel.Children.RemoveAt(itemsWrapPanel.Children.Count - 1);
            InitializeNewCard();
            addNewItemButton();
        }
        private void OnButtonClickBack(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(addEntryPermitPage);


        }
        private void OnButtonClickNext(object sender, RoutedEventArgs e)
        {
            if (EntryPermitUploadFilesPage != null)
                this.NavigationService.Navigate(EntryPermitUploadFilesPage);
        }
        private void OnButtonClickRemoveItem(object sender, RoutedEventArgs e)
        {
            Button removeButton = sender as Button;
            StackPanel itemsStackPanel = removeButton.Parent as StackPanel;
            ScrollViewer itemsScrollViewer = itemsStackPanel.Parent as ScrollViewer;
            StackPanel mainStackPanel = itemsScrollViewer.Parent as StackPanel;
            Border mainBorder = mainStackPanel.Parent as Border;
            itemsWrapPanel.Children.Remove(mainBorder);

            for (int i = 0; i < itemsWrapPanel.Children.Count - 1; i++)
            {
                Border itemsBorder = itemsWrapPanel.Children[i] as Border;
                StackPanel maxItemsStackPanel = itemsBorder.Child as StackPanel;
                Label header = maxItemsStackPanel.Children[0] as Label;
                header.Content = $"Item {i + 1}";
            }
        }
        /// <summary>
        /// ///////////////////////////////////////////// ON MOUSE LEFT BUTTON DOWN /////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDownFileLabel(object sender, MouseButtonEventArgs e)
        {
            if (EntryPermitUploadFilesPage != null)
                this.NavigationService.Navigate(EntryPermitUploadFilesPage);

        }
        private void OnMouseLeftButtonDownBasicInfoLabel(object sender, MouseButtonEventArgs e)
        {


            this.NavigationService.Navigate(addEntryPermitPage);
        }
        /// <summary>
        /// ////////////////////////////////////////////// GET / FILL FUNCTIONS////////////////////////////
        /// </summary>
        private void GetCurrency()
        {
            currencies.Clear();
            if (!commonQueries.GetCurrencyTypes(ref currencies))
                return;
            BASIC_STRUCTS.CURRENCY_STRUCT currencyItem = new CURRENCY_STRUCT();
            currencyItem.currencyName = "Other";
            currencyItem.currencyId = 0;
            if (!currencies.Contains(currencyItem))
            {
                currencies.Add(currencyItem);
            }
        }
        private void GetStockTypes()
        {
            stockTypes.Clear();
            if (!commonQueries.GetStockCategories(ref stockTypes))
                return;
        }
        private void GetRFPRequestorTeam()
        {
            if (!commonQueries.GetRFPRequestors(ref requestors))
                return;

        }
        private void FillCurrencyComboBox(ComboBox currencyComboBox)
        {
            for(int i =0; i<currencies.Count;i++)
            {
                ComboBoxItem currencyItem = new ComboBoxItem();
                currencyItem.Content = currencies[i].currencyName;
                currencyItem.Tag = currencies[i].currencyId;

                currencyComboBox.Items.Add(currencyItem);
            }
        }
        private void FillStockTypesComboBox(ComboBox stockTypeComboBox)
        {
            for (int i = 0; i < stockTypes.Count; i++)
            {
                ComboBoxItem stockTypeItem = new ComboBoxItem();
                stockTypeItem.Content = stockTypes[i].stock_type_name;
                stockTypeItem.Tag = stockTypes[i].stock_type_id;

                stockTypeComboBox.Items.Add(stockTypeItem);
            }
        }

    }
}
