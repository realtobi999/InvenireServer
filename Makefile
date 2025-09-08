run:
	clear && cd ./src/InvenireServer.Presentation && dotnet run

watch:
	clear && cd ./src/InvenireServer.Presentation && dotnet watch

build:
	clear && cd ./tests/InvenireServer.Tests && dotnet build

test:
	clear && cd ./tests/InvenireServer.Tests && dotnet test

update_database:
	clear && dotnet ef database update --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj

drop_database:
	clear && dotnet ef database drop --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj

add_migration:
	clear && dotnet ef migrations add $(name) --output-dir ./Persistence/Migrations --project ./src/InvenireServer.Infrastructure/InvenireServer.Infrastructure.csproj --startup-project ./src/InvenireServer.Presentation/InvenireServer.Presentation.csproj