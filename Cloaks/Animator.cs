using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Cloaks
{
    public class Animator
    {
        private static readonly TimeSpan HALF_SECOND = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan FULL_SECOND = TimeSpan.FromMilliseconds(1000);

        private readonly Storyboard StoryBoard = new Storyboard();
        private IEasingFunction Smooth { get; set; }

        public Animator()
        {
            Smooth = new QuarticEase()
            {
                EasingMode = EasingMode.EaseInOut
            };
        }


        public void Fade(DependencyObject Object)
        {
            DoubleAnimation FadeIn = new DoubleAnimation()
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(HALF_SECOND),
            };
            Storyboard.SetTarget(FadeIn, Object);
            Storyboard.SetTargetProperty(FadeIn, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(FadeIn);
            StoryBoard.Begin();
        }

        public void FadeOut(DependencyObject Object)
        {
            DoubleAnimation Fade = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(HALF_SECOND),
            };
            Storyboard.SetTarget(Fade, Object);
            Storyboard.SetTargetProperty(Fade, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(Fade);
            StoryBoard.Begin();
        }

        public void ObjectShift(DependencyObject Object, Thickness Get, Thickness Set)
        {
            ThicknessAnimation Animation = new ThicknessAnimation()
            {
                From = Get,
                To = Set,
                Duration = FULL_SECOND,
                EasingFunction = Smooth,
            };
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(FrameworkElement.MarginProperty));
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }
    }
}