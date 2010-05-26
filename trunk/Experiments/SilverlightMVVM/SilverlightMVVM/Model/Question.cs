using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SilverlightMVVM.Model
{
    public class Question : INotifyPropertyChanged
    {

        public string Text { get; set; }
        public string ActualAnswer { get; set; }
        private string _provided;
        public string ProvidedAnswer
        {
            get
            {
                return _provided;
            }
            set
            {
                _provided = value;
                RaisePropertyChanged("ProvidedAnswer");
                RaisePropertyChanged("Grade");
            }
        }

        public bool Grade
        {
            get { return (ActualAnswer == _provided); }
            set
            {
                RaisePropertyChanged("Grade");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    } 
}
