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

        public Gesture(Canvas canvas, Skeleton skeleton, Rectangle button, Boolean buttonOn)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;
            this.button = button;
            this.buttonOn = buttonOn;
        }

        public Boolean gestureStart()
        {
            Joint hand = skeleton.Joints[JointType.HandLeft];
            double handYDrawn = hand.Position.Y * -250 + 247.5;
            double handXDrawn = hand.Position.X * 250 + 247.5;
            double pliesButtonTop = Canvas.GetTop(this.button);
            double pliesButtonBottom = pliesButtonTop + 23;
            double pliesButtonLeft = Canvas.GetLeft(this.button);
            double pliesButtonRight = pliesButtonLeft + 75;
            if (handYDrawn >= pliesButtonTop && handYDrawn <= pliesButtonBottom && handXDrawn >= pliesButtonLeft && handXDrawn <= pliesButtonRight)
            {
                this.frameCount++;

                Rectangle buttonFill = new Rectangle();
                buttonFill.Width = this.button.ActualWidth * this.frameCount/this.holdButtonLength;
                buttonFill.Height = this.button.ActualHeight;
                buttonFill.Fill = buttonOn ? Brushes.Red : Brushes.GreenYellow;

                TextBlock buttonText = new TextBlock();
                buttonText.Text = "Plies";
                buttonText.Foreground = Brushes.WhiteSmoke;
                canvas.Children.Add(buttonText);
                canvas.Children.Add(buttonFill);
                Canvas.SetTop(buttonFill, Canvas.GetTop(this.button));
                Canvas.SetLeft(buttonFill, Canvas.GetLeft(this.button));
                Canvas.SetTop(buttonText, Canvas.GetTop(this.button) + 5);
                Canvas.SetLeft(buttonText, Canvas.GetLeft(this.button) + 5);
            }
            else
            {
                this.frameCount = 0;
            }
            if (this.frameCount == this.holdButtonLength)
            {
                this.frameCount = 0;
                this.buttonOn = !this.buttonOn;
                return true;
            }
            return false;
        }
    }
}
