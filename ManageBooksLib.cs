using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace ManageBooksLib
{
	public class ManageBooksLib
	{
		public DataSet dataSet;

		public void setStockData()
		{
			//try for a datatable, the datatable must be set outside the functions
			System.Data.DataTable BooksStock = new System.Data.DataTable("BooksStock");
			//declare variables for datacolumn and datarow
			DataColumn column;
			DataRow row;
			//create new DataColumn set DataType
			//ColumnName and add to DataTable. //BS_ISBN(string), BS_Stock(int)
			column = new DataColumn();
			column.DataType = System.Type.GetType("System.String");
			column.ColumnName = "BS_ISBN";
			column.Unique = true;
			column.ReadOnly = true;
			//add tho the  datacolumnCollection
			BooksStock.Columns.Add(column);

			column = new DataColumn();
			column.DataType = System.Type.GetType("System.Int32");
			column.ColumnName = "BS_Stock";
			column.Unique = false;
			column.ReadOnly = false;
			//add tho the  datacolumnCollection
			BooksStock.Columns.Add(column);

			DataColumn[] PrimaryKeyColumns = new DataColumn[1];
			PrimaryKeyColumns[0] = BooksStock.Columns["BS_ISBN"];
			BooksStock.PrimaryKey = PrimaryKeyColumns;

			//insert Data
			dataSet = new DataSet();
			dataSet.Tables.Add(BooksStock);
			//create DataRow Obj Return of the King!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "0008376085";
			row["BS_Stock"] = 17;
			BooksStock.Rows.Add(row);

			//create DataRow Obj On the Shoulders of giants!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "9780762416981";
			row["BS_Stock"] = 12;
			BooksStock.Rows.Add(row);

			//create DataRow Obj H.P. and the Goblet of fire!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "9780747546245";
			row["BS_Stock"] = 29;
			BooksStock.Rows.Add(row);

			//create DataRow Obj 1984!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "9780452284234";
			row["BS_Stock"] = 8;
			BooksStock.Rows.Add(row);

			//create DataRow Obj Dune!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "0441013597";
			row["BS_Stock"] = 3;
			BooksStock.Rows.Add(row);

			//create DataRow Obj Brave new World!
			row = BooksStock.NewRow();
			row["BS_ISBN"] = "3425048570";
			row["BS_Stock"] = 15;
			BooksStock.Rows.Add(row);
		}

		public struct Book
		{
			public string ISBN { get; set; }
			public int Quantity { get; set; }
			public double Price { get; set; }
			public int Discount { get; set; }

			public Book(string isbn, int quantity, double price, int discount) : this()
			{
				this.ISBN = isbn;
				this.Quantity = quantity;
				this.Price = price;
				this.Discount = discount;
			}
		}

		public struct ShippingInfo
		{
			public string Address { get; set; }
			public string ZipCode { get; set; }
			public string Region { get; set; }
			public string Country { get; set; }

			public ShippingInfo(string Address, string ZipCode, string Region, string Country) : this()
			{
				this.Address = Address;
				this.ZipCode = ZipCode;
				this.Region = Region;
				this.Country = Country;
			}
		}

		public List<Book> setCartData()//this function creates a Cart List which is used to create a Book[] Cart
		{
			List<Book> Cart = new List<Book>();
			Cart.Add(new Book("0008376085", 1, 25.00, 10)); //Return of the king
			Cart.Add(new Book("0441013597", 2, 17.00, 5)); //Dune
			Cart.Add(new Book("9780452284234", 3, 12.00, 0)); //1984 //9780452284234

			return Cart;
		}

		public bool ValidISBN(string ISBN)
		{
			string isbn = ISBN;
			bool result = false;
			//long j;
			//remove any dashes, that might exist
			if (isbn.Contains('-')) isbn = isbn.Replace("-", "");
			//check if string is empty
			if (!string.IsNullOrEmpty(isbn))
			{
				long j;
				if (isbn.Length == 10)
				{
					// Check if it contains any non numeric chars, if yes, return false
					if (!Int64.TryParse(isbn.Substring(0, isbn.Length - 1), out j))
						result = false;
					// Checking if the last char is not 'X' and
					// and if it's a numeric value
					char lastChar = isbn[isbn.Length - 1];
					if (lastChar == 'X' && !Int64.TryParse(lastChar.ToString(), out j))
						result = false;
					int sum = 0;
					// Using the alternative way of calculation
					for (int i = 0; i < 9; i++)
						sum += Int32.Parse(isbn[i].ToString()) * (i + 1);
					// Getting the remainder or the checkdigit
					int remainder = sum % 11;
					// Check if the checkdigit is same as the last number of ISBN 10 code
					result = (remainder == int.Parse(isbn[9].ToString()));
				}
				else if (isbn.Length == 13)
				{
					int sum = 0;
					//multiply by 1 or 3, alternating, from left to right and summing them
					for (int i = 0; i < 12; i++)
					{
						sum += Int32.Parse(isbn[i].ToString()) * (i % 2 == 1 ? 3 : 1);
					}
					//Check if the check digit(last digit) is the same with our resulting check digit
					int remainder = sum % 10;
					int checkDigit = 10 - remainder;
					if (checkDigit == 10) checkDigit = 0;
					result = (checkDigit == int.Parse(isbn[12].ToString()));
				}
				else
					result = false;
			}
			return result;
		}/*the function fails to perform if the imput is 0123456789 or 0123456789012*/

		public void CheckCardNum(string CardID, ref bool ValidCard, ref string CardType)
		{

			//card id normalization
			if (CardID.Contains('-')) CardID = CardID.Replace("-", "");
			if (CardID.Contains(' ')) CardID = CardID.Replace(" ", "");
			if (!string.IsNullOrEmpty(CardID))
			{
				int i, checkSum = 0;
				//compute checksum for even digits
				for (i = CardID.Length - 1; i >= 0; i -= 2)
					checkSum += (Int32.Parse(CardID[i].ToString()));
				//compute checksum of odd digits multiplied by 2
				for (i = CardID.Length - 2; i >= 0; i -= 2)
				{
					int val = ((Int32.Parse(CardID[i].ToString())) * 2);
					while (val > 0)
					{
						checkSum += val % 10;
						val /= 10;
					}
				}
				if ((checkSum % 10) == 0) ValidCard = true;
				else ValidCard = false;

				//this set of string[] are the data set for the card Type check
				string[] cAmExp = new string[] { "34", "37" };
				string[] cMaest = new string[] { "5018", "5020", "5038", "5893", "6304", "6759", "6761", "6762", "6763" };
				string[] cMastC = new string[] { "51", "52", "53", "54", "55", "22","23", "24", "25", "26", "27" };
				string[] cVisa = new string[] { "4" };


				//check card type
				if (ValidCard)
				{
					foreach (string s in cAmExp)
						if (CardID.StartsWith(s)) CardType = "American Express";
					foreach (string s in cMaest)
						if (CardID.StartsWith(s)) CardType = "Maestro";
					foreach (string s in cMastC)
						if (CardID.StartsWith(s)) CardType = "Master Card";
					foreach (string s in cVisa)
						if (CardID.StartsWith(s)) CardType = "Visa";
				}
				else
                {
					CardType = "";
                }

				if (Equals("", CardType)) { ValidCard = false; }// if the card number is a correct calculation but the card type is not accepted, then the cardnumber is not accepted
			}
		}

		public void CheckZipCode(ShippingInfo SI, ref bool ValidZC, ref bool ValidRg)
		{
			//normalize zip code
			if (SI.ZipCode.Contains('-')) SI.ZipCode = SI.ZipCode.Replace("-", "");
			if (SI.ZipCode.Contains(' ')) SI.ZipCode = SI.ZipCode.Replace(" ", "");

			//check if Zip Code is valid
			long j;
			if (!Int64.TryParse(SI.ZipCode.Substring(0, SI.ZipCode.Length - 1), out j))
				ValidZC = false;
			else if (SI.ZipCode.Length != 5)
				ValidZC = false;
			else
				ValidZC = true;

			//create a hash table for the Zip code and region matches.
			Hashtable ht = new Hashtable();
			string[] post = new string[] { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "80" };
			ht.Add("ATTIKHS", post);

			post = new string[] { "20" };
			ht.Add("KORINTHIAS", post);

			post = new string[] { "21" };
			ht.Add("ARGOLIDAS", post);

			post = new string[] { "22" };
			ht.Add("ARKADIAS", post);

			post = new string[] { "23" };
			ht.Add("LAKONIAS", post);

			post = new string[] { "24" };
			ht.Add("MESSINIAS", post);

			post = new string[] { "25", "26" };
			ht.Add("ACHAIAS", post);

			post = new string[] { "27" };
			ht.Add("HLEIAS", post);

			post = new string[] { "28" };
			ht.Add("KEFALLINIAS", post);

			post = new string[] { "29" };
			ht.Add("ZAKINTHOU", post);

			post = new string[] { "30" };
			ht.Add("AITOLOAKARNANIAS", post);

			post = new string[] { "31" };
			ht.Add("LEUKADAS", post);

			post = new string[] { "32" };
			ht.Add("BOIWTIAS", post);

			post = new string[] { "33" };
			ht.Add("FWKIDAS", post);

			post = new string[] { "34" };
			ht.Add("EUBOIAS", post);

			post = new string[] { "35" };
			ht.Add("FTHIOTIDAS", post);

			post = new string[] { "36" };
			ht.Add("EURUTANIAS", post);

			post = new string[] { "37", "38" };
			ht.Add("MAGNISIAS", post);

			post = new string[] { "40", "41" };
			ht.Add("LARISAS", post);

			post = new string[] { "42" };
			ht.Add("TRIKALWN", post);

			post = new string[] { "43" };
			ht.Add("KARDITSAS", post);

			post = new string[] { "44", "45" };
			ht.Add("IWANNINWN", post);

			post = new string[] { "46" };
			ht.Add("THESPRWTIAS", post);

			post = new string[] { "47" };
			ht.Add("ARTAS", post);

			post = new string[] { "48" };
			ht.Add("PREBEZAS", post);

			post = new string[] { "49" };
			ht.Add("KERKURAS", post);

			post = new string[] { "50" };
			ht.Add("KOZANHS", post);

			post = new string[] { "51" };
			ht.Add("GREBENWN", post);

			post = new string[] { "52" };
			ht.Add("KASTORIAS", post);

			post = new string[] { "53" };
			ht.Add("FLWRINAS", post);

			post = new string[] { "54", "55", "56", "57" };
			ht.Add("THESSALONIKHS", post);

			post = new string[] { "58" };
			ht.Add("PELLAS", post);

			post = new string[] { "59" };
			ht.Add("HMATHIAS", post);

			post = new string[] { "60" };
			ht.Add("PIERIAS", post);

			post = new string[] { "61" };
			ht.Add("KILKIS", post);

			post = new string[] { "62" };
			ht.Add("SERRWN", post);

			post = new string[] { "63" };
			ht.Add("XALKIDIKHS", post);

			post = new string[] { "64", "65" };
			ht.Add("KABALAS", post);

			post = new string[] { "66" };
			ht.Add("DRAMAS", post);

			post = new string[] { "67" };
			ht.Add("JANTHHS", post);

			post = new string[] { "68" };
			ht.Add("EBROU", post);

			post = new string[] { "69" };
			ht.Add("RODOPHS", post);

			post = new string[] { "70", "71" };
			ht.Add("HRAKLEIOU", post);

			post = new string[] { "72" };
			ht.Add("LASITHIOU", post);

			post = new string[] { "73" };
			ht.Add("XANIWN", post);

			post = new string[] { "74" };
			ht.Add("RETHUMNHS", post);

			post = new string[] { "81" };
			ht.Add("LESBOU", post);

			post = new string[] { "82" };
			ht.Add("XIOU", post);

			post = new string[] { "83" };
			ht.Add("SAMOU", post);

			post = new string[] { "84" };
			ht.Add("KUKLADWN", post);

			post = new string[] { "85" };
			ht.Add("DWDEKANHSOU", post);

			//check if the Region 
			ICollection keys = ht.Keys;
			if (ValidZC)
			{
				foreach (string k in keys)
				{
					string[] postcode = (string[])ht[k];
					if (SI.Region == k)
					{
						foreach (string s in postcode)
						{
							if (SI.ZipCode.StartsWith(s))
							{
								ValidRg = true;
								break;
							}
						}
						break;
					}
					else
						ValidRg = false;
				}
			}
		}//works with greeklish names of regions, Attikhs/Lesbou/Hrakleiou

		public bool IsAvailable(DataTable BooksStock, string ISBN, int Quantity, ref int Stock)
		{
			bool flag = false;
			Stock = 0;
			foreach (DataRow r in BooksStock.Rows)
			{
				if ((string)r[0] == ISBN)
				{
					if (Quantity >= 0)
					{
						if ((int)r[1] >= Quantity)
						{
							flag = true;
						}
						Stock = (int)r[1];
						break;
					}
				}
			}
			return flag;
		}

		public double BooksCost(Book[] Cart)
		{
			double totalPrice = 0;
			int copies = 0;
			int disc = 0;
			double itemP = 0;
			foreach (Book k in Cart)
			{
				copies = k.Quantity;
				disc = k.Discount;
				itemP = k.Price;
				if (copies >= 0 && disc >= 0 && itemP >= 0)
				{
					totalPrice += (copies * itemP) - ((copies * itemP) * disc) / 100;
				}
			}
			return (double)decimal.Round((decimal)totalPrice, 2);
		}//To be tested this requires a Book[] Cart array

		public double ShippingCost(ShippingInfo SI)
		{
			//declare the variables
			double costcalc = -1;
			string[] hpeirEl = new string[] { "KORINTHIAS", "ARGOLIDAS", "ARKADIAS", "LAKONIAS", "MESSINIAS", "ACHAIAS", "HLEIAS", "AITOLOAKARNANIAS", "BOIWTIAS", "FWKIDAS", "FTHIWTIDAS", "EURUTANIAS", "MAGNISIAS", "LARISAS", "TRIKALWN", "KARDITSAS", "IWANNINWN", "THESPRWTIAS", "ARTAS", "PREBEZAS", "KOZANHS", "GREBENWN", "KASTORIAS", "FLWRINAS", "THESSALONIKHS", "PELLAS", "HMATHIAS", "PIERIAS", "KILKIS", "SERRWN", "XALKIDIKHS", "KABALAS", "DRAMAS", "JANTHHS", "EBROU", "RODOPHS" };
			string[] nhsioEL = new string[] { "KEFALLINIAS", "ZAKUNTHOU", "LEUKADAS", "EUBOIAS", "KERKYRAS", "HRAKLEIOU", "LASITHIOU", "XANIWN", "RETHUMNHS", "LESBOU", "XIOU", "SAMOU", "KUKLADWN", "DWDEKANHSOU" };

			//check if the country and region exist.
			if (SI.Country.Equals("GREECE"))
			{
				foreach (string s in hpeirEl)
					if (hpeirEl.Contains(SI.Region))
					{
						costcalc = 3.5;
					}
				foreach (string s in nhsioEL)
					if (nhsioEL.Contains(SI.Region))
					{
						costcalc = 5;
					}
				if (SI.Region.Equals("ATTIKHS"))
					costcalc = 0;
			}
			else
			{
				costcalc = 8.5;
			}
			return costcalc;
		}//The function needs a ShippingInfo objecto to be tested

		public bool ProceedToBuy(DataTable BooksStock, Book[] Cart)
		{
			bool flag = true;
			int Stock = 0;

			foreach (Book k in Cart)
			{
				//Console.WriteLine(k.ISBN);
				//if !IsAvailable is true then the book does not exist in the BooksStock so it turns the flag to false
				if (!(IsAvailable(BooksStock, k.ISBN, k.Quantity, ref Stock)))
				{
					flag = false;
				}
			}
			return flag;
		}

		public void UpdateCartBasedOnAvailability(DataTable BooksStock, ref Book[] Cart)
		{
			List<int> stock_array = new List<int>();
			List<string> isbn_array = new List<string>();
			int i = 0;
			int j = 0;
			int len = Cart.Length;

			int Stock = 0;
			//bool flag = true;
			foreach (Book k in Cart)
			{
				if (!(IsAvailable(BooksStock, k.ISBN, k.Quantity, ref Stock)))
				{
					//k.Quantity = Stock;
					stock_array.Add(Stock);
					isbn_array.Add(k.ISBN);
				}
				i++;
			}
			for (i = 0; i < len; i++)
			{
				foreach (string isbn in isbn_array)
				{
					if (isbn.Equals(Cart[i].ISBN))
					{
						Cart[i].Quantity = stock_array[j];
					}
					j++;
				}
				j = 0;
			}
		}
	
	}
}
