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
        int viewAddCondition;

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
            materialReleasePermit =parentWindow.materialReleasePermit;
            rfp = parentWindow.rfps;
            workOrder = parentWindow.workOrder;
            addReleasePermitItem = parentWindow.releasePermitItemPage;
            viewAddCondition = parentWindow.viewAddCondition;
            //addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);
            InitializeComponent();
            if (parentWindow.viewAddCondition==COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                ReleaseDatePicker.SelectedDate = parentWindow.materialReleasePermit.GetReleaseDate();
                ReleaseDatePicker.IsEnabled = false;
                MaterialRecieverComboBox.Items.Clear();
                MaterialRecieverComboBox.Items.Add(parentWindow.materialReleasePermit.GetMaterialReceiver().employee_name);
                MaterialRecieverComboBox.SelectedIndex = 0;
                MaterialRecieverComboBox.IsEnabled = false;
                SerialIdTextBox.Text = parentWindow.materialReleasePermit.GetReleaseId();
                SerialIdTextBox.IsEnabled = false;
                if (parentWindow.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpSerial ==0)
                {
                    workFormComboBox.SelectedIndex = 1;
                    orderSerials.Items.Clear();
                    workFormComboBox.IsEnabled = false;
                    orderSerials.Items.Add(parentWindow.materialReleasePermit.GetWorkOrder().orderSerial);
                    orderSerials.SelectedIndex = 0;
                    orderSerials.IsEnabled = false;

                    contactComboBox.Items.Clear();
                    contactComboBox.Items.Add(parentWindow.materialReleasePermit.GetWorkOrder().GetContactName());
                    contactComboBox.SelectedIndex = 0;
                    contactComboBox.IsEnabled = false;

                    if(parentWindow.materialReleasePermit.GetReleasePermitStatus()==COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT || parentWindow.materialReleasePermit.GetReleasePermitStatus() == COMPANY_WORK_MACROS.SERVICE_REPORT_RECEIVED)
                    {
                        serviceReportCheckBox.IsChecked = true;
                        
                    }
                    else if(parentWindow.materialReleasePermit.GetReleasePermitStatus() == COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL || parentWindow.materialReleasePermit.GetReleasePermitStatus() == COMPANY_WORK_MACROS.RECEIVAL_NOTE_RECEIVED)
                    {
                        receivalNoteCheckBox.IsChecked = true;
                    }
                    serviceReportCheckBox.IsEnabled = false;
                    receivalNoteCheckBox.IsEnabled = false;
                }
                else
                {
                    workFormComboBox.SelectedIndex = 0;
                    rfpRequesters.Items.Clear();
                    rfpRequesters.Items.Add(parentWindow.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfp_requestor_team_name);
                    rfpRequesters.SelectedIndex = 0;
                    workFormComboBox.IsEnabled = false;
                    rfpRequesters.IsEnabled = false;

                    rfpSerials.Items.Clear();
                    rfpSerials.Items.Add(parentWindow.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpID);
                    rfpSerials.SelectedIndex = 0;
                    rfpSerials.IsEnabled = false;   



                }

            }
            else
            {
                GetOrderIDs();
                GetRFPsRequestorTeams();
                GetCurrentEnrolledEmployees();
                GetNewEntrySerial();
                FillRFPRequestorTeamComboBox();
                FillOrderIDsComboBox();
            }

           

          
          
        }

        private void GetOrderIDs()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return;
        }
        private void GetRFPsRequestorTeams()
        {
            if (!commonQueries.GetRFPRequestorsForStockItems(ref requsters))
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
            

            requstersFiltered.ForEach(a => rfpRequesters.Items.Add(a.requestor_team.team_name));
        }
        private void FillOrderIDsComboBox()
        {
          
            for(int i=0;i<workOrders.Count;i++) 
            {
                if (workOrders[i].contract_type_id == COMPANY_WORK_MACROS.SUPPLY_ONLY_CONTRACT_TYPE || workOrders[i].contract_type_id == COMPANY_WORK_MACROS.SUPPLY_AND_INSTALL_CONTRACT_TYPE)
                {
                    orderSerials.Items.Add(workOrders[i].order_id);
                }
            
            }
        }
      

        private void BasicInfoLableMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnMouseDownItemsInfoLabel(object sender, MouseButtonEventArgs e)
        {
            if (workFormComboBox.SelectedIndex==-1) {

                System.Windows.Forms.MessageBox.Show("You have to choose rfp or order", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            materialReleasePermit.SetReleaseId(SerialIdTextBox.Text);
            addReleasePermitItem.InitializeStock();
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
            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                materialReleasePermit.SetMaterialReceiver(employees[MaterialRecieverComboBox.SelectedIndex].employee_id);
                materialReleasePermit.SetMaterialReceiverName(employees[MaterialRecieverComboBox.SelectedIndex].employee_name);
            }
           

        }

        //private void rfpCheckedChecked(object sender, RoutedEventArgs e)
        //{
        //    orderChecked.IsChecked = false;
        //    mainBorder.Visibility = Visibility.Visible;

        //    WrapPanel rfpRequestorTeamWrapPanel=mainPanel.Children[0] as WrapPanel;
        //    rfpRequestorTeamWrapPanel.Visibility = Visibility.Visible;

        //    WrapPanel rfpIDWrapPanel = mainPanel.Children[1] as WrapPanel;
        //    rfpIDWrapPanel.Visibility = Visibility.Visible;

        //    WrapPanel orderIdWrapPanel = mainPanel.Children[2] as WrapPanel;
        //    orderIdWrapPanel.Visibility = Visibility.Collapsed;

        //    WrapPanel orderContactWrapPanel = mainPanel.Children[3] as WrapPanel;
        //    orderContactWrapPanel.Visibility = Visibility.Collapsed;

        //    WrapPanel orderEndingChoice = mainPanel.Children[4] as WrapPanel;
        //    orderEndingChoice.Visibility = Visibility.Collapsed;

        //    ComboBox orderSerialsComboBox = orderIdWrapPanel.Children[1] as ComboBox;
        //    orderSerialsComboBox.SelectedIndex = -1;


        //    addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

        //    if (parentWindow.isView == true)
        //    {
        //       // addReleasePermitItem.ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addReleasePermitItem, parentWindow.releasePermitPage, parentWindow, ref parentWindow.materialReleasePermit);
        //    }


        //}

        //private void orderCheckedChecked(object sender, RoutedEventArgs e)
        //{
        //    rfpChecked.IsChecked = false;
        //    mainBorder.Visibility = Visibility.Visible;

        //    WrapPanel rfpRequestorPanel = mainPanel.Children[0] as WrapPanel;
        //    rfpRequestorPanel.Visibility = Visibility.Collapsed;

        //    WrapPanel rfpIDPanel = mainPanel.Children[1] as WrapPanel;
        //    rfpIDPanel.Visibility = Visibility.Collapsed;

        //    WrapPanel orderSerialPanel = mainPanel.Children[2] as WrapPanel;
        //    orderSerialPanel.Visibility = Visibility.Visible;

        //    WrapPanel orderContactPanel = mainPanel.Children[3] as WrapPanel;
        //    orderContactPanel.Visibility = Visibility.Visible;

        //    WrapPanel chooseToBeClosedWith = mainPanel.Children[4] as WrapPanel;
        //    chooseToBeClosedWith.Visibility = Visibility.Visible;


        //    //addReleasePermitItem = new AddReleasePermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, parentWindow);

        //    if (parentWindow.isView == true) {
        //        //addReleasePermitItem.ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, addReleasePermitItem, parentWindow.releasePermitPage, parentWindow, ref parentWindow.materialReleasePermit);
        //    }
        //}

        private void rfpRequestersSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                if (!commonQueries.GetTeamRFPsMappedIds(ref rfps, requstersFiltered[rfpRequesters.SelectedIndex].requestor_team.team_id, COMPANY_WORK_MACROS.RFP_AT_STOCK))
                    return;
                rfpSerials.Items.Clear();

                rfps.ForEach(a => rfpSerials.Items.Add(a.rfpID));
            }

            


        }

        private bool InitializeCompanyContacts()
        {
        
            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                workOrder.InitializeWorkOrderInfo(workOrders[orderSerials.SelectedIndex].order_serial);

                companyContacts.Clear();
                contactComboBox.Items.Clear();
                contactComboBox.IsEnabled = true;
                if (workFormComboBox.SelectedIndex == 1)
                {
                    if (!commonQueries.GetCompanyContacts(workOrder.GetCompanySerial(), ref companyContacts))
                        return false;
                }

                for (int i = 0; i < companyContacts.Count(); i++)
                {
                    contactComboBox.Items.Add(companyContacts[i].contact.contact_name);
                }
                return true;

            }
            return true;
        }

        private void OnSelectionChangedOrderSerial(object sender, SelectionChangedEventArgs e)
        {
            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                serialProducts.Clear();
                workOrder.SetOrderIssueDateToToday();



                ComboBox ordersComboBox = sender as ComboBox;

                if (ordersComboBox.SelectedIndex != -1)
                {
                    InitializeCompanyContacts();

                    workOrder.InitializeWorkOrderInfo(workOrders[ordersComboBox.SelectedIndex].order_serial);

                }

                for (int i = 0; i < workOrder.GetOrderProductsList().Length; i++)
                {


                    if (workOrder.GetOrderProductsList()[i].has_serial_number == true)
                    {
                        int numberOfSerials = 0;
                        workOrder.GetNumOfProductSerialsForAProduct(workOrder.GetOrderSerial(), workOrder.GetOrderProductsList()[i].productNumber, ref numberOfSerials);

                        serialProducts.Add(numberOfSerials);
                    }

                    else
                    {

                        serialProducts.Add(0);

                    }

                }

                materialReleasePermit.SetWorkOrder(workOrder);
                materialReleasePermit.SetRfp(null);
            }
          
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
            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                ComboBox contactsComboBox = sender as ComboBox;

                if (contactsComboBox.SelectedIndex != -1)
                {
                    materialReleasePermit.SetContactId(workOrder.GetContactId());
                    materialReleasePermit.SetBranchSerialId(workOrder.GetAddressSerial());

                    materialReleasePermit.SetSalesPersonId(workOrder.GetSalesPersonId());
                }
                else
                {


                    materialReleasePermit.SetContactId(0);
                }
            }
           
        }
        private void OnSelectionChangedRfpIdComboBox(object sender, SelectionChangedEventArgs e)
        {
        

            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                ComboBox rfpSerialsComboBox = sender as ComboBox;

                serialProducts.Clear();
                rfp.rfpItems.Clear();


                if (rfpSerialsComboBox.SelectedIndex != -1)
                    if (!rfp.InitializeRFP(rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion))
                        return;


                if (!commonQueries.GetRfpItemsMappingMergedItems(rfps[rfpSerialsComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialsComboBox.SelectedIndex].rfpVersion, rfps[rfpSerialsComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems))
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
                for (int i = 0; i < rfpItems.Count; i++)
                {
                    if (rfp.rfpItems.Any(f => f.product_category.category_id == rfpItems[i].product_category.category_id
                                            && f.product_type.type_id == rfpItems[i].product_type.type_id
                                            && f.product_brand.brand_id == rfpItems[i].product_brand.brand_id
                                            && f.product_model.model_id == rfpItems[i].product_model.model_id
                                            && f.product_specs.spec_id == rfpItems[i].product_specs.spec_id))
                    {
                        PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItemmin = new PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT();
                        rfpItemmin = rfpItems[i];
                        rfpItemmin.rfp_item_number = rfp.rfpItems.Find(f => f.product_category.category_id == rfpItems[i].product_category.category_id
                                            && f.product_type.type_id == rfpItems[i].product_type.type_id
                                            && f.product_brand.brand_id == rfpItems[i].product_brand.brand_id
                                            && f.product_model.model_id == rfpItems[i].product_model.model_id
                                            && f.product_specs.spec_id == rfpItems[i].product_specs.spec_id).rfp_item_number;
                        rfpItems.RemoveAt(i);
                        rfpItems.Insert(i, rfpItemmin);
                    }
                }
                materialReleasePermit.SetRfp(rfp);
                materialReleasePermit.SetWorkOrder(null);

                parentWindow.releasePermitItemPage.checkedItemsCounterLabel.Content = "0";
                parentWindow.releasePermitItemPage.checkedItemsCounter = 0;
                parentWindow.releasePermitItemPage.checkedRFPItems.Clear();
                parentWindow.releasePermitItemPage.checkedOrderItems.Clear();
                parentWindow.releasePermitItemPage.selectedItems.Clear();
            }
            

        }
        /// <summary>
        /// /////////////////////////////////// ON UNCHECK CKECKBOX ///////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnUnCheckRFPCheckBox(object sender, RoutedEventArgs e) {

        //    rfpPanel.Visibility = Visibility.Collapsed;
        //    rfpIdPanel.Visibility = Visibility.Collapsed;   
        //    mainBorder.Visibility = Visibility.Collapsed;
        //}

        //private void OnUncheckOrderCheckBox(object sender, RoutedEventArgs e)
        //{

        //    orderPanel.Visibility = Visibility.Collapsed;
        //    orderContactPanel.Visibility=Visibility.Collapsed;
        //    chooseToBeClosedWith.Visibility=Visibility.Collapsed;
        //    serviceReportCheckBox.IsChecked=false;
        //    receivalNoteCheckBox.IsChecked=false;
        //    mainBorder.Visibility=Visibility.Collapsed;
        //}

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
            if(viewAddCondition == COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
               // 
                this.NavigationService.Navigate(parentWindow.releasePermitItemPage);
            }
            else
            {
                if (workFormComboBox.SelectedIndex == -1)
                {

                    System.Windows.Forms.MessageBox.Show("You have to choose rfp or order", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                if (workFormComboBox.SelectedIndex == 1 && serviceReportCheckBox.IsChecked == false && receivalNoteCheckBox.IsChecked == false)
                {
                    System.Windows.Forms.MessageBox.Show("Please choose ending it with receival note or service report", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                materialReleasePermit.SetReleaseId(SerialIdTextBox.Text);

                // addReleasePermitItem.addReleasePermitPage = this;
                parentWindow.releasePermitItemPage.InitializeStock();
                this.NavigationService.Navigate(parentWindow.releasePermitItemPage);
            }
           


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

        private void OnSelectionChangedWorkForm(object sender, SelectionChangedEventArgs e)
        {
            if(workFormComboBox.SelectedIndex !=-1)
            {
                if(workFormComboBox.SelectedIndex==0)
                {
                    rfpPanel.Visibility = Visibility.Visible;
                    rfpIdPanel.Visibility = Visibility.Visible;


                    orderPanel.Visibility = Visibility.Collapsed;
                    orderContactPanel.Visibility = Visibility.Collapsed;
                    chooseToBeClosedWith.Visibility = Visibility.Collapsed;

                }
                else
                {
                    rfpPanel.Visibility = Visibility.Collapsed;
                    rfpIdPanel.Visibility = Visibility.Collapsed;

                    orderPanel.Visibility = Visibility.Visible;
                    orderContactPanel.Visibility = Visibility.Visible;
                    chooseToBeClosedWith.Visibility = Visibility.Visible;
                }
            }

        }
    }
}
