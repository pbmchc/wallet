using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Helpers;
using Wallet.Models;

namespace Wallet.ViewModel
{
    public class VirtualWallet : BindableBase
    {
        ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();

        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value); }
        }

        List<KeyedList<string, Transaction>> _groupedTransactions;

        public List<KeyedList<string, Transaction>> GroupedTransactions
        {
            get
            {
                _groupedTransactions = (from b in Transactions
                                 orderby b.Category
                                 group b by b.Category into catTransactions
                                 select new KeyedList<string, Transaction>(catTransactions.Key, catTransactions)).ToList();
                return _groupedTransactions;
            }
        }

        double _summary;

        public double Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        public void FillTransactions()
        {
            var db = new DatabaseHelper();
            _transactions = db.GetAllTranactions();
            _summary = db.SumAllTransactions();
        }
    }
}
