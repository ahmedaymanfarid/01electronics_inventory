using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ModelsWindow.xaml
    /// </summary>
    public partial class ModelsWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        public ModelsWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref Model mPrduct, int mViewAddCondition, bool openFilesPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            modelBasicInfoPage = new ModelBasicInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mPrduct, mViewAddCondition);
            modelUpsSpecsPage = new ModelUpsSpecsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mPrduct, mViewAddCondition);
            modelAdditionalInfoPage = new ModelAdditionalInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mPrduct, mViewAddCondition);
            modelUploadFilesPage = new ModelUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mPrduct, mViewAddCondition);

            if (openFilesPage)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                this.NavigationService.Navigate(modelUploadFilesPage);
            }
            else
            {
                modelBasicInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

                this.NavigationService.Navigate(modelBasicInfoPage);
            }
        }
    }
}
