using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Model
{
    class CalculatorModel
    {
        public double Add(double a, double b)
        {
            return a + b;
        }

        public double Substract(double a, double b)
        {
            return a - b;
        }

        public double Multiply(double a, double b)
        {
            return a * b;
        }

        public double Divide(double a, double b)
        {
            if (b!=0) return a / b;
            else return double.NaN;
        }

        public double Modulus(double a, double b)
        {
            return a % b;
        }

        public double SquareRoot(double a)
        {
            return Math.Sqrt(a);
        }

        public double Square(double a)
        {
            return Math.Pow(a, 2);
        }

        public double Negative(double a)
        {
            return a * (-1);
        }

        public double Inverse(double a)
        {
            if (a != 0) return 1 / a;
            else return double.NaN;
        }
    }
}
