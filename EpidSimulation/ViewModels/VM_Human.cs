using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using EpidSimulation.Models;
using System.Windows.Controls;
using System.Windows.Data;
using EpidSimulation.Utils;

namespace EpidSimulation.ViewModels
{
    class VM_Human : VM_BASIC
    {
        public Ellipse CondCircle;
        public Ellipse MaskCircle = null;
        public Ellipse SocDistCircle = null;
        public Human human;

        public VM_Human(Human human)
        {
            this.human = human;
            //===== Создание фигур
            CondCircle = CreateCircle(human.Position.X, human.Position.Y, Human.Config.RadiusHuman, Brushes.Green, Brushes.Black);
            CondCircle.StrokeThickness = Human.Config.RadiusHuman / 10;

            MaskCircle = CreateCircle(human.Position.X, human.Position.Y, Human.Config.RadiusHuman / 2, Brushes.White, Brushes.Black);
            MaskCircle.StrokeThickness = Human.Config.RadiusHuman / 10;
            MaskCircle.Visibility = System.Windows.Visibility.Hidden;
            if (human.Mask)
            {
                MaskCircle.Visibility = System.Windows.Visibility.Visible;
            }

            SocDistCircle = CreateCircle(human.Position.X, human.Position.Y, Human.Config.RadiusSocDist, Brushes.DeepSkyBlue, null);
            SocDistCircle.Visibility = System.Windows.Visibility.Hidden;

            //==== Привязка координаты Х
            Binding bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("X"),
                Converter = new SummParamConverter(),
                ConverterParameter = -Human.Config.RadiusHuman
            };
            CondCircle.SetBinding(Canvas.LeftProperty, bind);
            
            bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("X"),
                Converter = new SummParamConverter(),
                ConverterParameter = -Human.Config.RadiusHuman / 2
            };
            MaskCircle.SetBinding(Canvas.LeftProperty, bind);
                        
            bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("X"),
                    Converter = new SummParamConverter(),
                    ConverterParameter = -Human.Config.RadiusSocDist 
                };
            SocDistCircle.SetBinding(Canvas.LeftProperty, bind);
            
            //===== Привязка координаты Y
            bind = new Binding
            {
                Source = human,
                Path = new System.Windows.PropertyPath("Y"),
                Converter = new SummParamConverter(),
                ConverterParameter = -Human.Config.RadiusHuman
            };
            CondCircle.SetBinding(Canvas.TopProperty, bind);

            bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("Y"),
                    Converter = new SummParamConverter(),
                    ConverterParameter = -Human.Config.RadiusHuman / 2
                };
            MaskCircle.SetBinding(Canvas.TopProperty, bind);
            
            bind = new Binding
                {
                    Source = human,
                    Path = new System.Windows.PropertyPath("Y"),
                    Converter = new SummParamConverter(),
                    ConverterParameter = -Human.Config.RadiusSocDist 
                };
            SocDistCircle.SetBinding(Canvas.TopProperty, bind);
            
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
