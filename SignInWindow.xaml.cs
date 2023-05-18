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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SignInWindow : NavigationWindow
    {
        private static SQLServer sqlServer;
        private static CommonQueries commonQueries;
        private static SecuredCommonQueries securedCommonQueries;
        private static CommonFunctions commonFunctions;
        private static IntegrityChecks integrityChecks;

        public SignInWindow()
        {
            sqlServer = new SQLServer();
            commonFunctions = new CommonFunctions();
            commonQueries = new CommonQueries(ref sqlServer, ref commonFunctions);
            securedCommonQueries = new SecuredCommonQueries(ref sqlServer, ref commonFunctions);
            integrityChecks = new IntegrityChecks(ref commonQueries);

            InitializeComponent();

            SignInPage signIn = new SignInPage(ref commonQueries, ref commonFunctions, ref integrityChecks);

            this.NavigationService.Navigate(signIn);

        }
    }
}
