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
    class Gesture
    {
        private int frameCount = 0;
        public Boolean gestureComplete = false;
        public Skeleton skeleton = null;
        public Canvas canvas;
        public UIElement button;

        public Gesture(Canvas canvas, Skeleton skeleton, UIElement button)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;
            this.button = button;
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            TextBlock blah = new TextBlock();
            Canvas.SetTop(blah, 0);
            Canvas.SetLeft(blah, 500);
            blah.Text = "FIRED!!!!";
            canvas.Children.Add(blah);
            gestureStart();
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
            TextBlock text = new TextBlock();
            if (handYDrawn >= pliesButtonTop && handYDrawn <= pliesButtonBottom && handXDrawn >= pliesButtonLeft && handXDrawn <= pliesButtonRight)
            {
                text.FontSize = 25;
                text.Foreground = Brushes.WhiteSmoke;
                Canvas.SetTop(text, 0);
                Canvas.SetLeft(text, 100);
                text.Inlines.Add("here!!!????!?! " + this.frameCount);
                text.Background = Brushes.Purple;
                canvas.Children.Add(text);
                this.frameCount++;
            }
            else
            {
                this.frameCount = 0;
            }
            if (this.frameCount == 15)
            {
                return true;
            }
            return false;
        }
    }
}
