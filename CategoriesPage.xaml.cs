using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected FTPServer ftpServer;

        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories;
        protected List<String> categoriesSummaryPoints;
        
        public CategoriesPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            ftpServer = new FTPServer();

            InitializeComponent();

            categories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
            categoriesSummaryPoints = new List<string>();
            //categoriesSummaryPoints = new List<string>();

            InitializeCategories();
            InitializeProductSummaryPoints();
            SetUpPageUIElements();
        }
        private void InitializeCategories()
        {
            if (!commonQueries.GetProductCategories(ref categories))
                return;
        }

        public void InitializeProductSummaryPoints()
        {
            categoriesSummaryPoints.Add("A non-interruptible clean and stabilized form of power to protect your industry machines data centers and all your electrical devices.");
            categoriesSummaryPoints.Add("The generating set of our PRO range covers your emergency needs within the world-wide standards of electrical of supplies.");
            categoriesSummaryPoints.Add("Rechargeable battery first invented in 1859 by French physicist Gaston Planté");
            categoriesSummaryPoints.Add("The main component of an electrical distribution system that divides electrical power to the branch circuits while providing protection devices for each circuit in a commonQueries enclosure.");
            categoriesSummaryPoints.Add("Protection devices are installed with the aims of protection of assets and ensuring continued supply of energy.");
            categoriesSummaryPoints.Add("An electrical safety device that quickly breaks an electrical circuit with leakage current to ground.");
            categoriesSummaryPoints.Add("Enterprise resource planning (ERP) consists of technologies and systems companies use to manage and integrate their core business processes.");
        }
        public void SetUpPageUIElements()
        {
            for (int i = 0; i < categories.Count(); i++)
            {

                RowDefinition rowI = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI);
                RowDefinition rowI1 = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI1);

                Grid gridI = new Grid();

                RowDefinition imageRow = new RowDefinition();
                gridI.RowDefinitions.Add(imageRow);

                Image productImage = new Image();

                string src = String.Format(@"/01electronics_inventory;component/Photos/categories/" + categories[i].categoryId + ".jpg");
                productImage.Source = new BitmapImage(new Uri(src, UriKind.Relative));
                productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;
                productImage.MouseDown += ImageMouseDown;
                productImage.Tag = categories[i].categoryId.ToString();
                gridI.Children.Add(productImage);
                Grid.SetRow(productImage, 1);

                Grid imageGrid = new Grid();
                imageGrid.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                imageGrid.Width = 350;
                imageGrid.Height = 150;
                imageGrid.Margin = new Thickness(100, -20, 0, 0);
                imageGrid.HorizontalAlignment = HorizontalAlignment.Left;

                RowDefinition headerRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(headerRow);
                headerRow.Height = new GridLength(40);


                RowDefinition pointsRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(pointsRow);

                Grid headerGrid = new Grid();
                RowDefinition headerGridRow = new RowDefinition();
                headerGrid.RowDefinitions.Add(headerGridRow);
                Grid.SetRow(headerGrid, 0);

                Label headerLabel = new Label();
                headerLabel.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                headerLabel.FontFamily = new FontFamily("Sans Serif");
                headerLabel.FontSize = 17;
                headerLabel.FontWeight = FontWeights.Bold;
                headerLabel.Padding = new Thickness(10);
                headerLabel.Content = categories[i].category;

                Grid.SetRow(headerLabel, 0);
                headerGrid.Children.Add(headerLabel);
                imageGrid.Children.Add(headerGrid);

                TextBlock pointsTextBlock = new TextBlock();
                pointsTextBlock.Foreground = Brushes.Black;
                pointsTextBlock.TextWrapping = TextWrapping.Wrap;
                pointsTextBlock.FontSize = 15;
                pointsTextBlock.FontStyle = FontStyles.Italic;
                if (i < categoriesSummaryPoints.Count)
                {
                    pointsTextBlock.Text = categoriesSummaryPoints[i];
                }
                pointsTextBlock.Padding = new Thickness(20);

                Grid.SetRow(pointsTextBlock, 1);
                imageGrid.Children.Add(pointsTextBlock);

                gridI.Children.Add(imageGrid);
                Grid.SetRow(imageGrid, 0);
                ProductsGrid.Children.Add(gridI);
                Grid.SetRow(gridI, i);
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
            ReservationsPage reservationsPage = new ReservationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(reservationsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();

            Product selectedProduct = new Product();
            selectedProduct.SetCategoryID(int.Parse(tmp));

            ProductsPage productsPage = new ProductsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(productsPage);
        }
        //private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        //{
        //    StatisticsPage statisticsPage = new StatisticsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
        //    NavigationService.Navigate(statisticsPage);
        //}
    }
}
