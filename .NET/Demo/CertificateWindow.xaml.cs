﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for CertificateWindow.xaml
    /// </summary>
    public partial class CertificateWindow : Window
    {
        public CertificateWindow(Action<string, string> action)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
        }
    }
}
