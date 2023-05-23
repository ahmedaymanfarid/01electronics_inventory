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
    /// Interaction logic for adminWindow.xaml
    /// </summary>

    public partial class AdminWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private static Employee loggedInUser;

        List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> rfpRequestors;
        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> rfpAssignees;
        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> authorizedEmployees;

        public AdminWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = new Employee();

            InitializeComponent();

            rfpRequestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            rfpAssignees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            authorizedEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            commonQueries.GetRFPRequestors(ref rfpRequestors);
            commonQueries.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID, ref rfpAssignees);


            foreach (PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT employee in rfpRequestors)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tmp = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tmp.employee_id = employee.employee_id;
                tmp.team.team_id = employee.requestor_team.team_id;
                tmp.team.team_name = employee.requestor_team.team_name;
                tmp.employee_name = employee.employee_name;
                authorizedEmployees.Add(tmp);

            }
            foreach (COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT employee in rfpAssignees)
            {
                authorizedEmployees.Add(employee);
            }

            foreach (COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT employee in authorizedEmployees)
            {

                salesCombo.Items.Add(employee.employee_name);

            }
        }


        private void salesComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loggedInUser.InitializeEmployeeInfo(authorizedEmployees[salesCombo.SelectedIndex].employee_id);
        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            mainWindow.Show();
            this.Close();
        }
    }
}
