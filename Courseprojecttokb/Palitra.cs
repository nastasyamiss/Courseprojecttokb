using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courseprojecttokb
{
    class Palitra
    {
        public string Number { get; set; }
        public string HEXFormat { get; set; }
        public char Symbol { get; set; }
        public Palitra(List<Numbers> Nitki, string hex)
        {
            Numbers temp = Nitki.Find(x => x.HEXFormat.Equals(hex));
            this.Number = temp.Number;
        }
    }
}
