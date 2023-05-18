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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;

        private Employee signupEmployee;

        String employeePassword;
        String confirmPassword;
        String employeeHashedPassword;

        protected String errorMessage;

        public SignUpPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            signupEmployee = new Employee();

            InitializeComponent();
        }

        bool CheckemployeeBusinessEmailEdit()
        {
            String inputString = businessEmailTextBox.Text;
            String modifiedString = null;

            if (!integrityChecks.CheckEmployeeSignUpEmailEditBox(inputString, ref modifiedString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!signupEmployee.InitializeEmployeeInfo(modifiedString))
                return false;

            businessEmailTextBox.Text = signupEmployee.GetEmployeeBusinessEmail();

            return true;
        }

        bool CheckEmployeePersonalEmailEdit()
        {
            String inputString = personalEmailTextBox.Text;
            String modifiedString = null;

            if (!integrityChecks.CheckEmployeePersonalEmailEditBox(inputString, ref modifiedString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            personalEmailTextBox.Text = modifiedString;

            return true;
        }

        bool CheckEmployeePasswordEdits()
        {
            employeePassword = passwordTextBox.Password;
            confirmPassword = confirmPasswordTextBox.Password;

            if (employeePassword != confirmPassword)
                return false;

            return true;
        }
        private void OnButtonClickedSignUp(object sender, RoutedEventArgs e)
        {
            if (!CheckemployeeBusinessEmailEdit())
                return;
            if (!CheckEmployeePasswordEdits())
                return;
            if (!CheckEmployeePersonalEmailEdit())
                return;

            if (!signupEmployee.CreateNewPassword(employeePassword))
                return;
            if (!signupEmployee.CreateNewPersonalEmail(personalEmailTextBox.Text))
                return;

            SignInPage signIn = new SignInPage(ref commonQueries, ref commonFunctions, ref integrityChecks);
            this.NavigationService.Navigate(signIn);
        }

    }
}

