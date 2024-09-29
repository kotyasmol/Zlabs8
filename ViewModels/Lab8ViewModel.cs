using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab8ViewModel : INotifyPropertyChanged
    {
        private string _selectedFilePath;
        private string _key;
        private string _statusMessage;

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
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

        public ICommand SelectFileCommand { get; }
        public ICommand EncryptCommand { get; }

        public Lab8ViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            EncryptCommand = new RelayCommand(EncryptFile);
        }

        private void SelectFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; // Выбор всех файлов

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName; // Устанавливаем путь к выбранному файлу
            }
        }

        private void EncryptFile()
        {
            if (string.IsNullOrEmpty(SelectedFilePath) || string.IsNullOrEmpty(Key) || Key.Length < 6)
            {
                StatusMessage = "Пожалуйста, выберите файл и введите ключ длиной не менее 6 символов.";
                return;
            }

            var fileData = File.ReadAllBytes(SelectedFilePath);
            var keyBytes = Encoding.UTF8.GetBytes(Key);
            byte[] encryptedData = new byte[fileData.Length];

            // Генерируем ключ для шифрования
            byte[] generatedKey = GenerateKey(fileData.Length, keyBytes);

            // Шифрование методом однократного гаммирования
            for (int i = 0; i < fileData.Length; i++)
            {
                encryptedData[i] = (byte)(fileData[i] ^ generatedKey[i]);
            }

            // Сохранение зашифрованного файла
            string encryptedFilePath = Path.Combine(Path.GetDirectoryName(SelectedFilePath), "encrypted_file.bin");
            File.WriteAllBytes(encryptedFilePath, encryptedData);
            StatusMessage = "Файл успешно зашифрован и сохранен как 'encrypted_file.bin'.";
        }

        private byte[] GenerateKey(int length, byte[] key)
        {
            byte[] fullKey = new byte[length];
            for (int i = 0; i < length; i++)
            {
                fullKey[i] = key[i % key.Length]; // Повторяем ключ при необходимости
            }
            return fullKey;
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
