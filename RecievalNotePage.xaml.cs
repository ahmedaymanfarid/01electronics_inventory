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
    /// Interaction logic for RecievalNotePage.xaml
    /// </summary>
    public partial class RecievalNotePage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        Grid previousGrid = null;

        Expander previousExpander = null;

        List<BASIC_STRUCTS.RECIEVAL_NOTE_MIN_STRUCT> recievalNotes = new List<BASIC_STRUCTS.RECIEVAL_NOTE_MIN_STRUCT>(); 

        public RecievalNotePage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            InitializeUiElements();
        }



        public void InitializeUiElements()
        {

            recievalNotesStackPanel.Children.Clear();

            commonQueries.GetRecievalNotes(ref recievalNotes);



            for (int i = 0; i < recievalNotes.Count; i++)
            {


                Grid card = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
                card.MouseEnter += CardMouseEnter;
                card.MouseLeave += CardMouseLeave;

                card.Tag = recievalNotes[i].recieval_note_serial;

                card.RowDefinitions.Add(new RowDefinition());
                card.RowDefinitions.Add(new RowDefinition());
                card.RowDefinitions.Add(new RowDefinition());
                card.RowDefinitions.Add(new RowDefinition());


                card.ColumnDefinitions.Add(new ColumnDefinition());
                card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });



                Label header = new Label();
                header.Content = $"{recievalNotes[i].recieval_note_id}";

                header.Style = (Style)FindResource("stackPanelItemHeader");

                card.Children.Add(header);

                Grid.SetRow(header, 0);

                Grid.SetColumn(header, 0);



                Label transactionDate = new Label() { Margin = new Thickness(0, 0, 0, 5) };
                transactionDate.Content = $"{recievalNotes[i].recieval_note_date.ToString("yyyy-MM-dd")}";

                transactionDate.Style = (Style)FindResource("stackPanelItemBody");

                card.Children.Add(transactionDate);

                Grid.SetRow(transactionDate, 1);

                Grid.SetColumn(transactionDate, 0);


                Label nickName = new Label();
                nickName.Content = $"{recievalNotes[i].added_by_name}";

                nickName.Style = (Style)FindResource("stackPanelItemBody");
                nickName.HorizontalAlignment = HorizontalAlignment.Left;

                card.Children.Add(nickName);

                Grid.SetRow(nickName, 2);

                Grid.SetColumn(nickName, 0);


                Label releasePermitIdLabel = new Label();
                releasePermitIdLabel.Content = $"RELEASE-PERMIT-ID:{recievalNotes[i].release_permit_id}";

                releasePermitIdLabel.Style = (Style)FindResource("stackPanelItemBody");
                releasePermitIdLabel.HorizontalAlignment = HorizontalAlignment.Left;

                card.Children.Add(releasePermitIdLabel);

                Grid.SetRow(releasePermitIdLabel, 3);

                Grid.SetColumn(releasePermitIdLabel, 0);



                StackPanel expand = new StackPanel();


                Button editButton = new Button();

                editButton.Content = "EDIT";

                editButton.Click += EditButtonClick;

                editButton.Tag = recievalNotes[i].recieval_note_serial;


                Button viewButton = new Button();

                viewButton.Click += ViewButtonClick;
                viewButton.Tag = recievalNotes[i].recieval_note_serial;

                viewButton.Content = "VIEW";

                expand.Children.Add(viewButton);
                expand.Children.Add(editButton);

                Expander expander = new Expander();

                expander.Expanded += ExpanderExpanded;

                expander.Content = expand;


                Grid.SetRow(expander, 0);
                Grid.SetColumn(expander, 1);

                card.Children.Add(expander);

                recievalNotesStackPanel.Children.Add(card);
            }
        }

        private void CardMouseEnter(object sender, MouseEventArgs e)
        {

            if (previousGrid != null)
            {

                Label serialPrevious = previousGrid.Children[0] as Label;
                Label transactionDatePrevious = previousGrid.Children[1] as Label;
                Label nickNamePrevious = previousGrid.Children[2] as Label;
                Label releasePermitIdPrevious = previousGrid.Children[3] as Label;


                previousGrid.Background = Brushes.White;
                serialPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                transactionDatePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                nickNamePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                releasePermitIdPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            }

            Grid card = sender as Grid;
            Label serial = card.Children[0] as Label;
            Label transactionDate = card.Children[1] as Label;
            Label nickName = card.Children[2] as Label;
            Label releasePermitId = card.Children[3] as Label;



            BrushConverter brush = new BrushConverter();
            card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            serial.Foreground = Brushes.White;
            transactionDate.Foreground = Brushes.White;
            nickName.Foreground = Brushes.White;
            releasePermitId.Foreground = Brushes.White;

            previousGrid = card;
        }

        private void CardMouseLeave(object sender, MouseEventArgs e)
        {
            Grid card = sender as Grid;
            if (card.IsMouseOver == false && recievalNotesStackPanel.IsMouseOver == false)
            {

                Label serialPrevious = previousGrid.Children[0] as Label;
                Label transactionDatePrevious = previousGrid.Children[1] as Label;
                Label nickNamePrevious = previousGrid.Children[2] as Label;
                Label releasePermitIdPrevious = previousGrid.Children[3] as Label;


                previousGrid.Background = Brushes.White;
                serialPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                transactionDatePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                nickNamePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                releasePermitIdPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            }

        }



        private void ExpanderExpanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;

            if (previousExpander != null && previousExpander != expander)
            {

                previousExpander.IsExpanded = false;
            }

            previousExpander = expander;

        }

        private void ViewButtonClick(object sender, RoutedEventArgs e)
        {
            Button ViewButton = sender as Button;

            RecievalNoteWindow rentryPermitWindow = new RecievalNoteWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, true);

            rentryPermitWindow.addRecievalNoteItemPage.ViewItems(Convert.ToInt32(ViewButton.Tag));


            rentryPermitWindow.Show();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
        }

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

        }

        private void OnRecievalNoteItemsMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            RecievalNoteWindow rentryPermitWindow = new RecievalNoteWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);

            rentryPermitWindow.Show();



        }
    }
}
