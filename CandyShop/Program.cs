//Существует продавец, он имеет у себя список товаров, 
//и при нужде, может вам его показать, также продавец 
//может продать вам товар. После продажи товар переходит
//к вам, и вы можете также посмотреть свои вещи.
//Возможные классы – игрок, продавец, товар. 

namespace CandyShop
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Merchant merchant = new Merchant();
			Player player = new Player();
			int amountGoods;

			List<Product> products = new List<Product>
			{
				new Product("Кислинка", 12, 3),
				new Product("Солёнка", 2, 12),
				new Product("Советская конфета такая рассыпается во рту типа батончика", 1, 59),
				new Product("Сластёнка", 50, 10)
			};

			merchant.SetGoods(products);

			while (Console.ReadKey().Key != ConsoleKey.Escape)
			{
				Console.WriteLine("Навигация:\n" +
					"Для выхода нажмите Esc\n" +
					"Для продолжения Enter\n" +
					"Для возврата к этому окну Sbacebar\n"
					);

				while (Console.ReadKey().Key != ConsoleKey.Spacebar)
				{
					Console.WriteLine($"Денег у покупателя - {player.Money}\n" +
						$"Денег у продавца - {merchant.Money}");
					merchant.ShowProducts();
					Console.WriteLine("Введите название товара, который " +
						"хотите купить");

					Product product = player.Choose(products);

					Console.WriteLine("Введите количество товара, который хотите купить\n");

					while (int.TryParse(Console.ReadLine(), out amountGoods) == false)
					{
						Console.WriteLine("Введите количество товара одним числом, " +
							"вы не можете купить товара больше чем есть у продавца\n");

						if (Console.ReadKey().Key != ConsoleKey.Spacebar)
						{
							break;
						}
					}

					if (product.Amount<amountGoods)
					{
						Console.WriteLine("Столько единиц этого товара нет");
						break;
					}

					if (player.TryGiveMoney(product, amountGoods))
					{
						player.BuyProduct(product, amountGoods);
						merchant.SellProduct(product, amountGoods);
					}
					else
					{
						break;
					}

					player.ShowProducts();
				}
			}
		}
	}
	abstract class Person //зачем игроку свой лист продуктов???? если его сюда можно пихнуть
	{
		protected int money;
		protected List<Product> products;

		public Person()
		{
			products = new List<Product>();
		}

		public List<Product> Products { get { return products; } private set { products = value; } }
		public int Money { get { return money; } private set { money = value; } }

		public void SetMoney(int money)
		{
			Money = money;
		}

		public abstract void ShowProducts();
	}

	class Player : Person
	{

		public Player()
		{
			money = 100;
		}

		public override void ShowProducts()
		{
			Console.WriteLine("Купленное:");

			foreach (var item in products)
			{
				Console.WriteLine($"{item.Name}, Количество: {item.Amount}");
			}
		}

		public void BuyProduct(Product product, int amountGoods)
		{
			AddProduct(product, amountGoods);
		}

		public Product Choose(List<Product> products)
		{
			Product product;

			while (TryGetProductByName(products, out product)==false)
			{
				Console.WriteLine("Такого товара нет, введите другое название");
			}

			return product;
		}

		public bool TryGiveMoney(Product product, int amountGoods)
		{
			if (Money < product.Price*amountGoods)
			{
				Console.WriteLine($"Не хватает - {product.Price*amountGoods - Money}," +
					$"выберете, что-то другое");

				return false;
			}
			else
			{
				int PlayerMoney = Money - product.Price*amountGoods;
				SetMoney(PlayerMoney);

				return true;
			}
		}

		private bool TryGetProductByName(List<Product> products, out Product product)
		{
			string itemName = Console.ReadLine();

			foreach (var iproduct in products)
			{
				if (iproduct.Name == itemName)
				{
					product = iproduct;
					return true;
				}
			}

			product = null;
			return false;
		}

		private void AddProduct(Product product, int amountGoods) //бляааа 
		{
			Product playerProduct = new Product(product.Name, amountGoods, product.Price);
			products.Add(playerProduct);
		}
	}

	class Merchant : Person
	{
		public Merchant()
		{
			money = 0;
		}

		public void SetGoods(List<Product> goods)
		{
			products = goods;
		}

		public void SellProduct(Product product, int amountGoods)
		{
			int productNumberInList = 0;
			bool isProductGone = false;

			for (int i = 0; i < Products.Count; i++)
			{
				Product iProduct = Products[i];
				if (iProduct.Name == product.Name)
				{
					iProduct.SetAmount(iProduct.Amount - amountGoods);
					productNumberInList = i;

					if (iProduct.Amount==0)
					{
						isProductGone = true;
					}
				}
			}

			if (isProductGone==true)
			{
				Products.RemoveAt(productNumberInList);
			}

			int profit = product.Price*amountGoods;
			SetMoney(profit+Money);
		}

		public override void ShowProducts()
		{
			Console.WriteLine("Товары на продажу:");

			foreach (var item in Products)
			{
				Console.WriteLine($"{item.Name}, Количество: {item.Amount}, Цена: {item.Price}");
			}
		}

	}

	public class Product
	{
		public Product(string name, int amount, int price)
		{
			Name = name;
			Amount = amount;
			Price=price;
		}

		public string Name { get; private set; }
		public int Amount { get; private set; }
		public int Price { get; private set; }

		public void SetAmount(int amount)
		{
			Amount = amount;
		}

	}
}