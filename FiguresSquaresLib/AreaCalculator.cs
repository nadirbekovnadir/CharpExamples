using System;

namespace FiguresAreasLib
{
    // Воспльзуюсь интерфейсом, который позволит легко добавлять новые фигуры
    // Выбрал интерфейс, так как данный подход поволит избежать проблемы, когда класс фигуры уже имеет родителя (параллелограмм -> квадрат)
    public interface IAreaCalculatable
    {
        public double Area();
    }

    // Так как, думаю, лучше показать побольше собственных "умений" реализую интерфейс явно
    // Он будет использоваться специальным статическим классом, который будет производить вычисления
    // Максимально не прямой путь, но в задании не уточнялось, как передает клиент параметры, может ли создавать объекты класса фигура
    // Можно даже объявить классы фигур скрытыми, оставив публичным лишь интерфейс (просто мысль...)
    public class Circle : IAreaCalculatable
    {
        // Лучше буду явно спецификаторы использовать
        private double radius;

        public Circle(double radius)
        {
            this.radius = radius;
        }

        double IAreaCalculatable.Area()
        {
            return Math.PI * Math.Pow(radius, 2);   
        }
    }

    public class Triangle : IAreaCalculatable
    {
        // Сэкономлю место по вертикали
        // Можно было выполнить в качестве свойств, чтобы параметры легко менять и проверять
        double a, b, c;

        // Проверка валидности данных опущена
        public Triangle(double a, double b, double c)
        {
            // Возможно создание невозможного треугольника
            // Проверю в лоб, мало смысла усложнять
            if (a + b < c || c + a < b || b + c < a)
                throw new Exception("Incorrect Triangular");

            this.a = a;
            this.b = b;
            this.c = c;
        }

        // При выборе метода с вычислением углов (теорема косинусов)
        // можно было бы ввести угловую точность
        private double precision = double.Epsilon * 2;

        public double Precision
        {
            get => precision;
            set
            {
                if (value < double.Epsilon)
                    precision = double.Epsilon;
                else
                    precision = value;
            } 
        }


        public bool IsRectangular(out double cathetus1, out double cathetus2)
        {
            // Тут можно применить несколько подходов
            // Воспользуюсь самым (на мой взгляд простым и очевидным

            double p_a = Math.Pow(a, 2);
            double p_b = Math.Pow(b, 2);
            double p_c = Math.Pow(c, 2);

            cathetus1 = default;
            cathetus2 = default;

            // Если бы пришлось вычислять более длинную последовательность, то потрбеовался бы массив с циклом
            if (Math.Abs(p_a + p_b - p_c) <= Precision)
            {
                cathetus1 = a;
                cathetus2 = b;
                return true;
            }

            if (Math.Abs(p_c + p_a - p_b) <= Precision)
            {
                cathetus1 = c;
                cathetus2 = a;
                return true;
            }

            if (Math.Abs(p_b + p_c - p_a) <= Precision)
            {
                cathetus1 = b;
                cathetus2 = c;
                return true;
            }

            return false;
        }

        double IAreaCalculatable.Area()
        {
            //Конечно данная оптимизация (условная...) ни к месту, но пусть будет
            double c1, c2;
            if (IsRectangular(out c1, out c2))
            {
                return c1 * c2 / 2;
            }

            // В первый раз после школы использую Герона...
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
    }

    public static class AreaCalculator
    {
        public static double GetArea(IAreaCalculatable obj)
        {
            return obj.Area();
        }
    }
}
