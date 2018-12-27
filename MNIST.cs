using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace TensorShow
{
    class MNIST
    {
        struct Dataset
        {
            public double[] X { get; set; }
            public int[] y { get; set; }
            public int Samples { get; set; }
            public int Features { get; set; }
        }

        static void Evaluate(Func<double[], int> model, double[] testX, int[] testY)
        {
            Console.WriteLine("Evaluating...");

            int correct = 0;
            for (int i = 0; i < testY.Length; i++)
            {
                int prediction = model(testX.Skip(i * 784).Take(784).ToArray());
                Console.WriteLine($"Prediction: {prediction}, Actually: {testY[i]}");
                correct += (prediction == testY[i]) ? 1 : 0;
            }
            Console.WriteLine($"Correct: {correct}/{testY.Length}");
            Console.ReadLine();
        }

        static Dataset LoadMNIST()
        {
            double[] images = (from img_byte in File.ReadAllText("data/train.dat").Split(' ')
                               select Double.Parse(img_byte) / 255).ToArray();

            int[] labels = (from img_byte in File.ReadAllText("data/labels.dat").Split(' ')
                            select Int32.Parse(img_byte)).ToArray();

            return new Dataset
            {
                X = images,
                y = labels,
                Samples = 60000,
                Features = 784
            };
        }

        static void MainMNIST(string[] args)
        {
            Optimizer gd = new GradientDescentOptimizer(0.01);

            var W = Matrix.Load("W.model");
            var b = Matrix.Load("b.model");

            Console.WriteLine("Loading data...");
            Dataset data = LoadMNIST();
            

            int epochs = 1, batch_size = 128;
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int batch = 0; batch < 20000 / batch_size; batch++)
                {
                    var error = new Variable(0);
                    for (int i = 0; i < batch_size; i++)
                    {
                        int skip = batch * batch_size + i;

                        Vector x = new Vector(data.Features, data.X.Skip(skip * data.Features).Take(data.Features).ToArray());
                        Vector y = Vector.OneHot(data.y[skip], 10);

                        error += Vector.ReduceSquared((W * x + b) - y) * (1.0 / batch_size);
                    }
                    TensorSlow.Minimize(gd, error);
                    Console.WriteLine($"[Batch {epoch + 1}.{batch + 1}] Loss: {error.Value}");
                }
            }

            W.Save("W.model");
            b.Save("b.model");
            Console.ReadLine();

            //var W = ReadFromBinaryFile<Matrix>("W.model");
            //var b = ReadFromBinaryFile<Vector>("b.model");

            int skiptest = 20000, take = 500;
            double[] testX = data.X.Skip(skiptest * data.Features).Take(take * data.Features).ToArray();
            int[] testY = data.y.Skip(skiptest).Take(take).ToArray();
            Func<double[], int> model = x => Vector.ArgMax(W * (new Vector(data.Features, x)) + b);
            Evaluate(model, testX, testY);
        }
    }
}
