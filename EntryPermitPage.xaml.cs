using _01electronics_library;
using Spire.Doc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for EntryPermitPage.xaml
    /// </summary>
    public partial class EntryPermitPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;
        int viewAddCondition;
        Grid previousGrid=null;

        Expander previousExpander = null;

        MaterialEntryPermit entryPermit = new MaterialEntryPermit();

        List<INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT> materialEntryPermits;

        public EntryPermitPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            materialEntryPermits = new List<INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT>();
            InitializeComponent();
            if (loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
                addButton.IsEnabled = false;
            InitializeUiElements();
        }


        public void InitializeUiElements() {

            EntryPermitStackPanel.Children.Clear();

            GetMaterialEntryPermits();

            //for (int i = 0; i < materialEntryPermits.Count; i++) {


            //    for (int j = i + 1; j < materialEntryPermits.Count; j++) {


            //        if (materialEntryPermits[i].entry_permit_serial == materialEntryPermits[j].entry_permit_serial) {

            //            materialEntryPermits.Remove(materialEntryPermits[j]);
            //            j--;

            //        }
            //    }

            //}

           
            for (int i = 0; i < materialEntryPermits.Count; i++) {
                Border cardBorder = new Border();
                cardBorder.Margin = new Thickness(10);
                cardBorder.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                cardBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                cardBorder.BorderThickness = new Thickness(1);
                cardBorder.CornerRadius = new CornerRadius(20);

                DropShadowEffect shadow = new DropShadowEffect();
                shadow.BlurRadius = 20;
                shadow.Color = Color.FromRgb(211, 211, 211);
                cardBorder.Effect = shadow;

                Grid card = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
                card.RowDefinitions.Add(new RowDefinition());
                card.ColumnDefinitions.Add(new ColumnDefinition());
                card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });

                Label header = new Label();
                header.Content = $"Entry Permit - {materialEntryPermits[i].entry_permit_id}";
                header.Style = (Style)FindResource("stackPanelItemHeader");

                Grid.SetRow(header, 0);
                Grid.SetColumn(header, 0);
                card.Children.Add(header);
                card.Tag = materialEntryPermits[i].entry_permit_serial;

                card.RowDefinitions.Add(new RowDefinition());
                Label transactionDateLabel = new Label();
                transactionDateLabel.Content = materialEntryPermits[i].transaction_date;
                transactionDateLabel.Style = (Style)FindResource("stackPanelItemHeader");


                Grid.SetRow(transactionDateLabel, card.Children.Count);
                Grid.SetColumn(transactionDateLabel, 0);
                card.Children.Add(transactionDateLabel);

                card.RowDefinitions.Add(new RowDefinition());
                Label wareHouseLocationLabel = new Label();
                wareHouseLocationLabel.Content = materialEntryPermits[i].warehouse_location.location_nick_name;
                wareHouseLocationLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Grid.SetRow(wareHouseLocationLabel, card.Children.Count);
                Grid.SetColumn(wareHouseLocationLabel, 0);
                card.Children.Add(wareHouseLocationLabel);

                if (materialEntryPermits[i].rfp.rfpSerial !=0)
                {
                    card.RowDefinitions.Add(new RowDefinition());
                    Label rfpIdLabel = new Label();
                    rfpIdLabel.Style = (Style)FindResource("stackPanelItemBody");
                    rfpIdLabel.Content =materialEntryPermits[i].rfp.rfpID + "-" + materialEntryPermits[i].rfp.work_form ;
                    Grid.SetRow(rfpIdLabel, card.Children.Count);
                    Grid.SetColumn(rfpIdLabel, 0);
                    card.Children.Add(rfpIdLabel);
                }
                int itemNo = 0;
                int entryProductQuantity = 1;
                Label quantityLabel = new Label();
                for (int j = 0; j < materialEntryPermits[i].items.Count; j++)
                {
                    if (j>0 && materialEntryPermits[i].items[j].product_category.category_id == materialEntryPermits[i].items[j-1].product_category.category_id
                        && materialEntryPermits[i].items[j].product_type.type_id == materialEntryPermits[i].items[j-1].product_type.type_id
                        && materialEntryPermits[i].items[j].product_brand.brand_id== materialEntryPermits[i].items[j - 1].product_brand.brand_id
                        && materialEntryPermits[i].items[j].product_model.model_id == materialEntryPermits[i].items[j - 1].product_model.model_id
                        && materialEntryPermits[i].items[j].product_specs.spec_id == materialEntryPermits[i].items[j - 1].product_specs.spec_id)
                    {
                        entryProductQuantity++;
                        quantityLabel.Content = "Item Quantity: " + entryProductQuantity;
                        continue;
                    }
                    else
                    {
                        if (materialEntryPermits[i].items[j].product_serial_number != null && materialEntryPermits[i].items[j].quantity == 0)
                            entryProductQuantity = 1;
                    }
                    if(materialEntryPermits[i].items[j].quantity!=0)
                    {
                        entryProductQuantity = materialEntryPermits[i].items[j].quantity;
                    }
                    card.RowDefinitions.Add(new RowDefinition());
                    Label itemsLabel = new Label();
                    itemsLabel.Margin = new Thickness(0,0,0,10);
                    itemsLabel.Style = (Style)FindResource("stackPanelItemBody");
                    itemsLabel.Content = $@"Item {itemNo += 1}: "+materialEntryPermits[i].items[j].product_category.category_name+"-"+
                                          materialEntryPermits[i].items[j].product_type.product_name+"-"+
                                          materialEntryPermits[i].items[j].product_brand.brand_name+"-"+
                                          materialEntryPermits[i].items[j].product_model.model_name+"-"+
                                          materialEntryPermits[i].items[j].product_specs.spec_name ;

                    
                    card.RowDefinitions.Add(new RowDefinition());

                    Border quantity = new Border();
                    quantity.Margin = new Thickness(5);
                    quantity.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                    quantity.Background = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                    quantity.BorderThickness = new Thickness(1);
                    quantity.CornerRadius = new CornerRadius(10);
                    quantity.HorizontalAlignment = HorizontalAlignment.Right;
                    quantity.Width = 170;


                    quantityLabel = new Label();
                    quantityLabel.Content = "Item Quantity: " + entryProductQuantity;
                    quantityLabel.Style = (Style)FindResource("stackPanelItemHeader");
                    quantityLabel.Foreground = Brushes.White;
                    quantityLabel.FontSize = 12;
                    quantityLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    quantity.Child = quantityLabel;

                    Grid.SetRow(itemsLabel,card.Children.Count);
                    Grid.SetColumn(itemsLabel, 0);

                    Grid.SetRow(quantity,card.Children.Count);
                    Grid.SetColumn(quantity,1);
                 
                    card.Children.Add(itemsLabel);
                    card.Children.Add(quantity);
                }

                StackPanel expand = new StackPanel();
                Button editButton = new Button();
                editButton.Content = "EDIT";
                editButton.Click += EditButtonClick;

                Button viewButton = new Button();
                viewButton.Click += ViewButtonOnClick;
                viewButton.Content = "VIEW";

                expand.Children.Add(viewButton);
                if(loggedInUser.GetEmployeeTeamId()==COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
                expand.Children.Add(editButton);

                card.RowDefinitions.Add(new RowDefinition());
                Expander expander = new Expander();
                expander.Expanded += ExpanderExpanded;
                expander.Content = expand;
                expander.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(expander, 0);
                Grid.SetColumn(expander, 1);

                card.Children.Add(expander);
                cardBorder.Child= card;
                EntryPermitStackPanel.Children.Add(cardBorder);

            }




        }
        public bool GetMaterialEntryPermits() {

            if (!commonQueries.GetEntryPermits(ref materialEntryPermits))
                return false;

            return true;
        }
        private void ExpanderExpanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;

            if (previousExpander != null&&previousExpander!= expander) {

                previousExpander.IsExpanded = false;
            }

            previousExpander = expander;

        }
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {

            Button viewButton = sender as Button;

            StackPanel stackPanel = viewButton.Parent as StackPanel;
            Expander expander = stackPanel.Parent as Expander;
            Grid card = expander.Parent as Grid;


            INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT materialEntryPermit = materialEntryPermits.FirstOrDefault(a => a.entry_permit_serial == Convert.ToInt32(card.Tag));


            entryPermit.SetEntryPermitSerialid(materialEntryPermit.entry_permit_serial);
            entryPermit.InitializeMaterialEntryPermit();

            AddEntryPermitWindow addEntryPermitWindow = new AddEntryPermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser,COMPANY_WORK_MACROS.ENTRY_PERMIT_EDIT_CONDITION, ref entryPermit,this);


            addEntryPermitWindow.EntryPermitPage.TransactionDatePicker.Text = materialEntryPermit.transaction_date.ToString();

            addEntryPermitWindow.EntryPermitPage.WareHouseCombo.SelectedItem = materialEntryPermit.warehouse_location.location_nick_name;

            addEntryPermitWindow.EntryPermitPage.entryPermitIdTextBox.Text = materialEntryPermit.entry_permit_id;




            //bool hasSerial = false;

            //int m;
            //for (int i = 0; i < entryPermit.GetItems().Count; i++)
            //{

            //    m = i;
            //    if (hasSerial == false)
            //    {
            //        if (i > 0)
            //            addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.InitializeNewCard();
            //    }
            //    else
            //    {

            //        if (i > 0)
            //        {

            //            m = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children.Count - 1;
            //        }


            //    }



            //    if (entryPermit.GetItems()[i].rfp_info.rfpSerial == 0)
            //    {

            //        if (!entryPermit.GetItems()[i].is_company_product)
            //        {


            //            Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //            WrapPanel rfpCheckPanel = itemCard.Children[1] as WrapPanel;

            //            CheckBox rfpCheckBox = rfpCheckPanel.Children[1] as CheckBox;


            //            ScrollViewer scroll = itemCard.Children[19] as ScrollViewer;

            //            Button generateButton = itemCard.Children[18] as Button;

            //            Grid serialsGrid = scroll.Content as Grid;


            //            WrapPanel startSerialPanel = itemCard.Children[13] as WrapPanel;

            //            TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;



            //            WrapPanel endSerialPanel = itemCard.Children[14] as WrapPanel;

            //            TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;


            //            WrapPanel quantityPanel = itemCard.Children[15] as WrapPanel;

            //            TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;



            //            WrapPanel pricePanel = itemCard.Children[17] as WrapPanel;

            //            TextBox priceTextBox = pricePanel.Children[1] as TextBox;


            //            WrapPanel currencyPanel = itemCard.Children[16] as WrapPanel;

            //            ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;

            //            WrapPanel choicePanel = itemCard.Children[4] as WrapPanel;

            //            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            //            choiceComboBox.SelectedIndex = 0;



            //            WrapPanel genericCategoryPanel = itemCard.Children[5] as WrapPanel;

            //            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;

            //            genericCategoryComboBox.SelectedItem = entryPermit.GetItems()[i].product_category.category_name;



            //            WrapPanel genericProductPanel = itemCard.Children[6] as WrapPanel;

            //            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


            //            genericProductComboBox.SelectedItem = entryPermit.GetItems()[i].product_type.product_name;



            //            WrapPanel genericBrandPanel = itemCard.Children[7] as WrapPanel;

            //            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            //            genericBrandComboBox.SelectedItem = entryPermit.GetItems()[i].product_brand.brand_name;


            //            WrapPanel genericModelPanel = itemCard.Children[8] as WrapPanel;

            //            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;


            //            genericModelComboBox.SelectedItem = entryPermit.GetItems()[i].product_model.model_name;


            //            priceTextBox.Text = entryPermit.GetItems()[i].item_price.ToString();

            //            currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == entryPermit.GetItems()[i].item_currency.currencyId).currencyName;

            //            quantityTextBox.Text = entryPermit.GetItems()[i].quantity.ToString();




            //            if (entryPermit.GetItems()[i].product_serial_number != "0")
            //            {
            //                WrapPanel panel1 = new WrapPanel();
            //                Label serialNumber = new Label();
            //                serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                if (entryPermit.GetItems()[i].entry_permit_item_serial == 1)
            //                    startSerialTextBox.Text = entryPermit.GetItems()[i].product_serial_number;

            //                try
            //                {
            //                    if (entryPermit.GetItems()[i].entry_permit_item_serial != 1 && ((entryPermit.GetItems()[i + 1].product_serial_number == "0" || entryPermit.GetItems()[i + 1].entry_permit_item_serial == 1)))
            //                    {
            //                        endSerialTextBox.Text = entryPermit.GetItems()[i].product_serial_number;

            //                    }
            //                }

            //                catch
            //                {

            //                    endSerialTextBox.Text = entryPermit.GetItems()[i].product_serial_number;


            //                }


            //                serialNumber.Content = $"Serial {entryPermit.GetItems()[i].entry_permit_item_serial}";

            //                TextBox textSerial = new TextBox();


            //                textSerial.Text = entryPermit.GetItems()[i].product_serial_number;

            //                textSerial.Style = (Style)FindResource("textBoxStyle");

            //                serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                panel1.Children.Add(serialNumber);
            //                panel1.Children.Add(textSerial);


            //                Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                serialsGrid.Children.Add(panel1);

            //                try
            //                {
            //                    if (entryPermit.GetItems()[i + 1].product_serial_number == "0")
            //                        hasSerial = false;

            //                    else if (entryPermit.GetItems()[i].entry_permit_item_serial == entryPermit.GetItems()[i + 1].entry_permit_item_serial - 1 && entryPermit.GetItems()[i + 1].entry_permit_item_serial != 0)
            //                    {

            //                        hasSerial = true;

            //                    }
            //                    else
            //                        hasSerial = false;


            //                }
            //                catch
            //                {

            //                }



            //            }



            //        }

            //        else if (entryPermit.GetItems()[i].is_company_product)
            //        {

            //            Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //            WrapPanel quantityPanel = itemCard.Children[16] as WrapPanel;

            //            TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;



            //            WrapPanel startSerialPanel = itemCard.Children[14] as WrapPanel;

            //            TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;



            //            WrapPanel endSerialPanel = itemCard.Children[15] as WrapPanel;

            //            TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;



            //            ScrollViewer scroll = itemCard.Children[22] as ScrollViewer;

            //            Button generateButton = itemCard.Children[21] as Button;

            //            Grid serialsGrid = scroll.Content as Grid;



            //            WrapPanel pricePanel = itemCard.Children[18] as WrapPanel;

            //            TextBox priceTextBox = pricePanel.Children[1] as TextBox;


            //            WrapPanel currencyPanel = itemCard.Children[17] as WrapPanel;

            //            ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;


            //            WrapPanel choicePanel = itemCard.Children[4] as WrapPanel;

            //            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;


            //            choiceComboBox.SelectedIndex = 1;

            //            WrapPanel CompanyCategoryPanel = itemCard.Children[9] as WrapPanel;

            //            ComboBox companyCategoryComboBox = CompanyCategoryPanel.Children[1] as ComboBox;

            //            //companyCategoryComboBox.SelectedItem = entryPermit.GetItems()[i].company.product_name;


            //            WrapPanel CompanyProductPanel = itemCard.Children[10] as WrapPanel;

            //            ComboBox companyProductComboBox = CompanyProductPanel.Children[1] as ComboBox;

            //            companyProductComboBox.SelectedItem = entryPermit.GetItems()[i].product_type.product_name;



            //            WrapPanel companyBrandPanel = itemCard.Children[11] as WrapPanel;

            //            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            //            companyBrandComboBox.SelectedItem = entryPermit.GetItems()[i].product_brand.brand_name;



            //            WrapPanel companyModelPanel = itemCard.Children[12] as WrapPanel;

            //            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            //            companyModelComboBox.SelectedItem = entryPermit.GetItems()[i].product_model.model_name;


            //            priceTextBox.Text = entryPermit.GetItems()[i].item_price.ToString();

            //            currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == entryPermit.GetItems()[i].item_currency.currencyId).currencyName;

            //            quantityTextBox.Text = entryPermit.GetItems()[i].quantity.ToString();



            //            if (entryPermit.GetItems()[i].product_serial_number != "0")
            //            {
            //                WrapPanel panel1 = new WrapPanel();
            //                Label serialNumber = new Label();
            //                serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                if (entryPermit.GetItems()[i].entry_permit_item_serial == 1)
            //                    startSerialTextBox.Text = entryPermit.GetItems()[i].product_serial_number;

            //                try
            //                {
            //                    if (entryPermit.GetItems()[i].entry_permit_item_serial != 1 && ((entryPermit.GetItems()[i + 1].product_serial_number == "0" || entryPermit.GetItems()[i + 1].entry_permit_item_serial == 1)))
            //                    {
            //                        endSerialTextBox.Text = entryPermit.GetItems()[i].product_serial_number;

            //                    }
            //                }

            //                catch
            //                {
            //                }


            //                serialNumber.Content = $"Serial {entryPermit.GetItems()[i].entry_permit_item_serial}";

            //                TextBox textSerial = new TextBox();


            //                textSerial.Text = entryPermit.GetItems()[i].product_serial_number;

            //                textSerial.Style = (Style)FindResource("textBoxStyle");

            //                serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                panel1.Children.Add(serialNumber);
            //                panel1.Children.Add(textSerial);


            //                Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                serialsGrid.Children.Add(panel1);

            //                try
            //                {
            //                    if (entryPermit.GetItems()[i + 1].product_serial_number == "0")
            //                        hasSerial = false;

            //                    else if (entryPermit.GetItems()[i].entry_permit_item_serial == entryPermit.GetItems()[i + 1].entry_permit_item_serial - 1 && entryPermit.GetItems()[i + 1].entry_permit_item_serial != 0)
            //                    {

            //                        hasSerial = true;

            //                    }
            //                    else
            //                        hasSerial = false;


            //                }
            //                catch
            //                {

            //                }

            //            }


            //        }

            //    }


            //    else
            //    {

            //        Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;


            //        WrapPanel quantityPanel = itemCard.Children[15] as WrapPanel;

            //        TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;


            //        WrapPanel pricePanel = itemCard.Children[17] as WrapPanel;

            //        TextBox priceTextBox = pricePanel.Children[1] as TextBox;

            //        WrapPanel currencyPanel = itemCard.Children[16] as WrapPanel;

            //        ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;



            //        WrapPanel rfpPanel = itemCard.Children[1] as WrapPanel;

            //        CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;

            //        rfpCheckBox.IsChecked = true;

            //        WrapPanel rfpItemDescriptionPanel = itemCard.Children[3] as WrapPanel;

            //        ComboBox rfpItemDescriptionComboBox = rfpItemDescriptionPanel.Children[1] as ComboBox;



            //        WrapPanel rfpRequsterPanel = itemCard.Children[2] as WrapPanel;

            //        ComboBox requsterComboBox = rfpRequsterPanel.Children[1] as ComboBox;

            //        requsterComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRequsters().FirstOrDefault(a => a.requestor_team.team_id == entryPermit.GetItems()[i].rfp_info.rfpRequestorTeam).requestor_team.team_name;


            //        ComboBox rfpSerialComboBox = rfpRequsterPanel.Children[3] as ComboBox;


            //        addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfps().Clear();

            //        List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT> rfps = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfps();

            //        commonQueries.GetTeamRFPs(ref rfps, addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRequsters()[requsterComboBox.SelectedIndex].requestor_team.team_id);


            //        rfpSerialComboBox.Items.Clear();
            //        rfps.ForEach(a => rfpSerialComboBox.Items.Add(a.rfpID));

            //        rfpSerialComboBox.SelectedItem = rfps.FirstOrDefault(a => a.rfpSerial == entryPermit.GetItems()[i].rfp_info.rfpSerial && a.rfpVersion == entryPermit.GetItems()[i].rfp_info.rfpVersion).rfpID;



            //        List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfpItems();

            //        rfpItems.Clear();

            //        commonQueries.GetRfpItemsMapping(entryPermit.GetItems()[i].rfp_info.rfpSerial, entryPermit.GetItems()[i].rfp_info.rfpVersion, entryPermit.GetItems()[i].rfp_info.rfpRequestorTeam, ref rfpItems);


            //        rfpItemDescriptionComboBox.Items.Clear();



            //        for (int j = 0; j < rfpItems.Count; j++)
            //        {


            //            if (rfpItems[i].product_category.category_id == 0)
            //            {

            //                rfpItemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);

            //            }
            //            else
            //            {

            //                rfpItemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);


            //            }


            //        }


            //        PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT item = rfpItems.FirstOrDefault(a => a.rfpSerial == entryPermit.GetItems()[i].rfp_info.rfpSerial && a.rfpVersion == entryPermit.GetItems()[i].rfp_info.rfpVersion && a.rfpRequestorTeam == entryPermit.GetItems()[i].rfp_info.rfpRequestorTeam && a.rfp_item_number == entryPermit.GetItems()[i].rfp_item_number);



            //        if (rfpItems[i].product_category.category_id == 0)
            //        {

            //            rfpItemDescriptionComboBox.SelectedItem = item.product_category.category_name + "," + item.product_type.product_name + "," + item.product_brand.brand_name + "," + item.product_model.model_name;

            //        }
            //        else
            //        {

            //            rfpItemDescriptionComboBox.SelectedItem = item.product_category.category_name + "," + item.product_type.product_name + "," + item.product_brand.brand_name + "," + item.product_model.model_name;


            //        }


            //        priceTextBox.Text = entryPermit.GetItems()[i].item_price.ToString();

            //        currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == entryPermit.GetItems()[i].item_currency.currencyId).currencyName;

            //        quantityTextBox.Text = entryPermit.GetItems()[i].quantity.ToString();



            //    }

            //}


            addEntryPermitWindow.Show();
        }
        private void ViewButtonOnClick(object sender, RoutedEventArgs e)
        {

            Button viewButton=sender as Button;

            StackPanel stackPanel= viewButton.Parent as StackPanel;
            Expander expander=  stackPanel.Parent as Expander;
            Grid card= expander.Parent as Grid;

            INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT materialEntryPermit=materialEntryPermits.FirstOrDefault(a => a.entry_permit_serial == Convert.ToInt32(card.Tag));


            entryPermit.SetEntryPermitSerialid(materialEntryPermit.entry_permit_serial);
            entryPermit.InitializeMaterialEntryPermit();

            AddEntryPermitWindow addEntryPermitWindow = new AddEntryPermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser,COMPANY_WORK_MACROS.ENTRY_PERMIT_VIEW_CONDITION,ref entryPermit);
           // addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.a.Visibility = Visibility.Collapsed;
            addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.EntryPermitUploadFilesPage = new EntryPermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addEntryPermitWindow.EntryPermitPage.addEntryPermitItem, addEntryPermitWindow.EntryPermitPage, addEntryPermitWindow,ref entryPermit);

            addEntryPermitWindow.EntryPermitPage.TransactionDatePicker.IsEnabled = false;

            addEntryPermitWindow.EntryPermitPage.WareHouseCombo.IsEnabled = false;
            addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.finishButton.IsEnabled = false;

            addEntryPermitWindow.EntryPermitPage.TransactionDatePicker.Text = materialEntryPermit.transaction_date.ToString();

            addEntryPermitWindow.EntryPermitPage.WareHouseCombo.SelectedItem = materialEntryPermit.warehouse_location.location_nick_name;

            addEntryPermitWindow.EntryPermitPage.entryPermitIdTextBox.Text = materialEntryPermit.entry_permit_id;

            addEntryPermitWindow.EntryPermitPage.entryPermitIdTextBox.IsReadOnly = true;

            
            addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.finishButton.IsEnabled = false;


            //int m=0;

            //int itemCounter = 1;




            //List<INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT> items = new List<INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT>();


            //for (int i = 0; i < entryPermit.GetItems().Count; i++) {


            //    if (entryPermit.GetItems()[i].product_type.product_name == "")
            //    {

            //        List<INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT> itemm = entryPermit.GetItems().Where(a => a.product_category.category_id == entryPermit.GetItems()[i].product_category.category_id && a.product_type.type_id == entryPermit.GetItems()[i].product_type.type_id && a.product_brand.brand_id == entryPermit.GetItems()[i].product_brand.brand_id && a.product_model.model_id == entryPermit.GetItems()[i].product_model.model_id).ToList();

            //        itemm.ForEach(a => items.Add(a));


            //        entryPermit.GetItems().RemoveAll(a => a.product_category.category_id == entryPermit.GetItems()[i].product_category.category_id && a.product_type.type_id == entryPermit.GetItems()[i].product_type.type_id && a.product_brand.brand_id == entryPermit.GetItems()[i].product_brand.brand_id && a.product_model.model_id == entryPermit.GetItems()[i].product_model.model_id);
            //        i--;
            //    }

            //    else {

            //        List<INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT> itemm = entryPermit.GetItems().Where(a => a.product_type.type_id == entryPermit.GetItems()[i].product_type.type_id && a.product_brand.brand_id == entryPermit.GetItems()[i].product_brand.brand_id && a.product_model.model_id == entryPermit.GetItems()[i].product_model.model_id).ToList();

            //        itemm.ForEach(a => items.Add(a));


            //        entryPermit.GetItems().RemoveAll(a => a.product_type.type_id == entryPermit.GetItems()[i].product_type.type_id && a.product_brand.brand_id == entryPermit.GetItems()[i].product_brand.brand_id && a.product_model.model_id == entryPermit.GetItems()[i].product_model.model_id);

            //        i--;

            //    }

            //}
            

            //for (int i = 0; i < items.Count; i++) {


            //    if (items[i].rfp_info.rfpSerial == 0)
            //    {

            //        if (items[i].product_type.product_name == "")
            //        {


            //            Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //            WrapPanel rfpCheckPanel = itemCard.Children[1] as WrapPanel;

            //            CheckBox rfpCheckBox = rfpCheckPanel.Children[1] as CheckBox;

            //            rfpCheckBox.IsEnabled = false;

            //            ScrollViewer scroll = itemCard.Children[19] as ScrollViewer;

            //            Button generateButton = itemCard.Children[18] as Button;
            //            generateButton.IsEnabled = false;

            //            Grid serialsGrid = scroll.Content as Grid;


            //            WrapPanel startSerialPanel = itemCard.Children[14] as WrapPanel;

            //            TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;
            //            startSerialTextBox.IsEnabled = false;



            //            WrapPanel endSerialPanel = itemCard.Children[15] as WrapPanel;

            //            TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;

            //            endSerialTextBox.IsEnabled = false;

            //            WrapPanel quantityPanel = itemCard.Children[16] as WrapPanel;

            //            TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;

            //            quantityTextBox.IsEnabled = false;


            //            WrapPanel pricePanel = itemCard.Children[18] as WrapPanel;

            //            TextBox priceTextBox = pricePanel.Children[1] as TextBox;
            //            priceTextBox.IsEnabled = false;


            //            WrapPanel currencyPanel = itemCard.Children[17] as WrapPanel;

            //            ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;
            //            currencyComboBox.IsEnabled = false;

            //            WrapPanel choicePanel = itemCard.Children[4] as WrapPanel;

            //            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            //            choiceComboBox.SelectedIndex = 0;

            //            choiceComboBox.IsEnabled = false;


            //            WrapPanel genericCategoryPanel = itemCard.Children[5] as WrapPanel;

            //            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;
            //            genericCategoryComboBox.IsEnabled = false;

            //            genericCategoryComboBox.SelectedItem = items[i].product_category.category_name;



            //            WrapPanel genericProductPanel = itemCard.Children[6] as WrapPanel;

            //            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;
            //            genericProductComboBox.IsEnabled = false;


            //            genericProductComboBox.SelectedItem = items[i].product_type.product_name;



            //            WrapPanel genericBrandPanel = itemCard.Children[7] as WrapPanel;

            //            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;
            //            genericBrandComboBox.IsEnabled = false;


            //            genericBrandComboBox.SelectedItem = items[i].product_brand.brand_name;


            //            WrapPanel genericModelPanel = itemCard.Children[8] as WrapPanel;

            //            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;
            //            genericModelComboBox.IsEnabled = false;


            //            genericModelComboBox.SelectedItem = items[i].product_model.model_name;


            //            priceTextBox.Text = items[i].item_price.ToString();

            //            currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //            quantityTextBox.Text = items[i].quantity.ToString();




            //            if (items[i].product_serial_number != "")
            //            {
            //                if (i != 0)
            //                {

            //                    if (items[i - 1].product_category.category_id == items[i].product_category.category_id &&
            //                        items[i - 1].product_type.type_id == items[i].product_type.type_id &&
            //                        items[i - 1].product_brand.brand_id == items[i].product_brand.brand_id &&
            //                        items[i - 1].product_model.model_id == items[i].product_model.model_id)

            //                    {

            //                        WrapPanel panel1 = new WrapPanel();
            //                        Label serialNumber = new Label();
            //                        serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                        serialNumber.Content = $"Serial {itemCounter}";

            //                        itemCounter++;
            //                        TextBox textSerial = new TextBox();
            //                        textSerial.IsEnabled = false;


            //                        textSerial.Text = items[i].product_serial_number;

            //                        textSerial.Style = (Style)FindResource("textBoxStyle");

            //                        serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                        panel1.Children.Add(serialNumber);
            //                        panel1.Children.Add(textSerial);


            //                        Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                        serialsGrid.Children.Add(panel1);



            //                    }

            //                    else
            //                    {

            //                        itemCounter = 1;
            //                        addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.InitializeNewCard();

            //                        m = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children.Count - 1;


            //                        Grid itemCard1 = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;



            //                        WrapPanel rfpCheckPanel1 = itemCard1.Children[1] as WrapPanel;

            //                        CheckBox rfpCheckBox1 = rfpCheckPanel1.Children[1] as CheckBox;

            //                        rfpCheckBox1.IsEnabled = false;


            //                        ScrollViewer scroll1 = itemCard.Children[22] as ScrollViewer;


            //                        Grid serialsGrid1 = scroll1.Content as Grid;

            //                        WrapPanel panel1 = new WrapPanel();
            //                        Label serialNumber = new Label();
            //                        serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                        serialNumber.Content = $"Serial {itemCounter}";

            //                        TextBox textSerial = new TextBox();
            //                        textSerial.IsEnabled = false;


            //                        textSerial.Text = items[i].product_serial_number;

            //                        textSerial.Style = (Style)FindResource("textBoxStyle");

            //                        serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                        panel1.Children.Add(serialNumber);
            //                        panel1.Children.Add(textSerial);


            //                        Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                        serialsGrid.Children.Add(panel1);

            //                    }




            //                }

            //                else
            //                {


            //                    startSerialTextBox.Text = items[i].product_serial_number;


            //                    WrapPanel panel1 = new WrapPanel();
            //                    Label serialNumber = new Label();
            //                    serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                    serialNumber.Content = $"Serial {items[i].entry_permit_item_serial}";

            //                    TextBox textSerial = new TextBox();
            //                    textSerial.IsEnabled = false;


            //                    textSerial.Text = items[i].product_serial_number;

            //                    textSerial.Style = (Style)FindResource("textBoxStyle");

            //                    serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                    panel1.Children.Add(serialNumber);
            //                    panel1.Children.Add(textSerial);


            //                    Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                    serialsGrid.Children.Add(panel1);

            //                }


            //            }



            //        }

            //        else if (items[i].product_category.category_name == "")
            //        {

   
            //            if (items[i].product_serial_number != "")
            //            {
            //                //Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //                //WrapPanel quantityPanel = itemCard.Children[15] as WrapPanel;

            //                //TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;

            //                //quantityTextBox.IsEnabled = false;


            //                //WrapPanel startSerialPanel = itemCard.Children[13] as WrapPanel;

            //                //TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;
            //                //startSerialTextBox.IsEnabled = false;



            //                //WrapPanel endSerialPanel = itemCard.Children[14] as WrapPanel;

            //                //TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;

            //                //endSerialTextBox.IsEnabled = false;


            //                //ScrollViewer scroll = itemCard.Children[19] as ScrollViewer;

            //                //Button generateButton = itemCard.Children[18] as Button;
            //                //generateButton.IsEnabled = false;

            //                //Grid serialsGrid = scroll.Content as Grid;



            //                //WrapPanel pricePanel = itemCard.Children[17] as WrapPanel;

            //                //TextBox priceTextBox = pricePanel.Children[1] as TextBox;

            //                //priceTextBox.IsEnabled = false;

            //                //WrapPanel currencyPanel = itemCard.Children[16] as WrapPanel;

            //                //ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;

            //                //currencyComboBox.IsEnabled = false;

            //                //WrapPanel choicePanel = itemCard.Children[4] as WrapPanel;

            //                //ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            //                //choiceComboBox.IsEnabled = false;

            //                //choiceComboBox.SelectedIndex = 1;

            //                //WrapPanel CompanyCategoryPanel = itemCard.Children[9] as WrapPanel;

            //                //ComboBox companyCategoryComboBox = CompanyCategoryPanel.Children[1] as ComboBox;

            //                ////companyCategoryComboBox.SelectedItem = entryPermit.GetItems()[i].company.product_name;

            //                //companyCategoryComboBox.IsEnabled = false;


            //                //WrapPanel CompanyProductPanel = itemCard.Children[10] as WrapPanel;

            //                //ComboBox companyProductComboBox = CompanyProductPanel.Children[1] as ComboBox;

            //                //companyProductComboBox.SelectedItem = items[i].product_type.product_name;

            //                //companyProductComboBox.IsEnabled = false;


            //                //WrapPanel companyBrandPanel = itemCard.Children[11] as WrapPanel;

            //                //ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            //                //companyBrandComboBox.SelectedItem = items[i].product_brand.brand_name;

            //                //companyBrandComboBox.IsEnabled = false;


            //                //WrapPanel companyModelPanel = itemCard.Children[12] as WrapPanel;

            //                //ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            //                //companyModelComboBox.SelectedItem = items[i].product_model.model_name;
            //                //companyModelComboBox.IsEnabled = false;



            //                //priceTextBox.Text = items[i].item_price.ToString();

            //                //currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //                //quantityTextBox.Text = items[i].quantity.ToString();


            //                if (i != 0)
            //                {

            //                    if (items[i - 1].product_type.type_id == items[i].product_type.type_id &&
            //                        items[i - 1].product_brand.brand_id == items[i].product_brand.brand_id &&
            //                        items[i - 1].product_model.model_id == items[i].product_model.model_id)

            //                    {

            //                        Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //                        WrapPanel quantityPanel = itemCard.Children[16] as WrapPanel;

            //                        TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;

            //                        quantityTextBox.IsEnabled = false;


            //                        WrapPanel startSerialPanel = itemCard.Children[14] as WrapPanel;

            //                        TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;
            //                        startSerialTextBox.IsEnabled = false;



            //                        WrapPanel endSerialPanel = itemCard.Children[15] as WrapPanel;

            //                        TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;

            //                        endSerialTextBox.IsEnabled = false;


            //                        if (items.Count - 1 == i) {

            //                            endSerialTextBox.Text = items[i].product_serial_number;
            //                        }


            //                        ScrollViewer scroll = itemCard.Children[22] as ScrollViewer;

            //                        Button generateButton = itemCard.Children[21] as Button;
            //                        generateButton.IsEnabled = false;

            //                        Grid serialsGrid = scroll.Content as Grid;

            //                        WrapPanel panel1 = new WrapPanel();
            //                        Label serialNumber = new Label();
            //                        serialNumber.Style = (Style)FindResource("tableItemLabel");

            //                        itemCounter++;

            //                        serialNumber.Content = $"Serial {itemCounter}";

            //                        TextBox textSerial = new TextBox();
            //                        textSerial.IsEnabled = false;







            //                        textSerial.Text = items[i].product_serial_number;

            //                        textSerial.Style = (Style)FindResource("textBoxStyle");

            //                        serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                        panel1.Children.Add(serialNumber);
            //                        panel1.Children.Add(textSerial);


            //                        Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                        serialsGrid.Children.Add(panel1);



            //                    }

            //                    else
            //                    {

            //                        itemCounter = 1;
            //                        addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.InitializeNewCard();

            //                        m = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children.Count - 1;

            //                        Grid itemCard1 = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;



            //                        WrapPanel rfpCheckPanel1 = itemCard1.Children[1] as WrapPanel;

            //                        CheckBox rfpCheckBox1 = rfpCheckPanel1.Children[1] as CheckBox;

            //                        rfpCheckBox1.IsEnabled = false;


            //                        WrapPanel quantityPanel1 = itemCard1.Children[16] as WrapPanel;

            //                        TextBox quantityTextBox1 = quantityPanel1.Children[1] as TextBox;

            //                        quantityTextBox1.IsEnabled = false;


            //                        WrapPanel startSerialPanel1 = itemCard1.Children[14] as WrapPanel;

            //                        TextBox startSerialTextBox1 = startSerialPanel1.Children[1] as TextBox;
            //                        startSerialTextBox1.IsEnabled = false;



            //                        WrapPanel endSerialPanel1 = itemCard1.Children[15] as WrapPanel;

            //                        TextBox endSerialTextBox1 = endSerialPanel1.Children[1] as TextBox;

            //                        endSerialTextBox1.IsEnabled = false;


            //                        ScrollViewer scroll1 = itemCard1.Children[22] as ScrollViewer;

            //                        Button generateButton1 = itemCard1.Children[21] as Button;
            //                        generateButton1.IsEnabled = false;

            //                        Grid serialsGrid1 = scroll1.Content as Grid;



            //                        WrapPanel pricePanel1 = itemCard1.Children[18] as WrapPanel;

            //                        TextBox priceTextBox1 = pricePanel1.Children[1] as TextBox;

            //                        priceTextBox1.IsEnabled = false;

            //                        WrapPanel currencyPanel1 = itemCard1.Children[17] as WrapPanel;

            //                        ComboBox currencyComboBox1 = currencyPanel1.Children[1] as ComboBox;

            //                        currencyComboBox1.IsEnabled = false;

            //                        WrapPanel choicePanel1 = itemCard1.Children[4] as WrapPanel;

            //                        ComboBox choiceComboBox1 = choicePanel1.Children[1] as ComboBox;

            //                        choiceComboBox1.IsEnabled = false;

            //                        choiceComboBox1.SelectedIndex = 1;

            //                        WrapPanel CompanyCategoryPanel1 = itemCard1.Children[9] as WrapPanel;

            //                        ComboBox companyCategoryComboBox1 = CompanyCategoryPanel1.Children[1] as ComboBox;

            //                        companyCategoryComboBox1.SelectedItem = items[i].product_category.category_name;

            //                        companyCategoryComboBox1.IsEnabled = false;


            //                        WrapPanel CompanyProductPanel1 = itemCard1.Children[10] as WrapPanel;

            //                        ComboBox companyProductComboBox1 = CompanyProductPanel1.Children[1] as ComboBox;

            //                        companyProductComboBox1.SelectedItem = items[i].product_type.product_name;

            //                        companyProductComboBox1.IsEnabled = false;


            //                        WrapPanel companyBrandPanel1 = itemCard1.Children[11] as WrapPanel;

            //                        ComboBox companyBrandComboBox1 = companyBrandPanel1.Children[1] as ComboBox;

            //                        companyBrandComboBox1.SelectedItem = items[i].product_brand.brand_name;

            //                        companyBrandComboBox1.IsEnabled = false;


            //                        WrapPanel companyModelPanel1 = itemCard1.Children[12] as WrapPanel;

            //                        ComboBox companyModelComboBox1 = companyModelPanel1.Children[1] as ComboBox;

            //                        companyModelComboBox1.SelectedItem = items[i].product_model.model_name;
            //                        companyModelComboBox1.IsEnabled = false;


            //                        priceTextBox1.Text = items[i].item_price.ToString();

            //                        currencyComboBox1.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //                        quantityTextBox1.Text = items[i].quantity.ToString();


            //                        WrapPanel panel1 = new WrapPanel();
            //                        Label serialNumber = new Label();
            //                        serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                        serialNumber.Content = $"Serial {itemCounter}";

            //                        TextBox textSerial = new TextBox();
            //                        textSerial.IsEnabled = false;


            //                        textSerial.Text = items[i].product_serial_number;

            //                        textSerial.Style = (Style)FindResource("textBoxStyle");

            //                        serialsGrid1.RowDefinitions.Add(new RowDefinition());

            //                        panel1.Children.Add(serialNumber);
            //                        panel1.Children.Add(textSerial);


            //                        Grid.SetRow(panel1, serialsGrid1.RowDefinitions.Count - 1);

            //                        serialsGrid1.Children.Add(panel1);


            //                    }

            //                }

            //                else
            //                {

            //                    itemCounter = 1;

            //                    Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;


            //                    WrapPanel rfpCheckPanel1 = itemCard.Children[1] as WrapPanel;

            //                    CheckBox rfpCheckBox1 = rfpCheckPanel1.Children[1] as CheckBox;

            //                    rfpCheckBox1.IsEnabled = false;


            //                    WrapPanel quantityPanel = itemCard.Children[16] as WrapPanel;

            //                    TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;

            //                    quantityTextBox.IsEnabled = false;


            //                    WrapPanel startSerialPanel = itemCard.Children[14] as WrapPanel;

            //                    TextBox startSerialTextBox = startSerialPanel.Children[1] as TextBox;
            //                    startSerialTextBox.IsEnabled = false;



            //                    WrapPanel endSerialPanel = itemCard.Children[15] as WrapPanel;

            //                    TextBox endSerialTextBox = endSerialPanel.Children[1] as TextBox;

            //                    endSerialTextBox.IsEnabled = false;


            //                    ScrollViewer scroll = itemCard.Children[22] as ScrollViewer;

            //                    Button generateButton = itemCard.Children[21] as Button;
            //                    generateButton.IsEnabled = false;

            //                    Grid serialsGrid = scroll.Content as Grid;



            //                    WrapPanel pricePanel = itemCard.Children[18] as WrapPanel;

            //                    TextBox priceTextBox = pricePanel.Children[1] as TextBox;

            //                    priceTextBox.IsEnabled = false;

            //                    WrapPanel currencyPanel = itemCard.Children[17] as WrapPanel;

            //                    ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;

            //                    currencyComboBox.IsEnabled = false;

            //                    WrapPanel choicePanel = itemCard.Children[4] as WrapPanel;

            //                    ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            //                    choiceComboBox.IsEnabled = false;

            //                    choiceComboBox.SelectedIndex = 1;

            //                    WrapPanel CompanyCategoryPanel = itemCard.Children[9] as WrapPanel;

            //                    ComboBox companyCategoryComboBox = CompanyCategoryPanel.Children[1] as ComboBox;

            //                    companyCategoryComboBox.SelectedItem = items[i].product_category.category_name;

            //                    companyCategoryComboBox.IsEnabled = false;


            //                    WrapPanel CompanyProductPanel = itemCard.Children[10] as WrapPanel;

            //                    ComboBox companyProductComboBox = CompanyProductPanel.Children[1] as ComboBox;

            //                    companyProductComboBox.SelectedItem = items[i].product_type.product_name;

            //                    companyProductComboBox.IsEnabled = false;


            //                    WrapPanel companyBrandPanel = itemCard.Children[11] as WrapPanel;

            //                    ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            //                    companyBrandComboBox.SelectedItem = items[i].product_brand.brand_name;

            //                    companyBrandComboBox.IsEnabled = false;


            //                    WrapPanel companyModelPanel = itemCard.Children[12] as WrapPanel;

            //                    ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            //                    companyModelComboBox.SelectedItem = items[i].product_model.model_name;
            //                    companyModelComboBox.IsEnabled = false;



            //                    priceTextBox.Text = items[i].item_price.ToString();

            //                    currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //                    quantityTextBox.Text = items[i].quantity.ToString();

                              
            //                    startSerialTextBox.IsEnabled = false;



            //                    endSerialTextBox.IsEnabled = false;



            //                    generateButton.IsEnabled = false;


            //                    startSerialTextBox.Text = items[i].product_serial_number;

            //                    WrapPanel panel1 = new WrapPanel();
            //                    Label serialNumber = new Label();
            //                    serialNumber.Style = (Style)FindResource("tableItemLabel");


            //                    serialNumber.Content = $"Serial {itemCounter}";

            //                    TextBox textSerial = new TextBox();
            //                    textSerial.IsEnabled = false;


            //                    textSerial.Text = items[i].product_serial_number;

            //                    textSerial.Style = (Style)FindResource("textBoxStyle");

            //                    serialsGrid.RowDefinitions.Add(new RowDefinition());

            //                    panel1.Children.Add(serialNumber);
            //                    panel1.Children.Add(textSerial);


            //                    Grid.SetRow(panel1, serialsGrid.RowDefinitions.Count - 1);

            //                    serialsGrid.Children.Add(panel1);

            //                }

            //            }

            //            else {


            //                itemCounter = 1;

            //                if(i!=0)
            //                addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.InitializeNewCard();

            //                m = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children.Count - 1;

            //                Grid itemCard1 = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;

            //                WrapPanel rfpCheckPanel1 = itemCard1.Children[1] as WrapPanel;

            //                CheckBox rfpCheckBox1 = rfpCheckPanel1.Children[1] as CheckBox;

            //                rfpCheckBox1.IsEnabled = false;

            //                WrapPanel quantityPanel1 = itemCard1.Children[16] as WrapPanel;

            //                TextBox quantityTextBox1 = quantityPanel1.Children[1] as TextBox;

            //                quantityTextBox1.IsEnabled = false;


            //                WrapPanel startSerialPanel1 = itemCard1.Children[14] as WrapPanel;

            //                TextBox startSerialTextBox1 = startSerialPanel1.Children[1] as TextBox;
            //                startSerialTextBox1.IsEnabled = false;



            //                WrapPanel endSerialPanel1 = itemCard1.Children[15] as WrapPanel;

            //                TextBox endSerialTextBox1 = endSerialPanel1.Children[1] as TextBox;

            //                endSerialTextBox1.IsEnabled = false;


            //                ScrollViewer scroll1 = itemCard1.Children[22] as ScrollViewer;

            //                Button generateButton1 = itemCard1.Children[21] as Button;
            //                generateButton1.IsEnabled = false;

            //                Grid serialsGrid1 = scroll1.Content as Grid;



            //                WrapPanel pricePanel1 = itemCard1.Children[18] as WrapPanel;

            //                TextBox priceTextBox1 = pricePanel1.Children[1] as TextBox;

            //                priceTextBox1.IsEnabled = false;

            //                WrapPanel currencyPanel1 = itemCard1.Children[17] as WrapPanel;

            //                ComboBox currencyComboBox1 = currencyPanel1.Children[1] as ComboBox;

            //                currencyComboBox1.IsEnabled = false;

            //                WrapPanel choicePanel1 = itemCard1.Children[4] as WrapPanel;

            //                ComboBox choiceComboBox1 = choicePanel1.Children[1] as ComboBox;

            //                choiceComboBox1.IsEnabled = false;

            //                choiceComboBox1.SelectedIndex = 1;

            //                WrapPanel CompanyCategoryPanel1 = itemCard1.Children[9] as WrapPanel;

            //                ComboBox companyCategoryComboBox1 = CompanyCategoryPanel1.Children[1] as ComboBox;

            //                companyCategoryComboBox1.SelectedItem = items[i].product_category.category_name;

            //                companyCategoryComboBox1.IsEnabled = false;


            //                WrapPanel CompanyProductPanel1 = itemCard1.Children[10] as WrapPanel;

            //                ComboBox companyProductComboBox1 = CompanyProductPanel1.Children[1] as ComboBox;

            //                companyProductComboBox1.SelectedItem = items[i].product_type.product_name;

            //                companyProductComboBox1.IsEnabled = false;


            //                WrapPanel companyBrandPanel1 = itemCard1.Children[11] as WrapPanel;

            //                ComboBox companyBrandComboBox1 = companyBrandPanel1.Children[1] as ComboBox;

            //                companyBrandComboBox1.SelectedItem = items[i].product_brand.brand_name;

            //                companyBrandComboBox1.IsEnabled = false;


            //                WrapPanel companyModelPanel1 = itemCard1.Children[12] as WrapPanel;

            //                ComboBox companyModelComboBox1 = companyModelPanel1.Children[1] as ComboBox;

            //                companyModelComboBox1.SelectedItem = items[i].product_model.model_name;
            //                companyModelComboBox1.IsEnabled = false;



            //                priceTextBox1.Text = items[i].item_price.ToString();

            //                currencyComboBox1.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //                quantityTextBox1.Text = items[i].quantity.ToString();

            //            }


            //        }

                    

            //    }


            //    else {

            //        Grid itemCard = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.Home.Children[m] as Grid;


            //        WrapPanel quantityPanel = itemCard.Children[16] as WrapPanel;

            //        TextBox quantityTextBox = quantityPanel.Children[1] as TextBox;

            //        quantityTextBox.IsEnabled = false;

            //        WrapPanel pricePanel = itemCard.Children[18] as WrapPanel;

            //        TextBox priceTextBox = pricePanel.Children[1] as TextBox;
            //        priceTextBox.IsEnabled = false;

            //        WrapPanel currencyPanel = itemCard.Children[17] as WrapPanel;

            //        ComboBox currencyComboBox = currencyPanel.Children[1] as ComboBox;

            //        currencyComboBox.IsEnabled = false;


            //        WrapPanel rfpPanel = itemCard.Children[1] as WrapPanel;

            //        CheckBox rfpCheckBox = rfpPanel.Children[1] as CheckBox;

            //        rfpCheckBox.IsChecked = true;
            //        rfpCheckBox.IsEnabled = false;



            //        WrapPanel rfpItemDescriptionPanel = itemCard.Children[3] as WrapPanel;

            //        ComboBox rfpItemDescriptionComboBox = rfpItemDescriptionPanel.Children[1] as ComboBox;

            //        rfpItemDescriptionComboBox.IsEnabled = false;


            //        WrapPanel rfpRequsterPanel = itemCard.Children[2] as WrapPanel;

            //        ComboBox requsterComboBox = rfpRequsterPanel.Children[1] as ComboBox;

            //        requsterComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRequsters().FirstOrDefault(a => a.requestor_team.team_id == items[i].rfp_info.rfpRequestorTeam).requestor_team.team_name;

            //        requsterComboBox.IsEnabled = false;

            //        ComboBox rfpSerialComboBox = rfpRequsterPanel.Children[3] as ComboBox;
            //        rfpSerialComboBox.IsEnabled = false;


            //        addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfps().Clear();

            //        List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT>rfps=addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfps();

            //        commonQueries.GetTeamRFPs(ref rfps, addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRequsters()[requsterComboBox.SelectedIndex].requestor_team.team_id);


            //        rfpSerialComboBox.Items.Clear();
            //        rfps.ForEach(a => rfpSerialComboBox.Items.Add(a.rfpID));

            //        rfpSerialComboBox.SelectedItem = rfps.FirstOrDefault(a => a.rfpSerial == items[i].rfp_info.rfpSerial && a.rfpVersion == items[i].rfp_info.rfpVersion).rfpID;



            //        List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>rfpItems=addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetRfpItems();

            //        rfpItems.Clear();

            //        commonQueries.GetRfpItemsMapping(items[i].rfp_info.rfpSerial, items[i].rfp_info.rfpVersion, items[i].rfp_info.rfpRequestorTeam, ref rfpItems);


            //        rfpItemDescriptionComboBox.Items.Clear();



            //        for (int j = 0; j < rfpItems.Count; j++) {


            //            if (rfpItems[i].product_category.category_id == 0)
            //            {

            //                rfpItemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);

            //            }
            //            else {

            //                rfpItemDescriptionComboBox.Items.Add(rfpItems[i].product_category.category_name + "," + rfpItems[i].product_type.product_name + "," + rfpItems[i].product_brand.brand_name + "," + rfpItems[i].product_model.model_name);


            //            }


            //        }


            //       PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT item=rfpItems.FirstOrDefault(a => a.rfpSerial == items[i].rfp_info.rfpSerial && a.rfpVersion == items[i].rfp_info.rfpVersion && a.rfpRequestorTeam == items[i].rfp_info.rfpRequestorTeam && a.rfp_item_number == items[i].rfp_item_number);



            //        if (rfpItems[i].product_category.category_id == 0)
            //        {

            //            rfpItemDescriptionComboBox.SelectedItem= item.product_category.category_name + "," + item.product_type.product_name + "," + item.product_brand.brand_name + "," + item.product_model.model_name;

            //        }
            //        else
            //        {

            //            rfpItemDescriptionComboBox.SelectedItem = item.product_category.category_name + "," + item.product_type.product_name + "," + item.product_brand.brand_name + "," + item.product_model.model_name;


            //        }


            //        priceTextBox.Text = items[i].item_price.ToString();

            //        currencyComboBox.SelectedItem = addEntryPermitWindow.EntryPermitPage.addEntryPermitItem.GetCurrencies().FirstOrDefault(a => a.currencyId == items[i].item_currency.currencyId).currencyName;

            //        quantityTextBox.Text = items[i].quantity.ToString();



            //    }

            //}


            addEntryPermitWindow.Show();

        }
        private void CardMouseEnter(object sender, MouseEventArgs e)
        {

            if (previousGrid != null) {
 
                Label serialPrevious = previousGrid.Children[0] as Label;
                Label transactionDatePrevious = previousGrid.Children[1] as Label;
                Label nickNamePrevious = previousGrid.Children[2] as Label;

                previousGrid.Background = Brushes.White;
                serialPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                transactionDatePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                nickNamePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }

            Grid card= sender as Grid;
            Label serial= card.Children[0] as Label;
            Label transactionDate = card.Children[1] as Label;
            Label warehouseNiceName = card.Children[2] as Label;


            BrushConverter brush = new BrushConverter();
            card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            serial.Foreground = Brushes.White;
            transactionDate.Foreground = Brushes.White;
            warehouseNiceName.Foreground = Brushes.White;

            previousGrid = card;
        }
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            MaterialEntryPermit material = null;
            AddEntryPermitWindow entryPermitWindow = new AddEntryPermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser,COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION,ref material,this);

            entryPermitWindow.Show();
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
            EntryPermitPage entryPermitPage = new EntryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser );
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
