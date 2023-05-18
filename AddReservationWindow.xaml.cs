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
    /// Interaction logic for AddReservationWindow.xaml
    /// </summary>
    public partial class AddReservationWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public AddReservationBasicInfoPage addReservationBasicInfoPage;
        public AddReservationItemsPage addReservationItemsPage;

        public AddReservationWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref MaterialReservation mMaterialReservation, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            addReservationBasicInfoPage = new AddReservationBasicInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaterialReservation, mViewAddCondition);
            addReservationItemsPage = new AddReservationItemsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaterialReservation, mViewAddCondition);

            addReservationBasicInfoPage.addReservationItemsPage = addReservationItemsPage;
            this.NavigationService.Navigate(addReservationBasicInfoPage);
        }

    }
}
