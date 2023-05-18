﻿using _01electronics_library;
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
    /// Interaction logic for RentryPermitWindow.xaml
    /// </summary>
    public partial class RentryPermitWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public bool isView;

        public AddRentryItemPage addRentryPermitPage;

        public RentryPermitWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, bool mIsView=false)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            isView = mIsView;

            InitializeComponent();

            addRentryPermitPage = new AddRentryItemPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this);


            frame.Content = addRentryPermitPage;
        }
    }
}
