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
    /// Interaction logic for AddReleasePermitPage.xaml
    /// </summary>
    public partial class AddReleasePermitPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public AddReleasePermitItemPage addReleasePermitItem;

        AddReleasePermitWindow parentWindow;
        public WorkOrder workOrder = new WorkOrder();

        public RFP rfp = new RFP();

        public MaterialReleasePermits materialReleasePermit = new MaterialReleasePermits();
        public List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

        public List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrders = new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();

         List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requsters = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
        public List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requstersFiltered = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();

        public List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT> rfps = new List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT>();
        List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> companyContacts=new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();


        public List<int> serialProducts = new List<int>();


        public AddReleasePermitPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow w)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            commonQueries.GetWorkOrders(ref workOrders);

            commonQueries.GetRFPRequestors(ref requsters);


            for (int i = 0; i < requsters.Count; i++)
            {
                if (i != 0 && requsters[i].requestor_team.team_name != requsters[i - 1].requestor_team.team_name)
                {
                    requstersFiltered.Add(requsters[i]);
                }
                else if (i == 0)
                {
                    requstersFiltered.Add(requsters[i]);
                }
            }

            InitializeComponent();

            parentWindow = w;

            addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

            employees.Clear();
            commonQueries.GetCurrentlyEnrolledEmployees(ref employees);

            employees.ForEach(a => MaterialRecieverComboBox.Items.Add(a.employee_name));


            materialReleasePermit.SetReleaseBy(loggedInUser.GetEmployeeId());

            materialReleasePermit.GetNewEntrySerial();



            WrapPanel orderPanel = mainPanel.Children[1] as WrapPanel;
            ComboBox orderSerialsComboBox= orderPanel.Children[0] as ComboBox;


            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;
            ComboBox rfpRequstersComboBox = rfpPanel.Children[0] as ComboBox;

            requstersFiltered.ForEach(a => rfpRequstersComboBox.Items.Add(a.requestor_team.team_name));

            workOrders.ForEach(a => orderSerialsComboBox.Items.Add(a.order_id));  

        }


        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            parentWindow.Close();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {

            materialReleasePermit.SetReleaseId(SerialIdTextBox.Text);
           // addReleasePermitItem.addReleasePermitPage = this;

            this.NavigationService.Navigate(addReleasePermitItem);


        }

        private void BasicInfoLableMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (orderChecked.IsChecked != true && rfpChecked.IsChecked != true) {

                System.Windows.Forms.MessageBox.Show("You have to choose rfp or order", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            materialReleasePermit.SetReleaseId(SerialIdTextBox.Text);
           // addReleasePermitItem.addReleasePermitPage = this;
            this.NavigationService.Navigate(addReleasePermitItem);

        }

        private void ReleaseDatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            materialReleasePermit.SetReleaseDate(Convert.ToDateTime(ReleaseDatePicker.SelectedDate));

        }

        private void MaterialRecieverComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterialRecieverComboBox.SelectedIndex == -1)
                return;

            materialReleasePermit.SetMaterialReceiver(employees[MaterialRecieverComboBox.SelectedIndex].employee_id);

        }

        private void rfpCheckedChecked(object sender, RoutedEventArgs e)
        {
            orderChecked.IsChecked = false;

            WrapPanel rfpPanel=mainPanel.Children[0] as WrapPanel;
            rfpPanel.Visibility = Visibility.Visible;


            WrapPanel orderPanel = mainPanel.Children[1] as WrapPanel;
            orderPanel.Visibility = Visibility.Collapsed;

           ComboBox orderSerialsComboBox= orderPanel.Children[0] as ComboBox;

            orderSerialsComboBox.SelectedIndex = -1;


            addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

            if (parentWindow.isView == true)
            {
               // addReleasePermitItem.ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addReleasePermitItem, parentWindow.releasePermitPage, parentWindow, ref parentWindow.materialReleasePermit);
            }


        }

        private void orderCheckedChecked(object sender, RoutedEventArgs e)
        {
            rfpChecked.IsChecked = false;


            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;
            rfpPanel.Visibility = Visibility.Collapsed;


            WrapPanel orderPanel = mainPanel.Children[1] as WrapPanel;
            orderPanel.Visibility = Visibility.Visible;


            addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

            if (parentWindow.isView == true) {
                //addReleasePermitItem.ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addReleasePermitItem, parentWindow.releasePermitPage, parentWindow, ref parentWindow.materialReleasePermit);
            }
        }

        private void rfpRequestersSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;

           ComboBox rfpsRequsters= rfpPanel.Children[0] as ComboBox;

            ComboBox rfpComboBox = rfpPanel.Children[1] as ComboBox;
            commonQueries.GetTeamRFPs(ref rfps, requstersFiltered[rfpsRequsters.SelectedIndex].requestor_team.team_id);
            rfpComboBox.Items.Clear();

            rfps.ForEach(a => rfpComboBox.Items.Add(a.rfpID));


        }

        private bool InitializeCompanyContacts()
        {
            WrapPanel orderPanel = mainPanel.Children[1] as WrapPanel;

           ComboBox ordersComboBox= orderPanel.Children[0] as ComboBox;

            ComboBox companyContactsComboBox = orderPanel.Children[1] as ComboBox;


            workOrder.InitializeWorkOrderInfo(workOrders[ordersComboBox.SelectedIndex].order_serial);

            companyContacts.Clear();
            companyContactsComboBox.Items.Clear();
            companyContactsComboBox.IsEnabled = true;
            if (orderChecked.IsChecked == true)
            {
                if (!commonQueries.GetCompanyContacts(workOrder.GetCompanySerial(), ref companyContacts))
                    return false;
            }
          
            for (int i = 0; i < companyContacts.Count(); i++)
            {
                companyContactsComboBox.Items.Add(companyContacts[i].contact.contact_name);
            }
            return true;
        }

        private void orderSerialsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serialProducts.Clear();
            workOrder.SetOrderIssueDateToToday();

            WrapPanel orderPanel = mainPanel.Children[1] as WrapPanel;

            ComboBox ordersComboBox = orderPanel.Children[0] as ComboBox;

            if (ordersComboBox.SelectedIndex != -1)
            {
                InitializeCompanyContacts();

                workOrder.InitializeWorkOrderInfo(workOrders[ordersComboBox.SelectedIndex].order_serial);

            }

            for (int i = 0; i < workOrder.GetOrderProductsList().Length; i++) {


                if (workOrder.GetOrderProductsList()[i].has_serial_number == true)
                {
                    int numberOfSerials = 0;
                    workOrder.GetNumOfProductSerialsForAProduct(workOrder.GetOrderSerial(), workOrder.GetOrderProductsList()[i].productNumber, ref numberOfSerials);

                    serialProducts.Add(numberOfSerials);
                }

                else {

                    serialProducts.Add(0);
                
                }

            }

            materialReleasePermit.SetWorkOrder(workOrder);
            materialReleasePermit.SetRfp(null);


        }

        private void OnBtnClickAddContact(object sender, RoutedEventArgs e)
        {
            //AddContactWindow currentWindow = new AddContactWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, workOrder.GetCompanySerial(), workOrder.GetCompanyName());
            //currentWindow.Closed += OnClosedAddContactWindow;
            //currentWindow.Show();

        }

        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            if (!InitializeCompanyContacts())
                return;
        }

        private void rfpSerialsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;
            ComboBox rfpSerialsComboBox = rfpPanel.Children[1] as ComboBox;

            serialProducts.Clear();



            if(rfpSerialsComboBox.SelectedIndex!=-1)
            rfp.InitializeRFP(rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion);


            commonQueries.GetRfpItemsMapping(rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion, rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems);

            for (int i = 0; i < rfpItems.Count; i++) {

                if (rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED && rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_AT_STOCK) {

                    rfpItems.Remove(rfpItems[i]);
                    i--;
                }

            
            }


            for (int i = 0; i < rfpItems.Count; i++) {

                if (rfpItems[i].product_model.model_name != "")
                {

                    if (rfpItems[i].product_model.has_serial_number == true)
                    {


                        serialProducts.Add(0);


                    }

                    else 
                    {

                        serialProducts.Add(0);
                    
                    }


                }


                else
                {


                    if (rfpItems[i].product_model.has_serial_number == true)
                    {


                        serialProducts.Add(0);


                    }

                    else
                    {

                        serialProducts.Add(0);

                    }

                }
            
            }

            materialReleasePermit.SetRfp(rfp);
            materialReleasePermit.SetWorkOrder(null);



        }

        private void OnContactsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

           ComboBox contactsComboBox =sender as ComboBox;

            if (contactsComboBox.SelectedIndex != -1)
            {
                materialReleasePermit.SetContactId(workOrder.GetContactId());
                materialReleasePermit.SetBranchSerialId(workOrder.GetAddressSerial());

                materialReleasePermit.SetSalesPersonId(workOrder.GetSalesPersonId());
            }
            else {


                materialReleasePermit.SetContactId(0);
            }
        }

        private void OnRfpUnChecked(object sender, RoutedEventArgs e) {

            rfpPanel.Visibility = Visibility.Collapsed;

        }

        private void OnOrderUnChecked(object sender, RoutedEventArgs e)
        {

            orderPanel.Visibility = Visibility.Collapsed;


        }
    }
}
