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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static _01electronics_library.BASIC_STRUCTS;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for AddGenericProductWindow.xaml
    /// </summary>
    public partial class AddGenericProductWindow : System.Windows.Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY> categoryList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS> typeList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> productbrandList;
        List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND> brandList;

        List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> measureUnitList;
        List<BASIC_STRUCTS.PRICING_CRITERIA> pricingCriteria;

        GenericModel genericModel;
        System.Windows.Controls.ComboBox additionalInfo;

        public AddGenericProductWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            categoryList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_CATEGORY>();
            typeList= new List<BASIC_STRUCTS.GENERIC_PRODUCTS_PRODUCTS>();
            productbrandList= new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();
            brandList = new List<BASIC_STRUCTS.GENERIC_PRODUCTS_BRAND>();

            measureUnitList = new List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT>();
            pricingCriteria = new List<BASIC_STRUCTS.PRICING_CRITERIA>();
            
            genericModel = new GenericModel();

            integrityChecks = new IntegrityChecks(ref commonQueries);
            additionalInfo = new System.Windows.Controls.ComboBox();
            //EDIT STYLE NAME
            additionalInfo.Style = (System.Windows.Style)FindResource("comboBoxStyleCard2");
            additionalInfo.SelectionChanged += OnSelChangedAdditionalInfo;
            genericModel.SetAddedBy(loggedInUser.GetEmployeeId());
            InitializeComponent();
            DisableComboBoxes();
            FillCategoryComboBox();
        }
        private void DisableComboBoxes()
        {
            typeComboBox.IsEnabled= false;
            brandComboBox.IsEnabled= false;
            modelTextBlock.IsEnabled= false;
            itemUnitComboBox.IsEnabled= false;
            pricingCriteriaComboBox.IsEnabled= false;
            hasSerialNumberCheckBox.IsEnabled = false;
        }
       
        /// ////////////////////////////////////FILL COMBOBOXES///////////////////////////
   
        private void FillCategoryComboBox()
        {
            if(commonQueries.GetGenericProductCategories(ref categoryList))
            {
                for(int i=0;i<categoryList.Count;i++)
                {
                    categoryComboBox.Items.Add(categoryList[i].category_name);
                }
            }
          
        }
        private void FillProductTypeComboBox( int categoryID)
        {
            if(commonQueries.GetGenericProducts(ref typeList , categoryID))
            {
                for(int i=0;i< typeList.Count;i++)
                {
                    typeComboBox.Items.Add(typeList[i].product_name);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("ERROR");
            }
        }

        private void FillProductBrandComboBox(int categoryID , int productID)
        {
            if(commonQueries.GetGenericProductBrands(productID,categoryID,ref productbrandList))
            {
                for (int i = 0; i < productbrandList.Count; i++)
                {
                    brandComboBox.Items.Add(productbrandList[i].brand_name);
                }
                additionalInfo.Items.Clear();
                additionalInfo.Text = "<<MORE DATA>>";
                FillBrandComboBox(ref additionalInfo);
                brandComboBox.Items.Add(additionalInfo);
            }
        }
        private void FillBrandComboBox(ref System.Windows.Controls.ComboBox brandCombobox)
        {
            if (commonQueries.GetGenericBrands(ref brandList))
            {
                for (int i = 0; i < brandList.Count; i++)
                {
                    brandCombobox.Items.Add(brandList[i].brand_name);
                }
            }
        }

        private void FillItemUnitComboBox()
        {
            if (commonQueries.GetMeasureUnits(ref measureUnitList))
            {
                for (int i = 0; i < measureUnitList.Count; i++)
                {
                    itemUnitComboBox.Items.Add(measureUnitList[i].measure_unit);
                }
            }
        }
        private void FillPricingCriteriaComboBox()
        {
            if (commonQueries.GetPricingCriteria(ref pricingCriteria))
            {
                for (int i = 0; i < pricingCriteria.Count; i++)
                {
                    pricingCriteriaComboBox.Items.Add(pricingCriteria[i].pricing_criteria_name);
                }
            }
        }
        //////////////// ON SELECTION CHANGED //////////////////
        private void OnSelChangedCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
           // DisableComboBoxes();
           // typeComboBox.Items.Clear();
           // if (categoryComboBox.SelectedIndex != -1)
           // {
           //     typeComboBox.IsEnabled = true;
           //     typeComboBox.Items.Clear();
           //     FillProductTypeComboBox(categoryList[categoryComboBox.SelectedIndex].category_id);
           // }
           // else if (categoryComboBox.SelectedIndex == -1 && categoryComboBox.Text != string.Empty)
           // {
           //     typeComboBox.IsEnabled = true;
           //     typeComboBox.Items.Clear();
           // }


        }
        private void OnSelChangedAdditionalInfo(object sender, SelectionChangedEventArgs e)
        {
            brandComboBox.Text=additionalInfo.SelectedItem.ToString();
            brandComboBox.SelectedIndex = int.MaxValue;
            genericModel.SetBrandId(brandList[additionalInfo.SelectedIndex].brand_id);
            
        }
        private void OnSelChangedTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
           // brandComboBox.IsEnabled = false;
           // brandComboBox.Items.Clear();
           // brandComboBox.Text = string.Empty;
           // if (typeComboBox.SelectedIndex!=-1 )
           // {
           //     brandComboBox.IsEnabled = true;
           //     brandComboBox.Items.Clear();
           //     FillProductBrandComboBox(categoryList[categoryComboBox.SelectedIndex].category_id, typeList[typeComboBox.SelectedIndex].product_id);
           //   
           //    // FillBrandComboBox(ref additionalInfo);
           //    
           // }
           // else if (typeComboBox.SelectedIndex == -1 && typeComboBox.Text != string.Empty)
           // {
           //     brandComboBox.IsEnabled = true;
           //     brandComboBox.Items.Clear();
           // }


        }

        private void OnSelChangedBrandComboBox(object sender, SelectionChangedEventArgs e)
        {
           // modelTextBlock.IsEnabled = false;
           // modelTextBlock.Text = string.Empty;
           // itemUnitComboBox.Items.Clear();
           // pricingCriteriaComboBox.Items.Clear();
           // if (brandComboBox.SelectedIndex != -1)
           // {
           //     modelTextBlock.IsEnabled = true;
           //     itemUnitComboBox.IsEnabled = true;
           //     pricingCriteriaComboBox.IsEnabled = true; 
           //     hasSerialNumberCheckBox.IsEnabled = true;
           //
           //
           //    
           //     FillItemUnitComboBox();
           //     FillPricingCriteriaComboBox();
           // }
           // else if (brandComboBox.SelectedIndex == -1 && brandComboBox.Text != string.Empty)
           // {
           //     modelTextBlock.IsEnabled = true;
           //     itemUnitComboBox.IsEnabled = true;
           //     pricingCriteriaComboBox.IsEnabled = true;
           //     hasSerialNumberCheckBox.IsEnabled = true;
           //
           //
           //
           //     FillItemUnitComboBox();
           //     FillPricingCriteriaComboBox();
           // }


        }

        private void OnButtonClickSave(object sender, RoutedEventArgs e)
        {
            string errorMsg = string.Empty;
            if (!integrityChecks.CheckAcceptanceOfCharactersNotIntigers(categoryComboBox.Text, ref errorMsg))
            {
                categoryComboBox.Text = string.Empty;
                System.Windows.Forms.MessageBox.Show(errorMsg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                errorMsg = string.Empty;
                if (!integrityChecks.CheckAcceptanceOfCharactersNotIntigers(typeComboBox.Text, ref errorMsg))
                {
                    typeComboBox.Text = string.Empty;
                    System.Windows.Forms.MessageBox.Show(errorMsg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    errorMsg = string.Empty;
                    if (!integrityChecks.CheckAcceptanceOfCharactersNotIntigers(brandComboBox.Text, ref errorMsg))
                    {
                        brandComboBox.Text = string.Empty;
                        System.Windows.Forms.MessageBox.Show(errorMsg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    else
                    {
                        errorMsg = string.Empty;
                        if (!integrityChecks.CheckAcceptanceOfCharactersNotIntigers(modelTextBlock.Text, ref errorMsg))
                        {
                            modelTextBlock.Text = string.Empty;
                            System.Windows.Forms.MessageBox.Show(errorMsg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                        else
                        {
                            ///////////////////////////////////////////////////////////////////
                            ///IF THE CATEGORY COMBOBOX IS EMPTY OR ENTERED A NEW ONE ELSE THE CATEGORY ALREADY EXISTS ///////////
                            /////////////////////////////////////////////////////////////////

                            
                            if (categoryComboBox.SelectedIndex == -1)
                            {

                                if (categoryComboBox.Text != string.Empty)
                                {
                                   
                                    ///////////////////////////////////////////////////////////////////
                                    ///IF THE TYPE COMBOBOX IS EMPTY OR ENTERED A NEW ONE ELSE THE TYPE ALREADY EXISTS ///////////
                                    /////////////////////////////////////////////////////////////////
                                    if (typeComboBox.SelectedIndex == -1)
                                    {
                                        if (typeComboBox.Text != string.Empty)
                                        {

                                            ///////////////////////////////////////////////////////////////////
                                            ///IF THE BRAND COMBOBOX IS EMPTY OR ENTERED A NEW ONE ELSE THE BRAND ALREADY EXISTS ///////////
                                            /////////////////////////////////////////////////////////////////
                                            if (brandComboBox.SelectedIndex == -1 && additionalInfo.SelectedIndex == -1)
                                            {
                                                if (brandComboBox.Text != string.Empty)
                                                {

                                                    ///////////////////////////////////////////////////////////////////
                                                    ///BY DEFULT IF THE USER REACHED THE MODEL COMBOBOX SO HE MUST ENTER NEW ONE ///////////
                                                    /////////////////////////////////////////////////////////////////
                                                    if (modelTextBlock.Text != string.Empty)
                                                    {
                                                        genericModel.SetModelName(modelTextBlock.Text);
                                                        if (itemUnitComboBox.SelectedIndex != -1)
                                                        {
                                                            genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                            if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                            {
                                                                genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                                if (hasSerialNumberCheckBox.IsChecked == true)
                                                                {
                                                                    genericModel.SetModelHasSerialNumber(true);
                                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                                    if (!genericModel.IssuNewCategory())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        genericModel.SetProductName(typeComboBox.Text);
                                                                        if (!genericModel.IssuNewProduct())
                                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                        {
                                                                            genericModel.SetBrandName(brandComboBox.Text);
                                                                            if (!genericModel.IssuNewBrand())
                                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                            else
                                                                            {
                                                                                if (!genericModel.IssuNewModel())
                                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                                else
                                                                                {
                                                                                    this.Close();
                                                                                }
                                                                            }
                                                                        }
                                                                    }


                                                                }
                                                                else
                                                                {
                                                                    genericModel.SetModelHasSerialNumber(false);
                                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                                    if (!genericModel.IssuNewCategory())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        genericModel.SetProductName(typeComboBox.Text);
                                                                        if (!genericModel.IssuNewProduct())
                                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                        {
                                                                            genericModel.SetBrandName(brandComboBox.Text);
                                                                            if (!genericModel.IssuNewBrand())
                                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                            else
                                                                            {
                                                                                if (!genericModel.IssuNewModel())
                                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                                else
                                                                                {
                                                                                    this.Close();
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        }


                                                    }
                                                    else
                                                    {
                                                        genericModel.SetCategoryName(categoryComboBox.Text);
                                                        if (!genericModel.IssuNewCategory())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetProductName(typeComboBox.Text);
                                                            if (!genericModel.IssuNewProduct())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                                genericModel.SetBrandName(brandComboBox.Text);
                                                                if (!genericModel.IssuNewBrand())
                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                else
                                                                {
                                                                    this.Close();
                                                                }
                                                            }
                                                        }
                                                        //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }



                                                }
                                                else
                                                {
                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                    if (!genericModel.IssuNewCategory())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                    else
                                                    {
                                                        genericModel.SetProductName(typeComboBox.Text);
                                                        if (!genericModel.IssuNewProduct())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            this.Close();
                                                        }
                                                    }
                                                    //System.Windows.Forms.MessageBox.Show("Type must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }
                                            else
                                            {
                                                genericModel.SetBrandName(brandComboBox.Text);
                                                if (brandComboBox.SelectedIndex != -1)
                                                    genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);

                                                if (modelTextBlock.Text != string.Empty)
                                                {
                                                    genericModel.SetModelName(modelTextBlock.Text);

                                                    if (itemUnitComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                    if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                    if (hasSerialNumberCheckBox.IsChecked == true)
                                                    {
                                                        genericModel.SetModelHasSerialNumber(true);
                                                        genericModel.SetCategoryName(categoryComboBox.Text);
                                                        if (!genericModel.IssuNewCategory())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetProductName(typeComboBox.Text);
                                                            if (!genericModel.IssuNewProduct())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                                genericModel.SetBrandName(brandComboBox.Text);
                                                                if (!genericModel.IssuproductBrand())
                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                else
                                                                {
                                                                    if (!genericModel.IssuNewModel())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        this.Close();
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        genericModel.SetModelHasSerialNumber(false);
                                                        genericModel.SetCategoryName(categoryComboBox.Text);
                                                        if (!genericModel.IssuNewCategory())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetProductName(typeComboBox.Text);
                                                            if (!genericModel.IssuNewProduct())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                                genericModel.SetBrandName(brandComboBox.Text);
                                                                if (!genericModel.IssuproductBrand())
                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                else
                                                                {
                                                                    if (!genericModel.IssuNewModel())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        this.Close();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    


                                                }
                                                else
                                                {
                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                    if (!genericModel.IssuNewCategory())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                    else
                                                    {
                                                        genericModel.SetProductName(typeComboBox.Text);
                                                        if (!genericModel.IssuNewProduct())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                          
                                                            if (!genericModel.IssuproductBrand())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                                this.Close();
                                                            }
                                                        }


                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            genericModel.SetCategoryName(categoryComboBox.Text);
                                            if (!genericModel.IssuNewCategory())
                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                               this.Close();
                                            //System.Windows.Forms.MessageBox.Show("Type must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        genericModel.SetProductName(typeComboBox.Text);
                                        genericModel.SetProductId(typeList[typeComboBox.SelectedIndex].product_id);
                                        if (brandComboBox.SelectedIndex == -1 && additionalInfo.SelectedIndex == -1 )
                                        {
                                            if (brandComboBox.Text != string.Empty)
                                            {
                                               
                                                ///////////////////////////////////////////////////////////////////
                                                ///BY DEFULT IF THE USER REACHED THE MODEL COMBOBOX SO HE MUST ENTER NEW ONE ///////////
                                                /////////////////////////////////////////////////////////////////
                                                if (modelTextBlock.Text != string.Empty)
                                                {
                                                    genericModel.SetModelName(modelTextBlock.Text);
                                                    if (itemUnitComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                        if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                        {
                                                            genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                            {
                                                                genericModel.SetModelHasSerialNumber(true);

                                                                if (!genericModel.IssuNewModel())
                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                else
                                                                    this.Close();
                                                            }
                                                            else
                                                            {
                                                                genericModel.SetModelHasSerialNumber(false);
                                                                if (!genericModel.IssuNewModel())
                                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                else
                                                                    this.Close();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }





                                                }
                                                else
                                                {
                                                    genericModel.SetBrandName(brandComboBox.Text);
                                                    if (!genericModel.IssuNewBrand())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                    else
                                                        this.Close();
                                                    //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }



                                            }
                                            else
                                            {
                                                System.Windows.Forms.MessageBox.Show("Type must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            genericModel.SetBrandName(brandComboBox.Text);
                                            if (brandComboBox.SelectedIndex != -1)
                                                genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);
                                           
                                            if (modelTextBlock.Text != string.Empty)
                                            {
                                                genericModel.SetModelName(modelTextBlock.Text);
                                                if (itemUnitComboBox.SelectedIndex != -1)
                                                {
                                                    genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                    if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                        if (hasSerialNumberCheckBox.IsChecked == true)
                                                        {
                                                            genericModel.SetModelHasSerialNumber(true);
                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                        }
                                                        else
                                                        {
                                                            genericModel.SetModelHasSerialNumber(false);
                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
                                                else
                                                {
                                                    System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }





                                            }
                                            else
                                            {
                                                if (!genericModel.IssuproductBrand())
                                                    System.Windows.Forms.MessageBox.Show("Already Exists", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                else
                                                    this.Close();
                                                //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }

                                        }



                                    }

                                }
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("Category must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                genericModel.SetCategoryName(categoryComboBox.Text);
                                genericModel.SetCategoryId(categoryList[categoryComboBox.SelectedIndex].category_id);

                                if (typeComboBox.SelectedIndex == -1)
                                {
                                    if (typeComboBox.Text != string.Empty)
                                    {

                                        ///////////////////////////////////////////////////////////////////
                                        ///IF THE BRAND COMBOBOX IS EMPTY OR ENTERED A NEW ONE ELSE THE BRAND ALREADY EXISTS ///////////
                                        /////////////////////////////////////////////////////////////////
                                        if (brandComboBox.SelectedIndex == -1 && additionalInfo.SelectedIndex == -1 )
                                        {
                                            if (brandComboBox.Text != string.Empty)
                                            {

                                                ///////////////////////////////////////////////////////////////////
                                                ///BY DEFULT IF THE USER REACHED THE MODEL COMBOBOX SO HE MUST ENTER NEW ONE ///////////
                                                /////////////////////////////////////////////////////////////////
                                                if (modelTextBlock.Text != string.Empty)
                                                {
                                                    genericModel.SetModelName(modelTextBlock.Text);
                                                    if (itemUnitComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                        if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                        {
                                                            genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                            if (hasSerialNumberCheckBox.IsChecked == true)
                                                            {
                                                                genericModel.SetModelHasSerialNumber(true);
                                                               
                                                                    genericModel.SetProductName(typeComboBox.Text);
                                                                    if (!genericModel.IssuNewProduct())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        genericModel.SetBrandName(brandComboBox.Text);
                                                                        if (!genericModel.IssuNewBrand())
                                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                        {
                                                                            if (!genericModel.IssuNewModel())
                                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                            this.Close();
                                                                        }
                                                                    }
                                                                


                                                            }
                                                            else
                                                            {
                                                                genericModel.SetModelHasSerialNumber(false);
                                                               
                                                                    genericModel.SetProductName(typeComboBox.Text);
                                                                    if (!genericModel.IssuNewProduct())
                                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                    else
                                                                    {
                                                                        genericModel.SetBrandName(brandComboBox.Text);
                                                                        if (!genericModel.IssuNewBrand())
                                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                        {
                                                                            if (!genericModel.IssuNewModel())
                                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                                        else
                                                                            this.Close();
                                                                        }
                                                                   }
                                                                
                                                            }
                                                        }
                                                        else
                                                        {
                                                            System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }


                                                }
                                                else
                                                {
                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                   
                                                        genericModel.SetProductName(typeComboBox.Text);
                                                        if (!genericModel.IssuNewProduct())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetBrandName(brandComboBox.Text);
                                                            if (!genericModel.IssuNewBrand())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                            this.Close();
                                                        }
                                                    
                                                    //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }



                                            }
                                            else
                                            {
                                                genericModel.SetCategoryName(categoryComboBox.Text);
                                               
                                                    genericModel.SetProductName(typeComboBox.Text);
                                                    if (!genericModel.IssuNewProduct())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                else
                                                    this.Close();
                                                
                                                //System.Windows.Forms.MessageBox.Show("Type must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            genericModel.SetBrandName(brandComboBox.Text);
                                            if (brandComboBox.SelectedIndex != -1)
                                                genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);

                                            if (modelTextBlock.Text != string.Empty)
                                            {
                                                genericModel.SetModelName(modelTextBlock.Text);

                                                if (itemUnitComboBox.SelectedIndex != -1)
                                                {
                                                    genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                }
                                                else
                                                {
                                                    System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                                if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                {
                                                    genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                }
                                                else
                                                {
                                                    System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                                if (hasSerialNumberCheckBox.IsChecked == true)
                                                {
                                                    genericModel.SetModelHasSerialNumber(true);
                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                    
                                                        genericModel.SetProductName(typeComboBox.Text);
                                                        if (!genericModel.IssuNewProduct())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetBrandName(brandComboBox.Text);
                                                            if (!genericModel.IssuproductBrand())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                            }
                                                        }
                                                    
                                                }
                                                else
                                                {
                                                    genericModel.SetModelHasSerialNumber(false);
                                                    genericModel.SetCategoryName(categoryComboBox.Text);
                                                  
                                                        genericModel.SetProductName(typeComboBox.Text);
                                                        if (!genericModel.IssuNewProduct())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                        {
                                                            genericModel.SetBrandName(brandComboBox.Text);
                                                            if (!genericModel.IssuproductBrand())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                            {
                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                            }
                                                        }
                                                    
                                                }
                                               

                                            }
                                            else
                                            {
                                                genericModel.SetCategoryName(categoryComboBox.Text);
                                               
                                                    genericModel.SetProductName(typeComboBox.Text);
                                                    if (!genericModel.IssuNewProduct())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                    else
                                                    {

                                                    if (!genericModel.IssuproductBrand())
                                                        System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                    else
                                                        this.Close();
                                                    }


                                                
                                            }
                                        }
                                    }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Category Already Exists.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        // genericModel.SetCategoryName(categoryComboBox.Text);
                                        // if (!genericModel.IssuNewCategory())
                                        //     System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                        //System.Windows.Forms.MessageBox.Show("Type must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    genericModel.SetProductName(typeComboBox.Text);
                                    genericModel.SetProductId(typeList[typeComboBox.SelectedIndex].product_id);
                                    if (brandComboBox.SelectedIndex == -1 && additionalInfo.SelectedIndex==-1)
                                    {
                                        if (brandComboBox.Text != string.Empty)
                                        {

                                            ///////////////////////////////////////////////////////////////////
                                            ///BY DEFULT IF THE USER REACHED THE MODEL COMBOBOX SO HE MUST ENTER NEW ONE ///////////
                                            /////////////////////////////////////////////////////////////////
                                            if (modelTextBlock.Text != string.Empty)
                                            {
                                                genericModel.SetModelName(modelTextBlock.Text);
                                                if (itemUnitComboBox.SelectedIndex != -1)
                                                {
                                                    genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                    if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                    {
                                                        genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                        if (hasSerialNumberCheckBox.IsChecked == true)
                                                        {
                                                            genericModel.SetModelHasSerialNumber(true);

                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                        }
                                                        else
                                                        {
                                                            genericModel.SetModelHasSerialNumber(false);
                                                            if (!genericModel.IssuNewModel())
                                                                System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                            else
                                                                this.Close();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
                                                else
                                                {
                                                    System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }





                                            }
                                            else
                                            {
                                                genericModel.SetBrandName(brandComboBox.Text);
                                                if (!genericModel.IssuNewBrand())
                                                    System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                else
                                                    this.Close();
                                                //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }



                                        }
                                        else
                                        {
                                            System.Windows.Forms.MessageBox.Show("Type Already Exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        genericModel.SetBrandName(brandComboBox.Text);
                                        if(brandComboBox.SelectedIndex!=-1)
                                        genericModel.SetBrandId(brandList[brandComboBox.SelectedIndex].brand_id);

                                        if (modelTextBlock.Text != string.Empty)
                                        {
                                            genericModel.SetModelName(modelTextBlock.Text);
                                            if (itemUnitComboBox.SelectedIndex != -1)
                                            {
                                                genericModel.SetModelitemUnit(measureUnitList[itemUnitComboBox.SelectedIndex].measure_unit_id);
                                                if (pricingCriteriaComboBox.SelectedIndex != -1)
                                                {
                                                    genericModel.SetModelpricingCriteria(pricingCriteria[pricingCriteriaComboBox.SelectedIndex].pricing_criteria_id);
                                                    if (hasSerialNumberCheckBox.IsChecked == true)
                                                    {
                                                        genericModel.SetModelHasSerialNumber(true);
                                                        if (!genericModel.IssuNewModel())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                            this.Close();
                                                    }
                                                    else
                                                    {
                                                        genericModel.SetModelHasSerialNumber(false);
                                                        if (!genericModel.IssuNewModel())
                                                            System.Windows.Forms.MessageBox.Show("Server connection failed! Please check your internet connection and try again", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                                        else
                                                            this.Close();
                                                    }
                                                }
                                                else
                                                {
                                                    System.Windows.Forms.MessageBox.Show("Pricing Criteria must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }
                                            else
                                            {
                                                System.Windows.Forms.MessageBox.Show("Item Unit must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }





                                        }
                                        else
                                        {
                                            if (!genericModel.IssuproductBrand())
                                                System.Windows.Forms.MessageBox.Show("Already Exists", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                            else
                                                this.Close();
                                            //System.Windows.Forms.MessageBox.Show("Model must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }

                                    }



                                }


                            }
                        }
                    }
                }
            }

          
           
        }

        private void OnButtonClickCategoryComboBox(object sender, SelectionChangedEventArgs e)
        {
          
        }
        private void OnTextChangedCategoryComboBox(object sender, RoutedEventArgs e)
        {
          
        }

        private void OnButtonClickTypeComboBox(object sender, TextChangedEventArgs e)
        {
           
        }

        private void OnButtonClickbrandComboBox(object sender, TextChangedEventArgs e)
        {
           
        }

        private void OnMouseLeaveCategoryComboBox(object sender, System.Windows.Input.MouseEventArgs e)
        {
           

                if (categoryComboBox.SelectedIndex != -1)
                {
                   
                        typeComboBox.IsEnabled = true;
                        typeComboBox.Items.Clear();
                        brandComboBox.Items.Clear();
                        brandComboBox.Text = string.Empty;
                        FillProductTypeComboBox(categoryList[categoryComboBox.SelectedIndex].category_id);
                    
                   
                }
                else if (categoryComboBox.SelectedIndex == -1 && categoryComboBox.Text != string.Empty)
                {
                    if (typeComboBox.IsEnabled == false)
                    {
                        typeComboBox.IsEnabled = true;
                        typeComboBox.Items.Clear();
                        brandComboBox.Items.Clear();
                        brandComboBox.Text = string.Empty;
                    }
               
                }
           
        }

        private void OnMouseLeaveTypeComboBox(object sender, System.Windows.Input.MouseEventArgs e)
        {
            brandComboBox.IsEnabled = false;
            brandComboBox.Items.Clear();
            brandComboBox.Text = string.Empty;
            if (typeComboBox.SelectedIndex != -1)
            {
                brandComboBox.IsEnabled = true;
                brandComboBox.Items.Clear();
                FillProductBrandComboBox(categoryList[categoryComboBox.SelectedIndex].category_id, typeList[typeComboBox.SelectedIndex].product_id);

            }
            else if (typeComboBox.SelectedIndex == -1 && typeComboBox.Text != string.Empty)
            {
                
                brandComboBox.IsEnabled = true;
                brandComboBox.Items.Clear();
                FillBrandComboBox(ref brandComboBox);
            }
        }

        private void OnMouseLeaveBrandComboBox(object sender, System.Windows.Input.MouseEventArgs e)
        {
            modelTextBlock.IsEnabled = true;
            modelTextBlock.Text = string.Empty;
            itemUnitComboBox.Items.Clear();
            pricingCriteriaComboBox.Items.Clear();
            hasSerialNumberCheckBox.IsChecked = false;
            if (brandComboBox.SelectedIndex != -1)
            {
                modelTextBlock.IsEnabled = true;
                itemUnitComboBox.IsEnabled = true;
                pricingCriteriaComboBox.IsEnabled = true;
                hasSerialNumberCheckBox.IsEnabled = true;



                FillItemUnitComboBox();
                FillPricingCriteriaComboBox();
            }
            else if (brandComboBox.SelectedIndex == -1 && brandComboBox.Text != string.Empty)
            {
                modelTextBlock.IsEnabled = true;
                itemUnitComboBox.IsEnabled = true;
                pricingCriteriaComboBox.IsEnabled = true;
                hasSerialNumberCheckBox.IsEnabled = true;



                FillItemUnitComboBox();
                FillPricingCriteriaComboBox();
            }
        }
    }
}
