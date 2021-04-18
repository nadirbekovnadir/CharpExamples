using System;
using FiguresAreasLib;

namespace FiguresAreas
{
    class Program
    {
        static void Main(string[] args)
        {
            Triangle tr1 = new Triangle(3, 4, 5);
            Triangle tr2 = new Triangle(2, 3, 3);

            Circle c1 = new Circle(22);
            Circle c2 = new Circle(11);

            IAreaCalculatable i1 = tr1;
            IAreaCalculatable i2 = tr2;

            IAreaCalculatable i3 = c1;
            IAreaCalculatable i4 = c2;

            double a1 = AreaCalculator.GetArea(i1);
            double a2 = AreaCalculator.GetArea(i2);

            double a3 = AreaCalculator.GetArea(i3);
            double a4 = AreaCalculator.GetArea(i4);
        }
    }
}
