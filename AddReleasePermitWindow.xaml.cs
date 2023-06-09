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
    /// Interaction logic for AddReleasePermitWindow.xaml
    /// </summary>
    public partial class AddReleasePermitWindow : Window
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        public bool isView;

        public delegate void func(int serial);

        public func func1;

        public AddReleasePermitPage releasePermitPage;
        public MaterialReleasePermits materialReleasePermit;
        public AddReleasePermitItemPage releasePermitItemPage;
        public bool serviceReport;
        public bool rfp;

        public WorkOrder workOrder;
        public RFP rfps;

        public AddReleasePermitWindow(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, MaterialReleasePermits mMaterialReleasePermit=null, bool mIsView=false,func function=null)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries; 
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            serviceReport = false;
            rfp = false;
            InitializeComponent();
            isView=mIsView;
            workOrder = new WorkOrder();
            rfps = new RFP();
            func1=function;
            
            releasePermitPage = new AddReleasePermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, this);
            releasePermitItemPage = new AddReleasePermitItemPage(ref  commonQueries, ref commonFunctions,ref integrityChecks,ref loggedInUser, this);
            if (isView == true)
                materialReleasePermit = mMaterialReleasePermit;


            frame.Content = releasePermitPage;

        }
    }
}
