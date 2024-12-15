
dotnet ef migrations add InitialCreate --project DataAccess --startup-project WebHost

::dotnet ef migrations add DeletePromocodes --project PromoCodeFactory.DataAccess --startup-project PromoCodeFactory.WebHost
pause 