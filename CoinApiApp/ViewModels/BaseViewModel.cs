using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoinApiApp.ViewModels
{
    public class BaseViewModel // клас для відправки повідомлення про зміни
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // метод для виклику у set властивостях для зміни даних у UI
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
