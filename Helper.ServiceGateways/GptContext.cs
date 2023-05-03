using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace Helper.ServiceGateways;

public class GptContext : DbContext
{
    private const string DatabaseFilename = "chatGPT.db";
    public static string ProgramName => "ChatGptHelper";
    public DbSet<Query> Queries { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<ApiInformation> ApiInformations { get; set; }
    public DbSet<ImageQuery> ImageQueries { get; set; }
    public DbSet<ImageResult> ImageResults { get; set; }
    
    public DbSet<EmbedResult> EmbedResults { get; set; }

    private string DbPath { get; }

    public GptContext()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChatGptHelper");
        var dbLocation = Path.Combine(backupFolder, DatabaseFilename);
        DbPath = dbLocation;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Query>()
            .HasOne<Response>(q => q.Response)
            .WithOne(r => r.Query)
            .HasForeignKey<Response>(r => r.ResponseId)
            ;
        
        mb.Entity<Response>().Navigation(r => r.Answers).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<Answer>().Navigation(a => a.Response).UsePropertyAccessMode(PropertyAccessMode.Property);

        mb.Entity<ImageQuery>().Navigation(iq => iq.ImageResults).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<ImageResult>().Navigation(ir => ir.ImageQuery).UsePropertyAccessMode(PropertyAccessMode.Property);

        mb.Entity<EmbedResult>().HasKey(er => er.EmbedThingId);

        //dotnet ef migrations add Initial
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
    }
}