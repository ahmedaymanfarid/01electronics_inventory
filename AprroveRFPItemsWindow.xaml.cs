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
    /// Interaction logic for AprroveRFPItemsWindow.xaml
    /// </summary>
    public partial class AprroveRFPItemsWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        private RFP rfp;

        public AprroveRFPItemsWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref RFP mRFP)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            rfp = mRFP;

            InitializeComponent();

            InitializeItemsStackPanel();
        }

        private void InitializeItemsStackPanel()
        {
            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
            {
                for (int i = 0; i < rfp.rfpItems.Count(); i++)
                {
                    //if (rfp.rfpItems[i].item_availablilty_status.status_id == COMPANY_WORK_MACROS.RFP_ITEM_PENDING_STOCK_RECEIVAL)
                    //{
                        CheckBox checkBox = new CheckBox();
                        checkBox.Style = (Style)FindResource("miniCheckBoxStyle");
                        checkBox.Width = 500.00;
                        checkBox.Content = rfp.rfpItems[i].item_description;
                        checkBox.Tag = i;
                        checkBox.VerticalAlignment = VerticalAlignment.Center;
                        checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                        itemsStackPanel.Children.Add(checkBox);

                    //}
                }
            }
            else
            {
                for (int i = 0; i < rfp.rfpItems.Count(); i++)
                {
                    //if (rfp.rfpItems[i].item_availablilty_status.status_id == COMPANY_WORK_MACROS.RFP_ITEM_PENDING_SITE_RECIEVAL)
                    //{
                        CheckBox checkBox = new CheckBox();
                        checkBox.Style = (Style)FindResource("miniCheckBoxStyle");
                        checkBox.Width = 500.00;
                        checkBox.Content = rfp.rfpItems[i].item_description;
                        checkBox.Tag = i;

                        itemsStackPanel.Children.Add(checkBox);
                    //}
                }
            }
        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.INVENTORY_TEAM_ID)
            {
                for (int i = 0; i < itemsStackPanel.Children.Count; i++)
                {
                    CheckBox currentCheckBox = (CheckBox)itemsStackPanel.Children[i];
                    if (currentCheckBox.IsChecked == true)
                    {
                       // int itemAvailabilityStatus = COMPANY_WORK_MACROS.RFP_ITEM_STOCK_RECEIVED;
                       // int itemStatus = COMPANY_WORK_MACROS.RFP_ITEM_AT_STOCK;
                       // int approvedLevel = COMPANY_WORK_MACROS.RFP_ITEM_APPROVED_INVENTORY;
                       //
                       //
                       //
                       // if (!rfp.ApproveRFPItem(itemStatus, itemAvailabilityStatus, rfp.rfpItems[int.Parse(currentCheckBox.Tag.ToString())].item_number, approvedLevel, loggedInUser.GetEmployeeId()))
                       //     return;
                        

                    }
                }
            }
            else
            {
                for (int i = 0; i < itemsStackPanel.Children.Count; i++)
                {
                    CheckBox currentCheckBox = (CheckBox)itemsStackPanel.Children[i];
                    if (currentCheckBox.IsChecked == true)
                    {
                       // int itemAvailabilityStatus = COMPANY_WORK_MACROS.RFP_ITEM_SITE_RECEIVED;
                       // int itemStatus = COMPANY_WORK_MACROS.RFP_ITEM_AT_SITE;
                       // int approvedLevel = COMPANY_WORK_MACROS.RFP_ITEM_APPROVED_SITE_ENGINEER;
                       //
                       // if (!rfp.ApproveRFPItem(itemStatus, itemAvailabilityStatus, rfp.rfpItems[int.Parse(currentCheckBox.Tag.ToString())].item_number, approvedLevel, loggedInUser.GetEmployeeId()))
                       //     return;
                    }
                }
            }

            this.Close();
        }
    }
}
