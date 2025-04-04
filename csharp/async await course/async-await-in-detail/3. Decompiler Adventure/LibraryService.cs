using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class LibraryService(HttpClient httpClient)
{
	public async Task<List<LibraryModel>> GetLibraries()
	{
		var response = await httpClient.GetAsync("some domain");
		response.EnsureSuccessStatusCode();
		var contentStream = await response.Content.ReadAsStreamAsync();
		var libraries = await JsonSerializer.DeserializeAsync<List<LibraryModel>>(contentStream);
		return libraries ?? throw new InvalidOperationException("Lib");
	}
}

class LibraryModel
{
	public required string something;
	public required string something2;
	public required string something3;
}