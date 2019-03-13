using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaskHelper.Core.ViewModels
{
    public class BindableBase : INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
