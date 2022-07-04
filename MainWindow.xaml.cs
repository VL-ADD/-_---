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
using System.Threading;
using System.IO;

namespace laboratornie_raboti
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    
    public class Mymodel_1
    {
        public PlotModel Model_1 { get; private set; }
        public Mymodel_1()
        {
            PlotModel model_lab1_te = new PlotModel();
            model_lab1_te.PlotType = PlotType.XY;

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            
            lab1TE lab1te = new lab1TE();
            List<DataPoint> Points = lab1te.plotting();
            string title = lab1te.FORMULA;

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //model_lab1_te.Title = title;
            var series = new LineSeries { Title = "", MarkerType = MarkerType.Square };
            foreach (DataPoint point in Points)
            {
                series.Points.Add(point);
            }
            model_lab1_te.Series.Add(series);

            this.Model_1 = model_lab1_te;
        }
    }


    public partial class MainWindow : Window
    {
        bool flag_change_choce = false;

        public MainWindow()
        {
            InitializeComponent();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
             
        }

        public void changeWinSize(object sender, EventArgs e )
        {
            double DPI = 96.0;/// тность виртуальных пикселей в WPF
            double koef_px_WPF_mm = 1 / (DPI / 25.4); /////коеффициент для преобразования  пиксели в пикселях для перевода из едениц размеров wpf (размер в пикселях * на коеф и получим значение в мм)
            
            PlotCanvasFell.Children.Clear();
            lab1TE lab1 = new lab1TE();
            CanvasPlot cp = new CanvasPlot();

            double a = PlotCanvasFell.ActualWidth;
            double b = PlotCanvasFell.ActualHeight;

            double A = 12;
            double B = 8;

            //double A = a * koef_px_WPF_mm;
            //double B = b * koef_px_WPF_mm;


            List<Point> points = new List<Point>();

            List<DataPoint> ForPlotPoints = new List<DataPoint>();/////////////// финальный результат точек
            ForPlotPoints = lab1.plotting();

            return_canvas_plot Plot_dat = new return_canvas_plot();

            /////////////////////////////////////////////////////////
            if (a / b < A / B)
            {
                // a = b * (A / B);  
                b = a * (A / B);
            }
            if (b / a < B / A)
            {
               // a = b * (B / A);
                b = a * (B / A);
            }

            if (a / b > A / B)
            {
                a = b * (A / B);
            }
            if (b / a > B / A)
            {
                b = a * (B / A);
            }
            ////////////////////////////////////////////////////////////
            Plot_dat = cp.Calculate_Param_Plot(ForPlotPoints, a, b, A, B);
            points = Plot_dat.points_plot;

            List<DataPoint> point_in_sm = new List<DataPoint>();/////////////// точки в размерности сантиметров 2-й шаг
            List<DataPoint> point_in_mm = new List<DataPoint>();/////////////// точки в размерности миллиметров в размерности экрана
            List<DataPoint> point_in_px1 = new List<DataPoint>();/////////////// точки в размерности пикселей
            point_in_sm = Plot_dat.point_in_sm;
            point_in_mm = Plot_dat.point_in_mm;
            point_in_px1 = Plot_dat.point_in_px1;

            ////write_in_file_tables(List<DataPoint> InputPoint, List<DataPoint> point_in_sm, List<DataPoint> point_in_mm, List<DataPoint> point_in_px1, List<DataPoint> ForPlotPoints)

            Thread FileWriteThread = new Thread(new ParameterizedThreadStart(write_in_file_tables));
            FileWriteThread.Start(Plot_dat);


            for (int i = 0; i < points.Count - 1; i++)
            {
                Line MyLine = new Line();

                MyLine.X1 = points[i].X;// BeginPoint.X;
                MyLine.Y1 = points[i].Y;// BeginPoint.Y;
                MyLine.X2 = points[i + 1].X;// EndPoint.X;
                MyLine.Y2 = points[i + 1].Y;// EndPoint.Y;
                MyLine.Stroke = System.Windows.Media.Brushes.Chocolate;
                MyLine.StrokeThickness = 3;
                PlotCanvasFell.Children.Add(MyLine);
            }

            int NUL_X = (int)(a + Plot_dat.disp_X);
            int NUL_Y = (int)(b + Plot_dat.disp_Y);

            /////////добавляем координатные линии
            Line Х_Line = new Line();
            Х_Line.X1 = 0;// BeginPoint.X;
            Х_Line.Y1 = NUL_Y;// BeginPoint.Y;
            Х_Line.X2 = a;// EndPoint.X;
            Х_Line.Y2 = NUL_Y;// EndPoint.Y;
            Х_Line.Stroke = System.Windows.Media.Brushes.Silver;
            Х_Line.StrokeThickness = 2;
            PlotCanvasFell.Children.Add(Х_Line);

            Line Y_Line = new Line();
            Y_Line.X1 = NUL_X;// BeginPoint.X;
            Y_Line.Y1 = 0;// BeginPoint.Y;
            Y_Line.X2 = NUL_X;// EndPoint.X;
            Y_Line.Y2 = b;// EndPoint.Y;
            Y_Line.Stroke = System.Windows.Media.Brushes.Silver;
            Y_Line.StrokeThickness = 2;
            PlotCanvasFell.Children.Add(Y_Line);

            
            
        }
        static public void write_in_file_tables(object PP)
        {
            return_canvas_plot PPP = (return_canvas_plot)PP;
            List<DataPoint> InputPoint = PPP.point_input;
            List<DataPoint> point_in_sm = PPP.point_in_sm;
            List<DataPoint> point_in_mm = PPP.point_in_mm;
            List<DataPoint> point_in_px1 = PPP.point_in_px1;
            List<Point> ForPlotPoints = PPP.points_plot;

            string path1 = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ начальные_значения.txt";
            string path2 = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ значения_в_сантиметрах.txt";
            string path3 = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ значения_в_миллиметрах.txt";
            string path4 = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ значения_в_пикселях_без_коррекции.txt";
            string path5 = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ значения_в_пиксилях_рез.txt";

            string TEXT = "        начальные  значения        \n" +
                          "|-----------------------------------|\n" +
                          "|     X           |     Y           |\n" +
                          "|-----------------------------------|\n" ; //// 17 chars for X/Y

            foreach(DataPoint el in InputPoint)
            {
                string X = Convert.ToString( Math.Round( el.X, 3 ) );
                string Y = Convert.ToString( Math.Round( el.Y, 3 ) );

                X =   X + new string(' ', 17 - X.Length) ;
                Y =   Y + new string(' ', 17 - Y.Length) ;

                TEXT = TEXT + "|" + X + "|" + Y + "|\n";
            }
            TEXT = TEXT + "|-----------------------------------|\n";
            
            try
            {
                using (StreamWriter sw = new StreamWriter(path1, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
                   TEXT = "        значения в сантиметрах       \n" +
                          "|-----------------------------------|\n" +
                          "|     X           |     Y           |\n" +
                          "|-----------------------------------|\n"; //// 17 chars for X/Y

            foreach (DataPoint el in point_in_sm)
            {
                string X = Convert.ToString(Math.Round(el.X, 3));
                string Y = Convert.ToString(Math.Round(el.Y, 3));

                X = X + new string(' ', 17 - X.Length);
                Y = Y + new string(' ', 17 - Y.Length);

                TEXT = TEXT + "|" + X + "|" + Y + "|\n";
            }
            TEXT = TEXT + "|-----------------------------------|\n";

            try
            {
                using (StreamWriter sw = new StreamWriter(path2, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            TEXT = "        значения в миллиметрах       \n" +
                   "|-----------------------------------|\n" +
                   "|     X           |     Y           |\n" +
                   "|-----------------------------------|\n"; //// 17 chars for X/Y

            foreach (DataPoint el in point_in_mm)
            {
                string X = Convert.ToString(Math.Round(el.X, 3));
                string Y = Convert.ToString(Math.Round(el.Y, 3));

                X = X + new string(' ', 17 - X.Length);
                Y = Y + new string(' ', 17 - Y.Length);

                TEXT = TEXT + "|" + X + "|" + Y + "|\n";
            }
            TEXT = TEXT + "|-----------------------------------|\n";

            try
            {
                using (StreamWriter sw = new StreamWriter(path3, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            TEXT = "  значения_в_пикселях_без_коррекции  \n" +
                   "|-----------------------------------|\n" +
                   "|     X           |     Y           |\n" +
                   "|-----------------------------------|\n"; //// 17 chars for X/Y

            foreach (DataPoint el in point_in_px1)
            {
                string X = Convert.ToString(Math.Round(el.X, 3));
                string Y = Convert.ToString(Math.Round(el.Y, 3));

                X = X + new string(' ', 17 - X.Length);
                Y = Y + new string(' ', 17 - Y.Length);

                TEXT = TEXT + "|" + X + "|" + Y + "|\n";
            }
            TEXT = TEXT + "|-----------------------------------|\n";

            try
            {
                using (StreamWriter sw = new StreamWriter(path4, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            TEXT = "       значения_в_пиксилях_рез       \n" +
                   "|-----------------------------------|\n" +
                   "|     X           |     Y           |\n" +
                   "|-----------------------------------|\n"; //// 17 chars for X/Y

            foreach (Point el in ForPlotPoints)
            {
                string X = Convert.ToString(Math.Round(el.X, 3));
                string Y = Convert.ToString(Math.Round(el.Y, 3));

                X = X + new string(' ', 17 - X.Length);
                Y = Y + new string(' ', 17 - Y.Length);

                TEXT = TEXT + "|" + X + "|" + Y + "|\n";
            }
            TEXT = TEXT + "|-----------------------------------|\n";

            try
            {
                using (StreamWriter sw = new StreamWriter(path5, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }
            return;
        }

        public void exit_click(object sender, EventArgs e)
        {
            
        }

        public void leftmouse1(object sender, EventArgs e) // событие выбора элемента из комбобокса лабораторных работ по labsList_Malch
        {
            int i = -1;
            int i1 = -1;

            try
            {
                i = labsList_Malch.SelectedIndex;
                i1 = labsList_Max.SelectedIndex;
            }
            catch (Exception)
            {   }

            if (i >= 0)
            {
                if (i1 >= 0)
                {
                    labsList_Max.SelectedIndex = -1;
                    labsList_Max.Text = "лаб-раб МСКС";
                }
            }
            flag_change_choce = true;
        }

        

        public void leftmouse2(object sender, EventArgs e)// событие выбора элемента из комбобокса лабораторных работ по labsList_Max
        {
            int i = -1;
            int i1 = -1;

            try
            {
                i1 = labsList_Malch.SelectedIndex;
                i = labsList_Max.SelectedIndex;
            }
            catch (Exception)
            { }

            if (i >= 0)
            {
                if (i1 >= 0)
                {
                    labsList_Malch.SelectedIndex = -1;
                    labsList_Malch.Text = "лаб-раб ТИЭ";
                }
            }
            flag_change_choce = true;
        }

       

        public void lab1_TE_start(object sender, EventArgs e)// событие старт первой лабораторной Мальчевой
        {
            Mymodel_1 mod = new Mymodel_1();
            lab1TE lab = new lab1TE();
            OxyPlot.Wpf.PlotView plotty = new OxyPlot.Wpf.PlotView();
            plotty.Model = mod.Model_1;
            plot_grid.Children.Add(plotty);
            double[,] sinstep = lab.list_step_sin();
            double[,] expstep = lab.list_step_exp();
            lab1_formula_text.Text = lab.FORMULA;
            int All_steps = 0;

            expText.Text = "";
            sinText.Text = "";
            rezultText.Text = "";
            for (int i =0; i< lab.numPoint; i ++)
            {
                expText.Text = expText.Text + "кол-во шагов = " + Convert.ToString(expstep[i,1]) + "  результат = " + Convert.ToString(expstep[i,0]) + "\n";            
                sinText.Text = sinText.Text + "кол-во шагов = " + Convert.ToString(sinstep[i, 1]) + "  результат = " + Convert.ToString(sinstep[i, 0]) + "\n";
                rezultText.Text = rezultText.Text + "результат = " + Convert.ToString(expstep[i, 0] * sinstep[i, 0]) + "  при Z =  " +Convert.ToString( (lab.Zmax - lab.Zmin) / lab.numPoint * (i) + lab.Zmin ) + "\n";
                All_steps = All_steps + Convert.ToInt32(expstep[i, 1]) + Convert.ToInt32(sinstep[i, 1]);
            }
            
            lab1_deltas.Text = "при погрешности = " + Convert.ToString(lab.SIGm) + " кол-во шагов = " + Convert.ToString(All_steps) + "\n";

            lab.SIGm = lab.SIGm - lab.SIGm / 100 * 60;
            sinstep = lab.list_step_sin();
            expstep = lab.list_step_exp();
            All_steps = 0;

            for (int i = 0; i < lab.numPoint; i++)
            {
                All_steps = All_steps + Convert.ToInt32(expstep[i, 1]) + Convert.ToInt32(sinstep[i, 1]);
            }

            lab1_deltas.Text = lab1_deltas.Text +  "при погрешности = " + Convert.ToString(lab.SIGm) + " кол-во шагов = " + Convert.ToString(All_steps) + "\n";
            
            string path = @"D:\магистратура\учёба 2 курс магистратура\laboratornie_raboti\ таблица лаб 1.txt";
            
            string TEXTexp = "                               Таблица результатов  exp                                            \n" +
                          "|-------------------------------------------------------------------------------------------------------------|\n" +
                          "| №  |  formula:   El(0) = 1                 |  |Xi|max  |  Погрешность         |      кол-во операций        |\n" +
                          "|    |     El(i-1) + El(i-1) * (X1 / k)      |           |                      |   +/-        |  */:         |\n" +
                          "---------------------------------------------------------------------------------------------------------------\n" +
                          "|    |                                       |    1.2    |                      |              |              |\n";
            
            string TEXTsin = "                               Таблица результатов  sin                                            \n" +
                          "|-------------------------------------------------------------------------------------------------------------|\n" +
                          "| №  |  formula:   El(0) = X                 |  |Xi|max  |  Погрешность         |      кол-во операций        |\n" +
                          "|    |El(i-1) + (-(El) * (X*X/(2*k*(2*k+1))))|           |                      |   +/-        |  */:         |\n" +
                          "---------------------------------------------------------------------------------------------------------------\n" +
                          "|    |                                       |    1.8    |                      |              |              |\n";
            
            string TEXTall = "                               Таблица результатов  sin                                            \n" +
                         "|--------------------------------------------------------------------------------------------------------------|\n" +
                         "| №  |  formula:   El(0) = X                 |  |Xi|max  |  Погрешность          |      кол-во операций        |\n" +
                         "|    | El(i-1)+(-(El)*(X*X/(2*k*(2*k+1)))) + |    1.8    |                       |   +/-        |  */:         |\n" +
                         "----------------------------------------------------------------------------------------------------------------\n" +
                         "|    |     El(i-1) + El(i-1) * (X1 / k)      |    1.2    |                       |              |              |\n";
            
            for (int i = 0; i < 10; i++)
            {
                double N_step_exp = 0;
                double N_step_sin = 0;
                double N_step_all = 0;
                string sigma = Convert.ToString( Math.Round( lab.SIGm , 10));

                for (int j = 0; j < lab.numPoint; j++)
                {
                    N_step_all = N_step_all + expstep[i, 1] + sinstep[i, 1];
                    N_step_sin = N_step_sin + sinstep[i, 1];
                    N_step_exp = N_step_exp + expstep[i, 1];
                }

                string numSumExp = Convert.ToString(N_step_exp);
                string numMulExp = Convert.ToString(N_step_exp * 2);
                string num = i.ToString();
                string f = "     El(i-1) + El(i-1) * (X1 / k)      ";

                numSumExp = numSumExp + new string(' ', 14 - numSumExp.Length);
                numMulExp = numMulExp + new string(' ', 14 - numMulExp.Length);
                num = num + new string(' ', 4 - num.Length);
                sigma = sigma + new string(' ', 23 - sigma.Length);
                TEXTexp = TEXTexp + "|" + num + "|" + f + "|" + "    1.2    |" + sigma + "|" + numSumExp + "|" + numMulExp + "|\n" ;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                f = "El(i-1) + (-(El) * (X*X/(2*k*(2*k+1))))";
                   
                numSumExp = Convert.ToString(N_step_sin * 3);
                numMulExp = Convert.ToString(N_step_sin * 6);
                numSumExp = numSumExp + new string(' ', 14 - numSumExp.Length);
                numMulExp = numMulExp + new string(' ', 14 - numMulExp.Length);

                TEXTsin = TEXTsin + "|" + num + "|" + f + "|" + "    1.2    |" + sigma + "|" + numSumExp + "|" + numMulExp + "|\n";

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                f = "Y = e^(-1x) * sin(1.2*X1 + 0.8*X2)     ";
                numSumExp = Convert.ToString((N_step_sin * 3) + N_step_exp);
                numMulExp = Convert.ToString((N_step_sin * 6) + (N_step_exp * 2));
                numSumExp = numSumExp + new string(' ', 14 - numSumExp.Length);
                numMulExp = numMulExp + new string(' ', 14 - numMulExp.Length);

                TEXTall = TEXTall + "|" + num + "|" + f + "|" + "    1.2    |" + sigma + "|" + numSumExp + "|" + numMulExp + "|\n";
                
                lab.SIGm = lab.SIGm + lab.SIGm / 100 * 10 * i;
                sinstep = lab.list_step_sin();
                expstep = lab.list_step_exp();
            }
            TEXTexp = TEXTexp + "-------------------------------------------------------------------------------------------------------\n";
            TEXTsin = TEXTsin + "-------------------------------------------------------------------------------------------------------\n";
            TEXTall = TEXTall + "-------------------------------------------------------------------------------------------------------\n";


            string TEXT = TEXTexp + "\n\n\n\n" + TEXTsin + "\n\n\n\n" + TEXTall;

            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(TEXT);
                }
            }
            catch (Exception)
            { }

            

        }

        private void labsList_Malch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
     
        }

        

       
        public void dropDown2(object sender, EventArgs e)// событие закрытия комбобокса лабораторных работ по labsList_Max
        {
            if (flag_change_choce)
            {
                MessageBox.Show(Convert.ToString(labsList_Max.SelectedIndex));
                flag_change_choce = false;
            }
        }

        public void dropDown1(object sender, EventArgs e)// событие закрытия комбобокса лабораторных работ по labsList_Malch
        {
            //if (flag_change_choce)
            //{
            //    MessageBox.Show(Convert.ToString(labsList_Malch.SelectedIndex));
            //    flag_change_choce = false;
            //}
            if(labsList_Malch.SelectedIndex == 0)
            {
                lab1_TE.Visibility = Visibility.Visible;
                lab2_TE.Visibility = Visibility.Hidden;
            }
            if (labsList_Malch.SelectedIndex == 1)
            {
                lab1_TE.Visibility = Visibility.Hidden;
                lab2_TE.Visibility = Visibility.Visible;
            }
        }
    }
}
