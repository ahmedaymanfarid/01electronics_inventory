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
    /// Interaction logic for AddEntryPermitPage.xaml
    /// </summary>
    public partial class AddEntryPermitPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public  AddEntryPermitItemPage addEntryPermitItem ;

        List<BASIC_STRUCTS.WARE_HOUSE_LOCATION> Locations = new List<BASIC_STRUCTS.WARE_HOUSE_LOCATION>();

        AddEntryPermitWindow entryPermitWindow;

        public MaterialEntryPermit materialEntryPermit = new MaterialEntryPermit();

        public AddEntryPermitPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser,  AddEntryPermitWindow w, bool isedit, ref MaterialEntryPermit noldMaterialEntryPermit)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();
            
            entryPermitWindow = w;

            commonQueries.GetWareHouseLocations(ref Locations);
            Locations.ForEach(a => WareHouseCombo.Items.Add(a.nickName));


            if (isedit == true) {

                materialEntryPermit.SetEntryPermitSerialid(noldMaterialEntryPermit.GetEntryPermitSerialid());

                materialEntryPermit.InitializeMaterialEntryPermit();

            }

                addEntryPermitItem = new AddEntryPermitItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, entryPermitWindow, isedit, ref noldMaterialEntryPermit);


        }



        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            materialEntryPermit.SetEntryPermitId(entryPermitIdTextBox.Text);


            addEntryPermitItem.addEntryPermitPage = this;
            this.NavigationService.Navigate(addEntryPermitItem);

        }

        private void BasicInfoLableMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TransactionDatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            materialEntryPermit.SetTransactionDate(Convert.ToDateTime(TransactionDatePicker.SelectedDate));

        }

        private void WareHouseComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            materialEntryPermit.SetNickName(Locations[WareHouseCombo.SelectedIndex].nickName);
            materialEntryPermit.SetWareHouseLocationId(Locations[WareHouseCombo.SelectedIndex].address.location_id);
            materialEntryPermit.SetAddedBy(loggedInUser.GetEmployeeId());


            LocationTextBox.Text = $"{Locations[WareHouseCombo.SelectedIndex].address.district.district_name + ", " + Locations[WareHouseCombo.SelectedIndex].address.city.city_name + ", " + Locations[WareHouseCombo.SelectedIndex].address.state_governorate.state_name + ", "  + Locations[WareHouseCombo.SelectedIndex].address.country.country_name}";
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            entryPermitWindow.Close();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {

            materialEntryPermit.SetEntryPermitId(entryPermitIdTextBox.Text);

            addEntryPermitItem.addEntryPermitPage = this;

            this.NavigationService.Navigate(addEntryPermitItem);


        }
    }
}
