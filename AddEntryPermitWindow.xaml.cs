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
    /// Interaction logic for AddEntryPermitWindow.xaml
    /// </summary>
    public partial class AddEntryPermitWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        EntryPermitPage page = null;

        bool editable = false;
        public  AddEntryPermitPage EntryPermitPage;

        public AddEntryPermitWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, bool isedit, ref MaterialEntryPermit oldMaterialEntryPermit, EntryPermitPage mpage=null)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            editable = isedit;
            page=mpage;

            EntryPermitPage = new AddEntryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this, isedit, ref oldMaterialEntryPermit);


            frame.Content = EntryPermitPage;
        }


        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


        }
        private void OnAddEntryPermitWindowClosed(object sender, EventArgs e)
        {

            if (page != null)
                page.InitializeUiElements();
        }
    }
}
