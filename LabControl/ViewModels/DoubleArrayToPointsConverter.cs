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
        private double widthValue = 100;
        private double scaleValue = 1;
        private double offsetValue;
        #endregion

        #region Properties

        public double Width
        {
            get { return widthValue; }
            set { widthValue = value; }
        }

        public double Scale
        {
            get { return scaleValue; }
            set { scaleValue = value; }
        }

        public double Offset
        {
            get { return offsetValue; }
            set { offsetValue = value; }
        }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] values = value as double[];
            if (values == null)
            {
                throw new ArgumentException("value", "Must be double[]");
            }

            PointCollection points = new PointCollection(values.Length);
            for (int i = 0; i < values.Length; ++i)
            {
                double x = i * Width / values.Length;
                double y = values[i] * Scale + Offset;
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
