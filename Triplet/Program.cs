using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Triplet
{
    // Данная программа рассчитана на то, что текстовый файл представляет собой символы юникода в диапазоне от ('A' - 65) до ('z' - 122)
    // Возможно определение набора из другого количества символов (от одного до количества, которое не сделает хэш-таблицу бесконечной...)
    // Также можно выбрать количество блоков, на которые будет разбит файл (потоки)

    public class Program
    {
        // Значение множителя для хэша
        // Выбирается большим нежели диапазон символов (1103 - 1040 = 63)
        // Можно задавать диапазон в качестве разницы между символами 'А' - 'я' (в самом классе хэша с возможностью выбора кодировки)
        const int p = 67;

        // Количество определяемых символов (триплет)
        const int n = 3;

        // Максимальное число потоков, оптимальное для данного процессора (лог ядра)
        // Для 10мб будет избыточно использовать большое количество процессов, так как накладные расходы могут нивелировать прирост
        static int blocks = Environment.ProcessorCount;

        // Максимальная длина символов, которая будет обработана в одном потоке
        // Рассчитывается в коде
        static int maxLength;

        // Количество выводимых триплетов
        const int tripletsToPrint = 10;

        // Размер хэш-таблицы
        static int tableSize = SimplePow(p, n);

        // Хэш таблица и таблица ее индексов (для сортировки в конце)
        static int[] hashTable = new int[tableSize];
        static int[] valuesTable = Enumerable.Range(0, tableSize).ToArray();

        // Хранение пути до файла и строки с самим файлом
        static string filePath;
        static string text;

        public static async Task Main(string[] args)
        {
            // Запуск измеряющего таймера
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Сохранение пути до файла из параметров
            // filePath = args[0];
            filePath = "TestFileUtf8.txt";

            // Инициализация класса для подсчета хэшей
            HashHelper.Init(p, n);

            // Считывание файла (10 мб - мало, можно считать целиком)
            StreamReader sr = new StreamReader(filePath);
            text = sr.ReadToEnd();

            // Рассчет длины одного блока
            // Округление вверх используется, чтобы не потерять элементы
            maxLength = (int)Math.Ceiling((double)text.Length / blocks);

            // Создаем список дл хранения исполняемых задач
            List<Task> tasks = new List<Task>();
            // Создаем делегат, который и будет выполняемой параллельно задачей
            // Делегат сам по себе можно выполнять асинхронно, но, думаю, не очень удобное решение
            Action<object> a = new Action<object>((param) => FileProcessing((int)param));

            // Формируем все задачи
            // В качестве альтернативы можно использовать Parallel.For()
            // Что упростило бы передачу параметра счетчика и являлось бы простым и элегантным решением

            // Parallel.For(0, blocks, (i) =>
            // {
            //     FileProcessing(i);
            // });

            // Можно использовать создание и запуск в одном действии, как приведено ниже
            // tasks.Add(Task.Factory.StartNew(a, (object)i));
            for (int i = 0; i < blocks; ++i)
                tasks.Add(new Task(a, (object)i));

            // Запускаем все задачи
            tasks.ForEach((task) => task.Start());

            // Ожидаем выполнения всех задач
            await Task.WhenAll(tasks);

            // Сортировка массива индексов по ключам, лежащим в таблице хэшей
            // Вероятно, не самый быстрый подход с использованием Array (зависит от используемого метода сортировки)
            // Возможны альтернативы со словарем
            Array.Sort<int, int>(hashTable, valuesTable, new MyComparer());

            // Вывод наиболее встречаемых триплетов
            for (int i = 0; i < tripletsToPrint; ++i)
            {

                if (hashTable[i] == 0)
                    continue;

                if (i != 0)
                    Console.Write(", ");

                // Данная строка оставлена, чтобы можно посмотреть количество каждого из триплетов
                // Console.Write("{0} - {1}", new String(HashHelper.GetString(valuesTable[i])), hashTable[i]);
                Console.Write(new String(HashHelper.GetString(valuesTable[i])));
            }
            Console.WriteLine();

            // Остановка таймера и вывод значения в мс
            sw.Stop();
            var timeSpan = sw.ElapsedMilliseconds;
            Console.WriteLine(timeSpan);
        }

        // Вероятно от скуки написал возведение в степень...
        static int SimplePow(int x, int y)
        {
            int res = 1;
            while (y != 0)
            {
                res *= x;
                --y;
            }

            return res;
        }

        // В данном методе производится обработка определенного блока текста
        // Чтобы отсеять символы, которые не попадают в заданный диапазон ('\r', например), можно добавить проверку на попадание в диапазон (от 'А' до 'я')
        // Однако, данной задачи не стоит (предполагается использование букв), не вижу смысла замедлять алгоритм
        static void FileProcessing(int block)
        {
            // Рассчет границ обрабатываемого текста
            int leftBorder = block * maxLength;
            int rightBorder = Math.Min((block + 1) * maxLength, text.Length - n + 1);

            // Запись всех триплетов в таблицу хэшей
            char[] temp = new char[n];
            bool isCorrect = true;
            // Небольшое ускорение
            int i, j;
            for (i = leftBorder; i < rightBorder; ++i)
            {
                isCorrect = true;
                // Налезание на соседний блок позволит учесть триплеты, находящиеся на стыке блоков
                for (j = 0; j < n; ++j)
                {
                    // Проверяем валидность символа
                    if (!HashHelper.VerifyChar(text[i + j]))
                    {
                        // Чтобы не рассматривать почем зря интервал до
                        // некорректного символа осуществим смещение
                        i += j;
                        isCorrect = false;
                        break;
                    }

                    temp[j] = text[i + j];
                }

                // Чтобы не использовать семафоры, мютексы и прочие, требующие умственного напряжение, методы,
                // воспользовался простейшей атомарной операцией
                // Можно было в качестве альтернативы пользоваться отдельной хэш-таблицей для каждого потока, 
                // а затем осуществлять их слияние в единую
                if (isCorrect)
                    Interlocked.Increment(ref hashTable[HashHelper.GetHash(temp)]);
            }
        }
    }

    // Для сортировки по убыванию
    public class MyComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

}
