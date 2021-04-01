using System;
using System.Collections.Generic;
using System.Text;

namespace Triplet
{

    // Класс выполняющий преобразование строки в хэш-значение и обратно
    static class HashHelper
    {
        // Выбирается большим, нежели мощность алфавита
        static int p;
        // Длина строки
        static int n;

        // Диапазон используемых значений
        // Можно было из него вычислять размер таблицы (p)
        const char leftExcBorder = (char)('А' - 1);
        const char rightExcBorder = (char)('я' + 1);
        // Для ускорения формирования хэша и обратного преобразования
        static int[] ps;

        // Вдруг строка будет велика, то вынесение инициализации пригодится
        public static void Init(int pp, int nn)
        {
            p = pp;
            n = nn;

            ps = new int[n];

            ps[0] = 1;
            for (int i = 1; i < n; ++i)
                ps[i] = ps[i - 1] * p;
        }

        // Простейшая схема Горнера
        public static int GetHash(in char[] s)
        {
            int h = 0;
            for (int i = 0; i < n; ++i)
                h += ps[i] * (s[i] - leftExcBorder);

            return h;
        }

        // Получение исходной строки
        public static char[] GetString(int h)
        {
            char[] chars = new char[n];

            for (int i = n - 1; i >= 0; --i)
            {
                chars[i] = (char)(h / ps[i]);
                h -= ps[i] * chars[i];
                chars[i] += leftExcBorder;
            }

            return chars;
        }

        public static bool VerifyChar(in char c)
        {
            if (c > leftExcBorder && c < rightExcBorder)
                return true;

            return false;
        }
    }
}
