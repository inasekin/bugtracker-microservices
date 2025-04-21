
dotnet ef migrations add InitialCreate --project IssueService.DAL --startup-project IssueService.Api

:: dotnet ef migrations add AddFiles --project IssueService.DAL --startup-project IssueService.Api

::dotnet ef migrations add DeletePromocodes --project PromoCodeFactory.DataAccess --startup-project PromoCodeFactory.WebHost
pause 