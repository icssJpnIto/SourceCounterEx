using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SourceCounterEx.ViewModels
{
    public class TickerViewModel:ViewModel
    {
        public TickerViewModel()
        {
            Timer timer = new Timer();
            timer.Interval = 60000; 
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public String Now
        {
            get { return DateTime.Now.ToString("yyyy/MM/dd HH:mm"); }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.OnPropertyChanged(nameof(this.Now));
        }


    }
}
