using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using EpidSimulation.Backend;
using System.Windows.Controls;
using System.Windows.Data;

namespace EpidSimulation.Frontend
{
    class FigureHuman
    {
        public Ellipse CondCircle;
        public Ellipse MaskCircle = null;
        public Ellipse SocDistCircle = null;
        public Human human;

        public FigureHuman(Human human)
        {
            this.human = human;
            //===== Создание фигур
            CondCircle = CreateCircle(human.X, human.Y, Human.Config.RadiusMan, Brushes.Green, Brushes.Black);
            CondCircle.StrokeThickness = Human.Config.RadiusMan / 10;
            if (human.Mask)
            {
                MaskCircle = CreateCircle(human.X, human.Y, Human.Config.RadiusMan / 2, Brushes.White, Brushes.Black);
                MaskCircle.StrokeThickness = Human.Config.RadiusMan / 10;
            }
            if (human.SocDist)
            {
                SocDistCircle = CreateCircle(human.X, human.Y, Human.Config.RadiusSoc, Brushes.DeepSkyBlue, null);
                SocDistCircle.Visibility = System.Windows.Visibility.Hidden;
            }

            //==== Привязка координаты Х
            Binding bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("X"),
                Converter = new LeftToCenterConverter(),
                ConverterParameter = -Human.Config.RadiusMan
            };
            CondCircle.SetBinding(Canvas.LeftProperty, bind);

            if (MaskCircle != null)
            {
                bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("X"),
                    Converter = new LeftToCenterConverter(),
                    ConverterParameter = -Human.Config.RadiusMan / 2
                };
                MaskCircle.SetBinding(Canvas.LeftProperty, bind);
            }
            if (SocDistCircle != null)
            {
                bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("X"),
                    Converter = new LeftToCenterConverter(),
                    ConverterParameter = -Human.Config.RadiusSoc 
                };
                SocDistCircle.SetBinding(Canvas.LeftProperty, bind);
            }
            //===== Привязка координаты Y
            bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("Y"),
                Converter = new LeftToCenterConverter(),
                ConverterParameter = -Human.Config.RadiusMan 
            };
            CondCircle.SetBinding(Canvas.TopProperty, bind);

            if (MaskCircle != null)
            {
                bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("Y"),
                    Converter = new LeftToCenterConverter(),
                    ConverterParameter = -Human.Config.RadiusMan / 2
                };
                MaskCircle.SetBinding(Canvas.TopProperty, bind);
            }
            if (SocDistCircle != null)
            {
                bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("Y"),
                    Converter = new LeftToCenterConverter(),
                    ConverterParameter = -Human.Config.RadiusSoc 
                };
                SocDistCircle.SetBinding(Canvas.TopProperty, bind);
            }

            //===== Привязка состояния человека
            bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("Condition"),
                Converter = new IntToBrushConverter(),
                ConverterParameter = "Cond"
            };
            CondCircle.SetBinding(Ellipse.FillProperty, bind);

            //===== Привязка состояния рук человека
            bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("InfectHand"),
                Converter = new IntToBrushConverter(),
                ConverterParameter = "Hand"
            };
            CondCircle.SetBinding(Ellipse.StrokeProperty, bind);

        }


        private Ellipse CreateCircle(double x, double y, double radius, Brush brushFill, Brush brushStroke)
        {
            Ellipse circle = new Ellipse
            {
                Width = 2 * radius,
                Height = 2 * radius,
                Fill = brushFill,
            };
            if (brushStroke != null)
                circle.Stroke = brushStroke;
            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
            return circle;
        }

        public void SetVisible(int visible, bool debug)
        {
            CondCircle.Visibility = (System.Windows.Visibility)visible;
            if (MaskCircle != null)
                MaskCircle.Visibility = (System.Windows.Visibility)visible;
            if (SocDistCircle != null)
            {
                if (debug && visible == 0)
                    SocDistCircle.Visibility = System.Windows.Visibility.Visible;
                else
                    SocDistCircle.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    
    }
}
