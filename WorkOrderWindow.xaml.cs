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
using System.Windows.Shapes;
using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for WorkOrderWindow.xaml
    /// </summary>
    public partial class WorkOrderWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;

        public WorkOrderWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, bool openFilesPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            workOrderAdditionalInfoPage = new WorkOrderAdditionalInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition);
            workOrderPaymentAndDeliveryPage = new WorkOrderPaymentAndDeliveryPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderAdditionalInfoPage);
            workOrderProductsPage = new WorkOrderProductsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderPaymentAndDeliveryPage, ref workOrderAdditionalInfoPage);
            workOrderProjectInfoPage = new WorkOrderProjectInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderProductsPage);
            workOrderBasicInfoPage = new WorkOrderBasicInfoPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderProjectInfoPage, ref workOrderProductsPage);
            if (mViewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage = new WorkOrderUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref mWorkOrder, mViewAddCondition);
            }
            if (openFilesPage)
            {

                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProjectInfoPage.workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                this.NavigationService.Navigate(workOrderUploadFilesPage);

            }
            else
            {
                workOrderBasicInfoPage.workOrderProductsPage = workOrderProjectInfoPage.workOrderProductsPage;
                workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
                workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

                this.NavigationService.Navigate(workOrderBasicInfoPage);
            }
        }
    }
}
