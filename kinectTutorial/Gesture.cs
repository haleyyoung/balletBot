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
using Microsoft.Kinect;
using kinectTutorial;

namespace kinectTutorial
{
    /// <summary>
    /// The Gesture class handles the interaction of the user with a specific
    /// button or other type of selection object on the interface.
    /// A Gesture allows the user to control the current state of their dance
    /// session.
    /// </summary>
    class Gesture
    {
        private int frameCount = 0;
        private int holdButtonLength = 30;
        public Boolean gestureComplete = false;
        public Skeleton skeleton = null;
        public Canvas canvas;
        public Rectangle button;
        public Boolean buttonOn;
        public Brush buttonOnColor = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#59B6A0");
        public Brush buttonOnBorderColor = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#DEF0EC");
        public Brush buttonOffColor = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#E02B50");
        public Brush buttonOffBorderColor = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#EBFFE0");

        public Gesture(Canvas canvas, Skeleton skeleton, Rectangle button, Boolean buttonOn)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;
            this.button = button;
            this.buttonOn = buttonOn;
        }

        public Boolean gestureStart()
        {
            double windowWidth = canvas.Width;
            double windowHeight = canvas.Height;
            Joint hand = skeleton.Joints[JointType.HandLeft];
            double handYDrawn = (windowHeight / 1.25 - 5) + hand.Position.Y * -(windowHeight / 2);
            double handXDrawn = hand.Position.X * (windowWidth / 2) + (windowWidth / 2 - 5);
            double buttonTop = Canvas.GetTop(this.button);
            double buttonBottom = buttonTop + this.button.ActualHeight;
            double buttonLeft = Canvas.GetLeft(this.button);
            double buttonRight = buttonLeft + this.button.ActualWidth;
            Rectangle buttonFill = new Rectangle();

            if (handYDrawn >= buttonTop && handYDrawn <= buttonBottom &&
                handXDrawn >= buttonLeft && handXDrawn <= buttonRight)
            {
                if (this.frameCount == this.holdButtonLength)
                {
                    return false;
                }
                    this.frameCount++;

                    buttonFill.Width = this.button.ActualWidth * this.frameCount / this.holdButtonLength;
                    buttonFill.Height = this.button.ActualHeight;
                    buttonFill.Fill = buttonOn ? this.buttonOffColor : this.buttonOnColor;
                    buttonFill.Stroke = buttonOn ? this.buttonOnBorderColor : this.buttonOffBorderColor;
                    canvas.Children.Add(buttonFill);
                    Canvas.SetTop(buttonFill, Canvas.GetTop(this.button));
                    Canvas.SetLeft(buttonFill, Canvas.GetLeft(this.button));
            }
            else
            {
                this.frameCount = 0;
            }
            if (this.frameCount == this.holdButtonLength)
            {
                this.buttonOn = !this.buttonOn;
                this.button.Fill = buttonFill.Fill;
                this.button.Stroke = buttonFill.Stroke;
                return true;
            }
            return false;
        }

        public void turnOffButton()
        {
            this.buttonOn = false;
            this.button.Fill = this.buttonOffColor;
            this.button.Stroke = this.buttonOffBorderColor;
        }
    }
}