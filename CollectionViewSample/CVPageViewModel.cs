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
        private double fsize = 18;
        private double w = 0;
        private double w0 = 0;
        private double w1 = 0;
        private double w2 = 0;
        private double w3 = 0;
        double rh = 0;
        private double cvgheight = 0;
        private double numericentrywidth = 40;
        private bool settingsvisible = false;
        private List<CVcontent> cvc = new();

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public List<CVcontent> Cvc
        {
            get { return cvc; }
            set
            {
                ContentColumnsWidth(value);
                cvc = value;
                OnPropertyChanged(nameof(Cvc));
            }
        }

        public double CvgHeight
        {
            get { return cvgheight; }
            set
            {
                cvgheight = value;
                OnPropertyChanged(nameof(CvgHeight));
            }
        }

        public void CorrectStarColumnWidth(double width)
        {
            double _w = Math.Max(width, w0 + w1 + w2 + w3);
            W = _w;
            OnPropertyChanged(nameof(W));
            for (int i = 0; i < cvc.Count; i++)
            {
                cvc[i].W = _w;
            }
        }
        private void ContentColumnsWidth(List<CVcontent> clist)
        {
            double fsize = Fsize;
            double _w0 = ScreenMetrics.MeasureTextWidth("55555", fsize) + 20;
            double _w1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + 40;
            double _w2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + 40;
            double _w3 = 0;
            for (int i = 0; i < clist.Count; i++)
            {
                _w0 = Math.Max(_w0, ScreenMetrics.MeasureTextWidth(clist[i].ItemNo, fsize) + 20);
                _w1 = Math.Max(_w1, ScreenMetrics.MeasureTextWidth(clist[i].FirstName, fsize) + 20);
                _w2 = Math.Max(_w2, ScreenMetrics.MeasureTextWidth(clist[i].LastName, fsize) + 20);
                if (!clist[i].Occupation.Equals(string.Empty))
                    _w3 = Math.Max(_w3, ScreenMetrics.MeasureTextWidth(clist[i].Occupation, fsize) + 20);

            }
            double cw = Math.Max(SafeWidth, _w0 + _w1 + _w2 + _w3);
            W = cw;
            for (int i = 0; i < clist.Count; i++)
            {
                clist[i].Cw0 = _w0;
                clist[i].Cw1 = _w1;
                clist[i].Cw2 = _w2;
                if (_w3 > 0)
                    clist[i].Cw3 = _w3;
                clist[i].W = cw;
                clist[i].Rh = rh;
            }

            Cw0 = _w0;
            Cw1 = _w1;
            Cw2 = _w2;
            if (_w3 > 0)
                Cw3 = _w3;
        }
        public int CvcCount
        {
            get { return cvc.Count; }
        }
        public double SafeWidth { get; set; }
        public double Fsize
        {
            get { return fsize; }
            set
            {
                fsize = value;
                Cw0 = ScreenMetrics.MeasureTextWidth("55555", value) + 20;
                Cw1 = ScreenMetrics.MeasureTextWidth("Column 1", value) + 40;
                Cw2 = ScreenMetrics.MeasureTextWidth("Column 2", value) + 40;
                Cw3 = ScreenMetrics.MeasureTextWidth("Column 3", value) + 40;
                Rh = ScreenMetrics.MeasureTextHeight("abcdef", value) + 10;
                NumericEntryWidth = ScreenMetrics.MeasureTextWidth("555555", value) + 10;
            }
        }
        public double NumericEntryWidth
        {
            get { return numericentrywidth; }
            set
            {
                numericentrywidth = value;
                OnPropertyChanged(nameof(NumericEntryWidth));
            }
        }
        public bool SettingsVisible
        {
            get { return settingsvisible; }
            set
            {
                settingsvisible = value;
                OnPropertyChanged(nameof(SettingsVisible));
            }
        }
        //DataTemplate WidthRequest
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

        public double Cw3 
        {
            get { return w3;}
            set { w3 = value; }
        }
        public double Rh
        {
            get { return rh; }
            set
            {
                rh = value;
                OnPropertyChanged(nameof(H));
            }
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
            get { return new GridLength(rh, GridUnitType.Absolute); }
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
