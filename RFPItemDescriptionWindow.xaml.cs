using _01electronics_library;
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
using System.Windows.Shapes;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for RFPItemDescriptionWindow.xaml
    /// </summary>
    public partial class RFPItemDescriptionWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> companyProductCategoryList;
        List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> companyProductList;
        List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> companyProductBrandList;
        List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> companyProductModelList;
        List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT> companyModelSpecs;

        List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> genericCategoryList;
        List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> genericProductTypeList;
        List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> genericProductBrandsList;
        List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT> genericProductModelList;

        PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem;
        int viewAddCondition;

        string description;
        RFP rfp;

        public RFPItemDescriptionWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref RFP mRfp, ref PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT mrfpItem , int view)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            companyProductCategoryList = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            companyProductList = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            companyProductBrandList = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            companyProductModelList = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();
            companyModelSpecs= new List<PRODUCTS_STRUCTS.PRODUCT_SPECS_STRUCT>();

            genericCategoryList = new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
            genericProductTypeList = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
            genericProductBrandsList = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
            genericProductModelList = new List<PRODUCTS_STRUCTS.PRODUCT_MODEL_STRUCT>();


            rfp = mRfp;
            rfpItem = mrfpItem;
            description = string.Empty;
            viewAddCondition = view;

            rfp.SetRFPRequestor(loggedInUser);
            InitializationFunction();
            CheckEditOrAdd();
        }
        private void CheckEditOrAdd()
        {
            DisableFunction();
            
            if (rfpItem.is_company_product)
            {
                //edit = true;
                
                
                mapCompanyProductCheckBox.IsChecked = true;
                mapGenericProductCheckBox.IsChecked = false;

                descriptionCheckBox.IsChecked = false;

                categoryCompanyComboBox.Items.Clear();
                FillCompanyProductCategoryComboBox();

                categoryCompanyComboBox.IsEnabled = true;
                typeCompanyComboBox.IsEnabled = true;
                brandCompanyComboBox.IsEnabled = true;
                modelCompanyTextBlock.IsEnabled = true;

                for (int i = 0; i < companyProductCategoryList.Count; i++)
                {
                    if (rfpItem.product_category.category_id == companyProductCategoryList[i].category_id)
                    {
                        categoryCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductType(rfpItem.product_category.category_id);
                for (int i = 0; i < companyProductList.Count; i++)
                {
                    if (rfpItem.product_type.type_id == companyProductList[i].type_id)
                    {
                        typeCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductBrand(rfpItem.product_type.type_id);
                for (int i = 0; i < companyProductBrandList.Count; i++)
                {
                    if (rfpItem.product_brand.brand_id == companyProductBrandList[i].brand_id)
                    {
                        brandCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductModel(rfpItem.product_type, rfpItem.product_brand);
                for (int i = 0; i < companyProductModelList.Count; i++)
                {
                    if (rfpItem.product_model.model_id == companyProductModelList[i].model_id)
                    {
                        modelCompanyTextBlock.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyModelSpecs(rfpItem.product_category.category_id, rfpItem.product_type.type_id, rfpItem.product_brand.brand_id, rfpItem.product_model.model_id);
                for (int i = 0; i < companyModelSpecs.Count; i++)
                {
                    if (rfpItem.product_specs.spec_id == companyModelSpecs[i].spec_id)
                    {
                        specsComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (rfpItem.product_model.model_name != "" && rfpItem.product_model.model_name != null && rfpItem.product_category.category_id != 0)
            {
                // edit = true;
                viewAddCondition = COMPANY_WORK_MACROS.RFP_EDIT_CONDITION;
                mapCompanyProductCheckBox.IsChecked = false;
                mapGenericProductCheckBox.IsChecked = true;
                descriptionCheckBox.IsChecked = false;
                categoryComboBox.IsEnabled = true;
                typeComboBox.IsEnabled = true;
                brandComboBox.IsEnabled = true;
                modelTextBlock.IsEnabled = true;
                categoryComboBox.Items.Clear();
                FillGenericCategory();
                for (int i = 0; i < genericCategoryList.Count; i++)
                {
                    if (rfpItem.product_category.category_id == genericCategoryList[i].category_id)
                    {
                        categoryComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericType(rfpItem.product_category.category_id);
                for (int i = 0; i < genericProductTypeList.Count; i++)
                {
                    if (rfpItem.product_type.type_id == genericProductTypeList[i].type_id)
                    {
                        typeComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericBrand(rfpItem.product_category.category_id, rfpItem.product_type.type_id);
                for (int i = 0; i < genericProductBrandsList.Count; i++)
                {
                    if (rfpItem.product_brand.brand_id == genericProductBrandsList[i].brand_id)
                    {
                        brandComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericModel(rfpItem.product_category.category_id, rfpItem.product_type.type_id, rfpItem.product_brand.brand_id);
                for (int i = 0; i < genericProductModelList.Count; i++)
                {
                    if (rfpItem.product_model.model_id == genericProductModelList[i].model_id)
                    {
                        modelTextBlock.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (rfpItem.item_description != null)
            {
                //edit = true;
                viewAddCondition = COMPANY_WORK_MACROS.RFP_EDIT_CONDITION;
                mapCompanyProductCheckBox.IsChecked = false;
                mapGenericProductCheckBox.IsChecked = false;
                descriptionCheckBox.IsChecked = true;

                descriptionTextBox.Text = rfpItem.item_description;
                descriptionTextBox.IsEnabled = true;
            }
            //else
            //{
            //    InitializationFunction();
            //}

        }
        private void InitializationFunction()
        {
            DisableFunction();
            FillCompanyProductCategoryComboBox();
            FillGenericCategory();
        }
        /// <summary>
        /// ////////////////////////////////////////// FILL FUNCTIONS ////////////////////////////////////////
        /// </summary>
        private void FillCompanyProductCategoryComboBox()
        {
            if (!commonQueries.GetProductCategories(ref companyProductCategoryList))
                return;
            else
            {
                for (int i = 0; i < companyProductCategoryList.Count; i++)
                {
                    categoryCompanyComboBox.Items.Add(companyProductCategoryList[i].category_name);
                }
            }
        }
        private void FillCompanyProductType(int category_id)
        {
            if (!commonQueries.GetCompanyProducts(ref companyProductList, category_id))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                typeCompanyComboBox.Items.Clear();
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductList.Count; i++)
                {
                    typeCompanyComboBox.Items.Add(companyProductList[i].product_name);
                }
            }
        }
        private void FillCompanyProductBrand(int type_id)
        {
            if (!commonQueries.GetProductBrands(type_id, ref companyProductBrandList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductBrandList.Count; i++)
                {
                    brandCompanyComboBox.Items.Add(companyProductBrandList[i].brand_name);
                }
            }
        }
        private void FillCompanyProductModel(PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT product, PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT brand)
        {
            if (!commonQueries.GetCompanyModels(product, brand, ref companyProductModelList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductModelList.Count; i++)
                {
                    modelCompanyTextBlock.Items.Add(companyProductModelList[i].model_name);
                }
            }
        }
        private void FillGenericCategory()
        {
            if (!commonQueries.GetGenericProductCategories(ref genericCategoryList))
                return;
            else
            {
                typeComboBox.Items.Clear();
                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                for (int i = 0; i < genericCategoryList.Count; i++)
                {
                    categoryComboBox.Items.Add(genericCategoryList[i].category_name);
                }
            }
        }
        private void FillGenericType(int category_id)
        {
            if (!commonQueries.GetGenericProducts(ref genericProductTypeList, category_id))
                return;
            else
            {
                typeComboBox.Items.Clear();
                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                for (int i = 0; i < genericProductTypeList.Count; i++)
                {
                    typeComboBox.Items.Add(genericProductTypeList[i].product_name);
                }
            }

        }
        private void FillGenericBrand(int category_id, int type_id)
        {
            if (!commonQueries.GetGenericProductBrands(type_id, category_id, ref genericProductBrandsList))
                return;
            else
            {

                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                for (int i = 0; i < genericProductBrandsList.Count; i++)
                {
                    brandComboBox.Items.Add(genericProductBrandsList[i].brand_name);
                }
            }
        }
        private void FillGenericModel(int category_id, int type_id, int brand_id)
        {
            if (!commonQueries.GetGenericBrandModels(type_id, brand_id, category_id, ref genericProductModelList))
                return;
            else
            {
                modelTextBlock.Items.Clear();
                for (int i = 0; i < genericProductModelList.Count; i++)
                {
                    modelTextBlock.Items.Add(genericProductModelList[i].model_name);
                }
            }
        }
        private void FillCompanyModelSpecs(int companyCategoryId, int companyProductId, int companyBrandId, int companyModelId)
        {
            if (!commonQueries.GetModelSpecsNames(companyCategoryId, companyProductId, companyBrandId, companyModelId, ref companyModelSpecs))
                return;
            specsComboBox.Items.Clear();
            for (int i = 0; i < companyModelSpecs.Count; i++)
            {
                specsComboBox.Items.Add(companyModelSpecs[i].spec_name);
            }
        }
        /// <summary>
        /// /////////////////////////////////////////// DISABLE COMBOBOXES FUNCTIONS //////////////////////////////////
        /// </summary>
        private void DisableFunction()
        {
            categoryComboBox.IsEnabled = false;
            typeComboBox.IsEnabled = false;
            brandComboBox.IsEnabled = false;
            modelTextBlock.IsEnabled = false;

            categoryCompanyComboBox.IsEnabled = false;
            typeCompanyComboBox.IsEnabled = false;
            brandCompanyComboBox.IsEnabled = false;
            modelCompanyTextBlock.IsEnabled = false;
            specsComboBox.IsEnabled = false;

            descriptionTextBox.IsEnabled = false;
        }
        private void DisableCompanyBoxes()
        {
            categoryCompanyComboBox.IsEnabled = false;
            typeCompanyComboBox.IsEnabled = false;
            brandCompanyComboBox.IsEnabled = false;
            modelCompanyTextBlock.IsEnabled = false;
            specsComboBox.IsEnabled = false;
        }
        private void DisableGenericBoxes()
        {
            categoryComboBox.IsEnabled = false;
            typeComboBox.IsEnabled = false;
            brandComboBox.IsEnabled = false;
            modelTextBlock.IsEnabled = false;
        }
        /// <summary>
        /// ///////////////////////////////////// ENABLE COMBOBOXES FUNCTIONS ///////////////////////////////////////
        /// </summary>
        private void EnableCompanyBoxes()
        {
            categoryCompanyComboBox.IsEnabled = true;
            //typeCompanyComboBox.IsEnabled = true;
            //brandCompanyComboBox.IsEnabled = true;
            //modelCompanyTextBlock.IsEnabled = true;
        }
        private void EnableGenericBoxes()
        {
            categoryComboBox.IsEnabled = true;
            //typeComboBox.IsEnabled = true;
            //brandComboBox.IsEnabled = true;
            //modelTextBlock.IsEnabled = true;
        }
        /// <summary>
        /// ////////////////////////////////////// CLEAR COMBOBOXES FUNCTIONS ///////////////////////////////////////////
        /// </summary>
        private void ClearGenericBoxes()
        {
            categoryComboBox.SelectedIndex = -1;
            typeComboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;
            modelTextBlock.SelectedIndex = -1;
        }
        private void ClearCompanyBoxes()
        {
            categoryCompanyComboBox.SelectedIndex = -1;
            typeCompanyComboBox.SelectedIndex = -1;
            brandCompanyComboBox.SelectedIndex = -1;
            modelCompanyTextBlock.SelectedIndex = -1;
            specsComboBox.SelectedIndex = -1;
        }
        /// <summary>
        /// //////////////////////////////////////////// COMBOBOXES SELECTION CHANGED ///////////////////////////////
        /// </summary>

        private void OnSelChangedCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedIndex != -1)
            {
                typeComboBox.Items.Clear();
                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                int category_id = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                typeComboBox.IsEnabled = true;
                FillGenericType(category_id);
            }
        }

        private void OnSelChangedTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (typeComboBox.SelectedIndex != -1)
            {
                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                int category_id = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                int type_id = genericProductTypeList[typeComboBox.SelectedIndex].type_id;
                brandComboBox.IsEnabled = true;
                FillGenericBrand(category_id, type_id);
            }
        }

        private void OnSelChangedBrandComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (brandComboBox.SelectedIndex != -1)
            {
                modelTextBlock.Items.Clear();
                modelTextBlock.IsEnabled = true;
                int category_id = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                int type_id = genericProductTypeList[typeComboBox.SelectedIndex].type_id;
                int brand_id = genericProductBrandsList[brandComboBox.SelectedIndex].brand_id;
                FillGenericModel(category_id, type_id, brand_id);
            }
        }

        private void OnSelChangedCompanyCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (categoryCompanyComboBox.SelectedIndex != -1)
            {
                typeCompanyComboBox.Items.Clear();
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                typeCompanyComboBox.IsEnabled = true;
                int companCategoryId = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex].category_id;
                FillCompanyProductType(companCategoryId);
            }
        }

        private void OnSelChangedCompanyTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (typeCompanyComboBox.SelectedIndex != -1)
            {
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                brandCompanyComboBox.IsEnabled = true;
                int companyTypeId = companyProductList[typeCompanyComboBox.SelectedIndex].type_id;
                FillCompanyProductBrand(companyTypeId);
            }
        }

        private void OnSelChangedCompanyBrandComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (brandCompanyComboBox.SelectedIndex != -1 && typeCompanyComboBox.SelectedIndex != -1)
            {
                modelCompanyTextBlock.Items.Clear();
                modelCompanyTextBlock.IsEnabled = true;
                FillCompanyProductModel(companyProductList[typeCompanyComboBox.SelectedIndex], companyProductBrandList[brandCompanyComboBox.SelectedIndex]);
            }
        }
        private void OnSelChangedModelCompanyTextBlock(object sender, SelectionChangedEventArgs e)
        {
            if (modelCompanyTextBlock.SelectedIndex != -1 && brandCompanyComboBox.SelectedIndex != -1 && typeCompanyComboBox.SelectedIndex != -1)
            {
                specsComboBox.Items.Clear();
                specsComboBox.IsEnabled = true;
                FillCompanyModelSpecs(companyProductCategoryList[categoryCompanyComboBox.SelectedIndex].category_id, companyProductList[typeCompanyComboBox.SelectedIndex].type_id, companyProductBrandList[brandCompanyComboBox.SelectedIndex].brand_id, companyProductModelList[modelCompanyTextBlock.SelectedIndex].model_id);
            }
        }
        /// <summary>
        /// ////////////////////////////////////////////// ON CLICK CKECK BOXES ///////////////////////////////////
        /// </summary>

        private void OnCheckMapToGenericProduct(object sender, RoutedEventArgs e)
        {
            if (mapGenericProductCheckBox.IsChecked == true)
            {
                DisableFunction();
                EnableGenericBoxes();
                ClearCompanyBoxes();
                mapCompanyProductCheckBox.IsChecked = false;
                descriptionCheckBox.IsChecked = false;
                descriptionTextBox.Text = string.Empty;
            }
            else
            {
                DisableGenericBoxes();
                ClearGenericBoxes();
            }
        }

        private void OnCheckMapToCompanyProduct(object sender, RoutedEventArgs e)
        {
            if (mapCompanyProductCheckBox.IsChecked == true)
            {
                DisableFunction();
                EnableCompanyBoxes();
                ClearGenericBoxes();
                mapGenericProductCheckBox.IsChecked = false;
                descriptionCheckBox.IsChecked = false;
                descriptionTextBox.Text = string.Empty;
            }
            else
            {
                DisableCompanyBoxes();
                ClearCompanyBoxes();
            }
        }

        private void OnCheckEnterDescription(object sender, RoutedEventArgs e)
        {
            if (descriptionCheckBox.IsChecked == true)
            {
                DisableFunction();
                descriptionTextBox.IsEnabled = true;
                mapCompanyProductCheckBox.IsChecked = false;
                mapGenericProductCheckBox.IsChecked = false;
                ClearGenericBoxes();
                ClearCompanyBoxes();
            }
            else
            {
                descriptionTextBox.IsEnabled = false;
            }
        }
        /// <summary>
        /// //////////////////////////////////////////// ON BUTTON CLICK SAVE ///////////////////////////////////
        /// </summary>
        private void OnButtonClickSave(object sender, RoutedEventArgs e)
        {
            if (mapCompanyProductCheckBox.IsChecked == true)
            {
                if (modelCompanyTextBlock.SelectedIndex != -1)
                {
                    rfpItem.product_category = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex];
                    rfpItem.product_type = companyProductList[typeCompanyComboBox.SelectedIndex];
                    rfpItem.product_brand = companyProductBrandList[brandCompanyComboBox.SelectedIndex];
                    rfpItem.product_model = companyProductModelList[modelCompanyTextBlock.SelectedIndex];
                    rfpItem.product_specs= companyModelSpecs[specsComboBox.SelectedIndex];
                    rfpItem.item_description = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex].category_name + " - " +
                                               companyProductList[typeCompanyComboBox.SelectedIndex].product_name + " - " +
                                               companyProductBrandList[brandCompanyComboBox.SelectedIndex].brand_name + " - " +
                                               companyProductModelList[modelCompanyTextBlock.SelectedIndex].model_name + " - " +
                                               companyModelSpecs[specsComboBox.SelectedIndex].spec_name;
                    rfpItem.is_company_product = true;

                    //rfpItem.product_category.category_id = 0;
                    //rfpItem.product_category.category_name = string.Empty;
                    //rfpItem.product_type.type_id = 0;
                    //rfpItem.product_type.product_name = string.Empty;
                    //rfpItem.product_brand.brand_id = 0;
                    //rfpItem.product_brand.brand_name = string.Empty;
                    //rfpItem.product_model.model_id = 0;
                    //rfpItem.product_model.model_name = string.Empty;

                    if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)
                        rfp.AddRFPItem(rfpItem);
                    else
                    {
                        rfp.rfpItems[rfpItem.rfp_item_number-1] = rfpItem;
                       
                    }
                    this.Close();

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please make sure to fill comboboxes till you reach the model , If this was possible.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            else if (mapGenericProductCheckBox.IsChecked == true)
            {
                if (modelTextBlock.SelectedIndex != -1)
                {
                    rfpItem.product_category = genericCategoryList[categoryComboBox.SelectedIndex];
                    rfpItem.product_type = genericProductTypeList[typeComboBox.SelectedIndex];
                    rfpItem.product_brand = genericProductBrandsList[brandComboBox.SelectedIndex];
                    rfpItem.product_model = genericProductModelList[modelTextBlock.SelectedIndex];
                    rfpItem.item_description = genericCategoryList[categoryComboBox.SelectedIndex].category_name + " - " +
                                               genericProductTypeList[typeComboBox.SelectedIndex].product_name + " - " +
                                               genericProductBrandsList[brandComboBox.SelectedIndex].brand_name + " - " +
                                               genericProductModelList[modelTextBlock.SelectedIndex].model_name;
                    rfpItem.is_company_product = false;

                    //rfpItem.product_category.category_id = 0;
                    //rfpItem.product_category.category_name = string.Empty;
                    //rfpItem.product_type.type_id = 0;
                    //rfpItem.product_type.product_name = string.Empty;
                    //rfpItem.product_brand.brand_id = 0;
                    //rfpItem.product_brand.brand_name = string.Empty;
                    //rfpItem.product_model.model_id = 0;
                    //rfpItem.product_model.model_name = string.Empty;
                    //rfpItem.product_specs.spec_id = 0;
                    //rfpItem.product_specs.spec_name = string.Empty;
                    if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)

                        rfp.AddRFPItem(rfpItem);

                    else
                    {
                        rfp.rfpItems[rfpItem.rfp_item_number - 1] = rfpItem;
                    }
                    this.Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please make sure to fill comboboxes till you reach the model , If this was possible.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            else
            {
                rfpItem.product_category.category_id = 0;
                rfpItem.product_category.category_name = string.Empty;
                rfpItem.product_type.type_id = 0;
                rfpItem.product_type.product_name = string.Empty;
                rfpItem.product_brand.brand_id = 0;
                rfpItem.product_brand.brand_name = string.Empty;
                rfpItem.product_model.model_id = 0;
                rfpItem.product_model.model_name = string.Empty;
                rfpItem.item_description = descriptionTextBox.Text;

                //rfpItem.product_category.category_id = 0;
                //rfpItem.product_category.category_name = string.Empty;
                //rfpItem.product_type.type_id = 0;
                //rfpItem.product_type.product_name = string.Empty;
                //rfpItem.product_brand.brand_id = 0;
                //rfpItem.product_brand.brand_name = string.Empty;
                //rfpItem.product_model.model_id = 0;
                //rfpItem.product_model.model_name = string.Empty;
                if (viewAddCondition != COMPANY_WORK_MACROS.RFP_EDIT_CONDITION)

                    rfp.AddRFPItem(rfpItem);

                else
                    rfp.rfpItems[rfpItem.rfp_item_number - 1] = rfpItem;
                this.Close();
            }
        }
    }
}

