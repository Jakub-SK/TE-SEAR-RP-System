using System;
using System.Collections.Generic;
using System.Text;

namespace SEAR_DataContract
{
    public class Certificate
    {
        public Certificate()
        {
            certPassword = "SEAR_RP";
        }
        public string certPassword {  get; }
    }
}
