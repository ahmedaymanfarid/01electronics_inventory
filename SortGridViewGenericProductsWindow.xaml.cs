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
    /// Interaction logic for SortGridViewGenericProductsWindow.xaml
    /// </summary>
    public partial class SortGridViewGenericProductsWindow : Window
    {
        PRODUCTS_STRUCTS.SORT_GENERIC_PRODUCTS sortGenericProductsStruct;
        Employee loggedInUser;
        GenericProductsPage genericProductsPage;
        public SortGridViewGenericProductsWindow(ref Employee loggedInUserr , ref PRODUCTS_STRUCTS.SORT_GENERIC_PRODUCTS sortGenericProducts , ref GenericProductsPage page)
        {
            InitializeComponent();
            loggedInUser = loggedInUserr;
            sortGenericProductsStruct = sortGenericProducts;
            genericProductsPage = page;
        }

        private void OnCheckCategory(object sender, RoutedEventArgs e)
        {
            if(categoryCheckBox.IsChecked==true)
            {
                typeCheckBox.IsChecked = false;
                brandCheckBox.IsChecked = false;
                modelCheckBox.IsChecked = false;
                pricingCriteriaCheckBox.IsChecked = false;
                sortGenericProductsStruct.category_name = true;
                sortGenericProductsStruct.brand = false;
                sortGenericProductsStruct.type = false;
                sortGenericProductsStruct.model = false;
                sortGenericProductsStruct.pricing_criteria = false;
            }
        }

        private void OnCheckType(object sender, RoutedEventArgs e)
        {
            if (typeCheckBox.IsChecked == true)
            {
                categoryCheckBox.IsChecked = false;
                brandCheckBox.IsChecked = false;
                modelCheckBox.IsChecked = false;
                pricingCriteriaCheckBox.IsChecked = false;

                sortGenericProductsStruct.category_name = false;
                sortGenericProductsStruct.brand = false;
                sortGenericProductsStruct.type = true;
                sortGenericProductsStruct.model = false;
                sortGenericProductsStruct.pricing_criteria = false;
            }
        }

        private void OnCheckBrand(object sender, RoutedEventArgs e)
        {
            if (brandCheckBox.IsChecked == true)
            {
                typeCheckBox.IsChecked = false;
                categoryCheckBox.IsChecked = false;
                modelCheckBox.IsChecked = false;
                pricingCriteriaCheckBox.IsChecked = false;

                sortGenericProductsStruct.category_name = false;
                sortGenericProductsStruct.brand = true;
                sortGenericProductsStruct.type = false;
                sortGenericProductsStruct.model = false;
                sortGenericProductsStruct.pricing_criteria = false;
            }
        }

        private void OnCheckModel(object sender, RoutedEventArgs e)
        {
            if (modelCheckBox.IsChecked == true)
            {
                typeCheckBox.IsChecked = false;
                brandCheckBox.IsChecked = false;
                categoryCheckBox.IsChecked = false;
                pricingCriteriaCheckBox.IsChecked = false;

                sortGenericProductsStruct.category_name = false;
                sortGenericProductsStruct.brand = false;
                sortGenericProductsStruct.type = false;
                sortGenericProductsStruct.model = true;
                sortGenericProductsStruct.pricing_criteria = false;
            }
        }

        private void OnCheckPricingCriteria(object sender, RoutedEventArgs e)
        {
            if (pricingCriteriaCheckBox.IsChecked == true)
            {
                typeCheckBox.IsChecked = false;
                brandCheckBox.IsChecked = false;
                modelCheckBox.IsChecked = false;
                categoryCheckBox.IsChecked = false;

                sortGenericProductsStruct.category_name = false ;
                sortGenericProductsStruct.brand = false;
                sortGenericProductsStruct.type = false;
                sortGenericProductsStruct.model = false;
                sortGenericProductsStruct.pricing_criteria = true;
            }
        }

        private void OnButtonClickSave(object sender, RoutedEventArgs e)
        {
          
            genericProductsPage.OnClose(ref sortGenericProductsStruct);
            this.Close();
        }
    }
}
