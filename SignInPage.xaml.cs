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
using System.Net.Mail;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;

        public static Employee loggedInUser;

        String employeeEmail;
        String employeePassword;

        protected String errorMessage;

        public SignInPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = new Employee();

            InitializeComponent();

            if (_01electronics_inventory.Properties.Settings.Default.Email != null)
            {
                employeeEmailTextBox.Text = _01electronics_inventory.Properties.Settings.Default.Email;
            }

            if (_01electronics_inventory.Properties.Settings.Default.PassWordCheck != false)
            {
                RememberMeCheckBox.IsChecked = true;

            }
            if (_01electronics_inventory.Properties.Settings.Default.PassWord != null)
            {
                employeePasswordTextBox.Password = _01electronics_inventory.Properties.Settings.Default.PassWord;
            }

        }

        private void OnButtonClickedSignIn(object sender, RoutedEventArgs e)
        {
            employeeEmail = employeeEmailTextBox.Text;

            if (!integrityChecks.CheckEmployeeLoginEmailEditBox(employeeEmail, ref employeeEmail, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            loggedInUser.InitializeEmployeeInfo(employeeEmail);

            if (loggedInUser.GetEmployeeDepartmentId() != COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID &&
                loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
            {
                System.Windows.Forms.MessageBox.Show("Unauthorized Access, Please contact your system adminstrator for authorisation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            employeePassword = employeePasswordTextBox.Password;

            if (!integrityChecks.CheckEmployeePasswordEditBox(employeePassword, loggedInUser.GetEmployeeId(), ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (_01electronics_inventory.Properties.Settings.Default.PassWordCheck)
            {
                _01electronics_inventory.Properties.Settings.Default.Email = employeeEmailTextBox.Text;
                _01electronics_inventory.Properties.Settings.Default.PassWord = employeePasswordTextBox.Password;
                _01electronics_inventory.Properties.Settings.Default.Save();
            }
            else
            {
                _01electronics_inventory.Properties.Settings.Default.Email = employeeEmailTextBox.Text;
                _01electronics_inventory.Properties.Settings.Default.Save();
            }

            employeePassword = employeePasswordTextBox.Password;


            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID || loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.ERP_SYSTEM_DEVELOPMENT_TEAM_ID)
            {
                AdminWindow adminWindow = new AdminWindow(ref commonQueries, ref commonFunctions, ref integrityChecks);
                adminWindow.Show();
                NavigationWindow currentWindoww = (NavigationWindow)this.Parent;
                currentWindoww.Close();
                return;
            }

            employeePassword = employeePasswordTextBox.Password;

            if (!integrityChecks.CheckEmployeePasswordEditBox(employeePassword, loggedInUser.GetEmployeeId(), ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            MainWindow mainWindowOpen = new MainWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);

            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();

            mainWindowOpen.Show();

            Window.GetWindow(this).Close();
        }

        private void OnButtonClickedSignUp(object sender, RoutedEventArgs e)
        {
            SignUpPage signUp = new SignUpPage(ref commonQueries, ref commonFunctions, ref integrityChecks);
            this.NavigationService.Navigate(signUp);
        }


        private void RememberMeISChecked(object sender, RoutedEventArgs e)
        {

            employeePasswordTextBox.Password = _01electronics_inventory.Properties.Settings.Default.PassWord;

            _01electronics_inventory.Properties.Settings.Default.PassWordCheck = true;
            _01electronics_inventory.Properties.Settings.Default.Save();
        }

        private void RememberMeisUnchecked(object sender, RoutedEventArgs e)
        {
            _01electronics_inventory.Properties.Settings.Default.PassWord = "";
            employeePasswordTextBox.Password = "";
            _01electronics_inventory.Properties.Settings.Default.PassWordCheck = false;
            _01electronics_inventory.Properties.Settings.Default.Save();
        }

        private void OnBtnClicklForgetPassword(object sender, MouseButtonEventArgs e)
        {
            employeeEmail = employeeEmailTextBox.Text;
            ForgetPasswordPage forgetPasswordMail = new ForgetPasswordPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref employeeEmail);
            this.NavigationService.Navigate(forgetPasswordMail);
        }
    }
}
