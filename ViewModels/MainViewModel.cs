using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDRAC_IPMI.ViewModels
{
    public class MainViewModel
    {
        public GeneralViewModel GeneralViewModel { get; }
        public PowerViewModel PowerViewModel { get; }

        public MainViewModel()
        {
            GeneralViewModel = new GeneralViewModel();
            PowerViewModel = new PowerViewModel();
        }
    }
}
