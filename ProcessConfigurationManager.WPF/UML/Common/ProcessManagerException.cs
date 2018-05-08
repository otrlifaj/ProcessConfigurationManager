using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProcessConfigurationManager.WPF.UML
{
    public class ProcessManagerException : Exception
    {
        public ProcessManagerException() : base()
        {

        }

        public ProcessManagerException(string message) : base(message)
        {
        }

        public ProcessManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProcessManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
