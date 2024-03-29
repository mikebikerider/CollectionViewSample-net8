﻿using System.ComponentModel;
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
        private double w3 = 0;
        private double cvheightrequest;
        private double cvwidthrequest;
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
                cvc = value;
                SetColumnsWidth();
                OnPropertyChanged(nameof(Cvc));
            }
        }
        public void UpdateDataSource()
        {
            OnPropertyChanged(nameof(Cvc));
        }
        private async  void SetColumnsWidth()
        {
            double[] w = await ContentColumnsWidth();
            Cw0 = w[0];
            Cw1 = w[1];
            Cw2 = w[2];
            if (Cw3 > 0)
                Cw3 = w[3];
            
        }
        public double CvWidthRequest
        {
            get { return cvwidthrequest; }
            set
            {
                cvwidthrequest = value;
                OnPropertyChanged(nameof(CvWidthRequest));
            }
        }
        public double CvHeightRequest
        {
            get { return cvheightrequest; }
            set
            {
                cvheightrequest = value;
                OnPropertyChanged(nameof(CvHeightRequest));
            }
        }
        public double ContentWidth
        {
            get { return Math.Max(SafeWidth, w0 + w1 + w2 + w3); }
        }
        public Task<double[]> ContentColumnsWidth()
        {
            double fsize = Fsize;
            double _w0 = ScreenMetrics.MeasureTextWidth("55555", fsize) + 20;
            double _w1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + 10;
            double _w2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + 10;
            double _w3 = Cw3;

            for (int i = 0; i < cvc.Count; i++)
            {
                _w0 = Math.Max(_w0, ScreenMetrics.MeasureTextWidth(cvc[i].ItemNo, fsize) + 20);
                _w1 = Math.Max(_w1, ScreenMetrics.MeasureTextWidth(cvc[i].FirstName, fsize) + 20);
                _w2 = Math.Max(_w2, ScreenMetrics.MeasureTextWidth(cvc[i].LastName, fsize) + 20);
                if (_w3 > 0)
                    _w3 = Math.Max(_w3, ScreenMetrics.MeasureTextWidth(cvc[i].Occupation, fsize) + 20);

            }
            double cw = Math.Max(SafeWidth, _w0 + _w1 + _w2 + _w3);
            for (int i = 0; i < cvc.Count; i++)
            {
                cvc[i].Cw0 = _w0;
                cvc[i].Cw1 = _w1;
                cvc[i].Cw2 = _w2;
                cvc[i].Cw3 = _w3;
                cvc[i].W = cw;
            }
            double[] _w = [_w0, _w1, _w2, _w3];
            OnPropertyChanged(nameof(ContentWidth));
            return Task.FromResult(_w);
        }
        public int CvcCount
        {
            get { return cvc.Count; }
        }
        public double SafeWidth { get; set; }
        public double Fsize { get; set; } = 16;
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
