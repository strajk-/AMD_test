using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AMD_test;

namespace AMD_test
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent ();
        }


        private void Test_Click(object sender, RoutedEventArgs e)
        {
            test.IsEnabled = false;
            lb.Items.Clear ();
            Application.Current.Dispatcher.Invoke (DispatcherPriority.Background, new Action (delegate { }));
            DateTime dt1 = DateTime.Now;
            DateTime dt2;
            //
            // run test 5 times
            //
            for (int i = 0; i < 5; ++i) {
                // 
                // stage1
                //
                XReflection.Init1 ();
                dt2 = DateTime.Now;
                lb.Items.Add (string.Format ("{0} Init1 : duration {1}", i, (dt2 - dt1).ToString ()));
                Application.Current.Dispatcher.Invoke (DispatcherPriority.Background, new Action (delegate { }));
                dt1 = dt2;
                //
                // stage2
                //
                XReflection.Init2 ();
                dt2 = DateTime.Now;
                lb.Items.Add (string.Format ("{0} Init2 : duration {1}", i, (dt2 - dt1).ToString ()));
                Application.Current.Dispatcher.Invoke (DispatcherPriority.Background, new Action (delegate { }));
                dt1 = dt2;
                //
                // with a Intel System all the times are nearly the same between 1 and 2 seconds
                //
                // with a AMD Ryzen System after the 3 Run the time for the stage2 rises to nearly 20 seconds
                // and rests at this level
                //
            }
            test.IsEnabled = true;
        }
    }
}


