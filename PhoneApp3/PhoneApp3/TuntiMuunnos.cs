using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp3
{
    class TuntiMuunnos
    {


       public String MuunnaTunnit(String arvo){

           string[] words = arvo.Split('T');
           string[] tunnit_muunnos = words[1].Split(':');
           int tunnituusi = Int32.Parse(tunnit_muunnos[0]);
           int valmis = tunnituusi + 2;
           string takas = valmis.ToString();
           string lopullinen = takas + ":" + tunnit_muunnos[1];
         
           return lopullinen;
       }

       public String MuunnaAika(String aika) { 

           string[] muunnos = aika.Split('T');
           string uusiaika = muunnos[1];
           string[] muunnosuusiaika = uusiaika.Split(':');
           string valmisaika = muunnosuusiaika[0] + ":" + muunnosuusiaika[1];


           return "klo:" + valmisaika;
       }
    }
}
