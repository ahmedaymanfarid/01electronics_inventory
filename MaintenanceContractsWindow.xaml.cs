using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for MaintenanceContractsWindow.xaml
    /// </summary>
    public partial class MaintenanceContractsWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsProjectsPage maintContractsProjectInfoPage;
        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;

        public MaintenanceContractsWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, bool openFilesPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            maintContractsAdditionalInfoPage = new MaintContractsAdditionalInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition);
            maintContractsPaymentAndDeliveryPage = new MaintContractsPaymentAndDeliveryPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsAdditionalInfoPage);
            maintContractsProductsPage = new MaintContractsProductsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsPaymentAndDeliveryPage, ref maintContractsAdditionalInfoPage);
            maintContractsProjectInfoPage = new MaintContractsProjectsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsProductsPage);
            maintContractsBasicInfoPage = new MaintContractsBasicInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsProjectInfoPage);
            
            if (mViewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage = new MaintContractsUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mMaintContracts, mViewAddCondition);
            }
            if (openFilesPage)
            {
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;

                this.NavigationService.Navigate(maintContractsUploadFilesPage);

            }
            else
            {
                maintContractsBasicInfoPage.maintContractsProjectInfoPage = maintContractsProjectInfoPage;
                maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
                maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

                this.NavigationService.Navigate(maintContractsBasicInfoPage);
            }
        }
    }
}
