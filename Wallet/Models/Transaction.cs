using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Models
{
    public class Transaction : BindableBase
    {
        private int _id;
        private string _category;
        private string _title;
        private double _amount;
        private string _comment;
        private string _date;
        private string _imagePath;
        private string _photoPath;

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty(ref _imagePath, value); }
        }

        public string PhotoPath
        {
            get { return _photoPath; }
            set { SetProperty(ref _photoPath, value); }
        }

    }
}
