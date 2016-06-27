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
    public sealed partial class DetailsTran : Page
    {
        private Transaction _transaction;
        CoreApplicationView view;

        public DetailsTran()
        {
            this.InitializeComponent();
            view = CoreApplication.GetCurrentView();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, args) =>
            {
                args.Handled = true;
                Frame.Navigate(typeof(MainPage));
            };

            if (e.Parameter != null)
            {
                var navigationData = (object[])e.Parameter;
                _transaction = (Transaction)navigationData[0];
            }

            DataContext = _transaction;
            fadeInStoryboard.Begin();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialogBox = new ContentDialogEdit();
            dialogBox.DataContext = (Transaction)DataContext;

            var result = await dialogBox.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var db = new DatabaseHelper();
                db.UpdateTransaction(_transaction);
                MessageDialog msg = new MessageDialog("Changes saved!");
                await msg.ShowAsync();
            }
            DataContext = _transaction;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ConfirmationDialog = new MessageDialog("Are you sure you want to delete " + _transaction.Title + "?");
            ConfirmationDialog.Commands.Add(new UICommand("Delete"));
            ConfirmationDialog.Commands.Add(new UICommand("Cancel"));
            ConfirmationDialog.DefaultCommandIndex = 0;
            ConfirmationDialog.CancelCommandIndex = 1;
            var result = await ConfirmationDialog.ShowAsync();
            if (result.Label.Equals("Delete"))
            {
                var db = new DatabaseHelper();              

                string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                string path = root + @"\Assets";
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (!string.IsNullOrWhiteSpace(_transaction.PhotoPath))
                {
                   string fileName = _transaction.PhotoPath.Replace(path + "\\", "");
                   var file = await folder.GetFileAsync(fileName);
                   await file.DeleteAsync();
                }
                db.DeleteTransaction(_transaction.Id);
                Frame.Navigate(typeof(MainPage));
            }

        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
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
                StorageFile storageFile = args.Files[0];
                string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                string path = root + @"\Assets";
                var copyFolder = await StorageFolder.GetFolderFromPathAsync(path);

                if (!string.IsNullOrWhiteSpace(_transaction.PhotoPath))
                {
                    string fileName = _transaction.PhotoPath.Replace(path + "\\", "");
                    var oldFile = await copyFolder.GetFileAsync(fileName);
                    await oldFile.DeleteAsync();
                }
                int counter = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["counter"];
                counter = counter + 1;
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

                _transaction.PhotoPath = file.Path;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["counter"] = counter;
                imgPhoto.Source = new BitmapImage(new Uri(@_transaction.PhotoPath, UriKind.RelativeOrAbsolute));
                var db = new DatabaseHelper();
                db.UpdateTransaction(_transaction);
                MessageDialog msg = new MessageDialog("Picture updated!");
                await msg.ShowAsync();
                DataContext = _transaction;
            }
        }
    }
}
