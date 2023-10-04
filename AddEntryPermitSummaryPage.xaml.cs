using _01electronics_library;
using _01electronics_windows_library;
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
using static _01electronics_library.INVENTORY_STRUCTS;
using static _01electronics_library.PROCUREMENT_STRUCTS;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddEntryPermitSummaryPage.xaml
    /// </summary>
    public partial class AddEntryPermitSummaryPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;
        AddEntryPermitPage addEntryPermitPage;
        ExportPDF exportPDF;
        EmailFormat email;
        List<string> sendToList;
        List<string> CClist;
        object obj;
        public AddEntryPermitSummaryPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, int mViewAddCondition, ref MaterialEntryPermit oldMaterialEntryPermit,AddEntryPermitPage mAddEntryPermitPage, EntryPermitPage mpage = null)
        {
            commonQueries = mCommonQueries;
            commonFunctions= mCommonFunctions;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            InitializeComponent();
            addEntryPermitPage = mAddEntryPermitPage;
            sendToList = new List<string>();
            CClist = new List<string>();
            email = new EmailFormat();
        }
        public void InitializeSummarySheet()
        {
            
            int rowCount = 1;
            transactionDateLabel.Content = $"Transaction Date: {addEntryPermitPage.TransactionDatePicker.Text}";
            entryIDLabel.Content = $"Entry ID: {addEntryPermitPage.materialEntryPermit.GetEntryPermitId()} ";
            wareHouseLabel.Content = $"Warehouse: {addEntryPermitPage.materialEntryPermit.GetNickName()} ";
            wareHouseLocationLabel.Content = $"Location:  {addEntryPermitPage.LocationTextBox.Text} ";
            entryPermitHeader.Content = $"Entry Permit {addEntryPermitPage.entryPermitIdTextBox.Text}";
            if (itemsGrid.Children.Count > 4)
            {
                itemsGrid.RowDefinitions.RemoveRange(1, itemsGrid.RowDefinitions.Count - 1);
                itemsGrid.Children.RemoveRange(4, itemsGrid.Children.Count);

            }

            if (addEntryPermitPage.addEntryPermitItem.viewAddCondition == COMPANY_WORK_MACROS.ENTRY_PERMIT_ADD_CONDITION)
            {
                //int rfpItemCount = 1;

                for (int i = 0; i < addEntryPermitPage.addEntryPermitItem.itemsWrapPanel.Children.Count - 1; i += 2)
                {

                    Border itemBorder = addEntryPermitPage.addEntryPermitItem.itemsWrapPanel.Children[i] as Border;
                    Border generateSerialsBorder = addEntryPermitPage.addEntryPermitItem.itemsWrapPanel.Children[i + 1] as Border;

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


                    ComboBoxItem categoryItem = (ComboBoxItem)categoryComboBox.SelectedItem;
                    ComboBoxItem typeItem = (ComboBoxItem)typecomboBox.SelectedItem;
                    ComboBoxItem brandItem = (ComboBoxItem)brandComboBox.SelectedItem;
                    ComboBoxItem modelItem = (ComboBoxItem)modelComboBox.SelectedItem;

                    if (categoryItem == null)
                        return;

                    ComboBoxItem currencyItem = (ComboBoxItem)currencyComboBox.SelectedItem;
                    ComboBoxItem stockTypeItem = (ComboBoxItem)stockTypeComboBox.SelectedItem;

                    ComboBoxItem specsItem = (ComboBoxItem)specsComboBox.SelectedItem;


                    if (workFormComboBox.SelectedIndex == 0)
                    {
                        ComboBoxItem descriptionItem = (ComboBoxItem)rfpItemdescriptionComboBox.SelectedItem;
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        itemsGrid.RowDefinitions.Add(rowDefinition);

                        TextBlock itemName = new TextBlock();
                        itemName.TextWrapping = TextWrapping.Wrap;
                        itemName.Style = (Style)FindResource("tableSubItemTextblock");
                        itemName.FontSize = 15;
                        itemName.Margin = new Thickness(10);
                        itemName.Text= descriptionItem.Content as string;
                        Grid.SetRow(itemName, rowCount);
                        Grid.SetColumn(itemName, 0);
                        itemsGrid.Children.Add(itemName);

                        TextBlock neededQuantity = new TextBlock();
                        neededQuantity.TextWrapping = TextWrapping.Wrap;
                        neededQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        neededQuantity.FontSize = 15;
                        neededQuantity.Margin = new Thickness(10);
                        neededQuantity.Text = quantityTextBox.Text;
                        Grid.SetRow(neededQuantity, rowCount);
                        Grid.SetColumn(neededQuantity, 1);
                        itemsGrid.Children.Add(neededQuantity);

                        TextBlock enteredQuantity = new TextBlock();
                        enteredQuantity.TextWrapping = TextWrapping.Wrap;
                        enteredQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        enteredQuantity.FontSize = 15;
                        enteredQuantity.Margin = new Thickness(10);
                        enteredQuantity.Text = quantityTextBox.Text;

                        Grid.SetRow(enteredQuantity, rowCount);
                        Grid.SetColumn(enteredQuantity, 2);
                        itemsGrid.Children.Add(enteredQuantity);

                        StackPanel serialsPdfStackPanel = new StackPanel();


                        for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                        {


                            WrapPanel serialWrapPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                            TextBox serialNumber = serialWrapPanel.Children[1] as TextBox;
                            ComboBox serialStockCategory = serialWrapPanel.Children[2] as ComboBox;
                            DatePicker datePicker = serialWrapPanel.Children[3] as DatePicker;

                            TextBlock serialNumberPDF = new TextBlock();
                            serialNumberPDF.TextWrapping = TextWrapping.Wrap;
                            serialNumberPDF.Style = (Style)FindResource("tableSubItemTextblock");
                            serialNumberPDF.FontSize = 15;
                            serialNumberPDF.Margin = new Thickness(10);
                            serialNumberPDF.Text = serialNumber.Text;

                            serialsPdfStackPanel.Children.Add(serialNumberPDF);

                        }

                        Grid.SetRow(serialsPdfStackPanel, rowCount);
                        Grid.SetColumn(serialsPdfStackPanel, 3);
                        serialsPdfStackPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        serialsPdfStackPanel.Arrange(new Rect(0, 0, serialsPdfStackPanel.DesiredSize.Width, serialsPdfStackPanel.DesiredSize.Height));
                        if(generatedSerialsStackPanel.Children.Count!=0)
                        rowDefinition.Height = new GridLength((float)serialsPdfStackPanel.DesiredSize.Height+10);
                        itemsGrid.Children.Add(serialsPdfStackPanel);

                    }
                    else
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        itemsGrid.RowDefinitions.Add(rowDefinition);
                        TextBlock itemName = new TextBlock();
                        itemName.TextWrapping = TextWrapping.Wrap;
                        itemName.Style = (Style)FindResource("tableSubItemTextblock");
                        itemName.FontSize = 15;
                        itemName.Margin = new Thickness(10);
                        itemName.Text = $@"{categoryItem.Content} - {typeItem.Content} - {brandItem.Content} - {modelItem.Content} {specsItem.Content}";
                        Grid.SetRow(itemName, rowCount);
                        Grid.SetColumn(itemName, 0);
                        itemsGrid.Children.Add(itemName);

                        TextBlock neededQuantity = new TextBlock();
                        neededQuantity.TextWrapping = TextWrapping.Wrap;
                        neededQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        neededQuantity.FontSize = 15;
                        neededQuantity.Margin = new Thickness(10);
                        neededQuantity.Text = quantityTextBox.Text;
                        Grid.SetRow(neededQuantity, rowCount);
                        Grid.SetColumn(neededQuantity, 1);
                        itemsGrid.Children.Add(neededQuantity);

                        TextBlock enteredQuantity = new TextBlock();
                        enteredQuantity.TextWrapping = TextWrapping.Wrap;
                        enteredQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        enteredQuantity.FontSize = 15;
                        enteredQuantity.Margin = new Thickness(10);
                        enteredQuantity.Text = quantityTextBox.Text;

                        Grid.SetRow(enteredQuantity, rowCount);
                        Grid.SetColumn(enteredQuantity, 2);
                        itemsGrid.Children.Add(enteredQuantity);

                        StackPanel serialsPdfStackPanel = new StackPanel();

                        for (int j = 0; j < generatedSerialsStackPanel.Children.Count; j++)
                        {


                            WrapPanel serialWrapPanel = generatedSerialsStackPanel.Children[j] as WrapPanel;
                            TextBox serialNumber = serialWrapPanel.Children[1] as TextBox;
                            ComboBox serialStockCategory = serialWrapPanel.Children[2] as ComboBox;

                            TextBlock serialNumberPDF = new TextBlock();
                            serialNumberPDF.TextWrapping = TextWrapping.Wrap;
                            serialNumberPDF.Style = (Style)FindResource("tableSubItemTextblock");
                            serialNumberPDF.FontSize = 15;
                            serialNumberPDF.Margin = new Thickness(10);
                            serialNumberPDF.Text = serialNumber.Text;

                            serialsPdfStackPanel.Children.Add(serialNumberPDF);
                        }
                        Grid.SetRow(serialsPdfStackPanel, rowCount);
                        Grid.SetColumn(serialsPdfStackPanel, 3);

                        serialsPdfStackPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        serialsPdfStackPanel.Arrange(new Rect(0, 0, serialsPdfStackPanel.DesiredSize.Width, serialsPdfStackPanel.DesiredSize.Height));
                        if(generatedSerialsStackPanel.Children.Count!=0)
                            rowDefinition.Height = new GridLength((float)serialsPdfStackPanel.DesiredSize.Height);
                        itemsGrid.Children.Add(serialsPdfStackPanel);
                    }

                    rowCount++;

                }


               
                

            }
            else
            {
                for (int i = 0; i < addEntryPermitPage.addEntryPermitItem.itemsWrapPanel.Children.Count - 1; i++)
                {
                    Border itemBorder = addEntryPermitPage.addEntryPermitItem.itemsWrapPanel.Children[i] as Border;


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
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        itemsGrid.RowDefinitions.Add(rowDefinition);
                        TextBlock itemName = new TextBlock();
                        itemName.TextWrapping = TextWrapping.Wrap;
                        itemName.Style = (Style)FindResource("tableSubItemTextblock");
                        itemName.FontSize = 15;
                        itemName.Margin = new Thickness(10);
                        itemName.Text = descriptionItem.Content as string;
                        Grid.SetRow(itemName, rowCount);
                        Grid.SetColumn(itemName, 0);
                        itemsGrid.Children.Add(itemName);

                        TextBlock neededQuantity = new TextBlock();
                        neededQuantity.TextWrapping = TextWrapping.Wrap;
                        neededQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        neededQuantity.FontSize = 15;
                        neededQuantity.Margin = new Thickness(10);
                        neededQuantity.Text = quantityTextBox.Text;
                        Grid.SetRow(neededQuantity, rowCount);
                        Grid.SetColumn(neededQuantity, 1);
                        itemsGrid.Children.Add(neededQuantity);

                        TextBlock enteredQuantity = new TextBlock();
                        enteredQuantity.TextWrapping = TextWrapping.Wrap;
                        enteredQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        enteredQuantity.FontSize = 15;
                        enteredQuantity.Margin = new Thickness(10);
                        enteredQuantity.Text = quantityTextBox.Text;

                        Grid.SetRow(enteredQuantity, rowCount);
                        Grid.SetColumn(enteredQuantity, 2);
                        itemsGrid.Children.Add(enteredQuantity);

                        TextBlock serialNumberPDF = new TextBlock();
                        serialNumberPDF.TextWrapping = TextWrapping.Wrap;
                        serialNumberPDF.Style = (Style)FindResource("tableSubItemTextblock");
                        serialNumberPDF.FontSize = 15;
                        serialNumberPDF.Margin = new Thickness(10);
                        serialNumberPDF.Text = productSerialTextBox.Text;
                        Grid.SetRow(serialNumberPDF, rowCount);
                        Grid.SetColumn(serialNumberPDF, 3);
                        itemsGrid.Children.Add(serialNumberPDF);
                    }
                    else
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        itemsGrid.RowDefinitions.Add(rowDefinition);
                        TextBlock itemName = new TextBlock();
                        itemName.TextWrapping = TextWrapping.Wrap;
                        itemName.Style = (Style)FindResource("tableSubItemTextblock");
                        itemName.FontSize = 15;
                        itemName.Margin = new Thickness(10);
                        itemName.Text = $@"{categoryItem.Content} - {typeItem.Content} - {brandItem.Content} - {modelItem.Content} {specsItem.Content}";
                        Grid.SetRow(itemName, rowCount);
                        Grid.SetColumn(itemName, 0);
                        itemsGrid.Children.Add(itemName);

                        TextBlock neededQuantity = new TextBlock();
                        neededQuantity.TextWrapping = TextWrapping.Wrap;
                        neededQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        neededQuantity.FontSize = 15;
                        neededQuantity.Margin = new Thickness(10);
                        neededQuantity.Text = quantityTextBox.Text;
                        Grid.SetRow(neededQuantity, rowCount);
                        Grid.SetColumn(neededQuantity, 1);
                        itemsGrid.Children.Add(neededQuantity);

                        TextBlock enteredQuantity = new TextBlock();
                        enteredQuantity.TextWrapping = TextWrapping.Wrap;
                        enteredQuantity.Style = (Style)FindResource("tableSubItemTextblock");
                        enteredQuantity.FontSize = 15;
                        enteredQuantity.Margin = new Thickness(10);
                        enteredQuantity.Text = quantityTextBox.Text;

                        Grid.SetRow(enteredQuantity, rowCount);
                        Grid.SetColumn(enteredQuantity, 2);
                        itemsGrid.Children.Add(enteredQuantity);

                        TextBlock serialNumberPDF = new TextBlock();
                        serialNumberPDF.TextWrapping = TextWrapping.Wrap;
                        serialNumberPDF.Style = (Style)FindResource("tableSubItemTextblock");
                        serialNumberPDF.FontSize = 15;
                        serialNumberPDF.Margin = new Thickness(10);
                        serialNumberPDF.Text = productSerialTextBox.Text;
                        Grid.SetRow(serialNumberPDF, rowCount);
                        Grid.SetColumn(serialNumberPDF, 3);
                        itemsGrid.Children.Add(serialNumberPDF);
                    }
                    rowCount++;
                }
            }

        }
        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(addEntryPermitPage.addEntryPermitItem);
        }

        private void OnNextButtonOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {
            addEntryPermitPage.addEntryPermitItem.OnButtonClickFinish(sender,e);
            pdfGrid.Children.Remove(printer);
            sheetBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            sheetBorder.Arrange(new Rect(0, 0, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height));
            exportPDF = new ExportPDF((float)sheetBorder.DesiredSize.Width, (float)sheetBorder.DesiredSize.Height);
            string pdfPath = exportPDF.CreatePDF($"EntryPermit-{addEntryPermitPage.entryPermitIdTextBox.Text}", pdfGrid, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height);
            Grid.SetRow(printer, 0);
            Grid.SetColumn(printer, 1);
            printer.HorizontalAlignment = HorizontalAlignment.Right;
            pdfGrid.Children.Add(printer);

            sendToList.Add("manar.shaaban@01electronics.net");
            CClist.Add("ahmed_ayman@01electronics.net");
            obj = (object)addEntryPermitPage.materialEntryPermit;

            email.SendEmail(ref sendToList, ref CClist, $"[New]Entry Permit - {addEntryPermitPage.entryPermitIdTextBox.Text}", "NEW ENTRY PERMIT ADDED", ref obj, BASIC_MACROS.IS_ENTRY_PERMIT, pdfPath);


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
            string pdfPath = exportPDF.CreatePDF($"EntryPermit-{addEntryPermitPage.entryPermitIdTextBox.Text}", pdfGrid, sheetBorder.DesiredSize.Width, sheetBorder.DesiredSize.Height);
            Grid.SetRow(printer, 0);
            Grid.SetColumn(printer, 1);
            printer.HorizontalAlignment = HorizontalAlignment.Right;
            pdfGrid.Children.Add(printer);
        }
    }
}
