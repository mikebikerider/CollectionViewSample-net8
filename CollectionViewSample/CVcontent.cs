using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollectionViewSample
{
    public class CVcontent : INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        public event PropertyChangedEventHandler? PropertyChanged;
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


        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(textcolor));
                OnPropertyChanged(nameof(color));
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
#if ANDROID
                OnPropertyChanged(nameof(W));
#endif
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
        public GridLength H
        {
            get { return new GridLength(Rh,GridUnitType.Absolute);}
        }
    }

}
