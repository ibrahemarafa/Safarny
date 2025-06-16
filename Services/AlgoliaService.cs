using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AlgoliaService
{
    private readonly SearchClient _client;
    private readonly IConfiguration _config;

    public AlgoliaService(IConfiguration config)
    {
        _config = config;
        _client = new SearchClient(config["Algolia:ApplicationId"], config["Algolia:ApiKey"]);
    }

    // دالة البحث في Algolia
    public async Task<IEnumerable<Dictionary<string, object>>> SearchAsync(string indexKey, string query)
    {
        // جلب اسم الفهرس من إعدادات الـ JSON
        var indexName = _config[$"Algolia:Indexes:{indexKey}"];
        if (string.IsNullOrEmpty(indexName)) return new List<Dictionary<string, object>>();

        // تنفيذ البحث في Algolia باستخدام الاستعلام
        var index = _client.InitIndex(indexName);
        var result = await index.SearchAsync<Dictionary<string, object>>(new Query(query)
        {
            HitsPerPage = 10
        });

        // إزالة خاصية _highlightResult من كل نتيجة
        foreach (var hit in result.Hits)
        {
            hit.Remove("_highlightResult");
        }

        return result.Hits; // إرجاع النتائج بعد إزالة _highlightResult
    }
}
