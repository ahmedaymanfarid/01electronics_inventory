using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ModelUploadFilesPage.xaml
    /// </summary>
    public partial class ModelUploadFilesPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected Model product;

        protected FTPServer ftpObject;

        protected int counter;
        protected int viewAddCondition;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        WrapPanel wrapPanel = new WrapPanel();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Grid overwriteFileGrid;
        private Label currentLabel;

        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelAdditionalInfoPage modelAdditionalInfoPage;

        protected String errorMessage;

        List<string> ftpFiles;

        ProgressBar progressBar = new ProgressBar();
        Grid UploadIconGrid = new Grid();

        protected bool fileUploaded;
        protected bool fileDownloaded;
        protected bool uploadThisFile = false;
        protected bool checkFileInServer = false;
        protected bool canEdit = false;
        protected bool productNameEdited = false;
        protected bool productSummaryPointsEdited = false;
        protected bool productPhotoEdited = false;

        /// <summary>
        /// MOVE TO XML
        /// </summary>

        public ModelUploadFilesPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref Model mProduct, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            ftpObject = new FTPServer();

            product = mProduct;

            counter = 0;
            canEdit = false;
            viewAddCondition = mViewAddCondition;

            InitializeComponent();

            ftpFiles = new List<string>(); progressBar.Style = (Style)FindResource("ProgressBarStyle");
            progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            progressBar.Width = 200;


            uploadFilesStackPanel.Children.Clear();


            serverFolderPath = product.GetProductFolderServerPath();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                try
                {
                    uploadFilesStackPanel.Children.Clear();
                    uploadFilesStackPanel.Children.Add(wrapPanel);
                    String[] tmp = Directory.GetFiles(product.GetModelFolderLocalPath());
                    String localFile = "";
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i].Contains(".pdf"))
                        {
                            localFile = tmp[i];
                        }
                    }
                    localFileName = System.IO.Path.GetFileName(localFile);
                    InsertIconGrid(localFile);
                    currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];

                    if (tmp.Length == 0)
                    {
                        uploadFilesStackPanel.Children.Clear();
                        InsertDragAndDropOrBrowseGrid();

                    }
                }
                catch (Exception ex)
                {
                    uploadFilesStackPanel.Children.Clear();

                    InsertDragAndDropOrBrowseGrid();
                }
            }
            else
            {
                InsertDragAndDropOrBrowseGrid();
            }
           
        }


        ////////////////////////////////////////////////////////////////////
        ///  
        ///////////////////////////////////////////////////////////////////

        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {
            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] temp = (string[])e.Data.GetData(DataFormats.FileDrop);

                e.Effects = DragDropEffects.All;

                for (int i = 0; i < temp.Count(); i++)
                {
                    localFolderPath = temp[i];
                    localFileName = System.IO.Path.GetFileName(localFolderPath);
                    serverFileName = (String)product.GetProductID().ToString() + ".jpg";

                    CheckIfFileAlreadyUploaded(localFileName);

                    if (uploadThisFile == true && checkFileInServer == false)
                    {

                        if (wrapPanel.Children.Count != 0)
                            wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

                        progressBar.Visibility = Visibility.Visible;

                        ftpFiles.Add(localFileName);

                        InsertIconGrid(localFolderPath);

                        currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
                        //currentSelectedFile.Children.Add(progressBar);
                        //Grid.SetRow(progressBar, 3);

                        //uploadBackground.RunWorkerAsync();

                        //while (uploadBackground.IsBusy)
                        //{
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        uploadThisFile = false;
                    }
                    else if (uploadThisFile == true)
                    {
                        //uploadBackground.RunWorkerAsync();

                        //while (uploadBackground.IsBusy)
                        //{
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        uploadThisFile = false;

                        //SystemWatcher.fromSoftware = true;
                        File.Copy(localFolderPath, product.GetProductPhotoLocalPath());
                    }
                }
            }
        }

        private void InsertIconGrid(string localFolderPath)
        {
            UploadIconGrid = new Grid();
            UploadIconGrid.Margin = new Thickness(24);
            //UploadIconGrid.Width = 250;
            UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            UploadIconGrid.RowDefinitions.Add(row1);
            UploadIconGrid.RowDefinitions.Add(row2);

            Image icon = new Image();

            LoadIcon(ref icon, localFolderPath);

            resizeImage(ref icon, 350, 150);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);
            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            icon.VerticalAlignment = VerticalAlignment.Stretch;
            Label name = new Label();
            name.Content = localFileName;
            name.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(name);
            Grid.SetRow(name, 1);
            wrapPanel.Children.Add(UploadIconGrid);
        }
        private void LoadIcon(ref Image icon, string ftpFiles)
        {
            if (ftpFiles.Contains(".pdf"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\pdf_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".doc") || ftpFiles.Contains(".docs") || ftpFiles.Contains(".docx"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\word_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".txt") || ftpFiles.Contains(".rtf"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\text_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".xls") || ftpFiles.Contains(".xlsx") || ftpFiles.Contains(".csv"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\excel_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".jpg") || ftpFiles.Contains(".png") || ftpFiles.Contains(".raw") || ftpFiles.Contains(".jpeg") || ftpFiles.Contains(".gif"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\image_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".rar") || ftpFiles.Contains(".zip") || ftpFiles.Contains(".gzip"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\winrar_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".ppt") || ftpFiles.Contains(".pptx") || ftpFiles.Contains(".pptm"))
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\powerpoint_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles != ".." || ftpFiles != ".")
                icon = new Image { Source = new BitmapImage(new Uri(@"Icons\unknown_icon.jpg", UriKind.Relative)) };
        }
        private void CheckIfFileAlreadyUploaded(string fileName)
        {

            if (ftpFiles.Count == 0)
                uploadThisFile = true;

            else
            {
                for (int i = 0; i < ftpFiles.Count(); i++)
                {
                    if (ftpFiles[i] == fileName)
                    {
                        var result = System.Windows.MessageBox.Show("This file has already been uploaded, are you sure you want to overwrite?", "FTP Server", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            uploadThisFile = true;

                            BrushConverter brush = new BrushConverter();
                            overwriteFileGrid = (Grid)wrapPanel.Children[i];
                            Label overwriteFileLabel = (Label)overwriteFileGrid.Children[2];
                            overwriteFileLabel.Content = "Overwriting";
                            overwriteFileLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");
                            //overwriteFileGrid.Children.Add(progressBar);
                            Grid.SetRow(progressBar, 3);
                        }
                        else
                            uploadThisFile = false;

                        checkFileInServer = true;
                        break;
                    }
                    else
                    {
                        uploadThisFile = true;
                        checkFileInServer = false;
                    }
                }
            }

        }
        private void OnClickIconGrid(object sender, MouseButtonEventArgs e)
        {
            previousSelectedFile = currentSelectedFile;
            currentSelectedFile = (Grid)sender;
            currentLabel = (Label)currentSelectedFile.Children[1];
            BrushConverter brush = new BrushConverter();

            if (previousSelectedFile != null && previousSelectedFile != currentSelectedFile)
            {
                previousSelectedFile.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                Label previousLabel = (Label)previousSelectedFile.Children[1];
                previousLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            }

            if (previousSelectedFile != currentSelectedFile)
            {
                currentSelectedFile.Background = (Brush)brush.ConvertFrom("#105A97");
                currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog downloadFile = new System.Windows.Forms.FolderBrowserDialog();

                if (downloadFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //serverFileName = currentLabel.Content.ToString();
                serverFileName = (String)product.GetProductID().ToString() + ".jpg";
                integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

                localFolderPath = downloadFile.SelectedPath;
                localFileName = serverFileName;

                progressBar.Visibility = Visibility.Visible;
                //currentSelectedFile.Children.Add(progressBar);
                Grid.SetRow(progressBar, 3);


                //downloadBackground.RunWorkerAsync();
            }
        }
        public void resizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///EXTRA FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelUploadFilesPage = this;

            modelBasicInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;
            modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }

        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            modelAdditionalInfoPage.modelBasicInfoPage = modelBasicInfoPage;
            modelAdditionalInfoPage.modelUploadFilesPage = this;

            NavigationService.Navigate(modelAdditionalInfoPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED AND MOUSE LEAVE
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBrowseButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog uploadFile = new OpenFileDialog();

            if (uploadFile.ShowDialog() == false)
                return;

            if (!integrityChecks.CheckFileEditBox(uploadFile.FileName, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            localFolderPath = uploadFile.FileName;
            localFileName = System.IO.Path.GetFileName(localFolderPath);

            serverFileName = localFileName;

            ftpFiles.Add(localFileName);

            InsertIconGrid(localFolderPath);
            currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
            //SystemWatcher.fromSoftware = true;

            try
            {
                String tmp = product.GetModelFolderLocalPath() + Path.GetFileName(localFolderPath);
                File.Copy(localFolderPath, product.GetModelFolderLocalPath() + Path.GetFileName(localFolderPath));
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory(product.GetModelFolderLocalPath());
                File.Copy(localFolderPath, product.GetModelFolderLocalPath() + Path.GetFileName(localFolderPath));
            }

        }

        private void OnClickRetryButton(object sender, RoutedEventArgs e)
        {

            FTPServer fTPServer = new FTPServer();

            if (!fTPServer.CheckExistingFolder(serverFolderPath))
            {
                if (!fTPServer.CreateNewFolder(serverFolderPath))
                {
                    uploadFilesStackPanel.Children.Clear();
                    InsertErrorRetryButton();
                    return;
                }
            }
            else
            {
                ftpFiles.Clear();
                if (!fTPServer.ListFilesInFolder(serverFolderPath, ref ftpFiles, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    uploadFilesStackPanel.Children.Clear();
                    InsertErrorRetryButton();
                    return;
                }

            }

            if (ftpFiles.Count != 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);

                for (int i = 0; i < ftpFiles.Count; i++)
                {
                    if (ftpFiles[i] != "." || ftpFiles[i] != "..") ;
                    //InsertIconGridFromServer(i);
                }

            }
            else if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                InsertDragAndDropOrBrowseGrid();
            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////




        private void InsertDragAndDropOrBrowseGrid()
        {

            Grid grid = new Grid();

            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"Icons\drop_files_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            resizeImage(ref icon, 250, 150);
            grid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label orLabel = new Label();
            orLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            orLabel.Content = "OR";
            orLabel.FontWeight = FontWeights.Bold;
            orLabel.FontSize = 20;
            orLabel.Foreground = Brushes.Gray;
            grid.Children.Add(orLabel);
            Grid.SetRow(orLabel, 1);

            Button browseFileButton = new Button();
            browseFileButton.Style = (Style)FindResource("buttonBrowseStyle");
            browseFileButton.Width = 200;
            browseFileButton.Background = null;
            browseFileButton.Foreground = Brushes.Gray;
            browseFileButton.Content = "BROWSE FILE";
            browseFileButton.Click += OnClickBrowseButton;
            grid.Children.Add(browseFileButton);
            Grid.SetRow(browseFileButton, 2);

            uploadFilesStackPanel.Children.Add(grid);

        }

        private void InsertErrorRetryButton()
        {
            Grid grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"Icons\no_internet_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            resizeImage(ref icon, 250, 250);
            grid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Button retryButton = new Button();
            retryButton.Style = (Style)FindResource("buttonBrowseStyle");
            retryButton.Width = 200;
            retryButton.Background = null;
            retryButton.Foreground = Brushes.Gray;
            retryButton.Content = "Retry";
            retryButton.Click += OnClickRetryButton;
            grid.Children.Add(retryButton);
            Grid.SetRow(retryButton, 1);

            uploadFilesStackPanel.Children.Add(grid);
        }


    }
}
