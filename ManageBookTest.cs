using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ManageBooksLib;
using System.Data;
using System.Collections.Generic;

namespace ManageBooksTest
{
    [TestClass]
    public class ManageBooksTest
    {
        DataSet dataSet;

        [TestMethod]
        public void TestValidISBN()
        {
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            object[,] values =
            {
                {1, "0441013597", true},
                {2, "9985478956258", false},
                {3, "9780452284234", true},
                {4, "9987456987", false}, //out of pure luck this number is considered a valid ISBN
                {5, "12354", false}
            };

            int i = 0;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {

                    bool result = bl.ValidISBN((string)values[i, 1]);
                    Assert.AreEqual(values[i, 2], result);

                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }
        }

        [TestMethod]
        public void TestCheckCardNum()
        { //string CardID, ref bool ValidCard, ref string CardType
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            object[,] values =
            {
                {1, "4539539506301810", true, "Visa"},
                {2, "2720993710917882", true, "Master Card"},
                {3, "5018847132438428", true, "Maestro"},
                {4, "348369577482888", true, "American Express"}, 
                {5, "272099371091788", false, ""}
            };

            int i = 0;
            string CardType = "";
            bool ValidCard = false;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {

                    bl.CheckCardNum((string)values[i, 1], ref ValidCard, ref CardType);
                    Assert.AreEqual(values[i, 2], ValidCard);
                    Assert.AreEqual((string)values[i,3], CardType);

                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }
        }

        [TestMethod]
        public void TestCheckZipCode()
        {//ShippingInfo SI, ref bool ValidZC, ref bool ValidRg
            //SI(string Address, string ZipCode, string Region, string Country) all upper case letters, country in english
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            
            object[,] values =
            {
                {1, "PAPANASTASH 11", "17124", "ATTIKHS", "GREECE", true, true},
                {2, "GEORGIOU PSALTI 7", "84101", "KUKLADWN", "GREECE", true, true},
                {3, "DARDANELIWN 80", "20-723", "ZAKINTHOU", "GREECE", true, false},
                {4,  "MALAGANIAS 87", "159 753", "", "UNITED KINGDOM", false, false},
                {5,  "TZABELA 60", "112255", "attikhs", "greece",  false, false}
            };

            int i = 0;
            bool ValidZC = false;
            bool ValidRg = false;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {
                    ManageBooksLib.ManageBooksLib.ShippingInfo SI = new ManageBooksLib.ManageBooksLib.ShippingInfo();
                    SI.Address = (string)values[i,1];
                    SI.ZipCode = (string)values[i,2];
                    SI.Region = (string)values[i,3];
                    SI.Country = (string)values[i,4];
                    bl.CheckZipCode(SI, ref ValidZC, ref ValidRg);
                    
                    Assert.AreEqual(values[i, 5], ValidZC);
                    Assert.AreEqual(values[i, 6],ValidRg);
                    ValidRg = ValidZC = false;

                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }
        }

        [TestMethod]
        public void TestIsAvailable()
        {//datatable BooksStock, string ISBN, int Quantity, ref int Stock
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            bl.setStockData();//function is in the ManageBooksLib.cs file and creates the BooksStock Datatable with some predetermined data
            DataTable BooksStock = bl.dataSet.Tables["BooksStock"];

            object[,] values =
            {
                {1, "0441013597", 3, true}, //dune
                {2, "9780452284234", 11, false}, //1984
                {3, "1212121212112", 19, false},
                {4, "3246739634", 1, false},
                {5, "0008376085", -1, false}//Lotr RotK
            };

            int i = 0;
            int Stock = -1;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {

                    bool result = bl.IsAvailable(BooksStock, (string)values[i,1], (int)values[i,2], ref Stock);
                    Assert.AreEqual(values[i, 3],result);
                    Stock = -1;

                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }
        }

