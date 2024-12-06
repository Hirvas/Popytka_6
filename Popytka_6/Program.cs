using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Text;

namespace GrayScottModel
{
	static class Program
	{
		static void Main()
		{
			Application.Run(new Form1());
		}
	}
	public partial class Form1 : Form
	{
		private int gridSize = 100;
		private double[,] A, B;
		private double feedRate = 0.029;
		private double killRate = 0.057;
		private double diffusionA = 1.0;
		private double diffusionB = 0.5;
		private double dt = 1.0;

		public Form1()
		{
			A = new double[gridSize, gridSize];
			B = new double[gridSize, gridSize];

			Random Random = new Random();

			// Инициализация
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					A[i, j] = 1.0; // Начальная концентрация A
					B[i, j] = 0.5; // Начальная концентрация B

					if (Random.Next(0, 100)  < 5)
					{
						A[i, j] = 0.5; // Начальная концентрация A
						B[i, j] = 1.0; // Начальная концентрация B
					}
				}
			}

			// Создание начального паттерна

			///*
			Timer timer = new Timer();
			timer.Interval = 1000; // Обновление каждые 1000 мс
			timer.Tick += Timer_Tick;
			timer.Start();
			//*/
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

			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					newA[i, j] =(Serkle(i, j, A, diffusionA) - A[i, j] * B[i, j] * B[i, j] + feedRate * (1 - A[i, j])) * dt;
					newB[i, j] =(Serkle(i, j, B, diffusionB) + A[i, j] * B[i, j] * B[i, j] - (killRate + feedRate) * B[i, j]) * dt;
				}
			}
			A = newA;
			B = newB;

        }

		private double Serkle(int x, int y, double[,] AB, double diffusion)
		{
			double mas = AB[x,y];

            for (int i = -1; i < 2; i++)
			{
                int a = SwitchXY(x, i);
                for (int j = -1; j < 2; j++)
				{
					int b = SwitchXY(y, j);
					if (i != 0 & j != 0)
					{ mas += AB[a, b] * 0.05; }
					else if (i == 0 & j == 0)
					{ mas += -AB[a, b]; }
					else
					{ mas += AB[a, b] * 0.2; }			
                }
			}
			return (mas * diffusion);			
		}

		private int SwitchXY(int XY, int IJ)
		{
			if (XY + IJ < 0) 
				{ XY = gridSize - 1; }
			else if (XY + IJ > gridSize - 1) 
				{ XY = 0; }
			else 
				{ XY = XY + IJ; }
	        return (XY);
        }


		/*
		private double ColorBlock(double Color)
		{
			if (Color < 0) { Color = 0; return (Color); }
			if (Color * 255 > 255) { Color = 255; return (Color); }
			return (Color*225);
		}
		//*/

		protected override void OnPaint(PaintEventArgs e)
		{
			/*
			double[,] RisA = Sigmoid(A);
            double[,] RisB = Sigmoid(B);
			//*/
            base.OnPaint(e);
            for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					//int ColorF = (int)(A[i, j] * B[i, j]/2*255);
                    //Color color = Color.FromArgb((int)RisA[i,j], (int)RisB[i, j], 0);
                    Color color = Color.FromArgb((int)(A[i, j] * 255), (int)(B[i, j] * 255), 255);
                    //Color color = Color.FromArgb(ColorF, ColorF, ColorF);
                    using (Brush brush = new SolidBrush(color))
					{
						e.Graphics.FillRectangle(brush, i * 5, j * 5, 5, 5); // Масштабирование для наглядности
					}
				}
			}
		}
	}
}
