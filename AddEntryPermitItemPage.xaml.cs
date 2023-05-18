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

        bool editable = false;
        private AddEntryPermitWindow entryPermitWindow;
        public AddEntryPermitPage addEntryPermitPage;

        public MaterialEntryPermit oldMaterialEntryPermit = null;

        public EntryPermitUploadFilesPage EntryPermitUploadFilesPage=null;

        List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> genericCategories = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY>();
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> genericProducts = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS>();
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> genericBrands = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL> genericModels = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL>();

        List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> companyCategories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> companyProducts = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        List<COMPANY_WORK_MACROS.BRAND_STRUCT> companyBrands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        List<COMPANY_WORK_MACROS.MODEL_STRUCT> companyModels = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
        List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requesters = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();

        List<PROCUREMENT_STRUCTS.RFPS_MIN_STRUCT> rfps = new List<PROCUREMENT_STRUCTS.RFPS_MIN_STRUCT>();

        List<PROCUREMENT_STRUCTS.RFP_ITEM_MAPPING> rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MAPPING>();

        List<BASIC_STRUCTS.STOCK_TYPES> stockTypes = new List<BASIC_STRUCTS.STOCK_TYPES>();

        List<COMPANY_WORK_MACROS.SPEC_STRUCT> specs = new List<COMPANY_WORK_MACROS.SPEC_STRUCT>();

        List<MATERIAL_RESERVATION_MED_STRUCT> materialReservations = new List<MATERIAL_RESERVATION_MED_STRUCT>();


        public AddEntryPermitItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddEntryPermitWindow mEntryPermitWindow,bool isedit, ref MaterialEntryPermit moldMaterialEntryPermit) 
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            editable = isedit;

            entryPermitWindow = mEntryPermitWindow;
            
            oldMaterialEntryPermit = moldMaterialEntryPermit;
            
            InitializeComponent();
            InitializeNewCard();


            commonQueries.GetStockCategories(ref stockTypes);
        }


        public List<BASIC_STRUCTS.CURRENCY_STRUCT> GetCurrencies()
        {
            return currencies;
        }

        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MAPPING> GetRfpItems()
        {
            return rfpItems;
        }

        public List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> GetRequsters() 
        {
            return requesters;
        }

        public List<PROCUREMENT_STRUCTS.RFPS_MIN_STRUCT> GetRfps()
        {

            return rfps;

        }

        public void InitializeNewCard()
        {
            Home.RowDefinitions.Add(new RowDefinition());

            Grid card = new Grid() { Background = Brushes.White,Margin=new Thickness(25)};
            card.HorizontalAlignment = HorizontalAlignment.Stretch;
            card.VerticalAlignment = VerticalAlignment.Stretch;

            Grid.SetRow(card, Home.RowDefinitions.Count - 1);

            card.Tag = Home.RowDefinitions.Count;
            card.ColumnDefinitions.Add(new ColumnDefinition());
            card.ColumnDefinitions.Add(new ColumnDefinition());

            Grid gridSerials = new Grid();

           
            
            ScrollViewer scroll = new ScrollViewer() { Height=700,Margin=new Thickness(0,100,0,0)};

            scroll.Content = gridSerials;

            Grid.SetColumn(scroll, 1);
            Grid.SetRowSpan(scroll, 11);


            card.RowDefinitions.Add(new RowDefinition(){ Height=new GridLength(60)});
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());

            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());

            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());






            Grid header = new Grid();
            BrushConverter converter = new BrushConverter();

            Label numberLabel = new Label();

            numberLabel.Content = $"{"Item "+Home.RowDefinitions.Count}";
            numberLabel.Style = (Style)FindResource("tableHeaderItem");

            header.Background = (Brush)converter.ConvertFrom("#105A97");

            header.Children.Add(numberLabel);

            Grid.SetRow(header, 0);
            Grid.SetColumnSpan(header, 2);
            card.Children.Add(header);


            WrapPanel rfpPanel = new WrapPanel();

            Label rfpLabel = new Label();

            rfpLabel.Content = "RFP";

            rfpLabel.Style= (Style)FindResource("tableItemLabel");


            CheckBox rfpCheckBox = new CheckBox();


            rfpCheckBox.Style= (Style)FindResource("checkBoxStyle");

            rfpCheckBox.Checked += RfpCheckBoxChecked;

            rfpCheckBox.Unchecked += RfpCheckBoxUnchecked;


            rfpPanel.Children.Add(rfpLabel);

            rfpPanel.Children.Add(rfpCheckBox);


            Grid.SetRow(rfpPanel, 1);
            card.Children.Add(rfpPanel);



            WrapPanel rfpRequstorPanel = new WrapPanel();

            Label rfpRequestorLabel = new Label();

            rfpRequestorLabel.Content = "Requester Team";

            rfpRequestorLabel.Style = (Style)FindResource("tableItemLabel");


            ComboBox rfpRequstorComboBox = new ComboBox();


            rfpRequstorComboBox.Style = (Style)FindResource("miniComboBoxStyle");

            rfpRequstorComboBox.SelectionChanged += RfpRequstorComboBoxSelectionChanged;

            rfpRequstorComboBox.IsEnabled = false;

            requesters.Clear();
            commonQueries.GetRFPRequestors(ref requesters);


            requesters.ForEach(a => rfpRequstorComboBox.Items.Add(a.employee_team));
            

            rfpRequstorPanel.Children.Add(rfpRequestorLabel);

            rfpRequstorPanel.Children.Add(rfpRequstorComboBox);


            Grid.SetRow(rfpRequstorPanel, 2);
            card.Children.Add(rfpRequstorPanel);

            ComboBox rfpSerialComboBox = new ComboBox();


            rfpSerialComboBox.SelectionChanged += RfpSerialComboBoxSelectionChanged;

            rfpSerialComboBox.Style = (Style)FindResource("miniComboBoxStyle");

            rfpSerialComboBox.IsEnabled = false;

            rfpSerialComboBox.Margin = new Thickness(73, 0, 0, 0);

            rfpRequstorPanel.Children.Add(rfpSerialComboBox);




            WrapPanel rfpItemDescriptionPanel = new WrapPanel();

            Label rfpItemDescriptionLabel = new Label();

            rfpItemDescriptionLabel.Content = "Item Description";

            rfpItemDescriptionLabel.Style = (Style)FindResource("tableItemLabel");


            ComboBox rfpItemDescriptionComboBox = new ComboBox();


            rfpItemDescriptionComboBox.IsEnabled = false;
            rfpItemDescriptionComboBox.Style = (Style)FindResource("comboBoxStyle");

            rfpItemDescriptionComboBox.SelectionChanged += OnRfpItemDescriptionComboBoxSelectionChanged;

            rfpItemDescriptionPanel.Children.Add(rfpItemDescriptionLabel);

            rfpItemDescriptionPanel.Children.Add(rfpItemDescriptionComboBox);


            Grid.SetRow(rfpItemDescriptionPanel, 3);

            card.Children.Add(rfpItemDescriptionPanel);



            WrapPanel choicePanel = new WrapPanel();


            Label choiceLabel = new Label();
            choiceLabel.Content = "Choose Type";
            choiceLabel.Style = (Style)FindResource("tableItemLabel");
            ComboBox choiceComboBox = new ComboBox();

            choiceComboBox.Style = (Style)FindResource("comboBoxStyle");
            choiceComboBox.SelectionChanged += ChoiceComboBoxSelectionChanged;

            choiceComboBox.Items.Add("Generic Product");
            choiceComboBox.Items.Add("Company Product");


            choicePanel.Children.Add(choiceLabel);
            choicePanel.Children.Add(choiceComboBox);

            Grid.SetRow(choicePanel, 4);

            card.Children.Add(choicePanel);


            WrapPanel wrapPanel1 = new WrapPanel();
            Label genericCategory = new Label();
            genericCategory.Content = "Generic Category";
            genericCategory.Style = (Style)FindResource("tableItemLabel");
            ComboBox genericCategoryComoBox = new ComboBox();
            genericCategoryComoBox.IsEnabled = false;

            genericCategoryComoBox.Style= (Style)FindResource("comboBoxStyle");
            genericCategoryComoBox.SelectionChanged += GenericCategoryComoBoxSelectionChanged;
            genericCategories.Clear();
            commonQueries.GetGenericProductCategories(ref genericCategories);
            genericCategories.ForEach(a=> genericCategoryComoBox.Items.Add(a.category_name));
            wrapPanel1.Children.Add(genericCategory);
            wrapPanel1.Children.Add(genericCategoryComoBox);


            Grid.SetRow(wrapPanel1, 5);
           
            card.Children.Add(wrapPanel1);

            WrapPanel wrapPanel2 = new WrapPanel();


            Label genericProduct = new Label();
            genericProduct.Content = "Generic Product";
            genericProduct.Style = (Style)FindResource("tableItemLabel");

            ComboBox genericProductComoBox = new ComboBox();
            genericProductComoBox.SelectionChanged += GenericProductComoBoxSelectionChanged;
            genericProductComoBox.IsEnabled = false;


            genericProductComoBox.Style = (Style)FindResource("comboBoxStyle");

            wrapPanel2.Children.Add(genericProduct);
            wrapPanel2.Children.Add(genericProductComoBox);

            card.Children.Add(wrapPanel2);

            Grid.SetRow(wrapPanel2, 6);



            WrapPanel wrapPanel3 = new WrapPanel();

            Label genericBrand = new Label();
            genericBrand.Content = "Generic Brand";
            genericBrand.Style = (Style)FindResource("tableItemLabel");

            ComboBox genericBrandComoBox = new ComboBox();
            genericBrandComoBox.IsEnabled = false;


            genericBrandComoBox.SelectionChanged += GenericBrandComoBoxSelectionChanged;

            genericBrandComoBox.Style = (Style)FindResource("comboBoxStyle");



            wrapPanel3.Children.Add(genericBrand);
            wrapPanel3.Children.Add(genericBrandComoBox);

            card.Children.Add(wrapPanel3);

            Grid.SetRow(wrapPanel3, 7);


            WrapPanel wrapPanel4 = new WrapPanel();


            Label genericModel = new Label();
            genericModel.Content = "Generic Model";
            genericModel.Style = (Style)FindResource("tableItemLabel");

            ComboBox genericModelComboBox = new ComboBox();
            genericModelComboBox.IsEnabled = false;

            genericModelComboBox.Style = (Style)FindResource("comboBoxStyle");

            wrapPanel4.Children.Add(genericModel);
            wrapPanel4.Children.Add(genericModelComboBox);

            card.Children.Add(wrapPanel4);

            Grid.SetRow(wrapPanel4, 8);

            WrapPanel wrapPanel5 = new WrapPanel();

            Label companyCategory = new Label();
            companyCategory.Content = "Company Category";
            companyCategory.Style = (Style)FindResource("tableItemLabel");


            ComboBox companyCategoryComoBox = new ComboBox();

            companyCategoryComoBox.IsEnabled = false;

            companyCategories.Clear();
            commonQueries.GetProductCategories(ref companyCategories);

            companyCategories.ForEach(a => companyCategoryComoBox.Items.Add(a.category));
            companyCategoryComoBox.Style = (Style)FindResource("comboBoxStyle");

            companyCategoryComoBox.SelectionChanged += CompanyCategoryComoBoxSelectionChanged;



            wrapPanel5.Children.Add(companyCategory);
            wrapPanel5.Children.Add(companyCategoryComoBox);

            card.Children.Add(wrapPanel5);

            Grid.SetRow(wrapPanel5, 9);


            WrapPanel wrapPanel6 = new WrapPanel();

            Label companyProduct = new Label();
            companyProduct.Content = "Company Product";
            companyProduct.Style = (Style)FindResource("tableItemLabel");
            

            ComboBox companyProductComoBox = new ComboBox();

            companyProductComoBox.IsEnabled = false;

            companyProductComoBox.SelectionChanged += CompanyProductComoBoxSelectionChanged;
            



            companyProductComoBox.Style = (Style)FindResource("comboBoxStyle");


            wrapPanel6.Children.Add(companyProduct);
            wrapPanel6.Children.Add(companyProductComoBox);

            card.Children.Add(wrapPanel6);

            Grid.SetRow(wrapPanel6, 10);


            WrapPanel wrapPanel7 = new WrapPanel();


            Label companyBrand = new Label();
            companyBrand.Content = "Company Brand";
            companyBrand.Style = (Style)FindResource("tableItemLabel");


            ComboBox companyBrandComoBox = new ComboBox();

            companyBrandComoBox.IsEnabled = false;
            companyBrandComoBox.SelectionChanged += CompanyBrandComoBoxSelectionChanged;


            companyBrandComoBox.Style = (Style)FindResource("comboBoxStyle");



            wrapPanel7.Children.Add(companyBrand);
            wrapPanel7.Children.Add(companyBrandComoBox);

            card.Children.Add(wrapPanel7);

            Grid.SetRow(wrapPanel7, 11);



            WrapPanel wrapPanel8 = new WrapPanel();


            Label companyModel = new Label();
            companyModel.Content = "Company Model";
            companyModel.Style = (Style)FindResource("tableItemLabel");

            ComboBox companyModelComoBox = new ComboBox();

            companyModelComoBox.IsEnabled = false;
            companyModelComoBox.Style = (Style)FindResource("comboBoxStyle");
            companyModelComoBox.SelectionChanged += OnCompanyModelComoBoxSelectionChanged;


            wrapPanel8.Children.Add(companyModel);
            wrapPanel8.Children.Add(companyModelComoBox);

            card.Children.Add(wrapPanel8);

            Grid.SetRow(wrapPanel8, 12);



            WrapPanel wrapPanel9 = new WrapPanel();


            Label companySpecs = new Label();
            companySpecs.Content = "Specs";
            companySpecs.Style = (Style)FindResource("tableItemLabel");

            ComboBox specsComoBox = new ComboBox();

            specsComoBox.IsEnabled = false;
            specsComoBox.Style = (Style)FindResource("comboBoxStyle");


            wrapPanel9.Children.Add(companySpecs);
            wrapPanel9.Children.Add(specsComoBox);

            card.Children.Add(wrapPanel9);

            wrapPanel9.Visibility = Visibility.Collapsed;

            Grid.SetRow(wrapPanel9, 13);


            WrapPanel wrapPanel10 = new WrapPanel();


            Label startSerial = new Label();
            startSerial.Content = "Start Serial";
            startSerial.Style = (Style)FindResource("tableItemLabel");


            TextBox startSerialTextBox = new TextBox();
            startSerialTextBox.Text = "";

            startSerialTextBox.Style = (Style)FindResource("textBoxStyle");


            wrapPanel10.Children.Add(startSerial);
            wrapPanel10.Children.Add(startSerialTextBox);

            card.Children.Add(wrapPanel10);

            Grid.SetRow(wrapPanel10, 14);


            WrapPanel wrapPanel11 = new WrapPanel();


            Label endSerial = new Label();
            endSerial.Content = "End Serial";
            endSerial.Style = (Style)FindResource("tableItemLabel");


            TextBox endSerialTextBox = new TextBox();

            endSerialTextBox.Style = (Style)FindResource("textBoxStyle");
            endSerialTextBox.Text = "";



            wrapPanel11.Children.Add(endSerial);
            wrapPanel11.Children.Add(endSerialTextBox);

            card.Children.Add(wrapPanel11);

            Grid.SetRow(wrapPanel11, 15);


            WrapPanel wrapPanel12 = new WrapPanel();


            Label quantity = new Label();
            quantity.Content = "Quantity";
            quantity.Style = (Style)FindResource("tableItemLabel");

            TextBox quantityTextBox = new TextBox();
            quantityTextBox.Style = (Style)FindResource("textBoxStyle");
            quantityTextBox.Text = "";

            quantityTextBox.TextChanged += OnQuantityTextBoxTextChanged;



            wrapPanel12.Children.Add(quantity);
            wrapPanel12.Children.Add(quantityTextBox);

            card.Children.Add(wrapPanel12);

            Grid.SetRow(wrapPanel12, 16);



            WrapPanel wrapPanel13 = new WrapPanel();


            Label Currency = new Label();
            Currency.Content = "Currency";
            Currency.Style = (Style)FindResource("tableItemLabel");

            ComboBox CurrencyComoBox = new ComboBox();

            currencies.Clear();
            commonQueries.GetCurrencyTypes(ref currencies);

            currencies.ForEach(a => CurrencyComoBox.Items.Add(a.currencyName));

            CurrencyComoBox.Style = (Style)FindResource("comboBoxStyle");


            wrapPanel13.Children.Add(Currency);
            wrapPanel13.Children.Add(CurrencyComoBox);

            card.Children.Add(wrapPanel13);

            Grid.SetRow(wrapPanel13, 17);



            WrapPanel wrapPanel14 = new WrapPanel();


            Label Price = new Label();
            Price.Content = "Price";
            Price.Style = (Style)FindResource("tableItemLabel");

            TextBox priceTextBox = new TextBox();

            priceTextBox.Style = (Style)FindResource("textBoxStyle");


            wrapPanel14.Children.Add(Price);
            wrapPanel14.Children.Add(priceTextBox);

            card.Children.Add(wrapPanel14);

            Grid.SetRow(wrapPanel14, 18);

            //WrapPanel wrapPanel14 = new WrapPanel();


            //Label productCodeLabel = new Label();
            //productCodeLabel.Content = "ProductCode";
            //productCodeLabel.Style = (Style)FindResource("tableItemLabel");

            //TextBox productCodeTextBox = new TextBox();

            //productCodeTextBox.Style = (Style)FindResource("textBoxStyle");


            //wrapPanel14.Children.Add(productCodeLabel);
            //wrapPanel14.Children.Add(productCodeTextBox);

            //card.Children.Add(wrapPanel14);

            //Grid.SetRow(wrapPanel14, 18);
            //wrapPanel14.Visibility = Visibility.Collapsed;



            WrapPanel wrapPanel15 = new WrapPanel();


            Label stockType = new Label();
            stockType.Content = "Stock Type";
            stockType.Style = (Style)FindResource("tableItemLabel");

            ComboBox stockTypesComboBox = new ComboBox();

            stockTypesComboBox.Style = (Style)FindResource("comboBoxStyle");

            commonQueries.GetStockCategories(ref stockTypes);


            stockTypes.ForEach(a => stockTypesComboBox.Items.Add(a.stock_type_name));

            wrapPanel15.Children.Add(stockType);
            wrapPanel15.Children.Add(stockTypesComboBox);

            card.Children.Add(wrapPanel15);

            Grid.SetRow(wrapPanel15, 19);
            wrapPanel15.Visibility = Visibility.Collapsed;






            WrapPanel wrapPanel16 = new WrapPanel();


            Label validityDate = new Label();
            validityDate.Content = "validity Date";
            validityDate.Style = (Style)FindResource("tableItemLabel");

            DatePicker validityDatePicker = new DatePicker();

            validityDatePicker.Style = (Style)FindResource("datePickerStyle");

            DateTime dt = new DateTime();
            commonQueries.GetTodaysDate(ref dt);
            validityDatePicker.SelectedDate = dt;


            wrapPanel16.Children.Add(validityDate);
            wrapPanel16.Children.Add(validityDatePicker);

            card.Children.Add(wrapPanel16);

            Grid.SetRow(wrapPanel16, 20);
            wrapPanel16.Visibility = Visibility.Collapsed;




            Button generateSerialsButton = new Button();
            generateSerialsButton.Style = (Style)FindResource("buttonStyle");

            generateSerialsButton.Click += GenerateSerialsButtonClick;

            generateSerialsButton.Content = $"Generate {'\n'} Serials";

            Grid.SetRow(generateSerialsButton, 21);
            card.Children.Add(generateSerialsButton);

            card.Children.Add(scroll);

            //OBJECT NAME
            Home.Children.Add(card);

            


        }

        private void OnQuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {


           TextBox quantityTextBox=sender as TextBox;

           WrapPanel quantityPanel= quantityTextBox.Parent as WrapPanel;

           Grid card= quantityPanel.Parent as Grid;

           WrapPanel itemsPanel=  card.Children[3] as WrapPanel;


            WrapPanel rfpPanel = card.Children[1] as WrapPanel;

            CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;



            ComboBox itemsComboBox = itemsPanel.Children[1] as ComboBox;

            if (quantityTextBox.Text == "")
                return;

            if (quantityTextBox.Text == "0") 
            {
                System.Windows.Forms.MessageBox.Show("quantity cannot be 0 !", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                quantityTextBox.Text = "";
                quantityTextBox.Focusable = true;
                return;
            }
                

               bool isLetter  =  quantityTextBox.Text.ToList().Exists(a=>char.IsLetter(a));

            if (isLetter == true) {
                quantityTextBox.Text = "";

                return;
            }

            if (int.Parse(quantityTextBox.Text) > Convert.ToInt32(itemsComboBox.Tag)&& rfpCheckBox.IsChecked==true) {


                System.Windows.Forms.MessageBox.Show("quantity cannot be more than rfp items quantity!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                quantityTextBox.Text = "";
                return;
            
            }
        }

        private void OnRfpItemDescriptionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

           ComboBox rfpItemsComboBox= sender as ComboBox;

            if(rfpItemsComboBox.SelectedIndex==-1)
                return;

           WrapPanel rfpItemsPanel= rfpItemsComboBox.Parent as WrapPanel;


           Grid card=  rfpItemsPanel.Parent as Grid;

           WrapPanel rfpRequsterPanel=card.Children[2] as WrapPanel;

           ComboBox serialComboBox=rfpRequsterPanel.Children[2] as ComboBox;

            WrapPanel startSerialPanel = card.Children[14] as WrapPanel;
            WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

            WrapPanel stockTypePanel = card.Children[19] as WrapPanel;

            WrapPanel validityDatePanel = card.Children[20] as WrapPanel;


            Button generateSerialsButton = card.Children[21] as Button;





            commonQueries.GetRfpItemsMapping(rfps[serialComboBox.SelectedIndex].rfpSerial, rfps[serialComboBox.SelectedIndex].rfpVersion, rfps[serialComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems);

            

            rfpItemsComboBox.Tag = rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.item_quantity;

            if (rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.company_category.category != "")
            {


                if (rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.company_model.has_serial_number == false)
                {

                    startSerialPanel.Visibility = Visibility.Collapsed;
                    endSerialPanel.Visibility = Visibility.Collapsed;

                    stockTypePanel.Visibility = Visibility.Visible;
                    validityDatePanel.Visibility = Visibility.Visible;

                    generateSerialsButton.Visibility = Visibility.Collapsed;

                }

                else {

                    startSerialPanel.Visibility = Visibility.Visible;
                    endSerialPanel.Visibility = Visibility.Visible;
                    stockTypePanel.Visibility = Visibility.Collapsed;

                    validityDatePanel.Visibility = Visibility.Collapsed;
                    generateSerialsButton.Visibility = Visibility.Visible;




                }

            }

            else {


                if (rfpItems[rfpItemsComboBox.SelectedIndex].rfpItem.generic_product_model.has_serial_number == false)
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


        private void OnCompanyModelComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox companyModelComboBox=sender as ComboBox;
            WrapPanel companyModelPanel=companyModelComboBox.Parent as WrapPanel;
            Grid card= companyModelPanel.Parent as Grid;


            WrapPanel stockTypesPanel = card.Children[19] as WrapPanel;


            WrapPanel categoryPanel = card.Children[9] as WrapPanel;
            ComboBox categoryComboBox = categoryPanel.Children[1] as ComboBox;

            WrapPanel productPanel = card.Children[10] as WrapPanel;
            ComboBox productComboBox = productPanel.Children[1] as ComboBox;

            WrapPanel brandPanel = card.Children[11] as WrapPanel;
            ComboBox brandComboBox = brandPanel.Children[1] as ComboBox;


            WrapPanel specsPanel = card.Children[13] as WrapPanel;
            ComboBox specsComboBox= specsPanel.Children[1] as ComboBox;


            WrapPanel startSerialPanel = card.Children[14] as WrapPanel;
            WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

            Button generateSerialsButton = card.Children[21] as Button;




            if (companyModelComboBox.SelectedIndex == -1)
                return;


            if (companyModels[companyModelComboBox.SelectedIndex].has_serial_number == false)
            {

                stockTypesPanel.Visibility = Visibility.Visible;

                startSerialPanel.Visibility = Visibility.Collapsed;

                endSerialPanel.Visibility = Visibility.Collapsed;

                generateSerialsButton.Visibility = Visibility.Collapsed;

            }

            else {

                startSerialPanel.Visibility = Visibility.Visible;

                endSerialPanel.Visibility = Visibility.Visible;

                generateSerialsButton.Visibility = Visibility.Visible;

            }

            specsComboBox.IsEnabled = true;

            specsComboBox.Items.Clear();


            commonQueries.GetModelSpecsNames(companyCategories[categoryComboBox.SelectedIndex].categoryId, companyProducts[productComboBox.SelectedIndex].typeId, companyBrands[brandComboBox.SelectedIndex].brandId, companyModels[companyModelComboBox.SelectedIndex].modelId, ref specs);



            specs.ForEach(a => specsComboBox.Items.Add(a.spec_name));

        }

        private void RfpSerialComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

                ComboBox serialComboBox=sender as ComboBox;

                WrapPanel serialPanel=serialComboBox.Parent as WrapPanel;

                Grid card= serialPanel.Parent as Grid;

                ComboBox requsterComboBox= serialPanel.Children[1] as ComboBox;

            rfps.Clear();

            if (serialComboBox.SelectedIndex == -1)
                return;

        //SEPARATE GET DATA FUNCTIONS FROM GUI FUNCTIONS
        //COMMON QUERIES
        //IF NOT RETURN
           commonQueries.GetTeamRFPs(ref rfps, requesters[requsterComboBox.SelectedIndex].team_id);


            rfpItems.Clear();

            if (!commonQueries.GetRfpItemsMapping(rfps[serialComboBox.SelectedIndex].rfpSerial, rfps[serialComboBox.SelectedIndex].rfpVersion, rfps[serialComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems))
                return;


            for (int i = 0; i < rfpItems.Count; i++) {

                if (rfpItems[i].rfpItem.item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED) {

                    rfpItems.RemoveAt(i);
                    i--;
                }
            }


           WrapPanel descriptionPanel= card.Children[3] as WrapPanel;

           ComboBox itemDescription= descriptionPanel.Children[1] as ComboBox;

            itemDescription.Items.Clear();
            itemDescription.IsEnabled = true;



            if (rfpItems.Count == 0) {

                System.Windows.Forms.MessageBox.Show("RFP Items are not mapped!.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                return;
            }

            for (int i = 0; i < rfpItems.Count; i++) {


                if (rfpItems[i].rfpItem.generic_product_category.category_id == 0)
                {
                    itemDescription.Items.Add(rfpItems[i].rfpItem.company_product.typeName + "," + rfpItems[i].rfpItem.company_brand.brandName + "," + rfpItems[i].rfpItem.company_model.modelName);


                }

                else {


                    itemDescription.Items.Add(rfpItems[i].rfpItem.generic_product_category.category_name + "," + rfpItems[i].rfpItem.generic_product_type.product_name + "," + rfpItems[i].rfpItem.generic_product_brand.brand_name+","+rfpItems[i].rfpItem.generic_product_model.model_name);



                }

            }


        }

        private void RfpRequstorComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox rfpRequesterComboBoxBox = sender as ComboBox;

            WrapPanel rfpRequsetorPanel = rfpRequesterComboBoxBox.Parent as WrapPanel;


            Grid card = rfpRequsetorPanel.Parent as Grid;

          ComboBox serialComboBox= rfpRequsetorPanel.Children[2] as ComboBox;

            if (rfpRequesterComboBoxBox.SelectedIndex == -1)
                return;
            rfps.Clear();
            serialComboBox.Items.Clear();
            commonQueries.GetTeamRFPs(ref rfps, requesters[rfpRequesterComboBoxBox.SelectedIndex].team_id);

            rfps.ForEach(a => serialComboBox.Items.Add(a.rfpID));
            serialComboBox.IsEnabled = true;

        }


        private void RfpCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

            //CREATE A NEW FUNCTION TO DISABLE THE GUI AND CALL IT HERE

            CheckBox rfpCheckBox = sender as CheckBox;

            WrapPanel rfpPanel = rfpCheckBox.Parent as WrapPanel;


            Grid card = rfpPanel.Parent as Grid;
            WrapPanel requestorPanel = card.Children[2] as WrapPanel;

            ScrollViewer scroll = card.Children[22] as ScrollViewer;
            Grid serialsGrid= scroll.Content as Grid;

            serialsGrid.Children.Clear();


            ComboBox requstorComboBox = requestorPanel.Children[1] as ComboBox;

            requstorComboBox.IsEnabled = false;
            requstorComboBox.SelectedIndex = -1;


          ComboBox serialComboBox=requestorPanel.Children[2] as ComboBox;


            serialComboBox.IsEnabled = false;
            serialComboBox.SelectedIndex = -1;

            WrapPanel itemDescriptionPanel = card.Children[3] as WrapPanel;

            WrapPanel choicePanel = card.Children[4] as WrapPanel;


            ComboBox itemDescriptionComboBox = itemDescriptionPanel.Children[1] as ComboBox;

            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;


            choiceComboBox.IsEnabled = true;
            itemDescriptionComboBox.IsEnabled = false;
            itemDescriptionComboBox.SelectedIndex = -1;


        }

        private void RfpCheckBoxChecked(object sender, RoutedEventArgs e)
        {

          CheckBox rfpCheckBox=sender as CheckBox;

          WrapPanel rfpPanel=rfpCheckBox.Parent as WrapPanel;


          Grid card=rfpPanel.Parent as Grid;
          WrapPanel requestorPanel=card.Children[2] as WrapPanel;

          ComboBox requstorComboBox=requestorPanel.Children[1] as ComboBox;


            WrapPanel choicePanel = card.Children[4] as WrapPanel;
           ComboBox choiceComboBox= choicePanel.Children[1] as ComboBox;


            WrapPanel GenericCategoryPanel = card.Children[5] as WrapPanel;
            ComboBox genericCategoryComboBox = GenericCategoryPanel.Children[1] as ComboBox;



            WrapPanel GenericProductPanel = card.Children[6] as WrapPanel;
            ComboBox genericProductComboBox = GenericProductPanel.Children[1] as ComboBox;



            WrapPanel genricBrandPanel = card.Children[7] as WrapPanel;
            ComboBox genericBrandComboBox = genricBrandPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel = card.Children[8] as WrapPanel;
            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;





            WrapPanel companyCategoryPanel = card.Children[9] as WrapPanel;
            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;



            WrapPanel companyProductPanel = card.Children[10] as WrapPanel;
            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;



            WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;
            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;


            WrapPanel companyModelPanel = card.Children[12] as WrapPanel;
            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            requstorComboBox.IsEnabled = true;
            choiceComboBox.IsEnabled = false;
            choiceComboBox.SelectedIndex = -1;


            genericCategoryComboBox.IsEnabled = false;
            genericCategoryComboBox.SelectedIndex = -1;

            genericProductComboBox.IsEnabled = false;
            genericProductComboBox.Items.Clear();

            genericBrandComboBox.IsEnabled = false;
            genericBrandComboBox.Items.Clear();

            genericModelComboBox.IsEnabled = false;
            genericModelComboBox.Items.Clear();


            companyCategoryComboBox.IsEnabled = false;
            companyCategoryComboBox.SelectedIndex = -1;

            companyProductComboBox.IsEnabled = false;
            companyProductComboBox.Items.Clear();


            companyBrandComboBox.IsEnabled = false;
            companyBrandComboBox.Items.Clear();


            companyModelComboBox.IsEnabled = false;
            companyModelComboBox.Items.Clear();






        }

        private void CompanyBrandComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox companyBrandComboBox=sender as ComboBox;

            WrapPanel companyBrandPanel=companyBrandComboBox.Parent as WrapPanel;

            Grid card=companyBrandPanel.Parent as Grid;

            WrapPanel companyModelPanel=card.Children[12] as WrapPanel;


            WrapPanel companySpecsPanel = card.Children[13] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;



            WrapPanel stockTypesPanel = card.Children[18] as WrapPanel;


            WrapPanel companyProductPanel = card.Children[10] as WrapPanel;

            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;

            ComboBox companyModelComboBox=companyModelPanel.Children[1] as ComboBox;

            if (companyBrandComboBox.SelectedIndex == -1)
                return;

            companyModelComboBox.IsEnabled = true;
            stockTypesPanel.Visibility = Visibility.Collapsed;

            companySpecsComboBox.Items.Clear();
            companySpecsComboBox.IsEnabled = false;


            companyModels.Clear();
            commonQueries.GetCompanyModels(companyProducts[companyProductComboBox.SelectedIndex], companyBrands[companyBrandComboBox.SelectedIndex], ref companyModels);


            companyModelComboBox.Items.Clear();
            companyModels.ForEach(a => companyModelComboBox.Items.Add(a.modelName));

        }

        private void CompanyProductComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyProductComboBox=sender as ComboBox;
           WrapPanel CompanyProductPanel=companyProductComboBox.Parent as WrapPanel;
           Grid Card=CompanyProductPanel.Parent as Grid;

            WrapPanel companyBrandPanel=Card.Children[11] as WrapPanel;


            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;


            WrapPanel companyModelPanel = Card.Children[12] as WrapPanel;




            WrapPanel companySpecsPanel = Card.Children[13] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;


            WrapPanel stockTypesPanel = Card.Children[18] as WrapPanel;



            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            if (companyProductComboBox.SelectedIndex == -1)
                return;


            companySpecsComboBox.Items.Clear();
            companySpecsComboBox.IsEnabled = false;
            companyModelComboBox.Items.Clear();


            companyBrandComboBox.IsEnabled = true;
            companyModelComboBox.IsEnabled = false;

            stockTypesPanel.Visibility = Visibility.Collapsed;

            companyBrands.Clear();
            commonQueries.GetProductBrands(companyProducts[companyProductComboBox.SelectedIndex].typeId, ref companyBrands);

            companyBrandComboBox.Items.Clear();
            companyBrands.ForEach(a => companyBrandComboBox.Items.Add(a.brandName));
        }

        private void CompanyCategoryComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //from 6
           ComboBox companyCategoryComboBox= sender as ComboBox;

           WrapPanel categoryPanel= companyCategoryComboBox.Parent as WrapPanel;

           Grid card= categoryPanel.Parent as Grid;

           WrapPanel companyProductPanel= card.Children[10] as WrapPanel;

           ComboBox companyProductComboBox=companyProductPanel.Children[1] as ComboBox;


            WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;

            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;
             

            WrapPanel companyModelPanel = card.Children[12] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;



            WrapPanel companySpecsPanel = card.Children[13] as WrapPanel;

            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;



            WrapPanel stockTypesPanel = card.Children[18] as WrapPanel;


            companySpecsComboBox.Items.Clear();
            companySpecsComboBox.IsEnabled = false;

            if (companyCategoryComboBox.SelectedIndex == -1)
                return;

            companyProductComboBox.IsEnabled = true;
            companyBrandComboBox.IsEnabled = false;
            companyBrandComboBox.Items.Clear();
            companyModelComboBox.IsEnabled = false;
            companyModelComboBox.Items.Clear();
            stockTypesPanel.Visibility = Visibility.Collapsed;

            companyProducts.Clear();
            commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[companyCategoryComboBox.SelectedIndex].categoryId);

            companyProductComboBox.Items.Clear();
            companyProducts.ForEach(a => companyProductComboBox.Items.Add(a.typeName));


        }

        private void ChoiceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 2 start of the generic
            //6 start of the company
           ComboBox choiceComboBox=sender as ComboBox;

           WrapPanel choiceWrapPanel=choiceComboBox.Parent as WrapPanel;
           Grid card=choiceWrapPanel.Parent as Grid;


            WrapPanel CompanyCategoryWrapPanel = card.Children[9] as WrapPanel;
            ComboBox companyCategoryComboBox = CompanyCategoryWrapPanel.Children[1] as ComboBox;


            WrapPanel CompanyProductWrapPanel =card.Children[10] as WrapPanel;
           ComboBox companyProductComboBox= CompanyProductWrapPanel.Children[1] as ComboBox;

            WrapPanel CompanyBrandWrapPanel = card.Children[11] as WrapPanel;
            ComboBox companyBrandComboBox = CompanyBrandWrapPanel.Children[1] as ComboBox;
            WrapPanel CompanyModelWrapPanel = card.Children[12] as WrapPanel;
            ComboBox companyModelComboBox = CompanyModelWrapPanel.Children[1] as ComboBox;


            WrapPanel CompanySpecsWrapPanel = card.Children[13] as WrapPanel;




            WrapPanel genericCategoryWrapPanel = card.Children[5] as WrapPanel;
            ComboBox genericCategoryComboBox = genericCategoryWrapPanel.Children[1] as ComboBox;

            WrapPanel genericProductWrapPanel = card.Children[6] as WrapPanel;
            ComboBox genericProductComboBox = genericProductWrapPanel.Children[1] as ComboBox;

            WrapPanel genericBrandWrapPanel = card.Children[7] as WrapPanel;
            ComboBox genericBrandComboBox = genericBrandWrapPanel.Children[1] as ComboBox;

            WrapPanel genericModelWrapPanel = card.Children[8] as WrapPanel;
            ComboBox genericModelComboBox = genericModelWrapPanel.Children[1] as ComboBox;



            if (choiceComboBox.SelectedIndex == -1)
                return;

            //generic
            if (choiceComboBox.SelectedIndex == 0)
            {

                companyCategoryComboBox.IsEnabled = false;
                companyCategoryComboBox.SelectedIndex = -1;

                companyProductComboBox.IsEnabled = false;
                companyProductComboBox.Items.Clear();

                companyBrandComboBox.IsEnabled = false;
                companyBrandComboBox.Items.Clear();

                companyModelComboBox.IsEnabled = false;
                companyModelComboBox.Items.Clear();



                genericCategoryComboBox.IsEnabled = true;
                genericCategoryComboBox.SelectedIndex = 0;

                CompanySpecsWrapPanel.Visibility = Visibility.Collapsed;

                //genericCategoryComboBox.Items.Clear();

                //genericProductComboBox.Items.Clear();

                //genericBrandComboBox.Items.Clear();

                //genericModelComboBox.Items.Clear();



            }

            //company
            else if(choiceComboBox.SelectedIndex==1) {

                genericCategoryComboBox.IsEnabled = false;
                genericCategoryComboBox.SelectedIndex = -1;

                genericProductComboBox.IsEnabled = false;
                genericProductComboBox.Items.Clear();

                genericBrandComboBox.IsEnabled = false;
                genericBrandComboBox.Items.Clear();

                genericModelComboBox.IsEnabled = false;
                genericModelComboBox.Items.Clear();


                companyCategoryComboBox.IsEnabled = true;
                companyCategoryComboBox.SelectedIndex = 0;


                CompanySpecsWrapPanel.Visibility = Visibility.Visible;




            }
        }

        private void GenericBrandComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

           ComboBox genericBrandComboBox=sender as ComboBox;

            WrapPanel genericBrandPanel = genericBrandComboBox.Parent as WrapPanel;

            Grid Card = genericBrandPanel.Parent as Grid;


            WrapPanel genericProductPanel = Card.Children[6] as WrapPanel;

            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel=Card.Children[8] as WrapPanel;
           ComboBox genericModelComboBox=genericModelPanel.Children[1] as ComboBox;


            WrapPanel genericCategoryPanel = Card.Children[5] as WrapPanel;


            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;

            if (genericBrandComboBox.SelectedIndex == -1)
                return;


            genericModelComboBox.IsEnabled = true;
            genericModels.Clear();

            commonQueries.GetGenericBrandModels(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericBrands[genericBrandComboBox.SelectedIndex].brand_id ,genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericModels);

            genericModelComboBox.Items.Clear();

            genericModels.ForEach(a => genericModelComboBox.Items.Add(a.model_name));
        }

        private void GenericCategoryComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

           ComboBox genericCategoryComboBox=sender as ComboBox;
            WrapPanel genericCategoryPanel=genericCategoryComboBox.Parent as WrapPanel;

           Grid Card=genericCategoryPanel.Parent as Grid;

          WrapPanel genericProductPanel=Card.Children[6] as WrapPanel;

           ComboBox genericProductComboBox=genericProductPanel.Children[1] as ComboBox;

            WrapPanel genericBrandPanel = Card.Children[7] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel = Card.Children[8] as WrapPanel;
            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;


            if (genericCategoryComboBox.SelectedIndex == -1)
                return;

            genericProductComboBox.IsEnabled = true;

            genericBrandComboBox.IsEnabled = false;
            genericBrandComboBox.Items.Clear();

            genericModelComboBox.IsEnabled = false;
            genericModelComboBox.Items.Clear();

            genericProducts.Clear();
            commonQueries.GetGenericProducts(ref genericProducts, genericCategories[genericCategoryComboBox.SelectedIndex].category_id);

            genericProductComboBox.Items.Clear();
            genericProducts.ForEach(a => genericProductComboBox.Items.Add(a.product_name));


        }

        private void GenericProductComoBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox genericProductComboBox=sender as ComboBox;

            WrapPanel genericProductPanel = genericProductComboBox.Parent as WrapPanel;

            Grid Card = genericProductPanel.Parent as Grid;

            WrapPanel genericCategoryPanel = Card.Children[5] as WrapPanel;


            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;


            WrapPanel genericBrandPanel = Card.Children[7] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;

            WrapPanel genericModelPanel = Card.Children[8] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



            if (genericProductComboBox.SelectedItem == null)
                return;

            genericBrandComboBox.IsEnabled = true;
            genericModelComboBox.IsEnabled = false;
            genericModelComboBox.Items.Clear();

            genericBrands.Clear();

            commonQueries.GetGenericProductBrands(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericBrands);

            genericBrandComboBox.Items.Clear();
            genericModelComboBox.Items.Clear();


            genericBrands.ForEach(a => genericBrandComboBox.Items.Add(a.brand_name));


        }

        private void GenerateSerialsButtonClick(object sender, RoutedEventArgs e)
        {
            Button generateButton=sender as Button;


            Grid card= generateButton.Parent as Grid;


            WrapPanel rfpPanel = card.Children[1] as WrapPanel;

           CheckBox rfpCheckBox= rfpPanel.Children[1] as CheckBox;

            WrapPanel startSerialWrapPanel=card.Children[14] as WrapPanel;


                TextBox startSerialTextBox=startSerialWrapPanel.Children[1] as TextBox;

            WrapPanel endSerialWrapPanel = card.Children[15] as WrapPanel;


                TextBox endSerialTextBox = endSerialWrapPanel.Children[1] as TextBox;


            WrapPanel quantityWrapPanel = card.Children[16] as WrapPanel;


            TextBox quantityTextBox = quantityWrapPanel.Children[1] as TextBox;



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

                ScrollViewer serialsScroll = card.Children[22] as ScrollViewer;
                Grid serialsGrid = serialsScroll.Content as Grid;

                serialsGrid.RowDefinitions.Clear();
                serialsGrid.ColumnDefinitions.Clear();
                serialsGrid.Children.Clear();



                for (int i = 0; i < (int.Parse(number2) - int.Parse(number)) + 1; i++)
                {

                    WrapPanel wrapPanel = new WrapPanel();

                
                    Label serialNumber = new Label();
                    serialNumber.Style = (Style)FindResource("tableItemLabel");
                    serialNumber.Content = $"Serial {i + 1}";

                    serialNumber.HorizontalContentAlignment = HorizontalAlignment.Center;

                    serialNumber.Width = 80;

                    TextBox textSerial = new TextBox();
                    textSerial.Style = (Style)FindResource("miniTextBoxStyle");



                    




                    //TextBox productCodeTextBox = new TextBox();
                    //productCodeTextBox.Style = (Style)FindResource("miniTextBoxStyle");

                    //productCodeTextBox.Text = "Product Code";

                    //productCodeTextBox.Foreground = Brushes.Gray;

                    //productCodeTextBox.MouseEnter += OnProductCodeTextBoxMouseEnter;
                    //productCodeTextBox.MouseLeave += OnProductCodeTextBoxMouseLeave;



                    ComboBox stockTypeCombobox = new ComboBox();
                    stockTypeCombobox.Style = (Style)FindResource("miniComboBoxStyle");
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

                    serialsGrid.RowDefinitions.Add(new RowDefinition());

                    



                    Grid.SetRow(wrapPanel, serialsGrid.RowDefinitions.Count - 1);
               


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


                    serialsGrid.Children.Add(wrapPanel);

                }

            }


            else if (startSerialTextBox.Text != "" && quantityTextBox.Text == "1" && endSerialTextBox.Text == "") {

                ScrollViewer serialsScroll = card.Children[22] as ScrollViewer;
                Grid serialsGrid = serialsScroll.Content as Grid;

                serialsGrid.RowDefinitions.Clear();
                serialsGrid.ColumnDefinitions.Clear();
                serialsGrid.Children.Clear();


                WrapPanel wrapPanel = new WrapPanel();

                Label serialNumber = new Label();
                serialNumber.Style = (Style)FindResource("tableItemLabel");
                serialNumber.Content = $"Serial 1";

                serialNumber.Width = 80;


                TextBox textSerial = new TextBox();

                textSerial.Style = (Style)FindResource("textBoxStyle");
                serialsGrid.RowDefinitions.Add(new RowDefinition());



                textSerial.Text = startSerialTextBox.Text;


                Grid.SetRow(wrapPanel, serialsGrid.RowDefinitions.Count - 1);


                wrapPanel.Children.Add(serialNumber);
                wrapPanel.Children.Add(textSerial);

                serialsGrid.Children.Add(wrapPanel);

            }






        }

        private void OnProductCodeTextBoxMouseLeave(object sender, MouseEventArgs e)
        {
            TextBox productCodeTextBox = sender as TextBox;
            if (productCodeTextBox.Text == "")
            {
                productCodeTextBox.Text = "Product Code";

                productCodeTextBox.Foreground = Brushes.Gray;
            }
            else {

                productCodeTextBox.Foreground = Brushes.Black;


            }

        }

        private void OnProductCodeTextBoxMouseEnter(object sender, MouseEventArgs e)
        {
           TextBox productCodeTextBox= sender as TextBox;


            if(productCodeTextBox.Text=="Product Code")
               productCodeTextBox.Text = "";
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            entryPermitWindow.Close();
        }

        private void FinishButtonClick(object sender, RoutedEventArgs e)
        {

            if (addEntryPermitPage.WareHouseCombo.SelectedIndex == -1 || addEntryPermitPage.TransactionDatePicker.Text == ""|| addEntryPermitPage.entryPermitIdTextBox.Text=="") {

                System.Windows.Forms.MessageBox.Show("WareHouse or transaction date or entryPermitId is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            addEntryPermitPage.materialEntryPermit.GetItems().Clear();
            materialReservations.Clear();


            int rfpItemCount = 1;

            for (int i = 0; i < Home.Children.Count; i++) {

                Grid card=Home.Children[i] as Grid;


                WrapPanel rfpPanel = card.Children[1] as WrapPanel;


                WrapPanel rfpRequsterPanel = card.Children[2] as WrapPanel;

                ComboBox requesterComboBox= rfpRequsterPanel.Children[1] as ComboBox;

                ComboBox rfpSerialComboBox = rfpRequsterPanel.Children[2] as ComboBox;


                WrapPanel itemDescriptionPanel = card.Children[3] as WrapPanel;

                ComboBox itemDescriptionComboBox=itemDescriptionPanel.Children[1] as ComboBox;

                CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;

                WrapPanel choicePanel = card.Children[4] as WrapPanel;

                ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

                
                WrapPanel genericCategoryPanel = card.Children[5] as WrapPanel;

                ComboBox genericCategoryComboBox=genericCategoryPanel.Children[1] as ComboBox;


                WrapPanel genericProductPanel = card.Children[6] as WrapPanel;

                ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


                WrapPanel genericBrandPanel = card.Children[7] as WrapPanel;

                ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;



                WrapPanel genericModelPanel = card.Children[8] as WrapPanel;

                ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



                WrapPanel companyCategoryPanel = card.Children[9] as WrapPanel;

                ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;



                WrapPanel companyProductPanel = card.Children[10] as WrapPanel;

                ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;




                WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;

                ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;



                WrapPanel companyModelPanel = card.Children[12] as WrapPanel;

                ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;



                WrapPanel specsPanel = card.Children[13] as WrapPanel;

                ComboBox specsComboBox = specsPanel.Children[1] as ComboBox;


                WrapPanel startSerialPanel = card.Children[14] as WrapPanel;

                TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;



                WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

                TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;



                WrapPanel quantitylPanel = card.Children[16] as WrapPanel;

                TextBox quantityTextBox = quantitylPanel.Children[1] as TextBox;



                WrapPanel currencyPanel = card.Children[17] as WrapPanel;

                ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;




                WrapPanel pricePanel = card.Children[18] as WrapPanel;

                TextBox priceTextBox = pricePanel.Children[1] as TextBox;



                WrapPanel stockTypePanel = card.Children[19] as WrapPanel;

                ComboBox stockTypeComboBoxMain = stockTypePanel.Children[1] as ComboBox;


                WrapPanel datePanel = card.Children[20] as WrapPanel;

                DatePicker validityDate = datePanel.Children[1] as DatePicker;


                ScrollViewer scroll = card.Children[22] as ScrollViewer;

                Grid serialGrid= scroll.Content as Grid;

                //if the checkbox is unchecked

                if (rfpCheckBox.IsChecked == false)
                {

                    //if the choice is generic
                    if (choiceComboBox.SelectedIndex == 0)
                    {


                        if (genericModelComboBox.SelectedIndex == -1)
                        {

                            System.Windows.Forms.MessageBox.Show("You have to choose a model", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                            return;
                        }

                        else
                        {

                            if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
                            {



                                BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();

                                materialEntryItem.genericCategory.category_id = genericCategories[genericCategoryComboBox.SelectedIndex].category_id;

                                genericProducts.Clear();

                                commonQueries.GetGenericProducts(ref genericProducts, genericCategories[genericCategoryComboBox.SelectedIndex].category_id);

                                materialEntryItem.genericProduct.product_id = genericProducts[genericProductComboBox.SelectedIndex].product_id;

                                genericBrands.Clear();

                                commonQueries.GetGenericProductBrands(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericBrands);

                                materialEntryItem.genericBrand.brand_id = genericBrands[genericBrandComboBox.SelectedIndex].brand_id;

                                genericModels.Clear();

                                commonQueries.GetGenericBrandModels(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericBrands[genericBrandComboBox.SelectedIndex].brand_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericModels);


                                materialEntryItem.genericModel.model_id = genericModels[genericModelComboBox.SelectedIndex].model_id;

                                materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;

                                materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                                commonQueries.GetModelSpecsNames(companyCategories[companyCategoryComboBox.SelectedIndex].categoryId, companyProducts[companyProductComboBox.SelectedIndex].typeId, companyBrands[companyBrandComboBox.SelectedIndex].brandId, companyModels[companyModelComboBox.SelectedIndex].modelId, ref specs);


                                materialEntryItem.companySpec.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                materialEntryItem.companySpec.spec_name = specs[specsComboBox.SelectedIndex].spec_name;



                                if (stockTypeComboBoxMain.SelectedIndex == -1)
                                {

                                    System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return;

                                }

                                materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_id;
                                materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_name;
                                materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;




                                addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                            }

                            else
                            {
                                int count = 1;

                                for (int j = 0; j < serialGrid.Children.Count; j ++)
                                {
                                   

                                    BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();

                                    materialEntryItem.genericCategory.category_id = genericCategories[genericCategoryComboBox.SelectedIndex].category_id;

                                    genericProducts.Clear();

                                    commonQueries.GetGenericProducts(ref genericProducts, genericCategories[genericCategoryComboBox.SelectedIndex].category_id);

                                    materialEntryItem.genericProduct.product_id = genericProducts[genericProductComboBox.SelectedIndex].product_id;

                                    genericBrands.Clear();

                                    commonQueries.GetGenericProductBrands(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericBrands);

                                    materialEntryItem.genericBrand.brand_id = genericBrands[genericBrandComboBox.SelectedIndex].brand_id;

                                    genericModels.Clear();

                                    commonQueries.GetGenericBrandModels(genericProducts[genericProductComboBox.SelectedIndex].product_id, genericBrands[genericBrandComboBox.SelectedIndex].brand_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericModels);


                                    materialEntryItem.genericModel.model_id = genericModels[genericModelComboBox.SelectedIndex].model_id;
                                    materialEntryItem.quantity = 0;

                                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                    materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;


                                    commonQueries.GetModelSpecsNames(companyCategories[companyCategoryComboBox.SelectedIndex].categoryId, companyProducts[companyProductComboBox.SelectedIndex].typeId, companyBrands[companyBrandComboBox.SelectedIndex].brandId, companyModels[companyModelComboBox.SelectedIndex].modelId, ref specs);


                                    materialEntryItem.companySpec.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                    materialEntryItem.companySpec.spec_name = specs[specsComboBox.SelectedIndex].spec_name;




                                    materialEntryItem.entry_permit_item_serial = count;

                                    count++;

                                    WrapPanel serialPanel = serialGrid.Children[j] as WrapPanel;
                                    TextBox serialText = serialPanel.Children[1] as TextBox;
                                    ComboBox stockTypeComboBox = serialPanel.Children[2] as ComboBox;

                                    if (stockTypeComboBox.SelectedIndex == -1) {

                                        System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;

                                    }

                                    materialEntryItem.product_serial_number = serialText.Text;

                                    materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                                    materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                                    materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;



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


                        else {

                            if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
                            {

                                BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();



                                materialEntryItem.companyCategory.categoryId = companyCategories[companyCategoryComboBox.SelectedIndex].categoryId;
                                companyProducts.Clear();

                                commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[companyCategoryComboBox.SelectedIndex].categoryId);

                                materialEntryItem.companyProduct.typeId = companyProducts[companyProductComboBox.SelectedIndex].typeId;

                                companyBrands.Clear();

                                commonQueries.GetProductBrands(companyProducts[companyProductComboBox.SelectedIndex].typeId, ref companyBrands);

                                materialEntryItem.companyBrand.brandId = companyBrands[companyBrandComboBox.SelectedIndex].brandId;

                                companyModels.Clear();

                                commonQueries.GetCompanyModels(companyProducts[companyProductComboBox.SelectedIndex], companyBrands[companyBrandComboBox.SelectedIndex], ref companyModels);


                                materialEntryItem.companyModel.modelId = companyModels[companyModelComboBox.SelectedIndex].modelId;

                                if(priceTextBox.Text!="")
                                materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;

                                materialEntryItem.quantity = int.Parse(quantityTextBox.Text);


                                commonQueries.GetModelSpecsNames(companyCategories[companyCategoryComboBox.SelectedIndex].categoryId, companyProducts[companyProductComboBox.SelectedIndex].typeId, companyBrands[companyBrandComboBox.SelectedIndex].brandId, companyModels[companyModelComboBox.SelectedIndex].modelId, ref specs);


                                materialEntryItem.companySpec.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                materialEntryItem.companySpec.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                                if (stockTypeComboBoxMain.SelectedIndex == -1)
                                {

                                    System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return;

                                }

                                materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_id;
                                materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_name;
                                materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBoxMain.SelectedIndex].added_by;


                                addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);


                            }

                            else
                            {
                                int count = 1;

                                for (int j = 0; j < serialGrid.Children.Count; j++)
                                {

                                    BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();

                                    materialEntryItem.companyCategory.categoryId = companyCategories[companyCategoryComboBox.SelectedIndex].categoryId;

                                    companyProducts.Clear();

                                    commonQueries.GetCompanyProducts(ref companyProducts, companyCategories[companyCategoryComboBox.SelectedIndex].categoryId);

                                    materialEntryItem.companyProduct.typeId = companyProducts[companyProductComboBox.SelectedIndex].typeId;

                                    companyBrands.Clear();

                                    commonQueries.GetProductBrands(companyProducts[companyProductComboBox.SelectedIndex].typeId, ref companyBrands);

                                    materialEntryItem.companyBrand.brandId = companyBrands[companyBrandComboBox.SelectedIndex].brandId;

                                    companyModels.Clear();

                                    commonQueries.GetCompanyModels(companyProducts[companyProductComboBox.SelectedIndex], companyBrands[companyBrandComboBox.SelectedIndex], ref companyModels);


                                    materialEntryItem.companyModel.modelId = companyModels[companyModelComboBox.SelectedIndex].modelId;

                                    materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                                    materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;


                                    commonQueries.GetModelSpecsNames(companyCategories[companyCategoryComboBox.SelectedIndex].categoryId, companyProducts[companyProductComboBox.SelectedIndex].typeId, companyBrands[companyBrandComboBox.SelectedIndex].brandId, companyModels[companyModelComboBox.SelectedIndex].modelId, ref specs);


                                    materialEntryItem.companySpec.spec_id = specs[specsComboBox.SelectedIndex].spec_id;
                                    materialEntryItem.companySpec.spec_name = specs[specsComboBox.SelectedIndex].spec_name;


                                    materialEntryItem.quantity = 0;



                                    materialEntryItem.entry_permit_item_serial = count;

                                    count++;

                                    WrapPanel serialPanel = serialGrid.Children[j] as WrapPanel;
                                    TextBox serialText = serialPanel.Children[1] as TextBox;
                                    ComboBox stockTypeComboBox = serialPanel.Children[2] as ComboBox;

                                    if (stockTypeComboBox.SelectedIndex == -1)
                                    {

                                        System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        return;

                                    }

                                    materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                                    materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                                    materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                                    materialEntryItem.product_serial_number = serialText.Text;

                                    addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                                }




                            }


                        }



                    }


                }

                //if the checkbox is checked

                else {


                    //////////////////////////////////////////////////
                    //to implement rfp items after the mapping is done
                    //////////////////////////////////////////////////

                    if (startSerialTextBox.Text == "" && endSerialTextBox.Text == "" && quantityTextBox.Text != "")
                    {

                        BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();

                        materialEntryItem.rfp_info.rfpRequestorTeam = requesters[requesterComboBox.SelectedIndex].team_id;

                        rfps.Clear();

                        commonQueries.GetTeamRFPs(ref rfps, requesters[requesterComboBox.SelectedIndex].team_id);

                        materialEntryItem.rfp_info.rfpSerial = rfps[rfpSerialComboBox.SelectedIndex].rfpSerial;
                        materialEntryItem.rfp_info.rfpVersion = rfps[rfpSerialComboBox.SelectedIndex].rfpVersion;

                        rfpItems.Clear();

                        commonQueries.GetRfpItemsMapping(rfps[rfpSerialComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                        materialEntryItem.rfp_item_number = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.item_number;
                        materialEntryItem.entry_permit_item_serial = rfpItemCount;


                        if (priceTextBox.Text != "")
                        materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                        materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;

                        materialEntryItem.quantity = int.Parse(quantityTextBox.Text);

                        MATERIAL_RESERVATION_MED_STRUCT materialReservation = new MATERIAL_RESERVATION_MED_STRUCT();


                        materialReservation.hold_until=validityDate.DisplayDate;

                        materialReservation.reserved_by_id=loggedInUser.GetEmployeeId();

                        materialReservation.rfp_serial=materialEntryItem.rfp_info.rfpSerial;

                        materialReservation.rfp_version=materialEntryItem.rfp_info.rfpVersion;

                        materialReservation.rfp_item_no=materialEntryItem.rfp_item_number;

                        materialReservation.rfp_requestor_team=materialEntryItem.rfp_info.rfpRequestorTeam;

                        materialReservation.quantity=materialEntryItem.quantity;

                        materialReservation.entry_permit_item_serial=rfpItemCount;

                        materialReservations.Add(materialReservation);


                        if (rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_category.category_id == 0)
                        {


                            materialEntryItem.companyCategory.categoryId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_category.categoryId;
                            materialEntryItem.companyProduct.typeId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_product.typeId;
                            materialEntryItem.companyBrand.brandId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_brand.brandId;
                            materialEntryItem.companyModel.modelId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_model.modelId;
                            materialEntryItem.companySpec.spec_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_model_spec.spec_id;


                        }

                        else {

                            materialEntryItem.genericCategory.category_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_category.category_id;
                            materialEntryItem.genericProduct.product_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_type.product_id;
                            materialEntryItem.genericBrand.brand_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_brand.brand_id;
                            materialEntryItem.genericModel.model_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_model.model_id;


                        }


                        if (stockTypeComboBoxMain.SelectedIndex == -1)
                        {

                            System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return;

                        }

                        materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_id;
                        materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBoxMain.SelectedIndex].stock_type_name;


                        addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                        rfpItemCount++;


                    }

                    else
                    {

                        for (int j = 0; j < serialGrid.Children.Count; j ++)
                        {


                            BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM materialEntryItem = new BASIC_STRUCTS.MATERIAL_ENTRY_PERMIT_ITEM();

                            materialEntryItem.rfp_info.rfpRequestorTeam = requesters[requesterComboBox.SelectedIndex].team_id;

                            rfps.Clear();

                            commonQueries.GetTeamRFPs(ref rfps, requesters[requesterComboBox.SelectedIndex].team_id);

                            materialEntryItem.rfp_info.rfpSerial = rfps[rfpSerialComboBox.SelectedIndex].rfpSerial;
                            materialEntryItem.rfp_info.rfpVersion = rfps[rfpSerialComboBox.SelectedIndex].rfpVersion;

                            rfpItems.Clear();

                            commonQueries.GetRfpItemsMapping(rfps[rfpSerialComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialComboBox.SelectedIndex].rfpVersion, materialEntryItem.rfp_info.rfpRequestorTeam, ref rfpItems);

                            materialEntryItem.rfp_item_number = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.item_number;


                            if(priceTextBox.Text!="")
                            materialEntryItem.item_price = decimal.Parse(priceTextBox.Text);

                            materialEntryItem.item_currency.currencyId = currencies[currencyComboBox.SelectedIndex].currencyId;

                            materialEntryItem.quantity =1;



                            materialEntryItem.entry_permit_item_serial = rfpItemCount;


                            MATERIAL_RESERVATION_MED_STRUCT materialReservation = new MATERIAL_RESERVATION_MED_STRUCT();


                            materialReservation.hold_until = validityDate.DisplayDate;

                            materialReservation.reserved_by_id = loggedInUser.GetEmployeeId();

                            materialReservation.rfp_serial = materialEntryItem.rfp_info.rfpSerial;

                            materialReservation.rfp_version = materialEntryItem.rfp_info.rfpVersion;

                            materialReservation.rfp_item_no = materialEntryItem.rfp_item_number;

                            materialReservation.rfp_requestor_team = materialEntryItem.rfp_info.rfpRequestorTeam;

                            materialReservation.quantity = materialEntryItem.quantity;

                            materialReservation.entry_permit_item_serial = rfpItemCount;

                            materialReservations.Add(materialReservation);

                            rfpItemCount++;


                            WrapPanel serialPanel = serialGrid.Children[j] as WrapPanel;
                            TextBox serialText = serialPanel.Children[1] as TextBox;

                            ComboBox stockTypeComboBox = serialPanel.Children[2] as ComboBox;

                            if (stockTypeComboBox.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose the stock Type for each serial", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return;

                            }

                            materialEntryItem.product_serial_number = serialText.Text;


                            materialEntryItem.stock_type.stock_type_id = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_id;
                            materialEntryItem.stock_type.stock_type_name = stockTypes[stockTypeComboBox.SelectedIndex].stock_type_name;
                            materialEntryItem.stock_type.added_by = stockTypes[stockTypeComboBox.SelectedIndex].added_by;

                            if (rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_category.category_name == "")
                            {
                                materialEntryItem.companyCategory.categoryId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_category.categoryId;
                                materialEntryItem.companyProduct.typeId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_product.typeId;
                                materialEntryItem.companyBrand.brandId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_brand.brandId;
                                materialEntryItem.companyModel.modelId = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_model.modelId;
                                materialEntryItem.companySpec.spec_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.company_model_spec.spec_id;



                            }

                            else
                            {

                                materialEntryItem.genericCategory.category_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_category.category_id;
                                materialEntryItem.genericProduct.product_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_type.product_id;
                                materialEntryItem.genericBrand.brand_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_brand.brand_id;
                                materialEntryItem.genericModel.model_id = rfpItems[itemDescriptionComboBox.SelectedIndex].rfpItem.generic_product_model.model_id;


                            }


                            addEntryPermitPage.materialEntryPermit.AddItem(materialEntryItem);

                        }


                    }
   
                }

            }


            if (editable == false) {
                if (!addEntryPermitPage.materialEntryPermit.IssueNewEntryPermit())
                    return;


                if (materialReservations.Count != 0) {

                    for (int i = 0; i < materialReservations.Count; i++)
                    {
                        BASIC_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservationItem = materialReservations[i];
                        reservationItem.entry_permit_serial = addEntryPermitPage.materialEntryPermit.GetEntryPermitSerialid();

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

            for (int i = 0; i < Home.Children.Count; i++) {

                Grid card = Home.Children[i] as Grid;


                WrapPanel rfpPanel = card.Children[1] as WrapPanel;


                WrapPanel rfpRequsterPanel = card.Children[2] as WrapPanel;

                ComboBox requesterComboBox = rfpRequsterPanel.Children[1] as ComboBox;

                ComboBox rfpSerialComboBox = rfpRequsterPanel.Children[2] as ComboBox;


                WrapPanel itemDescriptionPanel = card.Children[3] as WrapPanel;

                ComboBox itemDescriptionComboBox = itemDescriptionPanel.Children[1] as ComboBox;

                CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;

                WrapPanel choicePanel = card.Children[4] as WrapPanel;

                ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;


                WrapPanel genericCategoryPanel = card.Children[5] as WrapPanel;

                ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;


                WrapPanel genericProductPanel = card.Children[6] as WrapPanel;

                ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


                WrapPanel genericBrandPanel = card.Children[7] as WrapPanel;

                ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;



                WrapPanel genericModelPanel = card.Children[8] as WrapPanel;

                ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



                WrapPanel companyCategoryPanel = card.Children[9] as WrapPanel;

                ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;



                WrapPanel companyProductPanel = card.Children[10] as WrapPanel;

                ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;




                WrapPanel companyBrandPanel = card.Children[11] as WrapPanel;

                ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;



                WrapPanel companyModelPanel = card.Children[12] as WrapPanel;

                ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;



                WrapPanel startSerialPanel = card.Children[14] as WrapPanel;

                TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;



                WrapPanel endSerialPanel = card.Children[15] as WrapPanel;

                TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;



                WrapPanel quantitylPanel = card.Children[16] as WrapPanel;

                TextBox quantityTextBox = quantitylPanel.Children[1] as TextBox;



                WrapPanel currencyPanel = card.Children[17] as WrapPanel;

                ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;




                WrapPanel pricePanel = card.Children[18] as WrapPanel;

                TextBox priceTextBox = pricePanel.Children[1] as TextBox;


                WrapPanel stockTypePanel = card.Children[19] as WrapPanel;

                ComboBox stockTypeComboBox = stockTypePanel.Children[1] as ComboBox;


                WrapPanel datePanel = card.Children[20] as WrapPanel;

                DatePicker holdUntilDate= datePanel.Children[1] as DatePicker;

                Button generateButton = card.Children[21] as Button;



                ScrollViewer scroll = card.Children[22] as ScrollViewer;

                Grid serialGrid = scroll.Content as Grid;


                generateButton.IsEnabled = false;
                requesterComboBox.IsEnabled = false;

                rfpSerialComboBox.IsEnabled = false;

                stockTypeComboBox.IsEnabled = false;

                holdUntilDate.IsEnabled = false;

                itemDescriptionComboBox.IsEnabled = false;
                choiceComboBox.IsEnabled = false;

                genericCategoryComboBox.IsEnabled = false;
                genericProductComboBox.IsEnabled = false;
                genericBrandComboBox.IsEnabled = false;
                genericModelComboBox.IsEnabled = false;

                companyCategoryComboBox.IsEnabled = false;
                companyProductComboBox.IsEnabled = false;
                companyBrandComboBox.IsEnabled = false;
                companyModelComboBox.IsEnabled = false;

                currencyComboBox.IsEnabled = false;

                startSerialTextBox.IsReadOnly = true;
                endSerialTextBox.IsReadOnly = true;
                quantityTextBox.IsReadOnly = true;
                priceTextBox.IsReadOnly = true;



                rfpCheckBox.IsEnabled = false;
                serialGrid.IsEnabled = false;


            }

            EntryPermitUploadFilesPage = new EntryPermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, addEntryPermitPage, entryPermitWindow, ref addEntryPermitPage.materialEntryPermit);


            this.NavigationService.Navigate(EntryPermitUploadFilesPage);

            this.nextButton.IsEnabled = true;
            this.finishButton.IsEnabled = false;


        }

        private void BasicInfoLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            this.NavigationService.Navigate(addEntryPermitPage);
        }

        private void AddNewItemButtonClick(object sender, RoutedEventArgs e)
        {
            InitializeNewCard();

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(addEntryPermitPage);


        }

        private void NextButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (EntryPermitUploadFilesPage != null)
                this.NavigationService.Navigate(EntryPermitUploadFilesPage);
        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EntryPermitUploadFilesPage != null)
                this.NavigationService.Navigate(EntryPermitUploadFilesPage);

        }
    }
}
