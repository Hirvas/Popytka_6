using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;


namespace GrayScottModel
{
	static class Program
	{
		static void Main()
		{
			Application.Run(new Form1());
			//33
		}
	}
	public partial class Form1 : Form
	{
		private int gridSize = 100;
		private double[,] A, B;
		private double feedRate = 0.055;
		private double killRate = 0.062;
		private double diffusionA = 1.0;
		private double diffusionB = 0.5;
		private double dt = 1.0;

		public Form1()
		{
			A = new double[gridSize, gridSize];
			B = new double[gridSize, gridSize];

			// Инициализация
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					A[i, j] = 1.0; // Начальная концентрация A
					B[i, j] = 0.0; // Начальная концентрация B

					if (new Random().Next(0, 100) < 20)
					{
						A[i, j] = 0.0; // Начальная концентрация A
						B[i, j] = 1.0; // Начальная концентрация B
					}
				}
			}

			// Создание начального паттерна


			Timer timer = new Timer();
			timer.Interval = 1000; // Обновление каждые 1000 мс
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateSimulation();
			Invalidate(); // Перерисовка формы
		}

		private void UpdateSimulation()
		{
			double[,] newA = new double[gridSize, gridSize];
			double[,] newB = new double[gridSize, gridSize];

			for (int i = 1; i < gridSize - 1; i++)
			{
				for (int j = 1; j < gridSize - 1; j++)
				{
					double laplacianA = A[i - 1, j] + A[i + 1, j] + A[i, j - 1] + A[i, j + 1] - 4 * A[i, j];
					double laplacianB = B[i - 1, j] + B[i + 1, j] + B[i, j - 1] + B[i, j + 1] - 4 * B[i, j];

					newA[i, j] = A[i, j] + (diffusionA * laplacianA - A[i, j] * B[i, j] * B[i, j] + feedRate * (1 - A[i, j])) * dt;
					newB[i, j] = B[i, j] + (diffusionB * laplacianB + A[i, j] * B[i, j] * B[i, j] - (killRate + feedRate) * B[i, j]) * dt;
				}
			}



			A = TtOtT(newA);
			B = TtOtT(newB);
		}



		private double[,] TtOtT(double[,] Mass)
		{
			// Преобразование двумерного массива в одномерный
			var flattened = Mass.Cast<double>().ToArray();

			// Применение функции сигмоиды ко всем элементам массива
			var sigmoids = flattened.Select(x => 1.0 / (1.0 + Math.Exp(-x))).ToArray();

			// Преобразование обратно в двумерный массив
			double[,] result = new double[Mass.GetLength(0), Mass.GetLength(1)];
			Buffer.BlockCopy(sigmoids, 0, result, 0, sigmoids.Length * sizeof(double));
			return (result);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					Color color = Color.FromArgb((int)(A[i, j] * 255), (int)(B[i, j] * 255), 0);
					using (Brush brush = new SolidBrush(color))
					{
						e.Graphics.FillRectangle(brush, i * 5, j * 5, 5, 5); // Масштабирование для наглядности
					}
				}
			}
		}
	}
}
