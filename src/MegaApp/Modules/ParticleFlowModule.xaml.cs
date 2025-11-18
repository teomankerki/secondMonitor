using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MegaApp.Modules;

public partial class ParticleFlowModule : UserControl
{
    private readonly DispatcherTimer _timer;
    private readonly List<Particle> _particles = new();
    private readonly Random _random = new();

    public ParticleFlowModule()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _timer.Tick += (_, _) => Animate();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SpawnParticles(24);
        _timer.Start();
    }

    private void SpawnParticles(int count)
    {
        ParticleCanvas.Children.Clear();
        _particles.Clear();

        for (var i = 0; i < count; i++)
        {
            var size = _random.Next(6, 18);
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(Color.FromArgb(120, 127, (byte)_random.Next(160, 220), 174))
            };

            ParticleCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, _random.NextDouble() * Math.Max(1, ParticleCanvas.ActualWidth));
            Canvas.SetTop(ellipse, _random.NextDouble() * Math.Max(1, ParticleCanvas.ActualHeight));

            _particles.Add(new Particle
            {
                Shape = ellipse,
                VelocityX = (_random.NextDouble() - 0.5) * 0.8,
                VelocityY = (_random.NextDouble() - 0.5) * 0.8
            });
        }
    }

    private void Animate()
    {
        if (ParticleCanvas.ActualWidth == 0)
        {
            return;
        }

        foreach (var particle in _particles)
        {
            var left = Canvas.GetLeft(particle.Shape) + particle.VelocityX;
            var top = Canvas.GetTop(particle.Shape) + particle.VelocityY;

            if (left < 0 || left > ParticleCanvas.ActualWidth - particle.Shape.Width)
            {
                particle.VelocityX *= -1;
            }

            if (top < 0 || top > ParticleCanvas.ActualHeight - particle.Shape.Height)
            {
                particle.VelocityY *= -1;
            }

            Canvas.SetLeft(particle.Shape, Math.Clamp(left, 0, Math.Max(0, ParticleCanvas.ActualWidth - particle.Shape.Width)));
            Canvas.SetTop(particle.Shape, Math.Clamp(top, 0, Math.Max(0, ParticleCanvas.ActualHeight - particle.Shape.Height)));
        }
    }

    private sealed class Particle
    {
        public required Ellipse Shape { get; init; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
    }
}
