using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace laboratornie_raboti
{
    public class lab1TE
    {
        private readonly double SIGxi = 2.3;
        public double SIGm = 0.00032;
        private readonly double SIGy_dop = 9.0;
        private readonly double Xi = 0.9;
        public readonly double Zmax = 1.8;
        public readonly double Zmin = -1.8;
        public readonly int numPoint = 100;
        private double Y { get; set; }// аргумент
        private double S { get; set; }// масштаб
        public readonly string FORMULA = "Y = e^(-1x) * sin(1.2*X1 + 0.8*X2)";
        private string LogFile { get; set; }// файл для записи логов

        public List<double> CalculateEXP(double Zi) // расчет решения в точке Zi
        {
            double X1 = 0 - (Zi * 2 / 3);
            int k = 0;
            double El = 1;
            double REZ = 1;
            
            while (this.SIGm < Math.Abs(El))
            {
                k++;
                El = El * ( (X1) / k);
                REZ = REZ + El;
            }
            List<double> ret = new List<double>() { REZ, k };
            return ret;
        }

        public List<double> CalculateSin(double Zi)
        {
            double XZ = Zi;
            int k = 0;
            double El = XZ; //Math.Pow(-1, k) * Math.Pow(XZ, 2 * k + 1) / this.Factorial(2 * k + 1);
            double rez = El;

            while (this.SIGm < Math.Abs(El))
            {
                k++;
                El = -(El) * (XZ * XZ / (2*k * (2*k+1))); 
                rez = rez + El;
            }
            List<double> ret = new List<double>() { rez, k };
            return ret;
        }

        public double[,] list_step_sin()
        {
            double[,] list_step = new double[numPoint, 2];
            double Z = this.Zmin;
            double step = (this.Zmax - this.Zmin) / numPoint;
            int i = 0;

            while (Z <= Zmax)
            {
                list_step[i, 0] = CalculateSin(Z)[0];
                list_step[i, 1] = CalculateSin(Z)[1];
                i++;
                Z = Z + step;
            }
            return list_step;
        }

        public double[,] list_step_exp()
        {
            double[,] list_step = new double[numPoint, 2];
            double Z = this.Zmin;
            double step = (this.Zmax - this.Zmin) / numPoint;
            int i = 0;

            while (Z <= Zmax)
            {
                list_step[i, 0] = CalculateEXP(Z)[0];
                list_step[i, 1] = CalculateEXP(Z)[1];
                i++;
                Z = Z + step;
            }
            return list_step;
        }

        public List<DataPoint> plotting(/*double Zmin, double Zmax, double numPoint*/)
        {
            List<DataPoint> points = new List<DataPoint>();
            double Z = Zmin;
            double step = (this.Zmax - this.Zmin) / numPoint;
            double Y = 0.0;

            while (Z <= Zmax)
            {
                double exp = CalculateEXP(Z)[0];
                double sin = CalculateSin(Z)[0];
                Y = exp * sin;
                points.Add(new DataPoint(Z, Y));
                Z = Z + step;
            }
            return points;
        }

        public int Factorial(int X)
        {
            int rez = 1;
            int flag = 2;
            if (X >= 1)
            {
                if (X >= 2)
                {
                    while (flag < X + 1)
                    {
                        rez = rez * flag;
                        flag++;
                    }
                }
            }
            //else
            //{
            //    rez = -1;
            //}
            return rez;
        }
    }
}
