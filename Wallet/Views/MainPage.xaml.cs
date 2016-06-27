using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModel;
using Wallet.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Wallet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vWallet = new VirtualWallet();
            vWallet.FillTransactions();
            DataContext = vWallet;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"] != null)
                {
                if (!string.IsNullOrWhiteSpace(Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"].ToString()))
                    SummaryHub.Header = "Welcome back " + Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"] + "!";
                else
                    SummaryHub.Header = "Welcome back User!";

            }
            else
                SummaryHub.Header = "Welcome back User!";

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["reorder"] != null)
            {
                if ((bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["reorder"])
                    btnReorder.Visibility = Visibility.Visible;
                else
                    btnReorder.Visibility = Visibility.Collapsed;
            }
        }

        private void btnToAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTran));
        }

        private void lvAllTrans_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(DetailsTran), new object[] {(Transaction) e.ClickedItem });
        }

        private async void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ConfirmationDialog = new MessageDialog("Are you sure you want to delete all transactions?");
            ConfirmationDialog.Commands.Add(new UICommand("Delete"));
            ConfirmationDialog.Commands.Add(new UICommand("Cancel"));
            ConfirmationDialog.DefaultCommandIndex = 0;
            ConfirmationDialog.CancelCommandIndex = 1;
            var result = await ConfirmationDialog.ShowAsync();

            if (result.Label.Equals("Delete"))
            {
                var db = new DatabaseHelper();
                db.DeleteTransactions();
                string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                string path = root + @"\Assets";
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                foreach (var f in files)
                {
                    if (f.Name.Contains("PHOTO"))
                    await f.DeleteAsync();
                }
                Border br = FindChildControl<Border>(SummaryHub, "brBalance") as Border;
                br.Background = new SolidColorBrush(Colors.AliceBlue);
                Frame.Navigate(typeof(MainPage));

            }
        }

        private void btnReorder_Click(object sender, RoutedEventArgs e)
        {
           ListView ls = FindChildControl<ListView>(SimpleListHub, "lvAllTrans") as ListView;
            if (ls.ReorderMode == ListViewReorderMode.Enabled)
                ls.ReorderMode = ListViewReorderMode.Disabled;
            else
                ls.ReorderMode = ListViewReorderMode.Enabled;
        }

        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;

                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    return child;
                }
                else
                {
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Border br = FindChildControl<Border>(SummaryHub, "brBalance") as Border;
            TextBlock tbl = FindChildControl<TextBlock>(SummaryHub, "tblSum") as TextBlock;
            SolidColorBrush slb = FindChildControl<SolidColorBrush>(SummaryHub, "a") as SolidColorBrush;

            if (double.Parse(tbl.Text) > 0)
            br.Background = new SolidColorBrush(Colors.LightGreen);
            else if (double.Parse(tbl.Text) == 0)
                br.Background = new SolidColorBrush(Colors.AliceBlue);
            else
                br.Background = new SolidColorBrush(Colors.LightPink);

            Storyboard s = new Storyboard();
            DoubleAnimation dbl = new DoubleAnimation();
            dbl.EnableDependentAnimation = true;
            dbl.From = 112;
            dbl.To = 90;
            dbl.AutoReverse = true;
            //dbl.RepeatBehavior = new RepeatBehavior(2);
            dbl.Duration = new Duration(TimeSpan.FromMilliseconds(600));
            Storyboard.SetTarget(dbl, br);
            Storyboard.SetTargetProperty(dbl, "Height");
            s.Children.Add(dbl);
            s.Begin();

            var story = new Storyboard();
            DoubleAnimation animWidth = new DoubleAnimation();
            animWidth.EnableDependentAnimation = true;
            animWidth.From = 112;
            animWidth.To = 90;
            animWidth.AutoReverse = true;
            //animWidth.RepeatBehavior = new RepeatBehavior(2);
            animWidth.Duration = new Duration(TimeSpan.FromMilliseconds(600));
            Storyboard.SetTarget(animWidth, br);
            Storyboard.SetTargetProperty(animWidth, "Width");
            story.Children.Add(animWidth);
            story.Begin();


        }
    }
}
