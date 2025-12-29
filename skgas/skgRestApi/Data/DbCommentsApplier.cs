
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace skgRestApi.Data;

/// <summary>
/// Provides functionality to apply XML documentation comments as PostgreSQL table and column comments based on entity
/// and property summaries.
/// </summary>
/// <remarks>This class reads XML documentation generated at compile time and updates the PostgreSQL database
/// schema with corresponding comments for entities and their properties. It is intended for use with Entity Framework
/// Core DbContext implementations targeting PostgreSQL databases. All members are static and thread safety is ensured
/// by design.</remarks>
public static class DbCommentsApplier
{
    /// <summary>
    /// 어셈블리 XML 문서(컴파일 시 생성되는 .xml)를 읽어 엔티티와 프로퍼티에 대한 summary를
    /// PostgreSQL COMMENT ON TABLE / COMMENT ON COLUMN으로 적용합니다.
    /// </summary>
    public static async Task ApplyCommentsAsync(ApplicationDbContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        // 가능한 XML 문서 경로 후보 (DbContext가 속한 어셈블리 우선)
        var modelAssembly = Assembly.GetAssembly(typeof(ApplicationDbContext)) ?? Assembly.GetExecutingAssembly();
        var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{modelAssembly.GetName().Name}.xml");
        if (!File.Exists(xmlPath))
        {
            // 다른 후보로 현재 엔트리 어셈블리 시도
            var altPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
            if (File.Exists(altPath)) xmlPath = altPath;
            else return; // XML 문서가 없으면 종료
        }

        XDocument doc;
        try
        {
            doc = XDocument.Load(xmlPath);
        }
        catch
        {
            return;
        }

        var members = doc.Root?.Element("members")?.Elements("member");
        if (members == null) return;

        static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            return string.Join(" ", s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
        }

        string? GetSummary(string memberName) =>
            members.FirstOrDefault(x => (string?)x.Attribute("name") == memberName)
                   ?.Element("summary")?.Value is string v ? Normalize(v) : null;

        var model = context.Model;

        // 트랜잭션으로 묶음(안정성)
        await using var tx = await context.Database.BeginTransactionAsync();

        foreach (var entityType in model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null) continue;

            var typeMember = $"T:{clrType.FullName}";
            var tableComment = GetSummary(typeMember);
            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema();

            if (!string.IsNullOrEmpty(tableComment) && !string.IsNullOrEmpty(tableName))
            {
                var sql = BuildCommentOnTable(schema, tableName, tableComment);
                await context.Database.ExecuteSqlRawAsync(sql);
            }

            // 칼럼 코멘트
            foreach (var prop in entityType.GetProperties())
            {
                var propMember = $"P:{clrType.FullName}.{prop.Name}";
                var colComment = GetSummary(propMember);
                if (string.IsNullOrWhiteSpace(colComment)) continue;

                var storeIdentifier = StoreObjectIdentifier.Table(tableName, schema);
                var columnName = prop.GetColumnName(storeIdentifier) ?? prop.Name;
                if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(tableName)) continue;

                var sql = BuildCommentOnColumn(schema, tableName, columnName, colComment);
                await context.Database.ExecuteSqlRawAsync(sql);
            }
        }

        await tx.CommitAsync();
    }

    static string Escape(string s) => s.Replace("'", "''");

    static string BuildCommentOnTable(string? schema, string table, string comment)
    {
        var escaped = Escape(comment);
        if (string.IsNullOrEmpty(schema))
            return $"COMMENT ON TABLE \"{table}\" IS '{escaped}';";
        return $"COMMENT ON TABLE \"{schema}\".\"{table}\" IS '{escaped}';";
    }

    static string BuildCommentOnColumn(string? schema, string table, string column, string comment)
    {
        var escaped = Escape(comment);
        if (string.IsNullOrEmpty(schema))
            return $"COMMENT ON COLUMN \"{table}\".\"{column}\" IS '{escaped}';";
        return $"COMMENT ON COLUMN \"{schema}\".\"{table}\".\"{column}\" IS '{escaped}';";
    }
}