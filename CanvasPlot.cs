using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using System.Reflection;

namespace laboratornie_raboti
{
    public class return_canvas_plot
    {
        public List<Point> points_plot = new List<Point>(); ////////////////////// точки для построения графика
        public List<DataPoint> point_in_sm = new List<DataPoint>();/////////////// точки в размерности сантиметров 2-й шаг
        public List<DataPoint> point_in_mm = new List<DataPoint>();/////////////// точки в размерности миллиметров в размерности экрана
        public List<DataPoint> point_in_px1 = new List<DataPoint>();/////////////// точки в размерности пикселей
        public List<DataPoint> point_input = new List<DataPoint>();/////////////// точки в input

        public double disp_X { get; set; }
        public double disp_Y { get; set; }
    }

    public class CanvasPlot
    {

        public readonly double ScreanWidth = 1920;
        public readonly double ScreanHeight = 1080;
        public readonly double ScreanSize = 15.6; ///// в дюймах


        public void CreatePlotCanvas(List<Point> Points, ref Canvas Fell)
        {
            List<Line> lines = new List<Line>();

            for (int i = 0; i < Points.Count - 1; i++)
            {
                Line l = new Line();
                l.X1 = Points[i].X;
                l.Y1 = Points[i].Y;
                l.X2 = Points[i + 1].X;
                l.Y2 = Points[i + 1].Y;
                l.Visibility = Visibility.Visible;
                l.Stroke = Brushes.Black;
                l.StrokeThickness = 4;
                lines.Add(l);

                Fell.Children.Add(l);
            }
        }

