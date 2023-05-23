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
    /// Interaction logic for RFPBasicInfoPage.xaml
    /// </summary>
    public partial class RFPBasicInfoPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> assignees;
        private List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> rfpRequestors;
        private List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> workOrders;
        List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT> tmpList;
        private List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT> maintenanceContracts;
        private List<PROJECT_MACROS.PROJECT_STRUCT> projects;
        private List<int> requestorTeamId;
        private List<int> requestorId;
        private RFP rfp;
        private int viewAddCondition;
        private List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT> workFormList;
        public RFPItemsPage rfpItemsPage;
        public RFPUploadFilesPage rfpUploadFilesPage;
        public List<int> workFormIdList;
        public List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT> rfpMappedItems;

        public RFPBasicInfoPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref RFP mRfp, ref int mViewAddCondition, ref RFPItemsPage mRfpItemsPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            requestorTeamId = new List<int>();
            requestorId = new List<int>();
            rfp = mRfp;
            viewAddCondition = mViewAddCondition;
            rfpItemsPage = mRfpItemsPage;

            assignees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            rfpRequestors = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            workOrders = new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
            maintenanceContracts = new List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT>();
            projects = new List<PROJECT_MACROS.PROJECT_STRUCT>();
            workFormList = new List<COMPANY_WORK_MACROS.WORK_FORM_STRUCT>();
            workFormIdList = new List<int>();
            tmpList = new List<PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT>();
            rfpMappedItems = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();

            if (!commonQueries.GetRFPRequestors(ref rfpRequestors))
                return;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_ADD_CONDITION)
            {
                InitializeIssueDatePickerAdd();
                InitializeDeadlineDatePickerAdd();
            }

            //InitializeRequestorCombo();
            InitializeRequestorTeamComboBox();
            InitializeAssigneeCombo();
            InitializeOrderIdCombo();
            InitializeContractIdCombo();
            InitializeProjectCombo();
            InitializeStockComboBox();
            InitializeRFPItems(rfp.GetRFPSerial(), rfp.GetRFPVersion(), rfp.GetRFPRequestorTeamId());

            SetRequestorTeamComboBox();
            SetRequestorComboBox();
            SetIssueDatePicker();
            SetDeadlineDatePicker();
            SetAssigneeCombo();
            SetOrderCombo();
            SetContractCombo();
            SetProjectCombo();
            SetNotesTextBox();
            SetStockComboBox();

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
            {
                DisableUIElements();

            }

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_REVISE_CONDITION)
            {
                deadlineDatePicker.IsEnabled = false;
                requestorTeamCombo.IsEnabled = false;
                requestorCombo.IsEnabled = false;
                assigneeCombo.IsEnabled = false;
                orderSerialCheckBox.IsEnabled = false;
                orderSerialCombo.IsEnabled = false;
                contractSerialCheckBox.IsEnabled = false;
                contractSerialCombo.IsEnabled = false;
                projectCheckBox.IsEnabled = false;
                projectsCombo.IsEnabled = false;
                stockCheckBox.IsEnabled = false;
                stockCombo.IsEnabled = false;
                InitializeIssueDatePickerAdd();
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
            {
                //if (loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()) || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.SOFTWARE_DEVELOPMENT_DEPARTMENT_ID))
                //{
                issueDatePicker.IsEnabled = false;
                deadlineDatePicker.IsEnabled = false;
                requestorTeamCombo.IsEnabled = false;
                requestorCombo.IsEnabled = false;
                assigneeCombo.IsEnabled = false;
                orderSerialCheckBox.IsEnabled = false;
                orderSerialCombo.IsEnabled = false;
                contractSerialCheckBox.IsEnabled = false;
                contractSerialCombo.IsEnabled = false;
                projectCheckBox.IsEnabled = false;
                projectsCombo.IsEnabled = false;
                //notesTextBox.IsReadOnly = true;
                stockCheckBox.IsEnabled = false;
                stockCombo.IsEnabled = false;

                //  }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FilterRequestorsList()
        {
            for (int i = 0; i < tmpList.Count(); i++)
            {
                //if (tmpList[i].employee_id == )
            }

            requestorCombo.SelectedItem = rfp.GetRFPRequestor().GetEmployeeName();
        }

        private void SetRequestorComboBox()
        {
            //requestorCombo.SelectedIndex = 0;
            //requestorCombo.IsEnabled = false;
            //rfp.InitializeRequestorInfo(loggedInUser.GetEmployeeId());

            ////temp logic

            requestorCombo.Items.Add(rfp.GetRFPRequestor().GetEmployeeName());
            requestorCombo.SelectedIndex = 0;
        }

        private void SetIssueDatePicker()
        {
            if (rfp.GetRFPIssueDate() != DateTime.MinValue)
                issueDatePicker.SelectedDate = DateTime.Parse(rfp.GetRFPIssueDate().ToString());
        }

        private void SetDeadlineDatePicker()
        {
            if (rfp.GetRFPDeadlineDate() != DateTime.MinValue)
                deadlineDatePicker.SelectedDate = DateTime.Parse(rfp.GetRFPDeadlineDate().ToString());
        }
        private void SetAssigneeCombo()
        {
            if (rfp.GetRFPAssignedOfficer().GetEmployeeId() != 0)
            {
                assigneeCombo.SelectedIndex = assignees.FindIndex(x1 => x1.employee_id == rfp.GetRFPAssignedOfficer().GetEmployeeId());
            }
        }

        private void SetOrderCombo()
        {
            if (rfp.GetOrderSerial() != 0)
            {
                orderSerialCheckBox.IsChecked = true;
                orderSerialCombo.SelectedIndex = workOrders.FindIndex(x1 => x1.order_serial == rfp.GetOrderSerial());
            }
        }

        private void SetContractCombo()
        {
            if (rfp.GetContractSerial() != 0)
            {
                contractSerialCheckBox.IsChecked = true;
                contractSerialCombo.SelectedIndex = maintenanceContracts.FindIndex(x1 => x1.contract_serial == rfp.GetContractSerial());
            }
        }

        private void SetProjectCombo()
        {
            if (rfp.GetProjectSerial() != 0)
            {
                projectCheckBox.IsChecked = true;
                projectsCombo.SelectedIndex = projects.FindIndex(x1 => x1.project_serial == rfp.GetProjectSerial());
            }
        }

        private void SetNotesTextBox()
        {
            if (rfp.GetRFPNotes() != "")
            {
                notesTextBox.Text = rfp.GetRFPNotes();
            }
        }
        private void SetRequestorTeamComboBox()
        {
            requestorTeamCombo.SelectedItem = rfp.GetRFPRequestorTeam();
        }
        private void SetStockComboBox()
        {
            if ((rfp.GetWorkFormName() != "" && rfp.GetWorkFormName() != null && (rfp.GetWorkForm() != 1 && rfp.GetWorkForm() != 2 && rfp.GetWorkForm() != 3)))
            {
                stockCheckBox.IsChecked = true;
                stockCombo.SelectedItem = workFormList[rfp.GetWorkForm()].work_form_name;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeIssueDatePickerAdd()
        {
            issueDatePicker.SelectedDate = DateTime.Today;
        }
        private void InitializeDeadlineDatePickerAdd()
        {
            //CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1));
            //deadlineDatePicker.BlackoutDates.Add(cdr);
        }

        private void InitializeRequestorTeamComboBox()
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
            {
                requestorTeamCombo.Items.Add(rfp.GetRFPRequestorTeam());
                requestorTeamCombo.SelectedIndex = 0;
                requestorTeamCombo.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
            {
                requestorTeamCombo.Items.Clear();
                requestorTeamId.Clear();
                if (!commonQueries.GetRFPRequestorsUnOrdered(ref rfpRequestors))
                    return;
                else
                {
                    for (int i = 0; i < rfpRequestors.Count; i++)
                    {
                        if (rfp.GetRFPRequestor().GetEmployeeId() == rfpRequestors[i].employee_id)
                        {
                            if (requestorTeamCombo.Items.Contains(rfpRequestors[i].requestor_team.team_name))
                                continue;
                            else
                            {
                                requestorTeamCombo.Items.Add(rfpRequestors[i].requestor_team.team_name);
                                requestorTeamId.Add(rfpRequestors[i].requestor_team.team_id);
                            }
                        }
                    }
                    if (requestorTeamCombo.Items.Count == 1)
                    {
                        requestorTeamCombo.SelectedIndex = 0;
                        requestorTeamCombo.IsEnabled = false;
                    }
                }
            }
            else
            {
                requestorTeamCombo.Items.Clear();
                requestorTeamId.Clear();
                if (!commonQueries.GetRFPRequestorsUnOrdered(ref rfpRequestors))
                    return;
                else
                {
                    for (int i = 0; i < rfpRequestors.Count; i++)
                    {
                        if (loggedInUser.GetEmployeeId() == rfpRequestors[i].employee_id)
                        {
                            if (requestorTeamCombo.Items.Contains(rfpRequestors[i].requestor_team.team_name))
                                continue;
                            else
                            {
                                requestorTeamCombo.Items.Add(rfpRequestors[i].requestor_team.team_name);
                                requestorTeamId.Add(rfpRequestors[i].requestor_team.team_id);
                            }
                        }
                    }
                    if (requestorTeamCombo.Items.Count == 1)
                    {
                        requestorTeamCombo.SelectedIndex = 0;
                        requestorTeamCombo.IsEnabled = false;
                    }
                }
            }

        }

        private void InitializeRequestorCombo()
        {
            requestorCombo.Items.Clear();

            //if (loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID)
            //if (!loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()))
            //    requestorCombo.Items.Add(loggedInUser.GetEmployeeName());
            //else
            //{
            //temp logic
            if (!commonQueries.GetRFPRequestors(ref rfpRequestors))
                return;
            //  PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT tempRequestor = new PROCUREMENT_STRUCTS.RFPS_REQUESTORS_STRUCT();
            //  tempRequestor.employee_id = loggedInUser.GetEmployeeId();
            //  tempRequestor.employee_name = loggedInUser.GetEmployeeName();
            //  tempRequestor.team_id = loggedInUser.GetEmployeeTeamId();
            //  tempRequestor.employee_team = loggedInUser.GetEmployeeTeam();

            // rfpRequestors.Add(tempRequestor);

            for (int i = 0; i < rfpRequestors.Count; i++)
            {
                requestorCombo.Items.Add(rfpRequestors[i].employee_name);
            }
            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION && (loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString())))
            {

                //requestorCombo.IsEnabled = false;
                requestorCombo.SelectedIndex = rfpRequestors.FindIndex(employee => employee.employee_id == loggedInUser.GetEmployeeId());
            }

            // }
        }

        private bool InitializeAssigneeCombo()
        {
            if (!commonQueries.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID, ref assignees))
                return false;

            for (int i = assignees.Count - 1; i >= 0; i--)
            {
                //if (assignees[i].position.position_id == 8 || assignees[i].team.team_id == COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID)
                if (assignees[i].position.position_id == 8 || assignees[i].team.team_id.ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString()))
                    continue;
                else
                    assignees.RemoveAt(i);
            }

            for (int j = 0; j < assignees.Count; j++)
                assigneeCombo.Items.Add(assignees[j].employee_name);

            return true;

        }

        private bool InitializeOrderIdCombo()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return false;

            for (int i = 0; i < workOrders.Count(); i++)
            {
                orderSerialCombo.Items.Add(workOrders[i].order_id);
            }


            return true;
        }

        private bool InitializeContractIdCombo()
        {
            List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT> tmpList = new List<SALES_STRUCTS.MAINTENANCE_CONTRACT_MIN_STRUCT>();
            if (!commonQueries.GetMaintenanceContracts(ref tmpList))
                return false;


            for (int i = 0; i < tmpList.Count; i++)
            {
                if (!tmpList.Exists
                (tmpMaint => tmpMaint.contract_serial == tmpList[i].contract_serial
                && tmpList[i].contract_version < tmpMaint.contract_version))
                {
                    maintenanceContracts.Add(tmpList[i]);
                }
            }

            for (int i = 0; i < maintenanceContracts.Count(); i++)
            {
                contractSerialCombo.Items.Add(maintenanceContracts[i].contract_id);
            }

            return true;
        }

        private bool InitializeProjectCombo()
        {
            if (!commonQueries.GetCompanyProjects(ref projects))
                return false;

            for (int i = 0; i < projects.Count(); i++)
            {
                projectsCombo.Items.Add(projects[i].project_name);
            }
            return true;
        }

        private bool InitializeRequestorTeamCombo()
        {
            for (int i = 0; i < projects.Count(); i++)
            {
                projectsCombo.Items.Add(projects[i].project_name);
            }
            return true;
        }
        private bool InitializeStockComboBox()
        {
            if (!commonQueries.GetWorkForm(ref workFormList))
                return false;
            stockCombo.Items.Add(workFormList[0].work_form_name);
            workFormIdList.Add(workFormList[0].work_form_id);
            for (int i = workFormList.Count - 2; i < workFormList.Count; i++)
            {
                stockCombo.Items.Add(workFormList[i].work_form_name);
                workFormIdList.Add(workFormList[i].work_form_id);
            }
            return true;
        }
        private bool InitializeRFPItems(int rfpSerial, int rfpVersion, int requestorTeam)
        {
            if (!commonQueries.GetRfpItemsMapping(rfpSerial, rfpVersion, requestorTeam, ref rfpMappedItems))
            { return false; }
            for (int i = 0; i < rfp.rfpItems.Count; i++)
            {
                PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT foundItem = rfpMappedItems.Find(f => f.rfp_item_number == rfp.rfpItems[i].rfp_item_number);
                if (foundItem.rfp_item_number != 0)
                {
                    PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem = new PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT();
                    
                    rfpItem.product_category = foundItem.product_category;
                    rfpItem.product_type = foundItem.product_type;
                    rfpItem.product_brand = foundItem.product_brand;
                    rfpItem.product_model = foundItem.product_model;
                    rfpItem.product_specs = foundItem.product_specs;
                    rfpItem.is_company_product = foundItem.is_company_product;

                    
                    rfpItem.itemSelected = rfp.rfpItems[i].itemSelected;
                    rfpItem.item_description = rfp.rfpItems[i].item_description;
                    rfpItem.measure_unit = rfp.rfpItems[i].measure_unit;
                    rfpItem.measure_unit_id = rfp.rfpItems[i].measure_unit_id;
                    rfpItem.item_notes = rfp.rfpItems[i].item_notes;
                    rfpItem.rfp_item_number = rfp.rfpItems[i].rfp_item_number;
                    rfpItem.item_quantity = rfp.rfpItems[i].item_quantity;
                    rfpItem.item_status = rfp.rfpItems[i].item_status;
                    rfpItem.item_vendors = new List<PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT>();
                    for (int j = 0; j < rfp.rfpItems[i].item_vendors.Count; j++)
                    {
                        PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT brand = new PROCUREMENT_STRUCTS.VENDOR_MIN_STRUCT();
                        brand.vendor_id = rfp.rfpItems[i].item_vendors[j].vendor_id;
                        brand.vendor_name = rfp.rfpItems[i].item_vendors[j].vendor_name;
                        rfpItem.item_vendors.Add(brand);
                    }
                    rfp.rfpItems.RemoveAt(i);
                    rfp.rfpItems.Insert(i, rfpItem);

                }
            }
            return true;
        }
        private void DisableUIElements()
        {
            issueDatePicker.IsEnabled = false;
            deadlineDatePicker.IsEnabled = false;
            requestorTeamCombo.IsEnabled = false;
            requestorCombo.IsEnabled = false;
            orderSerialCombo.IsEnabled = false;
            projectsCombo.IsEnabled = false;
            assigneeCombo.IsEnabled = false;
            contractSerialCombo.IsEnabled = false;
            notesTextBox.IsEnabled = false;
            orderSerialCheckBox.IsEnabled = false;
            projectCheckBox.IsEnabled = false;
            contractSerialCheckBox.IsEnabled = false;
            stockCheckBox.IsEnabled = false;
            stockCombo.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnDateChangedIssueDatePicker(object sender, SelectionChangedEventArgs e)
        {
            if (issueDatePicker.SelectedDate != null && viewAddCondition == COMPANY_WORK_MACROS.RFP_REVISE_CONDITION)
                rfp.SetRFPRevisedIssueDate(DateTime.Parse(issueDatePicker.SelectedDate.ToString()));
            else if (issueDatePicker.SelectedDate != null)
                rfp.SetRFPIssueDate(DateTime.Parse(issueDatePicker.SelectedDate.ToString()));
        }
        private void OnDateChangedDeadlineDatePicker(object sender, SelectionChangedEventArgs e)
        {
            if (deadlineDatePicker.SelectedDate != null)
                rfp.SetRFPDeadlineDate(DateTime.Parse(deadlineDatePicker.SelectedDate.ToString()));
        }
        private void OnSelChangedRequestorCombo(object sender, SelectionChangedEventArgs e)
        {
            //if(requestorCombo.SelectedIndex != -1)
            //    rfp.InitializeRequestorInfo(loggedInUser.GetEmployeeId()); 

            ////added temp logic
            if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
            {
                if (requestorCombo.SelectedIndex != -1 && requestorId.Count != 0)
                {
                    rfp.InitializeRequestorInfo(rfpRequestors[requestorId[requestorCombo.SelectedIndex]].employee_id);
                    rfp.SetRFPRequestorTeamId(rfpRequestors[requestorId[requestorCombo.SelectedIndex]].requestor_team.team_id);
                    rfp.SetRFPRequestorTeam(rfpRequestors[requestorId[requestorCombo.SelectedIndex]].requestor_team.team_name);
                }
            }
        }

        private void OnSelChangedAssigneeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (assigneeCombo.SelectedIndex != -1)
                rfp.InitializeAssignedOfficerInfo(assignees[assigneeCombo.SelectedIndex].employee_id);
        }

        private void OnSelChangedOrderSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (orderSerialCombo.SelectedIndex != -1)
            {
                rfp.SetOrderSerial(workOrders[orderSerialCombo.SelectedIndex].order_serial);
                rfp.SetWorkForm(1);
            }
            else
            {
                rfp.SetOrderSerial(0);
                rfp.SetWorkForm(0);
            }
        }
        private void OnSelChangedContractSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (contractSerialCombo.SelectedIndex != -1)
            {
                rfp.SetContractSerial(maintenanceContracts[contractSerialCombo.SelectedIndex].contract_serial);
                rfp.SetContractVersion(maintenanceContracts[contractSerialCombo.SelectedIndex].contract_version);
                rfp.SetWorkForm(2);
            }
            else
            {
                rfp.SetContractSerial(0);
                rfp.SetContractVersion(0);
                rfp.SetWorkForm(0);
            }
        }

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            if (projectsCombo.SelectedIndex != -1)
            {
                rfp.SetProjectSerial(projects[projectsCombo.SelectedIndex].project_serial);
                rfp.SetProjectName(projectsCombo.SelectedItem.ToString());
                rfp.SetWorkForm(3);
            }
            else
            {
                rfp.SetProjectSerial(0);
                rfp.SetWorkForm(0);
            }
        }

        private void OnTextChangedNotesTextBox(object sender, TextChangedEventArgs e)
        {
            rfp.SetRFPNotes(notesTextBox.Text);
        }
        private void OnSelChangedStockCombo(object sender, SelectionChangedEventArgs e)
        {
            if (stockCombo.SelectedIndex != -1)
            {
                rfp.SetWorkForm(workFormIdList[stockCombo.SelectedIndex]);
                rfp.SetWorkFormName(stockCombo.SelectedItem.ToString());
            }
        }
        private void OnSelChangedRequestorTeamCombo(object sender, SelectionChangedEventArgs e)
        {
            requestorCombo.IsEnabled = false;
            requestorCombo.Items.Clear();
            requestorId.Clear();
            if (requestorTeamCombo.SelectedIndex != -1)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                {
                    rfp.SetRFPRequestorTeamId(requestorTeamId[requestorTeamCombo.SelectedIndex]);
                    rfp.SetRFPRequestorTeam(requestorTeamCombo.SelectedItem.ToString());
                    for (int i = 0; i < rfpRequestors.Count; i++)
                    {
                        if (requestorTeamId[requestorTeamCombo.SelectedIndex] == rfpRequestors[i].requestor_team.team_id)
                        {
                            if (loggedInUser.GetEmployeeName() == rfpRequestors[i].employee_name)
                            {
                                requestorCombo.Items.Add(rfpRequestors[i].employee_name);
                                requestorId.Add(i);
                            }
                        }
                    }
                    requestorCombo.SelectedIndex = 0;
                    if (viewAddCondition != COMPANY_WORK_MACROS.RFP_VIEW_CONDITION && (loggedInUser.GetEmployeeTeamId().ToString().Contains(COMPANY_ORGANISATION_MACROS.PROCUREMENT_TEAM_ID.ToString())))
                    {

                        //requestorCombo.IsEnabled = false;
                        requestorCombo.SelectedIndex = rfpRequestors.FindIndex(employee => employee.employee_id == loggedInUser.GetEmployeeId());
                    }
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CHECK/UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckOrderSerial(object sender, RoutedEventArgs e)
        {
            orderSerialCombo.IsEnabled = true;
            contractSerialCheckBox.IsChecked = false;
            projectCheckBox.IsChecked = false;
            stockCheckBox.IsChecked = false;
        }

        private void OnUnCheckOrderSerial(object sender, RoutedEventArgs e)
        {
            orderSerialCombo.SelectedIndex = -1;
            orderSerialCombo.IsEnabled = false;
        }

        private void OnCheckContractId(object sender, RoutedEventArgs e)
        {
            contractSerialCombo.IsEnabled = true;
            orderSerialCheckBox.IsChecked = false;
            projectCheckBox.IsChecked = false;
            stockCheckBox.IsChecked = false;
        }

        private void OnUnCheckContractSerial(object sender, RoutedEventArgs e)
        {
            contractSerialCombo.SelectedIndex = -1;
            contractSerialCombo.IsEnabled = false;
        }


        private void OnCheckProjectCheckBox(object sender, RoutedEventArgs e)
        {
            projectsCombo.IsEnabled = true;
            orderSerialCheckBox.IsChecked = false;
            contractSerialCheckBox.IsChecked = false;
            stockCheckBox.IsChecked = false;
        }

        private void OnUnCheckProjectCheckBox(object sender, RoutedEventArgs e)
        {
            projectsCombo.SelectedIndex = -1;
            projectsCombo.IsEnabled = false;
        }
        private void OnCheckStockCheckBox(object sender, RoutedEventArgs e)
        {
            stockCombo.IsEnabled = true;
            orderSerialCheckBox.IsChecked = false;
            contractSerialCheckBox.IsChecked = false;
            projectCheckBox.IsChecked = false;
        }

        private void OnUnCheckStockCheckBox(object sender, RoutedEventArgs e)
        {
            stockCombo.SelectedIndex = -1;
            stockCombo.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// NAVIGATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {


            rfpItemsPage.rfpBasicInfoPage = this;
            rfpItemsPage.rfpUploadFilesPage = rfpUploadFilesPage;
            NavigationService.Navigate(rfpItemsPage);


        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickOItemsInfo(object sender, MouseButtonEventArgs e)
        {


            rfpItemsPage.rfpBasicInfoPage = this;
            rfpItemsPage.rfpUploadFilesPage = rfpUploadFilesPage;
            NavigationService.Navigate(rfpItemsPage);

        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.RFP_VIEW_CONDITION)
            {
                rfpUploadFilesPage.rfpBasicInfoPage = this;
                rfpUploadFilesPage.rfpItemsPage = rfpItemsPage;
                NavigationService.Navigate(rfpUploadFilesPage);
            }

        }
        private bool CheckForCheckedCheckBoxes()
        {
            if (projectCheckBox.IsChecked == false && orderSerialCheckBox.IsChecked == false && contractSerialCheckBox.IsChecked == false && stockCheckBox.IsChecked == false)
                return false;
            else
                return true;
        }


    }
}
