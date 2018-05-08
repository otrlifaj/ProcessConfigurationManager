using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessConfigurationManager.WPF
{
    public class Utils
    {
        public Utils()
        {

        }

        public string FormatExceptionString(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null && ex.InnerException.Message != null)
            {
                message += "\n" + ex.InnerException.Message;
            }
            return message;
        }

        public void ShowExceptionMessageBox(Exception ex)
        {
            var message = FormatExceptionString(ex);
            MessageBox.Show(message);
        }
    }
}
