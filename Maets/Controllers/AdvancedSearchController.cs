using Dapper;
using Maets.Domain.Entities;
using Maets.Models.ViewModels;
using Maets.Services.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Maets.Controllers;

public class AdvancedSearchController : Controller
{
    private readonly DbConnectionFactory _connectionFactory;
    
    public AdvancedSearchController(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Filter1(Guid? companyId)
    {
        await LoadCompaniesViewBag();
        
        if (companyId is null || !await CheckEntityRowExists(companyId.Value, "Companies"))
        {
            return View((object?)null);
        }

        const string sql = @"select * from Apps where PublisherId = @PublisherId";
        var parameters = new
        {
            PublisherId = companyId
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<App>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Title, $"/store/app/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }

    [HttpGet]
    public async Task<IActionResult> Filter2(Guid? companyId)
    {
        await LoadCompaniesViewBag();
        
        if (companyId is null || !await CheckEntityRowExists(companyId.Value, "Companies"))
        {
            return View((object?)null);
        }
        
        const string sql = @"select distinct Apps.* from Apps
                                inner join Apps_Developers AD on Apps.Id = AD.AppId and AD.CompanyId = @DeveloperId";
        var parameters = new
        {
            DeveloperId = companyId
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<App>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Title, $"/store/app/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    } 
    
    [HttpGet]
    public async Task<IActionResult> Filter3(int? labelsCount)
    {
        if (labelsCount is null) 
        {
            return View((object?)null);
        }
        
        const string sql = @"select distinct Apps.* from Apps
                                where (select count(*) from Apps_Labels where AppId = Apps.Id) >= @MinLabelsCount";
        var parameters = new
        {
            MinLabelsCount = labelsCount
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<App>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Title, $"/store/app/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    } 

    [HttpGet]
    public async Task<IActionResult> Filter4(DateTime? releasedBefore)
    {
        if (releasedBefore is null)
        {
            return View((object?)null);
        }
        
        const string sql = @"select Companies.* from Companies where exists(select * from Apps where Apps.ReleaseDate < @ReleasedBefore)";
        var parameters = new
        {
            ReleasedBefore = releasedBefore
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<Company>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Name, $"/companies/details/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }
    
    [HttpGet]
    public async Task<IActionResult> Filter5(string? gamesCount)
    {
        if (gamesCount is null)
        {
            return View((object?)null);
        }
        
        const string sql = @"select Company.* from Companies Company
            where
                (select count(*) from Apps where PublisherId = Company.Id) +
                (select count(distinct AppId) from Apps_Developers where CompanyId = Company.Id) >= @MinGamesCount";
        var parameters = new
        {
            MinGamesCount = gamesCount
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<Company>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Name, $"/companies/details/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }
    
    [HttpGet]
    public async Task<IActionResult> Filter6(Guid? appId)
    {
        await LoadAppsViewBag();
        
        if (appId is null || !await CheckEntityRowExists(appId.Value, "Apps"))
        {
            return View((object?)null);
        }
        
        const string sql = @"
            with AppLabels as (
                select * from Apps_Labels where AppId = @AppId
            )
            select * from Apps where not exists(
                select * from AppLabels where not exists(
                    select * from Apps_Labels where Apps_Labels.LabelId = AppLabels.LabelId and Apps_Labels.AppId = Apps.Id
                )
            )";
        var parameters = new
        {
            AppId = appId
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<App>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Title, $"/store/app/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }
    
    [HttpGet]
    public async Task<IActionResult> Filter7(int? reviewsCount, Guid? appId)
    {
        await LoadAppsViewBag();
        
        if (reviewsCount is null || appId is null || !await CheckEntityRowExists(appId.Value, "Apps"))
        {
            return View((object?)null);
        }
        
        const string sql = @"
            with GoodReviews as (
                select Reviews.* from Reviews
                where Reviews.Score > (
                    select AVG(AppReview.Score) from Reviews AppReview
                    where AppReview.AppId = @AppId
                )
            )
            select * from Apps
            where (select count(*) from GoodReviews where AppId = Apps.Id) >= @ReviewsCount";
        var parameters = new
        {
            ReviewsCount = reviewsCount,
            AppId = appId
        };
        
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<App>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Title, $"/store/app/{x.Id}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }
    
    [HttpGet]
    public async Task<IActionResult> Filter8(int? averageScore)
    {
        if (averageScore is null)
        {
            return View((object?)null);
        }
        
        const string sql = @"
            with AggregatedLabels as (
                select Labels.Id, AVG(Reviews.Score) as AverageScore from Labels
                    inner join Apps_Labels on Labels.Id = Apps_Labels.LabelId
                    inner join Apps on Apps.Id = Apps_Labels.AppId
                    inner join Reviews on Apps.Id = Reviews.AppId
                group by Labels.Id
            )
            select * from Labels
            where Id in (select Id from AggregatedLabels where AverageScore > @AverageScore)";
        var parameters = new
        {
            AverageScore = averageScore
        };
        using var connection = await _connectionFactory.GetDataConnection();
        var apps = await connection.QueryAsync<Label>(sql, parameters);

        var appResults = apps.Select(x => new SearchResultDto(x.Name, $"/Store/Search?labels={x.Name}"));
        
        return View(new AdvancedSearchViewModel(sql, appResults));
    }
    
    private async Task<bool> CheckEntityRowExists(Guid id, string tableName)
    {
        var query = $"select count(1) from [{tableName}] where Id = @id";
        var parameters = new { id };
        using var connection = await _connectionFactory.GetDataConnection();
        return connection.ExecuteScalar<bool>(query, parameters);
    }

    private async Task LoadCompaniesViewBag()
    {
        var query = "select Id, Name from Companies";
        using var connection = await _connectionFactory.GetDataConnection();
        var entities = await connection.QueryAsync(query);
        ViewBag.Companies = new SelectList(entities, "Id", "Name");
    }
    
    private async Task LoadAppsViewBag()
    {
        var query = "select Id, Title from Apps";
        using var connection = await _connectionFactory.GetDataConnection();
        var entities = await connection.QueryAsync(query);
        ViewBag.Apps = new SelectList(entities, "Id", "Title");
    }
}
