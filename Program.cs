using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace TensorShow
{
    class Program
    {
        static void Main()
        {
            int epochs = 6;
            double learningRate = 0.01;

            Optimizer gd = new GradientDescentOptimizer(learningRate);

            var W = new Parameter();
            var b = new Parameter();

            double[] X ={ 3.3,4.4,5.5,6.71,6.93,4.168,9.779,6.182,7.59,2.167, 7.042,10.791,5.313,7.997,5.654,9.27,3.1 };
            double[] y = {1.7, 2.76, 2.09, 3.19, 1.694, 1.573, 3.366, 2.596, 2.53, 1.221, 2.827, 3.465, 1.65, 2.904, 2.42, 2.94, 1.3 };

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                var loss = new Variable(0);
                for (int i = 0; i < X.Length; i++)
                {
                    var prediction = W * X[i] + b;
                    var error = (prediction + -y[i]).Power(2);
                    loss += error;
                }
                Console.WriteLine("[Epoch: {0}] mean error = {1}", epoch, loss.Value / X.Length);
                TensorSlow.Minimize(gd, loss);
            }

            Console.WriteLine(" y = {0}x + {1}", W.Value, b.Value);

            Chart chart = new Chart();
            chart.Size = new System.Drawing.Size(600, 600);
            chart.ChartAreas.Add("ChartArea1");
            chart.Legends.Add("legend1");

            chart.Series.Add("points");
            for (int i = 0; i < X.Length; i++)
                chart.Series["points"].Points.AddXY(X[i], y[i]);
            chart.Series["points"].ChartType = SeriesChartType.Point;

            chart.Series.Add("line");
            chart.Series["line"].Points.AddXY(0, b.Value);
            chart.Series["line"].Points.AddXY(X.Max(), W.Value * X.Max() + b.Value);
            chart.Series["line"].ChartType = SeriesChartType.Line;

            chart.SaveImage("output.jpg", ChartImageFormat.Jpeg);
            Console.ReadKey();
        }
    }
}
