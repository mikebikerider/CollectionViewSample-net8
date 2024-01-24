using System.ComponentModel;

namespace CollectionViewSample
{
    public class CVcontent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private bool isSelected = false;
        private double w;

        public int ItemNumber { get; set; }
        public string ItemNo
        {
            get { return ItemNumber.ToString(); }
        }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public bool IsLightTheme { get; set; } = true;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(textcolor)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(color)));
            }
        }
        public Color color
        {
            get
            {
                if (IsLightTheme)
                {
                    if (isSelected)
                        return Colors.Black;
                    else
                        return Colors.White;
                }
                else
                {
                    if (isSelected)
                        return Colors.White;
                    else
                        return Colors.Black;
                }
            }
        }
        public Color textcolor
        {
            get
            {
                if (IsLightTheme)
                {
                    if (isSelected)
                        return Colors.White;
                    else
                        return Colors.Black;
                }
                else
                {
                    if (isSelected)
                        return Colors.Black;
                    else
                        return Colors.White;
                }
            }
        }
        public double W
        {
            get { return w; }

            set
            {
                w = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(W)));
            }
        }
        public double Cw0 { get; set; }
        public double Cw1 { get; set; }
        public double Cw2 { get; set; }
        public double Cw3 { get; set; }
        public double Rh { get; set; }
        public double Cw
        {
            get { return Cw0 + Cw1 + Cw2 + Cw3; }
        }
        public GridLength W0
        {
            get { return new GridLength(Cw0, GridUnitType.Absolute); }
        }
        public GridLength W1
        {
            get { return new GridLength(Cw1, GridUnitType.Absolute); }
        }
        public GridLength W2
        {
            get { return new GridLength(Cw2, GridUnitType.Absolute); }
        }
        public GridLength W3
        {
            get { return new GridLength(Cw3, GridUnitType.Absolute); }
        }
        public GridLength H
        {
            get { return new GridLength(Rh, GridUnitType.Absolute); }
        }
    }
}
