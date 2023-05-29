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
    /// Interaction logic for RFPItemSerialsWindow.xaml
    /// </summary>
    public partial class RFPItemSerialsWindow : Window
    {
        RFP rfp;
        Stock stock;
        PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT rfpItem;
        decimal quantityReserved;
        List<string> selectedSerials;
        List<List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>> stockAvailableItemsListOfList;
        Employee loggedUser;
        bool edit;
        public RFPItemSerialsWindow(ref RFP mRfp, ref Stock mStock, ref List<List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>> mStockAvailableItemsListofList, ref PROCUREMENT_STRUCTS.RFP_ITEM_MAX_STRUCT mrfpItem, ref List<string> selectedSerials, ref int neededQuantity, ref Employee loggedInUser, ref bool Edit)
        {
            InitializeComponent();
            rfp = mRfp;
            stock = mStock;
            rfpItem = mrfpItem;
            this.selectedSerials = selectedSerials;
            quantityReserved = rfpItem.item_quantity;
            stockAvailableItemsListOfList = mStockAvailableItemsListofList;
            loggedUser = loggedInUser;
            edit = Edit;
            CheckCompanyOrGeneric();
        }
        public void CheckCompanyOrGeneric()
        {
            stock.GetStockAvailabiltyList().Clear();

            if (rfpItem.is_company_product)
            {
                if (!stock.GetAvailableCompanySerials(rfpItem.product_category.category_id, rfpItem.product_type.type_id, rfpItem.product_brand.brand_id, rfpItem.product_model.model_id, rfpItem.product_specs.spec_id))
                    return;

                CheckIfItemAlreadyExists();

                InitializeItemDescription(true);
                InitializeSerialsStackPanel();
            }
            else
            {

                CheckIfItemAlreadyExists();

                InitializeItemDescription(false);
                InitializeSerialsStackPanel();
            }

        }
        private void CheckIfItemAlreadyExists()
        {
            if (stockAvailableItemsListOfList.Count != 0)
            {
                for (int i = 0; i < stockAvailableItemsListOfList.Count; i++)
                {
                    List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT> reservedList = stockAvailableItemsListOfList[i];
                    INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservedExistingItem = reservedList.Find(f => f.rfp_item_description == rfpItem.item_description);
                    if (reservedExistingItem.rfp_item_no != 0)
                    {
                        for (int j = 0; j < reservedList.Count; j++)
                        {
                            int index = stock.GetStockAvailabiltyList().FindIndex(f => f.entry_permit_item.product_serial_number == reservedList[j].serial_number);
                            string selectedSerialNumber = selectedSerials.Find(f => f == reservedList[j].serial_number);
                            if (selectedSerialNumber == null)
                            {
                                stock.GetStockAvailabiltyList().RemoveAt(index);
                            }



                        }
                    }
                }
            }
        }
        public void InitializeSerialsStackPanel()
        {
            for (int i = 0; i < stock.GetStockAvailabiltyList().Count; i++)
            {
                CheckBox serialCheckBox = new CheckBox();
                serialCheckBox.Content = stock.GetStockAvailabiltyList()[i].entry_permit_item.product_serial_number;
                serialCheckBox.Style = (Style)FindResource("checkBoxStyle");
                serialCheckBox.Tag = i;
                serialsStackPanel.Children.Add(serialCheckBox);
                serialCheckBox.Checked += OnCheckSerialCheckBox;
                serialCheckBox.Unchecked += UnCheckSerialCheckBox;
                if (selectedSerials.Count != 0)
                {
                    string serial = selectedSerials.Find(f => f == stock.GetStockAvailabiltyList()[i].entry_permit_item.product_serial_number);
                    int index = selectedSerials.IndexOf(serial);
                    if (serial != null)
                    {
                        selectedSerials.RemoveAt(index);
                        selectedSerials.Remove("");
                        //quantityReserved++;
                        serialCheckBox.IsChecked = true;

                    }
                }
            }

        }
        private void InitializeItemDescription(bool isCompany)
        {
            if (isCompany)
            {
                categoryComboBox.Items.Add(rfpItem.product_category.category_name);
                categoryComboBox.SelectedIndex = 0;

                typeComboBox.Items.Add(rfpItem.product_type.product_name);
                typeComboBox.SelectedIndex = 0;

                brandComboBox.Items.Add(rfpItem.product_brand.brand_name);
                brandComboBox.SelectedIndex = 0;

                modelTextBlock.Items.Add(rfpItem.product_model.model_name);
                modelTextBlock.SelectedIndex = 0;

                specsContainer.Visibility = Visibility.Visible;
                specsTextBlock.Items.Add(rfpItem.product_specs.spec_name);
                specsTextBlock.SelectedIndex = 0;
            }
            else
            {
                categoryComboBox.Items.Add(rfpItem.product_category.category_name);
                categoryComboBox.SelectedIndex = 0;

                typeComboBox.Items.Add(rfpItem.product_type.product_name);
                typeComboBox.SelectedIndex = 0;

                brandComboBox.Items.Add(rfpItem.product_brand.brand_name);
                brandComboBox.SelectedIndex = 0;

                modelTextBlock.Items.Add(rfpItem.product_model.model_name);
                modelTextBlock.SelectedIndex = 0;

                specsContainer.Visibility = Visibility.Collapsed;
            }
        }
        private void UnCheckSerialCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox serialCheckBox = (CheckBox)sender;
            string serial = selectedSerials.Find(f => f == serialCheckBox.Content.ToString());
            int index = selectedSerials.IndexOf(serial);
            if (serial != null)
                selectedSerials.RemoveAt(index);
            quantityReserved++;
        }

        private void OnCheckSerialCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox serialCheckBox = (CheckBox)sender;
            if (quantityReserved == 0)
            {
                System.Windows.Forms.MessageBox.Show("Can't exceed the needed quantity", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                serialCheckBox.IsChecked = false;
                quantityReserved--;
            }
            else
            {
                selectedSerials.Add(serialCheckBox.Content.ToString());
                quantityReserved--;
            }
        }

        private void OnButtonClickSave(object sender, RoutedEventArgs e)
        {
            List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT> reservedSerials = new List<INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT>();
            if (edit)
            {

            }
            for (int i = 0; i < selectedSerials.Count; i++)
            {
                INVENTORY_STRUCTS.STOCK_AVAILABILITY stockAvailabilityItem = stock.GetStockAvailabiltyList().Find(f => f.entry_permit_item.product_serial_number == selectedSerials[i]);
                INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT reservedserial = new INVENTORY_STRUCTS.MATERIAL_RESERVATION_MED_STRUCT();
                if (stockAvailabilityItem.entry_permit_item.entry_permit_serial != 0)
                {
                    reservedserial.entry_permit_serial = stockAvailabilityItem.entry_permit_item.entry_permit_serial;
                    reservedserial.entry_permit_item_serial = stockAvailabilityItem.entry_permit_item.entry_permit_item_serial;
                    reservedserial.rfp_requestor_team = rfp.GetRFPRequestorTeamId();
                    reservedserial.rfp_serial = rfp.GetRFPSerial();
                    reservedserial.rfp_version = rfp.GetRFPVersion();
                    reservedserial.rfp_item_no = rfpItem.rfp_item_number;
                    reservedserial.rfp_item_description = rfpItem.item_description;
                    reservedserial.rfp_description = rfp.GetRFPNotes();
                    reservedserial.rfp_item_notes = rfpItem.item_notes;
                    reservedserial.rfp_measure_unit.unit_id = rfpItem.measure_unit_id;
                    reservedserial.rfp_measure_unit.measure_unit = rfpItem.measure_unit;
                    reservedserial.quantity = Decimal.ToInt32(rfpItem.item_quantity);
                    reservedserial.serial_number = stockAvailabilityItem.entry_permit_item.product_serial_number;
                    reservedserial.reserved_by_id = loggedUser.GetEmployeeId();
                    reservedserial.reservation_date = DateTime.Now;
                    reservedserial.hold_until = DateTime.Now;
                    reservedSerials.Add(reservedserial);
                }

            }
            if (edit)
            {
                stockAvailableItemsListOfList[rfpItem.rfp_item_number - 1] = reservedSerials;

            }
            else
            {
                stockAvailableItemsListOfList.Add(reservedSerials);
            }

            this.Close();
        }
    }
}
