﻿using _01electronics_library;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for MaintContractsProjectsPage.xaml
    /// </summary>
    public partial class MaintContractsProjectsPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        MaintenanceContract maintContract;

        private int viewAddCondition;

        private List<PROJECT_MACROS.PROJECT_STRUCT> projects = new List<PROJECT_MACROS.PROJECT_STRUCT>();
        private List<PROJECT_MACROS.PROJECT_SITE_STRUCT> projectLocations;
        private List<PROJECT_MACROS.PROJECT_SITE_STRUCT> addedLocations;

        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;

        private List<int> skip = new List<int>();

        Grid trialGrid = new Grid();
        int rowCounter = 1;

        public MaintContractsProjectsPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, ref MaintContractsProductsPage mMaintContractsProductsPage)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            maintContractsProductsPage = mMaintContractsProductsPage;

            viewAddCondition = mViewAddCondition;

            maintContract = mMaintContracts;

            projectLocations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();
            addedLocations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();


            InitializeComponent();

            InitializeProjectsCombo();

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                checkAllCheckBox.IsEnabled = false;
                projectCheckBox.IsChecked = true;
                projectCheckBox.IsEnabled = false;
                projectComboBox.SelectedItem = maintContract.GetprojectName();
                projectComboBox.IsEnabled = false;
                checkAllCheckBox.IsChecked = true;

            }
            else
            {
                projectCheckBox.IsEnabled = true;
                projectCheckBox.IsChecked = true;

                projectComboBox.SelectedItem = maintContract.GetprojectName();

                checkAllCheckBox.IsEnabled = true;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeProjectsCombo()
        {
            commonQueries.GetClientProjects(ref projects);

            for (int i = 0; i < projects.Count; i++)
                projectComboBox.Items.Add(projects[i].project_name);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {

                addedLocations.Clear();
                projectLocations.Clear();
                locationsGrid.Children.Clear();
                locationsGrid.RowDefinitions.Clear();

                maintContract.SetMaintContractProjectInfo(projects[projectComboBox.SelectedIndex].project_serial, projects[projectComboBox.SelectedIndex].project_name);


                commonQueries.GetProjectLocations(projects[projectComboBox.SelectedIndex].project_serial, ref projectLocations);

                List<PROJECT_MACROS.PROJECT_SITE_STRUCT> temp = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();

                maintContract.GetProjectLocations(ref temp);


                for (int i = 0; i < projectLocations.Count; i++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = projectLocations[i].site_location.country.country_name + "," + projectLocations[i].site_location.city.city_name + "," + projectLocations[i].site_location.state_governorate.state_name + "," + projectLocations[i].site_location.district.district_name;
                    checkBox.Tag = i;
                    checkBox.Style = (Style)FindResource("checkBoxStyle");
                    checkBox.Checked += OnCheckProjectLocation;
                    checkBox.Unchecked += OnUnCheckProjectLocation;
                    checkBox.Width = 500.0;

                    if (temp.Exists(x1 => x1.location_id == projectLocations[i].location_id))
                        checkBox.IsChecked = true;

                    RowDefinition row = new RowDefinition();
                    locationsGrid.RowDefinitions.Add(row);

                    locationsGrid.Children.Add(checkBox);
                    Grid.SetRow(checkBox, i);

                }

                checkAllCheckBox.IsEnabled = true;
            }
            else
            {
                projectLocations.Clear();
                addedLocations.Clear();
                maintContract.GetProjectLocations(ref projectLocations);

                for (int i = 0; i < projectLocations.Count; i++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = projectLocations[i].site_location.country.country_name + "," + projectLocations[i].site_location.city.city_name + "," + projectLocations[i].site_location.state_governorate.state_name + "," + projectLocations[i].site_location.district.district_name;
                    checkBox.IsEnabled = false;
                    checkBox.IsChecked = true;
                    checkBox.Style = (Style)FindResource("checkBoxStyle");
                    checkBox.Width = 500.0;

                    RowDefinition row = new RowDefinition();
                    locationsGrid.RowDefinitions.Add(row);

                    locationsGrid.Children.Add(checkBox);
                    Grid.SetRow(checkBox, i);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////CHECK/UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckCheckAllCheckBox(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < locationsGrid.Children.Count; i++)
            {
                CheckBox currentcheckBox = (CheckBox)locationsGrid.Children[i];
                currentcheckBox.IsChecked = true;
            }
        }

        private void OnUnCheckCheckAllCheckBox(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < locationsGrid.Children.Count; i++)
            {
                CheckBox currentcheckBox = (CheckBox)locationsGrid.Children[i];
                currentcheckBox.IsChecked = false;
            }
        }

        private void OnCheckProject(object sender, RoutedEventArgs e)
        {
            projectComboBox.IsEnabled = true;
        }

        private void OnUnCheckProject(object sender, RoutedEventArgs e)
        {
            projectComboBox.SelectedItem = null;
            projectComboBox.IsEnabled = false;
            locationsGrid.Children.Clear();
        }

        private void OnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Add(projectLocations[((int)currentCheckBox.Tag)]);
        }

        private void OnUnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Remove(projectLocations[((int)currentCheckBox.Tag)]);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsBasicInfoPage.maintContractsProjectInfoPage = this;

            NavigationService.Navigate(maintContractsBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsProductsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsProductsPage.maintContractsProjectsPage = this;

            NavigationService.Navigate(maintContractsProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProjectsPage = this;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsAdditionalInfoPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsAdditionalInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsAdditionalInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsAdditionalInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsAdditionalInfoPage.maintContractsProjectsPage = this;

            NavigationService.Navigate(maintContractsAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
                maintContractsUploadFilesPage.maintContractsProjectsPage = this;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsProductsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsProductsPage.maintContractsProjectsPage = this;

            NavigationService.Navigate(maintContractsProductsPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                maintContract.SetProjectLocations(addedLocations);

            maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;
            maintContractsBasicInfoPage.maintContractsProjectInfoPage = this;

            NavigationService.Navigate(maintContractsBasicInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetProjectComboBox()
        {
            projectComboBox.SelectedItem = maintContract.GetprojectName();
            if (projectComboBox.SelectedItem != null)
            {
                projectCheckBox.IsChecked = true;
                projectCheckBox.IsEnabled = true;
            }
        }
    }
}

