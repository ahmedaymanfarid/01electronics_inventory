using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _01electronics_library;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddContactWindow.xaml
    /// </summary>
    public partial class AddContactWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public SupplierContact contact;
        public Supplier supplier;

        String firstName;
        String lastName;

        List<PROCUREMENT_STRUCTS.SUPPLIER_MIN_STRUCT> suppliers;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> supplierAddresses;
        List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT> departments;
       
        private List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT> countryCodes;

        protected String errorMessage;


        public AddContactWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            contact = new SupplierContact();

            suppliers = new List<PROCUREMENT_STRUCTS.SUPPLIER_MIN_STRUCT>();
            supplierAddresses = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            departments = new List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT>();
            countryCodes = new List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT>();

            if (!commonQueries.GetAllSuppliers(ref suppliers))
                return;

            for (int i = 0; i < suppliers.Count; i++)
            {
                companyNameComboBox.Items.Add(suppliers[i].supplier_name);
            }

            if (!commonQueries.GetContactTitles(ref departments))
                return;
            for (int i = 0; i < departments.Count; i++)
            {
                employeeDepartmentComboBox.Items.Add(departments[i].department_name);
            }

            contactGenderComboBox.Items.Add("Male");
            contactGenderComboBox.Items.Add("Female");

            companyBranchComboBox.IsEnabled = false;

            InitializeCountryCodeCombos();
        }

        private bool InitializeCountryCodeCombos()
        {
            if (!commonQueries.GetCountryCodes(ref countryCodes))
                return false;

            businessPhoneCountryCodeCombo.Items.Clear();
            personalPhoneCountryCodeCombo.Items.Clear();

            for(int i = 0; i < countryCodes.Count; i++)
            {
                String temp = countryCodes[i].iso3 + "   " + countryCodes[i].phone_code;
                businessPhoneCountryCodeCombo.Items.Add(temp);
                personalPhoneCountryCodeCombo.Items.Add(temp);
            }

            return true;
        }

        private void OnTextChangedFirstName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {
            if (companyNameComboBox.SelectedItem != null)
            {

                if (!commonQueries.GetSupplierAddresses(suppliers[companyNameComboBox.SelectedIndex].supplier_serial, ref supplierAddresses))
                    return;

                companyBranchComboBox.Items.Clear();
                companyBranchComboBox.IsEnabled = true;
                for (int i = 0; i < supplierAddresses.Count; i++)
                {
                    companyBranchComboBox.Items.Add(supplierAddresses[i].district + ", " + supplierAddresses[i].city+ ", " + supplierAddresses[i].state_governorate + ", " + supplierAddresses[i].country);
                }
                companyBranchComboBox.SelectedIndex = 0;
            }
            else
            {
                companyBranchComboBox.IsEnabled = false;
                companyBranchComboBox.SelectedItem = null;
                

            }
        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            if (supplierAddresses.Count != 0 && companyBranchComboBox.SelectedIndex != -1)
            {
                contact.SetAddressSerial(supplierAddresses[companyBranchComboBox.SelectedIndex].address_serial);

                businessPhoneCountryCodeCombo.SelectedIndex = countryCodes.FindIndex(x1 => x1.country_id == supplierAddresses[companyBranchComboBox.SelectedIndex].address / 1000000);
                personalPhoneCountryCodeCombo.SelectedIndex = businessPhoneCountryCodeCombo.SelectedIndex;
            }
        }

        private void OnSelChangedDepartment(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnSelChangedTeam(object sender, SelectionChangedEventArgs e)
        {
        }
        private void OnTextChangedBusinessPhone(object sender, TextChangedEventArgs e)
        {
        }
        private void OnTextChangedPersonalPhone(object sender, TextChangedEventArgs e)
        {
        }

        private void OnTextChangedBusinessEmail(object sender, TextChangedEventArgs e)
        {
        }

        private void OnTextChangedPersonalEmail(object sender, TextChangedEventArgs e)
        {
        }

        private bool CheckContactFirstNameEditBox()
        {
            String inputString = employeeFirstNameTextBox.Text;
            String outputString = employeeFirstNameTextBox.Text;


            if (!integrityChecks.CheckContactNameEditBox(inputString, ref outputString, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            firstName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeFirstNameTextBox.Text = firstName;

            return true;
        }

        private bool CheckContactLastNameEditBox()
        {
            String inputString = employeeLastNameTextBox.Text;
            String outputString = employeeLastNameTextBox.Text;

            if (!integrityChecks.CheckContactNameEditBox(inputString, ref outputString, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            lastName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeLastNameTextBox.Text = lastName;

            return true;
        }
        
        private bool CheckContactBusinessPhoneEditBox()
        {
            String inputString = employeeBusinessPhoneTextBox.Text;
            String outputString = employeeBusinessPhoneTextBox.Text;

            if (!integrityChecks.CheckContactPhoneEditBox(inputString, ref outputString, supplierAddresses[companyBranchComboBox.SelectedIndex].address / 1000000, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            contact.AddNewContactPhone(outputString);
            employeeBusinessPhoneTextBox.Text = outputString;

            return true;
        } 
        
        private bool CheckContactPersonalPhoneEditBox()
        {
            String inputString = employeePersonalPhoneTextBox.Text;
            String outputString = employeePersonalPhoneTextBox.Text;

            if (!integrityChecks.CheckContactPhoneEditBox(inputString, ref outputString, supplierAddresses[companyBranchComboBox.SelectedIndex].address / 1000000, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(outputString!= string.Empty)
            {
               contact.AddNewContactPhone(outputString);
               employeePersonalPhoneTextBox.Text = outputString;
            }

            return true;
        }
        
        private bool CheckContactBusinessEmailEditBox()
        {
            String inputString = employeeBusinessEmailTextBox.Text;
            String outputString = employeeBusinessEmailTextBox.Text;

            if (!integrityChecks.CheckContactBusinessEmailEditBox(inputString, supplierAddresses[companyBranchComboBox.SelectedIndex].address / 1000000, ref outputString, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //YOU SHALL USE THIS FUNCTION TO HANDLE IDS AND EMAILS AUTOMATICALLY
            contact.SetContactBusinessEmail(outputString);
            employeeBusinessEmailTextBox.Text = contact.GetContactBusinessEmail();

            return true;
        }
        

        private bool CheckContactGenderComboBox()
        {
            if (contactGenderComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Supplier Contact gender must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            contact.SetContactGender(contactGenderComboBox.SelectedItem.ToString());
            return true;
        }

        private bool CheckCompanyComboBox()
        {
            if (companyNameComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Supplier must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            contact.SetSupplierName(companyNameComboBox.SelectedItem.ToString());

            return true; 
        } 
        
        private bool CheckCompanyBranchComboBox()
        {
            if (companyBranchComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Supplier branch must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true; 
        }
        
        private bool CheckDepartmentComboBox()
        {
            if (employeeDepartmentComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Supplier Contact Department must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            contact.SetContactDepartment(departments[employeeDepartmentComboBox.SelectedIndex].department_id, employeeDepartmentComboBox.SelectedItem.ToString());

            return true; 
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckContactFirstNameEditBox())
                return;
            if (!CheckContactLastNameEditBox())
                return;
            if (!CheckContactGenderComboBox())
                return;
            if (!CheckCompanyComboBox())
                return; 
            if (!CheckCompanyBranchComboBox())
                return; 
            if (!CheckDepartmentComboBox())
                return; 
            if (!CheckContactBusinessPhoneEditBox())
                return; 
            if (!CheckContactPersonalPhoneEditBox())
                return; 
            if (!CheckContactBusinessEmailEditBox())
                return;  

            //YOU DON'T NEED TO WRITE A FUNCTION TO GET NEW CONTACT ID, THE CONTACT CLASS HANDLES IT ALREADY

            contact.SetAddressSerial(supplierAddresses[companyBranchComboBox.SelectedIndex].address_serial);

            contact.SetContactSalesPerson(loggedInUser);

            contact.IssueNewContact();

            //YOU DON'T NEED TO GET A NEW EMAIL/PHONE ID, THIS IS A NEW CONTACT, SO THE EMAIL ID SHALL BE 1,
            //ALSO, YOU SHALL ONLY USE CONTACT.ADDNEWPERSONALEMAIL() / CONTACT.ADDNEWCONTACTPHONE(), 
            //THIS FUNCTION SHALL HANDLE THE IDs BY ITSELF

            for (int i = 0; i < contact.GetNumberOfSavedContactPhones(); i++)
                contact.InsertIntoContactMobile(i + 1, contact.GetContactPhones()[i]);

            this.Close();
        }

        private void OnSelChangedGender(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnTextChangedLastName(object sender, TextChangedEventArgs e)
        {

        }

    }
}
