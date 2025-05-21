using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models
{
    public class IsDurum
    {
        public string isBaslik { get; set; }
        public string isAciklama { get; set; }
        public DateTime? iletilenTarih { get; set; }
        public DateTime? yapılanTarih { get; set; }
        public string durumAd { get; set; }
        public string isYorum { get; set; }

    }

    public class IsDurumModel
    {
        public List<IsDurum> isDurumlarr { get; set; }
    }
}
