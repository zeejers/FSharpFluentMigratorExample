# F# FluentMigrator Migrations Example Using Postgres
I wanted a simple way to manage DB migrations in my F# projects, and found this to work closest to what I had in mind. I really like the experience of ecto, so it is unfortunate that we do not have something as simple as "mix ecto.gen.migration AddTodosTable name:string text:string", but this comes close.

The idea is that you include this migrations project as a sub-project in your solution (.sln) called "Migrations" and run it like this:
```bash
# Revert all migrations (Maybe improve to just drop all the tables?)
dotnet run --project=Migrations -- reset

# Run migrations
dotnet run --project=Migrations -- migrate
```

# Defining Columns

Defining columns is simple enough, can even make an FK pretty easily.
```F#
// AddTodosTable.fs
override this.Up() =
    this.Create.Table("Todos")
        .WithColumn("Id").AsInt64().PrimaryKey().Identity()
        .WithColumn("Text").AsString() |> ignore
    this.Create.Table("Notes")
        .WithColumn("Id").AsInt64().PrimaryKey().Identity()
        .WithColumn("Text").AsString()
        .WithColumn("TodoId").AsInt64().ForeignKey("Todos", "Id")
        |> ignore
```
# Defining a New Migration
- Create a new file in Migrations/ like "AddUsersTable.fs"
- Copy from existing sample "AddTodosTable.fs" migration sample, modify it to do what you want
- Add the assembly in the "Program.fs" like so
```F#
open System
open FluentMigrator.Runner
open FluentMigrator.Runner.Initialization
open Microsoft.Extensions.DependencyInjection

open Migrations.AddTodosTable
open Migrations.AddYourNewTable

// ...
// HERE
let MigrationAssemblies = [|typeof<AddTodosTable>.Assembly; typeof<AddYourNewTable>.Assembly|]

// ...

let ResetDatabase(serviceProvider: IServiceProvider) =
    let runner = serviceProvider.GetRequiredService<IMigrationRunner>()
    runner.MigrateDown(Migrations.AddTodosTable.VersionNumber)
    // HERE
    runner.MigrateDown(Migrations.AddYourNewTable.VersionNumber)


```

# Summary
Overall, still feels a little too manual and could use some improvement. One improvment could be centrally storing all versions in a Versions file maybe, or reading from the migrations dir with a naming convention to avoid having to make edits to Program.fs. In general, an even higher level db migration wrapper would be great.