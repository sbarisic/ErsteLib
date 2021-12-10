using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErsteLib;
using System.IO;

namespace ErsteTest {
	class Program {
		static void Main(string[] args) {
			Erste Lib = new Erste();
			Lib.LoadData("data/HR1124020063207999932.json");
			Lib.LoadData("data/HR9524020061031262160.json");

			Lib.Calculate();
		}
	}
}
