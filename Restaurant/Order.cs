using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Restaurant {
	public class Order {
		private readonly JObject _jo;

		public Order() {
			Items = new List<Item>();
			_jo = new JObject();
		}

		public Order(string json) {
			Items = new List<Item>();
			_jo = JObject.Parse(json);
			var jarray = _jo["Items"] as JArray;
			foreach (var jitem in jarray)
				Items.Add(new Item(jitem as JObject));
		}

		public Guid OrderId {
			get => _jo.Value<Guid>("OrderId");
			set => _jo["OrderId"] = value;
		}

		public int TableNumber {
			get => _jo.Value<int>("TableId");
			set => _jo["TableId"] = value;
		}

		public string Waiter {
			get => _jo.Value<string>("Server");
			set => _jo["Server"] = value;
		}

		public string CookedBy {
			get => _jo.Value<string>("CookedBy");
			set => _jo["CookedBy"] = value;
		}

		public int CookingTime {
			get => _jo.Value<int>("CookingTime");
			set => _jo["CookingTime"] = value;
		}

		public int Subtotal {
			get => _jo.Value<int>("Subtotal");
			set => _jo["Subtotal"] = value;
		}

		public int Tax {
			get => _jo.Value<int>("Tax");
			set => _jo["Tax"] = value;
		}

		public int Total {
			get => _jo.Value<int>("Total");
			set => _jo["Total"] = value;
		}

		public bool Paid {
			get => _jo.Value<bool>("Paid");
			set => _jo["Paid"] = value;
		}

		public bool IsDodge {
			get => _jo.Value<bool>("IsDodge");
			set => _jo["IsDodge"] = value;
		}

		public List<Item> Items { get; }

		public string ToJson() {
			var jarray = new JArray();
			foreach (var item in Items)
				jarray.Add(item.JObject);

			_jo["Items"] = jarray;
			return $"{_jo}";
		}

		public class Item {
			public Item() {
				JObject = new JObject();
			}

			public Item(JObject jo) {
				JObject = jo;
			}

			public JObject JObject { get; }

			public string Name {
				get => JObject.Value<string>("Name");
				set => JObject["Name"] = value;
			}

			public int Quantity {
				get => JObject.Value<int>("Quantity");
				set => JObject["Quantity"] = value;
			}

			public int Price {
				get => JObject.Value<int>("Price");
				set => JObject["Price"] = value;
			}

			public int Total {
				get => JObject.Value<int>("Total");
				set => JObject["Total"] = value;
			}
		}
	}
}
