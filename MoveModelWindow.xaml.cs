using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using _01electronics_library;
using _01electronics_windows_library;


namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for MoveModelWindow.xaml
    /// </summary>
    public partial class MoveModelWindow : Window
    {
        CommonQueries commonQueries;
        Model currentModell;
        List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT> categories=new List<PRODUCTS_STRUCTS.PRODUCT_CATEGORY_STRUCT>();
        List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT> products = new List<PRODUCTS_STRUCTS.PRODUCT_TYPE_STRUCT>();
        List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT> brands = new List<PRODUCTS_STRUCTS.PRODUCT_BRAND_STRUCT>();
        public MoveModelWindow(Model CurrentModel)
        {
            InitializeComponent();
            currentModell = CurrentModel;
            commonQueries = new CommonQueries();

            modelTextBox.Text = currentModell.GetModelName();
            modelTextBox.IsReadOnly = true;

            commonQueries.GetProductCategories(ref categories);

            categories.ForEach(a => CategoryCombo.Items.Add(a.category_name));

            ProductsCombo.IsEnabled = false;
            BrandComboBox.IsEnabled = false;
        }

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsCombo.Items.Clear();
            products.Clear();
            commonQueries.GetCompanyProducts(ref products, categories[CategoryCombo.SelectedIndex].category_id);
            products.ForEach(a => ProductsCombo.Items.Add(a.product_name));
            ProductsCombo.IsEnabled = true;
            ProductsCombo.SelectedItem = ProductsCombo.Items[0];
        }

        private void ProductsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsCombo.Items.Count == 0)
                return;
            brands.Clear();
            BrandComboBox.Items.Clear();
            commonQueries.GetProductBrands(products[ProductsCombo.SelectedIndex].type_id, ref brands);
            brands.ForEach(a => BrandComboBox.Items.Add(a.brand_name));
            BrandComboBox.IsEnabled = true;
            if (BrandComboBox.Items.Count == 0)
                return;
            BrandComboBox.SelectedItem = BrandComboBox.Items[0];
        }

        private void OnSaveChangesButtonClick(object sender, RoutedEventArgs e)
        {
            if (CategoryCombo.SelectedIndex == -1 || ProductsCombo.SelectedIndex == -1 || BrandComboBox.SelectedIndex == -1) {

                MessageBox.Show("You have to enter all of the data");
                return;
            }


          

            int productId = products[ProductsCombo.SelectedIndex].type_id;
            int brand_id = brands[BrandComboBox.SelectedIndex].brand_id;

            Model newModel = new Model();

            newModel.SetBrandID(brand_id);
            newModel.SetProductID(productId);
            newModel.SetModelName(currentModell.GetModelName());

            newModel.SetCategoryID(categories[CategoryCombo.SelectedIndex].category_id);

            newModel.SetModelsummaryPoints(currentModell.GetModelSummaryPoints());

            currentModell.GetModelSpecs().ForEach(a => newModel.SetModelSpecs(a));
            newModel.SetModelApplications(currentModell.GetModelApplications());

            newModel.SetModelBenefits(currentModell.GetModelBenefits());
            newModel.SetModelStandardFeatures(currentModell.GetModelStandardFeatures());

            //newModel.MoveModel(currentModell.GetProductID(),currentModell.GetBrandID(),currentModell.GetModelID(),currentModell.GetCategoryID());
            //currentModell.DeleteModel();

            newModel.GetNewModelID();
            currentModell.GetNewModelPhotoLocalPath();
            newModel.GetNewModelPhotoLocalPath();


            //SystemWatcher.fromSoftware = true;
            if (File.Exists(currentModell.GetModelPhotoLocalPath()))
            {
                if (!File.Exists(newModel.GetModelPhotoLocalPath())) {

                    File.Copy(currentModell.GetModelPhotoLocalPath(), newModel.GetModelPhotoLocalPath());
                    File.Delete(currentModell.GetModelPhotoLocalPath());
                }                
            }

            this.Close();


        }
    }
}
