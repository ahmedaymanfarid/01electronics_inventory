using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AddRentryItemPage.xaml
    /// </summary>
    public partial class AddRentryItemPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public RentryPermitWindow parentWindow ;
        AddRentryUploadFilesPage rentryPageUploadFilesPage;

        bool first = true;

        RentryPermit reEntryPermit ;

        List<BASIC_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT> releasePermits = new List<BASIC_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT>();

        public AddRentryItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, RentryPermitWindow mWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            parentWindow = mWindow;

            InitializeComponent();
            
            commonQueries.GetReleasePermits(ref releasePermits);

            releasePermits.ForEach(a => releasePermitsComboBox.Items.Add(a.release_Permit_Id));

            ReEntryDate.SelectedDate = DateTime.Now;

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnReEntryPermitFilesMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (parentWindow.isView) {
                this.NavigationService.Navigate(rentryPageUploadFilesPage);
            }

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {


            if (releasePermitsComboBox.SelectedIndex == -1) {

                System.Windows.Forms.MessageBox.Show("release Permit is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                return;
            }

            int counter = 0;

            if (first == true)
            ReleasePermitItemsGrid.Children.RemoveAt(0);

            first = false;

            for (int i = 0; i < ReleasePermitItemsGrid.Children.Count; i++)
            {


                reEntryPermit.SetAddedBy(loggedInUser.GetEmployeeId());

                BASIC_STRUCTS.MATERIAL_RE_ENTRY_ITEM reEntryItem = new BASIC_STRUCTS.MATERIAL_RE_ENTRY_ITEM();

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

                        reEntryItem.reEntry_Permit_item_serial = counter;

                        reEntryItem.release_serial= Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        reEntryItem.release_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);


 
                        reEntryPermit.AddReEntryItem(reEntryItem);



                    }

                }



                else
                {

                    Grid quantityGrid = item.Children[2] as Grid;
                    CheckBox quantityCheckBox = quantityGrid.Children[0] as CheckBox;
                    TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;


                    if (quantityCheckBox.IsChecked == true)
                    {



                        reEntryItem.reEntry_Permit_item_serial = counter;

                        reEntryItem.release_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        reEntryItem.release_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);





                        reEntryPermit.AddReEntryItem(reEntryItem);


                    }

                }

            }

                reEntryPermit.SetReEntryDate(Convert.ToDateTime(ReEntryDate.SelectedDate));
            

            if (!reEntryPermit.IssueNewReEntryPermit())
                return;

            rentryPageUploadFilesPage = new AddRentryUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, ref reEntryPermit);


            this.NavigationService.Navigate(rentryPageUploadFilesPage);


            ViewItems(reEntryPermit.GetRentryPermitSerial());

        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            parentWindow.Close();

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnreleasePermitsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (parentWindow.isView == true)
                return;

            reEntryPermit = new RentryPermit();

            ReleasePermitItemsGrid.Children.Clear();

           ComboBox releaseSerialsComboBox= sender as ComboBox;

            if (releaseSerialsComboBox.SelectedIndex != -1) {

                reEntryPermit.SetReleaseSerial(releasePermits[releaseSerialsComboBox.SelectedIndex].release_Permit_Serial);

                reEntryPermit.InitializeMaterialReleasePermit();
            }

            InitializeItemsUi(reEntryPermit,false);


        }

        public void InitializeItemsUi(RentryPermit reEntryPermit,bool isView) {

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

            ReleasePermitItemsGrid.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(100)});

            Grid.SetRow(header, ReleasePermitItemsGrid.RowDefinitions.Count - 1);

            ReleasePermitItemsGrid.Children.Add(header);

            if (isView == false) {

                for (int i = 0; i < reEntryPermit.GetReleaseItems().Count; i++)
                {


                    if (reEntryPermit.GetReleaseItems()[i].release_permit_item_status != COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL &&
                        reEntryPermit.GetReleaseItems()[i].release_permit_item_status != COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL)
                    {

                        reEntryPermit.GetReleaseItems().Remove(reEntryPermit.GetReleaseItems()[i]);
                        i--;

                    }

                }


            }
           



            for (int i = 0; i < reEntryPermit.GetReleaseItems().Count; i++) {


                ReleasePermitItemsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });

                    Grid itemm = new Grid();

                    itemm.ShowGridLines = true;

                    itemm.Tag = reEntryPermit.GetReleaseSerial().ToString() + " " + reEntryPermit.GetReleaseItems()[i].release_permit_item_serial;


                    itemm.ColumnDefinitions.Add(new ColumnDefinition());
                    itemm.ColumnDefinitions.Add(new ColumnDefinition());
                    itemm.ColumnDefinitions.Add(new ColumnDefinition());
                    itemm.ColumnDefinitions.Add(new ColumnDefinition());


                    itemm.Tag = reEntryPermit.GetReleaseSerial().ToString() + " " + reEntryPermit.GetReleaseItems()[i].release_permit_item_serial;

                    Label entryPermitSerialLabel = new Label();


                    entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                    entryPermitSerialLabel.Content = reEntryPermit.GetReleaseId();

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

                if (reEntryPermit.GetReleaseItems()[i].materialItemGenericModel.modelName!="")
                    ItemName.Text = $"{reEntryPermit.GetReleaseItems()[i].materialItemGenericCategory.category_name + "-" + reEntryPermit.GetReleaseItems()[i].materialitemGenericproduct.typeName + "-" + reEntryPermit.GetReleaseItems()[i].materialItemGenericBrand.brandName + "-" + reEntryPermit.GetReleaseItems()[i].materialItemGenericModel.modelName}";
                else
                    ItemName.Text = $"{reEntryPermit.GetReleaseItems()[i].materialItemcompanyCategory.category + "-" + reEntryPermit.GetReleaseItems()[i].materialitemCompanyproduct.typeName + "-" + reEntryPermit.GetReleaseItems()[i].materialItemCompanyBrand.brandName + "-" + reEntryPermit.GetReleaseItems()[i].materialItemCompanyModel.modelName}";


                Grid.SetColumn(ItemName, 1);

                itemm.Children.Add(ItemName);


            if (reEntryPermit.GetReleaseItems()[i].entryPermit_product_serial_number != "")
            {

                    
                CheckBox chooseItemSerialCheckBox = new CheckBox();

                chooseItemSerialCheckBox.Content = reEntryPermit.GetReleaseItems()[i].entryPermit_product_serial_number;

                chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                    if (isView == true) {
                        chooseItemSerialCheckBox.IsEnabled = false;
                        chooseItemSerialCheckBox.IsChecked = true;
                    }

                    chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");



                Grid.SetColumn(chooseItemSerialCheckBox, 2);

                itemm.Children.Add(chooseItemSerialCheckBox);



            }

            if (reEntryPermit.GetReleaseItems()[i].entryPermit_product_serial_number == "")
            {



                Grid quantityGrid = new Grid();

                quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                TextBox quantityAvailableTextBox = new TextBox();

                quantityAvailableTextBox.Tag = reEntryPermit.GetReleaseItems()[i].released_quantity_release.ToString();


                quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                quantityAvailableTextBox.Text = reEntryPermit.GetReleaseItems()[i].released_quantity_release.ToString();
                quantityAvailableTextBox.IsEnabled = false;


                TextBox quantityTextBox = new TextBox();

                quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");



                CheckBox quantityCheckBox = new CheckBox();

                quantityCheckBox.Width = 30;

                quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");


   

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

        public void ViewItems(int rentrySerial) {

          
            finishButton.IsEnabled = false;

            RentryPermit rentryPermit = new RentryPermit();

            rentryPermit.SetRentryPermitSerial(rentrySerial);
            if (!rentryPermit.InitializeMaterialReEntryPermit())
                return;


            ReEntryDate.IsEnabled = false;

            ReEntryDate.SelectedDate = rentryPermit.GetReEntryDate();
            releasePermitsComboBox.SelectedItem = rentryPermit.GetReleaseId();
            releasePermitsComboBox.IsEnabled = false;

            rentryPageUploadFilesPage = new AddRentryUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, ref rentryPermit);

            InitializeItemsUi(rentryPermit, true);

            
        
        
        }
    }
}
