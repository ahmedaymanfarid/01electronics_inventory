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
using System.Windows.Shapes;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddReleasePermitWindow.xaml
    /// </summary>
    public partial class AddReleasePermitWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public int viewAddCondition;

        public delegate void func(int serial);

        public func func1;

        public AddReleasePermitPage releasePermitPage;
        public MaterialReleasePermits materialReleasePermit;
        public AddReleasePermitItemPage releasePermitItemPage;
        public AddReleasePermitSummary releasePermitSummary;
        public bool serviceReport;
        public bool rfp;

        public WorkOrder workOrder;
        public RFP rfps;

        public AddReleasePermitWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, MaterialReleasePermits mMaterialReleasePermit=null, int mViewAddCondition = 0,func function=null)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries; 
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            serviceReport = false;
            rfp = false;
            InitializeComponent();
            viewAddCondition=mViewAddCondition;
            workOrder = new WorkOrder();
           //
            rfps = new RFP();
            func1=function;
            if (viewAddCondition == COMPANY_WORK_MACROS.VIEW_RELEASE || viewAddCondition==COMPANY_WORK_MACROS.EDIT_RELEASE)
                materialReleasePermit = mMaterialReleasePermit;
            else
                materialReleasePermit = new MaterialReleasePermits();

            releasePermitPage = new AddReleasePermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this);
            releasePermitItemPage = new AddReleasePermitItemPage(ref  commonQueries, ref commonFunctions,ref integrityChecks,ref loggedInUser, this);
            releasePermitSummary = new AddReleasePermitSummary(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this);


            if (viewAddCondition == COMPANY_WORK_MACROS.VIEW_RELEASE)
                frame.Content =releasePermitSummary;
            else
                 frame.Content = releasePermitPage;

        }
    }
}
