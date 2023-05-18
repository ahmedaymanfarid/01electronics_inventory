using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        FTPServer rfpsSynchronization;
        FTPServer productsSynchronization;

        BackgroundWorker rfpsSync;
        BackgroundWorker productsSync;

        String localPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system";
        String rfpsLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\RFPS";
        String productsLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\products_photos";

        public MainWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            

            rfpsSynchronization = new FTPServer();
            productsSynchronization = new FTPServer();

            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(rfpsLocalPath);
                Directory.CreateDirectory(productsLocalPath);
            }
            
            if (!Directory.Exists(rfpsLocalPath))
            {
                Directory.CreateDirectory(rfpsLocalPath);
            }

            if (!Directory.Exists(productsLocalPath))
            {
                Directory.CreateDirectory(productsLocalPath);
            }

            rfpsSync = new BackgroundWorker();
            rfpsSync.DoWork += BackgroundRFPsSynchronization;
            rfpsSync.ProgressChanged += OnRFPsSyncProgressChanged;
            rfpsSync.RunWorkerCompleted += OnRFPsSyncBackgroundComplete;
            rfpsSync.WorkerReportsProgress = true;
            
            rfpsSync.RunWorkerAsync();


            productsSync = new BackgroundWorker();
            productsSync.DoWork += BackgroundProductsSynchronization;
            productsSync.ProgressChanged += OnProductsSyncProgressChanged;
            productsSync.RunWorkerCompleted += OnProductsSyncBackgroundComplete;
            productsSync.WorkerReportsProgress = true;

            productsSync.RunWorkerAsync();

            DashboardPage rfpsPage = new DashboardPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(rfpsPage);

        }

        protected void OnRFPsSyncProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }

        protected void BackgroundRFPsSynchronization(object sender, DoWorkEventArgs e)
        {
            String errorMessage = String.Empty;
            if (!rfpsSynchronization.DownloadFolder(BASIC_MACROS.RFP_FILES_PATH, rfpsLocalPath, ref errorMessage))
                return;

        }

        protected void OnRFPsSyncBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
        }


        protected void OnProductsSyncProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }

        protected void BackgroundProductsSynchronization(object sender, DoWorkEventArgs e)
        {
            String errorMessage = String.Empty;
            if (!productsSynchronization.DownloadFolder(BASIC_MACROS.PRODUCTS_PHOTOS_PATH, productsLocalPath, ref errorMessage))
                return;
        }

        protected void OnProductsSyncBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
        }
    }
}
