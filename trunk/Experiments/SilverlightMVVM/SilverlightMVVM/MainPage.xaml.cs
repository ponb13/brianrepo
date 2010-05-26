using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightMVVM.Model;
using SilverlightMVVM.ViewModel;

namespace SilverlightMVVM
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            QuestionViewModel qData = new QuestionViewModel();

            qData.FetchQuestions();

            QuestionDataView.DataContext = qData.Questions; 
        }
    }
}
