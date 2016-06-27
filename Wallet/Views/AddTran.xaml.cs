using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallet.Helpers;
using Wallet.Models;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Wallet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTran : Page
    {
        Transaction transaction;
        CoreApplicationView view;

        public AddTran()
        {
            this.InitializeComponent();
            transaction = new Transaction();
            view = CoreApplication.GetCurrentView();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, args) =>
            {
                args.Handled = true;
                Frame.Navigate(typeof(MainPage));
            };

            fadeInStoryboard.Begin();

        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem boxItem = (ComboBoxItem)cbCategory.SelectedItem;
            if (boxItem != null && !String.IsNullOrWhiteSpace(tbTitle.Text) && !String.IsNullOrWhiteSpace(tbAmount.Text) && (rbExpense.IsChecked == true || rbIncome.IsChecked == true))
            {
                transaction.Title = tbTitle.Text;
                string amountStr = tbAmount.Text.Replace(",", ".");
                try
                {
                    if (double.Parse(tbAmount.Text) > 0)
                    {
                        double amount = double.Parse(amountStr);
                        amountStr = string.Format("{0:0.00}", amount);
                        transaction.Amount = double.Parse(amountStr) * -1;
                        transaction.Date = string.Format("{0:dd/MM/yyyy}", dpDate.Date);
                        if (rbIncome.IsChecked == true)
                        {
                            transaction.Category = "Income";
                            transaction.Amount = transaction.Amount * -1;
                            transaction.ImagePath = "/Assets/cash.png";
                        }
                        else
                        {
                            transaction.Category = boxItem.Content.ToString();
                            switch (boxItem.Content.ToString())
                            {
                                case "Home": transaction.ImagePath = "/Assets/home.png"; break;
                                case "Entertainment": transaction.ImagePath = "/Assets/beer.png"; break;
                                case "Medical": transaction.ImagePath = "/Assets/heart.png"; break;
                                case "Other": transaction.ImagePath = "/Assets/other.png"; break;
                            }
                        }
                        transaction.Comment = tbComment.Text;
                        var db = new DatabaseHelper();
                        db.AddTransaction(transaction);
                        Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        MessageDialog msg = new MessageDialog("TYpe in positive amount");
                        await msg.ShowAsync();
                    }
                }
                catch (FormatException ex)
                {
                    MessageDialog msg = new MessageDialog("Wrong amount");
                    await msg.ShowAsync();
                }
            }
            else
            {
                MessageDialog msg = new MessageDialog("You did not fill all the blanks");
                await msg.ShowAsync();
            }
        }

        private async void dpDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (dpDate.Date > DateTime.Today)
            {
                MessageDialog dateMsg = new MessageDialog("The date cannot be later than today");
                await dateMsg.ShowAsync();
                dpDate.Date = DateTime.Today;
            }
        }

        private void rbIncome_Checked(object sender, RoutedEventArgs e)
        {
            cbCategory.Visibility = Visibility.Collapsed;
            tblCategory.Visibility = Visibility.Collapsed;
        }

        private void rbExpense_Checked(object sender, RoutedEventArgs e)
        {
            tblCategory.Visibility = Visibility.Visible;
            cbCategory.Visibility = Visibility.Visible;
        }

        private void btnPicture_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;
        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;

                string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                string path = root + @"\Assets";
                var copyFolder = await StorageFolder.GetFolderFromPathAsync(path);

                if (!string.IsNullOrWhiteSpace(transaction.PhotoPath))
                {
                    string fileName = transaction.PhotoPath.Replace(path + "\\", "");
                    var oldFile = await copyFolder.GetFileAsync(fileName);
                    await oldFile.DeleteAsync();
                }

                int counter = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["counter"];
                counter = counter+1;
                StorageFile storageFile = args.Files[0];
                StorageFile file;

                try
                {
                    file = await copyFolder.GetFileAsync(storageFile.DisplayName + "PHOTO" + counter);
                    await file.DeleteAsync();
                    file = await storageFile.CopyAsync(copyFolder);
                    await file.RenameAsync(file.DisplayName + "PHOTO" + counter);
                }
                catch (FileNotFoundException)
                {

                    file = await storageFile.CopyAsync(copyFolder);
                    await file.RenameAsync(file.DisplayName + "PHOTO" + counter);
                }

                transaction.PhotoPath = file.Path;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["counter"] = counter;
                imgPhoto.Source = new BitmapImage(new Uri(@transaction.PhotoPath, UriKind.RelativeOrAbsolute));
            }
        }
    }
}

