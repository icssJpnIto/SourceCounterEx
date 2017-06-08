using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SourceCounterEx.Command
{
    public interface IRelayCommand:ICommand
    {
        event EventHandler Executed;
        event EventHandler Executing;
    }
}
