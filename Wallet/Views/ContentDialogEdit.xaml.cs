using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ContentDialogEdit : ContentDialog
    {
        private Transaction _transaction;
        private string _temp;
        private string _comment;
        private string _date;
        string format;
        CultureInfo provider = CultureInfo.InvariantCulture;
        private double _amount;
        ComboBoxItem boxItem;

        public ContentDialogEdit()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            _transaction = (Transaction)DataContext;
            _temp = _transaction.Title;
            _amount = _transaction.Amount;
            _comment = _transaction.Comment;
            _date = _transaction.Date;
            format = "dd/MM/yyyy";
            dpDate.Date = DateTime.ParseExact(_date, format, provider);
            if (_transaction.Category == "Income")
            {
                cbCategory.Visibility = Visibility.Collapsed;
                tbType.Text = _transaction.Category;
            }
            else
            {
                tbType.Text = "Expense";
                boxItem = (ComboBoxItem)cbCategory.FindName(_transaction.Category);
                boxItem.IsSelected = true;
                tbAmount.Text = (double.Parse(tbAmount.Text) * -1).ToString();
            }
            tbComment.Text = _transaction.Comment;


        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            boxItem = (ComboBoxItem)cbCategory.SelectedItem;
            if (!string.IsNullOrWhiteSpace(tbTitle.Text) && !string.IsNullOrWhiteSpace(tbAmount.Text))
            {
                string amountStr = tbAmount.Text.Replace(",", ".");
                try
                {
                    _transaction.Title = tbTitle.Text;
                    if (double.Parse(tbAmount.Text) > 0)
                    {
                        double amount = double.Parse(amountStr);
                        amountStr = string.Format("{0:0.##}", amount);
                        _transaction.Amount = double.Parse(amountStr);
                        _transaction.Date = string.Format("{0:dd/MM/yyyy}", dpDate.Date);
                        if (boxItem != null)
                        {
                            _transaction.Category = boxItem.Content.ToString();
                            _transaction.Amount = double.Parse(amountStr) * -1;
                            switch (boxItem.Content.ToString())
                            {
                                case "Home": _transaction.ImagePath = "/Assets/home.png"; break;
                                case "Other": _transaction.ImagePath = "/Assets/other.png"; break;
                                case "Medical": _transaction.ImagePath = "/Assets/heart.png"; break;
                                case "Entertainment": _transaction.ImagePath = "/Assets/beer.png"; break;
                            }
                        }
                        _transaction.Comment = tbComment.Text;
                    }
                    else
                    {
                        args.Cancel = true;
                        tbError.Text = "positive amount";
                    }
                }
                catch (FormatException ex)
                {
                    args.Cancel = true;
                    tbError.Text = "wrong format of amount";
                }
            }
            else
            {
                args.Cancel = true;
                _transaction.Title = _temp;
                _transaction.Amount = _amount;
                tbError.Text = "Not blanks are filled corectly";
            }
        }
        private void dpDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (dpDate.Date > DateTime.Today)
            {
                dpDate.Date = DateTime.Today;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            if (_transaction.Category != "Income")
                _transaction.Amount = _amount * -1;
            else
                _transaction.Amount = _amount;
            _transaction.Title = _temp;
            _transaction.Comment = _comment;
            _transaction.Date = _date;
            Hide();
        }
    }
}
