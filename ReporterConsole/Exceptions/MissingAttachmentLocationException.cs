using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.Exceptions
{
    class MissingAttachmentLocationException : Exception
    {
        public MissingAttachmentLocationException(string message) : base(message)
        {
        }
    }
}
