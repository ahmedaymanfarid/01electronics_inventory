using _01electronics_library;
using System;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using _01electronics_windows_library;



namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddReleasePermitSummary.xaml
    /// </summary>
    public partial class AddReleasePermitSummary : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;
        AddReleasePermitWindow parentWindow;
        ExportPDF exportPDF;
        EmailFormat email;
        List<string> serials;
        List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems;
        List<string> sendToList;
        List<string> CClist;
        object obj;
        
        public AddReleasePermitSummary(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow mParentWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            parentWindow = mParentWindow;
            serials = new List<string>();
            rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
            email = new EmailFormat();
            sendToList = new List<string>();
            CClist = new List<string>();
            obj = new object();
            parentWindow.materialReleasePermit.SetLoggedInUser(loggedInUser);
            InitializeComponent();
            InitializeSummarySheet();
        }
        public void InitializeSummarySheet()
        {
            if(parentWindow.viewAddCondition==COMPANY_WORK_MACROS.VIEW_RELEASE || parentWindow.viewAddCondition==COMPANY_WORK_MACROS.EDIT_RELEASE)
            {
                finishButton.IsEnabled = false;
                int rowCount = 1;
                releaseDateLabel.Content = "Release Date: ";
                releaseIdLabel.Content = "Release ID: ";
                recieverNameLabel.Content = "Reciever Name: ";
                workFormLabel.Content = "Work Form: ";
                workOrderSerialLabel.Content = "Work Order Serial: ";
                workOrderContactLabel.Content = "Work Order Contact: ";
                rfpRequestorTeamLabel.Content = "RFP Requestor Team: ";
                rfpIDLabel.Content = "RFP ID: ";
                releasePermitHeader.Content = $"Release Permit {parentWindow.materialReleasePermit.GetReleaseId()}";

                if (itemsGrid.Children.Count > 4)
                {
                    itemsGrid.RowDefinitions.RemoveRange(1, itemsGrid.RowDefinitions.Count - 1);
                    itemsGrid.Children.RemoveRange(4, itemsGrid.Children.Count);

                }

                releaseDateLabel.Content += parentWindow.materialReleasePermit.GetReleaseDate().ToString();
                releaseIdLabel.Content += parentWindow.materialReleasePermit.GetReleaseId();
                recieverNameLabel.Content += parentWindow.materialReleasePermit.GetMaterialReceiver().employee_name;
                if (parentWindow.releasePermitPage.workFormComboBox.SelectedIndex == 0)
                {
                    workOrderSerialLabel.Visibility = Visibility.Hidden;
                    workOrderContactLabel.Visibility = Visibility.Hidden;
                    workFormLabel.Content += "RFP";
                    rfpRequestorTeamLabel.Visibility = Visibility.Visible;
                    rfpRequestorTeamLabel.Content += parentWindow.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfp_requestor_team_name;
                    rfpIDLabel.Visibility = Visibility.Visible;
                    rfpIDLabel.Content += parentWindow.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpID;
                }
                else
                {
                    rfpRequestorTeamLabel.Visibility = Visibility.Hidden;
                    rfpIDLabel.Visibility = Visibility.Hidden;
                    workFormLabel.Content += "Work Order";
                    workOrderSerialLabel.Visibility = Visibility.Visible;
                    workOrderSerialLabel.Content += parentWindow.materialReleasePermit.GetWorkOrder().GetOrderID();
                    workOrderContactLabel.Visibility = Visibility.Visible;
                    workOrderContactLabel.Content += parentWindow.materialReleasePermit.GetWorkOrder().GetContactName();
                }
                if (parentWindow.materialReleasePermit.GetReleaseItems().Count != 0)
                {
                   
                        for(int i = 0;i< parentWindow.materialReleasePermit.GetReleaseItems().Count;i++)
                        {
                            //CheckBox selectedEntryPermit = selectedEntries;
                            //WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;
                            //INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                            //PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItem = (PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)checkBoxContentWrapPanel.Tag;

                            RowDefinition rowDefinition = new RowDefinition();
                            rowDefinition.Height = new GridLength(100);
                            itemsGrid.RowDefinitions.Add(rowDefinition);
                            TextBlock itemName = new TextBlock();
                            itemName.TextWrapping = TextWrapping.Wrap;
                            itemName.Style = (Style)FindResource("tableSubItemTextblock");
                            itemName.FontSize = 15;
                            itemName.Margin = new Thickness(10);
                            if (parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanySpecs.spec_id!=0)
                            {
                                itemName.Text = $@"{parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemcompanyCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialitemCompanyproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanyBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanyModel.model_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanySpecs.spec_name}";
                            }
                            else 
                            {
                                itemName.Text = $@"{parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemGenericCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialitemGenericproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemGenericBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemGenericModel.model_name}";
                            }
                            Grid.SetColumn(itemName, 0);
                            Grid.SetRow(itemName, rowCount);
                            itemsGrid.Children.Add(itemName);

                            TextBlock requestedQuantity = new TextBlock();
                            requestedQuantity.TextWrapping = TextWrapping.Wrap;
                            requestedQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                            requestedQuantity.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].entered_quantity.ToString();

                            Grid.SetColumn(requestedQuantity, 1);
                            Grid.SetRow(requestedQuantity, rowCount);
                            itemsGrid.Children.Add(requestedQuantity);

                            if (parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number=="")
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].released_quantity_release.ToString();

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = "None";

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }
                            else
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = "1";

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number ;

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }

                            rowCount++;

                        }

                }




            }
            else
            {
                int rowCount = 1;
                releaseDateLabel.Content = "Release Date: ";
                releaseIdLabel.Content = "Release ID: ";
                recieverNameLabel.Content = "Reciever Name: ";
                workFormLabel.Content = "Work Form: ";
                workOrderSerialLabel.Content = "Work Order Serial: ";
                workOrderContactLabel.Content = "Work Order Contact: ";
                rfpRequestorTeamLabel.Content = "RFP Requestor Team: ";
                rfpIDLabel.Content = "RFP ID: ";
                releasePermitHeader.Content = $"Release Permit {parentWindow.materialReleasePermit.GetReleaseId()}";

                if (itemsGrid.Children.Count > 4)
                {
                    itemsGrid.RowDefinitions.RemoveRange(1, itemsGrid.RowDefinitions.Count - 1);
                    itemsGrid.Children.RemoveRange(4, itemsGrid.Children.Count);

                }

                releaseDateLabel.Content += parentWindow.materialReleasePermit.GetReleaseDate().ToString();
                releaseIdLabel.Content += parentWindow.materialReleasePermit.GetReleaseId();
                recieverNameLabel.Content += parentWindow.materialReleasePermit.GetMaterialReceiver().employee_name;
                if (parentWindow.releasePermitPage.workFormComboBox.SelectedIndex == 0)
                {
                    workOrderSerialLabel.Visibility = Visibility.Hidden;
                    workOrderContactLabel.Visibility = Visibility.Hidden;
                    workFormLabel.Content += "RFP";
                    rfpRequestorTeamLabel.Visibility = Visibility.Visible;
                    rfpRequestorTeamLabel.Content += parentWindow.materialReleasePermit.GetRfp().GetRFPRequestorTeam();
                    rfpIDLabel.Visibility = Visibility.Visible;
                    rfpIDLabel.Content += parentWindow.materialReleasePermit.GetRfp().GetRFPId();
                }
                else
                {
                    rfpRequestorTeamLabel.Visibility = Visibility.Hidden;
                    rfpIDLabel.Visibility = Visibility.Hidden;
                    workFormLabel.Content += "Work Order";
                    workOrderSerialLabel.Visibility = Visibility.Visible;
                    workOrderSerialLabel.Content += parentWindow.materialReleasePermit.GetWorkOrder().GetOrderID();
                    workOrderContactLabel.Visibility = Visibility.Visible;
                    workOrderContactLabel.Content += parentWindow.materialReleasePermit.GetWorkOrder().GetContactName();
                }
                if (parentWindow.releasePermitItemPage.selectedItems.Count != 0)
                {
                    if (parentWindow.releasePermitItemPage.isRFP)
                    {
                        foreach (CheckBox selectedEntries in parentWindow.releasePermitItemPage.selectedItems)
                        {
                            CheckBox selectedEntryPermit = selectedEntries;
                            WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;

                            INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                            PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItem = (PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)checkBoxContentWrapPanel.Tag;

                            RowDefinition rowDefinition = new RowDefinition();
                            rowDefinition.Height = new GridLength(100);
                            itemsGrid.RowDefinitions.Add(rowDefinition);
                            TextBlock itemName = new TextBlock();
                            itemName.TextWrapping = TextWrapping.Wrap;
                            itemName.Style = (Style)FindResource("tableSubItemTextblock");
                            itemName.FontSize = 15;
                            itemName.Margin = new Thickness(10);
                            if (rfpItem.is_company_product)
                            {
                                itemName.Text = $@"{rfpItem.product_category.category_name} - {rfpItem.product_type.product_name} - {rfpItem.product_brand.brand_name} - {rfpItem.product_model.model_name} - {rfpItem.product_specs.spec_name}";
                            }
                            else
                            {
                                itemName.Text = $@"{rfpItem.product_category.category_name} - {rfpItem.product_type.product_name} - {rfpItem.product_brand.brand_name} - {rfpItem.product_model.model_name} ";
                            }
                            Grid.SetColumn(itemName, 0);
                            Grid.SetRow(itemName, rowCount);
                            itemsGrid.Children.Add(itemName);

                            TextBlock requestedQuantity = new TextBlock();
                            requestedQuantity.TextWrapping = TextWrapping.Wrap;
                            requestedQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                            requestedQuantity.Text = rfpItem.item_quantity.ToString();

                            Grid.SetColumn(requestedQuantity, 1);
                            Grid.SetRow(requestedQuantity, rowCount);
                            itemsGrid.Children.Add(requestedQuantity);

                            if (checkBoxContentWrapPanel.Children.Count > 2)
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = ((TextBox)checkBoxContentWrapPanel.Children[6]).Text;

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = "None";

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }
                            else
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = "1";

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = entryPermitItem.product_serial_number;

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }

                            rowCount++;







                        }
                    }
                    else
                    {
                        foreach (CheckBox selectedEntries in parentWindow.releasePermitItemPage.selectedItems)
                        {
                            CheckBox selectedEntryPermit = selectedEntries;
                            WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;

                            INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderItem = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)checkBoxContentWrapPanel.Tag;

                            RowDefinition rowDefinition = new RowDefinition();
                            rowDefinition.Height = new GridLength(100);
                            itemsGrid.RowDefinitions.Add(rowDefinition);
                            TextBlock itemName = new TextBlock();
                            itemName.TextWrapping = TextWrapping.Wrap;
                            itemName.Style = (Style)FindResource("tableSubItemTextblock");
                            itemName.FontSize = 15;
                            itemName.Margin = new Thickness(10);
                            itemName.Text = $@"{orderItem.product_category.category_name} - {orderItem.productType.product_name} - {orderItem.productBrand.brand_name} - {orderItem.productModel.model_name} - {orderItem.productSpec.spec_name}";



                            Grid.SetColumn(itemName, 0);
                            Grid.SetRow(itemName, rowCount);
                            itemsGrid.Children.Add(itemName);

                            TextBlock requestedQuantity = new TextBlock();
                            requestedQuantity.TextWrapping = TextWrapping.Wrap;
                            requestedQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                            requestedQuantity.Text = orderItem.productQuantity.ToString();

                            Grid.SetColumn(requestedQuantity, 1);
                            Grid.SetRow(requestedQuantity, rowCount);
                            itemsGrid.Children.Add(requestedQuantity);

                            if (checkBoxContentWrapPanel.Children.Count > 2)
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = ((TextBox)checkBoxContentWrapPanel.Children[6]).Text;

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = "None";

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }
                            else
                            {
                                TextBlock releasedQuanity = new TextBlock();
                                releasedQuanity.TextWrapping = TextWrapping.Wrap;
                                releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                                releasedQuanity.Text = "1";

                                Grid.SetColumn(releasedQuanity, 2);
                                Grid.SetRow(releasedQuanity, rowCount);
                                itemsGrid.Children.Add(releasedQuanity);

                                TextBlock serialNumber = new TextBlock();
                                serialNumber.TextWrapping = TextWrapping.Wrap;
                                serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                                serialNumber.Text = entryPermitItem.product_serial_number;

                                Grid.SetColumn(serialNumber, 3);
                                Grid.SetRow(serialNumber, rowCount);
                                itemsGrid.Children.Add(serialNumber);
                            }

                            rowCount++;


                        }
                    }

                }
            }
           
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(parentWindow.releasePermitItemPage);
        }

        private void OnNextButtonOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {
            if(parentWindow.releasePermitItemPage.selectedItems.Count==0)
            {
                System.Windows.Forms.MessageBox.Show("Please select items.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            if(parentWindow.releasePermitPage.MaterialRecieverComboBox.SelectedIndex==-1)
            {
                System.Windows.Forms.MessageBox.Show("Reciever is not selected.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            if(parentWindow.releasePermitPage.ReleaseDatePicker.SelectedDate==null)
            {
                System.Windows.Forms.MessageBox.Show("Release date is not specified ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            if(parentWindow.releasePermitPage.workFormComboBox.SelectedIndex==-1)
            {
                System.Windows.Forms.MessageBox.Show("Work from is not selected", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            else
            {
                if(parentWindow.releasePermitPage.workFormComboBox.SelectedIndex==0)
                {
                    if(parentWindow.releasePermitPage.rfpRequesters.SelectedIndex==-1)
                    {
                        System.Windows.Forms.MessageBox.Show("RFP requestor team is not specified ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if(parentWindow.releasePermitPage.rfpSerials.SelectedIndex==-1)
                    {
                        System.Windows.Forms.MessageBox.Show("RFP ID is not selected.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (parentWindow.releasePermitPage.contactComboBox.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Work order contact is not specified  ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if (parentWindow.releasePermitPage.orderSerials.SelectedIndex == -1)
                    {
                        System.Windows.Forms.MessageBox.Show("Work order is not selected.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    if ((parentWindow.releasePermitPage.receivalNoteCheckBox.IsChecked==false) && (parentWindow.releasePermitPage.serviceReportCheckBox.IsChecked==false))
                    {
                        System.Windows.Forms.MessageBox.Show("Please choose the closing method (service report / recieval note) ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                
                }
            }

            if (parentWindow.releasePermitItemPage.isRFP)
            {
                int itemSerial = 1;
            
                if (parentWindow.releasePermitItemPage.selectedItems.Count==0)
                {
                    System.Windows.Forms.MessageBox.Show("Please select items first.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                foreach (CheckBox selectedEntries in parentWindow.releasePermitItemPage.selectedItems)
                {
                    CheckBox selectedEntryPermit = selectedEntries;
                    WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;

                    INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                    PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItem = (PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)checkBoxContentWrapPanel.Tag;
                    rfpItems.Add(rfpItem);
                    serials.Add(entryPermitItem.product_serial_number);
                    INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM releasePermitItem = new INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM();
                    releasePermitItem.release_permit_item_serial = itemSerial;
                    releasePermitItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT;
                    if(checkBoxContentWrapPanel.Children.Count>2)
                    {
                        TextBox releasedTextBox = checkBoxContentWrapPanel.Children[6] as TextBox;
                        if(releasedTextBox.Text=="")
                        {
                            System.Windows.Forms.MessageBox.Show("To be released quantity is not specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                        releasePermitItem.released_quantity_release=int.Parse(releasedTextBox.Text);
                        releasePermitItem.entry_permit_serial = entryPermitItem.entry_permit_serial;
                        releasePermitItem.entry_permit_item_serial = entryPermitItem.entry_permit_item_serial;
                        releasePermitItem.rfp_info.rfpSerial = parentWindow.materialReleasePermit.GetRfp().GetRFPSerial();
                        releasePermitItem.rfp_info.rfpRequestorTeam = parentWindow.materialReleasePermit.GetRfp().GetRFPRequestorTeamId();
                        releasePermitItem.rfp_info.rfpVersion = parentWindow.materialReleasePermit.GetRfp().GetRFPVersion();
                        releasePermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        if(entryPermitItem.is_company_product|| entryPermitItem.product_category.category_id!=0)
                        {
                            releasePermitItem.materialItemcompanyCategory.category_id=entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemCompanyproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemCompanyBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemCompanyModel.model_id = entryPermitItem.product_model.model_id;
                            releasePermitItem.materialItemCompanySpecs.spec_id = entryPermitItem.product_specs.spec_id;

                            releasePermitItem.materialItemcompanyCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemCompanyproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemCompanyBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemCompanyModel.model_name = entryPermitItem.product_model.model_name;
                            releasePermitItem.materialItemCompanySpecs.spec_name = entryPermitItem.product_specs.spec_name;
                        }
                        else
                        {
                            releasePermitItem.materialItemGenericCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemGenericproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemGenericBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemGenericModel.model_id = entryPermitItem.product_model.model_id;
                          

                            releasePermitItem.materialItemGenericCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemGenericproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemGenericBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemGenericModel.model_name = entryPermitItem.product_model.model_name;
                       
                        }

                        releasePermitItem.entered_quantity = Convert.ToInt32(rfpItem.item_quantity);
                    }
                    else
                    {
                        releasePermitItem.released_quantity_release = 1;
                        releasePermitItem.entry_permit_serial = entryPermitItem.entry_permit_serial;
                        releasePermitItem.entry_permit_item_serial = entryPermitItem.entry_permit_item_serial;
                        releasePermitItem.rfp_info.rfpSerial = parentWindow.materialReleasePermit.GetRfp().GetRFPSerial();
                        releasePermitItem.rfp_info.rfpRequestorTeam = parentWindow.materialReleasePermit.GetRfp().GetRFPRequestorTeamId();
                        releasePermitItem.rfp_info.rfpVersion = parentWindow.materialReleasePermit.GetRfp().GetRFPVersion();
                        releasePermitItem.rfp_item_number = rfpItem.rfp_item_number;
                        releasePermitItem.entryPermit_product_serial_number = entryPermitItem.product_serial_number;

                        if (entryPermitItem.is_company_product || entryPermitItem.product_category.category_id != 0)
                        {
                            releasePermitItem.materialItemcompanyCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemCompanyproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemCompanyBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemCompanyModel.model_id = entryPermitItem.product_model.model_id;
                            releasePermitItem.materialItemCompanySpecs.spec_id = entryPermitItem.product_specs.spec_id;

                            releasePermitItem.materialItemcompanyCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemCompanyproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemCompanyBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemCompanyModel.model_name = entryPermitItem.product_model.model_name;
                            releasePermitItem.materialItemCompanySpecs.spec_name = entryPermitItem.product_specs.spec_name;
                        }
                        else
                        {
                            releasePermitItem.materialItemGenericCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemGenericproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemGenericBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemGenericModel.model_id = entryPermitItem.product_model.model_id;


                            releasePermitItem.materialItemGenericCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemGenericproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemGenericBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemGenericModel.model_name = entryPermitItem.product_model.model_name;

                        }
                        releasePermitItem.entered_quantity = Convert.ToInt32(rfpItem.item_quantity);
                    }
                    itemSerial++;
                    releasePermitItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                    parentWindow.materialReleasePermit.AddReleaseItem(releasePermitItem);
                }
                parentWindow.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL);
                if (!parentWindow.materialReleasePermit.IssueNewMaterialRelease(ref serials, ref rfpItems,0, 0))
                    return;


            }
            else
            {
                rfpItems.Clear();
                int itemSerial = 1;
              
                if (parentWindow.releasePermitItemPage.selectedItems.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please select items first.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                CheckBox orderHolder = parentWindow.releasePermitItemPage.orderItemHolder;
                PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderProduct = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)orderHolder.Tag;
                foreach (CheckBox selectedEntries in parentWindow.releasePermitItemPage.selectedItems)
                {
                    CheckBox selectedEntryPermit = selectedEntries;
                    WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;

                    INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderItem = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)checkBoxContentWrapPanel.Tag;
                    serials.Add(entryPermitItem.product_serial_number);
                    INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM releasePermitItem = new INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM();
                    releasePermitItem.release_permit_item_serial = itemSerial;
                    releasePermitItem.release_permit_item_status = COMPANY_WORK_MACROS.RECEIVAL_NOTE_RECEIVED;
                    if (checkBoxContentWrapPanel.Children.Count > 2)
                    {
                        TextBox releasedTextBox = checkBoxContentWrapPanel.Children[6] as TextBox;
                        if (releasedTextBox.Text == "")
                        {
                            System.Windows.Forms.MessageBox.Show("To be released quantity is not specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                        releasePermitItem.released_quantity_release = int.Parse(releasedTextBox.Text);
                        releasePermitItem.entry_permit_serial = entryPermitItem.entry_permit_serial;
                        releasePermitItem.entry_permit_item_serial = entryPermitItem.entry_permit_item_serial;
                        releasePermitItem.order_serial = parentWindow.materialReleasePermit.GetWorkOrder().orderSerial;
                        releasePermitItem.workOrder_product_number = orderItem.productNumber;
                        releasePermitItem.workOrder_serial = parentWindow.materialReleasePermit.GetWorkOrder().orderSerial;

                        if (entryPermitItem.is_company_product || entryPermitItem.product_category.category_id != 0)
                        {
                            releasePermitItem.materialItemcompanyCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemCompanyproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemCompanyBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemCompanyModel.model_id = entryPermitItem.product_model.model_id;
                            releasePermitItem.materialItemCompanySpecs.spec_id = entryPermitItem.product_specs.spec_id;

                            releasePermitItem.materialItemcompanyCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemCompanyproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemCompanyBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemCompanyModel.model_name = entryPermitItem.product_model.model_name;
                            releasePermitItem.materialItemCompanySpecs.spec_name = entryPermitItem.product_specs.spec_name;
                        }
                        else
                        {
                            releasePermitItem.materialItemGenericCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemGenericproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemGenericBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemGenericModel.model_id = entryPermitItem.product_model.model_id;


                            releasePermitItem.materialItemGenericCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemGenericproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemGenericBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemGenericModel.model_name = entryPermitItem.product_model.model_name;

                        }
                        releasePermitItem.entered_quantity = orderItem.productQuantity;
                    }
                    else
                    {
                        releasePermitItem.released_quantity_release =1 ;
                        releasePermitItem.entry_permit_serial = entryPermitItem.entry_permit_serial;
                        releasePermitItem.entry_permit_item_serial = entryPermitItem.entry_permit_item_serial;
                        releasePermitItem.order_serial = parentWindow.materialReleasePermit.GetWorkOrder().orderSerial;
                        releasePermitItem.workOrder_product_number = orderItem.productNumber;
                        releasePermitItem.entryPermit_product_serial_number = entryPermitItem.product_serial_number;
                        releasePermitItem.workOrder_serial = parentWindow.materialReleasePermit.GetWorkOrder().orderSerial;

                        if (entryPermitItem.is_company_product || entryPermitItem.product_category.category_id != 0)
                        {
                            releasePermitItem.materialItemcompanyCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemCompanyproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemCompanyBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemCompanyModel.model_id = entryPermitItem.product_model.model_id;
                            releasePermitItem.materialItemCompanySpecs.spec_id = entryPermitItem.product_specs.spec_id;

                            releasePermitItem.materialItemcompanyCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemCompanyproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemCompanyBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemCompanyModel.model_name = entryPermitItem.product_model.model_name;
                            releasePermitItem.materialItemCompanySpecs.spec_name = entryPermitItem.product_specs.spec_name;
                        }
                        else
                        {
                            releasePermitItem.materialItemGenericCategory.category_id = entryPermitItem.product_category.category_id;
                            releasePermitItem.materialitemGenericproduct.type_id = entryPermitItem.product_type.type_id;
                            releasePermitItem.materialItemGenericBrand.brand_id = entryPermitItem.product_brand.brand_id;
                            releasePermitItem.materialItemGenericModel.model_id = entryPermitItem.product_model.model_id;


                            releasePermitItem.materialItemGenericCategory.category_name = entryPermitItem.product_category.category_name;
                            releasePermitItem.materialitemGenericproduct.product_name = entryPermitItem.product_type.product_name;
                            releasePermitItem.materialItemGenericBrand.brand_name = entryPermitItem.product_brand.brand_name;
                            releasePermitItem.materialItemGenericModel.model_name = entryPermitItem.product_model.model_name;

                        }
                        releasePermitItem.entered_quantity = orderItem.productQuantity;
                    }
                    itemSerial++;
                    releasePermitItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                    parentWindow.materialReleasePermit.AddReleaseItem(releasePermitItem);
                }
                parentWindow.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT);
                if (!parentWindow.materialReleasePermit.IssueNewMaterialRelease(ref serials,ref rfpItems, parentWindow.materialReleasePermit.GetWorkOrder().orderSerial,orderProduct.productNumber))
                    return;

                
            }
            pdfGrid.Children.Remove(printer);
            sheetBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            sheetBorder.Arrange(new Rect(0, 0, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height));
            exportPDF = new ExportPDF((float)sheetBorder.DesiredSize.Width, (float)sheetBorder.DesiredSize.Height);
            string pdfPath = exportPDF.CreatePDF($"ReleasePermit-{parentWindow.releasePermitPage.SerialIdTextBox.Text}", pdfGrid, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height);
            Grid.SetRow(printer, 0);
            Grid.SetColumn(printer, 1);
            printer.HorizontalAlignment = HorizontalAlignment.Right;
            pdfGrid.Children.Add(printer);

            sendToList.Add("manar.shaaban@01electronics.net");
            CClist.Add("ahmed_ayman@01electronics.net");
            obj = (object)parentWindow.materialReleasePermit;

            email.SendEmail(ref sendToList, ref CClist, $"[New]Release Permit - {parentWindow.materialReleasePermit.GetReleaseId()}", "NEW RELEASE PERMIT ADDED", ref obj, BASIC_MACROS.IS_RELEASE_PERMIT, pdfPath);

        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void BasicInfoLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnMouseDownPrint(object sender, MouseButtonEventArgs e)
        {
            pdfGrid.Children.Remove(printer);
            sheetBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            sheetBorder.Arrange(new Rect(0, 0, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height));
            exportPDF = new ExportPDF((float)sheetBorder.DesiredSize.Width, (float)sheetBorder.DesiredSize.Height);
            string pdfPath=exportPDF.CreatePDF($"ReleasePermit-{parentWindow.releasePermitPage.SerialIdTextBox.Text}", pdfGrid, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height);
            Grid.SetRow(printer, 0);
            Grid.SetColumn(printer, 1);
            printer.HorizontalAlignment = HorizontalAlignment.Right;
            pdfGrid.Children.Add(printer);

        }
    }
}
