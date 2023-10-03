using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using _01electronics_library;
using _01electronics_windows_library;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for WorkOrderAdditionalInfoPage.xaml
    /// </summary>
    public partial class WorkOrderAdditionalInfoPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        WorkOrder workOrder;
        
        protected FTPServer fTPObject;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT>();

        private int viewAddCondition;
        private int warrantyPeriod = 0;
        private int orderValidityPeriod = 0;
        private int drawingDeadlineFrom = 0;
        private int drawingDeadlineTo = 0;
        private int isDrawing = 0;

        private string additionalDescription;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;

        public WorkOrderAdditionalInfoPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            viewAddCondition = mViewAddCondition;
            workOrder = mWorkOrder;

            fTPObject = new FTPServer();

            InitializeComponent();

            ConfigureDrawingSubmissionUIElements();


            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();
                SetContractTypeValue();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();

                if (workOrder.GetDrawingSubmissionDeadlineTimeUnitId() != 0)
                    drawingSubmissionCheckBox.IsChecked = true;
                drawingSubmissionCheckBox.IsEnabled = false;

                ConfigureUIElementsView();
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();

                nextButton.IsEnabled = true;
                finishButton.IsEnabled = false;
                cancelButton.IsEnabled = false;
                remainingCharactersWrapPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                //ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();

                //SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();
                ConfigureUIElementsView();

            }
        }
        public WorkOrderAdditionalInfoPage(ref WorkOrder mWorkOrder)
        {
            workOrder = mWorkOrder;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = false;
            workOrder.SetOrderHasDrawings(false);
        }

        private void EnableDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = true;
            workOrder.SetOrderHasDrawings(true);
        }

        private void ConfigureUIElementsView()
        {
            //drawingConditionsCheckBox.IsEnabled = false;
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            contractTypeComboBox.IsEnabled = false;
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = false;
            warrantyPeriodFromWhenCombo.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////
        private bool InitializeContractType()
        {
            if (!commonQueries.GetContractTypes(ref contractTypes))
                return false;
            contractTypes.Remove(contractTypes.Find(s1 => s1.contractId == 5));
            for (int i = 0; i < contractTypes.Count; i++)
                contractTypeComboBox.Items.Add(contractTypes[i].contractName);

            return true;
        }

        private bool InitializeTimeUnitComboBoxes()
        {
            if (!commonQueries.GetTimeUnits(ref timeUnits))
                return false;
            for (int i = 0; i < timeUnits.Count(); i++)
            {
                warrantyPeriodCombo.Items.Add(timeUnits[i].timeUnit);
                drawingDeadlineDateComboBox.Items.Add(timeUnits[i].timeUnit);
            }
            return true;
        }
        private bool InitializeDrawingDeadlineDateFromWhenComboBox()
        {
            if (!commonQueries.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                drawingDeadlineDateFromWhenComboBox.Items.Add(conditionStartDates[i].condition_type);
            return true;
        }
        private bool InitializeWarrantyPeriodFromWhenCombo()
        {
            if (!commonQueries.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                warrantyPeriodFromWhenCombo.Items.Add(conditionStartDates[i].condition_type);
            return true;
        }

        //////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////
        public void SetDrawingSubmissionValues()
        {

            drawingDeadlineFromTextBox.Text = workOrder.GetOrderDrawingSubmissionDeadlineMinimum().ToString();
            drawingDeadlineToTextBox.Text = workOrder.GetOrderDrawingSubmissionDeadlineMaximum().ToString();
            drawingDeadlineDateComboBox.Text = workOrder.GetOrderDrawingDeadlineTimeUnit();
            drawingDeadlineDateFromWhenComboBox.SelectedItem = workOrder.GetOrderDrawingSubmissionDeadlineCondition();

        }

        public void SetContractTypeValue()
        {

            contractTypeComboBox.SelectedItem = workOrder.GetOrderContractType();


        }

        public void SetWarrantyPeriodValues()
        {

            warrantyPeriodTextBox.Text = workOrder.GetOrderWarrantyPeriod().ToString();
            warrantyPeriodCombo.SelectedItem = workOrder.GetOrderWarrantyPeriodTimeUnit();
            warrantyPeriodFromWhenCombo.SelectedItem = workOrder.GetOrderWarrantyPeriodCondition();

        }

        public void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = workOrder.GetOrderNotes();
        }

        public void SetNullsToZeros()
        {
            if (drawingDeadlineFromTextBox.Text == null)
                drawingDeadlineFromTextBox.Text = "0";

            if (drawingDeadlineToTextBox.Text == null)
                drawingDeadlineToTextBox.Text = "0";

            if (warrantyPeriodTextBox.Text == null)
                warrantyPeriodTextBox.Text = "0";
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////



        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedIndex != -1)
            {
                workOrder.SetOrderWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
            }
        }

        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractTypeComboBox.SelectedIndex != -1)
            {
                workOrder.SetOrderContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);
            }

        }

        private void DrawingDeadlineDateFromWhenComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateFromWhenComboBox.SelectedIndex != -1)
            {
                workOrder.SetOrderDrawingSubmissionDeadlineCondition(conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].condition_id, conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].condition_type);
            }
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                workOrder.SetOrderWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_id, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_type);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void WarrantyPeriodTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(warrantyPeriodTextBox.Text, BASIC_MACROS.PHONE_STRING) && warrantyPeriodTextBox.Text != "")
            {
                warrantyPeriod = int.Parse(warrantyPeriodTextBox.Text);
                workOrder.SetOrderWarrantyPeriod(warrantyPeriod);
            }
            else
            {
                warrantyPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (additionalDescriptionTextBox.Text.Length <= COMPANY_WORK_MACROS.MAX_NOTES_TEXTBOX_CHAR_VALUE)
                additionalDescription = additionalDescriptionTextBox.Text;
            additionalDescriptionTextBox.Text = additionalDescription;
            additionalDescriptionTextBox.Select(additionalDescriptionTextBox.Text.Length, 0);
            counterLabel.Content = COMPANY_WORK_MACROS.MAX_NOTES_TEXTBOX_CHAR_VALUE - additionalDescriptionTextBox.Text.Length;
            workOrder.SetOrderNotes(additionalDescriptionTextBox.Text);
        }
        private void DrawingDeadlineFromTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
            {
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
                workOrder.SetOrderDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            }
            else
            {
                drawingDeadlineFrom = 0;
                drawingDeadlineFromTextBox.Text = null;
            }
        }
        private void DrawingDeadlineToTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineToTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineToTextBox.Text != "")
            {
                drawingDeadlineTo = int.Parse(drawingDeadlineToTextBox.Text);
                workOrder.SetOrderDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            }
            else
            {
                drawingDeadlineTo = 0;
                drawingDeadlineToTextBox.Text = null;
            }
        }
        private void DrawingDeadlineDateComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateComboBox.SelectedItem != null)
            {
                workOrder.SetOrderHasDrawings(true);
                workOrder.SetOrderDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
            }
            else
            {
                workOrder.SetOrderHasDrawings(false);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DrawingConditionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            isDrawing = 1;
        }

        private void DrawingConditionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            isDrawing = 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = this;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }

        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProjectInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProjectInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = this;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProjectInfoPage);
        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = this;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickAutomateWorkOrder(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            workOrder.GetOrderModelsSerialsList().Clear();

            List<PROJECT_MACROS.PROJECT_SITE_STRUCT> Locations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();
            workOrder.GetProjectLocations(ref Locations);
            //AN MAKE IT POP UP AS AN ERROR NOT MESSAGE
            if (workOrder.GetOrderProduct1Quantity() != 0)
            {
                if (!FillModelSerialsList(1, workOrder.GetOrderProduct1Quantity(), product1Grid))
                    return;
            }
            if (workOrder.GetOrderProduct2Quantity() != 0)
            {
                if (!FillModelSerialsList(2, workOrder.GetOrderProduct2Quantity(), product2Grid))
                    return;
            }
            if (workOrder.GetOrderProduct3Quantity() != 0)
            {
                if (!FillModelSerialsList(3, workOrder.GetOrderProduct3Quantity(), product3Grid))
                    return;
            }
            if (workOrder.GetOrderProduct4Quantity() != 0)
            {
                if (!FillModelSerialsList(4, workOrder.GetOrderProduct4Quantity(), product4Grid))
                    return;
            }
            if (workOrder.GetSalesPersonId() == 0)
                System.Windows.Forms.MessageBox.Show("Sales person must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetCompanyName() == null)
                System.Windows.Forms.MessageBox.Show("Company must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetAddressSerial() == 0)
                System.Windows.Forms.MessageBox.Show("Company address must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetContactId() == 0)
                System.Windows.Forms.MessageBox.Show("Contact must be specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct1TypeId() != 0 && workOrder.GetOrderProduct1Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 1 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct2TypeId() != 0 && workOrder.GetOrderProduct2Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 2 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct3TypeId() != 0 && workOrder.GetOrderProduct3Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 3 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct4TypeId() != 0 && workOrder.GetOrderProduct4Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 4 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderPercentDownPayment() + workOrder.GetOrderPercentOnDelivery() + workOrder.GetOrderPercentOnInstallation() != 100)
                System.Windows.Forms.MessageBox.Show("Error in payment condition values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderContractTypeId() == 0)
                System.Windows.Forms.MessageBox.Show("Contract type must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderIssueDate().ToString().Contains("1/1/0001"))
                System.Windows.Forms.MessageBox.Show("Work order issue date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.QUOTATION_RESOLVE_CONDITION)
                {
                    if (workOrder.GetprojectSerial() != 0)
                    {
                        if (Locations.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("You have to choose 1 location atleast", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    if (!workOrder.IssueNewOrder())
                        return;
                    if (workOrder.GetOfferID() != null)
                        if (!workOrder.ConfirmOffer())
                            return;
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                {
                    if (workOrder.GetprojectSerial() != 0)
                    {
                        if (Locations.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("You have to choose 1 location atleast", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!workOrder.EditWorkOrder(workOrderBasicInfoPage.oldWorkOrder))
                        return;
                }

                viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;

                WorkOrderWindow viewOffer = new WorkOrderWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, ref workOrder, viewAddCondition, true);
                viewOffer.Show();

                NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                currentWindow.Close();


            }
        }

        private void OrderSerialTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void OnSelChangedOrderIssueDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OrderIDTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            EnableDrawingSubmissionUIElements();
        }

        private void OnUnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            ConfigureDrawingSubmissionUIElements();
        }

        public void ShowModelsSerialsGrid(int index, int quantity)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION)
            {
                Grid parentProductGrid = ModelsSerialsWrapPanel.Children[index] as Grid;
                Grid currentProductGrid = parentProductGrid.Children[0] as Grid;
                parentProductGrid.Visibility = Visibility.Visible;
                ScrollViewer bodyScrollViewer = currentProductGrid.Children[1] as ScrollViewer;
                Grid modelSerialGrid = bodyScrollViewer.Content as Grid;

                for (int i = modelSerialGrid.RowDefinitions.Count - 1; i >= 1; i--)
                {
                    modelSerialGrid.Children.RemoveAt(i);
                    modelSerialGrid.RowDefinitions.RemoveAt(i);
                }

                if (quantity != 0)
                {
                    if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION)
                    {
                        ////// INITIALIZE FIRST GRID VALUES ///////

                        Grid firstRowGrid = modelSerialGrid.Children[0] as Grid;
                        System.Windows.Controls.TextBox firstRowSerialTextBox = firstRowGrid.Children[1] as System.Windows.Controls.TextBox;

                        Border borderIcon = firstRowGrid.Children[2] as Border;
                        System.Windows.Controls.Label serialStatusLabel = borderIcon.Child as System.Windows.Controls.Label;

                        firstRowSerialTextBox.IsEnabled = false;
                        PRODUCTS_STRUCTS.ORDER_MODEL_SERIAL_STRUCT currentModelSerial2 = workOrder.GetOrderModelsSerialsList().Find
                            (tmpSerial => tmpSerial.product_id == index + 1 && tmpSerial.model_serial_id == 1);
                        if (currentModelSerial2.model_serial_id != 0)
                        {
                            firstRowSerialTextBox.Text = currentModelSerial2.model_serial;
                            serialStatusLabel.Content = currentModelSerial2.serial_status_name;
                            if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION
                                && currentModelSerial2.serial_status == COMPANY_WORK_MACROS.OPEN_WORK_ORDER)
                                firstRowSerialTextBox.IsEnabled = true;
                            if (currentModelSerial2.serial_status == COMPANY_WORK_MACROS.CANCELLED_WORK_ORDER)
                            {
                                borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                            }
                            else if (currentModelSerial2.serial_status == COMPANY_WORK_MACROS.AT_STOCK_WORK_ORDER_ITEM ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.DELIVERED_WORK_ORDER_ITEM ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.INSTALLED_WORK_ORDER_ITEM ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.COMMISSIONED_WORK_ORDER_ITEM ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.SURVEY_DONE_WORK_ORDER ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.REPAIRED_WORK_ORDER ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.INSPECTION_DONE_WORK_ORDER ||
                                     currentModelSerial2.serial_status == COMPANY_WORK_MACROS.CLOSED_WORK_ORDER)
                            {
                                borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                            }
                            else
                            {
                                borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                            }

                        }
                        else
                        {
                            firstRowSerialTextBox.Text = "NULL";
                            firstRowSerialTextBox.IsEnabled = true;
                        }
                        //firstRowGrid.Visibility = Visibility.Collapsed;

                    }

                    for (int i = 1; i < quantity; i++)
                    {
                        RowDefinition serialRow = new RowDefinition();
                        serialRow.Height = new GridLength(75);
                        modelSerialGrid.RowDefinitions.Add(serialRow);

                        Grid gridI = new Grid();
                        Grid.SetRow(gridI, modelSerialGrid.RowDefinitions.Count - 1);

                        ColumnDefinition labelColumn = new ColumnDefinition();
                        labelColumn.Width = new GridLength(100);

                        ColumnDefinition serialColumn = new ColumnDefinition();

                        ColumnDefinition statusColumn = new ColumnDefinition();
                        statusColumn.Width = new GridLength(220);

                        gridI.ColumnDefinitions.Add(labelColumn);
                        gridI.ColumnDefinitions.Add(serialColumn);
                        gridI.ColumnDefinitions.Add(statusColumn);

                        System.Windows.Controls.Label serialIdLabel = new System.Windows.Controls.Label();
                        serialIdLabel.Margin = new Thickness(30, 0, 0, 0);
                        serialIdLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        serialIdLabel.Style = (Style)FindResource("labelStyle");
                        serialIdLabel.Content = "Serial #" + (i + 1).ToString();
                        Grid.SetColumn(serialIdLabel, 0);

                        System.Windows.Controls.TextBox serialTextBox = new System.Windows.Controls.TextBox();
                        serialTextBox.Style = (Style)FindResource("textBoxStyle");
                        serialTextBox.TextWrapping = TextWrapping.Wrap;
                        serialTextBox.Width = 180;
                        Grid.SetColumn(serialTextBox, 1);

                        Border borderIcon = new Border();
                        borderIcon.Style = (Style)FindResource("BorderIcon");
                        borderIcon.Width = 200;
                        Grid.SetColumn(borderIcon, 2);

                        System.Windows.Controls.Label serialStatusLabel = new System.Windows.Controls.Label();
                        serialStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                        borderIcon.Child = serialStatusLabel;

                        gridI.Children.Add(serialIdLabel);
                        gridI.Children.Add(serialTextBox);
                        gridI.Children.Add(borderIcon);
                        modelSerialGrid.Children.Add(gridI);

                        if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION)
                        {
                            serialTextBox.IsEnabled = false;

                            PRODUCTS_STRUCTS.ORDER_MODEL_SERIAL_STRUCT currentModelSerial = workOrder.GetOrderModelsSerialsList().Find
                                (tmpSerial => tmpSerial.product_id == index + 1 && tmpSerial.model_serial_id == i + 1);

                            //serialTextBox.Text = currentModelSerial.model_serial;
                            if (currentModelSerial.model_serial_id != 0)
                            {
                                serialTextBox.Text = currentModelSerial.model_serial;
                                serialStatusLabel.Content = currentModelSerial.serial_status_name;
                                if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION
                                    && currentModelSerial.serial_status == COMPANY_WORK_MACROS.OPEN_WORK_ORDER)
                                    serialTextBox.IsEnabled = true;
                                if (currentModelSerial.serial_status == COMPANY_WORK_MACROS.CANCELLED_WORK_ORDER)
                                {
                                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                                }
                                else if (currentModelSerial.serial_status == COMPANY_WORK_MACROS.AT_STOCK_WORK_ORDER_ITEM ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.DELIVERED_WORK_ORDER_ITEM ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.INSTALLED_WORK_ORDER_ITEM ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.COMMISSIONED_WORK_ORDER_ITEM ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.SURVEY_DONE_WORK_ORDER ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.REPAIRED_WORK_ORDER ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.INSPECTION_DONE_WORK_ORDER ||
                                         currentModelSerial.serial_status == COMPANY_WORK_MACROS.CLOSED_WORK_ORDER)
                                {
                                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                                }
                                else
                                {
                                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                                }

                            }
                            else
                            {
                                serialTextBox.Text = "NULL";
                                serialTextBox.IsEnabled = true;
                            }

                        }

                    }
                }
                else
                    parentProductGrid.Visibility = Visibility.Collapsed;
            }

        }
        private bool FillModelSerialsList(int index, int quantity, Grid currentGrid)
        {
            for (int i = 0; i < currentGrid.RowDefinitions.Count; i++)
            {
                Grid CurrentSerialGrid = currentGrid.Children[i] as Grid;
                System.Windows.Controls.TextBox currentTextBox = CurrentSerialGrid.Children[1] as System.Windows.Controls.TextBox;
                PRODUCTS_STRUCTS.ORDER_MODEL_SERIAL_STRUCT currentSerialItem = new PRODUCTS_STRUCTS.ORDER_MODEL_SERIAL_STRUCT();

                if (currentTextBox.Text == String.Empty || currentTextBox.Text == "NULL")
                {
                    System.Windows.Forms.MessageBox.Show("Product " + index + " Model Serial Number #" + (i + 1).ToString() + " Must Be Specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    currentSerialItem.product_id = index;
                    currentSerialItem.model_serial_id = i + 1;
                    currentSerialItem.model_serial = currentTextBox.Text.ToString();
                    currentSerialItem.serial_status = COMPANY_WORK_MACROS.OPEN_WORK_ORDER;
                    workOrder.GetOrderModelsSerialsList().Add(currentSerialItem);
                }
            }
            return true;
        }

    }
}
