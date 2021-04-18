using Microsoft.VisualStudio.TestTools.UnitTesting;
using FiguresAreasLib;
using System.Collections.Generic;

namespace FiguresAreasTest
{
    [TestClass]
    public class UnitTest
    {
        // Примитивный юнит тест (для меня первый с использованием чего-то помимо самописных классов (отпечаток embedded))
        // Можно было бы хранить набор данных в документе, подгружая их для теста
        [TestMethod]
        public void ValidAreaCalculation()
        {
            Triangle tr1 = new Triangle(3, 4, 5);
            Triangle tr2 = new Triangle(2, 3, 3);

            Circle c1 = new Circle(22);
            Circle c2 = new Circle(11);

            IAreaCalculatable[] figures = { tr1, tr2, c1, c2 };
            List<double> result = new List<double>();
            List<double> expected = new List<double> { 6, 2.828, 1520.53, 380.132 };

            foreach (var figure in figures)
            {
                result.Add(AreaCalculator.GetArea(figure));
            }

            double prec = 0.01;

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], result[i], prec, "Wrong Answer!");
            }

        }
    }
}
