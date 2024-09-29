using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System;

namespace Zlabs8.ViewModels
{
    public class Lab2ViewModel : INotifyPropertyChanged
    {
        private string _inputText;
        private string _outputText;

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public ICommand EncryptCommand { get; }

        public Lab2ViewModel()
        {
            EncryptCommand = new RelayCommand(Encrypt);
        }

        private void Encrypt()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                OutputText = "Введите строку для шифрования.";
                return;
            }

            // Генерация случайных простых чисел p и q разрядностью не менее 20 бит
            BigInteger p = GenerateRandomPrime(20);
            BigInteger q = GenerateRandomPrime(20);

            // RSA ключи
            RSAParameters rsaParams = GenerateRSAKeys(p, q);

            // Шифрование строки
            OutputText = EncryptText(InputText, rsaParams);
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

        // Простая проверка на простоту числа (можно заменить на более сложный алгоритм)
        private bool IsPrime(BigInteger number)
        {
            if (number < 2) return false;
            for (BigInteger i = 2; i <= BigInteger.Pow(number, 1 / 2); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        // Генерация RSA-ключей (модуль n, публичный и приватный экспоненты)
        private RSAParameters GenerateRSAKeys(BigInteger p, BigInteger q)
        {
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);
            BigInteger e = 65537; // Обычно выбирается стандартная публичная экспонента
            BigInteger d = ModInverse(e, phi);

            return new RSAParameters
            {
                Modulus = n.ToByteArray(),
                Exponent = e.ToByteArray(),
                D = d.ToByteArray()
            };
        }

        // Метод для вычисления обратного по модулю
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

        // Шифрование текста с использованием публичного ключа
        private string EncryptText(string text, RSAParameters rsaParams)
        {
            BigInteger n = new BigInteger(rsaParams.Modulus);
            BigInteger e = new BigInteger(rsaParams.Exponent);

            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            BigInteger textInt = new BigInteger(textBytes);

            // Шифрование: c = m^e mod n
            BigInteger cipherInt = BigInteger.ModPow(textInt, e, n);
            return Convert.ToBase64String(cipherInt.ToByteArray());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}