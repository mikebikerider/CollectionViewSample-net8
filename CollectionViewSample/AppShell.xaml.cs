
namespace CollectionViewSample
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        public NoScrollPage noscrollPage
        {
            get { return noscroll_page; }
        }
        public HScrollPage hscrollPage
        {
            get { return hscroll_page; }
        }
    }
}
