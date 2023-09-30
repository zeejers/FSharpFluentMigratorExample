open System
open FluentMigrator.Runner
open FluentMigrator.Runner.Initialization
open Microsoft.Extensions.DependencyInjection

open Migrations.AddTodosTable


let connectionString = System.Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
let MigrationAssemblies = [|typeof<AddTodosTable>.Assembly|]

let CreateServices () = 
    (new ServiceCollection())
        // Add common FluentMigrator services
        .AddFluentMigratorCore()
        .ConfigureRunner(fun rb ->
            // Add SQLite support to FluentMigrator
            rb.AddPostgres11_0()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(MigrationAssemblies).For.Migrations() |> ignore)
        // Enable logging to console in the FluentMigrator way
        .AddLogging(fun lb -> lb.AddFluentMigratorConsole() |> ignore)
        // Build the service provider
        .BuildServiceProvider(false)


let UpdateDatabase(serviceProvider: IServiceProvider) =
    // Instantiate the runner
    let runner = serviceProvider.GetRequiredService<IMigrationRunner>()

    // Execute the migrations
    runner.MigrateUp()

let ResetDatabase(serviceProvider: IServiceProvider) =
    let runner = serviceProvider.GetRequiredService<IMigrationRunner>()
    runner.MigrateDown(Migrations.AddTodosTable.VersionNumber)

[<EntryPoint>]
let main args  = 
    let action = 
        match args |> Array.toList with
        | [action] -> action
        |  _ -> "update"
    let serviceProvider = CreateServices()
    let scope = serviceProvider.CreateScope()
    match action.ToLower() with
    | "reset" -> ResetDatabase(serviceProvider) |> ignore
    | _ -> UpdateDatabase(scope.ServiceProvider) |> ignore
    0

    // Put the database update into a scope to ensure
    // that all resources will be disposed.
