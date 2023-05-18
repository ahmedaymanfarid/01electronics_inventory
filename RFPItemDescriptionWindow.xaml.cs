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

        List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> companyProductCategoryList;
        List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> companyProductList;
        List<COMPANY_WORK_MACROS.BRAND_STRUCT> companyProductBrandList;
        List<COMPANY_WORK_MACROS.MODEL_STRUCT> companyProductModelList;

        List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> genericCategoryList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> genericProductTypeList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> genericProductBrandsList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL> genericProductModelList;

        string description;
        RFP rfp;
        int index;
        PROCUREMENT_STRUCTS.RFPS_ITEMS_MIN_STRUCT rfpItem;
        bool edit = false;
        bool view;
        public RFPItemDescriptionWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref RFP mRfp, ref PROCUREMENT_STRUCTS.RFPS_ITEMS_MIN_STRUCT mrfpItem, ref int indexx, ref bool view)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            companyProductCategoryList = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
            companyProductList = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            companyProductBrandList = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
            companyProductModelList = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

            genericCategoryList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY>();
            genericProductTypeList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS>();
            genericProductBrandsList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();
            genericProductModelList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_MODEL>();

            rfp = mRfp;
            rfpItem = mrfpItem;
            description = string.Empty;
            index = indexx;
            this.view = view;
            rfp.SetRFPRequestor(loggedInUser);
            InitializationFunction();
            CheckEditOrAdd();
        }
        private void CheckEditOrAdd()
        {
            DisableFunction();
            if (rfpItem.company_model.modelName != "" && rfpItem.company_model.modelName != null && rfpItem.company_category.categoryId != 0)
            {
                edit = true;
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
                    if (rfpItem.company_category.categoryId == companyProductCategoryList[i].categoryId)
                    {
                        categoryCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductType(rfpItem.company_category.categoryId);
                for (int i = 0; i < companyProductList.Count; i++)
                {
                    if (rfpItem.company_product.typeId == companyProductList[i].typeId)
                    {
                        typeCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductBrand(rfpItem.company_product.typeId);
                for (int i = 0; i < companyProductBrandList.Count; i++)
                {
                    if (rfpItem.company_brand.brandId == companyProductBrandList[i].brandId)
                    {
                        brandCompanyComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillCompanyProductModel(rfpItem.company_product, rfpItem.company_brand);
                for (int i = 0; i < companyProductModelList.Count; i++)
                {
                    if (rfpItem.company_model.modelId == companyProductModelList[i].modelId)
                    {
                        modelCompanyTextBlock.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (rfpItem.generic_product_model.model_name != "" && rfpItem.generic_product_model.model_name != null && rfpItem.generic_product_category.category_id != 0)
            {
                edit = true;
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
                    if (rfpItem.generic_product_category.category_id == genericCategoryList[i].category_id)
                    {
                        categoryComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericType(rfpItem.generic_product_category.category_id);
                for (int i = 0; i < genericProductTypeList.Count; i++)
                {
                    if (rfpItem.generic_product_type.product_id == genericProductTypeList[i].product_id)
                    {
                        typeComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericBrand(rfpItem.generic_product_category.category_id, rfpItem.generic_product_type.product_id);
                for (int i = 0; i < genericProductBrandsList.Count; i++)
                {
                    if (rfpItem.generic_product_brand.brand_id == genericProductBrandsList[i].brand_id)
                    {
                        brandComboBox.SelectedIndex = i;
                        break;
                    }
                }
                FillGenericModel(rfpItem.generic_product_category.category_id, rfpItem.generic_product_type.product_id, rfpItem.generic_product_brand.brand_id);
                for (int i = 0; i < genericProductModelList.Count; i++)
                {
                    if (rfpItem.generic_product_model.model_id == genericProductModelList[i].model_id)
                    {
                        modelTextBlock.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (rfpItem.item_description != null)
            {
                edit = true;
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
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                for (int i = 0; i < companyProductCategoryList.Count; i++)
                {
                    categoryCompanyComboBox.Items.Add(companyProductCategoryList[i].category);
                }
            }
        }
        private void FillCompanyProductType(int categoryId)
        {
            if (!commonQueries.GetCompanyProducts(ref companyProductList, categoryId))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                typeCompanyComboBox.Items.Clear();
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductList.Count; i++)
                {
                    typeCompanyComboBox.Items.Add(companyProductList[i].typeName);
                }
            }
        }
        private void FillCompanyProductBrand(int typeId)
        {
            if (!commonQueries.GetProductBrands(typeId, ref companyProductBrandList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                brandCompanyComboBox.Items.Clear();
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductBrandList.Count; i++)
                {
                    brandCompanyComboBox.Items.Add(companyProductBrandList[i].brandName);
                }
            }
        }
        private void FillCompanyProductModel(COMPANY_WORK_MACROS.PRODUCT_STRUCT product, COMPANY_WORK_MACROS.BRAND_STRUCT brand)
        {
            if (!commonQueries.GetCompanyModels(product, brand, ref companyProductModelList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                modelCompanyTextBlock.Items.Clear();
                for (int i = 0; i < companyProductModelList.Count; i++)
                {
                    modelCompanyTextBlock.Items.Add(companyProductModelList[i].modelName);
                }
            }
        }
        private void FillGenericCategory()
        {
            if (!commonQueries.GetGenericProductCategories(ref genericCategoryList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
        private void FillGenericType(int categoryId)
        {
            if (!commonQueries.GetGenericProducts(ref genericProductTypeList, categoryId))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
        private void FillGenericBrand(int categoryId, int typeId)
        {
            if (!commonQueries.GetGenericProductBrands(typeId, categoryId, ref genericProductBrandsList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
        private void FillGenericModel(int categoryId, int typeId, int brandId)
        {
            if (!commonQueries.GetGenericBrandModels(typeId, brandId, categoryId, ref genericProductModelList))
                System.Windows.Forms.MessageBox.Show(" Server connection failed! Please check your internet connection and try again.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                modelTextBlock.Items.Clear();
                for (int i = 0; i < genericProductModelList.Count; i++)
                {
                    modelTextBlock.Items.Add(genericProductModelList[i].model_name);
                }
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

            descriptionTextBox.IsEnabled = false;
        }
        private void DisableCompanyBoxes()
        {
            categoryCompanyComboBox.IsEnabled = false;
            typeCompanyComboBox.IsEnabled = false;
            brandCompanyComboBox.IsEnabled = false;
            modelCompanyTextBlock.IsEnabled = false;
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
                int categoryId = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                typeComboBox.IsEnabled = true;
                FillGenericType(categoryId);
            }
        }

        private void OnSelChangedTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (typeComboBox.SelectedIndex != -1)
            {
                brandComboBox.Items.Clear();
                modelTextBlock.Items.Clear();
                int categoryId = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                int typeId = genericProductTypeList[typeComboBox.SelectedIndex].product_id;
                brandComboBox.IsEnabled = true;
                FillGenericBrand(categoryId, typeId);
            }
        }

        private void OnSelChangedBrandComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (brandComboBox.SelectedIndex != -1)
            {
                modelTextBlock.Items.Clear();
                modelTextBlock.IsEnabled = true;
                int categoryId = genericCategoryList[categoryComboBox.SelectedIndex].category_id;
                int typeId = genericProductTypeList[typeComboBox.SelectedIndex].product_id;
                int brandId = genericProductBrandsList[brandComboBox.SelectedIndex].brand_id;
                FillGenericModel(categoryId, typeId, brandId);
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
                int companCategoryId = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex].categoryId;
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
                int companyTypeId = companyProductList[typeCompanyComboBox.SelectedIndex].typeId;
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
                    rfpItem.company_category = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex];
                    rfpItem.company_product = companyProductList[typeCompanyComboBox.SelectedIndex];
                    rfpItem.company_brand = companyProductBrandList[brandCompanyComboBox.SelectedIndex];
                    rfpItem.company_model = companyProductModelList[modelCompanyTextBlock.SelectedIndex];
                    rfpItem.item_description = companyProductCategoryList[categoryCompanyComboBox.SelectedIndex].category + " - " +
                                               companyProductList[typeCompanyComboBox.SelectedIndex].typeName + " - " +
                                               companyProductBrandList[brandCompanyComboBox.SelectedIndex].brandName + " - " +
                                               companyProductModelList[modelCompanyTextBlock.SelectedIndex].modelName;

                    rfpItem.generic_product_category.category_id = 0;
                    rfpItem.generic_product_category.category_name = string.Empty;
                    rfpItem.generic_product_type.product_id = 0;
                    rfpItem.generic_product_type.product_name = string.Empty;
                    rfpItem.generic_product_brand.brand_id = 0;
                    rfpItem.generic_product_brand.brand_name = string.Empty;
                    rfpItem.generic_product_model.model_id = 0;
                    rfpItem.generic_product_model.model_name = string.Empty;

                    if (!edit)
                        rfp.AddRFPItem(rfpItem);
                    else
                    {
                        rfp.rfpItems[index] = rfpItem;
                        view = true;
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
                    rfpItem.generic_product_category = genericCategoryList[categoryComboBox.SelectedIndex];
                    rfpItem.generic_product_type = genericProductTypeList[typeComboBox.SelectedIndex];
                    rfpItem.generic_product_brand = genericProductBrandsList[brandComboBox.SelectedIndex];
                    rfpItem.generic_product_model = genericProductModelList[modelTextBlock.SelectedIndex];
                    rfpItem.item_description = genericCategoryList[categoryComboBox.SelectedIndex].category_name + " - " +
                                               genericProductTypeList[typeComboBox.SelectedIndex].product_name + " - " +
                                               genericProductBrandsList[brandComboBox.SelectedIndex].brand_name + " - " +
                                               genericProductModelList[modelTextBlock.SelectedIndex].model_name;

                    rfpItem.company_category.categoryId = 0;
                    rfpItem.company_category.category = string.Empty;
                    rfpItem.company_product.typeId = 0;
                    rfpItem.company_product.typeName = string.Empty;
                    rfpItem.company_brand.brandId = 0;
                    rfpItem.company_brand.brandName = string.Empty;
                    rfpItem.company_model.modelId = 0;
                    rfpItem.company_model.modelName = string.Empty;
                    if (!edit)

                        rfp.AddRFPItem(rfpItem);

                    else
                    {
                        rfp.rfpItems[index] = rfpItem;
                        view = true;
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
                rfpItem.company_category.categoryId = 0;
                rfpItem.company_category.category = string.Empty;
                rfpItem.company_product.typeId = 0;
                rfpItem.company_product.typeName = string.Empty;
                rfpItem.company_brand.brandId = 0;
                rfpItem.company_brand.brandName = string.Empty;
                rfpItem.company_model.modelId = 0;
                rfpItem.company_model.modelName = string.Empty;
                rfpItem.item_description = descriptionTextBox.Text;

                rfpItem.generic_product_category.category_id = 0;
                rfpItem.generic_product_category.category_name = string.Empty;
                rfpItem.generic_product_type.product_id = 0;
                rfpItem.generic_product_type.product_name = string.Empty;
                rfpItem.generic_product_brand.brand_id = 0;
                rfpItem.generic_product_brand.brand_name = string.Empty;
                rfpItem.generic_product_model.model_id = 0;
                rfpItem.generic_product_model.model_name = string.Empty;
                if (!edit)

                    rfp.AddRFPItem(rfpItem);

                else
                    rfp.rfpItems[index] = rfpItem;
                this.Close();
            }
        }
    }
}

