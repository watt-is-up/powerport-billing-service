.PHONY: ef-update-db

ef-update-db:
	@echo "Starting local PostgreSQL container..."
	docker compose up -d billing-db

	@echo "Running EF Core migrations..."
	@ConnectionStrings__BillingDb="Host=localhost;Port=5432;Database=billingdb;Username=billing;Password=secretpassword" \
	dotnet ef database update \
		--project src/BillingService/BillingService.csproj \
		--startup-project src/BillingService/BillingService.csproj

	@echo "Optional: stopping PostgreSQL container..."
	@docker compose down
