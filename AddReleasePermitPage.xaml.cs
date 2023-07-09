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
        public WorkOrder workOrder;

        public RFP rfp;


        public MaterialReleasePermits materialReleasePermit;
        public List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees;

        public List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrders;

         List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requsters;
        public List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> requstersFiltered;

        public List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT> rfps;
        List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> companyContacts;

        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems;


        public List<int> serialProducts;


        public AddReleasePermitPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow mParentWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            workOrders =new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
            requsters = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            requstersFiltered = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            rfps = new List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT>();
            companyContacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();
            rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
            serialProducts = new List<int>();
            parentWindow = mParentWindow;
            materialReleasePermit = new MaterialReleasePermits();
            rfp = parentWindow.rfps;
            workOrder = parentWindow.workOrder;
            //addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

            InitializeComponent();

            GetOrderIDs();
            GetRFPsRequestorTeams();
            GetCurrentEnrolledEmployees();
            GetNewEntrySerial();
            FillRFPRequestorTeamComboBox();
            FillOrderIDsComboBox();
          
        }

        private void GetOrderIDs()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return;
        }
        private void GetRFPsRequestorTeams()
        {
            if (!commonQueries.GetRFPRequestors(ref requsters))
                return;

            for (int i = 0; i < requsters.Count; i++)
            {
                PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT rfpRequestorFound = requstersFiltered.Find(f => f.requestor_team.team_id == requsters[i].requestor_team.team_id);
                if(rfpRequestorFound.requestor_team.team_id == 0)
                {
                    requstersFiltered.Add(requsters[i]);
                }
                //if (i != 0 && requsters[i].requestor_team.team_id != requsters[i - 1].requestor_team.team_id)
                //{
                //    requstersFiltered.Add(requsters[i]);
                //}
                //else if (i == 0)
                //{
                //    requstersFiltered.Add(requsters[i]);
                //}
            }
        }
        private void GetCurrentEnrolledEmployees()
        {
            employees.Clear();
            if (!commonQueries.GetCurrentlyEnrolledEmployees(ref employees))
                return;

            employees.ForEach(a => MaterialRecieverComboBox.Items.Add(a.employee_name));
        }
        private void GetNewEntrySerial()
        {
            materialReleasePermit.SetReleaseBy(loggedInUser.GetEmployeeId());
            if(!materialReleasePermit.GetNewEntrySerial())
                return;
        }
        private void FillRFPRequestorTeamComboBox()
        {
            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;
            ComboBox rfpRequstersComboBox = rfpPanel.Children[1] as ComboBox;

            requstersFiltered.ForEach(a => rfpRequstersComboBox.Items.Add(a.requestor_team.team_name));
        }
        private void FillOrderIDsComboBox()
        {
            WrapPanel orderPanel = mainPanel.Children[2] as WrapPanel;
            ComboBox orderSerialsComboBox = orderPanel.Children[1] as ComboBox;
            workOrders.ForEach(a => orderSerialsComboBox.Items.Add(a.order_id));
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
            this.NavigationService.Navigate(parentWindow.releasePermitItemPage);

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

            WrapPanel rfpRequestorTeamWrapPanel=mainPanel.Children[0] as WrapPanel;
            rfpRequestorTeamWrapPanel.Visibility = Visibility.Visible;

            WrapPanel rfpIDWrapPanel = mainPanel.Children[1] as WrapPanel;
            rfpIDWrapPanel.Visibility = Visibility.Visible;

            WrapPanel orderIdWrapPanel = mainPanel.Children[2] as WrapPanel;
            orderIdWrapPanel.Visibility = Visibility.Collapsed;

            WrapPanel orderContactWrapPanel = mainPanel.Children[3] as WrapPanel;
            orderContactWrapPanel.Visibility = Visibility.Collapsed;

            WrapPanel orderEndingChoice = mainPanel.Children[4] as WrapPanel;
            orderEndingChoice.Visibility = Visibility.Collapsed;

            ComboBox orderSerialsComboBox = orderIdWrapPanel.Children[1] as ComboBox;
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


            WrapPanel rfpRequestorPanel = mainPanel.Children[0] as WrapPanel;
            rfpRequestorPanel.Visibility = Visibility.Collapsed;

            WrapPanel rfpIDPanel = mainPanel.Children[1] as WrapPanel;
            rfpIDPanel.Visibility = Visibility.Collapsed;

            WrapPanel orderSerialPanel = mainPanel.Children[2] as WrapPanel;
            orderSerialPanel.Visibility = Visibility.Visible;

            WrapPanel orderContactPanel = mainPanel.Children[3] as WrapPanel;
            orderContactPanel.Visibility = Visibility.Visible;

            WrapPanel chooseToBeClosedWith = mainPanel.Children[4] as WrapPanel;
            chooseToBeClosedWith.Visibility = Visibility.Visible;


            //addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

            if (parentWindow.isView == true) {
                //addReleasePermitItem.ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addReleasePermitItem, parentWindow.releasePermitPage, parentWindow, ref parentWindow.materialReleasePermit);
            }
        }

        private void rfpRequestersSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WrapPanel rfpRequestorTeamWrapPanel = mainPanel.Children[0] as WrapPanel;
            ComboBox rfpsRequstersTeamComboBox= rfpRequestorTeamWrapPanel.Children[1] as ComboBox;

            WrapPanel rfpIDWrapPanel = mainPanel.Children[1] as WrapPanel;
            ComboBox rfpIDComboBox = rfpIDWrapPanel.Children[1] as ComboBox;

            if (!commonQueries.GetTeamRFPs(ref rfps, requstersFiltered[rfpsRequstersTeamComboBox.SelectedIndex].requestor_team.team_id))
                return;
            rfpIDComboBox.Items.Clear();

            rfps.ForEach(a => rfpIDComboBox.Items.Add(a.rfpID));


        }

        private bool InitializeCompanyContacts()
        {
            WrapPanel orderPanel = mainPanel.Children[2] as WrapPanel;
            WrapPanel orderContacts = mainPanel.Children[3] as WrapPanel;

            ComboBox ordersComboBox= orderPanel.Children[1] as ComboBox;
            ComboBox companyContactsComboBox = orderContacts.Children[1] as ComboBox;


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

        private void OnSelectionChangedOrderSerial(object sender, SelectionChangedEventArgs e)
        {
            serialProducts.Clear();
            workOrder.SetOrderIssueDateToToday();

            WrapPanel orderPanel = mainPanel.Children[2] as WrapPanel;

            ComboBox ordersComboBox = orderPanel.Children[1] as ComboBox;

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

      
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            if (!InitializeCompanyContacts())
                return;
        }

        
        /// <summary>
        /// ////////////////////////////////// ON SELECTION CHANGED /////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChangedContactsComboBox(object sender, SelectionChangedEventArgs e)
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
        private void OnSelectionChangedRfpIdComboBox(object sender, SelectionChangedEventArgs e)
        {
            WrapPanel rfpPanel = mainPanel.Children[0] as WrapPanel;
            ComboBox rfpSerialsComboBox = rfpPanel.Children[1] as ComboBox;

            serialProducts.Clear();

            

            if (rfpSerialsComboBox.SelectedIndex != -1)
                if (!rfp.InitializeRFP(rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion))
                    return;


            if (!commonQueries.GetRfpItemsMapping(rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion, rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems))
                return;

            for (int i = 0; i < rfpItems.Count; i++)
            {

                if (rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED && rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_AT_STOCK)
                {

                    rfpItems.Remove(rfpItems[i]);
                    i--;
                }


            }


            for (int i = 0; i < rfpItems.Count; i++)
            {

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
        /// <summary>
        /// /////////////////////////////////// ON UNCHECK CKECKBOX ///////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnCheckRFPCheckBox(object sender, RoutedEventArgs e) {

            rfpPanel.Visibility = Visibility.Collapsed;
            rfpIdPanel.Visibility = Visibility.Collapsed;   
        }

        private void OnUncheckOrderCheckBox(object sender, RoutedEventArgs e)
        {

            orderPanel.Visibility = Visibility.Collapsed;
            orderContactPanel.Visibility=Visibility.Collapsed;
            chooseToBeClosedWith.Visibility=Visibility.Collapsed;
            serviceReportCheckBox.IsChecked=false;
            receivalNoteCheckBox.IsChecked=false;
        }

        /// <summary>
        /// ///////////////////////////////////// ON BUTTON CLICK //////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnButtonClickCancel(object sender, RoutedEventArgs e)
        {
            parentWindow.Close();
        }

        private void OnButtonClickNext(object sender, RoutedEventArgs e)
        {
            if (orderChecked.IsChecked != true && rfpChecked.IsChecked != true)
            {

                System.Windows.Forms.MessageBox.Show("You have to choose rfp or order", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            if(orderChecked.IsChecked == true && serviceReportCheckBox.IsChecked==false && receivalNoteCheckBox.IsChecked == false )
            {
                System.Windows.Forms.MessageBox.Show("Please choose ending it with receival note or service report", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            materialReleasePermit.SetReleaseId(SerialIdTextBox.Text);
            // addReleasePermitItem.addReleasePermitPage = this;

            this.NavigationService.Navigate(parentWindow.releasePermitItemPage);


        }
        private void OnButtonClickAddContact(object sender, RoutedEventArgs e)
        {
            //AddContactWindow currentWindow = new AddContactWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, workOrder.GetCompanySerial(), workOrder.GetCompanyName());
            //currentWindow.Closed += OnClosedAddContactWindow;
            //currentWindow.Show();

        }

        private void OnCheckServiceReportCheckBox(object sender, RoutedEventArgs e)
        {
            parentWindow.serviceReport = true;
            parentWindow.rfp = false;
            receivalNoteCheckBox.IsChecked = false;
        }

        private void OnCheckReceivalNoteCheckBox(object sender, RoutedEventArgs e)
        {
            parentWindow.serviceReport = false;
            parentWindow.rfp = false;
            serviceReportCheckBox.IsChecked = false;
        }
    }
}
