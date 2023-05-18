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
using _01electronics_library;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddRFPWindow.xaml
    /// </summary>
    public partial class AddRFPWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public RFPBasicInfoPage rfpBasicInfoPage;
        public RFPItemsPage rfpItemsPage;
        public RFPUploadFilesPage rfpUploadFilesPage;

        public AddRFPWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref int mViewAddCondition, ref RFP rfp, bool openFilesPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            rfpBasicInfoPage = new RFPBasicInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref mViewAddCondition, ref rfpItemsPage);
            rfpItemsPage = new RFPItemsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref mViewAddCondition);

            if (mViewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                rfpUploadFilesPage = new RFPUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref rfp, ref mViewAddCondition);
            }
            if (openFilesPage)
            {
                rfpUploadFilesPage.rfpBasicInfoPage = rfpBasicInfoPage;
                rfpUploadFilesPage.rfpItemsPage = rfpItemsPage;

                this.NavigationService.Navigate(rfpUploadFilesPage);
            }
            else
            {
                rfpBasicInfoPage.rfpItemsPage = rfpItemsPage;
                rfpBasicInfoPage.rfpUploadFilesPage = rfpUploadFilesPage;

                this.NavigationService.Navigate(rfpBasicInfoPage);
            }
        }
    }
}
