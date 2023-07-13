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

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddRecievalNoteItemPage.xaml
    /// </summary>
    public partial class AddRecievalNoteItemPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public RecievalNoteWindow parentWindow;
        AddRecievalNoteUploadFilesPage recievalNoteUploadFilesPage;

        bool firstTimeFinishClickedCheck = true;

        MaterialRecievalNote recievalNote;

        List<INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT> releasePermits;

        public AddRecievalNoteItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, RecievalNoteWindow mWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            releasePermits = new List<INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT>();

            InitializeComponent();

            if (!commonQueries.GetReleasePermits(ref releasePermits))
                return;

            parentWindow = mWindow;

            releasePermits.ForEach(a => releasePermitsComboBox.Items.Add(a.release_Permit_Id));

            recievalDate.SelectedDate = DateTime.Now;
        }



        private void OnNextButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnRecievalNoteFilesClick(object sender, MouseButtonEventArgs e)
        {
            if (parentWindow.isView)
            {
                this.NavigationService.Navigate(recievalNoteUploadFilesPage);
            }

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {


            if (releasePermitsComboBox.SelectedIndex == -1)
            {

                System.Windows.Forms.MessageBox.Show("release Permit is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                return;
            }

            int counter = 0;

            recievalNote.GetRecievalNoteItems().Clear();

            if (firstTimeFinishClickedCheck == true)
                ReleasePermitItemsGrid.Children.RemoveAt(0);

            firstTimeFinishClickedCheck = false;

            for (int i = 0; i < ReleasePermitItemsGrid.Children.Count; i++)
            {

                recievalNote.SetAddedBy(loggedInUser.GetEmployeeId());

                INVENTORY_STRUCTS.MATERIAL_RECIEVAL_NOTE_ITEM recievalNoteItem = new INVENTORY_STRUCTS.MATERIAL_RECIEVAL_NOTE_ITEM();

                Grid item = ReleasePermitItemsGrid.Children[i] as Grid;

                bool isSerial = true;


                CheckBox productSerialCheckBox1 = item.Children[2] as CheckBox;
                


                if (productSerialCheckBox1 == null)
                    isSerial = false;

                if (isSerial == true)
                {

                    CheckBox productSerialCheckBox = item.Children[2] as CheckBox;

                    if (productSerialCheckBox.IsChecked == true)
                    {
                        counter++;

                        recievalNoteItem.recieval_note_item_serial = counter;

                        recievalNoteItem.release_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        recievalNoteItem.release_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);
                        if(Convert.ToInt32(productSerialCheckBox.Tag.ToString().Split('-')[3])==BASIC_MACROS.IS_WORK_ORDER)
                        {
                            recievalNoteItem.work_order_serial = Convert.ToInt32(productSerialCheckBox.Tag.ToString().Split('-')[0]);
                            recievalNoteItem.product_number = Convert.ToInt32(productSerialCheckBox.Tag.ToString().Split('-')[1]);
                            recievalNoteItem.work_order_serial_number_id = Convert.ToInt32(productSerialCheckBox.Tag.ToString().Split('-')[2]);
                        }
                      
                        

                        recievalNote.AddRecievalNoteItem(recievalNoteItem);
                    }

                }



                else
                {

                    Grid quantityGrid = item.Children[2] as Grid;
                    CheckBox quantityCheckBox = quantityGrid.Children[0] as CheckBox;
                    TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;


                    if (quantityCheckBox.IsChecked == true)
                    {

                        recievalNoteItem.recieval_note_item_serial = counter;

                        recievalNoteItem.release_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        recievalNoteItem.release_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);
                        if (Convert.ToInt32(quantityCheckBox.Tag.ToString().Split('-')[2]) == BASIC_MACROS.IS_WORK_ORDER)
                        {
                            recievalNoteItem.work_order_serial = Convert.ToInt32(quantityCheckBox.Tag.ToString().Split('-')[0]);
                            recievalNoteItem.product_number = Convert.ToInt32(quantityCheckBox.Tag.ToString().Split('-')[1]);
                           

                        }
                        recievalNote.AddRecievalNoteItem(recievalNoteItem);
                    }

                }
                
            }

            recievalNote.SetRecievalNoteDate(Convert.ToDateTime(recievalDate.SelectedDate));


            if (!recievalNote.IssueNewRecievalNote())
                return;
          
         

            recievalNoteUploadFilesPage = new AddRecievalNoteUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser , this, ref recievalNote);


            this.NavigationService.Navigate(recievalNoteUploadFilesPage);

            ViewItems(recievalNote.GetRecievalNoteSerial());

        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            parentWindow.Close();

        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnReleasePermitsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (parentWindow.isView == true)
                return;

            recievalNote = new MaterialRecievalNote();

            ReleasePermitItemsGrid.Children.Clear();

            ComboBox releaseSerialsComboBox = sender as ComboBox;

            if (releaseSerialsComboBox.SelectedIndex != -1)
            {

                recievalNote.SetReleaseSerial(releasePermits[releaseSerialsComboBox.SelectedIndex].release_Permit_Serial);

                recievalNote.InitializeMaterialReleasePermit();
            }

            InitializeItemsUi(recievalNote, false);


        }

        public void InitializeItemsUi(MaterialRecievalNote recievalNote, bool isView)
        {

            ReleasePermitItemsGrid.Children.Clear();
            ReleasePermitItemsGrid.RowDefinitions.Clear();

            Grid header = new Grid();
            header.ShowGridLines = true;
            header.ColumnDefinitions.Add(new ColumnDefinition());
            header.ColumnDefinitions.Add(new ColumnDefinition());
            header.ColumnDefinitions.Add(new ColumnDefinition());
            header.ColumnDefinitions.Add(new ColumnDefinition());


            Label entryPermitSerialId = new Label();

            entryPermitSerialId.Style = (Style)FindResource("tableHeaderItem");

            entryPermitSerialId.Content = "SERIAL ID";

            Grid.SetColumn(entryPermitSerialId, 0);

            header.Children.Add(entryPermitSerialId);



            Label itemNameLabel = new Label();

            itemNameLabel.Style = (Style)FindResource("tableHeaderItem");

            itemNameLabel.Content = "ITEM NAME";

            Grid.SetColumn(itemNameLabel, 1);

            header.Children.Add(itemNameLabel);


            Label serialNumber = new Label();

            serialNumber.Style = (Style)FindResource("tableHeaderItem");

            serialNumber.Content = "SERIAL NUMBER";

            Grid.SetColumn(serialNumber, 2);

            header.Children.Add(serialNumber);





            Label quantity = new Label();

            quantity.Style = (Style)FindResource("tableHeaderItem");

            quantity.Content = "Quantity";


            Grid.SetColumn(quantity, 3);

            header.Children.Add(quantity);

            ReleasePermitItemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });

            Grid.SetRow(header, ReleasePermitItemsGrid.RowDefinitions.Count - 1);

            ReleasePermitItemsGrid.Children.Add(header);

            if (isView == false)
            {

                for (int i = 0; i < recievalNote.GetReleaseItems().Count; i++)
                {


                    if (recievalNote.GetReleaseItems()[i].release_permit_item_status != COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL &&
                        recievalNote.GetReleaseItems()[i].release_permit_item_status != COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL)
                    {

                        recievalNote.GetReleaseItems().Remove(recievalNote.GetReleaseItems()[i]);
                        i--;

                    }

                }


            }




            for (int i = 0; i < recievalNote.GetReleaseItems().Count; i++)
            {


                ReleasePermitItemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });

                Grid itemm = new Grid();


                itemm.Tag = recievalNote.GetReleaseItems()[i].release_permit_item_serial;

                itemm.ShowGridLines = true;


                itemm.ColumnDefinitions.Add(new ColumnDefinition());
                itemm.ColumnDefinitions.Add(new ColumnDefinition());
                itemm.ColumnDefinitions.Add(new ColumnDefinition());
                itemm.ColumnDefinitions.Add(new ColumnDefinition());


                itemm.Tag = recievalNote.GetReleaseSerial().ToString() + " " + recievalNote.GetReleaseItems()[i].release_permit_item_serial;

                Label entryPermitSerialLabel = new Label();


                entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                entryPermitSerialLabel.Content = recievalNote.GetReleaseId();

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

                if (recievalNote.GetReleaseItems()[i].materialItemGenericModel.model_name != "")
                    ItemName.Text = $"{recievalNote.GetReleaseItems()[i].materialItemGenericCategory.category_name + "-" + recievalNote.GetReleaseItems()[i].materialitemGenericproduct.product_name + "-" + recievalNote.GetReleaseItems()[i].materialItemGenericBrand.brand_name + "-" + recievalNote.GetReleaseItems()[i].materialItemGenericModel.model_name}";
                else
                    ItemName.Text = $"{recievalNote.GetReleaseItems()[i].materialItemcompanyCategory.category_name + "-" + recievalNote.GetReleaseItems()[i].materialitemCompanyproduct.product_name + "-" + recievalNote.GetReleaseItems()[i].materialItemCompanyBrand.brand_name + "-" + recievalNote.GetReleaseItems()[i].materialItemCompanyModel.model_name}";


                Grid.SetColumn(ItemName, 1);

                itemm.Children.Add(ItemName);


                if (recievalNote.GetReleaseItems()[i].entryPermit_product_serial_number != "")
                {


                    CheckBox chooseItemSerialCheckBox = new CheckBox();

                    chooseItemSerialCheckBox.Content = recievalNote.GetReleaseItems()[i].entryPermit_product_serial_number;

                    chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                    if (isView == true)
                    {
                        chooseItemSerialCheckBox.IsEnabled = false;
                        chooseItemSerialCheckBox.IsChecked = true;
                    }

                    chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");
                    if(recievalNote.GetReleaseItems()[i].workOrder_serial != 0)
                    {
                        chooseItemSerialCheckBox.Tag = recievalNote.GetReleaseItems()[i].workOrder_serial + "-" + recievalNote.GetReleaseItems()[i].workOrder_product_number + "-" + recievalNote.GetReleaseItems()[i].workOrder_serial_Number_id +"-"+BASIC_MACROS.IS_WORK_ORDER;
                    }
                    else
                    {
                        chooseItemSerialCheckBox.Tag = recievalNote.GetReleaseItems()[i].rfp_info + "-" + recievalNote.GetReleaseItems()[i].rfp_item_number + "-" + BASIC_MACROS.IS_RFP;
                    }

                    Grid.SetColumn(chooseItemSerialCheckBox, 2);

                    itemm.Children.Add(chooseItemSerialCheckBox);



                }

                if (recievalNote.GetReleaseItems()[i].entryPermit_product_serial_number == "")
                {



                    Grid quantityGrid = new Grid();

                    quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                    TextBox quantityAvailableTextBox = new TextBox();

                    quantityAvailableTextBox.Tag = recievalNote.GetReleaseItems()[i].released_quantity_release.ToString();


                    quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                    quantityAvailableTextBox.Text = recievalNote.GetReleaseItems()[i].released_quantity_release.ToString();
                    quantityAvailableTextBox.IsEnabled = false;


                    TextBox quantityTextBox = new TextBox();

                    quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");



                    CheckBox quantityCheckBox = new CheckBox();

                    quantityCheckBox.Width = 30;

                    quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");
                    if (recievalNote.GetReleaseItems()[i].workOrder_serial != 0)
                    {
                        quantityCheckBox.Tag = recievalNote.GetReleaseItems()[i].workOrder_serial + "-" + recievalNote.GetReleaseItems()[i].workOrder_product_number+"-"+BASIC_MACROS.IS_WORK_ORDER;
                    }
                    else
                    {
                        quantityCheckBox.Tag = recievalNote.GetReleaseItems()[i].rfp_info + "-" + recievalNote.GetReleaseItems()[i].rfp_item_number + "-" + BASIC_MACROS.IS_RFP;
                    }



                    Grid.SetColumn(quantityCheckBox, 0);

                    Grid.SetColumn(quantityAvailableTextBox, 1);

                    Grid.SetColumn(quantityTextBox, 2);


                    quantityGrid.Children.Add(quantityCheckBox);
                    quantityGrid.Children.Add(quantityAvailableTextBox);
                    quantityGrid.Children.Add(quantityTextBox);


                    if (isView == true)
                    {
                        quantityGrid.IsEnabled = false;
                        quantityCheckBox.IsChecked = true;



                    }

                    Grid.SetColumn(quantityGrid, 3);

                    itemm.Children.Add(quantityGrid);

                }

                Grid.SetRow(itemm, ReleasePermitItemsGrid.RowDefinitions.Count - 1);

                ReleasePermitItemsGrid.Children.Add(itemm);

            }





        }


        public void ViewItems(int recievalNoteSerial)
        {


            finishButton.IsEnabled = false;

            MaterialRecievalNote materialRecievalNote = new MaterialRecievalNote();

            materialRecievalNote.SetRecievalNoteSerial(recievalNoteSerial);

            if (!materialRecievalNote.InitializeMaterialRecievalNote())
                return;


            recievalDate.IsEnabled = false;

            recievalDate.SelectedDate = materialRecievalNote.GetRecievalNoteDate();
            releasePermitsComboBox.SelectedItem = materialRecievalNote.GetReleaseId();
            releasePermitsComboBox.IsEnabled = false;

            recievalNoteUploadFilesPage = new AddRecievalNoteUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, ref materialRecievalNote);


            InitializeItemsUi(materialRecievalNote, true);

        }
    }
}
