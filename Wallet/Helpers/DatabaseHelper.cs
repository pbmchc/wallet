using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Models;

namespace Wallet.Helpers
{
    class DatabaseHelper
    {
        SQLiteConnection dbConn;

        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Transaction>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //SELECT SINGLE
        public Transaction GetSingleTransaction(int id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var singleTransaction = dbConn.Query<Transaction>("select * from 'Transaction' where Id =" + id).FirstOrDefault();
                return singleTransaction;
            }
        }

        //SELECT ALL
        public ObservableCollection<Transaction> GetAllTranactions()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Transaction> myTransactions = dbConn.Table<Transaction>().ToList();
                myTransactions.Reverse();
                ObservableCollection<Transaction> TransactionsList = new ObservableCollection<Transaction>(myTransactions);
                return TransactionsList;
            }
        }

        //UPDATE RECORD
        public void UpdateTransaction(Transaction transaction)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var selectedTransaction = dbConn.Query<Transaction>("select * from 'Transaction' where Id =" + transaction.Id).FirstOrDefault();
                if (selectedTransaction != null)
                {
                    selectedTransaction.Title = transaction.Title;
                    selectedTransaction.Category = transaction.Category;
                    selectedTransaction.Amount = transaction.Amount;
                    selectedTransaction.Comment = transaction.Comment;
                    selectedTransaction.ImagePath = transaction.ImagePath;
                    selectedTransaction.Date = transaction.Date;
                    selectedTransaction.PhotoPath = transaction.PhotoPath;

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(selectedTransaction);
                    });
                }
            }
        }

        //ADD RECORD
        public void AddTransaction(Transaction transaction)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(transaction);
                });
            }
        }

        //DELETE SINGLE 
        public void DeleteTransaction(int id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var transaction = dbConn.Query<Transaction>("select * from 'Transaction' where Id =" + id).FirstOrDefault();
                if (transaction != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(transaction);
                    });
                }
            }
        }

        //DELETE ALL
        public void DeleteTransactions()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<Transaction>();
                dbConn.CreateTable<Transaction>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        }

        //SUM ALL
        public double SumAllTransactions()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Transaction> myTransactions = dbConn.Table<Transaction>().ToList();
                double sum = myTransactions.Sum(t => t.Amount);
                return sum;
            }
        }
    }
}
