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
        
        public AddReleasePermitSummary(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow mParentWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            parentWindow = mParentWindow;
            
            InitializeComponent();
        }
        public void InitializeSummarySheet()
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

            if(itemsGrid.Children.Count>4)
            {
                itemsGrid.RowDefinitions.RemoveRange(1, itemsGrid.RowDefinitions.Count-1);
                itemsGrid.Children.RemoveRange(4, itemsGrid.Children.Count);
               
            }
          
            releaseDateLabel.Content+=parentWindow.materialReleasePermit.GetReleaseDate().ToString();
            releaseIdLabel.Content += parentWindow.materialReleasePermit.GetReleaseId();
            recieverNameLabel.Content += parentWindow.materialReleasePermit.GetMaterialReceiver().employee_name;
            if(parentWindow.releasePermitPage.workFormComboBox.SelectedIndex == 0 )
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
           if(parentWindow.releasePermitItemPage.selectedItems.Count!=0)
           {
                if(parentWindow.releasePermitItemPage.isRFP)
                {
                    foreach (CheckBox selectedEntries in parentWindow.releasePermitItemPage.selectedItems)
                    {
                        CheckBox selectedEntryPermit = selectedEntries;
                        WrapPanel checkBoxContentWrapPanel = (WrapPanel)selectedEntryPermit.Content;

                        INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT entryPermitItem = (INVENTORY_STRUCTS.ENTRY_PERMIT_ITEM_MAX_STRUCT)selectedEntryPermit.Tag;
                        PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = (PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT)checkBoxContentWrapPanel.Tag;

                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height =new GridLength(100);
                        itemsGrid.RowDefinitions.Add(rowDefinition);
                        TextBlock itemName = new TextBlock();
                        itemName.TextWrapping = TextWrapping.Wrap;
                        itemName.Style = (Style)FindResource("tableSubItemTextblock");
                        itemName.FontSize = 15;
                        itemName.Margin = new Thickness(10);
                        if(rfpItem.is_company_product)
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
                        requestedQuantity.Text=rfpItem.item_quantity.ToString();

                        Grid.SetColumn(requestedQuantity, 1);
                        Grid.SetRow(requestedQuantity, rowCount);
                        itemsGrid.Children.Add(requestedQuantity);

                        if (checkBoxContentWrapPanel.Children.Count>2)
                        {

                        }
                        else
                        {
                            TextBlock releasedQuanity = new TextBlock();
                            releasedQuanity.TextWrapping = TextWrapping.Wrap;
                            releasedQuanity.Style = (Style)FindResource("tableSubItemTextblock");
                            releasedQuanity.Text ="1";

                            Grid.SetColumn(releasedQuanity, 2);
                            Grid.SetRow(releasedQuanity, rowCount);
                            itemsGrid.Children.Add(releasedQuanity);

                            TextBlock serialNumber = new TextBlock();
                            serialNumber.TextWrapping = TextWrapping.Wrap;
                            serialNumber.Style = (Style)FindResource("tableSubItemTextblock");
                            serialNumber.Text =entryPermitItem.product_serial_number;

                            Grid.SetColumn(serialNumber, 3);
                            Grid.SetRow(serialNumber, rowCount);
                            itemsGrid.Children.Add(serialNumber);
                        }

                        rowCount++;





                       

                    }
                }
                else
                {

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
            exportPDF.CreatePDF($"ReleasePermit - {parentWindow.materialReleasePermit.GetReleaseId()}", pdfGrid, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height);
            Grid.SetRow(printer, 0);
            Grid.SetColumn(printer, 1);
            printer.HorizontalAlignment = HorizontalAlignment.Right;
            pdfGrid.Children.Add(printer);

        }
    }
}
