using _01electronics_library;
using Spire.Doc.Fields;
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
    /// Interaction logic for AddReservationBasicInfoPage.xaml
    /// </summary>
    public partial class AddReservationBasicInfoPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public RFP selectedRFP;
        public WorkOrder selectedWorkOrder;
        public Quotation selectedQuotation;

        protected int viewAddCondition;

        protected MaterialReservation materialReservation;

        public List<int> serialProducts = new List<int>();

        private List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT> rfps;
        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpItems;
        private List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrders;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> rfpRequestors;
        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT> selectedRFPItems;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> rfpRequestorTeams;
        private List<SALES_STRUCTS.QUOTATION_MAX_STRUCT> outgoingQuotationsList;

        public AddReservationItemsPage addReservationItemsPage;

        public AddReservationBasicInfoPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref MaterialReservation mMaterialReservation, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            materialReservation = mMaterialReservation;
            viewAddCondition = mViewAddCondition;

            rfps = new List<PROCUREMENT_STRUCTS.RFP_MIN_STRUCT>();
            rfpItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
            workOrders = new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
            rfpRequestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            rfpRequestorTeams = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            outgoingQuotationsList = new List<SALES_STRUCTS.QUOTATION_MAX_STRUCT>();

            reservationDatePicker.SelectedDate = commonFunctions.GetTodaysDate();
            holdUntilDatePicker.SelectedDate = commonFunctions.GetTodaysDate();

            if (!InitializeRFPTeams())
                return;
            
            if(!InitializeWorkOrders())
                return;

            if(!InitializeQuotations())
                return;

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INITIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InitializeRFPTeams()
        {
            rfpRequestors.Clear();

            if (!commonQueries.GetRFPRequestors(ref rfpRequestorTeams))
                return false;

            for (int i = 0; i < rfpRequestorTeams.Count; i++)
            {
                if (i != 0 && rfpRequestorTeams[i].requestor_team.team_name != rfpRequestorTeams[i - 1].requestor_team.team_name)
                {
                    rfpTeamComboBox.Items.Add(rfpRequestorTeams[i].requestor_team.team_name);
                    rfpRequestors.Add(rfpRequestorTeams[i]);
                }
                else if (i == 0)
                {
                    rfpTeamComboBox.Items.Add(rfpRequestorTeams[i].requestor_team.team_name);
                    rfpRequestors.Add(rfpRequestorTeams[i]);
                }
            }

            return true;
        }
        private bool InitializeRFPIDs(int teamId)
        {
            rfps.Clear();
            rfpSerialComboBox.Items.Clear();

            if (!commonQueries.GetTeamRFPs(ref rfps, teamId))
                return false;

            for (int i = 0; i < rfps.Count; i++)
            {
                rfpSerialComboBox.Items.Add(rfps[i].rfpID);
            }
            return true;
        }
        private bool InitializeWorkOrders()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return false;

            return true;
        }
        private bool InitializeQuotations()
        {
            if (!commonQueries.GetOutgoingQuotations(ref outgoingQuotationsList))
                return false;

            return true;
        }

        private void FillWorkOrdersComboBox()
        {
            orderQuotationComboBox.Items.Clear();

            for (int i = 0; i < workOrders.Count(); i++)
            {
                orderQuotationComboBox.Items.Add(workOrders[i].order_id);
            }
        }
        private void FillQuotationsComboBox()
        {
            orderQuotationComboBox.Items.Clear();

            for (int i = 0; i < outgoingQuotationsList.Count(); i++)
            {
                orderQuotationComboBox.Items.Add(outgoingQuotationsList[i].offer_id);             
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON BUTTON CLICK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            addReservationItemsPage.addReservationBasicInfoPage = this;
            NavigationService.Navigate(addReservationItemsPage);
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow parent = this.Parent as NavigationWindow;
            parent.Close();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON MOUSE LEFT BUTTON DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseLeftButtonDownBasicInfoLabel(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnMouseLeftButtonDownItemsInfoLabel(object sender, MouseButtonEventArgs e)
        {
            addReservationItemsPage.addReservationBasicInfoPage = this;
            NavigationService.Navigate(addReservationItemsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTION CHANGE DATE PICKERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelDateChangedReservationDate(object sender, SelectionChangedEventArgs e)
        {
            if (reservationDatePicker.SelectedDate != null)
                materialReservation.SetReservationDate((DateTime)reservationDatePicker.SelectedDate);
        }
        private void OnSelDateChangedHoldUntilDate(object sender, SelectionChangedEventArgs e)
        {
            if (holdUntilDatePicker.SelectedDate != null)
                materialReservation.SetHoldUntil((DateTime)holdUntilDatePicker.SelectedDate);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON CHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckRFP(object sender, RoutedEventArgs e)
        {
            workOrderCheckBox.IsChecked = false;
            quotationsCheckBox.IsChecked = false;

            orderQuotationComboBox.IsEnabled = false;

            rfpTeamComboBox.IsEnabled = true;
            rfpTeamComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            rfpTeamComboBox.Visibility = Visibility.Visible;
            rfpSerialComboBox.Visibility = Visibility.Visible;
            orderQuotationComboBox.Visibility = Visibility.Collapsed;

        }
        private void OnCheckWorkOrder(object sender, RoutedEventArgs e)
        {
            RFPCheckBox.IsChecked = false;
            quotationsCheckBox.IsChecked = false;

            orderQuotationComboBox.IsEnabled = true;

            FillWorkOrdersComboBox();
        }
        private void OnCheckQuotation(object sender, RoutedEventArgs e)
        {
            RFPCheckBox.IsChecked = false;
            workOrderCheckBox.IsChecked = false;

            orderQuotationComboBox.IsEnabled = true;

            FillQuotationsComboBox();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON UN CHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUncheckRFP(object sender, RoutedEventArgs e)
        {
            rfpTeamComboBox.IsEnabled = false;
            rfpSerialComboBox.IsEnabled = false;
            rfpSerialComboBox.Items.Clear();

            rfpTeamComboBox.Text = "RFP Team";
            rfpSerialComboBox.Text = "RFP ID";

            rfpSerialComboBox.Foreground = new SolidColorBrush(Colors.LightGray);
            rfpTeamComboBox.Foreground = new SolidColorBrush(Colors.LightGray);

            rfpTeamComboBox.Visibility = Visibility.Collapsed;
            rfpSerialComboBox.Visibility = Visibility.Collapsed;

            orderQuotationComboBox.Visibility = Visibility.Visible;
        }
        private void OnUncheckWorkOrder(object sender, RoutedEventArgs e)
        {
            orderQuotationComboBox.Items.Clear();

            orderQuotationComboBox.IsEnabled = false;
        }
        private void OnUncheckQuotation(object sender, RoutedEventArgs e)
        {
            orderQuotationComboBox.Items.Clear();

            orderQuotationComboBox.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTION CHANGE COMBO BOXES
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedRFPTeamComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (rfpTeamComboBox.SelectedIndex != -1)
            {
                if (!InitializeRFPIDs(rfpRequestors[rfpTeamComboBox.SelectedIndex].requestor_team.team_id))
                    return;

                rfpSerialComboBox.Text = "RFP ID";
                rfpSerialComboBox.IsEnabled = true;
                rfpSerialComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            else
            {
                rfpSerialComboBox.Items.Clear();
                rfpSerialComboBox.Text = "RFP ID";
                rfpSerialComboBox.IsEnabled = false;
                rfpSerialComboBox.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }
        private void OnSelChangedRFPIdComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (rfpSerialComboBox.SelectedIndex != -1)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION)
                {
                    selectedWorkOrder = null;
                    selectedQuotation = null;

                    selectedRFP = new RFP();
                    selectedRFPItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT>();

                    
                    if (!selectedRFP.InitializeRFP(rfps[rfpSerialComboBox.SelectedIndex].rfpRequestorTeam, rfps[rfpSerialComboBox.SelectedIndex].rfpSerial, rfps[rfpSerialComboBox.SelectedIndex].rfpVersion))
                        return;
                    selectedRFP.GetRFPItemList(ref selectedRFPItems);

                    materialReservation.SetRfpRequseterTeamId(rfps[rfpSerialComboBox.SelectedIndex].rfpRequestorTeam);
                    materialReservation.SetRfpSerial(rfps[rfpSerialComboBox.SelectedIndex].rfpSerial);
                    materialReservation.SetRfpVersion(rfps[rfpSerialComboBox.SelectedIndex].rfpVersion);

                    if (!commonQueries.GetRfpItemsMapping(rfps[rfpSerialComboBox.SelectedIndex].rfpSerial,
                                                         rfps[rfpSerialComboBox.SelectedIndex].rfpVersion,
                                                         rfps[rfpSerialComboBox.SelectedIndex].rfpRequestorTeam, ref rfpItems))
                        return;
                    
                    for (int i = 0; i < rfpItems.Count; i++)
                    {
                        if (rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_INVENTORY_REVISED && rfpItems[i].item_status.status_id != COMPANY_WORK_MACROS.RFP_AT_STOCK)
                        {
                            rfpItems.Remove(rfpItems[i]);
                            i--;
                        }
                    }

                    itemsBorder.IsEnabled = true;
                    addReservationItemsPage.ClearItemsPage();
                    for (int i = 0; i < rfpItems.Count; i++)
                    {
                        if (rfpItems[i].is_company_product)
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
                }
            }
            else
            {
                addReservationItemsPage.ClearItemsPage();
                itemsBorder.IsEnabled = false;
            }
        }
        private void OnSelChangeOrderQuotationComboBox(object sender, SelectionChangedEventArgs e)
        {
            serialProducts.Clear();
            if (workOrderCheckBox.IsChecked == true && orderQuotationComboBox.SelectedIndex != -1)
            {
                selectedRFP = null;
                selectedQuotation = null;

                selectedWorkOrder = new WorkOrder();

                if (!selectedWorkOrder.InitializeWorkOrderInfo(workOrders[orderQuotationComboBox.SelectedIndex].order_serial))
                    return;

                materialReservation.SetOrderSerial(workOrders[orderQuotationComboBox.SelectedIndex].order_serial);

                itemsBorder.IsEnabled = true;
                addReservationItemsPage.ClearItemsPage();
                for (int i = 0; i < selectedWorkOrder.GetOrderProductsList().Length; i++)
                {
                    if (selectedWorkOrder.GetOrderProductsList()[i].has_serial_number == true)
                    {
                        int numberOfSerials = 0;
                        selectedWorkOrder.GetNumOfProductSerialsForAProduct(selectedWorkOrder.GetOrderSerial(), selectedWorkOrder.GetOrderProductsList()[i].productNumber, ref numberOfSerials);
                        serialProducts.Add(numberOfSerials);
                    }

                    else
                    {
                        serialProducts.Add(0);
                    }

                }
            }
            else if (quotationsCheckBox.IsChecked == true && orderQuotationComboBox.SelectedIndex != -1)
            {
                selectedRFP = null;
                selectedWorkOrder = null;

                selectedQuotation = new Quotation();

                if (!selectedQuotation.InitializeWorkOfferInfo(outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_serial,
                                                               outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_version,
                                                               outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_proposer_id))
                    return;

                materialReservation.SetQuotationOfferSerial(outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_serial);
                materialReservation.SetQuotationOfferVersion(outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_version);
                materialReservation.SetQuotationOfferProposer(outgoingQuotationsList[orderQuotationComboBox.SelectedIndex].offer_proposer_id);

                itemsBorder.IsEnabled = true;
                addReservationItemsPage.ClearItemsPage();
                for (int i = 0; i < selectedQuotation.GetOfferProductsList().Length; i++)
                {
                    if (selectedQuotation.GetOfferProductsList()[i].has_serial_number == true)
                    {
                        int numberOfSerials = 0;
                        selectedQuotation.GetNumOfAvailableQuantityForAProduct(selectedQuotation.GetOfferProposerId(),
                                                                               selectedQuotation.GetOfferSerial(),
                                                                               selectedQuotation.GetOfferVersion(),
                                                                               selectedQuotation.GetOfferProductsList()[i].productNumber, ref numberOfSerials);
                        serialProducts.Add(numberOfSerials);
                    }
                    else
                    {
                        serialProducts.Add(0);
                    }

                }
            }
            else if (orderQuotationComboBox.SelectedIndex == -1)
            {
                addReservationItemsPage.ClearItemsPage();
                itemsBorder.IsEnabled = false;
            }
        }
    }
}
