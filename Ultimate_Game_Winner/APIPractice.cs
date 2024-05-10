using System;
using System.Net.Http;

public class APIPractice
{
	public APIPractice()
	{
		using (var client = new HttpClient())
		{
			var endpoint = new Uri("http://boardgamegeek.com/xmlapi/game/3727?stats=1");
			var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
		}
		
	}
}
