run:
	cd ./src/InvenireServer.Presentation && dotnet run

update_database:
	dotnet ef database update --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj

drop_database:
	dotnet ef database drop --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj

add_migration:
	dotnet ef migrations add $(name) --output-dir ./Persistence/Migrations --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj