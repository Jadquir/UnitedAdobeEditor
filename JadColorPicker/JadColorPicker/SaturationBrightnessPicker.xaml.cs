﻿using System;
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

namespace JadColorPicker
{
    /// <summary>
    /// SaturationBrightnessPicker.xaml etkileşim mantığı
    /// </summary>
    internal partial class SaturationBrightnessPicker : UserControl
    {
        public static readonly DependencyProperty HueProperty
           = DependencyProperty.Register(nameof(Hue), typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));
        public static readonly DependencyProperty SaturationProperty
            = DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0, OnSaturationChanged));
        public static readonly DependencyProperty BrightnessProperty
            = DependencyProperty.Register(nameof(Brightness), typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0, OnBrightnessChanged));
        private readonly SaturationBrightnessPickerAdorner _adorner;

        public SaturationBrightnessPicker()
        {
            InitializeComponent();
            _adorner = new SaturationBrightnessPickerAdorner(this);
            Loaded += SaturationBrightnessPickerOnLoaded;
        }

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Mouse.Capture(this);
            var pos = e.GetPosition(this).Clamp(this);
            Update(pos);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Mouse.Capture(null);
            var pos = e.GetPosition(this).Clamp(this);
            Update(pos);
        }

        private static void OnSaturationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var picker = (SaturationBrightnessPicker)o;
            var sat = (double)e.NewValue;
            var pos = picker._adorner.Position;
            picker._adorner.Position = new Point(sat * picker.ActualWidth, pos.Y);
        }

        private static void OnBrightnessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var picker = (SaturationBrightnessPicker)o;
            var bright = (double)e.NewValue;
            var pos = picker._adorner.Position;
            picker._adorner.Position = new Point(pos.X, (1 - bright) * picker.ActualHeight);
        }

        private void SaturationBrightnessPickerOnLoaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(_adorner);
            _adorner.Position = new Point(Saturation * ActualWidth, (1 - Brightness) * ActualHeight);
        }

        private void Update(Point p)
        {
            _adorner.Position = p;
            Saturation = p.X / ActualWidth;
            Brightness = 1 - (p.Y / ActualHeight); // directions reversed
        }
    }
}
