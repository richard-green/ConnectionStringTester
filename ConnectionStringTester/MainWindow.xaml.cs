using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConnectionStringTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtConnectionString.Text = "Data Source=(local); Initial Catalog=master; User=sa; Password=sa";
            ChangeConnectionString(txtConnectionString.Text);
        }

        #region User Events

        private void txtConnectionString_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionString(txtConnectionString.Text);
        }

        private void txtDataSource_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "Data Source", txtDataSource.Text);
        }

        private void txtInitialCatalog_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "Initial Catalog", txtInitialCatalog.Text);
        }

        private void txtUser_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "User", txtUser.Text);
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "Password", txtPassword.Text);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = txtConnectionString.Text;
            var dataSource = txtDataSource.Text;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                    }

                    MessageBox.Show("Connection successfully established to " + dataSource, "Test successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to connect to " + dataSource, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void lblConnectionString_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtConnectionString.Text);
        }

        private void lblDataSource_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtDataSource.Text);
        }

        private void lblInitialCatalog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtInitialCatalog.Text);
        }

        private void lblUser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtUser.Text);
        }

        private void lblPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtPassword.Text);
        }

        #endregion User Events

        #region Methods

        private void ChangeConnectionString(string newConnectionString)
        {
            DisplayParts(newConnectionString);
        }

        private void ChangeConnectionStringPart(string connectionString, string partName, string partValue)
        {
            List<string> newParts = new List<string>();

            var parts = connectionString.Split(';');
            bool updated = false;

            foreach (var part in parts.Select(s => s.Trim()).Where(s => s.Length > 0 && s.IndexOf('=') > 0))
            {
                var partBits = part.Split('=');

                if ( partBits[0].Equals(partName, StringComparison.CurrentCultureIgnoreCase)
                     ||
                     (partName.Equals("User", StringComparison.CurrentCultureIgnoreCase) &&
                      partBits[0].Equals("User ID", StringComparison.CurrentCultureIgnoreCase)))
                {
                    newParts.Add(string.Format("{0}={1}", partBits[0], partValue));
                    updated = true;
                }
                else
                {
                    newParts.Add(string.Format("{0}={1}", partBits[0], partBits[1]));
                }
            }

            if (updated == false)
            {
                newParts.Add(string.Format("{0}={1}", partName, partValue));
            }

            txtConnectionString.Text = string.Join("; ", newParts);
        }

        private void DisplayParts(string newConnectionString)
        {
            var parts = newConnectionString.Split(';');

            txtDataSource.Text = "";
            txtInitialCatalog.Text = "";
            txtUser.Text = "";
            txtPassword.Text = "";

            foreach (var part in parts)
            {
                var partBits = part.Split('=');

                switch (partBits[0].Trim().ToLower())
                {
                    case "data source":
                        txtDataSource.Text = partBits[1];
                        break;
                    case "initial catalog":
                        txtInitialCatalog.Text = partBits[1];
                        break;
                    case "user":
                    case "user id":
                        txtUser.Text = partBits[1];
                        break;
                    case "password":
                        txtPassword.Text = partBits[1];
                        break;
                }
            }
        }

        #endregion Methods
    }
}
