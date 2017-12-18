using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace gxy.controls
{
    /// <summary>
    /// Interaction logic for gxydatepicker.xaml
    /// </summary>
    public partial class gxydatepicker : DatePicker,INotifyPropertyChanged
    {

        protected DatePickerTextBox _datePickerTextbox;
        protected MaskedTextBox _masktextbox;
        protected readonly string _shortDatePattern;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private string _date;
        public string date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged();

            }
        }

        public gxydatepicker() : base()
        {
            InitializeComponent();
            this.DataContext = this;
            _shortDatePattern = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _datePickerTextbox = new DatePickerTextBox();
            _masktextbox = new MaskedTextBox();

            _datePickerTextbox = this.Template.FindName("PART_TextBox", this) as DatePickerTextBox;
            _datePickerTextbox.Visibility = System.Windows.Visibility.Hidden;


            Grid _grid = new Grid();
            _grid = (Grid)_datePickerTextbox.Parent;
            _grid.Children.Insert(1, _masktextbox);


            #region MaskTextBox Properties Configuration

            _masktextbox.Mask = "00/00/0000";
            _masktextbox.AutoSelectBehavior = AutoSelectBehavior.OnFocus;
            _masktextbox.AutoMoveFocus = true;
            _masktextbox.InsertKeyMode = InsertKeyMode.Overwrite;
            

            
            #endregion

            #region  CodeBehind TwoWays DataBinding Configuration

            Binding datebinding = new Binding("date");
            datebinding.Source = this;
            datebinding.Mode = BindingMode.TwoWay;
            datebinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            #endregion

            #region  Set DataBinding to DatePicker And MaskTextBox 

            _datePickerTextbox.SetBinding(DatePickerTextBox.TextProperty, datebinding);
            _masktextbox.SetBinding(MaskedTextBox.TextProperty, datebinding);

            #endregion

            #region  Subscribe Events for MaskTextBox

            if (_masktextbox != null)
            {
                _masktextbox.TextChanged += _datePickerTextbox_TextChanged;
                _masktextbox.PreviewKeyDown += _masktextbox_PreviewKeyDown;


            }
            #endregion

        }

        private void _masktextbox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

           if(date is null)
            {
                return;
            }

            // For MultiSelect Case
            if (((MaskedTextBox)sender).SelectionLength > 0 )
                {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    int _selectionStart = ((MaskedTextBox)sender).SelectionStart;
                    int _selectionlength = ((MaskedTextBox)sender).SelectionLength;


                    for (int i = 0; i < _selectionlength; i++)
                    {

                        if (date.ToCharArray()[_selectionStart].ToString() != "/")
                        {
                            date = date.Insert(_selectionStart, " ");
                            date = date.Remove(_selectionStart + 1, 1);
                        }
                        _selectionStart += 1;
                    }

                    e.Handled = true;
                    return;
                }


                //if (e.Key == Key.Delete || e.Key == Key.Back)
                //{
                //    e.Handled = true;
                //}
                //    return;

                }

            #region Logic For BackSpace Key And Delete Key
            if (e.Key == Key.Back)
            {
                if (((MaskedTextBox)sender).SelectionStart == 0)
                    return;


                if (date.ToCharArray()[((MaskedTextBox)sender).SelectionStart - 1].ToString() == " ")
                {
                    return;
                }
                else if (date.ToCharArray()[((MaskedTextBox)sender).SelectionStart - 1].ToString() != "/")
                {
                    

                    if(((MaskedTextBox)sender).SelectionLength==0)
                    {
                        date = date.Insert(((MaskedTextBox)sender).SelectionStart - 1, " ");
                        date = date.Remove(((MaskedTextBox)sender).SelectionStart, 1);
                    }
                }
                else
                {

                    if (((MaskedTextBox)sender).SelectionLength == 0)
                    {
                        date = date.Insert(((MaskedTextBox)sender).SelectionStart - 2, " ");
                        date = date.Remove(((MaskedTextBox)sender).SelectionStart - 1, 1);
                    }
                    else
                    {
                        int _selectionStart = ((MaskedTextBox)sender).SelectionStart;
                        int _selectionlength = ((MaskedTextBox)sender).SelectionLength;

                        for (int i = 0; i < _selectionlength; i++)
                        {
                            
                            date = date.Insert(_selectionStart, " ");
                            date = date.Remove(_selectionStart + 1, 1);
                            _selectionStart += 1;
                        }

                    }




                }
                e.Handled = true;
            }

            if (e.Key == Key.Delete)
            {
                if (date.ToCharArray()[((MaskedTextBox)sender).SelectionStart].ToString() != "/")
                {
                    date = date.Insert(((MaskedTextBox)sender).SelectionStart, " ");
                    date = date.Remove(((MaskedTextBox)sender).SelectionStart + 1, 1);
                }
                
                e.Handled = true;
            }
            #endregion
        }

        private void _datePickerTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime dt;
            if (DateTime.TryParseExact(_masktextbox.Text, _shortDatePattern, Thread.CurrentThread.CurrentCulture,
                System.Globalization.DateTimeStyles.None, out dt))
            {
                this.SelectedDate = dt;
            }
        }
    }



}
