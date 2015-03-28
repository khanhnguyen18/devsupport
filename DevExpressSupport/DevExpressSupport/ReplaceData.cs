using System;
using System.Collections.Generic;
using System.Text;

namespace DevExpressSupport
{
    public class ReplaceData
    {
        private string oldTring;

        public string OldString
        {
            get { return oldTring; }
            set { oldTring = value; }
        }

        private string newTring;

        public string NewString
        {
            get { return newTring; }
            set { newTring = value; }
        }

    }
}
