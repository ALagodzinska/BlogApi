HOW TO DO MIGRATIONS

 - If you dont have migrations in your project - 
	-	run * Install-Package Microsoft.EntityFrameworkCore.Tools * in Package Manager Console
		- that is for using migrations commands in Package Manager Console
	- then run * Add-Migration MigrationName *
	- after run * Update-Database *
