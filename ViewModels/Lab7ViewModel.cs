using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab7ViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _hashedPassword;
        private string _statusMessage;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string HashedPassword
        {
            get => _hashedPassword;
            set
            {
                _hashedPassword = value;
                OnPropertyChanged(nameof(HashedPassword));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public ICommand HashPasswordCommand { get; }

        public Lab7ViewModel()
        {
            HashPasswordCommand = new RelayCommand(HashPassword);
        }

        private void HashPassword()
        {
            if (string.IsNullOrEmpty(Password) || Password.Length != 26)
            {
                StatusMessage = "Пароль должен состоять из 26 символов.";
                return;
            }

            HashedPassword = Convert.ToBase64String(HashWithDES(Encoding.UTF8.GetBytes(Password)));
            StatusMessage = "Пароль успешно хеширован!";
        }

        private byte[] HashWithDES(byte[] data)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                // Генерация ключа для DES
                des.Key = GenerateRandomKey(8); // 8 байт для DES
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = des.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        private byte[] GenerateRandomKey(int keySizeInBytes)
        {
            byte[] key = new byte[keySizeInBytes];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
