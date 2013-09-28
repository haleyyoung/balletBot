using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace kinectTutorial
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class MovementMode
    {
        public Canvas canvas;
        public Skeleton skeleton;

        public MovementMode(Canvas canvas, Skeleton skeleton)
        {
            this.canvas = canvas;
            this.skeleton = skeleton;
        }

        public void Plies()
        {
            if (skeleton != null)
            {
                TextBlock text = new TextBlock();
                text.FontSize = 25;
                text.Foreground = Brushes.WhiteSmoke;
                Canvas.SetTop(text, 450);
                Canvas.SetLeft(text, 100);

                Joint leftHip = (skeleton.Joints[JointType.HipLeft]);
                Joint rightHip = (skeleton.Joints[JointType.HipRight]);
                Range range = new Range(leftHip.Position.Y, Range.intermediateRange);
                if (rightHip.Position.Y < range.minimum)
                {
                    text.Text = "Your RIGHT HIP is LOWER than your left";
                    text.Background = Brushes.DarkCyan;
                }
                else if (rightHip.Position.Y > range.maximum)
                {
                    text.Text = "Your LEFT HIP is LOWER than your right";
                    text.Background = Brushes.Purple;
                }
                canvas.Children.Add(text);
            }
        }
    }

    public class Range
    {
        public const float easyRange = 0.005f;
        public const float intermediateRange = 0.003f;
        public const float hardRange = 0.001f;

        public float minimum;
        public float maximum;

        public Range(float rangeBasis, float level)
        {
            this.minimum = rangeBasis - level;
            this.maximum = rangeBasis + level;
        }
    }
}
