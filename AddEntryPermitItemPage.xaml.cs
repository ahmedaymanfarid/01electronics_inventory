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
        private RFP rfp;
        private RFP oldRfp;
        int viewAddCondition;
        private AddEntryPermitWindow entryPermitWindow;
        public AddEntryPermitPage addEntryPermitPage;

        public MaterialEntryPermit oldMaterialEntryPermit = null;
        public MaterialReservation reservation;

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
        int itemNumber;

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
            rfp = new RFP();
            oldRfp = new RFP();
            rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
            reservation = new MaterialReservation();
            stockTypes = new List<INVENTORY_STRUCTS.STOCK_TYPES>();

            specs = new List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT>();

            materialReservations = new List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>();
            itemNumber = 1;
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
                        itemNumber=i+1;
                        InitializeNewCard(itemNumber);
                        //InitializeGenerateSerialCard();
                    }
                    int counter = 0;
                    for(int i=0;i<itemsWrapPanel.Children.Count;i++)
                    {
                        Border itemBorder = itemsWrapPanel.Children[i] as Border;
                        //Border generateSerialsBorder = itemsWrapPanel.Children[i + 1] as Border;

                        StackPanel itemsStackPanel = itemBorder.Child as StackPanel;
                        //StackPanel generateSerialsMainStackPanel = generateSerialsBorder.Child as StackPanel;

                        WrapPanel workFormWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                        ComboBox workFormComboBox = workFormWrapPanel.Children[1] as ComboBox;
                       

                        WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                        ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;
                       

                        WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                        ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                       

                        WrapPanel rfpItemDescription = itemsStackPanel.Children[5] as WrapPanel;
                        TextBlock rfpItemDescriptionLabel = rfpItemDescription.Children[0] as TextBlock;
                        ComboBox rfpItemdescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;
                 

                        WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                        ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;
                     

                        WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                        ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                     

                        WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                        ComboBox typecomboBox = typeWrapPanel.Children[1] as ComboBox;
                   

                        WrapPanel brandsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                        ComboBox brandComboBox = brandsWrapPanel.Children[1] as ComboBox;
                       

                        WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                        ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                   

                        WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                        ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                        WrapPanel serialNumberWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                        TextBox serialNumberTextBox = serialNumberWrapPanel.Children[1] as TextBox;


                        WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                        TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                      

                        WrapPanel priceWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                        TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                   

                        WrapPanel currencyWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                        ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
               

                        WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[16] as WrapPanel;
                        ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                

                        //WrapPanel serialsWrapPanel = generateSerialsMainStackPanel.Children[0] as WrapPanel;
                        //CheckBox serialsCheckBox = serialsWrapPanel.Children[0] as CheckBox; 
                       


                        //WrapPanel startSerialWrapPanel = generateSerialsMainStackPanel.Children[1] as WrapPanel;
                        //TextBox startSerialTexBox = startSerialWrapPanel.Children[1] as TextBox;
                 

                        //WrapPanel endSerialWrapPanel = generateSerialsMainStackPanel.Children[2] as WrapPanel;
                        //TextBox endSerialTexBox = endSerialWrapPanel.Children[1] as TextBox;
                

                        //ScrollViewer generatedSerialsScrollViewer = generateSerialsMainStackPanel.Children[3] as ScrollViewer;
                        //StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;

                        //Button generateSerialsButton = generateSerialsMainStackPanel.Children[4] as Button;

                        if (oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpSerial==0)
                        {
                            workFormComboBox.SelectedIndex = 1;
                            if (oldMaterialEntryPermit.GetItems()[counter].is_company_product)
                            {
                                choiceComboBox.SelectedIndex = 1;
                                categoryComboBox.Items.Clear();
                                categoryComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_category.category_name);
                                categoryComboBox.SelectedIndex = 0;

                                typecomboBox.Items.Clear();
                                typecomboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_type.product_name);
                                typecomboBox.SelectedIndex = 0;

                                brandComboBox.Items.Clear();
                                brandComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_name);
                                brandComboBox.SelectedIndex = 0;

                                modelComboBox.Items.Clear();
                                modelComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_model.model_name);
                                modelComboBox.SelectedIndex = 0;

                                specsComboBox.Items.Clear();
                                specsComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_specs.spec_name);
                                specsComboBox.SelectedIndex = 0;

                                if(oldMaterialEntryPermit.GetItems()[counter].product_serial_number!=null)
                                serialNumberTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number;


                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                                categoryComboBox.Items.Clear();
                                categoryComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_category.category_name);
                                categoryComboBox.SelectedIndex = 0;

                                typecomboBox.Items.Clear();
                                typecomboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_type.product_name);
                                typecomboBox.SelectedIndex = 0;

                                brandComboBox.Items.Clear();
                                brandComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_name);
                                brandComboBox.SelectedIndex = 0;

                                modelComboBox.Items.Clear();
                                modelComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_model.model_name);
                                modelComboBox.SelectedIndex = 0;
                            }

                        }
                        else
                        {
                            workFormComboBox.SelectedIndex = 0;
                            rfpRequestorTeamComboBox.Items.Clear();
                            rfpRequestorTeamComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfp_requestor_team_name);
                            rfpRequestorTeamComboBox.SelectedIndex = 0;

                            rfpIdComboBox.Items.Clear();
                            rfpIdComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpID);
                            rfpIdComboBox.SelectedIndex = 0;

                            rfpItemdescriptionComboBox.Items.Clear();
                            rfpItemdescriptionComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfp_item_description);
                            rfpItemdescriptionComboBox.SelectedIndex = 0;

                            if (oldMaterialEntryPermit.GetItems()[counter].is_company_product)
                            {
                                choiceComboBox.SelectedIndex = 1;
                                categoryComboBox.Items.Clear();
                                categoryComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_category.category_name);
                                categoryComboBox.SelectedIndex = 0;

                                typecomboBox.Items.Clear();
                                typecomboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_type.product_name);
                                typecomboBox.SelectedIndex = 0;

                                brandComboBox.Items.Clear();
                                brandComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_name);
                                brandComboBox.SelectedIndex = 0;

                                modelComboBox.Items.Clear();
                                modelComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_model.model_name);
                                modelComboBox.SelectedIndex = 0;

                                specsComboBox.Items.Clear();
                                specsComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_specs.spec_name);
                                specsComboBox.SelectedIndex = 0;

                                if (oldMaterialEntryPermit.GetItems()[counter].product_serial_number!=null)
                                serialNumberTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number.ToString();
                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                                categoryComboBox.Items.Clear();
                                categoryComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_category.category_name);
                                categoryComboBox.SelectedIndex = 0;

                                typecomboBox.Items.Clear();
                                typecomboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_type.product_name);
                                typecomboBox.SelectedIndex = 0;

                                brandComboBox.Items.Clear();
                                brandComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_name);
                                brandComboBox.SelectedIndex = 0;

                                modelComboBox.Items.Clear();
                                modelComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].product_model.model_name);
                                modelComboBox.SelectedIndex = 0;
                            }
                           

                        }
                        quantityTextBox.Text= oldMaterialEntryPermit.GetItems()[counter].quantity.ToString();   
                        priceTextBox.Text= oldMaterialEntryPermit.GetItems()[counter].item_price.ToString();
                        
                        currencyComboBox.Items.Clear();
                        currencyComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].item_currency.currencyName);
                        currencyComboBox.SelectedIndex = 0;

                        stockTypeComboBox.Items.Clear();
                        stockTypeComboBox.Items.Add(oldMaterialEntryPermit.GetItems()[counter].stock_type.stock_type_name);
                        stockTypeComboBox.SelectedIndex = 0;

                        //if(oldMaterialEntryPermit.GetItems()[counter].product_serial_number != null)
                        //{
                        //    serialsCheckBox.IsChecked = true;
                        //    startSerialTexBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number;
                        //    endSerialTexBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number;
                           
                        //}
                        //else
                        //{
                        //    serialsCheckBox.IsChecked = false; 
                        //}
                        counter++;
                        workFormComboBox.IsEnabled = false;
                        choiceComboBox.IsEnabled = false;
                        categoryComboBox.IsEnabled = false;
                        typecomboBox.IsEnabled = false;
                        brandComboBox.IsEnabled = false;
                        modelComboBox.IsEnabled = false;
                        specsComboBox.IsEnabled = false;
                        quantityTextBox.IsEnabled = false;
                        priceTextBox.IsEnabled = false;
                        currencyComboBox.IsEnabled = false;
                        stockTypeComboBox.IsEnabled = false;
                        serialNumberWrapPanel.Visibility = Visibility.Visible;
                        serialNumberTextBox.IsEnabled = false;
                        rfpRequestorTeamComboBox.IsEnabled = false;
                        rfpIdComboBox.IsEnabled = false;
                        rfpItemdescriptionComboBox.IsEnabled = false;
                        //serialsCheckBox.IsEnabled = false;
                        //startSerialTexBox.IsEnabled = false;
                        //endSerialTexBox.IsEnabled = false;
                        //generateSerialsButton.IsEnabled = false;
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //Border itemBorder = itemsWrapPanel.Children[i] as Border;
                        //StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                        //Label header = mainStackPanel.Children[0] as Label;
                        //header.Content = "Item " + (i + 1);
                        //ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                        //StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                        //for(int j = 0; j < oldMaterialEntryPermit.GetItems().Count;j++)
                        //{
                        //    CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;

                        //    rfpCheckBox.IsEnabled = false;
                        //    if (oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpSerial != 0)
                        //    {
                        //        rfpCheckBox.IsChecked = true;

                        //        WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                        //        rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                        //        ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                        //        ComboBoxItem rfpRequestorItem = new ComboBoxItem();


                        //        WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                        //        rfpIdWrapPanel.Visibility = Visibility.Visible;
                        //        ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                        //        rfpIdComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpID;
                        //        rfpIdComboBox.IsEnabled = false;

                        //        WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                        //        rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                        //        ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[2] as ComboBox;
                        //        rfpDescriptionComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfp_items[0].item_description;
                        //        rfpDescriptionComboBox.IsEnabled = true;


                        //    }
                        //    WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                        //    ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;
                        //    choiceComboBox.IsEnabled = false;
                        //    if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                        //    {
                        //        choiceComboBox.SelectedIndex = 1;
                        //    }
                        //    else
                        //    {
                        //        choiceComboBox.SelectedIndex = 0;
                        //    }

                        //    WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                        //    ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem categoryItem = new ComboBoxItem();
                        //    categoryItem.Content = oldMaterialEntryPermit.GetItems()[i].product_category.category_name;
                        //    categoryItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_category.category_id;
                        //    categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(categoryItem.Tag));
                        //    categoryComboBox.IsEnabled = false;

                        //    WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                        //    ComboBox  typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem typeItem = new ComboBoxItem();
                        //    typeItem.Content = oldMaterialEntryPermit.GetItems()[i].product_type.product_name;
                        //    typeItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_type.type_id;
                        //    typeComboBox.SelectedItem = typeComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(typeItem.Tag));
                        //    typeComboBox.IsEnabled = false;

                        //    WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                        //    ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem brandItem = new ComboBoxItem();
                        //    brandItem.Content = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_name;
                        //    brandItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_id;
                        //    brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().First(c=>c.Tag.Equals(brandItem.Tag));
                        //    brandComboBox.IsEnabled = false;

                        //    WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                        //    ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem modelItem = new ComboBoxItem();
                        //    modelItem.Content = oldMaterialEntryPermit.GetItems()[i].product_model.model_name;
                        //    modelItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_model.model_id;
                        //    modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(modelItem.Tag));
                        //    modelComboBox.IsEnabled = false;

                        //    WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                        //    ComboBox  specsComboBox = specsWrapPanel.Children[1] as ComboBox;


                        //    if (oldMaterialEntryPermit.GetItems()[i].is_company_product==true)
                        //    {
                        //        ComboBoxItem specsItem = new ComboBoxItem();
                        //        specsItem.Content = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_name;
                        //        specsItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_id;
                        //        specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().First(c=>c.Tag.Equals(specsItem.Tag));

                        //        specsWrapPanel.Visibility = Visibility.Visible;
                        //        specsComboBox.IsEnabled = false;

                        //    }
                        //    else
                        //    {

                        //        specsWrapPanel.Visibility = Visibility.Collapsed;
                        //    }

                        //    WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                        //    TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                        //    startSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                        //    startSerialTextBox.IsEnabled = false;

                        //    WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                        //    TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                        //    endSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                        //    endSerialTextBox.IsEnabled = false;

                        //    Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                        //    generateSerials.IsEnabled = false;

                        //    WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                        //    TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                        //    quantityTextBox.Text = oldMaterialEntryPermit.GetItems()[i].quantity.ToString();
                        //    quantityTextBox.IsEnabled = false;

                        //    WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                        //    TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                        //    priceTextBox.Text = oldMaterialEntryPermit.GetItems()[i].item_price.ToString();
                        //    priceTextBox.IsEnabled = false;

                        //    WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                        //    ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem currencyItem = new ComboBoxItem();
                        //    currencyItem.Content = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyName;
                        //    currencyItem.Tag = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyId;
                        //    currencyComboBox.SelectedItem = currencyComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(currencyItem.Tag));
                        //    currencyComboBox.IsEnabled = false;

                        //    WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                        //    ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                        //    ComboBoxItem stocktypeItem = new ComboBoxItem();
                        //    stocktypeItem.Content = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_name;
                        //    stocktypeItem.Tag = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_id;
                        //    stockTypeComboBox.SelectedItem = stockTypeComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(stocktypeItem.Tag));
                        //    stockTypeComboBox.IsEnabled = false;

                        //}
                    }
                }

            }
           else if(viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION)
            {
                //if (oldMaterialEntryPermit.GetItems().Count != 0)
                //{
                //    for (int i = 0; i < oldMaterialEntryPermit.GetItems().Count; i++)
                //    {
                //        itemNumber = i + 1;
                //        InitializeNewCard(itemNumber);
                //        InitializeGenerateSerialCard();
                //    }
                //    for (int i = 0; i < itemsWrapPanel.Children.Count; i++)
                //    {
                //        Border itemBorder = itemsWrapPanel.Children[i] as Border;
                //        StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                //        Label header = mainStackPanel.Children[0] as Label;
                //        header.Content = "Item " + (i + 1);
                //        ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                //        StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                //        for (int j = 0; j < oldMaterialEntryPermit.GetItems().Count; j++)
                //        {
                //            CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;

                //            //rfpCheckBox.IsEnabled = false;
                //            if (oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpSerial != 0)
                //            {
                //                rfpCheckBox.IsChecked = true;

                //                WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                //                rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                //                ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                //                ComboBoxItem rfpRequestorItem = new ComboBoxItem();


                //                WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                //                rfpIdWrapPanel.Visibility = Visibility.Visible;
                //                ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                //                rfpIdComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfpID;
                //                //rfpIdComboBox.IsEnabled = false;

                //                WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                //                rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                //                ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[2] as ComboBox;
                //                rfpDescriptionComboBox.SelectedItem = oldMaterialEntryPermit.GetItems()[i].rfp_info.rfp_items[0].item_description;
                //                //rfpDescriptionComboBox.IsEnabled = true;


                //            }
                //            WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                //            ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;
                //            //choiceComboBox.IsEnabled = false;
                //            if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                //            {
                //                choiceComboBox.SelectedIndex = 1;
                //            }
                //            else
                //            {
                //                choiceComboBox.SelectedIndex = 0;
                //            }

                //            WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                //            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem categoryItem = new ComboBoxItem();
                //            categoryItem.Content = oldMaterialEntryPermit.GetItems()[i].product_category.category_name;
                //            categoryItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_category.category_id;
                //            categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(categoryItem.Tag));
                //           // categoryComboBox.IsEnabled = false;

                //            WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                //            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem typeItem = new ComboBoxItem();
                //            typeItem.Content = oldMaterialEntryPermit.GetItems()[i].product_type.product_name;
                //            typeItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_type.type_id;
                //            typeComboBox.SelectedItem = typeComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(typeItem.Tag));
                //           // typeComboBox.IsEnabled = false;

                //            WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                //            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem brandItem = new ComboBoxItem();
                //            brandItem.Content = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_name;
                //            brandItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_brand.brand_id;
                //            brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(brandItem.Tag));
                //           // brandComboBox.IsEnabled = false;

                //            WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                //            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem modelItem = new ComboBoxItem();
                //            modelItem.Content = oldMaterialEntryPermit.GetItems()[i].product_model.model_name;
                //            modelItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_model.model_id;
                //            modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(modelItem.Tag));
                //          //  modelComboBox.IsEnabled = false;

                //            WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                //            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;


                //            if (oldMaterialEntryPermit.GetItems()[i].is_company_product == true)
                //            {
                //                ComboBoxItem specsItem = new ComboBoxItem();
                //                specsItem.Content = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_name;
                //                specsItem.Tag = oldMaterialEntryPermit.GetItems()[i].product_specs.spec_id;
                //                specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().First(c => c.Tag.Equals(specsItem.Tag));

                //                specsWrapPanel.Visibility = Visibility.Visible;
                //             //   specsComboBox.IsEnabled = false;

                //            }
                //            else
                //            {

                //                specsWrapPanel.Visibility = Visibility.Collapsed;
                //            }

                //            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                //            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                //            startSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                //           // startSerialTextBox.IsEnabled = false;

                //            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                //            TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                //            endSerialTextBox.Text = oldMaterialEntryPermit.GetItems()[i].product_serial_number;
                //           // endSerialTextBox.IsEnabled = false;

                //            Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                //           // generateSerials.IsEnabled = false;

                //            WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                //            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                //            quantityTextBox.Text = oldMaterialEntryPermit.GetItems()[i].quantity.ToString();
                //           // quantityTextBox.IsEnabled = false;

                //            WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                //            TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                //            priceTextBox.Text = oldMaterialEntryPermit.GetItems()[i].item_price.ToString();
                //            // priceTextBox.IsEnabled = false;

                //            WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                //            ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem currencyItem = new ComboBoxItem();
                //            currencyItem.Content = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyName;
                //            currencyItem.Tag = oldMaterialEntryPermit.GetItems()[i].item_currency.currencyId;
                //            currencyComboBox.SelectedItem = currencyComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(currencyItem.Tag));

                //            WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                //            ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                //            ComboBoxItem stocktypeItem = new ComboBoxItem();
                //            stocktypeItem.Content = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_name;
                //            stocktypeItem.Tag = oldMaterialEntryPermit.GetItems()[i].stock_type.stock_type_id;
                //            stockTypeComboBox.SelectedItem = stockTypeComboBox.Items.Cast<ComboBoxItem>().First(f => f.Tag.Equals(stocktypeItem.Tag));
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (oldMaterialEntryPermit.GetItems().Count != 0)
                {
                    for (int i = 0; i < oldMaterialEntryPermit.GetItems().Count; i++)
                    {
                        itemNumber = i + 1;
                        InitializeNewCard(itemNumber);
                        //InitializeGenerateSerialCard();
                    }
                    int counter = 0;
                    for (int i = 0; i < itemsWrapPanel.Children.Count; i++)
                    {
                        Border itemBorder = itemsWrapPanel.Children[i] as Border;
                       // Border generateSerialsBorder = itemsWrapPanel.Children[i + 1] as Border;

                        StackPanel itemsStackPanel = itemBorder.Child as StackPanel;
                      //  StackPanel generateSerialsMainStackPanel = generateSerialsBorder.Child as StackPanel;

                        WrapPanel workFormWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                        ComboBox workFormComboBox = workFormWrapPanel.Children[1] as ComboBox;


                        WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                        ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;


                        WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                        ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;


                        WrapPanel rfpItemDescription = itemsStackPanel.Children[5] as WrapPanel;
                        TextBlock rfpItemDescriptionLabel = rfpItemDescription.Children[0] as TextBlock;
                        ComboBox rfpItemdescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;


                        WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                        ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;


                        WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                        ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;


                        WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                        ComboBox typecomboBox = typeWrapPanel.Children[1] as ComboBox;


                        WrapPanel brandsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                        ComboBox brandComboBox = brandsWrapPanel.Children[1] as ComboBox;


                        WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                        ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;


                        WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                        ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                        WrapPanel serialNumberWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                        TextBox serialNumberTextBox = serialNumberWrapPanel.Children[1] as TextBox;

                        WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                        TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;


                        WrapPanel priceWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                        TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;


                        WrapPanel currencyWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                        ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;


                        WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[16] as WrapPanel;
                        ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;


                        //WrapPanel serialsWrapPanel = generateSerialsMainStackPanel.Children[0] as WrapPanel;
                        //CheckBox serialsCheckBox = serialsWrapPanel.Children[0] as CheckBox;



                        //WrapPanel startSerialWrapPanel = generateSerialsMainStackPanel.Children[1] as WrapPanel;
                        //TextBox startSerialTexBox = startSerialWrapPanel.Children[1] as TextBox;


                        //WrapPanel endSerialWrapPanel = generateSerialsMainStackPanel.Children[2] as WrapPanel;
                        //TextBox endSerialTexBox = endSerialWrapPanel.Children[1] as TextBox;


                        //ScrollViewer generatedSerialsScrollViewer = generateSerialsMainStackPanel.Children[3] as ScrollViewer;
                        //StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;

                        //Button generateSerialsButton = generateSerialsMainStackPanel.Children[4] as Button;

                        if (oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpSerial == 0)
                        {
                            workFormComboBox.SelectedIndex = 1;
                            if (oldMaterialEntryPermit.GetItems()[counter].is_company_product)
                            {
                                choiceComboBox.SelectedIndex = 1;
                                categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f=>Convert.ToInt32( f.Tag )==oldMaterialEntryPermit.GetItems()[counter].product_category.category_id);

                                
                                typecomboBox.SelectedItem= typecomboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_type.type_id);

                              
                                brandComboBox.SelectedItem= brandComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_id);

                                
                                modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_model.model_id);

                              
                                specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_specs.spec_id);

                                if (oldMaterialEntryPermit.GetItems()[counter].product_serial_number!=null)
                                serialNumberTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number.ToString();
                                serialNumberWrapPanel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                                categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_category.category_id);


                                typecomboBox.SelectedItem = typecomboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_type.type_id);


                                brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_id);


                                modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_model.model_id);

                                specsComboBox.Items.Clear();
                                specsComboBox.IsEnabled = false;
                                serialNumberWrapPanel.Visibility = Visibility.Collapsed;
                            }

                        }
                        else
                        {
                            workFormComboBox.SelectedIndex = 0;


                            ComboBoxItem requestorItem = new ComboBoxItem();
                            ComboBoxItem rfpIdItem = new ComboBoxItem();
                            ComboBoxItem itemDescriptionItem = new ComboBoxItem();

                            requestorItem.Content = oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfp_requestor_team_name;
                            requestorItem.Tag = oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpRequestorTeam;

                            rfpIdItem.Content = oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpID;
                            rfpIdItem.Tag = oldMaterialEntryPermit.GetItems()[counter].rfp_info;

                            itemDescriptionItem.Content = oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfp_item_description;
                            itemDescriptionItem.Tag = oldMaterialEntryPermit.GetItems()[counter].rfp_item;

                            rfpRequestorTeamComboBox.SelectedItem = rfpRequestorTeamComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => (Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpRequestorTeam));
                            rfpIdComboBox.Items.Add(rfpIdItem);
                           
                            rfpIdComboBox.SelectedItem= rfpIdComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => ((RFP_MIN_STRUCT)f.Tag).rfpID == oldMaterialEntryPermit.GetItems()[counter].rfp_info.rfpID);

                            rfpItemdescriptionComboBox.SelectedItem= rfpItemdescriptionComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => ((RFP_ITEM_MAX_STRUCT)f.Tag).rfp_item_number == oldMaterialEntryPermit.GetItems()[counter].rfp_item_number);



                            if (oldMaterialEntryPermit.GetItems()[counter].is_company_product)
                            {
                                choiceComboBox.SelectedIndex = 1;
                                categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag )== oldMaterialEntryPermit.GetItems()[counter].product_category.category_id);

                                typecomboBox.SelectedItem = typecomboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_type.type_id);

                                brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_id);

                                modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_model.model_id);

                                specsComboBox.SelectedItem = specsComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_specs.spec_id);
                                if(oldMaterialEntryPermit.GetItems()[counter].product_serial_number!=null)
                                serialNumberTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number.ToString();

                                serialNumberWrapPanel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                choiceComboBox.SelectedIndex = 0;
                                categoryComboBox.SelectedItem = categoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_category.category_id);

                                typecomboBox.SelectedItem = typecomboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_type.type_id);

                                brandComboBox.SelectedItem = brandComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_brand.brand_id);

                                modelComboBox.SelectedItem = modelComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].product_model.model_id);
                                specsComboBox.IsEnabled = false;
                                serialNumberWrapPanel.Visibility = Visibility.Collapsed;

                            }


                        }
                        quantityTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].quantity.ToString();
                        priceTextBox.Text = oldMaterialEntryPermit.GetItems()[counter].item_price.ToString();

                        currencyComboBox.SelectedItem = currencyComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].item_currency.currencyId);

                        stockTypeComboBox.SelectedItem= stockTypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(f => Convert.ToInt32(f.Tag) == oldMaterialEntryPermit.GetItems()[counter].stock_type.stock_type_id);


                        //if (oldMaterialEntryPermit.GetItems()[counter].product_serial_number != null)
                        //{
                        //    serialsCheckBox.IsChecked = true;
                        //    startSerialTexBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number;
                        //    endSerialTexBox.Text = oldMaterialEntryPermit.GetItems()[counter].product_serial_number;

                        //}
                        //else
                        //{
                        //    serialsCheckBox.IsChecked = false;
                        //}
                        counter++;

                    
                    }
                    addNewItemButton();
                }
            }
            else
            {
               // itemsWrapPanel.Children.RemoveAt(itemsWrapPanel.Children.Count - 1);
                InitializeNewCard(itemNumber);
                InitializeGenerateSerialCard();
                addNewItemButton();
            }
        }
        public void InitializeNewCard(int itemNumber)
        {
            ///////////// CREATE CARD ///////////////
            
            var border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.CornerRadius = new CornerRadius(10);
            // border.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            border.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
            border.Width = 500;
            border.Height = 600;
            border.Margin = new Thickness(10);
            border.Effect = new DropShadowEffect { ShadowDepth = 2, BlurRadius = 5, Color = Colors.LightGray };

            var stackPanel = new StackPanel();
            stackPanel.Background = Brushes.Transparent;
            stackPanel.Margin = new Thickness(10);
            border.Child = stackPanel;

           
            var header = new Label();
            header.Content = $"Item {itemNumber}";
            header.Style = (Style)FindResource("GridItem");
            header.HorizontalAlignment = HorizontalAlignment.Stretch;
            header.Foreground = Brushes.White;
            header.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
            header.HorizontalContentAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(header);

            // var scrollViewer = new ScrollViewer();
            // scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            // scrollViewer.Height = 450;
            //// scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            // stackPanel.Children.Add(scrollViewer);

            //var itemsStackPanel = new StackPanel();
            //itemsStackPanel.Background = Brushes.Transparent;
            //itemsStackPanel.Margin = new Thickness(10);
            //itemsStackPanel.Height = 560;
            //scrollViewer.Content= itemsStackPanel;

            //var rfpcheckBox = new CheckBox();
            //rfpcheckBox.Style = (Style)FindResource("checkBoxStyle");
            //rfpcheckBox.HorizontalAlignment = HorizontalAlignment.Left;
            //rfpcheckBox.Checked += OnCheckRFPCheckBox;
            //rfpcheckBox.Unchecked += OnUnCheckRFPCheckBox;
            //rfpcheckBox.Content = "RFP";

            var searchLabel = new Label();
            searchLabel.Style = (Style)FindResource("labelStyle");
            searchLabel.Content = "Search";
            searchLabel.Width = 110;
            searchLabel.Margin = new Thickness(0);

            var searchTextBox = new TextBox();
            searchTextBox.Style = (Style)FindResource("textBoxStyle");
            searchTextBox.Width = 240;
            searchTextBox.Name = "searchTextBox";
            searchTextBox.TextChanged += OnTextChangedSearchTextBox;

            var searchWrapPanel = new WrapPanel();
            searchWrapPanel.Children.Add(searchLabel);
            searchWrapPanel.Children.Add(searchTextBox);

            var workFormLabel = new Label();
            workFormLabel.Style = (Style)FindResource("labelStyle");
            workFormLabel.Content = "Work Form";
            workFormLabel.Width = 140;
            workFormLabel.Margin = new Thickness(0);
            workFormLabel.Name = "workForm";

            var workFormComboBox = new ComboBox();
            workFormComboBox.Style = (Style)FindResource("comboBoxStyleCard2");
            workFormComboBox.Items.Add("RFP");
            workFormComboBox.Items.Add("N/A");
            workFormComboBox.SelectionChanged += OnSelectionChangedWorkFormComboBox;

            var workFormWrapPanel = new WrapPanel();
            workFormWrapPanel.Children.Add(workFormLabel);
            workFormWrapPanel.Children.Add(workFormComboBox);

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
            itemDescriptionTextBlock.Text = "RFP Item";
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
           // specsWrapPanel.Visibility = Visibility.Collapsed;
            specsWrapPanel.Children.Add(specsLabel);
            specsWrapPanel.Children.Add(specsComboBox);

            var serialNumber = new Label();
            serialNumber.Style = (Style)FindResource("labelStyle");
            serialNumber.Margin = new Thickness(0);
            serialNumber.Background = Brushes.Transparent;
            serialNumber.Content = "Serial Number";
            serialNumber.Width = 140;
            serialNumber.Name = "specsLabel";
            // specsLabel.Visibility = Visibility.Collapsed;

            var serialNumberTextBox = new TextBox();
            serialNumberTextBox.Style = (Style)FindResource("textBoxStyle");
            serialNumberTextBox.Margin = new Thickness(0);
            serialNumberTextBox.Width = 240;
            // quantityTextBox.IsEnabled = false;
            serialNumberTextBox.Name = "serialNumberTextBox";


            var seriaNumberWrapPanel = new WrapPanel();
            seriaNumberWrapPanel.Visibility = Visibility.Collapsed;
            seriaNumberWrapPanel.Children.Add(serialNumber);
            seriaNumberWrapPanel.Children.Add(serialNumberTextBox);

            //var startSerialWrapPanel = new WrapPanel();

            //var startSerialLabel = new Label();
            //startSerialLabel.Style = (Style)FindResource("labelStyle");
            //startSerialLabel.Margin = new Thickness(0);
            //startSerialLabel.Background = Brushes.Transparent;
            //startSerialLabel.Content = "Start Serial";
            //startSerialLabel.Width = 140;
            //startSerialLabel.Name = "startSerialLabel";

            //var startSerialTextBox = new TextBox();
            //startSerialTextBox.Style = (Style)FindResource("textBoxStyle");
            //startSerialTextBox.Name = "startSerialTextBox";
            //startSerialTextBox.Margin = new Thickness(0);
            //startSerialTextBox.Width = 240;

            //startSerialWrapPanel.Children.Add(startSerialLabel);
            //startSerialWrapPanel.Children.Add(startSerialTextBox);

            //// Create the second WrapPanel control
            //var endSerialWrapPanel = new WrapPanel();

            //var endSerialLabel = new Label();
            //endSerialLabel.Style = (Style)FindResource("labelStyle");
            //endSerialLabel.Margin = new Thickness(0);
            //endSerialLabel.Background = Brushes.Transparent;
            //endSerialLabel.Content = "End Serial";
            //endSerialLabel.Width = 140;
            //endSerialLabel.Name = "endSerialLabel";

            //var endSerialTextBox = new TextBox();
            //endSerialTextBox.Style = (Style)FindResource("textBoxStyle");
            //endSerialTextBox.Name = "endSerialTextBox";
            //endSerialTextBox.Margin = new Thickness(0);
            //endSerialTextBox.Width = 240;

            //var generateButton = new Button();
            //generateButton.Style = (Style)FindResource("buttonStyle2");
            //generateButton.Width = 50;
            //generateButton.Content = "generate";
            //generateButton.FontSize = 10;
            //generateButton.Margin = new Thickness(10,0,0,0);
            //generateButton.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
            //generateButton.Foreground = Brushes.White;
            //generateButton.Click += OnButtonClickGenenrateSerials;

            //endSerialWrapPanel.Children.Add(endSerialLabel);
            //endSerialWrapPanel.Children.Add(endSerialTextBox);
            //endSerialWrapPanel.Children.Add(generateButton);

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

             //var generatedSerialsStackPanel = new StackPanel();
             ////generatedSerialsStackPanel.Visibility = Visibility.Collapsed;
             ////generatedSerialsStackPanel.Height = 100;
             
             //var generatedSerialsScrollViewer = new ScrollViewer();
             //generatedSerialsScrollViewer.Content = generatedSerialsStackPanel;
             //generatedSerialsScrollViewer.Height = 100;
             //generatedSerialsScrollViewer.Visibility = Visibility.Collapsed;
             //generatedSerialsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            itemsWrapPanel.Children.Add(border);
            stackPanel.Children.Add(searchWrapPanel);
            stackPanel.Children.Add(workFormWrapPanel);
            stackPanel.Children.Add(requestorWrapPanel);
            stackPanel.Children.Add(rfpIdWrapPanel);
            stackPanel.Children.Add(rfpItemDescriptionWrapPanel);
            stackPanel.Children.Add(choiseWrapPanel);
            stackPanel.Children.Add(companyCategoryWrapPanel);
            stackPanel.Children.Add(productCategoryWrapPanel);
            stackPanel.Children.Add(brandWrapPanel);
            stackPanel.Children.Add(modelWrapPanel);
            stackPanel.Children.Add(specsWrapPanel);
            stackPanel.Children.Add(seriaNumberWrapPanel);
            //stackPanel.Children.Add(startSerialWrapPanel);
           // stackPanel.Children.Add(endSerialWrapPanel);
            stackPanel.Children.Add(quantityWrapPanel);
            stackPanel.Children.Add(priceWrapPanel);
            stackPanel.Children.Add(currencyWrapPanel);
            stackPanel.Children.Add(stockTypeWrapPanel);
            //itemsStackPanel.Children.Add(generatedSerialsScrollViewer);
            if (itemsWrapPanel.Children.Count > 1 && viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            {
                RemoveItemButton(ref stackPanel);
            }


        }

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSelectionChangedWorkFormComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox workForm = sender as ComboBox;
            WrapPanel workFormWrapPanel = workForm.Parent as WrapPanel;
            StackPanel itemsStackPanel = workFormWrapPanel.Parent as StackPanel;

            WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
            WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel rfpItemDescriptionWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
           

            ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;
            ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
            ComboBox rfpItemDescriptionComboBox = rfpItemDescriptionWrapPanel.Children[1] as ComboBox;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;


            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;


            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            ComboBox typecomboBox = typeWrapPanel.Children[1] as ComboBox;


            WrapPanel brandsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            ComboBox brandComboBox = brandsWrapPanel.Children[1] as ComboBox;


            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;


            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

            WrapPanel serialNumberWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
            TextBox serialNumberTextBox = serialNumberWrapPanel.Children[1] as TextBox;


            WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;


            WrapPanel priceWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
            TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;


            WrapPanel currencyWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
            ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;


            WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[16] as WrapPanel;
            ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;


            if (workForm.SelectedIndex==0)
            {
                

                rfpRequestorTeamWrapPanel.Visibility = Visibility.Visible;
                rfpIdWrapPanel.Visibility = Visibility.Visible;
                rfpItemDescriptionWrapPanel.Visibility = Visibility.Visible;
                rfpRequestorTeamComboBox.IsEnabled = true;
                rfpIdComboBox.IsEnabled = true; 
                rfpItemDescriptionComboBox.IsEnabled = true;

                for (int i = 0; i < requestors.Count; i++)
                {
                    ComboBoxItem requestorTeamItem = new ComboBoxItem();

                    requestorTeamItem.Content = requestors[i].requestor_team.team_name;
                    requestorTeamItem.Tag = requestors[i].requestor_team.team_id;
                    if (rfpRequestorTeamComboBox.Items.Cast<ComboBoxItem>().Any(f => f.Tag.Equals(requestorTeamItem.Tag)))
                        continue;
                    rfpRequestorTeamComboBox.Items.Add(requestorTeamItem);

                    choiceComboBox.IsEnabled = false;
                    categoryComboBox.IsEnabled = false;
                    typecomboBox.IsEnabled = false;
                    brandComboBox.IsEnabled = false;
                    modelComboBox.IsEnabled = false;
                    specsComboBox.IsEnabled = false;
                    quantityTextBox.IsEnabled = false;
                }
            }
            else
            {
                rfpRequestorTeamWrapPanel.Visibility = Visibility.Collapsed;
                rfpIdWrapPanel.Visibility = Visibility.Collapsed;
                rfpItemDescriptionWrapPanel.Visibility = Visibility.Collapsed;

                rfpRequestorTeamComboBox.IsEnabled = false;
                rfpIdComboBox.IsEnabled = false;
                rfpItemDescriptionComboBox.IsEnabled = false;

                rfpRequestorTeamComboBox.Items.Clear();
                rfpIdComboBox.Items.Clear();
                rfpItemDescriptionComboBox.Items.Clear();

                choiceComboBox.IsEnabled = true;
                categoryComboBox.IsEnabled = true;
                quantityTextBox.IsEnabled = true;

            }
            choiceComboBox.SelectedIndex = -1;
            categoryComboBox.SelectedIndex = -1;
            typecomboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;
            modelComboBox.SelectedIndex = -1;
            specsComboBox.SelectedIndex = -1;
            serialNumberTextBox.Clear();
            quantityTextBox.Clear();
            priceTextBox.Clear();
            currencyComboBox.SelectedIndex = -1;
            stockTypeComboBox.SelectedIndex = -1;
          
        }

        public void InitializeGenerateSerialCard()
        {
            var border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.CornerRadius = new CornerRadius(10);
            // border.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            border.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            border.Width = 500;
            border.Height = 600;
            border.Margin = new Thickness(10);
            border.Effect = new DropShadowEffect { ShadowDepth = 2, BlurRadius = 5, Color = Colors.LightGray };

            var stackPanel = new StackPanel();
            stackPanel.Background = Brushes.Transparent;
            stackPanel.Margin = new Thickness(10);
            border.Child = stackPanel;

            var header = new Label();
            header.Content = $"Generate Serials Item {itemNumber}";
            header.Style = (Style)FindResource("GridItem");
            header.Foreground = Brushes.White;
            header.VerticalAlignment = VerticalAlignment.Center;

            var generateSerialCheckBox = new CheckBox();
            generateSerialCheckBox.Content = header;
            generateSerialCheckBox.HorizontalAlignment= HorizontalAlignment.Center;
            generateSerialCheckBox.VerticalAlignment= VerticalAlignment.Center;
            generateSerialCheckBox.VerticalContentAlignment= VerticalAlignment.Center;
            generateSerialCheckBox.Checked += OnCheckGenerateSerialCheckBox;
            generateSerialCheckBox.Unchecked += OnUnCheckGenerateSerialsCheckBox;
            generateSerialCheckBox.Margin=new Thickness(200,4,0,0);

            var generateSerialLabelBackGround = new WrapPanel();
            generateSerialLabelBackGround.Background= new SolidColorBrush(Color.FromRgb(16, 90, 151));
            generateSerialLabelBackGround.Children.Add(generateSerialCheckBox);
            generateSerialLabelBackGround.HorizontalAlignment = HorizontalAlignment.Center;
            generateSerialLabelBackGround.Width = 500;
            stackPanel.Children.Add(generateSerialLabelBackGround);

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
            startSerialTextBox.IsEnabled = false;

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
            endSerialTextBox.IsEnabled = false;

            var generatedSerialsStackPanel = new StackPanel();
            //generatedSerialsStackPanel.Visibility = Visibility.Collapsed;
            //generatedSerialsStackPanel.Height = 100;

            var generatedSerialsScrollViewer = new ScrollViewer();
            generatedSerialsScrollViewer.Content = generatedSerialsStackPanel;
            generatedSerialsScrollViewer.Height = 300;
            generatedSerialsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            var generateButton = new Button();
            generateButton.Style = (Style)FindResource("buttonStyle2");
            generateButton.Width = 200;
            generateButton.Content = "generate";
            generateButton.FontSize = 20;
            generateButton.Height = 40;
            generateButton.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
            generateButton.Foreground = Brushes.White;
            generateButton.Click += OnButtonClickGenenrateSerials;
            generateButton.IsEnabled = false;

            endSerialWrapPanel.Children.Add(endSerialLabel);
            endSerialWrapPanel.Children.Add(endSerialTextBox);

            stackPanel.Children.Add(startSerialWrapPanel);
            stackPanel.Children.Add(endSerialWrapPanel);
            stackPanel.Children.Add(generatedSerialsScrollViewer);
            stackPanel.Children.Add(generateButton);

            itemsWrapPanel.Children.Add(border);
        }

        private void OnUnCheckGenerateSerialsCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox generateSerialCheckBox = sender as CheckBox;
            WrapPanel generateSerialCheckBoxContainerWrapPnel = generateSerialCheckBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = generateSerialCheckBoxContainerWrapPnel.Parent as StackPanel;
            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            ScrollViewer generatedSerialsScrollViewer = itemsStackPanel.Children[3] as ScrollViewer;
            Button generateButton = itemsStackPanel.Children[4] as Button;

            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
            TextBox endSerialTextbox = endSerialWrapPanel.Children[1] as TextBox;
            StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;

            startSerialTextBox.IsEnabled = false;
            startSerialTextBox.Clear();
            endSerialTextbox.IsEnabled = false ;
            endSerialTextbox.Clear();
            generatedSerialsStackPanel.Children.Clear();
           

            generateButton.IsEnabled = false ;
        }

        private void OnCheckGenerateSerialCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox generateSerialCheckBox = sender as CheckBox;
            WrapPanel generateSerialCheckBoxContainerWrapPnel = generateSerialCheckBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = generateSerialCheckBoxContainerWrapPnel.Parent as StackPanel;
            WrapPanel startSerialWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            Button generateButton = itemsStackPanel.Children[4] as Button;

            TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
            TextBox endSerialTextbox = endSerialWrapPanel.Children[1] as TextBox;

            startSerialTextBox.IsEnabled = true;
            endSerialTextbox.IsEnabled = true;

            generateButton.IsEnabled = true;
        }

        private void addNewItemButton()
        {
            var border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.CornerRadius = new CornerRadius(10);
            // border.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            border.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            border.Width = 500;
            border.Height = 600;
            border.Margin = new Thickness(10);
            border.Effect = new DropShadowEffect { ShadowDepth = 2, BlurRadius = 5, Color = Colors.LightGray };

            var addNewItemButton = new Button();
            addNewItemButton.Content = "ADD NEW ITEM";
            addNewItemButton.Width = 200;
            addNewItemButton.Height = 50;
            addNewItemButton.Style = (Style)FindResource("buttonBrowseStyle");
            addNewItemButton.Click += OnButtonClickAddNewCard;
            addNewItemButton.Margin = new Thickness(10);
            border.Child = addNewItemButton;
            itemsWrapPanel.Children.Add(border);
        }
        private void RemoveItemButton(ref StackPanel itemsStackPanel)
        {
            var removeItemButton = new Button();
            removeItemButton.Content = "REMOVE";
            removeItemButton.Width = 100;
            removeItemButton.Style = (Style)FindResource("buttonBrowseStyle");
            removeItemButton.Margin=new Thickness(0);
            removeItemButton.Click += OnButtonClickRemoveItem; 
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

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;

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

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;

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

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;

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

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;

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

            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            WrapPanel brandWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;

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
           
            ComboBox rfpItemDescription = sender as ComboBox;
            WrapPanel rfpItemDescriptionWrapPanel = rfpItemDescription.Parent as WrapPanel;
            StackPanel itemsStackPanel = rfpItemDescriptionWrapPanel.Parent as StackPanel;

            WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
            ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;

            WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
            ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;

            WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
            ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;

            WrapPanel brandWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
            ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;

            WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;

            WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
            ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;


            if (rfpItemDescription.SelectedIndex!=-1 && viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            {
               


                WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                ComboBoxItem descriptionItem = rfpItemDescription.SelectedItem as ComboBoxItem;
                RFP_ITEM_MAX_STRUCT rfpItem = (RFP_ITEM_MAX_STRUCT)descriptionItem.Tag;

               
               
                
                  
               

                ComboBoxItem categoryItem = new ComboBoxItem();
                ComboBoxItem typeItem=new ComboBoxItem();
                ComboBoxItem brandItem = new ComboBoxItem();
                ComboBoxItem modelItem = new ComboBoxItem();
                ComboBoxItem specsItem = new ComboBoxItem();

                categoryItem.Content = rfpItem.product_category.category_name;
                categoryItem.Tag = rfpItem.product_category.category_id;

                typeItem.Content = rfpItem.product_type.product_name;
                typeItem.Tag = rfpItem.product_type.type_id;

                brandItem.Content = rfpItem.product_brand.brand_name;
                brandItem.Tag = rfpItem.product_brand.brand_id;

                modelItem.Content = rfpItem.product_model.model_name;
                modelItem.Tag = rfpItem.product_model.model_id;

                if(rfpItem.is_company_product)
                {
                    choiceComboBox.SelectedIndex = 1;
                    specsItem.Content = rfpItem.product_specs.spec_name;
                    specsItem.Tag = rfpItem.product_specs.spec_id;

                }
                else
                    choiceComboBox.SelectedIndex = 0;

                choiceComboBox.IsEnabled = false;

                categoryComboBox.Items.Clear();
                categoryComboBox.Items.Add(categoryItem);
                categoryComboBox.SelectedIndex = 0;
                typeComboBox.Items.Clear();
                typeComboBox.Items.Add(typeItem);
                typeComboBox.SelectedIndex = 0;
                brandComboBox.Items.Clear();
                brandComboBox.Items.Add(brandItem);
                brandComboBox.SelectedIndex = 0;
                modelComboBox.Items.Clear();
                modelComboBox.Items.Add(modelItem);
                modelComboBox.SelectedIndex = 0;
                specsComboBox.Items.Clear();
                specsComboBox.Items.Add(specsItem);
                specsComboBox.SelectedIndex = 0;

                categoryComboBox.IsEnabled = false;
                typeComboBox.IsEnabled = false;
                brandComboBox.IsEnabled = false;
                modelComboBox.IsEnabled = false;
                specsComboBox.IsEnabled = false;

                quantityTextBox.Text = rfpItem.item_quantity.ToString();
                quantityTextBox.IsEnabled = false;
            }
           

            //ComboBox rfpItemsComboBox = sender as ComboBox;

            //if (rfpItemsComboBox.SelectedIndex == -1)
            //    return;

            //WrapPanel rfpItemsPanel = rfpItemsComboBox.Parent as WrapPanel;


            //Grid card = rfpItemsPanel.Parent as Grid;

            //WrapPanel rfpRequsterPanel = card.Children[2] as WrapPanel;

            //ComboBox serialComboBox = rfpRequsterPanel.Children[2] as ComboBox;

            //WrapPanel startSerialPanel = card.Children[14] as WrapPanel;
            //WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

            //WrapPanel stockTypePanel = card.Children[19] as WrapPanel;

            //WrapPanel validityDatePanel = card.Children[20] as WrapPanel;


            //Button generateSerialsButton = card.Children[21] as Button;





            //commonQueries.GetRfpItemsMapping(rfps[serialComboBox.SelectedIndex].rfpSerial, rfps[serialComboBox.SelectedIndex].rfpVersion, rfps[serialComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems);



            //rfpItemsComboBox.Tag = rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity;

            //if (rfpItems[rfpItemsComboBox.SelectedIndex].product_category.category_name != "")
            //{


            //    if (rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == false)
            //    {

            //        startSerialPanel.Visibility = Visibility.Collapsed;
            //        endSerialPanel.Visibility = Visibility.Collapsed;

            //        stockTypePanel.Visibility = Visibility.Visible;
            //        validityDatePanel.Visibility = Visibility.Visible;

            //        generateSerialsButton.Visibility = Visibility.Collapsed;

            //    }

            //    else
            //    {

            //        startSerialPanel.Visibility = Visibility.Visible;
            //        endSerialPanel.Visibility = Visibility.Visible;
            //        stockTypePanel.Visibility = Visibility.Collapsed;

            //        validityDatePanel.Visibility = Visibility.Collapsed;
            //        generateSerialsButton.Visibility = Visibility.Visible;




            //    }

            //}

            //else
            //{


            //    if (rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == false)
            //    {

            //        startSerialPanel.Visibility = Visibility.Collapsed;
            //        endSerialPanel.Visibility = Visibility.Collapsed;
            //        stockTypePanel.Visibility = Visibility.Visible;
            //        validityDatePanel.Visibility = Visibility.Visible;

            //        generateSerialsButton.Visibility = Visibility.Collapsed;




            //    }


            //    else
            //    {

            //        startSerialPanel.Visibility = Visibility.Visible;
            //        endSerialPanel.Visibility = Visibility.Visible;

            //        stockTypePanel.Visibility = Visibility.Collapsed;
            //        validityDatePanel.Visibility = Visibility.Collapsed;

            //        generateSerialsButton.Visibility = Visibility.Visible;




            //    }


            //}


        }
        private void OnSelChangedRfpIdComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox rfpIdComboBox = sender as ComboBox;
            WrapPanel rfpIdWrapPanel = rfpIdComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = rfpIdWrapPanel.Parent as StackPanel;

            WrapPanel itemDescriptionWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
            TextBlock itemDescriptionLabel = itemDescriptionWrapPanel.Children[0] as TextBlock;
            ComboBox itemDescriptionComboBox = itemDescriptionWrapPanel.Children[1] as ComboBox;

            WrapPanel requestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
            ComboBox requestorTeamComboBox = requestorTeamWrapPanel.Children[1] as ComboBox;

         
            //ComboBox serialComboBox=sender as ComboBox;

            //WrapPanel serialPanel=serialComboBox.Parent as WrapPanel;

            //Grid card= serialPanel.Parent as Grid;

            //ComboBox requsterComboBox= serialPanel.Children[1] as ComboBox;

            rfps.Clear();

            if (rfpIdComboBox.SelectedIndex != -1 && viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            { 

            //SEPARATE GET DATA FUNCTIONS FROM GUI FUNCTIONS
            //COMMON QUERIES
            //IF NOT RETURN
            ComboBoxItem requestorTeam = requestorTeamComboBox.SelectedItem as ComboBoxItem;
            ComboBoxItem rfpId = rfpIdComboBox.SelectedItem as ComboBoxItem;
            PROCUREMENT_STRUCTS.RFP_MIN_STRUCT rfpItem = (RFP_MIN_STRUCT)rfpId.Tag;
            //if (!commonQueries.GetTeamRFPsMappedIds(ref rfps, Int32.Parse(requestorTeam.Tag.ToString())))
            //return;

            rfpItems.Clear();

            //if (!commonQueries.GetRfpItemsMapping(rfpItem.rfpSerial, rfpItem.rfpVersion, rfpItem.rfpRequestorTeam, ref rfpItems))
            //    return;
            rfp.rfpItems.Clear();
            if (!rfp.InitializeRFP(rfpItem.rfpRequestorTeam, rfpItem.rfpSerial, rfpItem.rfpVersion))
                return;

            itemDescriptionLabel.Tag = rfp;


            //for (int i = 0; i < rfp.rfpItems.Count; i++)
            //{

            //    if (rfp.rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED)
            //    {

            //        rfp.rfpItems.RemoveAt(i);
            //        i--;
            //    }
            //}


            //WrapPanel descriptionPanel= card.Children[3] as WrapPanel;

            //ComboBox itemDescription= descriptionPanel.Children[1] as ComboBox;

            itemDescriptionComboBox.Items.Clear();
            itemDescriptionComboBox.IsEnabled = true;




            //if (rfp.rfpItems.Count == 0)
            //{

            //    System.Windows.Forms.MessageBox.Show("Inventory must revise it first", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


            //    return;
            //}

            for (int i = 0; i < rfp.rfpItems.Count; i++)
            {


                //if (rfpItems[i].product_category.category_id == 0)
                //{
                //    itemDescriptionComboBox.Items.Add(rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);


                //}

                //else
                //{


                //    itemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);



                //}
                if (rfp.rfpItems[i].item_status.status_id == COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED && viewAddCondition==COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION)
                {

                    ComboBoxItem descriptionItem = new ComboBoxItem();
                    descriptionItem.Content = rfp.rfpItems[i].item_description;
                    descriptionItem.Tag = rfp.rfpItems[i];
                    itemDescriptionComboBox.Items.Add(descriptionItem);

                }
                else if(viewAddCondition==COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION||viewAddCondition==COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
                {
                    ComboBoxItem descriptionItem = new ComboBoxItem();
                    descriptionItem.Content = rfp.rfpItems[i].item_description;
                    descriptionItem.Tag = rfp.rfpItems[i];
                    itemDescriptionComboBox.Items.Add(descriptionItem);
                }


            }
            rfp = new RFP();
            }
        }
        private void OnSelChangedRfpRequestorTeamComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox requestorComboBox = sender as ComboBox;
            WrapPanel requestorTeamWrapPanel = requestorComboBox.Parent as WrapPanel;
            StackPanel itemsStackPanel = requestorTeamWrapPanel.Parent as StackPanel;
            WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
            WrapPanel rfpItemDescription = itemsStackPanel.Children[5] as WrapPanel;


            ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
            ComboBox rfpItemDescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

            if (requestorComboBox.SelectedIndex != -1 && viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION)
            {
                ComboBoxItem requestorTeamItem = requestorComboBox.SelectedItem as ComboBoxItem;
                rfps.Clear();
                rfpIdComboBox.Items.Clear();
                rfpItemDescriptionComboBox.Items.Clear();
                rfpIdComboBox.IsEnabled = true;
                if (!commonQueries.GetRFPsThatHaveInventoryRevisedItems(ref rfps))
                    return;
                for (int i = 0; i < rfps.Count; i++)
                {
                    if (rfps[i].rfpRequestorTeam==Convert.ToInt32(requestorTeamItem.Tag))
                    {
                        ComboBoxItem rfpId = new ComboBoxItem();
                        rfpId.Content = rfps[i].rfpID;
                        rfpId.Tag = rfps[i];

                        if (rfpIdComboBox.Items.Cast<ComboBoxItem>().Any(f => f.Tag.Equals(rfpId.Tag)))
                            continue;
                        rfpIdComboBox.Items.Add(rfpId);
                    }
                
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
            StackPanel itemsStackPanel = generateSerialsButton.Parent as StackPanel;
            Border generateSerialsCardBorder = itemsStackPanel.Parent as Border;
            int positionOfCardBorder = itemsWrapPanel.Children.IndexOf(generateSerialsCardBorder);

            WrapPanel startSerilWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
            TextBox startSerialTextBox = startSerilWrapPanel.Children[1] as TextBox;

            WrapPanel endSerialWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
            TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;   




            Border itemCard = itemsWrapPanel.Children[positionOfCardBorder - 1] as Border;
            StackPanel itemsCardStackPanel = itemCard.Child as StackPanel;
            WrapPanel workForm = itemsCardStackPanel.Children[2] as WrapPanel;
            WrapPanel quantity = itemsCardStackPanel.Children[13] as WrapPanel;

            ComboBox workFormComboBox = workForm.Children[1] as ComboBox;
            TextBox quantityTextBox = quantity.Children[1] as TextBox;







            //WrapPanel endSerialWrapPanel = generateSerialsButton.Parent as WrapPanel;
            //StackPanel itemsStackPanel = endSerialWrapPanel.Parent as StackPanel;

            //WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
            //WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;

            //TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
            //TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
            //TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;

            //CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;

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


                if ((Int32)(Convert.ToDecimal(quantityTextBox.Text.ToString())) != (int.Parse(number2) - int.Parse(number)) + 1)
                {

                    System.Windows.Forms.MessageBox.Show("The quantity dosent match the range!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                //ScrollViewer serialsScroll = card.Children[22] as ScrollViewer;
                //Grid serialsGrid = serialsScroll.Content as Grid;

                //serialsGrid.RowDefinitions.Clear();
                //serialsGrid.ColumnDefinitions.Clear();
                //serialsGrid.Children.Clear();

                ScrollViewer serialsGeneratedScrollViewer = itemsStackPanel.Children[3] as ScrollViewer;
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


                    if (workFormComboBox.SelectedIndex == 0) {

                        DatePicker vailidityDatePicker = new DatePicker();


                        vailidityDatePicker.Style = (Style)FindResource("minidatePickerStyle");
                        vailidityDatePicker.Margin = new Thickness(140, 0, 0, 0);

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

            if (viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION)
            {
                int rfpItemCount = 1;

                for (int i = 0; i < itemsWrapPanel.Children.Count - 1; i += 2)
                {

                    Border itemBorder = itemsWrapPanel.Children[i] as Border;
                    Border generateSerialsBorder = itemsWrapPanel.Children[i + 1] as Border;

                    StackPanel itemsStackPanel = itemBorder.Child as StackPanel;
                    StackPanel generateSerialsMainStackPanel = generateSerialsBorder.Child as StackPanel;

                    WrapPanel workFormWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                    ComboBox workFormComboBox = workFormWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                    ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                    ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpItemDescription = itemsStackPanel.Children[5] as WrapPanel;
                    TextBlock rfpItemDescriptionLabel = rfpItemDescription.Children[0] as TextBlock;
                    ComboBox rfpItemdescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

                    WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                    ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;

                    WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                    ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;

                    WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                    ComboBox typecomboBox = typeWrapPanel.Children[1] as ComboBox;

                    WrapPanel brandsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                    ComboBox brandComboBox = brandsWrapPanel.Children[1] as ComboBox;

                    WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                    ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;

                    WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                    ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                    WrapPanel productSerialWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                    TextBox productSerialTextBox = productSerialWrapPanel.Children[1] as TextBox;

                    WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                    TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;

                    WrapPanel priceWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                    TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;

                    WrapPanel currencyWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                    ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;

                    WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[16] as WrapPanel;
                    ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;




                    WrapPanel startSerialWrapPanel = generateSerialsMainStackPanel.Children[1] as WrapPanel;
                    TextBox startSerialTexBox = startSerialWrapPanel.Children[1] as TextBox;

                    WrapPanel endSerialWrapPanel = generateSerialsMainStackPanel.Children[2] as WrapPanel;
                    TextBox endSerialTexBox = endSerialWrapPanel.Children[1] as TextBox;

                    ScrollViewer generatedSerialsScrollViewer = generateSerialsMainStackPanel.Children[3] as ScrollViewer;
                    StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;

                    if (categoryComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Category is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (typecomboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Type is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (brandComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Brand is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (modelComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Model is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (choiceComboBox.SelectedIndex == 1)
                        if (specsComboBox.SelectedIndex == -1)
                        {
                            System.Windows.Forms.MessageBox.Show("Specs is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                    if (quantityTextBox.Text == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Quantity is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (priceTextBox.Text == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Price is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (currencyComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Currency is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (stockTypeComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Stock is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    ComboBoxItem categoryItem = (ComboBoxItem)categoryComboBox.SelectedItem;
                    ComboBoxItem typeItem = (ComboBoxItem)typecomboBox.SelectedItem;
                    ComboBoxItem brandItem = (ComboBoxItem)brandComboBox.SelectedItem;
                    ComboBoxItem modelItem = (ComboBoxItem)modelComboBox.SelectedItem;

                    ComboBoxItem currencyItem = (ComboBoxItem)currencyComboBox.SelectedItem;
                    ComboBoxItem stockTypeItem = (ComboBoxItem)stockTypeComboBox.SelectedItem;

                    ComboBoxItem specsItem = (ComboBoxItem)specsComboBox.SelectedItem;


                    if (workFormComboBox.SelectedIndex == 0)
                    {
                        ComboBoxItem descriptionItem = (ComboBoxItem)rfpItemdescriptionComboBox.SelectedItem;
                        RFP_ITEM_MAX_STRUCT rfpItem = (RFP_ITEM_MAX_STRUCT)descriptionItem.Tag;
                        rfpItem.product_category.category_id = Convert.ToInt32(categoryItem.Tag);
                        rfpItem.product_category.category_name = categoryItem.Content.ToString();
                        rfpItem.product_type.type_id = Convert.ToInt32(typeItem.Tag);
                        rfpItem.product_type.product_name = typeItem.Content.ToString();
                        rfpItem.product_brand.brand_id = Convert.ToInt32(brandItem.Tag);
                        rfpItem.product_brand.brand_name = brandItem.Content.ToString();
                        rfpItem.product_model.model_id = Convert.ToInt32(modelItem.Tag);
                        rfpItem.product_model.model_name = modelItem.Content.ToString();
                        RFP rfp = (RFP)rfpItemDescriptionLabel.Tag;
                        //rfp.rfpItems.Clear();
                        //if (!rfp.InitializeRFP(rfp.GetRFPRequestorTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                        //return;
                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            specsItem = (ComboBoxItem)specsComboBox.SelectedItem;
                            rfpItem.product_specs.spec_id = Convert.ToInt32(specsItem.Tag);
                            rfpItem.product_specs.spec_name = specsItem.Content.ToString();
                            rfpItem.is_company_product = true;
                            rfpItem.item_description = $@"{categoryItem.Content.ToString()} - {typeItem.Content.ToString()} - {brandItem.Content.ToString()} - {modelItem.Content.ToString()} - {specsItem.Content.ToString()}";

                        }
                        else
                        {
                            rfpItem.is_company_product = false;
                            rfpItem.item_description = $@"{categoryItem.Content.ToString()} - {typeItem.Content.ToString()} - {brandItem.Content.ToString()} - {modelItem.Content.ToString()} ";
                        }

                        //RFP_ITEM_MAX_STRUCT oldRfpItem = rfp.rfpItems.Find(f => f.rfp_item_number == rfpItem.rfp_item_number);
                        //int itemIndex = rfp.rfpItems.IndexOf(oldRfpItem);
                        //rfp.rfpItems.Remove(oldRfpItem);
                        //rfp.rfpItems.Insert(itemIndex, rfpItem);
                        //oldRfp.rfpItems.Clear();

                        //if (!oldRfp.InitializeRFP(rfp.GetRFPRequestorTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                        //    return;
                        //if (!rfp.EditRFPItems(ref oldRfp.rfpItems))
                        //    return;



                        INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                        entryPermitItem.product_category = rfpItem.product_category;
                        entryPermitItem.product_type = rfpItem.product_type;
                        entryPermitItem.product_brand = rfpItem.product_brand;
                        entryPermitItem.product_model = rfpItem.product_model;
                        entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                        entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                        entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                        entryPermitItem.rfp_info.rfpRequestorTeam = rfp.GetRFPRequestorTeamId();
                        entryPermitItem.rfp_info.rfpSerial = rfp.GetRFPSerial();
                        entryPermitItem.rfp_info.rfpVersion = rfp.GetRFPVersion();
                        entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        entryPermitItem.is_released = false;
                        entryPermitItem.stock_type.stock_type_id = Convert.ToInt32(stockTypeItem.Tag.ToString());
                        entryPermitItem.rfp_info.rfp_item_description = rfpItem.item_description;
                        entryPermitItem.rfp_item.measure_unit_id = rfpItem.measure_unit_id;
                        entryPermitItem.rfp_item.item_notes = rfpItem.item_notes;



                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            entryPermitItem.product_specs = rfpItem.product_specs;
                            entryPermitItem.is_company_product = true;
                        }
                        for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                        {


                            WrapPanel serialWrapPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                            TextBox serialNumber = serialWrapPanel.Children[1] as TextBox;
                            entryPermitItem.product_serial_number = serialNumber.Text;
                            ComboBox serialStockCategory = serialWrapPanel.Children[2] as ComboBox;
                            DatePicker datePicker = serialWrapPanel.Children[3] as DatePicker;
                            if (serialStockCategory.SelectedIndex == -1)
                            {
                                System.Windows.Forms.MessageBox.Show("Serial type is not specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                return;
                            }
                            entryPermitItem.valid_until = (DateTime)datePicker.SelectedDate;
                            addEntryPermitPage.materialEntryPermit.AddItem(entryPermitItem);

                            entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                            entryPermitItem.product_category = rfpItem.product_category;
                            entryPermitItem.product_type = rfpItem.product_type;
                            entryPermitItem.product_brand = rfpItem.product_brand;
                            entryPermitItem.product_model = rfpItem.product_model;
                            entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                            entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                            entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                            entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                            entryPermitItem.rfp_info.rfpRequestorTeam = rfp.GetRFPRequestorTeamId();
                            entryPermitItem.rfp_info.rfpSerial = rfp.GetRFPSerial();
                            entryPermitItem.rfp_info.rfpVersion = rfp.GetRFPVersion();
                            entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                            entryPermitItem.is_released = false;
                            entryPermitItem.stock_type.stock_type_id = stockTypes[serialStockCategory.SelectedIndex].stock_type_id;
                            entryPermitItem.valid_until = (DateTime)datePicker.SelectedDate;
                            entryPermitItem.rfp_info.rfp_item_description = rfpItem.item_description;
                            entryPermitItem.rfp_item.measure_unit_id = rfpItem.measure_unit_id;
                            entryPermitItem.rfp_item.item_notes = rfpItem.item_notes;

                            if (choiceComboBox.SelectedIndex == 1)
                            {
                                entryPermitItem.product_specs = rfpItem.product_specs;
                                entryPermitItem.is_company_product = true;
                            }

                        }
                        if (generatedSerialsStackPanel.Children.Count == 0)
                            addEntryPermitPage.materialEntryPermit.AddItem(entryPermitItem);

                    }
                    else
                    {

                        INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                        entryPermitItem.product_category.category_id = Convert.ToInt32(categoryItem.Tag);
                        entryPermitItem.product_category.category_name = categoryItem.Content.ToString();
                        entryPermitItem.product_type.type_id = Convert.ToInt32(typeItem.Tag);
                        entryPermitItem.product_type.product_name = typeItem.Content.ToString();
                        entryPermitItem.product_brand.brand_id = Convert.ToInt32(brandItem.Tag);
                        entryPermitItem.product_brand.brand_name = brandItem.Content.ToString();
                        entryPermitItem.product_model.model_id = Convert.ToInt32(modelItem.Tag);
                        entryPermitItem.product_model.model_name = modelItem.Content.ToString();
                        entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                        entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                        entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                        entryPermitItem.is_released = false;
                        entryPermitItem.stock_type.stock_type_id = Convert.ToInt32(stockTypeItem.Tag.ToString());
                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            entryPermitItem.product_specs.spec_id = Convert.ToInt32(specsItem.Tag);
                            entryPermitItem.product_specs.spec_name = specsItem.Content.ToString();
                            entryPermitItem.is_company_product = true;
                        }


                        for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                        {


                            WrapPanel serialWrapPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                            TextBox serialNumber = serialWrapPanel.Children[1] as TextBox;
                            entryPermitItem.product_serial_number = serialNumber.Text;
                            ComboBox serialStockCategory = serialWrapPanel.Children[2] as ComboBox;
                            DatePicker datePicker = serialWrapPanel.Children[3] as DatePicker;
                            if (serialStockCategory.SelectedIndex == -1)
                            {
                                System.Windows.Forms.MessageBox.Show("Serial type is not specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                return;
                            }
                            entryPermitItem.valid_until = (DateTime)datePicker.SelectedDate;
                            addEntryPermitPage.materialEntryPermit.AddItem(entryPermitItem);

                            entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                            entryPermitItem.product_category.category_id = Convert.ToInt32(categoryItem.Tag);
                            entryPermitItem.product_category.category_name = categoryItem.Content.ToString();
                            entryPermitItem.product_type.type_id = Convert.ToInt32(typeItem.Tag);
                            entryPermitItem.product_type.product_name = typeItem.Content.ToString();
                            entryPermitItem.product_brand.brand_id = Convert.ToInt32(brandItem.Tag);
                            entryPermitItem.product_brand.brand_name = brandItem.Content.ToString();
                            entryPermitItem.product_model.model_id = Convert.ToInt32(modelItem.Tag);
                            entryPermitItem.product_model.model_name = modelItem.Content.ToString();
                            entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                            entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                            entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                            entryPermitItem.is_released = false;
                            entryPermitItem.stock_type.stock_type_id = Convert.ToInt32(stockTypeItem.Tag.ToString());
                            entryPermitItem.valid_until = (DateTime)datePicker.SelectedDate;

                            if (choiceComboBox.SelectedIndex == 1)
                            {
                                entryPermitItem.product_specs.spec_id = Convert.ToInt32(specsItem.Tag);
                                entryPermitItem.product_specs.spec_name = specsItem.Content.ToString();
                                entryPermitItem.is_company_product = true;
                            }

                        }
                        if (generatedSerialsStackPanel.Children.Count == 0)
                            addEntryPermitPage.materialEntryPermit.AddItem(entryPermitItem);
                    }


                    //Border itemBorder = itemsWrapPanel.Children[i] as Border;
                    //StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                    //Label header = mainStackPanel.Children[0] as Label;

                    //ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                    //StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;


                    //CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;
                    ////rfpCheckBox.IsChecked = true;


                    //WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                    //rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                    //ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;



                    //WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                    //rfpIdWrapPanel.Visibility = Visibility.Visible;
                    //ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;



                    //WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                    //rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                    //ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[1] as ComboBox;


                    //WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                    //ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;


                    //WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                    //ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;



                    //WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                    //ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;



                    //WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                    //ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;



                    //WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                    //ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;



                    //WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                    //ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                    //WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                    //TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;



                    //WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                    //TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;



                    //Button generateSerials = endSerialWrapPanel.Children[2] as Button;


                    //WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                    //TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;



                    //WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                    //TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;



                    //WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                    //ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;


                    //WrapPanel stockWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                    //ComboBox stockComboBox = stockWrapPanel.Children[1] as ComboBox;

                    //ScrollViewer generatedSerialsScrollViewer = itemsStackPanel.Children[16] as ScrollViewer;
                    //StackPanel generatedSerialsStackPanel = generatedSerialsScrollViewer.Content as StackPanel;
                    //if the checkbox is unchecked
                    //    INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                    //    materialEntryItem.entry_permit_item_serial = i + 1;
                    //    if (workFormComboBox.SelectedIndex!=0)
                    //    {

                    //        //if the choice is generic
                    //        if (choiceComboBox.SelectedIndex == 0)
                    //        {


                    //            if (modelComboBox.SelectedIndex == -1)
                    //            {

                    //                System.Windows.Forms.MessageBox.Show("You have to choose a model", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                    //                return;
                    //            }

                    //            else
                    //            {

                    //                if (startSerialTexBox.Text == "" && endSerialTexBox.Text == "" && quantityTextBox.Text != "")
                    //                {



                    //                    // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                    //                    materialEntryItem.product_category.category_id = genericCategories[categoryComboBox.SelectedIndex].category_id;

                    //                    genericProducts.Clear();

                    //                    commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id);

                    //                    materialEntryItem.product_type.type_id = genericProducts[typecomboBox.SelectedIndex].type_id;

                    //                    genericBrands.Clear();

                    //                    commonQueries.GetGenericProductBrands(genericProducts[typecomboBox.SelectedIndex].type_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands);

                    //                    materialEntryItem.product_brand.brand_id = genericBrands[brandComboBox.SelectedIndex].brand_id;

                    //                    genericModels.Clear();

                    //                    commonQueries.GetGenericBrandModels(genericProducts[typecomboBox.SelectedIndex].type_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels);


                    //                    materialEntryItem.product_model.model_id = genericModels[modelComboBox.SelectedIndex].model_id;

                    //                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                    //                    if(currencyComboBox.SelectedIndex !=-1)
                    //                    materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //                    else
                    //                    {
                    //                        System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                        return;
                    //                    }

                    //                    materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                    //                    commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typecomboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                    //                    materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                    //                    materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;



                    //                    //if (stockTypeComboBoxMain.SelectedIndex == -1)
                    //                    //{

                    //                    //System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                    //return;

                    //                    //}

                    //                    //materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_id;
                    //                    //materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_name;
                    //                    //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;




                    //                    addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                    //                }

                    //                else
                    //                {
                    //                    int count = 1;

                    //                    for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                    //                    {


                    //                        //  INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                    //                        materialEntryItem.product_category.category_id = genericCategories[categoryComboBox.SelectedIndex].category_id;

                    //                        genericProducts.Clear();

                    //                        commonQueries.GetGenericProducts(ref genericProducts, genericCategories[categoryComboBox.SelectedIndex].category_id);

                    //                        materialEntryItem.product_type.type_id = genericProducts[typecomboBox.SelectedIndex].type_id;

                    //                        genericBrands.Clear();

                    //                        commonQueries.GetGenericProductBrands(genericProducts[typecomboBox.SelectedIndex].type_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericBrands);

                    //                        materialEntryItem.product_brand.brand_id = genericBrands[brandComboBox.SelectedIndex].brand_id;

                    //                        genericModels.Clear();

                    //                        commonQueries.GetGenericBrandModels(genericProducts[typecomboBox.SelectedIndex].type_id, genericBrands[brandComboBox.SelectedIndex].brand_id, genericCategories[categoryComboBox.SelectedIndex].category_id, ref genericModels);


                    //                        materialEntryItem.product_model.model_id = genericModels[modelComboBox.SelectedIndex].model_id;
                    //                        materialEntryItem.quantity = 0;

                    //                        materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                    //                        if (currencyComboBox.SelectedIndex != -1)
                    //                        {
                    //                            materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //                        }
                    //                        else
                    //                        {
                    //                            System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                            return;
                    //                        }

                    //                        commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typecomboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                    //                        materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                    //                        materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;




                    //                        materialEntryItem.entry_permit_item_serial = count;

                    //                        count++;

                    //                        WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                    //                        TextBox serialText = serialPanel.Children[0] as TextBox;
                    //                        ComboBox stocktypeComboBox = serialPanel.Children[1] as ComboBox;

                    //                        if (stocktypeComboBox.SelectedIndex == -1)
                    //                        {

                    //                            System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                            return;

                    //                        }

                    //                        materialEntryItem.product_serial_number = serialText.Text;



                    //                        materialEntryItem.stock_type.stock_type_id = stockTypes[stocktypeComboBox.SelectedIndex].stock_type_id;
                    //                        materialEntryItem.stock_type.stock_type_name = stockTypes[stocktypeComboBox.SelectedIndex].stock_type_name;
                    //                        //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;



                    //                        addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                    //                    }




                    //                }



                    //            }


                    //        }

                    //        //if the choice is company

                    //        else
                    //        {

                    //            if (specsComboBox.SelectedIndex == -1)
                    //            {

                    //                System.Windows.Forms.MessageBox.Show("You have to choose a spec", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                return;
                    //            }


                    //            else
                    //            {

                    //                if (startSerialTexBox.Text == "" && endSerialTexBox.Text == "" && quantityTextBox.Text != "")
                    //                {

                    //                    ///INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();


                    //                    materialEntryItem.is_company_product = true;
                    //                    materialEntryItem.product_category.category_id = companyCategories[categoryComboBox.SelectedIndex].category_id;
                    //                    companyProducts.Clear();

                    //                    commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].category_id);

                    //                    materialEntryItem.product_type.type_id = companyProducts[typecomboBox.SelectedIndex].type_id;

                    //                    companyBrands.Clear();

                    //                    commonQueries.GetProductBrands(companyProducts[typecomboBox.SelectedIndex].type_id, ref companyBrands);

                    //                    materialEntryItem.product_brand.brand_id = companyBrands[brandComboBox.SelectedIndex].brand_id;

                    //                    companyModels.Clear();

                    //                    commonQueries.GetCompanyModels(companyProducts[typecomboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels);


                    //                    materialEntryItem.product_model.model_id = companyModels[modelComboBox.SelectedIndex].model_id;

                    //                    specs.Clear();


                    //                    if (priceTextBox.Text != "")
                    //                        materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);
                    //                    if(currencyComboBox.SelectedIndex != -1)
                    //                    materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //                    else
                    //                    {
                    //                        System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                        return;
                    //                    }

                    //                    materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                    //                    commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typecomboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                    //                    materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                    //                    materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                    //                    if (stockTypeComboBox.SelectedIndex == -1)
                    //                    {

                    //                        System.Windows.Forms.MessageBox.Show("You have to choose the stock Type ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                        return;

                    //                    }

                    //                    if(stockTypeComboBox.SelectedIndex !=-1)
                    //                    {
                    //                        materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                    //                        materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                    //                    }

                    //                    //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;


                    //                    addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                    //                }

                    //                else
                    //                {
                    //                    int count = 1;

                    //                    for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                    //                    {

                    //                        // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                    //                        materialEntryItem.is_company_product = true;
                    //                        materialEntryItem.product_category.category_id = companyCategories[categoryComboBox.SelectedIndex].category_id;

                    //                        companyProducts.Clear();

                    //                        commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[categoryComboBox.SelectedIndex].category_id);

                    //                        materialEntryItem.product_type.type_id = companyProducts[typecomboBox.SelectedIndex].type_id;

                    //                        companyBrands.Clear();

                    //                        commonQueries.GetProductBrands(companyProducts[typecomboBox.SelectedIndex].type_id, ref companyBrands);

                    //                        materialEntryItem.product_brand.brand_id = companyBrands[brandComboBox.SelectedIndex].brand_id;

                    //                        companyModels.Clear();

                    //                        commonQueries.GetCompanyModels(companyProducts[typecomboBox.SelectedIndex], companyBrands[brandComboBox.SelectedIndex], ref companyModels);


                    //                        materialEntryItem.product_model.model_id = companyModels[modelComboBox.SelectedIndex].model_id;

                    //                        materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                    //                        if(currencyComboBox.SelectedIndex != -1)
                    //                        materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //                        else
                    //                        {
                    //                            System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                            return;
                    //                        }

                    //                        commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].category_id, companyProducts[typecomboBox.SelectedIndex].type_id, companyBrands[brandComboBox.SelectedIndex].brand_id, companyModels[modelComboBox.SelectedIndex].model_id, ref specs);


                    //                        materialEntryItem.product_specs.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                    //                        materialEntryItem.product_specs.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                    //                        materialEntryItem.quantity = 0;



                    //                        materialEntryItem.entry_permit_item_serial = count;

                    //                        count++;

                    //                        WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                    //                        TextBox serialText = serialPanel.Children[0] as TextBox;
                    //                        ComboBox stocktypeComBoBox = serialPanel.Children[1] as ComboBox;

                    //                        if (stocktypeComBoBox.SelectedIndex == -1)
                    //                        {

                    //                            System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                            return;

                    //                        }
                    //                         if(stockTypeComboBox.SelectedIndex != -1)
                    //                        {
                    //                            materialEntryItem.stock_type.stock_type_id = stockTypes[stocktypeComBoBox.SelectedIndex].stock_type_id;
                    //                            materialEntryItem.stock_type.stock_type_name = stockTypes[stocktypeComBoBox.SelectedIndex].stock_type_name;
                    //                        }

                    //                        //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                    //                        materialEntryItem.product_serial_number = serialText.Text;

                    //                        addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                    //                    }




                    //                }


                    //            }



                    //        }


                    //    }

                    //    //if the checkbox is checked

                    //    else
                    //    {


                    //        //////////////////////////////////////////////////
                    //        //to implement rfp items after the mapping is done
                    //        //////////////////////////////////////////////////

                    //        if (startSerialTexBox.Text == "" && endSerialTexBox.Text == "" && quantityTextBox.Text != "" && workFormComboBox.SelectedIndex==0)
                    //        {

                    //            // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                    //            materialEntryItem.rfp_info.rfpRequestorTeam = requestors[rfpRequestorTeamComboBox.SelectedIndex].requestor_team.team_id;

                    //            rfps.Clear();

                    //            commonQueries.GetTeamRFPs(ref rfps, requestors[rfpRequestorTeamComboBox.SelectedIndex].requestor_team.team_id);

                    //            materialEntryItem.rfp_info.rfpSerial = rfps[rfpIdComboBox.SelectedIndex].rfpSerial;
                    //            materialEntryItem.rfp_info.rfpVersion = rfps[rfpIdComboBox.SelectedIndex].rfpVersion;

                    //            rfpItems.Clear();

                    //            commonQueries.GetRfpItemsMapping(rfps[rfpIdComboBox.SelectedIndex].rfpSerial, rfps[rfpIdComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                    //            materialEntryItem.rfp_item_number = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].rfp_item_number;
                    //            materialEntryItem.entry_permit_item_serial = rfpItemCount;


                    //            if (priceTextBox.Text != "")
                    //                materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                    //            if(currencyComboBox.SelectedIndex != -1)
                    //            materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //            else
                    //            {
                    //                System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                return;
                    //            }

                    //            materialEntryItem.quantity = int.Parse(quantityTextBox.Text);

                    //            INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT materialReservation = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();


                    //            //materialReservation.hold_until = validityDate.DisplayDate;

                    //            materialReservation.reserved_by_id = loggedInUser.GetEmployeeId();

                    //            materialReservation.rfp_serial = materialEntryItem.rfp_info.rfpSerial;

                    //            materialReservation.rfp_version = materialEntryItem.rfp_info.rfpVersion;

                    //            materialReservation.rfp_item_no = materialEntryItem.rfp_item_number;

                    //            materialReservation.rfp_requestor_team = materialEntryItem.rfp_info.rfpRequestorTeam;

                    //            materialReservation.quantity = materialEntryItem.quantity;

                    //            materialReservation.entry_permit_item_serial = rfpItemCount;

                    //            materialReservations.Add(materialReservation);


                    //            if (rfpItems[rfpItemdescriptionComboBox.SelectedIndex].is_company_product == true)
                    //            {


                    //                materialEntryItem.product_category.category_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_category.category_id;
                    //                materialEntryItem.product_type.type_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_type.type_id;
                    //                materialEntryItem.product_brand.brand_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_brand.brand_id;
                    //                materialEntryItem.product_model.model_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_model.model_id;
                    //                materialEntryItem.product_specs.spec_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_specs.spec_id;


                    //            }

                    //            else
                    //            {

                    //                materialEntryItem.product_category.category_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_category.category_id;
                    //                materialEntryItem.product_type.type_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_type.type_id;
                    //                materialEntryItem.product_brand.brand_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_brand.brand_id;
                    //                materialEntryItem.product_model.model_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_model.model_id;


                    //            }


                    //            if (stockTypeComboBox.SelectedIndex == -1)
                    //            {

                    //                System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                return;

                    //            }

                    //            if(stockTypeComboBox.SelectedIndex !=-1)
                    //            {
                    //                materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                    //                materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                    //            }



                    //            addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                    //            rfpItemCount++;


                    //        }

                    //        else
                    //        {

                    //            for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                    //            {


                    //                // INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT materialEntryItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();

                    //                materialEntryItem.rfp_info.rfpRequestorTeam = requestors[rfpRequestorTeamComboBox.SelectedIndex].requestor_team.team_id;

                    //                rfps.Clear();

                    //                commonQueries.GetTeamRFPs(ref rfps, requestors[rfpRequestorTeamComboBox.SelectedIndex].requestor_team.team_id);

                    //                materialEntryItem.rfp_info.rfpSerial = rfps[rfpIdComboBox.SelectedIndex].rfpSerial;
                    //                materialEntryItem.rfp_info.rfpVersion = rfps[rfpIdComboBox.SelectedIndex].rfpVersion;

                    //                rfpItems.Clear();

                    //                commonQueries.GetRfpItemsMapping(rfps[rfpIdComboBox.SelectedIndex].rfpSerial, rfps[rfpIdComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                    //                materialEntryItem.rfp_item_number = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].rfp_item_number;


                    //                if (priceTextBox.Text != "")
                    //                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                    //                if(currencyComboBox.SelectedIndex !=-1)
                    //                materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;
                    //                else
                    //                {
                    //                    System.Windows.Forms.MessageBox.Show("Please choose currency", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                    return;
                    //                }
                    //                materialEntryItem.quantity = 1;



                    //                materialEntryItem.entry_permit_item_serial = rfpItemCount;


                    //                INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT materialReservation = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();


                    //                //  materialReservation.hold_until = validityDate.DisplayDate;

                    //                materialReservation.reserved_by_id = loggedInUser.GetEmployeeId();

                    //                materialReservation.rfp_serial = materialEntryItem.rfp_info.rfpSerial;

                    //                materialReservation.rfp_version = materialEntryItem.rfp_info.rfpVersion;

                    //                materialReservation.rfp_item_no = materialEntryItem.rfp_item_number;

                    //                materialReservation.rfp_requestor_team = materialEntryItem.rfp_info.rfpRequestorTeam;

                    //                materialReservation.quantity = materialEntryItem.quantity;

                    //                materialReservation.entry_permit_item_serial = rfpItemCount;

                    //                materialReservations.Add(materialReservation);

                    //                rfpItemCount++;


                    //                WrapPanel serialPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                    //                TextBox serialText = serialPanel.Children[0] as TextBox;

                    //                ComboBox stockTypeComBoBox = serialPanel.Children[1] as ComboBox;

                    //                if (stockTypeComBoBox.SelectedIndex == -1)
                    //                {

                    //                    System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //                    return;

                    //                }

                    //                materialEntryItem.product_serial_number = serialText.Text;


                    //                materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_id;
                    //                materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComBoBox.SelectedIndex].stock_type_name;
                    //                //materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                    //                if (rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_category.category_name == "")
                    //                {
                    //                    materialEntryItem.product_category.category_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_category.category_id;
                    //                    materialEntryItem.product_type.type_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_type.type_id;
                    //                    materialEntryItem.product_brand.brand_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_brand.brand_id;
                    //                    materialEntryItem.product_model.model_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_model.model_id;
                    //                    materialEntryItem.product_specs.spec_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_specs.spec_id;



                    //                }

                    //                else
                    //                {

                    //                    materialEntryItem.product_category.category_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_category.category_id;
                    //                    materialEntryItem.product_type.type_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_type.type_id;
                    //                    materialEntryItem.product_brand.brand_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_brand.brand_id;
                    //                    materialEntryItem.product_model.model_id = rfpItems[rfpItemdescriptionComboBox.SelectedIndex].product_model.model_id;


                    //                }


                    //                addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                    //            }


                    //        }

                    //    }

                    //}


                    //    if (viewAddCondition != COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION)
                    //    {
                    //        if (!addEntryPermitPage.materialEntryPermit.IssueNewEntryPermit())
                    //            return;


                    //        if (materialReservations.Count != 0)
                    //        {

                    //            for (int i = 0; i < materialReservations.Count; i++)
                    //            {
                    //                INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservationItem = materialReservations[i];
                    //                reservationItem.entry_permit_serial = addEntryPermitPage.materialEntryPermit.GetEntryPermitSerial();

                    //                materialReservations[i] = reservationItem;
                    //            }

                    //            MaterialReservation reservation = new MaterialReservation();
                    //            reservation.SetReservationDate(Convert.ToDateTime(addEntryPermitPage.TransactionDatePicker.SelectedDate));

                    //            reservation.SetReservedBy(loggedInUser.GetEmployeeId());
                    //            reservation.SetRfpSerial(materialReservations[0].rfp_serial);

                    //            reservation.SetReservationStatusId(COMPANY_WORK_MACROS.PENDING_RESERVATION);


                    //            if (!reservation.IssueMultipleReservations(materialReservations))
                    //                return;
                    //        }

                    //    }

                    //    else

                    //    {
                    //        addEntryPermitPage.materialEntryPermit.UpdateMaterialEntryPermit(oldMaterialEntryPermit);
                    //    }

                    //    addEntryPermitPage.WareHouseCombo.IsEnabled = false;
                    //    addEntryPermitPage.TransactionDatePicker.IsEnabled = false;

                    //for (int i = 0; i < itemsWrapPanel.Children.Count-1; i++)
                    //{

                    //    //Border itemBorder = itemsWrapPanel.Children[i] as Border;
                    //    //StackPanel mainStackPanel = itemBorder.Child as StackPanel;
                    //    //Label header = mainStackPanel.Children[0] as Label;
                    //    //ScrollViewer itemScrollViewer = mainStackPanel.Children[1] as ScrollViewer;
                    //    //StackPanel itemsStackPanel = itemScrollViewer.Content as StackPanel;
                    //    //CheckBox rfpCheckBox = itemsStackPanel.Children[0] as CheckBox;
                    //    //rfpCheckBox.IsEnabled = false;

                    //    //WrapPanel rfpRequestorsWrapPanel = itemsStackPanel.Children[1] as WrapPanel;
                    //    //rfpRequestorsWrapPanel.Visibility = Visibility.Visible;
                    //    //ComboBox rfpRequestorsComboBox = rfpRequestorsWrapPanel.Children[1] as ComboBox;
                    //    //rfpRequestorsComboBox.IsEnabled = false;

                    //    //WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                    //    //rfpIdWrapPanel.Visibility = Visibility.Visible;
                    //    //ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;
                    //    //rfpIdComboBox.IsEnabled = false;

                    //    //WrapPanel rfpDescriptipnWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                    //    //rfpDescriptipnWrapPanel.Visibility = Visibility.Visible;
                    //    //ComboBox rfpDescriptionComboBox = rfpDescriptipnWrapPanel.Children[1] as ComboBox;
                    //    //rfpDescriptionComboBox.IsEnabled = true;



                    //    //WrapPanel choiseWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                    //    //ComboBox choiceComboBox = choiseWrapPanel.Children[1] as ComboBox;


                    //    //WrapPanel categoryWrapPanel = itemsStackPanel.Children[5] as WrapPanel;
                    //    //ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;
                    //    //categoryComboBox.IsEnabled = false;

                    //    //WrapPanel typeWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                    //    //ComboBox typeComboBox = typeWrapPanel.Children[1] as ComboBox;
                    //    //typeComboBox.IsEnabled = false;

                    //    //WrapPanel brandWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                    //    //ComboBox brandComboBox = brandWrapPanel.Children[1] as ComboBox;
                    //    //brandComboBox.IsEnabled = false;

                    //    //WrapPanel modelWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                    //    //ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;
                    //    //modelComboBox.IsEnabled = false;

                    //    //WrapPanel specsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                    //    //ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                    //    //specsComboBox.IsEnabled = false;


                    //    //WrapPanel startSerialWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                    //    //TextBox startSerialTextBox = startSerialWrapPanel.Children[1] as TextBox;
                    //    //startSerialTextBox.IsEnabled = false;

                    //    //WrapPanel endSerialWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                    //    //TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;
                    //    //endSerialTextBox.IsEnabled = false;

                    //    //Button generateSerials = endSerialWrapPanel.Children[2] as Button;
                    //    //generateSerials.IsEnabled = false;

                    //    //WrapPanel quantityWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                    //    //TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;
                    //    //quantityTextBox.IsEnabled = false;

                    //    //WrapPanel priceWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                    //    //TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;
                    //    //priceTextBox.IsEnabled = false;

                    //    //WrapPanel currencyWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                    //    //ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;
                    //    //currencyComboBox.IsEnabled = false;

                    //    //WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                    //    //ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;
                    //    //stockTypeComboBox.IsEnabled = false;
                }
                if (viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION)
                {
                    if (!addEntryPermitPage.materialEntryPermit.IssueNewEntryPermit())
                        return;


                    for (int i = 0; i < addEntryPermitPage.materialEntryPermit.GetItems().Count; i++)
                    {

                        if (addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpSerial == 0)
                            continue;

                        INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservationItem = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();
                        reservationItem.entry_permit_serial = addEntryPermitPage.materialEntryPermit.GetEntryPermitSerial();
                        reservationItem.entry_permit_item_serial = addEntryPermitPage.materialEntryPermit.GetItems()[i].entry_permit_item_serial;
                        reservationItem.rfp_serial = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpSerial;
                        reservationItem.rfp_version = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpVersion;
                        reservationItem.rfp_item_no = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_item_number;
                        reservationItem.quantity = addEntryPermitPage.materialEntryPermit.GetItems()[i].quantity;
                        reservationItem.rfp_requestor_team = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpRequestorTeam;
                        reservationItem.reserved_by_id = loggedInUser.GetEmployeeId();
                        reservationItem.reservation_date = DateTime.Now;
                        reservationItem.hold_until = addEntryPermitPage.materialEntryPermit.GetItems()[i].valid_until;
                        reservationItem.rfp_item_description = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfp_item_description;
                        reservationItem.rfp_item.measure_unit_id = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_item.measure_unit_id;
                        reservationItem.rfp_item_notes = addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_item.item_notes;

                        materialReservations.Add(reservationItem);
                        reservation.SetRfpSerial(addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpSerial);
                        reservation.SetRfpRequseterTeamId(addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpRequestorTeam);
                        reservation.SetRfpVersion(addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_info.rfpVersion);
                        reservation.SetRfpItemNumber(addEntryPermitPage.materialEntryPermit.GetItems()[i].rfp_item_number);
                        reservation.SetEntryPermitSerial(addEntryPermitPage.materialEntryPermit.GetEntryPermitSerial());

                    }
                    if (!reservation.IssueMultipleReservations(materialReservations))
                        return;
                }



            }
            else
            {
                for (int i = 0; i < itemsWrapPanel.Children.Count-1; i++)
                {
                    Border itemBorder = itemsWrapPanel.Children[i] as Border;


                    StackPanel itemsStackPanel = itemBorder.Child as StackPanel;

                    WrapPanel workFormWrapPanel = itemsStackPanel.Children[2] as WrapPanel;
                    ComboBox workFormComboBox = workFormWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpRequestorTeamWrapPanel = itemsStackPanel.Children[3] as WrapPanel;
                    ComboBox rfpRequestorTeamComboBox = rfpRequestorTeamWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpIdWrapPanel = itemsStackPanel.Children[4] as WrapPanel;
                    ComboBox rfpIdComboBox = rfpIdWrapPanel.Children[1] as ComboBox;

                    WrapPanel rfpItemDescription = itemsStackPanel.Children[5] as WrapPanel;
                    TextBlock rfpItemDescriptionLabel = rfpItemDescription.Children[0] as TextBlock;
                    ComboBox rfpItemdescriptionComboBox = rfpItemDescription.Children[1] as ComboBox;

                    WrapPanel choiceWrapPanel = itemsStackPanel.Children[6] as WrapPanel;
                    ComboBox choiceComboBox = choiceWrapPanel.Children[1] as ComboBox;

                    WrapPanel categoryWrapPanel = itemsStackPanel.Children[7] as WrapPanel;
                    ComboBox categoryComboBox = categoryWrapPanel.Children[1] as ComboBox;

                    WrapPanel typeWrapPanel = itemsStackPanel.Children[8] as WrapPanel;
                    ComboBox typecomboBox = typeWrapPanel.Children[1] as ComboBox;

                    WrapPanel brandsWrapPanel = itemsStackPanel.Children[9] as WrapPanel;
                    ComboBox brandComboBox = brandsWrapPanel.Children[1] as ComboBox;

                    WrapPanel modelWrapPanel = itemsStackPanel.Children[10] as WrapPanel;
                    ComboBox modelComboBox = modelWrapPanel.Children[1] as ComboBox;

                    WrapPanel specsWrapPanel = itemsStackPanel.Children[11] as WrapPanel;
                    ComboBox specsComboBox = specsWrapPanel.Children[1] as ComboBox;

                    WrapPanel productSerialWrapPanel = itemsStackPanel.Children[12] as WrapPanel;
                    TextBox productSerialTextBox = productSerialWrapPanel.Children[1] as TextBox;

                    WrapPanel quantityWrapPanel = itemsStackPanel.Children[13] as WrapPanel;
                    TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;

                    WrapPanel priceWrapPanel = itemsStackPanel.Children[14] as WrapPanel;
                    TextBox priceTextBox = priceWrapPanel.Children[1] as TextBox;

                    WrapPanel currencyWrapPanel = itemsStackPanel.Children[15] as WrapPanel;
                    ComboBox currencyComboBox = currencyWrapPanel.Children[1] as ComboBox;

                    WrapPanel stockTypeWrapPanel = itemsStackPanel.Children[16] as WrapPanel;
                    ComboBox stockTypeComboBox = stockTypeWrapPanel.Children[1] as ComboBox;

                    if (categoryComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Category is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (typecomboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Type is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (brandComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Brand is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (modelComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Model is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (choiceComboBox.SelectedIndex == 1)
                        if (specsComboBox.SelectedIndex == -1)
                        {
                            System.Windows.Forms.MessageBox.Show("Specs is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                    if (quantityTextBox.Text == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Quantity is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (priceTextBox.Text == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Price is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (currencyComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Currency is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (stockTypeComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Stock is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }

                    ComboBoxItem categoryItem = (ComboBoxItem)categoryComboBox.SelectedItem;
                    ComboBoxItem typeItem = (ComboBoxItem)typecomboBox.SelectedItem;
                    ComboBoxItem brandItem = (ComboBoxItem)brandComboBox.SelectedItem;
                    ComboBoxItem modelItem = (ComboBoxItem)modelComboBox.SelectedItem;

                    ComboBoxItem currencyItem = (ComboBoxItem)currencyComboBox.SelectedItem;
                    ComboBoxItem stockTypeItem = (ComboBoxItem)stockTypeComboBox.SelectedItem;

                    ComboBoxItem specsItem = (ComboBoxItem)specsComboBox.SelectedItem;


                    if (workFormComboBox.SelectedIndex == 0)
                    {
                        ComboBoxItem descriptionItem = (ComboBoxItem)rfpItemdescriptionComboBox.SelectedItem;
                        RFP_ITEM_MAX_STRUCT rfpItem = (RFP_ITEM_MAX_STRUCT)descriptionItem.Tag;
                        rfpItem.product_category.category_id = Convert.ToInt32(categoryItem.Tag);
                        rfpItem.product_category.category_name = categoryItem.Content.ToString();
                        rfpItem.product_type.type_id = Convert.ToInt32(typeItem.Tag);
                        rfpItem.product_type.product_name = typeItem.Content.ToString();
                        rfpItem.product_brand.brand_id = Convert.ToInt32(brandItem.Tag);
                        rfpItem.product_brand.brand_name = brandItem.Content.ToString();
                        rfpItem.product_model.model_id = Convert.ToInt32(modelItem.Tag);
                        rfpItem.product_model.model_name = modelItem.Content.ToString();
                        RFP rfp = (RFP)rfpItemDescriptionLabel.Tag;
                        //rfp.rfpItems.Clear();
                        //if (!rfp.InitializeRFP(rfp.GetRFPRequestorTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                        //return;
                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            specsItem = (ComboBoxItem)specsComboBox.SelectedItem;
                            rfpItem.product_specs.spec_id = Convert.ToInt32(specsItem.Tag);
                            rfpItem.product_specs.spec_name = specsItem.Content.ToString();
                            rfpItem.is_company_product = true;
                            rfpItem.item_description = $@"{categoryItem.Content.ToString()} - {typeItem.Content.ToString()} - {brandItem.Content.ToString()} - {modelItem.Content.ToString()} - {specsItem.Content.ToString()}";

                        }
                        else
                        {
                            rfpItem.is_company_product = false;
                            rfpItem.item_description = $@"{categoryItem.Content.ToString()} - {typeItem.Content.ToString()} - {brandItem.Content.ToString()} - {modelItem.Content.ToString()} ";
                        }

                        //RFP_ITEM_MAX_STRUCT oldRfpItem = rfp.rfpItems.Find(f => f.rfp_item_number == rfpItem.rfp_item_number);
                        //int itemIndex = rfp.rfpItems.IndexOf(oldRfpItem);
                        //rfp.rfpItems.Remove(oldRfpItem);
                        //rfp.rfpItems.Insert(itemIndex, rfpItem);
                        //oldRfp.rfpItems.Clear();

                        //if (!oldRfp.InitializeRFP(rfp.GetRFPRequestorTeamId(), rfp.GetRFPSerial(), rfp.GetRFPVersion()))
                        //    return;
                        //if (!rfp.EditRFPItems(ref oldRfp.rfpItems))
                        //    return;



                        INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                        entryPermitItem.product_category = rfpItem.product_category;
                        entryPermitItem.product_type = rfpItem.product_type;
                        entryPermitItem.product_brand = rfpItem.product_brand;
                        entryPermitItem.product_model = rfpItem.product_model;
                        entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                        entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                        entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                        entryPermitItem.rfp_info.rfpRequestorTeam = rfp.GetRFPRequestorTeamId();
                        entryPermitItem.rfp_info.rfpSerial = rfp.GetRFPSerial();
                        entryPermitItem.rfp_info.rfpVersion = rfp.GetRFPVersion();
                        entryPermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        entryPermitItem.is_released = false;
                        entryPermitItem.stock_type.stock_type_id = Convert.ToInt32(stockTypeItem.Tag.ToString());
                        entryPermitItem.rfp_info.rfp_item_description = rfpItem.item_description;
                        entryPermitItem.rfp_item.measure_unit_id = rfpItem.measure_unit_id;
                        entryPermitItem.rfp_item.item_notes = rfpItem.item_notes;
                        entryPermitItem.product_serial_number = productSerialTextBox.Text;


                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            entryPermitItem.product_specs = rfpItem.product_specs;
                            entryPermitItem.is_company_product = true;
                        }

                        addEntryPermitPage.materialEntryPermit.AddItem(entryPermitItem);


                    }
                    else
                    {

                        INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = new INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT();
                        entryPermitItem.product_category.category_id = Convert.ToInt32(categoryItem.Tag);
                        entryPermitItem.product_category.category_name = categoryItem.Content.ToString();
                        entryPermitItem.product_type.type_id = Convert.ToInt32(typeItem.Tag);
                        entryPermitItem.product_type.product_name = typeItem.Content.ToString();
                        entryPermitItem.product_brand.brand_id = Convert.ToInt32(brandItem.Tag);
                        entryPermitItem.product_brand.brand_name = brandItem.Content.ToString();
                        entryPermitItem.product_model.model_id = Convert.ToInt32(modelItem.Tag);
                        entryPermitItem.product_model.model_name = modelItem.Content.ToString();
                        entryPermitItem.quantity = Convert.ToInt32(Convert.ToDecimal(quantityTextBox.Text.ToString()));
                        entryPermitItem.item_price = Convert.ToDecimal(priceTextBox.Text.ToString());
                        entryPermitItem.item_currency.currencyId = Convert.ToInt32(currencyItem.Tag.ToString());
                        entryPermitItem.is_released = false;
                        entryPermitItem.stock_type.stock_type_id = Convert.ToInt32(stockTypeItem.Tag.ToString());
                        entryPermitItem.product_serial_number = productSerialTextBox.Text;
                        if (choiceComboBox.SelectedIndex == 1)
                        {
                            entryPermitItem.product_specs.spec_id = Convert.ToInt32(specsItem.Tag);
                            entryPermitItem.product_specs.spec_name = specsItem.Content.ToString();
                            entryPermitItem.is_company_product = true;
                        }

                    }
                }
                if (!addEntryPermitPage.materialEntryPermit.UpdateMaterialEntryPermit(oldMaterialEntryPermit))
                    return;
            }
            
           
            EntryPermitUploadFilesPage = new EntryPermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, addEntryPermitPage, entryPermitWindow, ref addEntryPermitPage.materialEntryPermit);


                this.NavigationService.Navigate(EntryPermitUploadFilesPage);

                this.nextButton.IsEnabled = true;
                this.finishButton.IsEnabled = false;

            
        }
        private void OnButtonClickAddNewCard (object sender, RoutedEventArgs e)
        {
            itemsWrapPanel.Children.RemoveAt(itemsWrapPanel.Children.Count - 1);
            itemNumber++;
            if(viewAddCondition==COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION)
            {
                InitializeNewCard(itemNumber);
                InitializeGenerateSerialCard();
                addNewItemButton();
            }
            else
            {
                InitializeNewCard(itemNumber);
                addNewItemButton();
            }
            
       
          
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
            Border mainBorder = itemsStackPanel.Parent as Border;
            
            itemNumber--;
            int position=itemsWrapPanel.Children.IndexOf(mainBorder);
            itemsWrapPanel.Children.Remove(mainBorder);
            if(viewAddCondition!=COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION)
            {
                itemsWrapPanel.Children.RemoveRange(position, 1);
                int itemNumberr = 1;
                for (int i = 0; i < itemsWrapPanel.Children.Count - 1; i += 2)
                {
                    Border itemsBorder = itemsWrapPanel.Children[i] as Border;
                    StackPanel maxItemsStackPanel = itemsBorder.Child as StackPanel;
                    Label header = maxItemsStackPanel.Children[0] as Label;
                    header.Content = $"Item {itemNumberr}";
                    itemNumberr++;
                }
            }
            else
            {
                for (int i = 0; i < itemsWrapPanel.Children.Count - 1; i ++)
                {
                    int itemNumberr = 1;
                    Border itemsBorder = itemsWrapPanel.Children[i] as Border;
                    StackPanel maxItemsStackPanel = itemsBorder.Child as StackPanel;
                    Label header = maxItemsStackPanel.Children[0] as Label;
                    header.Content = $"Item {itemNumberr}";
                    itemNumberr++;
                }

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
            if (!commonQueries.GetRFPRequestorsHaveItemsRevised(ref requestors))
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
