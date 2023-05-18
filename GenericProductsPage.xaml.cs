using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for ViewGenericProductsPage.xaml
    /// </summary>
    public partial class GenericProductsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> categoryList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> typeList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> brandList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL> modelList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS> genericProducts;
        List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> unitList;
        List<BASIC_STRUCTS.PRICING_CRITERIA> pricingCriteriaList;

        const int CATEGORY_COLUMN=0;
        const int TYPE_COLUMN=1;
        const int BRAND_COLUMN=2;
        const int MODEL_COLUMN=3;
        const int ITEM_UNIT_COLUMN=4;
        const int PRICING_CRITERIA_COLUMN=5;
        const int HAS_SERIAL_NUMBER = 6;
        int position;
        int rowCount;
        int newRowsAdded;
        GenericModel genericModel;

        int enterFunctionOnce;
        int enterFunctionOnce2;
        BASIC_STRUCTS.SORT_GENERIC_PRODUCTS sortGenericProducts;

        public GenericProductsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            categoryList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY>();
            typeList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS>();
            brandList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();
            modelList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL>();
            genericProducts = new List<BASIC_STRUCTS.GENERIC_PRODUCTS>();
            unitList = new List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT>();
            pricingCriteriaList = new List<BASIC_STRUCTS.PRICING_CRITERIA>();
            genericModel = new GenericModel();
            position = 0;
            rowCount = 1;
            newRowsAdded = 0;
            integrityChecks = new IntegrityChecks();
            enterFunctionOnce = 1;
            enterFunctionOnce2 = 1;
            genericModel.SetAddedBy(loggedInUser.GetEmployeeId());

            InitializeComponent();

            
            sortGenericProducts = new BASIC_STRUCTS.SORT_GENERIC_PRODUCTS();
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

        ////////////////////////////// INITIALIZATION FUNCTION //////////////////////////
        private void InitializationFunction()
        {
            if (!commonQueries.GetMeasureUnits(ref unitList))
            {
                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            if (!commonQueries.GetPricingCriteria(ref pricingCriteriaList))
            {
                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            FillCategoryList();
            FillCategoryStackPanel(categoryList);
        }
        private void FillCategoryList()
        {
            if (!commonQueries.GetGenericProductCategories(ref categoryList))
                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
        private void FillCategoryStackPanel(List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> categoryList)
        {
            viewCategoryStackPanel.Children.Clear();
            for (int i = 0; i < categoryList.Count; i++)
            {
                CreateCategoryGrid(categoryList[i]);
            }
        }
        private void CreateCategoryGrid(BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY category)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("borderCard1");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            border.Effect = newDropShadowEffect;

            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.MaxWidth = 200;

            ColumnDefinition column3 = new ColumnDefinition();


            ColumnDefinition column2 = new ColumnDefinition();
            column2.MaxWidth = 20;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column2);


            FillCtegoryGrid(grid, category);
            border.Child = grid;
            viewCategoryStackPanel.Children.Add(border);

        }
        private void FillCtegoryGrid(Grid grid, BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY category)
        {
            Label categoryName = new Label();
            categoryName.Style = (Style)FindResource("GridItem");
            categoryName.Content = category.category_name;
            grid.Tag = 0;
            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
            imgSource.EndInit();
            System.Windows.Controls.Image arrowDown = new System.Windows.Controls.Image();
            arrowDown.Source = imgSource;
            arrowDown.Tag = category.category_id.ToString()+'/'+"c";
            arrowDown.MouseDown += OnButtonClickedImageArrowDownCategory;


            Grid.SetColumn(categoryName, 0);
            grid.Children.Add(categoryName);

            Grid.SetColumn(arrowDown, 2);
            grid.Children.Add(arrowDown);

        }
        private void OnButtonClickedImageArrowDownCategory(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image arrowDown = (System.Windows.Controls.Image)sender;
            Grid parent = arrowDown.Parent as Grid;
            int categoryExpanded = Int32.Parse(parent.Tag.ToString());
            if (categoryExpanded==0)
            {
                parent.Tag = 1;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_up.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int categoryID = Int32.Parse(tag[0]);
                FillProductList(categoryID);
                ExpandCategoryListView(typeList, categoryID, tag[1]);
                
            }
            else
            {
                parent.Tag = 0;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int categoryID = Int32.Parse(tag[0]);
                int index = 0;
                for (int i = 0; i<viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tags = img.Tag.ToString().Split('/');
                    if(tags[0] == categoryID.ToString() && tags[1] == "c")
                    {
                        index = i;
                        break;
                    }
                }
                for (int i = index+1; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tags = img.Tag.ToString().Split('/');
                    if ( tags[1] != "c")
                    {
                        viewCategoryStackPanel.Children.RemoveAt(i);
                        i = i - 1;
                        
                    }
                    else
                    {
                        break;
                    }
                    
                }
                
            }

        }
        private void FillProductList(int categoryID)
        {
            if (commonQueries.GetGenericProducts(ref typeList, categoryID));
        }

        private void ExpandCategoryListView(List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> productList , int categoryID, string categorySign)
        {
            int index = 0;
            for(int i=0;i<viewCategoryStackPanel.Children.Count;i++)
            {
                Border border = (Border)viewCategoryStackPanel.Children[i];
                Grid grid = (Grid)border.Child;
                System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                string[] tag = img.Tag.ToString().Split('/');
                if (tag[0]==categoryID.ToString() && tag[1]=="c")
                {
                    index = i;
                    break;
                }
            }
           for(int i=0;i<productList.Count;i++)
            {
                Border border = CreateTypeGrid(productList[i] , categoryID);
                viewCategoryStackPanel.Children.Insert(index+1, border);
                index++;
            }
        }
        private Border CreateTypeGrid(BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS type , int categoryID)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("borderCard2");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            border.Effect = newDropShadowEffect;

            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.MaxWidth = 200;

            ColumnDefinition column3 = new ColumnDefinition();


            ColumnDefinition column2 = new ColumnDefinition();
            column2.MaxWidth = 20;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column2);


            FillTypeGrid(ref grid, type , categoryID);
            border.Child = grid;
            return border;

        }
        private void FillTypeGrid(ref Grid grid, BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS type , int categoryID)
        {
            Label typeName = new Label();
            typeName.Style = (Style)FindResource("GridItem");
            typeName.Content = type.product_name;
            grid.Tag = 0;
            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
            imgSource.EndInit();
            System.Windows.Controls.Image arrowDown = new System.Windows.Controls.Image();
            arrowDown.Source = imgSource;
            arrowDown.Tag = type.product_id.ToString()+'/'+"t"+'/'+categoryID.ToString();
            arrowDown.MouseDown += OnButtonClickedImageArrowDownType;

            Grid.SetColumn(typeName, 0);
            grid.Children.Add(typeName);

            Grid.SetColumn(arrowDown, 2);
            grid.Children.Add(arrowDown);

        }

        private void OnButtonClickedImageArrowDownType(object sender, MouseButtonEventArgs e)
        {
            
            System.Windows.Controls.Image arrowDown = (System.Windows.Controls.Image)sender;
            Grid parent = arrowDown.Parent as Grid;
            int typeExpanded = Int32.Parse(parent.Tag.ToString());
            if (typeExpanded==0)
            {
                parent.Tag = 1;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_up.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int typeID = Int32.Parse(tag[0]);
                int categoryID = Int32.Parse(tag[2]);
                FillBrandList(categoryID, typeID);
                ExpandTypeListView(brandList, categoryID, typeID);
            }
            else
            {
                parent.Tag = 0;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int typeID = Int32.Parse(tag[0]);
                int categoryID = Int32.Parse(tag[2]);
                int index = 0;
                for (int i = 0; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];

                    string[] tags = img.Tag.ToString().Split('/');
                    if (tags[1]=="t")
                    if (tags[2]==categoryID.ToString())
                    if (tags[0] == typeID.ToString())
                    {
                        index = i;
                        break;
                    }
                }
                for (int i = index + 1; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tags = img.Tag.ToString().Split('/');
                    if (tags[1]!="c")
                    {
                        if (tags[1] != "t")
                        {
                            viewCategoryStackPanel.Children.RemoveAt(i);
                            i = i - 1;
                        }

                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
        private void FillBrandList(int categoryID, int productID)
        {
            if (commonQueries.GetGenericProductBrands(productID,categoryID,ref brandList));
        }

        private void ExpandTypeListView(List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> brandList, int categoryID,int productID)
        {
            int index = 0;
            for (int i = 0; i < viewCategoryStackPanel.Children.Count; i++)
            {
                Border border = (Border)viewCategoryStackPanel.Children[i];
                Grid grid = (Grid)border.Child;
                System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                string[] tag = img.Tag.ToString().Split('/');
                if (tag[0] == productID.ToString() && tag[1] == "t" && tag[2]==categoryID.ToString())
                {
                    index = i;
                    break;
                }
            }
            for (int i = 0; i < brandList.Count; i++)
            {
                Border border = CreateBrandGrid(brandList[i], categoryID , productID);
                viewCategoryStackPanel.Children.Insert(index + 1, border);
                index++;
            }
        }

        private Border CreateBrandGrid(BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND brand, int categoryID , int productID)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("borderCard3");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            border.Effect = newDropShadowEffect;

            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.MaxWidth = 200;

            ColumnDefinition column3 = new ColumnDefinition();


            ColumnDefinition column2 = new ColumnDefinition();
            column2.MaxWidth = 20;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column2);


            FillBrandGrid(ref grid, brand, categoryID , productID );
            border.Child = grid;
            return border;

        }
        private void FillBrandGrid(ref Grid grid, BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND brand, int categoryID , int productID)
        {
            Label brandName = new Label();
            brandName.Style = (Style)FindResource("GridItem");
            brandName.Content = brand.brand_name;
            grid.Tag = 0;
            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
            imgSource.EndInit();
            System.Windows.Controls.Image arrowDown = new System.Windows.Controls.Image();
            arrowDown.Source = imgSource;
            arrowDown.Tag = brand.brand_id.ToString() + '/' + "b" + '/' + categoryID.ToString() + '/'+productID.ToString();
            arrowDown.MouseDown += OnButtonClickedImageArrowDownBrand;

            Grid.SetColumn(brandName, 0);
            grid.Children.Add(brandName);

            Grid.SetColumn(arrowDown, 2);
            grid.Children.Add(arrowDown);

        }
        private void OnButtonClickedImageArrowDownBrand(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image arrowDown = (System.Windows.Controls.Image)sender;
            Grid parent = arrowDown.Parent as Grid;
            int brandExpanded = Int32.Parse(parent.Tag.ToString());
            if (brandExpanded == 0)
            {
                parent.Tag = 1;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_up.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int typeID = Int32.Parse(tag[3]);
                int categoryID = Int32.Parse(tag[2]);
                int brandID = Int32.Parse(tag[0]);
                FillModelList(categoryID, typeID, brandID);
                ExpandBrandListView(modelList, categoryID, typeID, brandID);
            }
            else
            {
                parent.Tag = 0;
                BitmapImage imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
                imgSource.EndInit();
                arrowDown.Source = imgSource;
                string[] tag = arrowDown.Tag.ToString().Split('/');
                int typeID = Int32.Parse(tag[3]);
                int categoryID = Int32.Parse(tag[2]);
                int brandID = Int32.Parse(tag[0]);
                int index = 0;
                for (int i = 0; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tags = img.Tag.ToString().Split('/');
                    if (tags[1]=="b")
                    if (tags[2]==categoryID.ToString())
                        if (tags[3]==typeID.ToString())
                    if (tags[0] == brandID.ToString())
                    {
                        index = i;
                        break;
                    }
                }
                for (int i = index + 1; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = viewCategoryStackPanel.Children[i] as Border;
                    Grid grid = border.Child as Grid;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tags = img.Tag.ToString().Split('/');
                    if (tags[1] =="m" )
                    {
                        viewCategoryStackPanel.Children.RemoveAt(i);
                        i = i - 1;

                    }
                    else
                    {
                        break;
                    }

                }

            }
        }
        private void FillModelList(int categoryID, int productID , int brandID)
        {
            if (commonQueries.GetGenericBrandModels(productID, brandID,categoryID, ref modelList)) ;
        }
        private Border CreateModelGrid(BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL model , int categoryID , int typeID , int brandID)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("borderCard4");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            border.Effect = newDropShadowEffect;

            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.MaxWidth = 200;

            ColumnDefinition column3 = new ColumnDefinition();


            ColumnDefinition column2 = new ColumnDefinition();
            column2.MaxWidth = 20;

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column3);
            grid.ColumnDefinitions.Add(column2);


            FillModelGrid(ref grid, model,categoryID,typeID,brandID);
            border.Child = grid;
            return border;

        }
        private void FillModelGrid(ref Grid grid, BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL model, int categoryID, int productID , int brandID)
        {
            Label modelName = new Label();
            modelName.Style = (Style)FindResource("GridItem");
            modelName.Content = model.model_name;

            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(@"Icons\arrow_down.png", UriKind.Relative);
            imgSource.EndInit();
            System.Windows.Controls.Image arrowDown = new System.Windows.Controls.Image();
            arrowDown.Source = imgSource;
            arrowDown.Tag = model.model_id.ToString() + '/' + "m" + '/' + categoryID.ToString() + '/' + productID.ToString()+'/'+brandID.ToString();
            arrowDown.Visibility = Visibility.Collapsed;

            Grid.SetColumn(modelName, 0);
            grid.Children.Add(modelName);

            Grid.SetColumn(arrowDown, 2);
            grid.Children.Add(arrowDown);

        }
        private void ExpandBrandListView(List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL> model , int categoryID, int productID, int brandID)
        {
            
                int index = 0;
                for (int i = 0; i < viewCategoryStackPanel.Children.Count; i++)
                {
                    Border border = (Border)viewCategoryStackPanel.Children[i];
                    Grid grid = (Grid)border.Child;
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)grid.Children[1];
                    string[] tag = img.Tag.ToString().Split('/');
                    if (tag[0] == brandID.ToString() && tag[1] == "b" && tag[2] == categoryID.ToString() && tag[3]==productID.ToString())
                    {
                        index = i;
                        break;
                    }
                }
                for (int i = 0; i < modelList.Count; i++)
                {
                    Border border = CreateModelGrid(modelList[i], categoryID, productID , brandID);
                    viewCategoryStackPanel.Children.Insert(index + 1, border);
                    index++;
                }
            
        }
        private void OnButtonClickedAdd(object sender, RoutedEventArgs e)
        {
            AddGenericProductWindow addGenericProductWindow = new AddGenericProductWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            addGenericProductWindow.Show();
            addGenericProductWindow.Closed += OnCloseAddGenericProductWindow; ;
        }

        private void OnCloseAddGenericProductWindow(object sender, EventArgs e)
        {
            InitializationFunction();
        }

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            viewCategoryStackPanel.Children.Clear();
            ListViewBorder.Style = (Style)FindResource("selectedMainTabBorder"); 
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem"); 

            tableViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            sortButton.Visibility = Visibility.Collapsed;
            InitializationFunction();
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            ListViewBorder.Style = (Style)FindResource("unselectedMainTabBorder");
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            tableViewBorder.Style = (Style)FindResource("selectedMainTabBorder");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            sortButton.Visibility = Visibility.Visible;
            InitializeTableView();
        }

        private void InitializeTableView()
        {
            position = 0;
            viewCategoryStackPanel.Children.Clear();
            genericProducts.Clear();
            Grid grid = new Grid();
            if (!commonQueries.GetGenericProducts(ref genericProducts))
                MessageBox.Show("Error");
           
            if(sortGenericProducts.category==true)
            {
                genericProducts = genericProducts.OrderBy(g1 => g1.category.category_name).ToList<BASIC_STRUCTS.GENERIC_PRODUCTS>();
            
            }
            else if(sortGenericProducts.type==true)
            {
                genericProducts = genericProducts.OrderBy(g1 => g1.type.product_name).ToList<BASIC_STRUCTS.GENERIC_PRODUCTS>();
            }
            else if(sortGenericProducts.brand==true)
            {
                genericProducts = genericProducts.OrderBy(g1 => g1.brand.brand_name).ToList<BASIC_STRUCTS.GENERIC_PRODUCTS>();
            }
            else if(sortGenericProducts.model==true)
            {
                genericProducts = genericProducts.OrderBy(g1 => g1.model.model_name).ToList<BASIC_STRUCTS.GENERIC_PRODUCTS>();
            }
            else if(sortGenericProducts.pricing_criteria==true)
            {
                genericProducts = genericProducts.OrderBy(g1 => g1.model.pricing_criteria.pricing_criteria_name).ToList<BASIC_STRUCTS.GENERIC_PRODUCTS>(); 
            }

            /////////// ELEMENTS +2 FOR THE ADDITIONAL PARTS (HEAD) AND (TAIL) ///////
            grid = CreateTable(8, genericProducts.Count + 1);
            grid.Children.Clear();
            GridHeader("CATEGORY",0,grid);
            GridHeader("TYPE", 1, grid);
            GridHeader("BRAND", 2, grid);
            GridHeader("MODEL", 3, grid);
            GridHeader("ITEM PRICE", 4, grid);
            GridHeader("PRICING CRITERIA", 5, grid);
            GridHeader("HAS SERIAL NUMBER", 6, grid);


            Border plusImgBorder = new Border();
            plusImgBorder.Style = (Style)FindResource("borderCard5");
            System.Windows.Media.Color colors = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffectt = new DropShadowEffect();
            newDropShadowEffectt.BlurRadius = 20;
            newDropShadowEffectt.Color = colors;
            plusImgBorder.Effect = newDropShadowEffectt;

            BitmapImage imgeSource = new BitmapImage();
            imgeSource.BeginInit();
            imgeSource.UriSource = new Uri(@"Icons\plusSign.png", UriKind.Relative);
            imgeSource.EndInit();
            System.Windows.Controls.Image addRow = new System.Windows.Controls.Image();
            addRow.Source = imgeSource;
            addRow.Width = 30;
            addRow.Height = 30;
            addRow.MouseDown += OnMouseDownAddRow;
            addRow.Tag = position++;
            plusImgBorder.Child = addRow;
            Grid.SetColumn(plusImgBorder, 7);
            Grid.SetRow(plusImgBorder, 0);
            grid.Children.Add(plusImgBorder);
            rowCount = 1;
            for (int i=0;i<genericProducts.Count;i++)
            {
                if(rowCount%2==0)
                {
                    GridEvenRows(0, rowCount, genericProducts[i].category.category_name, genericProducts[i].category.category_id,grid);
                    GridEvenRows(1, rowCount, genericProducts[i].type.product_name, genericProducts[i].type.product_id,grid);
                    GridEvenRows(2, rowCount, genericProducts[i].brand.brand_name, genericProducts[i].brand.brand_id, grid);
                    GridEvenRows(3, rowCount, genericProducts[i].model.model_name, genericProducts[i].model.model_id,grid);
                    GridEvenRows(4, rowCount, genericProducts[i].model.item_unit.measure_unit, genericProducts[i].model.item_unit.unit_id, grid);
                    GridEvenRows(5, rowCount, genericProducts[i].model.pricing_criteria.pricing_criteria_name, genericProducts[i].model.pricing_criteria.pricing_criteria_id, grid);
                    if (genericProducts[i].model.has_serial_number)
                         GridEvenRows(6, rowCount,"YES", 0, grid);
                    else
                        GridEvenRows(6, rowCount, "NO", 0, grid);


                    Border imgBorder = new Border();
                    imgBorder.Style = (Style)FindResource("EvenRow");
                    System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                    DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                    newDropShadowEffect.BlurRadius = 20;
                    newDropShadowEffect.Color = color;
                    imgBorder.Effect = newDropShadowEffect;

                    BitmapImage imgSource = new BitmapImage();
                    imgSource.BeginInit();
                    imgSource.UriSource = new Uri(@"Icons\crossRed.png", UriKind.Relative);
                    imgSource.EndInit();
                    System.Windows.Controls.Image deleteRow = new System.Windows.Controls.Image();
                    deleteRow.Source = imgSource;
                    deleteRow.Tag = position++;
                    deleteRow.Width = 25;
                    deleteRow.Height = 25;
                    deleteRow.MouseDown += OnButtonClickDeleteRow;
                    imgBorder.Child = deleteRow;
                    Grid.SetColumn(imgBorder, 7);
                    Grid.SetRow(imgBorder, rowCount);
                    grid.Children.Add(imgBorder);
                }
                else
                {
                    GridOddRows(0, rowCount, genericProducts[i].category.category_name, genericProducts[i].category.category_id, grid);
                    GridOddRows(1, rowCount, genericProducts[i].type.product_name, genericProducts[i].type.product_id, grid);
                    GridOddRows(2, rowCount, genericProducts[i].brand.brand_name, genericProducts[i].brand.brand_id, grid);
                    GridOddRows(3, rowCount, genericProducts[i].model.model_name, genericProducts[i].model.model_id, grid);
                    GridOddRows(4, rowCount, genericProducts[i].model.item_unit.measure_unit, genericProducts[i].model.item_unit.unit_id, grid);
                    GridOddRows(5, rowCount, genericProducts[i].model.pricing_criteria.pricing_criteria_name, genericProducts[i].model.pricing_criteria.pricing_criteria_id, grid);
                    if (genericProducts[i].model.has_serial_number)
                        GridOddRows(6, rowCount, "YES", 0, grid);
                    else
                        GridOddRows(6, rowCount, "NO", 0, grid);
                    Border imgBorder = new Border();
                    imgBorder.Style = (Style)FindResource("OddRow");
                    System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                    DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                    newDropShadowEffect.BlurRadius = 20;
                    newDropShadowEffect.Color = color;
                    imgBorder.Effect = newDropShadowEffect;

                    BitmapImage imgSource = new BitmapImage();
                    imgSource.BeginInit();
                    imgSource.UriSource = new Uri(@"Icons\crossRed.png", UriKind.Relative);
                    imgSource.EndInit();
                    System.Windows.Controls.Image deleteRow = new System.Windows.Controls.Image();
                    deleteRow.Source = imgSource;
                    deleteRow.Tag = position++;
                    deleteRow.Width = 25;
                    deleteRow.Height = 25;
                    deleteRow.MouseDown += OnButtonClickDeleteRow;
                    imgBorder.Child = deleteRow;
                    Grid.SetColumn(imgBorder, 7);
                    Grid.SetRow(imgBorder, rowCount);
                    grid.Children.Add(imgBorder);
                }
                rowCount++;
            }
            viewCategoryStackPanel.Children.Add(grid);


        }

        private void OnMouseDownAddRow(object sender, MouseButtonEventArgs e)
        {
            if (newRowsAdded == 0)
            {
                newRowsAdded++;
                System.Windows.Controls.Image addRowImage = (System.Windows.Controls.Image)sender;
                Border addRowImageParent = addRowImage.Parent as Border;
                Grid masterGrid = addRowImageParent.Parent as Grid;
                int rows = masterGrid.Children.Count / 8;
                AddNewRow(ref masterGrid, ref rowCount, 8);
            }
            else
            {
                MessageBox.Show("Please Fill The Added Row First");
            }
        }

        private void GridHeader( string content , int column , Grid grid)
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
        

        private void OnMouseLeaveHasSerialCheckBox(object sender, MouseEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Grid smallGrid = checkBox.Parent as Grid;
            TextBlock label = smallGrid.Children[0] as TextBlock;
            Border parent = smallGrid.Parent as Border;
            Grid masterGrid = parent.Parent as Grid;
            int position = Int32.Parse(checkBox.Tag.ToString());

            Border categoryBorder = masterGrid.Children[position - 6] as Border;
            Grid categoryGrid = categoryBorder.Child as Grid;
            int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

            Border typeBorder = masterGrid.Children[position - 5] as Border;
            Grid typeGrid = typeBorder.Child as Grid;
            int typeId = Int32.Parse(typeGrid.Tag.ToString());

            Border brandBorder = masterGrid.Children[position - 4] as Border;
            Grid brandGrid = brandBorder.Child as Grid;
            int brandId = Int32.Parse(brandGrid.Tag.ToString());

            Border modelBorder = masterGrid.Children[position - 3] as Border;
            Grid modelGrid = modelBorder.Child as Grid;
            TextBlock modelTextBlock = modelGrid.Children[0] as TextBlock;
            int modelId = Int32.Parse(modelGrid.Tag.ToString());

            Border itemBorder = masterGrid.Children[position - 2] as Border;
            Grid itemGrid = itemBorder.Child as Grid;
            int itemId = Int32.Parse(itemGrid.Tag.ToString());

            Border priceBorder = masterGrid.Children[position - 1] as Border;
            Grid priceGrid = priceBorder.Child as Grid;
            int priceId = Int32.Parse(priceGrid.Tag.ToString());

          
            genericModel.SetCategoryId(categoryId);
            genericModel.SetProductId(typeId);
            genericModel.SetBrandId(brandId);
            genericModel.SetModelId(modelId);

            if (checkBox.IsChecked == true)
            {
             
                    if (!genericModel.UpdateModel(modelTextBlock.Text, itemId, true, priceId))
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        label.Text = "YES";
                        label.Visibility = Visibility.Visible;
                        label.Foreground = System.Windows.Media.Brushes.Green;
                        checkBox.Visibility = Visibility.Collapsed;
                    }
                
            }
            else
            {
              
                    if (!genericModel.UpdateModel(modelTextBlock.Text, itemId, false, priceId))
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        label.Text = "NO";
                        label.Visibility = Visibility.Visible;
                        label.Foreground = System.Windows.Media.Brushes.Green;
                        checkBox.Visibility = Visibility.Collapsed;
                    }
              
            }

        }
        private void OnMouseLeaveItemPriceOrPricingCriteria(object sender, MouseEventArgs e)
        {
           
               
                ComboBox comboBox = (ComboBox)sender;
                Grid parent = comboBox.Parent as Grid;
                TextBlock label = parent.Children[0] as TextBlock;
                Border parentOfGrid = parent.Parent as Border;
                Grid masterGrid = parentOfGrid.Parent as Grid;
                int position = Int32.Parse(comboBox.Tag.ToString());
                int columnNumber = Int32.Parse(label.Tag.ToString());

                if (columnNumber == ITEM_UNIT_COLUMN)
                {
                    Border modelBorder = masterGrid.Children[position - 1] as Border;
                    Grid modelGrid = modelBorder.Child as Grid;
                    TextBlock modelLabel = modelGrid.Children[0] as TextBlock;

                    if (comboBox.Text == string.Empty && comboBox.SelectedIndex == -1 && modelLabel.Text == string.Empty)
                    {

                        label.Visibility = Visibility.Visible;
                        comboBox.Visibility = Visibility.Collapsed;
                    }
                    else if (comboBox.SelectedIndex != -1 && modelLabel.Text != string.Empty && Int32.Parse(modelGrid.Tag.ToString()) == 0)
                    {
                        Border categoryBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                        Grid categoryGridItemUnit = categoryBorderItemUnit.Child as Grid;

                        Border typeBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                        Grid typeGridItemUnit = typeBorderItemUnit.Child as Grid;

                        Border brandBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
                        Grid brandGridItemUnit = brandBorderItemUnit.Child as Grid;

                        Border borderPrice = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) + 1] as Border;
                        Grid gridPrice = borderPrice.Child as Grid;
                        TextBlock pricetextBlock = gridPrice.Children[0] as TextBlock;
                        ComboBox priceComboBox = gridPrice.Children[1] as ComboBox;

                        Border hasSerialNumber = masterGrid.Children[position + 2] as Border;
                        Grid hasSerialGrid = hasSerialNumber.Child as Grid;
                        TextBlock hasSerialTextBlock = hasSerialGrid.Children[0] as TextBlock;
                        CheckBox hasSerialCheckBox = hasSerialGrid.Children[1] as CheckBox;
                    if (priceComboBox.SelectedIndex != -1)
                    {
                        if (hasSerialCheckBox.IsChecked == true)
                        {
                            genericModel.SetModelHasSerialNumber(true);
                            hasSerialTextBlock.Text = "YES";
                        }
                        else
                        {
                            genericModel.SetModelHasSerialNumber(false);
                            hasSerialTextBlock.Text = "NO";
                        }

                        genericModel.SetCategoryId(Int32.Parse(categoryGridItemUnit.Tag.ToString()));
                        genericModel.SetProductId(Int32.Parse(typeGridItemUnit.Tag.ToString()));
                        genericModel.SetBrandId(Int32.Parse(brandGridItemUnit.Tag.ToString()));
                        genericModel.SetModelName(modelLabel.Text);
                        genericModel.SetModelitemUnit(unitList[comboBox.SelectedIndex].measure_unit_id);
                        genericModel.SetModelpricingCriteria(pricingCriteriaList[priceComboBox.SelectedIndex].pricing_criteria_id);
                     
                            if (!genericModel.IssuNewModel())
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                modelGrid.Tag = genericModel.GetModelId();
                                modelLabel.Visibility = Visibility.Visible;
                                modelLabel.Foreground = System.Windows.Media.Brushes.Green;
                                label.Visibility = Visibility.Visible;
                                label.Text = comboBox.Text;
                                label.Foreground = System.Windows.Media.Brushes.Green;
                                comboBox.Visibility = Visibility.Collapsed;
                                pricetextBlock.Text = priceComboBox.Text;
                                pricetextBlock.Visibility = Visibility.Visible;
                                pricetextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                priceComboBox.Visibility = Visibility.Collapsed;
                                hasSerialTextBlock.Visibility = Visibility.Visible;
                                hasSerialCheckBox.Visibility = Visibility.Collapsed;
                            }
                       
                    }

                    }
                    else
                    {
                        if (comboBox.SelectedIndex != -1)
                        {


                            switch (Int32.Parse(label.Tag.ToString()))
                            {
                                case ITEM_UNIT_COLUMN:
                                    Border categoryBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                                    Grid categoryGridItemUnit = categoryBorderItemUnit.Child as Grid;

                                    Border typeBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                                    Grid typeGridItemUnit = typeBorderItemUnit.Child as Grid;

                                    Border brandBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
                                    Grid brandGridItemUnit = brandBorderItemUnit.Child as Grid;

                                    Border ModelBorder = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
                                    Grid ModelGrid = ModelBorder.Child as Grid;
                                    TextBlock modelName = ModelGrid.Children[0] as TextBlock;

                                    Border pricingCriteriaBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) + 1] as Border;
                                    Grid pricingCriteriaGridItemUnit = pricingCriteriaBorderItemUnit.Child as Grid;

                                    genericModel.SetCategoryId(Int32.Parse(categoryGridItemUnit.Tag.ToString()));
                                    genericModel.SetProductId(Int32.Parse(typeGridItemUnit.Tag.ToString()));
                                    genericModel.SetBrandId(Int32.Parse(brandGridItemUnit.Tag.ToString()));
                                    genericModel.SetModelId(Int32.Parse(ModelGrid.Tag.ToString()));
                                    genericModel.SetModelitemUnit(unitList[comboBox.SelectedIndex].measure_unit_id);
                                    genericModel.SetModelpricingCriteria(Int32.Parse(pricingCriteriaGridItemUnit.Tag.ToString()));
                              
                                    if (genericModel.UpdateModel(modelName.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
                                    {
                                        UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
                                    }
                                    else
                                    {
                                        label.Visibility = Visibility.Visible;
                                        label.Text = comboBox.Text;
                                        label.Foreground = System.Windows.Media.Brushes.Red;
                                        comboBox.Visibility = Visibility.Collapsed;
                                    }
                               
                                break;
                                case PRICING_CRITERIA_COLUMN:
                                    Border categoryBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 5] as Border;
                                    Grid categoryGridPricingCriteria = categoryBorderPricingCriteria.Child as Grid;

                                    Border typeBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                                    Grid typeGridPricingCriteria = typeBorderPricingCriteria.Child as Grid;

                                    Border brandBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                                    Grid brandGridPricingCriteria = brandBorderPricingCriteria.Child as Grid;

                                    Border ModelBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
                                    Grid ModelGridPricingCriteria = ModelBorderPricingCriteria.Child as Grid;
                                    TextBlock modelName1 = ModelGridPricingCriteria.Children[0] as TextBlock;

                                    Border borderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
                                    Grid gridItemUnit = borderItemUnit.Child as Grid;

                                    genericModel.SetCategoryId(Int32.Parse(categoryGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetProductId(Int32.Parse(typeGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetBrandId(Int32.Parse(brandGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetModelId(Int32.Parse(ModelGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetModelitemUnit(Int32.Parse(gridItemUnit.Tag.ToString()));
                                    genericModel.SetModelpricingCriteria(pricingCriteriaList[comboBox.SelectedIndex].pricing_criteria_id);
                              
                                    if (genericModel.UpdateModel(modelName1.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
                                    {
                                        UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
                                    }
                                    else
                                    {
                                        label.Visibility = Visibility.Visible;
                                        label.Text = comboBox.Text;
                                        label.Foreground = System.Windows.Media.Brushes.Red;
                                        comboBox.Visibility = Visibility.Collapsed;
                                    }
                               
                                break;
                            }
                        }
                    }
                }
                else if (columnNumber == PRICING_CRITERIA_COLUMN)
                {
                    Border modelBorder = masterGrid.Children[position - 2] as Border;
                    Grid modelGrid = modelBorder.Child as Grid;
                    TextBlock modelLabel = modelGrid.Children[0] as TextBlock;

                    if (comboBox.Text == string.Empty && comboBox.SelectedIndex == -1 && modelLabel.Text == string.Empty)
                    {

                        label.Visibility = Visibility.Visible;
                        comboBox.Visibility = Visibility.Collapsed;
                    }
                    else if (comboBox.SelectedIndex != -1 && modelLabel.Text != string.Empty && Int32.Parse(modelGrid.Tag.ToString()) == 0)
                    {
                        Border categoryBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 5] as Border;
                        Grid categoryGridItemUnit = categoryBorderItemUnit.Child as Grid;

                        Border typeBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                        Grid typeGridItemUnit = typeBorderItemUnit.Child as Grid;

                        Border brandBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                        Grid brandGridItemUnit = brandBorderItemUnit.Child as Grid;

                        Border borderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
                        Grid gridItemUnit = borderItemUnit.Child as Grid;
                        TextBlock itemTextBlock = gridItemUnit.Children[0] as TextBlock;
                        ComboBox itemUnitComboBox = gridItemUnit.Children[1] as ComboBox;

                        Border hasSerialNumber = masterGrid.Children[position + 1] as Border;
                        Grid hasSerialGrid = hasSerialNumber.Child as Grid;
                        TextBlock hasSerialTextBlock = hasSerialGrid.Children[0] as TextBlock;
                        CheckBox hasSerialCheckBox = hasSerialGrid.Children[1] as CheckBox;

                    if (itemUnitComboBox.SelectedIndex != -1)
                    {

                        if (hasSerialCheckBox.IsChecked == true)
                        {
                            genericModel.SetModelHasSerialNumber(true);
                            hasSerialTextBlock.Text = "YES";
                        }
                        else
                        {
                            genericModel.SetModelHasSerialNumber(false);
                            hasSerialTextBlock.Text = "NO";
                        }

                        genericModel.SetCategoryId(Int32.Parse(categoryGridItemUnit.Tag.ToString()));
                        genericModel.SetProductId(Int32.Parse(typeGridItemUnit.Tag.ToString()));
                        genericModel.SetBrandId(Int32.Parse(brandGridItemUnit.Tag.ToString()));
                        genericModel.SetModelName(modelLabel.Text);
                        genericModel.SetModelitemUnit(unitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                        genericModel.SetModelpricingCriteria(pricingCriteriaList[comboBox.SelectedIndex].pricing_criteria_id);
                      
                            if (!genericModel.IssuNewModel())
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                modelGrid.Tag = genericModel.GetModelId();
                                gridItemUnit.Tag = genericModel.GetModelUnitId();
                                modelLabel.Visibility = Visibility.Visible;
                                modelLabel.Foreground = System.Windows.Media.Brushes.Green;
                                label.Visibility = Visibility.Visible;
                                label.Text = comboBox.Text;
                                label.Foreground = System.Windows.Media.Brushes.Green;
                                comboBox.Visibility = Visibility.Collapsed;
                                itemTextBlock.Text = itemUnitComboBox.Text;
                                itemTextBlock.Visibility = Visibility.Visible;
                                itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                itemUnitComboBox.Visibility = Visibility.Collapsed;
                                hasSerialTextBlock.Visibility = Visibility.Visible;
                                hasSerialCheckBox.Visibility = Visibility.Collapsed;
                            }
                        
                    }
                    }
                    else
                    {
                        if (comboBox.SelectedIndex != -1)
                        {


                            switch (Int32.Parse(label.Tag.ToString()))
                            {
                                case ITEM_UNIT_COLUMN:
                                    Border categoryBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                                    Grid categoryGridItemUnit = categoryBorderItemUnit.Child as Grid;

                                    Border typeBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                                    Grid typeGridItemUnit = typeBorderItemUnit.Child as Grid;

                                    Border brandBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
                                    Grid brandGridItemUnit = brandBorderItemUnit.Child as Grid;

                                    Border ModelBorder = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
                                    Grid ModelGrid = ModelBorder.Child as Grid;
                                    TextBlock modelName = ModelGrid.Children[0] as TextBlock;

                                    Border pricingCriteriaBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) + 1] as Border;
                                    Grid pricingCriteriaGridItemUnit = pricingCriteriaBorderItemUnit.Child as Grid;

                                    genericModel.SetCategoryId(Int32.Parse(categoryGridItemUnit.Tag.ToString()));
                                    genericModel.SetProductId(Int32.Parse(typeGridItemUnit.Tag.ToString()));
                                    genericModel.SetBrandId(Int32.Parse(brandGridItemUnit.Tag.ToString()));
                                    genericModel.SetModelId(Int32.Parse(ModelGrid.Tag.ToString()));
                                    genericModel.SetModelitemUnit(unitList[comboBox.SelectedIndex].measure_unit_id);
                                    genericModel.SetModelpricingCriteria(Int32.Parse(pricingCriteriaGridItemUnit.Tag.ToString()));
                            
                                    if (genericModel.UpdateModel(modelName.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
                                    {
                                        UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
                                    }
                                    else
                                    {
                                        label.Visibility = Visibility.Visible;
                                        label.Text = comboBox.Text;
                                        label.Foreground = System.Windows.Media.Brushes.Red;
                                        comboBox.Visibility = Visibility.Collapsed;
                                    }
                              
                                break;
                                case PRICING_CRITERIA_COLUMN:
                                    Border categoryBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 5] as Border;
                                    Grid categoryGridPricingCriteria = categoryBorderPricingCriteria.Child as Grid;

                                    Border typeBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
                                    Grid typeGridPricingCriteria = typeBorderPricingCriteria.Child as Grid;

                                    Border brandBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
                                    Grid brandGridPricingCriteria = brandBorderPricingCriteria.Child as Grid;

                                    Border ModelBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
                                    Grid ModelGridPricingCriteria = ModelBorderPricingCriteria.Child as Grid;
                                    TextBlock modelName1 = ModelGridPricingCriteria.Children[0] as TextBlock;

                                    Border borderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
                                    Grid gridItemUnit = borderItemUnit.Child as Grid;

                                    genericModel.SetCategoryId(Int32.Parse(categoryGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetProductId(Int32.Parse(typeGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetBrandId(Int32.Parse(brandGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetModelId(Int32.Parse(ModelGridPricingCriteria.Tag.ToString()));
                                    genericModel.SetModelitemUnit(Int32.Parse(gridItemUnit.Tag.ToString()));
                                    genericModel.SetModelpricingCriteria(pricingCriteriaList[comboBox.SelectedIndex].pricing_criteria_id);
                            
                                    if (genericModel.UpdateModel(modelName1.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
                                    {
                                        UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
                                    }
                                    else
                                    {
                                        label.Visibility = Visibility.Visible;
                                        label.Text = comboBox.Text;
                                        label.Foreground = System.Windows.Media.Brushes.Red;
                                        comboBox.Visibility = Visibility.Collapsed;
                                    }
                                
                                break;
                            }
                        }

                    }
                }
            
        }
        private void OnMouseLeaveToUpdateName(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Grid smallGrid = textBox.Parent as Grid;
            TextBlock label = smallGrid.Children[0] as TextBlock;
            Border smallGridBorder = smallGrid.Parent as Border;
            Grid masterGrid = smallGridBorder.Parent as Grid;
            int columnNumber = Int32.Parse(label.Tag.ToString());
            int position = Int32.Parse(textBox.Tag.ToString());
            int id = Int32.Parse(smallGrid.Tag.ToString());
            if (columnNumber==MODEL_COLUMN)
            {
                if(textBox.Text==string.Empty)
                {
                    
                    label.Visibility = Visibility.Visible;
                    textBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Border categoryBorder = masterGrid.Children[position - 3] as Border;
                    Grid categoryGrid = categoryBorder.Child as Grid;
                    int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

                    Border typeBorder = masterGrid.Children[position - 2] as Border;
                    Grid typeGrid = typeBorder.Child as Grid;
                    int typeId = Int32.Parse(typeGrid.Tag.ToString());

                    Border brandBorder = masterGrid.Children[position - 1] as Border;
                    Grid brandGrid = brandBorder.Child as Grid;
                    int brandId = Int32.Parse(brandGrid.Tag.ToString());

                    Border itemBorder = masterGrid.Children[position + 1] as Border;
                    Grid itemGrid = itemBorder.Child as Grid;
                    int itemId = Int32.Parse(itemGrid.Tag.ToString());

                    Border priceBorder = masterGrid.Children[position + 2] as Border;
                    Grid priceGrid = priceBorder.Child as Grid;
                    int priceId = Int32.Parse(priceGrid.Tag.ToString());

                    Border hasSerialBorder = masterGrid.Children[position + 3] as Border;
                    Grid hasSerialGrid = hasSerialBorder.Child as Grid;
                    CheckBox hasSerialCheckBox = hasSerialGrid.Children[1] as CheckBox;

                    genericModel.SetCategoryId(categoryId);
                    genericModel.SetProductId(typeId);
                    genericModel.SetBrandId(brandId);
                    genericModel.SetModelId(id);
                    if (hasSerialCheckBox.IsChecked==true)
                    {
                        string errorMsg1 = string.Empty;
                        string outPutString1 = string.Empty;
                     
                            if (!genericModel.UpdateModel(textBox.Text, itemId, true, priceId))
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                label.Text = textBox.Text;
                                label.Visibility = Visibility.Visible;
                                label.Foreground = System.Windows.Media.Brushes.Green;
                                textBox.Visibility = Visibility.Collapsed;
                            }
                       

                    }
                    else
                    {
                        string errorMsg1 = string.Empty;
                        string outPutString1 = string.Empty;
                       
                            if (!genericModel.UpdateModel(textBox.Text, itemId, false, priceId))
                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                label.Text = textBox.Text;
                                label.Visibility = Visibility.Visible;
                                label.Foreground = System.Windows.Media.Brushes.Green;
                                textBox.Visibility = Visibility.Collapsed;
                            }
                        
                   

                    }


                }
            }
        }
        private void OnMouseLeaveComboBoxToUpdateNames(object sender, MouseEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Grid smallGrid = comboBox.Parent as Grid;
            Border border = smallGrid.Parent as Border;
            Grid masterGrid = border.Parent as Grid;
            TextBlock label = smallGrid.Children[0] as TextBlock;
            int columnNumber = Int32.Parse(label.Tag.ToString());
            int id = Int32.Parse(smallGrid.Tag.ToString());
            int position = Int32.Parse(comboBox.Tag.ToString());
            if (comboBox.SelectedIndex == -1 && comboBox.Text != string.Empty)
            {
                if (columnNumber == CATEGORY_COLUMN)
                {
                    if (comboBox.Text != string.Empty)
                    {
                        genericModel.SetCategoryId(id);

                        if (!genericModel.UpdateCategoryName(comboBox.Text))
                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        else
                        {
                            UpdateColumn(columnNumber, id, null, comboBox, label, masterGrid);
                        }

                    }
                }
                else if (columnNumber == TYPE_COLUMN)
                {
                    if (comboBox.Text != string.Empty)
                    {
                        Border catregoryBorder = masterGrid.Children[position - 1] as Border;
                        Grid categoryGrid = catregoryBorder.Child as Grid;
                        int category = Int32.Parse(categoryGrid.Tag.ToString());

                        genericModel.SetCategoryId(category);
                        genericModel.SetProductId(id);

                        if (!genericModel.UpdateProductName(comboBox.Text))
                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        else
                        {
                            UpdateColumn(columnNumber, id, null, comboBox, label, masterGrid);
                        }

                    }
                }
                else if (columnNumber == BRAND_COLUMN)
                {
                    //////////////////// IF IT WAS EMPTY CELL ////////////////////////
                    if (label.Text == string.Empty)
                    {
                        Border catregoryBorder = masterGrid.Children[position - 2] as Border;
                        Grid categoryGrid = catregoryBorder.Child as Grid;
                        int category = Int32.Parse(categoryGrid.Tag.ToString());

                        Border typeBorder = masterGrid.Children[position - 1] as Border;
                        Grid typeGrid = typeBorder.Child as Grid;
                        int typeId = Int32.Parse(typeGrid.Tag.ToString());
                        if (comboBox.SelectedIndex == -1 && comboBox.Text != string.Empty)
                        {
                            genericModel.SetCategoryId(category);
                            genericModel.SetProductId(typeId);
                            genericModel.SetBrandName(comboBox.Text);

                            if (!genericModel.IssuNewBrand())
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                label.Text = comboBox.Text;
                                label.Visibility = Visibility.Visible;
                                comboBox.Visibility = Visibility.Collapsed;
                                label.Foreground = System.Windows.Media.Brushes.Green;
                            }

                        }
                    }

                }
            }
            else if (comboBox.SelectedIndex != -1)
            {
                if (enterFunctionOnce == 1)
                {

                    if (columnNumber == CATEGORY_COLUMN)
                    {

                        int categoryId = categoryList[comboBox.SelectedIndex].category_id;

                        Border typeBorder = masterGrid.Children[position + 1] as Border;
                        Grid typeGrid = typeBorder.Child as Grid;
                        TextBlock typeTextBlock = typeGrid.Children[0] as TextBlock;
                        genericModel.SetProductId(Int32.Parse(typeGrid.Tag.ToString()));
                        genericModel.SetProductName(typeTextBlock.Text);

                        Border brandBorder = masterGrid.Children[position + 2] as Border;
                        Grid brandGrid = brandBorder.Child as Grid;
                        TextBlock brandTextBlock = brandGrid.Children[0] as TextBlock;
                        genericModel.SetBrandId(Int32.Parse(brandGrid.Tag.ToString()));
                        genericModel.SetBrandName(brandTextBlock.Text);


                        Border modelBorder = masterGrid.Children[position + 3] as Border;
                        Grid modelGrid = modelBorder.Child as Grid;
                        TextBlock modelTextBlock = modelGrid.Children[0] as TextBlock;
                        genericModel.SetModelId(Int32.Parse(modelGrid.Tag.ToString()));
                        genericModel.SetModelName(modelTextBlock.Text);

                        Border itemBorder = masterGrid.Children[position + 4] as Border;
                        Grid itemGrid = itemBorder.Child as Grid;
                        TextBlock itemTextBlock = itemGrid.Children[0] as TextBlock;
                        genericModel.SetModelitemUnit(Int32.Parse(itemGrid.Tag.ToString()));

                        Border priceBorder = masterGrid.Children[position + 5] as Border;
                        Grid priceGrid = priceBorder.Child as Grid;
                        TextBlock priceTextblock = priceGrid.Children[0] as TextBlock;
                        genericModel.SetModelpricingCriteria(Int32.Parse(priceGrid.Tag.ToString()));

                        Border serialBorder = masterGrid.Children[position + 6] as Border;
                        Grid serialGrid = serialBorder.Child as Grid;
                        CheckBox serialCheckBox = serialGrid.Children[1] as CheckBox;

                        MoveType(id, Int32.Parse(typeGrid.Tag.ToString()), typeTextBlock.Text, categoryId);

                    }
                    else if (columnNumber == TYPE_COLUMN)
                    {
                        int typeId = typeList[comboBox.SelectedIndex].product_id;

                        Border categoryBorder = masterGrid.Children[position - 1] as Border;
                        Grid categoryGrid = categoryBorder.Child as Grid;
                        TextBlock categoryTextBlock = categoryGrid.Children[0] as TextBlock;
                        int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

                        Border brandBorder = masterGrid.Children[position + 1] as Border;
                        Grid brandGrid = brandBorder.Child as Grid;
                        TextBlock brandTextBlock = brandGrid.Children[0] as TextBlock;
                        genericModel.SetBrandId(Int32.Parse(brandGrid.Tag.ToString()));
                        genericModel.SetBrandName(brandTextBlock.Text);


                        Border modelBorder = masterGrid.Children[position + 2] as Border;
                        Grid modelGrid = modelBorder.Child as Grid;
                        TextBlock modelTextBlock = modelGrid.Children[0] as TextBlock;
                        genericModel.SetModelId(Int32.Parse(modelGrid.Tag.ToString()));
                        genericModel.SetModelName(modelTextBlock.Text);

                        Border itemBorder = masterGrid.Children[position + 3] as Border;
                        Grid itemGrid = itemBorder.Child as Grid;
                        TextBlock itemTextBlock = itemGrid.Children[0] as TextBlock;
                        genericModel.SetModelitemUnit(Int32.Parse(itemGrid.Tag.ToString()));

                        Border priceBorder = masterGrid.Children[position + 4] as Border;
                        Grid priceGrid = priceBorder.Child as Grid;
                        TextBlock priceTextblock = priceGrid.Children[0] as TextBlock;
                        genericModel.SetModelpricingCriteria(Int32.Parse(priceGrid.Tag.ToString()));

                        Border serialBorder = masterGrid.Children[position + 5] as Border;
                        Grid serialGrid = serialBorder.Child as Grid;
                        CheckBox serialCheckBox = serialGrid.Children[1] as CheckBox;

                        int newType = typeList[comboBox.SelectedIndex].product_id;

                        MoveBrand(categoryId, id, newType);

                    }
                    else if (columnNumber == BRAND_COLUMN)
                    {

                    }

                }
                enterFunctionOnce = 0;

            }
            else
            {
                comboBox.Visibility = Visibility.Collapsed;
                label.Visibility = Visibility.Visible;
            }

        }
        private void OnMouseLeaveToFillEmptyCell(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Grid textBoxParent = textBox.Parent as Grid;
            TextBlock label = textBoxParent.Children[0] as TextBlock;
            Border border = textBoxParent.Parent as Border;
            Grid masterGrid = border.Parent as Grid;
            int columnNumber = Int32.Parse(label.Tag.ToString());
            int position = Int32.Parse(textBox.Tag.ToString());
            int id = Int32.Parse(textBoxParent.Tag.ToString());
            if (columnNumber == TYPE_COLUMN)
            {

                Border categoryBorder = masterGrid.Children[position - 1] as Border;
                Grid categoryGrid = categoryBorder.Child as Grid;
                int categoryId = Int32.Parse(categoryGrid.Tag.ToString());
                genericModel.SetCategoryId(categoryId);
                genericModel.SetProductName(textBox.Text);

                if (!genericModel.IssuNewProduct())
                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else
                {
                    label.Text = textBox.Text;
                    label.Visibility = Visibility.Visible;
                    textBox.Visibility = Visibility.Collapsed;
                    label.Foreground = System.Windows.Media.Brushes.Green;
                }


            }

            else if (columnNumber == BRAND_COLUMN)
            {

            }
            else if (columnNumber == MODEL_COLUMN)
            {
                Border categoryBorder = masterGrid.Children[position - 3] as Border;
                Grid categoryGrid = categoryBorder.Child as Grid;
                int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

                Border typeBorder = masterGrid.Children[position - 2] as Border;
                Grid typeGrid = typeBorder.Child as Grid;
                int typeId = Int32.Parse(typeGrid.Tag.ToString());

                Border brandBorder = masterGrid.Children[position - 1] as Border;
                Grid brandGrid = brandBorder.Child as Grid;
                int brandId = Int32.Parse(brandGrid.Tag.ToString());

                Border itemUnitBorder = masterGrid.Children[position + 1] as Border;
                Grid itemUnitGrid = itemUnitBorder.Child as Grid;
                TextBlock itemLabel = itemUnitGrid.Children[0] as TextBlock;
                ComboBox itemUnitComboBox = itemUnitGrid.Children[1] as ComboBox;

                Border priceBorder = masterGrid.Children[position + 2] as Border;
                Grid priceGrid = priceBorder.Child as Grid;
                TextBlock priceTextBlock = priceGrid.Children[0] as TextBlock;
                ComboBox priceComboBox = priceGrid.Children[1] as ComboBox;

                Border hasSerialBorder = masterGrid.Children[position + 3] as Border;
                Grid hasSerialGrid = hasSerialBorder.Child as Grid;
                TextBlock hasSerialTextBlock = hasSerialGrid.Children[0] as TextBlock;
                CheckBox hasSerialCheckBox = hasSerialGrid.Children[1] as CheckBox;
                if (textBox.Text != string.Empty && itemUnitComboBox.SelectedIndex != -1 && priceComboBox.SelectedIndex != -1)
                {
                    int itemUnitId = unitList[itemUnitComboBox.SelectedIndex].measure_unit_id;
                    int priceId = pricingCriteriaList[priceComboBox.SelectedIndex].pricing_criteria_id;
                    if (hasSerialCheckBox.IsChecked == true)
                        genericModel.SetModelHasSerialNumber(true);
                    else
                        genericModel.SetModelHasSerialNumber(false);



                    genericModel.SetCategoryId(categoryId);
                    genericModel.SetProductId(typeId);
                    genericModel.SetBrandId(brandId);
                    genericModel.SetModelName(textBox.Text);
                    genericModel.SetModelitemUnit(itemUnitId);
                    genericModel.SetModelpricingCriteria(priceId);

                    if (!genericModel.IssuNewModel())
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        label.Text = textBox.Text;
                        label.Visibility = Visibility.Visible;
                        textBox.Visibility = Visibility.Collapsed;
                        label.Foreground = System.Windows.Media.Brushes.Green;
                    }

                }
                else if (textBox.Text != string.Empty && itemUnitComboBox.SelectedIndex == -1 && priceComboBox.SelectedIndex == -1)
                {


                    genericModel.SetCategoryId(categoryId);
                    genericModel.SetProductId(typeId);
                    genericModel.SetBrandId(brandId);
                    genericModel.SetModelName(textBox.Text);

                }
                else if (textBox.Text != string.Empty && itemUnitComboBox.SelectedIndex != -1 || priceComboBox.SelectedIndex != -1)
                {
                    genericModel.SetCategoryId(categoryId);
                    genericModel.SetProductId(typeId);
                    genericModel.SetBrandId(brandId);
                    genericModel.SetModelName(textBox.Text);

                    if (itemUnitComboBox.SelectedIndex != -1)
                    {
                        int itemUnitId = unitList[itemUnitComboBox.SelectedIndex].measure_unit_id;
                        genericModel.SetModelitemUnit(itemUnitId);
                    }
                    else
                    {
                        int priceId = pricingCriteriaList[priceComboBox.SelectedIndex].pricing_criteria_id;
                        genericModel.SetModelpricingCriteria(priceId);
                    }
                }
                else
                {
                    label.Visibility = Visibility.Visible;
                    itemLabel.Visibility = Visibility.Visible;
                    priceTextBlock.Visibility = Visibility.Visible;
                    hasSerialTextBlock.Visibility = Visibility.Visible;


                    itemUnitComboBox.Visibility = Visibility.Collapsed;
                    priceComboBox.Visibility = Visibility.Collapsed;
                    hasSerialCheckBox.Visibility = Visibility.Collapsed;
                }
            }

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
        private void GridOddRows(int column, int row, string content, int contentId, Grid grid)
        {
            Grid smallGrid = new Grid();
            Border gridHeader = new Border();
            gridHeader.MouseDown += OnButtonClickMouseDownToFillEmptyCells;
            gridHeader.Style = (Style)FindResource("OddRow");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            gridHeader.Effect = newDropShadowEffect;



            if (column == BRAND_COLUMN)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                //header.MouseDown += OnButtonClick;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                header.Foreground = System.Windows.Media.Brushes.Gray;
                header.Background = System.Windows.Media.Brushes.Transparent;
                TextBox textBox = new TextBox();
                textBox.Visibility = Visibility.Collapsed;
                textBox.Tag = position;
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
                smallGrid.Children.Add(textBox);
                smallGrid.Children.Add(comboBox);
            }
            else if (column == ITEM_UNIT_COLUMN || column == PRICING_CRITERIA_COLUMN)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownItemPriceOrPricingCriteriaHeader;
                header.Background = System.Windows.Media.Brushes.Transparent;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveItemPriceOrPricingCriteria;
                smallGrid.Children.Add(comboBox);
            }
            else if (column == HAS_SERIAL_NUMBER)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownToChooseSerialNumber;
                header.Background = System.Windows.Media.Brushes.Transparent;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                CheckBox checkBox = new CheckBox();
                checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                checkBox.VerticalAlignment = VerticalAlignment.Center;
                checkBox.Visibility = Visibility.Collapsed;
                checkBox.Tag = position++;
                checkBox.MouseLeave += OnMouseLeaveHasSerialCheckBox;
                smallGrid.Children.Add(checkBox);
            }
            else
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownToUpdateNames;
                header.Background = System.Windows.Media.Brushes.Transparent;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                TextBox textBox = new TextBox();
                textBox.Visibility = Visibility.Collapsed;
                textBox.Tag = position;
                textBox.MouseLeave += OnMouseLeaveToUpdateName;
                smallGrid.Children.Add(textBox);
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
                // comboBox.SelectionChanged += OnSelChangedToMoveItems;
                smallGrid.Children.Add(comboBox);
            }



            gridHeader.Child = smallGrid;
            Grid.SetRow(gridHeader, row);
            Grid.SetColumn(gridHeader, column);
            grid.Children.Add(gridHeader);
        }
        private void GridEvenRows(int column, int row, string content,int contentId, Grid grid)
        {
            Grid smallGrid = new Grid();
            Border gridHeader = new Border();
            gridHeader.MouseDown += OnButtonClickMouseDownToFillEmptyCells;
            gridHeader.Style = (Style)FindResource("EvenRow");
            System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 20;
            newDropShadowEffect.Color = color;
            gridHeader.Effect = newDropShadowEffect;

            if (column == BRAND_COLUMN)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                //header.MouseDown += OnButtonClick;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                header.Foreground = System.Windows.Media.Brushes.Gray;
                header.Background = System.Windows.Media.Brushes.White;
                TextBox textBox = new TextBox();
                textBox.Visibility = Visibility.Collapsed;
                textBox.Tag = position;
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
                smallGrid.Children.Add(textBox);
                smallGrid.Children.Add(comboBox);
            }
            else if( column == ITEM_UNIT_COLUMN || column ==PRICING_CRITERIA_COLUMN)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownItemPriceOrPricingCriteriaHeader;
                header.Background = System.Windows.Media.Brushes.White;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveItemPriceOrPricingCriteria;
                smallGrid.Children.Add(comboBox);
            }
            else if(column == HAS_SERIAL_NUMBER)
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownToChooseSerialNumber; 
                header.Background = System.Windows.Media.Brushes.White;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                CheckBox checkBox = new CheckBox();
                checkBox.HorizontalAlignment= HorizontalAlignment.Center;
                checkBox.VerticalAlignment = VerticalAlignment.Center;
                checkBox.Visibility = Visibility.Collapsed;
                checkBox.Tag = position++;
                checkBox.MouseLeave += OnMouseLeaveHasSerialCheckBox;
                smallGrid.Children.Add(checkBox);
            }
            else
            {
                TextBlock header = new TextBlock();
                header.Text = content;
                header.Style = (Style)FindResource("labelStyleCardOdd");
                header.Tag = column;
                header.MouseDown += OnMouseDownToUpdateNames;
                header.Background = System.Windows.Media.Brushes.White;
                smallGrid.Tag = contentId;
                smallGrid.Children.Add(header);
                TextBox textBox = new TextBox();
                textBox.Visibility = Visibility.Collapsed;
                textBox.Tag = position;
                textBox.MouseLeave += OnMouseLeaveToUpdateName;
                smallGrid.Children.Add(textBox);
                ComboBox comboBox = new ComboBox();
                comboBox.Visibility = Visibility.Collapsed;
                comboBox.Tag = position++;
                comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
               // comboBox.SelectionChanged += OnSelChangedToMoveItems;
                smallGrid.Children.Add(comboBox);
            }
            gridHeader.Child = smallGrid;
            Grid.SetRow(gridHeader, row);
            Grid.SetColumn(gridHeader, column);
            grid.Children.Add(gridHeader);
        }

        private void MoveType(int categoryOld , int typeOld ,string typeName, int categoryNew)
        {
            ////////////////// GET RELATED BRANDS AND MODELS /////////////////
            brandList.Clear();
            modelList.Clear();
            int productNew = 0;
            if(!commonQueries.GetGenericProductBrands(typeOld,categoryOld,ref brandList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                genericModel.SetCategoryId(categoryNew);
                genericModel.SetProductName(typeName);
            
                    if (!genericModel.IssuNewProduct())
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        for (int i = 0; i < brandList.Count; i++)
                        {
                            genericModel.SetBrandName(brandList[i].brand_name);
                            genericModel.SetBrandId(brandList[i].brand_id);
                           
                                if (!genericModel.IssuproductBrand())
                                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                else
                                {
                                    if (!commonQueries.GetGenericBrandModels(typeOld, brandList[i].brand_id, categoryOld, ref modelList))
                                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    else
                                    {
                                        genericModel.SetBrandId(brandList[i].brand_id);
                                        for (int j = 0; j < modelList.Count; j++)
                                        {


                                            ///////////////////////// NEW ENTRY FOR THE NEW CATEGORY ////////////////////////
                                            genericModel.SetCategoryId(categoryNew);
                                            genericModel.SetProductName(typeName);
                                            genericModel.SetBrandName(brandList[i].brand_name);
                                            genericModel.SetModelName(modelList[j].model_name);
                                            genericModel.SetModelitemUnit(modelList[j].item_unit.unit_id);
                                            genericModel.SetModelpricingCriteria(modelList[j].pricing_criteria.pricing_criteria_id);
                                            genericModel.SetModelHasSerialNumber(modelList[j].has_serial_number);


                                            if (!genericModel.IssuNewModel())
                                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                            else
                                            {
                                                //////////////////////////// UPDATE REFERENCED TABLES ///////////////////////
                                                ////////////// RESTORE OLD IDs AND SAVE NEWLY GENERATED IDs//////////////////////////
                                                productNew = genericModel.GetProductId();
                                                int brandNew = genericModel.GetBrandId();
                                                int modelNew = genericModel.GetModelId();

                                                genericModel.SetCategoryId(categoryOld);
                                                genericModel.SetProductId(typeOld);
                                                genericModel.SetBrandId(brandList[i].brand_id);
                                                genericModel.SetModelId(modelList[j].model_id);

                                                if (!genericModel.UpdateReferencedTables(categoryNew, productNew, brandNew, modelNew))
                                                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                else
                                                {
                                                    ///////////////////// DELETE OLD MODELS RELATED TO BRAND[i] ////////////////
                                                    if (!genericModel.DeleteModel())
                                                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                }
                                                genericModel.SetCategoryId(categoryNew);
                                                genericModel.SetProductId(productNew);
                                                genericModel.SetBrandId(brandList[i].brand_id);
                                            }

                                        }

                                        /////////////////////////// DELETE OLD BRAND[i] /////////////////////////////
                                        genericModel.SetCategoryId(categoryOld);
                                        genericModel.SetProductId(typeOld);
                                        genericModel.SetBrandId(brandList[i].brand_id);
                                        if (!genericModel.DeleteFromProductBrands())
                                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        else
                                        {
                                            genericModel.SetCategoryId(categoryNew);
                                            genericModel.SetProductId(productNew);
                                        }
                                    }
                                }
                          
                        }
                    }
             
                genericModel.SetCategoryId(categoryOld);
                genericModel.SetProductId(typeOld);
                /////////////////////////// DELETE OLD TYPE ////////////////////////////
                if (!genericModel.DeleteFromProduct())
                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else
                { //////////////////////////// DELETE OLD CATEGORY IF EMPTY /////////////////////////
                    if(!genericModel.DeleteFromCategory())
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                InitializeTableView();
            }
        }
        private void MoveBrand(int categoryOld, int typeOld, int typeNew)
        {
            ////////////////// GET RELATED BRANDS AND MODELS /////////////////
            brandList.Clear();
            modelList.Clear();
            int productNew = 0;
            if (!commonQueries.GetGenericProductBrands(typeOld, categoryOld, ref brandList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                genericModel.SetCategoryId(categoryOld);
                genericModel.SetProductId(typeNew);
                  for (int i = 0; i < brandList.Count; i++)
                    {
                        genericModel.SetBrandName(brandList[i].brand_name);
                        genericModel.SetBrandId(brandList[i].brand_id);
                        if (!genericModel.IssuproductBrand())
                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        else
                        {
                            if (!commonQueries.GetGenericBrandModels(typeOld, brandList[i].brand_id, categoryOld, ref modelList))
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                genericModel.SetBrandId(brandList[i].brand_id);
                                for (int j = 0; j < modelList.Count; j++)
                                {


                                    ///////////////////////// NEW ENTRY FOR THE NEW CATEGORY ////////////////////////
                                  
                                   
                                    genericModel.SetBrandName(brandList[i].brand_name);
                                    genericModel.SetModelName(modelList[j].model_name);
                                    genericModel.SetModelitemUnit(modelList[j].item_unit.unit_id);
                                    genericModel.SetModelpricingCriteria(modelList[j].pricing_criteria.pricing_criteria_id);
                                    genericModel.SetModelHasSerialNumber(modelList[j].has_serial_number);

                                    if (!genericModel.IssuNewModel())
                                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    else
                                    {
                                        //////////////////////////// UPDATE REFERENCED TABLES ///////////////////////
                                        ////////////// RESTORE OLD IDs AND SAVE NEWLY GENERATED IDs//////////////////////////
                                        productNew = genericModel.GetProductId();
                                        int brandNew = genericModel.GetBrandId();
                                        int modelNew = genericModel.GetModelId();

                                        genericModel.SetCategoryId(categoryOld);
                                        genericModel.SetProductId(typeOld);
                                        genericModel.SetBrandId(brandList[i].brand_id);
                                        genericModel.SetModelId(modelList[j].model_id);

                                        if (!genericModel.UpdateReferencedTables(categoryOld, productNew, brandNew, modelNew))
                                            System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        else
                                        {
                                            ///////////////////// DELETE OLD MODELS RELATED TO BRAND[i] ////////////////
                                            if (!genericModel.DeleteModel())
                                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        }
                                        genericModel.SetCategoryId(categoryOld);
                                        genericModel.SetProductId(productNew);
                                        genericModel.SetBrandId(brandList[i].brand_id);
                                    }


                                }

                                /////////////////////////// DELETE OLD BRAND[i] /////////////////////////////
                                genericModel.SetCategoryId(categoryOld);
                                genericModel.SetProductId(typeOld);
                                genericModel.SetBrandId(brandList[i].brand_id);
                                if (!genericModel.DeleteFromProductBrands())
                                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                else
                                {
                                    genericModel.SetCategoryId(categoryOld);
                                    genericModel.SetProductId(productNew);
                                }
                            }
                        }
                    }
                
                genericModel.SetCategoryId(categoryOld);
                genericModel.SetProductId(typeOld);
                /////////////////////////// DELETE OLD TYPE ////////////////////////////
                if (!genericModel.DeleteFromProduct())
                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else
                { //////////////////////////// DELETE OLD CATEGORY IF EMPTY /////////////////////////
                    if (!genericModel.DeleteFromCategory())
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                InitializeTableView();
            }
        }
        

        private void OnMouseDownToChooseSerialNumber(object sender, MouseButtonEventArgs e)
        {
           TextBlock hasSerialNumberTextBlock =(TextBlock)sender;
            Grid hasSerialNumberGrid = hasSerialNumberTextBlock.Parent as Grid;
            CheckBox hasSerialNumberCheckBox = hasSerialNumberGrid.Children[1] as CheckBox;
            hasSerialNumberCheckBox.Visibility = Visibility.Visible;
            hasSerialNumberTextBlock.Visibility = Visibility.Collapsed;
            hasSerialNumberCheckBox.Click += OnClickHasSerialNumberCheckBox;
        }

        private void OnClickHasSerialNumberCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox hasSerialNumberCheckBox = (CheckBox)sender;
            Grid hasSerialNumberGrid = hasSerialNumberCheckBox.Parent as Grid;
            TextBlock hasSerialNumberTextBlock = hasSerialNumberGrid.Children[0] as TextBlock;
            Border parent = hasSerialNumberGrid.Parent as Border;
            Grid masterGrid = parent.Parent as Grid;
            int position = Int32.Parse(hasSerialNumberCheckBox.Tag.ToString());

            Border categoryBorder = masterGrid.Children[position - 6] as Border;
            Grid categoryGrid = categoryBorder.Child as Grid;
            TextBlock categoryTextBlock = categoryGrid.Children[0] as TextBlock;
            int categoryId = Int32.Parse(categoryGrid.Tag.ToString());
            genericModel.SetCategoryId(categoryId);

            Border typeBorder = masterGrid.Children[position - 5] as Border;
            Grid typeGrid = typeBorder.Child as Grid;
            TextBlock typeTextBlock = typeGrid.Children[0] as TextBlock;
            int typeId = Int32.Parse(typeGrid.Tag.ToString());
            genericModel.SetProductId(typeId);

            Border brandBorder = masterGrid.Children[position - 4] as Border;
            Grid brandGrid = brandBorder.Child as Grid;
            TextBlock brandTextBlock = brandGrid.Children[0] as TextBlock;
            genericModel.SetBrandId(Int32.Parse(brandGrid.Tag.ToString()));
            genericModel.SetBrandName(brandTextBlock.Text);


            Border modelBorder = masterGrid.Children[position - 3] as Border;
            Grid modelGrid = modelBorder.Child as Grid;
            TextBlock modelTextBlock = modelGrid.Children[0] as TextBlock;
            genericModel.SetModelId(Int32.Parse(modelGrid.Tag.ToString()));
            genericModel.SetModelName(modelTextBlock.Text);

            Border itemBorder = masterGrid.Children[position - 2] as Border;
            Grid itemGrid = itemBorder.Child as Grid;
            TextBlock itemTextBlock = itemGrid.Children[0] as TextBlock;
            genericModel.SetModelitemUnit(Int32.Parse(itemGrid.Tag.ToString()));

            Border priceBorder = masterGrid.Children[position - 1] as Border;
            Grid priceGrid = priceBorder.Child as Grid;
            TextBlock priceTextblock = priceGrid.Children[0] as TextBlock;
            genericModel.SetModelpricingCriteria(Int32.Parse(priceGrid.Tag.ToString()));

            if (hasSerialNumberCheckBox.IsChecked==true)
            {

                
                    if (!genericModel.UpdateModel(modelTextBlock.Text, Int32.Parse(itemGrid.Tag.ToString()), true, Int32.Parse(priceGrid.Tag.ToString())))
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        hasSerialNumberTextBlock.Text = "YES";

                    }
              
            }
            else
            {
               
              
                    if (!genericModel.UpdateModel(modelTextBlock.Text, Int32.Parse(itemGrid.Tag.ToString()), false, Int32.Parse(priceGrid.Tag.ToString())))
                        System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                    {
                        hasSerialNumberTextBlock.Text = "YES";

                    }
               
            }
            hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;
            hasSerialNumberTextBlock.Visibility = Visibility.Visible;
        }

        private void OnButtonClickMouseDownToFillEmptyCells(object sender, MouseButtonEventArgs e)
        {
            enterFunctionOnce2 = 1;
            Border border = (Border)sender;
            Grid masterGrid = border.Parent as Grid;
            Grid smallGrid = border.Child as Grid;
            TextBlock textBlock = smallGrid.Children[0] as TextBlock;
            int columnNumber = Int32.Parse(textBlock.Tag.ToString());
            if (textBlock.Text == string.Empty)
            {


                if (columnNumber != ITEM_UNIT_COLUMN && columnNumber != PRICING_CRITERIA_COLUMN && columnNumber != MODEL_COLUMN && columnNumber != BRAND_COLUMN)
                {
                    TextBox textBox = smallGrid.Children[1] as TextBox;
                    int previousPosition = Int32.Parse(textBox.Tag.ToString()) - 1;
                    Border pastBorder = masterGrid.Children[previousPosition] as Border;
                    Grid pastSmallGrid = pastBorder.Child as Grid;
                    TextBlock pastLabel = pastSmallGrid.Children[0] as TextBlock;
                    if (pastLabel.Text != string.Empty)
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                        if (textBlock.Text == string.Empty)
                        {
                            textBox.Visibility = Visibility.Visible;
                            textBox.MouseLeave += OnMouseLeaveToFillEmptyCell;
                        }
                    }
                }
                else if(columnNumber == BRAND_COLUMN)
                {
                    ComboBox comboBox = smallGrid.Children[2] as ComboBox;
                    int previousPosition = Int32.Parse(comboBox.Tag.ToString())-1;
                    Border pastBorder = masterGrid.Children[previousPosition] as Border;
                    Grid pastSmallGrid = pastBorder.Child as Grid;
                    TextBlock pastLabel = pastSmallGrid.Children[0] as TextBlock;
                    if (pastLabel.Text != string.Empty)
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                        if (textBlock.Text == string.Empty)
                        {
                            brandList.Clear();
                            comboBox.Items.Clear();
                            if(!commonQueries.GetGenericBrands(ref brandList))
                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            for(int i=0;i<brandList.Count;i++)
                            {
                                comboBox.Items.Add(brandList[i].brand_name);
                            }
                            comboBox.Visibility = Visibility.Visible;
                            comboBox.IsEditable = true;
                            comboBox.MouseLeave +=OnMouseLeaveWhenANewRowIsAdded;
                            //comboBox.SelectionChanged += OnSelectionChangedComboBoxNewRowAdded;
                        }
                    }
                }
                else if (columnNumber == MODEL_COLUMN)
                {
                    TextBox textBox = smallGrid.Children[1] as TextBox;
                    int previousPosition = Int32.Parse(textBox.Tag.ToString()) - 1;
                    Border pastBorder = masterGrid.Children[previousPosition] as Border;
                    Grid pastSmallGrid = pastBorder.Child as Grid;
                    TextBlock pastLabel = pastSmallGrid.Children[0] as TextBlock;
                    if (pastLabel.Text != string.Empty)
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                        if (textBlock.Text == string.Empty)
                        {
                            textBox.Visibility = Visibility.Visible;
                            textBox.MouseLeave += OnMouseLeaveToFillEmptyCell;

                            int nextPosition = Int32.Parse(textBox.Tag.ToString()) + 1;
                            Border itemUnitBorder = masterGrid.Children[nextPosition] as Border;
                            Grid itemUnitGrid = itemUnitBorder.Child as Grid;
                            TextBlock itemLabel = itemUnitGrid.Children[0] as TextBlock;
                            ComboBox itemComboBox = itemUnitGrid.Children[1] as ComboBox;
                            itemComboBox.Visibility = Visibility.Visible;
                            itemComboBox.MouseLeave += OnMouseLeaveItemPriceOrPricingCriteria;
                            itemLabel.Visibility = Visibility.Collapsed;
                            itemComboBox.Items.Clear();
                            for (int i = 0; i < unitList.Count; i++)
                            {
                                itemComboBox.Items.Add(unitList[i].measure_unit);

                            }

                            int nextNextPosition = Int32.Parse(textBox.Tag.ToString()) + 2;
                            Border priceBorder = masterGrid.Children[nextNextPosition] as Border;
                            Grid priceGrid = priceBorder.Child as Grid;
                            TextBlock priceLabel = priceGrid.Children[0] as TextBlock;
                            ComboBox priceComboBox = priceGrid.Children[1] as ComboBox;
                            priceComboBox.Visibility = Visibility.Visible;
                            priceLabel.Visibility = Visibility.Collapsed;
                            priceComboBox.MouseLeave += OnMouseLeaveItemPriceOrPricingCriteria;
                            priceComboBox.Items.Clear();
                            for (int i = 0; i < pricingCriteriaList.Count; i++)
                            {
                                priceComboBox.Items.Add(pricingCriteriaList[i].pricing_criteria_name);
                            }

                            int hasSerialNumberPosition = Int32.Parse(textBox.Tag.ToString()) + 3;
                            Border hasSerialNumberBorder = masterGrid.Children[hasSerialNumberPosition] as Border;
                            Grid hasSerialNumberGrid = hasSerialNumberBorder.Child as Grid;
                            TextBlock hasSerialNumberTextBlock = hasSerialNumberGrid.Children[0] as TextBlock;
                            CheckBox hasSerialNumberCheckBox = hasSerialNumberGrid.Children[1] as CheckBox;
                            hasSerialNumberCheckBox.IsEnabled = true;
                            hasSerialNumberCheckBox.Visibility = Visibility.Visible;
                            hasSerialNumberTextBlock.Visibility = Visibility.Collapsed;



                        }
                    }
                }
            }
           
        }


        private void OnMouseDownToUpdateNames(object sender, MouseButtonEventArgs e)
        {
            TextBlock label = (TextBlock)sender;
            Grid smallGrid = label.Parent as Grid;
            Border border = smallGrid.Parent as Border;
            Grid outterGrid = border.Parent as Grid;
            TextBox textbox = smallGrid.Children[1] as TextBox;
            ComboBox comboBox = smallGrid.Children[2] as ComboBox;
            int columnNumber = Int32.Parse(label.Tag.ToString());
            int position = Int32.Parse(comboBox.Tag.ToString());
            int id = Int32.Parse(smallGrid.Tag.ToString());
            if(columnNumber == CATEGORY_COLUMN)
            {
                enterFunctionOnce = 1;
                categoryList.Clear();
                if(!commonQueries.GetGenericProductCategories(ref categoryList))
                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else
                for(int i=0;i<categoryList.Count;i++)
                    {
                        comboBox.Items.Add(categoryList[i].category_name);
                    }
                comboBox.IsEditable = true;
               // comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
               // comboBox.SelectionChanged += OnSelChangedToMoveItems;
                comboBox.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Collapsed;
            }
            else if(columnNumber == TYPE_COLUMN)
            {
                enterFunctionOnce = 1;
                typeList.Clear();
                Border categoryBorder = outterGrid.Children[position - 1] as Border;
                Grid categoryGrid = categoryBorder.Child as Grid;
                int categoryId = Int32.Parse(categoryGrid.Tag.ToString());
                if(!commonQueries.GetGenericProducts(ref typeList , categoryId))
                    System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                for (int i = 0; i < typeList.Count;i++)
                {
                    comboBox.Items.Add(typeList[i].product_name);
                }
                comboBox.IsEditable = true;
               // comboBox.MouseLeave += OnMouseLeaveComboBoxToUpdateNames;
               // comboBox.SelectionChanged += OnSelChangedToMoveItems;
                comboBox.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Collapsed;
            }
            else if(columnNumber==MODEL_COLUMN)
            {
                enterFunctionOnce = 1;
                label.Visibility = Visibility.Collapsed;
                textbox.Visibility = Visibility.Visible;

            }
           
          
        }
        private void OnMouseDownItemPriceOrPricingCriteriaHeader(object sender, MouseButtonEventArgs e)
        {
            TextBlock label = (TextBlock)sender;
            Grid smallGrid = label.Parent as Grid;
            Border border = smallGrid.Parent as Border;
            Grid outterGrid = border.Parent as Grid;
            ComboBox comboBox = smallGrid.Children[1] as ComboBox;
            comboBox.MouseLeave += OnMouseLeaveItemPriceOrPricingCriteria;
            comboBox.Visibility = Visibility.Visible;
            label.Visibility = Visibility.Collapsed;
            comboBox.Items.Clear();
            if (Int32.Parse(label.Tag.ToString())==4)
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    comboBox.Items.Add(unitList[i].measure_unit);
                }
            }
            else
            {
                for (int i = 0; i < pricingCriteriaList.Count; i++)
                {
                    comboBox.Items.Add(pricingCriteriaList[i].pricing_criteria_name);
                }
            }
        }
        private void OnMouseLeaveWhenANewRowIsAdded(object sender, MouseEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string outPutString = null;
            string errorMessage = null;
            Grid parent = comboBox.Parent as Grid;
            Border parentOfParent = parent.Parent as Border;
            Grid masterGrid = parentOfParent.Parent as Grid;
            TextBlock textBlock = parent.Children[0] as TextBlock;
            int NextPosition = Int32.Parse(comboBox.Tag.ToString()) + 1;
            int columnNumber = Int32.Parse(textBlock.Tag.ToString());
            if (comboBox.SelectedIndex == -1 && comboBox.Text != string.Empty)
            {
                if (!integrityChecks.CheckCompanyNameEditBox(comboBox.Text, ref outPutString, true, ref errorMessage))
                    MessageBox.Show("error");
                else
                {
                    if (columnNumber == CATEGORY_COLUMN)
                    {

                        genericModel.SetCategoryName(comboBox.Text);
                        Border typeBorder = masterGrid.Children[NextPosition] as Border;
                        Grid typeGrid = typeBorder.Child as Grid;
                        ComboBox typeComboBox = typeGrid.Children[1] as ComboBox;
                        typeComboBox.Items.Clear();
                        typeComboBox.IsEnabled = true;
                    }
                    else if (columnNumber == TYPE_COLUMN)
                    {
                        genericModel.SetProductName(comboBox.Text);

                        int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                        Border border = masterGrid.Children[categoryPosition] as Border;
                        Grid grid = border.Child as Grid;
                        ComboBox categoryComboBox = grid.Children[1] as ComboBox;
                        brandList.Clear();
                        if (!commonQueries.GetGenericBrands(ref brandList))
                            MessageBox.Show("error");
                        else
                        {
                            Border brandBorder = masterGrid.Children[NextPosition] as Border;
                            Grid brandGrid = brandBorder.Child as Grid;
                            ComboBox brandComboBox = brandGrid.Children[1] as ComboBox;
                            brandComboBox.Items.Clear();
                            for (int i = 0; i < brandList.Count; i++)
                            {
                                brandComboBox.Items.Add(brandList[i].brand_name);

                            }
                            brandComboBox.IsEnabled = true;

                        }

                    }
                    else if (columnNumber == BRAND_COLUMN)
                    {
                        genericModel.SetBrandName(comboBox.Text);
                        genericModel.SetAddedBy(loggedInUser.GetEmployeeId());
                        int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 2;
                        Border categoryBorder = masterGrid.Children[categoryPosition] as Border;
                        Grid categoryGrid = categoryBorder.Child as Grid;
                        ComboBox categoryComboBox = categoryGrid.Children[1] as ComboBox;
                        int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

                        int typePosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                        Border typeBorder = masterGrid.Children[typePosition] as Border;
                        Grid typeGrid = typeBorder.Child as Grid;
                        ComboBox typeComboBox = typeGrid.Children[1] as ComboBox;
                        int typeId = Int32.Parse(typeGrid.Tag.ToString());

                        genericModel.SetCategoryId(categoryId);
                        genericModel.SetProductId(typeId);
                        // if (!genericModel.IssuNewBrand())
                        //     System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        // else
                        // {
                        //    
                        //         textBlock.Visibility = Visibility.Visible;
                        //         comboBox.Visibility = Visibility.Collapsed;
                        //         textBlock.Text = comboBox.Text;
                        //         textBlock.Foreground = System.Windows.Media.Brushes.Green;
                        //     
                        // }


                    }
                }
            }
            else if (comboBox.SelectedIndex != -1)
            {
                if (columnNumber == CATEGORY_COLUMN)
                {
                    typeList.Clear();
                    if (!commonQueries.GetGenericProducts(ref typeList, categoryList[comboBox.SelectedIndex].category_id))
                        MessageBox.Show("Error");
                    else
                    {
                        Border border = masterGrid.Children[NextPosition] as Border;
                        Grid grid = border.Child as Grid;
                        ComboBox typeComboBox = grid.Children[1] as ComboBox;
                        typeComboBox.Items.Clear();

                        for (int i = 0; i < typeList.Count; i++)
                        {
                            typeComboBox.Items.Add(typeList[i].product_name);

                        }
                        typeComboBox.IsEnabled = true;
                    }
                }
                else if (columnNumber == TYPE_COLUMN)
                {
                    int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                    Border border = masterGrid.Children[categoryPosition] as Border;
                    Grid grid = border.Child as Grid;
                    ComboBox categoryComboBox = grid.Children[1] as ComboBox;
                    brandList.Clear();
                    if (!commonQueries.GetGenericProductBrands(typeList[comboBox.SelectedIndex].product_id, categoryList[categoryComboBox.SelectedIndex].category_id, ref brandList))
                        MessageBox.Show("error");
                    else
                    {
                        Border brandBorder = masterGrid.Children[NextPosition] as Border;
                        Grid brandGrid = brandBorder.Child as Grid;
                        ComboBox brandComboBox = brandGrid.Children[1] as ComboBox;
                        brandComboBox.Items.Clear();
                        for (int i = 0; i < brandList.Count; i++)
                        {
                            brandComboBox.Items.Add(brandList[i].brand_name);

                        }
                        brandComboBox.IsEnabled = true;
                    }

                }
                else if (columnNumber == BRAND_COLUMN)
                {

                    genericModel.SetBrandName(comboBox.Text);
                    genericModel.SetAddedBy(loggedInUser.GetEmployeeId());
                    Border modelBorder = masterGrid.Children[NextPosition] as Border;
                    Grid modelGrid = modelBorder.Child as Grid;
                    TextBox modelComboBox = modelGrid.Children[1] as TextBox;

                    modelComboBox.IsEnabled = true;

                    Border itemUnitBorder = masterGrid.Children[NextPosition + 1] as Border;
                    Grid itemUnitGrid = itemUnitBorder.Child as Grid;
                    ComboBox itemUnitComboBox = itemUnitGrid.Children[1] as ComboBox;

                    itemUnitComboBox.IsEditable = false;
                    itemUnitComboBox.Items.Clear();

                    Border pricingCriteriaBorder = masterGrid.Children[NextPosition + 2] as Border;
                    Grid pricingCriteriaGrid = pricingCriteriaBorder.Child as Grid;
                    ComboBox pricingCriteriaComboBox = pricingCriteriaGrid.Children[1] as ComboBox;
                    pricingCriteriaComboBox.IsEditable = false;

                    Border hasSerialNumberBorder = masterGrid.Children[NextPosition + 3] as Border;
                    Grid hasSerialNumberGrid = hasSerialNumberBorder.Child as Grid;
                    CheckBox hasSerialNumberCheckBox = hasSerialNumberGrid.Children[1] as CheckBox;
                    hasSerialNumberCheckBox.IsEnabled = true;

                    pricingCriteriaComboBox.Items.Clear();

                    unitList.Clear();
                    pricingCriteriaList.Clear();


                    if (!commonQueries.GetMeasureUnits(ref unitList))
                        MessageBox.Show("Error");
                    else
                    {
                        for (int i = 0; i < unitList.Count; i++)
                        {
                            itemUnitComboBox.Items.Add(unitList[i].measure_unit);
                        }
                        itemUnitComboBox.IsEnabled = true;
                    }

                    if (!commonQueries.GetPricingCriteria(ref pricingCriteriaList))
                        MessageBox.Show("Error");
                    else
                    {
                        for (int i = 0; i < pricingCriteriaList.Count; i++)
                        {
                            pricingCriteriaComboBox.Items.Add(pricingCriteriaList[i].pricing_criteria_name);
                        }
                        pricingCriteriaComboBox.IsEnabled = true;
                    }
                    //int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 2;
                    //Border categoryBorder = masterGrid.Children[categoryPosition] as Border;
                    //Grid categoryGrid = categoryBorder.Child as Grid;
                    //ComboBox categoryComboBox = categoryGrid.Children[1] as ComboBox;
                    //nt categoryId = categoryList[categoryComboBox.SelectedIndex].category_id;
                    //
                    //
                    //int typePosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                    //Border typeBorder = masterGrid.Children[typePosition] as Border;
                    //Grid typeGrid = typeBorder.Child as Grid;
                    //ComboBox typeComboBox = typeGrid.Children[1] as ComboBox;
                    //int typeId = Int32.Parse(typeGrid.Tag.ToString());
                    //
                    //genericModel.SetCategoryId(categoryId);
                    //genericModel.SetProductId(typeId);
                    //genericModel.SetBrandId(brandList[comboBox.SelectedIndex].brand_id);
                    //if (!genericModel.IssuproductBrand())
                    //     System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    // else
                    // {
                    //     textBlock.Visibility = Visibility.Visible;
                    //     comboBox.Visibility = Visibility.Collapsed;
                    //     textBlock.Text = comboBox.Text;
                    //     textBlock.Foreground = System.Windows.Media.Brushes.Green;
                    // }
                }
            }
        }
        private void OnMouseLeaveModelTextBoxWhenANewRowIsAdded(object sender, MouseEventArgs e)
        {

        }


        private void OnButtonLeave(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Grid parent = textBox.Parent as Grid;
            TextBlock label = parent.Children[0] as TextBlock;
            Border parentOfGrid = parent.Parent as Border;
            Grid masterGrid = parentOfGrid.Parent as Grid;
           if(textBox.Text!=string.Empty)
            {
               switch (Int32.Parse(label.Tag.ToString()))
                {
                    case CATEGORY_COLUMN:
                        genericModel.SetCategoryId(Int32.Parse(parent.Tag.ToString()));
                      
                            if (genericModel.UpdateCategoryName(textBox.Text))
                            {
                                UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), textBox, null, label, masterGrid);
                            }

                            else
                            {
                                label.Visibility = Visibility.Visible;
                                label.Text = textBox.Text;
                                label.Foreground = System.Windows.Media.Brushes.Red;
                                textBox.Visibility = Visibility.Collapsed;
                            }
                       
                         break;
                    case TYPE_COLUMN:
                        Border categoryBorder = masterGrid.Children[Int32.Parse(textBox.Tag.ToString())-1] as Border;
                        Grid categoryGrid = categoryBorder.Child as Grid;
                        genericModel.SetCategoryId(Int32.Parse(categoryGrid.Tag.ToString()));
                        genericModel.SetProductId(Int32.Parse(parent.Tag.ToString()));
                       
                            if (genericModel.UpdateProductName(textBox.Text))
                            {
                                UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), textBox, null, label, masterGrid);
                            }
                            else
                            {
                                label.Visibility = Visibility.Visible;
                                label.Foreground = System.Windows.Media.Brushes.LightGray;
                                textBox.Visibility = Visibility.Collapsed;
                            }
                       
                      
                        break;
                    case BRAND_COLUMN:
                      
                          label.Visibility = Visibility.Visible;
                          label.Foreground = System.Windows.Media.Brushes.LightGray;
                          textBox.Visibility = Visibility.Collapsed;
                      
                        break;
                    case MODEL_COLUMN:

                        Border categoryBorderBrand = masterGrid.Children[Int32.Parse(textBox.Tag.ToString()) - 3] as Border;
                        Grid categoryGridBrand = categoryBorderBrand.Child as Grid;
                        
                        Border typeBorderBrand = masterGrid.Children[Int32.Parse(textBox.Tag.ToString()) - 2] as Border;
                        Grid typeGridBrand = typeBorderBrand.Child as Grid;

                        Border brandBorderBrand = masterGrid.Children[Int32.Parse(textBox.Tag.ToString()) - 1] as Border;
                        Grid brandGridBrand = brandBorderBrand.Child as Grid;

                        Border itemUnitBorderBrand = masterGrid.Children[Int32.Parse(textBox.Tag.ToString()) +1] as Border;
                        Grid itemUnitGridBrand = itemUnitBorderBrand.Child as Grid;

                        Border pricingCriteriaBorderBrand = masterGrid.Children[Int32.Parse(textBox.Tag.ToString()) + 2] as Border;
                        Grid pricingCriteriaGridBrand = pricingCriteriaBorderBrand.Child as Grid;

                        genericModel.SetCategoryId(Int32.Parse(categoryGridBrand.Tag.ToString()));
                        genericModel.SetProductId(Int32.Parse(typeGridBrand.Tag.ToString()));
                        genericModel.SetBrandId(Int32.Parse(brandGridBrand.Tag.ToString()));
                        genericModel.SetModelId(Int32.Parse(parent.Tag.ToString()));
                        genericModel.SetModelitemUnit(Int32.Parse(itemUnitGridBrand.Tag.ToString()));
                        genericModel.SetModelpricingCriteria(Int32.Parse(pricingCriteriaGridBrand.Tag.ToString()));
                       
                            if (genericModel.UpdateModel(textBox.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
                            {
                                UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), textBox, null, label, masterGrid);
                            }
                            else
                            {
                                label.Visibility = Visibility.Visible;
                                label.Text = textBox.Text;
                                label.Foreground = System.Windows.Media.Brushes.Red;
                                textBox.Visibility = Visibility.Collapsed;
                            }
                       
                        
                        break;
                    default: break;
                }
            }
        }

       // private void OnSelChangedComboBox(object sender, SelectionChangedEventArgs e)
       // {
       //     
       //     ComboBox comboBox = sender as ComboBox;
       //     if (comboBox.SelectedIndex != -1)
       //     {
       //         Grid parent = comboBox.Parent as Grid;
       //         TextBlock label = parent.Children[0] as TextBlock;
       //         Border parentOfGrid = parent.Parent as Border;
       //         Grid masterGrid = parentOfGrid.Parent as Grid;
       //
       //         switch (Int32.Parse(label.Tag.ToString()))
       //         {
       //             case ITEM_UNIT_COLUMN:
       //                 Border categoryBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
       //                 Grid categoryGridItemUnit = categoryBorderItemUnit.Child as Grid;
       //
       //                 Border typeBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
       //                 Grid typeGridItemUnit = typeBorderItemUnit.Child as Grid;
       //
       //                 Border brandBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
       //                 Grid brandGridItemUnit = brandBorderItemUnit.Child as Grid;
       //
       //                 Border ModelBorder = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
       //                 Grid ModelGrid = ModelBorder.Child as Grid;
       //                 TextBlock modelName = ModelGrid.Children[0] as TextBlock;
       //
       //                 Border pricingCriteriaBorderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) + 1] as Border;
       //                 Grid pricingCriteriaGridItemUnit = pricingCriteriaBorderItemUnit.Child as Grid;
       //
       //                 genericModel.SetCategoryId(Int32.Parse(categoryGridItemUnit.Tag.ToString()));
       //                 genericModel.SetProductId(Int32.Parse(typeGridItemUnit.Tag.ToString()));
       //                 genericModel.SetBrandId(Int32.Parse(brandGridItemUnit.Tag.ToString()));
       //                 genericModel.SetModelId(Int32.Parse(ModelGrid.Tag.ToString()));
       //                 genericModel.SetModelitemUnit(unitList[comboBox.SelectedIndex].measure_unit_id);
       //                 genericModel.SetModelpricingCriteria(Int32.Parse(pricingCriteriaGridItemUnit.Tag.ToString()));
       //                 if (genericModel.UpdateModel(modelName.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
       //                 {
       //                     UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
       //                 }
       //                 else
       //                 {
       //                     label.Visibility = Visibility.Visible;
       //                     label.Text = comboBox.Text;
       //                     label.Foreground = System.Windows.Media.Brushes.Red;
       //                     comboBox.Visibility = Visibility.Collapsed;
       //                 }
       //                 break;
       //             case PRICING_CRITERIA_COLUMN:
       //                 Border categoryBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 5] as Border;
       //                 Grid categoryGridPricingCriteria = categoryBorderPricingCriteria.Child as Grid;
       //
       //                 Border typeBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 4] as Border;
       //                 Grid typeGridPricingCriteria = typeBorderPricingCriteria.Child as Grid;
       //
       //                 Border brandBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 3] as Border;
       //                 Grid brandGridPricingCriteria = brandBorderPricingCriteria.Child as Grid;
       //
       //                 Border ModelBorderPricingCriteria = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 2] as Border;
       //                 Grid ModelGridPricingCriteria = ModelBorderPricingCriteria.Child as Grid;
       //                 TextBlock modelName1 = ModelGridPricingCriteria.Children[0] as TextBlock;
       //
       //                 Border borderItemUnit = masterGrid.Children[Int32.Parse(comboBox.Tag.ToString()) - 1] as Border;
       //                 Grid gridItemUnit = borderItemUnit.Child as Grid;
       //
       //                 genericModel.SetCategoryId(Int32.Parse(categoryGridPricingCriteria.Tag.ToString()));
       //                 genericModel.SetProductId(Int32.Parse(typeGridPricingCriteria.Tag.ToString()));
       //                 genericModel.SetBrandId(Int32.Parse(brandGridPricingCriteria.Tag.ToString()));
       //                 genericModel.SetModelId(Int32.Parse(ModelGridPricingCriteria.Tag.ToString()));
       //                 genericModel.SetModelitemUnit(Int32.Parse(gridItemUnit.Tag.ToString()));
       //                 genericModel.SetModelpricingCriteria(pricingCriteriaList[comboBox.SelectedIndex].pricing_criteria_id);
       //                 if (genericModel.UpdateModel(modelName1.Text, genericModel.GetModelUnitId(), true, genericModel.GetModelPricingCriteria()))
       //                 {
       //                     UpdateColumn(Int32.Parse(label.Tag.ToString()), Int32.Parse(parent.Tag.ToString()), null, comboBox, label, masterGrid);
       //                 }
       //                 else
       //                 {
       //                     label.Visibility = Visibility.Visible;
       //                     label.Text = comboBox.Text;
       //                     label.Foreground = System.Windows.Media.Brushes.Red;
       //                     comboBox.Visibility = Visibility.Collapsed;
       //                 }
       //                 break;
       //         }
       //     }
       // }

        public void UpdateColumn(int column , int ContentId,TextBox textBox ,ComboBox comboBox, TextBlock textBlock , Grid grid)
        {
            int id = ContentId;
            switch (column)
            {
                case CATEGORY_COLUMN:
                    if (newRowsAdded == 0)
                    {
                        for (int i = 8; i < grid.Children.Count; i += 8)
                        {
                            Border border = grid.Children[i] as Border;
                            Grid borderChild = border.Child as Grid;
                            if (Int32.Parse(borderChild.Tag.ToString()) == id)
                            {
                                textBlock = borderChild.Children[0] as TextBlock;
                                textBlock.Text = comboBox.Text;
                                comboBox.Visibility = Visibility.Collapsed;
                                textBlock.Visibility = Visibility.Visible;
                                textBlock.Foreground = System.Windows.Media.Brushes.Green;
                            }
                        }
                    }
                    else
                    {
                        ////////////////////////////// ADDED A NEW ROW AND THE TAG ISN'T SETTED YET ///////////////////////////////////////////////////////
                        int newChildren=newRowsAdded * 8;
                        int masterGridOldChildren = grid.Children.Count-newChildren;
                        for (int i = 8; i < masterGridOldChildren; i += 8)
                        {
                            Border border = grid.Children[i] as Border;
                            Grid borderChild = border.Child as Grid;
                            if (Int32.Parse(borderChild.Tag.ToString()) == id)
                            {
                                textBlock = borderChild.Children[0] as TextBlock;
                                textBlock.Text = comboBox.Text;
                                comboBox.Visibility = Visibility.Collapsed;
                                textBlock.Visibility = Visibility.Visible;
                                textBlock.Foreground = System.Windows.Media.Brushes.Green;
                            }
                        }
                        for(int i= masterGridOldChildren; i< grid.Children.Count; i+=8)
                        {
                            Border border = grid.Children[i] as Border;
                            Grid borderChild = border.Child as Grid;
                            ComboBox combobox = borderChild.Children[1] as ComboBox;
                            combobox.Items.Clear();
                            FillCategoryList();
                            for(int j=0;j<categoryList.Count;j++ )
                            {
                                combobox.Items.Add(categoryList[j].category_name);
                            }

                        }
                    }
                    break;
                case TYPE_COLUMN:
                    for(int i=9;i < grid.Children.Count;i+= 8)
                    {
                        Border categoryborder = grid.Children[i-1] as Border;
                        Grid categoryborderChild = categoryborder.Child as Grid;
                        if(Int32.Parse(categoryborderChild.Tag.ToString())== genericModel.GetCategoryId())
                        {
                            Border typeBorder = grid.Children[i] as Border;
                            Grid typeBorderChild = typeBorder.Child as Grid;
                            if(id== Int32.Parse(typeBorderChild.Tag.ToString()))
                            {
                                textBlock = typeBorderChild.Children[0] as TextBlock;
                                textBlock.Text = comboBox.Text;
                                comboBox.Visibility = Visibility.Collapsed;
                                textBlock.Visibility = Visibility.Visible;
                                textBlock.Foreground = System.Windows.Media.Brushes.Green;
                            }
                        }
                       

                    }
                    break;
                case BRAND_COLUMN:
                    //for (int i = 8; i < grid.Children.Count; i += 6)
                    //{
                    //    Border categoryborder = grid.Children[i - 2] as Border;
                    //    Grid categoryborderChild = categoryborder.Child as Grid;
                    //
                    //    Border typeborder = grid.Children[i - 1] as Border;
                    //    Grid typeborderChild = typeborder.Child as Grid;
                    //    if (Int32.Parse(categoryborderChild.Tag.ToString()) == genericModel.GetCategoryId())
                    //    {
                    //        if (Int32.Parse(typeborderChild.Tag.ToString()) == genericModel.GetBrandId())
                    //        {
                    //            Border brandBorder = grid.Children[i] as Border;
                    //            Grid brandBorderChild = brandBorder.Child as Grid;
                    //            if (id == Int32.Parse(brandBorderChild.Tag.ToString()))
                    //            {
                    //                textBlock = brandBorderChild.Children[0] as TextBlock;
                    //                textBlock.Text = textBox.Text;
                    //                textBox.Visibility = Visibility.Collapsed;
                    //                textBlock.Visibility = Visibility.Visible;
                    //                textBlock.Foreground = System.Windows.Media.Brushes.Green;
                    //            }
                    //        }
                    //    }


                    //}
                    break;
                case MODEL_COLUMN:
                    for (int i = 11; i < grid.Children.Count; i += 8)
                    {
                        Border categoryborder = grid.Children[i - 3] as Border;
                        Grid categoryborderChild = categoryborder.Child as Grid;

                        Border typeborder = grid.Children[i - 2] as Border;
                        Grid typeborderChild = typeborder.Child as Grid;

                        Border brandborder = grid.Children[i - 1] as Border;
                        Grid brandborderChild = brandborder.Child as Grid;

                        if (Int32.Parse(categoryborderChild.Tag.ToString()) == genericModel.GetCategoryId())
                        {
                            if (Int32.Parse(typeborderChild.Tag.ToString()) == genericModel.GetProductId())
                            {
                                if (Int32.Parse(brandborderChild.Tag.ToString()) == genericModel.GetBrandId())
                                {
                                    Border ModelBorder = grid.Children[i] as Border;
                                    Grid ModelBorderChild = ModelBorder.Child as Grid;
                                    if (id == Int32.Parse(ModelBorderChild.Tag.ToString()))
                                    {
                                        textBlock = ModelBorderChild.Children[0] as TextBlock;
                                        textBlock.Text = textBox.Text;
                                        textBox.Visibility = Visibility.Collapsed;
                                        textBlock.Visibility = Visibility.Visible;
                                        textBlock.Foreground = System.Windows.Media.Brushes.Green;
                                    }
                                }
                            }
                        }
                    }
                        break;
                case ITEM_UNIT_COLUMN:
                    for (int i = 12; i < grid.Children.Count; i += 8)
                    {
                        Border categoryborder = grid.Children[i - 4] as Border;
                        Grid categoryborderChild = categoryborder.Child as Grid;

                        Border typeborder = grid.Children[i - 3] as Border;
                        Grid typeborderChild = typeborder.Child as Grid;

                        Border brandborder = grid.Children[i - 2] as Border;
                        Grid brandborderChild = brandborder.Child as Grid;

                        Border modelborder = grid.Children[i - 1] as Border;
                        Grid modelborderChild = modelborder.Child as Grid;

                        if (Int32.Parse(categoryborderChild.Tag.ToString()) == genericModel.GetCategoryId())
                        {
                            if (Int32.Parse(typeborderChild.Tag.ToString()) == genericModel.GetProductId())
                            {
                                if (Int32.Parse(brandborderChild.Tag.ToString()) == genericModel.GetBrandId())
                                {
                                    if (Int32.Parse(modelborderChild.Tag.ToString()) == genericModel.GetModelId())
                                    {
                                        Border itemUnitBorder = grid.Children[i] as Border;
                                        Grid itemUnitBorderChild = itemUnitBorder.Child as Grid;
                                        if (id == Int32.Parse(itemUnitBorderChild.Tag.ToString()))
                                        {
                                            textBlock = itemUnitBorderChild.Children[0] as TextBlock;
                                            textBlock.Text = comboBox.SelectedItem.ToString();
                                            comboBox.Visibility = Visibility.Collapsed;
                                            textBlock.Visibility = Visibility.Visible;
                                            textBlock.Foreground = System.Windows.Media.Brushes.Green;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PRICING_CRITERIA_COLUMN:
                    for (int i = 13; i < grid.Children.Count; i += 8)
                    {
                        Border categoryborder = grid.Children[i - 5] as Border;
                        Grid categoryborderChild = categoryborder.Child as Grid;

                        Border typeborder = grid.Children[i - 4] as Border;
                        Grid typeborderChild = typeborder.Child as Grid;

                        Border brandborder = grid.Children[i - 3] as Border;
                        Grid brandborderChild = brandborder.Child as Grid;

                        Border modelborder = grid.Children[i - 2] as Border;
                        Grid modelborderChild = modelborder.Child as Grid;

                        Border itemborder = grid.Children[i - 1] as Border;
                        Grid itemborderChild = itemborder.Child as Grid;

                        if (Int32.Parse(categoryborderChild.Tag.ToString()) == genericModel.GetCategoryId())
                        {
                            if (Int32.Parse(typeborderChild.Tag.ToString()) == genericModel.GetProductId())
                            {
                                if (Int32.Parse(brandborderChild.Tag.ToString()) == genericModel.GetBrandId())
                                {
                                    if (Int32.Parse(modelborderChild.Tag.ToString()) == genericModel.GetModelId())
                                    {
                                        if (Int32.Parse(itemborderChild.Tag.ToString()) == genericModel.GetModelUnitId())
                                        {
                                            Border pricingCreteriaBorder = grid.Children[i] as Border;
                                            Grid pricingCreteriaBorderChild = pricingCreteriaBorder.Child as Grid;
                                            if (id == Int32.Parse(pricingCreteriaBorderChild.Tag.ToString()))
                                            {
                                                textBlock = pricingCreteriaBorderChild.Children[0] as TextBlock;
                                                textBlock.Text = comboBox.SelectedItem.ToString();
                                                comboBox.Visibility = Visibility.Collapsed;
                                                textBlock.Visibility = Visibility.Visible;
                                                textBlock.Foreground = System.Windows.Media.Brushes.Green;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
          
        }
        private void AddNewRow(ref Grid masterGrid , ref int rowCount , int items)
        {
            RowDefinition newRow = new RowDefinition();
            masterGrid.RowDefinitions.Add(newRow);
           
            if (rowCount % 2 == 0)
            {
                for (int i = 0; i < items; i++)
                {
                    if(i==items-1)
                    {
                        Border border = new Border();
                        border.Style = (Style)FindResource("EvenRow");
                        System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                        DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                        newDropShadowEffect.BlurRadius = 20;
                        newDropShadowEffect.Color = color;
                        border.Effect = newDropShadowEffect;
                        BitmapImage imgeSource = new BitmapImage();
                        imgeSource.BeginInit();
                        imgeSource.UriSource = new Uri(@"Icons\checkSign.png", UriKind.Relative);
                        imgeSource.EndInit();
                        System.Windows.Controls.Image checkSign = new System.Windows.Controls.Image();
                        checkSign.Source = imgeSource;
                        checkSign.Width = 30;
                        checkSign.Height = 30;
                        checkSign.Tag = position++;
                        checkSign.MouseDown += OnButtonClickCheckSign;
                        Grid grid = new Grid();
                        border.Child = grid;

                        BitmapImage imgSource = new BitmapImage();
                        imgSource.BeginInit();
                        imgSource.UriSource = new Uri(@"Icons\crossRed.png", UriKind.Relative);
                        imgSource.EndInit();
                        System.Windows.Controls.Image deleteRow = new System.Windows.Controls.Image();
                        deleteRow.Source = imgSource;
                        deleteRow.Tag = position++;
                        deleteRow.Width = 25;
                        deleteRow.Height = 25;
                        deleteRow.MouseDown += OnButtonClickDeleteRow;
                        deleteRow.Visibility = Visibility.Collapsed;
                        grid.Children.Add(checkSign);
                        grid.Children.Add(deleteRow);
                        Grid.SetRow(border, rowCount);
                        Grid.SetColumn(border, i);
                        masterGrid.Children.Add(border);

                    }
                    else
                    {

                        Border border = new Border();
                        border.Style = (Style)FindResource("EvenRow");
                        System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                        DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                        newDropShadowEffect.BlurRadius = 20;
                        newDropShadowEffect.Color = color;
                        border.Effect = newDropShadowEffect;
                        Grid grid = new Grid();
                        border.Child = grid;
                        TextBlock textBlock = new TextBlock();
                        textBlock.Style = (Style)FindResource("labelStyleCardOdd");
                        textBlock.Tag = i;
                        textBlock.MouseDown += OnMouseDownToUpdateNames;
                        textBlock.Background = System.Windows.Media.Brushes.Transparent;
                        textBlock.Visibility = Visibility.Collapsed;
                        ComboBox comboBox = new ComboBox();
                        TextBox textBox = new TextBox();
                        CheckBox checkBox = new CheckBox();
                        checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                        checkBox.VerticalAlignment= VerticalAlignment.Center;
                        if (i==MODEL_COLUMN)
                        {
                           
                            textBox.Tag = position++;
                            textBox.MouseLeave += OnMouseLeaveModelTextBoxWhenANewRowIsAdded;
                            textBox.IsEnabled = false;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(textBox);
                        }
                        else if(i==HAS_SERIAL_NUMBER)
                        {
                            checkBox.IsEnabled = false;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(checkBox);
                        }
                        else
                        {
                           
                            comboBox.IsEditable = true;
                            comboBox.Tag = position++;
                           // comboBox.SelectionChanged += OnSelectionChangedComboBoxNewRowAdded;
                            comboBox.MouseLeave += OnMouseLeaveWhenANewRowIsAdded;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(comboBox);
                        }
                     
                        if (i != CATEGORY_COLUMN)
                        {
                            comboBox.IsEnabled = false;
                            textBox.IsEnabled = false;
                            checkBox.IsEnabled = false;

                        }
                        else
                        {
                            
                            for(int j=0; j<categoryList.Count;j++)
                            {
                                comboBox.Items.Add(categoryList[j].category_name);
                            }
                        }
                        Grid.SetRow(border, rowCount);
                        Grid.SetColumn(border, i);
                        masterGrid.Children.Add(border);
                    }
                   
                }

            }
            else
            {
                for (int i = 0; i < items; i++)
                {
                    if (i == items - 1)
                    {
                        Border border = new Border();
                        border.Style = (Style)FindResource("OddRow");
                        System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                        DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                        newDropShadowEffect.BlurRadius = 20;
                        newDropShadowEffect.Color = color;
                        border.Effect = newDropShadowEffect;
                        BitmapImage imgeSource = new BitmapImage();
                        imgeSource.BeginInit();
                        imgeSource.UriSource = new Uri(@"Icons\checkSign.png", UriKind.Relative);
                        imgeSource.EndInit();
                        System.Windows.Controls.Image checkSign = new System.Windows.Controls.Image();
                        checkSign.Source = imgeSource;
                        checkSign.Width = 30;
                        checkSign.Height = 30;
                        checkSign.Tag = position++;
                        checkSign.MouseDown += OnButtonClickCheckSign;
                        Grid grid = new Grid();
                        border.Child = grid;

                        BitmapImage imgSource = new BitmapImage();
                        imgSource.BeginInit();
                        imgSource.UriSource = new Uri(@"Icons\crossRed.png", UriKind.Relative);
                        imgSource.EndInit();
                        System.Windows.Controls.Image deleteRow = new System.Windows.Controls.Image();
                        deleteRow.Source = imgSource;
                        deleteRow.Tag = position++;
                        deleteRow.Width = 25;
                        deleteRow.Height = 25;
                        deleteRow.MouseDown += OnButtonClickDeleteRow;
                        deleteRow.Visibility = Visibility.Collapsed;
                        grid.Children.Add(checkSign);
                        grid.Children.Add(deleteRow);

                        Grid.SetRow(border, rowCount);
                        Grid.SetColumn(border, i);
                        masterGrid.Children.Add(border);

                    }
                    else
                    {
                        Border border = new Border();
                        border.Style = (Style)FindResource("OddRow");
                        System.Windows.Media.Color color = (System.Windows.Media.Brushes.LightGray).Color;
                        DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                        newDropShadowEffect.BlurRadius = 20;
                        newDropShadowEffect.Color = color;
                        border.Effect = newDropShadowEffect;
                        Grid grid = new Grid();
                        border.Child = grid;
                        TextBlock textBlock = new TextBlock();
                        textBlock.Style = (Style)FindResource("labelStyleCardOdd");
                        textBlock.Tag = i;
                        textBlock.MouseDown += OnMouseDownToUpdateNames;
                        textBlock.Background = System.Windows.Media.Brushes.Transparent;
                        textBlock.Visibility = Visibility.Collapsed;
                        ComboBox comboBox = new ComboBox();
                        TextBox textBox = new TextBox();
                        CheckBox checkBox = new CheckBox();
                        checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                        checkBox.VerticalAlignment = VerticalAlignment.Center;
                        if (i == MODEL_COLUMN)
                        {

                            textBox.Tag = position++;
                            textBox.MouseLeave += OnMouseLeaveModelTextBoxWhenANewRowIsAdded;
                            textBox.IsEnabled = false;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(textBox);
                        }
                        else if (i == HAS_SERIAL_NUMBER)
                        {
                            checkBox.IsEnabled = false;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(checkBox);
                        }
                        else
                        {

                            comboBox.IsEditable = true;
                            comboBox.Tag = position++;
                           // comboBox.SelectionChanged += OnSelectionChangedComboBoxNewRowAdded;
                            comboBox.MouseLeave += OnMouseLeaveWhenANewRowIsAdded;
                            grid.Children.Add(textBlock);
                            grid.Children.Add(comboBox);
                        }
                        if (i != CATEGORY_COLUMN)
                        {
                            comboBox.IsEnabled = false;
                            textBox.IsEnabled = false;
                            checkBox.IsEnabled = false;
                        }
                        else
                        {
                            for (int j = 0; j < categoryList.Count; j++)
                            {
                                comboBox.Items.Add(categoryList[j].category_name);
                            }
                        }
                        Grid.SetRow(border, rowCount);
                        Grid.SetColumn(border, i);
                        masterGrid.Children.Add(border);
                    }
                }
            }

            rowCount += 1;
        }

        private void OnButtonClickDeleteRow(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image deleteRow = (System.Windows.Controls.Image)sender;
            Border imageParent = deleteRow.Parent as Border;
            Grid masterGrid = imageParent.Parent as Grid;
            int imagePosition = Int32.Parse(deleteRow.Tag.ToString());

            Border categoryBorder = masterGrid.Children[imagePosition - 7] as Border;
            Grid categoryGrid = categoryBorder.Child as Grid;
            TextBlock categoryTextBlock = categoryGrid.Children[0] as TextBlock;
            int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

            Border typeBorder = masterGrid.Children[imagePosition - 6] as Border;
            Grid typeGrid = typeBorder.Child as Grid;
            TextBlock typeTextBlock = typeGrid.Children[0] as TextBlock;
            int typeId = Int32.Parse(typeGrid.Tag.ToString());

            Border brandBorder = masterGrid.Children[imagePosition - 5] as Border;
            Grid brandGrid = brandBorder.Child as Grid;
            TextBlock brandTextBlock = brandGrid.Children[0] as TextBlock;
            int brandId = Int32.Parse(brandGrid.Tag.ToString());

            Border modelBorder = masterGrid.Children[imagePosition - 4] as Border;
            Grid modelGrid = modelBorder.Child as Grid;
            int modelId = Int32.Parse(modelGrid.Tag.ToString());

            genericModel.SetCategoryId(categoryId);
            genericModel.SetProductId(typeId);
            genericModel.SetBrandId(brandId);
            genericModel.SetModelId(modelId);
            if (modelId != 0)
            {
                if (!genericModel.DeleteModel())
                    System.Windows.Forms.MessageBox.Show("Couldn't Delete The Model.  Please report this to your system adminstrator ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else 
                    InitializeTableView();
            }
            else
            {
                if(brandId != 0)
                {
                    if (!genericModel.DeleteFromProductBrands())
                        System.Windows.Forms.MessageBox.Show("Couldn't Delete The Brand.  Please report this to your system adminstrator", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                        InitializeTableView();
                }
                else if(typeId !=0)
                {
                    if(!genericModel.DeleteFromProduct())
                        System.Windows.Forms.MessageBox.Show("Couldn't Delete The Type. Please report this to your system adminstrator.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                        InitializeTableView();
                }
                else if(categoryId !=0)
                {
                    if(!genericModel.DeleteFromCategory())
                        System.Windows.Forms.MessageBox.Show("Couldn't Delete The Type. Please report this to your system adminstrator.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    else
                        InitializeTableView();
                }
            }
        }

        private void OnButtonClickCheckSign(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image checkSign = (System.Windows.Controls.Image)sender;
            int positionOfTheCheckSign = Int32.Parse(checkSign.Tag.ToString());
            Grid parent = checkSign.Parent as Grid;
            Border parentOfParent = parent.Parent as Border;
            Grid masterGrid = parentOfParent.Parent as Grid;

            System.Windows.Controls.Image deleteRow = parent.Children[1] as System.Windows.Controls.Image;

            Border categoryBorder = masterGrid.Children[positionOfTheCheckSign - 6] as Border;
            Grid categoryGrid = categoryBorder.Child as Grid;
            ComboBox categoryComboBox = categoryGrid.Children[1] as ComboBox;
            TextBlock categoryTextBlock = categoryGrid.Children[0] as TextBlock;

            Border typeBorder = masterGrid.Children[positionOfTheCheckSign - 5] as Border;
            Grid typeGrid = typeBorder.Child as Grid;
            ComboBox typeComboBox = typeGrid.Children[1] as ComboBox;
            TextBlock typeTextBlock = typeGrid.Children[0] as TextBlock;

            Border brandBorder = masterGrid.Children[positionOfTheCheckSign - 4] as Border;
            Grid brandGrid = brandBorder.Child as Grid;
            ComboBox brandComboBox = brandGrid.Children[1] as ComboBox;
            TextBlock brandTextBlock = brandGrid.Children[0] as TextBlock;

            Border modelBorder = masterGrid.Children[positionOfTheCheckSign - 3] as Border;
            Grid modelGrid = modelBorder.Child as Grid;
            TextBox modelTextBox = modelGrid.Children[1] as TextBox;
            TextBlock modelTextBlock = modelGrid.Children[0] as TextBlock;

            Border itemUnit = masterGrid.Children[positionOfTheCheckSign - 2] as Border;
            Grid itemGrid = itemUnit.Child as Grid;
            ComboBox itemComboBox = itemGrid.Children[1] as ComboBox;
            TextBlock itemTextBlock = itemGrid.Children[0] as TextBlock;

            Border pricingBorder = masterGrid.Children[positionOfTheCheckSign - 1] as Border;
            Grid pricingGrid = pricingBorder.Child as Grid;
            ComboBox pricingComboBox = pricingGrid.Children[1] as ComboBox;
            TextBlock pricingTextBlock = pricingGrid.Children[0] as TextBlock;

            Border hasSerialNumberBorder = masterGrid.Children[positionOfTheCheckSign] as Border;
            Grid hasSerialNumberGrid = hasSerialNumberBorder.Child as Grid;
            CheckBox hasSerialNumberCheckBox = hasSerialNumberGrid.Children[1] as CheckBox;
            TextBlock hasSerialNumberTextBlock = hasSerialNumberGrid.Children[0] as TextBlock;

            if(categoryComboBox.SelectedIndex==-1 && categoryComboBox.Text != string.Empty)
            {
                
                    if(typeComboBox.SelectedIndex ==-1 && typeComboBox.Text != string.Empty)
                    {
                       
                            if(brandComboBox.SelectedIndex==-1 && brandComboBox.Text !=string.Empty)
                            {

                                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1)
                                        {
                                            genericModel.SetModelName(modelTextBox.Text);
                                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                genericModel.SetModelHasSerialNumber(true);
                                            else
                                                genericModel.SetModelHasSerialNumber(false);
                                            string errorMsg1 = string.Empty;
                                            string outPutString1 = string.Empty;
                                           
                                                if (!genericModel.IssuNewCategory())
                                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                      else
                                                      {
                                                    
                                                                 if (!genericModel.IssuNewProduct())
                                                                     System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                 else
                                                                 {
                                                                     if (!genericModel.IssuNewBrand())
                                                                         System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                     else
                                                                     {
                                                                         if (!genericModel.IssuNewModel())
                                                                             System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                         else
                                                                         {
                                                           
                                                           
                                                           
                                                           
                                                                             categoryTextBlock.Text = genericModel.GetCategoryName();
                                                                             categoryTextBlock.Visibility = Visibility.Visible;
                                                                             categoryComboBox.Visibility = Visibility.Collapsed;
                                                                             categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                           
                                                                             typeTextBlock.Text = genericModel.GetProductName();
                                                                             typeTextBlock.Visibility = Visibility.Visible;
                                                                             typeComboBox.Visibility = Visibility.Collapsed;
                                                                             typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                           
                                                                             brandTextBlock.Text = genericModel.GetBrandName();
                                                                             brandTextBlock.Visibility = Visibility.Visible;
                                                                             brandComboBox.Visibility = Visibility.Collapsed;
                                                                             brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                                             modelTextBlock.Text = genericModel.GetModelName();
                                                                             modelTextBlock.Visibility = Visibility.Visible;
                                                                             modelTextBox.Visibility = Visibility.Collapsed;
                                                                             modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                                             itemTextBlock.Text = genericModel.GetModelUnitName();
                                                                             itemTextBlock.Visibility = Visibility.Visible;
                                                                             itemComboBox.Visibility = Visibility.Collapsed;
                                                                             itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                                             pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                                                             pricingTextBlock.Visibility = Visibility.Visible;
                                                                             pricingComboBox.Visibility = Visibility.Collapsed;
                                                                             pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                                             if (hasSerialNumberCheckBox.IsChecked == true)
                                                                                 hasSerialNumberTextBlock.Text = "YES";
                                                                             else
                                                                                 hasSerialNumberTextBlock.Text = "NO";
                                                                             hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                                                             hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;
                                                                             hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                                           
                                                                             deleteRow.Visibility = Visibility.Visible;
                                                                             checkSign.Visibility = Visibility.Collapsed;
                                                           
                                                                             newRowsAdded -= 1;
                                                           
                                                                         }
                                                           
                                                                     }
                                                                 }
                                                             
                                                       }
                                           
                                        }
                              
                            }
                            else
                            {
                               if(!genericModel.IssuNewCategory())
                                   System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                               else
                               {
                                   if (!genericModel.IssuNewProduct())
                                       System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                   else
                                   {
                                if (brandComboBox.SelectedIndex != -1)
                                {
                                    genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);
                                    if (!genericModel.IssuproductBrand())
                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    else
                                    {
                                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1)
                                        {
                                            genericModel.SetModelName(modelTextBox.Text);
                                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                genericModel.SetModelHasSerialNumber(true);
                                            else
                                                genericModel.SetModelHasSerialNumber(false);
                                            string errorMsg1 = string.Empty;
                                            string outPutString1 = string.Empty;


                                            if (!genericModel.IssuNewModel())
                                                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                            else
                                            {




                                                categoryTextBlock.Text = genericModel.GetCategoryName();
                                                categoryTextBlock.Visibility = Visibility.Visible;
                                                categoryComboBox.Visibility = Visibility.Collapsed;
                                                categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                                typeTextBlock.Text = genericModel.GetProductName();
                                                typeTextBlock.Visibility = Visibility.Visible;
                                                typeComboBox.Visibility = Visibility.Collapsed;
                                                typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                                brandTextBlock.Text = genericModel.GetBrandName();
                                                brandTextBlock.Visibility = Visibility.Visible;
                                                brandComboBox.Visibility = Visibility.Collapsed;
                                                brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                                modelTextBlock.Text = genericModel.GetModelName();
                                                modelTextBlock.Visibility = Visibility.Visible;
                                                modelTextBox.Visibility = Visibility.Collapsed;
                                                modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                                itemTextBlock.Text = genericModel.GetModelUnitName();
                                                itemTextBlock.Visibility = Visibility.Visible;
                                                itemComboBox.Visibility = Visibility.Collapsed;
                                                itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                                pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                                pricingTextBlock.Visibility = Visibility.Visible;
                                                pricingComboBox.Visibility = Visibility.Collapsed;
                                                pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                                if (hasSerialNumberCheckBox.IsChecked == true)
                                                    hasSerialNumberTextBlock.Text = "YES";
                                                else
                                                    hasSerialNumberTextBlock.Text = "NO";
                                                hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                                hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;
                                                hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                                deleteRow.Visibility = Visibility.Visible;
                                                checkSign.Visibility = Visibility.Collapsed;

                                                newRowsAdded -= 1;

                                            }






                                        }

                                    }
                                }
                                else
                                {
                                    categoryTextBlock.Text = genericModel.GetCategoryName();
                                    categoryTextBlock.Visibility = Visibility.Visible;
                                    categoryComboBox.Visibility = Visibility.Collapsed;
                                    categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    typeTextBlock.Text = genericModel.GetProductName();
                                    typeTextBlock.Visibility = Visibility.Visible;
                                    typeComboBox.Visibility = Visibility.Collapsed;
                                    typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    // brandTextBlock.Text = genericModel.GetBrandName();
                                    brandTextBlock.Visibility = Visibility.Visible;
                                    brandComboBox.Visibility = Visibility.Collapsed;
                                    brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    //  modelTextBlock.Text = genericModel.GetModelName();
                                    modelTextBlock.Visibility = Visibility.Visible;
                                    modelTextBox.Visibility = Visibility.Collapsed;
                                    modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    //  itemTextBlock.Text = genericModel.GetModelUnitName();
                                    itemTextBlock.Visibility = Visibility.Visible;
                                    itemComboBox.Visibility = Visibility.Collapsed;
                                    itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    //pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                    pricingTextBlock.Visibility = Visibility.Visible;
                                    pricingComboBox.Visibility = Visibility.Collapsed;
                                    pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    // if (hasSerialNumberCheckBox.IsChecked == true)
                                    //  hasSerialNumberTextBlock.Text = "YES";
                                    // else
                                    //hasSerialNumberTextBlock.Text = "NO";
                                    hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                    hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                    deleteRow.Visibility = Visibility.Visible;
                                    checkSign.Visibility = Visibility.Collapsed;

                                    newRowsAdded -= 1;
                                }
                             
                                   }
                               }
                       
                            }
                         
                    }
                    else
                    {
                          if (!genericModel.IssuNewCategory())
                              System.Windows.Forms.MessageBox.Show("Category Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                          else
                          {
                              categoryTextBlock.Text = genericModel.GetCategoryName();
                              categoryTextBlock.Visibility = Visibility.Visible;
                              categoryComboBox.Visibility = Visibility.Collapsed;
                              categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                          
                          
                             // typeTextBlock.Text = genericModel.GetProductName();
                              typeTextBlock.Visibility = Visibility.Visible;
                              typeComboBox.Visibility = Visibility.Collapsed;
                              typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                          
                          
                             // brandTextBlock.Text = genericModel.GetBrandName();
                              brandTextBlock.Visibility = Visibility.Visible;
                              brandComboBox.Visibility = Visibility.Collapsed;
                              brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                          
                            //  modelTextBlock.Text = genericModel.GetModelName();
                              modelTextBlock.Visibility = Visibility.Visible;
                              modelTextBox.Visibility = Visibility.Collapsed;
                              modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                          
                            //  itemTextBlock.Text = genericModel.GetModelUnitName();
                              itemTextBlock.Visibility = Visibility.Visible;
                              itemComboBox.Visibility = Visibility.Collapsed;
                              itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                          
                              //pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                              pricingTextBlock.Visibility = Visibility.Visible;
                              pricingComboBox.Visibility = Visibility.Collapsed;
                              pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                              hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                              hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                              deleteRow.Visibility = Visibility.Visible;
                              checkSign.Visibility = Visibility.Collapsed;
                          
                              newRowsAdded -= 1;
                          }

                    }    
                
            }
            else if(categoryComboBox.SelectedIndex !=-1)
            {
                genericModel.SetCategoryName(categoryComboBox.SelectedItem.ToString());
                genericModel.SetCategoryId(categoryList[categoryComboBox.SelectedIndex].category_id);
                if (typeComboBox.SelectedIndex == -1 && typeComboBox.Text != string.Empty)
                {

                    if (brandComboBox.SelectedIndex == -1 && brandComboBox.Text != string.Empty)
                    {
                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1 )
                        {
                            genericModel.SetModelName(modelTextBox.Text);
                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                            if (hasSerialNumberCheckBox.IsChecked == true)
                                genericModel.SetModelHasSerialNumber(true);
                            else
                                genericModel.SetModelHasSerialNumber(false);

                            if (!genericModel.IssuNewCategory())
                                System.Windows.Forms.MessageBox.Show("Category Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                if (!genericModel.IssuNewProduct())
                                    System.Windows.Forms.MessageBox.Show("Product Type Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                else
                                {
                                    if (!genericModel.IssuNewBrand())
                                        System.Windows.Forms.MessageBox.Show("Model Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    else
                                    {
                                        if (!genericModel.IssuNewModel())
                                            MessageBox.Show("Error");
                                        else
                                        {



                                            categoryTextBlock.Text = genericModel.GetCategoryName();
                                            categoryTextBlock.Visibility = Visibility.Visible;
                                            categoryComboBox.Visibility = Visibility.Collapsed;
                                            categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                            typeTextBlock.Text = genericModel.GetProductName();
                                            typeTextBlock.Visibility = Visibility.Visible;
                                            typeComboBox.Visibility = Visibility.Collapsed;
                                            typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                            brandTextBlock.Text = genericModel.GetBrandName();
                                            brandTextBlock.Visibility = Visibility.Visible;
                                            brandComboBox.Visibility = Visibility.Collapsed;
                                            brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            modelTextBlock.Text = genericModel.GetModelName();
                                            modelTextBlock.Visibility = Visibility.Visible;
                                            modelTextBox.Visibility = Visibility.Collapsed;
                                            modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            itemTextBlock.Text = genericModel.GetModelUnitName();
                                            itemTextBlock.Visibility = Visibility.Visible;
                                            itemComboBox.Visibility = Visibility.Collapsed;
                                            itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                            pricingTextBlock.Visibility = Visibility.Visible;
                                            pricingComboBox.Visibility = Visibility.Collapsed;
                                            pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                hasSerialNumberTextBlock.Text = "YES";
                                            else
                                                hasSerialNumberTextBlock.Text = "NO";
                                            hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                            hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                            hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                            deleteRow.Visibility = Visibility.Visible;
                                            checkSign.Visibility = Visibility.Collapsed;

                                            newRowsAdded -= 1;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if(!genericModel.IssuNewBrand())
                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                categoryTextBlock.Text = genericModel.GetCategoryName();
                                categoryTextBlock.Visibility = Visibility.Visible;
                                categoryComboBox.Visibility = Visibility.Collapsed;
                                categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                typeTextBlock.Text = genericModel.GetProductName();
                                typeTextBlock.Visibility = Visibility.Visible;
                                typeComboBox.Visibility = Visibility.Collapsed;
                                typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                brandTextBlock.Text = genericModel.GetBrandName();
                                brandTextBlock.Visibility = Visibility.Visible;
                                brandComboBox.Visibility = Visibility.Collapsed;
                                brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                             //   modelTextBlock.Text = genericModel.GetModelName();
                                modelTextBlock.Visibility = Visibility.Visible;
                                modelTextBox.Visibility = Visibility.Collapsed;
                                modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                //itemTextBlock.Text = genericModel.GetModelUnitName();
                                itemTextBlock.Visibility = Visibility.Visible;
                                itemComboBox.Visibility = Visibility.Collapsed;
                                itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                              //  pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                pricingTextBlock.Visibility = Visibility.Visible;
                                pricingComboBox.Visibility = Visibility.Collapsed;
                                pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                              //  if (hasSerialNumberCheckBox.IsChecked == true)
                              //      hasSerialNumberTextBlock.Text = "YES";
                              //  else
                                 //  hasSerialNumberTextBlock.Text = "NO";
                                hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                deleteRow.Visibility = Visibility.Visible;
                                checkSign.Visibility = Visibility.Collapsed;

                                newRowsAdded -= 1;
                            }

                        }
                       // else if (modelTextBox.Text == string.Empty)
                       // {
                       //     System.Windows.Forms.MessageBox.Show("Model Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                       // }
                       // else if (itemComboBox.SelectedIndex == -1)
                       // {
                       //     System.Windows.Forms.MessageBox.Show("Item Price Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                       // }
                       // else if (pricingComboBox.SelectedIndex == -1)
                       // {
                       //     System.Windows.Forms.MessageBox.Show("Pricing Criteria Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                       // }

                    }
                    else if (brandComboBox.SelectedIndex != -1)
                    {
                        genericModel.SetProductName(typeComboBox.Text);
                        genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);
                        genericModel.SetBrandName(brandList[brandComboBox.SelectedIndex].brand_name);

                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1)
                        {
                            genericModel.SetModelName(modelTextBox.Text);
                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                            if (hasSerialNumberCheckBox.IsChecked == true)
                                genericModel.SetModelHasSerialNumber(true);
                            else
                                genericModel.SetModelHasSerialNumber(false);

                            if (!genericModel.IssuNewCategory())
                                System.Windows.Forms.MessageBox.Show("Category Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                if (!genericModel.IssuNewProduct())
                                    System.Windows.Forms.MessageBox.Show("Product Type Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                else
                                {
                                    if (!genericModel.IssuNewBrand())
                                        System.Windows.Forms.MessageBox.Show("Model Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    else
                                    {
                                        if (!genericModel.IssuNewModel())
                                            MessageBox.Show("Error");
                                        else
                                        {



                                            categoryTextBlock.Text = genericModel.GetCategoryName();
                                            categoryTextBlock.Visibility = Visibility.Visible;
                                            categoryComboBox.Visibility = Visibility.Collapsed;
                                            categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                            typeTextBlock.Text = genericModel.GetProductName();
                                            typeTextBlock.Visibility = Visibility.Visible;
                                            typeComboBox.Visibility = Visibility.Collapsed;
                                            typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                            brandTextBlock.Text = genericModel.GetBrandName();
                                            brandTextBlock.Visibility = Visibility.Visible;
                                            brandComboBox.Visibility = Visibility.Collapsed;
                                            brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            modelTextBlock.Text = genericModel.GetModelName();
                                            modelTextBlock.Visibility = Visibility.Visible;
                                            modelTextBox.Visibility = Visibility.Collapsed;
                                            modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            itemTextBlock.Text = genericModel.GetModelUnitName();
                                            itemTextBlock.Visibility = Visibility.Visible;
                                            itemComboBox.Visibility = Visibility.Collapsed;
                                            itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                            pricingTextBlock.Visibility = Visibility.Visible;
                                            pricingComboBox.Visibility = Visibility.Collapsed;
                                            pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                hasSerialNumberTextBlock.Text = "YES";
                                            else
                                                hasSerialNumberTextBlock.Text = "NO";
                                            hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                            hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                            hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                            deleteRow.Visibility = Visibility.Visible;
                                            checkSign.Visibility = Visibility.Collapsed;

                                            newRowsAdded -= 1;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!genericModel.IssuNewProduct())
                                System.Windows.Forms.MessageBox.Show("Product Type Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                if (!genericModel.IssuNewBrand())
                                    System.Windows.Forms.MessageBox.Show("Product Type Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                else
                                {
                                    categoryTextBlock.Text = genericModel.GetCategoryName();
                                    categoryTextBlock.Visibility = Visibility.Visible;
                                    categoryComboBox.Visibility = Visibility.Collapsed;
                                    categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    typeTextBlock.Text = genericModel.GetProductName();
                                    typeTextBlock.Visibility = Visibility.Visible;
                                    typeComboBox.Visibility = Visibility.Collapsed;
                                    typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    brandTextBlock.Text = genericModel.GetBrandName();
                                    brandTextBlock.Visibility = Visibility.Visible;
                                    brandComboBox.Visibility = Visibility.Collapsed;
                                    brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    //modelTextBlock.Text = genericModel.GetModelName();
                                    modelTextBlock.Visibility = Visibility.Visible;
                                    modelTextBox.Visibility = Visibility.Collapsed;
                                    modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                   // itemTextBlock.Text = genericModel.GetModelUnitName();
                                    itemTextBlock.Visibility = Visibility.Visible;
                                    itemComboBox.Visibility = Visibility.Collapsed;
                                    itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    //pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                    pricingTextBlock.Visibility = Visibility.Visible;
                                    pricingComboBox.Visibility = Visibility.Collapsed;
                                    pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    if (hasSerialNumberCheckBox.IsChecked == true)
                                        hasSerialNumberTextBlock.Text = "YES";
                                    else
                                        hasSerialNumberTextBlock.Text = "NO";
                                    hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                    hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                    hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                    deleteRow.Visibility = Visibility.Visible;
                                    checkSign.Visibility = Visibility.Collapsed;

                                    newRowsAdded -= 1;
                                }
                            }

                        }



                    }
                    else
                    {
                        if(!genericModel.IssuNewProduct())
                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        else
                        {
                            categoryTextBlock.Text = genericModel.GetCategoryName();
                            categoryTextBlock.Visibility = Visibility.Visible;
                            categoryComboBox.Visibility = Visibility.Collapsed;
                            categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                            typeTextBlock.Text = genericModel.GetProductName();
                            typeTextBlock.Visibility = Visibility.Visible;
                            typeComboBox.Visibility = Visibility.Collapsed;
                            typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                          //  brandTextBlock.Text = genericModel.GetBrandName();
                            brandTextBlock.Visibility = Visibility.Visible;
                            brandComboBox.Visibility = Visibility.Collapsed;
                            brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                          //  modelTextBlock.Text = genericModel.GetModelName();
                            modelTextBlock.Visibility = Visibility.Visible;
                            modelTextBox.Visibility = Visibility.Collapsed;
                            modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                           // itemTextBlock.Text = genericModel.GetModelUnitName();
                            itemTextBlock.Visibility = Visibility.Visible;
                            itemComboBox.Visibility = Visibility.Collapsed;
                            itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                           // pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                            pricingTextBlock.Visibility = Visibility.Visible;
                            pricingComboBox.Visibility = Visibility.Collapsed;
                            pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                          //  if (hasSerialNumberCheckBox.IsChecked == true)
                          //    //  hasSerialNumberTextBlock.Text = "YES";
                          //  else
                              //  hasSerialNumberTextBlock.Text = "NO";
                            hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                            hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                            hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                            deleteRow.Visibility = Visibility.Visible;
                            checkSign.Visibility = Visibility.Collapsed;

                            newRowsAdded -= 1;
                        }
                    }
                   
                }
                else if(typeComboBox.SelectedIndex !=-1)
                {
                    genericModel.SetProductName(typeList[typeComboBox.SelectedIndex].product_name);
                    genericModel.SetProductId(typeList[typeComboBox.SelectedIndex].product_id);

                    if (brandComboBox.SelectedIndex == -1 && brandComboBox.Text != string.Empty)
                    {


                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1)
                        {
                            genericModel.SetModelName(modelTextBox.Text);
                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                            if (!genericModel.IssuNewBrand())
                                System.Windows.Forms.MessageBox.Show("Model Must Be Specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                if (hasSerialNumberCheckBox.IsChecked == true)
                                    genericModel.SetModelHasSerialNumber(true);
                                else
                                    genericModel.SetModelHasSerialNumber(false);
                                if (!genericModel.IssuNewModel())
                                    MessageBox.Show("Error");
                                else
                                {



                                    categoryTextBlock.Text = genericModel.GetCategoryName();
                                    categoryTextBlock.Visibility = Visibility.Visible;
                                    categoryComboBox.Visibility = Visibility.Collapsed;
                                    categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    typeTextBlock.Text = genericModel.GetProductName();
                                    typeTextBlock.Visibility = Visibility.Visible;
                                    typeComboBox.Visibility = Visibility.Collapsed;
                                    typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                    brandTextBlock.Text = genericModel.GetBrandName();
                                    brandTextBlock.Visibility = Visibility.Visible;
                                    brandComboBox.Visibility = Visibility.Collapsed;
                                    brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    modelTextBlock.Text = genericModel.GetModelName();
                                    modelTextBlock.Visibility = Visibility.Visible;
                                    modelTextBox.Visibility = Visibility.Collapsed;
                                    modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    itemTextBlock.Text = genericModel.GetModelUnitName();
                                    itemTextBlock.Visibility = Visibility.Visible;
                                    itemComboBox.Visibility = Visibility.Collapsed;
                                    itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                    pricingTextBlock.Visibility = Visibility.Visible;
                                    pricingComboBox.Visibility = Visibility.Collapsed;
                                    pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                    if (hasSerialNumberCheckBox.IsChecked == true)
                                        hasSerialNumberTextBlock.Text = "YES";
                                    else
                                        hasSerialNumberTextBlock.Text = "NO";
                                    hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                    hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                    hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                    deleteRow.Visibility = Visibility.Visible;
                                    checkSign.Visibility = Visibility.Collapsed;

                                    newRowsAdded -= 1;
                                }

                            }

                        }
                        else
                        {
                            if (!genericModel.IssuNewBrand())
                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            else
                            {
                                categoryTextBlock.Text = genericModel.GetCategoryName();
                                categoryTextBlock.Visibility = Visibility.Visible;
                                categoryComboBox.Visibility = Visibility.Collapsed;
                                categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                typeTextBlock.Text = genericModel.GetProductName();
                                typeTextBlock.Visibility = Visibility.Visible;
                                typeComboBox.Visibility = Visibility.Collapsed;
                                typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                brandTextBlock.Text = genericModel.GetBrandName();
                                brandTextBlock.Visibility = Visibility.Visible;
                                brandComboBox.Visibility = Visibility.Collapsed;
                                brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                //   modelTextBlock.Text = genericModel.GetModelName();
                                modelTextBlock.Visibility = Visibility.Visible;
                                modelTextBox.Visibility = Visibility.Collapsed;
                                modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                //itemTextBlock.Text = genericModel.GetModelUnitName();
                                itemTextBlock.Visibility = Visibility.Visible;
                                itemComboBox.Visibility = Visibility.Collapsed;
                                itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                //  pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                pricingTextBlock.Visibility = Visibility.Visible;
                                pricingComboBox.Visibility = Visibility.Collapsed;
                                pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                //  if (hasSerialNumberCheckBox.IsChecked == true)
                                //      hasSerialNumberTextBlock.Text = "YES";
                                //  else
                                //  hasSerialNumberTextBlock.Text = "NO";
                                hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                deleteRow.Visibility = Visibility.Visible;
                                checkSign.Visibility = Visibility.Collapsed;

                                newRowsAdded -= 1;
                            }
                        }

                    }
                    else if (brandComboBox.SelectedIndex != -1)
                    {
                        genericModel.SetBrandName(brandComboBox.SelectedItem.ToString());
                        genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);

                        if (modelTextBox.Text != string.Empty && itemComboBox.SelectedIndex != -1 && pricingComboBox.SelectedIndex != -1)
                        {
                            genericModel.SetModelName(modelTextBox.Text);
                            genericModel.SetModelitemUnit(unitList[itemComboBox.SelectedIndex].measure_unit_id);
                            genericModel.SetModelitemUnitName(unitList[itemComboBox.SelectedIndex].measure_unit);
                            genericModel.SetModelpricingCriteria(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_id);
                            genericModel.SetModelpricingCriteriaName(pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name);
                            if (hasSerialNumberCheckBox.IsChecked == true)
                                genericModel.SetModelHasSerialNumber(true);
                            else
                                genericModel.SetModelHasSerialNumber(false);
                            if (!genericModel.IssuNewModel())
                                MessageBox.Show("Error");
                            else
                            {



                                categoryTextBlock.Text = genericModel.GetCategoryName();
                                categoryTextBlock.Visibility = Visibility.Visible;
                                categoryComboBox.Visibility = Visibility.Collapsed;
                                categoryTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                typeTextBlock.Text = genericModel.GetProductName();
                                typeTextBlock.Visibility = Visibility.Visible;
                                typeComboBox.Visibility = Visibility.Collapsed;
                                typeTextBlock.Foreground = System.Windows.Media.Brushes.Green;


                                brandTextBlock.Text = genericModel.GetBrandName();
                                brandTextBlock.Visibility = Visibility.Visible;
                                brandComboBox.Visibility = Visibility.Collapsed;
                                brandTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                modelTextBlock.Text = genericModel.GetModelName();
                                modelTextBlock.Visibility = Visibility.Visible;
                                modelTextBox.Visibility = Visibility.Collapsed;
                                modelTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                itemTextBlock.Text = genericModel.GetModelUnitName();
                                itemTextBlock.Visibility = Visibility.Visible;
                                itemComboBox.Visibility = Visibility.Collapsed;
                                itemTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                pricingTextBlock.Text = pricingCriteriaList[pricingComboBox.SelectedIndex].pricing_criteria_name;
                                pricingTextBlock.Visibility = Visibility.Visible;
                                pricingComboBox.Visibility = Visibility.Collapsed;
                                pricingTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                                if (hasSerialNumberCheckBox.IsChecked == true)
                                    hasSerialNumberTextBlock.Text = "YES";
                                else
                                    hasSerialNumberTextBlock.Text = "NO";
                                hasSerialNumberTextBlock.Visibility = Visibility.Visible;
                                hasSerialNumberTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                                hasSerialNumberCheckBox.Visibility = Visibility.Collapsed;

                                deleteRow.Visibility = Visibility.Visible;
                                checkSign.Visibility = Visibility.Collapsed;

                                newRowsAdded -= 1;
                            }
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Type and Category Already Exists", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                       
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Type and Category Already Exists", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Category Already Exists", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Category Must Be Specified", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }



        }

        

        private void OnSelectionChangedComboBoxNewRowAdded(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Grid parent = comboBox.Parent as Grid;
            Border parentOfParent = parent.Parent as Border;
            Grid masterGrid = parentOfParent.Parent as Grid;
            TextBlock textBlock = parent.Children[0] as TextBlock;
            int NextPosition = Int32.Parse(comboBox.Tag.ToString()) + 1;
            int columnNumber = Int32.Parse(textBlock.Tag.ToString());
            if (comboBox.SelectedIndex != -1)
            {
                if (columnNumber == CATEGORY_COLUMN)
                {
                    typeList.Clear();
                    if (!commonQueries.GetGenericProducts(ref typeList, categoryList[comboBox.SelectedIndex].category_id))
                        MessageBox.Show("Error");
                    else
                    {
                        Border border = masterGrid.Children[NextPosition] as Border;
                        Grid grid = border.Child as Grid;
                        ComboBox typeComboBox = grid.Children[1] as ComboBox;
                        typeComboBox.Items.Clear();
                       
                        for (int i = 0; i < typeList.Count; i++)
                        {
                            typeComboBox.Items.Add(typeList[i].product_name);

                        }
                        typeComboBox.IsEnabled = true;
                    }
                }
                else if (columnNumber == TYPE_COLUMN)
                {
                    int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                    Border border = masterGrid.Children[categoryPosition] as Border;
                    Grid grid = border.Child as Grid;
                    ComboBox categoryComboBox = grid.Children[1] as ComboBox;
                    brandList.Clear();
                    if (!commonQueries.GetGenericProductBrands(typeList[comboBox.SelectedIndex].product_id, categoryList[categoryComboBox.SelectedIndex].category_id, ref brandList))
                        MessageBox.Show("error");
                    else
                    {
                        Border brandBorder = masterGrid.Children[NextPosition] as Border;
                        Grid brandGrid = brandBorder.Child as Grid;
                        ComboBox brandComboBox = brandGrid.Children[1] as ComboBox;
                        brandComboBox.Items.Clear();
                        for (int i = 0; i < brandList.Count; i++)
                        {
                            brandComboBox.Items.Add(brandList[i].brand_name);

                        }
                        brandComboBox.IsEnabled = true;
                    }

                }
                else if(columnNumber==BRAND_COLUMN)
                {
                    genericModel.SetBrandName(comboBox.Text);
                    genericModel.SetAddedBy(loggedInUser.GetEmployeeId());
                    int categoryPosition = Int32.Parse(comboBox.Tag.ToString()) - 2;
                    Border categoryBorder = masterGrid.Children[categoryPosition] as Border;
                    Grid categoryGrid = categoryBorder.Child as Grid;
                    ComboBox categoryComboBox = categoryGrid.Children[1] as ComboBox;
                    int categoryId = Int32.Parse(categoryGrid.Tag.ToString());

                    int typePosition = Int32.Parse(comboBox.Tag.ToString()) - 1;
                    Border typeBorder = masterGrid.Children[typePosition] as Border;
                    Grid typeGrid = typeBorder.Child as Grid;
                    ComboBox typeComboBox = typeGrid.Children[1] as ComboBox;
                    int typeId = Int32.Parse(typeGrid.Tag.ToString());

                    genericModel.SetCategoryId(categoryId);
                    genericModel.SetProductId(typeId);
                    genericModel.SetBrandId(brandList[comboBox.SelectedIndex].brand_id);
                      // if (!genericModel.IssuproductBrand())
                      //     System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                      // else
                      // {
                      //     textBlock.Visibility = Visibility.Visible;
                      //     comboBox.Visibility = Visibility.Collapsed;
                      //     textBlock.Text = comboBox.Text;
                      //     textBlock.Foreground = System.Windows.Media.Brushes.Green;
                      // }
                }     
            }
        }

        private void OnButtonClickSortGridView(object sender, RoutedEventArgs e)
        {
            GenericProductsPage page = this;
            SortGridViewGenericProductsWindow sortGenericProductWindow = new SortGridViewGenericProductsWindow(ref loggedInUser , ref sortGenericProducts ,ref page );
            sortGenericProductWindow.Show();
        }

        public void OnClose( ref BASIC_STRUCTS.SORT_GENERIC_PRODUCTS sortGenericProduct)
        {
            sortGenericProducts = sortGenericProduct;
            InitializeTableView();
           
        }

    }
}
