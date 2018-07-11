using Newtonsoft.Json.Linq;
using Restaurant;
using Xunit;

namespace Restaurants.Tests {
	public class OrderTests {
		[Fact]
		private void PreservesUnknownFields() {
			var sourceJson = @"
                {
                    ""OrderId"": ""00000000 - 0000 - 0000 - 0000 - 000000000000"",
                    ""Waiter"": ""Tom"",
                    ""Subtotal"": 3,
                    ""SpitInFood"": ""false"",
                    ""Items"": [
                    {
                        ""Price"": 3,
                        ""SecretField"": 678
                    }
                    ]
                }";

			var o = new Order(sourceJson);
			o.Items[0].Price = 4;

			var targetJson = @"
                {
                    ""OrderId"": ""00000000 - 0000 - 0000 - 0000 - 000000000000"",
                    ""Waiter"": ""Tom"",
                    ""Subtotal"": 3,
                    ""SpitInFood"": ""false"",
                    ""Items"": [
                    {
                        ""Price"": 4,
                        ""SecretField"": 678
                    }
                    ]
                }";

			Assert.Equal(
				JObject.Parse(targetJson).ToString(),
				JObject.Parse(o.ToJson()).ToString());
		}
	}
}
