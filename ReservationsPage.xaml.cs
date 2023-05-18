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
using static _01electronics_library.BASIC_STRUCTS;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ReservationsPage.xaml
    /// </summary>
    public partial class ReservationsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected MaterialReservation materialReservation;
        protected List<BASIC_STRUCTS.MATERIAL_RESERVATION_MAX_STRUCT> reservationsList;
        protected int viewAddCondition;

        public ReservationsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            materialReservation = new MaterialReservation();
            reservationsList = new List<BASIC_STRUCTS.MATERIAL_RESERVATION_MAX_STRUCT>();

            InitializeComponent();

            if (!InitializeReservationsList())
                return;

            InitializeReservationsGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INITIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InitializeReservationsList()
        {
            if(!commonQueries.GetReservations(ref reservationsList))
                return false;

            return true;
        }
        private void InitializeReservationsGrid()
        {
            for (int i = reservationsGrid.RowDefinitions.Count() - 1; i > 0; i--)
                reservationsGrid.RowDefinitions.RemoveAt(i);

            for (int i = reservationsGrid.Children.Count - 1; i > 11; i--)
                reservationsGrid.Children.RemoveAt(i);

            for (int i = 0; i < reservationsList.Count(); i++)
            {
                reservationsGrid.RowDefinitions.Add(new RowDefinition());
                int columnCounter = 0;

                CheckBox checkBoxHeader = new CheckBox();
                checkBoxHeader.Style = (Style)FindResource("checkBoxStyle");
                checkBoxHeader.Margin = new Thickness(73,0,0,0);

                TextBlock entryPermitIdHeader = new TextBlock();
                entryPermitIdHeader.Text = reservationsList[i].entry_permit_id;
                entryPermitIdHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock entryPermitItemHeader = new TextBlock();
                entryPermitItemHeader.Style = (Style)FindResource("tableSubItemTextblock");

                if (reservationsList[i].entry_permit_generic_product.category.category_name != String.Empty)                
                    entryPermitItemHeader.Text = reservationsList[i].entry_permit_generic_product.category.category_name + " - " 
                                               + reservationsList[i].entry_permit_generic_product.type.product_name + " - "
                                               + reservationsList[i].entry_permit_generic_product.brand.brand_name + " - "
                                               + reservationsList[i].entry_permit_generic_product.model.model_name;
                else if (reservationsList[i].entry_permit_company_product.category.category != String.Empty)
                    entryPermitItemHeader.Text = reservationsList[i].entry_permit_company_product.category.category + " - "
                                               + reservationsList[i].entry_permit_company_product.product.typeName + " - "
                                               + reservationsList[i].entry_permit_company_product.brand.brandName + " - "
                                               + reservationsList[i].entry_permit_company_product.model.modelName + " - "
                                               + reservationsList[i].entry_permit_company_product.spec.spec_name;

                TextBlock itemDescriptionHeader = new TextBlock();
                itemDescriptionHeader.Style = (Style)FindResource("tableSubItemTextblock");
                
                if(reservationsList[i].rfp_id != String.Empty)
                {
                    if(reservationsList[i].rfp_generic_product.category.category_name != String.Empty)
                        itemDescriptionHeader.Text = reservationsList[i].rfp_generic_product.category.category_name + " - "
                                                   + reservationsList[i].rfp_generic_product.type.product_name + " - "
                                                   + reservationsList[i].rfp_generic_product.brand.brand_name + " - "
                                                   + reservationsList[i].rfp_generic_product.model.model_name;
                    else if (reservationsList[i].rfp_company_product.category.category != String.Empty)
                        itemDescriptionHeader.Text = reservationsList[i].rfp_company_product.category.category + " - "
                                                   + reservationsList[i].rfp_company_product.product.typeName + " - "
                                                   + reservationsList[i].rfp_company_product.brand.brandName + " - "
                                                   + reservationsList[i].rfp_company_product.model.modelName + " - "
                                                   + reservationsList[i].rfp_company_product.spec.spec_name;
                }
                else if (reservationsList[i].order_id != String.Empty)
                {
                    itemDescriptionHeader.Text = reservationsList[i].order_company_product.category.category + " - "
                                               + reservationsList[i].order_company_product.product.typeName + " - "
                                               + reservationsList[i].order_company_product.brand.brandName + " - "
                                               + reservationsList[i].order_company_product.model.modelName + " - "
                                               + reservationsList[i].order_company_product.spec.spec_name;
                }
                else if (reservationsList[i].quotation_id != String.Empty)
                {
                    itemDescriptionHeader.Text = reservationsList[i].quotation_company_product.category.category + " - "
                                               + reservationsList[i].quotation_company_product.product.typeName + " - "
                                               + reservationsList[i].quotation_company_product.brand.brandName + " - "
                                               + reservationsList[i].quotation_company_product.model.modelName + " - "
                                               + reservationsList[i].quotation_company_product.spec.spec_name;
                }

                TextBlock quantityHeader = new TextBlock();
                quantityHeader.Text = reservationsList[i].quantity.ToString();
                quantityHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock serialNumberHeader = new TextBlock();
                
                if(reservationsList[i].serial_number != String.Empty)
                    serialNumberHeader.Text = reservationsList[i].serial_number;
                else
                    serialNumberHeader.Text = "NULL";
                serialNumberHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock workFormHeader = new TextBlock();
                workFormHeader.Text = reservationsList[i].work_form;
                workFormHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock formIdHeader = new TextBlock();
                formIdHeader.Style = (Style)FindResource("tableSubItemTextblock");

                if (reservationsList[i].rfp_id != String.Empty)
                {
                    formIdHeader.Text = reservationsList[i].rfp_id;
                }
                else if (reservationsList[i].order_id != String.Empty)
                {
                    formIdHeader.Text = reservationsList[i].order_id;
                }
                else if (reservationsList[i].quotation_id != String.Empty)
                {
                    formIdHeader.Text = reservationsList[i].quotation_id;
                }

                TextBlock reservationDateHeader = new TextBlock();
                reservationDateHeader.Text = reservationsList[i].reservation_date.ToString("dd-MM-yyyy");
                reservationDateHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock reservedByHeader = new TextBlock();
                reservedByHeader.Text = reservationsList[i].reserved_by_name;
                reservedByHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock holdUntilDateHeader = new TextBlock();
                holdUntilDateHeader.Text = reservationsList[i].hold_until.ToString("dd-MM-yyyy");
                holdUntilDateHeader.Style = (Style)FindResource("tableSubItemTextblock");

                TextBlock statusHeader = new TextBlock();
                statusHeader.Text = reservationsList[i].reserved_status_name;
                statusHeader.Style = (Style)FindResource("tableSubItemTextblock");

                reservationsGrid.Children.Add(checkBoxHeader);
                Grid.SetRow(checkBoxHeader, i + 1);
                Grid.SetColumn(checkBoxHeader, columnCounter++);
                
                reservationsGrid.Children.Add(entryPermitIdHeader);
                Grid.SetRow(entryPermitIdHeader, i + 1);
                Grid.SetColumn(entryPermitIdHeader, columnCounter++);
                
                reservationsGrid.Children.Add(entryPermitItemHeader);
                Grid.SetRow(entryPermitItemHeader, i + 1);
                Grid.SetColumn(entryPermitItemHeader, columnCounter++);
                
                reservationsGrid.Children.Add(itemDescriptionHeader);
                Grid.SetRow(itemDescriptionHeader, i + 1);
                Grid.SetColumn(itemDescriptionHeader, columnCounter++);
                
                reservationsGrid.Children.Add(quantityHeader);
                Grid.SetRow(quantityHeader, i + 1);
                Grid.SetColumn(quantityHeader, columnCounter++);
                
                reservationsGrid.Children.Add(serialNumberHeader);
                Grid.SetRow(serialNumberHeader, i + 1);
                Grid.SetColumn(serialNumberHeader, columnCounter++);

                reservationsGrid.Children.Add(workFormHeader);
                Grid.SetRow(workFormHeader, i + 1);
                Grid.SetColumn(workFormHeader, columnCounter++);

                reservationsGrid.Children.Add(formIdHeader);
                Grid.SetRow(formIdHeader, i + 1);
                Grid.SetColumn(formIdHeader, columnCounter++);

                reservationsGrid.Children.Add(reservationDateHeader);
                Grid.SetRow(reservationDateHeader, i + 1);
                Grid.SetColumn(reservationDateHeader, columnCounter++);

                reservationsGrid.Children.Add(reservedByHeader);
                Grid.SetRow(reservedByHeader, i + 1);
                Grid.SetColumn(reservedByHeader, columnCounter++);
                
                reservationsGrid.Children.Add(holdUntilDateHeader);
                Grid.SetRow(holdUntilDateHeader, i + 1);
                Grid.SetColumn(holdUntilDateHeader, columnCounter++);
                
                reservationsGrid.Children.Add(statusHeader);
                Grid.SetRow(statusHeader, i + 1);
                Grid.SetColumn(statusHeader, columnCounter++);
            }

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

            EntryPermitPage entryPermitPage = new EntryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);

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

        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            viewAddCondition = COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION;

            AddReservationWindow addReservationWindow = new AddReservationWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref materialReservation,  viewAddCondition);
            addReservationWindow.Show();
            addReservationWindow.Closed += OnCloseAddReservationWindow;
        }

        private void OnCloseAddReservationWindow(object sender, EventArgs e)
        {

        }
    }
}
