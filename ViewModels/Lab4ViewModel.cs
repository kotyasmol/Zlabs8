using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab4ViewModel : INotifyPropertyChanged
    {
        private string _selectedFilePath;

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath)); // Уведомляем об изменении свойства
            }
        }
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public ICommand SelectFileCommand { get; }
        public ICommand EncryptCommand { get; }

        public Lab4ViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            EncryptCommand = new RelayCommand(EncryptFile);
        }

        // Реализация метода для выбора файла
        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName; // Присваиваем выбранный файл
            }
        }

        // Реализация метода для шифрования файла
        private void EncryptFile()
        {
            if (string.IsNullOrEmpty(SelectedFilePath))
            {
                // Логика, если файл не выбран
                return;
            }

            string encryptedFilePath = SelectedFilePath + ".encrypted";

            // Генерация ключа (8 байт для DES)
            byte[] key = GenerateRandomKey();

            // Выполнение шифрования DES
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = key;
                des.Mode = CipherMode.ECB; // Режим блочного шифрования
                des.Padding = PaddingMode.PKCS7; // Добавление отступов

                using (FileStream fsInput = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream fsEncrypted = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                using (ICryptoTransform encryptor = des.CreateEncryptor())
                using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, encryptor, CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[32 / 8]; // 32 бита (4 байта)
                    int bytesRead;

                    while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

        }

        // Метод для генерации случайного ключа (8 байт)
        private byte[] GenerateRandomKey()
        {
            byte[] key = new byte[8]; // 8 байт = 64 бита
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
