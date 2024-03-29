﻿using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using static _01electronics_library.PROCUREMENT_STRUCTS;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Controls.ComboBox;
using Page = System.Windows.Controls.Page;
using TextBox = System.Windows.Controls.TextBox;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddReleasePermitItemPage.xaml
    /// </summary>
    public partial class AddReleasePermitItemPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        MaintenanceContract maintenanceContract;

        int itemsCounter=1;
        int numberOfSelectedItems = 0;

        AddReleasePermitWindow parentWindow;

        List<string> serials;

        private MaterialEntryPermit materialEntry;
        private AddReleasePermitPage addReleasePermitPage;
        private ReleasePermitUploadFilesPage ReleasePermitUploadFilesPage;
        private List<INVENTORY_STRUCTS.ENTRY_PERMIT_MIN_STRUCT> entryPermits;


        private List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> genericCategories;
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> genericProducts;
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> genericBrands;
        private List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> genericModels;

        private List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> companyCategories;
        private List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> companyProducts;
        private List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> companyBrands;
        private List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> companyModels;

        private List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT> workOrders;

        private List<PROJECT_MACROS.PROJECT_SITE_STRUCT> workOrdersLocations;

        private List<PROJECT_MACROS.PROJECT_SITE_STRUCT> maintenanceContractLocations;

        private List<PROJECT_MACROS.PROJECT_SITE_STRUCT> companyProjectLocations;

        private List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT> specs;

       public List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT> checkedOrderItems;
       public  List<PROCUREMENT_STRUCTS.RFP_MAX_STRUCT> checkedRFPItems;
        List<INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT> entryPermitList;
        public List<CheckBox> selectedItems;
        CheckBox rfpItemCheckBoxHolder;
        public CheckBox orderItemHolder;

        private WorkOrder workOrder;
        private RFP rfp;
        int orderItemNumber;
        public int checkedItemsCounter;
        public bool isRFP;
        int viewAddCondition;
        public AddReleasePermitItemPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, AddReleasePermitWindow mReleasePermitWindow)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            materialEntry = new MaterialEntryPermit();

            genericCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            genericProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();

            genericBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            genericModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            companyCategories = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            companyProducts = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            companyBrands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            companyModels = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();

            workOrders = new List<SALES_STRUCTS.WORK_ORDER_MIN_STRUCT>();

            workOrdersLocations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();

            maintenanceContractLocations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();
            companyProjectLocations = new List<PROJECT_MACROS.PROJECT_SITE_STRUCT>();
            
            specs = new List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT>();
            genericCategories= new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            serials = new List<string>();
            entryPermits= new List<INVENTORY_STRUCTS.ENTRY_PERMIT_MIN_STRUCT>();
            maintenanceContract = new MaintenanceContract();
            parentWindow = mReleasePermitWindow;
            workOrder = parentWindow.workOrder;
            rfp = parentWindow.rfps;
            orderItemNumber = 0;
            addReleasePermitPage = parentWindow.releasePermitPage;
            checkedOrderItems= new List<SALES_STRUCTS.WORK_ORDER_MAX_STRUCT>();
            checkedRFPItems = new List<RFP_MAX_STRUCT>();
            entryPermitList = new List<INVENTORY_STRUCTS.ENTRY_PERMIT_MAX_STRUCT>();
            selectedItems = new List<CheckBox>();
            checkedItemsCounter = 0;
            isRFP = false;
            rfpItemCheckBoxHolder = new CheckBox();
            orderItemHolder = new CheckBox();
            viewAddCondition = parentWindow.viewAddCondition;
            GetAllEntryPermits();
            InitializeStock();
            GetGenericItems();
            GetAllWorkOrders();
            //CheckAddOrView();
            //LocationsWrapPanel.Margin = new Thickness(0,15,0,0);
           
        }
        public void GetAllEntryPermits()
        {
            if (!commonQueries.GetEntryPermits(ref entryPermitList))
                return;
        }
        public void GetAllWorkOrders()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return;
        }
        public void CheckAddOrView()
        {
            if (parentWindow.viewAddCondition == COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                finishButton.IsEnabled = false;
                addRecieval.Visibility = Visibility.Visible;
                addReEntry.Visibility = Visibility.Visible;

            }
        }
        public void GetGenericItems()
        {
            genericCategories.Clear();
            if (!commonQueries.GetGenericProductCategories(ref genericCategories))
                return;
        }
        public void InitializeStock() 
        {
            if(viewAddCondition==COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                if (addReleasePermitPage.workFormComboBox.SelectedIndex != -1 && addReleasePermitPage.workFormComboBox.SelectedIndex == 0)
                {
                    workFormLabel.Content = "RFP Items";
                    CheckBox rfpItem = new CheckBox();
                    rfpItem.Style = (Style)FindResource("checkBoxStyle");
                    rfpItem.Margin = new Thickness(10);
                    rfpItem.Width = 500;
                    rfpItem.Checked += OnCheckRFPItem;
                    rfpItem.Unchecked += OnUncheckRFPItem;
                    TextBlock rfpItemContent = new TextBlock();
                    rfpItemContent.Tag = Home.Children.Count;
                    rfpItemContent.TextWrapping = TextWrapping.Wrap;
                    rfpItem.IsEnabled = false;
                    if (parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemcompanyCategory.category_id!=0)
                    {
                        rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemcompanyCategory.category_name} - " +
                          $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialitemCompanyproduct.product_name} - " +
                          $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemCompanyBrand.brand_name} -" +
                          $" {parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemCompanyModel.model_name} - " +
                          $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemCompanySpecs.spec_name} - Reserved Quantity: {parentWindow.materialReleasePermit.GetReleaseItems()[0].entered_quantity} - " +
                          $"To Be Released Quantity:{parentWindow.materialReleasePermit.GetReleaseItems()[0].released_quantity_release}  ";
                    }

                    else
                    {
                        rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemGenericCategory.category_name} -" +
                                        $" {parentWindow.materialReleasePermit.GetReleaseItems()[0].materialitemGenericproduct.product_name} - " +
                                        $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemGenericBrand.brand_name} - " +
                                        $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].materialItemGenericModel.model_name} - Needed Quantity : {parentWindow.materialReleasePermit.GetReleaseItems()[0].entered_quantity} - "
                                        + $"To Be Released Quantity: {parentWindow.materialReleasePermit.GetReleaseItems()[0].released_quantity_release} ";
                    }

                    rfpItem.Content = rfpItemContent;
                    rfpItem.Tag = parentWindow.materialReleasePermit.GetReleaseItems()[0];
                    Home.Children.Add(rfpItem);
                    rfpItem.IsChecked = true;

                }
                else
                {
                    workFormLabel.Content = "WorkOrder Items";
                    CheckBox workOrderItem = new CheckBox();
                    workOrderItem.Style = (Style)FindResource("checkBoxStyle");
                    workOrderItem.Margin = new Thickness(10);
                    workOrderItem.Width = 500;
                    workOrderItem.Checked += OnCheckWorkOrderItem;
                    workOrderItem.Unchecked += OnUncheckWorkOrderItem;
                    workOrderItem.IsEnabled = false;
                    TextBlock workOrderItemContent = new TextBlock();
                    workOrderItemContent.TextWrapping = TextWrapping.Wrap;
                    workOrderItemContent.Tag = Home.Children.Count;
                    workOrderItemContent.Text = $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].orderCategory.category_name} - " +
                               $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].orderproduct.product_name} - " +
                               $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].orderBrand.brand_name} -" +
                               $" {parentWindow.materialReleasePermit.GetReleaseItems()[0].orderModel.model_name} - " +
                               $"{parentWindow.materialReleasePermit.GetReleaseItems()[0].orderSpecs.spec_name}  -" +
                               $"\nNeeded Quantity : {parentWindow.materialReleasePermit.GetReleaseItems()[0].orderItemQuantity}" +
                               $"\nTo Be Released Quantity: {parentWindow.materialReleasePermit.GetReleaseItems()[0].released_quantity_release}";

                    workOrderItem.Content = workOrderItemContent;
                    workOrderItem.Tag = parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[0];
                    Home.Children.Add(workOrderItem);
                    workOrderItem.IsChecked = true;
                }
            }
            else
            {
                //Label workFormLabel = Home.Children[0] as Label;
                if (checkedRFPItems.Count > 0)
                {

                    if (addReleasePermitPage.workFormComboBox.SelectedIndex != -1 && addReleasePermitPage.workFormComboBox.SelectedIndex == 0)
                    {
                        if (!checkedRFPItems.Any(f => f.rfp_serial == parentWindow.materialReleasePermit.GetRfp().GetRFPSerial() && f.rfp_version == parentWindow.materialReleasePermit.GetRfp().GetRFPVersion() && f.requestor_team_id == parentWindow.materialReleasePermit.GetRfp().GetRFPRequestorTeamId()))
                        {
                            Home.Children.Clear();
                            checkedItemsWrapPanel.Children.Clear();
                            checkedOrderItems.Clear();
                        }
                        else
                        {
                            checkedOrderItems.Clear();
                            return;
                        }


                        workFormLabel.Content = "RFP Items";
                        if (addReleasePermitPage.rfpSerials.SelectedIndex != -1)
                        {
                            for (int i = 0; i < parentWindow.materialReleasePermit.GetRfp().rfpItems.Count; i++)
                            {

                                CheckBox rfpItem = new CheckBox();
                                rfpItem.Style = (Style)FindResource("checkBoxStyle");
                                rfpItem.Margin = new Thickness(10);
                                rfpItem.Width = 500;
                                rfpItem.Checked += OnCheckRFPItem;
                                rfpItem.Unchecked += OnUncheckRFPItem;
                                TextBlock rfpItemContent = new TextBlock();
                                rfpItemContent.Tag = Home.Children.Count;
                                rfpItemContent.TextWrapping = TextWrapping.Wrap;

                                if (parentWindow.materialReleasePermit.GetRfp().rfpItems[i].is_company_product)
                                {
                                    rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_category.category_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_type.product_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_brand.brand_name} -" +
                                      $" {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_model.model_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_specs.spec_name} - Reserved Quantity: {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_quantity} - " +
                                      $"To Be Released Quantity:  ";
                                }

                                else
                                {
                                    rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_category.category_name} -" +
                                                    $" {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_type.product_name} - " +
                                                    $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_brand.brand_name} - " +
                                                    $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_model.model_name} - Needed Quantity : {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_quantity} - "
                                                    + $"To Be Released Quantity:  ";
                                }

                                rfpItem.Content = rfpItemContent;
                                rfpItem.Tag = parentWindow.materialReleasePermit.GetRfp().rfpItems[i];
                                if (parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                                    Home.Children.Add(rfpItem);
                            }
                        }

                    }
                    else if (addReleasePermitPage.workFormComboBox.SelectedIndex == 1)
                    {
                        if (!checkedOrderItems.Any(f => f.order_serial == parentWindow.materialReleasePermit.GetWorkOrder().orderSerial))
                        {
                            Home.Children.Clear();
                            checkedItemsWrapPanel.Children.Clear();
                            checkedRFPItems.Clear();
                        }
                        else
                        {

                            return;
                        }
                        workFormLabel.Content = "Work Order Items";
                        for (int i = 0; i < parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList().Length; i++)
                        {
                            CheckBox workOrderItem = new CheckBox();
                            workOrderItem.Style = (Style)FindResource("checkBoxStyle");
                            workOrderItem.Margin = new Thickness(10);
                            workOrderItem.Width = 500;
                            workOrderItem.Checked += OnCheckWorkOrderItem;
                            workOrderItem.Unchecked += OnUncheckWorkOrderItem;
                            TextBlock workOrderItemContent = new TextBlock();
                            workOrderItemContent.TextWrapping = TextWrapping.Wrap;
                            workOrderItemContent.Tag = Home.Children.Count;


                            workOrderItemContent.Text = $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productType.product_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productBrand.brand_name} -" +
                                $" {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productModel.model_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productSpec.spec_name} - Needed Quantity : {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productQuantity}";

                            workOrderItem.Content = workOrderItemContent;
                            if (parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_id != 0 && parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_status.status_id < COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL)
                            {
                                workOrderItem.Tag = parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i];
                                Home.Children.Add(workOrderItem);
                            }

                        }
                    }
                }
                else if (checkedOrderItems.Count > 0)
                {
                    if (addReleasePermitPage.workFormComboBox.SelectedIndex != -1 && addReleasePermitPage.workFormComboBox.SelectedIndex == 0)
                    {
                        if (!checkedRFPItems.Any(f => f.rfp_serial == parentWindow.materialReleasePermit.GetRfp().GetRFPSerial() && f.rfp_version == parentWindow.materialReleasePermit.GetRfp().GetRFPVersion() && f.requestor_team_id == parentWindow.materialReleasePermit.GetRfp().GetRFPRequestorTeamId()))
                        {
                            Home.Children.Clear();
                            checkedItemsWrapPanel.Children.Clear();
                            checkedOrderItems.Clear();
                        }
                        else
                        {
                            checkedOrderItems.Clear();
                            return;
                        }
                        workFormLabel.Content = "RFP Items";
                        if (addReleasePermitPage.rfpSerials.SelectedIndex != -1)
                        {
                            for (int i = 0; i < parentWindow.materialReleasePermit.GetRfp().rfpItems.Count; i++)
                            {
                                CheckBox rfpItem = new CheckBox();
                                rfpItem.Style = (Style)FindResource("checkBoxStyle");
                                rfpItem.Margin = new Thickness(10);
                                rfpItem.Width = 500;
                                rfpItem.Checked += OnCheckRFPItem;
                                rfpItem.Unchecked += OnUncheckRFPItem;
                                TextBlock rfpItemContent = new TextBlock();
                                rfpItemContent.Tag = Home.Children.Count;
                                rfpItemContent.TextWrapping = TextWrapping.Wrap;

                                if (parentWindow.materialReleasePermit.GetRfp().rfpItems[i].is_company_product)
                                {
                                    rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_category.category_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_type.product_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_brand.brand_name} -" +
                                      $" {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_model.model_name} - " +
                                      $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_specs.spec_name} - Needed Quantity : {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_quantity}";
                                }

                                else
                                {
                                    rfpItemContent.Text = $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_category.category_name} -" +
                                                    $" {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_type.product_name} - " +
                                                    $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_brand.brand_name} - " +
                                                    $"{parentWindow.materialReleasePermit.GetRfp().rfpItems[i].product_model.model_name} - Needed Quantity : {parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_quantity} ";
                                }

                                rfpItem.Content = rfpItemContent;
                                rfpItem.Tag = parentWindow.materialReleasePermit.GetRfp().rfpItems[i];
                                if (parentWindow.materialReleasePermit.GetRfp().rfpItems[i].item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                                    Home.Children.Add(rfpItem);
                            }
                        }

                    }
                    else if (addReleasePermitPage.workFormComboBox.SelectedIndex == 1)
                    {
                        if (!checkedOrderItems.Any(f => f.order_serial == parentWindow.materialReleasePermit.GetWorkOrder().orderSerial))
                        {
                            Home.Children.Clear();
                            checkedItemsWrapPanel.Children.Clear();
                            checkedRFPItems.Clear();
                        }
                        else
                        {

                            return;
                        }

                        workFormLabel.Content = "Work Order Items";
                        for (int i = 0; i < parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList().Length; i++)
                        {
                            CheckBox workOrderItem = new CheckBox();
                            workOrderItem.Style = (Style)FindResource("checkBoxStyle");
                            workOrderItem.Margin = new Thickness(10);
                            workOrderItem.Width = 500;
                            workOrderItem.Checked += OnCheckWorkOrderItem;
                            workOrderItem.Unchecked += OnUncheckWorkOrderItem;
                            TextBlock workOrderItemContent = new TextBlock();
                            workOrderItemContent.TextWrapping = TextWrapping.Wrap;
                            workOrderItemContent.Tag = Home.Children.Count;


                            workOrderItemContent.Text = $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productType.product_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productBrand.brand_name} - " +
                                $" {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productModel.model_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productSpec.spec_name}  - Needed Quantity : {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productQuantity} ";

                            workOrderItem.Content = workOrderItemContent;
                            if (parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_id != 0 && parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_status.status_id < COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL)
                            {
                                workOrderItem.Tag = parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i];
                                Home.Children.Add(workOrderItem);
                            }

                        }
                    }
                }
                else
                {
                    Home.Children.Clear();
                    checkedItemsWrapPanel.Children.Clear();
                    if (addReleasePermitPage.workFormComboBox.SelectedIndex != -1 && addReleasePermitPage.workFormComboBox.SelectedIndex == 0)
                    {

                        workFormLabel.Content = "RFP Items";
                        if (addReleasePermitPage.rfpSerials.SelectedIndex != -1)
                        {
                            for (int i = 0; i < parentWindow.releasePermitPage.rfpItems.Count; i++)
                            {


                                CheckBox rfpItem = new CheckBox();
                                rfpItem.Style = (Style)FindResource("checkBoxStyle");
                                rfpItem.Margin = new Thickness(10);
                                rfpItem.Width = 500;
                                rfpItem.Checked += OnCheckRFPItem;
                                rfpItem.Unchecked += OnUncheckRFPItem;
                                TextBlock rfpItemContent = new TextBlock();
                                rfpItemContent.Tag = Home.Children.Count;
                                rfpItemContent.TextWrapping = TextWrapping.Wrap;

                                if (parentWindow.releasePermitPage.rfpItems[i].is_company_product)
                                {

                                    rfpItemContent.Text = $"{parentWindow.releasePermitPage.rfpItems[i].product_category.category_name} - " +
                                      $"{parentWindow.releasePermitPage.rfpItems[i].product_type.product_name} - " +
                                      $"{parentWindow.releasePermitPage.rfpItems[i].product_brand.brand_name} -" +
                                      $" {parentWindow.releasePermitPage.rfpItems[i].product_model.model_name} - " +
                                      $"{parentWindow.releasePermitPage.rfpItems[i].product_specs.spec_name} -" +
                                      $"\nReserved Quantity: {parentWindow.releasePermitPage.rfpItems[i].item_quantity} " +
                                      $"\nTo Be Released Quantity:  ";
                                }

                                else
                                {
                                    rfpItemContent.Text = $"{parentWindow.releasePermitPage.rfpItems[i].product_category.category_name} -" +
                                                    $" {parentWindow.releasePermitPage.rfpItems[i].product_type.product_name} - " +
                                                    $"{parentWindow.releasePermitPage.rfpItems[i].product_brand.brand_name} - " +
                                                    $"{parentWindow.releasePermitPage.rfpItems[i].product_model.model_name} -" +
                                                    $"\nReserved Quantity: {parentWindow.releasePermitPage.rfpItems[i].item_quantity} " +
                                                    $"\nTo Be Released Quantity:  ";
                                }

                                rfpItem.Content = rfpItemContent;
                                rfpItem.Tag = parentWindow.releasePermitPage.rfpItems[i];

                                if (parentWindow.releasePermitPage.rfpItems[i].item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                                    Home.Children.Add(rfpItem);
                            }
                        }

                    }
                    else if (addReleasePermitPage.workFormComboBox.SelectedIndex == 1)
                    {
                        workFormLabel.Content = "Work Order Items";
                        for (int i = 0; i < parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList().Length; i++)
                        {
                            CheckBox workOrderItem = new CheckBox();
                            workOrderItem.Style = (Style)FindResource("checkBoxStyle");
                            workOrderItem.Margin = new Thickness(10);
                            workOrderItem.Width = 500;
                            workOrderItem.Checked += OnCheckWorkOrderItem;
                            workOrderItem.Unchecked += OnUncheckWorkOrderItem;
                            TextBlock workOrderItemContent = new TextBlock();
                            workOrderItemContent.TextWrapping = TextWrapping.Wrap;
                            workOrderItemContent.Tag = Home.Children.Count;


                            workOrderItemContent.Text = $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productType.product_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productBrand.brand_name} -" +
                                $" {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productModel.model_name} - " +
                                $"{parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productSpec.spec_name}  -" +
                                $"\nNeeded Quantity : {parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].productQuantity}" +
                                $"\nTo Be Released Quantity:  ";

                            workOrderItem.Content = workOrderItemContent;
                            if (parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_category.category_id != 0 && parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i].product_status.status_id < COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL)
                            {
                                workOrderItem.Tag = parentWindow.materialReleasePermit.GetWorkOrder().GetOrderProductsList()[i];
                                Home.Children.Add(workOrderItem);
                            }


                        }
                    }
                }
            }

        }

        private void OnUncheckWorkOrderItem(object sender, RoutedEventArgs e)
        {
            CheckBox workOrderItemCheckBox = (CheckBox)sender;
            int position = Home.Children.IndexOf(workOrderItemCheckBox);
            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderProduct = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)workOrderItemCheckBox.Tag;
            SALES_STRUCTS.WORK_ORDER_MAX_STRUCT workOrder = checkedOrderItems.Find(f => f.products.Contains(orderProduct));
            checkedOrderItems.Remove(workOrder);
            orderItemHolder = null;
            for (int i = 0; i < checkedItemsWrapPanel.Children.Count; i++)
            {
                Border workOrderBorder = checkedItemsWrapPanel.Children[i] as Border;
                if ((position + 1) == Convert.ToInt32(workOrderBorder.Tag))
                {
                    checkedItemsWrapPanel.Children.RemoveAt(i);
                    break;
                }
            }
        }

        private void OnCheckWorkOrderItem(object sender, RoutedEventArgs e)
        {
            CheckBox workOrderCheckedItemCheckBox = (CheckBox)sender;
            orderItemHolder = workOrderCheckedItemCheckBox;
            SALES_STRUCTS.WORK_ORDER_MAX_STRUCT workOrder = new SALES_STRUCTS.WORK_ORDER_MAX_STRUCT();
            workOrder.order_serial = addReleasePermitPage.materialReleasePermit.GetWorkOrder().GetOrderSerial();
            workOrder.products = new List<PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT>();
            workOrder.products.Add((PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)workOrderCheckedItemCheckBox.Tag);
            checkedOrderItems.Add(workOrder);
            int position = Home.Children.IndexOf(workOrderCheckedItemCheckBox);
            CreateSelectedItemsCard(false, position + 1, workOrderCheckedItemCheckBox);
        }

        private void OnUncheckRFPItem(object sender, RoutedEventArgs e)
        {
            CheckBox rfpItemCheckBox = (CheckBox)sender;
            int position = Home.Children.IndexOf(rfpItemCheckBox);
            RFP_ITEM_MIN_STRUCT rfpItem = (RFP_ITEM_MIN_STRUCT)rfpItemCheckBox.Tag;
            RFP_MAX_STRUCT rfp = checkedRFPItems.Find(f => f.rfps_items_min.Contains(rfpItem));
            checkedRFPItems.Remove(rfp);
            rfpItemCheckBoxHolder = null;
            for(int i = 0;i<checkedItemsWrapPanel.Children.Count;i++)
            {
                Border rfpItemBorder = checkedItemsWrapPanel.Children[i] as Border;
                if((position+1)==Convert.ToInt32(rfpItemBorder.Tag))
                {
                    checkedItemsWrapPanel.Children.RemoveAt(i);
                    break;
                }
            }
          
        }

        private void OnCheckRFPItem(object sender, RoutedEventArgs e)
        {
            if(viewAddCondition==COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                checkedItemsCounter = 0;
                checkedItemsCounterLabel.Content = checkedItemsCounter;
                CheckBox rfpItemCheckBox = (CheckBox)sender;
                RFP_MAX_STRUCT rfp = new RFP_MAX_STRUCT();
                rfp.rfp_serial = addReleasePermitPage.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpSerial;
                rfp.requestor_team_id = addReleasePermitPage.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpRequestorTeam;
                rfp.rfp_version = addReleasePermitPage.materialReleasePermit.GetReleaseItems()[0].rfp_info.rfpVersion;
                //rfp.rfps_items_min = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
                //rfp.rfps_items_min.Add((PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)rfpItemCheckBox.Tag);
                checkedRFPItems.Add(rfp);
                rfpItemCheckBoxHolder = rfpItemCheckBox;
                int position = Home.Children.IndexOf(rfpItemCheckBox);
                CreateSelectedItemsCard(true, position + 1, rfpItemCheckBox);
                for (int i = 0; i < Home.Children.Count; i++)
                {
                    CheckBox rfpItemCheckBoxx = (CheckBox)Home.Children[i];
                    if (rfpItemCheckBoxx != rfpItemCheckBox)
                        rfpItemCheckBoxx.IsChecked = false;
                }
            }
            else
            {
                checkedItemsCounter = 0;
                checkedItemsCounterLabel.Content = checkedItemsCounter;
                CheckBox rfpItemCheckBox = (CheckBox)sender;
                RFP_MAX_STRUCT rfp = new RFP_MAX_STRUCT();
                rfp.rfp_serial = addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPSerial();
                rfp.requestor_team_id = addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPRequestorTeamId();
                rfp.rfp_version = addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPVersion();
                rfp.rfps_items_min = new List<PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT>();
                rfp.rfps_items_min.Add((PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)rfpItemCheckBox.Tag);
                checkedRFPItems.Add(rfp);
                rfpItemCheckBoxHolder = rfpItemCheckBox;
                int position = Home.Children.IndexOf(rfpItemCheckBox);
                CreateSelectedItemsCard(true, position + 1, rfpItemCheckBox);
                for (int i = 0; i < Home.Children.Count; i++)
                {
                    CheckBox rfpItemCheckBoxx = (CheckBox)Home.Children[i];
                    if (rfpItemCheckBoxx != rfpItemCheckBox)
                        rfpItemCheckBoxx.IsChecked = false;
                }

            }

        }
        public void CreateSelectedItemsCard(bool isRFP, int itemNumber, CheckBox checkedCheckBox)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                if (isRFP)
                {
                    this.isRFP = true;
                    Border itemsBorder = new Border();
                    itemsBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    itemsBorder.Margin = new Thickness(24);
                    itemsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    itemsBorder.BorderThickness = new Thickness(1);
                    itemsBorder.Width = 600;
                    itemsBorder.Height = 500;
                    itemsBorder.Background = Brushes.White;
                    itemsBorder.Tag = itemNumber;
                    DropShadowEffect dropShadowEffect = new DropShadowEffect();
                    dropShadowEffect.ShadowDepth = 1;
                    dropShadowEffect.Opacity = 0.1;
                    itemsBorder.Effect = dropShadowEffect;
                    Label workFormLabel = new Label();
                    workFormLabel.Content = $"RFP Item #{checkedItemsWrapPanel.Children.Count + 1}";
                    workFormLabel.Width = 600;
                    workFormLabel.Background = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    workFormLabel.Foreground = Brushes.White;
                    workFormLabel.Padding = new Thickness(250, 10, 0, 10);
                    workFormLabel.Style = (Style)FindResource("labelStyle");

                    StackPanel checkedItemStackPanel = new StackPanel();
                    itemsBorder.Child = checkedItemStackPanel;
                    checkedItemStackPanel.Children.Add(workFormLabel);
                    ScrollViewer scrollViewer = new ScrollViewer();
                    StackPanel innerStackPanel = new StackPanel();
                    
                    scrollViewer.Content = innerStackPanel;
                    scrollViewer.Height = 400;
                    checkedItemStackPanel.Children.Add(scrollViewer);
                    INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM orderProduct = (INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM)checkedCheckBox.Tag;
                    for (int i = 0; i < parentWindow.materialReleasePermit.GetReleaseItems().Count; i++)
                    {
                        if (parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number == "")
                        {
                            CheckBox entryPermitItemCheckBox = new CheckBox();
                            entryPermitItemCheckBox.Style = (Style)FindResource("checkBoxStyle");
                            entryPermitItemCheckBox.Margin = new Thickness(10);
                            entryPermitItemCheckBox.Width = 500;
                            entryPermitItemCheckBox.Checked += OnCheckEntryPermitItem;
                            entryPermitItemCheckBox.Unchecked += OnUnCheckEnrtyPermitItem;
                            entryPermitItemCheckBox.Tag = parentWindow.materialReleasePermit.GetReleaseItems()[i];

                            WrapPanel entryPermitItemCheckBoxContentWrapPanel = new WrapPanel();
                            entryPermitItemCheckBoxContentWrapPanel.Tag = orderProduct;
                            TextBlock entryPermitCheckBoxContent = new TextBlock();
                            entryPermitCheckBoxContent.TextWrapping = TextWrapping.Wrap;
                            entryPermitCheckBoxContent.Text = $@"Entry Permit {parentWindow.materialReleasePermit.GetReleaseItems()[i].material_entry_permit_id} :{parentWindow.materialReleasePermit.GetReleaseItems()[i].orderCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderModel.model_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderSpecs.spec_name} ";

                            Label availableQuantityLabel = new Label();
                            availableQuantityLabel.Style = (Style)FindResource("labelStyle");
                            availableQuantityLabel.Margin = new Thickness(0);
                            availableQuantityLabel.Content = "Quantity:";

                            TextBox availableQuantityTextBox = new TextBox();
                            availableQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                            availableQuantityTextBox.Margin = new Thickness(0);
                            availableQuantityTextBox.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].entered_quantity.ToString();
                            availableQuantityTextBox.IsEnabled = false;

                            Label reservedQuantityLabel = new Label();
                            reservedQuantityLabel.Style = (Style)FindResource("labelStyle");
                            reservedQuantityLabel.Margin = new Thickness(0);
                            reservedQuantityLabel.Width = 170;
                            reservedQuantityLabel.Content = "Reserved Quantity:";

                            TextBox reservedQuantity = new TextBox();
                            reservedQuantity.Style = (Style)FindResource("microTextBoxStyle");
                            reservedQuantity.Margin = new Thickness(0);
                            reservedQuantity.Text = orderProduct.entered_quantity.ToString();
                            reservedQuantity.Tag = orderProduct;
                            reservedQuantity.IsEnabled = false;

                            Label toBeReleasedQuantity = new Label();
                            toBeReleasedQuantity.Style = (Style)FindResource("labelStyle");
                            toBeReleasedQuantity.Margin = new Thickness(0);
                            toBeReleasedQuantity.Width = 180;
                            toBeReleasedQuantity.Content = "To Be Released Quantity:";

                            TextBox toBeReleasedQuantityTextBox = new TextBox();
                            toBeReleasedQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                            toBeReleasedQuantityTextBox.Margin = new Thickness(0);
                            toBeReleasedQuantityTextBox.TextChanged += OnTextChangedToBeReleasedQuantity;
                            toBeReleasedQuantityTextBox.IsEnabled = false;
                            toBeReleasedQuantityTextBox.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].released_quantity_release.ToString();

                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(entryPermitCheckBoxContent);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityLabel);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityTextBox);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantityLabel);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantity);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantity);
                            entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantityTextBox);

                            entryPermitItemCheckBox.Content = entryPermitItemCheckBoxContentWrapPanel;
                            innerStackPanel.Children.Add(entryPermitItemCheckBox);
                            
                            entryPermitItemCheckBox.IsChecked = true;
                            entryPermitItemCheckBox.IsEnabled = false;
                            toBeReleasedQuantityTextBox.IsEnabled = false;
                        }
                        else
                        {
                            WrapPanel entryPermitItemContentWrapPanel = new WrapPanel();
                            TextBlock entryPermitContent = new TextBlock();
                            entryPermitContent.TextWrapping = TextWrapping.Wrap;
                            entryPermitContent.Style = (Style)FindResource("tableSubItemTextblock");
                            entryPermitContent.Text = $@"Entry Permit {parentWindow.materialReleasePermit.GetReleaseItems()[i].material_entry_permit_id}: {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemcompanyCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialitemCompanyproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanyBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanyModel.model_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].materialItemCompanySpecs.spec_name} ";

                            Label serialNumber = new Label();
                            serialNumber.Style = (Style)FindResource("labelStyle");
                            serialNumber.Margin = new Thickness(0);
                            serialNumber.Width = 200;
                            serialNumber.Content = $"Serial Number :{parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number} ";

                            entryPermitItemContentWrapPanel.Children.Add(entryPermitContent);
                            entryPermitItemContentWrapPanel.Children.Add(serialNumber);
                            if (innerStackPanel.Children.Count <= orderProduct.entered_quantity)
                                innerStackPanel.Children.Add(entryPermitItemContentWrapPanel);

                        }

                    }

                    checkedItemsWrapPanel.Children.Add(itemsBorder);
                }
                else
                {
                    this.isRFP = false;
                    Border itemsBorder = new Border();
                    itemsBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    itemsBorder.Margin = new Thickness(24);
                    itemsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    itemsBorder.BorderThickness = new Thickness(1);
                    itemsBorder.Width = 600;
                    itemsBorder.Height = 500;
                    itemsBorder.Background = Brushes.White;
                    itemsBorder.Tag = itemNumber;
                    DropShadowEffect dropShadowEffect = new DropShadowEffect();
                    dropShadowEffect.ShadowDepth = 1;
                    dropShadowEffect.Opacity = 0.1;
                    itemsBorder.Effect = dropShadowEffect;
                    Label workFormLabel = new Label();
                    workFormLabel.Content = $"Work Order Item #{checkedItemsWrapPanel.Children.Count + 1}";
                    workFormLabel.Width = 600;
                    workFormLabel.Background = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    workFormLabel.Foreground = Brushes.White;
                    workFormLabel.Padding = new Thickness(250, 10, 0, 10);
                    workFormLabel.Style = (Style)FindResource("labelStyle");

                    StackPanel checkedItemStackPanel = new StackPanel();
                    itemsBorder.Child = checkedItemStackPanel;
                    checkedItemStackPanel.Children.Add(workFormLabel);
                    StackPanel innerStackPanel = new StackPanel();
                    checkedItemStackPanel.Children.Add(innerStackPanel);
                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderProduct = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)checkedCheckBox.Tag;
                    for (int i = 0; i < parentWindow.materialReleasePermit.GetReleaseItems().Count; i++)
                    {
                          if (parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number=="")
                             {
                                    CheckBox entryPermitItemCheckBox = new CheckBox();
                                    entryPermitItemCheckBox.Style = (Style)FindResource("checkBoxStyle");
                                    entryPermitItemCheckBox.Margin = new Thickness(10);
                                    entryPermitItemCheckBox.Width = 500;
                                    entryPermitItemCheckBox.Checked += OnCheckEntryPermitItem;
                                    entryPermitItemCheckBox.Unchecked += OnUnCheckEnrtyPermitItem;
                                    entryPermitItemCheckBox.Tag = parentWindow.materialReleasePermit.GetReleaseItems()[i];

                                    WrapPanel entryPermitItemCheckBoxContentWrapPanel = new WrapPanel();
                                    entryPermitItemCheckBoxContentWrapPanel.Tag = orderProduct;
                                    TextBlock entryPermitCheckBoxContent = new TextBlock();
                                    entryPermitCheckBoxContent.TextWrapping = TextWrapping.Wrap;
                                    entryPermitCheckBoxContent.Text = $@"Entry Permit {parentWindow.materialReleasePermit.GetReleaseItems()[i].material_entry_permit_id} :{parentWindow.materialReleasePermit.GetReleaseItems()[i].orderCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderModel.model_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderSpecs.spec_name} ";

                                    Label availableQuantityLabel = new Label();
                                    availableQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    availableQuantityLabel.Margin = new Thickness(0);
                                    availableQuantityLabel.Content = "Quantity:";

                                    TextBox availableQuantityTextBox = new TextBox();
                                    availableQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    availableQuantityTextBox.Margin = new Thickness(0);
                                    availableQuantityTextBox.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].entered_quantity.ToString();
                                    availableQuantityTextBox.IsEnabled = false;

                                    Label reservedQuantityLabel = new Label();
                                    reservedQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    reservedQuantityLabel.Margin = new Thickness(0);
                                    reservedQuantityLabel.Width = 170;
                                    reservedQuantityLabel.Content = "Reserved Quantity:";

                                    TextBox reservedQuantity = new TextBox();
                                    reservedQuantity.Style = (Style)FindResource("microTextBoxStyle");
                                    reservedQuantity.Margin = new Thickness(0);
                                    reservedQuantity.Text = orderProduct.productQuantity.ToString();
                                    reservedQuantity.Tag = orderProduct;
                                    reservedQuantity.IsEnabled = false;

                                    Label toBeReleasedQuantity = new Label();
                                    toBeReleasedQuantity.Style = (Style)FindResource("labelStyle");
                                    toBeReleasedQuantity.Margin = new Thickness(0);
                                    toBeReleasedQuantity.Width = 180;
                                    toBeReleasedQuantity.Content = "To Be Released Quantity:";

                                    TextBox toBeReleasedQuantityTextBox = new TextBox();
                                    toBeReleasedQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    toBeReleasedQuantityTextBox.Margin = new Thickness(0);
                                    toBeReleasedQuantityTextBox.TextChanged += OnTextChangedToBeReleasedQuantity;
                                    toBeReleasedQuantityTextBox.IsEnabled = false;
                                    toBeReleasedQuantityTextBox.Text = parentWindow.materialReleasePermit.GetReleaseItems()[i].released_quantity_release.ToString();

                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(entryPermitCheckBoxContent);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityTextBox);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantityTextBox);

                                    entryPermitItemCheckBox.Content = entryPermitItemCheckBoxContentWrapPanel;
                                    innerStackPanel.Children.Add(entryPermitItemCheckBox);

                                   entryPermitItemCheckBox.IsChecked = true;
                            entryPermitItemCheckBox.IsEnabled = false;
                            toBeReleasedQuantityTextBox.IsEnabled = false;
                        }
                         else
                         {
                                    WrapPanel entryPermitItemContentWrapPanel = new WrapPanel();
                                    TextBlock entryPermitContent = new TextBlock();
                                    entryPermitContent.TextWrapping = TextWrapping.Wrap;
                                    entryPermitContent.Style = (Style)FindResource("tableSubItemTextblock");
                                    entryPermitContent.Text = $@"Entry Permit {parentWindow.materialReleasePermit.GetReleaseItems()[i].material_entry_permit_id}: {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderCategory.category_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderproduct.product_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderBrand.brand_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderModel.model_name} - {parentWindow.materialReleasePermit.GetReleaseItems()[i].orderSpecs.spec_name} ";

                            Label serialNumber = new Label();
                                    serialNumber.Style = (Style)FindResource("labelStyle");
                                    serialNumber.Margin = new Thickness(0);
                            serialNumber.Width = 200;
                                    serialNumber.Content = $"Serial Number :{parentWindow.materialReleasePermit.GetReleaseItems()[i].entryPermit_product_serial_number} ";

                                    entryPermitItemContentWrapPanel.Children.Add(entryPermitContent);
                                    entryPermitItemContentWrapPanel.Children.Add(serialNumber);
                                    if (innerStackPanel.Children.Count <= orderProduct.productQuantity)
                                        innerStackPanel.Children.Add(entryPermitItemContentWrapPanel);

                         }

                    }

                    checkedItemsWrapPanel.Children.Add(itemsBorder);
                }
                itemsScroll.ScrollToBottom();
            }
            else
            {
                if (isRFP)
                {
                    this.isRFP = true;
                    Border itemsBorder = new Border();
                    itemsBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    itemsBorder.Margin = new Thickness(24);
                    itemsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    itemsBorder.BorderThickness = new Thickness(1);
                    itemsBorder.Width = 600;
                    itemsBorder.Height = 500;
                    itemsBorder.Background = Brushes.White;
                    itemsBorder.Tag = itemNumber;
                    DropShadowEffect dropShadowEffect = new DropShadowEffect();
                    dropShadowEffect.ShadowDepth = 1;
                    dropShadowEffect.Opacity = 0.1;
                    itemsBorder.Effect = dropShadowEffect;
                    Label workFormLabel = new Label();
                    workFormLabel.Content = $"RFP Item #{itemNumber}";
                    workFormLabel.Width = 600;
                    workFormLabel.Background = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    workFormLabel.Foreground = Brushes.White;
                    workFormLabel.Padding = new Thickness(250, 10, 0, 10);
                    workFormLabel.Style = (Style)FindResource("labelStyle");

                    StackPanel checkedItemStackPanel = new StackPanel();
                    itemsBorder.Child = checkedItemStackPanel;

                    checkedItemStackPanel.Children.Add(workFormLabel);


                    ScrollViewer itemScrollViewer = new ScrollViewer();
                    StackPanel innerStackPanel = new StackPanel();
                    itemScrollViewer.Content = innerStackPanel;
                    itemScrollViewer.Height = 400;

                    checkedItemStackPanel.Children.Add(itemScrollViewer);

                    PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItem = (PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT)checkedCheckBox.Tag;
                    TextBlock checkedCheckBoxContent = (TextBlock)checkedCheckBox.Content;
                    for (int i = 0; i < entryPermitList.Count; i++)
                    {
                        for (int j = 0; j < entryPermitList[i].items.Count; j++)
                        {
                            if (
                                rfpItem.product_category.category_id == entryPermitList[i].items[j].product_category.category_id &&
                                rfpItem.product_type.type_id == entryPermitList[i].items[j].product_type.type_id &&
                                rfpItem.product_brand.brand_id == entryPermitList[i].items[j].product_brand.brand_id &&
                                rfpItem.product_model.model_id == entryPermitList[i].items[j].product_model.model_id &&
                                rfpItem.product_specs.spec_id == entryPermitList[i].items[j].product_specs.spec_id)
                            {

                                if (!entryPermitList[i].items[j].product_model.has_serial_number && entryPermitList[i].items[j].product_serial_number == "")
                                {
                                    CheckBox entryPermitItemCheckBox = new CheckBox();
                                    entryPermitItemCheckBox.Style = (Style)FindResource("checkBoxStyle");
                                    entryPermitItemCheckBox.Margin = new Thickness(10);
                                    entryPermitItemCheckBox.Width = 500;
                                    entryPermitItemCheckBox.Checked += OnCheckEntryPermitItem;
                                    entryPermitItemCheckBox.Unchecked += OnUnCheckEnrtyPermitItem;
                                    entryPermitItemCheckBox.Tag = entryPermitList[i].items[j];

                                    WrapPanel entryPermitItemCheckBoxContentWrapPanel = new WrapPanel();
                                    entryPermitItemCheckBoxContentWrapPanel.Tag = rfpItem;
                                    TextBlock entryPermitCheckBoxContent = new TextBlock();
                                    entryPermitCheckBoxContent.TextWrapping = TextWrapping.Wrap;

                                    entryPermitCheckBoxContent.Text = $@"Entry Permit {entryPermitList[i].entry_permit_id} :{entryPermitList[i].items[j].product_category.category_name} - {entryPermitList[i].items[j].product_type.product_name} - {entryPermitList[i].items[j].product_brand.brand_name} - {entryPermitList[i].items[j].product_model.model_name} - {entryPermitList[i].items[j].product_specs.spec_name} ";

                                    Label availableQuantityLabel = new Label();
                                    availableQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    availableQuantityLabel.Margin = new Thickness(0);
                                    availableQuantityLabel.Content = "Quantity:";

                                    TextBox availableQuantityTextBox = new TextBox();
                                    availableQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    availableQuantityTextBox.Margin = new Thickness(0);
                                    availableQuantityTextBox.Text = entryPermitList[i].items[j].quantity.ToString();
                                    availableQuantityTextBox.IsEnabled = false;

                                    Label reservedQuantityLabel = new Label();
                                    reservedQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    reservedQuantityLabel.Margin = new Thickness(0);
                                    reservedQuantityLabel.Width = 170;
                                    reservedQuantityLabel.Content = "Reserved Quantity:";

                                    TextBox reservedQuantity = new TextBox();
                                    reservedQuantity.Style = (Style)FindResource("microTextBoxStyle");
                                    reservedQuantity.Margin = new Thickness(0);
                                    reservedQuantity.Text = rfpItem.item_quantity.ToString();
                                    reservedQuantity.Tag = rfpItem.item_quantity.ToString();
                                    reservedQuantity.IsEnabled = false;

                                    Label toBeReleasedQuantity = new Label();
                                    toBeReleasedQuantity.Style = (Style)FindResource("labelStyle");
                                    toBeReleasedQuantity.Margin = new Thickness(0);
                                    toBeReleasedQuantity.Width = 180;
                                    toBeReleasedQuantity.Content = "To Be Released Quantity:";

                                    TextBox toBeReleasedQuantityTextBox = new TextBox();
                                    toBeReleasedQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    toBeReleasedQuantityTextBox.Margin = new Thickness(0);
                                    toBeReleasedQuantityTextBox.TextChanged += OnTextChangedToBeReleasedQuantity;
                                    toBeReleasedQuantityTextBox.IsEnabled = false;

                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(entryPermitCheckBoxContent);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityTextBox);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantityTextBox);

                                    entryPermitItemCheckBox.Content = entryPermitItemCheckBoxContentWrapPanel;
                                    innerStackPanel.Children.Add(entryPermitItemCheckBox);
                                }
                                else
                                {
                                    CheckBox entryPermitItemCheckBox = new CheckBox();
                                    entryPermitItemCheckBox.Style = (Style)FindResource("checkBoxStyle");
                                    entryPermitItemCheckBox.Margin = new Thickness(10);
                                    entryPermitItemCheckBox.Width = 500;
                                    entryPermitItemCheckBox.Checked += OnCheckEntryPermitItem;
                                    entryPermitItemCheckBox.Unchecked += OnUnCheckEnrtyPermitItem;
                                    entryPermitItemCheckBox.Tag = entryPermitList[i].items[j];

                                    WrapPanel entryPermitItemContentWrapPanel = new WrapPanel();
                                    entryPermitItemContentWrapPanel.Tag = rfpItem;
                                    TextBlock entryPermitContent = new TextBlock();
                                    entryPermitContent.TextWrapping = TextWrapping.Wrap;
                                    entryPermitContent.Text = $@"Entry Permit {entryPermitList[i].entry_permit_id} :{entryPermitList[i].items[j].product_category.category_name} - {entryPermitList[i].items[j].product_type.product_name} - {entryPermitList[i].items[j].product_brand.brand_name} - {entryPermitList[i].items[j].product_model.model_name} - {entryPermitList[i].items[j].product_specs.spec_name} ";

                                    Label serialNumber = new Label();
                                    serialNumber.Style = (Style)FindResource("labelStyle");
                                    serialNumber.Margin = new Thickness(10, 0, 0, 20);
                                    serialNumber.Width = 300;
                                    serialNumber.Content = $"Serial Number :{entryPermitList[i].items[j].product_serial_number} ";

                                    entryPermitItemContentWrapPanel.Children.Add(entryPermitContent);
                                    entryPermitItemContentWrapPanel.Children.Add(serialNumber);
                                    entryPermitItemCheckBox.Content = entryPermitItemContentWrapPanel;

                                    if (entryPermitList[i].items[j].rfp_info.rfpRequestorTeam == addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPRequestorTeamId()
                                        && entryPermitList[i].items[j].rfp_info.rfpSerial == addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPSerial()
                                        && entryPermitList[i].items[j].rfp_info.rfpVersion == addReleasePermitPage.materialReleasePermit.GetRfp().GetRFPVersion())
                                    {
                                        entryPermitItemCheckBox.IsChecked = true;

                                    }
                                    else if (parentWindow.materialReleasePermit.GetRfp().rfpItems.Any(f => f.product_serial_number == entryPermitList[i].items[j].product_serial_number && (f.product_category.category_id == entryPermitList[i].items[j].product_category.category_id)
                                                                                                      && (f.product_type.type_id == entryPermitList[i].items[j].product_type.type_id)
                                                                                                      && (f.product_brand.brand_id == entryPermitList[i].items[j].product_brand.brand_id)
                                                                                                      && (f.product_model.model_id == entryPermitList[i].items[j].product_model.model_id)) && rfpItem.item_status.status_id == COMPANY_WORK_MACROS.RFP_AT_STOCK)
                                    {
                                        entryPermitItemCheckBox.IsChecked = true;
                                    }


                                    innerStackPanel.Children.Add(entryPermitItemCheckBox);

                                }

                            }
                        }
                    }

                    checkedItemsWrapPanel.Children.Add(itemsBorder);
                }
                else
                {
                    this.isRFP = false;
                    Border itemsBorder = new Border();
                    itemsBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    itemsBorder.Margin = new Thickness(24);
                    itemsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    itemsBorder.BorderThickness = new Thickness(1);
                    itemsBorder.Width = 600;
                    itemsBorder.Height = 500;
                    itemsBorder.Background = Brushes.White;
                    itemsBorder.Tag = itemNumber;
                    DropShadowEffect dropShadowEffect = new DropShadowEffect();
                    dropShadowEffect.ShadowDepth = 1;
                    dropShadowEffect.Opacity = 0.1;
                    itemsBorder.Effect = dropShadowEffect;
                    Label workFormLabel = new Label();
                    workFormLabel.Content = $"Work Order Item #{checkedItemsWrapPanel.Children.Count + 1}";
                    workFormLabel.Width = 600;
                    workFormLabel.Background = new SolidColorBrush(Color.FromArgb(255, 16, 90, 151));
                    workFormLabel.Foreground = Brushes.White;
                    workFormLabel.Padding = new Thickness(250, 10, 0, 10);
                    workFormLabel.Style = (Style)FindResource("labelStyle");

                    StackPanel checkedItemStackPanel = new StackPanel();
                    itemsBorder.Child = checkedItemStackPanel;
                    checkedItemStackPanel.Children.Add(workFormLabel);
                    StackPanel innerStackPanel = new StackPanel();
                    checkedItemStackPanel.Children.Add(innerStackPanel);
                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderProduct = (PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT)checkedCheckBox.Tag;
                    for (int i = 0; i < entryPermitList.Count; i++)
                    {
                        for (int j = 0; j < entryPermitList[i].items.Count; j++)
                        {
                            if (orderProduct.product_category.category_id == entryPermitList[i].items[j].product_category.category_id &&
                              orderProduct.productType.type_id == entryPermitList[i].items[j].product_type.type_id &&
                              orderProduct.productBrand.brand_id == entryPermitList[i].items[j].product_brand.brand_id &&
                              orderProduct.productModel.model_id == entryPermitList[i].items[j].product_model.model_id &&
                              orderProduct.productSpec.spec_id == entryPermitList[i].items[j].product_specs.spec_id)
                            {

                                if (!entryPermitList[i].items[j].product_model.has_serial_number && entryPermitList[i].items[j].product_serial_number == "")
                                {
                                    CheckBox entryPermitItemCheckBox = new CheckBox();
                                    entryPermitItemCheckBox.Style = (Style)FindResource("checkBoxStyle");
                                    entryPermitItemCheckBox.Margin = new Thickness(10);
                                    entryPermitItemCheckBox.Width = 500;
                                    entryPermitItemCheckBox.Checked += OnCheckEntryPermitItem;
                                    entryPermitItemCheckBox.Unchecked += OnUnCheckEnrtyPermitItem;
                                    entryPermitItemCheckBox.Tag = entryPermitList[i].items[j];

                                    WrapPanel entryPermitItemCheckBoxContentWrapPanel = new WrapPanel();
                                    entryPermitItemCheckBoxContentWrapPanel.Tag = orderProduct;
                                    TextBlock entryPermitCheckBoxContent = new TextBlock();
                                    entryPermitCheckBoxContent.TextWrapping = TextWrapping.Wrap;
                                    entryPermitCheckBoxContent.Text = $@"Entry Permit {entryPermitList[i].entry_permit_id} :{entryPermitList[i].items[j].product_category.category_name} - {entryPermitList[i].items[j].product_type.product_name} - {entryPermitList[i].items[j].product_brand.brand_name} - {entryPermitList[i].items[j].product_model.model_name} - {entryPermitList[i].items[j].product_specs.spec_name} ";

                                    Label availableQuantityLabel = new Label();
                                    availableQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    availableQuantityLabel.Margin = new Thickness(0);
                                    availableQuantityLabel.Content = "Quantity:";

                                    TextBox availableQuantityTextBox = new TextBox();
                                    availableQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    availableQuantityTextBox.Margin = new Thickness(0);
                                    availableQuantityTextBox.Text = entryPermitList[i].items[j].quantity.ToString();
                                    availableQuantityTextBox.IsEnabled = false;

                                    Label reservedQuantityLabel = new Label();
                                    reservedQuantityLabel.Style = (Style)FindResource("labelStyle");
                                    reservedQuantityLabel.Margin = new Thickness(0);
                                    reservedQuantityLabel.Width = 170;
                                    reservedQuantityLabel.Content = "Reserved Quantity:";

                                    TextBox reservedQuantity = new TextBox();
                                    reservedQuantity.Style = (Style)FindResource("microTextBoxStyle");
                                    reservedQuantity.Margin = new Thickness(0);
                                    reservedQuantity.Text = orderProduct.productQuantity.ToString();
                                    reservedQuantity.Tag = orderProduct;
                                    reservedQuantity.IsEnabled = false;

                                    Label toBeReleasedQuantity = new Label();
                                    toBeReleasedQuantity.Style = (Style)FindResource("labelStyle");
                                    toBeReleasedQuantity.Margin = new Thickness(0);
                                    toBeReleasedQuantity.Width = 180;
                                    toBeReleasedQuantity.Content = "To Be Released Quantity:";

                                    TextBox toBeReleasedQuantityTextBox = new TextBox();
                                    toBeReleasedQuantityTextBox.Style = (Style)FindResource("microTextBoxStyle");
                                    toBeReleasedQuantityTextBox.Margin = new Thickness(0);
                                    toBeReleasedQuantityTextBox.TextChanged += OnTextChangedToBeReleasedQuantity;
                                    toBeReleasedQuantityTextBox.IsEnabled = false;

                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(entryPermitCheckBoxContent);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(availableQuantityTextBox);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantityLabel);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(reservedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantity);
                                    entryPermitItemCheckBoxContentWrapPanel.Children.Add(toBeReleasedQuantityTextBox);

                                    entryPermitItemCheckBox.Content = entryPermitItemCheckBoxContentWrapPanel;
                                    innerStackPanel.Children.Add(entryPermitItemCheckBox);
                                }
                                else
                                {
                                    WrapPanel entryPermitItemContentWrapPanel = new WrapPanel();
                                    TextBlock entryPermitContent = new TextBlock();
                                    entryPermitContent.TextWrapping = TextWrapping.Wrap;
                                    entryPermitContent.Style = (Style)FindResource("labelStyle");
                                    entryPermitContent.Text = $@"Entry Permit {entryPermitList[i].entry_permit_id} :{entryPermitList[i].items[j].product_category.category_name} - {entryPermitList[i].items[j].product_type.product_name} - {entryPermitList[i].items[j].product_brand.brand_name} - {entryPermitList[i].items[j].product_model.model_name} - {entryPermitList[i].items[j].product_specs.spec_name} ";

                                    Label serialNumber = new Label();
                                    serialNumber.Style = (Style)FindResource("labelStyle");
                                    serialNumber.Margin = new Thickness(0);
                                    serialNumber.Width = 200;
                                    serialNumber.Content = $"Serial Number :{entryPermitList[i].items[j].product_serial_number} ";

                                    entryPermitItemContentWrapPanel.Children.Add(entryPermitContent);
                                    entryPermitItemContentWrapPanel.Children.Add(serialNumber);
                                    if (innerStackPanel.Children.Count <= orderProduct.productQuantity)
                                        innerStackPanel.Children.Add(entryPermitItemContentWrapPanel);

                                }

                            }

                        }
                    }




                    checkedItemsWrapPanel.Children.Add(itemsBorder);
                }
                itemsScroll.ScrollToBottom();
            }
           
        }

       

        private void OnTextChangedToBeReleasedQuantity(object sender, TextChangedEventArgs e)
        {
            if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)
            {
                TextBox toBeReleasedTextBox = (TextBox)sender;
                if (toBeReleasedTextBox.Text != "")
                {


                    if (!int.TryParse(toBeReleasedTextBox.Text, out int value))
                    {
                        toBeReleasedTextBox.Text = string.Empty;
                        System.Windows.Forms.MessageBox.Show("Please enter numbers not characters.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;

                    }
                    else
                    {
                        WrapPanel contentWrapPanel = (WrapPanel)toBeReleasedTextBox.Parent;
                        TextBox reservedTextBox = (TextBox)contentWrapPanel.Children[4];
                        decimal reservedQuantity = decimal.Parse(reservedTextBox.Text);
                        int toBereleasedQuantity = int.Parse(toBeReleasedTextBox.Text);
                        if (toBereleasedQuantity > reservedQuantity)
                        {
                            toBeReleasedTextBox.Text = string.Empty;
                            System.Windows.Forms.MessageBox.Show("Released quantity exceeded the reserved quantity.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return;
                        }
                        else if (toBereleasedQuantity <= reservedQuantity)
                        {
                            reservedQuantity -= toBereleasedQuantity;
                            reservedTextBox.Text = reservedQuantity.ToString();
                            TextBlock checkBoxContent = (TextBlock)orderItemHolder.Content;
                            string[] splitedContent = checkBoxContent.Text.Split('\n');
                            splitedContent[1] = "Reserved Quantity: " + reservedQuantity.ToString();
                            splitedContent[2] = "To Be Released Quantity: " + toBereleasedQuantity;
                            checkBoxContent.Text = String.Join("\n", splitedContent);
                        }
                        toBeReleasedTextBox.Tag = toBereleasedQuantity;
                    }
                }
                else
                {
                    int toBereleasedQuantity = int.Parse(toBeReleasedTextBox.Tag.ToString());
                    WrapPanel contentWrapPanel = (WrapPanel)toBeReleasedTextBox.Parent;
                    TextBox reservedTextBox = (TextBox)contentWrapPanel.Children[4];
                    decimal reservedQuantity = decimal.Parse(reservedTextBox.Text);
                    reservedQuantity += toBereleasedQuantity;
                    if (reservedQuantity <= decimal.Parse(reservedTextBox.Tag.ToString()))
                    {
                        reservedTextBox.Text = reservedQuantity.ToString();
                        TextBlock checkBoxContent = (TextBlock)rfpItemCheckBoxHolder.Content;
                        string[] splitedContent = checkBoxContent.Text.Split('\n');
                        splitedContent[1] = "Reserved Quantity: " + reservedQuantity.ToString();
                        splitedContent[2] = "To Be Released Quantity: 0";
                        checkBoxContent.Text = String.Join("\n", splitedContent);
                    }


                }
            }
           
          
 


        }

        private void OnUnCheckEnrtyPermitItem(object sender, RoutedEventArgs e)
        {
            CheckBox checkedItem = (CheckBox)sender;
            if(isRFP)
            {
                WrapPanel checkedItemContent = (WrapPanel)checkedItem.Content;
                if (checkedItemContent.Children.Count > 2)
                {
                    TextBox toBeReleasedQuantity = (TextBox)checkedItemContent.Children[6];
                    toBeReleasedQuantity.IsEnabled = false;
                    selectedItems.Remove(checkedItem);
                }
                else
                {
                    checkedItemsCounter--;
                    //checkedItemsCounterLabel.Content = checkedItemsCounter;
                    TextBlock checkBoxContent = (TextBlock)rfpItemCheckBoxHolder.Content;
                    string[] splitedContent = checkBoxContent.Text.Split('\n');
                    splitedContent[2] = "To Be Released Quantity: " + checkedItemsCounter;
                    checkBoxContent.Text = String.Join("\n", splitedContent);
                    selectedItems.Remove(checkedItem);
                }
            }
         
        }

        private void OnCheckEntryPermitItem(object sender, RoutedEventArgs e)
        {
            //CheckBox entryPermitItemCheckBox = (CheckBox) sender;
            // WrapPanel entryCheckBoxContent = entryPermitItemCheckBox.Content as WrapPanel;
            // TextBox reservedQuantity = entryCheckBoxContent.Children[4] as TextBox;
            // reservedQuantity.IsEnabled = true;
           if(viewAddCondition!=COMPANY_WORK_MACROS.VIEW_RELEASE)

            {
                CheckBox checkedItem = (CheckBox)sender;
                if (isRFP)
                {
                    WrapPanel checkedItemContent = (WrapPanel)checkedItem.Content;
                    if (checkedItem.Content != null)
                        if (checkedItemContent.Children.Count > 2)
                        {
                            TextBox toBeReleasedQuantity = (TextBox)checkedItemContent.Children[6];
                            toBeReleasedQuantity.IsEnabled = true;
                            selectedItems.Add(checkedItem);
                        }
                        else
                        {
                            checkedItemsCounter++;
                            //checkedItemsCounterLabel.Content = checkedItemsCounter;
                            TextBlock checkBoxContent = (TextBlock)rfpItemCheckBoxHolder.Content;
                            string[] splitedContent = checkBoxContent.Text.Split('\n');
                            splitedContent[2] = "To Be Released Quantity: " + checkedItemsCounter;
                            checkBoxContent.Text = String.Join("\n", splitedContent);
                            selectedItems.Add(checkedItem);
                        }
                }
                else
                {
                    WrapPanel checkedItemContent = (WrapPanel)checkedItem.Content;
                    if (checkedItem.Content != null)
                        if (checkedItemContent.Children.Count > 2)
                        {
                            TextBox toBeReleasedQuantity = (TextBox)checkedItemContent.Children[6];
                            toBeReleasedQuantity.IsEnabled = true;
                            selectedItems.Add(checkedItem);
                        }
                        else
                        {
                            checkedItemsCounter++;
                            //checkedItemsCounterLabel.Content = checkedItemsCounter;
                            TextBlock checkBoxContent = (TextBlock)orderItemHolder.Content;
                            string[] splitedContent = checkBoxContent.Text.Split('\n');
                            splitedContent[2] = "To Be Released Quantity: " + checkedItemsCounter;
                            checkBoxContent.Text = String.Join("\n", splitedContent);
                            selectedItems.Add(checkedItem);
                        }
                }

            }


        }

        private void OnSelChangedSpecsComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox specsComboBox = sender as ComboBox;

            WrapPanel specsPanel = specsComboBox.Parent as WrapPanel;
            StackPanel home = specsPanel.Parent as StackPanel;

            WrapPanel choicePanel = home.Children[0] as WrapPanel;
            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            WrapPanel companyBrandPanel = home.Children[3] as WrapPanel;
            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyProductPanel = home.Children[2] as WrapPanel;
            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;

            WrapPanel companyCategoryPanel = home.Children[1] as WrapPanel;
            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;

            WrapPanel modelPanel = home.Children[4] as WrapPanel;
            ComboBox modelComboBox = modelPanel.Children[1] as ComboBox;

            if(specsComboBox.SelectedIndex !=-1)
            {
                ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem brandItem = companyBrandComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem modelItem = modelComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem specsItem = specsComboBox.SelectedItem as ComboBoxItem;

                PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT orderItem = workOrder.GetOrderProductsList().ToList().Find(f => f.product_category.category_id == Int32.Parse(categoryItem.Tag.ToString()) &&
                                                                                                                    f.productType.type_id == Int32.Parse(productItem.Tag.ToString()) &&
                                                                                                                    f.productBrand.brand_id == Int32.Parse(brandItem.Tag.ToString()) &&
                                                                                                                    f.productModel.model_id == Int32.Parse(modelItem.Tag.ToString()) &&
                                                                                                                    f.productSpec.spec_id == Int32.Parse(specsItem.Tag.ToString()));
                orderItemNumber = orderItem.productNumber;
            }
        }

        private void FilterItems() {


            Grid items = Home.Children[Home.Children.Count - 1] as Grid;
            items.Background = Brushes.White;

            items.Children.Clear();
            items.RowDefinitions.Clear();

          

            Grid Itemsheader = new Grid();

            Itemsheader.ShowGridLines = true;
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
            Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());


            Label entryPermitSerialId = new Label();

            entryPermitSerialId.Style = (Style)FindResource("tableHeaderItem");

            entryPermitSerialId.Content = "SERIAL ID";

            Grid.SetColumn(entryPermitSerialId, 0);

            Itemsheader.Children.Add(entryPermitSerialId);



            Label itemNameLabel = new Label();

            itemNameLabel.Style = (Style)FindResource("tableHeaderItem");

            itemNameLabel.Content = "ITEM NAME";

            Grid.SetColumn(itemNameLabel, 1);

            Itemsheader.Children.Add(itemNameLabel);


            Label serialNumber = new Label();

            serialNumber.Style = (Style)FindResource("tableHeaderItem");

            serialNumber.Content = "SERIAL NUMBER";

            Grid.SetColumn(serialNumber, 2);

            Itemsheader.Children.Add(serialNumber);





            Label quantity = new Label();

            quantity.Style = (Style)FindResource("tableHeaderItem");

            quantity.Content = "Quantity";


            Grid.SetColumn(quantity, 3);

            Itemsheader.Children.Add(quantity);




            items.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(Itemsheader, items.RowDefinitions.Count - 1);
            items.Children.Add(Itemsheader);


            ScrollViewer scroll = new ScrollViewer();
            scroll.CanContentScroll = true;
            scroll.Height = 600;
            Grid itemsBody = new Grid();

            itemsBody.ShowGridLines = true;

            scroll.Content = itemsBody;

            items.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(scroll, items.RowDefinitions.Count - 1);
            items.Children.Add(scroll);



            WrapPanel choicePanel = Home.Children[0] as WrapPanel;

            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;



            WrapPanel companyCategoryPanel = Home.Children[1] as WrapPanel;
            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;

            WrapPanel companyProductPanel = Home.Children[2] as WrapPanel;
            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;

            WrapPanel companyBrandPanel = Home.Children[3] as WrapPanel;
            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyModelPanel = Home.Children[4] as WrapPanel;
            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            WrapPanel companySpecsPanel = Home.Children[5] as WrapPanel;
            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;


            if (choiceComboBox.SelectedIndex == 0) {

                int previousSerial = 0;

                itemsCounter=1;

                for (int i = 0; i < entryPermits.Count; i++)
                {
                    if (previousSerial == entryPermits[i].entry_permit_serial)
                        continue;

                    previousSerial = entryPermits[i].entry_permit_serial;


                    materialEntry.SetEntryPermitSerialid(entryPermits[i].entry_permit_serial);
                    materialEntry.InitializeMaterialEntryPermit();



                    for (int j = 0; j < materialEntry.GetItems().Count; j++) {

                        
                        if (companyCategoryComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                            if (Int32.Parse(categoryItem.Tag.ToString()) != materialEntry.GetItems()[j].product_category.category_id)
                                continue;
                        }


                        if (companyProductComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;

                            if (Int32.Parse(productItem.Tag.ToString()) != materialEntry.GetItems()[j].product_type.type_id)
                                continue;
                        }


                        if (companyBrandComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem brandItem = companyBrandComboBox.SelectedItem as ComboBoxItem;

                            if (Int32.Parse(brandItem.Tag.ToString()) != materialEntry.GetItems()[j].product_brand.brand_id)
                                continue;
                        }


                        if (companyModelComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem modelItem = companyModelComboBox.SelectedItem as ComboBoxItem;
                            if (Int32.Parse(modelItem.Tag.ToString()) != materialEntry.GetItems()[j].product_model.model_id)
                                continue;
                        }


                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(60)});


                        Grid itemm = new Grid();

                        itemm.ShowGridLines = true;

                        itemm.Tag = entryPermits[i].entry_permit_serial.ToString()+" "+materialEntry.GetItems()[j].entry_permit_item_serial+" "+itemsCounter;

                        itemsCounter++;

                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());




                        Label entryPermitSerialLabel = new Label();


                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();

                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));



                        Grid.SetColumn(entryPermitSerialLabel, 0);

                        itemm.Children.Add(entryPermitSerialLabel);


                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;



                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;

                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;

                        ItemName.VerticalAlignment = VerticalAlignment.Top;


                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");

                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));


                        ItemName.Text = $"{materialEntry.GetItems()[j].product_category.category_name + "-" + materialEntry.GetItems()[j].product_type.product_name + "-" + materialEntry.GetItems()[j].product_brand.brand_name + "-" + materialEntry.GetItems()[j].product_model.model_name}";


                        Grid.SetColumn(ItemName, 1);

                        itemm.Children.Add(ItemName);


                        if (materialEntry.GetItems()[j].product_serial_number != "") {

                           
                            CheckBox chooseItemSerialCheckBox = new CheckBox();

                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;

                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                            chooseItemSerialCheckBox.Checked += ChooseItemSerialCheckBoxChecked;

                            chooseItemSerialCheckBox.Unchecked += ChooseItemSerialCheckBoxUnchecked;



                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");


                            Grid.SetColumn(chooseItemSerialCheckBox, 2);

                            itemm.Children.Add(chooseItemSerialCheckBox);



                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "") {



                            Grid quantityGrid = new Grid();

                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                            TextBox quantityAvailableTextBox = new TextBox();

                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity- materialEntry.GetItems()[j].released_quantity;


                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;


                            TextBox quantityTextBox = new TextBox();

                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;


                            CheckBox quantityCheckBox = new CheckBox();

                            quantityCheckBox.Width = 30;

                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");


                            quantityCheckBox.Checked += QuantityCheckBoxChecked;

                            quantityCheckBox.Unchecked += QuantityCheckBoxUnchecked;

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;

                            Grid.SetColumn(quantityCheckBox, 0);

                            Grid.SetColumn(quantityAvailableTextBox, 1);

                            Grid.SetColumn(quantityTextBox, 2);


                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);






                            Grid.SetColumn(quantityGrid, 3);

                            itemm.Children.Add(quantityGrid);




                        }



                        Grid.SetRow(itemm, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemm);

                    }

                 

                }


            }

            else if (choiceComboBox.SelectedIndex == 1) {
                int previousSerial = 0;

                itemsCounter = 1;

                for (int i=0;i<entryPermits.Count;i++)
                {
                    if (previousSerial == entryPermits[i].entry_permit_serial)
                        continue;

                    previousSerial = entryPermits[i].entry_permit_serial;


                    materialEntry.SetEntryPermitSerialid(entryPermits[i].entry_permit_serial);
                    materialEntry.InitializeMaterialEntryPermit();




                    for (int j = 0; j < materialEntry.GetItems().Count; j++)
                    {



                        if (companyCategoryComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                            if (Int32.Parse(categoryItem.Tag.ToString()) != materialEntry.GetItems()[j].product_category.category_id)
                                continue;
                        }


                        if (companyProductComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;

                            if (Int32.Parse(productItem.Tag.ToString()) != materialEntry.GetItems()[j].product_type.type_id)
                                continue;
                        }


                        if (companyBrandComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem brandItem = companyBrandComboBox.SelectedItem as ComboBoxItem;

                            if (Int32.Parse(brandItem.Tag.ToString()) != materialEntry.GetItems()[j].product_brand.brand_id)
                                continue;
                        }


                        if (companyModelComboBox.SelectedIndex != -1)
                        {
                            ComboBoxItem modelItem = companyModelComboBox.SelectedItem as ComboBoxItem;
                            if (Int32.Parse(modelItem.Tag.ToString()) != materialEntry.GetItems()[j].product_model.model_id)
                                continue;
                        }


                        if (materialEntry.GetItems()[j].is_released == true)
                            continue;

                        itemsBody.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60)});


                        Grid itemm = new Grid();

                        itemm.ShowGridLines = true;

                        itemm.Tag = entryPermits[i].entry_permit_serial.ToString()+ " " +materialEntry.GetItems()[j].entry_permit_item_serial+" "+ itemsCounter;


                        itemsCounter++;

                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());
                        itemm.ColumnDefinitions.Add(new ColumnDefinition());



                        Label entryPermitSerialLabel = new Label();


                        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

                        entryPermitSerialLabel.Content = materialEntry.GetEntryPermitId();


                        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));



                        Grid.SetColumn(entryPermitSerialLabel, 0);

                        itemm.Children.Add(entryPermitSerialLabel);


                        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;


                        TextBlock ItemName = new TextBlock();
                        ItemName.TextWrapping = TextWrapping.Wrap;
                        ItemName.HorizontalAlignment = HorizontalAlignment.Left;


                        ItemName.Style = (Style)FindResource("cardTextBlockStyle");

                        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                        ItemName.Text = $"{materialEntry.GetItems()[j].product_type.product_name + "-" + materialEntry.GetItems()[j].product_brand.brand_name + "-" + materialEntry.GetItems()[j].product_model.model_name}";


                        Grid.SetColumn(ItemName, 1);

                        itemm.Children.Add(ItemName);



                        if (materialEntry.GetItems()[j].product_serial_number != "")
                        {


                            CheckBox chooseItemSerialCheckBox = new CheckBox();

                            chooseItemSerialCheckBox.Content = materialEntry.GetItems()[j].product_serial_number;



                            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");

                            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;

                            chooseItemSerialCheckBox.Checked += ChooseItemSerialCheckBoxChecked;

                            chooseItemSerialCheckBox.Unchecked += ChooseItemSerialCheckBoxUnchecked;



                            Grid.SetColumn(chooseItemSerialCheckBox, 2);

                            itemm.Children.Add(chooseItemSerialCheckBox);



                        }

                        if (materialEntry.GetItems()[j].product_serial_number == "")
                        {

                            Grid quantityGrid = new Grid();
                            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;

                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());



                            TextBox quantityAvailableTextBox = new TextBox();

                            quantityAvailableTextBox.Tag = materialEntry.GetItems()[j].quantity- materialEntry.GetItems()[j].released_quantity;


                            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityAvailableTextBox.Text = $"{materialEntry.GetItems()[j].quantity - materialEntry.GetItems()[j].released_quantity}";
                            quantityAvailableTextBox.IsEnabled = false;


                            TextBox quantityTextBox = new TextBox();

                            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");

                            quantityTextBox.TextChanged += QuantityTextBoxTextChanged;


                            CheckBox quantityCheckBox = new CheckBox();

                            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");

                            quantityCheckBox.Width = 30;

                            quantityCheckBox.Checked += QuantityCheckBoxChecked;

                            quantityCheckBox.Unchecked += QuantityCheckBoxUnchecked;

                                


                            Grid.SetColumn(quantityCheckBox, 0);

                            Grid.SetColumn(quantityAvailableTextBox, 1);

                            Grid.SetColumn(quantityTextBox, 2);


                            quantityGrid.Children.Add(quantityCheckBox);
                            quantityGrid.Children.Add(quantityAvailableTextBox);
                            quantityGrid.Children.Add(quantityTextBox);



                            Grid.SetColumn(quantityGrid, 3);

                            itemm.Children.Add(quantityGrid);



                        }



                        Grid.SetRow(itemm, itemsBody.RowDefinitions.Count - 1);
                        itemsBody.Children.Add(itemm);



                    }



                }

            }



            checkedItemsWrapPanel.Children.Clear();
        }


        /// ////////////////////////////////////////////       
        //ON CHECKS FUNCTIONS
        /// ////////////////////////////////////////////       
        private void QuantityCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems--;


            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox = sender as CheckBox;

           Grid quantityGrid= chooseItemCheckBox.Parent as Grid;

           TextBox quantityTextBox= quantityGrid.Children[2] as TextBox;

            quantityTextBox.Text = "";

            Grid card = chooseItemCheckBox.Tag as Grid;

            WrapPanel locationsWrapPanel = card.Parent as WrapPanel;
            locationsWrapPanel.Children.Remove(card);


            WrapPanel checkBoxesPanel=  card.Children[1] as WrapPanel;

            CheckBox rfpCheckBox=  checkBoxesPanel.Children[0] as CheckBox;

            CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;

           Grid orderOrRfp= card.Children[2] as Grid;

            if (orderCheckBox.IsChecked == true)
            {

                ComboBox orderItems = orderOrRfp.Children[0] as ComboBox;

                if (orderItems.SelectedIndex != -1)
                {

                    if (quantityTextBox.Text != "")
                        addReleasePermitPage.serialProducts[orderItems.SelectedIndex] -= int.Parse(quantityTextBox.Text);
                }
            }


            else
            {

                ComboBox rfpItems = orderOrRfp.Children[0] as ComboBox;

                if (rfpItems.SelectedIndex != -1)
                {

                    if (quantityTextBox.Text != "")
                        addReleasePermitPage.serialProducts[rfpItems.SelectedIndex] -= int.Parse(quantityTextBox.Text);
                }

            }


        }

        private void QuantityCheckBoxChecked(object sender, RoutedEventArgs e)
        {

            numberOfSelectedItems++;

            CheckBox quantityCheckBox = sender as CheckBox;


            Grid quantityGrid = quantityCheckBox.Parent as Grid;

            Grid item= quantityGrid.Parent as Grid;



            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();

            TextBlock itemName = item.Children[1] as TextBlock;


            Grid card = new Grid() { Margin = new Thickness(15, 30, 15, 30) };
            card.Height = 500;

            card.Background = Brushes.White;


            card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());


            Grid header = new Grid() { };
            header.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            TextBlock itemHeader = new TextBlock();

            header.Children.Add(itemHeader);

            itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
            itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            itemHeader.FontSize = 16;

            itemHeader.Text = itemName.Text + " ," + quantityCheckBox.Content;

            itemHeader.Foreground = Brushes.White;

            itemHeader.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(header, 0);


            quantityCheckBox.Tag = card;



            WrapPanel checkBoxesPanel = new WrapPanel();

            CheckBox rfpCheckBox = new CheckBox();

            rfpCheckBox.Style = (Style)FindResource("checkBoxStyle");

            rfpCheckBox.Content = "RFP";

            rfpCheckBox.Checked += RfpCheckBoxChecked;


            rfpCheckBox.HorizontalAlignment = HorizontalAlignment.Left;


            rfpCheckBox.Tag = item;



            CheckBox orderCheckBox = new CheckBox();

            orderCheckBox.Style = (Style)FindResource("checkBoxStyle");

            orderCheckBox.Content = "WorkOrder";
            orderCheckBox.Checked += OrderCheckBoxChecked;

            orderCheckBox.HorizontalAlignment = HorizontalAlignment.Right;

            orderCheckBox.Tag = item;



            checkBoxesPanel.Children.Add(rfpCheckBox);

            checkBoxesPanel.Children.Add(orderCheckBox);


            Grid.SetRow(checkBoxesPanel, 1);
            checkBoxesPanel.Visibility = Visibility.Collapsed;


            Grid rfpOrOrder = new Grid() { Margin = new Thickness(0, -40, 0, 0) };


            Grid.SetRow(rfpOrOrder, 2);


            card.Children.Add(header);
            card.Children.Add(checkBoxesPanel);
            card.Children.Add(rfpOrOrder);

            checkedItemsWrapPanel.Children.Add(card);


            if (addReleasePermitPage.workFormComboBox.SelectedIndex == 0)
            {

                rfpCheckBox.IsChecked = true;
            }

            else if (addReleasePermitPage.workFormComboBox.SelectedIndex ==1)
            {

                orderCheckBox.IsChecked = true;

            }


        }

        private void ChooseItemSerialCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

            numberOfSelectedItems--;


            WrapPanel selectedItemsPanel = Home.Children[Home.Children.Count - 2] as WrapPanel;

            Label selectedItemsLabel = selectedItemsPanel.Children[1] as Label;

            selectedItemsLabel.Content = numberOfSelectedItems.ToString();
            CheckBox chooseItemCheckBox=sender as CheckBox;

            Grid card= chooseItemCheckBox.Tag as Grid;


            Grid rfpOrOrder= card.Children[2] as Grid;

            ComboBox items=  rfpOrOrder.Children[0] as ComboBox;

            if(items.SelectedIndex!=-1)
            parentWindow.releasePermitPage.serialProducts[items.SelectedIndex]--;

           WrapPanel locationsWrapPanel= card.Parent as WrapPanel;
            locationsWrapPanel.Children.Remove(card);
        }

        private void ChooseItemSerialCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            numberOfSelectedItems++;

             CheckBox itemSerialCheckBox =sender as CheckBox;


             Grid item= itemSerialCheckBox.Parent as Grid;



             WrapPanel selectedItemsPanel=Home.Children[Home.Children.Count - 2] as WrapPanel;

             Label selectedItemsLabel=selectedItemsPanel.Children[1] as Label;

             selectedItemsLabel.Content = numberOfSelectedItems.ToString();

             TextBlock itemName=  item.Children[1] as TextBlock;


            Grid card = new Grid() { Margin=new Thickness(15,30,15,30)};
            card.Height = 500;

            card.Background = Brushes.White;


            card.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(60)});
            card.RowDefinitions.Add(new RowDefinition());
            card.RowDefinitions.Add(new RowDefinition());


            Grid header = new Grid() {};
            header.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            TextBlock itemHeader = new TextBlock();

            header.Children.Add(itemHeader);

            itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
            itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

            itemHeader.FontSize = 16;

            itemHeader.Text = itemName.Text+" ,"+ itemSerialCheckBox.Content;

            itemHeader.Foreground = Brushes.White;

            itemHeader.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(header, 0);

          
            itemSerialCheckBox.Tag=card;



            WrapPanel checkBoxesPanel = new WrapPanel();

            CheckBox rfpCheckBox = new CheckBox();

            rfpCheckBox.Style = (Style)FindResource("checkBoxStyle");

            rfpCheckBox.Content = "RFP";

            rfpCheckBox.Checked += RfpCheckBoxChecked;


            rfpCheckBox.HorizontalAlignment = HorizontalAlignment.Left;



            CheckBox orderCheckBox = new CheckBox();

            orderCheckBox.Style = (Style)FindResource("checkBoxStyle");

            orderCheckBox.Content = "WorkOrder";
            orderCheckBox.Checked += OrderCheckBoxChecked;

            orderCheckBox.HorizontalAlignment = HorizontalAlignment.Right;



            checkBoxesPanel.Children.Add(rfpCheckBox);

            checkBoxesPanel.Children.Add(orderCheckBox);


            checkBoxesPanel.Visibility = Visibility.Collapsed;


            Grid.SetRow(checkBoxesPanel, 1);


            Grid rfpOrOrder = new Grid() { Margin=new Thickness(0,-40,0,0)};


            Grid.SetRow(rfpOrOrder, 2);


            card.Children.Add(header);
            card.Children.Add(checkBoxesPanel);
            card.Children.Add(rfpOrOrder);

            checkedItemsWrapPanel.Children.Add(card);


            if (rfp.GetRFPSerial()!=0) {

                rfpCheckBox.IsChecked = true;
            }

            else if (workOrder.GetOrderSerial()!=0) {

                orderCheckBox.IsChecked = true;
            
            }

        }

        private void RfpCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox rfpCheckBox = sender as CheckBox;


            WrapPanel checkPanel = rfpCheckBox.Parent as WrapPanel;


            CheckBox orderCheckBox=  checkPanel.Children[1] as CheckBox;

            orderCheckBox.IsChecked = false;

            Grid card = checkPanel.Parent as Grid;


            Grid order = card.Children[2] as Grid;

            order.Children.Clear();
            order.RowDefinitions.Clear();


            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());




            ComboBox rfpsItemsComboBox = new ComboBox();

            rfpsItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
            rfpsItemsComboBox.IsEnabled = true;

            rfpsItemsComboBox.SelectionChanged += RfpsItemsComboBoxSelectionChanged;



            ComboBox locations = new ComboBox();
            locations.Style = (Style)FindResource("comboBoxStyle");
            locations.IsEnabled = true;
            locations.Visibility = Visibility.Visible;





            TextBox projectName = new TextBox();
            projectName.Style = (Style)FindResource("filterTextBoxStyle");
            projectName.IsReadOnly = true;
            projectName.Visibility = Visibility.Visible;

            projectName.Width = 200;




            TextBox identityTextBox = new TextBox();
            identityTextBox.Style = (Style)FindResource("filterTextBoxStyle");
            identityTextBox.IsReadOnly = true;
            identityTextBox.Visibility = Visibility.Visible;



            Grid textBoxesGrid = new Grid();

            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());


            Grid.SetColumn(identityTextBox, 0);
            Grid.SetColumn(projectName, 1);

            textBoxesGrid.Children.Add(identityTextBox);
            textBoxesGrid.Children.Add(projectName);



            Grid.SetRow(rfpsItemsComboBox, 0);

            Grid.SetRow(locations, 1);

            Grid.SetRow(textBoxesGrid, 2);




            order.Children.Add(rfpsItemsComboBox);

            order.Children.Add(locations);

            order.Children.Add(textBoxesGrid);


            ComboBox rfpSerialsComboBox= addReleasePermitPage.rfpPanel.Children[1] as ComboBox;

            if (addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
            {

                WorkOrder workOrder = new WorkOrder();
                workOrder.InitializeWorkOrderInfo(addReleasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                workOrdersLocations.Clear();
                workOrder.GetProjectLocations(ref workOrdersLocations);


                workOrdersLocations.ForEach(a => locations.Items.Add(a.site_location.district.district_name + " ," + a.site_location.city.city_name + " ," + a.site_location.state_governorate.state_name + " ," + a.site_location.country.country_name));

                identityTextBox.Text = workOrder.GetOrderID();

                projectName.Text = workOrder.GetprojectName();

            }
            else if (addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
            {
                MaintenanceContract maintenanceContract = new MaintenanceContract();
                maintenanceContract.InitializeMaintenanceContractInfo(addReleasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), addReleasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                maintenanceContractLocations.ForEach(a => locations.Items.Add(a.site_location.district.district_name + " ," + a.site_location.city.city_name + " ," + a.site_location.state_governorate.state_name + " ," + a.site_location.country.country_name));

                identityTextBox.Text = maintenanceContract.GetMaintContractID();

                projectName.Text = maintenanceContract.GetMaintContractProjectName();


            }
            else if (addReleasePermitPage.rfp.GetProjectSerial() != 0)
            {

                commonQueries.GetCompanyProjectLocations(addReleasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                companyProjectLocations.ForEach(a => locations.Items.Add(a.site_location.district.district_name + " ," + a.site_location.city.city_name + " ," + a.site_location.state_governorate.state_name + " ," + a.site_location.country.country_name));

                identityTextBox.Text = "PROJECT";

                projectName.Text = addReleasePermitPage.rfp.GetProjectName();



            }
            else
            {

                identityTextBox.Text = "PROJECT";

                projectName.Text = "01ELECTRONICS";
            }

            rfpsItemsComboBox.Items.Clear();

            for (int i = 0; i < addReleasePermitPage.rfpItems.Count; i++)
            {
                //String itemName = rfpItemsDescription[i].item_description;
                String itemName = String.Empty;
                if (addReleasePermitPage.rfpItems[i].product_model.model_name != "" &&addReleasePermitPage.rfpItems[i].product_model.model_name != null)
                {
                    itemName = addReleasePermitPage.rfpItems[i].product_category.category_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_type.product_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_brand.brand_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_model.model_name;
                }
                else if (addReleasePermitPage.rfpItems[i].product_model.model_name != "" && addReleasePermitPage.rfpItems[i].product_model.model_name != null)
                {
                    itemName = addReleasePermitPage.rfpItems[i].product_category.category_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_type.product_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_brand.brand_name + " - " +
                               addReleasePermitPage.rfpItems[i].product_model.model_name;
                }
                if (itemName != string.Empty)
                    rfpsItemsComboBox.Items.Add(itemName);
            }


        }

        private void RfpsItemsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox rfpItemsComboBox=sender as ComboBox;

            if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.model_name != "")
            {
                if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == true)
                {
                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Insufficient rfp item quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }

                    else {

                        addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;


                    }

                }
                else {

                    Grid rfpCard = rfpItemsComboBox.Parent as Grid;

                    Grid card = rfpCard.Parent as Grid;

                    WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                    CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                    Grid item = rfpCheckBox.Tag as Grid;

                    Grid quantityGrid = item.Children[2] as Grid;

                    if (quantityGrid != null)
                    {

                        TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                        if (availableQuantityTextBox.Text != "")
                            addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                    }

                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Insufficient rfp item quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }


                }

            }
            else
            {

                if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].product_model.has_serial_number == true)
                {
                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("Insufficient rfp item quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }
                    else
                    {

                        addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex]++;


                    }



                }
                else
                {

                    Grid rfpCard = rfpItemsComboBox.Parent as Grid;

                    Grid card = rfpCard.Parent as Grid;

                    WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                    CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                    Grid item = rfpCheckBox.Tag as Grid;

                    Grid quantityGrid = item.Children[2] as Grid;

                    if (quantityGrid != null)
                    {

                        TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                        if (availableQuantityTextBox.Text != "")
                            addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                    }

                    if (addReleasePermitPage.rfpItems[rfpItemsComboBox.SelectedIndex].item_quantity < addReleasePermitPage.serialProducts[rfpItemsComboBox.SelectedIndex])
                    {
                        System.Windows.Forms.MessageBox.Show("RfpItemQuantity are not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        return;

                    }


                }
            }

        }

        private void OrderCheckBoxChecked(object sender, RoutedEventArgs e)
        {

            CheckBox orderCheckBox = sender as CheckBox;


            WrapPanel checkPanel = orderCheckBox.Parent as WrapPanel;

            Grid card = checkPanel.Parent as Grid;


            CheckBox rfpCheckBox = checkPanel.Children[0] as CheckBox;

            rfpCheckBox.IsChecked = false;


            Grid order = card.Children[2] as Grid;

            order.Children.Clear();
            order.RowDefinitions.Clear();


            order.RowDefinitions.Add(new RowDefinition());
            order.RowDefinitions.Add(new RowDefinition());


            ComboBox orderItemsComboBox = new ComboBox();
            orderItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
            orderItemsComboBox.IsEnabled = true;

            orderItemsComboBox.SelectionChanged += OnOrderItemsComboBoxSelectionChanged;


            ComboBox orderLocationsComboBox = new ComboBox();
            orderLocationsComboBox.Style = (Style)FindResource("comboBoxStyle");
            orderLocationsComboBox.IsEnabled = true;

         
            Grid.SetRow(orderItemsComboBox, 0);

            Grid.SetRow(orderLocationsComboBox, 1);



            order.Children.Add(orderItemsComboBox);

            order.Children.Add(orderLocationsComboBox);



            workOrdersLocations.Clear();
            parentWindow.releasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);


            workOrdersLocations.ForEach(a => orderLocationsComboBox.Items.Add(a.site_location.district.district_name + " ," + a.site_location.city.city_name + " ," + a.site_location.state_governorate.state_name + " ," + a.site_location.country.country_name));


            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = parentWindow.releasePermitPage.workOrder.GetOrderProductsList();

            orderItemsComboBox.Items.Clear();

            for (int i = 0; i < workOrderProducts.Length; i++)
            {
                if(workOrderProducts[i].product_category.category_name!=null)
                   orderItemsComboBox.Items.Add(workOrderProducts[i].product_category.category_name + " ," + workOrderProducts[i].productType.product_name + " ," + workOrderProducts[i].productBrand.brand_name + " ," + workOrderProducts[i].productModel.model_name);
            }

        }

        private void OnOrderItemsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox orderItemsComboBox= sender as ComboBox;

            if (parentWindow.releasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].has_serial_number == true)
            {
                parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex]++;


                if (parentWindow.releasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                {
                     System.Windows.Forms.MessageBox.Show("Insufficient order item quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }
            else {


                Grid orderCard = orderItemsComboBox.Parent as Grid;

                Grid card = orderCard.Parent as Grid;

                WrapPanel checkBoxesPanel = card.Children[1] as WrapPanel;

                CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;

                Grid item = orderCheckBox.Tag as Grid;

                Grid quantityGrid = item.Children[2] as Grid;

                if (quantityGrid != null)
                {

                    TextBox availableQuantityTextBox = quantityGrid.Children[2] as TextBox;
                    if (availableQuantityTextBox.Text != "")
                        parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex] += int.Parse(availableQuantityTextBox.Text);
                }

                if (parentWindow.releasePermitPage.workOrder.GetOrderProductsList()[orderItemsComboBox.SelectedIndex].productQuantity < parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                {
                    System.Windows.Forms.MessageBox.Show("Insufficient order item quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    return;

                }

               

            }


        }


        /// ////////////////////////////////////////////       
        //ON SELECTION CHANGED FUNCTIONS
        /// ////////////////////////////////////////////       
        private void OnOrdersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox ordersComboBox= sender as ComboBox;

            if (ordersComboBox.SelectedIndex == -1)
                return;


            WorkOrder workOrder = new WorkOrder();


            Grid order = ordersComboBox.Parent as Grid;

            ComboBox orderItemsComboBox = order.Children[1] as ComboBox;
            orderItemsComboBox.IsEnabled = true;


            ComboBox orderLocationComboBox = order.Children[2] as ComboBox;

            orderLocationComboBox.Items.Clear();

            workOrder.InitializeWorkOrderInfo(workOrders[ordersComboBox.SelectedIndex].order_serial);

            workOrdersLocations.Clear();
            workOrder.GetProjectLocations(ref workOrdersLocations);


            workOrdersLocations.ForEach(a => orderLocationComboBox.Items.Add(a.site_location.district.district_name + " ," + a.site_location.city.city_name + " ," + a.site_location.state_governorate.state_name + " ," + a.site_location.country.country_name));

            orderLocationComboBox.IsEnabled = true;

            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[]workOrderProducts=workOrder.GetOrderProductsList();

            orderItemsComboBox.Items.Clear();

            for (int i = 0; i < workOrderProducts.Length; i++) {

                orderItemsComboBox.Items.Add(workOrderProducts[i].product_category.category_name + " ," + workOrderProducts[i].productType.product_name + " ," + workOrderProducts[i].productBrand.brand_name + " ," + workOrderProducts[i].productModel.model_name);
            }

        }

        private void OnCompanyModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyModelComboBox= sender as ComboBox;


            WrapPanel companyModelPanel=companyModelComboBox.Parent as WrapPanel;
            StackPanel home= companyModelPanel.Parent as StackPanel;

            WrapPanel choicePanel = home.Children[0] as WrapPanel;
            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            WrapPanel companyBrandPanel=home.Children[3] as WrapPanel;
            ComboBox companyBrandComboBox= companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyProductPanel = home.Children[2] as WrapPanel;
            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;

            WrapPanel companyCategoryPanel = home.Children[1] as WrapPanel;
            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;

            WrapPanel companySpecsPanel = home.Children[5] as WrapPanel;
            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;

            if(companyModelComboBox.SelectedIndex !=-1)
            {
                ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem brandItem = companyBrandComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem modelItem = companyModelComboBox.SelectedItem as ComboBoxItem;

                if (choiceComboBox.SelectedIndex ==1)
                {
                    if (workOrder.GetOrderSerial() != 0)
                    {
                        for (int i = 0; i < workOrder.GetOrderProductsList().Count(); i++)
                        {
                            if (Int32.Parse(categoryItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].product_category.category_id && Int32.Parse(productItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productType.type_id && Int32.Parse(brandItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productBrand.brand_id && Int32.Parse(modelItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productModel.model_id)
                            {
                                if (companySpecsComboBox.Items.Cast<ComboBoxItem>().Any(f => Int32.Parse(f.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productSpec.spec_id) == false)
                                {
                                    ComboBoxItem specItem = new ComboBoxItem();
                                    specItem.Content = workOrder.GetOrderProductsList()[i].productSpec.spec_name;
                                    specItem.Tag = workOrder.GetOrderProductsList()[i].productSpec.spec_id;
                                    companySpecsComboBox.Items.Add(specItem);
                                }

                            }
                        }
                    }
                    else
                    {

                    }

                }
                FilterItems();
            }
           
        

        }

        private void OnCompanyBrandComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyBrandComboBox = sender as ComboBox;
            WrapPanel companyBrandPanel = companyBrandComboBox.Parent as WrapPanel;
            StackPanel home = companyBrandPanel.Parent as StackPanel;

            WrapPanel choicePanel = home.Children[0] as WrapPanel;
            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            WrapPanel companyCategoryPanel = home.Children[1] as WrapPanel;
            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;

            WrapPanel companyproductPanel = home.Children[2] as WrapPanel;
            ComboBox companyProductComboBox = companyproductPanel.Children[1] as ComboBox;

            WrapPanel companyModelPanel = home.Children[4] as WrapPanel;
            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            WrapPanel companySpecsPanel = home.Children[5] as WrapPanel;
            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;

            companyModelComboBox.Items.Clear();
            
            if(companyBrandComboBox.SelectedIndex != -1)
            {
                ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;    
                ComboBoxItem brandItem = companyBrandComboBox.SelectedItem as ComboBoxItem;


                if(choiceComboBox.SelectedIndex == 0)
                {
                 
                    genericModels.Clear();

                    if (!commonQueries.GetGenericBrandModels(Int32.Parse(productItem.Tag.ToString()), Int32.Parse(brandItem.Tag.ToString()),Int32.Parse(categoryItem.Tag.ToString()), ref genericModels))
                        return;


                    genericModels.ForEach(a => companyModelComboBox.Items.Add(a.model_name));


                  
                }
                else
                {
                    companyModels.Clear();
                    //if (!commonQueries.GetCompanyModels(companyProducts[companyProductComboBox.SelectedIndex], companyBrands[companyBrandComboBox.SelectedIndex], ref companyModels))
                    //    return;
                    //companyModels.ForEach(a => companyModelComboBox.Items.Add(a.model_name));
                    if (workOrder.GetOrderSerial() != 0)
                    {
                        for (int i = 0; i < workOrder.GetOrderProductsList().Count(); i++)
                        {
                            if (Int32.Parse(categoryItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].product_category.category_id && Int32.Parse(productItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productType.type_id && Int32.Parse(brandItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productBrand.brand_id)
                            {
                                if (companyModelComboBox.Items.Cast<ComboBoxItem>().Any(f => Int32.Parse(f.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productModel.model_id) == false)
                                {
                                    ComboBoxItem modelItem = new ComboBoxItem();
                                    modelItem.Content = workOrder.GetOrderProductsList()[i].productModel.model_name;
                                    modelItem.Tag = workOrder.GetOrderProductsList()[i].productModel.model_id;
                                    companyModelComboBox.Items.Add(modelItem);
                                }
                            }
                        }
                    }
                    else
                    {

                    }

                }

                FilterItems();
            }

           
        }

        private void OnCompanyProductComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox companyProductComboBox=sender as ComboBox;
            WrapPanel companyProductPanel = companyProductComboBox.Parent as WrapPanel;
            StackPanel home=companyProductPanel.Parent as StackPanel;

            WrapPanel choicePanel = home.Children[0] as WrapPanel;
            ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

            WrapPanel companyCategoryPanel=home.Children[1] as WrapPanel;
            ComboBox companyCategoryComboBox=companyCategoryPanel.Children[1] as ComboBox;

            WrapPanel companyBrandPanel = home.Children[3] as WrapPanel;
            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;

            WrapPanel companyModelPanel = home.Children[4] as WrapPanel;
            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;

            WrapPanel companyspecsPanel = home.Children[5] as WrapPanel;
            ComboBox companyspecsComboBox = companyspecsPanel.Children[1] as ComboBox;

            companyBrandComboBox.Items.Clear();
            companyModelComboBox.Items.Clear();
            companyspecsComboBox.Items.Clear();



            if(companyProductComboBox.SelectedIndex !=-1)
            {
                ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                ComboBoxItem productItem = companyProductComboBox.SelectedItem as ComboBoxItem;

                if(choiceComboBox.SelectedIndex ==0 )
                {
                    genericBrands.Clear();
                    if (!commonQueries.GetGenericProductBrands(Int32.Parse(productItem.Tag.ToString()), Int32.Parse(categoryItem.Tag.ToString()), ref genericBrands))
                        return;
                    genericBrands.ForEach(a => companyBrandComboBox.Items.Add(a.brand_name));
                }
                else
                {
                    companyBrands.Clear();
                    //if(!commonQueries.GetProductBrands(Int32.Parse(productItem.Tag.ToString()), ref companyBrands))
                    //    return;
                    //companyBrands.ForEach(a => companyBrandComboBox.Items.Add(a.brand_name));
                    if (workOrder.GetOrderSerial() != 0)
                    {
                        for (int i = 0; i < workOrder.GetOrderProductsList().Count(); i++)
                        {
                            if (Int32.Parse(categoryItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].product_category.category_id && Int32.Parse(productItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productType.type_id)
                            {
                                if (companyBrandComboBox.Items.Cast<ComboBoxItem>().Any(f => Int32.Parse(f.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productBrand.brand_id) == false)
                                {
                                    ComboBoxItem brandItem = new ComboBoxItem();
                                    brandItem.Content = workOrder.GetOrderProductsList()[i].productBrand.brand_name;
                                    brandItem.Tag = workOrder.GetOrderProductsList()[i].productBrand.brand_id;
                                    companyBrandComboBox.Items.Add(brandItem);
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
                FilterItems();
            }
          

        }

        private void OnCompanyCategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                ComboBox companyCategoryComboBox=sender as ComboBox;
                WrapPanel companyCategoryPanel=companyCategoryComboBox.Parent as WrapPanel;

                StackPanel home= companyCategoryPanel.Parent as StackPanel;

                WrapPanel choicePanel = home.Children[0] as WrapPanel;
                ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;

                WrapPanel companyProductPanel= home.Children[2] as WrapPanel;
                ComboBox companyProductComboBox=  companyProductPanel.Children[1] as ComboBox;


                WrapPanel companyBrandPanel = home.Children[3] as WrapPanel;
                ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;



                WrapPanel companyModelPanel = home.Children[4] as WrapPanel;
                ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;
            if(companyCategoryComboBox.SelectedIndex !=-1)
            {
                if (choiceComboBox.SelectedIndex == 1)
                {
                    WrapPanel specsPanel = home.Children[5] as WrapPanel;
                    ComboBox specsComboBox = specsPanel.Children[1] as ComboBox;
                    specsComboBox.Items.Clear();
                    companyProductComboBox.Items.Clear();
                    companyBrandComboBox.Items.Clear();
                    companyModelComboBox.Items.Clear();

                    companyProducts.Clear();
                    ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                    //if (!commonQueries.GetCompanyProducts(ref companyProducts, Int32.Parse(categoryItem.Tag.ToString())))
                    //    return;

                    //companyProductComboBox.Items.Clear();
                    //companyProducts.ForEach(a => companyProductComboBox.Items.Add(a.product_name));
                    if(workOrder.GetOrderSerial() !=0)
                    {
                        for(int i=0;i<workOrder.GetOrderProductsList().Count();i++)
                        {
                            if (Int32.Parse(categoryItem.Tag.ToString()) == workOrder.GetOrderProductsList()[i].product_category.category_id)
                            {
                                if(companyProductComboBox.Items.Cast<ComboBoxItem>().Any(f=>Int32.Parse(f.Tag.ToString()) == workOrder.GetOrderProductsList()[i].productType.type_id)==false)
                                {
                                    ComboBoxItem productItem = new ComboBoxItem();
                                    productItem.Content = workOrder.GetOrderProductsList()[i].productType.product_name;
                                    productItem.Tag = workOrder.GetOrderProductsList()[i].productType.type_id;
                                    companyProductComboBox.Items.Add(productItem);
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    companyProductComboBox.Items.Clear();
                    companyBrandComboBox.Items.Clear();
                    companyModelComboBox.Items.Clear();
                    genericProducts.Clear();
                    companyProductComboBox.Items.Clear();
                    ComboBoxItem categoryItem = companyCategoryComboBox.SelectedItem as ComboBoxItem;
                    if (!commonQueries.GetGenericProducts(ref genericProducts, Int32.Parse(categoryItem.Tag.ToString())))
                        return;
                       genericProducts.ForEach(a => companyProductComboBox.Items.Add(a.product_name));

                }
                FilterItems();
            }
            
          

           

        }

        private void OnGenericModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            FilterItems();
        }

        private void OnGenericBrandComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox genericBrandCombo = sender as ComboBox;
            WrapPanel genericBrandpanel = genericBrandCombo.Parent as WrapPanel;

            Grid Home = genericBrandpanel.Parent as Grid;

            WrapPanel generiModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = generiModelPanel.Children[1] as ComboBox;


            WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;



            WrapPanel genericProductPanel = Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;



            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;

            genericModelComboBox.Items.Clear();

            genericModels.Clear();

            if (genericBrandComboBox.SelectedIndex == -1)
                return;

            commonQueries.GetGenericBrandModels(genericProducts[genericProductComboBox.SelectedIndex].type_id, genericBrands[genericBrandComboBox.SelectedIndex].brand_id, genericCategories[genericCategoryComboBox.SelectedIndex].category_id, ref genericModels);


            genericModels.ForEach(a => genericModelComboBox.Items.Add(a.model_name));


            FilterItems();
        }

        private void OnGenericProductComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox genericProductCombo = sender as ComboBox;
            WrapPanel genericProductpanel = genericProductCombo.Parent as WrapPanel;

            Grid Home = genericProductpanel.Parent as Grid;

            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;



            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;



            WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;


            genericModelComboBox.Items.Clear();
            genericBrandComboBox.Items.Clear();


            if (genericProductCombo.SelectedIndex == -1)
                return;

                




            FilterItems();


        }

        private void OnGenericCategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox genericCategoryCombo=sender as ComboBox;
            WrapPanel genericCategorypanel= genericCategoryCombo.Parent as WrapPanel;

            Grid Home=genericCategorypanel.Parent as Grid;

            WrapPanel genericProductPanel=Home.Children[2] as WrapPanel;

            ComboBox genericProductComboBox=genericProductPanel.Children[1] as ComboBox;


            WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;

            genericProducts.Clear();

            genericProductComboBox.Items.Clear();
            genericBrandComboBox.Items.Clear();
            genericModelComboBox.Items.Clear();

            if (genericCategoryCombo.SelectedIndex == -1)
                return;

          





            FilterItems();


        }

        private void OnChoiceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox choiceComboBox = sender as ComboBox;
            WrapPanel choicePanel = choiceComboBox.Parent as WrapPanel;

            StackPanel Home = choicePanel.Parent as StackPanel;

            //WrapPanel genericCategoryPanel = Home.Children[1] as WrapPanel;

            //ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;

            //WrapPanel genericProductPanel = Home.Children[2] as WrapPanel;

            //ComboBox genericProductComboBox = genericProductPanel.Children[1] as ComboBox;


            //WrapPanel genericBrandPanel = Home.Children[3] as WrapPanel;

            //ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;


            //WrapPanel genericModelPanel = Home.Children[4] as WrapPanel;

            //ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;


            WrapPanel companyCategoryPanel = Home.Children[1] as WrapPanel;

            ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;


            WrapPanel companyProductPanel = Home.Children[2] as WrapPanel;

            ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;


            WrapPanel companyBrandPanel = Home.Children[3] as WrapPanel;

            ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;


            WrapPanel companyModelPanel = Home.Children[4] as WrapPanel;

            ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;


            WrapPanel companySpecsPanel = Home.Children[5] as WrapPanel;
            Label specsLabel = companySpecsPanel.Children[0] as Label;
            ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;




            if (choiceComboBox.SelectedIndex == 0)
            {

                //genericCategoryComboBox.IsEnabled = true;
                //genericProductComboBox.IsEnabled = true;
                //genericBrandComboBox.IsEnabled = true;
                //genericModelComboBox.IsEnabled = true;

                //companyCategoryComboBox.IsEnabled = false;
                //companyCategoryComboBox.SelectedIndex = -1;
                //companyProductComboBox.IsEnabled = false;
                //companyBrandComboBox.IsEnabled = false;
                //companyModelComboBox.IsEnabled = false;
                //companySpecsComboBox.IsEnabled = false;


                companyCategoryComboBox.IsEnabled = true;
                companyProductComboBox.IsEnabled = true;
                companyBrandComboBox.IsEnabled = true;
                companyModelComboBox.IsEnabled = true;
                companySpecsComboBox.IsEnabled = true;

                companyCategoryComboBox.Items.Clear();
                companyProductComboBox.Items.Clear();
                companyBrandComboBox.Items.Clear();
                companyModelComboBox.Items.Clear();
                companySpecsComboBox.Items.Clear();
                companySpecsComboBox.Visibility = Visibility.Collapsed;
                specsLabel.Visibility = Visibility.Collapsed;

                for(int i = 0;i<genericCategories.Count;i++)
                {
                    companyCategoryComboBox.Items.Add(genericCategories[i].category_name);
                }

            }

            else            
            {

                //genericCategoryComboBox.IsEnabled = false;
                //genericCategoryComboBox.SelectedIndex = -1;
                //genericProductComboBox.IsEnabled = false;
                //genericBrandComboBox.IsEnabled = false;
                //genericModelComboBox.IsEnabled = false;


                companyCategoryComboBox.IsEnabled = true;
                companyProductComboBox.IsEnabled = true;
                companyBrandComboBox.IsEnabled = true;
                companyModelComboBox.IsEnabled = true;
                companySpecsComboBox.IsEnabled = true;

                companyCategoryComboBox.Items.Clear();
                companyProductComboBox.Items.Clear();
                companyBrandComboBox.Items.Clear();
                companyModelComboBox.Items.Clear();
                companySpecsComboBox.Items.Clear();
                companySpecsComboBox.Visibility = Visibility.Visible;
                specsLabel.Visibility = Visibility.Visible;
                // companySpecsComboBox.Visibility = Visibility.Collapsed;
                if (workOrder.GetOrderSerial() != 0)
                {
                    for (int i = 0; i < workOrder.GetOrderProductsList().Count(); i++)
                    {
                        ComboBoxItem categoryItem = new ComboBoxItem();
                        categoryItem.Content = workOrder.GetOrderProductsList()[i].product_category.category_name;
                        categoryItem.Tag = workOrder.GetOrderProductsList()[i].product_category.category_id;
                        if (companyCategoryComboBox.Items.Cast<ComboBoxItem>().Any(f => f.Tag == categoryItem.Tag)==false && workOrder.GetOrderProductsList()[i].product_category.category_name!=null)
                            companyCategoryComboBox.Items.Add(categoryItem);

                        
                       

                    }
                }
                else
                {
                    for(int i = 0;i<rfp.rfpItems.Count();i++)
                    {
                        ComboBoxItem categoryItem = new ComboBoxItem();
                        categoryItem.Content = rfp.rfpItems[i].product_category.category_name;
                        categoryItem.Tag = rfp.rfpItems[i].product_category.category_id;
                        if (companyCategoryComboBox.Items.Cast<ComboBoxItem>().Any(f => f.Tag == categoryItem.Tag) == false && rfp.rfpItems[i].product_category.category_name != null)
                            companyCategoryComboBox.Items.Add(categoryItem);
                    }
                }

            }




        }

        /// ////////////////////////////////////////////       
        //TEXT CHANGE FUNCTIONS 
        ////////////////////////////////////////////////
        private void QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox quantity = sender as TextBox;

            Grid quantityGrid = quantity.Parent as Grid;
            TextBox availableQuantity = quantityGrid.Children[1] as TextBox;

            for (int i = 0; i < quantity.Text.Length; i++)
            {


                if (!char.IsDigit(quantity.Text[i]))
                {

                    quantity.Text = "";

                    return;
                }
            }

            if (quantity.Text == "")
            {
                availableQuantity.Text = availableQuantity.Tag.ToString();

                return;

            }

            if (int.Parse(quantity.Text) > Convert.ToInt32(availableQuantity.Tag))
            {

                System.Windows.Forms.MessageBox.Show("The available quantity is not enough", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                availableQuantity.Text = availableQuantity.Tag.ToString();
                quantity.Text = "";

                return;

            }


            availableQuantity.Text = (Convert.ToInt32(availableQuantity.Tag) - Convert.ToInt32(quantity.Text)).ToString();

        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {

            this.NavigationService.Navigate(parentWindow.releasePermitPage);


        }

        private void OnNextButtonOnClick(object sender, RoutedEventArgs e)
        {
            parentWindow.releasePermitSummary.InitializeSummarySheet();
            this.NavigationService.Navigate(parentWindow.releasePermitSummary);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {

            parentWindow.Close();

        }

        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {

            if (parentWindow.releasePermitPage.ReleaseDatePicker.DisplayDate.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show("release date is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            if (parentWindow.releasePermitPage.SerialIdTextBox.Text == "") {

                System.Windows.Forms.MessageBox.Show("Serial id is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }


            if (!FillReleasePermitItems())
                return;

            int status = 0;

            ///////////////////////////////////////////////////////////////////////
            ///YOU NEED TO SET ORDER / RFP STATUS, AND SET RELEASE PERMIT STATUS
            ///////////////////////////////////////////////////////////////////////
            if (parentWindow.releasePermitPage.workFormComboBox.SelectedIndex == 1)
            {
                if (parentWindow.releasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && parentWindow.releasePermitPage.contactComboBox.SelectedIndex != -1)
                    status = COMPANY_WORK_MACROS.ORDER_PENDING_ClIENT_RECIEVAL;
                else
                    status = COMPANY_WORK_MACROS.ORDER_RECEIVED_BY_CLIENT;
            }
            else
                status = COMPANY_WORK_MACROS.RFP_PENDING_SITE_RECEIVAL;


            //parentWindow.releasePermitPage.materialReleasePermit.GetReleaseItems().ForEach(a => a.release_permit_item_status = status);
            parentWindow.releasePermitPage.workOrder.GetOrderProductsList().ToList().ForEach(a => a.product_status.status_id = status);

            if (parentWindow.releasePermitPage.workFormComboBox.SelectedIndex == 1)
            {
                if (parentWindow.releasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && parentWindow.releasePermitPage.contactComboBox.SelectedIndex != -1)
                    parentWindow.releasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL);
                else
                    parentWindow.releasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.RECEIVAL_NOTE_RECEIVED);
            }

            else
                parentWindow.releasePermitPage.materialReleasePermit.SetReleasePermitStatusId(COMPANY_WORK_MACROS.RECEIVAL_NOTE_RECEIVED);



            
            if (!parentWindow.releasePermitPage.materialReleasePermit.IssueNewMaterialRelease(ref serials,ref parentWindow.releasePermitPage.rfpItems , workOrder.GetOrderSerial(),orderItemNumber))
                return;


            ReleasePermitUploadFilesPage = new ReleasePermitUploadFilesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, this.parentWindow.releasePermitPage, parentWindow, ref parentWindow.releasePermitPage.materialReleasePermit);

            ReleasePermitUploadFilesPage.addReleasePermitItemPage = this;
            NavigationService.Navigate(ReleasePermitUploadFilesPage);


            parentWindow.func1.Invoke(parentWindow.releasePermitPage.materialReleasePermit.GetReleaseSerial());


        }

        private void BasicInfoLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(parentWindow.releasePermitPage);
        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (ReleasePermitUploadFilesPage != null)
                this.NavigationService.Navigate(ReleasePermitUploadFilesPage);
        }

        private bool FillReleasePermitItems() {



            parentWindow.releasePermitPage.materialReleasePermit.GetReleaseItems().Clear();

            Grid items = Home.Children[Home.Children.Count - 1] as Grid;

            ScrollViewer scroll = items.Children[1] as ScrollViewer;

            Grid itemsBody = scroll.Content as Grid;

            serials.Clear();

            int counter = 0;

            for (int i = 0; i < itemsBody.Children.Count; i++)
            {

                counter++;

                INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM releaseItem = new INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM();

                Grid item = itemsBody.Children[i] as Grid;

                bool isSerial = true;


                CheckBox productSerialCheckBox1 = item.Children[2] as CheckBox;


                if (productSerialCheckBox1 == null)
                    isSerial = false;




                if (isSerial == true)
                {

                    CheckBox productSerialCheckBox = item.Children[2] as CheckBox;

                    if (productSerialCheckBox.IsChecked == true)
                    {


                        serials.Add(productSerialCheckBox.Content.ToString());


                        releaseItem.release_permit_item_serial = counter;

                        releaseItem.entry_permit_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        releaseItem.entry_permit_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);
                        releaseItem.released_quantity_release = numberOfSelectedItems;



                        Grid location = productSerialCheckBox.Tag as Grid;


                        WrapPanel checkBoxesPanel = location.Children[1] as WrapPanel;

                        CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                        CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;



                        if (rfpCheckBox.IsChecked == true)
                        {


                            Grid rfpLocationGrid = location.Children[2] as Grid;



                            ComboBox rfpItems = rfpLocationGrid.Children[0] as ComboBox;

                            ComboBox locationsComboBox = rfpLocationGrid.Children[1] as ComboBox;

                            //Grid identityTextBoxGrid = rfpLocationGrid.Children[2] as Grid;


                            releaseItem.rfp_info.rfpRequestorTeam = parentWindow.releasePermitPage.rfp.GetRFPRequestorTeamId();


                            releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT;

                            releaseItem.rfp_info.rfpSerial = parentWindow.releasePermitPage.rfp.GetRFPSerial();
                            releaseItem.rfp_info.rfpVersion = parentWindow.releasePermitPage.rfp.GetRFPVersion();





                            if (rfpItems.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show($"you have to choose an rfp item:(if there are no items please contact the inventory to map the items)", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                                return false;
                            }

                            releaseItem.rfp_item_number = parentWindow.releasePermitPage.rfpItems[rfpItems.SelectedIndex].rfp_item_number;


                            if (parentWindow.releasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
                            {
                                WorkOrder workOrder = new WorkOrder();


                                workOrder.InitializeWorkOrderInfo(parentWindow.releasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                                workOrdersLocations.Clear();
                                workOrder.GetProjectLocations(ref workOrdersLocations);

                                releaseItem.order_serial = workOrder.GetOrderSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                    releaseItem.order_project_serial = workOrder.GetprojectSerial();


                                }

                            }

                            else if (parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
                            {


                                maintenanceContract.InitializeMaintenanceContractInfo(parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                                releaseItem.contract_serial = maintenanceContract.GetMaintContractSerial();
                                releaseItem.contract_version = maintenanceContract.GetMaintContractVersion();

                                releaseItem.contract_project_serial = maintenanceContract.GetprojectSerial();


                                maintenanceContractLocations.Clear();

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.contract_location_id = maintenanceContractLocations[locationsComboBox.SelectedIndex].location_id;


                                }

                            }


                            else if (parentWindow.releasePermitPage.rfp.GetProjectSerial() != 0)
                            {


                                companyProjectLocations.Clear();

                                commonQueries.GetCompanyProjectLocations(parentWindow.releasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                                releaseItem.company_project_serial = parentWindow.releasePermitPage.rfp.GetProjectSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {
                                    releaseItem.company_project_location = companyProjectLocations[locationsComboBox.SelectedIndex].location_id;


                                }


                            }


                        }


                        else if (orderCheckBox.IsChecked == true)
                        {


                            if (parentWindow.releasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && parentWindow.releasePermitPage.contactComboBox.SelectedIndex != -1)
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                            else
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT;

                            Grid orderLocationGrid = location.Children[2] as Grid;

                            ComboBox orderItemsComboBox = orderLocationGrid.Children[0] as ComboBox;


                            ComboBox locationsComboBox = orderLocationGrid.Children[1] as ComboBox;



                    

                            ComboBox ordersComboBox = parentWindow.releasePermitPage.orderSerials as ComboBox;

                            if (ordersComboBox.SelectedIndex != -1)
                            {




                                releaseItem.workOrder_serial = parentWindow.releasePermitPage.workOrder.GetOrderSerial();

                                releaseItem.order_serial = parentWindow.releasePermitPage.workOrder.GetOrderSerial();


                                releaseItem.order_project_serial = parentWindow.releasePermitPage.workOrder.GetprojectSerial();


                                if (orderItemsComboBox.SelectedIndex != -1)
                                {

                                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = parentWindow.releasePermitPage.workOrder.GetOrderProductsList();

                                    releaseItem.workOrder_product_number = workOrderProducts[orderItemsComboBox.SelectedIndex].productNumber;


                                    if (workOrderProducts[orderItemsComboBox.SelectedIndex].productQuantity < parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                                    {

                                        System.Windows.Forms.MessageBox.Show("exceeded the max allowed quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        return false;
                                    }

                                }

                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("You have to choose workOrder Item ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return false;

                                }


                                workOrdersLocations.Clear();

                                parentWindow.releasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);

                                if (locationsComboBox.SelectedIndex != -1)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                }

                            }

                            else
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose workOrder", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return false;

                            }

                        }


                        else
                        {
                            System.Windows.Forms.MessageBox.Show($"rfp or workOrder should be checked on item number {item.Tag.ToString().Split(' ')[2]}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return false;
                        }

                        parentWindow.releasePermitPage.materialReleasePermit.AddReleaseItem(releaseItem);
                        parentWindow.releasePermitPage.materialReleasePermit.UpdateIsReleasedInfo(releaseItem.entry_permit_serial, releaseItem.entry_permit_item_serial, true);



                    }

                }



                else
                {

                    Grid quantityGrid = item.Children[2] as Grid;
                    CheckBox quantityCheckBox = quantityGrid.Children[0] as CheckBox;
                    TextBox quantityTextBox = quantityGrid.Children[2] as TextBox;


                    if (quantityCheckBox.IsChecked == true)
                    {

                        Grid card = quantityCheckBox.Tag as Grid;


                        releaseItem.release_permit_item_serial = counter;

                        releaseItem.entry_permit_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[0]);
                        releaseItem.entry_permit_item_serial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);

                        if (quantityTextBox.Text != "")
                            releaseItem.released_quantity_release = int.Parse(quantityTextBox.Text);


                        Grid location = quantityCheckBox.Tag as Grid;


                        WrapPanel checkBoxesPanel = location.Children[1] as WrapPanel;

                        CheckBox rfpCheckBox = checkBoxesPanel.Children[0] as CheckBox;

                        CheckBox orderCheckBox = checkBoxesPanel.Children[1] as CheckBox;



                        if (rfpCheckBox.IsChecked == true)
                        {

                            releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_SERVICE_REPORT;

                            Grid rfpLocationGrid = location.Children[2] as Grid;

                            ComboBox rfpItems = rfpLocationGrid.Children[0] as ComboBox;

                            ComboBox locationsComboBox = rfpLocationGrid.Children[1] as ComboBox;





                            releaseItem.rfp_info.rfpRequestorTeam = parentWindow.releasePermitPage.rfp.GetRFPRequestorTeamId();

                            releaseItem.rfp_info.rfpSerial = parentWindow.releasePermitPage.rfp.GetRFPSerial();

                            releaseItem.rfp_info.rfpVersion = parentWindow.releasePermitPage.rfp.GetRFPVersion();

                            if (rfpItems.SelectedIndex == -1)
                            {

                                System.Windows.Forms.MessageBox.Show($"you have to choose an rfp item:(if there are no items please contact the inventory to map the items)", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


                                return false;
                            }


                            releaseItem.rfp_item_number = parentWindow.releasePermitPage.rfpItems[rfpItems.SelectedIndex].rfp_item_number;


                            if (parentWindow.releasePermitPage.rfp.GetWorkOrder().GetOrderSerial() != 0)
                            {

                                WorkOrder workOrder = new WorkOrder();


                                workOrder.InitializeWorkOrderInfo(parentWindow.releasePermitPage.rfp.GetWorkOrder().GetOrderSerial());

                                workOrdersLocations.Clear();
                                workOrder.GetProjectLocations(ref workOrdersLocations);

                                releaseItem.order_serial = workOrder.GetOrderSerial();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;

                                    releaseItem.order_project_serial = workOrder.GetprojectSerial();


                                }




                            }
                            else if (parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractSerial() != 0)
                            {


                                maintenanceContract.InitializeMaintenanceContractInfo(parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractSerial(), parentWindow.releasePermitPage.rfp.GetMaintContract().GetMaintContractVersion());

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();


                                releaseItem.contract_serial = maintenanceContract.GetMaintContractSerial();
                                releaseItem.contract_version = maintenanceContract.GetMaintContractVersion();

                                releaseItem.contract_project_serial = maintenanceContract.GetprojectSerial();


                                maintenanceContractLocations.Clear();

                                maintenanceContractLocations = maintenanceContract.GetMaintContractProjectLocations();

                                if (locationsComboBox.Items.Count != 0)
                                {

                                    releaseItem.contract_location_id = maintenanceContractLocations[locationsComboBox.SelectedIndex].location_id;


                                }



                            }
                            else
                            {

                                companyProjectLocations.Clear();

                                commonQueries.GetCompanyProjectLocations(parentWindow.releasePermitPage.rfp.GetProjectSerial(), ref companyProjectLocations);

                                releaseItem.company_project_serial = parentWindow.releasePermitPage.rfp.GetProjectSerial();
                                if (locationsComboBox.Items.Count != 0)
                                {
                                    releaseItem.company_project_location = companyProjectLocations[locationsComboBox.SelectedIndex].location_id;


                                }


                            }


                        }


                        else if (orderCheckBox.IsChecked == true)
                        {

                            if (parentWindow.releasePermitPage.MaterialRecieverComboBox.SelectedIndex == -1 && parentWindow.releasePermitPage.contactComboBox.SelectedIndex != -1)
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL;
                            else
                                releaseItem.release_permit_item_status = COMPANY_WORK_MACROS.RECEIVAL_NOTE_RECEIVED;

                            Grid orderLocationGrid = location.Children[2] as Grid;


                            ComboBox orderItemsComboBox = orderLocationGrid.Children[0] as ComboBox;


                            ComboBox locationsComboBox = orderLocationGrid.Children[1] as ComboBox;


                            

                            ComboBox ordersComboBox = parentWindow.releasePermitPage.orderSerials as ComboBox;


                            if (ordersComboBox.SelectedIndex != -1)
                            {

                                releaseItem.workOrder_serial = parentWindow.releasePermitPage.workOrder.GetOrderSerial();

                                releaseItem.order_serial = parentWindow.releasePermitPage.workOrder.GetOrderSerial();


                                releaseItem.order_project_serial = parentWindow.releasePermitPage.workOrder.GetprojectSerial();


                                if (orderItemsComboBox.SelectedIndex != -1)
                                {

                                    PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] workOrderProducts = parentWindow.releasePermitPage.workOrder.GetOrderProductsList();

                                    releaseItem.workOrder_product_number = workOrderProducts[orderItemsComboBox.SelectedIndex].productNumber;


                                    if (workOrderProducts[orderItemsComboBox.SelectedIndex].productQuantity < parentWindow.releasePermitPage.serialProducts[orderItemsComboBox.SelectedIndex])
                                    {

                                        System.Windows.Forms.MessageBox.Show("exceeded the max allowed quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        return false;
                                    }

                                }

                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("You have to choose workOrder Item ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    return false;
                                }


                                workOrdersLocations.Clear();

                                parentWindow.releasePermitPage.workOrder.GetProjectLocations(ref workOrdersLocations);

                                if (locationsComboBox.SelectedIndex != -1)
                                {
                                    releaseItem.order_location_id = workOrdersLocations[locationsComboBox.SelectedIndex].location_id;
                                }

                            }


                            else
                            {

                                System.Windows.Forms.MessageBox.Show("You have to choose workOrder", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                return false;

                            }



                        }


                        else
                        {

                            System.Windows.Forms.MessageBox.Show($"rfp or workOrder should be checked on item number {item.Tag.ToString().Split(' ')[2]}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                            return false;

                        }

                        parentWindow.releasePermitPage.materialReleasePermit.AddReleaseItem(releaseItem);
                        parentWindow.releasePermitPage.materialReleasePermit.UpdateReleasedQuantity(int.Parse(quantityTextBox.Text), releaseItem.entry_permit_serial, releaseItem.entry_permit_item_serial);
                    }

                }


            }
            return true;

        }

      
    }
}
