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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for StockAvailabilityPage.xaml
    /// </summary>
    public partial class StockAvailabilityPage : System.Windows.Controls.Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        Stock stock;

        int position;
        int rowCount;

        public StockAvailabilityPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();
            
            stock = new Stock();
            position = 0;
            rowCount = 1;

            InitializationFunction();
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
            ReservationsPage reservationsPage = new ReservationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(reservationsPage);
        }
        private void OnButtonClickedRecievalNotes(object sender, MouseButtonEventArgs e)
        {
            RecievalNotePage recievalNotePage = new RecievalNotePage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(recievalNotePage);
        }
        ////////////////////////////////////// INITIALIZATION FUNCTION /////////////////////
        private void InitializationFunction()
        {
            //stock.SetStockAvailabilityList(ref stock.GetStockAvailabiltyList());
            FillStockAvailability();
        }
        private void FillStockAvailability()
        {
            if (!stock.GetAllEntryPermitsItems())
               return;
            else
            {
                FillStockStackPanel();
            }
        }
        private void FillStockStackPanel()
        {
            stockStackPanel.Margin = new Thickness(30, 0, 0, 0);
            stockStackPanel.Children.Clear();
            ////////////// HEADER IS INCLUDED IM ROWS COUNT /////////////////////////
            Grid grid = CreateTable(12, stock.GetStockAvailabiltyList().Count + 1);
            GridHeader("LOCATION", 0, grid);
            GridHeader("STOCK CATEGORY", 1, grid);
            GridHeader("CATEGORY", 2, grid);
            GridHeader("PRODUCT", 3, grid);
            GridHeader("BRAND", 4, grid);
            GridHeader("MODEL", 5, grid);
            GridHeader("Specs", 6, grid);

            GridHeader("AVAILABLE QUANTITY", 7, grid);
            //GridHeader("RELEASED QUANTITY", 11, grid);
            GridHeader("RESERVED QUANTITY", 8, grid);
            rowCount = 1;
            for(int i=0;i<stock.GetStockAvailabiltyList().Count;i++)
            {
                if(rowCount%2==0)
                {
                    GridEvenRows(0, rowCount, stock.GetStockAvailabiltyList()[i].ware_house_location.location_nick_name, stock.GetStockAvailabiltyList()[i].ware_house_location.location_id, grid);
                    GridEvenRows(1, rowCount, stock.GetStockAvailabiltyList()[i].stock_category_name, stock.GetStockAvailabiltyList()[i].stock_category_id, grid);

                    if (stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name != "")
                    {

                        GridEvenRows(2, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_id, grid);
                        GridEvenRows(3, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.product_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.type_id, grid);
                        GridEvenRows(4, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_id, grid);
                        GridEvenRows(5, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_id, grid);

                    }

                    else {

                        GridEvenRows(2, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_name, 0, grid);
                        GridEvenRows(3, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.product_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.type_id, grid);
                        GridEvenRows(4, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_id, grid);
                        GridEvenRows(5, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_id, grid);

                    }

                    if (stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name == null || stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name == "")
                    {
                        
                            INVENTORY_STRUCTS.STOCK_AVAILABILITY item = new INVENTORY_STRUCTS.STOCK_AVAILABILITY();
                        item = stock.GetStockAvailabiltyList()[i];
                        item.entry_permit_item.product_specs.spec_name = "NULL";
                        stock.GetStockAvailabiltyList()[i] = item;

                    }

                    GridEvenRows(6, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_id, grid);

                    int availableQuantity = stock.GetStockAvailabiltyList()[i].entry_permit_item.quantity; /*- (stock.GetStockAvailabiltyList()[i].entry_permit_item.released_quantity + stock.GetStockAvailabiltyList()[i].material_reservation.quantity);*/
                    GridEvenRows(7, rowCount, availableQuantity.ToString(), 0, grid);
                    //GridEvenRows(11, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.released_quantity.ToString(),0, grid);
                    GridEvenRows(8, rowCount, stock.GetStockAvailabiltyList()[i].material_reservation.quantity.ToString(), 0, grid);
                }
                else
                {
                    GridOddRows(0, rowCount, stock.GetStockAvailabiltyList()[i].ware_house_location.location_nick_name, stock.GetStockAvailabiltyList()[i].ware_house_location.location_id, grid);
                    GridOddRows(1, rowCount, stock.GetStockAvailabiltyList()[i].stock_category_name, stock.GetStockAvailabiltyList()[i].stock_category_id, grid);

                    if (stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name != "")
                    {

                        GridOddRows(2, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_id, grid);
                        GridOddRows(3, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.product_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.type_id, grid);
                        GridOddRows(4, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_id, grid);
                        GridOddRows(5, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_id, grid);

                    }

                    else {

                        GridOddRows(2, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_category.category_name, 0, grid);
                        GridOddRows(3, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.product_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_type.type_id, grid);
                        GridOddRows(4, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_brand.brand_id, grid);
                        GridOddRows(5, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_model.model_id, grid);


                    }

                    if (stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name == null|| stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name=="") {
                        INVENTORY_STRUCTS.STOCK_AVAILABILITY item = new INVENTORY_STRUCTS.STOCK_AVAILABILITY();
                        item = stock.GetStockAvailabiltyList()[i];
                        item.entry_permit_item.product_specs.spec_name = "NULL";
                        stock.GetStockAvailabiltyList()[i] = item;
                    }

                    GridOddRows(6, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_name, stock.GetStockAvailabiltyList()[i].entry_permit_item.product_specs.spec_id, grid);


                    int availableQuantity = stock.GetStockAvailabiltyList()[i].entry_permit_item.quantity; /*- (stock.GetStockAvailabiltyList()[i].entry_permit_item.released_quantity + stock.GetStockAvailabiltyList()[i].material_reservation.quantity);*/
                    GridOddRows(7, rowCount, availableQuantity.ToString(), 0, grid);
                    //GridOddRows(11, rowCount, stock.GetStockAvailabiltyList()[i].entry_permit_item.released_quantity.ToString(), 0, grid);
                    GridOddRows(8, rowCount, stock.GetStockAvailabiltyList()[i].material_reservation.quantity.ToString(), 0, grid);
                }
                rowCount++;
            }
            stockStackPanel.Children.Add(grid);

        }
        private Grid CreateTable(int columns, int rows)
        {
            Grid grid = new Grid();
            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
            return grid;
        }
        private void GridHeader(string content, int column, Grid grid)
        {

            Border gridHeader = new Border();
            gridHeader.Tag = position++;
            gridHeader.Style = (Style)FindResource("borderCard5");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            gridHeader.Effect = newDropShadowEffect;

            TextBlock header = new TextBlock();
            header.Text = content;
            header.Style = (Style)FindResource("labelStyleCard2");

            gridHeader.Child = header;
            Grid.SetRow(gridHeader, 0);
            Grid.SetColumn(gridHeader, column);
            grid.Children.Add(gridHeader);
        }
        private void GridEvenRows(int column, int row, string content, int contentId, Grid grid)
        {
            Grid smallGrid = new Grid();
            Border gridHeader = new Border();
            gridHeader.Style = (Style)FindResource("EvenRow");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            gridHeader.Effect = newDropShadowEffect;
            gridHeader.Child = smallGrid;
            TextBlock header = new TextBlock();
            header.Text = content;
            header.Style = (Style)FindResource("labelStyleCardEven");
            header.Tag = column;
            smallGrid.Tag = contentId;
            smallGrid.Children.Add(header);
            gridHeader.Child = smallGrid;
            Grid.SetRow(gridHeader, row);
            Grid.SetColumn(gridHeader, column);
            grid.Children.Add(gridHeader);
        }
        private void GridOddRows(int column, int row, string content, int contentId, Grid grid)
        {
            Grid smallGrid = new Grid();
            Border gridHeader = new Border();
            gridHeader.Style = (Style)FindResource("OddRow");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            gridHeader.Effect = newDropShadowEffect;
            gridHeader.Child = smallGrid;
            TextBlock header = new TextBlock();
            header.Text = content;
            header.Style = (Style)FindResource("labelStyleCardOdd");
            header.Tag = column;
            smallGrid.Tag = contentId;
            smallGrid.Children.Add(header);
            gridHeader.Child = smallGrid;
            Grid.SetRow(gridHeader, row);
            Grid.SetColumn(gridHeader, column);
            grid.Children.Add(gridHeader);
        }

    }
}
