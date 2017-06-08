using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SourceCounterEx.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Xml;

namespace SourceCounterEx.View
{
   

    public partial class MainWindow 
    {


        public static readonly DependencyProperty IsReportVisibleProperty =
            DependencyProperty.Register("IsReportVisible", typeof(Boolean), typeof(MainWindow), new PropertyMetadata(true));

        public Boolean IsReportVisible
        {
            get { return (Boolean)GetValue(IsReportVisibleProperty); }
            set { SetValue(IsReportVisibleProperty, value); }
        }


        public static readonly DependencyProperty IsStepVisibleProperty =
           DependencyProperty.Register("IsStepVisible", typeof(Boolean), typeof(MainWindow), new PropertyMetadata(true));
        public Boolean IsStepVisible
        {
            get { return (Boolean)GetValue(IsStepVisibleProperty); }
            set { SetValue(IsStepVisibleProperty, value); }
        }

        public MainWindow()
        {
            this.MetroDialogOptions.ColorScheme =  MetroDialogColorScheme.Accented;
            this.DataContext = new MainViewModel(DialogCoordinator.Instance,this);
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            //外部ファイルより設定可能
            XmlDataProvider xdp = this.TryFindResource("lstVerNo") as XmlDataProvider;
            if (xdp != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("VerNo.xml");
                xdp.Document = doc;
                //xdp.XPath = "VerNos/VerNo";
            }

            



        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowTop(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(0);
        }
        private void TopFlyoutCloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(0);
        }
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
            

        }
    }
}
