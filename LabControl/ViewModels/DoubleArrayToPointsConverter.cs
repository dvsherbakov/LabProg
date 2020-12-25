using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LabControl.ViewModels
{
    [ValueConversion(typeof(double[]), typeof(PointCollection))]
    internal class DoubleArrayToPointsConverter : IValueConverter
    {
        #region Fields
        private double f_WidthValue = 250;
        private int f_ScaleValue = 1;
        private double f_OffsetValue;
        #endregion

        #region Properties
        
        public double Width
        {
            get => f_WidthValue;
            set => f_WidthValue = value;
        }
        
        public int Scale
        {
            private get { return f_ScaleValue; }
            set { f_ScaleValue = value; }
        }

        public double Offset
        {
            get => f_OffsetValue;
            set { f_OffsetValue = value; }
        }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double[] values))
            {
                throw new ArgumentException("value", "Must be double[]");
            }

            var points = new PointCollection(values.Length);
            for (var i = 0; i < values.Length; ++i)
            {
                var x = i * Width / values.Length;
                var y = values[i] * Scale + Offset;
                points.Add(new Point(x, y));
            }

            return points;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
