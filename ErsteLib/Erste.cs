using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ErsteLib {
	public class Erste {
		List<ErsteTransakcija> Transakcije = new List<ErsteTransakcija>();



		public void LoadData(string FileName) {
			string JsonSrc = File.ReadAllText(FileName);
			string Racun = Path.GetFileNameWithoutExtension(FileName);

			ErsteTransakcija[] LoadedTrans = JsonConvert.DeserializeObject<ErsteTransakcija[]>(JsonSrc);
			foreach (var T in LoadedTrans)
				T.OriginRacun = Racun;

			Transakcije.AddRange(LoadedTrans);
		}

		public void Calculate() {
			DateTime From = new DateTime(2021, 10, 15);
			DateTime To = new DateTime(2021, 11, 15);

			ErsteTransakcija[] Trans = Transakcije.Where(T => T.booking >= From && T.booking < To).OrderBy(T => T.booking).ToArray();
			float Suma = 0;

			List<ErsteKategorijaTransakcija> Kategorije = new List<ErsteKategorijaTransakcija>();

			for (int i = 0; i < Trans.Length; i++) {
				Suma += Trans[i].amount.valueFlt;

				string kat = GetCategoryName(Trans[i]);
				ErsteKategorijaTransakcija Kat = Kategorije.Where(K => K.Name == kat).FirstOrDefault();

				if (Kat == null) {
					Kat = new ErsteKategorijaTransakcija(kat);
					Kategorije.Add(Kat);
				}

				Kat.Add(Trans[i]);
			}

			foreach (var K in Kategorije) {
				Console.WriteLine(K.ToString());
			}
		}

		string GetCategoryName(ErsteTransakcija Trans) {
			if (Trans.partnerName == null && (Trans.bookingTypeTranslation == "Isplata na bankomatu" || Trans.bookingTypeTranslation == "Uplata na bankomatu"))
				return "Bankomat";

			string PartnerName = Trans.partnerName.ToUpper();

			if (PartnerName.Contains("PBZTINA") || PartnerName.Contains("TIFON") || PartnerName.Contains("CRODUX") || PartnerName.Contains("SOLA"))
				return "Benzinska";

			if (PartnerName.Contains("MCDRIVE") || PartnerName.Contains("BURGER") || PartnerName.Contains("MCDONALDS") || PartnerName.Contains("MESNICA")
				|| PartnerName.Contains("GLOVO") || PartnerName.Contains("PIZZERIA") || PartnerName.Contains("KFC"))
				return "Hrana";

			if (PartnerName.Contains("PPK") || PartnerName.Contains("KONZUM") || PartnerName.Contains("KAUFLAND") || PartnerName.Contains("SPAR") || PartnerName.Contains("MUELLER")
				|| PartnerName.Contains("LIDL") || PartnerName.Contains("PLODINE") || PartnerName.Contains("PEVEX") || PartnerName.Contains("DROGERIE MARKT"))
				return "Trgovine";

			if (PartnerName.Contains("KOMUNALAC") || PartnerName.Contains("KOMUNALIJE") || PartnerName.Contains("HEP ELEKTRA") || PartnerName.Contains("VODNE USLUGE") || PartnerName.Contains("CROATIA OSIGURANJE")
				|| PartnerName.Contains("PLINARA") || PartnerName.Contains("HT D.D.") || PartnerName.Contains("NAKNADA ZA UREĐENJE VODA"))
				return "Rezije";

			/*if (Trans.reference.ToUpper().Contains("KEKS"))
				return "Keks";*/

			if (Trans.OriginRacun == "HR9524020061031262160" || Trans.reference.Contains("KEKS Pay"))
				return "Keks";

			if (Trans.partnerName != null)
				return Trans.partnerName;

			return "Ostalo";
		}
	}

	public class ErsteKategorijaTransakcija {
		public string Name;
		public List<ErsteTransakcija> Transakcije = new List<ErsteTransakcija>();
		public float Suma = 0;

		public ErsteKategorijaTransakcija(string Name) {
			this.Name = Name;
		}

		public void Add(ErsteTransakcija Trans) {
			Transakcije.Add(Trans);
			Suma += Trans.amount.valueFlt;
		}

		public override string ToString() {
			return string.Format("{0}\t{1:0.00} HRK", Name, Suma);
		}
	}

	public class ErsteTransakcija {
		public string OriginRacun;

		public DateTime booking {
			get; set;
		}

		public object valuation {
			get; set;
		}

		public string partnerName {
			get; set;
		}

		public ErsteAccount partnerAccount {
			get; set;
		}

		public ErsteAmount amount {
			get; set;
		}

		public string reference {
			get; set;
		}

		public string referenceNumber {
			get; set;
		}

		public string[] categories {
			get; set;
		}

		public bool favorite {
			get; set;
		}

		public string constantSymbol {
			get; set;
		}

		public string variableSymbol {
			get; set;
		}

		public string specificSymbol {
			get; set;
		}

		public string bookingTypeTranslation {
			get; set;
		}

		public override string ToString() {
			return string.Format("{0} - {1} ({2})", partnerName, bookingTypeTranslation, amount);
		}
	}

	public class ErsteAccount {
		public string iban {
			get; set;
		}

		public string bic {
			get; set;
		}

		public string number {
			get; set;
		}

		public string bankCode {
			get; set;
		}

		public string countryCode {
			get; set;
		}

		public string prefix {
			get; set;
		}

		public string secondaryId {
			get; set;
		}

		public override string ToString() {
			return iban;
		}
	}

	public class ErsteAmount {
		public int value {
			get; set;
		}

		public int precision {
			get; set;
		}

		public string currency {
			get; set;
		}

		public float valueFlt {
			get {
				return (float)(value / Math.Pow(10, precision));
			}
		}

		public override string ToString() {
			return string.Format("{0:0.00} {1}", valueFlt, currency);
		}
	}
}
