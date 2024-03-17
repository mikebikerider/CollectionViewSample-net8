using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CollectionViewSample
{
    class CVPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool isBusy = false;
        private bool buttonEnabled = true;
        private string updownText = "Bottom";
        private string updownGlyph = "\ue258";
        private double cvopacity = 1;
        private double w = 0;
        private double w0 = 0;
        private double w1 = 0;
        private double w2 = 0;
        private List<CVcontent> cvc = new List<CVcontent>();

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public List<CVcontent> Cvc
        {
            get { return cvc; }
            set
            {
                cvc = value;
                OnPropertyChanged(nameof(Cvc));
            }
        }
        public void UpdateDataSource()
        {
            OnPropertyChanged(nameof(Cvc));
        }
        public double W
        {
            get { return w; }

            set
            {
                w = value;
                OnPropertyChanged(nameof(W));
            }
        }
        public double Cw0
        {
            get { return w0; }
            set
            {
                w0 = value;
                OnPropertyChanged(nameof(W0));
            }
        }
        public double Cw1
        {
            get { return w1; }
            set
            {
                w1 = value;
                OnPropertyChanged(nameof(W1));
            }
        }
        public double Cw2
        {
            get { return w2; }
            set
            {
                w2 = value;
                OnPropertyChanged(nameof(W2));
            }
        }
        /*
        public double Cw3 
        {
            get { return w3;}
            set { w3 = value; }
        }
        public double Cw
        {
            get { return Cw0 + Cw1 + Cw2 + Cw3; }
        }
        */
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

        public bool ButtonEnabled
        {
            get { return buttonEnabled; }
            set { buttonEnabled = value; }
        }
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                buttonEnabled = !value;
                if (value)
                    cvopacity = .5;
                else
                    cvopacity = 1;
                OnPropertyChanged(nameof(ButtonEnabled));
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(CVopacity));
            }
        }
        public string UpDownText
        {
            get { return updownText; }
            set
            {
                updownText = value;
                OnPropertyChanged(nameof(UpDownText));
            }
        }
        public bool GlyphCorrection
        {
            get
            {
#if IOS
                return false;
#else
                return Preferences.Get("AndroidGlyphCorrecion", false);
#endif
            }
        }
        public string UpDownGlyph
        {
            get { return updownGlyph; }
            set
            {
                updownGlyph = value;
                OnPropertyChanged(nameof(UpDownGlyph));
            }
        }
        public double CVopacity
        {
            get { return cvopacity; }
            set { cvopacity = value; }
        }
        public void UpdateUpDownGlyph()
        {
            string temp = updownGlyph;
            UpDownGlyph = "";
            UpDownGlyph = temp;
        }
        private string bytearray2string(byte[] b)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string glyph2string(string glyph)
        {
            byte[] b = Encoding.BigEndianUnicode.GetBytes(glyph);
            return bytearray2string(b);
        }
    }
}
