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

        // Изменено на кириллицу (только буквы)
        const char offs = (char)('А' - 1);
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
                h += ps[i] * (s[i] - offs);

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
                chars[i] += offs;
            }

            return chars;
        }
    }
}
