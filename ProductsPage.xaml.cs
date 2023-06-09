using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace _01electronics_inventory
{

    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected FTPServer ftpServer;

        List<Image> productImages = new List<Image>();
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> products;
        protected List<String> productSummaryPoints;
        
        protected Product selectedProduct;
        protected String returnMessage;
        private Expander currentExpander;
        private Expander previousExpander;

        protected int mViewAddCondition;

        public ProductsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;

            ftpServer = new FTPServer();

            InitializeComponent();
            
            products = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            productSummaryPoints = new List<string>();
            selectedProduct = mSelectedProduct;

            InitializeProducts();
            InitializeProductSummaryPoints();
            SetUpPageUIElements();

            //sync.DoWork += SyncDoWork;
            //sync.RunWorkerAsync();


        }

        private void SyncDoWork(object sender, DoWorkEventArgs e)
        {
            //ftpServer.UploadForSynchronization();
        }

        private void InitializeProducts()
        {
            if (!commonQueries.GetCompanyProducts(ref products, selectedProduct.GetCategoryID()))
                return;
        }
        public void InitializeProductSummaryPoints()
        {
            if (!commonQueries.GetProductsSummaryPoints(ref productSummaryPoints, selectedProduct.GetCategoryID()))
                return;
        }

        public void SetUpPageUIElements()
        {
            ProductsGrid.Children.Clear();
            ProductsGrid.RowDefinitions.Clear();
            ProductsGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < products.Count(); i++)
            {
                bool foundImage = true;
                RowDefinition rowI = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI);

                Grid gridI = new Grid();


                RowDefinition imageRow = new RowDefinition();
                gridI.RowDefinitions.Add(imageRow);

                selectedProduct.SetProductID(products[i].type_id);

                Image productImage = new Image();

                BitmapImage src = new BitmapImage();

                src.BeginInit();
                src.UriSource = new Uri(selectedProduct.GetProductPhotoLocalPath(), UriKind.Relative);

                src.CacheOption = BitmapCacheOption.OnLoad;


                if (!File.Exists(selectedProduct.GetProductPhotoLocalPath()))
                {
                    //productImages.Add(productImage);
                    foundImage = false;
                    Border border1 = new Border();
                    border1.BorderThickness = new Thickness(0, 1, 0, 0);
                    BrushConverter converter = new BrushConverter();

                    border1.Background = (Brush)converter.ConvertFrom("#EDEDED");
                    border1.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border1.VerticalAlignment = VerticalAlignment.Stretch;

                    border1.Height = 300;
                    border1.BorderBrush = (Brush)converter.ConvertFrom("#105A97");
                    border1.Tag = products[i].type_id.ToString();
                    border1.MouseLeftButtonDown += BorderMouseLeftButtonDown;
                    gridI.Children.Add(border1);
                    Grid.SetRow(border1, 0);

                }
                else
                {
                    try
                    {
                        src.EndInit();
                    }
                    catch (Exception c)
                    {

                        foundImage = false;
                        Border border1 = new Border();
                        border1.BorderThickness = new Thickness(0, 1, 0, 0);

                        border1.Height = 300;

                        border1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        border1.VerticalAlignment = VerticalAlignment.Stretch;
                        BrushConverter converter = new BrushConverter();
                        border1.BorderBrush = (Brush)converter.ConvertFrom("#105A97");
                        border1.Background = (Brush)converter.ConvertFrom("#EDEDED");
                        border1.Tag = products[i].type_id.ToString();
                        border1.MouseLeftButtonDown += BorderMouseLeftButtonDown;
                        gridI.Children.Add(border1);
                        Grid.SetRow(border1, 0);

                    }
                }

                productImage.Tag = products[i].type_id.ToString();
                productImage.Source = src;
                productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;
                productImage.MouseDown += ImageMouseDown;

                if (foundImage == true)
                {

                    gridI.Children.Add(productImage);
                    Grid.SetRow(productImage, 0);

                }


                Expander expander = new Expander();
                expander.Tag = products[i].type_id.ToString();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                expander.Margin = new Thickness(12);

                StackPanel expanderStackPanel = new StackPanel();
                expanderStackPanel.Orientation = Orientation.Vertical;

                BrushConverter brushConverter = new BrushConverter();

                Button ViewButton = new Button();
                ViewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                ViewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                ViewButton.Click += OnBtnClickViewProduct;
                ViewButton.Content = "View";


                expanderStackPanel.Children.Add(ViewButton);

                expander.Content = expanderStackPanel;

                gridI.Children.Add(expander);

                Grid imageGrid = new Grid();
                imageGrid.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                imageGrid.Width = 350;
                imageGrid.Height = 150;
                imageGrid.Margin = new Thickness(100, -20, 0, 0);
                imageGrid.HorizontalAlignment = HorizontalAlignment.Left;

                RowDefinition headerRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(headerRow);
                headerRow.Height = new GridLength(50);


                RowDefinition pointsRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(pointsRow);

                Grid headerGrid = new Grid();
                RowDefinition headerGridRow = new RowDefinition();
                headerGrid.RowDefinitions.Add(headerGridRow);
                Grid.SetRow(headerGrid, 0);

                TextBlock headerLabel = new TextBlock() { TextWrapping = TextWrapping.WrapWithOverflow };
                headerLabel.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                headerLabel.FontFamily = new FontFamily("Sans Serif");
                headerLabel.FontSize = 17;
                headerLabel.FontWeight = FontWeights.Bold;
                headerLabel.Padding = new Thickness(10);
                headerLabel.Text = products[i].product_name;

                Grid.SetRow(headerLabel, 0);
                headerGrid.Children.Add(headerLabel);
                imageGrid.Children.Add(headerGrid);

                TextBlock pointsTextBlock = new TextBlock();
                pointsTextBlock.Foreground = Brushes.Black;
                pointsTextBlock.TextWrapping = TextWrapping.Wrap;
                pointsTextBlock.FontSize = 15;
                pointsTextBlock.FontStyle = FontStyles.Italic;
                if (i < productSummaryPoints.Count)
                {
                    pointsTextBlock.Text = productSummaryPoints[i];
                }
                pointsTextBlock.Padding = new Thickness(20);

                Grid.SetRow(pointsTextBlock, 1);
                imageGrid.Children.Add(pointsTextBlock);

                gridI.Children.Add(imageGrid);
                Grid.SetRow(imageGrid, 0);
                ProductsGrid.Children.Add(gridI);
                Grid.SetRow(gridI, i);


                //if (i==products.Count-1&&productImages.Count!=0)
                //{
                //    if (background.IsBusy == false)
                //        background.RunWorkerAsync();

                //}
            }

            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.ERP_SYSTEM_DEVELOPMENT_TEAM_ID ||
                loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID ||
                (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
            {
                addBtn.Visibility = Visibility.Visible;
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

        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();

            //Product selectedProduct = new Product();
            selectedProduct.SetProductID(int.Parse(tmp));
            //selectedProduct.SetCategoryID(selectedProduct.GetCategoryID());

            Brand selectedBrand = new Brand();
            selectedBrand.SetProductID(selectedProduct.GetProductID());
            selectedBrand.SetCategoryID(selectedProduct.GetCategoryID());
            selectedBrand.SetProductName(selectedProduct.GetProductName());
            selectedBrand.SetCategoryName(selectedProduct.GetCategoryName());

            BrandsPage brandsPage = new BrandsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedBrand/* ref selectedProduct*/);
            this.NavigationService.Navigate(brandsPage);
        }


        private void BorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border currentBorder = sender as Border;
            String tmp = currentBorder.Tag.ToString();

            //Product selectedProduct = new Product();
            selectedProduct.SetProductID(int.Parse(tmp));
            //selectedProduct.SetCategoryID(selectedProduct.GetCategoryID());

            Brand selectedBrand = new Brand();
            selectedBrand.SetProductID(selectedProduct.GetProductID());
            selectedBrand.SetCategoryID(selectedProduct.GetCategoryID());
            selectedBrand.SetProductName(selectedProduct.GetProductName());
            selectedBrand.SetCategoryName(selectedProduct.GetCategoryName());

            BrandsPage brandsPage = new BrandsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedBrand/* ref selectedProduct*/);
            this.NavigationService.Navigate(brandsPage);
        }

        //private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        //{
        //    StatisticsPage statisticsPage = new StatisticsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
        //    NavigationService.Navigate(statisticsPage);
        //}


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Expander HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            if (currentExpander != null)
                previousExpander = currentExpander;

            currentExpander = (Expander)sender;

            if (previousExpander != currentExpander && previousExpander != null)
                previousExpander.IsExpanded = false;

            Grid currentGrid = (Grid)currentExpander.Parent;

            currentExpander.VerticalAlignment = VerticalAlignment.Top;
        }
        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            currentExpander.Margin = new Thickness(12);
        }
        private void addBtnMouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 1.0;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);
        }
        private void addBtnMouseLeave(object sender, MouseEventArgs e)
        {

            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 0.5;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);

        }
        private void OnBtnClickViewProduct(object sender, RoutedEventArgs e)
        {
            Button viewButton = (Button)sender;
            StackPanel expanderStackPanel = (StackPanel)viewButton.Parent;
            Expander expander = (Expander)expanderStackPanel.Parent;
            selectedProduct.SetProductID(int.Parse(expander.Tag.ToString()));
            selectedProduct.InitializeProductInfo();
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
            AddProductWindow addProductWindow = new AddProductWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedProduct, ref mViewAddCondition);
            addProductWindow.Closed += OnCloseAddPRoductsWindow;
            addProductWindow.Show();
        }
        private void onBtnAddClick(object sender, MouseButtonEventArgs e)
        {
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION;
            AddProductWindow addProductWindow = new AddProductWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref selectedProduct, ref mViewAddCondition);
            addProductWindow.Closed += OnCloseAddPRoductsWindow;
            addProductWindow.Show();
        }
        private void OnCloseAddPRoductsWindow(object sender, EventArgs e)
        {
            products.Clear();

            InitializeProducts();
            InitializeProductSummaryPoints();
            SetUpPageUIElements();
        }
    }
}
