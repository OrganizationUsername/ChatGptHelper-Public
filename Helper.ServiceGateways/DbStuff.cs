using Helper.Core;
using Helper.Core.jsonModels;
using Helper.Core.Utils;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways;

public class DbStuff
{
    public readonly GptContext GptContext;
    private static string ProgramName => "ChatGptHelper";

    public DbStuff(GptContext gptContext)
    {
        GptContext = gptContext;
        CreateDatabaseIfNeeded();
    }

    public bool HasApiKey() => GptContext.ApiInformations.Any();

    public void SetApiKey(string newApiKey)
    {
        GptContext.ApiInformations.Add(new() { ApiKey = newApiKey });
        GptContext.SaveChanges();
    }

    public string GetApiKey() => GptContext.ApiInformations.First().ApiKey;

    public int GetEmbedCount() => GptContext.EmbedResults.Count();

    public List<EmbedPiece> GetBestEmbeds(float[] vector, int desiredCount = 100)
    {
        var totalEmbedCount = GetEmbedCount();
        var batchSize = 1000;
        var currentThing = 0;

        var top100 = new List<EmbedPiece>(desiredCount * 2);

        while (currentThing < totalEmbedCount)
        {
            var tempThing = GptContext.EmbedResults
                .OrderByDescending(er => er.EmbedThingId)
                .Take(batchSize)
                .ToList()
                .Select(er => new EmbedPiece
                {
                    EmbedThingId = er.EmbedThingId,
                    Text = er.Text,
                    Vector = EmbedPiece.DeserializeVector(er.Vector),
                }).OrderByDescending(ep => MathHelper.DotProduct(ep.Vector, vector))
                .Take(desiredCount);
            top100.AddRange(tempThing);
            top100 = top100
                .OrderByDescending(ep => MathHelper.DotProduct(ep.Vector, vector))
                .Take(desiredCount)
                .ToList();
            currentThing += batchSize;
        }

        return top100;
    }

    public async Task<List<EmbedPiece>> GetSomeEmbeds(int takeLimit = 100) =>
        await GptContext.EmbedResults
            .OrderByDescending(er => er.EmbedThingId)
            .Take(takeLimit)
            .OrderBy(er => er.EmbedThingId)
            .Select(er => new EmbedPiece
            {
                EmbedThingId = er.EmbedThingId,
                Text = er.Text,
                Vector = EmbedPiece.DeserializeVector(er.Vector),
            })
            .ToListAsync();

    public async Task<List<Query>> GetQueries(int takeLimit = 100) =>
        await GptContext.Queries
            .OrderByDescending(q => q.QueryId)
            .Take(takeLimit)
            .Include(q => q.Response.Answers)
            .OrderBy(q => q.QueryId)
            .ToListAsync();

    public async Task<List<ImageQuery>> GetImageQueries(int takeLimit = 100) =>
        await GptContext.ImageQueries
            .OrderByDescending(iq => iq.ImageQueryId)
            .Include(iq => iq.ImageResults)
            .OrderBy(iq => iq.ImageQueryId)
            .ToListAsync();

    private static void CreateDatabaseIfNeeded()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
    }

    public async Task StoreEmbed(EmbedResult e)
    {
        GptContext.EmbedResults.Add(e);
        await GptContext.SaveChangesAsync();
    }

    public async Task<ImageQuery> StoreStuff(ImageQuery iq)
    {
        GptContext.ImageQueries.Add(iq);
        await GptContext.SaveChangesAsync();
        return iq;
    }

    public async Task<Query> StoreQuery(string text, int tokenCount, QueryResponse qr, string modelUsed)
    {
        var query = new Query() { Text = text.Trim(), TokenCount = tokenCount };

        var r = new Response()
        {
            Query = query,
            TokenCount = qr.usage.completion_tokens, ModelUsed = modelUsed
        };
        foreach (var a in qr.choices) { r.Answers.Add(new() { Text = a.text.Trim() }); }

        query.Response = r;

        var q = GptContext.Queries.Add(query);
        await GptContext.SaveChangesAsync();
        return q.Entity;
    }
}