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
using System.Security.Cryptography;
using _01electronics_library;
using System.Windows.Forms;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        String employeePassword;
        String confirmPassword;
        String employeeHashedPassword;

        private String employeeEmail;

        public ChangePasswordPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref string mEmployeeEmail)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;

            InitializeComponent();

            employeeEmail = mEmployeeEmail;
        }

        private void OnButtonClickedChange(object sender, RoutedEventArgs e)
        {
            if (!CheckEmployeePasswordEdits())
                return;
            if (!loggedInUser.InitializeEmployeeInfo(employeeEmail))
                return;


            if (!loggedInUser.UpdateEmployeePassword(employeePassword))
                return;

            SignInPage SignInPage = new SignInPage(ref commonQueries, ref commonFunctions, ref integrityChecks);
            this.NavigationService.Navigate(SignInPage);

        }
        bool CheckEmployeePasswordEdits()
        {
            employeePassword = newPasswordTextBox.Password;
            confirmPassword = cNewPasswordTextBox.Password;

            if (employeePassword != confirmPassword)
                return false;

            return true;
        }

    }
}
