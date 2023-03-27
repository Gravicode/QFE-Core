using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using HtmlToXamlConvert.Helper;

namespace HtmlToXamlConvert
{
    public class HtmlToXamlPresentationModel : INotifyPropertyChanged
    {
        private string theHTMLToBind;
        public string TheHTMLToBind
        {
            get
            {
                return theHTMLToBind;
            }

            set
            {
                if (theHTMLToBind != value)
                {
                    theHTMLToBind = value;
                    OnPropertyChanged("TheHTMLToBind");
                }
            }
        }

        private string parsedHTML;
        public string ParsedHTML
        {
            get
            {
                return parsedHTML;
            }

            set
            {
                if (parsedHTML != value)
                {
                    parsedHTML = value;
                    OnPropertyChanged("ParsedHTML");
                }
            }
        }

        public DelegateCommand<string> BindTheHTML { get; set; }

        public HtmlToXamlPresentationModel()
        {
            BindTheHTML = new DelegateCommand<string>(OnBindHTML);

            TheHTMLToBind = "<label><strong>Hello World</strong></label>";
        }

        private void OnBindHTML(string html)
        {
            ParsedHTML = (string)html;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }



        #endregion
    }
}
