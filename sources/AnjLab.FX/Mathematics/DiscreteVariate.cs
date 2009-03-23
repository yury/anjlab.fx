using System;
using System.Collections.Generic;

namespace AnjLab.FX.Mathematics
{
    public class DiscreteVariate
    {
        private readonly IList<double> _values = new List<double>();

        public DiscreteVariate()
        {
        }

        public DiscreteVariate(IEnumerable<double> values)
        {
            foreach (double value in values)
                _values.Add(value);
        }

        public void AddValue(double value)
        {
            _values.Add(value);
        }

        public double Mx
        {
            get
            {
                double sum = 0;
                foreach (double value in _values)
                    sum += value;
                return sum / _values.Count;
            }
        }

        public double Dx
        {
            get
            {
                double mathExpectation = Mx;

                DiscreteVariate var = new DiscreteVariate();
                foreach (double value in _values)
                    var.AddValue(Math.Pow(value - mathExpectation, 2));

                return var.Mx;
            }
        }

        public double Sx
        {
            get { return Math.Sqrt(Dx); }
        }

        public double MathExpectation { get { return Mx; } }
        public double Dispersion { get { return Dx; } }
        public double QuadraticDeviation { get { return Sx; } }
    }
}
