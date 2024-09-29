using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Zlabs8.ViewModels
{
    public class Lab6ViewModel : INotifyPropertyChanged
    {
        private string _selectedFilePath;
        private string _compressedData;
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

        public string CompressedData
        {
            get => _compressedData;
            set
            {
                _compressedData = value;
                OnPropertyChanged(nameof(CompressedData));
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
        public ICommand CompressCommand { get; }

        public Lab6ViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            CompressCommand = new RelayCommand(CompressFile);
        }

        private void SelectFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*" // Выбор всех файлов
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName; // Устанавливаем путь к выбранному файлу
            }
        }

        private void CompressFile()
        {
            if (string.IsNullOrEmpty(SelectedFilePath))
            {
                StatusMessage = "Пожалуйста, выберите файл для сжатия.";
                return;
            }

            // Сжимаем файл с использованием Хаффмана и RLE
            var fileData = File.ReadAllBytes(SelectedFilePath);
            var huffmanEncoded = HuffmanCompress(fileData);
            var rleEncoded = RLECompress(huffmanEncoded);

            // Обновляем статус и сжатые данные
            CompressedData = Convert.ToBase64String(rleEncoded); // Сохранение в Base64 для отображения
            StatusMessage = "Файл успешно сжат!";
        }

        private byte[] HuffmanCompress(byte[] data)
        {
            var huffmanTree = new HuffmanTree();
            var root = huffmanTree.BuildTree(data);
            var huffmanCodes = huffmanTree.BuildCodes(root);

            // Кодируем данные
            var encodedData = new StringBuilder();
            foreach (var byteValue in data)
            {
                // Проверяем, что код для данного байта существует
                if (huffmanCodes.TryGetValue(byteValue, out var code))
                {
                    encodedData.Append(code);
                }
                else
                {
                    StatusMessage = $"Код не найден для байта: {byteValue}.";
                    return Array.Empty<byte>(); // Или выберите другое поведение
                }
            }

            // Преобразуем закодированные данные в массив байтов
            return ConvertBitStringToByteArray(encodedData.ToString());
        }

        private byte[] RLECompress(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return Array.Empty<byte>(); // Возвращаем пустой массив, если входные данные пустые
            }

            using (MemoryStream ms = new MemoryStream())
            {
                int count = 1;
                for (int i = 1; i < data.Length; i++)
                {
                    if (data[i] == data[i - 1])
                    {
                        count++;
                    }
                    else
                    {
                        ms.WriteByte(data[i - 1]);
                        ms.WriteByte((byte)count);
                        count = 1;
                    }
                }
                // Записываем последние данные
                ms.WriteByte(data[^1]);
                ms.WriteByte((byte)count);
                return ms.ToArray();
            }
        }


        // Конвертация битовой строки в массив байтов
        private byte[] ConvertBitStringToByteArray(string bitString)
        {
            int numBytes = (bitString.Length + 7) / 8;
            byte[] byteArray = new byte[numBytes];

            for (int i = 0; i < bitString.Length; i++)
            {
                if (bitString[i] == '1')
                {
                    byteArray[i / 8] |= (byte)(1 << (7 - (i % 8)));
                }
            }

            return byteArray;
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Класс для узлов дерева Хаффмана
    public class HuffmanNode
    {
        public byte Value { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public HuffmanNode(byte value, int frequency)
        {
            Value = value;
            Frequency = frequency;
        }
    }

    // Класс для дерева Хаффмана
    public class HuffmanTree
    {
        public HuffmanNode BuildTree(byte[] data)
        {
            var frequencies = data.GroupBy(b => b)
                                  .ToDictionary(g => g.Key, g => g.Count());

            var priorityQueue = new SortedSet<HuffmanNode>(Comparer<HuffmanNode>.Create((x, y) =>
            {
                var result = x.Frequency.CompareTo(y.Frequency);
                if (result == 0)
                    return x.Value.CompareTo(y.Value);
                return result;
            }));

            foreach (var symbol in frequencies)
            {
                priorityQueue.Add(new HuffmanNode(symbol.Key, symbol.Value));
            }

            while (priorityQueue.Count > 1)
            {
                var left = priorityQueue.Min;
                priorityQueue.Remove(left);
                var right = priorityQueue.Min;
                priorityQueue.Remove(right);

                var merged = new HuffmanNode(0, left.Frequency + right.Frequency)
                {
                    Left = left,
                    Right = right
                };

                priorityQueue.Add(merged);
            }

            return priorityQueue.Min;
        }

        public Dictionary<byte, string> BuildCodes(HuffmanNode root)
        {
            var codes = new Dictionary<byte, string>();
            BuildCodesRecursive(root, "", codes);
            return codes;
        }

        private void BuildCodesRecursive(HuffmanNode node, string code, Dictionary<byte, string> codes)
        {
            if (node.Left == null && node.Right == null)
            {
                codes[node.Value] = code;
                return;
            }

            BuildCodesRecursive(node.Left, code + "0", codes);
            BuildCodesRecursive(node.Right, code + "1", codes);
        }
    }
}