        [TestMethod]
        public void TestBooksCost() 
        {//Book[] Cart
            //in this unit test we must check that the expected price of a cart is the same as the returned value
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();

            object[,] values =
            {//{case, (quantity, price, discount)*3, total_price}
                {1, 1, 25.00, 10, 2, 17.00, 5, 3, 12.00, 0, 90.8},
                {2, 3, 7.5, 5, 8, 12.8, 12, 2, 8.40, 10, 126.61},
                {3, 10, 10, 10, 1, 9, 50, 1, 10, 10, 103.5},
                {4, 1, 45, 0, 2, -7, 0, -1, 12, 7, 45},
                {5, 1, 45, -5, 2, -7, 0, -1, 12, 7, 67.91}//the negatives will not be accepted, and the result will be 0
            };

            int i = 0;
            try
            {
                for (i = 0; i < values.GetLength(0); i++) {
                    ManageBooksLib.ManageBooksLib.Book[] cart = new ManageBooksLib.ManageBooksLib.Book[3];
                    int j = 0;
                    int y = 1;
                    foreach (ManageBooksLib.ManageBooksLib.Book k in cart)
                    {

                        cart[j].ISBN = "";
                        cart[j].Quantity = (int)values[i, y];
                        y++;
                        cart[j].Price = Convert.ToDouble(values[i, y]);
                        y++;
                        cart[j].Discount = (int)values[i, y];
                        y++;
                        j++;

                    }
                    
                    double result = bl.BooksCost(cart);
                    Assert.AreEqual(Convert.ToDouble(values[i, 10]), result);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }
        }

        [TestMethod]
        public void TestShippingCost()
        { //ShippingInfo SI
            
            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();

            object[,] values =
            {
                {1, "PAPANASTASH 11", "17124", "ATTIKHS", "GREECE", 0},
                {2, "GEORGIOU PSALTI 7", "84101", "KUKLADWN", "GREECE", 5},
                {3, "DARDANELIWN 80", "20723", "KORINTHIAS", "GREECE", 3.5},
                {4, "BAKER'S STREET 221B", "159753", "STATFORD", "UNITED KINGDOM", 8.5},
                {5, "TZABELA 60", "11225", "attikhs", "greece",  0}//gives back 8.5 because it considers "greece" different from "GREECE" and gives the cost as a non greek region
            };

            int i = 0;

            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {
                    ManageBooksLib.ManageBooksLib.ShippingInfo SI = new ManageBooksLib.ManageBooksLib.ShippingInfo();
                    SI.Address = (string)values[i, 1];
                    SI.ZipCode = (string)values[i, 2];
                    SI.Region = (string)values[i, 3];
                    SI.Country = (string)values[i, 4];

                    double result = bl.ShippingCost(SI);
                    Assert.AreEqual(Convert.ToDouble(values[i, 5]), result);

                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }

        }

        [TestMethod]
        public void TestProceedToBuy()
        {//DataTable BooksStock, Book[] Cart

            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            bl.setStockData();//function is in the ManageBooksLib.cs file and creates the BooksStock Datatable with some predetermined data
            DataTable BooksStock = bl.dataSet.Tables["BooksStock"];

            object[,] values =
            {//{case, bookISBN*3, bool are all available?}
                {1, "0008376085", "9780762416981", "0441013597", true},
                {2, "3425048570", "9780452284234", "1111111111", false},
                {3, "2225548996", "9998547521556", "9780747546245", false},
                {4, "7974659852258", "4679138525753", "9875432160", false},
                {5, "iekanfhapqo23", "geaofiwb34bik", "09unf3we 0", false}//the negatives will not be accepted, and the result will be 0
            };

            int i = 0;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {
                    ManageBooksLib.ManageBooksLib.Book[] cart = new ManageBooksLib.ManageBooksLib.Book[3];
                    int j = 0;
                    foreach (ManageBooksLib.ManageBooksLib.Book k in cart)
                    {

                        cart[j].ISBN = (string)values[i,j+1];
                        j++;

                    }

                    bool result = bl.ProceedToBuy(BooksStock, cart);
                    Assert.AreEqual(values[i, 4], result);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}.", values[i, 0], e.Message);
            }

        }

        [TestMethod]
        public void TestUpdateCartBasedOnAvailability()
        { //DataTable BooksStock, ref Book[] Cart

            ManageBooksLib.ManageBooksLib bl = new ManageBooksLib.ManageBooksLib();
            bl.setStockData();//function is in the ManageBooksLib.cs file and creates the BooksStock Datatable with some predetermined data
            DataTable BooksStock = bl.dataSet.Tables["BooksStock"];

            object[,] values =
            {//{case, (bookISBN, Quantity)*3, expected Stock*3}
                {1, "0008376085", 14, "9780762416981", 7, "0441013597", 4, 14, 7, 3},//rotk, shoulders, dune
                {2, "3425048570", 5, "9780452284234", 9, "1111111111", 11, 5, 8, 0},//bnw, 1984, ---
                {3, "2225548996", 1, "9998547521556", 1, "9780747546245", -1, 0, 0, 0},//---, ---, gobl
                {4, "7974659852258", 3, "4679138525753", 2, "9875432160", 5, 0, 0, 0},//---, ---, ---
                {5, "iekanfhapqo23", 7, "geaofiwb34bik", 4, "09unf3we 0", 3, 0, 0, 0}//---, ---, ---
            };

            int i = 0;
            int j = 0;
            int y = 0;
            try
            {
                for (i = 0; i < values.GetLength(0); i++)
                {
                    ManageBooksLib.ManageBooksLib.Book[] cart = new ManageBooksLib.ManageBooksLib.Book[3];
                    j = y = 0;
                    foreach (ManageBooksLib.ManageBooksLib.Book k in cart)
                    {

                        cart[j].ISBN = (string)values[i, y + 1];
                        y++;
                        cart[j].Quantity = (int)values[i, y + 1];
                        y++;
                        j++;

                    }

                    bl.UpdateCartBasedOnAvailability(BooksStock, ref cart);
                    j = 7;
                    foreach (ManageBooksLib.ManageBooksLib.Book k in cart)
                    {
                        Assert.AreEqual(values[i, j], k.Quantity);
                        j++;
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("failed test case: {0}. Reason: {1}", values[i, 0], e.Message);
            }

        }


        


    }
}
