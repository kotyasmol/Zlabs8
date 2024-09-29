using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab3ViewModel : INotifyPropertyChanged
    {
        private string _inputText;
        private string _signatureResult;
        private string _errorMessage;

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public string SignatureResult
        {
            get => _signatureResult;
            set
            {
                _signatureResult = value;
                OnPropertyChanged(nameof(SignatureResult));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand GenerateSignatureCommand { get; }

        public Lab3ViewModel()
        {
            GenerateSignatureCommand = new RelayCommand(GenerateSignature);
        }

        private void GenerateSignature()
        {
            if (string.IsNullOrWhiteSpace(InputText) || InputText.Length < 256)
            {
                ErrorMessage = "Текст должен быть не менее 256 символов.";
                return;
            }

            try
            {
                // Генерация случайных простых чисел x и g (12 бит)
                BigInteger p = GenerateRandomPrime(12);
                BigInteger g = GenerateRandomPrime(12);

                // Выбор секретного ключа x
                BigInteger x = GenerateRandomPrime(12);

                // Открытый ключ: y = g^x mod p
                BigInteger y = BigInteger.ModPow(g, x, p);

                // Генерация случайного k, взаимно простого с (p-1)
                BigInteger k;
                do
                {
                    k = GenerateRandomPrime(12);
                } while (BigInteger.GreatestCommonDivisor(k, p - 1) != 1);

                // Подпись
                // 1. r = g^k mod p
                BigInteger r = BigInteger.ModPow(g, k, p);

                // 2. s = (H(m) - x * r) * k^(-1) mod (p - 1)
                BigInteger hash = GetMessageHash(InputText);
                BigInteger s = ((hash - x * r) * ModInverse(k, p - 1)) % (p - 1);

                // Формирование подписи
                SignatureResult = $"Подпись: (r = {r}, s = {s})";
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        // Метод генерации случайных простых чисел с заданной разрядностью
        private BigInteger GenerateRandomPrime(int bitLength)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[bitLength / 8];
                BigInteger number;

                do
                {
                    rng.GetBytes(bytes);
                    number = new BigInteger(bytes);
                } while (!IsPrime(number));

                return number;
            }
        }

        // Простая проверка на простоту числа
        private bool IsPrime(BigInteger number)
        {
            if (number < 2) return false;
            for (BigInteger i = 2; i <= BigInteger.Pow(number, 1 / 2); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        // Метод вычисления обратного по модулю
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m, t, q;
            BigInteger x0 = 0, x1 = 1;

            if (m == 1) return 0;

            while (a > 1)
            {
                q = a / m;
                t = m;
                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0) x1 += m0;

            return x1;
        }

        // Хэширование текста сообщения (упрощенный алгоритм для демонстрации)
        private BigInteger GetMessageHash(string message)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                return new BigInteger(hashBytes);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}