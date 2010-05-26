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
using System.Collections.ObjectModel;
using SilverlightMVVM.Model;

namespace SilverlightMVVM.ViewModel
{
    public class QuestionViewModel
    {
        public ObservableCollection<Question> Questions { get; set; }

        public void FetchQuestions()
        {
            ObservableCollection<Question> q = new ObservableCollection<Question>();
            q.Add(new Question() { Text = "What is 2 + 2 ?", ActualAnswer = "4" });
            q.Add(new Question() { Text = "What is 9 - 2 ?", ActualAnswer = "7" });
            q.Add(new Question() { Text = "What is 2 - 2 ?", ActualAnswer = "0" });

            Questions = q;
        } 
    }
}
