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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_inventory
{
    /// <summary>
    /// Interaction logic for ReleasePermitPage.xaml
    /// </summary>
    public partial class ReleasePermitPage : Page
    {
        private CommonQueries commonQueries;
        private SecuredCommonQueries securedCommonQueries;
        private CommonFunctions commonFunctions;
        private IntegrityChecks integrityChecks;
        private Employee loggedInUser;

        List<INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT> materialReleasePermits;

        Grid previousGrid = null;

        Expander previousExpander = null;
        AddReleasePermitWindow addReleasePermitWindow;
        int checkedItems = -1;
        List<string> ReEntry;
        List<string> recievalNote;

        List<Grid> gridItems;

        CheckBox checkAll;

        int itemsCount = 0;

        List<int> checkedReleasePermititemSerials;

        int viewAddCondition;
        MaterialReleasePermits materialRelease;

        public ReleasePermitPage(ref CommonQueries mCommonQueries, ref CommonFunctions mCommonFunctions, ref IntegrityChecks mIntegrityChecks, ref Employee mLoggedInUser)
        {
            commonFunctions = mCommonFunctions;
            commonQueries = mCommonQueries;
            integrityChecks = mIntegrityChecks;
            loggedInUser = mLoggedInUser;
            InitializeComponent();
            materialReleasePermits = new List<INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_MIN_STRUCT>();
            InitializeUiElements();
            ReEntry = new List<string>();
            recievalNote = new List<string>();
            materialRelease = new MaterialReleasePermits();
            gridItems = new List<Grid>();

            checkedReleasePermititemSerials = new List<int>();

        }

        public void InitializeUiElements()
        {

            ReleasePermitStackPanel.Children.Clear();

            commonQueries.GetReleasePermits(ref materialReleasePermits);

            for(int i =0; i<materialReleasePermits.Count; i++)
            {
                Border cardBorder = new Border();
                cardBorder.Margin = new Thickness(10);
                cardBorder.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                cardBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                cardBorder.BorderThickness = new Thickness(1);
                cardBorder.CornerRadius = new CornerRadius(20);

                DropShadowEffect shadow = new DropShadowEffect();
                shadow.BlurRadius = 20;
                shadow.Color = Color.FromRgb(211, 211, 211);
                cardBorder.Effect = shadow;

                Grid card = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
                card.RowDefinitions.Add(new RowDefinition());
                card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(470) });
                card.ColumnDefinitions.Add(new ColumnDefinition() /*{ Width = new GridLength(200) }*/);
                card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(85) });

                Label header = new Label();
                header.Content = $"Release Permit - {materialReleasePermits[i].release_Permit_Id}";
                header.Style = (Style)FindResource("stackPanelItemHeader");

                Grid.SetRow(header, 0);
                Grid.SetColumn(header, 0);
                card.Children.Add(header);
                card.Tag = materialReleasePermits[i].release_Permit_Serial;

                card.RowDefinitions.Add(new RowDefinition());
                Label releaseDate = new Label();
                releaseDate.Content ="Release Date: " + materialReleasePermits[i].release_date.ToString("yyyy-MM-dd");
                releaseDate.Style = (Style)FindResource("stackPanelItemHeader");

                Grid.SetRow(releaseDate, card.Children.Count);
                Grid.SetColumn(releaseDate, 0);
                card.Children.Add(releaseDate);

                int itemNo = 0;
                int releasedQuantity = 1;
                Label quantityLabel = new Label();
                Border quantity = new Border();
                for (int j=0; j < materialReleasePermits[i].material_release_items.Count; j++)
                {
                    if (materialReleasePermits[i].material_release_items[j].rfp_info.rfpSerial != 0)
                    {
                        

                        if (materialReleasePermits[i].material_release_items[j].materialItemcompanyCategory.category_id != null)
                        {
                            if(j>0 && materialReleasePermits[i].material_release_items[j].materialItemcompanyCategory.category_id == materialReleasePermits[i].material_release_items[j-1].materialItemcompanyCategory.category_id
                                   && materialReleasePermits[i].material_release_items[j].materialitemCompanyproduct.type_id == materialReleasePermits[i].material_release_items[j-1].materialitemCompanyproduct.type_id
                                   && materialReleasePermits[i].material_release_items[j].materialItemCompanyBrand.brand_id == materialReleasePermits[i].material_release_items[j-1].materialItemCompanyBrand.brand_id
                                   && materialReleasePermits[i].material_release_items[j].materialItemCompanyModel.model_id == materialReleasePermits[i].material_release_items[j-1].materialItemCompanyModel.model_id
                                   && materialReleasePermits[i].material_release_items[j].materialItemCompanySpecs.spec_name == materialReleasePermits[i].material_release_items[j-1].materialItemCompanySpecs.spec_name)
                            {
                                releasedQuantity++;
                                quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                                continue;
                            }

                            if (materialReleasePermits[i].material_release_items[j].rfp_info.rfpID == "")
                            {
                                continue;
                            }
                            if (materialReleasePermits[i].material_release_items[j].materialItemcompanyCategory.category_name == "")
                                continue;
                            if (j==0)
                            {
                                card.RowDefinitions.Add(new RowDefinition());
                                Label rfpIdLabel = new Label();
                                rfpIdLabel.Margin = new Thickness(0, 0, 0, 10);
                                rfpIdLabel.Style = (Style)FindResource("stackPanelItemBody");
                                rfpIdLabel.Content = materialReleasePermits[i].material_release_items[j].rfp_info.rfpID + "-" + materialReleasePermits[i].material_release_items[j].rfp_info.work_form;
                                Grid.SetRow(rfpIdLabel, card.Children.Count);
                                Grid.SetColumn(rfpIdLabel, 0);
                                card.Children.Add(rfpIdLabel);
                            }
                            

                            
                        }
                        else if(materialReleasePermits[i].material_release_items[j].materialItemGenericCategory.category_id != null)
                        {
                            if (j > 0 && materialReleasePermits[i].material_release_items[j].materialItemGenericCategory.category_id == materialReleasePermits[i].material_release_items[j - 1].materialItemGenericCategory.category_id
                                   && materialReleasePermits[i].material_release_items[j].materialitemGenericproduct.type_id == materialReleasePermits[i].material_release_items[j - 1].materialitemGenericproduct.type_id
                                   && materialReleasePermits[i].material_release_items[j].materialItemGenericBrand.brand_id == materialReleasePermits[i].material_release_items[j - 1].materialItemGenericBrand.brand_id
                                   && materialReleasePermits[i].material_release_items[j].materialItemGenericModel.model_id == materialReleasePermits[i].material_release_items[j - 1].materialItemGenericModel.model_id
                                  )
                            {
                                releasedQuantity++;
                                continue;
                            }
                            else if(j>0)
                            {
                                card.RowDefinitions.Add(new RowDefinition());
                                Label itemsLabel = new Label();
                                itemsLabel.Margin = new Thickness(0, 0, 0, 10);
                                itemsLabel.Style = (Style)FindResource("stackPanelItemBody");
                                itemsLabel.Content = $@"Item {itemNo += 1}: {materialReleasePermits[i].material_release_items[j].materialItemcompanyCategory.category_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialitemCompanyproduct.product_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanyBrand.brand_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanyModel.model_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanySpecs.spec_name}";

                                releasedQuantity = 1; 

                                quantity.Margin = new Thickness(5);
                                quantity.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                                quantity.Background = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                                quantity.BorderThickness = new Thickness(1);
                                quantity.CornerRadius = new CornerRadius(10);
                                quantity.HorizontalAlignment = HorizontalAlignment.Right;
                                quantity.Width = 170;


                                quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                                quantityLabel.Style = (Style)FindResource("stackPanelItemHeader");
                                quantityLabel.Foreground = Brushes.White;
                                quantityLabel.FontSize = 13;
                                quantityLabel.HorizontalAlignment = HorizontalAlignment.Center;

                                quantity.Child = quantityLabel;

                                releasedQuantity = 1;

                                card.RowDefinitions.Add(new RowDefinition());
                                Border status = new Border();
                                status.Margin = new Thickness(5);
                                status.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFF00"));
                                status.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFF00"));
                                status.BorderThickness = new Thickness(1);
                                status.CornerRadius = new CornerRadius(20);
                                status.HorizontalAlignment = HorizontalAlignment.Center;

                                Label statusLabel = new Label();
                                statusLabel.Content = materialReleasePermits[i].material_release_items[j].release_permit_item_status_name;
                                statusLabel.Style = (Style)FindResource("stackPanelItemHeader");
                                statusLabel.Foreground = Brushes.White;
                                statusLabel.HorizontalAlignment = HorizontalAlignment.Center;

                                status.Child = statusLabel;

                                Grid.SetRow(itemsLabel, card.Children.Count);
                                Grid.SetColumn(itemsLabel, 0);

                                Grid.SetRow(status, card.Children.Count);
                                Grid.SetColumn(status, 1);

                                card.Children.Add(itemsLabel);
                                card.Children.Add(status);

                                Grid.SetRow(quantity, card.Children.Count);
                                Grid.SetColumn(quantity, 1);

                                card.Children.Add(quantity);
                            }
                            if (materialReleasePermits[i].material_release_items[j].rfp_info.rfpID == "")
                            {
                                continue;
                            }
                            if (materialReleasePermits[i].material_release_items[j].materialItemGenericCategory.category_name == "")
                                continue;
                            if (j==0)
                            {
                                card.RowDefinitions.Add(new RowDefinition());
                                Label rfpIdLabel = new Label();
                                rfpIdLabel.Margin = new Thickness(0, 0, 0, 10);
                                rfpIdLabel.Style = (Style)FindResource("stackPanelItemBody");
                                rfpIdLabel.Content = materialReleasePermits[i].material_release_items[j].rfp_info.rfpID + "-" + materialReleasePermits[i].material_release_items[j].rfp_info.work_form;
                                Grid.SetRow(rfpIdLabel, card.Children.Count);
                                Grid.SetColumn(rfpIdLabel, 0);
                                card.Children.Add(rfpIdLabel);

                                card.RowDefinitions.Add(new RowDefinition());
                                Label itemsLabel = new Label();
                                itemsLabel.Margin = new Thickness(0, 0, 0, 10);
                                itemsLabel.Style = (Style)FindResource("stackPanelItemBody");
                                itemsLabel.Content = $@"Item {itemNo += 1}: {materialReleasePermits[i].material_release_items[j].materialItemcompanyCategory.category_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialitemCompanyproduct.product_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanyBrand.brand_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanyModel.model_name} - "
                                                       + $@"{materialReleasePermits[i].material_release_items[j].materialItemCompanySpecs.spec_name}";

                                quantity.Margin = new Thickness(5);
                                quantity.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                                quantity.Background = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                                quantity.BorderThickness = new Thickness(1);
                                quantity.CornerRadius = new CornerRadius(10);
                                quantity.HorizontalAlignment = HorizontalAlignment.Right;
                                quantity.Width = 170;


                                quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                                quantityLabel.Style = (Style)FindResource("stackPanelItemHeader");
                                quantityLabel.Foreground = Brushes.White;
                                quantityLabel.FontSize = 13;
                                quantityLabel.HorizontalAlignment = HorizontalAlignment.Center;

                                quantity.Child = quantityLabel;

                                releasedQuantity = 1;

                                card.RowDefinitions.Add(new RowDefinition());
                                Border status = new Border();
                                status.Margin = new Thickness(5);
                                status.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFF00"));
                                status.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFF00"));
                                status.BorderThickness = new Thickness(1);
                                status.CornerRadius = new CornerRadius(20);
                                status.HorizontalAlignment = HorizontalAlignment.Center;

                                Label statusLabel = new Label();
                                statusLabel.Content = materialReleasePermits[i].material_release_items[j].release_permit_item_status_name;
                                statusLabel.Style = (Style)FindResource("stackPanelItemHeader");
                                statusLabel.Foreground = Brushes.White;
                                statusLabel.HorizontalAlignment = HorizontalAlignment.Center;

                                status.Child = statusLabel;

                                Grid.SetRow(itemsLabel, card.Children.Count);
                                Grid.SetColumn(itemsLabel, 0);

                                Grid.SetRow(status, card.Children.Count);
                                Grid.SetColumn(status, 1);

                                card.Children.Add(itemsLabel);
                                card.Children.Add(status);

                                Grid.SetRow(quantity, card.Children.Count);
                                Grid.SetColumn(quantity, 1);

                                card.Children.Add(quantity);
                            }
                           

                            
                        }
                    }
                    else
                    {

                        

                        if(j>0 && materialReleasePermits[i].material_release_items[j].orderCategory.category_id == materialReleasePermits[i].material_release_items[j-1].orderCategory.category_id
                               && materialReleasePermits[i].material_release_items[j].orderproduct.type_id == materialReleasePermits[i].material_release_items[j-1].orderproduct.type_id
                               && materialReleasePermits[i].material_release_items[j].orderBrand.brand_id == materialReleasePermits[i].material_release_items[j-1].orderBrand.brand_id
                               && materialReleasePermits[i].material_release_items[j].orderModel.model_id == materialReleasePermits[i].material_release_items[j-1].orderModel.model_id
                               && materialReleasePermits[i].material_release_items[j].orderSpecs.spec_id == materialReleasePermits[i].material_release_items[j-1].orderSpecs.spec_id)
                        {
                            releasedQuantity++;
                            quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                            continue;

                        }
                        else if(j>0)
                        {
                           

                            card.RowDefinitions.Add(new RowDefinition());
                            Label itemsLabel = new Label();
                            itemsLabel.Margin = new Thickness(0, 0, 0, 10);
                            itemsLabel.Style = (Style)FindResource("stackPanelItemBody");
                            itemsLabel.Content = $@"Item {itemNo += 1}: {materialReleasePermits[i].material_release_items[j].orderCategory.category_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderproduct.product_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderBrand.brand_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderModel.model_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderSpecs.spec_name}";

                            releasedQuantity = 1;

                            card.RowDefinitions.Add(new RowDefinition());

                            quantity = new Border();
                            quantity.Margin = new Thickness(5);
                            quantity.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                            quantity.Background = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                            quantity.BorderThickness = new Thickness(1);
                            quantity.CornerRadius = new CornerRadius(10);
                            quantity.HorizontalAlignment = HorizontalAlignment.Right;
                            quantity.Width = 170;

                            quantityLabel = new Label();
                            quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                            quantityLabel.Style = (Style)FindResource("stackPanelItemHeader");
                            quantityLabel.Foreground = Brushes.White;
                            quantityLabel.FontSize=13;
                            quantityLabel.HorizontalAlignment = HorizontalAlignment.Center;

                            quantity.Child = quantityLabel;

                            releasedQuantity = 1;

                            Border status = new Border();
                            status.Margin = new Thickness(5);
                            status.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FDB813"));
                            status.Background = (Brush)(new BrushConverter().ConvertFrom("#FDB813"));
                            status.BorderThickness = new Thickness(1);
                            status.CornerRadius = new CornerRadius(10);
                            status.HorizontalAlignment = HorizontalAlignment.Right;

                            Label statusLabel = new Label();
                            statusLabel.Content = materialReleasePermits[i].material_release_items[j].release_permit_item_status_name;
                            statusLabel.Style = (Style)FindResource("stackPanelItemHeader");
                            statusLabel.Foreground = Brushes.White;
                            statusLabel.HorizontalAlignment = HorizontalAlignment.Center;

                            status.Child = statusLabel;




                            Grid.SetRow(itemsLabel, card.Children.Count);
                            Grid.SetColumn(itemsLabel, 0);

                            Grid.SetRow(status, card.Children.Count);
                            Grid.SetColumn(status, 2);

                            Grid.SetRow(quantity, card.Children.Count);
                            Grid.SetColumn(quantity, 1);

                            card.Children.Add(itemsLabel);
                            card.Children.Add(status);
                            card.Children.Add(quantity);
                        }
                       
                        if (materialReleasePermits[i].material_release_items[j].work_order_id == "")
                        {
                            continue;
                        }
                        if (materialReleasePermits[i].material_release_items[j].orderCategory.category_name == "")
                            continue;
                        if (j==0)
                        {
                            card.RowDefinitions.Add(new RowDefinition());
                            Label workOrderLabel = new Label();
                            workOrderLabel.Margin = new Thickness(0, 0, 0, 10);
                            workOrderLabel.Style = (Style)FindResource("stackPanelItemBody");
                            workOrderLabel.Content = $@"Order: {materialReleasePermits[i].material_release_items[j].work_order_id}";

                         

                            

                            Grid.SetRow(workOrderLabel, card.Children.Count);
                            Grid.SetColumn(workOrderLabel, 0);
                            card.Children.Add(workOrderLabel);

                            //Grid.SetRow(quantity, card.Children.Count-1);
                            //Grid.SetColumn(quantity, 2);
                            //card.Children.Add(quantity);



                            card.RowDefinitions.Add(new RowDefinition());
                            Label itemsLabel = new Label();
                            itemsLabel.Margin = new Thickness(0, 0, 0, 10);
                            itemsLabel.Style = (Style)FindResource("stackPanelItemBody");
                            itemsLabel.Content = $@"Item {itemNo += 1}: {materialReleasePermits[i].material_release_items[j].orderCategory.category_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderproduct.product_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderBrand.brand_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderModel.model_name} - "
                                                   + $@"{materialReleasePermits[i].material_release_items[j].orderSpecs.spec_name}";

                            card.RowDefinitions.Add(new RowDefinition());

                            
                            quantity.Margin = new Thickness(5);
                            quantity.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                            quantity.Background = (Brush)(new BrushConverter().ConvertFrom("#105A97"));
                            quantity.BorderThickness = new Thickness(1);
                            quantity.CornerRadius = new CornerRadius(10);
                            quantity.HorizontalAlignment = HorizontalAlignment.Right;
                            quantity.Width = 170;

                            //quantityLabel = new Label();
                            quantityLabel.Content = "Released Quantity: " + releasedQuantity;
                            quantityLabel.Style = (Style)FindResource("stackPanelItemHeader");
                            quantityLabel.Foreground = Brushes.White;
                            quantityLabel.FontSize = 13;
                            quantityLabel.HorizontalAlignment = HorizontalAlignment.Center;

                            quantity.Child = quantityLabel;

                            releasedQuantity = 1;

                            Border status = new Border();
                            status.Margin = new Thickness(5);
                            status.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FDB813"));
                            status.Background = (Brush)(new BrushConverter().ConvertFrom("#FDB813"));
                            status.BorderThickness = new Thickness(1);
                            status.CornerRadius = new CornerRadius(10);
                            status.HorizontalAlignment = HorizontalAlignment.Right;

                            Label statusLabel = new Label();
                            statusLabel.Content = materialReleasePermits[i].material_release_items[j].release_permit_item_status_name;
                            statusLabel.Style = (Style)FindResource("stackPanelItemHeader");
                            statusLabel.Foreground = Brushes.White;
                            statusLabel.HorizontalAlignment = HorizontalAlignment.Center;

                            status.Child = statusLabel;




                            Grid.SetRow(itemsLabel, card.Children.Count);
                            Grid.SetColumn(itemsLabel, 0);

                            Grid.SetRow(status, card.Children.Count);
                            Grid.SetColumn(status, 2);

                            Grid.SetRow(quantity, card.Children.Count);
                            Grid.SetColumn(quantity, 1);

                            card.Children.Add(itemsLabel);
                            card.Children.Add(status);
                            card.Children.Add(quantity);
                        }
                       


                       
                    }
                    

                }
                StackPanel expand = new StackPanel();

                Button editButton = new Button();
                editButton.Content = "EDIT";
                editButton.Click += EditButtonClick;
                editButton.Tag = materialReleasePermits[i].release_Permit_Serial;


                Button viewButton = new Button();
                viewButton.Click += ViewButton_Click;
                viewButton.Tag = materialReleasePermits[i].release_Permit_Serial;
                viewButton.Content = "VIEW";

                expand.Children.Add(viewButton);
                expand.Children.Add(editButton);

                Expander expander = new Expander();
                expander.Expanded += ExpanderExpanded;
                expander.Content = expand;
                expander.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(expander, 0);
                Grid.SetColumn(expander, 2);

                card.Children.Add(expander);

                cardBorder.Child = card;
                ReleasePermitStackPanel.Children.Add(cardBorder);
            }

            //for (int i = 0; i < materialReleasePermits.Count; i++)
            //{


            //    Grid card = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
            //    card.MouseEnter += CardMouseEnter;
            //    card.MouseLeave += CardMouseLeave;

            //    card.Tag = materialReleasePermits[i].release_Permit_Serial;

            //    card.RowDefinitions.Add(new RowDefinition());
            //    card.RowDefinitions.Add(new RowDefinition());
            //    card.RowDefinitions.Add(new RowDefinition());

            //    card.ColumnDefinitions.Add(new ColumnDefinition());
            //    card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });



            //    Label header = new Label();
            //    header.Content = $@"Release Permit - {materialReleasePermits[i].release_Permit_Id}";

            //    header.Style = (Style)FindResource("stackPanelItemHeader");

            //    card.Children.Add(header);

            //    Grid.SetRow(header, 0);

            //    Grid.SetColumn(header, 0);



            //    Label transactionDate = new Label() { Margin = new Thickness(0, 0, 0, 5) };
            //    transactionDate.Content = $"Transaction Date : {materialReleasePermits[i].release_date.ToString("yyyy-MM-dd")}";

            //    transactionDate.Style = (Style)FindResource("stackPanelItemBody");

            //    card.Children.Add(transactionDate);

            //    Grid.SetRow(transactionDate, 1);

            //    Grid.SetColumn(transactionDate, 0);

            //    if (materialReleasePermits[i].)
            //    Label workFrom = new Label();
            //    workFrom.Content = $"Work Form : {materialReleasePermits[i].}";

            //    workFrom.Style = (Style)FindResource("stackPanelItemBody");
            //    workFrom.HorizontalAlignment = HorizontalAlignment.Left;

            //    card.Children.Add(workFrom);

            //    Grid.SetRow(workFrom, 2);

            //    Grid.SetColumn(workFrom, 0);


            //    StackPanel expand = new StackPanel();



            //    Button editButton = new Button();

            //    editButton.Content = "EDIT";

            //    editButton.Click += EditButtonClick;

            //    editButton.Tag = materialReleasePermits[i].release_Permit_Serial;


            //    Button viewButton = new Button();

            //    viewButton.Click += ViewButton_Click;
            //    viewButton.Tag = materialReleasePermits[i].release_Permit_Serial;

            //    viewButton.Content = "VIEW";

            //    expand.Children.Add(viewButton);
            //    expand.Children.Add(editButton);

            //    Expander expander = new Expander();

            //    expander.Expanded += ExpanderExpanded;

            //    expander.Content = expand;


            //    Grid.SetRow(expander, 0);
            //    Grid.SetColumn(expander, 1);

            //    card.Children.Add(expander);

            //    ReleasePermitStackPanel.Children.Add(card);
            //}




        }

        private void CardMouseLeave(object sender, MouseEventArgs e)
        {
            Grid card = sender as Grid;
            if (card.IsMouseOver == false &&ReleasePermitStackPanel.IsMouseOver==false) {

                Label serialPrevious = previousGrid.Children[0] as Label;
                Label transactionDatePrevious = previousGrid.Children[1] as Label;
                Label nickNamePrevious = previousGrid.Children[2] as Label;

                previousGrid.Background = Brushes.White;
                serialPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                transactionDatePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                nickNamePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }
            
        }

        private void ExpanderExpanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;

            if (previousExpander != null && previousExpander != expander)
            {

                previousExpander.IsExpanded = false;
            }

            previousExpander = expander;

        }

        private void CardMouseEnter(object sender, MouseEventArgs e)
        {

            if (previousGrid != null)
            {

                Label serialPrevious = previousGrid.Children[0] as Label;
                Label transactionDatePrevious = previousGrid.Children[1] as Label;
                Label nickNamePrevious = previousGrid.Children[2] as Label;

                previousGrid.Background = Brushes.White;
                serialPrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                transactionDatePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

                nickNamePrevious.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            }

            Grid card = sender as Grid;
            Label serial = card.Children[0] as Label;
            Label transactionDate = card.Children[1] as Label;
            Label warehouseNiceName = card.Children[2] as Label;


            BrushConverter brush = new BrushConverter();
            card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            serial.Foreground = Brushes.White;
            transactionDate.Foreground = Brushes.White;
            warehouseNiceName.Foreground = Brushes.White;

            previousGrid = card;
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button viewButton = sender as Button;
            int materialReleasePermitSerial = Convert.ToInt32(viewButton.Tag.ToString());
            viewAddCondition = COMPANY_WORK_MACROS.VIEW_RELEASE;
            materialRelease.SetReleaseSerial(materialReleasePermitSerial);
            materialRelease.InitializeMaterialReleasePermit();
            addReleasePermitWindow = new AddReleasePermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, materialRelease, viewAddCondition);
            addReleasePermitWindow.Show();
            //View(Convert.ToInt32(viewButton.Tag));



        }

        //public void View(int releasePermitSerial) {

        //    MaterialReleasePermits materialReleasePermit = new MaterialReleasePermits();

        //    materialReleasePermit.SetReleaseSerial(Convert.ToInt32(releasePermitSerial));

        //    materialReleasePermit.InitializeMaterialReleasePermit();

        //    List<INVENTORY_STRUCTS.MATERIAL_RELEASE_PERMIT_ITEM> releaseItems = materialReleasePermit.GetReleaseItems();


        //     addReleasePermitWindow = new AddReleasePermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, materialReleasePermit, true);




        //    addReleasePermitWindow.releasePermitPage.MaterialRecieverComboBox.SelectedItem = materialReleasePermit.GetMaterialReceiver().employee_name;
        //    addReleasePermitWindow.releasePermitPage.ReleaseDatePicker.Text = Convert.ToString(materialReleasePermit.GetReleaseDate());


        //    addReleasePermitWindow.releasePermitPage.SerialIdTextBox.Text = materialReleasePermit.GetReleaseId();


        //    addReleasePermitWindow.releasePermitPage.SerialIdTextBox.IsReadOnly = true;
        //    addReleasePermitWindow.releasePermitPage.ReleaseDatePicker.IsEnabled = false;
        //    addReleasePermitWindow.releasePermitPage.MaterialRecieverComboBox.IsEnabled = false;


        //    //addReleasePermitWindow.releasePermitPage.rfpChecked.IsEnabled = false;
        //    //addReleasePermitWindow.releasePermitPage.orderChecked.IsEnabled = false;

        //    //addReleasePermitWindow.releasePermitPage.mainPanel.IsEnabled = false;

        //    if (releaseItems[0].rfp_info.rfpSerial != 0)
        //    {

        //        //addReleasePermitWindow.releasePermitPage.rfpChecked.IsChecked = true;


        //        addReleasePermitWindow.releasePermitPage.rfpRequesters.SelectedItem = addReleasePermitWindow.releasePermitPage.requstersFiltered.FirstOrDefault(a => a.requestor_team.team_id == releaseItems[0].rfp_info.rfpRequestorTeam).requestor_team.team_name;
        //        addReleasePermitWindow.releasePermitPage.rfpSerials.SelectedItem = addReleasePermitWindow.releasePermitPage.rfps.FirstOrDefault(a => a.rfpSerial == releaseItems[0].rfp_info.rfpSerial).rfpID;


        //    }
        //    else
        //    {

        //        //addReleasePermitWindow.releasePermitPage.orderChecked.IsChecked = true;

        //        addReleasePermitWindow.releasePermitPage.orderSerials.SelectedItem = addReleasePermitWindow.releasePermitPage.workOrders.FirstOrDefault(a => a.order_serial == releaseItems[0].workOrder_serial).order_id;

        //        addReleasePermitWindow.releasePermitPage.orderSerials.IsEnabled = false;

        //        addReleasePermitWindow.releasePermitPage.contactComboBox.IsEnabled = false;

        //        if (addReleasePermitWindow.releasePermitPage.contactComboBox.Items.Count == 1)
        //            addReleasePermitWindow.releasePermitPage.contactComboBox.SelectedIndex = 0;

        //    }


        //    addReleasePermitWindow.releasePermitItemPage.addRecieval.Click += OnAddRecievalClick;
        //    addReleasePermitWindow.releasePermitItemPage.addReEntry.Click += OnAddReEntryClick;


        //    WrapPanel choicePanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[0] as WrapPanel;
        //    ComboBox choiceComboBox = choicePanel.Children[1] as ComboBox;
        //    choiceComboBox.IsEnabled = false;
        //    choicePanel.Visibility = Visibility.Collapsed;


        //    WrapPanel genericCategoryPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[1] as WrapPanel;
        //    ComboBox genericCategoryComboBox = genericCategoryPanel.Children[1] as ComboBox;
        //    genericCategoryComboBox.IsEnabled = false;
        //    genericCategoryPanel.Visibility = Visibility.Collapsed;


        //    WrapPanel genericProductanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[2] as WrapPanel;
        //    ComboBox genericProductComboBox = genericProductanel.Children[1] as ComboBox;
        //    genericProductComboBox.IsEnabled = false;

        //    genericProductanel.Visibility = Visibility.Collapsed;



        //    WrapPanel genericBrandPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[3] as WrapPanel;
        //    ComboBox genericBrandComboBox = genericBrandPanel.Children[1] as ComboBox;
        //    genericBrandComboBox.IsEnabled = false;

        //    genericBrandPanel.Visibility = Visibility.Collapsed;



        //    WrapPanel genericModelPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[4] as WrapPanel;
        //    ComboBox genericModelComboBox = genericModelPanel.Children[1] as ComboBox;
        //    genericModelComboBox.IsEnabled = false;

        //    genericModelPanel.Visibility = Visibility.Collapsed;




        //    //WrapPanel companyCategoryPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[5] as WrapPanel;
        //    //ComboBox companyCategoryComboBox = companyCategoryPanel.Children[1] as ComboBox;
        //    //companyCategoryComboBox.IsEnabled = false;

        //    //companyCategoryPanel.Visibility = Visibility.Collapsed;

        //    WrapPanel companySpecsPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[5] as WrapPanel;
        //    ComboBox companySpecsComboBox = companySpecsPanel.Children[1] as ComboBox;
        //    companySpecsComboBox.IsEnabled = false;
        //    companySpecsPanel.Visibility = Visibility.Collapsed;
        //    //WrapPanel companyProductPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[6] as WrapPanel;
        //    //ComboBox companyProductComboBox = companyProductPanel.Children[1] as ComboBox;
        //    //companyProductComboBox.IsEnabled = false;

        //    //companyProductPanel.Visibility = Visibility.Collapsed;





        //    //WrapPanel companyBrandPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[7] as WrapPanel;
        //    //ComboBox companyBrandComboBox = companyBrandPanel.Children[1] as ComboBox;
        //    //companyBrandComboBox.IsEnabled = false;

        //    //companyBrandPanel.Visibility = Visibility.Collapsed;



        //    //WrapPanel companyModelPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[8] as WrapPanel;
        //    //ComboBox companyModelComboBox = companyModelPanel.Children[1] as ComboBox;
        //    //companyModelComboBox.IsEnabled = false;

        //    //companyModelPanel.Visibility = Visibility.Collapsed;




        //    //



        //    WrapPanel selectedItemsPanel = addReleasePermitWindow.releasePermitItemPage.Home.Children[6] as WrapPanel;
        //    Label selectedItemslabel = selectedItemsPanel.Children[1] as Label;

        //    if (releaseItems.Count != 0)
        //        selectedItemslabel.Content = $"{releaseItems.Count}";

        //    Grid items = addReleasePermitWindow.releasePermitItemPage.Home.Children[addReleasePermitWindow.releasePermitItemPage.Home.Children.Count - 1] as Grid;
        //    items.Background = Brushes.White;

        //    items.Children.Clear();
        //    items.RowDefinitions.Clear();



        //    Grid Itemsheader = new Grid();

        //    Itemsheader.ShowGridLines = true;

        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(500)});
        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition());
        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
        //    Itemsheader.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});


        //    Grid checkAllGrid = new Grid();
        //    checkAllGrid.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
        //    checkAll = new CheckBox();

        //    checkAll.Foreground= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        //    checkAllGrid.Children.Add(checkAll);

        //    checkAll.Style = (Style)FindResource("checkBoxStyle");

        //    checkAll.Content = "SELECT ALL";
        //    checkAll.Checked += OnCheckAllChecked;
        //    checkAll.Unchecked += CheckAll_Unchecked;



        //    Grid.SetColumn(checkAllGrid, 0);

        //    Itemsheader.Children.Add(checkAllGrid);



        //    Label entryPermitSerialId = new Label();

        //    entryPermitSerialId.Style = (Style)FindResource("tableHeaderItem");

        //    entryPermitSerialId.Content = "SERIAL ID";

        //    Grid.SetColumn(entryPermitSerialId, 1);

        //    Itemsheader.Children.Add(entryPermitSerialId);



        //    Label itemNameLabel = new Label();

        //    itemNameLabel.Style = (Style)FindResource("tableHeaderItem");

        //    itemNameLabel.Content = "ITEM NAME";

        //    Grid.SetColumn(itemNameLabel, 2);

        //    Itemsheader.Children.Add(itemNameLabel);


        //    Label serialNumber = new Label();

        //    serialNumber.Style = (Style)FindResource("tableHeaderItem");

        //    serialNumber.Content = "SERIAL NUMBER";

        //    Grid.SetColumn(serialNumber, 3);

        //    Itemsheader.Children.Add(serialNumber);





        //    Label quantity = new Label();

        //    quantity.Style = (Style)FindResource("tableHeaderItem");

        //    quantity.Content = "Quantity";


        //    Grid.SetColumn(quantity, 4);

        //    Itemsheader.Children.Add(quantity);



        //    Label status = new Label();

        //    status.Style = (Style)FindResource("tableHeaderItem");

        //    status.Content = "STATUS";


        //    Grid.SetColumn(status, 5);

        //    Itemsheader.Children.Add(status);



            


        //    items.RowDefinitions.Add(new RowDefinition());

        //    Grid.SetRow(Itemsheader, items.RowDefinitions.Count - 1);
        //    items.Children.Add(Itemsheader);


        //    ScrollViewer scroll = new ScrollViewer();
        //    scroll.CanContentScroll = true;
        //    scroll.Height = 600;
        //    Grid itemsBody = new Grid();

        //    itemsBody.ShowGridLines = true;

        //    scroll.Content = itemsBody;

        //    items.RowDefinitions.Add(new RowDefinition());

        //    Grid.SetRow(scroll, items.RowDefinitions.Count - 1);
        //    items.Children.Add(scroll);
        //    Grid itemm = null;




        //    for (int i = 0; i < releaseItems.Count; i++)
        //    {


        //        itemsBody.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });


        //        itemm = new Grid();

        //        itemm.Tag = materialReleasePermit.GetReleaseId() +" "+ releaseItems[i].release_permit_item_serial;
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition());
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition());
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(500) });
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition() );
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
        //        itemm.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) }) ;


        //        gridItems.Add(itemm);

        //        CheckBox checkItem = new CheckBox();


        //        checkItem.Style = (Style)FindResource("checkBoxStyle");

        //        checkItem.Checked += OnCheckItemChecked;

        //        checkItem.Unchecked += OnCheckItemUnchecked;




        //        Grid.SetColumn(checkItem, 0);

        //        itemm.Children.Add(checkItem);



        //        Label entryPermitSerialLabel = new Label();


        //        entryPermitSerialLabel.Style = (Style)FindResource("tableItemLabel");

        //        entryPermitSerialLabel.Content = releaseItems[i].material_entry_permit_id;

        //        entryPermitSerialLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));



        //        Grid.SetColumn(entryPermitSerialLabel, 1);

        //        itemm.Children.Add(entryPermitSerialLabel);


        //        entryPermitSerialLabel.HorizontalAlignment = HorizontalAlignment.Left;



        //        TextBlock ItemName = new TextBlock();
        //        ItemName.TextWrapping = TextWrapping.Wrap;

        //        ItemName.HorizontalAlignment = HorizontalAlignment.Left;

        //        ItemName.VerticalAlignment = VerticalAlignment.Top;


        //        ItemName.Style = (Style)FindResource("cardTextBlockStyle");

        //        ItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

        //        if (releaseItems[i].materialItemGenericCategory.category_name != "")
        //            ItemName.Text = $"{releaseItems[i].materialItemGenericCategory.category_name + "-" + releaseItems[i].materialitemGenericproduct.product_name + "-" + releaseItems[i].materialItemGenericBrand.brand_name + "-" + releaseItems[i].materialItemGenericModel.model_name}";
        //        else
        //            ItemName.Text = $"{releaseItems[i].materialItemcompanyCategory.category_name + "-" + releaseItems[i].materialitemCompanyproduct.product_name + "-" + releaseItems[i].materialItemCompanyBrand.brand_name + "-" + releaseItems[i].materialItemCompanyModel.model_name}";




        //        Grid.SetColumn(ItemName, 2);

        //        itemm.Children.Add(ItemName);





        //        if (releaseItems[i].entryPermit_product_serial_number != "")
        //        {

        //            itemm.ShowGridLines = true;


        //            CheckBox chooseItemSerialCheckBox = new CheckBox();

        //            chooseItemSerialCheckBox.Content = releaseItems[i].entryPermit_product_serial_number;

        //            chooseItemSerialCheckBox.HorizontalAlignment = HorizontalAlignment.Left;



        //            chooseItemSerialCheckBox.Style = (Style)FindResource("checkBoxStyle");
        //            chooseItemSerialCheckBox.IsChecked = true;
        //            chooseItemSerialCheckBox.IsEnabled = false;


        //            Grid.SetColumn(chooseItemSerialCheckBox, 3);

        //            itemm.Children.Add(chooseItemSerialCheckBox);


        //        }

        //        else
        //        {


        //            Grid quantityGrid = new Grid();

        //            quantityGrid.HorizontalAlignment = HorizontalAlignment.Left;
        //            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //            quantityGrid.ColumnDefinitions.Add(new ColumnDefinition());


        //            TextBox quantityAvailableTextBox = new TextBox();

        //            quantityAvailableTextBox.Tag = releaseItems[i].entered_quantity - releaseItems[i].released_quantity_entryItem;


        //            quantityAvailableTextBox.Style = (Style)FindResource("filterTextBoxStyle");

        //            quantityAvailableTextBox.Text = $"{releaseItems[i].entered_quantity - releaseItems[i].released_quantity_entryItem}";
        //            quantityAvailableTextBox.IsEnabled = false;


        //            TextBox quantityTextBox = new TextBox();

        //            quantityTextBox.Style = (Style)FindResource("filterTextBoxStyle");


        //            quantityTextBox.IsReadOnly = true;


        //            CheckBox quantityCheckBox = new CheckBox();

        //            quantityCheckBox.Width = 30;

        //            quantityCheckBox.Style = (Style)FindResource("checkBoxStyle");

        //            quantityCheckBox.IsChecked = true;
        //            quantityCheckBox.IsEnabled = false;


        //            Grid.SetColumn(quantityCheckBox, 0);

        //            Grid.SetColumn(quantityAvailableTextBox, 1);

        //            Grid.SetColumn(quantityTextBox, 2);


        //            quantityGrid.Children.Add(quantityCheckBox);
        //            quantityGrid.Children.Add(quantityAvailableTextBox);
        //            quantityGrid.Children.Add(quantityTextBox);



        //            Grid.SetColumn(quantityGrid, 4);

        //            itemm.Children.Add(quantityGrid);

        //        }

        //        //itemm.Children.Add(ReEntryCheckBox);
        //        //itemm.Children.Add(recievalNoteCheckBox);

        //        List<BASIC_STRUCTS.STATUS_STRUCT> releasePermitStatuses = new List<BASIC_STRUCTS.STATUS_STRUCT>();

        //        commonQueries.GetReleasePermitItemStatuses(ref releasePermitStatuses);


        //        TextBlock statusName = new TextBlock();

        //        statusName.HorizontalAlignment = HorizontalAlignment.Center;

        //        statusName.VerticalAlignment = VerticalAlignment.Top;

        //        statusName.FontWeight= FontWeights.DemiBold;
        //        statusName.FontSize = 20;
        //        statusName.Foreground = Brushes.White;
        //        statusName.Margin = new Thickness(10,0,0,0);

        //        statusName.Text= "NULL";

        //        if (releaseItems[i].release_permit_item_status!=0)
        //            statusName.Text = releasePermitStatuses.First(a => a.status_id == releaseItems[i].release_permit_item_status).status_name;


        //        Border borderStatusName = new Border();

        //        borderStatusName.CornerRadius = new CornerRadius(13);

        //        borderStatusName.BorderThickness = new Thickness(1);

        //        borderStatusName.Margin = new Thickness(5);

        //        borderStatusName.HorizontalAlignment = HorizontalAlignment.Stretch;

        //        if (releaseItems[i].release_permit_item_status == COMPANY_WORK_MACROS.PENDING_EMPLOYEE_RECIEVAL || releaseItems[i].release_permit_item_status == COMPANY_WORK_MACROS.PENDING_CLIENT_RECIEVAL)
        //            borderStatusName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBF00"));
        //        else if (releaseItems[i].release_permit_item_status == COMPANY_WORK_MACROS.RECIEVED_BY_CLIENT || releaseItems[i].release_permit_item_status == COMPANY_WORK_MACROS.RECIEVED_BY_EMPLOYEE)
        //            borderStatusName.Background = Brushes.Green;
        //        else
        //            borderStatusName.Background = Brushes.Red;

        //        borderStatusName.Child = statusName;

        //        Grid.SetColumn(borderStatusName, 5);

        //        itemm.Children.Add(borderStatusName);


        //        Grid.SetRow(itemm, itemsBody.RowDefinitions.Count - 1);
        //        itemsBody.Children.Add(itemm);


        //        Grid card = new Grid() { Margin = new Thickness(15, 30, 15, 30) };

        //        card.Width = 500;
        //        card.Height = 500;

        //        card.Background = Brushes.White;

        //        card.IsEnabled = false;


        //        card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
        //        card.RowDefinitions.Add(new RowDefinition());


        //        Grid header = new Grid() { };
        //        header.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

        //        TextBlock itemHeader = new TextBlock();

        //        header.Children.Add(itemHeader);

        //        itemHeader.Style = (Style)FindResource("cardTextBlockStyle");
        //        itemHeader.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));

        //        itemHeader.FontSize = 16;


        //        if (releaseItems[i].materialItemGenericCategory.category_name != "")
        //            itemHeader.Text = $"{releaseItems[i].materialItemGenericCategory.category_name + "-" + releaseItems[i].materialitemGenericproduct.product_name + "-" + releaseItems[i].materialItemGenericBrand.brand_name + "-" + releaseItems[i].materialItemGenericModel.model_name + " -" + releaseItems[i].entryPermit_product_serial_number}";
        //        else
        //            itemHeader.Text = $"{releaseItems[i].materialItemcompanyCategory.category_name + "-" + releaseItems[i].materialitemCompanyproduct.product_name + "-" + releaseItems[i].materialItemCompanyBrand.brand_name + "-" + releaseItems[i].materialItemCompanyModel.model_name + " -" + releaseItems[i].entryPermit_product_serial_number}";

        //        itemHeader.Foreground = Brushes.White;

        //        itemHeader.HorizontalAlignment = HorizontalAlignment.Center;

        //        Grid.SetRow(header, 0);




        //        Grid order = new Grid() { Margin = new Thickness(0, -40, 0, 0) };


        //        Grid.SetRow(order, 1);


        //        card.Children.Add(header);
        //        card.Children.Add(order);


        //        if (releaseItems[i].rfp_info.rfpSerial != 0)
        //        {


        //            order.Children.Clear();
        //            order.RowDefinitions.Clear();


        //            order.RowDefinitions.Add(new RowDefinition());
        //            order.RowDefinitions.Add(new RowDefinition());
        //            order.RowDefinitions.Add(new RowDefinition());



        //            ComboBox rfpsItemsComboBox = new ComboBox();

        //            rfpsItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
        //            rfpsItemsComboBox.IsEnabled = false;






        //            foreach (PROCUREMENT_STRUCTS.RFP_ITEM_MIN_STRUCT rfpItem in addReleasePermitWindow.releasePermitPage.rfpItems)
        //            {

        //                if (rfpItem.product_category.category_name == "")
        //                {

        //                    rfpsItemsComboBox.Items.Add(rfpItem.product_category.category_name + " ," + rfpItem.product_type.product_name + " ," + rfpItem.product_brand.brand_name + " ," + rfpItem.product_model.model_name);
        //                }

        //                else
        //                {

        //                    rfpsItemsComboBox.Items.Add(rfpItem.product_category.category_name + " ," + rfpItem.product_type.product_name + " ," + rfpItem.product_brand.brand_name + " ," + rfpItem.product_model.model_name);
        //                }
        //            }

        //            int index = 0;
        //            for (int m = 0; m < addReleasePermitWindow.releasePermitPage.rfpItems.Count; m++)
        //            {

        //                if (addReleasePermitWindow.releasePermitPage.rfpItems[m].rfp_item_number == releaseItems[i].rfp_item_number)
        //                {

        //                    index = m;
        //                    break;
        //                }

        //            }
        //            rfpsItemsComboBox.SelectedIndex = index;

        //            //continue here
        //            ComboBox locations = new ComboBox();
        //            locations.Style = (Style)FindResource("comboBoxStyle");
        //            locations.IsEnabled = false;
        //            locations.Visibility = Visibility.Visible;





        //            TextBox projectName = new TextBox();
        //            projectName.Style = (Style)FindResource("filterTextBoxStyle");
        //            projectName.IsReadOnly = true;
        //            projectName.Visibility = Visibility.Visible;

        //            projectName.Width = 200;




        //            TextBox identityTextBox = new TextBox();
        //            identityTextBox.Style = (Style)FindResource("filterTextBoxStyle");
        //            identityTextBox.IsReadOnly = true;
        //            identityTextBox.Visibility = Visibility.Visible;



        //            Grid textBoxesGrid = new Grid();

        //            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //            textBoxesGrid.ColumnDefinitions.Add(new ColumnDefinition());


        //            Grid.SetColumn(identityTextBox, 0);
        //            Grid.SetColumn(projectName, 1);

        //            textBoxesGrid.Children.Add(identityTextBox);
        //            textBoxesGrid.Children.Add(projectName);



        //            Grid.SetRow(rfpsItemsComboBox, 0);

        //            Grid.SetRow(locations, 1);

        //            Grid.SetRow(textBoxesGrid, 2);


        //            order.Children.Add(rfpsItemsComboBox);

        //            order.Children.Add(locations);

        //            order.Children.Add(textBoxesGrid);


        //        }

        //        else
        //        {


        //            order.RowDefinitions.Add(new RowDefinition());
        //            order.RowDefinitions.Add(new RowDefinition());



        //            PRODUCTS_STRUCTS.ORDER_PRODUCT_STRUCT[] products = addReleasePermitWindow.releasePermitPage.workOrder.GetOrderProductsList();


        //            ComboBox orderItemsComboBox = new ComboBox();
        //            orderItemsComboBox.Style = (Style)FindResource("comboBoxStyle");
        //            orderItemsComboBox.IsEnabled = false;



        //            for (int j = 0; j < products.Length; j++)
        //            {
        //                orderItemsComboBox.Items.Add(products[j].product_category.category_name + " ," + products[j].productType.product_name + " ," + products[j].productBrand.brand_name + " ," + products[j].productModel.model_name);

        //            }


        //            orderItemsComboBox.SelectedItem = releaseItems[i].orderCategory.category_name + " ," + releaseItems[i].orderproduct.product_name + " ," + releaseItems[i].orderBrand.brand_name + " ," + releaseItems[i].orderModel.model_name;

        //            ComboBox orderLocationsComboBox = new ComboBox();
        //            orderLocationsComboBox.Style = (Style)FindResource("comboBoxStyle");
        //            orderLocationsComboBox.IsEnabled = false;

        //            if (releaseItems[i].order_project_serial != 0)
        //            {

        //                List<BASIC_STRUCTS.ADDRESS_STRUCT> addresses = addReleasePermitWindow.releasePermitPage.workOrder.GetProjectLocations();


        //                for (int j = 0; j < addresses.Count; j++)
        //                {

        //                    orderLocationsComboBox.Items.Add(addresses[i].district.district_name + " ," + addresses[i].city.city_name + " ," + addresses[i].state_governorate.state_name + " ," + addresses[i].country.country_name);


        //                }


        //                orderLocationsComboBox.SelectedItem = releaseItems[i].orderLocation.district.district_name + " ," + releaseItems[i].orderLocation.city.city_name + " ," + releaseItems[i].orderLocation.state_governorate.state_name + " ," + releaseItems[i].orderLocation.country.country_name;


        //            }


        //            Grid.SetRow(orderItemsComboBox, 0);

        //            Grid.SetRow(orderLocationsComboBox, 1);



        //            order.Children.Add(orderItemsComboBox);

        //            order.Children.Add(orderLocationsComboBox);



        //        }

        //        addReleasePermitWindow.releasePermitItemPage.checkedItemsWrapPanel.Children.Add(card);

        //    }


        //    addReleasePermitWindow.Show();


        //}

        private void OnAddReEntryClick(object sender, RoutedEventArgs e)
        {
            RentryPermitWindow rentryPermitWindow = new RentryPermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, false);

            rentryPermitWindow.addRentryPermitPage.releasePermitsComboBox.SelectedItem = gridItems[0].Tag.ToString().Split(' ')[0];
            rentryPermitWindow.addRentryPermitPage.releasePermitsComboBox.IsEnabled = false;


            for (int i = 1; i < rentryPermitWindow.addRentryPermitPage.ReleasePermitItemsGrid.Children.Count; i++)
            {


                Grid item = rentryPermitWindow.addRentryPermitPage.ReleasePermitItemsGrid.Children[i] as Grid;
                int entryItemSerial = Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);

                CheckBox serialCheckBox = item.Children[2] as CheckBox;
                serialCheckBox.IsChecked = true;

                if (checkedReleasePermititemSerials.Exists(a => a == entryItemSerial))
                    continue;

                rentryPermitWindow.addRentryPermitPage.ReleasePermitItemsGrid.Children.Remove(rentryPermitWindow.addRentryPermitPage.ReleasePermitItemsGrid.Children[i]);

            }


            rentryPermitWindow.Show();

        }

        private void OnAddRecievalClick(object sender, RoutedEventArgs e)
        {
            RecievalNoteWindow recievalNoteWindow = new RecievalNoteWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser, false);

            recievalNoteWindow.addRecievalNoteItemPage.releasePermitsComboBox.SelectedItem = gridItems[0].Tag.ToString().Split(' ')[0];
            recievalNoteWindow.addRecievalNoteItemPage.releasePermitsComboBox.IsEnabled = false;


            for (int i = 1; i < recievalNoteWindow.addRecievalNoteItemPage.ReleasePermitItemsGrid.Children.Count; i++) {


               Grid item= recievalNoteWindow.addRecievalNoteItemPage.ReleasePermitItemsGrid.Children[i] as Grid;
               int entryItemSerial= Convert.ToInt32(item.Tag.ToString().Split(' ')[1]);

              CheckBox serialCheckBox=item.Children[2] as CheckBox;
                serialCheckBox.IsChecked = true;

                if (checkedReleasePermititemSerials.Exists(a => a == entryItemSerial))
                    continue;

                recievalNoteWindow.addRecievalNoteItemPage.ReleasePermitItemsGrid.Children.Remove(recievalNoteWindow.addRecievalNoteItemPage.ReleasePermitItemsGrid.Children[i]);

            }


            recievalNoteWindow.Show();
        }

        private void OnCheckItemUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox itemCheckBox = sender as CheckBox;
            Grid item = itemCheckBox.Parent as Grid;

            checkedReleasePermititemSerials.Remove(int.Parse(item.Tag.ToString().Split(' ')[1]));

            itemsCount--;

            if (itemsCount < gridItems.Count)
                checkAll.IsChecked = false;

        }

        private void OnCheckItemChecked(object sender, RoutedEventArgs e)
        {
            CheckBox itemCheckBox= sender as CheckBox;

            Grid item= itemCheckBox.Parent as Grid;

            if(checkedReleasePermititemSerials.Exists(a=> a==int.Parse(item.Tag.ToString().Split(' ')[1]))==false)
            checkedReleasePermititemSerials.Add(int.Parse(item.Tag.ToString().Split(' ')[1]));

            itemsCount++;

            if (itemsCount == gridItems.Count)
                checkAll.IsChecked = true;

        }

        private void CheckAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (itemsCount < gridItems.Count)
                return;

            for (int i = 0; i < gridItems.Count; i++)
            {

                CheckBox itemCheckBox = gridItems[i].Children[0] as CheckBox;
                itemCheckBox.IsChecked = false;


            }

            checkedReleasePermititemSerials.Clear();
        }

        private void OnCheckAllChecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < gridItems.Count; i++) {

               CheckBox itemCheckBox= gridItems[i].Children[0] as CheckBox;
                if(itemCheckBox.IsChecked==false)
                checkedReleasePermititemSerials.Add(int.Parse(gridItems[i].Tag.ToString().Split(' ')[1]));

                itemCheckBox.IsChecked = true;
                

            }
        }

        private void OnRecievalNoteCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox recievalNoteCheckBox = sender as CheckBox;

            checkedItems--;

            if (checkedItems == -1)
                addReleasePermitWindow.releasePermitPage.addReleasePermitItem.finishButton.IsEnabled = false;

            recievalNote.Remove(recievalNoteCheckBox.Content.ToString());

        }

        private void OnRecievalNoteCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox recievalNoteCheckBox = sender as CheckBox;

            checkedItems++;

            bool exists = ReEntry.Exists(a => a == recievalNoteCheckBox.Content.ToString());

            if (exists == false)
            {

                addReleasePermitWindow.releasePermitPage.addReleasePermitItem.finishButton.IsEnabled = true;

                recievalNote.Add(recievalNoteCheckBox.Content.ToString());
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Cannot re_enter and add receivalNote in an item", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                recievalNoteCheckBox.IsChecked = false;
            }


        }

        private void OnReEntryCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox reEntryCheckBox = sender as CheckBox;

            checkedItems--;

            if(checkedItems==-1)
                addReleasePermitWindow.releasePermitPage.addReleasePermitItem.finishButton.IsEnabled = false;

            ReEntry.Remove(reEntryCheckBox.Content.ToString());

        }

        private void OnReEntryCheckBoxChecked(object sender, RoutedEventArgs e)
        {
           CheckBox reEntryCheckBox =sender as CheckBox;

            checkedItems++;


            bool exists = recievalNote.Exists(a => a == reEntryCheckBox.Content.ToString());


            if (exists == false)
            {

                addReleasePermitWindow.releasePermitPage.addReleasePermitItem.finishButton.IsEnabled = true;

                ReEntry.Add(reEntryCheckBox.Content.ToString());
            }

            else
            {
                System.Windows.Forms.MessageBox.Show("Cannot re_enter and add receivalNote in an item", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                reEntryCheckBox.IsChecked = false;
            }

        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            AddReleasePermitWindow releasePermitWindow = new AddReleasePermitWindow(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser,null,viewAddCondition);

            releasePermitWindow.Show();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickedDashboard(object sender, MouseButtonEventArgs e)
        {
            DashboardPage dashboardPagePage = new DashboardPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(dashboardPagePage);

        }
        private void OnButtonClickedRFPs(object sender, MouseButtonEventArgs e)
        {
            RFPsPage rfpPage = new RFPsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(rfpPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }
        private void OnButtonClickedCompanyProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage categoriesPage = new CategoriesPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(categoriesPage);
        }
        private void OnButtonClickedGenericProducts(object sender, MouseButtonEventArgs e)
        {
            GenericProductsPage genericProductsPage = new GenericProductsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(genericProductsPage);
        }
        private void OnButtonClickedStoreLocations(object sender, MouseButtonEventArgs e)
        {
            WarehouseLocationsPage genericProductsPage = new WarehouseLocationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(genericProductsPage);
        }
        private void OnButtonClickedEntryPermits(object sender, MouseButtonEventArgs e)
        {
            EntryPermitPage entryPermitPage = new EntryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser );
            this.NavigationService.Navigate(entryPermitPage);

        }
        private void OnButtonClickedReleasePermits(object sender, MouseButtonEventArgs e)
        {
            ReleasePermitPage releasePermitPage = new ReleasePermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(releasePermitPage);
        }
        private void OnButtonClickedStockAvailability(object sender, MouseButtonEventArgs e)
        {
            StockAvailabilityPage stockAvailabilityPage = new StockAvailabilityPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);

            this.NavigationService.Navigate(stockAvailabilityPage);
        }
        private void OnButtonClickedRentryPermit(object sender, MouseButtonEventArgs e)
        {
            RentryPermitPage rentryPermitPage = new RentryPermitPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(rentryPermitPage);
        }
        private void OnButtonClickedReservations(object sender, MouseButtonEventArgs e)
        {
            ReservationsPage reservationsPage = new ReservationsPage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(reservationsPage);
        }
        private void OnButtonClickedRecievalNotes(object sender, MouseButtonEventArgs e)
        {
            RecievalNotePage recievalNotePage = new RecievalNotePage(ref commonQueries, ref commonFunctions, ref integrityChecks, ref loggedInUser);
            this.NavigationService.Navigate(recievalNotePage);
        }
    }
}
