using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StyleAndTemplates
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Variable, which will remember the last user, who has logged in
        /// </summary>
        int toRemember;
        List<User> userList;

        public MainWindow()
        {
            InitializeComponent();
            userList = new List<User>();
            LoadSerialize();
            if (toRemember != -1 && userList.Count > toRemember)
            {
                textBoxUsername.Text = userList[toRemember].Name;
                textPassword.Password = userList[toRemember].Password;
                checkBoxRemember.IsChecked = true;
            }
        }

        #region Methods

        public void LoadSerialize()
        {
            if (!File.Exists("User.dat")) return;
            FileStream FS = new FileStream("User.dat", FileMode.Open, FileAccess.Read);
            BinaryFormatter BF = new BinaryFormatter();
            toRemember = (int)BF.Deserialize(FS);
            userList = BF.Deserialize(FS) as List<User>;
            FS.Flush();
            FS.Close();
        }

        public void SaveSerialize()
        {
            FileStream FS = new FileStream("User.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter BF = new BinaryFormatter();
            if (checkBoxRemember.IsChecked == false) toRemember = -1;
            BF.Serialize(FS, toRemember);
            BF.Serialize(FS, userList);
            FS.Flush();
            FS.Close();
        }

        /// <summary>
        /// Check for log or registration
        /// </summary>
        /// <returns></returns>
        private bool FieldCheck(bool reg_log)
        {
            if (textBoxUsername.Text.Length < 4 || textPassword.Password.Length < 4)
            {
                MessageBox.Show("Fields must have at least 4 characters!", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (reg_log == true)
            {
                if (userList.Find(o => o.Name == textBoxUsername.Text) != default
                    && userList.Find(o => o.Password == textPassword.Password) != default)
                {
                    MessageBox.Show("Login or password are already taken\nOr doesn't exist", "Warning", MessageBoxButton.OK,
                         MessageBoxImage.Warning);
                    return false;
                }
            }
            else
            {
                User toFind = userList.Find(o => o.Name == textBoxUsername.Text);
                if (toFind != default)
                {
                    if (toFind.Password == textPassword.Password)
                    {
                        toRemember = userList.IndexOf(toFind);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password!", "Warning", MessageBoxButton.OK,
                         MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Login or password are already taken or incorrect\nOr doesn't exist", "Warning", MessageBoxButton.OK,
                           MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Events

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (FieldCheck(true) == false) return;
            userList.Add(new User(textBoxUsername.Text, textPassword.Password));
            MessageBox.Show("Successful registration!", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            SaveSerialize();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (FieldCheck(false) == false) return;
            if (checkBoxRemember.IsChecked == false) toRemember = -1;
            Keyboard newKeyToWorkWith = new Keyboard();
            Hide();
            newKeyToWorkWith.Title += " | " + textBoxUsername.Text;
            newKeyToWorkWith.Tag = textBoxUsername.Text;
            newKeyToWorkWith.ShowDialog();
            Activate();
            Show();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSerialize();
        }

        #endregion
    }

    /// <summary>
    /// User class to work with
    /// </summary>
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public User(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
