using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using Label = System.Windows.Controls.Label;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ModelBasicInfoPage.xaml
    /// </summary>
    public partial class ModelBasicInfoPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        protected FTPServer ftpObject;

        private Model product;
        public int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelUploadFilesPage modelUploadFilesPage;
        List<string> ftpFiles;

        ProgressBar progressBar = new ProgressBar();
        
        protected String serverFolderPath;
        protected String serverFileName;

        public String localFolderPath;
        protected String localFileName;

        protected bool fileUploaded;
        protected bool fileDownloaded;
        protected bool uploadThisFile = false;
        protected bool checkFileInServer = false;
        protected bool canEdit = false;
        protected bool productNameEdited = false;
        protected bool productSummaryPointsEdited = false;
        protected bool productPhotoEdited = false;

        WrapPanel wrapPanel = new WrapPanel();
        Grid UploadIconGrid = new Grid();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Grid overwriteFileGrid;
        private Label currentLabel;
        private String errorMessage = "";
        public bool edit_photo = false;
        public ModelBasicInfoPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser, ref Model mPrduct, int mViewAddCondition)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;

            viewAddCondition = mViewAddCondition;
            ftpFiles = new List<String>();
            ftpObject = new FTPServer();

            product = mPrduct;

            InitializeComponent();

            ftpFiles = new List<string>(); progressBar.Style = (Style)FindResource("ProgressBarStyle");
            progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            progressBar.Width = 200;

            uploadFilesStackPanel.Children.Clear();

            serverFolderPath = product.GetModelFolderServerPath();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

                modelNameTextBox.Visibility = Visibility.Visible;
                summeryPointsTextBox.Visibility = Visibility.Visible;
                InsertDragAndDropOrBrowseGrid();

                product.GetNewModelID();
                product.GetNewModelPhotoLocalPath();





            }

            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

                editPictureButton.Visibility = Visibility.Visible;
                serverFileName = (String)product.GetModelID().ToString() + ".jpg";
                product.GetNewModelPhotoLocalPath();
                localFolderPath = product.GetModelFolderLocalPath();


                try
                {
                    uploadFilesStackPanel.Children.Clear();
                    uploadFilesStackPanel.Children.Add(wrapPanel);
                    String[] tmp = Directory.GetFiles(product.GetModelFolderLocalPath());
                    String localFile = "";
                    for (int i = 0; i < tmp.Length; i++)
                    {

                        if (tmp[i].Contains(".jpg") || tmp[i].Contains(".png") || tmp[i].Contains(".raw") || tmp[i].Contains(".jpeg"))
                        {
                            localFile = tmp[i];
                        }
                    }
                    localFileName = System.IO.Path.GetFileName(localFile);
                    product.SetModelPhotoLocalPath(localFile);

                    uploadFilesStackPanel.Children.Clear();


                    InsertIconGrid(localFile);



                }
                catch
                {


                    product.SetModelPhotoServerPath(product.GetModelFolderServerPath() + "/" + product.GetModelID() + ".jpg");
                    //if (product.DownloadPhotoFromServer(product.GetModelPhotoServerPath(), product.GetModelPhotoLocalPath()))
                    //{

                    //    Image brandLogo = new Image();
                    //    //string src = String.Format(@"/01electronics_crm;component/photos/brands/" + brandsList[i].brand_id + ".jpg
                    //    BitmapImage src = new BitmapImage();
                    //    src.BeginInit();
                    //    src.UriSource = new Uri(product.GetModelFolderLocalPath(), UriKind.Relative);
                    //    src.CacheOption = BitmapCacheOption.OnLoad;
                    //    src.EndInit();
                    //    brandLogo.Source = src;
                    //    brandLogo.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    //    brandLogo.VerticalAlignment = VerticalAlignment.Stretch;
                    //    wrapPanel.Children.Add(brandLogo);

                    //    uploadFilesStackPanel.Children.Add(wrapPanel);
                    //}
                }
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);

                //NameLabel.Visibility = Visibility.Visible;
                //summeryPointsTextBlock.Visibility = Visibility.Visible;
                InitializeInfo();

            }

            //
            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            {

                SpecsType.Content = "Genset Specs";

            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeInfo()
        {
            modelNameTextBox.Visibility = Visibility.Collapsed;
            modelNameLabel.Visibility = Visibility.Visible;
            modelNameLabel.Content = product.GetModelName();
            summeryPointsTextBox.Visibility = Visibility.Collapsed;
            summeryPointsLabel.Visibility = Visibility.Visible;

            try
            {
                summeryPointsLabel.Text = product.GetModelSummaryPoints()[0];
                for (int i = 1; i < product.GetModelSummaryPoints().Count; i++)
                {
                    AddNewSymmeryPoint(i - 1, "Point #", product.GetModelSummaryPoints()[i], standardFeaturesGrid, standardFeaturesGrid.Children[i - 1] as Grid, onClickHandler);

                }
            }
            catch (Exception ex)
            {

            }





        }
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
                    serverFileName = (String)product.GetModelID().ToString() + ".jpg";

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



                        uploadThisFile = false;
                    }
                    else if (uploadThisFile == true)
                    {


                        uploadThisFile = false;
                    }
                }
            }
        }
        private void LoadIcon(ref Image icon)
        {

            Image productImage = new Image();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION || edit_photo == true)
            {
                modelAdditionalInfoPage.finishButton.IsEnabled = true;
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(localFolderPath, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                productImage.Source = src;
                productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;

                Grid.SetRow(productImage, 0);

            }
            else
            {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(localFolderPath + localFileName, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                productImage.Source = src;
                productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;

                Grid.SetRow(productImage, 0);


                //    try
                //    {
                //
                //        BitmapImage src = new BitmapImage();
                //        src.BeginInit();
                //        src.UriSource = new Uri(product.GetProductPhotoLocalPath(), UriKind.Relative);
                //        src.CacheOption = BitmapCacheOption.OnLoad;
                //        src.EndInit();
                //        productImage.Source = src;
                //        productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //        productImage.VerticalAlignment = VerticalAlignment.Stretch;
                //
                //        Grid.SetRow(productImage, 0);
                //
                //    }
                //    catch
                //    {
                //        product.SetPhotoServerPath(product.GetProductFolderServerPath() + "/" + product.GetProductID() + ".jpg");
                //        if (product.DownloadPhotoFromServer())
                //        {
                //            BitmapImage src = new BitmapImage();
                //            src.BeginInit();
                //            src.UriSource = new Uri(product.GetProductPhotoLocalPath(), UriKind.Relative);
                //            src.CacheOption = BitmapCacheOption.OnLoad;
                //            src.EndInit();
                //            productImage.Source = src;
                //            productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //            productImage.VerticalAlignment = VerticalAlignment.Stretch;
                //            
                //            Grid.SetRow(productImage, 0);
                //        }
                //    }
            }
            icon = productImage;

            canEdit = true;
            editPictureButton.Visibility = Visibility.Visible;

            //if (productImage.Width!=1800 || productImage.Height!= 600)
            //{
            //    MessageBox.Show("Picture Should be 1800px X 600px ", "Error", (System.Windows.Forms.MessageBoxButtons)MessageBoxButton.OK, (System.Windows.Forms.MessageBoxIcon)MessageBoxImage.Error);

            //}
            //else
            //{
            //    icon = productImage;

            //}

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
        public void resizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }
        void updateLabelIds(String content, Grid currentGrid, int index)
        {
            for (int i = index; i < currentGrid.Children.Count; i++)
            {
                Grid innerGrid = currentGrid.Children[i] as Grid;
                Label header = innerGrid.Children[0] as Label;
                header.Content = content + (i + 1).ToString();
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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

            LoadIcon(ref icon);

            //resizeImage(ref icon, 350, 150);
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
        private void InsertIconGridFromServer(int i)
        {
            UploadIconGrid.Margin = new Thickness(24);
            //UploadIconGrid.Width = 250;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();

            UploadIconGrid.RowDefinitions.Add(row1);
            UploadIconGrid.RowDefinitions.Add(row2);
            UploadIconGrid.RowDefinitions.Add(row3);
            UploadIconGrid.RowDefinitions.Add(row4);

            UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

            Image icon = new Image();

            LoadIcon(ref icon);

            //resizeImage(ref icon, 350, 150);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            icon.VerticalAlignment = VerticalAlignment.Stretch;

            Label name = new Label();
            name.Content = ftpFiles[i];
            name.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(name);
            Grid.SetRow(name, 1);

            Label status = new Label();
            status.Content = "SUBMITTED";
            status.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            BrushConverter brush = new BrushConverter();
            status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
            UploadIconGrid.Children.Add(status);
            Grid.SetRow(status, 2);

            wrapPanel.Children.Add(UploadIconGrid);
        }
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
            resizeImage(ref icon, 200, 100);
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
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            localFolderPath = uploadFile.FileName;
            localFileName = System.IO.Path.GetFileName(localFolderPath);

            //serverFileName = localFileName;

            serverFileName = (String)product.GetModelID().ToString() + ".jpg";
            ftpFiles.Add(localFileName);

            InsertIconGrid(localFolderPath);
            currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
            //currentSelectedFile.Children.Add(progressBar);
            Grid.SetRow(progressBar, 3);

            //uploadBackground.RunWorkerAsync();
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
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
                    if (ftpFiles[i] != "." || ftpFiles[i] != "..")
                        InsertIconGridFromServer(i);
                }

            }
            else if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                InsertDragAndDropOrBrowseGrid();
            }

        }
        private void OnClickAddFilesGrid(object sender, MouseButtonEventArgs e)
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
                OpenFileDialog uploadFile = new OpenFileDialog();

                if (uploadFile.ShowDialog() == false)
                    return;

                if (!integrityChecks.CheckFileEditBox(uploadFile.FileName, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                localFolderPath = uploadFile.FileName;
                localFileName = System.IO.Path.GetFileName(localFolderPath);

                serverFileName = localFileName;

                CheckIfFileAlreadyUploaded(localFileName);

                if (uploadThisFile == true && checkFileInServer == false)
                {
                    ftpFiles.Add(localFileName);

                    uploadFilesStackPanel.Children.Clear();
                    uploadFilesStackPanel.Children.Add(wrapPanel);

                    if (wrapPanel.Children.Count != 0)
                        wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

                    InsertIconGrid(localFolderPath);

                    currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
                    //currentSelectedFile.Children.Add(progressBar);
                    Grid.SetRow(progressBar, 3);


                    uploadThisFile = false;
                }
                else if (checkFileInServer == true)
                {
                    //uploadBackground.RunWorkerAsync();

                    uploadThisFile = false;
                }
            }
        }
        private void onBtnEditClick(object sender, RoutedEventArgs e)
        {
            edit_photo = true;
            OpenFileDialog uploadFile = new OpenFileDialog();

            if (uploadFile.ShowDialog() == false)
                return;

            if (!integrityChecks.CheckFileEditBox(uploadFile.FileName, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            localFolderPath = uploadFile.FileName;
            localFileName = System.IO.Path.GetFileName(localFolderPath);

            product.SetModelPhotoServerPath(product.GetProductFolderServerPath() + "/" + product.GetProductID() + ".jpg");
            product.SetModelPhotoLocalPath(localFolderPath + "/" + localFileName);


            wrapPanel.Children.Clear();
            uploadFilesStackPanel.Children.Clear();
            //product.UploadPhotoToServer(product.GetModelPhotoServerPath(), product.GetModelPhotoLocalPath());

            serverFileName = (String)product.GetModelID().ToString() + ".jpg";
            //localFolderPath = product.GetProductPhotoLocalPath();
            //uploadBackground.RunWorkerAsync();
            uploadFilesStackPanel.Children.Clear();
            uploadFilesStackPanel.Children.Add(wrapPanel);
            //uploadFilesStackPanel.Children.Add(wrapPanel);
            productPhotoEdited = true;

            UploadIconGrid.Children.Clear();
            UploadIconGrid.RowDefinitions.Clear();
            InsertIconGrid(localFolderPath);

            //currentSelectedFile.Children.Add(progressBar);

            //saveChangesButton.IsEnabled = true;

            /*
                        ftpFiles.Remove(localFileName);
                        product.DeletePhotoFromServer();

                        //serverFileName = localFileName;

                        serverFileName = (String)product.GetProductID().ToString() + ".jpg";
                        ftpFiles.Add(localFileName);


                        InsertIconGrid("pending", localFolderPath);
                        currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
                        currentSelectedFile.Children.Add(progressBar);
                        Grid.SetRow(progressBar, 3);

                        uploadBackground.RunWorkerAsync();*/
        }
        private void NameMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            modelNameTextBox.Text = modelNameLabel.Content.ToString();
            modelNameTextBox.Visibility = Visibility.Visible;
            modelNameTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            modelNameTextBox.VerticalContentAlignment = VerticalAlignment.Center;
            modelNameLabel.Visibility = Visibility.Collapsed;
        }
        private void modelNameTextBoxMouseLeave(object sender, MouseEventArgs e)
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

                if (modelNameTextBox.Text.ToString() != product.GetModelName())
                {
                    BrushConverter brushConverter = new BrushConverter();
                    modelNameLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                    modelNameTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                    product.is_changed = true;
                    modelNameLabel.Content = modelNameTextBox.Text.ToString();
                    product.SetModelName(modelNameTextBox.Text.ToString());
                    product.is_changed = true;
                    modelAdditionalInfoPage.CheckFinishButton();

                }
                else
                {
                    BrushConverter brushConverter = new BrushConverter();
                    modelNameLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                    modelNameTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                    product.is_changed = false;
                    modelAdditionalInfoPage.CheckFinishButton();

                }


                modelNameTextBox.Visibility = Visibility.Collapsed;
                modelNameLabel.Visibility = Visibility.Visible;




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
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                //serverFileName = currentLabel.Content.ToString();
                serverFileName = (String)product.GetModelID().ToString() + ".jpg";
                integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

                localFolderPath = downloadFile.SelectedPath;
                localFileName = serverFileName;

                progressBar.Visibility = Visibility.Visible;
                //currentSelectedFile.Children.Add(progressBar);
                Grid.SetRow(progressBar, 3);

                Label currentStatusLabel = (Label)currentSelectedFile.Children[2];
                currentStatusLabel.Content = "DOWNLOADING";
                currentStatusLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");

            }
        }
        private void OnClickRemoveFeature(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            //int tagID = int.Parse(innerGrid.Tag.ToString());

            int index = outerGrid.Children.IndexOf(innerGrid);

            if (outerGrid.Children.Count == 2)
            {
                innerGrid.Children.Clear();
                Image addVendorImage = new Image();
                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                addVendorImage.Height = 20;
                addVendorImage.Width = 20;
                addVendorImage.MouseLeftButtonDown += onClickHandler;
                Grid previousGrid = outerGrid.Children[0] as Grid;

                previousGrid.Children.Add(addVendorImage);
                Grid.SetColumn(addVendorImage, 2);
                outerGrid.Children.Remove(innerGrid);
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
            else if (index == outerGrid.Children.Count - 1 && outerGrid.Children.Count != 0)
            {
                outerGrid.Children.Remove(innerGrid);
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);

                Grid previousInnerGrid = (Grid)outerGrid.Children[index - 1];
                Image plusIcon = (Image)innerGrid.Children[3];
                innerGrid.Children.Remove(plusIcon);
                Grid.SetColumn(plusIcon, 2);
                previousInnerGrid.Children.Add(plusIcon);
                product.is_changed = true;
                modelAdditionalInfoPage.CheckFinishButton();
            }
            else
            {
                for (int i = index; i < outerGrid.Children.Count - 1; i++)
                {
                    if (i == index)
                        Grid.SetRow(innerGrid, outerGrid.RowDefinitions.Count - 1);
                    else
                        Grid.SetRow(outerGrid.Children[i], i - 1);

                }

                outerGrid.Children.Remove(innerGrid);

                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }

            String Content = "Point #";
            updateLabelIds(Content, outerGrid, index);
        }
        private void OnClickEditFeature(object sender, MouseButtonEventArgs e)
        {
            TextBlock currentLabel = (TextBlock)sender;
            Grid innerGrid = (Grid)currentLabel.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            Image deleteIcon = (Image)innerGrid.Children[3];
            TextBox TextBox = (TextBox)innerGrid.Children[1];
            TextBox.MouseLeave += SummeryPointMouseLeave;

            TextBox.Text = currentLabel.Text.ToString();

            deleteIcon.Visibility = Visibility.Visible;
            TextBox.Visibility = Visibility.Visible;
            currentLabel.Visibility = Visibility.Collapsed;
            product.is_changed = true;
            modelAdditionalInfoPage.CheckFinishButton();
        }
        private void onClickHandler(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            int index = standardFeaturesGrid.Children.IndexOf(parentGrid);

            AddNewSymmeryPoint(index, "Point #", standardFeaturesGrid, parentGrid, onClickHandler);
            product.is_changed = true;
            modelAdditionalInfoPage.CheckFinishButton();


        }
        private void AddNewSymmeryPoint(int index, String labelContent, Grid mainGrid, Grid parentGrid, MouseButtonEventHandler onClickHandler)
        {
            if (index != 0 && index != -1)
                parentGrid.Children.RemoveAt(3);
            else if (index == 0)
                parentGrid.Children.RemoveAt(2);

            Grid.SetRow(parentGrid, mainGrid.RowDefinitions.Count - 1);
            //Image removeIcon = new Image();
            //removeIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            //removeIcon.Width = 20;
            //removeIcon.Height = 20;
            //removeIcon.Margin = new Thickness(10, 0, 10, 0);
            //removeIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            //Grid.SetColumn(removeIcon, 2);
            //Grid.SetRow(removeIcon, index);
            //
            //parentGrid.Children.Add(removeIcon);
            //}A

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(75);
            mainGrid.RowDefinitions.Add(row);

            /////NEW FEATURE GRID
            Grid gridI = new Grid();
            Grid.SetRow(gridI, index + 1);

            ColumnDefinition labelColumn = new ColumnDefinition();
            labelColumn.Width = new GridLength(250);

            ColumnDefinition featureColumn = new ColumnDefinition();

            ColumnDefinition imageColumn = new ColumnDefinition();
            imageColumn.Width = new GridLength(90);

            gridI.ColumnDefinitions.Add(labelColumn);
            gridI.ColumnDefinitions.Add(featureColumn);
            gridI.ColumnDefinitions.Add(imageColumn);

            Label featureIdLabel = new Label();
            featureIdLabel.Margin = new Thickness(30, 0, 0, 0);
            featureIdLabel.Width = 200;
            featureIdLabel.HorizontalAlignment = HorizontalAlignment.Left;
            featureIdLabel.Style = (Style)FindResource("labelStyle");
            featureIdLabel.Content = labelContent + (index + 2).ToString();
            //featureIdLabel.Content = "Feature #" + (index + 2).ToString();
            Grid.SetColumn(featureIdLabel, 0);

            TextBox featureTextBox = new TextBox();
            featureTextBox.Style = (Style)FindResource("textBoxStyle");
            featureTextBox.TextWrapping = TextWrapping.Wrap;
            featureTextBox.TextChanged += FeatureTextBoxTextChanged;

            Grid.SetColumn(featureTextBox, 1);

            TextBlock featureTextBlock = new TextBlock();
            featureTextBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.Visibility = Visibility.Visible;
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.FontSize = 16;
            featureTextBlock.MouseDown += OnClickEditFeature;
            featureTextBlock.FontWeight = FontWeights.DemiBold;
            featureTextBlock.Width = 384;
            featureTextBlock.Style = (Style)FindResource("cardTextBlockStyle");
            Grid.SetColumn(featureTextBlock, 1);


            Image deleteIcon = new Image();
            deleteIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            deleteIcon.Width = 20;
            deleteIcon.Height = 20;
            //deleteIcon.Margin = new Thickness(0, 0, 10, 0);
            deleteIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            Grid.SetColumn(deleteIcon, 2);

            Image addIcon = new Image();
            addIcon.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
            addIcon.Width = 20;
            addIcon.Height = 20;
            addIcon.Margin = new Thickness(55, 0, 10, 0);
            addIcon.MouseLeftButtonDown += onClickHandler;
            //addIcon.Tag = selectedGridiD;
            //addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);

            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(featureTextBlock);
            gridI.Children.Add(deleteIcon);
            gridI.Children.Add(addIcon);

            //if (index != 0)
            //{ 
            //}

            /////////////////////////////////////////////

            //standardFeaturesGrid.Children.Add(gridI);
            mainGrid.Children.Add(gridI);
        }
        private void AddNewSymmeryPoint(int index, String labelContent, String SummryPointContent, Grid mainGrid, Grid parentGrid, MouseButtonEventHandler onClickHandler)
        {
            if (index != 0 && index != -1)
                parentGrid.Children.RemoveAt(4);
            else if (index == 0)
                parentGrid.Children.RemoveAt(2);

            Grid.SetRow(parentGrid, mainGrid.RowDefinitions.Count - 1);
            //Image removeIcon = new Image();
            //removeIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            //removeIcon.Width = 20;
            //removeIcon.Height = 20;
            //removeIcon.Margin = new Thickness(10, 0, 10, 0);
            //removeIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            //Grid.SetColumn(removeIcon, 2);
            //Grid.SetRow(removeIcon, index);
            //
            //parentGrid.Children.Add(removeIcon);
            //}A

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(75);
            mainGrid.RowDefinitions.Add(row);

            /////NEW FEATURE GRID
            Grid gridI = new Grid();
            Grid.SetRow(gridI, index + 1);

            ColumnDefinition labelColumn = new ColumnDefinition();
            labelColumn.Width = new GridLength(250);

            ColumnDefinition featureColumn = new ColumnDefinition();
            featureColumn.Width = new GridLength(500);

            ColumnDefinition imageColumn = new ColumnDefinition();
            imageColumn.Width = new GridLength(100);

            gridI.ColumnDefinitions.Add(labelColumn);
            gridI.ColumnDefinitions.Add(featureColumn);
            gridI.ColumnDefinitions.Add(imageColumn);

            Label featureIdLabel = new Label();
            featureIdLabel.Margin = new Thickness(30, 0, 0, 0);
            featureIdLabel.Width = 200;
            featureIdLabel.HorizontalAlignment = HorizontalAlignment.Left;
            featureIdLabel.Style = (Style)FindResource("labelStyle");
            featureIdLabel.Content = labelContent + (index + 2).ToString();
            //featureIdLabel.Content = "Feature #" + (index + 2).ToString();
            Grid.SetColumn(featureIdLabel, 0);

            TextBox featureTextBox = new TextBox();
            featureTextBox.Style = (Style)FindResource("textBoxStyle");
            featureTextBox.TextWrapping = TextWrapping.Wrap;
            featureTextBox.Visibility = Visibility.Collapsed;
            featureTextBox.Margin = new Thickness(30, 0, 0, 0);
            featureTextBox.Margin = new Thickness(30, 12, 12, 12);
            featureTextBox.Width = 384;
            featureTextBox.TextChanged += FeatureTextBoxTextChanged;
            Grid.SetColumn(featureTextBox, 1);


            TextBlock featureTextBlock = new TextBlock();
            featureTextBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            featureTextBlock.Text = SummryPointContent;
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.Visibility = Visibility.Visible;
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.FontSize = 16;
            featureTextBlock.MouseDown += OnClickEditFeature;
            featureTextBlock.FontWeight = FontWeights.DemiBold;
            featureTextBlock.Width = 384;
            featureTextBlock.Style = (Style)FindResource("cardTextBlockStyle");

            Grid.SetColumn(featureTextBlock, 1);
            //featureTextBlock.FontFamily = FontFamily.("Sans Serif");

            //TextBlock featureTextBlock = new TextBlock();
            //featureTextBlock.Style = (Style)FindResource("textBlockStyle");
            //featureTextBlock.Foreground = Brushes.Blue;
            //featureTextBlock.FontSize = 30;

            Image deleteIcon = new Image();
            deleteIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            deleteIcon.Width = 20;
            deleteIcon.Height = 20;
            //deleteIcon.Margin = new Thickness(0, 0, 10, 0);
            deleteIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            Grid.SetColumn(deleteIcon, 2);
            deleteIcon.Visibility = Visibility.Collapsed;


            Image addIcon = new Image();
            addIcon.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
            addIcon.Width = 20;
            addIcon.Height = 20;
            addIcon.Margin = new Thickness(55, 0, 10, 0);
            addIcon.MouseLeftButtonDown += onClickHandler;
            //addIcon.Tag = selectedGridiD;
            //addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);

            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(featureTextBlock);
            gridI.Children.Add(deleteIcon);

            gridI.Children.Add(addIcon);

            //if (index != 0)
            //{ 
            //}

            /////////////////////////////////////////////

            //standardFeaturesGrid.Children.Add(gridI);
            mainGrid.Children.Add(gridI);
        }
        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            product.SetModelName(modelNameTextBox.Text.ToString());
            modelAdditionalInfoPage.modelBasicInfoPage = this;
            modelAdditionalInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }

            NavigationService.Navigate(modelAdditionalInfoPage);

        }
        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {
            product.SetModelName(modelNameTextBox.Text.ToString());
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }

            NavigationService.Navigate(modelUpsSpecsPage);
        }
        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            product.SetModelName(modelNameTextBox.Text.ToString());

            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;
                NavigationService.Navigate(modelUploadFilesPage);
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }


        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }
        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            product.SetModelName(modelNameTextBox.Text.ToString());
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                try
                {
                    product.GetModelSummaryPoints().Clear();
                    for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                    {
                        if (i == standardFeaturesGrid.Children.Count - 1)
                        {
                            Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                            TextBox feature = innerGrid.Children[1] as TextBox;
                            TextBlock featureTextBlockl = new TextBlock();
                            featureTextBlockl = innerGrid.Children[3] as TextBlock;
                            feature.Text = featureTextBlockl.Text.ToString();

                            if (feature.Text != String.Empty)
                                product.GetModelSummaryPoints().Add(feature.Text.ToString());
                        }
                        else
                        {
                            Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                            TextBox feature = innerGrid.Children[1] as TextBox;
                            TextBlock featureTextBlockl = new TextBlock();
                            featureTextBlockl = innerGrid.Children[2] as TextBlock;
                            feature.Text = featureTextBlockl.Text.ToString();

                            if (feature.Text.ToString() != String.Empty)
                                product.GetModelSummaryPoints().Add(feature.Text.ToString());

                        }

                    }

                }
                catch (Exception ex)
                {

                }
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }

            NavigationService.Navigate(modelUpsSpecsPage);
        }
        private void summeryPointsLabel1MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            summeryPointsTextBox.Text = summeryPointsLabel.Text.ToString();
            summeryPointsLabel.Visibility = Visibility.Collapsed;
            summeryPointsTextBox.Visibility = Visibility.Visible;


        }
        private void SummaryPoint1MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                if (summeryPointsTextBox.Text.ToString() != product.GetModelSummaryPoints()[0])
                {
                    BrushConverter brushConverter = new BrushConverter();
                    summeryPointsLabel.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                    summeryPointsTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                    product.is_changed = true;
                    modelAdditionalInfoPage.CheckFinishButton();


                }
                else
                {
                    BrushConverter brushConverter = new BrushConverter();
                    summeryPointsLabel.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                    summeryPointsTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                    product.is_changed = false;
                    modelAdditionalInfoPage.CheckFinishButton();


                }
                summeryPointsLabel.Text = summeryPointsTextBox.Text;
                summeryPointsTextBox.Visibility = Visibility.Collapsed;
                summeryPointsLabel.Visibility = Visibility.Visible;
            }

        }
        private void SummeryPointMouseLeave(object sender, MouseEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                TextBox currentTextBox = (TextBox)sender;
                Grid innerGrid = (Grid)currentTextBox.Parent;
                Grid outerGrid = (Grid)innerGrid.Parent;
                Image deleteIcon = (Image)innerGrid.Children[3];
                TextBlock currentTextBlockl = (TextBlock)innerGrid.Children[2];

                for (int i = 1; i < outerGrid.Children.Count; i++)
                {
                    if (currentTextBox.Text.ToString() != currentTextBlockl.Text)
                    {
                        BrushConverter brushConverter = new BrushConverter();
                        currentTextBlockl.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                        currentTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#FF0000");
                        product.is_changed = true;
                        modelAdditionalInfoPage.CheckFinishButton();
                    }
                    else
                    {
                        BrushConverter brushConverter = new BrushConverter();
                        currentTextBlockl.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                        currentTextBox.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                        product.is_changed = false;
                        modelAdditionalInfoPage.CheckFinishButton();
                    }
                }
                currentTextBlockl.Text = currentTextBox.Text;

                deleteIcon.Visibility = Visibility.Collapsed;
                currentTextBox.Visibility = Visibility.Collapsed;
                currentTextBlockl.Visibility = Visibility.Visible;
            }
        }
        private void FeatureTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox currentTextBox = new TextBox();
            currentTextBox = sender as TextBox;
            Grid currentGrid = new Grid();
            currentGrid = currentTextBox.Parent as Grid;
            TextBlock currentTextBlock = new TextBlock();
            currentTextBlock = currentGrid.Children[2] as TextBlock;

            currentTextBlock.Text = currentTextBox.Text;
        }

    }
}
