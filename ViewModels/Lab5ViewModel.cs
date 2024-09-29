using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab5ViewModel : INotifyPropertyChanged
    {
        private string _inputBinary;
        private string _encodedBinary;
        private string _decodedBinary;
        private string _statusMessage;

        public string InputBinary
        {
            get => _inputBinary;
            set
            {
                _inputBinary = value;
                OnPropertyChanged(nameof(InputBinary));
            }
        }

        public string EncodedBinary
        {
            get => _encodedBinary;
            set
            {
                _encodedBinary = value;
                OnPropertyChanged(nameof(EncodedBinary));
            }
        }

        public string DecodedBinary
        {
            get => _decodedBinary;
            set
            {
                _decodedBinary = value;
                OnPropertyChanged(nameof(DecodedBinary));
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

        public ICommand EncodeCommand { get; }
        public ICommand DecodeCommand { get; }

        public Lab5ViewModel()
        {
            EncodeCommand = new RelayCommand(Encode);
            DecodeCommand = new RelayCommand(Decode);
        }

        private void Encode()
        {
            if (string.IsNullOrEmpty(InputBinary) || InputBinary.Length != 4 || !InputBinary.All(c => c == '0' || c == '1'))
            {
                StatusMessage = "Введите корректную двоичную последовательность длиной 4 бита.";
                return;
            }

            // Кодирование с помощью метода Хэмминга (7,4)
            var dataBits = InputBinary.Select(c => int.Parse(c.ToString())).ToArray();
            var parityBits = new int[3];

            // Установка данных в кодовую последовательность
            var codeBits = new int[7];
            codeBits[2] = dataBits[0];
            codeBits[4] = dataBits[1];
            codeBits[5] = dataBits[2];
            codeBits[6] = dataBits[3];

            // Вычисление контрольных битов
            parityBits[0] = (codeBits[2] + codeBits[4] + codeBits[6]) % 2; // p1
            parityBits[1] = (codeBits[2] + codeBits[5] + codeBits[6]) % 2; // p2
            parityBits[2] = (codeBits[4] + codeBits[5] + codeBits[6]) % 2; // p4

            codeBits[0] = parityBits[0]; // p1
            codeBits[1] = parityBits[1]; // p2
            codeBits[3] = parityBits[2]; // p4

            EncodedBinary = string.Join("", codeBits);
            StatusMessage = "Кодирование успешно!";
        }

        private void Decode()
        {
            if (string.IsNullOrEmpty(EncodedBinary) || EncodedBinary.Length != 7 || !EncodedBinary.All(c => c == '0' || c == '1'))
            {
                StatusMessage = "Введите корректную двоичную кодовую последовательность длиной 7 бит.";
                return;
            }

            var codeBits = EncodedBinary.Select(c => int.Parse(c.ToString())).ToArray();

            // Проверка и вычисление контрольных битов
            int p1 = (codeBits[0] + codeBits[2] + codeBits[4] + codeBits[6]) % 2;
            int p2 = (codeBits[1] + codeBits[2] + codeBits[5] + codeBits[6]) % 2;
            int p4 = (codeBits[3] + codeBits[4] + codeBits[5] + codeBits[6]) % 2;

            // Определение позиции ошибки
            int errorPosition = (p1 << 0) + (p2 << 1) + (p4 << 2);
            if (errorPosition > 0)
            {
                StatusMessage = $"Ошибка обнаружена в позиции: {errorPosition}. Исправляем...";
                codeBits[errorPosition - 1] ^= 1; // Исправляем ошибку
            }

            // Извлечение данных
            DecodedBinary = $"{codeBits[2]}{codeBits[4]}{codeBits[5]}{codeBits[6]}";
            StatusMessage += " Коррекция завершена!";
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