        public return_canvas_plot Calculate_Param_Plot(List<DataPoint> InputPoints, double In_X_Size /*a*/, double In_Y_Size/*b*/, double A, double B)
        {

            double Sx_r_sm = 0;///// отношение ширины окна дискретного поля в сантиметры относительно пропорций по оси X
            double Sy_r_sm = 0;///// отношение ширины окна дискретного поля в сантиметры относительно пропорций по оси Y

            double Dx = 0; ///// отношение ширины окна дискретного поля к пропорции графика по оси X
            double Dy = 0; ///// отношение ширины окна дискретного поля к пропорции графика по оси Y

            double DPI = 96.0;/// тность виртуальных пикселей в WPF
            double koef_px_WPF_mm = 1 / (DPI / 25.4); /////коеффициент для преобразования  пиксели в пикселях для перевода из едениц размеров wpf (размер в пикселях * на коеф и получим значение в мм)

            /////////////////////////////////////////////////////////////////////   1  преобразование размеров под поле графика   //////////////////

            double[] MaxXY = GetMax_X_Y(InputPoints);
            double[] MinXY = GetMin_X_Y(InputPoints);

            double Xr_Max = MaxXY[0];
            double Xr_Min = MinXY[0];
            double Yr_Max = MaxXY[1];
            double Yr_Min = MinXY[1];

            Dx = Xr_Max - Xr_Min;
            Dy = Yr_Max - Yr_Min;

            Sx_r_sm = A  / Dx;
            Sy_r_sm = B  / Dy;

            List<DataPoint> X_Y_sm_Points = new List<DataPoint>(); ///// переводим значение точек в сантиметры под размеры окна

            foreach (DataPoint el in InputPoints)
            {
                double X = el.X * Sx_r_sm;
                double Y = el.Y * Sy_r_sm;
                X_Y_sm_Points.Add(new DataPoint(X, Y));
            }
            /////////////////////////////////////////////////////////////////////   2 адаптация к физическим размерам экрана(поля canvas)   //////////////////
 
            double Xe_mm = In_X_Size * koef_px_WPF_mm; // ширина поля в мм !!!!!!!!!!!!!!!
            double Ye_mm = In_Y_Size * koef_px_WPF_mm;// высота поля в мм!!!!!!!!!!!!!!!!!!!

            /////////////////////////////////////////// сохраняем масштаб
            ///

            //if (In_X_Size / In_Y_Size < A / B)
            //{
            //    In_X_Size = In_Y_Size * (A / B);

            //}
            //if (In_Y_Size / In_X_Size < B / A)
            //{
            //    In_Y_Size = In_X_Size * (B / A);

            //}


            /////////////////////////////////////////////////////////


            double Sx_sm_mm = Xe_mm / A;
            double Sy_sm_mm = Ye_mm / B;

            double S_SM_MM = Sx_sm_mm; /// выбираем общий масштаб для преобразования, выбирается наименьший мз масштабов
            if (S_SM_MM > Sy_sm_mm)
            {
                S_SM_MM = Sy_sm_mm;
            }

            //// пересчитываем рабочую область экрана на которую будем выводить графики
            double Xe_w_mm = A * S_SM_MM;//////
            double Ye_w_mm = B * S_SM_MM;//////

            List<DataPoint> X_Y_mm_scr_points = new List<DataPoint>();// пересчитываем координаты точек под новое поле

            foreach (DataPoint el in X_Y_sm_Points)
            {
                double X = el.X * S_SM_MM;
                double Y = el.Y * S_SM_MM;
                X_Y_mm_scr_points.Add(new DataPoint(X, Y));
            }
            //////////////////////////////////////////////////////////////
            
            //// пересчет рабочей лбласти в пиксели (оно тут не надо)

            double Xmax = In_X_Size;
            double Ymax = In_Y_Size;

            double Xm_w = Xmax * Xe_w_mm / Xe_mm;
            double Ym_w = Ymax * Ye_w_mm / Ye_mm;

            ///// масштаб перехода из мм в пиксели (экрана)

            double Spx = Xm_w / Xe_mm;
            double Spy = Ym_w / Ye_mm;

            ///////////////////// пересчет точек в пиксели без учета сдвига 
            List<DataPoint> X_Y_p1_points = new List<DataPoint>();

            foreach (DataPoint el in X_Y_mm_scr_points)
            {
                double X = Math.Floor(el.X * Spx);
                double Y = Math.Floor(el.Y * Spy);
                X_Y_p1_points.Add(new DataPoint(X, Y));
            }
            
            /////////////////////////////////////////////////////// расчёт смещения точек в границы экрана

            int DispX = Convert.ToInt32( GetMin_X_Y(X_Y_p1_points)[0]);
            int DispY = Convert.ToInt32( GetMin_X_Y(X_Y_p1_points)[1]);

            if (DispX >= 0)
            { DispX = 0; }

            if (DispY >= 0)
            { DispY = 0; }

            //////////////////// пересчет координат точек и одновременно смещение в границы экрана

            List<Point> X_Y_Pixel_points = new List<Point>();
            
            int P = Convert.ToInt32(Ymax); ////ширина рабочего поля
            bool flagP = false;//////проверка на чётность
            if (P % 2 == 0)
            {
                P = P / 2;
                flagP = true;
            }
            else
            {
                P = P / 2 + 1;
            }

            foreach (DataPoint el in X_Y_p1_points)
            {
                int X = (int)el.X;
                int Y = (int)el.Y;

                int Ty = 0;

                X = X + Math.Abs( DispX); ////смещаем точки в положительные координаты
                Y = Y + Math.Abs( DispY);
                
                if (flagP)
                {
                    Ty = (P + 1 - Y) * 2 - 1;///// смещение для точек четного поля
                }
                else
                {
                    Ty = (P - Y) * 2;///// смещение для точек нечетного поля
                }

                ///смещаем 

                Y = Y + Ty;
                X_Y_Pixel_points.Add(new Point(X, Y));
            }
            return_canvas_plot RET = new return_canvas_plot();
            RET.points_plot = X_Y_Pixel_points;
            RET.point_in_sm = X_Y_sm_Points;
            RET.point_in_mm = X_Y_mm_scr_points;
            RET.point_in_px1 = X_Y_p1_points;
            RET.point_input = InputPoints;
            RET.disp_X = DispX;
            RET.disp_Y = DispY;
            return RET;
        }

        private double[] GetMax_X_Y(List<DataPoint> points)/// метод возвращает массив состоящий из двух элементов максимального значения X  и Y из коллекции точек
        {
            double[] max_X_Y = new double[2];
            double max_X = points[0].X;
            double max_Y = points[0].Y;

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X > max_X)
                {
                    max_X = points[i].X;
                }
                if (points[i].Y > max_Y)
                {
                    max_Y = points[i].Y;
                }
            }
            max_X_Y[0] = max_X;
            max_X_Y[1] = max_Y;
            return max_X_Y;
        }

        private double[] GetMin_X_Y(List<DataPoint> points)/// метод возвращает массив состоящий из двух элементов минимального значения X и Y из коллекции точек
        {
            double[] min_X_Y = new double[2];
            double min_X = points[0].X;
            double min_Y = points[0].Y;

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < min_X)
                {
                    min_X = points[i].X;
                }
                if (points[i].Y < min_Y)
                {
                    min_Y = points[i].Y;
                }
            }
            min_X_Y[0] = min_X;
            min_X_Y[1] = min_Y;
            return min_X_Y;
        }
    }
}
